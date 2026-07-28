using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace ntra_missions
{
    /// <summary>
    /// Converts a shared Google Maps link to an Excel sheet of points
    /// (name, latitude, longitude, description).
    ///
    /// Supports the link types missions produce:
    ///  - a saved-places LIST (the lists where repeaters are pinned) -> the
    ///    list token is read from the resolved link and the places are fetched
    ///    from Google's list API (the places are NOT in the page HTML).
    ///  - a single shared place  -> coordinates read straight from the URL.
    ///  - a MY MAPS map (mid=...) -> downloaded as KML.
    /// </summary>
    public class GoogleMapsToExcel
    {
        public class MapPoint
        {
            public string Name;
            public string Latitude;
            public string Longitude;
            public string Description;
        }

        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";

        private readonly JavaScriptSerializer serializer;

        public GoogleMapsToExcel()
        {
            serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
        }

        // ---- generic helpers ----

        private static void EnsureTls()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        // Follows redirects (short goo.gl links) and returns the final URL + body.
        private static void ResolvePage(string url, out string finalUrl, out string html)
        {
            EnsureTls();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = true;
            request.UserAgent = UserAgent;
            request.Accept = "text/html,application/xhtml+xml,*/*";
            request.Headers[HttpRequestHeader.Cookie] = "CONSENT=YES+"; // skip EU consent page

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                finalUrl = response.ResponseUri.ToString();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    html = reader.ReadToEnd();
            }
        }

        private static string DownloadString(string url)
        {
            EnsureTls();

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers[HttpRequestHeader.UserAgent] = UserAgent;
                client.Headers[HttpRequestHeader.Cookie] = "CONSENT=YES+";
                return client.DownloadString(url);
            }
        }

        private static string CoordinateText(double value)
        {
            return value.ToString("0.########", CultureInfo.InvariantCulture);
        }

        private static bool TryGetDouble(object value, out double result)
        {
            result = 0;

            if (value is double) { result = (double)value; return true; }
            if (value is decimal) { result = (double)(decimal)value; return true; }
            if (value is int) { result = (int)value; return true; }
            if (value is long) { result = (long)value; return true; }

            return false;
        }

        private static string FirstString(object value)
        {
            string s = value as string;
            if (s != null)
                return s;

            object[] arr = value as object[];
            if (arr != null)
            {
                foreach (object item in arr)
                {
                    string inner = FirstString(item);
                    if (!string.IsNullOrEmpty(inner))
                        return inner;
                }
            }

            return null;
        }

        // ---- branch A: saved-places list via the list API ----

        // The list token appears in the resolved URL / page as "11m1!2s<token>"
        // or ".../placelists/list/<token>".
        private static string ExtractListToken(string url, string html)
        {
            foreach (string source in new[] { url, html })
            {
                if (string.IsNullOrEmpty(source))
                    continue;

                Match m = Regex.Match(source, @"placelists/list/([A-Za-z0-9_\-]+)");
                if (m.Success)
                    return m.Groups[1].Value;

                m = Regex.Match(source, @"11m\d!2s([A-Za-z0-9_\-]{15,})");
                if (m.Success)
                    return m.Groups[1].Value;

                m = Regex.Match(source, @"!2s([A-Za-z0-9_\-]{25,})");
                if (m.Success)
                    return m.Groups[1].Value;
            }

            return null;
        }

        private List<MapPoint> FetchListPlaces(string token)
        {
            // pb: 1s<token>, 2e2 = shared list, 3e2 = full detail, 4i500 = page size
            string url = "https://www.google.com/maps/preview/entitylist/getlist" +
                         "?authuser=0&hl=en&gl=us&pb=!1m1!1s" + token + "!2e2!3e2!4i500";

            string response = DownloadString(url);

            int start = response.IndexOf('[');
            if (start < 0)
                return null;

            object data = serializer.DeserializeObject(response.Substring(start));

            List<MapPoint> points = new List<MapPoint>();

            // confirmed layout: root[0][8] = places;
            // place[2] = name, place[3] = note, place[1][5] = [.,.,lat,lng]
            object[] root = data as object[];
            if (root != null && root.Length > 0)
            {
                object[] level0 = root[0] as object[];
                if (level0 != null && level0.Length > 8)
                {
                    object[] places = level0[8] as object[];
                    if (places != null)
                    {
                        foreach (object placeObj in places)
                        {
                            MapPoint point = ParseListPlace(placeObj as object[]);
                            if (point != null)
                                points.Add(point);
                        }
                    }
                }
            }

            // fallback: hunt anywhere in case Google shifts the layout
            if (points.Count == 0)
                ScanForPlaces(data, points);

            return points;
        }

        private static MapPoint ParseListPlace(object[] place)
        {
            if (place == null || place.Length < 2)
                return null;

            double lat, lon;
            if (!FindCoordinate(place[1], out lat, out lon))
                return null;

            string name = place.Length > 2 ? FirstString(place[2]) : null;
            string note = place.Length > 3 ? FirstString(place[3]) : null;

            return new MapPoint
            {
                Name = name ?? "",
                Latitude = CoordinateText(lat),
                Longitude = CoordinateText(lon),
                Description = note ?? ""
            };
        }

        // A coordinate array looks like [null, null, lat, lng].
        private static bool IsCoordinate(object[] arr, out double lat, out double lon)
        {
            lat = 0;
            lon = 0;

            if (arr == null || arr.Length < 4)
                return false;

            if (!TryGetDouble(arr[2], out lat) || !TryGetDouble(arr[3], out lon))
                return false;

            if (lat == 0 && lon == 0)
                return false;

            return Math.Abs(lat) <= 90.0 && Math.Abs(lon) <= 180.0 && (lat != Math.Floor(lat) || lon != Math.Floor(lon));
        }

        private static bool FindCoordinate(object node, out double lat, out double lon)
        {
            lat = 0;
            lon = 0;

            object[] arr = node as object[];
            if (arr == null)
                return false;

            if (IsCoordinate(arr, out lat, out lon))
                return true;

            foreach (object child in arr)
            {
                if (FindCoordinate(child, out lat, out lon))
                    return true;
            }

            return false;
        }

        private static void ScanForPlaces(object node, List<MapPoint> results)
        {
            object[] arr = node as object[];
            if (arr == null)
                return;

            MapPoint point = ParseListPlace(arr);
            if (point != null)
            {
                results.Add(point);
                return;
            }

            foreach (object child in arr)
                ScanForPlaces(child, results);
        }

        // ---- branch B: single shared place (maps/search/<lat>,<lng>) ----

        private static MapPoint ParseSinglePlace(string url, string html)
        {
            Match m = Regex.Match(url, @"/maps/search/(-?\d+\.\d+),\+?(-?\d+\.\d+)");
            if (!m.Success)
                m = Regex.Match(url, @"@(-?\d+\.\d+),(-?\d+\.\d+)");

            if (!m.Success)
                return null;

            string name = "";
            Match t = Regex.Match(html ?? "", @"<meta content=""([^""]+)"" property=""og:title""");
            if (t.Success)
                name = t.Groups[1].Value;

            return new MapPoint
            {
                Name = name,
                Latitude = m.Groups[1].Value,
                Longitude = m.Groups[2].Value,
                Description = ""
            };
        }

        // ---- branch C: My Maps (mid=...) via KML ----

        private static string ExtractMid(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            Match match = Regex.Match(url, @"[?&]mid=([^&#]+)");
            return match.Success ? match.Groups[1].Value : null;
        }

        private static string DownloadKml(string mapId)
        {
            EnsureTls();

            string url = "https://www.google.com/maps/d/kml?mid=" + mapId + "&forcekml=1";

            byte[] data;
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.UserAgent] = UserAgent;
                data = client.DownloadData(url);
            }

            if (data.Length > 2 && data[0] == 0x50 && data[1] == 0x4B) // KMZ zip
            {
                using (MemoryStream ms = new MemoryStream(data))
                using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        if (entry.Name.ToLowerInvariant().EndsWith(".kml"))
                        {
                            using (StreamReader reader = new StreamReader(entry.Open(), Encoding.UTF8))
                                return reader.ReadToEnd();
                        }
                    }
                }
                return null;
            }

            return Encoding.UTF8.GetString(data);
        }

        private static string CleanHtml(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return "";

            string text = Regex.Replace(raw, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<[^>]+>", "");
            text = text.Replace("&nbsp;", " ").Replace("&amp;", "&")
                       .Replace("&lt;", "<").Replace("&gt;", ">")
                       .Replace("&quot;", "\"").Replace("&apos;", "'");
            return text.Trim();
        }

        private static List<MapPoint> ParseKmlPoints(string kml)
        {
            List<MapPoint> points = new List<MapPoint>();

            XDocument document = XDocument.Parse(kml);

            foreach (XElement placemark in document.Descendants())
            {
                if (placemark.Name.LocalName != "Placemark")
                    continue;

                XElement point = null;
                foreach (XElement child in placemark.Descendants())
                {
                    if (child.Name.LocalName == "Point") { point = child; break; }
                }
                if (point == null)
                    continue;

                string coordinates = "";
                foreach (XElement child in point.Elements())
                {
                    if (child.Name.LocalName == "coordinates") { coordinates = child.Value.Trim(); break; }
                }

                string[] parts = coordinates.Split(',');
                if (parts.Length < 2)
                    continue;

                string name = "";
                string description = "";
                foreach (XElement child in placemark.Elements())
                {
                    if (child.Name.LocalName == "name")
                        name = child.Value.Trim();
                    else if (child.Name.LocalName == "description")
                        description = CleanHtml(child.Value);
                }

                points.Add(new MapPoint
                {
                    Name = name,
                    Longitude = parts[0].Trim(),
                    Latitude = parts[1].Trim(),
                    Description = description
                });
            }

            return points;
        }

        // ---- Excel output (same interop pattern as ExcelExport) ----

        private static void WriteExcel(List<MapPoint> points)
        {
            var excelApp = new Excel.Application();
            excelApp.Visible = false;

            Excel.Workbooks excelWorkBooks = excelApp.Workbooks;
            excelApp.Workbooks.Add();
            Excel.Workbook excelWorkBook = excelWorkBooks[1];
            Excel.Worksheet excelWorkSheet = excelWorkBook.Worksheets[1];

            excelWorkSheet.Cells[1, 1] = "Name";
            excelWorkSheet.Cells[1, 2] = "Latitude";
            excelWorkSheet.Cells[1, 3] = "Longitude";
            excelWorkSheet.Cells[1, 4] = "Description";

            int rowNumber = 2;

            for (int i = 0; i < points.Count; i++)
            {
                excelWorkSheet.Cells[rowNumber, 1] = points[i].Name;
                excelWorkSheet.Cells[rowNumber, 2] = points[i].Latitude;
                excelWorkSheet.Cells[rowNumber, 3] = points[i].Longitude;
                excelWorkSheet.Cells[rowNumber, 4] = points[i].Description;

                rowNumber++;
            }

            excelWorkSheet.Columns.AutoFit();
            excelApp.Visible = true;
        }

        /// <summary>
        /// Full pipeline: share link -> points -> Excel sheet.
        /// Shows its own message boxes; returns false when anything fails.
        /// </summary>
        public bool ExportFromLink(string shareLink)
        {
            if (string.IsNullOrWhiteSpace(shareLink))
            {
                MessageBox.Show("Paste the share link of your Google Maps list first.",
                    "No link", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            shareLink = shareLink.Trim();
            if (!shareLink.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                shareLink = "https://" + shareLink;

            string finalUrl, html;
            try
            {
                ResolvePage(shareLink, out finalUrl, out html);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open the link. Check your internet connection.\n\n" + ex.Message,
                    "Download failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            List<MapPoint> points = null;

            try
            {
                // My Maps
                string mid = ExtractMid(shareLink);
                if (mid == null)
                    mid = ExtractMid(finalUrl);

                if (mid != null)
                {
                    string kml = DownloadKml(mid);
                    if (!string.IsNullOrEmpty(kml))
                        points = ParseKmlPoints(kml);
                }
                else
                {
                    // saved-places list
                    string token = ExtractListToken(finalUrl, html);
                    if (token != null)
                        points = FetchListPlaces(token);

                    // single shared place
                    if (points == null || points.Count == 0)
                    {
                        MapPoint single = ParseSinglePlace(finalUrl, html);
                        if (single != null)
                            points = new List<MapPoint> { single };
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to read the map data:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (points == null || points.Count == 0)
            {
                MessageBox.Show("Could not find any saved places in this link.\n\n" +
                    "Open your list in Google Maps, tap Share, and use that link. The list must be shared publicly.",
                    "Nothing to export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                WriteExcel(points);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create the Excel sheet:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
