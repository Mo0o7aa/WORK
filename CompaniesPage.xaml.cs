using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for CompaniesPage.xaml
    /// </summary>
    public partial class CompaniesPage : Page
    {
        private Employee loggedInUser;

        CommonQueries commonQueries;

        private List<BASIC_STRUCTS.COMPANY_STRUCT> companies;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> workFields;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> bands;
        private List<BASIC_STRUCTS.BAND_UNIT_STRUCT> bandUnits;

        public CompaniesPage(ref Employee mLoggedInUser)
        {
            commonQueries = new CommonQueries();

            loggedInUser = mLoggedInUser;

            companies = new List<BASIC_STRUCTS.COMPANY_STRUCT>();
            bands = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            bandUnits = new List<BASIC_STRUCTS.BAND_UNIT_STRUCT>();

            InitializeComponent();

            if (!commonQueries.GetCompanies(ref companies))
                return;

            if (!commonQueries.GetBands(ref bands))
                return;

            if (!commonQueries.GetBandUnits(ref bandUnits))
                return;

            for(int i = 0; i < bandUnits.Count; i++)
            {
                bandsComboBox.Items.Add(bandUnits[i].unit);
            }

            workFields = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            userNameLabel.Content = "Username: " + loggedInUser.GetEmployeeUserName();

            InitializeWorkFieldCombo();

            InitializeCompaniesStackPanel();
            InitializeCompaniesGrid();
        }

        private bool InitializeWorkFieldCombo()
        {
            if (!commonQueries.GetCompanyFieldOfWork(ref workFields))
                return false;

            for (int i = 0; i < workFields.Count; i++)
            {
                fieldComboBox.Items.Add(workFields[i].value);
            }

            return true;
        }

        private void InitializeCompaniesStackPanel()
        {
            companiesStackPanel.Children.Clear();

            for (int i = 0; i < companies.Count; i++)
            {
                if (searchCheckBox.IsChecked == true && searchTextBox.Text != "")
                {
                    string search = searchTextBox.Text;
                    bool contains = companies[i].company_name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }

                if (fieldOfWorkCheckBox.IsChecked == true && fieldComboBox.SelectedIndex != -1)
                {
                    if (companies[i].company_field_of_work_id != workFields[fieldComboBox.SelectedIndex].key)
                        continue;
                }

                if (bandsCheckBox.IsChecked == true && bandsComboBox.SelectedIndex != -1 && bandTextBox.Text != "")
                {
                    bool band = false;

                    for (int j = 0; j < companies[i].cf_and_bw_frequencies.Count; j++)
                    {
                        if ((long.Parse(bandTextBox.Text.ToString()) * long.Parse(bandUnits[bandsComboBox.SelectedIndex].factor.ToString())) <= companies[i].cf_and_bw_frequencies[j].max && (long.Parse(bandTextBox.Text.ToString()) * long.Parse(bandUnits[bandsComboBox.SelectedIndex].factor.ToString())) >= companies[i].cf_and_bw_frequencies[j].min)
                        {
                            band = true;
                            break;
                        }
                    }

                    for (int j = 0; j < companies[i].start_and_stop_frequencies.Count; j++)
                    {
                        if ((long.Parse(bandTextBox.Text.ToString()) * long.Parse(bandUnits[bandsComboBox.SelectedIndex].factor.ToString())) <= companies[i].start_and_stop_frequencies[j].max && (long.Parse(bandTextBox.Text.ToString()) * long.Parse(bandUnits[bandsComboBox.SelectedIndex].factor.ToString())) >= companies[i].start_and_stop_frequencies[j].min)
                        {
                            band = true;
                            break;
                        }
                    }

                    if (band == false)
                        continue;
                }

                BrushConverter brushConverter = new BrushConverter();

                Border border = new Border() { BorderThickness = new Thickness(3) };
                border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");

                Grid tempGrid = new Grid();
                tempGrid.ColumnDefinitions.Add(new ColumnDefinition());
                tempGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });

                border.Child = tempGrid;

                Expander expander = new Expander();
                expander.Tag = companies[i].company_serial;
                expander.Margin = new Thickness(12);
                expander.VerticalAlignment = VerticalAlignment.Top;
                expander.HorizontalAlignment = HorizontalAlignment.Left;

                StackPanel expanderStackPanel = new StackPanel();

                Button viewButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                viewButton.Content = "View";
                viewButton.Click += OnClickViewCompany;

                Button editButton = new Button() { Style = (Style)FindResource("expanderButtonStyle") };
                editButton.Content = "Edit";
                editButton.Click += OnClickEditCompany;

                expanderStackPanel.Children.Add(viewButton);
                expanderStackPanel.Children.Add(editButton);

                expander.Content = expanderStackPanel;

                tempGrid.Children.Add(expander);
                Grid.SetColumn(expander, 1);

                StackPanel tempStackPanel = new StackPanel();

                tempGrid.Children.Add(tempStackPanel);
                Grid.SetColumn(tempStackPanel, 0);

                Label companyNameLabel = new Label();
                companyNameLabel.Style = (Style)FindResource("stackPanelHeaderLabelStyle");
                companyNameLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
                companyNameLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                //companyNameLabel.Foreground = Brushes.White;
                //companyNameLabel.Background = (Brush)brushConverter.ConvertFrom("#000080");
                companyNameLabel.Margin = new Thickness(12);
                companyNameLabel.Content = companies[i].company_name;

                tempStackPanel.Children.Add(companyNameLabel);

                WrapPanel workFieldWrapPanel = new WrapPanel();

                Label workFieldLabelTitle = new Label();
                workFieldLabelTitle.Content = "Work field: ";
                workFieldLabelTitle.Style = (Style)FindResource("wideStackPanelLabelStyleTitle");

                Label workFieldLabel = new Label();
                workFieldLabel.Style = (Style)FindResource("wideStackPanelLabelStyle");
                workFieldLabel.Content = companies[i].company_field_of_work;

                workFieldWrapPanel.Children.Add(workFieldLabelTitle);
                workFieldWrapPanel.Children.Add(workFieldLabel);

                tempStackPanel.Children.Add(workFieldWrapPanel);

                for(int j = 0; j < companies[i].cf_and_bw_frequencies.Count; j++)
                {
                    Grid tempCFGrid = new Grid() { HorizontalAlignment = HorizontalAlignment.Center};
                    tempCFGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    tempCFGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    tempCFGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    tempCFGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    Label cfLabel = new Label() { Style = (Style)FindResource("labelStyle")};
                    cfLabel.Content = "Centre Freq.: ";

                    Label cfLabelValue = new Label() { Style = (Style)FindResource("mediumLabelStyleBlack") };
                    cfLabelValue.Content = companies[i].cf_and_bw_frequencies[j].centre_frequency + companies[i].cf_and_bw_frequencies[j].centre_frequency_unit;


                    Label bwLabel = new Label() { Style = (Style)FindResource("labelStyle") };
                    bwLabel.Content = "Bandwidth: ";

                    Label bwLabelValue = new Label() { Style = (Style)FindResource("mediumLabelStyleBlack") };
                    bwLabelValue.Content = companies[i].cf_and_bw_frequencies[j].bandwidth + companies[i].cf_and_bw_frequencies[j].bandwidth_unit;

                    tempCFGrid.Children.Add(cfLabel);
                    Grid.SetColumn(cfLabel, 0);

                    tempCFGrid.Children.Add(cfLabelValue);
                    Grid.SetColumn(cfLabelValue, 1);

                    tempCFGrid.Children.Add(bwLabel);
                    Grid.SetColumn(bwLabel, 2);

                    tempCFGrid.Children.Add(bwLabelValue);
                    Grid.SetColumn(bwLabelValue, 3);

                    tempStackPanel.Children.Add(tempCFGrid);
                }

                for (int j = 0; j < companies[i].start_and_stop_frequencies.Count; j++)
                {
                    Grid tempStartGrid = new Grid() { HorizontalAlignment = HorizontalAlignment.Center };
                    tempStartGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    tempStartGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    tempStartGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    tempStartGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    Label cfLabel = new Label() { Style = (Style)FindResource("labelStyle") };
                    cfLabel.Content = "Start Freq.: ";

                    Label cfLabelValue = new Label() { Style = (Style)FindResource("mediumLabelStyleBlack") };
                    cfLabelValue.Content = companies[i].start_and_stop_frequencies[j].start_frequency + " " + companies[i].start_and_stop_frequencies[j].start_frequency_unit;


                    Label bwLabel = new Label() { Style = (Style)FindResource("labelStyle") };
                    bwLabel.Content = "Stop Freq.: ";

                    Label bwLabelValue = new Label() { Style = (Style)FindResource("mediumLabelStyleBlack") };
                    bwLabelValue.Content = companies[i].start_and_stop_frequencies[j].stop_frequency + " " + companies[i].start_and_stop_frequencies[j].stop_frequency_unit;

                    tempStartGrid.Children.Add(cfLabel);
                    Grid.SetColumn(cfLabel, 0);

                    tempStartGrid.Children.Add(cfLabelValue);
                    Grid.SetColumn(cfLabelValue, 1);

                    tempStartGrid.Children.Add(bwLabel);
                    Grid.SetColumn(bwLabel, 2);

                    tempStartGrid.Children.Add(bwLabelValue);
                    Grid.SetColumn(bwLabelValue, 3);

                    tempStackPanel.Children.Add(tempStartGrid);
                }

                Grid contactsGrid = new Grid();

                for (int j = 0; j < companies[i].contacts.Count; j++)
                {
                    contactsGrid.RowDefinitions.Add(new RowDefinition());

                    Grid tempContactGrid = new Grid();
                    tempContactGrid.RowDefinitions.Add(new RowDefinition());

                    tempContactGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    tempContactGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    tempContactGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    //Label contactHeaderLabel = new Label();
                    //contactHeaderLabel.Style = (Style)FindResource("stackPanelHeaderLabelStyle");
                    //contactHeaderLabel.Content = "Contact " + (j + 1);
                    //contactHeaderLabel.Background = (Brush)brushConverter.ConvertFrom("#000080");
                    //contactHeaderLabel.Foreground = Brushes.White;
                    //contactHeaderLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                    //
                    //tempContactGrid.Children.Add(contactHeaderLabel);
                    //Grid.SetRow(contactHeaderLabel, 0);
                    //Grid.SetColumnSpan(contactHeaderLabel, 3);

                    StackPanel nameStackPanel = new StackPanel();
                    nameStackPanel.Orientation = Orientation.Vertical;

                    Label contactNameLabelTitle = new Label();
                    contactNameLabelTitle.Content = "Name: ";
                    contactNameLabelTitle.Style = (Style)FindResource("wideStackPanelLabelStyleTitle");

                    Label contactNameLabel = new Label();
                    contactNameLabel.Style = (Style)FindResource("stackPanelLabelStyle");
                    contactNameLabel.Content = companies[i].contacts[j].contact_name;

                    nameStackPanel.Children.Add(contactNameLabelTitle);
                    nameStackPanel.Children.Add(contactNameLabel);

                    tempContactGrid.Children.Add(nameStackPanel);
                    Grid.SetRow(nameStackPanel, 0);
                    Grid.SetColumn(nameStackPanel, 0);

                    StackPanel numberStackPanel = new StackPanel();
                    numberStackPanel.Orientation = Orientation.Vertical;

                    Label contactNumberLabelTitle = new Label();
                    contactNumberLabelTitle.Content = "Number: ";
                    contactNumberLabelTitle.Style = (Style)FindResource("wideStackPanelLabelStyleTitle");

                    Label contactNumberLabel = new Label();
                    contactNumberLabel.Style = (Style)FindResource("stackPanelLabelStyle");
                    contactNumberLabel.Content = companies[i].contacts[j].contact_phone;

                    numberStackPanel.Children.Add(contactNumberLabelTitle);
                    numberStackPanel.Children.Add(contactNumberLabel);

                    tempContactGrid.Children.Add(numberStackPanel);
                    Grid.SetRow(numberStackPanel, 0);
                    Grid.SetColumn(numberStackPanel, 1);

                    StackPanel emailStackPanel = new StackPanel();
                    emailStackPanel.Orientation = Orientation.Vertical;

                    Label contactEmailLabelTitle = new Label();
                    contactEmailLabelTitle.Content = "Email: ";
                    contactEmailLabelTitle.Style = (Style)FindResource("wideStackPanelLabelStyleTitle");

                    Label contactEmailLabel = new Label();
                    contactEmailLabel.Style = (Style)FindResource("wideStackPanelLabelStyle");
                    contactEmailLabel.Content = companies[i].contacts[j].contact_email;

                    emailStackPanel.Children.Add(contactEmailLabelTitle);
                    emailStackPanel.Children.Add(contactEmailLabel);

                    tempContactGrid.Children.Add(emailStackPanel);
                    Grid.SetRow(emailStackPanel, 0);
                    Grid.SetColumn(emailStackPanel, 2);

                    contactsGrid.Children.Add(tempContactGrid);
                    Grid.SetRow(tempContactGrid, contactsGrid.RowDefinitions.Count - 1);
                }

                tempStackPanel.Children.Add(contactsGrid);

                companiesStackPanel.Children.Add(border);
            }
        }

        private void InitializeCompaniesGrid()
        {
            companiesGrid.Children.Clear();
            companiesGrid.RowDefinitions.Clear();

            companiesGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(70)});

            BrushConverter brushConverter = new BrushConverter();

            Grid tempHeaderGrid = new Grid();
            tempHeaderGrid.ShowGridLines = true;

            tempHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition());
            tempHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(){ Width = new GridLength(300) });
            tempHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(){ Width = new GridLength(700) });
            tempHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(){ Width = new GridLength(700) });
            tempHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(){ Width = new GridLength(700) });

            ///////////HEADER//////

            Label companyNameHeader = new Label();
            companyNameHeader.Style = (Style)FindResource("stackPanelHeaderLabelStyle");
            companyNameHeader.Content = "Company Name";
            companyNameHeader.Foreground = Brushes.White;
            companyNameHeader.Background = (Brush)brushConverter.ConvertFrom("#000080");
            companyNameHeader.HorizontalAlignment = HorizontalAlignment.Stretch;
            companyNameHeader.HorizontalContentAlignment = HorizontalAlignment.Center;

            Label workFieldHeader = new Label();
            workFieldHeader.Style = (Style)FindResource("stackPanelHeaderLabelStyle");
            workFieldHeader.Content = "Workfield";
            workFieldHeader.Foreground = Brushes.White;
            workFieldHeader.Background = (Brush)brushConverter.ConvertFrom("#000080");
            workFieldHeader.HorizontalAlignment = HorizontalAlignment.Stretch;
            workFieldHeader.HorizontalContentAlignment = HorizontalAlignment.Center;


            Label contact1Header = new Label();
            contact1Header.Style = (Style)FindResource("stackPanelHeaderLabelStyle");
            contact1Header.Content = "Contact 1";
            contact1Header.Foreground = Brushes.White;
            contact1Header.Background = (Brush)brushConverter.ConvertFrom("#000080");
            contact1Header.Width = 700;
            contact1Header.HorizontalContentAlignment = HorizontalAlignment.Center;

            Label contact2Header = new Label();
            contact2Header.Style = (Style)FindResource("stackPanelHeaderLabelStyle");
            contact2Header.Content = "Contact 2";
            contact2Header.Foreground = Brushes.White;
            contact2Header.Background = (Brush)brushConverter.ConvertFrom("#000080");
            contact2Header.Width = 700;
            contact2Header.HorizontalContentAlignment = HorizontalAlignment.Center;

            Label contact3Header = new Label();
            contact3Header.Style = (Style)FindResource("stackPanelHeaderLabelStyle");
            contact3Header.Content = "Contact 3";
            contact3Header.Foreground = Brushes.White;
            contact3Header.Background = (Brush)brushConverter.ConvertFrom("#000080");
            contact3Header.Width = 700;
            contact3Header.HorizontalContentAlignment = HorizontalAlignment.Center;

            tempHeaderGrid.Children.Add(companyNameHeader);
            Grid.SetColumn(companyNameHeader, 0);

            tempHeaderGrid.Children.Add(workFieldHeader);
            Grid.SetColumn(workFieldHeader, 1);

            tempHeaderGrid.Children.Add(contact1Header);
            Grid.SetColumn(contact1Header, 2);

            tempHeaderGrid.Children.Add(contact2Header);
            Grid.SetColumn(contact2Header, 3);

            tempHeaderGrid.Children.Add(contact3Header);
            Grid.SetColumn(contact3Header, 4);


            companiesGrid.Children.Add(tempHeaderGrid);
            Grid.SetRow(tempHeaderGrid, 0);

            for (int i = 0; i < companies.Count; i++)
            {
                if (searchCheckBox.IsChecked == true && searchTextBox.Text != "")
                {
                    string search = searchTextBox.Text;
                    bool contains = companies[i].company_name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }

                if (fieldOfWorkCheckBox.IsChecked == true && fieldComboBox.SelectedIndex != -1)
                {
                    if (companies[i].company_field_of_work_id != workFields[fieldComboBox.SelectedIndex].key)
                        continue;
                }

                companiesGrid.RowDefinitions.Add(new RowDefinition());

                Grid tempCompaniesGrid = new Grid();
                tempCompaniesGrid.ShowGridLines = true;

                tempCompaniesGrid.ColumnDefinitions.Add(new ColumnDefinition());
                tempCompaniesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });
                tempCompaniesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(700) });
                tempCompaniesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(700) });
                tempCompaniesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(700) });


                Label companyNameLabel = new Label();
                companyNameLabel.MouseLeftButtonDown += CopyLabelContent;
                companyNameLabel.ToolTip = "Copy";
                companyNameLabel.Style = (Style)FindResource("mediumLabelStyle");
                companyNameLabel.Content = companies[i].company_name;
                companyNameLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                companyNameLabel.HorizontalAlignment = HorizontalAlignment.Center;

                tempCompaniesGrid.Children.Add(companyNameLabel);
                Grid.SetColumn(companyNameLabel, 0);

                Label workFieldLabel = new Label();
                workFieldLabel.MouseLeftButtonDown += CopyLabelContent;
                workFieldLabel.ToolTip = "Copy";
                workFieldLabel.Style = (Style)FindResource("wideLabelStyle");
                workFieldLabel.Content = companies[i].company_field_of_work;
                workFieldLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                workFieldLabel.HorizontalAlignment = HorizontalAlignment.Center;

                tempCompaniesGrid.Children.Add(workFieldLabel);
                Grid.SetColumn(workFieldLabel, 1);

                for (int j = 0; j < companies[i].contacts.Count; j++)
                {
                    Grid contactsGrid = new Grid();
                    contactsGrid.ShowGridLines = true;

                    contactsGrid.RowDefinitions.Add(new RowDefinition());
                    contactsGrid.RowDefinitions.Add(new RowDefinition());

                    contactsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200)});
                    contactsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200)});
                    contactsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300)});

                    ///HEADER///
                    Label contactNameHeader = new Label();
                    contactNameHeader.Style = (Style)FindResource("stackPanelSecondaryHeaderLabelStyle");
                    contactNameHeader.Content = "Name";
                    contactNameHeader.Width = 200;

                    Label contactNumberHeader = new Label();
                    contactNumberHeader.Style = (Style)FindResource("stackPanelSecondaryHeaderLabelStyle");
                    contactNumberHeader.Content = "Number";
                    contactNumberHeader.Width = 200;


                    Label contactEmailHeader = new Label();
                    contactEmailHeader.Style = (Style)FindResource("stackPanelSecondaryHeaderLabelStyle");
                    contactEmailHeader.Content = "Email";
                    contactEmailHeader.Width = 300;

                    contactsGrid.Children.Add(contactNameHeader);
                    Grid.SetColumn(contactNameHeader, 0);
                    Grid.SetRow(contactNameHeader, 0);

                    contactsGrid.Children.Add(contactNumberHeader);
                    Grid.SetColumn(contactNumberHeader, 1);
                    Grid.SetRow(contactNumberHeader, 0);

                    contactsGrid.Children.Add(contactEmailHeader);
                    Grid.SetColumn(contactEmailHeader, 2);
                    Grid.SetRow(contactEmailHeader, 0);


                    Label contactNameLabel = new Label();
                    contactNameLabel.Style = (Style)FindResource("wideLabelStyle");
                    contactNameLabel.Content = companies[i].contacts[j].contact_name;
                    contactNameLabel.HorizontalContentAlignment = HorizontalAlignment.Center;

                    contactsGrid.Children.Add(contactNameLabel);
                    Grid.SetColumn(contactNameLabel, 0);
                    Grid.SetRow(contactNameLabel, 1);

                    Label contactNumberLabel = new Label();
                    contactNumberLabel.Style = (Style)FindResource("wideLabelStyle");
                    contactNumberLabel.Content = companies[i].contacts[j].contact_phone;
                    contactNumberLabel.HorizontalContentAlignment = HorizontalAlignment.Center;

                    contactsGrid.Children.Add(contactNumberLabel);
                    Grid.SetColumn(contactNumberLabel, 1);
                    Grid.SetRow(contactNumberLabel, 1);

                    Label contactEmailLabel = new Label();
                    contactEmailLabel.Style = (Style)FindResource("wideLabelStyle");
                    contactEmailLabel.Content = companies[i].contacts[j].contact_email;
                    contactEmailLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                    contactEmailLabel.Width = 300;

                    contactsGrid.Children.Add(contactEmailLabel);
                    Grid.SetColumn(contactEmailLabel, 2);
                    Grid.SetRow(contactEmailLabel, 1);


                    tempCompaniesGrid.Children.Add(contactsGrid);
                    Grid.SetColumn(contactsGrid, j + 2);
                }

                companiesGrid.Children.Add(tempCompaniesGrid);
                Grid.SetRow(tempCompaniesGrid, companiesGrid.RowDefinitions.Count - 1);
            }
        }

        private void CopyLabelContent(object sender, MouseButtonEventArgs e)
        {
            Label currentLabel = (Label)sender;
            Clipboard.SetText(currentLabel.Content.ToString());
        }

        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            Company company = new Company();

            int companyCondition = BASIC_STRUCTS.COMPANY_ADD_CONDITION;

            CompanyWindow companyWindow = new CompanyWindow(ref loggedInUser, ref company, ref companyCondition);
            companyWindow.Closed += OnClosedCompanyWindow;
            companyWindow.Show();
        }

        private void OnClickEditCompany(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel parentStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)parentStackPanel.Parent;

            Company company = new Company();

            if (!company.InitializeCompanyInfo(int.Parse(currentExpander.Tag.ToString())))
                return;

            int companyCondition = BASIC_STRUCTS.COMPANY_EDIT_CONDITION;

            CompanyWindow companyWindow = new CompanyWindow(ref loggedInUser, ref company, ref companyCondition);
            companyWindow.Closed += OnClosedCompanyWindow;
            companyWindow.Show();
        }


        private void OnClickViewCompany(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel parentStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)parentStackPanel.Parent;

            Company company = new Company();

            if (!company.InitializeCompanyInfo(int.Parse(currentExpander.Tag.ToString())))
                return;

            int companyCondition = BASIC_STRUCTS.COMPANY_VIEW_CONDITION;

            CompanyWindow companyWindow = new CompanyWindow(ref loggedInUser, ref company, ref companyCondition);
            companyWindow.Show();
        }

        private void OnClosedCompanyWindow(object sender, EventArgs e)
        {
            if (!commonQueries.GetCompanies(ref companies))
                return;

            InitializeCompaniesStackPanel();
            InitializeCompaniesGrid();
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

        private void MonitoringMenuSelection(object sender, RoutedEventArgs e)
        {
            MissionsPage missionsPage = new MissionsPage(ref loggedInUser);
            NavigationService.Navigate(missionsPage);
        }

        private void InspectionMenuSelection(object sender, RoutedEventArgs e)
        {

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

        private void OnCheckFieldCheckBox(object sender, RoutedEventArgs e)
        {
            fieldComboBox.IsEnabled = true;
            fieldComboBox.SelectedIndex = 1;
        }

        private void OnUncheckFieldCheckBox(object sender, RoutedEventArgs e)
        {
            fieldComboBox.SelectedIndex = -1;
            fieldComboBox.IsEnabled = false;
        }

        private void OnTextChangedSearchTextBox(object sender, TextChangedEventArgs e)
        {
            InitializeCompaniesStackPanel();
            InitializeCompaniesGrid();
        }

        private void OnSelChangedFieldCombo(object sender, SelectionChangedEventArgs e)
        {
            InitializeCompaniesStackPanel();
            InitializeCompaniesGrid();
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

        private void OnCheckBandsCheckBox(object sender, RoutedEventArgs e)
        {
            bandTextBox.IsEnabled = true;
            bandsComboBox.IsEnabled = true;
        }

        private void OnUncheckBandsCheckBox(object sender, RoutedEventArgs e)
        {
            bandTextBox.Text = "";
            bandsComboBox.SelectedIndex = -1;

            bandTextBox.IsEnabled = false;
            bandsComboBox.IsEnabled = false;

            InitializeCompaniesStackPanel();
            InitializeCompaniesGrid();
        }

        private void OnSelChangedBandsCombo(object sender, SelectionChangedEventArgs e)
        {
            if(bandTextBox.Text != "")
            {
                InitializeCompaniesStackPanel();
                InitializeCompaniesGrid();
            }
        }

        private void OnTextChangedBandTextBox(object sender, TextChangedEventArgs e)
        {
            if (bandsComboBox.SelectedIndex != -1)
            {
                InitializeCompaniesStackPanel();
                InitializeCompaniesGrid();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[A-Za-z]+");
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}

