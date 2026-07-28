using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for SitesPage.xaml
    /// </summary>
    public partial class SitesPage : Page
    {
        private Employee loggedInUser;
        private readonly CommonQueries commonQueries;

        private List<BASIC_STRUCTS.MIN_SITE_STRUCT> sites;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> companies;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> cities;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> employees;

        private readonly ObservableCollection<SiteRow> siteRows;
        private ICollectionView sitesView;

        private readonly Popup popUp;
        private readonly DispatcherTimer dispatchertimer;

        public SitesPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;
            commonQueries = new CommonQueries();

            sites = new List<BASIC_STRUCTS.MIN_SITE_STRUCT>();
            companies = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            cities = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            employees = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            siteRows = new ObservableCollection<SiteRow>();

            popUp = new Popup();
            popUp.PopupAnimation = PopupAnimation.Fade;
            popUp.Placement = PlacementMode.Mouse;
            popUp.VerticalOffset = -20;
            popUp.AllowsTransparency = true;

            dispatchertimer = new DispatcherTimer();
            dispatchertimer.Tick += dispatcherTimer_Tick;
            dispatchertimer.Interval = new TimeSpan(0, 0, 0, 1, 50);

            InitializeComponent();

            pageHeader.Attach(loggedInUser);

            if (!commonQueries.GetAllSites(ref sites))
                return;

            sitesView = CollectionViewSource.GetDefaultView(siteRows);
            sitesView.Filter = FilterSite;

            sitesList.ItemsSource = sitesView;
            sitesDataGrid.ItemsSource = sitesView;

            RebuildRows();
            FillFilterComboBoxes();
        }

        private void FillFilterComboBoxes()
        {
            if (!commonQueries.GetCompanies(ref companies))
                return;

            companyComboBox.Items.Clear();
            for (int i = 0; i < companies.Count; i++)
                companyComboBox.Items.Add(companies[i].value);

            if (!commonQueries.GetCities(ref cities))
                return;

            cityComboBox.Items.Clear();
            for (int j = 0; j < cities.Count; j++)
                cityComboBox.Items.Add(cities[j].value);

            if (!commonQueries.GetEngineers(ref employees))
                return;

            addedByComboBox.Items.Clear();
            for (int k = 0; k < employees.Count; k++)
                addedByComboBox.Items.Add(employees[k].value);
        }

        private void RebuildRows()
        {
            siteRows.Clear();

            for (int i = 0; i < sites.Count; i++)
                siteRows.Add(new SiteRow(sites[i]));

            UpdateCount();
        }

        private bool FilterSite(object item)
        {
            SiteRow row = item as SiteRow;
            if (row == null)
                return false;

            if (searchCheckBox.IsChecked == true && searchTextBox.Text != "")
            {
                string siteNumberAndRegion = row.SiteNumber + " " + row.Region;
                if (siteNumberAndRegion.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
                    return false;
            }

            if (companyCheckBox.IsChecked == true && companyComboBox.SelectedIndex != -1)
            {
                if (row.CompanySerial != companies[companyComboBox.SelectedIndex].key)
                    return false;
            }

            if (cityCheckBox.IsChecked == true && cityComboBox.SelectedIndex != -1)
            {
                if (row.CityId != cities[cityComboBox.SelectedIndex].key)
                    return false;
            }

            if (addedByCheckBox.IsChecked == true && addedByComboBox.SelectedIndex != -1)
            {
                if (row.AddedById != employees[addedByComboBox.SelectedIndex].key)
                    return false;
            }

            if (longCheckBox.IsChecked == true && longTextBox.Text != "")
            {
                if (row.Longitude == null || row.Longitude.IndexOf(longTextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
                    return false;
            }

            if (latCheckBox.IsChecked == true && latTextBox.Text != "")
            {
                if (row.Latitude == null || row.Latitude.IndexOf(latTextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
                    return false;
            }

            return true;
        }

        private void RefreshView()
        {
            if (sitesView == null)
                return;

            sitesView.Refresh();
            UpdateCount();
        }

        private void UpdateCount()
        {
            if (sitesView == null)
                return;

            int count = 0;
            foreach (object item in sitesView)
                count++;

            countText.Text = count.ToString();
        }

        private void OnClickShowHistory(object sender, RoutedEventArgs e)
        {
            SiteRow row = (SiteRow)((Button)sender).Tag;

            List<BASIC_STRUCTS.COMPLAINT_STRUCT> tempComplaints = new List<BASIC_STRUCTS.COMPLAINT_STRUCT>();
            List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> tempMissions = new List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT>();

            if (!commonQueries.GetComplaints(ref tempComplaints, row.CompanySerial, row.SiteSerial))
                return;

            if (!commonQueries.GetMissions(ref tempMissions, row.CompanySerial, row.SiteSerial))
                return;

            ComplaintsHistoryWindow complaintsHistoryWindow = new ComplaintsHistoryWindow(ref loggedInUser, tempComplaints, tempMissions);
            complaintsHistoryWindow.Show();
        }

        private void OnClickEditSite(object sender, RoutedEventArgs e)
        {
            SiteRow row = (SiteRow)((Button)sender).Tag;

            Site currentSite = new Site();

            if (!currentSite.InitializeSite(row.CompanySerial, row.SiteSerial))
                return;

            int viewAddCondition = BASIC_STRUCTS.SITE_EDIT_CONDITION;

            SiteWindow siteWindow = new SiteWindow(ref loggedInUser, ref currentSite, ref viewAddCondition);
            siteWindow.Closed += OnClosedSiteWindow;
            siteWindow.Show();
        }

        private void OnClickCopyCooridinates(object sender, RoutedEventArgs e)
        {
            SiteRow row = (SiteRow)((Button)sender).Tag;

            Clipboard.SetText(row.Latitude + "," + row.Longitude);

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

        private void OnClosedSiteWindow(object sender, EventArgs e)
        {
            if (!commonQueries.GetAllSites(ref sites))
                return;

            RebuildRows();
        }

        private void OnCheckedListView(object sender, RoutedEventArgs e)
        {
            if (sitesDataGrid == null)
                return;

            sitesList.Visibility = Visibility.Visible;
            sitesDataGrid.Visibility = Visibility.Collapsed;
        }

        private void OnCheckedTableView(object sender, RoutedEventArgs e)
        {
            sitesList.Visibility = Visibility.Collapsed;
            sitesDataGrid.Visibility = Visibility.Visible;
        }

        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            Site site = new Site();
            int viewAddCondition = BASIC_STRUCTS.SITE_ADD_CONDITION;

            SiteWindow siteWindow = new SiteWindow(ref loggedInUser, ref site, ref viewAddCondition);
            siteWindow.Closed += OnClosedSiteWindow;
            siteWindow.Show();
        }

        private void OnCheckSearchCheckbox(object sender, RoutedEventArgs e)
        {
            searchTextBox.IsEnabled = true;
        }

        private void OnUncheckSearchCheckbox(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";
            searchTextBox.IsEnabled = false;
        }

        private void OnCheckCompanyCheckbox(object sender, RoutedEventArgs e)
        {
            companyComboBox.IsEnabled = true;
            companyComboBox.SelectedIndex = 0;
        }

        private void OnUncheckCompanyCheckbox(object sender, RoutedEventArgs e)
        {
            companyComboBox.SelectedIndex = -1;
            companyComboBox.IsEnabled = false;
        }

        private void OnCheckCityCheckbox(object sender, RoutedEventArgs e)
        {
            cityComboBox.IsEnabled = true;
            cityComboBox.SelectedIndex = 0;
        }

        private void OnUncheckCityCheckbox(object sender, RoutedEventArgs e)
        {
            cityComboBox.SelectedIndex = -1;
            cityComboBox.IsEnabled = false;
        }

        private void OnCheckAddedByCheckbox(object sender, RoutedEventArgs e)
        {
            addedByComboBox.IsEnabled = true;
            addedByComboBox.SelectedIndex = 0;
        }

        private void OnUncheckAddedByCheckBox(object sender, RoutedEventArgs e)
        {
            addedByComboBox.SelectedIndex = -1;
            addedByComboBox.IsEnabled = false;
        }

        private void OnCheckLongCheckbox(object sender, RoutedEventArgs e)
        {
            longTextBox.IsEnabled = true;
        }

        private void OnUncheckLongCheckBox(object sender, RoutedEventArgs e)
        {
            longTextBox.Text = "";
            longTextBox.IsEnabled = false;
        }

        private void OnCheckLatCheckbox(object sender, RoutedEventArgs e)
        {
            latTextBox.IsEnabled = true;
        }

        private void OnUncheckLatCheckBox(object sender, RoutedEventArgs e)
        {
            latTextBox.Text = "";
            latTextBox.IsEnabled = false;
        }

        private void OnTextChangedSearchTextbox(object sender, TextChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnSelChangedCompanyCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnSelChangedCityCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnSelChangedAddedByCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnTextChangedLongTextBox(object sender, TextChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnTextChangedLatTextBox(object sender, TextChangedEventArgs e)
        {
            RefreshView();
        }

        private void OnCopyDataGrid(object sender, DataGridRowClipboardEventArgs e)
        {
            if (sitesDataGrid.CurrentCell.Column == null)
                return;

            var currentCell = e.ClipboardRowContent[sitesDataGrid.CurrentCell.Column.DisplayIndex];
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(currentCell);
        }

        private void OnBtnClickImport(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String filePath = fileDialog.FileNames[0];
                String fileName = System.IO.Path.GetFileName(filePath);

                ExcelExport excelExport = new ExcelExport();
                excelExport.ImportCompanySites(fileName, filePath, ref loggedInUser);

                if (!commonQueries.GetAllSites(ref sites))
                    return;

                RebuildRows();
            }
        }

        private void OnBtnClickExport(object sender, RoutedEventArgs e)
        {
            List<BASIC_STRUCTS.MIN_SITE_STRUCT> selectedSites = new List<BASIC_STRUCTS.MIN_SITE_STRUCT>();

            foreach (object item in sitesView)
            {
                SiteRow row = (SiteRow)item;
                selectedSites.Add(sites.Find(x1 => x1.site_serial == row.SiteSerial));
            }

            ExcelExport export = new ExcelExport();
            export.ExportCompanySites(ref selectedSites);
        }
    }
}
