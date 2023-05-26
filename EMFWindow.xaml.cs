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

        public EMFWindow(ref Employee mLoggedInUser, ref int mViewAddCondition, ref EMF mEMFPoint)
        {
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            emfPoint = mEMFPoint;

            commonQueries = new CommonQueries();

            pointStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            emfPoints = new List<EMF>();

            InitializeComponent();

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

                if(viewAddCondition == BASIC_STRUCTS.EMF_VIEW_CONDITION)
                {
                    nameTextBox.IsReadOnly = true;
                    areaTextBox.IsReadOnly = true;
                    districtTextBox.IsReadOnly = true;
                    latTextBox.IsReadOnly = true;
                    longTextBox.IsReadOnly = true;
                    statusComboBox.IsEnabled = false;
                }
            }

        }

        private void GetPointStatus()
        {
            if (!commonQueries.GetEMFPointStatus(ref pointStatus))
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

            currentGrid.RowDefinitions.Add(new RowDefinition());
            currentGrid.RowDefinitions.Add(new RowDefinition());
            currentGrid.RowDefinitions.Add(new RowDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            currentGrid.ColumnDefinitions.Add(new ColumnDefinition());

            WrapPanel currentNameWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center};
            Label currentNameLabel = new Label() { Content = "Name: ", Style = (Style)FindResource("labelStyleBlack") };
            TextBox currentNameTextBox = new TextBox() {Style = (Style)FindResource("textboxStyle") };

            currentNameWrapPanel.Children.Add(currentNameLabel);
            currentNameWrapPanel.Children.Add(currentNameTextBox);

            WrapPanel currentAreaWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            Label currentAreaLabel = new Label() { Content = "Area",  Style = (Style)FindResource("labelStyleBlack") };
            TextBox currentAreaTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };

            currentAreaWrapPanel.Children.Add(currentAreaLabel);
            currentAreaWrapPanel.Children.Add(currentAreaTextBox);

            WrapPanel currentDistrictWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            Label currentDistrictLabel = new Label() { Content = "District: ", Style = (Style)FindResource("labelStyleBlack") };
            TextBox currentDistrictTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };

            currentDistrictWrapPanel.Children.Add(currentDistrictLabel);
            currentDistrictWrapPanel.Children.Add(currentDistrictTextBox);

            WrapPanel currentLatWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            Label currentLatLabel = new Label() { Content = "Latitude: ", Style = (Style)FindResource("labelStyleBlack") };
            TextBox currentLatTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };
            currentLatTextBox.PreviewTextInput += NumberValidationTextBox;

            currentLatWrapPanel.Children.Add(currentLatLabel);
            currentLatWrapPanel.Children.Add(currentLatTextBox);

            WrapPanel currentLongWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            Label currentLongLabel = new Label() { Content = "Longitude: ", Style = (Style)FindResource("labelStyleBlack") };
            TextBox currentLongTextBox = new TextBox() { Style = (Style)FindResource("textboxStyle") };
            currentLongTextBox.PreviewTextInput += NumberValidationTextBox;

            currentLongWrapPanel.Children.Add(currentLongLabel);
            currentLongWrapPanel.Children.Add(currentLongTextBox);

            WrapPanel currentStatusWrapPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            Label currentStatusLabel = new Label() { Content = "Status: ", Style = (Style)FindResource("labelStyleBlack") };
            ComboBox currentStatusComboBox = new ComboBox() { Style = (Style)FindResource("comboBoxStyle"), IsEnabled = false };
            FillStatusCombo(ref currentStatusComboBox);

            currentStatusWrapPanel.Children.Add(currentStatusLabel);
            currentStatusWrapPanel.Children.Add(currentStatusComboBox);

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

            if (viewAddCondition != BASIC_STRUCTS.EMF_VIEW_CONDITION)
            {
                String imageSource = @"\Photos\red_cross_icon.png";

                Image currentRedCrossImage = new Image() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                currentRedCrossImage.Height = 50;
                currentRedCrossImage.Width = 50;
                currentRedCrossImage.ToolTip = "Remove sector";
                currentRedCrossImage.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));
                currentRedCrossImage.MouseLeftButtonDown += OnClickRemovePoint;

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

                if(currentNameTextBox.Text == "")
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

                if (viewAddCondition == BASIC_STRUCTS.EMF_ADD_CONDITION)
                {
                    if (!emfPoint.IssueNewPoint())
                        return;

                    this.Close();
                }

                if(viewAddCondition == BASIC_STRUCTS.EMF_EDIT_CONDITION)
                {
                    emfPoint.SetSerial(pointSerial);

                    if (!emfPoint.EditEMFPoint())
                        return;

                    this.Close();
                }
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[A-Za-z]+");
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
