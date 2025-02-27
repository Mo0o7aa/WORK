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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Button = System.Windows.Controls.Button;
using Clipboard = System.Windows.Clipboard;
using Label = System.Windows.Controls.Label;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Orientation = System.Windows.Controls.Orientation;

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


        BrushConverter brushConverter;

        public SitesPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;
            commonQueries = new CommonQueries();

            sites = new List<BASIC_STRUCTS.MIN_SITE_STRUCT>();
            companies = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            cities = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            employees = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            brushConverter = new BrushConverter();

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
            InitializeSitesGrid();
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
                border.Tag = sites[i].site_serial;
                border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");

                Grid grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(400) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(400) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(400) });

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
                Grid.SetColumn(currentLongWrapPanel, 2);
                Grid.SetRow(currentLongWrapPanel, 2);

                WrapPanel currentLatWrapPanel = new WrapPanel();

                Label currentLatLabel = new Label();
                currentLatLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                currentLatLabel.Content = "Lat: ";

                Label currentLatLabelValue = new Label();
                currentLatLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                currentLatLabelValue.Content = sites[i].latitude;

                currentLatWrapPanel.Children.Add(currentLatLabel);
                currentLatWrapPanel.Children.Add(currentLatLabelValue);

                grid.Children.Add(currentLatWrapPanel);
                Grid.SetColumn(currentLatWrapPanel, 1);
                Grid.SetRow(currentLatWrapPanel, 2);

                Expander expander = new Expander();
                expander.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
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
                copyCoordinatesButton.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                copyCoordinatesButton.Click += OnClickCopyCooridinates;

                Button editButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                editButton.Content = "Edit";
                editButton.Width = 100;
                editButton.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                editButton.Click += OnClickEditSite;

                Button showHistoryButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                showHistoryButton.Content = "Show History";
                showHistoryButton.Width = 100;
                showHistoryButton.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
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

            countLabel.Content = sitesStackPanel.Children.Count.ToString();
        }

        private void InitializeSitesGrid()
        {
            sitesGrid.ItemsSource = null;

            String sqlQuery = @"  select  companies.name as Company_Name,
                                          site_number as Site_Number,
		                                  lat as Latitude,
		                                  long as Longitude,
		                                  cities.city as City,
		                                  region as Region,
		                                  employee_info.name as Added_By
	     
                                  from NTRA.dbo.company_sites
                                  left join NTRA.dbo.companies
                                  on company_sites.company_serial = companies.serial
                                  left join NTRA.dbo.cities
                                  on company_sites.city = cities.id
                                  left join NTRA.dbo.employee_info
                                  on company_sites.added_by = employee_info.employee_id";

            SQLServer sql = new SQLServer();

            String sqlQueryPart2 = @" where ";
            bool firstCondition = true;

            if(searchCheckBox.IsChecked == true && searchTextBox.Text != "")
            {
                sqlQueryPart2 += " (company_sites.site_number like N'%";
                sqlQueryPart2 += searchTextBox.Text + "%' or company_sites.region like N'%" + searchTextBox.Text + "%') ";
                firstCondition = false;
            }
            if(companyCheckBox.IsChecked == true && companyComboBox.SelectedIndex != -1)
            {
                if (!firstCondition)
                    sqlQueryPart2 += " and ";

                sqlQueryPart2 += " company_sites.company_serial = ";
                sqlQueryPart2 += companies[companyComboBox.SelectedIndex].key + " ";
                firstCondition = false;
            }
            if (cityCheckBox.IsChecked == true && cityComboBox.SelectedIndex != -1)
            {
                if (!firstCondition)
                    sqlQueryPart2 += " and ";

                sqlQueryPart2 += " company_sites.city = ";
                sqlQueryPart2 += cities[cityComboBox.SelectedIndex].key + " ";
                firstCondition = false;
            }
            if (addedByCheckBox.IsChecked == true && addedByComboBox.SelectedIndex != -1)
            {
                if (!firstCondition)
                    sqlQueryPart2 += " and ";

                sqlQueryPart2 += " company_sites.added_by = ";
                sqlQueryPart2 += employees[addedByComboBox.SelectedIndex].key + " ";
                firstCondition = false;
            }
            if (latCheckBox.IsChecked == true && latTextBox.Text != "")
            {
                if (!firstCondition)
                    sqlQueryPart2 += " and ";

                sqlQueryPart2 += " company_sites.lat like'%";
                sqlQueryPart2 += latTextBox.Text + "%' ";
                firstCondition = false;
            }
            if (longCheckBox.IsChecked == true && longTextBox.Text != "")
            {
                if (!firstCondition)
                    sqlQueryPart2 += " and ";

                sqlQueryPart2 += " company_sites.long like'%";
                sqlQueryPart2 += longTextBox.Text + "%' ";
                firstCondition = false;
            }

            if (!firstCondition)
                sqlQuery += sqlQueryPart2;

            sqlQuery += " order by company_sites.date_added DESC";

            if (!sql.DataGridExport(ref sitesGrid, sqlQuery))
                return;

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

            String siteCoordinates = sites[int.Parse(expander.Tag.ToString())].latitude + "," + sites[int.Parse(expander.Tag.ToString())].longitude;
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
            InitializeSitesGrid();
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
            DashBoard dashBoard = new DashBoard(ref loggedInUser);
            NavigationService.Navigate(dashBoard);
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
            listViewScrollViewer.Visibility = Visibility.Visible;
            tableViewScrollViewer.Visibility = Visibility.Collapsed;

            tableViewBorder.Background = (Brush)brushConverter.ConvertFrom("#000080");
            listViewBorder.Background = Brushes.White;

            Label currentListViewLabel = (Label)listViewBorder.Child;
            Label currentTableViewLabel = (Label)tableViewBorder.Child;

            currentListViewLabel.Foreground = (Brush)brushConverter.ConvertFrom("#000080");
            currentTableViewLabel.Foreground = Brushes.White;
        }

        private void OnClickTableView(object sender, MouseButtonEventArgs e)
        {
            listViewScrollViewer.Visibility = Visibility.Collapsed;
            tableViewScrollViewer.Visibility = Visibility.Visible;

            listViewBorder.Background = (Brush)brushConverter.ConvertFrom("#000080");
            tableViewBorder.Background = Brushes.White;

            Label currentListViewLabel = (Label)listViewBorder.Child;
            Label currentTableViewLabel = (Label)tableViewBorder.Child;

            currentTableViewLabel.Foreground = (Brush)brushConverter.ConvertFrom("#000080");
            currentListViewLabel.Foreground = Brushes.White;
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
            InitializeSitesGrid();
        }

        private void OnSelChangedCompanyCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeSitesStackPanel();
            InitializeSitesGrid();
        }

        private void OnSelChangedCityCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeSitesStackPanel();
            InitializeSitesGrid();
        }

        private void OnSelChangedAddedByCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeSitesStackPanel();
            InitializeSitesGrid();
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
            InitializeSitesGrid();
        }

        private void OnTextChangedLatTextBox(object sender, TextChangedEventArgs e)
        {
            InitializeSitesStackPanel();
            InitializeSitesGrid();
        }

        private void OnCopyDataGrid(object sender, DataGridRowClipboardEventArgs e)
        {
            var currentCell = e.ClipboardRowContent[sitesGrid.CurrentCell.Column.DisplayIndex];
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(currentCell);
        }

        private void OnBtnClickImport(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {

                String filePath = fileDialog.FileNames[0];
                String fileName = System.IO.Path.GetFileName(filePath);


                ExcelExport excelExport = new ExcelExport();
                excelExport.ImportCompanySites(fileName, filePath, ref loggedInUser);

                if (!commonQueries.GetAllSites(ref sites))
                    return;

                InitializeSitesStackPanel();
                InitializeSitesGrid();
            }
        }

        private void OnBtnClickExport(object sender, RoutedEventArgs e)
        {
            List<BASIC_STRUCTS.MIN_SITE_STRUCT> selectedSites = new List<BASIC_STRUCTS.MIN_SITE_STRUCT>();

            for(int i = 0; i < sitesStackPanel.Children.Count; i++)
            {
                Border currentBorder = (Border)sitesStackPanel.Children[i];
                selectedSites.Add(sites.Find(x1 => x1.site_serial == int.Parse(currentBorder.Tag.ToString())));
            }

            ExcelExport export = new ExcelExport();
            export.ExportCompanySites(ref selectedSites);


        }

        private void RepeatersMenueSelection(object sender, RoutedEventArgs e)
        {
            RepeatersPage repeatersPage = new RepeatersPage(ref loggedInUser);
            NavigationService.Navigate(repeatersPage);
        }
    }
}
