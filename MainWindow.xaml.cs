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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        Employee loggedInUser;
        public MainWindow(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;
            InitializeComponent();

            EmployeesPage employeesPage = new EmployeesPage(ref loggedInUser);
            NavigationService.Navigate(employeesPage);

            //MissionsPage missionsPage = new MissionsPage(ref loggedInUser);
            //NavigationService.Navigate(missionsPage);
        }
    }
}
