using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for EMFPage.xaml
    /// </summary>
    public partial class EMFPage : Page
    {
        Employee loggedInUser;

        CommonQueries commonQueries;

        private List<BASIC_STRUCTS.EMF_PLAN_STRUCT> plans;
        private List<BASIC_STRUCTS.EMF_STRUCT> emfPoints;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> pointStatus;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> engineers;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> planStatus;

        Expander currentExpander;
        Expander previousExpander;

        Popup popUp;
        DispatcherTimer dispatchertimer;

        public EMFPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            commonQueries = new CommonQueries();

            emfPoints = new List<BASIC_STRUCTS.EMF_STRUCT>();
            pointStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            plans = new List<BASIC_STRUCTS.EMF_PLAN_STRUCT>();
            engineers = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            planStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            popUp = new Popup();
            popUp.PopupAnimation = PopupAnimation.Fade;
            popUp.Placement = PlacementMode.Mouse;
            popUp.VerticalOffset = -20;
            popUp.AllowsTransparency = true;

            dispatchertimer = new DispatcherTimer();
            dispatchertimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatchertimer.Interval = new TimeSpan(0, 0, 0, 1, 50);

            InitializeComponent();

            userNameLabel.Content = userNameLabel.Content + " " + loggedInUser.GetEmployeeUserName();

            if (!commonQueries.GetEMFPoints(ref emfPoints))
                return;
            if (!commonQueries.GetEMFPlans(ref plans))
                return;

            InitializePointStatusComboBox();
            InitializeAssignedComboBox();
            InitializePlanStatusComboBox();

            InitializeEMFPointsStackPanel();
            InitializeEMFPlansStackPanel();

        }

        private void InitializePointStatusComboBox()
        {
            if (!commonQueries.GetEMFPointStatus(ref pointStatus))
                return;

            statusComboBox.Items.Clear();

            for(int i = 0; i < pointStatus.Count; i++)
            {
                statusComboBox.Items.Add(pointStatus[i].value);
            }
        }

        private void InitializeAssignedComboBox()
        {
            if (!commonQueries.GetEngineers(ref engineers))
                return;

            assignedComboBox.Items.Clear();

            for (int i = 0; i < engineers.Count; i++)
            {
                assignedComboBox.Items.Add(engineers[i].value);
            }
        }


        private void InitializePlanStatusComboBox()
        {
            if (!commonQueries.GetEMFPlanStatus(ref planStatus))
                return;

            planStatusComboBox.Items.Clear();

            for (int i = 0; i < planStatus.Count; i++)
            {
                planStatusComboBox.Items.Add(planStatus[i].value);
            }
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

        }

        private void employeeMenuSelection(object sender, RoutedEventArgs e)
        {
            EmployeesPage employeesPage = new EmployeesPage(ref loggedInUser);
            NavigationService.Navigate(employeesPage);
        }

        private void companyMenuSelection(object sender, RoutedEventArgs e)
        {
            CompaniesPage companiesPage = new CompaniesPage(ref loggedInUser);
            NavigationService.Navigate(companiesPage);
        }
        private void SitesMenuSelection(object sender, RoutedEventArgs e)
        {
            SitesPage sitesPage = new SitesPage(ref loggedInUser);
            NavigationService.Navigate(sitesPage);
        }
        private void CompanyEquipmentMenuSelection(object sender, RoutedEventArgs e)
        {
            CompanyEquipmentPage companyEquipmentPage = new CompanyEquipmentPage(ref loggedInUser);
            NavigationService.Navigate(companyEquipmentPage);
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


        private void OnClickListView(object sender, MouseButtonEventArgs e)
        {
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

        

        private void InitializeEMFPointsStackPanel()
        {
            BrushConverter brushConverter = new BrushConverter();

            pointsStackPanel.Children.Clear();

            for (int i = 0; i < emfPoints.Count; i++)
            {

                if (statusCheckBox.IsChecked == true && statusComboBox.SelectedIndex != -1)
                {
                    if (emfPoints[i].emf_status_id != pointStatus[statusComboBox.SelectedIndex].key)
                        continue;
                }
                if (nameCheckBox.IsChecked == true && nameTextBox.Text != "")
                {
                    string search = nameTextBox.Text;
                    bool contains = emfPoints[i].name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                
                    if (!contains)
                        continue;
                }
                if (areaCheckBox.IsChecked == true && areaTextBox.Text != "")
                {
                    string search = areaTextBox.Text;
                    bool contains = emfPoints[i].area.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }
                if (districtCheckBox.IsChecked == true && districtTextBox.Text != "")
                {
                    string search = districtTextBox.Text;
                    bool contains = emfPoints[i].district.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }
                if (latCheckBox.IsChecked == true && latTextBox.Text != "")
                {
                    string search = latTextBox.Text;
                    bool contains = emfPoints[i].latitude.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }
                if (longCheckBox.IsChecked == true && longTextBox.Text != "")
                {
                    string search = longTextBox.Text;
                    bool contains = emfPoints[i].longitude.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }
                if (actualLatCheckBox.IsChecked == true && actualLatTextBox.Text != "")
                {
                    string search = actualLatTextBox.Text;
                    bool contains = emfPoints[i].actual_latitude.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }
                if (actualLongCheckBox.IsChecked == true && actualLongTextBox.Text != "")
                {
                    string search = actualLongTextBox.Text;
                    bool contains = emfPoints[i].actual_longitude.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }

                Border border = new Border() { BorderThickness = new Thickness(3) };
                border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");

                Grid grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() );

                WrapPanel pointNameWrapPanel = new WrapPanel();

                Label pointNameLabel = new Label();
                pointNameLabel.Style = (Style)FindResource("labelStyleBlack");
                pointNameLabel.Content = "Name: ";

                Label pointNameLabelValue = new Label();
                pointNameLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                pointNameLabelValue.Content = emfPoints[i].name;
                pointNameLabelValue.ToolTip = "Left Click to Copy!";
                pointNameLabelValue.MouseLeftButtonDown += OnClickCoppyLabel;

                pointNameWrapPanel.Children.Add(pointNameLabel);
                pointNameWrapPanel.Children.Add(pointNameLabelValue);

                grid.Children.Add(pointNameWrapPanel);
                Grid.SetColumn(pointNameWrapPanel, 0);
                Grid.SetRow(pointNameWrapPanel, 0);

                WrapPanel areaWrapPanel = new WrapPanel();

                Label areaLabel = new Label();
                areaLabel.Style = (Style)FindResource("labelStyleBlack");
                areaLabel.Content = "Area: ";

                Label areaLabelValue = new Label();
                areaLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                areaLabelValue.Content = emfPoints[i].area;
                areaLabelValue.ToolTip = "Left Click to Copy!";
                areaLabelValue.MouseLeftButtonDown += OnClickCoppyLabel;

                areaWrapPanel.Children.Add(areaLabel);
                areaWrapPanel.Children.Add(areaLabelValue);

                grid.Children.Add(areaWrapPanel);
                Grid.SetColumn(areaWrapPanel, 1);
                Grid.SetRow(areaWrapPanel, 0);

                WrapPanel districtWrapPanel = new WrapPanel();

                Label districtLabel = new Label();
                districtLabel.Style = (Style)FindResource("labelStyleBlack");
                districtLabel.Content = "District: ";

                Label districtLabelValue = new Label();
                districtLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                districtLabelValue.Content = emfPoints[i].district;
                districtLabelValue.ToolTip = "Left Click to Copy!";
                districtLabelValue.MouseLeftButtonDown += OnClickCoppyLabel;

                districtWrapPanel.Children.Add(districtLabel);
                districtWrapPanel.Children.Add(districtLabelValue);

                grid.Children.Add(districtWrapPanel);
                Grid.SetRow(districtWrapPanel, 0);
                Grid.SetColumn(districtWrapPanel, 2);


                WrapPanel latWrapPanel = new WrapPanel();

                Label latLabel = new Label();
                latLabel.Style = (Style)FindResource("labelStyleBlack");
                latLabel.Content = "Lat: ";

                Label latLabelValue = new Label();
                latLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                latLabelValue.Content = emfPoints[i].latitude;
                latLabelValue.MouseLeftButtonDown += OnClickCoppyLabel;
                latLabelValue.ToolTip = "Left Click to Copy!";
                latLabelValue.MouseLeftButtonDown += OnClickCoppyLabel;

                latWrapPanel.Children.Add(latLabel);
                latWrapPanel.Children.Add(latLabelValue);

                grid.Children.Add(latWrapPanel);
                Grid.SetRow(latWrapPanel, 1);
                Grid.SetColumn(latWrapPanel, 0);

                WrapPanel longWrapPanel = new WrapPanel();

                Label longLabel = new Label();
                longLabel.Style = (Style)FindResource("labelStyleBlack");
                longLabel.Content = "Long: ";

                Label longLabelValue = new Label();
                longLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                longLabelValue.Content = emfPoints[i].longitude;
                longLabelValue.ToolTip = "Left Click to Copy!";
                longLabelValue.MouseLeftButtonDown += OnClickCoppyLabel;

                longWrapPanel.Children.Add(longLabel);
                longWrapPanel.Children.Add(longLabelValue);

                grid.Children.Add(longWrapPanel);
                Grid.SetRow(longWrapPanel, 1);
                Grid.SetColumn(longWrapPanel, 1);

                if (emfPoints[i].actual_latitude != "")
                {
                    WrapPanel actualLatWrapPanel = new WrapPanel();

                    Label actualLatLabel = new Label();
                    actualLatLabel.Style = (Style)FindResource("labelStyleBlack");
                    actualLatLabel.Content = "Actual lat: ";

                    Label actualLatLabelValue = new Label();
                    actualLatLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                    actualLatLabelValue.Content = emfPoints[i].actual_latitude;
                    actualLatLabelValue.ToolTip = "Left Click to Copy!";
                    actualLatLabelValue.MouseLeftButtonDown += OnClickCoppyLabel;

                    actualLatWrapPanel.Children.Add(actualLatLabel);
                    actualLatWrapPanel.Children.Add(actualLatLabelValue);

                    grid.Children.Add(actualLatWrapPanel);
                    Grid.SetRow(actualLatWrapPanel, 2);
                    Grid.SetColumn(actualLatWrapPanel, 0);

                    WrapPanel actualLongWrapPanel = new WrapPanel();

                    Label actualLongLabel = new Label();
                    actualLongLabel.Style = (Style)FindResource("labelStyleBlack");
                    actualLongLabel.Content = "Actual long: ";

                    Label actualLongLabelValue = new Label();
                    actualLongLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                    actualLongLabelValue.Content = emfPoints[i].actual_longitude;
                    actualLongLabelValue.ToolTip = "Left Click to Copy!";
                    actualLongLabelValue.MouseLeftButtonDown += OnClickCoppyLabel;

                    actualLongWrapPanel.Children.Add(actualLongLabel);
                    actualLongWrapPanel.Children.Add(actualLongLabelValue);

                    grid.Children.Add(actualLongWrapPanel);
                    Grid.SetRow(actualLongWrapPanel, 2);
                    Grid.SetColumn(actualLongWrapPanel, 1);
                }

                Border statusBorder = new Border() { Margin = new Thickness(12)};
                statusBorder.Style = (Style)FindResource("statusBorderStyle");
                statusBorder.Margin = new Thickness(6);
                if (emfPoints[i].emf_status_id == BASIC_STRUCTS.OPEN_EMF_POINT_STATUS)
                    statusBorder.Background = Brushes.Red;
                else
                    statusBorder.Background = Brushes.Green;

                Label statusLabel = new Label();
                statusLabel.Style = (Style)FindResource("statusLabelStyle");
                statusLabel.Content = emfPoints[i].emf_status;

                statusBorder.Child = statusLabel;
                grid.Children.Add(statusBorder);
                Grid.SetRowSpan(statusBorder, 4);
                Grid.SetColumn(statusBorder, 3);

                Expander expander = new Expander();
                expander.HorizontalContentAlignment = HorizontalAlignment.Left;
                expander.VerticalAlignment = VerticalAlignment.Top;
                expander.ExpandDirection = ExpandDirection.Down;
                expander.Expanded += OnExpandExpander;
                expander.Margin = new Thickness(12);
                expander.Tag = i;

                StackPanel expanderstackPanel = new StackPanel();


                Button editButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                editButton.Content = "Edit";
                editButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                editButton.Click += OnClickEditPoint;

                expanderstackPanel.Children.Add(editButton);

                expander.Content = expanderstackPanel;

                grid.Children.Add(expander);
                Grid.SetRow(expander, 0);
                Grid.SetColumn(expander, 3);

                StackPanel bandStackPanel = new StackPanel();

                for(int j = 0; j < emfPoints[i].bands.Count; j++)
                {
                    Grid currentGrid = new Grid();
                    currentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300)});
                    currentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(400)});
                    currentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(400)});
                    currentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300)});


                    WrapPanel bandWrapPanel = new WrapPanel();

                    Label bandLabel = new Label();
                    bandLabel.Style = (Style)FindResource("labelStyleBlack");
                    bandLabel.Content = "Band: ";

                    Label bandLabelValue = new Label();
                    bandLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                    bandLabelValue.Content = emfPoints[i].bands[j].band_name;

                    bandWrapPanel.Children.Add(bandLabel);
                    bandWrapPanel.Children.Add(bandLabelValue);

                    currentGrid.Children.Add(bandWrapPanel);
                    Grid.SetColumn(bandWrapPanel, 0);


                    WrapPanel averageWrapPanel = new WrapPanel();

                    Label averageLabel = new Label();
                    averageLabel.Style = (Style)FindResource("wideLabelStyleBlack");
                    averageLabel.Content = "Average Power Density: ";

                    Label averageLabelValue = new Label();
                    averageLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                    averageLabelValue.Content = emfPoints[i].bands[j].average_power_density;

                    averageWrapPanel.Children.Add(averageLabel);
                    averageWrapPanel.Children.Add(averageLabelValue);

                    currentGrid.Children.Add(averageWrapPanel);
                    Grid.SetColumn(averageWrapPanel, 1);


                    WrapPanel maxWrapPanel = new WrapPanel();

                    Label maxLabel = new Label();
                    maxLabel.Style = (Style)FindResource("wideLabelStyleBlack");
                    maxLabel.Content = "Max Power Density: ";

                    Label maxLabelValue = new Label();
                    maxLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                    maxLabelValue.Content = emfPoints[i].bands[j].max_power_density;

                    maxWrapPanel.Children.Add(maxLabel);
                    maxWrapPanel.Children.Add(maxLabelValue);

                    currentGrid.Children.Add(maxWrapPanel);
                    Grid.SetColumn(maxWrapPanel, 2);

                    WrapPanel dateWrapPanel = new WrapPanel();

                    Label dateLabel = new Label();
                    dateLabel.Style = (Style)FindResource("labelStyleBlack");
                    dateLabel.Content = "Date: ";

                    Label dateLabelValue = new Label();
                    dateLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                    dateLabelValue.Content = emfPoints[i].date;

                    dateWrapPanel.Children.Add(dateLabel);
                    dateWrapPanel.Children.Add(dateLabelValue);

                    currentGrid.Children.Add(dateWrapPanel);
                    Grid.SetColumn(dateWrapPanel, 3);

                    bandStackPanel.Children.Add(currentGrid);
                }

                if(bandStackPanel.Children.Count != 0)
                {
                    grid.Children.Add(bandStackPanel);
                    Grid.SetRow(bandStackPanel, 3);
                    Grid.SetColumnSpan(bandStackPanel, 4);
                }

                border.Child = grid;

                pointsStackPanel.Children.Add(border);
            }
        }

        private void InitializeEMFPlansStackPanel()
        {
            BrushConverter brushConverter = new BrushConverter();

            plansStackPanel.Children.Clear();

            for (int i = 0; i < plans.Count; i++)
            {
                if (planStatusCheckBox.IsChecked == true && planStatusComboBox.SelectedIndex != -1)
                {
                    if (plans[i].status_id != planStatus[planStatusComboBox.SelectedIndex].key)
                        continue;
                }

                if (assignedEngineerCheckBox.IsChecked == true && assignedComboBox.SelectedIndex != -1)
                {
                    if (plans[i].assigned_engineer_id != engineers[assignedComboBox.SelectedIndex].key)
                        continue;
                }

                if (statusCheckBox.IsChecked == true && statusComboBox.SelectedIndex != -1)
                {
                    if (!plans[i].points.Exists(x1 => x1.emf_status_id == pointStatus[statusComboBox.SelectedIndex].key))
                        continue;
                }

                bool exists = true;

                for (int k = 0; k < plans[i].points.Count; k++)
                {
                    if (nameCheckBox.IsChecked == true && nameTextBox.Text != "")
                    {
                        string search = nameTextBox.Text;
                        bool contains = plans[i].points[k].name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (!contains)
                        {
                            exists = false;
                            break;
                        }
                    }
                    if (areaCheckBox.IsChecked == true && areaTextBox.Text != "")
                    {
                        string search = areaTextBox.Text;
                        bool contains = plans[i].points[k].area.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (!contains)
                        {
                            exists = false;
                            break;
                        }
                    }
                    if (districtCheckBox.IsChecked == true && districtTextBox.Text != "")
                    {
                        string search = districtTextBox.Text;
                        bool contains = plans[i].points[k].district.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (!contains)
                        {
                            exists = false;
                            break;
                        }
                    }
                    if (latCheckBox.IsChecked == true && latTextBox.Text != "")
                    {
                        string search = latTextBox.Text;
                        bool contains = plans[i].points[k].latitude.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (!contains)
                        {
                            exists = false;
                            break;
                        }
                    }
                    if (longCheckBox.IsChecked == true && longTextBox.Text != "")
                    {
                        string search = longTextBox.Text;
                        bool contains = plans[i].points[k].longitude.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (!contains)
                        {
                            exists = false;
                            break;
                        }
                    }
                    if (actualLatCheckBox.IsChecked == true && actualLatTextBox.Text != "")
                    {
                        string search = actualLatTextBox.Text;
                        bool contains = plans[i].points[k].actual_latitude.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (!contains)
                        {
                            exists = false;
                            break;
                        }
                    }
                    if (actualLongCheckBox.IsChecked == true && actualLongTextBox.Text != "")
                    {
                        string search = actualLongTextBox.Text;
                        bool contains = plans[i].points[k].actual_longitude.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (!contains)
                        {
                            exists = false;
                            break;
                        }
                    }
                }

                if (!exists)
                    continue;

                Border border = new Border() { BorderThickness = new Thickness(3) };
                border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");

                Grid grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() );

                WrapPanel planNameWrapPanel = new WrapPanel();

                Label planNameLabel = new Label();
                planNameLabel.Style = (Style)FindResource("labelStyleBlack");
                planNameLabel.Content = "Name: ";

                Label planNameLabelValue = new Label();
                planNameLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                planNameLabelValue.Content = plans[i].plan_name;
                planNameLabelValue.ToolTip = "Left Click to Copy!";
                planNameLabelValue.MouseLeftButtonDown += OnClickCoppyLabel;

                planNameWrapPanel.Children.Add(planNameLabel);
                planNameWrapPanel.Children.Add(planNameLabelValue);

                grid.Children.Add(planNameWrapPanel);
                Grid.SetColumn(planNameWrapPanel, 0);
                Grid.SetRow(planNameWrapPanel, 0);

                
                WrapPanel deadlinDateWrapPanel = new WrapPanel();

                Label deadlinDateLabel = new Label();
                deadlinDateLabel.Style = (Style)FindResource("labelStyleBlack");
                deadlinDateLabel.Content = "Deadline: ";

                Label deadlinDateLabelValue = new Label();
                deadlinDateLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                deadlinDateLabelValue.Content = plans[i].deadline_date.Day + "-" + plans[i].deadline_date.Month + "-" + plans[i].deadline_date.Year;
                deadlinDateLabelValue.ToolTip = "Left Click to Copy!";
                deadlinDateLabelValue.MouseLeftButtonDown += OnClickCoppyLabel;

                deadlinDateWrapPanel.Children.Add(deadlinDateLabel);
                deadlinDateWrapPanel.Children.Add(deadlinDateLabelValue);

                grid.Children.Add(deadlinDateWrapPanel);
                Grid.SetRow(deadlinDateWrapPanel, 0);
                Grid.SetColumn(deadlinDateWrapPanel, 1);

                
                WrapPanel assignedWrapPanel = new WrapPanel();

                Label assignedLabel = new Label();
                assignedLabel.Style = (Style)FindResource("labelStyleBlack");
                assignedLabel.Content = "Assigned: ";

                Label assignedLabelValue = new Label();
                assignedLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                assignedLabelValue.Content = plans[i].assigned_engineer;
                assignedLabelValue.ToolTip = "Left Click to Copy!";
                assignedLabelValue.MouseLeftButtonDown += OnClickCoppyLabel;

                assignedWrapPanel.Children.Add(assignedLabel);
                assignedWrapPanel.Children.Add(assignedLabelValue);

                grid.Children.Add(assignedWrapPanel);
                Grid.SetColumn(assignedWrapPanel, 0);
                Grid.SetRow(assignedWrapPanel, 1);


                Border statusBorder = new Border();
                statusBorder.Style = (Style)FindResource("statusBorderStyle");
                statusBorder.Margin = new Thickness(6);
                if (plans[i].status_id == BASIC_STRUCTS.OPEN_EMF_POINT_STATUS)
                    statusBorder.Background = Brushes.Red;
                else if (plans[i].status_id == BASIC_STRUCTS.PENDING_EMF_PLAN_STATUS)
                    statusBorder.Background = Brushes.Yellow;
                else if (plans[i].status_id == BASIC_STRUCTS.CLOSED_EMF_PLAN_STATUS)
                    statusBorder.Background = Brushes.Green;
                else
                    statusBorder.Background = Brushes.Orange;

                Label statusLabel = new Label();
                statusLabel.Style = (Style)FindResource("statusLabelStyle");
                statusLabel.Content = plans[i].status;

                statusBorder.Child = statusLabel;
                grid.Children.Add(statusBorder);
                Grid.SetRowSpan(statusBorder, 2);
                Grid.SetColumn(statusBorder, 2);

                Expander expander = new Expander();
                expander.HorizontalContentAlignment = HorizontalAlignment.Left;
                expander.VerticalAlignment = VerticalAlignment.Top;
                expander.ExpandDirection = ExpandDirection.Down;
                expander.Expanded += OnExpandExpander;
                expander.Margin = new Thickness(12);
                expander.Tag = i;

                StackPanel expanderstackPanel = new StackPanel();

                Button viewButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                viewButton.Content = "View";
                viewButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                viewButton.Click += OnClickViewPlan;

                Button editButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                editButton.Content = "Edit";
                editButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                editButton.Click += OnClickEditPlan;

                expanderstackPanel.Children.Add(viewButton);
                expanderstackPanel.Children.Add(editButton);

                expander.Content = expanderstackPanel;

                //grid.Children.Add(expander);
                //Grid.SetRow(expander, 0);
                //Grid.SetColumn(expander, 3);

                StackPanel pointsStackPanel = new StackPanel() { Margin = new Thickness(12)};

                for (int j = 0; j < plans[i].points.Count; j++)
                {
                    if(j == 0)
                    {
                        Label headerLabel = new Label() { Style = (Style)FindResource("stackPanelSecondaryHeaderLabelStyle"), Content = "EMF POINTS", Margin = new Thickness(0)};
                        pointsStackPanel.Children.Add(headerLabel);
                    }

                    Grid currentGrid = new Grid();
                    currentGrid.RowDefinitions.Add(new RowDefinition());
                    currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    currentGrid.ColumnDefinitions.Add(new ColumnDefinition());


                    WrapPanel pointNameWrapPanel = new WrapPanel();

                    Label pointNameLabel = new Label();
                    pointNameLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                    pointNameLabel.Content = "Point name: ";

                    Label pointNameLabelValue = new Label();
                    pointNameLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                    pointNameLabelValue.Content = plans[i].points[j].name;

                    pointNameWrapPanel.Children.Add(pointNameLabel);
                    pointNameWrapPanel.Children.Add(pointNameLabelValue);

                    currentGrid.Children.Add(pointNameWrapPanel);
                    Grid.SetColumn(pointNameWrapPanel, 0);


                    WrapPanel pointAreaWrapPanel = new WrapPanel();

                    Label pointAreaLabel = new Label();
                    pointAreaLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                    pointAreaLabel.Content = "Point area: ";

                    Label pointAreaLabelValue = new Label();
                    pointAreaLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                    pointAreaLabelValue.Content = plans[i].points[j].area;

                    pointAreaWrapPanel.Children.Add(pointAreaLabel);
                    pointAreaWrapPanel.Children.Add(pointAreaLabelValue);

                    currentGrid.Children.Add(pointAreaWrapPanel);
                    Grid.SetColumn(pointAreaWrapPanel, 1);


                    WrapPanel pointDistrictWrapPanel = new WrapPanel();

                    Label pointDistrictLabel = new Label();
                    pointDistrictLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                    pointDistrictLabel.Content = "Point district: ";

                    Label pointDistrictLabelValue = new Label();
                    pointDistrictLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                    pointDistrictLabelValue.Content = plans[i].points[j].district;

                    pointDistrictWrapPanel.Children.Add(pointDistrictLabel);
                    pointDistrictWrapPanel.Children.Add(pointDistrictLabelValue);

                    currentGrid.Children.Add(pointDistrictWrapPanel);
                    Grid.SetColumn(pointDistrictWrapPanel, 2);

                    pointsStackPanel.Children.Add(currentGrid);
                }

                if (pointsStackPanel.Children.Count != 0)
                {
                    grid.Children.Add(pointsStackPanel);
                    Grid.SetRow(pointsStackPanel, 2);
                    Grid.SetColumnSpan(pointsStackPanel, 3);
                }

                border.Child = grid;

                plansStackPanel.Children.Add(border);
            }
        }

        

        private void OnClickCoppyLabel(object sender, MouseButtonEventArgs e)
        {
            Label currentLabel = (Label)sender;
            Clipboard.SetText(currentLabel.Content.ToString());

            Label label = new Label();
            label.Style = (Style)FindResource("labelStyle");
            label.Content = @"Copied!";
            label.Width = 100;

            popUp.Child = label;

            popUp.IsOpen = true;

            dispatchertimer.Start();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            popUp.IsOpen = false;
            dispatchertimer.IsEnabled = false;

        }

        private void OnExpandExpander(object sender, RoutedEventArgs e)
        {
            previousExpander = currentExpander;
            currentExpander = (Expander)sender;

            if (previousExpander != null && previousExpander != currentExpander)
                previousExpander.IsExpanded = false;
        }

        private void OnCheckAssignedCheckBox(object sender, RoutedEventArgs e)
        {
            assignedComboBox.IsEnabled = true;
            assignedComboBox.SelectedIndex = engineers.FindIndex(x1 => x1.key == loggedInUser.GetEmployeeId());
        }

        private void OnUncheckAssignedCheckBox(object sender, RoutedEventArgs e)
        {
            assignedComboBox.SelectedIndex = -1;
            assignedComboBox.IsEnabled = false;
        }

        private void OnCheckPlanStatusCheckBox(object sender, RoutedEventArgs e)
        {
            planStatusComboBox.IsEnabled = true;
            planStatusComboBox.SelectedIndex = 0;
        }

        private void OnUncheckPlanStatusCheckBox(object sender, RoutedEventArgs e)
        {
            planStatusComboBox.SelectedIndex = -1;
            planStatusComboBox.IsEnabled = false;
        }

        private void OnCheckNameCheckBox(object sender, RoutedEventArgs e)
        {
            nameTextBox.IsEnabled = true;
        }

        private void OnUncheckNameCheckBox(object sender, RoutedEventArgs e)
        {
            nameTextBox.Text = "";
            nameTextBox.IsEnabled = false;
        }

        private void OnCheckAreaCheckBox(object sender, RoutedEventArgs e)
        {
            areaTextBox.IsEnabled = true;
        }

        private void OnUncheckAreaCheckBox(object sender, RoutedEventArgs e)
        {
            areaTextBox.Text = "";
            areaTextBox.IsEnabled = false;
        }

        private void OnCheckDistrictCheckBox(object sender, RoutedEventArgs e)
        {
            districtTextBox.IsEnabled = true;
        }

        private void OnUncheckDistrictCheckBox(object sender, RoutedEventArgs e)
        {
            districtTextBox.Text = "";
            districtTextBox.IsEnabled = false;
        }

        private void OnCheckLatCheckBox(object sender, RoutedEventArgs e)
        {
            latTextBox.IsEnabled = true;
        }

        private void OnUncheckLatCheckBox(object sender, RoutedEventArgs e)
        {
            latTextBox.Text = "";
            latTextBox.IsEnabled = false;
        }

        private void OnCheckLongCheckBox(object sender, RoutedEventArgs e)
        {
            longTextBox.IsEnabled = true;
        }

        private void OnUncheckLongCheckBox(object sender, RoutedEventArgs e)
        {
            longTextBox.Text = "";
            longTextBox.IsEnabled = false;
        }

        private void OnCheckactualLatCheckBoxCheckBox(object sender, RoutedEventArgs e)
        {
            actualLatTextBox.IsEnabled = true;
        }

        private void OnUncheckactualLatCheckBoxCheckBox(object sender, RoutedEventArgs e)
        {
            actualLatTextBox.Text = "";
            actualLatTextBox.IsEnabled = false;
        }

        private void OnCheckActualLongCheckBox(object sender, RoutedEventArgs e)
        {
            actualLongTextBox.IsEnabled = true;
        }

        private void OnUncheckActualLongCheckBox(object sender, RoutedEventArgs e)
        {
            actualLongTextBox.Text = "";
            actualLongTextBox.IsEnabled = false;
        }

        //private void OnCheckAvgCheckBox(object sender, RoutedEventArgs e)
        //{
        //    averageTextBox.IsEnabled = true;
        //}
        //
        //private void OnUncheckAvgCheckBox(object sender, RoutedEventArgs e)
        //{
        //    averageTextBox.Text = "";
        //    averageTextBox.IsEnabled = false;
        //}
        //
        //private void OnCheckMaxCheckBox(object sender, RoutedEventArgs e)
        //{
        //    maxTextBox.IsEnabled = true;
        //}
        //
        //private void OnUncheckMaxCheckBox(object sender, RoutedEventArgs e)
        //{
        //    maxTextBox.Text = "";
        //    maxTextBox.IsEnabled = false;
        //}

        private void OnTextChangedSearchTextBox(object sender, TextChangedEventArgs e)
        {
            InitializeEMFPointsStackPanel();
            InitializeEMFPlansStackPanel();
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

        private void OnSelChangedStatusComboBox(object sender, SelectionChangedEventArgs e)
        {
            InitializeEMFPointsStackPanel();
            InitializeEMFPlansStackPanel();
        }

        private void OnSelChangedAssignedComboBox(object sender, SelectionChangedEventArgs e)
        {
            InitializeEMFPointsStackPanel();
            InitializeEMFPlansStackPanel();
        }

        private void OnSelChangedPlanStatusComboBox(object sender, SelectionChangedEventArgs e)
        {
            InitializeEMFPointsStackPanel();
            InitializeEMFPlansStackPanel();
        }

        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            int viewAddCondition = BASIC_STRUCTS.EMF_ADD_CONDITION;

            EMF emfPoint = new EMF();

            EMFWindow emfWindow = new EMFWindow(ref loggedInUser, ref viewAddCondition, ref emfPoint);
            emfWindow.Closed += OnClosedPlanWindow;
            emfWindow.Show();
        }

        private void OnClosedPlanWindow(object sender, EventArgs e)
        {
            if (!commonQueries.GetEMFPlans(ref plans))
                return;
            if (!commonQueries.GetEMFPoints(ref emfPoints))
                return;

            InitializeEMFPointsStackPanel();
            InitializeEMFPlansStackPanel();
        }
        private void OnClickEditPoint(object sender, RoutedEventArgs e)
        {
            int viewAddCondition = BASIC_STRUCTS.EMF_EDIT_CONDITION;

            EMF emf = new EMF();

            if (!emf.InitializePoint(emfPoints[int.Parse(currentExpander.Tag.ToString())].emf_serial))
                return;

            EMFWindow emfWindow = new EMFWindow(ref loggedInUser, ref viewAddCondition, ref emf);
            emfWindow.Closed += OnClosedPlanWindow;
            emfWindow.Show();

        }

        private void OnBtnClickAddPlan(object sender, RoutedEventArgs e)
        {
            int viewAddCondition = BASIC_STRUCTS.EMF_PLAN_ADD_CONDITION;

            EMFPlan plan = new EMFPlan();

            EMFPlanWindow emfPlanWindow = new EMFPlanWindow(ref loggedInUser, ref plan, ref viewAddCondition);
            emfPlanWindow.Closed += OnClosedPlanWindow;
            emfPlanWindow.Show();
        }

        private void OnClickEditPlan(object sender, RoutedEventArgs e)
        {
            int viewAddCondition = BASIC_STRUCTS.EMF_PLAN_EDIT_CONDITION;

            EMFPlan plan = new EMFPlan();

            String tag = currentExpander.Tag.ToString();

            if (!plan.InitializePlan(plans[int.Parse(currentExpander.Tag.ToString())].plan_serial))
                return;

            EMFPlanWindow emfPlanWindow = new EMFPlanWindow(ref loggedInUser, ref plan, ref viewAddCondition);
            emfPlanWindow.Closed += OnClosedPlanWindow;
            emfPlanWindow.Show();
        }

        private void OnClickViewPlan(object sender, RoutedEventArgs e)
        {
            int viewAddCondition = BASIC_STRUCTS.EMF_PLAN_VIEW_CONDITION;

            EMFPlan plan = new EMFPlan();

            if (!plan.InitializePlan(plans[int.Parse(currentExpander.Tag.ToString())].plan_serial))
                return;

            EMFPlanWindow emfPlanWindow = new EMFPlanWindow(ref loggedInUser, ref plan, ref viewAddCondition);
            emfPlanWindow.Closed += OnClosedPlanWindow;
            emfPlanWindow.Show();
        }
    }
}
