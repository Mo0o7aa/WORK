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

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for ComplaintsPage.xaml
    /// </summary>
    public partial class ComplaintsPage : Page
    {
        private Employee loggedInUser;
        private CommonQueries commonQueries;
        private List<BASIC_STRUCTS.COMPLAINT_STRUCT> complaints;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> companies;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> complaintStatus;

        private Expander currentExpander;
        private Expander previousExpander;

        public ComplaintsPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            commonQueries = new CommonQueries();

            complaints = new List<BASIC_STRUCTS.COMPLAINT_STRUCT>();
            companies = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            complaintStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            InitializeComponent();

            if (!commonQueries.GetComplaints(ref complaints))
                return;

            if (!commonQueries.GetCompanies(ref companies))
                return;

            userNameLabel.Content = "Username: " + loggedInUser.GetEmployeeUserName();

            InitializeFilterComboBoxes();

            InitializeCompaintsStackPanel();
        }

        private void InitializeFilterComboBoxes()
        {
            for(int i = BASIC_STRUCTS.START_YEAR; i <= DateTime.Now.Year; i++)
            {
                yearComboBox.Items.Add(i);
            }

            yearCheckBox.IsChecked = true;

            monthComboBox.Items.Clear();

            for (int i = 1; i < 13; i++)
            {
                monthComboBox.Items.Add(i);
            }

            companyComboBox.Items.Clear();

            for (int i = 0; i < companies.Count; i++)
            {
                companyComboBox.Items.Add(companies[i].value);
            }

            if (!commonQueries.GetComplaintStatus(ref complaintStatus))
                return;

            statusComboBox.Items.Clear();

            for(int i = 0; i < complaintStatus.Count; i++)
            {
                statusComboBox.Items.Add(complaintStatus[i].value);
            }
        }

        private void InitializeCompaintsStackPanel()
        {
            BrushConverter brushConverter = new BrushConverter();

            complaintsStackPanel.Children.Clear();

            for(int i = 0; i < complaints.Count; i++)
            {
                if (searchCheckBox.IsChecked == true && searchTextBox.Text != "")
                {
                    string search = searchTextBox.Text;
                    bool contains = complaints[i].complaint_id.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }

                if (yearCheckBox.IsChecked == true && yearComboBox.SelectedIndex != -1)
                {
                    if (complaints[i].complaint_date.Year != int.Parse(yearComboBox.SelectedItem.ToString()))
                        continue;
                }

                if (monthCheckBox.IsChecked == true && monthComboBox.SelectedIndex != -1)
                {
                    if (complaints[i].complaint_date.Month != int.Parse(monthComboBox.SelectedItem.ToString()))
                        continue;
                }

                if (companyCheckBox.IsChecked == true && companyComboBox.SelectedIndex != -1)
                {
                    if (complaints[i].company_serial != companies[companyComboBox.SelectedIndex].key)
                        continue;
                }

                if (statusCheckBox.IsChecked == true && statusComboBox.SelectedIndex != -1)
                {
                    if (complaints[i].complaint_status_id != complaintStatus[statusComboBox.SelectedIndex].key)
                        continue;
                }

                Border border = new Border() { BorderThickness = new Thickness(3) };
                border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");

                Grid grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80)});
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(400)});
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(400)});
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300)});

                Label complaintHeaderLabel = new Label();
                complaintHeaderLabel.Style = (Style)FindResource("stackPanelHeaderLabelStyle");
                complaintHeaderLabel.Content = complaints[i].complaint_id;
                complaintHeaderLabel.Margin = new Thickness(10);

                grid.Children.Add(complaintHeaderLabel);
                Grid.SetColumnSpan(complaintHeaderLabel, 3);
                Grid.SetRow(complaintHeaderLabel, 0);

                WrapPanel companyNameWrapPanel = new WrapPanel();

                Label companyNameLabel = new Label();
                companyNameLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                companyNameLabel.Content = "Company: ";

                Label companyNameLabelValue = new Label();
                companyNameLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                companyNameLabelValue.Content = complaints[i].company_name;

                companyNameWrapPanel.Children.Add(companyNameLabel);
                companyNameWrapPanel.Children.Add(companyNameLabelValue);

                grid.Children.Add(companyNameWrapPanel);
                Grid.SetColumn(companyNameWrapPanel, 0);
                Grid.SetRow(companyNameWrapPanel, 1);

                WrapPanel cityWrapPanel = new WrapPanel();
                
                Label cityLabel = new Label();
                cityLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                cityLabel.Content = "Added by: ";
                
                Label cityLabelValue = new Label();
                cityLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                cityLabelValue.Content = complaints[i].added_by;
                
                cityWrapPanel.Children.Add(cityLabel);
                cityWrapPanel.Children.Add(cityLabelValue);
                
                grid.Children.Add(cityWrapPanel);
                Grid.SetColumn(cityWrapPanel, 0);
                Grid.SetRow(cityWrapPanel, 2);

                WrapPanel ticketNumberWrapPanel = new WrapPanel();

                Label ticketNumberLabel = new Label();
                ticketNumberLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                ticketNumberLabel.Content = "Ticket: ";

                Label ticketNumberLabelValue = new Label();
                ticketNumberLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                ticketNumberLabelValue.Content = complaints[i].ticket_number;

                ticketNumberWrapPanel.Children.Add(ticketNumberLabel);
                ticketNumberWrapPanel.Children.Add(ticketNumberLabelValue);

                grid.Children.Add(ticketNumberWrapPanel);
                Grid.SetRow(ticketNumberWrapPanel, 1);
                Grid.SetColumn(ticketNumberWrapPanel, 1);

                //WrapPanel regionWrapPanel = new WrapPanel();
                //
                //Label regionLabel = new Label();
                //regionLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                //regionLabel.Content = "Region: ";
                //
                //Label regionLabelValue = new Label();
                //regionLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                //regionLabelValue.Content = complaints[i].region;
                //
                //regionWrapPanel.Children.Add(regionLabel);
                //regionWrapPanel.Children.Add(regionLabelValue);
                //
                //grid.Children.Add(regionWrapPanel);
                //Grid.SetColumn(regionWrapPanel, 0);
                //Grid.SetRow(regionWrapPanel, 3);

                WrapPanel siteWrapPanel = new WrapPanel();

                Label siteLabel = new Label();
                siteLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                siteLabel.Content = "No of Sites: ";

                Label siteLabelValue = new Label();
                siteLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                siteLabelValue.Content = complaints[i].sites.Count();

                siteWrapPanel.Children.Add(siteLabel);
                siteWrapPanel.Children.Add(siteLabelValue);

                grid.Children.Add(siteWrapPanel);
                Grid.SetRow(siteWrapPanel, 2);
                Grid.SetColumn(siteWrapPanel, 1);

                Border statusBorder = new Border();
                statusBorder.Style = (Style)FindResource("statusBorderStyle");
                if (complaints[i].complaint_status_id == BASIC_STRUCTS.OPEN_COMPLAINT_STATUS)
                    statusBorder.Background = Brushes.Red;
                else if (complaints[i].complaint_status_id == BASIC_STRUCTS.PENDING_COMPLAINT_STATUS)
                    statusBorder.Background = Brushes.Yellow;
                else
                    statusBorder.Background = Brushes.Green;

                Label statusLabel = new Label();
                statusLabel.Style = (Style)FindResource("statusLabelStyle");
                statusLabel.Content = complaints[i].complaint_status;

                statusBorder.Child = statusLabel;
                grid.Children.Add(statusBorder);
                Grid.SetRowSpan(statusBorder, 4);
                Grid.SetColumn(statusBorder, 2);

                Expander expander = new Expander();
                expander.HorizontalContentAlignment = HorizontalAlignment.Left;
                expander.VerticalAlignment = VerticalAlignment.Top;
                expander.ExpandDirection = ExpandDirection.Down;
                expander.Expanded += OnExpandExpander;
                expander.Margin = new Thickness(0, 24, 0, 0);
                expander.Tag = i;

                StackPanel expanderstackPanel = new StackPanel();

                Button viewButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                viewButton.Content = "View";
                viewButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                viewButton.Click += OnClickViewComplaint;

                Button editButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                editButton.Content = "Edit";
                editButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                editButton.Click += OnClickEditComplaint;

                Button AddMissionButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                AddMissionButton.Content = "Add Mission";
                AddMissionButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                AddMissionButton.Click += OnClickAddMissionButton;

                Button deleteButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                deleteButton.Content = "Delete";
                deleteButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                deleteButton.Click += OnClickDeleteComplaint;


                expanderstackPanel.Children.Add(viewButton);
                expanderstackPanel.Children.Add(editButton);
                expanderstackPanel.Children.Add(AddMissionButton);

                expander.Content = expanderstackPanel;

                grid.Children.Add(expander);
                Grid.SetRow(expander, 0);
                Grid.SetRowSpan(expander, 2);
                Grid.SetColumn(expander, 2);

                WrapPanel dateWrapPanel = new WrapPanel();

                Label dateLabel = new Label();
                dateLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                dateLabel.Content = "Date: ";

                Label dateLabelValue = new Label();
                dateLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                dateLabelValue.Content = complaints[i].complaint_date;

                dateWrapPanel.Children.Add(dateLabel);
                dateWrapPanel.Children.Add(dateLabelValue);

                grid.Children.Add(dateWrapPanel);
                Grid.SetRow(dateWrapPanel, 3);
                Grid.SetColumn(dateWrapPanel, 0);

                border.Child = grid;

                complaintsStackPanel.Children.Add(border);
            }
        }

        private void OnClickAddMissionButton(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander expander = (Expander)currentStackPanel.Parent;

            int viewAddCondition = BASIC_STRUCTS.MISSION_ADD_CONDITION;

            Missions mission = new Missions();

            if (!mission.complaint.InitializeComplaint(complaints[int.Parse(expander.Tag.ToString())].company_serial, complaints[int.Parse(expander.Tag.ToString())].complaint_serial))
                return;

            MissionWindow missionWindow = new MissionWindow(ref loggedInUser, ref mission, ref viewAddCondition);
            missionWindow.Closed += OnClosedComplaintsWindow;
            missionWindow.Show();
        }

        private void OnClickDeleteComplaint(object sender, RoutedEventArgs e)
        {

        }

        private void OnClickEditComplaint(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander expander = (Expander)currentStackPanel.Parent;

            Complaints complaint = new Complaints();

            if (!complaint.InitializeComplaint(complaints[int.Parse(expander.Tag.ToString())].company_serial, complaints[int.Parse(expander.Tag.ToString())].complaint_serial))
                return;

            int viewAddCondition = BASIC_STRUCTS.COMPLAINT_EDIT_CONDITION;

            ComplaintsWindow complaintsWindow = new ComplaintsWindow(ref loggedInUser, ref complaint, ref viewAddCondition);
            complaintsWindow.Closed += OnClosedComplaintsWindow;
            complaintsWindow.Show();
        }

        private void OnClickViewComplaint(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander expander = (Expander)currentStackPanel.Parent;

            Complaints complaint = new Complaints();

            if (!complaint.InitializeComplaint(complaints[int.Parse(expander.Tag.ToString())].company_serial, complaints[int.Parse(expander.Tag.ToString())].complaint_serial))
                return;

            int viewAddCondition = BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION;

            ComplaintsWindow complaintsWindow = new ComplaintsWindow(ref loggedInUser, ref complaint, ref viewAddCondition);
            complaintsWindow.Closed += OnClosedComplaintsWindow;
            complaintsWindow.Show();
        }

        private void OnExpandExpander(object sender, RoutedEventArgs e)
        {
            previousExpander = currentExpander;
            currentExpander = (Expander)sender;

            if (previousExpander != null && previousExpander != currentExpander)
                previousExpander.IsExpanded = false;
        }

        private void MouseEnterMainMenu(object sender, MouseEventArgs e)
        {
            mainMenuGrid.Background = SystemColors.ActiveBorderBrush;
        }

        private void MouseLeaveMainMenu(object sender, MouseEventArgs e)
        {
            mainMenuGrid.Background = Brushes.LightGray;
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
        private void DashboardMenuSelection(object sender, RoutedEventArgs e)
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
        private void MonitoringMenuSelection(object sender, RoutedEventArgs e)
        {
            MissionsPage missionsPage = new MissionsPage(ref loggedInUser);
            NavigationService.Navigate(missionsPage);
        }

        private void InspectionMenuSelection(object sender, RoutedEventArgs e)
        {

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

        private void OnClickListView(object sender, MouseButtonEventArgs e)
        {
            BrushConverter brushConverter = new BrushConverter();

            listViewScrollViewer.Visibility = Visibility.Visible;
            tableViewScrollViewer.Visibility = Visibility.Collapsed;

            tableViewBorder.Background = (Brush)brushConverter.ConvertFrom("#000080");
            listViewBorder.Background = Brushes.White;

            Label listViewLabel = (Label)listViewBorder.Child;
            Label tableViewLabel = (Label)tableViewBorder.Child;

            listViewLabel.Foreground = (Brush)brushConverter.ConvertFrom("#000080");
            tableViewLabel.Foreground = Brushes.White;
            
        }

        private void OnClickTableView(object sender, MouseButtonEventArgs e)
        {
            BrushConverter brushConverter = new BrushConverter();

            listViewScrollViewer.Visibility = Visibility.Collapsed;
            tableViewScrollViewer.Visibility = Visibility.Visible;

            tableViewBorder.Background = Brushes.White;
            listViewBorder.Background = (Brush)brushConverter.ConvertFrom("#000080");

            Label listViewLabel = (Label)listViewBorder.Child;
            Label tableViewLabel = (Label)tableViewBorder.Child;

            listViewLabel.Foreground = Brushes.White;
            tableViewLabel.Foreground = (Brush)brushConverter.ConvertFrom("#000080");
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

            InitializeCompaintsStackPanel();
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

        private void OnTextChangedSearchTextBox(object sender, TextChangedEventArgs e)
        {
            InitializeCompaintsStackPanel();
        }

        private void OnSelChangedYearCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeCompaintsStackPanel();
        }

        private void OnSelChangedMonthCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeCompaintsStackPanel();
        }


        private void OnSelChangedCompanyCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeCompaintsStackPanel();
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

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeCompaintsStackPanel();
        }
    }
}
