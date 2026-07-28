using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ntra_missions
{
    /// <summary>
    /// Shared top bar for all pages: brand bar, user chrome and navigation.
    /// Pages call Attach(user) after InitializeComponent.
    /// </summary>
    public partial class PageHeader : UserControl
    {
        private Employee loggedInUser;

        private static readonly string[,] navItems =
        {
            { "Dashboard",        "DashBoard" },
            { "Employees",        "Employees" },
            { "Companies",        "Companies" },
            { "Company Equipment","CompanyEquipment" },
            { "Sites",            "Sites" },
            { "EMF",              "EMF" },
            { "Complaints",       "Complaints" },
            { "Monitoring Missions", "Missions" },
            { "Repeaters",        "Repeaters" },
            { "Inspection Missions", "Inspection" },
        };

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(PageHeader),
                new PropertyMetadata("", OnTitleChanged));

        public static readonly DependencyProperty CurrentTagProperty =
            DependencyProperty.Register("CurrentTag", typeof(string), typeof(PageHeader),
                new PropertyMetadata("", OnCurrentTagChanged));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string CurrentTag
        {
            get { return (string)GetValue(CurrentTagProperty); }
            set { SetValue(CurrentTagProperty, value); }
        }

        public PageHeader()
        {
            InitializeComponent();
            BuildNav();
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PageHeader)d).pageTitleText.Text = (string)e.NewValue;
        }

        private static void OnCurrentTagChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PageHeader)d).BuildNav();
        }

        public void Attach(Employee user)
        {
            loggedInUser = user;
            userNameText.Text = user != null ? user.GetEmployeeUserName() : "";
        }

        private void BuildNav()
        {
            navPanel.Children.Clear();

            for (int i = 0; i < navItems.GetLength(0); i++)
            {
                string label = navItems[i, 0];
                string tag = navItems[i, 1];

                // EMF is intentionally hidden from the navigation bar.
                if (tag == "EMF")
                    continue;

                Button button = new Button();
                button.Content = label;
                button.Tag = tag;
                button.Style = (Style)FindResource(tag == CurrentTag ? "NavPillActive" : "NavPill");

                if (tag == CurrentTag)
                    button.IsHitTestVisible = false;
                else
                    button.Click += OnClickNavItem;

                navPanel.Children.Add(button);
            }
        }

        private void OnClickNavItem(object sender, RoutedEventArgs e)
        {
            if (loggedInUser == null)
                return;

            NavigationService navigationService = NavigationService.GetNavigationService(this);
            if (navigationService == null)
                return;

            switch ((string)((Button)sender).Tag)
            {
                case "DashBoard":
                    navigationService.Navigate(new DashBoard(ref loggedInUser));
                    break;
                case "Employees":
                    navigationService.Navigate(new EmployeesPage(ref loggedInUser));
                    break;
                case "Companies":
                    navigationService.Navigate(new CompaniesPage(ref loggedInUser));
                    break;
                case "CompanyEquipment":
                    navigationService.Navigate(new CompanyEquipmentPage(ref loggedInUser));
                    break;
                case "Sites":
                    navigationService.Navigate(new SitesPage(ref loggedInUser));
                    break;
                case "EMF":
                    navigationService.Navigate(new EMFPage(ref loggedInUser));
                    break;
                case "Complaints":
                    navigationService.Navigate(new ComplaintsPage(ref loggedInUser));
                    break;
                case "Missions":
                    navigationService.Navigate(new MissionsPage(ref loggedInUser));
                    break;
                case "Repeaters":
                    navigationService.Navigate(new RepeatersPage(ref loggedInUser));
                    break;
                case "Inspection":
                    // not implemented yet (same as the old menu item)
                    break;
            }
        }

        private void OnClickChangePassword(object sender, RoutedEventArgs e)
        {
            if (loggedInUser == null)
                return;

            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow(ref loggedInUser);
            changePasswordWindow.Show();
        }

        private void OnClickSignOut(object sender, RoutedEventArgs e)
        {
            SignInWindow signInWindow = new SignInWindow();
            signInWindow.Show();

            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
                parentWindow.Close();
        }
    }
}
