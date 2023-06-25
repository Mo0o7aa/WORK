using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Table = Microsoft.Office.Interop.Word.Table;

namespace ntra_missions
{
    public class WordExport
    {
        Missions mission;
        CommonFunctions commonFunctions;
        Employee loggedInUser;

        Document doc;

        String fullDesktopWordFilePath;

        public WordExport(Missions mMission, ref Employee mLoggedInUser)
        {
            commonFunctions = new CommonFunctions();
            mission = mMission;
            loggedInUser = mLoggedInUser;

            doc = new Document();

            //commonFunctions.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();

            //if(!commonFunctions.CheckFile(BASIC_STRUCTS.FOLDER_SHARE_PATH + loggedInUser.GetEmployeeName() + @"\report_form.docx"))
            //    File.Copy(BASIC_STRUCTS.FOLDER_SHARE_PATH + "report_form.docx", BASIC_STRUCTS.FOLDER_SHARE_PATH + loggedInUser.GetEmployeeName() + @"\report_form.docx");

            if (commonFunctions.CheckFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\report_form.docx"))
            {
                try
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\report_form.docx");
                    File.Copy(BASIC_STRUCTS.FOLDER_SHARE_PATH + "report_form.docx", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\report_form.docx");
                }
                catch
                {
                    MessageBox.Show("Desktop file 'report_form.docx' automatic delete failed, please delete it manually to avoid any system conflicts! Go to desktop and delete file report_form.docx and then try again, Thank you!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                File.Copy(BASIC_STRUCTS.FOLDER_SHARE_PATH + "report_form.docx", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\report_form.docx");
            }


            try
            {

                doc = app.Documents.Open(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\report_form.docx");
                doc.Activate();
                //Document doc = app.Documents.Open(@"\Photos\report_form.docx");



                Bookmark companyName = doc.Bookmarks["Company_Name"];
                Range companyNameRange = companyName.Range;
                companyNameRange.Text = mission.complaint.GetCompanyName();
                doc.Bookmarks.Add("Company_Name", companyNameRange);
                ///////////////////////

                Bookmark complaintDate = doc.Bookmarks["Complaint_Date"];
                Range complaintDateRange = complaintDate.Range;
                complaintDateRange.Text = mission.complaint.GetComplaintDate().Day.ToString() + "-" + mission.complaint.GetComplaintDate().Month.ToString() + "-" + mission.complaint.GetComplaintDate().Year.ToString();
                doc.Bookmarks.Add("Complaint_Date", complaintDateRange);
                //////////////////////

                Bookmark missionDate = doc.Bookmarks["Mission_Date"];
                Range missionDateRange = missionDate.Range;
                missionDateRange.Text = mission.GetDate().Day.ToString() + "-" + mission.GetDate().Month.ToString() + "-" + mission.GetDate().Year.ToString();
                doc.Bookmarks.Add("Mission_Date", missionDateRange);
                //////////////////////

                Bookmark freqBands = doc.Bookmarks["Freq_Bands"];
                Range freqBandsRange = freqBands.Range;
                freqBandsRange.Text = "";
                for (int i = 0; i < mission.complaint.sites[0].sectors[0].sector_bands.Count; i++)
                {
                    freqBandsRange.Text += mission.complaint.sites[0].sectors[0].sector_bands[i];
                    if (i != mission.complaint.sites[0].sectors[0].sector_bands.Count - 1)
                        freqBandsRange.Text += " , ";
                }

                doc.Bookmarks.Add("Freq_Bands", freqBandsRange);
                //////////////////////

                Bookmark engineers = doc.Bookmarks["Engineers"];
                Range engineersRange = engineers.Range;
                engineersRange.Text = "";
                for (int i = 0; i < mission.GetEngineers().Count; i++)
                {
                    engineersRange.Text += mission.GetEngineers()[i].value;
                    if (i != mission.GetEngineers().Count - 1)
                        engineersRange.Text += " , ";
                }

                doc.Bookmarks.Add("Engineers", engineersRange);
                //////////////////////

                List<int> addedLocations = new List<int>();
                Bookmark locations = doc.Bookmarks["Location"];
                Range locationsRange = locations.Range;
                locationsRange.Text = "";
                for (int i = 0; i < mission.GetSites().Count; i++)
                {
                    if (i == 0 || !addedLocations.Exists(x1 => x1 == mission.complaint.sites[i].city_id))
                    {
                        int index = mission.complaint.sites.FindIndex(x1 => x1.site_serial == mission.GetSites()[i].site_serial);
                        locationsRange.Text += mission.complaint.sites[index].city + " - " + mission.complaint.sites[index].region;

                        addedLocations.Add(mission.complaint.sites[index].city_id);

                        if (i != mission.GetSites().Count - 1)
                            locationsRange.Text += ", ";
                    }
                }

                doc.Bookmarks.Add("Location", locationsRange);
                //////////////////////


                Bookmark sites = doc.Bookmarks["Site_Number"];
                Range sitesRange = sites.Range;
                sitesRange.Text = "";
                for (int i = 0; i < mission.GetSites().Count; i++)
                {
                    sitesRange.Text += mission.GetSites()[i].site_name;
                    if (i != mission.GetSites().Count - 1)
                        sitesRange.Text += " , ";
                }

                doc.Bookmarks.Add("Site_Number", sitesRange);
                //////////////////////

                Bookmark numberOfSites = doc.Bookmarks["Number_Of_Sites"];
                Range numberOfSitesRange = numberOfSites.Range;
                numberOfSitesRange.Text = mission.GetSites().Count.ToString();
                doc.Bookmarks.Add("Number_Of_Sites", numberOfSitesRange);
                //////////////////////

                Bookmark results = doc.Bookmarks["Results"];
                Range resultsRange = results.Range;
                resultsRange.Text = "";
                for (int i = 0; i < mission.GetSites().Count; i++)
                {
                    resultsRange.Text += mission.GetSites()[i].site_name + ": " + "\n" + mission.GetSites()[i].comment;
                    if (i != mission.GetSites().Count - 1)
                        resultsRange.Text += "\n" + "\n";
                }

                doc.Bookmarks.Add("Results", resultsRange);


                //////////////////////

                Bookmark notes = doc.Bookmarks["Notes"];
                Range notesRange = notes.Range;
                notesRange.Text = mission.GetComment();
                doc.Bookmarks.Add("Notes", notesRange);


                //////////////////////

                for (int i = 0; i < mission.GetSites().Count + 1; i++)
                {
                    //Dokki9324
                    //18\6

                    Microsoft.Office.Interop.Word.Range tableLocation = doc.Application.ActiveDocument.Range(doc.Sentences.Last.End - 1, doc.Sentences.Last.End);
                    //Microsoft.Office.Interop.Word.Range tableLocation = doc.Tables[doc.Tables.Count - 1].Range;
                    tableLocation.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);
                    tableLocation.InsertParagraphAfter();
                    tableLocation.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);
                    doc.Application.ActiveDocument.Tables.Add(tableLocation, 1, 1);

                    Table table0 = doc.Tables[i + 1];
                    Cell site0 = table0.Cell(1, 1);
                    if (i != 0)
                    {
                        int index = mission.complaint.sites.FindIndex(x1 => x1.site_serial == mission.GetSites()[i - 1].site_serial);
                        site0.Range.Text = i + ". \tبخصوص شكوى التداخل على محطة  " + mission.complaint.GetCompanyName() + "رقم " + mission.GetSites()[i - 1].site_name + "على الحيز الترددي  " + "--" + mission.complaint.sites[index].sectors[0].sector_bands[0] + "-:";
                        site0.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                    }
                }


                //doc.Save();
                fullDesktopWordFilePath = doc.FullName;
                doc.Close();

                Process process = Process.Start(fullDesktopWordFilePath);
                process.Exited += OnExitWordFile;
            }
            catch
            {
                doc.Close();
                MessageBox.Show("Something went wrong, please try again later!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void OnExitWordFile(object sender, EventArgs e)
        {
            try
            {
                File.Delete(fullDesktopWordFilePath);
            }
            catch
            {
                MessageBox.Show("Desktop file 'report_form.docx' automatic delete failed, please delete it manually to avoid any system conflicts! Go to desktop and delete file report_form.docx, Thank you!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
