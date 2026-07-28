using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for CompanyEquipment.xaml
    /// </summary>
    public partial class CompanyEquipmentPage : Page
    {
        private Employee loggedInUser;
        private readonly CommonQueries commonQueries;

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> handheldUnits;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> vehicles;

        private readonly ObservableCollection<NumberedRow> handheldRows;
        private readonly ObservableCollection<NumberedRow> vehicleRows;

        public CompanyEquipmentPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            commonQueries = new CommonQueries();

            handheldUnits = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            vehicles = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            handheldRows = new ObservableCollection<NumberedRow>();
            vehicleRows = new ObservableCollection<NumberedRow>();

            if (!commonQueries.GetHandhelds(ref handheldUnits))
                return;

            if (!commonQueries.GetCompanyVehicles(ref vehicles))
                return;

            InitializeComponent();

            pageHeader.Attach(loggedInUser);

            handheldList.ItemsSource = handheldRows;
            vehiclesList.ItemsSource = vehicleRows;

            RebuildRows();
        }

        private void RebuildRows()
        {
            handheldRows.Clear();
            for (int i = 0; i < handheldUnits.Count; i++)
                handheldRows.Add(new NumberedRow { Number = (i + 1) + "-", Text = handheldUnits[i].value });

            vehicleRows.Clear();
            for (int i = 0; i < vehicles.Count; i++)
                vehicleRows.Add(new NumberedRow { Number = (i + 1) + "-", Text = vehicles[i].value });
        }

        private void OnCheckedHandheld(object sender, RoutedEventArgs e)
        {
            if (vehiclesList == null)
                return;

            handheldList.Visibility = Visibility.Visible;
            vehiclesList.Visibility = Visibility.Collapsed;
        }

        private void OnCheckedVehicles(object sender, RoutedEventArgs e)
        {
            handheldList.Visibility = Visibility.Collapsed;
            vehiclesList.Visibility = Visibility.Visible;
        }

        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            CompanyEquipmentWindow companyEquipmentWindow = new CompanyEquipmentWindow(ref loggedInUser);
            companyEquipmentWindow.Closed += OnClosedCompanyEquipmentWindow;
            companyEquipmentWindow.Show();
        }

        private void OnClosedCompanyEquipmentWindow(object sender, EventArgs e)
        {
            if (!commonQueries.GetCompanyVehicles(ref vehicles))
                return;

            if (!commonQueries.GetHandhelds(ref handheldUnits))
                return;

            RebuildRows();
        }
    }
}
