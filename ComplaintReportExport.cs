using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;

namespace ntra_missions
{
    /// <summary>
    /// Generates the official interference-mission Word report for a complaint,
    /// following the "تقرير مأمورية" template: NTRA header, complaint subject,
    /// the affected sites/sectors table, then a dated entry for every mission
    /// conducted on the complaint (engineers + the mission comment as text).
    /// The data is read straight from the database.
    /// </summary>
    public class ComplaintReportExport
    {
        private readonly CommonQueries commonQueries;

        private const string ArabicFont = "Arial";

        public ComplaintReportExport()
        {
            commonQueries = new CommonQueries();
        }

        private static string ArabicDayName(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday: return "الأحد";
                case DayOfWeek.Monday: return "الإثنين";
                case DayOfWeek.Tuesday: return "الثلاثاء";
                case DayOfWeek.Wednesday: return "الأربعاء";
                case DayOfWeek.Thursday: return "الخميس";
                case DayOfWeek.Friday: return "الجمعة";
                default: return "السبت";
            }
        }

        private static string FormatDate(DateTime date)
        {
            return date.Day + "/" + date.Month + "/" + date.Year;
        }

        // appends one paragraph at the end of the document
        private static void AddParagraph(Word.Document doc, string text, int size, bool bold,
            Word.WdParagraphAlignment alignment, bool rightToLeft)
        {
            Word.Range range = doc.Content;
            range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            range.Text = text + "\r";

            range.Font.Name = ArabicFont;
            range.Font.NameBi = ArabicFont;
            range.Font.Size = size;
            range.Font.SizeBi = size;
            range.Font.Bold = bold ? 1 : 0;
            range.Font.BoldBi = bold ? 1 : 0;

            range.ParagraphFormat.ReadingOrder = rightToLeft
                ? Word.WdReadingOrder.wdReadingOrderRtl
                : Word.WdReadingOrder.wdReadingOrderLtr;
            range.ParagraphFormat.Alignment = alignment;
        }

        private static void AddSpacer(Word.Document doc)
        {
            AddParagraph(doc, "", 8, false, Word.WdParagraphAlignment.wdAlignParagraphCenter, true);
        }

