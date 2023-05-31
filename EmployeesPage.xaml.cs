using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {
        Employee loggedInUser;
        Employee selectedEmployee;
        CommonQueries commonQueries;

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> departments;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> titles;
        private List<BASIC_STRUCTS.EMPLOYEE_MIN_STRUCT> employees;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> positions;

        bool edit = false;

        public EmployeesPage(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;
            selectedEmployee = new Employee();


            commonQueries = new CommonQueries();

            departments = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            titles = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            employees = new List<BASIC_STRUCTS.EMPLOYEE_MIN_STRUCT>();
            positions = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            InitializeComponent();

            InitializeDepartmentsCombo();
            InitializeTitlesCombo();
            InitializePositionsCombo();

            userNameLabel.Content = "Username: " + loggedInUser.GetEmployeeUserName();

            if (!commonQueries.GetEngineers(ref employees))
                return;

            InitializeEmployeeCombo();
            SetPersonalInfo();

            if (selectedEmployee.GetEmployeePositionId() == BASIC_STRUCTS.MANAGER_POSITION)
            {
                employeeSelectionStackPanel.Visibility = Visibility.Visible;
            }

            if (loggedInUser.GetEmployeePositionId() == BASIC_STRUCTS.MANAGER_POSITION)
            {
                mainLabel.Content = "Manager Profile";
            }

        }

        private void InitializeEmployeeCombo()
        {
            employeeCombo.Items.Clear();

            for(int i = 0; i < employees.Count; i++)
            {
                employeeCombo.Items.Add(employees[i].username);
            }

            employeeCombo.SelectedIndex = employees.FindIndex(x1 => x1.employee_id == loggedInUser.GetEmployeeId());
        }
        private bool InitializeDepartmentsCombo()
        {
            if (!commonQueries.GetDepartments(ref departments))
                return false;

            for(int i = 0; i < departments.Count; i++)
            {
                employeeDepartmentComboBox.Items.Add(departments[i].value);
            }

            return true;
        }

        private bool InitializeTitlesCombo()
        {
            if (!commonQueries.GetTitles(ref titles))
                return false;

            for (int i = 0; i < titles.Count; i++)
            {
                employeeTitleComboBox.Items.Add(titles[i].value);
            }

            return true;
        }

        private bool InitializePositionsCombo()
        {
            if (!commonQueries.GetPositions(ref positions))
                return false;

            for (int i = 0; i < positions.Count; i++)
            {
                employeePositionComboBox.Items.Add(positions[i].value);
            }

            return true;
        }

        private void SetPersonalInfo()
        {
            employeeNameTextBox.Text = selectedEmployee.GetEmployeeName();
            employeeDepartmentComboBox.SelectedIndex = departments.FindIndex(x1 => x1.key == selectedEmployee.GetEmloyeeDepartmentId());
            employeeTitleComboBox.SelectedIndex = titles.FindIndex(x1 => x1.key == selectedEmployee.GetEmployeeTitleId());
            employeePositionComboBox.SelectedIndex = positions.FindIndex(x1 => x1.key == selectedEmployee.GetEmployeePositionId());
            employeeEmailTextBox.Text = selectedEmployee.GetEmployeeEmail();

            employeePhoneTextBox.Text = selectedEmployee.GetEmployeePhones()[0];

            if (selectedEmployee.GetEmployeePhones().Count > 1)
            {
                for (int i = 1; i < selectedEmployee.GetEmployeePhones().Count; i++)
                {
                    TextBox textBox = new TextBox();
                    textBox.Style = (Style)FindResource("textboxStyle");
                    textBox.Margin = new Thickness(12, 0, 0, 0);
                    textBox.Text = selectedEmployee.GetEmployeePhones()[i];

                    telephoneWrapPanel.Children.Add(textBox);
                }
            }
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

        private void DashboardMenuSelection(object sender, RoutedEventArgs e)
        {

        }

        private void CompanyMenuSelection(object sender, RoutedEventArgs e)
        {
            CompaniesPage companiesPage = new CompaniesPage(ref loggedInUser);
            NavigationService.Navigate(companiesPage);
        }


        private void CompanyEquipmentMenuSelection(object sender, RoutedEventArgs e)
        {
            CompanyEquipmentPage companyEquipmentPage = new CompanyEquipmentPage(ref loggedInUser);
            NavigationService.Navigate(companyEquipmentPage);
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

        private void InspectionMenuSelection(object sender, RoutedEventArgs e)
        {

        }

        private void MonitoringMissionsMenuSelection(object sender, RoutedEventArgs e)
        {
            MissionsPage missionsPage = new MissionsPage(ref loggedInUser);
            NavigationService.Navigate(missionsPage);
        }

        private void OnClickChangePasswordButton(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow(ref loggedInUser);
            changePasswordWindow.Show();
        }

        private void OnClickLogoutButton(object sender, RoutedEventArgs e)
        {
            SignInWindow signInWindow = new SignInWindow();
            signInWindow.Show();

            NavigationWindow parentWindow = (NavigationWindow)this.Parent;
            parentWindow.Close();
        }

        private void OnBtnClickEdit(object sender, RoutedEventArgs e)
        {
            if(edit == false)
            {
                addTelephoneButton.IsEnabled = true;
                removeTelephoneButton.IsEnabled = true;

                edit = true;
                editButton.Content = "Save Changes";

                employeeNameTextBox.IsReadOnly = false;
                employeeDepartmentComboBox.IsEnabled = true;
                employeeTitleComboBox.IsEnabled = true;
                employeePositionComboBox.IsEnabled = true;
                employeeEmailTextBox.IsReadOnly = false;

                for(int i = 1; i < telephoneWrapPanel.Children.Count; i++)
                {
                    TextBox currentTextBox = (TextBox)telephoneWrapPanel.Children[i];
                    currentTextBox.IsReadOnly = false;
                }
            }
            else
            {
                addTelephoneButton.IsEnabled = false;
                removeTelephoneButton.IsEnabled = false;
                edit = false;
                editButton.Content = "Edit Info";

                ////update info here

                selectedEmployee.SetEmployeeName(employeeNameTextBox.Text);

                selectedEmployee.SetEmloyeeDepartmentId(departments[employeeDepartmentComboBox.SelectedIndex].key);
                selectedEmployee.SetEmployeeDepartment(departments[employeeDepartmentComboBox.SelectedIndex].value);

                selectedEmployee.SetEmployeeTitleId(titles[employeeTitleComboBox.SelectedIndex].key);
                selectedEmployee.SetEmployeeTitle(titles[employeeTitleComboBox.SelectedIndex].value);

                selectedEmployee.SetEmployeePositionId(positions[employeePositionComboBox.SelectedIndex].key);
                selectedEmployee.SetEmployeePosition(positions[employeePositionComboBox.SelectedIndex].value);

                selectedEmployee.SetEmployeeEmail(employeeEmailTextBox.Text);

                List<String> tempPhones = new List<string>();

                for(int i = 1; i < telephoneWrapPanel.Children.Count; i++)
                {
                    TextBox currentTextBox = (TextBox)telephoneWrapPanel.Children[i];
                    tempPhones.Add(currentTextBox.Text);
                }

                selectedEmployee.SetEmployeePhones(ref tempPhones);

                if (!selectedEmployee.UpdateEmployeeInfo())
                    return;

                if (!selectedEmployee.DeleteEmployeeTelephones())
                    return;

                if (!selectedEmployee.InsertIntoEmployeeTelephones())
                    return;

                if(selectedEmployee.GetEmployeeId() == loggedInUser.GetEmployeeId())
                {
                    if(!loggedInUser.InitializeEmployeeInfo(selectedEmployee.GetEmployeeId()))
                    {
                        MessageBox.Show("Server connection failed! Check your internet connection.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }    
                }

                employeeNameTextBox.IsReadOnly = true;
                employeeDepartmentComboBox.IsEnabled = false;
                employeeTitleComboBox.IsEnabled = false;
                employeePositionComboBox.IsEnabled = false;
                employeeEmailTextBox.IsReadOnly = true;

                for (int i = 1; i < telephoneWrapPanel.Children.Count; i++)
                {
                    TextBox currentTextBox = (TextBox)telephoneWrapPanel.Children[i];
                    currentTextBox.IsReadOnly = true;
                }
            }
        }

        private void OnBtnClickAddTelephone(object sender, RoutedEventArgs e)
        {
            if (telephoneWrapPanel.Children.Count <= 3)
            {
                BrushConverter brushConverter = new BrushConverter();

                TextBox textBox = new TextBox();
                textBox.Style = (Style)FindResource("textboxStyle");
                textBox.Width = 250;
                textBox.Margin = new Thickness(12, 0, 0, 0);
                textBox.FontWeight = FontWeights.Bold;
                textBox.Foreground = (Brush)brushConverter.ConvertFrom("#000080");
                textBox.PreviewTextInput += NumberValidationTextBox;

                telephoneWrapPanel.Children.Add(textBox);
            }
            else
            {
                MessageBox.Show("Too many telephones for a single user!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnBtnClickRemoveTelephone(object sender, RoutedEventArgs e)
        {
            if(telephoneWrapPanel.Children.Count == 2)
            {
                MessageBox.Show("Min number of telephones per user reached!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                telephoneWrapPanel.Children.RemoveAt(telephoneWrapPanel.Children.Count - 1);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        

        private void OnSelChangedEmployeeComboBox(object sender, SelectionChangedEventArgs e)
        {
            if(employeeCombo.SelectedIndex != -1 && edit == false)
            {
                if (selectedEmployee.GetEmployeeId() != employees[employeeCombo.SelectedIndex].employee_id)
                {
                    if (!selectedEmployee.InitializeEmployeeInfo(employees[employeeCombo.SelectedIndex].employee_id))
                    {
                        MessageBox.Show("Server connection failed! Please check your internet connection.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        employeeCombo.SelectedIndex = employees.FindIndex(x1 => x1.employee_id == loggedInUser.GetEmployeeId());

                    }
                    SetPersonalInfo();
                }
            }
            else
            {
                if (employeeCombo.SelectedIndex != employees.FindIndex(x1 => x1.employee_id == selectedEmployee.GetEmployeeId()))
                {
                    MessageBox.Show("Editing employee info in progress! save changes first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    employeeCombo.SelectedIndex = employees.FindIndex(x1 => x1.employee_id == selectedEmployee.GetEmployeeId());
                }
            }
        }

    }
}
