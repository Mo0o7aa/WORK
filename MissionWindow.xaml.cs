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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for MissionWindow.xaml
    /// </summary>
    public partial class MissionWindow : Window
    {
        Employee loggedInUser;
        SQLServer sqlDatabase;
        CommonQueries commonQueries;

        Missions mission;

        int missionCondition;

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> companies;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> handheld;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> antennas;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> cables;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> vehichles;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> missionStatusList;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> cities;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> engineers;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> reasonsOfInterference;

        private List<int> removedSiteSerials;

        Popup popUp;

        public MissionWindow(ref Employee mLoggedInUser, ref Missions mMission, ref int mMissionCondition)
        {
            sqlDatabase = new SQLServer();
            commonQueries = new CommonQueries();

            loggedInUser = mLoggedInUser;
            mission = mMission;
            missionCondition = mMissionCondition;

            removedSiteSerials = new List<int>();

            InitializeComponent();

            companies = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            handheld = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            vehichles = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            missionStatusList = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            cities = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            engineers = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            reasonsOfInterference = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            antennas = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            cables = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>(); 

            GetCompanies();
            GetAntennas();
            GetCables();
            InitializeHandheldCombo();
            InitializeTypeCombo();
            InitializeVehichleCombo();
            InitializeMissionStatusCombo();
            InitializeCityCombo();
            InitializeEngineersCombo();
            InitializeReasonsOfInterferenceCombo();

            missionStartDatePicker.SelectedDate = DateTime.Today;
            missionEndDatePicker.SelectedDate = DateTime.Today;

            InitializeMissionTypeCombo();
            InitializeDatePicker(ref measurementLocationDatePicker);
            InitializeCompanyCombo(ref companyComboBox);

            if (missionCondition == BASIC_STRUCTS.MISSION_VIEW_CONDITION)
            {
                SetUIValues();
                finishButton.IsEnabled = false;
                headerLabel.Content = "View Mission";
            }

            if (missionCondition == BASIC_STRUCTS.MISSION_EDIT_CONDITION)
            {
                SetUIValues();
                headerLabel.Content = "Edit Mission";
                companyComboBox.IsEnabled = false;
                ticketComboBox.IsEnabled = false;
                engineer1ComboBox.IsEnabled = false;
            }

            if(missionCondition == BASIC_STRUCTS.MISSION_ADD_CONDITION)
            {
                missionStartDatePicker.SelectedDate = DateTime.Today;
                missionEndDatePicker.SelectedDate = DateTime.Today;
                engineer1ComboBox.SelectedIndex = engineers.FindIndex(x1 => x1.key == loggedInUser.GetEmployeeId());

                if(mission.complaint.GetSerial() > 0)
                {
                    companyComboBox.SelectedIndex = companies.FindIndex(x1 => x1.key == mission.complaint.GetCompanySerial());
                    companyComboBox.IsEnabled = false;

                    ticketComboBox.SelectedItem = mission.complaint.GetTicketNumber();
                    ticketComboBox.IsEnabled = false;
                }
            }

        }

        private void InitializeReasonsOfInterferenceCombo()
        {
            if (!commonQueries.GetReasonsOfInterfernce(ref reasonsOfInterference))
                return;

            reasonOfInterfereneCombo.Items.Clear();

            for(int i = 0; i < reasonsOfInterference.Count; i++)
            {
                reasonOfInterfereneCombo.Items.Add(reasonsOfInterference[i].value);
            }
        }

        private void FillReasonsOfInterferenceCombo(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            for (int i = 0; i < reasonsOfInterference.Count; i++)
            {
                mCombo.Items.Add(reasonsOfInterference[i].value);
            }
        }

        private void InitializeTypeCombo()
        {
            typeComboBox.Items.Clear();

            typeComboBox.Items.Add("Handheld");
            typeComboBox.Items.Add("Antenna");
            typeComboBox.Items.Add("Cable");
        }

        private void InitializeHandheldCombo()
        {
            if (!commonQueries.GetHandhelds(ref handheld))
                return;

            //handheldComboBox.Items.Clear();
            //
            //for(int i = 0; i < handheld.Count; i++)
            //{
            //    handheldComboBox.Items.Add(handheld[i].value);
            //}
        }

        private void GetAntennas()
        {
            if (!commonQueries.GetAntennas(ref antennas))
                return;
        }
        private void GetCables()
        {
            if (!commonQueries.GetCables(ref cables))
                return;
        }

        private void FillAntennaCombo(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            for(int i = 0; i < antennas.Count; i++)
            {
                mCombo.Items.Add(antennas[i].value);
            }
        }

        private void FillCableCombo(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            for (int i = 0; i < cables.Count; i++)
            {
                mCombo.Items.Add(cables[i].value);
            }
        }

        private void FillHandheld(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            for(int i = 0; i < handheld.Count; i++)
            {
                mCombo.Items.Add(handheld[i].value);
            }
        }

        private void FillTypeCombo(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            mCombo.Items.Add("Handheld");
            mCombo.Items.Add("Antenna");
            mCombo.Items.Add("Cable");
        }

        private void InitializeVehichleCombo()
        {
            if (!commonQueries.GetCompanyVehicles(ref vehichles))
                return;

            vehicleComboBox.Items.Clear();

            for (int i = 0; i < vehichles.Count; i++)
            {
                vehicleComboBox.Items.Add(vehichles[i].value);
            }
        }

        private void InitializeMissionStatusCombo()
        {
            if (!commonQueries.GetMissionStatus(ref missionStatusList))
                return;

            statusComboBox.Items.Clear();

            for (int i = 0; i < missionStatusList.Count; i++)
            {
                statusComboBox.Items.Add(missionStatusList[i].value);
            }
        }

        private void InitializeCityCombo()
        {
            if (!commonQueries.GetCities(ref cities))
                return;

            emfCityCombo.Items.Clear();

            for (int i = 0; i < cities.Count; i++)
            {
                emfCityCombo.Items.Add(cities[i].value);
            }
        }
        private void InitializeEngineersCombo()
        {
            if (!commonQueries.GetEngineers(ref engineers))
                return;

            engineer1ComboBox.Items.Clear();
            engineer2ComboBox.Items.Clear();
            engineer3ComboBox.Items.Clear();

            for (int i = 0; i < engineers.Count; i++)
            {
                ComboBoxItem tempItem = new ComboBoxItem();
                tempItem.Content = engineers[i].value;

                ComboBoxItem tempItem1 = new ComboBoxItem();
                tempItem1.Content = engineers[i].value;

                ComboBoxItem tempItem2 = new ComboBoxItem();
                tempItem2.Content = engineers[i].value;

                engineer1ComboBox.Items.Add(tempItem);
                engineer2ComboBox.Items.Add(tempItem1);
                engineer3ComboBox.Items.Add(tempItem2);
            }
        }

        private void InitializeMissionTypeCombo()
        {
            missionTypeCombo.Items.Clear();

            missionTypeCombo.Items.Add("Interference");
            missionTypeCombo.Items.Add("EMF");

            missionTypeCombo.SelectedIndex = 0;
        }
        
        private void InitializeDatePicker(ref DatePicker mDatePicker)
        {
            mDatePicker.SelectedDate = DateTime.Today;
        }

        private void InitializeCompanyCombo(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            for(int i = 0; i < companies.Count; i++)
            {
                mCombo.Items.Add(companies[i].value);
            }
        }

        private bool InitializeComplaintCombo(int mCompanySerial, ref ComboBox mCombo)
        {
            List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> tempComplaints = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            if (!commonQueries.GetCompanyComplaints(mCompanySerial , ref tempComplaints))
                return false;

            mCombo.Items.Clear();

            for(int i = 0; i < tempComplaints.Count; i++)
            {
                mCombo.Items.Add(tempComplaints[i].value);
            }

            mCombo.IsEnabled = true;

            return true;
        }

        private void InitializeSitesCombo(int mCompanySerial, int mComplaintSerial, ref ComboBox mCombo)
        {
            List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> tempSites = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            if (!commonQueries.GetCompanyComplaintSites(mCompanySerial, mComplaintSerial, ref tempSites))
                return;

            mCombo.Items.Clear();

            for(int i = 0; i < tempSites.Count; i++)
            {
                mCombo.Items.Add(tempSites[i].value);
            }
        }

        private void SetUIValues()
        {
            missionTypeCombo.IsEnabled = false;

            for(int i = 0; i < mission.GetEngineers().Count; i++)
            {
                Grid currentGrid = (Grid)mainGrid.Children[i];
                CheckBox currentCheckBox = (CheckBox)currentGrid.Children[0];
                ComboBox currentCombo = (ComboBox)currentGrid.Children[1];

                currentCheckBox.IsChecked = true;
                currentCombo.SelectedIndex = engineers.FindIndex( x1 => x1.key == mission.GetEngineers()[i].key);

                if(missionCondition == BASIC_STRUCTS.MISSION_VIEW_CONDITION)
                {
                    currentCombo.IsEnabled = false;
                }
            }

            if(missionCondition == BASIC_STRUCTS.MISSION_VIEW_CONDITION)
            {
                engineer2CheckBox.IsEnabled = false;
                engineer3CheckBox.IsEnabled = false;
            }

            Image currentAddItemImage = (Image)equipmentStackPanel.Children[equipmentStackPanel.Children.Count - 1];
            Grid currentEquipmentHeaderGrid = (Grid)equipmentStackPanel.Children[0];
            
            equipmentStackPanel.Children.Clear();

            equipmentStackPanel.Children.Add(currentEquipmentHeaderGrid);

            for(int i = 0; i < mission.GetEquipment().Count; i++)
            {
                Grid currentGrid = new Grid();
                currentGrid.Height = 70;
                currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
                currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
                currentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30)});

                if (missionCondition == BASIC_STRUCTS.MISSION_EDIT_CONDITION)
                {
                    ComboBox currentTypeCombo = new ComboBox();
                    currentTypeCombo.Style = (Style)FindResource("miniComboBoxStyle");
                    FillTypeCombo(ref currentTypeCombo);
                    currentTypeCombo.SelectionChanged += OnSelChangedTypeCombo;

                    ComboBox currentItemCombo = new ComboBox();
                    currentItemCombo.Style = (Style)FindResource("miniComboBoxStyle");
                    currentItemCombo.IsEnabled = false;

                    currentGrid.Children.Add(currentTypeCombo);
                    Grid.SetColumn(currentTypeCombo, 0);
                    currentGrid.Children.Add(currentItemCombo);
                    Grid.SetColumn(currentItemCombo, 1);

                    currentTypeCombo.SelectedItem = mission.GetEquipment()[i].type;
                    currentItemCombo.SelectedItem = mission.GetEquipment()[i].equipment;
                }
                else
                {
                    TextBlock currentTypeTextBlock = new TextBlock();
                    currentTypeTextBlock.Style = (Style)FindResource("miniTextblockStyle");
                    currentTypeTextBlock.Text = mission.GetEquipment()[i].type;

                    TextBlock currentItemTextBlock = new TextBlock();
                    currentItemTextBlock.Style = (Style)FindResource("miniTextblockStyle");
                    currentItemTextBlock.Text = mission.GetEquipment()[i].equipment;

                    currentGrid.Children.Add(currentTypeTextBlock);
                    Grid.SetColumn(currentTypeTextBlock, 0);
                    currentGrid.Children.Add(currentItemTextBlock);
                    Grid.SetColumn(currentItemTextBlock, 1);
                }

                if (i != 0 && missionCondition == BASIC_STRUCTS.MISSION_EDIT_CONDITION)
                {
                    Image currentRemoveItemImage = new Image();
                    currentRemoveItemImage.Source = new BitmapImage(new Uri("Photos/red_cross_icon.png", UriKind.Relative));
                    currentRemoveItemImage.Height = 40;
                    currentRemoveItemImage.Width = 40;
                    currentRemoveItemImage.HorizontalAlignment = HorizontalAlignment.Center;
                    currentRemoveItemImage.VerticalAlignment = VerticalAlignment.Center;
                    currentRemoveItemImage.ToolTip = "Remove Site";
                    currentRemoveItemImage.MouseLeftButtonDown += OnClickRemoveItem;

                    currentGrid.Children.Add(currentRemoveItemImage);
                    Grid.SetColumn(currentRemoveItemImage, 2);
                }

                equipmentStackPanel.Children.Add(currentGrid);

            }

            if (missionCondition == BASIC_STRUCTS.MISSION_EDIT_CONDITION)
                equipmentStackPanel.Children.Add(currentAddItemImage);

            vehicleComboBox.SelectedIndex = vehichles.FindIndex(x1 => x1.key == mission.GetVehicleId());
            statusComboBox.SelectedIndex = missionStatusList.FindIndex(x1 => x1.key == mission.GetStatusId());
            missionStartDatePicker.SelectedDate = mission.GetDate();
            missionEndDatePicker.SelectedDate = mission.GetEndDate();
            commentTextBox.Text = mission.GetComment();
            companyComboBox.SelectedIndex = companies.FindIndex(x1 => x1.key == mission.complaint.GetCompanySerial());
            ticketComboBox.SelectedItem = mission.complaint.GetTicketNumber();

            if(missionCondition == BASIC_STRUCTS.MISSION_VIEW_CONDITION)
            {
                vehicleComboBox.IsEnabled = false;
                statusComboBox.IsEnabled = false;
                missionStartDatePicker.IsEnabled = false;
                missionEndDatePicker.IsEnabled = false;
                commentTextBox.IsReadOnly = true;
                companyComboBox.IsEnabled = false;
                ticketComboBox.IsEnabled = false;
            }

            Image addSiteImage = (Image)sitesStacKPanel.Children[sitesStacKPanel.Children.Count - 1];
            sitesStacKPanel.Children.Clear();

            for(int i = 0; i < mission.GetSites().Count; i++)
            {
                Grid newSiteGrid = new Grid();

                newSiteGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });

                newSiteGrid.ColumnDefinitions.Add(new ColumnDefinition());
                newSiteGrid.ColumnDefinitions.Add(new ColumnDefinition());
                newSiteGrid.ColumnDefinitions.Add(new ColumnDefinition());
                newSiteGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

                WrapPanel currentSiteWrapPanel = new WrapPanel();
                currentSiteWrapPanel.VerticalAlignment = VerticalAlignment.Center;
                currentSiteWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;

                Label currentSiteLabel = new Label();
                currentSiteLabel.Style = (Style)FindResource("labelStyleBlack");
                currentSiteLabel.Content = "Site: ";

                ComboBox currentSiteComboBox = new ComboBox();
                currentSiteComboBox.Style = (Style)FindResource("comboBoxStyle");
                currentSiteComboBox.IsEditable = true;
                currentSiteComboBox.IsTextSearchEnabled = true;
                currentSiteComboBox.IsTextSearchCaseSensitive = false;

                for (int j = 0; j < mission.complaint.GetComplaintsSites().Count; j++)
                {
                    currentSiteComboBox.Items.Add(mission.complaint.GetComplaintsSites()[j].site_number);
                }

                currentSiteWrapPanel.Children.Add(currentSiteLabel);
                currentSiteWrapPanel.Children.Add(currentSiteComboBox);

                
                WrapPanel currentInteferenceWrapPanel = new WrapPanel();
                currentInteferenceWrapPanel.VerticalAlignment = VerticalAlignment.Center;
                currentInteferenceWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;

                Label currentInteferenceLabel = new Label();
                currentInteferenceLabel.Style = (Style)FindResource("labelStyleBlack");
                currentInteferenceLabel.Content = reasonOfIntLabel.Content;

                ComboBox currentInteferenceComboBox = new ComboBox();
                currentInteferenceComboBox.Style = (Style)FindResource("comboBoxStyle");
                FillReasonsOfInterferenceCombo(ref currentInteferenceComboBox);

                currentInteferenceWrapPanel.Children.Add(currentInteferenceLabel);
                currentInteferenceWrapPanel.Children.Add(currentInteferenceComboBox);


                WrapPanel currentCommentsWrapPanel = new WrapPanel();
                currentCommentsWrapPanel.VerticalAlignment = VerticalAlignment.Center;
                currentCommentsWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;

                Label currentCommentsLabel = new Label();
                currentCommentsLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                currentCommentsLabel.Content = "Comments: ";

                TextBox currentCommentsTextBox = new TextBox();
                currentCommentsTextBox.Style = (Style)FindResource("largeTextboxStyle");

                currentCommentsWrapPanel.Children.Add(currentCommentsLabel);
                currentCommentsWrapPanel.Children.Add(currentCommentsTextBox);

                newSiteGrid.Children.Add(currentSiteWrapPanel);
                Grid.SetColumn(currentSiteWrapPanel, 0);

                newSiteGrid.Children.Add(currentInteferenceWrapPanel);
                Grid.SetColumn(currentInteferenceWrapPanel, 1);

                newSiteGrid.Children.Add(currentCommentsWrapPanel);
                Grid.SetColumn(currentCommentsWrapPanel, 2);

                if (i != 0 && missionCondition == BASIC_STRUCTS.MISSION_EDIT_CONDITION)
                {
                    Image currentRemoveSiteImage = new Image();
                    currentRemoveSiteImage.Source = new BitmapImage(new Uri("Photos/red_cross_icon.png", UriKind.Relative));
                    currentRemoveSiteImage.Height = 50;
                    currentRemoveSiteImage.Width = 50;
                    currentRemoveSiteImage.ToolTip = "Remove Site";
                    currentRemoveSiteImage.MouseLeftButtonDown += OnClickRemoveSite;
                    currentRemoveSiteImage.Tag = mission.GetSites()[i].site_serial;

                    newSiteGrid.Children.Add(currentRemoveSiteImage);
                    Grid.SetColumn(currentRemoveSiteImage, 3);
                }

                currentSiteComboBox.SelectedItem = mission.GetSites()[i].site_name;
                currentInteferenceComboBox.SelectedItem = mission.GetSites()[i].reason_of_interference;
                currentCommentsTextBox.Text = mission.GetSites()[i].comment;

                if(missionCondition == BASIC_STRUCTS.COMPANY_VIEW_CONDITION)
                {
                    currentSiteComboBox.IsEnabled = false;
                    currentInteferenceComboBox.IsEnabled = false;
                    currentCommentsTextBox.IsReadOnly = true;
                }

                sitesStacKPanel.Children.Add(newSiteGrid);
            }    

            if(missionCondition == BASIC_STRUCTS.MISSION_EDIT_CONDITION)
            {
                sitesStacKPanel.Children.Add(addSiteImage);
            }
        }

        private bool GetCompanies()
        {
            companies.Clear();

            if (!commonQueries.GetCompanies(ref companies))
                return false;

            return true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void OnButtonClickSaveChanges(object sender, RoutedEventArgs e)
        {

            if (missionTypeCombo.SelectedIndex == 0)
            {
                if (engineer1ComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Engineer 1 must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (engineer2CheckBox.IsChecked == true && engineer2ComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Engineer 2 must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (engineer3CheckBox.IsChecked == true && engineer3ComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Engineer 3 must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (vehicleComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Vehicle must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (statusComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Status must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (companyComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Company must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (ticketComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Ticket number must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> tempSelectedEngineers = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

                BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT tempEngineer = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();

                tempEngineer.key = engineers[engineer1ComboBox.SelectedIndex].key;
                tempEngineer.value = engineers[engineer1ComboBox.SelectedIndex].value;

                tempSelectedEngineers.Add(tempEngineer);

                if (engineer2CheckBox.IsChecked == true)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT tempEngineer2 = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();

                    tempEngineer2.key = engineers[engineer2ComboBox.SelectedIndex].key;
                    tempEngineer2.value = engineers[engineer2ComboBox.SelectedIndex].value;

                    tempSelectedEngineers.Add(tempEngineer2);
                }

                if (engineer3CheckBox.IsChecked == true)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT tempEngineer3 = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();

                    tempEngineer3.key = engineers[engineer3ComboBox.SelectedIndex].key;
                    tempEngineer3.value = engineers[engineer3ComboBox.SelectedIndex].value;

                    tempSelectedEngineers.Add(tempEngineer3);
                }

                mission.SetEngineers(tempSelectedEngineers);
                mission.SetDate((DateTime)missionStartDatePicker.SelectedDate);
                mission.SetEndDate((DateTime)missionEndDatePicker.SelectedDate);
                mission.SetComment(commentTextBox.Text);

                if (mission.GetEndDate() < mission.GetDate())
                {
                    MessageBox.Show("Please check mission start and end date!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                List<BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT> tempEquipment = new List<BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT>();

                for (int j = 0; j < equipmentStackPanel.Children.Count - 1; j++)
                {
                    if (j == 0)
                        continue;

                    BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT tempItem = new BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT();

                    Grid currentGrid = (Grid)equipmentStackPanel.Children[j];
                    ComboBox currentTypeCombo = (ComboBox)currentGrid.Children[0];
                    ComboBox currentEquipmentCombo = (ComboBox)currentGrid.Children[1];


                    if (currentTypeCombo.SelectedIndex == -1)
                    {
                        MessageBox.Show("Equipment " + j + " type must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (currentEquipmentCombo.SelectedIndex == -1)
                    {
                        MessageBox.Show("Equipment " + j + " name must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    tempItem.type = currentTypeCombo.Text;
                    tempItem.equipment = currentEquipmentCombo.Text;

                    tempEquipment.Add(tempItem);
                }

                mission.SetEquipment(ref tempEquipment);

                List<BASIC_STRUCTS.MISSION_SITE_STRUCT> tempSites = new List<BASIC_STRUCTS.MISSION_SITE_STRUCT>();

                for (int i = 0; i < sitesStacKPanel.Children.Count - 1; i++)
                {
                    BASIC_STRUCTS.MISSION_SITE_STRUCT currentTempSite = new BASIC_STRUCTS.MISSION_SITE_STRUCT();

                    Grid currentGrid = (Grid)sitesStacKPanel.Children[i];

                    WrapPanel currentSiteWrapPanel = (WrapPanel)currentGrid.Children[0];
                    ComboBox currentSiteCombo = (ComboBox)currentSiteWrapPanel.Children[1];

                    if (currentSiteCombo.SelectedIndex == -1)
                    {
                        MessageBox.Show("Site " + (i + 1) + " must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    WrapPanel currentReasonOfIntWrapPanel = (WrapPanel)currentGrid.Children[1];
                    ComboBox currentReasonOfIntereferenceCombo = (ComboBox)currentReasonOfIntWrapPanel.Children[1];

                    if (currentReasonOfIntereferenceCombo.SelectedIndex == -1)
                    {
                        MessageBox.Show("Reason of interference for site " + (i + 1) + " must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        currentReasonOfIntereferenceCombo.Focusable = true;
                        return;
                    }

                    WrapPanel currentCommentWrapPanel = (WrapPanel)currentGrid.Children[2];
                    TextBox currentCommentTextBox = (TextBox)currentCommentWrapPanel.Children[1];

                    currentTempSite.site_name = mission.complaint.GetComplaintsSites()[currentSiteCombo.SelectedIndex].site_number;
                    currentTempSite.site_serial = mission.complaint.GetComplaintsSites()[currentSiteCombo.SelectedIndex].site_serial;

                    currentTempSite.reason_of_interference = reasonsOfInterference[currentReasonOfIntereferenceCombo.SelectedIndex].value;
                    currentTempSite.reason_of_interference_id = reasonsOfInterference[currentReasonOfIntereferenceCombo.SelectedIndex].key;

                    currentTempSite.comment = currentCommentTextBox.Text;

                    if (tempSites.Exists(x1 => x1.site_name == currentTempSite.site_name))
                    {
                        MessageBox.Show("Site " + currentTempSite.site_name + " has multiple entries!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    tempSites.Add(currentTempSite);
                }

                mission.SetSites(tempSites);

                mission.SetAddedBy(loggedInUser.GetEmployeeId(), loggedInUser.GetEmployeeName());

                if (missionCondition == BASIC_STRUCTS.MISSION_ADD_CONDITION)
                {
                    if (!mission.GetNewSerial())
                        return;

                    if (!mission.GenerateNewMissionId())
                        return;

                    if (!mission.InsertIntoInterferenceMissions())
                        return;

                    if (!mission.InsertIntoMissionEngineers())
                        return;

                    if (!mission.InsertIntoMissionEquipment())
                        return;

                    if (!mission.InsertIntoMissionSites())
                        return;

                    for(int i = 0; i < mission.GetSites().Count; i++)
                    {
                        if (!mission.complaint.ConfirmSiteStatus(mission.GetSites()[i].site_serial, mission.GetStatusId()))
                            return;
                    }

                    if (!mission.complaint.UpdateComplaintStatus())
                        return;
                }
                else if(missionCondition == BASIC_STRUCTS.MISSION_EDIT_CONDITION)
                {
                    if (!mission.UpdateMission())
                        return;
                    if (!mission.DeleteEngineers())
                        return;
                    if (!mission.DeleteEquipment())
                        return;
                    if (!mission.DeleteSites())
                        return;
                    if (!mission.InsertIntoMissionEngineers())
                        return;
                    if (!mission.InsertIntoMissionEquipment())
                        return;
                    if (!mission.InsertIntoMissionSites())
                        return;

                    for (int i = 0; i < mission.GetSites().Count; i++)
                    {
                        if (!mission.complaint.ConfirmSiteStatus(mission.GetSites()[i].site_serial, mission.GetStatusId()))
                            return;
                    }

                    //for (int i = 0; i < removedSiteSerials.Count; i++)
                    //{
                    //    if (!mission.complaint.CheckSiteStatusConfirmed(removedSiteSerials[i]))
                    //    {
                    //        if (!mission.complaint.UpdateSiteStatus(removedSiteSerials[i], BASIC_STRUCTS.PENDING_SITE_STATUS))
                    //            return;
                    //    }
                    //}

                    if (!mission.complaint.UpdateComplaintStatus())
                        return;
                }
            }

            this.Close();
        }

        private void OnSelChangedMissionTypeCombo(object sender, SelectionChangedEventArgs e)
        {
            if(missionTypeCombo.SelectedIndex == 0)
            {
                addMeasurementLocationButton.IsEnabled = false;
                emfScrollViewer.Visibility = Visibility.Collapsed;
                interferenceScrollViewer.Visibility = Visibility.Visible;
            }
            else if(missionTypeCombo.SelectedIndex == 1)
            {
                addMeasurementLocationButton.IsEnabled = true;
                emfScrollViewer.Visibility = Visibility.Visible;
                interferenceScrollViewer.Visibility = Visibility.Collapsed;
            }
        }

        private void OnSelChangedTicketCombo(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentTicketCombo = (ComboBox)sender;

            if (currentTicketCombo.SelectedIndex != -1)
            {
                WrapPanel currentTicketWrapPanel = (WrapPanel)currentTicketCombo.Parent;
                Grid currentGrid = (Grid)currentTicketWrapPanel.Parent;

                WrapPanel currentCompanyWrapPanel = (WrapPanel)currentGrid.Children[0];
                ComboBox currentCompanyCombo = (ComboBox)currentCompanyWrapPanel.Children[1];

                List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> tempComplaints = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

                if (!commonQueries.GetCompanyComplaints(companies[currentCompanyCombo.SelectedIndex].key, ref tempComplaints))
                    return;

                if (!mission.complaint.InitializeComplaint(mission.complaint.GetCompanySerial(), tempComplaints[currentTicketCombo.SelectedIndex].key))
                    return;

                for(int i = 0; i < sitesStacKPanel.Children.Count - 1; i++)
                {
                    Grid currentSiteGrid = (Grid)sitesStacKPanel.Children[i];
                    WrapPanel currentSiteWrapPanel = (WrapPanel)currentSiteGrid.Children[0];
                    ComboBox currentSiteCombo = (ComboBox)currentSiteWrapPanel.Children[1];

                    currentSiteCombo.Items.Clear();

                    for(int j = 0; j < mission.complaint.GetComplaintsSites().Count; j++)
                    {
                        currentSiteCombo.Items.Add(mission.complaint.GetComplaintsSites()[j].site_number);
                    }

                    currentSiteCombo.IsEnabled = true; 

                }
            }
            else
            {
                for (int i = 0; i < sitesStacKPanel.Children.Count - 1; i++)
                {
                    Grid currentSiteGrid = (Grid)sitesStacKPanel.Children[i];
                    WrapPanel currentSiteWrapPanel = (WrapPanel)currentSiteGrid.Children[0];
                    ComboBox currentSiteCombo = (ComboBox)currentSiteWrapPanel.Children[1];

                    currentSiteCombo.Items.Clear();

                    currentSiteCombo.IsEnabled = false;
                }
            }
        }

        private void OnSelChangedCompanyCombo(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentCompanyCombo = (ComboBox)sender;

            if (currentCompanyCombo.SelectedIndex != -1)
            {
                WrapPanel currentWrapPanel = (WrapPanel)currentCompanyCombo.Parent;
                Grid currentGrid = (Grid)currentWrapPanel.Parent;

                WrapPanel currentComplaintWrapPanel = (WrapPanel)currentGrid.Children[1];
                ComboBox currentComplaintCombo = (ComboBox)currentComplaintWrapPanel.Children[1];
                currentComplaintCombo.IsEnabled = true;
                
                if (!InitializeComplaintCombo(companies[currentCompanyCombo.SelectedIndex].key, ref currentComplaintCombo))
                    return;

                mission.complaint.SetCompanySerial(companies[currentCompanyCombo.SelectedIndex].key);
                mission.complaint.SetCompanyName(companies[currentCompanyCombo.SelectedIndex].value);
            }
        }

        private void OnCheckEngineer2(object sender, RoutedEventArgs e)
        {
            if(engineer1ComboBox.SelectedIndex != -1)
                engineer2ComboBox.Visibility = Visibility.Visible;
            else
            {
                MessageBox.Show("Engineer 1 must be selected first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                engineer2CheckBox.IsChecked = false;
            }
        }

        private void OnUncheckEngineer2(object sender, RoutedEventArgs e)
        {
            engineer2ComboBox.SelectedIndex = -1;
            engineer2ComboBox.Visibility = Visibility.Collapsed;
        }

        private void OnCheckEngineer3(object sender, RoutedEventArgs e)
        {
            if (engineer2CheckBox.IsChecked == true && engineer2ComboBox.SelectedIndex != -1)
            {
                engineer3ComboBox.Visibility = Visibility.Visible;
            }
            else if(engineer2CheckBox.IsChecked == true && engineer2ComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Engineer 2 must be selected first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                engineer3CheckBox.IsChecked = false;
            }
            else
            {
                engineer3CheckBox.IsChecked = false;
                engineer2CheckBox.IsChecked = true;
            }
        }

        private void OnUncheckEngineer3(object sender, RoutedEventArgs e)
        {
            engineer3ComboBox.SelectedIndex = -1;
            engineer3ComboBox.Visibility = Visibility.Collapsed;
        }

       
        private void OnSelChangedVehicleCombo(object sender, SelectionChangedEventArgs e)
        {
            if(vehicleComboBox.SelectedIndex != -1)
            {
                mission.SetVehicle(vehichles[vehicleComboBox.SelectedIndex].value);
                mission.SetVehicleId(vehichles[vehicleComboBox.SelectedIndex].key);
            }
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            if(statusComboBox.SelectedIndex != -1)
            {
                mission.SetStatus(missionStatusList[statusComboBox.SelectedIndex].value);
                mission.SetStatusId(missionStatusList[statusComboBox.SelectedIndex].key);
            }
        }

        private void OnClickAddSite(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;

            StackPanel currentStackPanel = (StackPanel)currentImage.Parent;

            currentStackPanel.Children.Remove(currentImage);

            Grid newSiteGrid = new Grid();

            newSiteGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100)});

            newSiteGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newSiteGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newSiteGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newSiteGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50)});

            WrapPanel currentSiteWrapPanel = new WrapPanel();
            currentSiteWrapPanel.VerticalAlignment = VerticalAlignment.Center;
            currentSiteWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;

            Label currentSiteLabel = new Label();
            currentSiteLabel.Style = (Style)FindResource("labelStyleBlack");
            currentSiteLabel.Content = "Site: ";
            
            ComboBox currentSiteComboBox = new ComboBox();
            currentSiteComboBox.Style = (Style)FindResource("comboBoxStyle");
            currentSiteComboBox.IsEditable = true;
            currentSiteComboBox.IsTextSearchEnabled = true;
            currentSiteComboBox.IsTextSearchCaseSensitive = false;

            for(int i = 0; i < mission.complaint.GetComplaintsSites().Count; i++)
            {
                currentSiteComboBox.Items.Add(mission.complaint.GetComplaintsSites()[i].site_number);
            }

            currentSiteWrapPanel.Children.Add(currentSiteLabel);
            currentSiteWrapPanel.Children.Add(currentSiteComboBox);


            WrapPanel currentInteferenceWrapPanel = new WrapPanel();
            currentInteferenceWrapPanel.VerticalAlignment = VerticalAlignment.Center;
            currentInteferenceWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;

            Label currentInteferenceLabel = new Label();
            currentInteferenceLabel.Style = (Style)FindResource("labelStyleBlack");
            currentInteferenceLabel.Content = reasonOfIntLabel.Content;

            ComboBox currentInteferenceComboBox = new ComboBox();
            currentInteferenceComboBox.Style = (Style)FindResource("comboBoxStyle");
            FillReasonsOfInterferenceCombo(ref currentInteferenceComboBox);

            currentInteferenceWrapPanel.Children.Add(currentInteferenceLabel);
            currentInteferenceWrapPanel.Children.Add(currentInteferenceComboBox);


            WrapPanel currentCommentsWrapPanel = new WrapPanel();
            currentCommentsWrapPanel.VerticalAlignment = VerticalAlignment.Center;
            currentCommentsWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;

            Label currentCommentsLabel = new Label();
            currentCommentsLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
            currentCommentsLabel.Content = "Comments: ";

            TextBox currentCommentsTextBox = new TextBox();
            currentCommentsTextBox.Style = (Style)FindResource("largeTextboxStyle");

            currentCommentsWrapPanel.Children.Add(currentCommentsLabel);
            currentCommentsWrapPanel.Children.Add(currentCommentsTextBox);


            Image currentRemoveSiteImage = new Image();
            currentRemoveSiteImage.Source = new BitmapImage(new Uri("Photos/red_cross_icon.png", UriKind.Relative));
            currentRemoveSiteImage.Height = 50;
            currentRemoveSiteImage.Width = 50;
            currentRemoveSiteImage.ToolTip = "Remove Site";
            currentRemoveSiteImage.MouseLeftButtonDown += OnClickRemoveSite;

            newSiteGrid.Children.Add(currentSiteWrapPanel);
            Grid.SetColumn(currentSiteWrapPanel, 0);
            
            newSiteGrid.Children.Add(currentInteferenceWrapPanel);
            Grid.SetColumn(currentInteferenceWrapPanel, 1);
            
            newSiteGrid.Children.Add(currentCommentsWrapPanel);
            Grid.SetColumn(currentCommentsWrapPanel, 2);

            newSiteGrid.Children.Add(currentRemoveSiteImage);
            Grid.SetColumn(currentRemoveSiteImage, 3);

            currentStackPanel.Children.Add(newSiteGrid);
            currentStackPanel.Children.Add(currentImage);

            currentImage.BringIntoView();
        }

        private void OnClickRemoveSite(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;

            Grid currentSiteGrid = (Grid)currentImage.Parent;
            StackPanel currentStackPanel = (StackPanel)currentSiteGrid.Parent;

            currentStackPanel.Children.Remove(currentSiteGrid);

            if(currentImage.Tag != null && currentImage.Tag.ToString() != "")
            {
                removedSiteSerials.Add(int.Parse(currentImage.Tag.ToString()));
            }
        }

        private void OnSelChangedEngineer1Combo(object sender, SelectionChangedEventArgs e)
        {
            CheckSelectedEngineer();
        }

        private void OnSelChangedEngineer2Combo(object sender, SelectionChangedEventArgs e)
        {
            CheckSelectedEngineer();
        }

        private void OnSelChangedEngineer3Combo(object sender, SelectionChangedEventArgs e)
        {
            CheckSelectedEngineer();
        }

        private void CheckSelectedEngineer()
        {
            for(int i = 0; i < engineer1ComboBox.Items.Count; i++)
            {
                ComboBoxItem tempItem = (ComboBoxItem)engineer1ComboBox.Items[i];
                tempItem.IsEnabled = true;
                ComboBoxItem tempItem1 = (ComboBoxItem)engineer2ComboBox.Items[i];
                tempItem1.IsEnabled = true;
                ComboBoxItem tempItem2 = (ComboBoxItem)engineer3ComboBox.Items[i];
                tempItem2.IsEnabled = true;
            }

            if (engineer1ComboBox.SelectedIndex != -1)
            {
                ComboBoxItem tempItem = (ComboBoxItem)engineer2ComboBox.Items[engineer1ComboBox.SelectedIndex];
                tempItem.IsEnabled = false;

                ComboBoxItem tempItem1 = (ComboBoxItem)engineer3ComboBox.Items[engineer1ComboBox.SelectedIndex];
                tempItem1.IsEnabled = false;
            }

            if (engineer2ComboBox.SelectedIndex != -1)
            {
                ComboBoxItem tempItem = (ComboBoxItem)engineer1ComboBox.Items[engineer2ComboBox.SelectedIndex];
                tempItem.IsEnabled = false;

                ComboBoxItem tempItem1 = (ComboBoxItem)engineer3ComboBox.Items[engineer2ComboBox.SelectedIndex];
                tempItem1.IsEnabled = false;
            }

            if (engineer3ComboBox.SelectedIndex != -1)
            {
                ComboBoxItem tempItem = (ComboBoxItem)engineer1ComboBox.Items[engineer2ComboBox.SelectedIndex];
                tempItem.IsEnabled = false;

                ComboBoxItem tempItem1 = (ComboBoxItem)engineer3ComboBox.Items[engineer2ComboBox.SelectedIndex];
                tempItem1.IsEnabled = false;
            }
        }

        private void OnLostFocusTicketNumberCombo(object sender, RoutedEventArgs e)
        {
            ComboBox currentComboBox = (ComboBox)sender;

            if (currentComboBox.SelectedIndex != -1)
            {
                if (currentComboBox.Text != currentComboBox.SelectedItem.ToString())
                {
                    MessageBox.Show("Ticket Number Must be selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if(currentComboBox.Text != "")
            {
                MessageBox.Show("Ticket Number Must be selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void OnCheckBroadBandCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Parent;

            WrapPanel currentWrapPanel = new WrapPanel();
            currentWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            currentWrapPanel.VerticalAlignment = VerticalAlignment.Center;
            currentWrapPanel.Tag = "BroadBand";
            currentWrapPanel.Height = 50;

            Label currentBandLabel = new Label();
            currentBandLabel.Content = "BroadBand";
            currentBandLabel.Style = (Style)FindResource("labelStyle");
            currentBandLabel.Width = 300;

            Label currentAveragePowerLabel = new Label();
            currentAveragePowerLabel.Style = (Style)FindResource("wideLabelStyleBlack");
            currentAveragePowerLabel.Content = "Average Power Density:";
            currentAveragePowerLabel.Width = 300;

            TextBox currentAveragePowerTextBox = new TextBox();
            currentAveragePowerTextBox.Style = (Style)FindResource("miniTextboxStyle");

            Label currentMaxPowerLabel = new Label();
            currentMaxPowerLabel.Style = (Style)FindResource("wideLabelStyleBlack");
            currentMaxPowerLabel.Content = "Max Power Density:";
            currentMaxPowerLabel.Width = 300;

            TextBox currentMaxPowerTextBox = new TextBox();
            currentMaxPowerTextBox.Style = (Style)FindResource("miniTextboxStyle");

            currentWrapPanel.Children.Add(currentBandLabel);
            currentWrapPanel.Children.Add(currentAveragePowerLabel);
            currentWrapPanel.Children.Add(currentAveragePowerTextBox);
            currentWrapPanel.Children.Add(currentMaxPowerLabel);
            currentWrapPanel.Children.Add(currentMaxPowerTextBox);

            currentStackPanel.Children.Add(currentWrapPanel);
        }

        private void OnUncheckBroadBandCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Parent;

            for(int i = 1; i < currentStackPanel.Children.Count; i++)
            {
                WrapPanel child = (WrapPanel)currentStackPanel.Children[i];

                if(child.Tag.ToString() == "BroadBand")
                {
                    currentStackPanel.Children.Remove(child);
                    break;
                }
            }

        }

        private void OnCheck700CheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Parent;

            WrapPanel currentWrapPanel = new WrapPanel();
            currentWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            currentWrapPanel.VerticalAlignment = VerticalAlignment.Center;
            currentWrapPanel.Tag = "700";
            currentWrapPanel.Height = 50;

            Label currentBandLabel = new Label();
            currentBandLabel.Content = "Band 700";
            currentBandLabel.Style = (Style)FindResource("labelStyle");
            currentBandLabel.Width = 300;

            Label currentAveragePowerLabel = new Label();
            currentAveragePowerLabel.Style = (Style)FindResource("wideLabelStyleBlack");
            currentAveragePowerLabel.Content = "Average Power Density:";
            currentAveragePowerLabel.Width = 300;

            TextBox currentAveragePowerTextBox = new TextBox();
            currentAveragePowerTextBox.Style = (Style)FindResource("miniTextboxStyle");

            Label currentMaxPowerLabel = new Label();
            currentMaxPowerLabel.Style = (Style)FindResource("wideLabelStyleBlack");
            currentMaxPowerLabel.Content = "Max Power Density:";
            currentMaxPowerLabel.Width = 300;

            TextBox currentMaxPowerTextBox = new TextBox();
            currentMaxPowerTextBox.Style = (Style)FindResource("miniTextboxStyle");

            currentWrapPanel.Children.Add(currentBandLabel);
            currentWrapPanel.Children.Add(currentAveragePowerLabel);
            currentWrapPanel.Children.Add(currentAveragePowerTextBox);
            currentWrapPanel.Children.Add(currentMaxPowerLabel);
            currentWrapPanel.Children.Add(currentMaxPowerTextBox);

            currentStackPanel.Children.Add(currentWrapPanel);
        }

        private void OnUncheck700CheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Parent;

            for (int i = 1; i < currentStackPanel.Children.Count; i++)
            {
                WrapPanel child = (WrapPanel)currentStackPanel.Children[i];

                if (child.Tag.ToString() == "700")
                {
                    currentStackPanel.Children.Remove(child);
                    break;
                }
            }
        }

        private void OnCheck900CheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Parent;

            WrapPanel currentWrapPanel = new WrapPanel();
            currentWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            currentWrapPanel.VerticalAlignment = VerticalAlignment.Center;
            currentWrapPanel.Tag = "900";
            currentWrapPanel.Height = 50;

            Label currentBandLabel = new Label();
            currentBandLabel.Content = "Band 900";
            currentBandLabel.Style = (Style)FindResource("labelStyle");
            currentBandLabel.Width = 300;

            Label currentAveragePowerLabel = new Label();
            currentAveragePowerLabel.Style = (Style)FindResource("wideLabelStyleBlack");
            currentAveragePowerLabel.Content = "Average Power Density:";
            currentAveragePowerLabel.Width = 300;

            TextBox currentAveragePowerTextBox = new TextBox();
            currentAveragePowerTextBox.Style = (Style)FindResource("miniTextboxStyle");

            Label currentMaxPowerLabel = new Label();
            currentMaxPowerLabel.Style = (Style)FindResource("wideLabelStyleBlack");
            currentMaxPowerLabel.Content = "Max Power Density:";
            currentMaxPowerLabel.Width = 300;

            TextBox currentMaxPowerTextBox = new TextBox();
            currentMaxPowerTextBox.Style = (Style)FindResource("miniTextboxStyle");

            currentWrapPanel.Children.Add(currentBandLabel);
            currentWrapPanel.Children.Add(currentAveragePowerLabel);
            currentWrapPanel.Children.Add(currentAveragePowerTextBox);
            currentWrapPanel.Children.Add(currentMaxPowerLabel);
            currentWrapPanel.Children.Add(currentMaxPowerTextBox);

            currentStackPanel.Children.Add(currentWrapPanel);
        }

        private void OnUncheck900CheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Parent;

            for (int i = 1; i < currentStackPanel.Children.Count; i++)
            {
                WrapPanel child = (WrapPanel)currentStackPanel.Children[i];

                if (child.Tag.ToString() == "900")
                {
                    currentStackPanel.Children.Remove(child);
                    break;
                }
            }
        }

        private void OnCheck1800CheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Parent;

            WrapPanel currentWrapPanel = new WrapPanel();
            currentWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            currentWrapPanel.VerticalAlignment = VerticalAlignment.Center;
            currentWrapPanel.Tag = "1800";
            currentWrapPanel.Height = 50;

            Label currentBandLabel = new Label();
            currentBandLabel.Content = "Band 1800";
            currentBandLabel.Style = (Style)FindResource("labelStyle");
            currentBandLabel.Width = 300;

            Label currentAveragePowerLabel = new Label();
            currentAveragePowerLabel.Style = (Style)FindResource("wideLabelStyleBlack");
            currentAveragePowerLabel.Content = "Average Power Density:";
            currentAveragePowerLabel.Width = 300;

            TextBox currentAveragePowerTextBox = new TextBox();
            currentAveragePowerTextBox.Style = (Style)FindResource("miniTextboxStyle");

            Label currentMaxPowerLabel = new Label();
            currentMaxPowerLabel.Style = (Style)FindResource("wideLabelStyleBlack");
            currentMaxPowerLabel.Content = "Max Power Density:";
            currentMaxPowerLabel.Width = 300;

            TextBox currentMaxPowerTextBox = new TextBox();
            currentMaxPowerTextBox.Style = (Style)FindResource("miniTextboxStyle");

            currentWrapPanel.Children.Add(currentBandLabel);
            currentWrapPanel.Children.Add(currentAveragePowerLabel);
            currentWrapPanel.Children.Add(currentAveragePowerTextBox);
            currentWrapPanel.Children.Add(currentMaxPowerLabel);
            currentWrapPanel.Children.Add(currentMaxPowerTextBox);

            currentStackPanel.Children.Add(currentWrapPanel);
        }

        private void OnUncheck1800CheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Parent;

            for (int i = 1; i < currentStackPanel.Children.Count; i++)
            {
                WrapPanel child = (WrapPanel)currentStackPanel.Children[i];

                if (child.Tag.ToString() == "1800")
                {
                    currentStackPanel.Children.Remove(child);
                    break;
                }
            }
        }

        private void OnCheck2100CheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Parent;

            WrapPanel currentWrapPanel = new WrapPanel();
            currentWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            currentWrapPanel.VerticalAlignment = VerticalAlignment.Center;
            currentWrapPanel.Tag = "2100";
            currentWrapPanel.Height = 50;

            Label currentBandLabel = new Label();
            currentBandLabel.Content = "Band 2100";
            currentBandLabel.Style = (Style)FindResource("labelStyle");
            currentBandLabel.Width = 300;

            Label currentAveragePowerLabel = new Label();
            currentAveragePowerLabel.Style = (Style)FindResource("wideLabelStyleBlack");
            currentAveragePowerLabel.Content = "Average Power Density:";
            currentAveragePowerLabel.Width = 300;

            TextBox currentAveragePowerTextBox = new TextBox();
            currentAveragePowerTextBox.Style = (Style)FindResource("miniTextboxStyle");

            Label currentMaxPowerLabel = new Label();
            currentMaxPowerLabel.Style = (Style)FindResource("wideLabelStyleBlack");
            currentMaxPowerLabel.Content = "Max Power Density:";
            currentMaxPowerLabel.Width = 300;

            TextBox currentMaxPowerTextBox = new TextBox();
            currentMaxPowerTextBox.Style = (Style)FindResource("miniTextboxStyle");

            currentWrapPanel.Children.Add(currentBandLabel);
            currentWrapPanel.Children.Add(currentAveragePowerLabel);
            currentWrapPanel.Children.Add(currentAveragePowerTextBox);
            currentWrapPanel.Children.Add(currentMaxPowerLabel);
            currentWrapPanel.Children.Add(currentMaxPowerTextBox);

            currentStackPanel.Children.Add(currentWrapPanel);
        }

        private void OnUncheck2100CheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Parent;

            for (int i = 1; i < currentStackPanel.Children.Count; i++)
            {
                WrapPanel child = (WrapPanel)currentStackPanel.Children[i];

                if (child.Tag.ToString() == "2100")
                {
                    currentStackPanel.Children.Remove(child);
                    break;
                }
            }
        }

        private void OnCheck2600CheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Parent;

            WrapPanel currentWrapPanel = new WrapPanel();
            currentWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            currentWrapPanel.VerticalAlignment = VerticalAlignment.Center;
            currentWrapPanel.Tag = "2600";
            currentWrapPanel.Height = 50;

            Label currentBandLabel = new Label();
            currentBandLabel.Content = "Band 2600";
            currentBandLabel.Style = (Style)FindResource("labelStyle");
            currentBandLabel.Width = 300;

            Label currentAveragePowerLabel = new Label();
            currentAveragePowerLabel.Style = (Style)FindResource("wideLabelStyleBlack");
            currentAveragePowerLabel.Content = "Average Power Density:";
            currentAveragePowerLabel.Width = 300;

            TextBox currentAveragePowerTextBox = new TextBox();
            currentAveragePowerTextBox.Style = (Style)FindResource("miniTextboxStyle");

            Label currentMaxPowerLabel = new Label();
            currentMaxPowerLabel.Style = (Style)FindResource("wideLabelStyleBlack");
            currentMaxPowerLabel.Content = "Max Power Density:";
            currentMaxPowerLabel.Width = 300;

            TextBox currentMaxPowerTextBox = new TextBox();
            currentMaxPowerTextBox.Style = (Style)FindResource("miniTextboxStyle");

            currentWrapPanel.Children.Add(currentBandLabel);
            currentWrapPanel.Children.Add(currentAveragePowerLabel);
            currentWrapPanel.Children.Add(currentAveragePowerTextBox);
            currentWrapPanel.Children.Add(currentMaxPowerLabel);
            currentWrapPanel.Children.Add(currentMaxPowerTextBox);

            currentStackPanel.Children.Add(currentWrapPanel);
        }

        private void OnUncheck2600CheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            Grid currentGrid = (Grid)currentCheckBox.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Parent;

            for (int i = 1; i < currentStackPanel.Children.Count; i++)
            {
                WrapPanel child = (WrapPanel)currentStackPanel.Children[i];

                if (child.Tag.ToString() == "2600")
                {
                    currentStackPanel.Children.Remove(child);
                    break;
                }
            }
        }

        private void OnButtonClickAddLocation(object sender, RoutedEventArgs e)
        {

        }

        private void OnClickAddEquipment(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            StackPanel currentStackPanel = (StackPanel)currentImage.Parent;

            Image currentAddItemImage = (Image)currentStackPanel.Children[currentStackPanel.Children.Count - 1];
            currentStackPanel.Children.Remove(currentAddItemImage);

            Grid currentGrid = new Grid();
            currentGrid.Height = 70;
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30)});

            ComboBox currentTypeCombo = new ComboBox();
            currentTypeCombo.Style = (Style)FindResource("miniComboBoxStyle");
            FillTypeCombo(ref currentTypeCombo);
            currentTypeCombo.SelectionChanged += OnSelChangedTypeCombo;

            ComboBox currentItemCombo = new ComboBox();
            currentItemCombo.Style = (Style)FindResource("miniComboBoxStyle");
            currentItemCombo.IsEnabled = false;

            Image currentRemoveItemImage = new Image();
            currentRemoveItemImage.Source = new BitmapImage(new Uri("Photos/red_cross_icon.png", UriKind.Relative));
            currentRemoveItemImage.Height = 40;
            currentRemoveItemImage.Width = 40;
            currentRemoveItemImage.HorizontalAlignment = HorizontalAlignment.Center;
            currentRemoveItemImage.VerticalAlignment = VerticalAlignment.Center;
            currentRemoveItemImage.ToolTip = "Remove Site";
            currentRemoveItemImage.MouseLeftButtonDown += OnClickRemoveItem;

            currentGrid.Children.Add(currentTypeCombo);
            Grid.SetColumn(currentTypeCombo, 0);
            currentGrid.Children.Add(currentItemCombo);
            Grid.SetColumn(currentItemCombo, 1);
            currentGrid.Children.Add(currentRemoveItemImage);
            Grid.SetColumn(currentRemoveItemImage, 2);

            currentStackPanel.Children.Add(currentGrid);
            currentStackPanel.Children.Add(currentAddItemImage);
        }

        private void OnSelChangedTypeCombo(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentTypeCombo = (ComboBox)sender;

            if (currentTypeCombo.SelectedIndex != -1)
            {
                Grid currentGrid = (Grid)currentTypeCombo.Parent;
                ComboBox currentItemCombo = (ComboBox)currentGrid.Children[1];
                currentItemCombo.IsEnabled = true;

                if (currentTypeCombo.SelectedIndex == 0)
                {
                    FillHandheld(ref currentItemCombo);
                }
                else if (currentTypeCombo.SelectedIndex == 1)
                {
                    FillAntennaCombo(ref currentItemCombo);
                }
                else if (currentTypeCombo.SelectedIndex == 2)
                {
                    FillCableCombo(ref currentItemCombo);
                }
            }
        }

        private void OnClickRemoveItem(object sender, MouseButtonEventArgs e)
        {
            Image currentItemImage = (Image)sender;
            Grid currentGrid = (Grid)currentItemImage.Parent;
            StackPanel currentStackPanel = (StackPanel)currentGrid.Parent;

            currentStackPanel.Children.Remove(currentGrid);
        }
    }
}
