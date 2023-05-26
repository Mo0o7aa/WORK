using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
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
    /// Interaction logic for SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        SQLServer sql;
        CommonQueries commonQueries;
        public SignInPage()
        {
            sql = new SQLServer();
            commonQueries = new CommonQueries();

            InitializeComponent();

            //this.Loaded += PageLoaded;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(signInButton);
            IInvokeProvider invokeProv = (IInvokeProvider)peer.GetPattern(PatternInterface.Invoke);
            invokeProv.Invoke();
        }

        private void OnBtnClickSignIn(object sender, RoutedEventArgs e)
        {
            int employeeId = 0;

            if (usernameTextBox.Text != "")
            {
                if (!commonQueries.CheckEmployeeUserName(usernameTextBox.Text, ref employeeId))
                    return;

                if (employeeId == 0)
                {
                    MessageBox.Show("Incorrect username!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please enter username!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            String password = string.Empty;

            if (passwordTextBox.Password != "")
            {
                if (!commonQueries.CheckEmployeePassword(employeeId, ref password))
                    return;


                var crypt = new SHA256Managed();
                string hashedPassword = String.Empty;
                byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(passwordTextBox.Password));
                foreach (byte theByte in crypto)
                {
                    hashedPassword += theByte.ToString("x2");
                }

                if (hashedPassword == password)
                {
                    Employee loggedInUser = new Employee();

                    if (!loggedInUser.InitializeEmployeeInfo(employeeId))
                    {
                        MessageBox.Show("Username and password are correct but server connection failed! Please check your internet connection and try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    MainWindow mainWindow = new MainWindow(ref loggedInUser);
                    mainWindow.Show();

                    NavigationWindow parent = (NavigationWindow)this.Parent;
                    parent.Close();
                }
                else
                {
                    MessageBox.Show("Incorrect Password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter password!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }

        private void OnBtnClickSignUp(object sender, RoutedEventArgs e)
        {
            SignUpPage signUpPage = new SignUpPage();
            NavigationService.Navigate(signUpPage);
        }
    }
}
