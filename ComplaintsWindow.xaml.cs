using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for ComplaintsWindow.xaml
    /// </summary>
    public partial class ComplaintsWindow : Window
    {
        private Employee loggedInUser;
        private Complaints complaint;
        private CommonQueries commonQueries;

        private SQLServer sql;

        private Popup popUp;

        private Company selectedCompany;

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> bands;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> bandsUnits;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> cities;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> complaintStatus;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> siteStatus;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> companies;
        private List<BASIC_STRUCTS.MIN_SITE_STRUCT> companySites;

        private int viewAddCondition;

        public ComplaintsWindow(ref Employee mLoggedInUser, ref Complaints mComplaint, ref int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            complaint = mComplaint;
            viewAddCondition = mViewAddCondition;

            commonQueries = new CommonQueries();
            sql = new SQLServer();
            popUp = new Popup();
            popUp.PopupAnimation = PopupAnimation.Fade;
            popUp.PlacementTarget = this;
            popUp.Placement = PlacementMode.Center;

            selectedCompany = new Company();

            bands = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            bandsUnits = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            cities = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            complaintStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            siteStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            companies = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            companySites = new List<BASIC_STRUCTS.MIN_SITE_STRUCT>();

            InitializeComponent();

            InitializeCompaniesComboBox();

            GetBands();
            GetBandUnits();
            GetCities();
            GetComplaintStatus();
            GetSiteStatus();
            InitializeComplaintStatusCombo(ref statusComboBox);

            InitializeSiteStatusCombo(ref filterStatusComboBox);
            filterStatusComboBox.SelectedIndex = -1;
            filterStatusComboBox.IsEnabled = true;
            
            InitializeSiteStatusCombo(ref siteStatusComboBox);
            InitializeSharedCombo(ref sharedComboBox);
            InitializeRRUCombo(ref rruComboBox);

            this.PreviewKeyDown += OnKeyBoardKeyDownMissionWindow;
            this.MouseLeftButtonDown += OnMouseClickMissionWindow;

            FillBandsGrid(ref sector1BandsGrid);

            if(viewAddCondition != BASIC_STRUCTS.COMPLAINT_ADD_CONDITION)
            {
                sitesGrid.Children.Clear();
                sitesGrid.RowDefinitions.Clear();

                SetUIValues();

                if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                    InitializeSitesGrid();
                else
                {
                    sitesGrid.Visibility = Visibility.Collapsed;
                    addSiteButton.IsEnabled = false;
                }
                companyNameComboBox.IsEnabled = false;

                if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                {
                    SetUIValuesBands();
                    DisableUIElements();
                }
            }
            else
            {
                filtersBorder.Visibility = Visibility.Collapsed;
                DateTime todaysDate = DateTime.Now;
                dateAddedDatePicker.SelectedDate = todaysDate;
            }

            if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_EDIT_CONDITION)
            {
                filtersBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void DisableUIElements()
        {
            companyNameComboBox.IsEnabled = false;
            dateAddedDatePicker.IsEnabled = false;
            ticketNumberTextBox.IsReadOnly = true;
            regionComboBox.IsEnabled = false;
            regionTextBox.IsReadOnly = true;
            statusComboBox.IsEditable = false;

            addSiteButton.IsEnabled = false;
            finishButton.IsEnabled = false;
        }

        private void SetUIValues()
        {
            companyNameComboBox.SelectedIndex = companies.FindIndex(x1 => x1.key == complaint.GetCompanySerial());
            dateAddedDatePicker.SelectedDate = complaint.GetComplaintDate();
            ticketNumberTextBox.Text = complaint.GetTicketNumber();
            regionComboBox.SelectedIndex = cities.FindIndex(x1 => x1.key == complaint.GetCityId());
            regionTextBox.Text = complaint.GetRegion();
            statusComboBox.SelectedIndex =complaintStatus.FindIndex(x1 => x1.key == complaint.GetComplaintStatusId());
        }

        private void SetUIValuesBands()
        {
            int rowNumber = 0;

            sector1BandsGrid.Children.Clear();

            //for (int i = 0; i < complaint.GetComplaintsBands().Count; i++)
            //{
            //    CheckBox checkBox = new CheckBox();
            //    checkBox.Style = (Style)FindResource("checkBoxStyle2");
            //    //checkBox.Width = 250;
            //    checkBox.HorizontalAlignment = HorizontalAlignment.Center;
            //    checkBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            //    checkBox.Content = complaint.GetComplaintsBands()[i];
            //    checkBox.IsChecked = true;
            //
            //    sector1BandsGrid.Children.Add(checkBox);
            //    Grid.SetRow(checkBox, rowNumber);
            //    //Grid.SetColumn(checkBox, columnNumber);
            //
            //    sector1BandsGrid.RowDefinitions.Add(new RowDefinition());
            //    rowNumber++;
            //
            //}
        }

        private void GetCompanySites()
        {
            if (!commonQueries.GetCompanySites(ref companySites, companies[companyNameComboBox.SelectedIndex].key))
                return;
        }

        private void InitializeSitesGrid()
        {
            sitesGrid.Children.Clear();
            sitesGrid.RowDefinitions.Clear();

            for (int i = 0; i < complaint.sites.Count; i++)
            {
                BrushConverter brushConverter = new BrushConverter();

                Site currentSite = new Site();

                if (!currentSite.InitializeSite(complaint.GetCompanySerial(), complaint.sites[i].site_serial))
                {
                    MessageBox.Show("Server connection failed, check your internet connection!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (searchTextBox.Text != "")
                {
                    bool contains = currentSite.GetSiteNumber().IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }

                if (filtersCheckBox.IsChecked == true)
                {
                    if (filterCityCombo.SelectedIndex != -1)
                    {
                        if (filterCityCombo.SelectedIndex != -1)
                        {
                            if (currentSite.GetCityId() != cities[filterCityCombo.SelectedIndex].key)
                                continue;
                        }
                    }

                    if (filterRegionTextBox.Text != "")
                    {
                        bool contains = currentSite.GetRegion().IndexOf(filterRegionTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (!contains)
                            continue;
                    }

                    if (filterStatusComboBox.SelectedIndex != -1)
                    {
                        if (complaint.sites[i].site_status_id != siteStatus[filterStatusComboBox.SelectedIndex].key)
                            continue;
                    }

                    if (filterLatTextBox.Text != "")
                    {
                        bool contains = currentSite.GetLat().IndexOf(filterLatTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (!contains)
                            continue;
                    }

                    if (filterLongTextBox.Text != "")
                    {
                        bool contains = currentSite.GetLong().IndexOf(filterLongTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (!contains)
                            continue;
                    }

                }

                Border border = new Border();

                border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");
                border.CornerRadius = new CornerRadius(20);
                border.BorderThickness = new Thickness(3);
                border.Margin = new Thickness(12);
                border.VerticalAlignment = VerticalAlignment.Top;

                Grid grid = new Grid();

                border.Child = grid;

                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
                grid.RowDefinitions.Add(new RowDefinition());

                Label siteLabelTitle = new Label();
                siteLabelTitle.HorizontalAlignment = HorizontalAlignment.Center;
                siteLabelTitle.HorizontalContentAlignment = HorizontalAlignment.Center;
                siteLabelTitle.Style = (Style)FindResource("headerLabelStyle");
                siteLabelTitle.Background = (Brush)brushConverter.ConvertFrom("#000080");
                siteLabelTitle.Width = 300;
                siteLabelTitle.FontSize = 20;
                siteLabelTitle.FontWeight = FontWeights.Bold;
                siteLabelTitle.Content = "Site";

                grid.Children.Add(siteLabelTitle);
                Grid.SetRow(siteLabelTitle, 0);
                Grid.SetColumnSpan(siteLabelTitle, 2);



                WrapPanel siteNumberWrapPanel = new WrapPanel();
                siteNumberWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
                siteNumberWrapPanel.VerticalAlignment = VerticalAlignment.Center;

                Label siteNumberLabel = new Label();
                siteNumberLabel.Style = (Style)FindResource("labelStyleBlack");
                siteNumberLabel.Content = "Site No: ";

                ComboBox currentSiteNumberComboBox = new ComboBox();
                currentSiteNumberComboBox.Margin = new Thickness(12, 0, 0, 0);
                currentSiteNumberComboBox.Style = (Style)FindResource("comboBoxStyle");
                InitializeSiteNumberCombo(ref currentSiteNumberComboBox);
                currentSiteNumberComboBox.Text = currentSite.GetSiteNumber();
                if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                    currentSiteNumberComboBox.IsEnabled = false;

                siteNumberWrapPanel.Children.Add(siteNumberLabel);
                siteNumberWrapPanel.Children.Add(currentSiteNumberComboBox);

                grid.Children.Add(siteNumberWrapPanel);
                Grid.SetRow(siteNumberWrapPanel, 1);
                Grid.SetColumn(siteNumberWrapPanel, 0);



                WrapPanel siteStatusWrapPanel = new WrapPanel();
                siteStatusWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
                siteStatusWrapPanel.VerticalAlignment = VerticalAlignment.Center;

                Label siteStatusLabel = new Label();
                siteStatusLabel.Style = (Style)FindResource("labelStyleBlack");
                siteStatusLabel.Content = "Status: ";

                ComboBox currentSiteStatusComboBox = new ComboBox();
                currentSiteStatusComboBox.Margin = new Thickness(12, 0, 0, 0);
                currentSiteStatusComboBox.Style = (Style)FindResource("comboBoxStyle");
                InitializeSiteStatusCombo(ref currentSiteStatusComboBox);
                currentSiteStatusComboBox.SelectedItem = complaint.sites[i].site_status;
                if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                    currentSiteStatusComboBox.IsEnabled = false;

                siteStatusWrapPanel.Children.Add(siteStatusLabel);
                siteStatusWrapPanel.Children.Add(currentSiteStatusComboBox);

                grid.Children.Add(siteStatusWrapPanel);
                Grid.SetRow(siteStatusWrapPanel, 1);
                Grid.SetColumn(siteStatusWrapPanel, 1);



                WrapPanel cityWrapPanel = new WrapPanel();
                cityWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
                cityWrapPanel.VerticalAlignment = VerticalAlignment.Center;

                Label cityLabel = new Label();
                cityLabel.Style = (Style)FindResource("labelStyleBlack");
                cityLabel.Content = "City: ";

                ComboBox currentCityComboBox = new ComboBox();
                currentCityComboBox.Margin = new Thickness(12, 0, 0, 0);
                currentCityComboBox.Style = (Style)FindResource("comboBoxStyle");
                InitializeCityCombo(ref currentCityComboBox);
                currentCityComboBox.SelectedIndex = cities.FindIndex(x1 => x1.key == currentSite.GetCityId());
                if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                    currentCityComboBox.IsEnabled = false;

                cityWrapPanel.Children.Add(cityLabel);
                cityWrapPanel.Children.Add(currentCityComboBox);

                grid.Children.Add(cityWrapPanel);
                Grid.SetRow(cityWrapPanel, 2);
                Grid.SetColumn(cityWrapPanel, 0);


                WrapPanel regionWrapPanel = new WrapPanel();
                regionWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
                regionWrapPanel.VerticalAlignment = VerticalAlignment.Center;

                Label regionLabel = new Label();
                regionLabel.Style = (Style)FindResource("labelStyleBlack");
                regionLabel.Content = "Region: ";

                TextBox currentRegionTextBox = new TextBox();
                currentRegionTextBox.Margin = new Thickness(12, 0, 0, 0);
                currentRegionTextBox.Style = (Style)FindResource("textboxStyle");
                currentRegionTextBox.Text = currentSite.GetRegion();
                if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                    currentRegionTextBox.IsReadOnly = true;

                regionWrapPanel.Children.Add(regionLabel);
                regionWrapPanel.Children.Add(currentRegionTextBox);

                grid.Children.Add(regionWrapPanel);
                Grid.SetRow(regionWrapPanel, 2);
                Grid.SetColumn(regionWrapPanel, 1);



                WrapPanel latWrapPanel = new WrapPanel();
                latWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
                latWrapPanel.VerticalAlignment = VerticalAlignment.Center;

                Label latLabel = new Label();
                latLabel.Style = (Style)FindResource("labelStyleBlack");
                latLabel.Content = "Lat: ";

                TextBox currentLatTextBox = new TextBox();
                currentLatTextBox.Margin = new Thickness(12, 0, 0, 0);
                currentLatTextBox.Style = (Style)FindResource("textboxStyle");
                currentLatTextBox.PreviewTextInput += NumberValidationTextBox;
                currentLatTextBox.Text = currentSite.GetLat();
                if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                    currentLatTextBox.IsReadOnly = true;

                latWrapPanel.Children.Add(latLabel);
                latWrapPanel.Children.Add(currentLatTextBox);

                grid.Children.Add(latWrapPanel);
                Grid.SetRow(latWrapPanel, 3);
                Grid.SetColumn(latWrapPanel, 0);

                WrapPanel longWrapPanel = new WrapPanel();
                longWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
                longWrapPanel.VerticalAlignment = VerticalAlignment.Center;

                Label longLabel = new Label();
                longLabel.Style = (Style)FindResource("labelStyleBlack");
                longLabel.Content = "Long: ";

                TextBox currentLongTextBox = new TextBox();
                currentLongTextBox.Margin = new Thickness(12, 0, 0, 0);
                currentLongTextBox.Style = (Style)FindResource("textboxStyle");
                currentLongTextBox.PreviewTextInput += NumberValidationTextBox;
                currentLongTextBox.Text = currentSite.GetLong();
                if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                    currentLongTextBox.IsReadOnly = true;

                longWrapPanel.Children.Add(longLabel);
                longWrapPanel.Children.Add(currentLongTextBox);

                grid.Children.Add(longWrapPanel);
                Grid.SetRow(longWrapPanel, 3);
                Grid.SetColumn(longWrapPanel, 1);

                if (selectedCompany.GetCompanyFieldOfWorkId() == BASIC_STRUCTS.MOBILE_OPERATOR)
                {
                    for (int j = 0; j < complaint.sites[i].sectors.Count; j++)
                    {
                        if (j != 0)
                            grid.RowDefinitions.Add(new RowDefinition());

                        Grid sectorGrid = new Grid();
                        sectorGrid.Margin = new Thickness(12);

                        sectorGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        sectorGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        sectorGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

                        sectorGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
                        sectorGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
                        sectorGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
                        sectorGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
                        sectorGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
                        sectorGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });

                        WrapPanel sectorWrappanel = new WrapPanel();
                        sectorWrappanel.VerticalAlignment = VerticalAlignment.Center;
                        sectorWrappanel.HorizontalAlignment = HorizontalAlignment.Center;

                        Label sectorLabel = new Label();
                        sectorLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                        sectorLabel.Content = "Sector No: ";

                        TextBox currentSectorTextBox = new TextBox();
                        currentSectorTextBox.Style = (Style)FindResource("miniTextboxStyle");
                        currentSectorTextBox.Margin = new Thickness(12, 0, 0, 0);
                        currentSectorTextBox.Text = complaint.sites[i].sectors[j].sector_number;
                        if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                            currentSectorTextBox.IsReadOnly = true;
                        if (currentSectorTextBox.Text == "")
                            sectorLabel.Foreground = Brushes.Red;

                        sectorWrappanel.Children.Add(sectorLabel);
                        sectorWrappanel.Children.Add(currentSectorTextBox);

                        sectorGrid.Children.Add(sectorWrappanel);
                        Grid.SetRow(sectorWrappanel, 0);
                        Grid.SetColumn(sectorWrappanel, 0);

                        WrapPanel rruWrappanel = new WrapPanel();
                        rruWrappanel.VerticalAlignment = VerticalAlignment.Center;
                        rruWrappanel.HorizontalAlignment = HorizontalAlignment.Center;

                        Label rruLabel = new Label();
                        rruLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                        rruLabel.Content = "RRU: ";

                        ComboBox currentrruComboBox = new ComboBox();
                        currentrruComboBox.Style = (Style)FindResource("miniComboBoxStyle");
                        currentrruComboBox.Margin = new Thickness(12, 0, 0, 0);
                        InitializeRRUCombo(ref currentrruComboBox);
                        currentrruComboBox.SelectedItem = complaint.sites[i].sectors[j].rru;
                        if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                            currentrruComboBox.IsEnabled = false;

                        rruWrappanel.Children.Add(rruLabel);
                        rruWrappanel.Children.Add(currentrruComboBox);

                        sectorGrid.Children.Add(rruWrappanel);
                        Grid.SetRow(rruWrappanel, 1);
                        Grid.SetColumn(rruWrappanel, 0);

                        WrapPanel sharedWrappanel = new WrapPanel();
                        sharedWrappanel.VerticalAlignment = VerticalAlignment.Center;
                        sharedWrappanel.HorizontalAlignment = HorizontalAlignment.Center;

                        Label sharedLabel = new Label();
                        sharedLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                        sharedLabel.Content = "Shared: ";

                        ComboBox currentSharedComboBox = new ComboBox();
                        currentSharedComboBox.Style = (Style)FindResource("miniComboBoxStyle");
                        currentSharedComboBox.Margin = new Thickness(12, 0, 0, 0);
                        InitializeSharedCombo(ref currentSharedComboBox);
                        if (complaint.sites[i].sectors[j].shared_with_id != 0)
                            currentSharedComboBox.SelectedItem = companies.Find(x1 => x1.key == complaint.sites[i].sectors[j].shared_with_id).value;
                        if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                            currentSharedComboBox.IsEnabled = false;
                        if (currentSharedComboBox.SelectedIndex == -1)
                        {
                            currentSharedComboBox.SelectedItem = "No";
                        }

                        sharedWrappanel.Children.Add(sharedLabel);
                        sharedWrappanel.Children.Add(currentSharedComboBox);

                        sectorGrid.Children.Add(sharedWrappanel);
                        Grid.SetRow(sharedWrappanel, 2);
                        Grid.SetColumn(sharedWrappanel, 0);

                        WrapPanel currentAzimuthWrappanel = new WrapPanel();
                        currentAzimuthWrappanel.VerticalAlignment = VerticalAlignment.Center;
                        currentAzimuthWrappanel.HorizontalAlignment = HorizontalAlignment.Center;

                        Label currentAzimuthLabel = new Label();
                        currentAzimuthLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                        currentAzimuthLabel.Content = "Azimuth";

                        TextBox currentAzimuthTextBox = new TextBox();
                        currentAzimuthTextBox.Style = (Style)FindResource("miniTextboxStyle");
                        currentAzimuthTextBox.Margin = new Thickness(12, 0, 0, 0);
                        currentAzimuthTextBox.Text = complaint.sites[i].sectors[j].azimuth.ToString();
                        if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                            currentAzimuthTextBox.IsEnabled = false;

                        currentAzimuthWrappanel.Children.Add(currentAzimuthLabel);
                        currentAzimuthWrappanel.Children.Add(currentAzimuthTextBox);

                        sectorGrid.Children.Add(currentAzimuthWrappanel);
                        Grid.SetRow(currentAzimuthWrappanel, 3);
                        Grid.SetColumn(currentAzimuthWrappanel, 0);

                        WrapPanel currentRTWPWrappanel = new WrapPanel();
                        currentRTWPWrappanel.VerticalAlignment = VerticalAlignment.Center;
                        currentRTWPWrappanel.HorizontalAlignment = HorizontalAlignment.Center;

                        Label currentRTWPLabel = new Label();
                        currentRTWPLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                        currentRTWPLabel.Content = "RTWP";

                        TextBox currentRTWPTextBox = new TextBox();
                        currentRTWPTextBox.Style = (Style)FindResource("miniTextboxStyle");
                        currentRTWPTextBox.Margin = new Thickness(12, 0, 0, 0);
                        currentRTWPTextBox.Text = complaint.sites[i].sectors[j].rtwp.ToString();
                        if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                            currentRTWPTextBox.IsEnabled = false;

                        currentRTWPWrappanel.Children.Add(currentRTWPLabel);
                        currentRTWPWrappanel.Children.Add(currentRTWPTextBox);

                        sectorGrid.Children.Add(currentRTWPWrappanel);
                        Grid.SetRow(currentRTWPWrappanel, 4);
                        Grid.SetColumn(currentRTWPWrappanel, 0);

                        Label sectorBandsLabel = new Label();
                        sectorBandsLabel.Style = (Style)FindResource("headerLabelStyle");
                        sectorBandsLabel.FontSize = 20;
                        sectorBandsLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
                        sectorBandsLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                        sectorBandsLabel.Background = (Brush)brushConverter.ConvertFrom("#000080");
                        sectorBandsLabel.Content = "Sector Bands";

                        sectorGrid.Children.Add(sectorBandsLabel);
                        Grid.SetRow(sectorBandsLabel, 0);
                        Grid.SetColumn(sectorBandsLabel, 1);

                        ScrollViewer scrollViewer = new ScrollViewer();
                        scrollViewer.HorizontalAlignment = HorizontalAlignment.Center;
                        scrollViewer.Height = 100;

                        Grid tempSectorsGrid = new Grid();
                        tempSectorsGrid.HorizontalAlignment = HorizontalAlignment.Center;

                        scrollViewer.Content = tempSectorsGrid;

                        sectorGrid.Children.Add(scrollViewer);
                        Grid.SetRow(scrollViewer, 1);
                        Grid.SetRowSpan(scrollViewer, 5);
                        Grid.SetColumn(scrollViewer, 1);

                        if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_EDIT_CONDITION)
                        {
                            tempSectorsGrid.RowDefinitions.Add(new RowDefinition());
                            FillBandsGrid(ref tempSectorsGrid);
                            for (int k = 0; k < tempSectorsGrid.Children.Count; k++)
                            {
                                CheckBox currentCheckBox = (CheckBox)tempSectorsGrid.Children[k];
                                if (complaint.sites[i].sectors[j].sector_bands.Exists(x1 => x1 == currentCheckBox.Content.ToString()))
                                    currentCheckBox.IsChecked = true;
                            }
                        }
                        else
                        {
                            for (int k = 0; k < complaint.sites[i].sectors[j].sector_bands.Count; k++)
                            {
                                CheckBox checkBox = new CheckBox();
                                checkBox.Style = (Style)FindResource("checkBoxStyle2");
                                checkBox.HorizontalAlignment = HorizontalAlignment.Center;
                                checkBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                                checkBox.Content = complaint.sites[i].sectors[j].sector_bands[k];
                                checkBox.IsChecked = true;
                                checkBox.IsEnabled = false;
                                checkBox.Tag = i;

                                tempSectorsGrid.RowDefinitions.Add(new RowDefinition());
                                tempSectorsGrid.Children.Add(checkBox);
                                Grid.SetRow(checkBox, tempSectorsGrid.RowDefinitions.Count - 1);
                            }
                        }

                        if (j != 0 && viewAddCondition != BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                        {
                            String imageSource = @"\Photos\red_cross_icon.png";

                            Image currentRedCrossImage = new Image();
                            currentRedCrossImage.Height = 50;
                            currentRedCrossImage.Width = 50;
                            currentRedCrossImage.ToolTip = "Remove sector";
                            currentRedCrossImage.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));
                            currentRedCrossImage.MouseLeftButtonDown += OnClickRemoveSector;

                            sectorGrid.Children.Add(currentRedCrossImage);
                            Grid.SetRowSpan(currentRedCrossImage, 3);
                            Grid.SetColumn(currentRedCrossImage, 2);
                        }

                        if (viewAddCondition != BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION && j == complaint.sites[i].sectors.Count - 1)
                        {
                            String plusIconSource = @"\Photos\plus_icon.png";

                            Image currentPlusIcon = new Image();
                            currentPlusIcon.Height = 30;
                            currentPlusIcon.Width = 30;
                            currentPlusIcon.ToolTip = "Add sector";
                            currentPlusIcon.Source = new BitmapImage(new Uri(plusIconSource, UriKind.Relative));
                            currentPlusIcon.MouseLeftButtonDown += OnClickAddSector;

                            sectorGrid.Children.Add(currentPlusIcon);
                            Grid.SetRow(currentPlusIcon, 3);
                            Grid.SetColumnSpan(currentPlusIcon, 3);
                        }

                        //ana sehltaha mn hena remove site icon   

                        grid.Children.Add(sectorGrid);
                        Grid.SetRow(sectorGrid, grid.RowDefinitions.Count - 1);
                        Grid.SetColumnSpan(sectorGrid, 2);

                    }
                }

                if (i != 0  && viewAddCondition != BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
                {
                    Image currentRemoveSiteIcon = new Image();

                    String imageSource = @"\Photos\red_cross_icon.png";

                    currentRemoveSiteIcon.Height = 50;
                    currentRemoveSiteIcon.Width = 50;
                    currentRemoveSiteIcon.ToolTip = "Remove Site";
                    currentRemoveSiteIcon.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));
                    currentRemoveSiteIcon.MouseLeftButtonDown += OnClickRemoveSite;
                    currentRemoveSiteIcon.HorizontalAlignment = HorizontalAlignment.Right;
                    currentRemoveSiteIcon.Margin = new Thickness(0, 0, 12, 0);


                    grid.Children.Add(currentRemoveSiteIcon);
                    Grid.SetRow(currentRemoveSiteIcon, 0);
                    Grid.SetColumn(currentRemoveSiteIcon, 1);
                }

                sitesGrid.RowDefinitions.Add(new RowDefinition());
                sitesGrid.Children.Add(border);
                Grid.SetRow(border, sitesGrid.RowDefinitions.Count - 1);

            }
        }

        private void OnMouseClickMissionWindow(object sender, MouseButtonEventArgs e)
        {
            if (sender != popUp)
                popUp.IsOpen = false;
        }

        private void OnKeyBoardKeyDownMissionWindow(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                popUp.IsOpen = false;
            }
        }

        private bool InitializeCompaniesComboBox()
        {
            if (!commonQueries.GetCompanies(ref companies))
                return false;

            for(int i = 0; i < companies.Count; i++)
            {
                companyNameComboBox.Items.Add(companies[i].value);
            }

            return true;
        }

        private bool GetBands()
        {

            if (!commonQueries.GetBands(ref bands))
                return false;

            return true;
        }

        private void FillBandsGrid(ref Grid mGrid)
        {
            mGrid.Children.Clear();

            int rowNumber = 0;

            if (viewAddCondition != BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION)
            {
                for (int i = 0; i < bands.Count; i++)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Style = (Style)FindResource("checkBoxStyle2");
                    checkBox.HorizontalAlignment = HorizontalAlignment.Center;
                    checkBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                    checkBox.Content = bands[i].value;
                    checkBox.Tag = i;
                    
                    mGrid.Children.Add(checkBox);
                    Grid.SetRow(checkBox, rowNumber);

                    if (i != bands.Count - 1)
                    {
                        mGrid.RowDefinitions.Add(new RowDefinition());
                        rowNumber++;
                    }
                }
            }
        }

        private bool GetBandUnits()
        {
            if (!commonQueries.GetBandUnits(ref bandsUnits))
                return false;

            return true;
        }

        private bool GetCities()
        {
            if (!commonQueries.GetCities(ref cities))
                return false;

            for(int i = 0; i < cities.Count; i++)
            {
                regionComboBox.Items.Add(cities[i].value);
                filterCityCombo.Items.Add(cities[i].value);
            }

            return true;
        }

        private void InitializeCityCombo(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            for(int i = 0; i < cities.Count; i++)
            {
                mCombo.Items.Add(cities[i].value);
            }
        }

        private bool GetComplaintStatus()
        {
            if (!commonQueries.GetComplaintStatus(ref complaintStatus))
                return false;

            return true;
        }

        private bool GetSiteStatus()
        {
            if (!commonQueries.GetSiteStatus(ref siteStatus))
                return false;

            return true;
        }

        private void InitializeSharedCombo(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            for(int i = 0; i < companies.Count; i++)
            {
                mCombo.Items.Add(companies[i].value);
            }

            mCombo.Items.Add("No");

            mCombo.SelectedIndex = mCombo.Items.Count - 1;
           
        }

        private void InitializeRRUCombo(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            mCombo.Items.Add("On Tower");
            mCombo.Items.Add("In Shelter");

            mCombo.SelectedIndex = 0;
        }

        private void InitializeComplaintStatusCombo(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            for(int i = 0; i < complaintStatus.Count; i++)
            {
                mCombo.Items.Add(complaintStatus[i].value);
            }

            if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_ADD_CONDITION)
            {
                mCombo.SelectedIndex = 0;
                mCombo.IsEnabled = false;
            }
            else
                mCombo.SelectedItem = complaint.GetComplaintStatus();
        }

        private void InitializeSiteStatusCombo(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();
            mCombo.IsEnabled = false;

            for (int i = 0; i < siteStatus.Count; i++)
            {
                mCombo.Items.Add(siteStatus[i].value);
            }

            mCombo.SelectedIndex = 0;
        }

        private void OnClickAddBand(object sender, RoutedEventArgs e)
        {
            BrushConverter brushConverter = new BrushConverter();

            Border border = new Border();
            border.BorderThickness = new Thickness(3);
            border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");

            Grid currentGrid = new Grid();
            currentGrid.Width = 500;
            currentGrid.Height = 300;
            currentGrid.Background = Brushes.White;
            currentGrid.RowDefinitions.Add(new RowDefinition());
            currentGrid.RowDefinitions.Add(new RowDefinition());
            currentGrid.RowDefinitions.Add(new RowDefinition());

            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Label label = new Label();
            label.Style = (Style)FindResource("labelStyle");
            label.Width = 400;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.HorizontalContentAlignment = HorizontalAlignment.Center;
            label.FontWeight = FontWeights.Bold;
            label.Content = "Please enter new Band and select unit";

            TextBox textBox = new TextBox();
            textBox.Style = (Style)FindResource("textboxStyle");
            textBox.KeyDown += OnTextChangedBandTextBox;
            textBox.PreviewTextInput += NumberValidationTextBox;
            textBox.Select(0, 0);

            ComboBox comboBox = new ComboBox();
            comboBox.Style = (Style)FindResource("miniComboBoxStyle");

            for(int i = 0; i < bandsUnits.Count; i++)
            {
                comboBox.Items.Add(bandsUnits[i].value);
            }

            Button currentFinishButton = new Button();
            currentFinishButton.Style = (Style)FindResource("buttonStyle");
            currentFinishButton.Click += OnBtnClickAddBand;
            currentFinishButton.Content = "Finish";

            Button currentCancelButton = new Button();
            currentCancelButton.Style = (Style)FindResource("buttonStyle");
            currentCancelButton.Click += OnBtnClickCancelBand;
            currentCancelButton.Content = "Cancel";

            currentGrid.Children.Add(label);
            currentGrid.Children.Add(textBox);
            currentGrid.Children.Add(comboBox);
            currentGrid.Children.Add(currentCancelButton);
            currentGrid.Children.Add(currentFinishButton);

            Grid.SetRow(label, 0);
            Grid.SetColumnSpan(label, 2);

            Grid.SetRow(textBox, 1);
            Grid.SetColumn(textBox, 0);

            Grid.SetRow(comboBox, 1);
            Grid.SetColumn(comboBox, 1);

            Grid.SetRow(currentCancelButton, 2);
            Grid.SetColumn(currentCancelButton, 1);

            Grid.SetRow(currentFinishButton, 2);
            Grid.SetColumn(currentFinishButton, 0);

            border.Child = currentGrid;

            popUp.Child = border;

            popUp.IsOpen = true;

        }

        private void OnBtnClickCancelBand(object sender, RoutedEventArgs e)
        {
            popUp.IsOpen = false;
        }

        private void OnBtnClickAddBand(object sender, RoutedEventArgs e)
        {

            Border currentBorder = (Border)popUp.Child;
            Grid currentGrid = (Grid)currentBorder.Child;
            TextBox currentBandTextBox = (TextBox)currentGrid.Children[1];
            ComboBox currentBandCombo = (ComboBox)currentGrid.Children[2];

            if (currentBandTextBox.Text == "")
            {
                MessageBox.Show("Band must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if(currentBandCombo.SelectedIndex == -1)
            {
                MessageBox.Show("Band unit must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                int bandSerial = 0;

                String sqlQuery = string.Empty;
                String sqlQueryPart1 = @"select max(id) from NTRA.dbo.bands";

                sqlQuery = sqlQueryPart1;

                BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
                SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

                if (!sql.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                    return;

                if (sql.rows.Count != 0)
                {
                    bandSerial = sql.rows[0].sql_int[0] + 1;
                }

                if (bandSerial == 0)
                    bandSerial = 1;

                sqlQuery = "insert into NTRA.dbo.bands values(";
                sqlQuery += bandSerial + ",'";
                sqlQuery += currentBandTextBox.Text + " " + currentBandCombo.Text + "');";

                if (!sql.InsertRows(sqlQuery))
                    return;

                GetBands();

                popUp.IsOpen = false;
            }
        }

        private void OnTextChangedBandTextBox(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                popUp.IsOpen = false;
            }
        }


        private void OnSelChangedCompanyNameCombo(object sender, SelectionChangedEventArgs e)
        {
            if(companyNameComboBox.SelectedIndex != -1)
            {
                complaint.SetCompanyName(companies[companyNameComboBox.SelectedIndex].value);
                complaint.SetCompanySerial(companies[companyNameComboBox.SelectedIndex].key);

                siteNumberComboBox.IsEnabled = true;
                GetCompanySites();
                InitializeSiteNumberCombo(ref siteNumberComboBox);
                
                if (!selectedCompany.InitializeCompanyInfo(companies[companyNameComboBox.SelectedIndex].key))
                    return;

                if (selectedCompany.GetCompanyFieldOfWorkId() != BASIC_STRUCTS.MOBILE_OPERATOR && sitesGrid.Children.Count != 0 && filtersBorder.Visibility == Visibility.Collapsed)
                {
                    firstSiteGrid.Children.Remove(sector1Grid);
                    mainGrid.RowDefinitions[0].Height = new GridLength(0);
                    //firstSiteGrid.Children.Remove(addSectorIcon);
                }
                else if(sitesGrid.Children.Count != 0 && firstSiteGrid.Children.Count == 7)
                {
                    firstSiteGrid.Children.Add(sector1Grid);
                    Grid.SetRow(sector1Grid, 4);
                    Grid.SetColumnSpan(sector1Grid, 2);
                    //firstSiteGrid.Children.Add(addSectorIcon);
                }
            }
        }

        private void OnSelChangedRegionComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (regionComboBox.SelectedIndex != -1)
            {
                complaint.SetCity(cities[regionComboBox.SelectedIndex].value);
                complaint.SetCityId(cities[regionComboBox.SelectedIndex].key);
            }
        }

        private void OnSelChangedSharedComboBox(object sender, SelectionChangedEventArgs e)
        {
            //if (sharedComboBox.SelectedIndex != -1)
            //{
            //    //complaint.SetSharedSite(sharedComboBox.SelectedItem.ToString());
            //}
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[A-Za-z]+");
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OnButtonClickSaveChanges(object sender, RoutedEventArgs e)
        {
            if(companyNameComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Company must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (dateAddedDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Complaint date must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (ticketNumberTextBox.Text == "")
            {
                MessageBox.Show("Ticket number must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            complaint.SetCompanySerial(companies[companyNameComboBox.SelectedIndex].key);
            complaint.SetCompanyName(companies[companyNameComboBox.SelectedIndex].value);
            complaint.SetComplaintDate(DateTime.Parse(dateAddedDatePicker.SelectedDate.ToString()));
            complaint.SetTicketNumber(ticketNumberTextBox.Text);
            complaint.SetComplaintStatus(statusComboBox.SelectedItem.ToString());
            complaint.SetComplaintStatusId(complaintStatus[statusComboBox.SelectedIndex].key);
            
            complaint.InitializeAddedBy(loggedInUser.GetEmployeeId());

            if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_EDIT_CONDITION)
            {

                if (!complaint.EditComplaint(selectedCompany.GetCompanyFieldOfWorkId()))
                    return;
            }
            else
            {

                List<BASIC_STRUCTS.SITE_STRUCT> tempSites = new List<BASIC_STRUCTS.SITE_STRUCT>();

                for (int i = 0; i < sitesGrid.RowDefinitions.Count; i++)
                {
                    BASIC_STRUCTS.SITE_STRUCT site = new BASIC_STRUCTS.SITE_STRUCT();
                    site.sectors = new List<BASIC_STRUCTS.SECTOR_STRUCT>();

                    Border currentBorder = (Border)sitesGrid.Children[i];
                    Grid currentGrid = (Grid)currentBorder.Child;

                    WrapPanel currentSiteNumberWrapPanel = (WrapPanel)currentGrid.Children[1];
                    ComboBox currentSiteNumberComboBox = (ComboBox)currentSiteNumberWrapPanel.Children[1];

                    if (currentSiteNumberComboBox.SelectedIndex != -1)
                    {
                        Site currentSite = new Site();
                        //if(!currentSite.InitializeSite(companies[companyNameComboBox.SelectedIndex].key, companySites[currentSiteNumberComboBox.SelectedIndex].site_serial))
                        //{
                        //    MessageBox.Show("Server connection failed, check your internet connection!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        //    return;
                        //}

                        site.site_serial = companySites[currentSiteNumberComboBox.SelectedIndex].site_serial;

                        WrapPanel currentSiteStatusWrapPanel = (WrapPanel)currentGrid.Children[2];
                        ComboBox currentSiteStatusComboBox = (ComboBox)currentSiteStatusWrapPanel.Children[1];
                        site.site_status = currentSiteStatusComboBox.SelectedItem.ToString();
                        site.site_status_id = siteStatus[currentSiteStatusComboBox.SelectedIndex].key;

                    }
                    else
                    {
                        Site currentSite = new Site();

                        currentSite.SetSiteNumber(currentSiteNumberComboBox.Text);

                        currentSite.SetAddedBy(loggedInUser.GetEmployeeName());
                        currentSite.SetAddedById(loggedInUser.GetEmployeeId());

                        currentSite.SetCompanySerial(companies[companyNameComboBox.SelectedIndex].key);
                        currentSite.SetCompanyName(companies[companyNameComboBox.SelectedIndex].value);

                        WrapPanel currentCityWrapPanel = (WrapPanel)currentGrid.Children[3];
                        ComboBox currentCityComboBox = (ComboBox)currentCityWrapPanel.Children[1];
                        currentSite.SetCity(cities[currentCityComboBox.SelectedIndex].value);
                        currentSite.SetCityId(cities[currentCityComboBox.SelectedIndex].key);

                        WrapPanel currentRegionWrapPanel = (WrapPanel)currentGrid.Children[4];
                        TextBox currentRegionTextBox = (TextBox)currentRegionWrapPanel.Children[1];
                        currentSite.SetRegion(currentRegionTextBox.Text);

                        WrapPanel currentLongWrapPanel = (WrapPanel)currentGrid.Children[5];
                        TextBox currentLongTextBox = (TextBox)currentLongWrapPanel.Children[1];
                        currentSite.SetLong(currentLongTextBox.Text);

                        WrapPanel currentLatWrapPanel = (WrapPanel)currentGrid.Children[6];
                        TextBox currentLatTextBox = (TextBox)currentLatWrapPanel.Children[1];
                        currentSite.SetLat(currentLatTextBox.Text);

                        if (!currentSite.IssueNewSite())
                        {
                            MessageBox.Show("Server connection failed, check your internet connection!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        site.site_serial = currentSite.GetSiteSerial();

                        WrapPanel currentSiteStatusWrapPanel = (WrapPanel)currentGrid.Children[2];
                        ComboBox currentSiteStatusComboBox = (ComboBox)currentSiteStatusWrapPanel.Children[1];
                        site.site_status = currentSiteStatusComboBox.SelectedItem.ToString();
                        site.site_status_id = siteStatus[currentSiteStatusComboBox.SelectedIndex].key;

                    }


                    int childIndex = 7;

                    //if (i != 0)
                    //    childIndex = 8;

                    if (selectedCompany.GetCompanyFieldOfWorkId() == BASIC_STRUCTS.MOBILE_OPERATOR)
                    {
                        for (int j = childIndex; j < currentGrid.Children.Count; j++)
                        {
                            BASIC_STRUCTS.SECTOR_STRUCT sector = new BASIC_STRUCTS.SECTOR_STRUCT();
                            sector.sector_bands = new List<string>();

                            if (currentGrid.Children[j].GetType() == typeof(Image))
                                continue;

                            Grid currentSectorGrid = (Grid)currentGrid.Children[j];
                            //childIndex++;
                            ////Because remove site icon is added after Grid in the handler add site, so for all sites except first site has to increment by 2


                            WrapPanel currentSectorNumberWrapPanel = (WrapPanel)currentSectorGrid.Children[0];
                            TextBox currentSectorNumberTextBox = (TextBox)currentSectorNumberWrapPanel.Children[1];
                            sector.sector_number = currentSectorNumberTextBox.Text;

                            WrapPanel currentrruWrapPanel = (WrapPanel)currentSectorGrid.Children[1];
                            ComboBox currentrruComboBox = (ComboBox)currentrruWrapPanel.Children[1];
                            sector.rru = currentrruComboBox.Text;

                            WrapPanel currentSharedWrapPanel = (WrapPanel)currentSectorGrid.Children[2];
                            ComboBox currentSharedComboBox = (ComboBox)currentSharedWrapPanel.Children[1];
                            if (currentSharedComboBox.SelectedIndex != -1 && currentSharedComboBox.SelectedIndex != companies.Count)
                            {
                                sector.shared_with = companies[currentSharedComboBox.SelectedIndex].value;
                                sector.shared_with_id = companies[currentSharedComboBox.SelectedIndex].key;
                            }
                            else
                            {
                                sector.shared_with = "";
                                sector.shared_with_id = 0;
                            }

                            WrapPanel currentAzimuthWrapPanel = (WrapPanel)currentSectorGrid.Children[3];
                            TextBox currentAzimuthTextBox = (TextBox)currentAzimuthWrapPanel.Children[1];
                            if (int.TryParse(currentAzimuthTextBox.Text, out int intValue))
                            {
                                MessageBox.Show("Azimuth must be an Integer or a decimal value, Please check your input for site " + currentSiteNumberComboBox.Text, "Error", MessageBoxButton.OK);
                                return;
                            }
                            sector.azimuth = int.Parse(currentAzimuthTextBox.Text);

                            WrapPanel currentRTWPWrapPanel = (WrapPanel)currentSectorGrid.Children[4];
                            TextBox currentRTWPTextBox = (TextBox)currentRTWPWrapPanel.Children[1];
                            if (decimal.TryParse(currentAzimuthTextBox.Text, out decimal decimalValue))
                            {
                                MessageBox.Show("RTWP mus be an Integer or a decimal value, Please check your input for site " + currentSiteNumberComboBox.Text, "Error", MessageBoxButton.OK);
                                return;
                            }
                            sector.rtwp = int.Parse(currentRTWPTextBox.Text);

                            ScrollViewer currentScrollViewer = (ScrollViewer)currentSectorGrid.Children[6];
                            Grid bandsGrid = (Grid)currentScrollViewer.Content;

                            for (int k = 0; k < bandsGrid.Children.Count; k++)
                            {
                                CheckBox currentCheckBox = (CheckBox)bandsGrid.Children[k];

                                if (currentCheckBox.IsChecked == true)
                                {
                                    sector.sector_bands.Add(currentCheckBox.Content.ToString());
                                }
                            }

                            site.sectors.Add(sector);
                        }
                    }

                    if (i == 0 || (tempSites.Count != 0 && !tempSites.Exists(x1 => x1.site_serial == site.site_serial)))
                        tempSites.Add(site);
                    else
                    {
                        MessageBox.Show("Site " + currentSiteNumberComboBox.Text + " is a duplicated site!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                }

                complaint.SetComplaintSites(ref tempSites);

                if (viewAddCondition == BASIC_STRUCTS.COMPLAINT_ADD_CONDITION)
                {
                    if (!complaint.IssueNewComplaint(selectedCompany.GetCompanyFieldOfWorkId()))
                        return;
                }
            }

            this.Close();
        }

        private void OnClickRemoveSector(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid currentGrid = (Grid)currentImage.Parent;
            Grid tempMainGrid = (Grid)currentGrid.Parent;

            Grid lastSectorsGrid = new Grid();
            bool lastItemIsImage = false;

            if (tempMainGrid.Children[tempMainGrid.Children.Count - 1].GetType() != typeof(Image))
            {
                lastSectorsGrid = (Grid)tempMainGrid.Children[tempMainGrid.Children.Count - 1];
            }
            else
            {
                lastSectorsGrid = (Grid)tempMainGrid.Children[tempMainGrid.Children.Count - 2];
                lastItemIsImage = true;
            }

            if (lastSectorsGrid == currentGrid)
            {
                Image currentAddSectorIcon = (Image)lastSectorsGrid.Children[lastSectorsGrid.Children.Count - 1];

                lastSectorsGrid.Children.Remove(currentAddSectorIcon);

                tempMainGrid.Children.Remove(currentGrid);
                tempMainGrid.RowDefinitions.RemoveAt(tempMainGrid.RowDefinitions.Count - 1);

                Grid newCurrentGrid = new Grid();

                if (lastItemIsImage == false)
                {
                    newCurrentGrid = (Grid)tempMainGrid.Children[tempMainGrid.Children.Count - 1];
                }
                else
                {
                    newCurrentGrid = (Grid)tempMainGrid.Children[tempMainGrid.Children.Count - 2];
                }

                newCurrentGrid.Children.Add(currentAddSectorIcon);
                Grid.SetRow(currentAddSectorIcon, 5);
                Grid.SetColumnSpan(currentAddSectorIcon, 3);

                if (!lastItemIsImage)
                {
                    for (int i = 0; i < tempMainGrid.Children.Count; i++)
                    {
                        var child = tempMainGrid.Children[i];
                        Grid.SetRow(child, i);
                    }
                }
                else
                {
                    for (int i = 0; i < tempMainGrid.Children.Count - 1; i++)
                    {
                        var child = tempMainGrid.Children[i];
                        Grid.SetRow(child, i);
                    }
                }
            }
            else
            {
                tempMainGrid.Children.Remove(currentGrid);
                tempMainGrid.RowDefinitions.RemoveAt(tempMainGrid.RowDefinitions.Count - 1);
            }
        }

        private void OnClickAddSector(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid currentGrid = (Grid)currentImage.Parent;
            Grid sectorsGrid = (Grid)currentGrid.Parent;


            Grid grid = new Grid();
            CreateNewSectorGrid(ref grid, true);

            currentGrid.Children.Remove(currentImage);

            sectorsGrid.RowDefinitions.Add(new RowDefinition());
            sectorsGrid.Children.Add(grid);
            Grid.SetColumnSpan(grid, 2);
            Grid.SetRow(grid, sectorsGrid.RowDefinitions.Count - 1);
        }

        private Grid CreateNewSectorGrid(ref Grid grid, bool addRemoveSectorIcon)
        {
            BrushConverter brushConverter = new BrushConverter();

            grid.Margin = new Thickness(12);

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });

            WrapPanel currentSectorWrappanel = new WrapPanel();
            currentSectorWrappanel.VerticalAlignment = VerticalAlignment.Center;
            currentSectorWrappanel.HorizontalAlignment = HorizontalAlignment.Center;

            Label currentSectorLabel = new Label();
            currentSectorLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
            currentSectorLabel.Content = "Sector No: ";

            TextBox currentSectorTextBox = new TextBox();
            currentSectorTextBox.Style = (Style)FindResource("miniTextboxStyle");
            currentSectorTextBox.Margin = new Thickness(12, 0, 0, 0);

            currentSectorWrappanel.Children.Add(currentSectorLabel);
            currentSectorWrappanel.Children.Add(currentSectorTextBox);

            grid.Children.Add(currentSectorWrappanel);
            Grid.SetRow(currentSectorWrappanel, 0);
            Grid.SetColumn(currentSectorWrappanel, 0);

            WrapPanel currentrruWrappanel = new WrapPanel();
            currentrruWrappanel.VerticalAlignment = VerticalAlignment.Center;
            currentrruWrappanel.HorizontalAlignment = HorizontalAlignment.Center;

            Label currentrruLabel = new Label();
            currentrruLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
            currentrruLabel.Content = "RRU: ";

            ComboBox currentrruComboBox = new ComboBox();
            currentrruComboBox.Style = (Style)FindResource("miniComboBoxStyle");
            currentrruComboBox.Margin = new Thickness(12, 0, 0, 0);
            InitializeRRUCombo(ref currentrruComboBox);

            currentrruWrappanel.Children.Add(currentrruLabel);
            currentrruWrappanel.Children.Add(currentrruComboBox);

            grid.Children.Add(currentrruWrappanel);
            Grid.SetRow(currentrruWrappanel, 1);
            Grid.SetColumn(currentrruWrappanel, 0);

            WrapPanel currentSharedWrappanel = new WrapPanel();
            currentSharedWrappanel.VerticalAlignment = VerticalAlignment.Center;
            currentSharedWrappanel.HorizontalAlignment = HorizontalAlignment.Center;

            Label currentSharedLabel = new Label();
            currentSharedLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
            currentSharedLabel.Content = "Shared: ";

            ComboBox currentSharedComboBox = new ComboBox();
            currentSharedComboBox.Style = (Style)FindResource("miniComboBoxStyle");
            currentSharedComboBox.Margin = new Thickness(12, 0, 0, 0);
            InitializeSharedCombo(ref currentSharedComboBox);

            currentSharedWrappanel.Children.Add(currentSharedLabel);
            currentSharedWrappanel.Children.Add(currentSharedComboBox);

            grid.Children.Add(currentSharedWrappanel);
            Grid.SetRow(currentSharedWrappanel, 2);
            Grid.SetColumn(currentSharedWrappanel, 0);

            WrapPanel currentAzimuthWrappanel = new WrapPanel();
            currentAzimuthWrappanel.VerticalAlignment = VerticalAlignment.Center;
            currentAzimuthWrappanel.HorizontalAlignment = HorizontalAlignment.Center;

            Label currentAzimuthLabel = new Label();
            currentAzimuthLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
            currentAzimuthLabel.Content = "Azimuth";

            TextBox currentAzimuthTextBox = new TextBox();
            currentAzimuthTextBox.Style = (Style)FindResource("miniTextboxStyle");
            currentAzimuthTextBox.Margin = new Thickness(12, 0, 0, 0);

            currentAzimuthWrappanel.Children.Add(currentAzimuthLabel);
            currentAzimuthWrappanel.Children.Add(currentAzimuthTextBox);

            grid.Children.Add(currentAzimuthWrappanel);
            Grid.SetRow(currentAzimuthWrappanel, 3);
            Grid.SetColumn(currentAzimuthWrappanel, 0);

            WrapPanel currentRTWPWrappanel = new WrapPanel();
            currentRTWPWrappanel.VerticalAlignment = VerticalAlignment.Center;
            currentRTWPWrappanel.HorizontalAlignment = HorizontalAlignment.Center;

            Label currentRTWPLabel = new Label();
            currentRTWPLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
            currentRTWPLabel.Content = "RTWP";

            TextBox currentRTWPTextBox = new TextBox();
            currentRTWPTextBox.Style = (Style)FindResource("miniTextboxStyle");
            currentRTWPTextBox.Margin = new Thickness(12, 0, 0, 0);

            currentRTWPWrappanel.Children.Add(currentRTWPLabel);
            currentRTWPWrappanel.Children.Add(currentRTWPTextBox);

            grid.Children.Add(currentRTWPWrappanel);
            Grid.SetRow(currentRTWPWrappanel, 4);
            Grid.SetColumn(currentRTWPWrappanel, 0);

            Label currentSectorBandsLabel = new Label();
            currentSectorBandsLabel.Style = (Style)FindResource("headerLabelStyle");
            currentSectorBandsLabel.FontSize = 20;
            currentSectorBandsLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
            currentSectorBandsLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
            currentSectorBandsLabel.Background = (Brush)brushConverter.ConvertFrom("#000080");
            currentSectorBandsLabel.Content = "Sector Bands";

            grid.Children.Add(currentSectorBandsLabel);
            Grid.SetRow(currentSectorBandsLabel, 0);
            Grid.SetColumn(currentSectorBandsLabel, 1);

            ScrollViewer currentScrollViewer = new ScrollViewer();
            currentScrollViewer.HorizontalAlignment = HorizontalAlignment.Center;
            currentScrollViewer.VerticalAlignment = VerticalAlignment.Top;
            //currentScrollViewer.Height = 80;

            Grid tempSectorsGrid = new Grid();
            tempSectorsGrid.HorizontalAlignment = HorizontalAlignment.Center;
            tempSectorsGrid.RowDefinitions.Add(new RowDefinition());

            currentScrollViewer.Content = tempSectorsGrid;

            grid.Children.Add(currentScrollViewer);
            Grid.SetRow(currentScrollViewer, 1);
            Grid.SetRowSpan(currentScrollViewer, 7);
            Grid.SetColumn(currentScrollViewer, 1);

            FillBandsGrid(ref tempSectorsGrid);

            if (addRemoveSectorIcon)
            {
                String imageSource = @"\Photos\red_cross_icon.png";

                Image redCrossImage = new Image();
                redCrossImage.Height = 50;
                redCrossImage.Width = 50;
                redCrossImage.ToolTip = "Remove sector";
                redCrossImage.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));
                redCrossImage.MouseLeftButtonDown += OnClickRemoveSector;

                grid.Children.Add(redCrossImage);
                Grid.SetRowSpan(redCrossImage, 6);
                Grid.SetColumn(redCrossImage, 2);
            }

            String plusIconSource = @"\Photos\plus_icon.png";

            Image plusIcon = new Image();
            plusIcon.Height = 30;
            plusIcon.Width = 30;
            plusIcon.ToolTip = "Add sector";
            plusIcon.Source = new BitmapImage(new Uri(plusIconSource, UriKind.Relative));
            plusIcon.MouseLeftButtonDown += OnClickAddSector;

            grid.Children.Add(plusIcon);
            Grid.SetRow(plusIcon, 5);
            Grid.SetColumnSpan(plusIcon, 3);


            return grid;
        }

        private void OnBtnClickAddSite(object sender, RoutedEventArgs e)
        {
            sitesGrid.RowDefinitions.Add(new RowDefinition());

            Border border = new Border();
            CreateNewSiteGrid(ref border);
            
            sitesGrid.Children.Add(border);
            Grid.SetRow(border, sitesGrid.RowDefinitions.Count - 1);

            companyNameComboBox.IsEnabled = false;

            border.BringIntoView();
        }

        private Border CreateNewSiteGrid(ref Border border)
        {
            BrushConverter brushConverter = new BrushConverter();

            border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");
            border.CornerRadius = new CornerRadius(20);
            border.BorderThickness = new Thickness(3);
            border.Margin = new Thickness(12);
            border.VerticalAlignment = VerticalAlignment.Top;

            Grid grid = new Grid();

            border.Child = grid;

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            grid.RowDefinitions.Add(new RowDefinition());

            Label siteLabelTitle = new Label();
            siteLabelTitle.HorizontalAlignment = HorizontalAlignment.Center;
            siteLabelTitle.HorizontalContentAlignment = HorizontalAlignment.Center;
            siteLabelTitle.Style = (Style)FindResource("headerLabelStyle");
            siteLabelTitle.Background = (Brush)brushConverter.ConvertFrom("#000080");
            siteLabelTitle.Width = 300;
            siteLabelTitle.FontSize = 20;
            siteLabelTitle.FontWeight = FontWeights.Bold;
            siteLabelTitle.Content = "Site";

            grid.Children.Add(siteLabelTitle);
            Grid.SetRow(siteLabelTitle, 0);
            Grid.SetColumnSpan(siteLabelTitle, 2);

            String imageSource = @"\Photos\red_cross_icon.png";

            Image removeSiteIcon = new Image();
            removeSiteIcon.Height = 50;
            removeSiteIcon.Width = 50;
            removeSiteIcon.ToolTip = "Remove Site";
            removeSiteIcon.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));
            removeSiteIcon.MouseLeftButtonDown += OnClickRemoveSite;
            removeSiteIcon.HorizontalAlignment = HorizontalAlignment.Right;
            removeSiteIcon.Margin = new Thickness(0, 0, 12, 0);

            WrapPanel currentSiteNumberWrapPanel = new WrapPanel();
            currentSiteNumberWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            currentSiteNumberWrapPanel.VerticalAlignment = VerticalAlignment.Center;

            Label currentSiteNumberLabel = new Label();
            currentSiteNumberLabel.Style = (Style)FindResource("labelStyleBlack");
            currentSiteNumberLabel.Content = "Site No: ";

            ComboBox currentSiteNumberComboBox = new ComboBox();
            currentSiteNumberComboBox.Margin = new Thickness(12, 0, 0, 0);
            currentSiteNumberComboBox.Style = (Style)FindResource("comboBoxStyle");
            currentSiteNumberComboBox.IsEditable = true;
            currentSiteNumberComboBox.IsTextSearchEnabled = true;
            currentSiteNumberComboBox.IsTextSearchCaseSensitive = false;
            currentSiteNumberComboBox.SelectionChanged += OnSelChangedSiteNumberCombo;
            InitializeSiteNumberCombo(ref currentSiteNumberComboBox);

            currentSiteNumberWrapPanel.Children.Add(currentSiteNumberLabel);
            currentSiteNumberWrapPanel.Children.Add(currentSiteNumberComboBox);

            grid.Children.Add(currentSiteNumberWrapPanel);
            Grid.SetRow(currentSiteNumberWrapPanel, 1);
            Grid.SetColumn(currentSiteNumberWrapPanel, 0);


            WrapPanel currentSiteStatusWrapPanel = new WrapPanel();
            currentSiteStatusWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            currentSiteStatusWrapPanel.VerticalAlignment = VerticalAlignment.Center;

            Label currentSiteStatusLabel = new Label();
            currentSiteStatusLabel.Style = (Style)FindResource("labelStyleBlack");
            currentSiteStatusLabel.Content = "Status: ";

            ComboBox currentSiteStatusComboBox = new ComboBox();
            currentSiteStatusComboBox.Margin = new Thickness(12, 0, 0, 0);
            currentSiteStatusComboBox.Style = (Style)FindResource("comboBoxStyle");
            InitializeSiteStatusCombo(ref currentSiteStatusComboBox);

            currentSiteStatusWrapPanel.Children.Add(currentSiteStatusLabel);
            currentSiteStatusWrapPanel.Children.Add(currentSiteStatusComboBox);

            grid.Children.Add(currentSiteStatusWrapPanel);
            Grid.SetRow(currentSiteStatusWrapPanel, 1);
            Grid.SetColumn(currentSiteStatusWrapPanel, 1);


            WrapPanel currentCityWrapPanel = new WrapPanel();
            currentCityWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            currentCityWrapPanel.VerticalAlignment = VerticalAlignment.Center;

            Label currentCityLabel = new Label();
            currentCityLabel.Style = (Style)FindResource("labelStyleBlack");
            currentCityLabel.Content = "City: ";

            ComboBox currentCityComboBox = new ComboBox();
            currentCityComboBox.Margin = new Thickness(12, 0, 0, 0);
            currentCityComboBox.Style = (Style)FindResource("comboBoxStyle");
            InitializeCityCombo(ref currentCityComboBox);

            currentCityWrapPanel.Children.Add(currentCityLabel);
            currentCityWrapPanel.Children.Add(currentCityComboBox);

            grid.Children.Add(currentCityWrapPanel);
            Grid.SetRow(currentCityWrapPanel, 2);
            Grid.SetColumn(currentCityWrapPanel, 0);


            WrapPanel currentRegionWrapPanel = new WrapPanel();
            currentRegionWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            currentRegionWrapPanel.VerticalAlignment = VerticalAlignment.Center;

            Label currentRegionLabel = new Label();
            currentRegionLabel.Style = (Style)FindResource("labelStyleBlack");
            currentRegionLabel.Content = "Region: ";

            TextBox currentRegionTextbox = new TextBox();
            currentRegionTextbox.Margin = new Thickness(12, 0, 0, 0);
            currentRegionTextbox.Style = (Style)FindResource("textboxStyle");

            currentRegionWrapPanel.Children.Add(currentRegionLabel);
            currentRegionWrapPanel.Children.Add(currentRegionTextbox);

            grid.Children.Add(currentRegionWrapPanel);
            Grid.SetRow(currentRegionWrapPanel, 2);
            Grid.SetColumn(currentRegionWrapPanel, 1);


            WrapPanel currentLongWrapPanel = new WrapPanel();
            currentLongWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            currentLongWrapPanel.VerticalAlignment = VerticalAlignment.Center;

            Label currentLongLabel = new Label();
            currentLongLabel.Style = (Style)FindResource("labelStyleBlack");
            currentLongLabel.Content = "Long: ";

            TextBox currentLongTextBox = new TextBox();
            currentLongTextBox.Margin = new Thickness(12, 0, 0, 0);
            currentLongTextBox.Style = (Style)FindResource("textboxStyle");
            currentLongTextBox.PreviewTextInput += NumberValidationTextBox;

            currentLongWrapPanel.Children.Add(currentLongLabel);
            currentLongWrapPanel.Children.Add(currentLongTextBox);

            grid.Children.Add(currentLongWrapPanel);
            Grid.SetRow(currentLongWrapPanel, 3);
            Grid.SetColumn(currentLongWrapPanel, 0);


            WrapPanel currentLatWrapPanel = new WrapPanel();
            currentLatWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            currentLatWrapPanel.VerticalAlignment = VerticalAlignment.Center;

            Label currentLatLabel = new Label();
            currentLatLabel.Style = (Style)FindResource("labelStyleBlack");
            currentLatLabel.Content = "Lat: ";

            TextBox currentLatTextBox = new TextBox();
            currentLatTextBox.Margin = new Thickness(12, 0, 0, 0);
            currentLatTextBox.Style = (Style)FindResource("textboxStyle");
            currentLatTextBox.PreviewTextInput += NumberValidationTextBox;

            currentLatWrapPanel.Children.Add(currentLatLabel);
            currentLatWrapPanel.Children.Add(currentLatTextBox);

            grid.Children.Add(currentLatWrapPanel);
            Grid.SetRow(currentLatWrapPanel, 3);
            Grid.SetColumn(currentLatWrapPanel, 1);

            ///shelt remove site icon mn hena

            //if (selectedCompany.GetCompanyFieldOfWorkId() == BASIC_STRUCTS.MOBILE_OPERATOR)
            //{
                Grid sectorGrid = new Grid();
                CreateNewSectorGrid(ref sectorGrid, false);

                grid.Children.Add(sectorGrid);
                Grid.SetRow(sectorGrid, 4);
                Grid.SetColumnSpan(sectorGrid, 2);
            //}


            grid.Children.Add(removeSiteIcon);
            Grid.SetRow(removeSiteIcon, 0);
            Grid.SetColumn(removeSiteIcon, 1);

            return border;
        }

        private void OnClickRemoveSite(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid currentGrid = (Grid)currentImage.Parent;
            Border currentBorder = (Border)currentGrid.Parent;
            WrapPanel currentSiteNumberWrapPanel = (WrapPanel)currentGrid.Children[1];
            ComboBox currentSiteNumberCombo = (ComboBox)currentSiteNumberWrapPanel.Children[1];

            if (currentSiteNumberCombo.SelectedIndex == -1 || !commonQueries.CheckSiteMissions(complaint.GetCompanySerial(), complaint.GetSerial(), companySites[currentSiteNumberCombo.SelectedIndex].site_serial))
            {
                sitesGrid.Children.Remove(currentBorder);
                sitesGrid.RowDefinitions.RemoveAt(sitesGrid.RowDefinitions.Count - 1);

                if (sitesGrid.RowDefinitions.Count == 1)
                    companyNameComboBox.IsEnabled = true;

                if (selectedCompany.GetCompanyFieldOfWorkId() != BASIC_STRUCTS.MOBILE_OPERATOR && sitesGrid.Children.Count != 0 && filtersBorder.Visibility == Visibility.Collapsed)
                {
                    firstSiteGrid.Children.Remove(sector1Grid);
                    mainGrid.RowDefinitions[0].Height = new GridLength(0);
                    //firstSiteGrid.Children.Remove(addSectorIcon);
                }
                else if (sitesGrid.Children.Count != 0 && firstSiteGrid.Children.Count == 7)
                {
                    firstSiteGrid.Children.Add(sector1Grid);
                    Grid.SetRow(sector1Grid, 4);
                    Grid.SetColumnSpan(sector1Grid, 2);
                    //firstSiteGrid.Children.Add(addSectorIcon);
                }
            }
            else
            {
                MessageBox.Show("Site " + currentSiteNumberCombo.Text + " can not be removed as missions has already been added for this site!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeSiteNumberCombo(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            for(int i = 0; i < companySites.Count; i++)
            {
                mCombo.Items.Add(companySites[i].site_number);
            }
        }

        private void OnSelChangedSiteNumberCombo(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentSiteCombo = (ComboBox)sender;
            WrapPanel currentSiteWrapPanel = (WrapPanel)currentSiteCombo.Parent;
            Grid parentGrid = (Grid)currentSiteWrapPanel.Parent;

            int companySiteIndex = currentSiteCombo.SelectedIndex;

            WrapPanel currentCityWrapPanel = (WrapPanel)parentGrid.Children[3];
            ComboBox currentCityCombo = (ComboBox)currentCityWrapPanel.Children[1];

            WrapPanel currentRegionWrapPanel = (WrapPanel)parentGrid.Children[4];
            TextBox currentRegionTextbox = (TextBox)currentRegionWrapPanel.Children[1];

            WrapPanel currentLongWrapPanel = (WrapPanel)parentGrid.Children[5];
            TextBox currentLongTextbox = (TextBox)currentLongWrapPanel.Children[1];

            WrapPanel currentLatWrapPanel = (WrapPanel)parentGrid.Children[6];
            TextBox currentLatTextbox = (TextBox)currentLatWrapPanel.Children[1];

            if (currentSiteCombo.SelectedIndex != -1)
            {
                currentCityCombo.SelectedItem = companySites[companySiteIndex].city;
                currentCityCombo.IsEnabled = false;

                currentRegionTextbox.Text = companySites[companySiteIndex].region;
                currentRegionTextbox.IsEnabled = false;

                currentLongTextbox.Text = companySites[companySiteIndex].longitude;
                currentLongTextbox.IsEnabled = false;

                currentLatTextbox.Text = companySites[companySiteIndex].latitude;
                currentLatTextbox.IsEnabled = false;
            }
            else
            {
                currentCityCombo.SelectedIndex = -1;
                currentCityCombo.IsEnabled = true;

                currentRegionTextbox.Text = "";
                currentRegionTextbox.IsEnabled = true;

                currentLongTextbox.Text = "";
                currentLongTextbox.IsEnabled = true;

                currentLatTextbox.Text = "";
                currentLatTextbox.IsEnabled = true;
            }
        }

        private void OnButtonClickSearch(object sender, RoutedEventArgs e)
        {
            InitializeSitesGrid();
        }

        private void OnButtonClickCancel(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";
            InitializeSitesGrid();
        }

        private void OnCheckShowFilters(object sender, RoutedEventArgs e)
        {
            filtersGrid.Visibility = Visibility.Visible;
        }

        private void OnUnCheckShowFilters(object sender, RoutedEventArgs e)
        {
            filterCityCombo.SelectedIndex = -1;
            filterRegionTextBox.Text = "";
            filterStatusComboBox.SelectedIndex = -1;
            filterLatTextBox.Text = "";
            filterLongTextBox.Text = "";

            filtersGrid.Visibility = Visibility.Collapsed;

            InitializeSitesGrid();
        }

        private void OnClickApplyFiltersButton(object sender, RoutedEventArgs e)
        {
            InitializeSitesGrid();
        }

        private void OnClickRemoveFiltersButton(object sender, RoutedEventArgs e)
        {
            filterCityCombo.SelectedIndex = -1;
            filterRegionTextBox.Text = "";
            filterStatusComboBox.SelectedIndex = -1;
            filterLatTextBox.Text = "";
            filterLongTextBox.Text = "";
        }
    }
}
