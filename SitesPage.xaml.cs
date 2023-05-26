using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for SitesPage.xaml
    /// </summary>
    public partial class SitesPage : Page
    {
        private Employee loggedInUser;
        private CommonQueries commonQueries;
        private List<BASIC_STRUCTS.MIN_SITE_STRUCT> sites;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> companies;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> cities;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> employees;

        private Expander currentExpander;
        private Expander previousExpander;

        private Popup popUp;
        private DispatcherTimer dispatchertimer;

        public SitesPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;
            commonQueries = new CommonQueries();

            sites = new List<BASIC_STRUCTS.MIN_SITE_STRUCT>();
            companies = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            cities = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            employees = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            popUp = new Popup();
            popUp.PopupAnimation = PopupAnimation.Fade;
            //popUp.PlacementTarget = this.titleLabel;
            popUp.Placement = PlacementMode.Mouse;
            popUp.VerticalOffset = -20;
            popUp.AllowsTransparency = true;

            dispatchertimer = new DispatcherTimer();
            dispatchertimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatchertimer.Interval = new TimeSpan(0, 0, 0, 1, 50);

            InitializeComponent();


            if (!commonQueries.GetAllSites(ref sites))
                return;

            userNameLabel.Content = "Username: " + loggedInUser.GetEmployeeUserName();

            InitializeSitesStackPanel();
            FillFilterComboBoxes();
        }

        private void FillFilterComboBoxes()
        {
            if (!commonQueries.GetCompanies(ref companies))
                return;

            companyComboBox.Items.Clear();

            for(int i = 0; i < companies.Count; i++)
            {
                companyComboBox.Items.Add(companies[i].value);
            }

            if (!commonQueries.GetCities(ref cities))
                return;

            cityComboBox.Items.Clear();

            for(int j = 0; j < cities.Count; j++)
            {
                cityComboBox.Items.Add(cities[j].value);
            }

            if (!commonQueries.GetEngineers(ref employees))
                return;

            addedByComboBox.Items.Clear();

            for(int k = 0; k < employees.Count; k++)
            {
                addedByComboBox.Items.Add(employees[k].value);
            }

        }

        private void InitializeSitesStackPanel()
        {
            BrushConverter brushConverter = new BrushConverter();

            sitesStackPanel.Children.Clear();

            for(int i = 0; i < sites.Count; i++)
            {

                if(searchCheckBox.IsChecked == true && searchTextBox.Text != "")
                {
                    string search = searchTextBox.Text;
                    String siteNumberAndRegion = sites[i].site_number + " " + sites[i].region;
                    bool contains = siteNumberAndRegion.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }

                if(companyCheckBox.IsChecked == true && companyComboBox.SelectedIndex != -1)
                {
                    if (sites[i].company_serial != companies[companyComboBox.SelectedIndex].key)
                        continue;
                }

                if (cityCheckBox.IsChecked == true && cityComboBox.SelectedIndex != -1)
                {
                    if (sites[i].city_id != cities[cityComboBox.SelectedIndex].key)
                        continue;
                }

                if (addedByCheckBox.IsChecked == true && addedByComboBox.SelectedIndex != -1)
                {
                    if (sites[i].added_by_id != employees[addedByComboBox.SelectedIndex].key)
                        continue;
                }

                if (longCheckBox.IsChecked == true && longTextBox.Text != "")
                {
                    bool contains = sites[i].longitude.IndexOf(longTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;

                }

                if (latCheckBox.IsChecked == true && latTextBox.Text != "")
                {
                    bool contains = sites[i].latitude.IndexOf(latTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;

                }

                Border border = new Border() { BorderThickness = new Thickness(3) };
                border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");

                Grid grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(400) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(400) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });

                WrapPanel siteNumberWrapPanel = new WrapPanel();

                Label siteNumberLabel = new Label();
                siteNumberLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                siteNumberLabel.Content = "Site No: ";

                Label siteNumberLabelValue = new Label();
                siteNumberLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                siteNumberLabelValue.Content = sites[i].site_number;

                siteNumberWrapPanel.Children.Add(siteNumberLabel);
                siteNumberWrapPanel.Children.Add(siteNumberLabelValue);

                grid.Children.Add(siteNumberWrapPanel);
                Grid.SetColumn(siteNumberWrapPanel, 1);
                Grid.SetRow(siteNumberWrapPanel, 0);

                WrapPanel companyNameWrapPanel = new WrapPanel();

                Label companyNameLabel = new Label();
                companyNameLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                companyNameLabel.Content = "Company: ";

                Label companyNameLabelValue = new Label();
                companyNameLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                companyNameLabelValue.Content = sites[i].company_name;

                companyNameWrapPanel.Children.Add(companyNameLabel);
                companyNameWrapPanel.Children.Add(companyNameLabelValue);

                grid.Children.Add(companyNameWrapPanel);
                Grid.SetColumn(companyNameWrapPanel, 0);
                Grid.SetRow(companyNameWrapPanel, 0);

                WrapPanel cityWrapPanel = new WrapPanel();

                Label cityLabel = new Label();
                cityLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                cityLabel.Content = "City: ";

                Label cityLabelValue = new Label();
                cityLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                cityLabelValue.Content = sites[i].city;

                cityWrapPanel.Children.Add(cityLabel);
                cityWrapPanel.Children.Add(cityLabelValue);

                grid.Children.Add(cityWrapPanel);
                Grid.SetColumn(cityWrapPanel, 0);
                Grid.SetRow(cityWrapPanel, 1);

                WrapPanel regionWrapPanel = new WrapPanel();

                Label regionLabel = new Label();
                regionLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                regionLabel.Content = "Region: ";

                Label regionLabelValue = new Label();
                regionLabelValue.Style = (Style)FindResource("wideStackPanelLabelStyle");
                regionLabelValue.Content = sites[i].region;

                regionWrapPanel.Children.Add(regionLabel);
                regionWrapPanel.Children.Add(regionLabelValue);

                grid.Children.Add(regionWrapPanel);
                Grid.SetColumnSpan(regionWrapPanel, 2);
                Grid.SetRow(regionWrapPanel, 2);

                WrapPanel addedByWrapPanel = new WrapPanel();

                Label addedByLabel = new Label();
                addedByLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                addedByLabel.Content = "Added by: ";

                Label addedByLabelValue = new Label();
                addedByLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                addedByLabelValue.Content = sites[i].added_by;

                addedByWrapPanel.Children.Add(addedByLabel);
                addedByWrapPanel.Children.Add(addedByLabelValue);

                grid.Children.Add(addedByWrapPanel);
                Grid.SetColumn(addedByWrapPanel, 1);
                Grid.SetRow(addedByWrapPanel, 1);

                WrapPanel currentLongWrapPanel = new WrapPanel();

                Label currentLongLabel = new Label();
                currentLongLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                currentLongLabel.Content = "Long: ";

                Label currentLongLabelValue = new Label();
                currentLongLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                currentLongLabelValue.Content = sites[i].longitude;

                currentLongWrapPanel.Children.Add(currentLongLabel);
                currentLongWrapPanel.Children.Add(currentLongLabelValue);

                grid.Children.Add(currentLongWrapPanel);
                Grid.SetColumn(currentLongWrapPanel, 1);
                Grid.SetRow(currentLongWrapPanel, 2);

                WrapPanel currentLatWrapPanel = new WrapPanel();

                Label currentLatLabel = new Label();
                currentLatLabel.Style = (Style)FindResource("labelStyleBlack");
                currentLatLabel.Content = "Lat: ";

                Label currentLatLabelValue = new Label();
                currentLatLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                currentLatLabelValue.Content = sites[i].latitude;

                currentLatWrapPanel.Children.Add(currentLatLabel);
                currentLatWrapPanel.Children.Add(currentLatLabelValue);

                grid.Children.Add(currentLatWrapPanel);
                Grid.SetColumn(currentLatWrapPanel, 2);
                Grid.SetRow(currentLatWrapPanel, 2);

                Expander expander = new Expander();
                expander.HorizontalContentAlignment = HorizontalAlignment.Left;
                expander.VerticalAlignment = VerticalAlignment.Top;
                expander.ExpandDirection = ExpandDirection.Down;
                expander.Expanded += OnExpandExpander;
                expander.Margin = new Thickness(0, 12, 0, 0);
                expander.Tag = i;

                StackPanel expanderstackPanel = new StackPanel();
                expanderstackPanel.Width = 100;

                Button copyCoordinatesButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                copyCoordinatesButton.Content = "Copy Coordinates";
                copyCoordinatesButton.Width = 100;
                copyCoordinatesButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                copyCoordinatesButton.Click += OnClickCopyCooridinates;

                Button editButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                editButton.Content = "Edit";
                editButton.Width = 100;
                editButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                editButton.Click += OnClickEditSite;

                Button showHistoryButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                showHistoryButton.Content = "Show History";
                showHistoryButton.Width = 100;
                showHistoryButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                showHistoryButton.Click += OnClickShowHistory;

                expanderstackPanel.Children.Add(copyCoordinatesButton);
                expanderstackPanel.Children.Add(editButton);
                expanderstackPanel.Children.Add(showHistoryButton);

                expander.Content = expanderstackPanel;

                grid.Children.Add(expander);
                Grid.SetRowSpan(expander, 2);
                Grid.SetColumn(expander, 2);

                border.Child = grid;
                sitesStackPanel.Children.Add(border);
            }
        }

        private void InitializeSitesGrid()
        {
            sitesGrid.Children.Clear();
            sitesGrid.RowDefinitions.Clear();
        }

        private void OnClickShowHistory(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander expander = (Expander)currentStackPanel.Parent;

            List<BASIC_STRUCTS.COMPLAINT_STRUCT> tempComplaints = new List<BASIC_STRUCTS.COMPLAINT_STRUCT>();
            List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> tempMissions = new List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT>();

            if (!commonQueries.GetComplaints(ref tempComplaints, sites[int.Parse(expander.Tag.ToString())].company_serial, sites[int.Parse(expander.Tag.ToString())].site_serial))
                return;

            if (!commonQueries.GetMissions(ref tempMissions, sites[int.Parse(expander.Tag.ToString())].company_serial, sites[int.Parse(expander.Tag.ToString())].site_serial))
                return;

            ComplaintsHistoryWindow complaintsHistoryWindow = new ComplaintsHistoryWindow(ref loggedInUser, tempComplaints, tempMissions);
            complaintsHistoryWindow.Show();
           

        }

        private void OnClickEditSite(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander expander = (Expander)currentStackPanel.Parent;

            Site currentSite = new Site();

            if (!currentSite.InitializeSite(sites[int.Parse(expander.Tag.ToString())].company_serial, sites[int.Parse(expander.Tag.ToString())].site_serial))
                return;

            int viewAddCondition = BASIC_STRUCTS.SITE_EDIT_CONDITION;

            SiteWindow siteWindow = new SiteWindow(ref loggedInUser, ref currentSite, ref viewAddCondition);
            siteWindow.Closed += OnClosedSiteWindow;
            siteWindow.Show();
        }

        private void OnClickCopyCooridinates(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander expander = (Expander)currentStackPanel.Parent;

            String siteCoordinates = sites[int.Parse(expander.Tag.ToString())].longitude + "," + sites[int.Parse(expander.Tag.ToString())].latitude;
            Clipboard.SetText(siteCoordinates);

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            Label label = new Label();
            label.Style = (Style)FindResource("labelStyle");
            label.Content = @"Copied!";
            label.Width = 100;

            popUp.Child = label;

            popUp.IsOpen = true;
            expander.IsExpanded = false;

            dispatchertimer.Start();

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            popUp.IsOpen = false;
            dispatchertimer.IsEnabled = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //popUp.IsOpen = false;
            //popUp.Visibility = Visibility.Collapsed;
            //Label popUpChild = (Label)popUp.Child;
            //popUpChild.Content = "";
            //popUpChild.Visibility = Visibility.Collapsed;
        }

        private void OnClosedSiteWindow(object sender, EventArgs e)
        {
            if (!commonQueries.GetAllSites(ref sites))
                return;

            InitializeSitesStackPanel();
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

        }

        private void OnClickTableView(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            Site site = new Site();
            int viewAddCondition = BASIC_STRUCTS.SITE_ADD_CONDITION;

            SiteWindow siteWindow = new SiteWindow(ref loggedInUser, ref site, ref viewAddCondition);
            siteWindow.Closed += OnClosedSiteWindow;
            siteWindow.Show();
        }

        private void OnCheckSearchCheckbox(object sender, RoutedEventArgs e)
        {
            searchTextBox.IsEnabled = true;
        }

        private void OnUncheckSearchCheckbox(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";
            searchTextBox.IsEnabled = false;
        }

        private void OnCheckCompanyCheckbox(object sender, RoutedEventArgs e)
        {
            companyComboBox.IsEnabled = true;
            companyComboBox.SelectedIndex = 0;
        }

        private void OnUncheckCompanyCheckbox(object sender, RoutedEventArgs e)
        {
            companyComboBox.SelectedIndex = -1;
            companyComboBox.IsEnabled = false;
        }

        private void OnCheckCityCheckbox(object sender, RoutedEventArgs e)
        {
            cityComboBox.IsEnabled = true;
            cityComboBox.SelectedIndex = 0;
        }

        private void OnUncheckCityCheckbox(object sender, RoutedEventArgs e)
        {
            cityComboBox.SelectedIndex = -1;
            cityComboBox.IsEnabled = false;
        }

        private void OnCheckAddedByCheckbox(object sender, RoutedEventArgs e)
        {
            addedByComboBox.IsEnabled = true;
            addedByComboBox.SelectedIndex = 0;
        }

        private void OnUncheckAddedByCheckBox(object sender, RoutedEventArgs e)
        {
            addedByComboBox.SelectedIndex = -1;
            addedByComboBox.IsEnabled = false;
        }

        private void OnTextChangedSearchTextbox(object sender, TextChangedEventArgs e)
        {
            InitializeSitesStackPanel();
        }

        private void OnSelChangedCompanyCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeSitesStackPanel();
        }

        private void OnSelChangedCityCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeSitesStackPanel();
        }

        private void OnSelChangedAddedByCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeSitesStackPanel();
        }

        private void OnCheckLongCheckbox(object sender, RoutedEventArgs e)
        {
            longTextBox.IsEnabled = true;
        }

        private void OnUncheckLongCheckBox(object sender, RoutedEventArgs e)
        {
            longTextBox.Text = "";
            longTextBox.IsEnabled = false;
        }

        private void OnCheckLatCheckbox(object sender, RoutedEventArgs e)
        {
            latTextBox.IsEnabled = true;
        }

        private void OnUncheckLatCheckBox(object sender, RoutedEventArgs e)
        {
            latTextBox.Text = "";
            latTextBox.IsEnabled = false;
        }

        private void OnTextChangedLongTextBox(object sender, TextChangedEventArgs e)
        {
            InitializeSitesStackPanel();
        }

        private void OnTextChangedLatTextBox(object sender, TextChangedEventArgs e)
        {
            InitializeSitesStackPanel();
        }

    }
}
