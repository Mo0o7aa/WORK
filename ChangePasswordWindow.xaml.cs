using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        CommonQueries commonQueries;
        Employee loggedInUser;
        SQLServer sqlDatabase;
        public ChangePasswordWindow(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;
            sqlDatabase = new SQLServer();
            commonQueries = new CommonQueries();

            InitializeComponent();
        }

        private void OnClickSaveChangesButton(object sender, RoutedEventArgs e)
        {
            if (oldPasswordBox.Password == "")
            {
                MessageBox.Show("Please enter your old password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (newPasswordTextBox.Password == "")
            {
                MessageBox.Show("Please enter your new password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (confirmPasswordTextBox.Password == "")
            {
                MessageBox.Show("Confirm password is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            String password = string.Empty;

            if (!commonQueries.CheckEmployeePassword(loggedInUser.GetEmployeeId(), ref password))
                return;

            String oldPassword = oldPasswordBox.Password;
            var oldCrypt = new SHA256Managed();
            string hashedOldPassword = String.Empty;
            byte[] oldCrypto = oldCrypt.ComputeHash(Encoding.ASCII.GetBytes(oldPassword));
            foreach (byte theByte in oldCrypto)
            {
                hashedOldPassword += theByte.ToString("x2");
            }

            if (password != hashedOldPassword)
            {
                MessageBox.Show("Old password is incorrect!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if(oldPasswordBox.Password == newPasswordTextBox.Password)
            {
                MessageBox.Show("New password can't be same as old password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(confirmPasswordTextBox.Password != newPasswordTextBox.Password)
            {
                MessageBox.Show("Both passwords do not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            String newPassword = newPasswordTextBox.Password;

            var crypt = new SHA256Managed();
            string hashedPassword = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(newPassword));
            foreach (byte theByte in crypto)
            {
                hashedPassword += theByte.ToString("x2");
            }

            String sqlQuery = string.Empty;

            String sqlQueryPart1 = "update NTRA.dbo.employee_password set password = ";
            String sqlQueryPart2 = " where employee_id = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += "'" + hashedPassword + "'";
            sqlQuery += sqlQueryPart2;
            sqlQuery += loggedInUser.GetEmployeeId();

            if (!sqlDatabase.InsertRows(sqlQuery))
                return;

            this.Close();
        }

        private void MouseEnterNewPasswordIcon(object sender, MouseEventArgs e)
        {
            //BrushConverter brushConverter = new BrushConverter();
            //
            //Label currentLabel = (Label)sender;
            //currentLabel.Background = (Brush)brushConverter.ConvertFrom("#000080");
            //currentLabel.Foreground = Brushes.White;

            newPasswordHiddenBox.Text = newPasswordTextBox.Password;
            newPasswordTextBox.Visibility = Visibility.Collapsed;
            newPasswordHiddenBox.Visibility = Visibility.Visible;
        }

        private void MouseLeaveNewPasswordIcon(object sender, MouseEventArgs e)
        {
            //BrushConverter brushConverter = new BrushConverter();
            //
            //Label currentLabel = (Label)sender;
            //currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#000080");
            //currentLabel.Background = Brushes.White;

            newPasswordTextBox.Visibility = Visibility.Visible;
            newPasswordHiddenBox.Visibility = Visibility.Collapsed;

        }

        private void MouseEnterConfirmPasswordIcon(object sender, MouseEventArgs e)
        {
            //BrushConverter brushConverter = new BrushConverter();
            //
            //Label currentLabel = (Label)sender;
            //currentLabel.Background = (Brush)brushConverter.ConvertFrom("#000080");
            //currentLabel.Foreground = Brushes.White;

            confirmPasswordHiddenBox.Text = confirmPasswordTextBox.Password;
            confirmPasswordTextBox.Visibility = Visibility.Collapsed;
            confirmPasswordHiddenBox.Visibility = Visibility.Visible;
        }

        private void MouseLeaveConfirmPasswordIcon(object sender, MouseEventArgs e)
        {
            //BrushConverter brushConverter = new BrushConverter();
            //
            //Label currentLabel = (Label)sender;
            //currentLabel.Foreground = (Brush)brushConverter.ConvertFrom("#000080");
            //currentLabel.Background = Brushes.White;

            confirmPasswordTextBox.Visibility = Visibility.Visible;
            confirmPasswordHiddenBox.Visibility = Visibility.Collapsed;
        }

        private void MouseEnterOldPasswordIcon(object sender, MouseEventArgs e)
        {
            oldPasswordHiddenTextBox.Text = oldPasswordBox.Password;
            oldPasswordBox.Visibility = Visibility.Collapsed;
            oldPasswordHiddenTextBox.Visibility = Visibility.Visible;
        }

        private void MouseLeaveOldPasswordIcon(object sender, MouseEventArgs e)
        {
            oldPasswordBox.Visibility = Visibility.Visible;
            oldPasswordHiddenTextBox.Visibility = Visibility.Collapsed;
        }
    }
}
