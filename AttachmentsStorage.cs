using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace ntra_missions
{
    /// <summary>
    /// Resolves the shared attachments folder.
    ///
    /// This was previously a single hardcoded UNC path
    /// (\\GIZA-ASAMEH\Giza Software\Attachments\) which really only resolved on the
    /// machine hosting the share. Everyone else silently saw no attachments, because
    /// the callers treated "folder is missing" and "share refused access" exactly the
    /// same as "nothing attached".
    ///
    /// Candidates are probed in order and the first reachable one wins, so a
    /// workstation that reaches the share through a mapped drive (Z:) and one that
    /// reaches it by UNC both work from the same build. Override per site with the
    /// AttachmentsPath / AttachmentsPathAlternates keys in App.config.
    /// </summary>
    public static class AttachmentsStorage
    {
        public const string COMPLAINTS_FOLDER = "Complaints";
        public const string MISSIONS_FOLDER = "Missions";

        private static readonly object syncRoot = new object();

        private static string resolvedRoot;
        private static bool resolved;
        private static string lastError;

        /// <summary>
        /// Root attachments folder (with a trailing slash), or null when none of the
        /// candidates can be reached. Probed once and then cached.
        /// </summary>
        public static string Root
        {
            get
            {
                lock (syncRoot)
                {
                    if (!resolved)
                        Resolve();

                    return resolvedRoot;
                }
            }
        }

        /// <summary>Why the last resolve or probe failed; null when everything is fine.</summary>
        public static string LastError
        {
            get
            {
                lock (syncRoot)
                {
                    return lastError;
                }
            }
        }

        /// <summary>
        /// Drops the cached root so the next access probes again. Worth calling after
        /// the user reconnects a drive, otherwise one early failure sticks for the
        /// whole session.
        /// </summary>
        public static void Reset()
        {
            lock (syncRoot)
            {
                resolved = false;
                resolvedRoot = null;
                lastError = null;
            }
        }

        private static void Resolve()
        {
            resolvedRoot = null;
            lastError = null;

            List<string> candidates = GetCandidateRoots();
            List<string> tried = new List<string>();

            for (int i = 0; i < candidates.Count; i++)
            {
                tried.Add(candidates[i]);

                try
                {
                    if (Directory.Exists(candidates[i]))
                    {
                        resolvedRoot = candidates[i];

                        // only a success is cached
                        resolved = true;
                        return;
                    }
                }
                catch (Exception exception)
                {
                    // keep probing the remaining candidates
                    lastError = candidates[i] + " -> " + exception.Message;
                }
            }

            // Deliberately leaving "resolved" false: a share that was momentarily
            // unreachable would otherwise stay broken for the rest of the session.
            lastError = "None of these attachment locations could be reached:" +
                Environment.NewLine + "    " +
                string.Join(Environment.NewLine + "    ", tried) +
                Environment.NewLine + Environment.NewLine +
                "Set the correct path in App.config (AttachmentsPath).";
        }

        private static List<string> GetCandidateRoots()
        {
            List<string> candidates = new List<string>();

            AddCandidate(candidates, ConfigurationManager.AppSettings["AttachmentsPath"]);

            string alternates = ConfigurationManager.AppSettings["AttachmentsPathAlternates"];

            if (!string.IsNullOrWhiteSpace(alternates))
            {
                string[] parts = alternates.Split(';');

                for (int i = 0; i < parts.Length; i++)
                    AddCandidate(candidates, parts[i]);
            }

            // Shipped fallbacks so an un-edited App.config still finds the share.
            //
            // The IP form comes first because the host name resolves to a link-local
            // IPv6 address as well as its IPv4 one; clients off that subnet follow the
            // IPv6 answer, fail to connect, and Directory.Exists() reports a plain
            // "false" that is indistinguishable from an empty folder.
            //
            // A drive letter is last: it is a per-user mapping and is not guaranteed to
            // exist, or to point at the same share, on another workstation.
            AddCandidate(candidates, @"\\10.165.202.23\Attachments");
            AddCandidate(candidates, @"\\GIZA-ASAMEH.TRA.GOV.EG\Attachments");
            AddCandidate(candidates, @"\\GIZA-ASAMEH\Attachments");
            AddCandidate(candidates, @"\\10.165.202.23\Giza Software\Attachments");
            AddCandidate(candidates, @"\\GIZA-ASAMEH\Giza Software\Attachments");
            AddCandidate(candidates, @"Z:\Attachments");

            return candidates;
        }

        private static void AddCandidate(List<string> candidates, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            string normalized = Environment.ExpandEnvironmentVariables(path.Trim());

            normalized = normalized.TrimEnd('\\', '/') + @"\";

            if (!candidates.Contains(normalized, StringComparer.OrdinalIgnoreCase))
                candidates.Add(normalized);
        }

        public static string GetComplaintFolder(string mComplaintId)
        {
            return CombineFolder(COMPLAINTS_FOLDER, mComplaintId);
        }

        public static string GetMissionFolder(string mMissionId)
        {
            return CombineFolder(MISSIONS_FOLDER, mMissionId);
        }

        private static string CombineFolder(string mCategory, string mId)
        {
            string root = Root;

            if (root == null)
                return null;

            return root + mCategory + @"\" + (mId ?? "") + @"\";
        }

        /// <summary>
        /// Files in an attachment folder. Returns an empty array when the folder
        /// simply does not exist yet (nothing attached), but lets access errors
        /// through so the caller can tell "empty" apart from "cannot read".
        /// </summary>
        public static FileInfo[] ListFiles(string mFolder)
        {
            if (string.IsNullOrEmpty(mFolder))
                return new FileInfo[0];

            DirectoryInfo directory = new DirectoryInfo(mFolder);

            if (!directory.Exists)
                return new FileInfo[0];

            return directory.GetFiles();
        }

        /// <summary>
        /// Lists every attachment folder of a category (Missions / Complaints) in a
        /// single round trip, keyed by folder name.
        ///
        /// This exists because Directory.Exists() answers "false" both when a folder
        /// is missing AND when the share refuses the connection, so probing each
        /// mission folder individually made a permission/credential problem look
        /// exactly like "nothing is attached" - no files and no error. Enumerating the
        /// parent once lets a real access failure throw where the caller can report
        /// it, and replaces N network calls with one.
        /// </summary>
        public static Dictionary<string, DirectoryInfo> ListCategoryFolders(string mCategory)
        {
            Dictionary<string, DirectoryInfo> folders =
                new Dictionary<string, DirectoryInfo>(StringComparer.OrdinalIgnoreCase);

            string root = Root;

            if (root == null)
                throw new IOException(LastError ?? "The attachments share is not reachable.");

            DirectoryInfo categoryDirectory = new DirectoryInfo(root + mCategory);

            if (!categoryDirectory.Exists)
            {
                // Could be "not created yet" or "cannot be read". Enumerating the root
                // throws on the second case, so the caller stops guessing.
                new DirectoryInfo(root).GetDirectories();

                return folders;
            }

            DirectoryInfo[] children = categoryDirectory.GetDirectories();

            for (int i = 0; i < children.Length; i++)
                folders[children[i].Name] = children[i];

            return folders;
        }

        /// <summary>
        /// Which account and which path were actually used - the two things needed to
        /// explain why one workstation sees attachments and another does not.
        /// </summary>
        public static string DescribeContext()
        {
            string account;

            try
            {
                account = WindowsIdentity.GetCurrent().Name;
            }
            catch
            {
                account = "(unknown)";
            }

            return "Windows account : " + account +
                Environment.NewLine +
                "Attachments path: " + (Root ?? "(none of the candidates could be reached)");
        }

        /// <summary>
        /// Makes sure an attachment folder exists, throwing a message worth showing
        /// to the user when the share itself is the problem.
        /// </summary>
        public static void EnsureFolder(string mFolder)
        {
            if (string.IsNullOrEmpty(mFolder))
                throw new IOException(LastError ?? "The attachments share is not reachable.");

            if (!Directory.Exists(mFolder))
                Directory.CreateDirectory(mFolder);
        }
    }
}
