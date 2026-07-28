using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using DialogResult = System.Windows.Forms.DialogResult;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for ComplaintsPage.xaml
    /// </summary>
    public partial class ComplaintsPage : Page
    {
        private Employee loggedInUser;
        private readonly CommonQueries commonQueries;
        private readonly CommonFunctions commonFunctions;

        private List<BASIC_STRUCTS.COMPLAINT_STRUCT> complaints;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> companies;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> complaintStatus;

        private readonly ObservableCollection<ComplaintRow> complaintRows;
        private ICollectionView complaintsView;

        public ComplaintsPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();

            complaints = new List<BASIC_STRUCTS.COMPLAINT_STRUCT>();
            companies = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            complaintStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            complaintRows = new ObservableCollection<ComplaintRow>();

            InitializeComponent();

            pageHeader.Attach(loggedInUser);

            if (!commonQueries.GetComplaints(ref complaints))
                return;

            if (!commonQueries.GetCompanies(ref companies))
                return;

            complaintsView = CollectionViewSource.GetDefaultView(complaintRows);
            complaintsView.Filter = FilterComplaint;
            complaintsList.ItemsSource = complaintsView;

            InitializeFilterComboBoxes();

            RebuildRows();
        }

        private void InitializeFilterComboBoxes()
        {
            for (int i = BASIC_STRUCTS.START_YEAR; i <= DateTime.Now.Year; i++)
                yearComboBox.Items.Add(i);

            yearCheckBox.IsChecked = true;

            monthComboBox.Items.Clear();
            for (int i = 1; i < 13; i++)
                monthComboBox.Items.Add(i);

            companyComboBox.Items.Clear();
            for (int i = 0; i < companies.Count; i++)
                companyComboBox.Items.Add(companies[i].value);

            if (!commonQueries.GetComplaintStatus(ref complaintStatus))
                return;

            statusComboBox.Items.Clear();
            for (int i = 0; i < complaintStatus.Count; i++)
                statusComboBox.Items.Add(complaintStatus[i].value);
        }

        private void RebuildRows()
        {
            complaintRows.Clear();

            for (int i = 0; i < complaints.Count; i++)
                complaintRows.Add(new ComplaintRow(complaints[i], loggedInUser.GetEmployeeId()));

            UpdateCount();
            LoadAttachmentsAsync();
        }

        // The attachments live on a network share — enumerate them off the UI
        // thread so the page shows instantly even when the share is slow.
        private void LoadAttachmentsAsync()
        {
            List<ComplaintRow> rows = new List<ComplaintRow>(complaintRows);

            Task.Run(() =>
            {
                foreach (ComplaintRow row in rows)
                {
                    try
                    {
                        string folder = BASIC_STRUCTS.FOLDER_SHARE_PATH + @"Complaints\" + row.ComplaintId + @"\";

                        if (!commonFunctions.CheckDirectory(folder))
                            continue;

                        FileInfo[] files = commonFunctions.ListFilesInFolder(folder);
                        ComplaintRow targetRow = row;

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            targetRow.Attachments.Clear();
                            foreach (FileInfo file in files)
                            {
                                targetRow.Attachments.Add(new FileRow
                                {
                                    FileName = file.Name,
                                    FullPath = file.FullName,
                                    CanDelete = targetRow.CanEdit
                                });
                            }
                        }));
                    }
                    catch
                    {
                        // share unreachable — show the complaint without attachments
                    }
                }
            });
        }

        private bool FilterComplaint(object item)
        {
            ComplaintRow row = item as ComplaintRow;
            if (row == null)
                return false;

            if (searchCheckBox.IsChecked == true && searchTextBox.Text != "")
            {
                bool contains = false;
                string search = searchTextBox.Text;

                foreach (string siteNumber in row.SiteNumbers)
                {
                    if (siteNumber.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                    return false;
            }

            if (yearCheckBox.IsChecked == true && yearComboBox.SelectedIndex != -1)
            {
                if (row.Date.Year != int.Parse(yearComboBox.SelectedItem.ToString()))
                    return false;
            }

            if (monthCheckBox.IsChecked == true && monthComboBox.SelectedIndex != -1)
            {
                if (row.Date.Month != int.Parse(monthComboBox.SelectedItem.ToString()))
                    return false;
            }

            if (companyCheckBox.IsChecked == true && companyComboBox.SelectedIndex != -1)
            {
                if (row.CompanySerial != companies[companyComboBox.SelectedIndex].key)
                    return false;
            }

            if (statusCheckBox.IsChecked == true && statusComboBox.SelectedIndex != -1)
            {
                if (row.StatusId != complaintStatus[statusComboBox.SelectedIndex].key)
                    return false;
            }

            return true;
        }

        private void RefreshView()
        {
            if (complaintsView == null)
                return;

            complaintsView.Refresh();
            UpdateCount();
        }

        private void UpdateCount()
        {
            if (complaintsView == null)
                return;

            int count = 0;
            foreach (object item in complaintsView)
                count++;

            countText.Text = count.ToString();
        }

        private void OnClickRemoveFile(object sender, RoutedEventArgs e)
        {
            FileRow file = (FileRow)((Button)sender).Tag;

            File.Delete(file.FullPath);

            foreach (ComplaintRow row in complaintRows)
            {
                if (row.Attachments.Contains(file))
                {
                    row.Attachments.Remove(file);
                    break;
                }
            }
        }

        private void OnClickOpenFile(object sender, MouseButtonEventArgs e)
        {
            TextBlock currentTextBlock = (TextBlock)sender;
            try
            {
                if (currentTextBlock.Tag != null && currentTextBlock.Tag.ToString() != "")
                    Process.Start(currentTextBlock.Tag.ToString());
            }
            catch
            {
                MessageBox.Show("Current file is not available!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnClickExportExcel(object sender, RoutedEventArgs e)
        {
            ComplaintRow row = (ComplaintRow)((Button)sender).Tag;

            Complaints complaint = new Complaints();

            if (!complaint.InitializeComplaint(row.CompanySerial, row.ComplaintSerial))
                return;

            ExcelExport excelExport = new ExcelExport();
            excelExport.ExportComplaint(complaint);
        }

        private void OnClickExportMap(object sender, RoutedEventArgs e)
        {
            ComplaintRow row = (ComplaintRow)((Button)sender).Tag;

            Complaints complaint = new Complaints();

            if (!complaint.InitializeComplaint(row.CompanySerial, row.ComplaintSerial))
                return;

            GenerateKMZ generateKmz = new GenerateKMZ();
            generateKmz.ExportComplaint(complaint);
        }

        private void OnClickAttachFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                ComplaintRow row = (ComplaintRow)((Button)sender).Tag;

                Complaints complaint = new Complaints();

                if (!complaint.InitializeComplaint(row.CompanySerial, row.ComplaintSerial))
                    return;

                bool failed = false;

                for (int i = 0; i < fileDialog.FileNames.Length; i++)
                {
                    String filePath = fileDialog.FileNames[i];
                    String fileName = System.IO.Path.GetFileName(filePath);

                    try
                    {
                        commonFunctions.CreateDirectory(BASIC_STRUCTS.FOLDER_SHARE_PATH + @"Complaints\" + complaint.GetComplaintId());
                        File.Copy(filePath, BASIC_STRUCTS.FOLDER_SHARE_PATH + @"Complaints\" + complaint.GetComplaintId() + @"\" + fileName);
                    }
                    catch
                    {
                        MessageBox.Show(fileName + " upload failed please try again later!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        failed = true;
                    }
                }

                if (failed == false)
                    MessageBox.Show("Upload complete!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadAttachmentsAsync();
            }
        }

        private void OnClickAddMissionButton(object sender, RoutedEventArgs e)
        {
            ComplaintRow row = (ComplaintRow)((Button)sender).Tag;

            int viewAddCondition = BASIC_STRUCTS.MISSION_ADD_CONDITION;

            Missions mission = new Missions();

            if (!mission.complaint.InitializeComplaint(row.CompanySerial, row.ComplaintSerial))
                return;

            MissionWindow missionWindow = new MissionWindow(ref loggedInUser, ref mission, ref viewAddCondition);
            missionWindow.Closed += OnClosedComplaintsWindow;
            missionWindow.Show();
        }

        private void OnClickEditComplaint(object sender, RoutedEventArgs e)
        {
            ComplaintRow row = (ComplaintRow)((Button)sender).Tag;

            Complaints complaint = new Complaints();

            if (!complaint.InitializeComplaint(row.CompanySerial, row.ComplaintSerial))
                return;

            int viewAddCondition = BASIC_STRUCTS.COMPLAINT_EDIT_CONDITION;

            ComplaintsWindow complaintsWindow = new ComplaintsWindow(ref loggedInUser, ref complaint, ref viewAddCondition);
            complaintsWindow.Closed += OnClosedComplaintsWindow;
            complaintsWindow.Show();
        }

        private void OnClickViewComplaint(object sender, RoutedEventArgs e)
        {
            ComplaintRow row = (ComplaintRow)((Button)sender).Tag;

            Complaints complaint = new Complaints();

            if (!complaint.InitializeComplaint(row.CompanySerial, row.ComplaintSerial))
                return;

            int viewAddCondition = BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION;

            ComplaintsWindow complaintsWindow = new ComplaintsWindow(ref loggedInUser, ref complaint, ref viewAddCondition);
            complaintsWindow.Closed += OnClosedComplaintsWindow;
            complaintsWindow.Show();
        }

        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            int viewAddConidtion = BASIC_STRUCTS.COMPLAINT_ADD_CONDITION;

            Complaints complaint = new Complaints();

            ComplaintsWindow complaintsWindow = new ComplaintsWindow(ref loggedInUser, ref complaint, ref viewAddConidtion);
            complaintsWindow.Closed += OnClosedComplaintsWindow;
            complaintsWindow.Show();
        }

        private void OnClosedComplaintsWindow(object sender, EventArgs e)
        {
            if (!commonQueries.GetComplaints(ref complaints))
                return;

            RebuildRows();
        }

        private void OnCheckSearchCheckBox(object sender, RoutedEventArgs e)
        {
            searchTextBox.IsEnabled = true;
        }

        private void OnUncheckSearchCheckBox(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";
            searchTextBox.IsEnabled = false;
        }

        private void OnCheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            yearComboBox.IsEnabled = true;
            yearComboBox.SelectedIndex = yearComboBox.Items.Count - 1;
        }

        private void OnUncheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            yearComboBox.SelectedIndex = -1;
            yearComboBox.IsEnabled = false;
        }

        private void OnCheckMonthChheckBox(object sender, RoutedEventArgs e)
        {
            monthComboBox.IsEnabled = true;
            monthComboBox.SelectedIndex = 0;
        }

        private void OnUncheckMonthCheckBox(object sender, RoutedEventArgs e)
        {
            monthComboBox.SelectedIndex = -1;
            monthComboBox.IsEnabled = false;
        }

        private void OnCheckCompanyCheckBox(object sender, RoutedEventArgs e)
        {
            companyComboBox.IsEnabled = true;
            companyComboBox.SelectedIndex = 0;
        }

        private void OnUncheckCompanyCheckBox(object sender, RoutedEventArgs e)
        {
            companyComboBox.SelectedIndex = -1;
            companyComboBox.IsEnabled = false;
        }

        private void OnCheckStatusCheckBox(object sender, RoutedEventArgs e)
        {
            statusComboBox.IsEnabled = true;
            statusComboBox.SelectedIndex = 0;
        }

        private void OnUncheckStatusCheckBox(object sender, RoutedEventArgs e)
        {
            statusComboBox.SelectedIndex = -1;
            statusComboBox.IsEnabled = false;
        }

        private void OnTextChangedSearchTextBox(object sender, TextChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnSelChangedYearCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnSelChangedMonthCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnSelChangedCompanyCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnBtnClickImport(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                String filePath = fileDialog.FileNames[0];
                String fileName = System.IO.Path.GetFileName(filePath);

                ExcelExport excelExport = new ExcelExport();
                excelExport.ImportComplaint(fileName, filePath, ref loggedInUser);

                if (!commonQueries.GetComplaints(ref complaints))
                    return;

                RebuildRows();
            }
        }

        private void OnBtnClickformat(object sender, RoutedEventArgs e)
        {
            Process.Start("\\\\GIZA-ASAMEH\\Giza Software\\Excel formats\\complaint format.xlsx");
        }
    }
}
