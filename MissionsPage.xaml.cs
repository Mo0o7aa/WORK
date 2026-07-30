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
    /// Interaction logic for MissionsPage.xaml
    /// </summary>
    public partial class MissionsPage : Page
    {
        private Employee loggedInUser;
        private readonly CommonQueries commonQueries;
        private readonly CommonFunctions commonFunctions;

        private List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> missions;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> engineers;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> serviceProviders;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> missionStatuses;

        private readonly ObservableCollection<MissionRow> missionRows;
        private ICollectionView missionsView;

        public MissionsPage(ref Employee mLoggedInUser)
        {
            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();

            missions = new List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT>();
            engineers = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            serviceProviders = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            missionStatuses = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            missionRows = new ObservableCollection<MissionRow>();

            loggedInUser = mLoggedInUser;

            InitializeComponent();

            pageHeader.Attach(loggedInUser);

            if (!commonQueries.GetMissions(ref missions))
                return;

            if (!commonQueries.GetServiceProviders(ref serviceProviders))
                return;

            if (!commonQueries.GetEngineers(ref engineers))
                return;

            if (!commonQueries.GetMissionStatus(ref missionStatuses))
                return;

            missionsView = CollectionViewSource.GetDefaultView(missionRows);
            missionsView.Filter = FilterMission;
            missionsList.ItemsSource = missionsView;

            InitializeYearCombo();
            InitializeMonthCombo();
            InitializeEmployeeCombo();
            InitializeServiceProviderCombo();
            InitializeStatusCombo();

            RebuildRows();
        }

        private void InitializeYearCombo()
        {
            for (int i = BASIC_STRUCTS.START_YEAR; i <= DateTime.Now.Year; i++)
                yearComboBox.Items.Add(i.ToString());

            yearCheckBox.IsChecked = true;
            yearComboBox.SelectedIndex = yearComboBox.Items.Count - 1;
        }

        private void InitializeMonthCombo()
        {
            for (int i = 1; i < 13; i++)
                monthComboBox.Items.Add(i);
        }

        private void InitializeEmployeeCombo()
        {
            for (int i = 0; i < engineers.Count; i++)
                employeeComboBox.Items.Add(engineers[i].value);
        }

        private void InitializeServiceProviderCombo()
        {
            for (int i = 0; i < serviceProviders.Count; i++)
                serviceProviderComboBox.Items.Add(serviceProviders[i].value);
        }

        private void InitializeStatusCombo()
        {
            for (int i = 0; i < missionStatuses.Count; i++)
                statusComboBox.Items.Add(missionStatuses[i].value);
        }

        private void RebuildRows()
        {
            missionRows.Clear();

            for (int i = 0; i < missions.Count; i++)
                missionRows.Add(new MissionRow(missions[i], loggedInUser.GetEmployeeId()));

            UpdateCount();
            LoadAttachmentsAsync();
        }

        // Attachments live on a network share — list them off the UI thread so
        // the page renders instantly even when the share is slow.
        private void LoadAttachmentsAsync()
        {
            List<MissionRow> rows = new List<MissionRow>(missionRows);

            Task.Run(() =>
            {
                foreach (MissionRow row in rows)
                {
                    try
                    {
                        string folder = BASIC_STRUCTS.FOLDER_SHARE_PATH + @"Missions\" + row.MissionId + @"\";

                        if (!commonFunctions.CheckDirectory(folder))
                            continue;

                        FileInfo[] files = commonFunctions.ListFilesInFolder(folder);
                        MissionRow targetRow = row;

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
                        // share unreachable — show the mission without attachments
                    }
                }
            });
        }

        private bool FilterMission(object item)
        {
            MissionRow row = item as MissionRow;
            if (row == null)
                return false;

            if (searchCheckBox.IsChecked == true && searchTextBox.Text != "")
            {
                bool contains = false;
                foreach (string siteName in row.SiteNames)
                {
                    if (siteName.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
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
                int year = int.Parse(yearComboBox.SelectedItem.ToString());
                if (row.StartDate.Year != year || row.EndDate.Year != year)
                    return false;
            }

            if (monthCheckBox.IsChecked == true && monthComboBox.SelectedIndex != -1)
            {
                int month = int.Parse(monthComboBox.SelectedItem.ToString());
                if (row.StartDate.Month != month || row.EndDate.Month != month)
                    return false;
            }

            if (employeeCheckBox.IsChecked == true && employeeComboBox.SelectedIndex != -1)
            {
                if (!row.EngineerKeys.Contains(engineers[employeeComboBox.SelectedIndex].key))
                    return false;
            }

            if (serviceProviderCheckBox.IsChecked == true && serviceProviderComboBox.SelectedIndex != -1)
            {
                if (row.CompanySerial != serviceProviders[serviceProviderComboBox.SelectedIndex].key)
                    return false;
            }

            if (statusCheckBox.IsChecked == true && statusComboBox.SelectedIndex != -1)
            {
                if (row.StatusId != missionStatuses[statusComboBox.SelectedIndex].key)
                    return false;
            }

            return true;
        }

        private void RefreshView()
        {
            if (missionsView == null)
                return;

            missionsView.Refresh();
            UpdateCount();
        }

        private void UpdateCount()
        {
            if (missionsView == null)
                return;

            int count = 0;
            foreach (object item in missionsView)
                count++;

            countText.Text = count.ToString();
        }

        private void OnClickRemoveFile(object sender, RoutedEventArgs e)
        {
            FileRow file = (FileRow)((Button)sender).Tag;

            File.Delete(file.FullPath);

            foreach (MissionRow row in missionRows)
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

        private void OnClickAttachFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                MissionRow row = (MissionRow)((Button)sender).Tag;

                Missions mission = new Missions();

                if (!mission.InitializeMission(row.CompanySerial, row.ComplaintSerial, row.MissionSerial))
                    return;

                bool failed = false;

                for (int i = 0; i < fileDialog.FileNames.Length; i++)
                {
                    String filePath = fileDialog.FileNames[i];
                    String fileName = System.IO.Path.GetFileName(filePath);

                    try
                    {
                        commonFunctions.CreateDirectory(BASIC_STRUCTS.FOLDER_SHARE_PATH + @"Missions\" + mission.GetMissionId());
                        File.Copy(filePath, BASIC_STRUCTS.FOLDER_SHARE_PATH + @"Missions\" + mission.GetMissionId() + @"\" + fileName);
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

        private void OnClickViewComplaint(object sender, RoutedEventArgs e)
        {
            MissionRow row = (MissionRow)((Button)sender).Tag;

            Complaints complaint = new Complaints();

            if (!complaint.InitializeComplaint(row.CompanySerial, row.ComplaintSerial))
                return;

            int viewAddCondition = BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION;

            ComplaintsWindow complaintsWindow = new ComplaintsWindow(ref loggedInUser, ref complaint, ref viewAddCondition);
            complaintsWindow.Show();
        }

        private void OnClickShowSite(object sender, RoutedEventArgs e)
        {
            MissionSiteRow siteRow = (MissionSiteRow)((Button)sender).Tag;

            Site currentSite = new Site();

            if (!currentSite.InitializeSite(siteRow.CompanySerial, siteRow.SiteSerial))
                return;

            int viewAddCondition = BASIC_STRUCTS.SITE_VIEW_CONDITION;

            SiteWindow siteWindow = new SiteWindow(ref loggedInUser, ref currentSite, ref viewAddCondition);
            siteWindow.Show();
        }

        private void OnClickFeedback(object sender, RoutedEventArgs e)
        {
            MissionRow row = (MissionRow)((Button)sender).Tag;

            Missions mission = new Missions();

            if (!mission.InitializeMission(row.CompanySerial, row.ComplaintSerial, row.MissionSerial))
                return;

            MissionFeedbackWindow missionFeedBackWindow = new MissionFeedbackWindow(ref loggedInUser, ref mission);
            missionFeedBackWindow.Closed += OnClosedMissionWindow;
            missionFeedBackWindow.Show();
        }

        private void OnClickWordExport(object sender, RoutedEventArgs e)
        {
            MissionRow row = (MissionRow)((Button)sender).Tag;

            Missions mission = new Missions();

            if (!mission.InitializeMission(row.CompanySerial, row.ComplaintSerial, row.MissionSerial))
                return;

            WordExport wordExport = new WordExport(mission, ref loggedInUser);
        }

        private void OnClickEdit(object sender, RoutedEventArgs e)
        {
            MissionRow row = (MissionRow)((Button)sender).Tag;

            int missionCondition = BASIC_STRUCTS.MISSION_EDIT_CONDITION;

            Missions mission = new Missions();

            if (!mission.InitializeMission(row.CompanySerial, row.ComplaintSerial, row.MissionSerial))
                return;

            MissionWindow missionWindow = new MissionWindow(ref loggedInUser, ref mission, ref missionCondition);
            missionWindow.Closed += OnClosedMissionWindow;
            missionWindow.Show();
        }

        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            MissionRow row = (MissionRow)((Button)sender).Tag;

            int missionCondition = BASIC_STRUCTS.MISSION_VIEW_CONDITION;

            Missions mission = new Missions();

            if (!mission.InitializeMission(row.CompanySerial, row.ComplaintSerial, row.MissionSerial))
                return;

            MissionWindow missionWindow = new MissionWindow(ref loggedInUser, ref mission, ref missionCondition);
            missionWindow.Show();
        }

        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            int missionCondition = BASIC_STRUCTS.MISSION_ADD_CONDITION;

            Missions mission = new Missions();

            MissionWindow missionWindow = new MissionWindow(ref loggedInUser, ref mission, ref missionCondition);
            missionWindow.Closed += OnClosedMissionWindow;
            missionWindow.Show();
        }

        private void OnClosedMissionWindow(object sender, EventArgs e)
        {
            if (!commonQueries.GetMissions(ref missions))
                return;

            RebuildRows();
        }

        private void OnCheckSearchCheckBox(object sender, RoutedEventArgs e)
        {
            searchTextBox.IsEnabled = true;
        }

        private void OnUncheckSearchCheckBox(object sender, RoutedEventArgs e)
        {
            searchTextBox.IsEnabled = false;
            searchTextBox.Text = "";
        }

        private void OnCheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            yearComboBox.IsEnabled = true;
            yearComboBox.SelectedIndex = 0;
        }

        private void OnUncheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            yearComboBox.IsEnabled = false;
            yearComboBox.SelectedIndex = -1;
        }

        private void OnCheckMonthCheckBox(object sender, RoutedEventArgs e)
        {
            monthComboBox.IsEnabled = true;
            monthComboBox.SelectedIndex = 0;
        }

        private void OnUncheckMonthCheckBox(object sender, RoutedEventArgs e)
        {
            monthComboBox.IsEnabled = false;
            monthComboBox.SelectedIndex = -1;
        }

        private void OnCheckEmployeeCheckBox(object sender, RoutedEventArgs e)
        {
            employeeComboBox.IsEnabled = true;
            employeeComboBox.SelectedIndex = engineers.FindIndex(x1 => x1.key == loggedInUser.GetEmployeeId());
        }

        private void OnUncheckEmployeeCheckBox(object sender, RoutedEventArgs e)
        {
            employeeComboBox.IsEnabled = false;
            employeeComboBox.SelectedIndex = -1;
        }

        private void OnCheckServiceProviderCheckBox(object sender, RoutedEventArgs e)
        {
            serviceProviderComboBox.IsEnabled = true;
            serviceProviderComboBox.SelectedIndex = 0;
        }

        private void OnUncheckServiceProviderCheckBox(object sender, RoutedEventArgs e)
        {
            serviceProviderComboBox.IsEnabled = false;
            serviceProviderComboBox.SelectedIndex = -1;
        }

        private void OnCheckStatusCheckBox(object sender, RoutedEventArgs e)
        {
            statusComboBox.IsEnabled = true;
            statusComboBox.SelectedIndex = 0;
        }

        private void OnUncheckStatusCheckBox(object sender, RoutedEventArgs e)
        {
            statusComboBox.IsEnabled = false;
            statusComboBox.SelectedIndex = -1;
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

        private void OnSelChangedEmployeeCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnSelChangedServiceProviderCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
        }
    }
}