        private static string JoinEngineers(List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> engineers)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < engineers.Count; i++)
                names.Add("مهندس " + engineers[i].value);

            return string.Join(" و ", names);
        }

        public void ExportComplaintReport(Complaints complaint)
        {
            List<BASIC_STRUCTS.SITE_STRUCT> sites = complaint.GetComplaintsSites();

            if (sites == null || sites.Count == 0)
            {
                MessageBox.Show("This complaint has no site data to build a report.",
                    "Nothing to export", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // all missions on this complaint, oldest first
            List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> allMissions =
                new List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT>();
            commonQueries.GetMissions(ref allMissions);

            List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> missions = allMissions
                .Where(m => m.company_serial == complaint.GetCompanySerial()
                         && m.complaint_serial == complaint.GetSerial())
                .OrderBy(m => m.mission_Date)
                .ToList();

            string company = complaint.GetCompanyName();
            string city = sites[0].city;
            string region = sites[0].region;

            Word.Application app = null;
            Word.Document doc = null;

            try
            {
                app = new Word.Application();
                app.Visible = false;
                doc = app.Documents.Add();

                Word.WdParagraphAlignment center = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                Word.WdParagraphAlignment right = Word.WdParagraphAlignment.wdAlignParagraphRight;

                // ---- NTRA header ----
                AddParagraph(doc, "الجهاز القومي لتنظيم الاتصالات", 16, true, center, true);
                AddParagraph(doc, "نيابة البنية التحتية والطيف الترددي", 14, true, center, true);
                AddParagraph(doc, "الإدارة التنفيذية لمراقبة الترددات", 14, true, center, true);
                AddSpacer(doc);

                // ---- title ----
                AddParagraph(doc, "تقرير", 20, true, center, true);
                AddParagraph(doc,
                    "بشأن : الشكوى الواردة من شركة " + company + " في محافظة " + city + " بمنطقة " + region,
                    16, true, center, true);
                AddSpacer(doc);

                // ---- subject ----
                AddParagraph(doc, "أولاً: الموضوع:", 14, true, right, true);
                AddParagraph(doc,
                    "بخصوص الشكوى الواردة من شركة " + company + " بشأن وجود تداخلات على " +
                    sites.Count + " محطات في محيط منطقة " + region,
                    14, false, right, true);
                AddSpacer(doc);

                // ---- procedures + affected sites/sectors table ----
                AddParagraph(doc, "ثانياً: الإجراءات:", 14, true, right, true);
                AddParagraph(doc,
                    "تم تلقي بيانات المحطات المتضررة من شركة " + company + " كما هو موضح كالتالي:",
                    14, false, right, true);

                BuildSitesTable(doc, sites);
                AddSpacer(doc);

                // ---- mission details ----
                AddParagraph(doc, "تفاصيل المأموريات:", 14, true, right, true);

                if (missions.Count == 0)
                {
                    AddParagraph(doc, "لا توجد مأموريات مسجلة على هذه الشكوى حتى الآن.", 14, false, right, true);
                }
                else
                {
                    for (int i = 0; i < missions.Count; i++)
                    {
                        BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT mission = missions[i];

                        string heading = "يوم " + ArabicDayName(mission.mission_Date) +
                            " الموافق " + FormatDate(mission.mission_Date) + " :";

                        AddParagraph(doc, heading, 14, true, right, true);

                        AddParagraph(doc, "القائمون بالمأمورية: " + JoinEngineers(mission.engineers),
                            14, false, right, true);

                        string comment = mission.mission_comment;
                        if (string.IsNullOrWhiteSpace(comment))
                            comment = "-";
                        AddParagraph(doc, comment, 14, false, right, true);
                        AddSpacer(doc);
                    }
                }

                // set the default document font as well (covers empty runs)
                doc.Content.Font.Name = ArabicFont;
                doc.Content.Font.NameBi = ArabicFont;

                app.Visible = true;
                app.Activate();
            }
            catch (Exception ex)
            {
                if (doc != null)
                {
                    try { doc.Close(); } catch { }
                }
                if (app != null)
                {
                    try { app.Quit(); } catch { }
                }

                MessageBox.Show("Failed to generate the report:\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void BuildSitesTable(Word.Document doc, List<BASIC_STRUCTS.SITE_STRUCT> sites)
        {
            int sectorRows = 0;
            for (int i = 0; i < sites.Count; i++)
                sectorRows += (sites[i].sectors != null ? sites[i].sectors.Count : 0);

            int totalRows = 1 + Math.Max(sectorRows, 0);

            Word.Range tableRange = doc.Content;
            tableRange.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

            Word.Table table = doc.Tables.Add(tableRange, totalRows, 6);
            table.Borders.Enable = 1;
            table.TableDirection = Word.WdTableDirection.wdTableDirectionLtr;
            table.Range.Font.Name = ArabicFont;
            table.Range.Font.Size = 11;
            table.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

            string[] headers = { "Site Name", "Sector Number", "Latitude", "Longitude", "RTWP", "Azimuth" };
            for (int c = 0; c < headers.Length; c++)
            {
                Word.Cell cell = table.Cell(1, c + 1);
                cell.Range.Text = headers[c];
                cell.Range.Font.Bold = 1;
            }

            int row = 2;
            for (int i = 0; i < sites.Count; i++)
            {
                BASIC_STRUCTS.SITE_STRUCT site = sites[i];
                if (site.sectors == null)
                    continue;

                for (int j = 0; j < site.sectors.Count; j++)
                {
                    if (row > totalRows)
                        break;

                    BASIC_STRUCTS.SECTOR_STRUCT sector = site.sectors[j];

                    table.Cell(row, 1).Range.Text = site.site_number ?? "";
                    table.Cell(row, 2).Range.Text = sector.sector_number ?? "";
                    table.Cell(row, 3).Range.Text = site.latitude ?? "";
                    table.Cell(row, 4).Range.Text = site.longitude ?? "";
                    table.Cell(row, 5).Range.Text = sector.rtwp.ToString();
                    table.Cell(row, 6).Range.Text = sector.azimuth.ToString();

                    row++;
                }
            }
        }
    }
}
