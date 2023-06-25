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
using System.Windows.Shapes;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for CompanyWindow.xaml
    /// </summary>
    public partial class CompanyWindow : Window
    {
        Employee loggedInUser;
        Company company;
        CommonQueries commonQueries;

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> workFields;
        private List<BASIC_STRUCTS.CONTACT_STRUCT> companyContacts;

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> bands;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> bandsUnits;

        private int companyCondition;

        public CompanyWindow(ref Employee mLoggedInUser, ref Company mCompany, ref int mCompanyCondition)
        {
            loggedInUser = mLoggedInUser;
            company = mCompany;
            companyCondition = mCompanyCondition;

            commonQueries = new CommonQueries();

            workFields = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            companyContacts = new List<BASIC_STRUCTS.CONTACT_STRUCT>();
            bands = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            bandsUnits = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            InitializeComponent();

            GetBands();
            GetBandUnits();
            InitializeFieldOfWorkCombo();

            SetUIElements();

            if(companyCondition == BASIC_STRUCTS.COMPANY_ADD_CONDITION)
            {
                bandsCheckBox.IsChecked = true;
            }
            else
            {
                companyNameTextBox.IsEnabled = false;
                workfieldComboBox.IsEnabled = false;
            }

            if(companyCondition == BASIC_STRUCTS.COMPANY_VIEW_CONDITION)
            {
                DisableUIElements();
            }

        }

        private void SetUIElements()
        {
            companyNameTextBox.Text = company.GetCompanyName();
            workfieldComboBox.SelectedIndex = workFields.FindIndex(x1 => x1.key == company.GetCompanyFieldOfWorkId());

            if (company.GetCompanyContacts().Count != 0)
            {
                contact1CheckBox.IsChecked = true;
                contact1NameTextBox.Text = company.GetCompanyContacts()[0].contact_name;
                contact1EmailTextBox.Text = company.GetCompanyContacts()[0].contact_email;
                contact1NumberTextBox.Text = company.GetCompanyContacts()[0].contact_phone;
            }
            if (company.GetCompanyContacts().Count > 1)
            {
                contact2CheckBox.IsChecked = true;
                contact2NameTextBox.Text = company.GetCompanyContacts()[1].contact_name;
                contact2EmailTextBox.Text = company.GetCompanyContacts()[1].contact_email;
                contact2NumberTextBox.Text = company.GetCompanyContacts()[1].contact_phone;
            }
            if (company.GetCompanyContacts().Count > 2)
            {
                contact3CheckBox.IsChecked = true;
                contact3NameTextBox.Text = company.GetCompanyContacts()[2].contact_name;
                contact3EmailTextBox.Text = company.GetCompanyContacts()[2].contact_email;
                contact3NumberTextBox.Text = company.GetCompanyContacts()[2].contact_phone;
            }

            if (company.GetCompanyStartFrequencies().Count != 0)
            {
                bandsCheckBox.IsChecked = true;

                bandWidthCheckBox.IsEnabled = false;
                bandsCheckBox.IsEnabled = false;

                centreFreqTextBox.Text = company.GetCompanyStartFrequencies()[0].start_frequency.ToString();
                centreFreqComboBox.Text = company.GetCompanyStartFrequencies()[0].start_frequency_unit;

                bandwidthTextBox.Text = company.GetCompanyStartFrequencies()[0].stop_frequency.ToString();
                bandwidthComboBox.Text = company.GetCompanyStartFrequencies()[0].stop_frequency_unit;

                for (int i = 1; i < company.GetCompanyStartFrequencies().Count; i++)
                {
                    Grid currentGrid = new Grid();
                    AddFrequency(ref currentGrid, i);
                    freqBandsGrid.Children.Add(currentGrid);

                }
            }
            else if(company.GetCompanyCFandBw().Count != 0)
            {
                bandWidthCheckBox.IsChecked = true;

                bandWidthCheckBox.IsEnabled = false;
                bandsCheckBox.IsEnabled = false;

                centreFreqTextBox.Text = company.GetCompanyCFandBw()[0].centre_frequency.ToString();
                centreFreqComboBox.Text = company.GetCompanyCFandBw()[0].centre_frequency_unit;

                bandwidthTextBox.Text = company.GetCompanyCFandBw()[0].bandwidth.ToString();
                bandwidthComboBox.Text = company.GetCompanyCFandBw()[0].bandwidth_unit;

                for (int i = 1; i < company.GetCompanyCFandBw().Count; i++)
                {
                    Grid currentGrid = new Grid();
                    AddFrequency(ref currentGrid, i);
                    freqBandsGrid.Children.Add(currentGrid);
                }
            }


        }

        private void DisableUIElements()
        {
            finishButton.IsEnabled = false;
            addFrequencyButton.IsEnabled = false;

            centreFreqTextBox.IsEnabled = false;
            centreFreqComboBox.IsEnabled = false;
            bandwidthTextBox.IsEnabled = false;
            bandwidthComboBox.IsEnabled = false;

            companyNameTextBox.IsReadOnly = true;
            workfieldComboBox.IsEnabled = false;

            contact1CheckBox.IsEnabled = false;
            contact2CheckBox.IsEnabled = false;
            contact3CheckBox.IsEnabled = false;

            contact1NameTextBox.IsReadOnly = true;
            contact1EmailTextBox.IsReadOnly = true;
            contact1NumberTextBox.IsReadOnly = true;

            contact2NameTextBox.IsReadOnly = true;
            contact2EmailTextBox.IsReadOnly = true;
            contact2NumberTextBox.IsReadOnly = true;

            contact3NameTextBox.IsReadOnly = true;
            contact3EmailTextBox.IsReadOnly = true;
            contact3NumberTextBox.IsReadOnly = true;

        }

        private bool GetBandUnits()
        {
            if (!commonQueries.GetBandUnits(ref bandsUnits))
                return false;

            for(int i = 0; i < bandsUnits.Count; i++)
            {
                centreFreqComboBox.Items.Add(bandsUnits[i].value);
                bandwidthComboBox.Items.Add(bandsUnits[i].value);
            }

            centreFreqComboBox.SelectedIndex = 1;
            bandwidthComboBox.SelectedIndex = 1;

            return true;
        }

        private void FillBandUnits(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            for (int i = 0; i < bandsUnits.Count; i++)
            {
                mCombo.Items.Add(bandsUnits[i].value);
            }

            mCombo.SelectedIndex = 1;
        }

        private bool GetBands()
        {

            if (!commonQueries.GetBands(ref bands))
                return false;

            return true;
        }


        private bool InitializeFieldOfWorkCombo()
        {
            if(!commonQueries.GetCompanyFieldOfWork(ref workFields))
                return false;

            for(int i = 0; i < workFields.Count; i++)
            {
                workfieldComboBox.Items.Add(workFields[i].value);
            }

            return true;
        }

        private void OnCheckContact1CheckBox(object sender, RoutedEventArgs e)
        {
            contact1NameWrapPanel.Visibility = Visibility.Visible;
            contact1NumberWrapPanel.Visibility = Visibility.Visible;
            contact1EmailWrapPanel.Visibility = Visibility.Visible;
        }

        private void OnUncheckContact1CheckBox(object sender, RoutedEventArgs e)
        {
            if (contact2CheckBox.IsChecked == false)
            {
                contact1NameWrapPanel.Visibility = Visibility.Collapsed;
                contact1NumberWrapPanel.Visibility = Visibility.Collapsed;
                contact1EmailWrapPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                contact1NameTextBox.Text = contact2NameTextBox.Text;
                contact1NumberTextBox.Text = contact2NumberTextBox.Text;
                contact1EmailTextBox.Text = contact2EmailTextBox.Text;

                contact1CheckBox.IsChecked = true;
                contact2CheckBox.IsChecked = false;
            }
        }


        private void OnCheckContact2CheckBox(object sender, RoutedEventArgs e)
        {
            if (contact1CheckBox.IsChecked == true)
            {
                contact2NameWrapPanel.Visibility = Visibility.Visible;
                contact2NumberWrapPanel.Visibility = Visibility.Visible;
                contact2EmailWrapPanel.Visibility = Visibility.Visible;
            }
            else
            {
                contact2CheckBox.IsChecked = false;
                contact1CheckBox.IsChecked = true;
            }
        }

        private void OnUncheckContact2CheckBox(object sender, RoutedEventArgs e)
        {
            if (contact3CheckBox.IsChecked == false)
            {
                contact2NameWrapPanel.Visibility = Visibility.Collapsed;
                contact2NumberWrapPanel.Visibility = Visibility.Collapsed;
                contact2EmailWrapPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                contact2NameTextBox.Text = contact3NameTextBox.Text;
                contact2NumberTextBox.Text = contact3NumberTextBox.Text;
                contact2EmailTextBox.Text = contact3EmailTextBox.Text;

                contact2CheckBox.IsChecked = true;
                contact3CheckBox.IsChecked = false;
            }
        }

        private void OnCheckContact3CheckBox(object sender, RoutedEventArgs e)
        {
            if (contact2CheckBox.IsChecked == true)
            {
                contact3NameWrapPanel.Visibility = Visibility.Visible;
                contact3NumberWrapPanel.Visibility = Visibility.Visible;
                contact3EmailWrapPanel.Visibility = Visibility.Visible;
            }
            else
            {
                contact3CheckBox.IsChecked = false;
                contact2CheckBox.IsChecked = true;
            }
        }

        private void OnUncheckContact3CheckBox(object sender, RoutedEventArgs e)
        {
            contact3NameWrapPanel.Visibility = Visibility.Collapsed;
            contact3NumberWrapPanel.Visibility = Visibility.Collapsed;
            contact3EmailWrapPanel.Visibility = Visibility.Collapsed;
        }

        private void OnSelChangedWorkFieldCombo(object sender, SelectionChangedEventArgs e)
        {
            if (workfieldComboBox.SelectedIndex != -1)
            {
                company.SetCompanyFieldOfWork(workFields[workfieldComboBox.SelectedIndex].value);
                company.SetCompanyFieldOfWorkId(workFields[workfieldComboBox.SelectedIndex].key);
            }
        }

        private void OnButtonClickSaveChanges(object sender, RoutedEventArgs e)
        {
            if(companyNameTextBox.Text != "")
                company.SetCompanyName(companyNameTextBox.Text);
            else
            {
                MessageBox.Show("Company name must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(company.GetCompanyFieldOfWorkId() == 0)
            {
                MessageBox.Show("Company field of work must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            company.InitializeAddedBy(loggedInUser.GetEmployeeId());

            companyContacts.Clear();

            if (contact1CheckBox.IsChecked == true)
            {
                BASIC_STRUCTS.CONTACT_STRUCT tempContact = new BASIC_STRUCTS.CONTACT_STRUCT();

                if(contact1NameTextBox.Text != "")
                {
                    tempContact.contact_name = contact1NameTextBox.Text;
                }
                else
                {
                    MessageBox.Show("Contact 1 name must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (contact1EmailTextBox.Text != "")
                {
                    tempContact.contact_email = contact1EmailTextBox.Text;
                }
                else
                {
                    MessageBox.Show("Contact 1 email must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (contact1NumberTextBox.Text != "")
                {
                    tempContact.contact_phone = contact1NumberTextBox.Text;
                }
                else
                {
                    MessageBox.Show("Contact 1 Phone must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                tempContact.contact_added_by_id = loggedInUser.GetEmloyeeDepartmentId();


                companyContacts.Add(tempContact);
            }

            if (contact2CheckBox.IsChecked == true)
            {
                BASIC_STRUCTS.CONTACT_STRUCT tempContact = new BASIC_STRUCTS.CONTACT_STRUCT();
                if (contact2NameTextBox.Text != "")
                {
                    tempContact.contact_name = contact2NameTextBox.Text;
                }
                else
                {
                    MessageBox.Show("Contact 2 name must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (contact2EmailTextBox.Text != "")
                {
                    tempContact.contact_email = contact2EmailTextBox.Text;
                }
                else
                {
                    MessageBox.Show("Contact 2 email must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (contact2NumberTextBox.Text != "")
                {
                    tempContact.contact_phone = contact2NumberTextBox.Text;
                }
                else
                {
                    MessageBox.Show("Contact 2 Phone must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                tempContact.contact_added_by_id = loggedInUser.GetEmloyeeDepartmentId();

                companyContacts.Add(tempContact);
            }

            if (contact3CheckBox.IsChecked == true)
            {
                BASIC_STRUCTS.CONTACT_STRUCT tempContact = new BASIC_STRUCTS.CONTACT_STRUCT();
                if (contact3NameTextBox.Text != "")
                {
                    tempContact.contact_name = contact3NameTextBox.Text;
                }
                else
                {
                    MessageBox.Show("Contact 3 name must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (contact3EmailTextBox.Text != "")
                {
                    tempContact.contact_email = contact3EmailTextBox.Text;
                }
                else
                {
                    MessageBox.Show("Contact 3 email must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (contact3NumberTextBox.Text != "")
                {
                    tempContact.contact_phone = contact3NumberTextBox.Text;
                }
                else
                {
                    MessageBox.Show("Contact 3 Phone must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                tempContact.contact_added_by_id = loggedInUser.GetEmloyeeDepartmentId();

                companyContacts.Add(tempContact);
            }

            company.SetCompanyContacts(companyContacts);

            if(bandsCheckBox.IsChecked == true)
            {
                List<BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT> tempBandsList = new List<BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT>();

                BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT tempBands = new BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT();

                for(int i = 0; i < freqBandsGrid.Children.Count; i++)
                {
                    Grid currentGrid = (Grid)freqBandsGrid.Children[i];

                    WrapPanel currentCFWrapPanel = (WrapPanel)currentGrid.Children[1];
                    TextBox currentCFTextBox = (TextBox)currentCFWrapPanel.Children[0];
                    ComboBox currentCFComboBox = (ComboBox)currentCFWrapPanel.Children[1];

                    WrapPanel currentBWWrapPanel = (WrapPanel)currentGrid.Children[3];
                    TextBox currentBWTextBox = (TextBox)currentBWWrapPanel.Children[0];
                    ComboBox currentBWComboBox = (ComboBox)currentBWWrapPanel.Children[1];

                    if (currentCFTextBox.Text == "")
                    {
                        MessageBox.Show("Start Freq " + (i + 1) + " must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (currentCFComboBox.SelectedIndex == -1)
                    {
                        MessageBox.Show("Start Freq unit " + (i + 1) + " must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (currentBWTextBox.Text == "")
                    {
                        MessageBox.Show("Stop Freq " + (i + 1) + " must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (currentBWComboBox.SelectedIndex == -1)
                    {
                        MessageBox.Show("Stop Freq unit " + (i + 1) + " must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }


                    tempBands.start_frequency = Decimal.Parse(currentCFTextBox.Text);
                    if (currentCFComboBox.SelectedIndex != -1)
                    {
                        tempBands.start_frequency_unit_id = bandsUnits[currentCFComboBox.SelectedIndex].key;
                        tempBands.start_frequency_unit = bandsUnits[currentCFComboBox.SelectedIndex].value;
                    }

                    tempBands.stop_frequency = Decimal.Parse(currentBWTextBox.Text);
                    if (currentBWComboBox.SelectedIndex != -1)
                    {
                        tempBands.stop_frequency_unit_id = bandsUnits[currentBWComboBox.SelectedIndex].key;
                        tempBands.stop_frequency_unit = bandsUnits[currentBWComboBox.SelectedIndex].value;
                    }

                    if(tempBands.stop_frequency < tempBands.start_frequency)
                    {
                        MessageBox.Show("Stop Freq. cannot be less than Start Freq., The error is in row " + i + 1 + " in company frequencies!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }


                    tempBandsList.Add(tempBands);
                }

                company.SetCompanyStartFrequencies(tempBandsList);

            }
            else
            {
                List<BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT> tempBandsList = new List<BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT>();

                BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT tempBands = new BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT();


                for (int i = 0; i < freqBandsGrid.Children.Count; i++)
                {
                    Grid currentGrid = (Grid)freqBandsGrid.Children[i];

                    WrapPanel currentCFWrapPanel = (WrapPanel)currentGrid.Children[1];
                    TextBox currentCFTextBox = (TextBox)currentCFWrapPanel.Children[0];
                    ComboBox currentCFComboBox = (ComboBox)currentCFWrapPanel.Children[1];

                    WrapPanel currentBWWrapPanel = (WrapPanel)currentGrid.Children[3];
                    TextBox currentBWTextBox = (TextBox)currentBWWrapPanel.Children[0];
                    ComboBox currentBWComboBox = (ComboBox)currentBWWrapPanel.Children[1];


                    if (currentCFTextBox.Text == "")
                    {
                        MessageBox.Show("Centre Freq " + (i + 1) + " must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (currentCFComboBox.SelectedIndex == -1)
                    {
                        MessageBox.Show("Centre Freq unit " + (i + 1) + " must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (currentBWTextBox.Text == "")
                    {
                        MessageBox.Show("Bandwidth " + (i + 1) + " must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (currentBWComboBox.SelectedIndex == -1)
                    {
                        MessageBox.Show("Bandwidth unit " + (i + 1) + " must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }


                    tempBands.centre_frequency = Decimal.Parse(currentCFTextBox.Text);
                    if (currentCFComboBox.SelectedIndex != -1)
                    {
                        tempBands.centre_frequency_unit_id = bandsUnits[currentCFComboBox.SelectedIndex].key;
                        tempBands.centre_frequency_unit = bandsUnits[currentCFComboBox.SelectedIndex].value;
                    }

                    tempBands.bandwidth = Decimal.Parse(currentBWTextBox.Text);
                    if (currentBWComboBox.SelectedIndex != -1)
                    {
                        tempBands.bandwidth_unit_id = bandsUnits[currentBWComboBox.SelectedIndex].key;
                        tempBands.bandwidth_unit = bandsUnits[currentBWComboBox.SelectedIndex].value;
                    }


                    tempBandsList.Add(tempBands);
                }

                company.SetCompanyCfAndBW(tempBandsList);
            }

            if (companyCondition == BASIC_STRUCTS.COMPANY_ADD_CONDITION)
            {
                if (!company.IssueNewCompany())
                    return;
            }
            else if(companyCondition == BASIC_STRUCTS.COMPANY_EDIT_CONDITION)
            {
                if (company.GetCompanyStartFrequencies().Count > 0)
                {
                    if (!company.DeleteCompanyStartFrequencies())
                        return;

                    if (!company.InsertIntoCompanyStartFrequencies())
                        return;
                }

                if (company.GetCompanyCFandBw().Count > 0)
                {
                    if (!company.DeleteCompanyCentreFrequency())
                        return;

                    if (!company.InsertIntoCompanyCentreFrequency())
                        return;
                }

                if (!company.EditCompanyContacts())
                    return;
            }

            this.Close();
        }

        private bool CheckEmailForm(String mEmail)
        {
            if (!mEmail.Contains("@"))
                return false;
            if (!mEmail.Contains("."))
                return false;

            return true;
        }

        private void OnCheckCompanyBandsCheckbox(object sender, RoutedEventArgs e)
        {
            bandWidthCheckBox.IsChecked = false;

            for (int i = 0; i < freqBandsGrid.Children.Count; i++)
            {
                Grid currentGrid = (Grid)freqBandsGrid.Children[i];

                Label currentCFLabel = (Label)currentGrid.Children[0];
                currentCFLabel.Content = "Start Freq.";

                Label currentBWLabel = (Label)currentGrid.Children[2];
                currentBWLabel.Content = "Stop Freq.";
            }
        }

        private void OnUncheckCompanyBandsCheckbox(object sender, RoutedEventArgs e)
        {
            bandWidthCheckBox.IsChecked = true;
        }

        private void OnCheckCentreFreqCheckbox(object sender, RoutedEventArgs e)
        {
            bandsCheckBox.IsChecked = false;

            for (int i = 0; i < freqBandsGrid.Children.Count; i++)
            {
                Grid currentGrid = (Grid)freqBandsGrid.Children[i];

                Label currentCFLabel = (Label)currentGrid.Children[0];
                currentCFLabel.Content = "Centre Freq.";

                Label currentBWLabel = (Label)currentGrid.Children[2];
                currentBWLabel.Content = "Bandwidth";
            }
        }

        private void OnUncheckedCentreFreqCheckbox(object sender, RoutedEventArgs e)
        {
            bandsCheckBox.IsChecked = true;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[A-Za-z]+");
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OnButtonClickAddFrequency(object sender, RoutedEventArgs e)
        {

            Grid currentGrid = new Grid();

            AddFrequency(ref currentGrid, 0);

            freqBandsGrid.Children.Add(currentGrid);
        }

        private void AddFrequency(ref Grid currentGrid, int i)
        {
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

            Label cfLabel = new Label();
            cfLabel.Style = (Style)FindResource("mediumLabelStyle");

            WrapPanel cfWrapPanel = new WrapPanel() { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };

            TextBox cfTextBox = new TextBox() { Style = (Style)FindResource("miniTextboxStyle") };
            cfTextBox.PreviewTextInput += NumberValidationTextBox;
            ComboBox cfCombo = new ComboBox() { Style = (Style)FindResource("miniComboBoxStyle"), Margin = new Thickness(12, 0, 0, 0) };
            FillBandUnits(ref cfCombo);


            cfWrapPanel.Children.Add(cfTextBox);
            cfWrapPanel.Children.Add(cfCombo);

            Label bwLabel = new Label();
            bwLabel.Style = (Style)FindResource("mediumLabelStyle");

            WrapPanel bwWrapPanel = new WrapPanel() { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };

            TextBox bwTextBox = new TextBox() { Style = (Style)FindResource("miniTextboxStyle") };
            bwTextBox.PreviewTextInput += NumberValidationTextBox;
            ComboBox bwCombo = new ComboBox() { Style = (Style)FindResource("miniComboBoxStyle"), Margin = new Thickness(12, 0, 0, 0) };
            FillBandUnits(ref bwCombo);


            Image currentRemoveFreqImage = new Image() { Margin = new Thickness(12, 0, 0, 0) };
            currentRemoveFreqImage.Source = new BitmapImage(new Uri("Photos/red_cross_icon.png", UriKind.Relative));
            currentRemoveFreqImage.Height = 40;
            currentRemoveFreqImage.Width = 40;
            currentRemoveFreqImage.ToolTip = "Remove Frequency";
            currentRemoveFreqImage.MouseLeftButtonDown += OnClickRemoveFrequency;

            bwWrapPanel.Children.Add(bwTextBox);
            bwWrapPanel.Children.Add(bwCombo);

            currentGrid.Children.Add(cfLabel);
            Grid.SetColumn(cfLabel, 0);

            currentGrid.Children.Add(cfWrapPanel);
            Grid.SetColumn(cfWrapPanel, 1);

            currentGrid.Children.Add(bwLabel);
            Grid.SetColumn(bwLabel, 2);

            currentGrid.Children.Add(bwWrapPanel);
            Grid.SetColumn(bwWrapPanel, 3);

            if (companyCondition != BASIC_STRUCTS.COMPANY_VIEW_CONDITION)
            {
                currentGrid.Children.Add(currentRemoveFreqImage);
                Grid.SetColumn(currentRemoveFreqImage, 4);
            }
            else
            {
                cfTextBox.IsEnabled = false;
                cfCombo.IsEnabled = false;
                bwTextBox.IsEnabled = false;
                bwCombo.IsEnabled = false;
            }

            
            if (bandsCheckBox.IsChecked == true)
            {
                cfLabel.Content = "Start Freq.";
                bwLabel.Content = "Stop Freq.";

                if (i != 0)
                {
                    cfTextBox.Text = company.GetCompanyStartFrequencies()[i].start_frequency.ToString();
                    cfCombo.Text = company.GetCompanyStartFrequencies()[i].start_frequency_unit;
                    bwTextBox.Text = company.GetCompanyStartFrequencies()[i].stop_frequency.ToString();
                    bwCombo.Text = company.GetCompanyStartFrequencies()[i].stop_frequency_unit;
                }

            }
            else
            {
                cfLabel.Content = "Centre Freq.";
                bwLabel.Content = "Bandwidth";

                if (i != 0)
                {
                    cfTextBox.Text = company.GetCompanyCFandBw()[i].centre_frequency.ToString();
                    cfCombo.Text = company.GetCompanyCFandBw()[i].centre_frequency_unit;
                    bwTextBox.Text = company.GetCompanyCFandBw()[i].bandwidth.ToString();
                    bwCombo.Text = company.GetCompanyCFandBw()[i].bandwidth_unit;
                }

            }
        }

        private void OnClickRemoveFrequency(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid currentGrid = (Grid)currentImage.Parent;

            freqBandsGrid.Children.Remove(currentGrid);
        }
    }
}
