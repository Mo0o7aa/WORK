using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Mail;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;
using Cursors = System.Windows.Input.Cursors;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.Forms.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using TextBox = System.Windows.Controls.TextBox;
using System.IO;
using System.Diagnostics;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for MissionsPage.xaml
    /// </summary>
    public partial class MissionsPage : Page
    {
        Employee loggedInUser;
        CommonQueries commonQueries;

        private List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> missions;
        private List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> filteredMissions;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> engineers;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> serviceProviders;

        private Expander currentExpander;
        private Expander previousExpander;

        CommonFunctions commonFunctions;
        
        public MissionsPage(ref Employee mLoggedInUser)
        {
            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();

            missions = new List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT>();
            filteredMissions = new List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT>();
            engineers = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            serviceProviders = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            loggedInUser = mLoggedInUser;

            InitializeComponent();

            userNameLabel.Content = userNameLabel.Content + " " + loggedInUser.GetEmployeeUserName();

            if (!commonQueries.GetMissions(ref missions))
                return;

            if (!commonQueries.GetServiceProviders(ref serviceProviders))
                return;

            if (!commonQueries.GetEngineers(ref engineers))
                return;

            InitializeYearCombo();
            InitializeMonthCombo();
            InitializeEmployeeCombo();
            InitializeServiceProviderCombo();

            InitializeMissionsStackPanel();
        }

        private class MissionSiteItemViewModel
        {
            public string SiteName { get; set; }
            public string Reason { get; set; }
            public string Comment { get; set; }
            public string SiteTag { get; set; }
        }

        private class MissionListItemViewModel
        {
            public string MissionId { get; set; }
            public List<string> Engineers { get; set; }
            public string RegionText { get; set; }
            public List<MissionSiteItemViewModel> Sites { get; set; }
            public string StatusText { get; set; }
            public bool IsPendingStatus { get; set; }
            public string StartDateText { get; set; }
            public string EndDateText { get; set; }
            public string MissionTag { get; set; }
            public string ComplaintTag { get; set; }
            public Visibility CanEditVisibility { get; set; }
        }

        private void InitializeYearCombo()
        {
            int initialYear = BASIC_STRUCTS.START_YEAR;
            int endYear = DateTime.Now.Year;

            for(int i = initialYear; i <= endYear; i++)
            {
                yearComboBox.Items.Add(initialYear.ToString());
                initialYear++;
            }

            yearCheckBox.IsChecked = true;
            yearComboBox.SelectedIndex = yearComboBox.Items.Count - 1;
        }

        private void InitializeMonthCombo()
        {
            for (int i = 1; i < 13; i++)
            {
                monthComboBox.Items.Add(i);
            }
        }

        private void InitializeEmployeeCombo()
        {
            for(int i = 0; i < engineers.Count; i++)
            {
                employeeComboBox.Items.Add(engineers[i].value);
            }
        }

        private void InitializeServiceProviderCombo()
        {
            for (int i = 0; i < serviceProviders.Count; i++)
            {
                serviceProviderComboBox.Items.Add(serviceProviders[i].value);
            }
        }

        private void InitializeMissionsStackPanel()
        {
            filteredMissions.Clear();

            for (int i = 0; i < missions.Count; i++)
            {
                if (searchCheckBox.IsChecked == true && searchTextBox.Text != "")
                {
                    bool contains = false;
                    string search = searchTextBox.Text;
                    for (int j = 0; j < missions[i].sites.Count; j++)
                    {
                        contains = missions[i].sites[j].site_name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                        if (contains)
                            break;
                    }
                    if (!contains)
                        continue;
                }

                if (yearCheckBox.IsChecked == true && yearComboBox.SelectedIndex != -1)
                {
                    if (missions[i].mission_Date.Year != int.Parse(yearComboBox.SelectedItem.ToString()) || missions[i].end_date.Year != int.Parse(yearComboBox.SelectedItem.ToString()))
                        continue;
                }

                if (monthCheckBox.IsChecked == true && monthComboBox.SelectedIndex != -1)
                {
                    if (missions[i].mission_Date.Month != int.Parse(monthComboBox.SelectedItem.ToString()) || missions[i].end_date.Month != int.Parse(monthComboBox.SelectedItem.ToString()))
                        continue;
                }

                if (employeeCheckBox.IsChecked == true && employeeComboBox.SelectedIndex != -1)
                {
                    if (!missions[i].engineers.Exists(x1 => x1.key == engineers[employeeComboBox.SelectedIndex].key))
                        continue;
                }

                if (serviceProviderCheckBox.IsChecked == true && serviceProviderComboBox.SelectedIndex != -1)
                {
                    if (missions[i].company_serial != serviceProviders[serviceProviderComboBox.SelectedIndex].key)
                        continue;
                }

                filteredMissions.Add(missions[i]);
            }

            missionsListView.ItemsSource = filteredMissions.Select(mission => new MissionListItemViewModel
            {
                MissionId = mission.mission_id,
                Engineers = mission.engineers.Select(x => "Eng.: " + x.value).ToList(),
                RegionText = mission.sites.Count > 0 ? "Area: " + mission.sites[0].region : "Area: -",
                Sites = mission.sites.Select(site => new MissionSiteItemViewModel
                {
                    SiteName = site.site_name,
                    Reason = site.reason_of_interference,
                    Comment = site.comment,
                    SiteTag = mission.company_serial + "," + mission.complaint_serial + "," + site.site_serial
                }).ToList(),
                StatusText = mission.status,
                IsPendingStatus = mission.status_id == BASIC_STRUCTS.MISSION_PENDING_OPERATOR_ACTION_STATUS || mission.status_id == BASIC_STRUCTS.MISSION_PENDING_NTRA_ACTION_STATUS,
                StartDateText = "Start Date: " + mission.mission_Date.Day + "/" + mission.mission_Date.Month + "/" + mission.mission_Date.Year,
                EndDateText = "End Date: " + mission.end_date.Day + "/" + mission.end_date.Month + "/" + mission.end_date.Year,
                MissionTag = mission.company_serial + "," + mission.complaint_serial + "," + mission.mission_serial,
                ComplaintTag = mission.company_serial + "," + mission.complaint_serial,
                CanEditVisibility = mission.added_by_id == loggedInUser.GetEmployeeId() ? Visibility.Visible : Visibility.Collapsed
            }).ToList();
        }

        private void OnClickRemoveFile(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;

            File.Delete(currentImage.Tag.ToString());

            InitializeMissionsStackPanel();
        }


        private void OnClickShowRepeaters(object sender, RoutedEventArgs e)
        {
            //Button currentButton = (Button)sender;
            //StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            //Expander currentExpander = (Expander)currentStackPanel.Parent;
            //
            //int viewAddCondition = BASIC_STRUCTS.REPEATER_EDIT_CONDITION;
            //List<BASIC_STRUCTS.REPEATER_STRUCT> currentRepeaters = new List<BASIC_STRUCTS.REPEATER_STRUCT>();
            //
            //currentRepeaters = missions.Find(x1 => x1.company_serial == int.Parse(currentExpander.Tag.ToString().Split(',')[0]) && x1.complaint_serial == int.Parse(currentExpander.Tag.ToString().Split(',')[1]) && x1.mission_serial == int.Parse(currentExpander.Tag.ToString().Split(',')[2])).repeaters;
            //
            //RepeatersWindow repeatersWindow = new RepeatersWindow(ref loggedInUser, ref currentRepeaters, ref viewAddCondition);
            //repeatersWindow.Closed += OnClosedMissionWindow;
            //repeatersWindow.Show();
        }

        private void OnClickAddRepeaters(object sender, RoutedEventArgs e)
        {
            //Button currentButton = (Button)sender;
            //StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            //Expander currentExpander = (Expander)currentStackPanel.Parent;
            //
            //int viewAddCondition = BASIC_STRUCTS.REPEATER_ADD_CONDITION;
            //
            //List<BASIC_STRUCTS.REPEATER_STRUCT> repeaters = new List<BASIC_STRUCTS.REPEATER_STRUCT>();
            //
            //BASIC_STRUCTS.REPEATER_STRUCT tempRepeater = new BASIC_STRUCTS.REPEATER_STRUCT();
            //tempRepeater.company_serial = int.Parse(currentExpander.Tag.ToString().Split(',')[0]);
            //tempRepeater.complaint_serial = int.Parse(currentExpander.Tag.ToString().Split(',')[1]);
            //tempRepeater.mission_serial = int.Parse(currentExpander.Tag.ToString().Split(',')[2]);
            //
            //repeaters.Add(tempRepeater);
            //
            //
            //RepeatersWindow repeatersWindow = new RepeatersWindow(ref loggedInUser, ref repeaters, ref viewAddCondition);
            //repeatersWindow.Closed += OnClosedMissionWindow;
            //repeatersWindow.Show();
        }


        private void OnClickOpenFile(object sender, MouseButtonEventArgs e)
        {
            TextBlock currentTextBlock = (TextBlock)sender;
            try
            {
                if (currentTextBlock.Tag != "")
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
                Button currentButton = (Button)sender;
                StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
                Expander expander = (Expander)currentStackPanel.Parent;

                Missions mission = new Missions();

                if (!mission.InitializeMission(int.Parse(currentExpander.Tag.ToString().Split(',')[0]), int.Parse(currentExpander.Tag.ToString().Split(',')[1]), int.Parse(currentExpander.Tag.ToString().Split(',')[2])))
                    return;

                bool failed = false;

                for (int i = 0; i < fileDialog.FileNames.Length; i++)
                {
                    String filePath = fileDialog.FileNames[i];
                    String fileName = System.IO.Path.GetFileName(filePath);

                    try
                    {
                        commonFunctions.CreateDirectory(BASIC_STRUCTS.FOLDER_SHARE_PATH + @"Missions\" + mission.GetMissionId());
                        File.Copy(fileDialog.FileName, BASIC_STRUCTS.FOLDER_SHARE_PATH + @"Missions\" + mission.GetMissionId() + @"\" + fileName);
                    }
                    catch
                    {
                        MessageBox.Show(fileName + " upload failed please try again later!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        failed = true;
                    }
                }

                if (failed == false)
                    MessageBox.Show("Upload complete!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                InitializeMissionsStackPanel();
            }
        }

        private void OnClickViewComplaint(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)currentStackPanel.Parent;

            Complaints complaint = new Complaints();

            if (!complaint.InitializeComplaint(int.Parse(currentExpander.Tag.ToString().Split(',')[0]), int.Parse(currentExpander.Tag.ToString().Split(',')[1])))
                return;

            int viewAddCondition = BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION;

            ComplaintsWindow complaintsWindow = new ComplaintsWindow(ref loggedInUser, ref complaint, ref viewAddCondition);
            complaintsWindow.Show();

        }

        private void OnClickShowSite(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;

            Site currentSite = new Site();

            String currentCompanySerial = currentButton.Tag.ToString().Split(',')[0];
            String currentSiteSerial = currentButton.Tag.ToString().Split(',')[2];

            if (!currentSite.InitializeSite(int.Parse(currentCompanySerial), int.Parse(currentSiteSerial)))
                return;

            int viewAddCondition = BASIC_STRUCTS.SITE_VIEW_CONDITION;

            SiteWindow siteWindow = new SiteWindow(ref loggedInUser, ref currentSite, ref viewAddCondition);

            siteWindow.Show();
        }

        private void OnClickFeedback(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)currentStackPanel.Parent;

            Missions mission = new Missions();

            if (!mission.InitializeMission(int.Parse(currentExpander.Tag.ToString().Split(',')[0]), int.Parse(currentExpander.Tag.ToString().Split(',')[1]), int.Parse(currentExpander.Tag.ToString().Split(',')[2])))
                return;

            MissionFeedbackWindow missionFeedBackWindow = new MissionFeedbackWindow(ref loggedInUser, ref mission);
            missionFeedBackWindow.Closed += OnClosedMissionWindow;
            missionFeedBackWindow.Show();

        }

        ///////BUTTON CLICK HANDLERS////////////////////////
        ////////////////////////////////////////////////////

        private void OnBtnClickDelete(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)currentStackPanel.Parent;


            Missions mission = new Missions();
            //mission.InitializeMission(missions[index].id);
            //
            //if (!mission.DeleteMission())
            //    return;

            if (!commonQueries.GetMissions(ref missions))
                return;

            //InitializeMissionsStackPanel();
        }

        private void OnClickWordExport(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)currentStackPanel.Parent;

            int missionCondition = BASIC_STRUCTS.MISSION_EDIT_CONDITION;

            Missions mission = new Missions();

            if (!mission.InitializeMission(int.Parse(currentExpander.Tag.ToString().Split(',')[0]), int.Parse(currentExpander.Tag.ToString().Split(',')[1]), int.Parse(currentExpander.Tag.ToString().Split(',')[2])))
                return;

            WordExport wordExport = new WordExport(mission, ref loggedInUser); 
        }

        private void OnClickEdit(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)currentStackPanel.Parent;

            int missionCondition = BASIC_STRUCTS.MISSION_EDIT_CONDITION;

            Missions mission = new Missions();

            if (!mission.InitializeMission(int.Parse(currentExpander.Tag.ToString().Split(',')[0]), int.Parse(currentExpander.Tag.ToString().Split(',')[1]), int.Parse(currentExpander.Tag.ToString().Split(',')[2])))
                return;

            MissionWindow missionWindow = new MissionWindow(ref loggedInUser, ref mission, ref missionCondition);
            missionWindow.Closed += OnClosedMissionWindow;
            missionWindow.Show();
        }

        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)currentStackPanel.Parent;

            int missionCondition = BASIC_STRUCTS.MISSION_VIEW_CONDITION;

            Missions mission = new Missions();
            
            if (!mission.InitializeMission(int.Parse(currentExpander.Tag.ToString().Split(',')[0]), int.Parse(currentExpander.Tag.ToString().Split(',')[1]), int.Parse(currentExpander.Tag.ToString().Split(',')[2])))
                return;

            MissionWindow missionWindow = new MissionWindow(ref loggedInUser, ref mission, ref missionCondition);
            missionWindow.Show();

        }
        private void OnClickChangePasswordButton(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow(ref loggedInUser);
            changePasswordWindow.Show();
        }

       
        private void OnClickLogoutButton(object sender, RoutedEventArgs e)
        {
            SignInWindow signInWindow = new SignInWindow();
            signInWindow.Show();

            NavigationWindow parentWindow = (NavigationWindow)this.Parent;
            parentWindow.Close();
        }

        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            int missionCondition = BASIC_STRUCTS.MISSION_ADD_CONDITION;

            Missions mission = new Missions();

            MissionWindow missionWindow = new MissionWindow(ref loggedInUser, ref mission, ref missionCondition);
            missionWindow.Closed += OnClosedMissionWindow;
            missionWindow.Show();
        }

        ///////ON CLOSED HANDLERS////////////////////////
        /////////////////////////////////////////////////

        

        private void OnClosedMissionWindow(object sender, EventArgs e)
        {
            if (!commonQueries.GetMissions(ref missions))
                return;

            InitializeMissionsStackPanel();
        }

        ///////ON CHECK/UNCHECK HANDLERS/////////////////
        /////////////////////////////////////////////////
        
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
        

        ///////SELECTION CHANGED HANDLERS////////////////
        /////////////////////////////////////////////////

        private void OnTextChangedSearchTextBox(object sender, TextChangedEventArgs e)
        {
            InitializeMissionsStackPanel();
        }
        private void OnSelChangedYearCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeMissionsStackPanel();
        }

        private void OnSelChangedEmployeeCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeMissionsStackPanel();
        }

        private void OnSelChangedServiceProviderCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeMissionsStackPanel();
        }
        private void OnSelChangedMonthCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeMissionsStackPanel();
        }
        private void OnExpandExpander(object sender, RoutedEventArgs e)
        {
            previousExpander = currentExpander;
            currentExpander = (Expander)sender;

            if (previousExpander != null && previousExpander != currentExpander)
                previousExpander.IsExpanded = false;
        }
        private void OnClickMainMenu(object sender, MouseButtonEventArgs e)
        {
            if (tabMenuButton.ContextMenu.IsOpen == false)
                tabMenuButton.ContextMenu.IsOpen = true;
            else
                tabMenuButton.ContextMenu.IsOpen = false;
        }

        private void OnClosedMenuContectMenu(object sender, RoutedEventArgs e)
        {
            mainMenuGrid.Focusable = false;
        }

        private void MouseEnterMainMenu(object sender, MouseEventArgs e)
        {
            mainMenuGrid.Background = SystemColors.ActiveBorderBrush;
        }

        private void MouseLeaveMainMenu(object sender, MouseEventArgs e)
        {
            mainMenuGrid.Background = Brushes.LightGray;
        }


        ////////NAVIGATION HANDLERS//////////
        /////////////////////////////////////
        
        private void DashboardMenuSelection(object sender, RoutedEventArgs e)
        {
            DashBoard dashBoard = new DashBoard(ref loggedInUser);
            NavigationService.Navigate(dashBoard);
        }

        private void InspectionMenuSelection(object sender, RoutedEventArgs e)
        {

        }

        private void employeeMenuSelection(object sender, RoutedEventArgs e)
        {
            EmployeesPage employeesPage = new EmployeesPage(ref loggedInUser);
            NavigationService.Navigate(employeesPage);
        }

        private void CompanyMenuSelection(object sender, RoutedEventArgs e)
        {
            CompaniesPage companiesPage = new CompaniesPage(ref loggedInUser);
            NavigationService.Navigate(companiesPage);
        }

        private void CompanyEquipmentMenuSelection(object sender, RoutedEventArgs e)
        {
            CompanyEquipmentPage companyEquipmentPage = new CompanyEquipmentPage(ref loggedInUser);
            NavigationService.Navigate(companyEquipmentPage);
        }
        private void SitesMenuSelection(object sender, RoutedEventArgs e)
        {
            SitesPage sitesPage = new SitesPage(ref loggedInUser);
            NavigationService.Navigate(sitesPage);
        }

        private void emfMenuSelection(object sender, RoutedEventArgs e)
        {
            EMFPage emfPage = new EMFPage(ref loggedInUser);
            NavigationService.Navigate(emfPage);
        }
        private void ComplaintsMenuSelection(object sender, RoutedEventArgs e)
        {
            ComplaintsPage complaintsPage = new ComplaintsPage(ref loggedInUser);
            NavigationService.Navigate(complaintsPage);
        }

        private void RepeatersMenueSelection(object sender, RoutedEventArgs e)
        {
            RepeatersPage repeatersPage = new RepeatersPage(ref loggedInUser);
            NavigationService.Navigate(repeatersPage);
        }
    }
}
