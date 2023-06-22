using Org.BouncyCastle.Math.EC.Rfc7748;
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
    /// Interaction logic for RepeatersWindow.xaml
    /// </summary>
    public partial class RepeatersWindow : Window
    {
        CommonQueries commonQueries;

        int viewAddCondition;
        Employee loggedInUser;
        
        List<BASIC_STRUCTS.REPEATER_STRUCT> repeaters;
        List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> missions;
        List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> repeatersStatus;

        public RepeatersWindow(ref Employee mLoggedInUser, ref List<BASIC_STRUCTS.REPEATER_STRUCT> mRepeaters, ref int mViewAddCondition)
        {
            InitializeComponent();

            commonQueries = new CommonQueries();
            missions = new List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT>();
            loggedInUser = mLoggedInUser;
            repeaters = mRepeaters;
            viewAddCondition = mViewAddCondition;
            repeatersStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            GetMissions();
            GetRepeatersStatus();

            if(viewAddCondition == BASIC_STRUCTS.REPEATER_VIEW_CONDITION)
            {
                repeatersStackPanel.Children.Clear();

                for(int i = 0; i < repeaters.Count; i++)
                {
                    Grid currentGrid = (Grid)CreateNewRepeaterGrid(i, true, false, true);
                    repeatersStackPanel.Children.Add(currentGrid);
                }

                finishButton.IsEnabled = false;
            }
            else if(viewAddCondition == BASIC_STRUCTS.REPEATER_EDIT_CONDITION)
            {
                repeatersStackPanel.Children.Clear();

                for (int i = 0; i < repeaters.Count; i++)
                {
                    Grid currentGrid = (Grid)CreateNewRepeaterGrid(i, true, false, false);
                    repeatersStackPanel.Children.Add(currentGrid);
                }
            }
            else
            {

            }

        }

        private bool GetMissions()
        {
            if (!commonQueries.GetMissions(ref missions))
                return false;

            for (int i = 0; i < missions.Count; i++)
            {
                missionComboBox.Items.Add(missions[i].mission_id);
            }

            return true;
        }

        private bool GetRepeatersStatus()
        {
            if (!commonQueries.GetRepeatersStatus(ref repeatersStatus))
                return false;

            for (int i = 0; i < repeatersStatus.Count; i++)
            {
                statusComboBox.Items.Add(repeatersStatus[i].value);
            }

            return true;
        }

        private void FillMissionComboBox(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            for (int i = 0; i < missions.Count; i++)
            {
                mCombo.Items.Add(missions[i].mission_id);
            }
        }

        private void FillRepeatersStatusComboBox(ref ComboBox mCombo)
        {
            mCombo.Items.Clear();

            for (int i = 0; i < repeatersStatus.Count; i++)
            {
                mCombo.Items.Add(repeatersStatus[i].value);
            }
        }


        private void OnClickAddRepeater(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;

            repeatersStackPanel.Children.Remove(currentImage);

            Grid currentGrid = CreateNewRepeaterGrid(repeatersStackPanel.Children.Count, false, true, false);

            repeatersStackPanel.Children.Add(currentGrid);

            repeatersStackPanel.Children.Add(currentImage);

        }

        private Grid CreateNewRepeaterGrid(int index, bool fill, bool addRemoveIcon, bool view)
        {
            Grid currentGrid = new Grid();
            currentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            currentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            currentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            currentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            currentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Label repeaterHeaderLabel = new Label() { Height = 50.00 };
            repeaterHeaderLabel.Style = (Style)FindResource("stackPanelHeaderLabelStyle");
            repeaterHeaderLabel.Content = "Repeater " + (index + 1);

            currentGrid.Children.Add(repeaterHeaderLabel);
            Grid.SetRow(repeaterHeaderLabel, 0);
            Grid.SetColumnSpan(repeaterHeaderLabel, 2);

            WrapPanel latWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

            Label latLabel = new Label() { Content = "Lat: ", Style = (Style)FindResource("labelStyleBlack") };

            TextBox currentLatTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };
            currentLatTextBox.PreviewTextInput += NumberValidationTextBox;
            if(fill)
            {
                currentLatTextBox.Text = repeaters[index].latitude;
            }
            if(view)
            {
                currentLatTextBox.IsReadOnly = true;
            }

            latWrapPanel.Children.Add(latLabel);
            latWrapPanel.Children.Add(currentLatTextBox);

            currentGrid.Children.Add(latWrapPanel);
            Grid.SetRow(latWrapPanel, 1);
            Grid.SetColumn(latWrapPanel, 0);

            WrapPanel longWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

            Label longLabel = new Label() { Content = "Long: ", Style = (Style)FindResource("labelStyleBlack") };

            TextBox currentLongTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };
            currentLongTextBox.PreviewTextInput += NumberValidationTextBox;
            if (fill)
            {
                currentLongTextBox.Text = repeaters[index].longitude;
            }
            if (view)
            {
                currentLongTextBox.IsReadOnly = true;
            }

            longWrapPanel.Children.Add(longLabel);
            longWrapPanel.Children.Add(currentLongTextBox);

            currentGrid.Children.Add(longWrapPanel);
            Grid.SetRow(longWrapPanel, 1);
            Grid.SetColumn(longWrapPanel, 1);

            WrapPanel addressWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

            Label addressLabel = new Label() { Content = "Address: ", Style = (Style)FindResource("labelStyleBlack") };

            TextBox currentAddressTextBox = new TextBox() { Style = (Style)FindResource("wideTextboxStyle") };
            if (fill)
            {
                currentAddressTextBox.Text = repeaters[index].address;
            }
            if (view)
            {
                currentAddressTextBox.IsReadOnly = true;
            }

            addressWrapPanel.Children.Add(addressLabel);
            addressWrapPanel.Children.Add(currentAddressTextBox);

            currentGrid.Children.Add(addressWrapPanel);
            Grid.SetRow(addressWrapPanel, 2);
            Grid.SetColumnSpan(addressWrapPanel, 2);

            WrapPanel dateWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

            Label dateLabel = new Label() { Content = "Date: ", Style = (Style)FindResource("labelStyleBlack") };

            DatePicker currentDatePicker = new DatePicker() { Style = (Style)FindResource("datePickerStyle") };
            if (fill)
            {
                currentDatePicker.SelectedDate = repeaters[index].date;
            }
            if (view)
            {
                currentDatePicker.IsEnabled = false;
            }

            dateWrapPanel.Children.Add(dateLabel);
            dateWrapPanel.Children.Add(currentDatePicker);

            currentGrid.Children.Add(dateWrapPanel);
            Grid.SetRow(dateWrapPanel, 3);
            Grid.SetColumn(dateWrapPanel, 0);

            WrapPanel statusWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

            Label statusLabel = new Label() { Content = "Status: ", Style = (Style)FindResource("labelStyleBlack") };

            ComboBox currentStatusComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };
            FillRepeatersStatusComboBox(ref currentStatusComboBox);
            if (fill)
            {
                currentStatusComboBox.SelectedIndex = repeatersStatus.FindIndex(x1 => x1.key == repeaters[index].status_id);
            }
            if (view)
            {
                currentStatusComboBox.IsEnabled = false;
            }

            statusWrapPanel.Children.Add(statusLabel);
            statusWrapPanel.Children.Add(currentStatusComboBox);

            currentGrid.Children.Add(statusWrapPanel);
            Grid.SetRow(statusWrapPanel, 3);
            Grid.SetColumn(statusWrapPanel, 1);


            WrapPanel missionWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

            Label missionLabel = new Label() { Content = "Mission (Optional): ", Style = (Style)FindResource("wideLabelStyleBlack") };

            ComboBox missionComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };
            FillMissionComboBox(ref missionComboBox);
            if (fill && repeaters[index].mission_serial != 0)
            {
                missionComboBox.SelectedIndex = missions.FindIndex(x1 => x1.company_serial == repeaters[index].company_serial && x1.complaint_serial == repeaters[index].complaint_serial && x1.mission_serial == repeaters[index].mission_serial);
            }
            if (view)
            {
                missionComboBox.IsEnabled = false;
            }

            missionWrapPanel.Children.Add(missionLabel);
            missionWrapPanel.Children.Add(missionComboBox);

            currentGrid.Children.Add(missionWrapPanel);
            Grid.SetRow(missionWrapPanel, 4);
            Grid.SetColumnSpan(missionWrapPanel, 2);

            if(addRemoveIcon && !view)
            {
                String imageSource = @"\Photos\red_cross_icon.png";

                Image currentRedCrossImage = new Image() { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12)};
                currentRedCrossImage.Height = 50;
                currentRedCrossImage.Width = 50;
                currentRedCrossImage.ToolTip = "Remove Repeater";
                currentRedCrossImage.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));
                currentRedCrossImage.MouseLeftButtonDown += OnClickRemoveRepeater;

                currentGrid.Children.Add(currentRedCrossImage);
                Grid.SetRow(currentRedCrossImage, 0);
                Grid.SetColumnSpan(currentRedCrossImage, 2);
            }

            return currentGrid;
        }

        private void OnClickRemoveRepeater(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid currentGrid = (Grid)currentImage.Parent;

            repeatersStackPanel.Children.Remove(currentGrid);
            FixRepeaterNumbers();
        }

        private void FixRepeaterNumbers()
        {
            for(int i = 0; i < repeatersStackPanel.Children.Count - 1; i++)
            {
                Grid currentGrid = (Grid)repeatersStackPanel.Children[i];

                try
                {
                    Label currentHeaderLabel = (Label)currentGrid.Children[i];
                    currentHeaderLabel.Content = "Repeater " + (i + 1);
                }
                catch
                {

                }
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[A-Za-z]+");
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OnSelChangedMissionCombo(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnButtonClickSaveChanges(object sender, RoutedEventArgs e)
        {

            List<BASIC_STRUCTS.REPEATER_STRUCT> currentRepeatersList = new List<BASIC_STRUCTS.REPEATER_STRUCT>();

            for(int i = 0; i < repeatersStackPanel.Children.Count; i++)
            {
                BASIC_STRUCTS.REPEATER_STRUCT currentRepeater = new BASIC_STRUCTS.REPEATER_STRUCT();

                if (repeatersStackPanel.Children[i].GetType() == typeof(Image))
                    continue;
                else
                {
                    Grid currentGrid = (Grid)repeatersStackPanel.Children[i];

                    WrapPanel currentLatWrapPanel = (WrapPanel)currentGrid.Children[1];

                    TextBox currentLatTextBox = (TextBox)currentLatWrapPanel.Children[1];
                    if (currentLatTextBox.Text == "")
                    {
                        MessageBox.Show("Latitude is not specified for repeater " + i, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    currentRepeater.latitude = currentLatTextBox.Text;

                    WrapPanel currentLongWrapPanel = (WrapPanel)currentGrid.Children[2];

                    TextBox currentLongTextBox = (TextBox)currentLongWrapPanel.Children[1];
                    if (currentLongTextBox.Text == "")
                    {
                        MessageBox.Show("Longitude is not specified for repeater " + i, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    currentRepeater.longitude = currentLongTextBox.Text;


                    WrapPanel currentAddressWrapPanel = (WrapPanel)currentGrid.Children[3];

                    TextBox currentAddressTextBox = (TextBox)currentAddressWrapPanel.Children[1];
                    if (currentAddressTextBox.Text == "")
                    {
                        MessageBox.Show("Address is not specified for repeater " + i, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    currentRepeater.address = currentAddressTextBox.Text;


                    WrapPanel currentdateWrapPanel = (WrapPanel)currentGrid.Children[4];

                    DatePicker currentDatePicker = (DatePicker)currentdateWrapPanel.Children[1];
                    if (currentDatePicker.Text == "")
                    {
                        MessageBox.Show("Date is not specified for repeater " + i, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    currentRepeater.date = (DateTime)currentDatePicker.SelectedDate;


                    WrapPanel currentStatusWrapPanel = (WrapPanel)currentGrid.Children[5];

                    ComboBox currentStatusComboBox = (ComboBox)currentStatusWrapPanel.Children[1];
                    if (currentStatusComboBox.SelectedIndex == -1)
                    {
                        MessageBox.Show("Status is not specified for repeater " + i, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    currentRepeater.status_id = repeatersStatus[currentStatusComboBox.SelectedIndex].key;
                    currentRepeater.status = repeatersStatus[currentStatusComboBox.SelectedIndex].value;


                    WrapPanel currentMissionWrapPanel = (WrapPanel)currentGrid.Children[6];

                    ComboBox currentMissionComboBox = (ComboBox)currentMissionWrapPanel.Children[1];
                    if (currentMissionComboBox.SelectedIndex != -1)
                    {
                        currentRepeater.company_serial = missions[currentMissionComboBox.SelectedIndex].company_serial;
                        currentRepeater.complaint_serial = missions[currentMissionComboBox.SelectedIndex].complaint_serial;
                        currentRepeater.mission_serial = missions[currentMissionComboBox.SelectedIndex].mission_serial;
                    }
                    else
                    {
                        currentRepeater.company_serial = 0;
                        currentRepeater.complaint_serial = 0;
                        currentRepeater.mission_serial = 0;
                    }

                    currentRepeatersList.Add(currentRepeater);

                }
            }

            if (!commonQueries.InsertIntoRepeaters(ref currentRepeatersList))
                return;

            this.Close();
        }
    }
}
