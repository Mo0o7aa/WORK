using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for RepeatersPage.xaml
    /// </summary>
    public partial class RepeatersPage : Page
    {
        Employee loggedInUser;
        SQLServer sqlServer;
        CommonQueries commonQueries;

        private Expander currentExpander;
        private Expander previousExpander;

        private List<BASIC_STRUCTS.REPEATER_STRUCT> repeaters;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> repeatersStatus;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> employees;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> cities;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> areas;

        public RepeatersPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;
            sqlServer = new SQLServer();
            commonQueries = new CommonQueries();

            repeaters = new List<BASIC_STRUCTS.REPEATER_STRUCT>();
            repeatersStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            employees = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            cities = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            areas = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            if (!commonQueries.GetRepeaters(ref repeaters))
                return;

            if (!commonQueries.GetRepeatersStatus(ref repeatersStatus))
                return;

            if (!commonQueries.GetCities(ref cities))
                return;

            InitializeComponent();

            cityCheckBox.IsChecked = true;

            userNameLabel.Content = "Username: " + loggedInUser.GetEmployeeUserName();


            InitializeRepeatersStatusCombo();
            InitializeCityCombo();

            InitializeRepeatersStackPanel();
            InitializeRepeatersGrid();
        }

        private bool InitializeEmployeeCombo()
        {
            //if (!commonQueries.GetEngineers(ref employees))
            //    return false;
            //
            //for (int i = 0; i < employees.Count; i++)
            //{
            //    employeeComboBox.Items.Add(employees[i].value);
            //}
            //
            return true;
        }

        private bool InitializeRepeatersStatusCombo()
        {
            for (int i = 0; i < repeatersStatus.Count; i++)
            {
                statusComboBox.Items.Add(repeatersStatus[i].value);
            }

            return true;
        }

        private bool InitializeCityCombo()
        {
            for (int i = 0; i < cities.Count; i++)
            {
                cityComboBox.Items.Add(cities[i].value);
            }

            return true;
        }

        private void InitializeRepeatersStackPanel()
        {
            repeatersStackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
            repeatersStackPanel.Children.Clear();

            for (int i = 0; i < repeaters.Count; i++)
            {
                if (searchLatCheckBox.IsChecked == true && latTextBox.Text != "")
                {
                    bool contains = false;
                    string search = latTextBox.Text;


                    contains = repeaters[i].latitude.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }

                if (searchLongCheckBox.IsChecked == true && longTextBox.Text != "")
                {
                    bool contains = false;
                    string search = latTextBox.Text;

                    contains = repeaters[i].longitude.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }

                if (cityCheckBox.IsChecked == true && cityComboBox.SelectedIndex != -1)
                {
                    if (repeaters[i].city_id != cities[cityComboBox.SelectedIndex].key)
                        continue;
                }

                if (areaCheckBox.IsChecked == true && areaComboBox.SelectedIndex != -1)
                {
                    if (repeaters[i].area != areas[areaComboBox.SelectedIndex].value)
                        continue;
                }

                if (statusCheckBox.IsChecked == true && statusComboBox.SelectedIndex != -1)
                {
                    if (repeaters[i].status_id != repeatersStatus[statusComboBox.SelectedIndex].key)
                        continue;
                }


                


                Grid repeatersGrid = new Grid();
                repeatersGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                repeatersGrid.VerticalAlignment = VerticalAlignment.Stretch;

                repeatersGrid.RowDefinitions.Add(new RowDefinition());
                repeatersGrid.RowDefinitions.Add(new RowDefinition());
                repeatersGrid.RowDefinitions.Add(new RowDefinition());
                repeatersGrid.RowDefinitions.Add(new RowDefinition());

                repeatersGrid.ColumnDefinitions.Add(new ColumnDefinition());
                repeatersGrid.ColumnDefinitions.Add(new ColumnDefinition());
                repeatersGrid.ColumnDefinitions.Add(new ColumnDefinition());
                repeatersGrid.ColumnDefinitions.Add(new ColumnDefinition());

                BrushConverter brushConverter = new BrushConverter();

                Border border = new Border() { BorderThickness = new Thickness(3) };
                border.Tag = repeaters[i].repeater_serial;
                border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");


                WrapPanel serialWrapPanel = new WrapPanel();

                Label serialLabel = new Label();
                serialLabel.Content = "Serial: ";
                serialLabel.Style = (Style)FindResource("labelStyleBlack");

                Label serialLabelValue = new Label();
                serialLabelValue.Content = repeaters[i].repeater_serial;
                serialLabelValue.Style = (Style)FindResource("mediumLabelStyle");

                serialWrapPanel.Children.Add(serialLabel);
                serialWrapPanel.Children.Add(serialLabelValue);

                repeatersGrid.Children.Add(serialWrapPanel);
                Grid.SetRow(serialWrapPanel, 0);
                Grid.SetColumn(serialWrapPanel, 0);




                WrapPanel cityWrapPanel = new WrapPanel();

                Label cityLabel = new Label();
                cityLabel.Content = "City: ";
                cityLabel.Style = (Style)FindResource("labelStyleBlack");

                Label cityValue = new Label();
                cityValue.Content = repeaters[i].city;
                cityValue.Style = (Style)FindResource("mediumLabelStyle");

                cityWrapPanel.Children.Add(cityLabel);
                cityWrapPanel.Children.Add(cityValue);

                repeatersGrid.Children.Add(cityWrapPanel);
                Grid.SetRow(cityWrapPanel, 0);
                Grid.SetColumn(cityWrapPanel, 1);



                WrapPanel areaWrapPanel = new WrapPanel();

                Label areaLabel = new Label();
                areaLabel.Content = "Area: ";
                areaLabel.Style = (Style)FindResource("labelStyleBlack");

                Label areaValue = new Label();
                areaValue.Content = repeaters[i].area;
                areaValue.Style = (Style)FindResource("wideLabelStyle");

                areaWrapPanel.Children.Add(areaLabel);
                areaWrapPanel.Children.Add(areaValue);

                repeatersGrid.Children.Add(areaWrapPanel);
                Grid.SetRow(areaWrapPanel, 0);
                Grid.SetColumn(areaWrapPanel, 2);





                WrapPanel coordinatedWrapPanel = new WrapPanel();

                Label coordinatesLabel = new Label();
                coordinatesLabel.Content = "Coordinates: ";
                coordinatesLabel.Style = (Style)FindResource("labelStyleBlack");

                Label coordinatesValue = new Label() { Cursor = Cursors.Hand, ToolTip = "Left click to Copy coordinates!"};
                coordinatesValue.Content = repeaters[i].latitude + ", " + repeaters[i].longitude;
                coordinatesValue.Style = (Style)FindResource("wideLabelStyle");
                coordinatesValue.Tag = repeaters[i].repeater_serial;
                coordinatesValue.MouseLeftButtonDown += OnClickCopyCoordinates;

                coordinatedWrapPanel.Children.Add(coordinatesLabel);
                coordinatedWrapPanel.Children.Add(coordinatesValue);

                repeatersGrid.Children.Add(coordinatedWrapPanel);
                Grid.SetRow(coordinatedWrapPanel, 1);
                Grid.SetColumn(coordinatedWrapPanel, 0);




                WrapPanel addressWrapPanel = new WrapPanel();

                Label addressLabel = new Label();
                addressLabel.Content = "Address: ";
                addressLabel.Style = (Style)FindResource("labelStyleBlack");

                Label AddressValue = new Label();
                AddressValue.Content = repeaters[i].address;
                AddressValue.Style = (Style)FindResource("wideLabelStyle");
                AddressValue.Width = 400;

                addressWrapPanel.Children.Add(addressLabel);
                addressWrapPanel.Children.Add(AddressValue);

                repeatersGrid.Children.Add(addressWrapPanel);
                Grid.SetRow(addressWrapPanel, 1);
                Grid.SetColumn(addressWrapPanel, 1);
                Grid.SetColumnSpan(addressWrapPanel, 2);


                WrapPanel commentWrapPanel = new WrapPanel();

                Label commentLabel = new Label();
                commentLabel.Content = "Comment: ";
                commentLabel.Style = (Style)FindResource("labelStyleBlack");

                Label commentValue = new Label();
                commentValue.Content = repeaters[i].comment;
                commentValue.Style = (Style)FindResource("wideLabelStyle");
                commentValue.Width = 400;

                commentWrapPanel.Children.Add(commentLabel);
                commentWrapPanel.Children.Add(commentValue);

                repeatersGrid.Children.Add(commentWrapPanel);
                Grid.SetRow(commentWrapPanel, 2);
                Grid.SetColumn(commentWrapPanel, 1);
                Grid.SetColumnSpan(commentWrapPanel, 2);




                WrapPanel addedByWrapPanel = new WrapPanel();

                Label addedByLabel = new Label();
                addedByLabel.Content = "Added By: ";
                addedByLabel.Style = (Style)FindResource("labelStyleBlack");

                Label addedByLabelValue = new Label();
                addedByLabelValue.Content = repeaters[i].added_by;
                addedByLabelValue.Style = (Style)FindResource("mediumLabelStyle");

                addedByWrapPanel.Children.Add(addedByLabel);
                addedByWrapPanel.Children.Add(addedByLabelValue);

                repeatersGrid.Children.Add(addedByWrapPanel);
                Grid.SetRow(addedByWrapPanel, 2);
                Grid.SetColumn(addedByWrapPanel, 0);




                Border statusBorder = new Border() { Margin = new Thickness(6)};
                statusBorder.Style = (Style)FindResource("statusBorderStyle");
                statusBorder.Width = 200;
                if (repeaters[i].status_id == BASIC_STRUCTS.PENDING_REPEATER_STATUS)
                    statusBorder.Background = Brushes.Red;
                else
                    statusBorder.Background = Brushes.Green;

                Label statusLabel = new Label();
                statusLabel.Content = repeaters[i].status;
                statusLabel.Style = (Style)FindResource("statusLabelStyle");
                statusLabel.Width = 200;

                statusBorder.Child = statusLabel;

                repeatersGrid.Children.Add(statusBorder);
                Grid.SetRow(statusBorder, 2);
                Grid.SetColumn(statusBorder, 3);




                Expander expander = new Expander();
                expander.Tag = repeaters[i].repeater_serial;
                expander.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                expander.VerticalAlignment = VerticalAlignment.Top;
                expander.ExpandDirection = ExpandDirection.Down;
                expander.Expanded += OnExpandExpander;
                expander.Margin = new Thickness(0, 24, 0, 0);

                StackPanel expanderStackPanel = new StackPanel();

                Button editButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                editButton.Content = "Edit";
                editButton.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                editButton.Click += OnClickEdit; ;


                expanderStackPanel.Children.Add(editButton);

                expander.Content = expanderStackPanel;

                repeatersGrid.Children.Add(expander);
                Grid.SetColumn(expander, 4);


                border.Child = repeatersGrid;

                repeatersStackPanel.Children.Add(border);
            }


            countLabel.Content = repeatersStackPanel.Children.Count.ToString();

        }

        private void InitializeRepeatersGrid()
        {
            repeatersGrid.ItemsSource = null;

            String sqlQuery = @"select	repeaters.serial as Repeater_Serial,
			                            cities.city as City,
			                            repeaters.area as Area,
			                            repeaters.address as Address,
			                            lat as Latitude,
			                            long as Longitude,
			                            repeaters.comment as Comment,
			                            repeaters_status.status as Repeater_Status,
			                            employee_info.name as Added_by

                              from NTRA.dbo.repeaters
                              
                              left join NTRA.dbo.cities
                              on repeaters.city = cities.id
                              
                              left join NTRA.dbo.repeaters_status
                              on repeaters.status = repeaters_status.id
                              
                              left join NTRA.dbo.employee_info
                              on repeaters.added_by = employee_info.employee_id";

            SQLServer sql = new SQLServer();

            //String sqlQueryPart2 = @" where ";
            //bool firstCondition = true;

            //if (searchCheckBox.IsChecked == true && searchTextBox.Text != "")
            //{
            //    sqlQueryPart2 += " (company_sites.site_number like N'%";
            //    sqlQueryPart2 += searchTextBox.Text + "%' or company_sites.region like N'%" + searchTextBox.Text + "%') ";
            //    firstCondition = false;
            //}
            //if (companyCheckBox.IsChecked == true && companyComboBox.SelectedIndex != -1)
            //{
            //    if (!firstCondition)
            //        sqlQueryPart2 += " and ";
            //
            //    sqlQueryPart2 += " company_sites.company_serial = ";
            //    sqlQueryPart2 += companies[companyComboBox.SelectedIndex].key + " ";
            //    firstCondition = false;
            //}
            //if (cityCheckBox.IsChecked == true && cityComboBox.SelectedIndex != -1)
            //{
            //    if (!firstCondition)
            //        sqlQueryPart2 += " and ";
            //
            //    sqlQueryPart2 += " company_sites.city = ";
            //    sqlQueryPart2 += cities[cityComboBox.SelectedIndex].key + " ";
            //    firstCondition = false;
            //}
            //if (addedByCheckBox.IsChecked == true && addedByComboBox.SelectedIndex != -1)
            //{
            //    if (!firstCondition)
            //        sqlQueryPart2 += " and ";
            //
            //    sqlQueryPart2 += " company_sites.added_by = ";
            //    sqlQueryPart2 += employees[addedByComboBox.SelectedIndex].key + " ";
            //    firstCondition = false;
            //}
            //if (latCheckBox.IsChecked == true && latTextBox.Text != "")
            //{
            //    if (!firstCondition)
            //        sqlQueryPart2 += " and ";
            //
            //    sqlQueryPart2 += " company_sites.lat like'%";
            //    sqlQueryPart2 += latTextBox.Text + "%' ";
            //    firstCondition = false;
            //}
            //if (longCheckBox.IsChecked == true && longTextBox.Text != "")
            //{
            //    if (!firstCondition)
            //        sqlQueryPart2 += " and ";
            //
            //    sqlQueryPart2 += " company_sites.long like'%";
            //    sqlQueryPart2 += longTextBox.Text + "%' ";
            //    firstCondition = false;
            //}
            //
            //if (!firstCondition)
            //    sqlQuery += sqlQueryPart2;
            //
            //sqlQuery += " order by company_sites.date_added DESC";

            if (!sql.DataGridExport(ref repeatersGrid, sqlQuery))
                return;

        }

        private void OnClickCopyCoordinates(object sender, MouseButtonEventArgs e)
        {
            Label currentLabel = sender as Label;
            int repeaterSerial = int.Parse(currentLabel.Tag.ToString());

            String coordinates;

            coordinates = repeaters.Find(x1 => x1.repeater_serial == repeaterSerial).latitude.ToString() + ", " + repeaters.Find(x1 => x1.repeater_serial == repeaterSerial).longitude.ToString();

            Clipboard.SetText(coordinates);

            MessageBox.Show("Copied to clipboard!", "Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnClickEdit(object sender, RoutedEventArgs e)
        {

        }

        private void OnCheckSearchLatCheckBox(object sender, RoutedEventArgs e)
        {
            latTextBox.IsEnabled = true;
        }

        private void OnUncheckSearchCheckBox(object sender, RoutedEventArgs e)
        {
            latTextBox.Text = string.Empty;
            latTextBox.IsEnabled = false;
        }

        private void OnCheckSearchLongCheckBox(object sender, RoutedEventArgs e)
        {
            longTextBox.IsEnabled = true;
        }

        private void OnUncheckSearchLongCheckBox(object sender, RoutedEventArgs e)
        {
            longTextBox.Text = string.Empty;
            longTextBox.IsEnabled = false;
        }

        private void OnCheckCityCheckBox(object sender, RoutedEventArgs e)
        {
            cityComboBox.IsEnabled = true;
            cityComboBox.SelectedIndex = 0;
        }

        private void OnUncheckCityCheckBox(object sender, RoutedEventArgs e)
        {
            areaComboBox.SelectedIndex = -1;
            areaComboBox.IsEnabled = false;
            cityComboBox.SelectedIndex = -1;
            cityComboBox.IsEnabled = false;
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

        private void OnCheckAreaCheckBox(object sender, RoutedEventArgs e)
        {
            if (cityCheckBox.IsChecked == true)
            {

                if (!commonQueries.GetCityAreas(ref areas, cities[cityComboBox.SelectedIndex].key))
                {
                    MessageBox.Show("Server connection failed, please try again later!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                areaComboBox.Items.Clear();
                for (int i = 0; i < areas.Count; i++)
                {
                    areaComboBox.Items.Add(areas[i].value);
                }

                areaComboBox.IsEnabled = true;
                areaComboBox.SelectedIndex = 0;

            }
            else
            {
                MessageBox.Show("City must be selected to choose an area!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnUnCheckAreaCheckBox(object sender, RoutedEventArgs e)
        {
            areaComboBox.SelectedIndex = -1;
            areaComboBox.IsEnabled = false;
            areaComboBox.Items.Clear();
        }


        private void OnTextChangedLatTextBox(object sender, TextChangedEventArgs e)
        {
            InitializeRepeatersStackPanel();
            InitializeRepeatersGrid();
        }

        private void OnTextChangedLongTextBox(object sender, TextChangedEventArgs e)
        {
            InitializeRepeatersStackPanel();
            InitializeRepeatersGrid();
        }

        private void OnSelChangedCityCombo(object sender, SelectionChangedEventArgs e)
        {
            
            InitializeRepeatersStackPanel();
            InitializeRepeatersGrid();
        }

        private void OnSelChangedAreaCombo(object sender, SelectionChangedEventArgs e)
        {

            InitializeRepeatersStackPanel();
            InitializeRepeatersGrid();
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            

            

            InitializeRepeatersStackPanel();
            InitializeRepeatersGrid();
        }



        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {

        }

        private void OnBtnClickFormat(object sender, RoutedEventArgs e)
        {
            //System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            //
            //if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //
            //    String filePath = fileDialog.FileNames[0];
            //    String fileName = System.IO.Path.GetFileName(filePath);
            //
            //
            //    ExcelExport excelExport = new ExcelExport();
            //    excelExport.ImportRepeaters(fileName, filePath, ref loggedInUser);
            //
            //    if (!commonQueries.GetRepeaters(ref repeaters))
            //        return;
            //
            //    InitializeRepeatersStackPanel();
            //}

            Process.Start("\\\\GIZA-ASAMEH\\Giza Software\\Excel formats\\repeaters format.xlsx");
        }


        private void OnBtnClickReImport(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                String filePath = fileDialog.FileNames[0];
                String fileName = System.IO.Path.GetFileName(filePath);


                ExcelExport excelExport = new ExcelExport();
                excelExport.ReImportRepeaters(fileName, filePath, ref loggedInUser);

                if (!commonQueries.GetRepeaters(ref repeaters))
                    return;

                InitializeRepeatersStackPanel();
            }
        }

        private void OnBtnClickExport(object sender, RoutedEventArgs e)
        {
            if (repeatersStackPanel == null)
                return;

            List<BASIC_STRUCTS.REPEATER_STRUCT> selectedRepeaters = new List<BASIC_STRUCTS.REPEATER_STRUCT>();

            for (int i = 0; i < repeatersStackPanel.Children.Count; i++)
            {
                Border currentBorder = (Border)repeatersStackPanel.Children[i];
                selectedRepeaters.Add(repeaters.Find(x1 => x1.repeater_serial == int.Parse(currentBorder.Tag.ToString())));
            }

            ExcelExport export = new ExcelExport();
            export.ExportRepeaters(ref selectedRepeaters);
        }

        /////////////////////////// Common Functions for every page///////////////////////

        private void OnExpandExpander(object sender, RoutedEventArgs e)
        {
            previousExpander = currentExpander;
            currentExpander = (Expander)sender;

            if (previousExpander != null && previousExpander != currentExpander)
                previousExpander.IsExpanded = false;
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

        private void CompanyEquipmentMenuSelection(object sender, RoutedEventArgs e)
        {
            CompanyEquipmentPage companyEquipmentPage = new CompanyEquipmentPage(ref loggedInUser);
            NavigationService.Navigate(companyEquipmentPage);
        }

        private void CompanyMenuSelection(object sender, RoutedEventArgs e)
        {
            CompaniesPage companiesPage = new CompaniesPage(ref loggedInUser);
            NavigationService.Navigate(companiesPage);
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

        private void MonitoringMenuSelection(object sender, RoutedEventArgs e)
        {
            MissionsPage missionsPage = new MissionsPage(ref loggedInUser);
            NavigationService.Navigate(missionsPage);
        }

        private void InspectionMenuSelection(object sender, RoutedEventArgs e)
        {

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


        private void OnClickListView(object sender, MouseButtonEventArgs e)
        {
            if (listViewScrollViewer == null || tableViewScrollViewer == null)
                return;

            listViewScrollViewer.Visibility = Visibility.Visible;
            tableViewScrollViewer.Visibility = Visibility.Collapsed;

            BrushConverter brushConverter = new BrushConverter();

            listViewBorder.Background = Brushes.White;
            Label listLabel = (Label)listViewBorder.Child;
            listLabel.Foreground = (Brush)brushConverter.ConvertFrom("#000080");

            tableViewBorder.Background = (Brush)brushConverter.ConvertFrom("#000080");
            Label tableLabel = (Label)tableViewBorder.Child;
            tableLabel.Foreground = Brushes.White;

        }

        private void OnClickTableView(object sender, MouseButtonEventArgs e)
        {
            if (listViewScrollViewer == null || tableViewScrollViewer == null)
                return;

            listViewScrollViewer.Visibility = Visibility.Collapsed;
            tableViewScrollViewer.Visibility = Visibility.Visible;

            BrushConverter brushConverter = new BrushConverter();

            listViewBorder.Background = (Brush)brushConverter.ConvertFrom("#000080");
            Label listLabel = (Label)listViewBorder.Child;
            listLabel.Foreground = Brushes.White;

            tableViewBorder.Background = Brushes.White;
            Label tableLabel = (Label)tableViewBorder.Child;
            tableLabel.Foreground = (Brush)brushConverter.ConvertFrom("#000080");
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[A-Za-z]+");
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    

    }
}
