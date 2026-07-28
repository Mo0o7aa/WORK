using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for CompaniesPage.xaml
    /// </summary>
    public partial class CompaniesPage : Page
    {
        private Employee loggedInUser;

        private readonly CommonQueries commonQueries;

        private List<BASIC_STRUCTS.COMPANY_STRUCT> companies;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> workFields;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> bands;
        private List<BASIC_STRUCTS.BAND_UNIT_STRUCT> bandUnits;

        private readonly ObservableCollection<CompanyRow> companyRows;
        private ICollectionView companiesView;

        public CompaniesPage(ref Employee mLoggedInUser)
        {
            commonQueries = new CommonQueries();

            loggedInUser = mLoggedInUser;

            companies = new List<BASIC_STRUCTS.COMPANY_STRUCT>();
            bands = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            bandUnits = new List<BASIC_STRUCTS.BAND_UNIT_STRUCT>();
            workFields = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            companyRows = new ObservableCollection<CompanyRow>();

            InitializeComponent();

            pageHeader.Attach(loggedInUser);

            if (!commonQueries.GetCompanies(ref companies))
                return;

            if (!commonQueries.GetBands(ref bands))
                return;

            if (!commonQueries.GetBandUnits(ref bandUnits))
                return;

            for (int i = 0; i < bandUnits.Count; i++)
                bandsComboBox.Items.Add(bandUnits[i].unit);

            InitializeWorkFieldCombo();

            companiesView = CollectionViewSource.GetDefaultView(companyRows);
            companiesView.Filter = FilterCompany;

            companiesList.ItemsSource = companiesView;
            companiesDataGrid.ItemsSource = companiesView;

            RebuildRows();
        }

        private bool InitializeWorkFieldCombo()
        {
            if (!commonQueries.GetCompanyFieldOfWork(ref workFields))
                return false;

            for (int i = 0; i < workFields.Count; i++)
                fieldComboBox.Items.Add(workFields[i].value);

            return true;
        }

        private void RebuildRows()
        {
            companyRows.Clear();

            for (int i = 0; i < companies.Count; i++)
                companyRows.Add(new CompanyRow(companies[i]));

            UpdateCount();
        }

        private bool FilterCompany(object item)
        {
            CompanyRow row = item as CompanyRow;
            if (row == null)
                return false;

            if (searchCheckBox.IsChecked == true && searchTextBox.Text != "")
            {
                if (row.Name == null || row.Name.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
                    return false;
            }

            if (fieldOfWorkCheckBox.IsChecked == true && fieldComboBox.SelectedIndex != -1)
            {
                if (row.WorkFieldId != workFields[fieldComboBox.SelectedIndex].key)
                    return false;
            }

            if (bandsCheckBox.IsChecked == true && bandsComboBox.SelectedIndex != -1 && bandTextBox.Text != "")
            {
                decimal enteredValue;
                if (decimal.TryParse(bandTextBox.Text, out enteredValue))
                {
                    decimal frequency = enteredValue * bandUnits[bandsComboBox.SelectedIndex].factor;

                    if (!row.MatchesBand(frequency))
                        return false;
                }
            }

            return true;
        }

        private void RefreshView()
        {
            if (companiesView == null)
                return;

            companiesView.Refresh();
            UpdateCount();
        }

        private void UpdateCount()
        {
            if (companiesView == null)
                return;

            int count = 0;
            foreach (object item in companiesView)
                count++;

            countText.Text = count.ToString();
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
            int companySerial = int.Parse(((Button)sender).Tag.ToString());

            Company company = new Company();

            if (!company.InitializeCompanyInfo(companySerial))
                return;

            int companyCondition = BASIC_STRUCTS.COMPANY_EDIT_CONDITION;

            CompanyWindow companyWindow = new CompanyWindow(ref loggedInUser, ref company, ref companyCondition);
            companyWindow.Closed += OnClosedCompanyWindow;
            companyWindow.Show();
        }

        private void OnClickViewCompany(object sender, RoutedEventArgs e)
        {
            int companySerial = int.Parse(((Button)sender).Tag.ToString());

            Company company = new Company();

            if (!company.InitializeCompanyInfo(companySerial))
                return;

            int companyCondition = BASIC_STRUCTS.COMPANY_VIEW_CONDITION;

            CompanyWindow companyWindow = new CompanyWindow(ref loggedInUser, ref company, ref companyCondition);
            companyWindow.Show();
        }

        private void OnClosedCompanyWindow(object sender, EventArgs e)
        {
            if (!commonQueries.GetCompanies(ref companies))
                return;

            RebuildRows();
        }

        private void OnCheckedListView(object sender, RoutedEventArgs e)
        {
            if (companiesDataGrid == null)
                return;

            companiesList.Visibility = Visibility.Visible;
            companiesDataGrid.Visibility = Visibility.Collapsed;
        }

        private void OnCheckedTableView(object sender, RoutedEventArgs e)
        {
            companiesList.Visibility = Visibility.Collapsed;
            companiesDataGrid.Visibility = Visibility.Visible;
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
            RefreshView();
        }

        private void OnSelChangedFieldCombo(object sender, SelectionChangedEventArgs e)
        {
            RefreshView();
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

            RefreshView();
        }

        private void OnSelChangedBandsCombo(object sender, SelectionChangedEventArgs e)
        {
            if (bandTextBox.Text != "")
                RefreshView();
        }

        private void OnTextChangedBandTextBox(object sender, TextChangedEventArgs e)
        {
            if (bandsComboBox.SelectedIndex != -1)
                RefreshView();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
