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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        Employee loggeInUser;
        CommonQueries commonQueries;

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> departments;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> titles;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> positions;

        public SignUpPage()
        {
            loggeInUser = new Employee();
            commonQueries = new CommonQueries();

            departments = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            titles = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            positions = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            InitializeComponent();

            InitializeDepartmentsCombo();


        }

        private bool InitializeDepartmentsCombo()
        {
            if (!commonQueries.GetDepartments(ref departments))
                return false;

            for(int i = 0; i < departments.Count; i++)
            {
                deprtmentComboBox.Items.Add(departments[i].value);
            }

            return true;
        }


        private void OnBtnClickSaveChanges(object sender, RoutedEventArgs e)
        {
            if (usernameTextBox.Text == "")
            {
                MessageBox.Show("Username must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (nameTextBox.Text == "")
            {
                MessageBox.Show("Name must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (emailTextBox.Text == "")
            {
                MessageBox.Show("Email must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (telephoneTextBox.Text == "")
            {
                MessageBox.Show("Telephone must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (deprtmentComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Department must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (passwordTextBox.Password == "")
            {
                MessageBox.Show("Password must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (confirmPasswordTextBox.Password == "")
            {
                MessageBox.Show("Confirm password must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (passwordTextBox.Password != confirmPasswordTextBox.Password)
            {
                MessageBox.Show("Password and confirm password are not matching!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!loggeInUser.GetNewEmployeeId())
                return;

            loggeInUser.SetEmployeeUserName(usernameTextBox.Text);
            loggeInUser.SetEmployeeName(nameTextBox.Text);
            loggeInUser.SetEmployeeEmail(emailTextBox.Text);

            List<String> tempPhones = new List<string>();
            tempPhones.Add(telephoneTextBox.Text);

            loggeInUser.SetEmployeePhones(ref tempPhones);

            loggeInUser.SetEmloyeeDepartmentId(departments[deprtmentComboBox.SelectedIndex].key);

            if (!loggeInUser.InsertIntoEmployeeEmails())
                return;
            if (!loggeInUser.InsertIntoEmployeeInfo())
                return;
            if (!loggeInUser.InsertIntoEmployeePasswords(passwordTextBox.Password))
                return;
            if (!loggeInUser.InsertIntoEmployeeTelephones())
                return;

            SignInPage signInPage = new SignInPage();
            NavigationService.Navigate(signInPage);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
