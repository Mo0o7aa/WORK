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
    /// Interaction logic for SiteWindow.xaml
    /// </summary>
    public partial class SiteWindow : Window
    {
        private Employee loggedInUser;
        private Site site;
        private int viewAddCondition;
        private CommonQueries commonQueries;

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> companies;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> cities;

        public SiteWindow(ref Employee mLoggedInUser, ref Site mSite, ref int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            site = mSite;
            viewAddCondition = mViewAddCondition;

            commonQueries = new CommonQueries();

            companies = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            cities = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            InitializeComponent();

            InitializeCompanyCombo();
            InitializeCityCombo();

            if(viewAddCondition != BASIC_STRUCTS.SITE_ADD_CONDITION)
            {
                SetUIElements();

                if(viewAddCondition == BASIC_STRUCTS.SITE_VIEW_CONDITION)
                    DisableUIElements();
            }
        }

        private void DisableUIElements()
        {
            companyNameComboBox.IsEnabled = false;
            siteNumberTextBox.IsReadOnly = true;
            cityComboBox.IsEnabled = false;
            regionTextBox.IsReadOnly = true;
            latTextBox.IsReadOnly = true;
            longTextBox.IsReadOnly = true;

            finishButton.IsEnabled = false;
        }

        private void SetUIElements()
        {
            companyNameComboBox.Text = site.GetCompanyName();
            siteNumberTextBox.Text = site.GetSiteNumber();
            cityComboBox.Text = site.GetCity();
            regionTextBox.Text = site.GetRegion();
            latTextBox.Text = site.GetLat();
            longTextBox.Text = site.GetLong();
        }

        private void InitializeCompanyCombo()
        {
            if (!commonQueries.GetCompanies(ref companies))
                return;

            companyNameComboBox.Items.Clear();

            for (int i = 0; i < companies.Count; i++)
            {
                companyNameComboBox.Items.Add(companies[i].value);
            }
        }

        private void InitializeCityCombo()
        {
            if (!commonQueries.GetCities(ref cities))
                return;

            cityComboBox.Items.Clear();

            for(int i = 0; i < cities.Count; i++)
            {
                cityComboBox.Items.Add(cities[i].value);
            }
        }

        private void OnSelChangedCompanyNameCombo(object sender, SelectionChangedEventArgs e)
        {
            if (companyNameComboBox.SelectedIndex != -1)
            {
                site.SetCompanyName(companies[companyNameComboBox.SelectedIndex].value);
                site.SetCompanySerial(companies[companyNameComboBox.SelectedIndex].key);
            }
        }

        private void OnSelChangedCityCombo(object sender, SelectionChangedEventArgs e)
        {
            if (cityComboBox.SelectedIndex != -1)
            {
                site.SetCity(cities[cityComboBox.SelectedIndex].value);
                site.SetCityId(cities[cityComboBox.SelectedIndex].key);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[A-Za-z]+");
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OnButtonClickSaveChanges(object sender, RoutedEventArgs e)
        {
            if (companyNameComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Company must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (siteNumberTextBox.Text == "")
            {
                MessageBox.Show("Site Number must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (cityComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("City must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (regionTextBox.Text == "")
            {
                MessageBox.Show("Region must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (latTextBox.Text == "")
            {
                MessageBox.Show("Latitude must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (longTextBox.Text == "")
            {
                MessageBox.Show("Longitude must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            site.SetSiteNumber(siteNumberTextBox.Text);

            if(!commonQueries.CheckSiteNameExists(site.GetCompanySerial(), site.GetSiteNumber()))
            {
                MessageBox.Show("Site " + site.GetSiteNumber() + " already exists for company " + site.GetCompanyName() + "!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            site.SetRegion(regionTextBox.Text);
            site.SetLat(latTextBox.Text);
            site.SetLong(longTextBox.Text);
            site.SetAddedBy(loggedInUser.GetEmployeeName());
            site.SetAddedById(loggedInUser.GetEmployeeId());

            if(viewAddCondition == BASIC_STRUCTS.SITE_ADD_CONDITION)
            {
                if (!site.IssueNewSite())
                    return;

                this.Close();
            }
            else
            {
                if (!site.EditSite())
                    return;

                this.Close();
            }
        }

        
    }
}
