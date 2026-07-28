using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for RepeatersPage.xaml
    /// </summary>
    public partial class RepeatersPage : Page
    {
        private Employee loggedInUser;
        private readonly CommonQueries commonQueries;

        private List<BASIC_STRUCTS.REPEATER_STRUCT> repeaters;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> repeatersStatus;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> cities;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> areas;

        private readonly ObservableCollection<RepeaterRow> repeaterRows;
        private ICollectionView repeatersView;

        public RepeatersPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;
            commonQueries = new CommonQueries();

            repeaters = new List<BASIC_STRUCTS.REPEATER_STRUCT>();
            repeatersStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            cities = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            areas = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            repeaterRows = new ObservableCollection<RepeaterRow>();

            if (!commonQueries.GetRepeaters(ref repeaters))
                return;

            if (!commonQueries.GetRepeatersStatus(ref repeatersStatus))
                return;

            if (!commonQueries.GetCities(ref cities))
                return;

            InitializeComponent();

            pageHeader.Attach(loggedInUser);

            repeatersView = CollectionViewSource.GetDefaultView(repeaterRows);
            repeatersView.Filter = FilterRepeater;

            repeatersList.ItemsSource = repeatersView;
            repeatersDataGrid.ItemsSource = repeatersView;

            cityCheckBox.IsChecked = true;

            InitializeRepeatersStatusCombo();
            InitializeCityCombo();

            RebuildRows();
        }

        private void InitializeRepeatersStatusCombo()
        {
            for (int i = 0; i < repeatersStatus.Count; i++)
                statusComboBox.Items.Add(repeatersStatus[i].value);
        }

        private void InitializeCityCombo()
        {
            for (int i = 0; i < cities.Count; i++)
                cityComboBox.Items.Add(cities[i].value);
        }

        private void RebuildRows()
        {
            repeaterRows.Clear();

            for (int i = 0; i < repeaters.Count; i++)
                repeaterRows.Add(new RepeaterRow(repeaters[i]));

            UpdateCount();
        }

        private bool FilterRepeater(object item)
        {
            RepeaterRow row = item as RepeaterRow;
            if (row == null)
                return false;

            if (searchLatCheckBox.IsChecked == true && latTextBox.Text != "")
            {
                if (row.Latitude == null || row.Latitude.IndexOf(latTextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
                    return false;
            }

            if (searchLongCheckBox.IsChecked == true && longTextBox.Text != "")
            {
                if (row.Longitude == null || row.Longitude.IndexOf(longTextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
                    return false;
            }

            if (cityCheckBox.IsChecked == true && cityComboBox.SelectedIndex != -1)
            {
                if (row.CityId != cities[cityComboBox.SelectedIndex].key)
                    return false;
            }

            if (areaCheckBox.IsChecked == true && areaComboBox.SelectedIndex != -1)
            {
                if (row.Area != areas[areaComboBox.SelectedIndex].value)
                    return false;
            }

            if (statusCheckBox.IsChecked == true && statusComboBox.SelectedIndex != -1)
            {
                if (row.StatusId != repeatersStatus[statusComboBox.SelectedIndex].key)
                    return false;
            }

            return true;
        }

        private void RefreshView()
        {
            if (repeatersView == null)
                return;

            repeatersView.Refresh();
            UpdateCount();
        }

        private void UpdateCount()
        {
            if (repeatersView == null)
                return;

            int count = 0;
            foreach (object item in repeatersView)
                count++;

            countText.Text = count.ToString();
        }

        private void OnClickCopyCoordinates(object sender, MouseButtonEventArgs e)
        {
            RepeaterRow row = (RepeaterRow)((TextBlock)sender).Tag;

            Clipboard.SetText(row.Latitude + ", " + row.Longitude);

            MessageBox.Show("Copied to clipboard!", "Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnClickEdit(object sender, RoutedEventArgs e)
        {
            // not implemented (same as before)
        }

        private void OnCheckedListView(object sender, RoutedEventArgs e)
        {
            if (repeatersDataGrid == null)
                return;

            repeatersList.Visibility = Visibility.Visible;
            repeatersDataGrid.Visibility = Visibility.Collapsed;
        }

        private void OnCheckedTableView(object sender, RoutedEventArgs e)
        {
            repeatersList.Visibility = Visibility.Collapsed;
            repeatersDataGrid.Visibility = Visibility.Visible;
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
                    areaComboBox.Items.Add(areas[i].value);

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
            RefreshView();
        }

        private void OnTextChangedLongTextBox(object sender, TextChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnSelChangedCityCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnSelChangedAreaCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnSelChangedStatusCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            // not implemented (same as before)
        }

        private void OnBtnClickMapToExcel(object sender, RoutedEventArgs e)
        {
            MapLinkWindow mapLinkWindow = new MapLinkWindow();
            mapLinkWindow.Show();
        }

        private void OnBtnClickFormat(object sender, RoutedEventArgs e)
        {
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

                RebuildRows();
            }
        }

        private void OnBtnClickExport(object sender, RoutedEventArgs e)
        {
            List<BASIC_STRUCTS.REPEATER_STRUCT> selectedRepeaters = new List<BASIC_STRUCTS.REPEATER_STRUCT>();

            foreach (object item in repeatersView)
            {
                RepeaterRow row = (RepeaterRow)item;
                selectedRepeaters.Add(repeaters.Find(x1 => x1.repeater_serial == row.Serial));
            }

            ExcelExport export = new ExcelExport();
            export.ExportRepeaters(ref selectedRepeaters);
        }
    }
}
