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
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> engineers;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> serviceProviders;

        private Expander currentExpander;
        private Expander previousExpander;
        
        public MissionsPage(ref Employee mLoggedInUser)
        {
            commonQueries = new CommonQueries();

            missions = new List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT>();
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
            missionsStackPanel.Orientation = Orientation.Vertical;
            missionsStackPanel.Children.Clear();
            
            for(int i = 0; i < missions.Count; i++)
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
                    if (missions[i].mission_Date.Year != int.Parse(yearComboBox.SelectedItem.ToString()))
                        continue;
                }
        
                if (monthCheckBox.IsChecked == true && monthComboBox.SelectedIndex != -1)
                {
                    if (missions[i].mission_Date.Month != int.Parse(monthComboBox.SelectedItem.ToString()))
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
        
        
                Grid missionGrid = new Grid();
                missionGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                missionGrid.VerticalAlignment = VerticalAlignment.Stretch;
        
                missionGrid.RowDefinitions.Add(new RowDefinition());
                missionGrid.RowDefinitions.Add(new RowDefinition());
                missionGrid.RowDefinitions.Add(new RowDefinition());
                missionGrid.RowDefinitions.Add(new RowDefinition());
                missionGrid.RowDefinitions.Add(new RowDefinition());
        
                missionGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300)});
                missionGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(500)});
                missionGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300)});
        
                BrushConverter brushConverter = new BrushConverter();
        
                Border border = new Border() { BorderThickness = new Thickness(3)};
                border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");
        
                //WrapPanel logoWrapPanel = new WrapPanel();
                //
                //String imageSource = @"\Photos\" + missions[i].service_provider_id.ToString() + ".png";
                //
                //Image logo = new Image();
                //logo.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));
                //
                //logo.Width = 80;
                //logo.Height = 80;
        
                Label missionIdLabel = new Label();
                missionIdLabel.Content = missions[i].mission_id;
                missionIdLabel.Style = (Style)FindResource("stackPanelHeaderLabelStyle");
                missionIdLabel.Width = 500;
        
                //logoWrapPanel.Children.Add(logo);
                //logoWrapPanel.Children.Add(missionIdLabel);
        
                missionGrid.Children.Add(missionIdLabel);
                Grid.SetRow(missionIdLabel, 0);
                Grid.SetColumnSpan(missionIdLabel, 3);

                StackPanel engineersStackPanel = new StackPanel();
                engineersStackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                engineersStackPanel.VerticalAlignment = VerticalAlignment.Stretch;

                for (int j = 0; j < missions[i].engineers.Count; j++)
                {
                    WrapPanel wrapPanel = new WrapPanel();
                    wrapPanel.HorizontalAlignment = HorizontalAlignment.Center;

                    Label tempEngineerLabel = new Label();
                    tempEngineerLabel.Content = "Eng." + (j + 1) + " :";
                    tempEngineerLabel.Style = (Style)FindResource("labelStyleBlack");

                    Label tempNameLabel = new Label();
                    tempNameLabel.Style = (Style)FindResource("mediumLabelStyle");
                    tempNameLabel.Content = missions[i].engineers[j].value;

                    wrapPanel.Children.Add(tempEngineerLabel);
                    wrapPanel.Children.Add(tempNameLabel);

                    engineersStackPanel.Children.Add(wrapPanel);
                }

                missionGrid.Children.Add(engineersStackPanel);
                Grid.SetRow(engineersStackPanel, 1);
                Grid.SetColumn(engineersStackPanel, 0);

                ScrollViewer sitesScrollViewer = new ScrollViewer();
                sitesScrollViewer.Height = 150;
                //sitesScrollViewer.Margin = new Thickness(12);
                sitesScrollViewer.HorizontalAlignment = HorizontalAlignment.Center;
                sitesScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                sitesScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

                Grid sitesGrid= new Grid();
                sitesGrid.ShowGridLines = true;
                sitesGrid.RowDefinitions.Add(new RowDefinition());
                sitesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200)});
                sitesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300)});
                sitesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300)});
                sitesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100)});

                Label siteNumberLabelHeader = new Label();
                siteNumberLabelHeader.Content = "Site Number";
                siteNumberLabelHeader.Style = (Style)FindResource("wideLabelStyle");
                siteNumberLabelHeader.HorizontalContentAlignment = HorizontalAlignment.Center; ;

                Label reasonOfIntLabelHeader = new Label();
                reasonOfIntLabelHeader.Content = "Reason of Int.";
                reasonOfIntLabelHeader.Style = (Style)FindResource("wideLabelStyle");
                reasonOfIntLabelHeader.HorizontalContentAlignment = HorizontalAlignment.Center;

                Label commentLabelHeader = new Label();
                commentLabelHeader.Content = "Comment";
                commentLabelHeader.Style = (Style)FindResource("wideLabelStyle");
                commentLabelHeader.HorizontalContentAlignment = HorizontalAlignment.Center;

                sitesGrid.Children.Add(siteNumberLabelHeader);
                sitesGrid.Children.Add(reasonOfIntLabelHeader);
                sitesGrid.Children.Add(commentLabelHeader);
                Grid.SetColumn(siteNumberLabelHeader, 0);
                Grid.SetColumn(reasonOfIntLabelHeader, 1);
                Grid.SetColumn(commentLabelHeader, 2);

                for (int j = 0; j < missions[i].sites.Count; j++)
                {
                    Label tempSiteNumberLabel = new Label();
                    tempSiteNumberLabel.Content = missions[i].sites[j].site_name;
                    tempSiteNumberLabel.Style = (Style)FindResource("labelStyleBlack");
                    
                    TextBox tempReasonOfIntTextBlock = new TextBox();
                    tempReasonOfIntTextBlock.Text = missions[i].sites[j].reason_of_interference;
                    tempReasonOfIntTextBlock.Style = (Style)FindResource("stackPanelTextboxStyle");

                    TextBox commentTextBlock = new TextBox();
                    commentTextBlock.Text = missions[i].sites[j].comment;
                    commentTextBlock.Style = (Style)FindResource("stackPanelTextboxStyle");

                    Button showSiteButton = new Button();
                    showSiteButton.Content = "View";
                    showSiteButton.Style = (Style)FindResource("buttonStyle");
                    showSiteButton.Width = 50;
                    showSiteButton.Height = 30;
                    showSiteButton.FontSize = 14;
                    showSiteButton.Tag = missions[i].company_serial + "," + missions[i].complaint_serial + "," + missions[i].sites[j].site_serial;
                    showSiteButton.Click += OnClickShowSite;

                    sitesGrid.RowDefinitions.Add(new RowDefinition());

                    sitesGrid.Children.Add(tempSiteNumberLabel);
                    sitesGrid.Children.Add(tempReasonOfIntTextBlock);
                    sitesGrid.Children.Add(commentTextBlock);
                    sitesGrid.Children.Add(showSiteButton);
                    
                    Grid.SetRow(tempSiteNumberLabel, j + 1);
                    Grid.SetColumn(tempSiteNumberLabel, 0);
                    
                    Grid.SetRow(tempReasonOfIntTextBlock, j + 1);
                    Grid.SetColumn(tempReasonOfIntTextBlock, 1);

                    Grid.SetRow(commentTextBlock, j + 1);
                    Grid.SetColumn(commentTextBlock, 2);

                    Grid.SetRow(showSiteButton, j + 1);
                    Grid.SetColumn(showSiteButton, 3);

                }

                sitesScrollViewer.Content = sitesGrid;
                missionGrid.Children.Add(sitesScrollViewer);
                Grid.SetRow(sitesScrollViewer, 2);
                Grid.SetColumn(sitesScrollViewer, 0);
                Grid.SetColumnSpan(sitesScrollViewer, 2);


                //Grid equipmentGrid = new Grid();
                //equipmentGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                //equipmentGrid.RowDefinitions.Add(new RowDefinition());
                //
                //ScrollViewer equipmentScrollViewer = new ScrollViewer();
                //equipmentScrollViewer.HorizontalAlignment = HorizontalAlignment.Center;
                //equipmentScrollViewer.Margin = new Thickness(12);
                //equipmentScrollViewer.Height = 150;
                //equipmentScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                //
                //equipmentScrollViewer.Content = equipmentGrid;
                //
                //Label equipmentHeaderLabel = new Label();
                //equipmentHeaderLabel.Content = "Equipment";
                //equipmentHeaderLabel.Style = (Style)FindResource("wideLabelStyle");
                //equipmentHeaderLabel.HorizontalAlignment = HorizontalAlignment.Center;
                //
                //equipmentGrid.Children.Add(equipmentHeaderLabel);
                //
                //for (int j = 0; j < missions[i].equipment.Count; j++)
                //{
                //    Label tempEquipmentLabel = new Label();
                //    tempEquipmentLabel.Content = missions[i].equipment[j];
                //    tempEquipmentLabel.Style = (Style)FindResource("wideLabelStyleBlack");
                //
                //    equipmentGrid.RowDefinitions.Add(new RowDefinition());
                //    equipmentGrid.Children.Add(tempEquipmentLabel);
                //    Grid.SetRow(tempEquipmentLabel, j + 1);
                //}


                //missionGrid.Children.Add(equipmentScrollViewer);
                //Grid.SetRow(equipmentScrollViewer, 2);
                //Grid.SetColumn(equipmentScrollViewer, 0);

                WrapPanel vehichleWrapPanel = new WrapPanel();
                vehichleWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
                vehichleWrapPanel.VerticalAlignment = VerticalAlignment.Center;

                Label vehicleLabel = new Label();
                vehicleLabel.Content = "Vehicle: ";
                vehicleLabel.Style = (Style)FindResource("labelStyleBlack");

                Label vehichleValueLabel = new Label();
                vehichleValueLabel.Content = missions[i].vehichle;
                vehichleValueLabel.Style = (Style)FindResource("mediumLabelStyle");

                vehichleWrapPanel.Children.Add(vehicleLabel);
                vehichleWrapPanel.Children.Add(vehichleValueLabel);

                missionGrid.Children.Add(vehichleWrapPanel);
                Grid.SetRow(vehichleWrapPanel, 1);
                Grid.SetColumn(vehichleWrapPanel, 1);

                Border statusBorder = new Border();
                statusBorder.Style = (Style)FindResource("statusBorderStyle");
                statusBorder.Width = 200;
                if (missions[i].status_id == BASIC_STRUCTS.MISSION_PENDING_OPERATOR_ACTION_STATUS || missions[i].status_id == BASIC_STRUCTS.MISSION_PENDING_NTRA_ACTION_STATUS)
                    statusBorder.Background = Brushes.Yellow;
                else
                    statusBorder.Background = Brushes.Green;

                Label statusLabel = new Label();
                statusLabel.Content = missions[i].status;
                statusLabel.Style = (Style)FindResource("statusLabelStyle");
                statusLabel.Width = 200;

                statusBorder.Child = statusLabel;

                missionGrid.Children.Add(statusBorder);
                Grid.SetRow(statusBorder, 1);
                Grid.SetColumn(statusBorder, 2);

                WrapPanel dateWrapPanel = new WrapPanel();
                dateWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
                dateWrapPanel.VerticalAlignment = VerticalAlignment.Center;

                Label dateLabel = new Label();
                dateLabel.Style = (Style)FindResource("labelStyleBlack");
                dateLabel.Content = "Date: ";

                Label dateLabelValue = new Label();
                dateLabelValue.Content = missions[i].mission_Date.Day + "/" + missions[i].mission_Date.Month + "/" + missions[i].mission_Date.Year;
                dateLabelValue.Style = (Style)FindResource("labelStyle");

                dateWrapPanel.Children.Add(dateLabel);
                dateWrapPanel.Children.Add(dateLabelValue);

                missionGrid.Children.Add(dateWrapPanel);
                Grid.SetRow(dateWrapPanel, 2);
                Grid.SetColumn(dateWrapPanel, 2);
        
                Expander expander = new Expander();
                expander.Tag = missions[i].company_serial + "," + missions[i].complaint_serial + "," + missions[i].mission_serial;
                expander.HorizontalContentAlignment = HorizontalAlignment.Left;
                expander.VerticalAlignment = VerticalAlignment.Top;
                expander.ExpandDirection = ExpandDirection.Down;
                expander.Expanded += OnExpandExpander;
                expander.Margin = new Thickness(0, 24, 0, 0);
        
                StackPanel expanderStackPanel = new StackPanel();
        
                Button viewButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                viewButton.Content = "View Mission";
                viewButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                viewButton.Click += OnBtnClickView;

                Button viewComplaintButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                viewComplaintButton.Content = "View Complaint";
                viewComplaintButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                viewComplaintButton.Tag = missions[i].company_serial + "," + missions[i].complaint_serial;
                viewComplaintButton.Click += OnClickViewComplaint;


                Button editButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                editButton.Content = "Edit";
                editButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                editButton.Click += OnClickEdit;
        
        
                Button deleteButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                deleteButton.Content = "Delete";
                deleteButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                deleteButton.Click += OnBtnClickDelete;
        
                expanderStackPanel.Children.Add(viewButton);
                expanderStackPanel.Children.Add(viewComplaintButton);
                expanderStackPanel.Children.Add(editButton);
                //expanderStackPanel.Children.Add(deleteButton);
                
                expander.Content = expanderStackPanel;
        
                missionGrid.Children.Add(expander);
                Grid.SetColumn(expander, 2);
        
                border.Child = missionGrid;
        
                missionsStackPanel.Children.Add(border);
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
    }
}
