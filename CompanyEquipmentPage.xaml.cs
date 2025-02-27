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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for CompanyEquipment.xaml
    /// </summary>
    public partial class CompanyEquipmentPage : Page
    {
        Employee loggedInUser;
        CommonQueries commonQueries;

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> handheldUnits;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> vehicles;

        public CompanyEquipmentPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            commonQueries = new CommonQueries();

            handheldUnits = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            vehicles = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();


            if (!commonQueries.GetHandhelds(ref handheldUnits))
                return;

            if (!commonQueries.GetCompanyVehicles(ref vehicles))
                return;

            InitializeComponent();

            userNameLabel.Content = "Username: " + loggedInUser.GetEmployeeUserName();

            InitializeHandheldGrid();
            InitializeVehiclesGrid();
        }

        private void InitializeHandheldGrid()
        {
            handheldGrid.Children.Clear();
            handheldGrid.RowDefinitions.Clear();

            for(int i = 0; i < handheldUnits.Count; i++)
            {
                handheldGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(70) });

                WrapPanel wrapPanel = new WrapPanel();
                wrapPanel.Margin = new Thickness(12);

                Label numberLabel = new Label();
                numberLabel.Style = (Style)FindResource("miniLabelStyle");
                numberLabel.Content = i + 1 + "- ";
                numberLabel.FontWeight = FontWeights.Bold;

                Label unitNameLabel = new Label();
                unitNameLabel.Style = (Style)FindResource("wideLabelStyle");
                unitNameLabel.Content = handheldUnits[i].value;
                unitNameLabel.FontWeight = FontWeights.Bold;

                wrapPanel.Children.Add(numberLabel);
                wrapPanel.Children.Add(unitNameLabel);

                handheldGrid.Children.Add(wrapPanel);
                Grid.SetRow(wrapPanel, handheldGrid.RowDefinitions.Count - 1);
            }
        }

        private void InitializeVehiclesGrid()
        {
            vehiclesGrid.Children.Clear();
            vehiclesGrid.RowDefinitions.Clear();

            for (int i = 0; i < vehicles.Count; i++)
            {
                vehiclesGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(70) });

                WrapPanel wrapPanel = new WrapPanel();
                wrapPanel.Margin = new Thickness(12);

                Label numberLabel = new Label();
                numberLabel.Style = (Style)FindResource("miniLabelStyle");
                numberLabel.Content = i + 1 + "- ";
                numberLabel.FontWeight = FontWeights.Bold;

                Label unitNameLabel = new Label();
                unitNameLabel.Style = (Style)FindResource("wideLabelStyle");
                unitNameLabel.Content = vehicles[i].value;
                unitNameLabel.FontWeight = FontWeights.Bold;

                wrapPanel.Children.Add(numberLabel);
                wrapPanel.Children.Add(unitNameLabel);

                vehiclesGrid.Children.Add(wrapPanel);
                Grid.SetRow(wrapPanel, vehiclesGrid.RowDefinitions.Count - 1);
            }
        }

        private void DashboardMenuSelection(object sender, RoutedEventArgs e)
        {
            DashBoard dashBoard = new DashBoard(ref loggedInUser);
            NavigationService.Navigate(dashBoard);
        }

        private void employeeMenuSelection(object sender, RoutedEventArgs e)
        {
            EmployeesPage employeesPage = new EmployeesPage(ref loggedInUser);
            NavigationService.Navigate(employeesPage);
        }

        private void CompanyMenuSelection(object sender, RoutedEventArgs e)
        {
            CompaniesPage companiesPage = new CompaniesPage(ref loggedInUser);
            NavigationService.Navigate(companiesPage);
        }
        private void SitesMenuSelection(object sender, RoutedEventArgs e)
        {
            SitesPage sitesPage = new SitesPage(ref loggedInUser);
            NavigationService.Navigate(sitesPage);
        }

        private void emfMenuSelection(object sender, RoutedEventArgs e)
        {
            EMFPage emfPage = new EMFPage(ref loggedInUser);
            NavigationService.Navigate(emfPage);
        }

        private void ComplaintsMenuSelection(object sender, RoutedEventArgs e)
        {
            ComplaintsPage complaintsPage = new ComplaintsPage(ref loggedInUser);
            NavigationService.Navigate(complaintsPage);
        }

        private void MonitoringMenuSelection(object sender, RoutedEventArgs e)
        {
            MissionsPage missionsPage = new MissionsPage(ref loggedInUser);
            NavigationService.Navigate(missionsPage);
        }

        private void InspectionMenuSelection(object sender, RoutedEventArgs e)
        {

        }

        private void OnClickHandheld(object sender, MouseButtonEventArgs e)
        {
            BrushConverter brushConverter = new BrushConverter();

            handheldScrollViewer.Visibility = Visibility.Visible;
            vehiclesScrollViewer.Visibility = Visibility.Collapsed;

            handHeldBorder.Background = Brushes.White;
            handheldLabel.Foreground = (Brush)brushConverter.ConvertFrom("#000080");


            vehiclesBorder.Background = (Brush)brushConverter.ConvertFrom("#000080");
            vehiclesLabel.Foreground = Brushes.White;
        }

        private void OnClickVehicles(object sender, MouseButtonEventArgs e)
        {
            BrushConverter brushConverter = new BrushConverter();

            handheldScrollViewer.Visibility = Visibility.Collapsed;
            vehiclesScrollViewer.Visibility = Visibility.Visible;

            handHeldBorder.Background = (Brush)brushConverter.ConvertFrom("#000080");
            handheldLabel.Foreground = Brushes.White;


            vehiclesBorder.Background = Brushes.White;
            vehiclesLabel.Foreground = (Brush)brushConverter.ConvertFrom("#000080");
        }

        private void MouseEnterMainMenu(object sender, MouseEventArgs e)
        {
            mainMenuGrid.Background = SystemColors.ActiveBorderBrush;
        }

        private void MouseLeaveMainMenu(object sender, MouseEventArgs e)
        {
            mainMenuGrid.Background = Brushes.LightGray;

        }

        private void OnClickMainMenu(object sender, MouseButtonEventArgs e)
        {
            if (tabMenuButton.ContextMenu.IsOpen == false)
                tabMenuButton.ContextMenu.IsOpen = true;
            else
                tabMenuButton.ContextMenu.IsOpen = false;
        }

        private void OnClickLogoutButton(object sender, RoutedEventArgs e)
        {
            SignInWindow signInWindow = new SignInWindow();
            signInWindow.Show();

            NavigationWindow parentWindow = (NavigationWindow)this.Parent;
            parentWindow.Close();
        }

        private void OnClickChangePasswordButton(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow(ref loggedInUser);
            changePasswordWindow.Show();
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

            InitializeHandheldGrid();
            InitializeVehiclesGrid();
        }

        private void RepeatersMenueSelection(object sender, RoutedEventArgs e)
        {
            RepeatersPage repeatersPage = new RepeatersPage(ref loggedInUser);
            NavigationService.Navigate(repeatersPage);
        }
    }
}
