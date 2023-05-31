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
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for EMFWindow.xaml
    /// </summary>
    public partial class EMFWindow : Window
    {
        private Employee loggedInUser;

        private CommonQueries commonQueries;

        private EMF emfPoint;

        private int viewAddCondition;

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> pointStatus;
        private List<EMF> emfPoints;

        private int pointSerial;

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> emfBandsList;

        public EMFWindow(ref Employee mLoggedInUser, ref int mViewAddCondition, ref EMF mEMFPoint)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            emfPoint = mEMFPoint;

            commonQueries = new CommonQueries();

            pointStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            emfPoints = new List<EMF>();
            emfBandsList = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            InitializeComponent();

            GetEMFBands();
            GetPointStatus();
            FillStatusCombo(ref statusComboBox);

            if(viewAddCondition != BASIC_STRUCTS.EMF_ADD_CONDITION)
            {
                pointSerial = emfPoint.GetSerial();

                nameTextBox.Text = emfPoint.GetName();
                areaTextBox.Text = emfPoint.GetArea();
                districtTextBox.Text = emfPoint.GetDistrict();
                latTextBox.Text = emfPoint.GetLat();
                longTextBox.Text = emfPoint.GetLong();
                statusComboBox.SelectedIndex = pointStatus.FindIndex(x1 => x1.key == emfPoint.GetStatusId());
                actualLatTextBox.Text = emfPoint.GetActualLat();
                actualLongTextBox.Text = emfPoint.GetActualLong();
                if(emfPoint.GetReadingDate().Year != 1900)
                    dateDatePicker.SelectedDate = emfPoint.GetReadingDate();

                for (int i = 0; i < emfPoint.pointBands.Count; i++)
                {
                    firstPointGrid.Children.Remove(addPointReadingImage);

                    firstPointGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80) });

                    WrapPanel bandsWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };

                    Label currentBandsLabel = new Label() { Style = (Style)FindResource("labelStyleBlack"), Content = "Band: " };

                    ComboBox currentBandComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };
                    FillEMFBandsCombo(ref currentBandComboBox);
                    currentBandComboBox.SelectedItem = emfPoint.pointBands[i].band_name;

                    bandsWrapPanel.Children.Add(currentBandsLabel);
                    bandsWrapPanel.Children.Add(currentBandComboBox);

                    firstPointGrid.Children.Add(bandsWrapPanel);
                    Grid.SetRow(bandsWrapPanel, firstPointGrid.RowDefinitions.Count - 2);
                    Grid.SetColumn(bandsWrapPanel, 0);


                    WrapPanel avgWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };

                    Label currentavgLabel = new Label() { Style = (Style)FindResource("labelStyleBlack"), Content = "Avg. Power: " };

                    TextBox currentAvgTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };
                    currentAvgTextBox.PreviewTextInput += NumberValidationTextBox;
                    currentAvgTextBox.Text = emfPoint.pointBands[i].average_power_density.ToString();

                    avgWrapPanel.Children.Add(currentavgLabel);
                    avgWrapPanel.Children.Add(currentAvgTextBox);

                    firstPointGrid.Children.Add(avgWrapPanel);
                    Grid.SetRow(avgWrapPanel, firstPointGrid.RowDefinitions.Count - 2);
                    Grid.SetColumn(avgWrapPanel, 1);

                    WrapPanel maxWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };

                    Label currentMaxLabel = new Label() { Style = (Style)FindResource("labelStyleBlack"), Content = "Max. Power: " };

                    TextBox currentMaxTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };
                    currentMaxTextBox.PreviewTextInput += NumberValidationTextBox;
                    currentMaxTextBox.Text = emfPoint.pointBands[i].max_power_density.ToString();


                    String imageSource = @"\Photos\red_cross_icon.png";

                    Image currentRedCrossImage = new Image() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
                    currentRedCrossImage.Height = 30;
                    currentRedCrossImage.Width = 30;
                    currentRedCrossImage.ToolTip = "Remove Reading";
                    currentRedCrossImage.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));
                    currentRedCrossImage.MouseLeftButtonDown += OnClickRemoveReading;


                    maxWrapPanel.Children.Add(currentMaxLabel);
                    maxWrapPanel.Children.Add(currentMaxTextBox);
                    if (viewAddCondition != BASIC_STRUCTS.EMF_VIEW_CONDITION)
                    {
                        maxWrapPanel.Children.Add(currentRedCrossImage);
                    }

                    firstPointGrid.Children.Add(maxWrapPanel);
                    Grid.SetRow(maxWrapPanel, firstPointGrid.RowDefinitions.Count - 2);
                    Grid.SetColumn(maxWrapPanel, 2);


                    if (viewAddCondition == BASIC_STRUCTS.EMF_VIEW_CONDITION)
                    {
                        currentBandComboBox.IsEnabled = false;
                        currentAvgTextBox.IsReadOnly = true;
                        currentMaxTextBox.IsReadOnly = true;
                    }

                    firstPointGrid.Children.Add(addPointReadingImage);
                    Grid.SetRow(addPointReadingImage, firstPointGrid.RowDefinitions.Count - 1);
                    Grid.SetColumnSpan(addPointReadingImage, 3);
                }

                if(viewAddCondition == BASIC_STRUCTS.EMF_VIEW_CONDITION)
                {
                    nameTextBox.IsReadOnly = true;
                    areaTextBox.IsReadOnly = true;
                    districtTextBox.IsReadOnly = true;
                    latTextBox.IsReadOnly = true;
                    longTextBox.IsReadOnly = true;
                    statusComboBox.IsEnabled = false;
                    actualLatTextBox.IsReadOnly = true;
                    actualLongTextBox.IsReadOnly = true;
                    dateDatePicker.IsEnabled = false;

                    addPointReadingImage.Visibility = Visibility.Collapsed;
                }
            }

        }

        private void GetPointStatus()
        {
            if (!commonQueries.GetEMFPointStatus(ref pointStatus))
                return;
        }

        private void GetEMFBands()
        {
            if (!commonQueries.GetEMFBands(ref emfBandsList))
                return;
        }

        private void FillStatusCombo(ref ComboBox currentCombo)
        {
            currentCombo.Items.Clear();

            for (int i = 0; i < pointStatus.Count; i++)
            {
                currentCombo.Items.Add(pointStatus[i].value);
            }

            if (viewAddCondition == BASIC_STRUCTS.EMF_ADD_CONDITION)
            {
                currentCombo.SelectedIndex = 0;
            }

        }

        private void OnButtonClickAddPoint(object sender, RoutedEventArgs e)
        {
            Grid currentGrid = new Grid() { Margin = new Thickness(24)};

            currentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80)});
            currentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80)});
            currentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80)});
            currentGrid.RowDefinitions.Add(new RowDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());

            WrapPanel currentNameWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center};
            Label currentNameLabel = new Label() { Content = "Name: ", Style = (Style)FindResource("labelStyleBlack") };
            TextBox currentNameTextBox = new TextBox() {Style = (Style)FindResource("textboxStyle") };

            currentNameWrapPanel.Children.Add(currentNameLabel);
            currentNameWrapPanel.Children.Add(currentNameTextBox);

            WrapPanel currentAreaWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            Label currentAreaLabel = new Label() { Content = "Area",  Style = (Style)FindResource("labelStyleBlack") };
            TextBox currentAreaTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };

            currentAreaWrapPanel.Children.Add(currentAreaLabel);
            currentAreaWrapPanel.Children.Add(currentAreaTextBox);

            WrapPanel currentDistrictWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            Label currentDistrictLabel = new Label() { Content = "District: ", Style = (Style)FindResource("labelStyleBlack") };
            TextBox currentDistrictTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };

            currentDistrictWrapPanel.Children.Add(currentDistrictLabel);
            currentDistrictWrapPanel.Children.Add(currentDistrictTextBox);

            WrapPanel currentLatWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            Label currentLatLabel = new Label() { Content = "Latitude: ", Style = (Style)FindResource("labelStyleBlack") };
            TextBox currentLatTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };
            currentLatTextBox.PreviewTextInput += NumberValidationTextBox;

            currentLatWrapPanel.Children.Add(currentLatLabel);
            currentLatWrapPanel.Children.Add(currentLatTextBox);

            WrapPanel currentLongWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            Label currentLongLabel = new Label() { Content = "Longitude: ", Style = (Style)FindResource("labelStyleBlack") };
            TextBox currentLongTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };
            currentLongTextBox.PreviewTextInput += NumberValidationTextBox;

            currentLongWrapPanel.Children.Add(currentLongLabel);
            currentLongWrapPanel.Children.Add(currentLongTextBox);

            WrapPanel currentStatusWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            Label currentStatusLabel = new Label() { Content = "Status: ", Style = (Style)FindResource("labelStyleBlack") };
            ComboBox currentStatusComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle"), IsEnabled = false };
            FillStatusCombo(ref currentStatusComboBox);

            currentStatusWrapPanel.Children.Add(currentStatusLabel);
            currentStatusWrapPanel.Children.Add(currentStatusComboBox);

            WrapPanel currentActualLatWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            Label currentActualLatLabel = new Label() { Content = @"Actual \Lat: ", Style = (Style)FindResource("labelStyleBlack") };
            TextBox currenActualtLatTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };
            currentLongTextBox.PreviewTextInput += NumberValidationTextBox;

            currentActualLatWrapPanel.Children.Add(currentActualLatLabel);
            currentActualLatWrapPanel.Children.Add(currenActualtLatTextBox);

            WrapPanel currentActualLongWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            Label currentActualLongLabel = new Label() { Content = "Actual &#10;Long: ", Style = (Style)FindResource("labelStyleBlack") };
            TextBox currenActualtLongTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };
            currentLongTextBox.PreviewTextInput += NumberValidationTextBox;

            currentActualLongWrapPanel.Children.Add(currentActualLongLabel);
            currentActualLongWrapPanel.Children.Add(currenActualtLongTextBox);

            
            
            currentGrid.Children.Add(currentNameWrapPanel);
            Grid.SetRow(currentNameWrapPanel, 0);
            Grid.SetColumn(currentNameWrapPanel, 0);

            currentGrid.Children.Add(currentAreaWrapPanel);
            Grid.SetRow(currentAreaWrapPanel, 0);
            Grid.SetColumn(currentAreaWrapPanel, 1);
            
            currentGrid.Children.Add(currentDistrictWrapPanel);
            Grid.SetRow(currentDistrictWrapPanel, 0);
            Grid.SetColumn(currentDistrictWrapPanel, 2);

            currentGrid.Children.Add(currentLatWrapPanel);
            Grid.SetRow(currentLatWrapPanel, 1);
            Grid.SetColumn(currentLatWrapPanel, 0);

            currentGrid.Children.Add(currentLongWrapPanel);
            Grid.SetRow(currentLongWrapPanel, 1);
            Grid.SetColumn(currentLongWrapPanel, 1);

            currentGrid.Children.Add(currentStatusWrapPanel);
            Grid.SetRow(currentStatusWrapPanel, 1);
            Grid.SetColumn(currentStatusWrapPanel, 2);

            currentGrid.Children.Add(currentActualLatWrapPanel);
            Grid.SetRow(currentActualLatWrapPanel, 2);
            Grid.SetColumn(currentActualLatWrapPanel, 0);

            currentGrid.Children.Add(currentActualLongWrapPanel);
            Grid.SetRow(currentActualLongWrapPanel, 2);
            Grid.SetColumn(currentActualLongWrapPanel, 1);

            if (viewAddCondition != BASIC_STRUCTS.EMF_VIEW_CONDITION)
            {
                String imageSource = @"\Photos\plus_icon.png";

                Image currentAddPointImage = new Image() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
                currentAddPointImage.Height = 30;
                currentAddPointImage.Width = 30;
                currentAddPointImage.ToolTip = "Add Reading";
                currentAddPointImage.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));
                currentAddPointImage.MouseLeftButtonDown += OnClickAddReading;

                currentGrid.Children.Add(currentAddPointImage);
                Grid.SetRow(currentActualLongWrapPanel, 3);
                Grid.SetColumnSpan(currentActualLongWrapPanel, 3);
            }

            if (viewAddCondition != BASIC_STRUCTS.EMF_VIEW_CONDITION)
            {
                String imageSource = @"\Photos\red_cross_icon.png";

                Image currentRedCrossImage = new Image() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
                currentRedCrossImage.Height = 50;
                currentRedCrossImage.Width = 50;
                currentRedCrossImage.ToolTip = "Remove Point";
                currentRedCrossImage.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));
                currentRedCrossImage.MouseLeftButtonDown += OnClickRemoveReading;

                currentGrid.Children.Add(currentRedCrossImage);
                Grid.SetRow(currentRedCrossImage, 2);
                Grid.SetColumnSpan(currentRedCrossImage, 3);
            }

            mainStackPanel.Children.Add(currentGrid);

        }

        private void OnClickRemovePoint(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid currentGrid = (Grid)currentImage.Parent;

            mainStackPanel.Children.Remove(currentGrid);
        }

        private void OnClickRemoveReading(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            WrapPanel currentMaxWrapPanel = (WrapPanel)currentImage.Parent;
            Grid currentGrid = (Grid)currentMaxWrapPanel.Parent;

            int currentChildIndex = firstPointGrid.Children.IndexOf(currentMaxWrapPanel);
            int currentRowIndex = Grid.GetRow(currentMaxWrapPanel);

            WrapPanel currentAvgWrapPanel = (WrapPanel)firstPointGrid.Children[currentChildIndex - 1];
            WrapPanel currentBandWrapPanel = (WrapPanel)firstPointGrid.Children[currentChildIndex - 2];


            //Image currentAddReadingImage = (Image)currentGrid.Children[currentGrid.Children.Count - 1];
            //currentGrid.Children.Remove(currentAddReadingImage);

            List<UIElement> children = new List<UIElement>();

            for (int i = currentChildIndex + 1; i < currentGrid.Children.Count; i++)
            {
                var child = currentGrid.Children[i];
                children.Add(child);
            }

            for(int i = 0; i < children.Count; i++)
            {
                currentGrid.Children.Remove(children[i]);
            }


            currentGrid.Children.Remove(currentMaxWrapPanel);
            currentGrid.Children.Remove(currentAvgWrapPanel);
            currentGrid.Children.Remove(currentBandWrapPanel);


            currentGrid.RowDefinitions.RemoveAt(currentRowIndex);

            int columnNumber = 0;

            for(int i = 0; i < children.Count; i++)
            {
                if(columnNumber == 3)
                {
                    columnNumber = 0;
                    currentRowIndex += 1;
                }

                currentGrid.Children.Add(children[i]);
                if (i != children.Count - 1)
                {
                    Grid.SetRow(children[i], currentRowIndex);
                    Grid.SetColumn(children[i], columnNumber);
                }
                else
                {
                    Grid.SetRow(children[i], currentRowIndex);
                    Grid.SetColumnSpan(children[i], 3);
                }
                columnNumber++;
            }

        }

        private void OnButtonClickSaveChanges(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < mainStackPanel.Children.Count; i++)
            {
                EMF emfPoint = new EMF();

                Grid currentGrid = (Grid)mainStackPanel.Children[i];
                WrapPanel currentNameWrapPanel = (WrapPanel)currentGrid.Children[0];
                TextBox currentNameTextBox = (TextBox)currentNameWrapPanel.Children[1];

                WrapPanel currentAreaWrapPanel = (WrapPanel)currentGrid.Children[1];
                TextBox currentAreaTextBox = (TextBox)currentAreaWrapPanel.Children[1];

                WrapPanel currentDistrictWrapPanel = (WrapPanel)currentGrid.Children[2];
                TextBox currentDistrictTextBox = (TextBox)currentDistrictWrapPanel.Children[1];

                WrapPanel currentLatWrapPanel = (WrapPanel)currentGrid.Children[3];
                TextBox currentLatTextBox = (TextBox)currentLatWrapPanel.Children[1];

                WrapPanel currentLongWrapPanel = (WrapPanel)currentGrid.Children[4];
                TextBox currentLongTextBox = (TextBox)currentLongWrapPanel.Children[1];

                WrapPanel currentStatusWrapPanel = (WrapPanel)currentGrid.Children[5];
                ComboBox currentStatusComboBox = (ComboBox)currentStatusWrapPanel.Children[1];

                WrapPanel currentActualLatWrapPanel = (WrapPanel)currentGrid.Children[6];
                TextBox currentActualLatTextBox = (TextBox)currentActualLatWrapPanel.Children[1];

                WrapPanel currentActualongtWrapPanel = (WrapPanel)currentGrid.Children[7];
                TextBox currentActualLongTextBox = (TextBox)currentActualongtWrapPanel.Children[1];

                WrapPanel currentDatetWrapPanel = (WrapPanel)currentGrid.Children[8];
                DatePicker currentDatePicker = (DatePicker)currentDatetWrapPanel.Children[1];

                if (currentNameTextBox.Text == "")
                {
                    MessageBox.Show("Name must be specified for point " + (i + 1) + "!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (currentAreaTextBox.Text == "")
                {
                    MessageBox.Show("Area must be specified for point " + (i + 1) + "!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (currentDistrictTextBox.Text == "")
                {
                    MessageBox.Show("District must be specified for point ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (currentLatTextBox.Text == "")
                {
                    MessageBox.Show("Latitude must be specified for point " + (i + 1) + "!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (currentLongTextBox.Text == "")
                {
                    MessageBox.Show("Longitude must be specified for point " + (i + 1) + "!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                emfPoint.SetName(currentNameTextBox.Text);
                emfPoint.SetArea(currentAreaTextBox.Text);
                emfPoint.SetDistrict(currentDistrictTextBox.Text);
                emfPoint.SetLat(currentLatTextBox.Text);
                emfPoint.SetLong(currentLongTextBox.Text);
                emfPoint.SetStatusId(pointStatus[currentStatusComboBox.SelectedIndex].key);
                emfPoint.SetStatus(pointStatus[currentStatusComboBox.SelectedIndex].value);
                emfPoint.SetActualLat(currentActualLatTextBox.Text);
                emfPoint.SetActualLong(currentActualLongTextBox.Text);
                
                if(currentDatePicker.SelectedDate != null)
                    emfPoint.SetReadingDate(DateTime.Parse(currentDatePicker.SelectedDate.ToString()));

                if (currentGrid.RowDefinitions.Count > 4)
                {

                    int childIndex = 9;

                    List<BASIC_STRUCTS.EMF_BAND_STRUCT> emfBands = new List<BASIC_STRUCTS.EMF_BAND_STRUCT>();

                    for (int j = 3; j < currentGrid.RowDefinitions.Count - 1; j++)
                    {
                        BASIC_STRUCTS.EMF_BAND_STRUCT tempBand = new BASIC_STRUCTS.EMF_BAND_STRUCT();

                        WrapPanel currentBandWrapPanel = (WrapPanel)currentGrid.Children[childIndex++];
                        ComboBox currentBandComboBox = (ComboBox)currentBandWrapPanel.Children[1];

                        WrapPanel currentAvgWrapPanel = (WrapPanel)currentGrid.Children[childIndex++];
                        TextBox currentAvgTextBox = (TextBox)currentAvgWrapPanel.Children[1];

                        WrapPanel currentMaxtWrapPanel = (WrapPanel)currentGrid.Children[childIndex++];
                        TextBox currentMaxTextBox = (TextBox)currentMaxtWrapPanel.Children[1];

                        if (currentMaxTextBox.Text == "")
                        {
                            MessageBox.Show("Max power must be specified in row " + (j + 1) + "!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (currentAvgTextBox.Text == "")
                        {
                            MessageBox.Show("Average power must be specified in row " + (j + 1) + "!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (currentBandComboBox.Text == "")
                        {
                            MessageBox.Show("Band must be specified in row " + (j + 1) + "!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        tempBand.band_name = currentBandComboBox.Text.ToString();
                        tempBand.band = emfBandsList[currentBandComboBox.SelectedIndex].key;
                        tempBand.average_power_density = Double.Parse(currentAvgTextBox.Text);
                        tempBand.max_power_density = Double.Parse(currentMaxTextBox.Text);

                        if (!emfBands.Exists(x1 => x1.band_name == tempBand.band_name))
                            emfBands.Add(tempBand);
                        else
                        {
                            MessageBox.Show("Band " + tempBand.band_name + " is specified more than once! Please add only one reading for each band", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    emfPoint.SetPointBands(ref emfBands);
                }

                if (viewAddCondition == BASIC_STRUCTS.EMF_ADD_CONDITION)
                {
                    if (commonQueries.CheckEMFPointNameExists(emfPoint.GetName()))
                    {

                        if(emfPoint.pointBands.Count != 0)
                        {
                            if (dateDatePicker.SelectedDate == null)
                            {
                                MessageBox.Show("Date must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }

                        if (!emfPoint.IssueNewPoint())
                            return;

                        if(emfPoint.pointBands.Count != 0)
                        {
                            if (!emfPoint.DeletePointBands())
                                return;

                            if (!emfPoint.InsertIntoEMFPointBands())
                                return;
                        }

                        if (!emfPoint.CheckPointStatus())
                            return;

                        mainStackPanel.Children.Remove(currentGrid);
                    }
                    else
                    {
                        MessageBox.Show("Point " + emfPoint.GetName() + " already exists in database! Two different EMF points can't have the same name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                if (viewAddCondition == BASIC_STRUCTS.EMF_EDIT_CONDITION)
                {
                    emfPoint.SetSerial(pointSerial);

                    if (emfPoint.pointBands.Count != 0)
                    {
                        if (dateDatePicker.SelectedDate == null)
                        {
                            MessageBox.Show("Date must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    if (!emfPoint.EditEMFPoint())
                        return;

                    if (!emfPoint.DeletePointBands())
                        return;

                    if (!emfPoint.InsertIntoEMFPointBands())
                        return;

                    if (!emfPoint.CheckPointStatus())
                        return;
                }
            }

            this.Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[A-Za-z]+");
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OnClickAddReading(object sender, MouseButtonEventArgs e)
        {
            Image currentImage = (Image)sender;
            Grid currentPointGrid = (Grid)currentImage.Parent;

            currentPointGrid.Children.Remove(currentImage);

            currentPointGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80)});

            WrapPanel bandsWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };

            Label currentBandsLabel= new Label() { Style = (Style)FindResource("labelStyleBlack"), Content = "Band: " };

            ComboBox currentBandComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle") };
            FillEMFBandsCombo(ref currentBandComboBox);

            bandsWrapPanel.Children.Add(currentBandsLabel);
            bandsWrapPanel.Children.Add(currentBandComboBox);

            currentPointGrid.Children.Add(bandsWrapPanel);
            Grid.SetRow(bandsWrapPanel, currentPointGrid.RowDefinitions.Count - 2);
            Grid.SetColumn(bandsWrapPanel, 0);


            WrapPanel avgWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };

            Label currentavgLabel = new Label() { Style = (Style)FindResource("labelStyleBlack"), Content = "Avg. Power: " };

            TextBox currentAvgTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };
            currentAvgTextBox.PreviewTextInput += NumberValidationTextBox;

            avgWrapPanel.Children.Add(currentavgLabel);
            avgWrapPanel.Children.Add(currentAvgTextBox);

            currentPointGrid.Children.Add(avgWrapPanel);
            Grid.SetRow(avgWrapPanel, currentPointGrid.RowDefinitions.Count - 2);
            Grid.SetColumn(avgWrapPanel, 1);

            WrapPanel maxWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };

            Label currentMaxLabel = new Label() { Style = (Style)FindResource("labelStyleBlack"), Content = "Max. Power: " };

            TextBox currentMaxTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };
            currentMaxTextBox.PreviewTextInput += NumberValidationTextBox;

            String imageSource = @"\Photos\red_cross_icon.png";

            Image currentRedCrossImage = new Image() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            currentRedCrossImage.Height = 30;
            currentRedCrossImage.Width = 30;
            currentRedCrossImage.ToolTip = "Remove Reading";
            currentRedCrossImage.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));
            currentRedCrossImage.MouseLeftButtonDown += OnClickRemoveReading;

            maxWrapPanel.Children.Add(currentMaxLabel);
            maxWrapPanel.Children.Add(currentMaxTextBox);
            maxWrapPanel.Children.Add(currentRedCrossImage);

            currentPointGrid.Children.Add(maxWrapPanel);
            Grid.SetRow(maxWrapPanel, currentPointGrid.RowDefinitions.Count - 2);
            Grid.SetColumn(maxWrapPanel, 2);

            currentPointGrid.Children.Add(currentImage);
            Grid.SetRow(currentImage, currentPointGrid.RowDefinitions.Count - 1);
            Grid.SetColumnSpan(currentImage, 3);


        }

        private void FillEMFBandsCombo(ref ComboBox currentCombo)
        {
            currentCombo.Items.Clear();

            for (int i = 0; i < emfBandsList.Count; i++)
            {
                currentCombo.Items.Add(emfBandsList[i].value);
            }

            currentCombo.SelectedIndex = 0;
        }
    }
}
