using LiveCharts;
using LiveCharts.Wpf;
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
using Separator = LiveCharts.Wpf.Separator;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class DashBoard : Page
    {
        private Employee loggedInUser;
        private CommonQueries commonQueries;
        private CommonFunctions commonFunctions;
        private List<BASIC_STRUCTS.COMPLAINT_STRUCT> complaints;
        private List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> missions;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> vehichles;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> labEquipment;
        private List<BASIC_STRUCTS.REPEATER_STRUCT> repeaters;


        public DashBoard(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            loggedInUser = mLoggedInUser;

            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();

            complaints = new List<BASIC_STRUCTS.COMPLAINT_STRUCT>();
            missions = new List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT>();
            vehichles = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            labEquipment = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            repeaters = new List<BASIC_STRUCTS.REPEATER_STRUCT>();

            userNameLabel.Content = "Username: " + loggedInUser.GetEmployeeUserName();

            startDatePicker.SelectedDate = new DateTime(DateTime.Now.Year, 1, 1);
            endDatePicker.SelectedDate = DateTime.Today;

            FillCategoryComboBox();

        }

        private void FillCategoryComboBox()
        {
            categoryCombo.Items.Clear();
            categoryCombo.Items.Add("Complaints");
            categoryCombo.Items.Add("Missions");
            categoryCombo.Items.Add("Vehicles");
            categoryCombo.Items.Add("Lab Equipment");
            categoryCombo.Items.Add("Repeaters");

            categoryCombo.SelectedIndex = 0;
        }

        

        //////DatePickers LOGIC////////////////////////////////

        private void OnSelChangedStartDatePicker(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedEndDatePicker(object sender, SelectionChangedEventArgs e)
        {

        }

        //////CATEGORY COMBOBOX LOGIC/////////////////////////////////
        private void onselChangedCategoryCombo(object sender, RoutedEventArgs e)
        {
            if (categoryCombo.SelectedItem == "Complaints")
            {


                if (complaints.Count == 0)
                {
                    if (!commonQueries.GetComplaints(ref complaints))
                        return;
                }

                /////////Number of Sites/////////////////////////////////////////////

                int vodafoneComplaintSites = 0;
                for (int i = 0; i < complaints.Count; i++)
                {
                    if (complaints[i].company_name == "Vodafone")
                    {
                        vodafoneComplaintSites += complaints[i].sites.Count;
                    }
                }

                int etisalatMasrComplaintSites = 0;
                for (int i = 0; i < complaints.Count; i++)
                {
                    if (complaints[i].company_name == "Etisalat Masr")
                    {
                        etisalatMasrComplaintSites += complaints[i].sites.Count;
                    }
                }

                int orangeComplaintSites = 0;
                for (int i = 0; i < complaints.Count; i++)
                {
                    if (complaints[i].company_name == "Orange")
                    {
                        orangeComplaintSites += complaints[i].sites.Count;
                    }
                }

                int weComplaintSites = 0;


                /////PIE CHART////////////////////////////////////////

                Label vodafoneGridLabel = new Label();
                vodafoneGridLabel.Style = (Style)FindResource("labelStyleBlack");
                vodafoneGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                vodafoneGridLabel.Content = vodafoneComplaintSites;

                numberOfSitesGrid.Children.Add(vodafoneGridLabel);
                Grid.SetColumn(vodafoneGridLabel, 1);
                Grid.SetRow(vodafoneGridLabel, 1);

                Label etisalatGridLabel = new Label();
                etisalatGridLabel.Style = (Style)FindResource("labelStyleBlack");
                etisalatGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                etisalatGridLabel.Content = etisalatMasrComplaintSites;

                numberOfSitesGrid.Children.Add(etisalatGridLabel);
                Grid.SetColumn(etisalatGridLabel, 1);
                Grid.SetRow(etisalatGridLabel, 2);

                Label orangeGridLabel = new Label();
                orangeGridLabel.Style = (Style)FindResource("labelStyleBlack");
                orangeGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                orangeGridLabel.Content = orangeComplaintSites;

                numberOfSitesGrid.Children.Add(orangeGridLabel);
                Grid.SetColumn(orangeGridLabel, 1);
                Grid.SetRow(orangeGridLabel, 3);

                Label weGridLabel = new Label();
                weGridLabel.Style = (Style)FindResource("labelStyleBlack");
                weGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                weGridLabel.Content = weComplaintSites;

                numberOfSitesGrid.Children.Add(weGridLabel);
                Grid.SetColumn(weGridLabel, 1);
                Grid.SetRow(weGridLabel, 4);


                numberOfSitesPieChart.Series = new SeriesCollection()
            {
                new PieSeries
                {
                    Title = "Vodafone",
                    Values = new ChartValues<Double> { vodafoneComplaintSites },
                    Fill = Brushes.Red
                },
                new PieSeries
                {
                    Title = "Etisalat",
                    Values = new ChartValues<Double> { etisalatMasrComplaintSites },
                    Fill = Brushes.Green
                },
                new PieSeries
                {
                    Title = "Orange",
                    Values = new ChartValues<Double> { orangeComplaintSites },
                    Fill = Brushes.DarkOrange
                },

                new PieSeries
                {
                    Title = "WE",
                    Values = new ChartValues<Double> { weComplaintSites },
                    Fill = Brushes.Navy
                },

            };

                /////////Number of Complaints/////////////////////////////////////////////

                String[] companies = { "Vodafone", "Etisalat", "Orange", "WE" };

                int vodafoneComplaints = 0;
                int etisalatMasrComplaints = 0;
                int orangeComplaints = 0;
                int weComplaints = 0;


                for (int i = 0; i < complaints.Count; i++)
                {
                    if (complaints[i].company_name == "Vodafone")
                    {
                        vodafoneComplaints += 1;
                    }
                    if (complaints[i].company_name == "Etisalat Masr")
                    {
                        etisalatMasrComplaints += 1;
                    }
                    if (complaints[i].company_name == "Orange")
                    {
                        orangeComplaints += 1;
                    }
                    if (complaints[i].company_name == "We")
                    {
                        orangeComplaints += 1;
                    }
                }



                /////PIE CHART////////////////////////////////////////

                Label vodafoneComplaintsGridLabel = new Label();
                vodafoneComplaintsGridLabel.Style = (Style)FindResource("labelStyleBlack");
                vodafoneComplaintsGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                vodafoneComplaintsGridLabel.Content = vodafoneComplaints;

                numberOfComplaintsGrid.Children.Add(vodafoneComplaintsGridLabel);
                Grid.SetColumn(vodafoneComplaintsGridLabel, 1);
                Grid.SetRow(vodafoneComplaintsGridLabel, 1);

                Label etisalatComplaintGridLabel = new Label();
                etisalatComplaintGridLabel.Style = (Style)FindResource("labelStyleBlack");
                etisalatComplaintGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                etisalatComplaintGridLabel.Content = etisalatMasrComplaints;

                numberOfComplaintsGrid.Children.Add(etisalatComplaintGridLabel);
                Grid.SetColumn(etisalatComplaintGridLabel, 1);
                Grid.SetRow(etisalatComplaintGridLabel, 2);

                Label orangeComplaintGridLabel = new Label();
                orangeComplaintGridLabel.Style = (Style)FindResource("labelStyleBlack");
                orangeComplaintGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                orangeComplaintGridLabel.Content = orangeComplaints;

                numberOfComplaintsGrid.Children.Add(orangeComplaintGridLabel);
                Grid.SetColumn(orangeComplaintGridLabel, 1);
                Grid.SetRow(orangeComplaintGridLabel, 3);

                Label weComplaintGridLabel = new Label();
                weComplaintGridLabel.Style = (Style)FindResource("labelStyleBlack");
                weComplaintGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                weComplaintGridLabel.Content = weComplaints;

                numberOfComplaintsGrid.Children.Add(weComplaintGridLabel);
                Grid.SetColumn(weComplaintGridLabel, 1);
                Grid.SetRow(weComplaintGridLabel, 4);


                numberOfComplaintsPieChart.Series = new SeriesCollection()
            {
                new PieSeries
                {
                    Title = "Vodafone",
                    Values = new ChartValues<Double> { vodafoneComplaints },
                    Fill = Brushes.Red
                },
                new PieSeries
                {
                    Title = "Etisalat",
                    Values = new ChartValues<Double> { etisalatMasrComplaints },
                    Fill = Brushes.Green
                },
                new PieSeries
                {
                    Title = "Orange",
                    Values = new ChartValues<Double> { orangeComplaints },
                    Fill = Brushes.DarkOrange
                },

                new PieSeries
                {
                    Title = "WE",
                    Values = new ChartValues<Double> { weComplaints },
                    Fill = Brushes.Navy
                },

            };

                /////////Complaints Status/////////////////////////////////////////////

                int vodafoneOpenComplaints = 0;
                int vodafonePendingComplaints = 0;
                int vodafoneClosedComplaints = 0;

                int etisalatOpenComplaints = 0;
                int etisalatPendingComplaints = 0;
                int etisalatClosedComplaints = 0;

                int orangetOpenComplaints = 0;
                int orangePendingComplaints = 0;
                int orangeClosedComplaints = 0;

                int weOpenComplaints = 0;
                int wePendingComplaints = 0;
                int weClosedComplaints = 0;

                for (int i = 0; i < complaints.Count; i++)
                {
                    if (complaints[i].company_name == "Vodafone")
                    {
                        if (complaints[i].complaint_status == "Open")
                        {
                            vodafoneOpenComplaints++;
                        }
                        else if (complaints[i].complaint_status == "Pending")
                        {
                            vodafonePendingComplaints++;
                        }
                        else
                        {
                            vodafoneClosedComplaints++;
                        }
                    }
                    if (complaints[i].company_name == "Etisalat Masr")
                    {
                        if (complaints[i].complaint_status == "Open")
                        {
                            etisalatOpenComplaints++;
                        }
                        else if (complaints[i].complaint_status == "Pending")
                        {
                            etisalatPendingComplaints++;
                        }
                        else
                        {
                            etisalatClosedComplaints++;
                        }
                    }
                    if (complaints[i].company_name == "Orange")
                    {
                        if (complaints[i].complaint_status == "Open")
                        {
                            orangetOpenComplaints++;
                        }
                        else if (complaints[i].complaint_status == "Pending")
                        {
                            orangePendingComplaints++;
                        }
                        else
                        {
                            orangeClosedComplaints++;
                        }
                    }
                    if (complaints[i].company_name == "We")
                    {
                        if (complaints[i].complaint_status == "Open")
                        {
                            weOpenComplaints++;
                        }
                        else if (complaints[i].complaint_status == "Pending")
                        {
                            wePendingComplaints++;
                        }
                        else
                        {
                            weClosedComplaints++;
                        }
                    }
                }

                /////////PIE CHARTS/////////////////////////////////////////////

                /////////VODAFONE///////////////////////////////////////////////

                Label vodafoneOpenComplaintsGridLabel = new Label();
                vodafoneOpenComplaintsGridLabel.Style = (Style)FindResource("labelStyleBlack");
                vodafoneOpenComplaintsGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                vodafoneOpenComplaintsGridLabel.Content = vodafoneOpenComplaints;

                vodafoneComplaintsGrid.Children.Add(vodafoneOpenComplaintsGridLabel);
                Grid.SetColumn(vodafoneOpenComplaintsGridLabel, 1);
                Grid.SetRow(vodafoneOpenComplaintsGridLabel, 1);

                Label vodafonePendingComplaintsGridLabel = new Label();
                vodafonePendingComplaintsGridLabel.Style = (Style)FindResource("labelStyleBlack");
                vodafonePendingComplaintsGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                vodafonePendingComplaintsGridLabel.Content = vodafonePendingComplaints;

                vodafoneComplaintsGrid.Children.Add(vodafonePendingComplaintsGridLabel);
                Grid.SetColumn(vodafonePendingComplaintsGridLabel, 1);
                Grid.SetRow(vodafonePendingComplaintsGridLabel, 2);

                Label vodafoneClosedComplaintsGridLabel = new Label();
                vodafoneClosedComplaintsGridLabel.Style = (Style)FindResource("labelStyleBlack");
                vodafoneClosedComplaintsGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                vodafoneClosedComplaintsGridLabel.Content = vodafoneClosedComplaints;

                vodafoneComplaintsGrid.Children.Add(vodafoneClosedComplaintsGridLabel);
                Grid.SetColumn(vodafoneClosedComplaintsGridLabel, 1);
                Grid.SetRow(vodafoneClosedComplaintsGridLabel, 3);


                vodafoneComplaintsPieChart.Series = new SeriesCollection()
            {
                new PieSeries
                {
                    Title = "Open",
                    Values = new ChartValues<Double> { vodafoneOpenComplaints },
                    Fill = Brushes.Red
                },
                new PieSeries
                {
                    Title = "Pending",
                    Values = new ChartValues<Double> { vodafonePendingComplaints },
                    Fill = Brushes.Yellow
                },
                new PieSeries
                {
                    Title = "Closed",
                    Values = new ChartValues<Double> { vodafoneClosedComplaints },
                    Fill = Brushes.Green
                },

            };

                /////////ETISALAT///////////////////////////////////////////////

                Label etisalatOpenComplaintsGridLabel = new Label();
                etisalatOpenComplaintsGridLabel.Style = (Style)FindResource("labelStyleBlack");
                etisalatOpenComplaintsGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                etisalatOpenComplaintsGridLabel.Content = etisalatOpenComplaints;

                etisalatComplaintsGrid.Children.Add(etisalatOpenComplaintsGridLabel);
                Grid.SetColumn(etisalatOpenComplaintsGridLabel, 1);
                Grid.SetRow(etisalatOpenComplaintsGridLabel, 1);

                Label etisalatPendingComplaintsGridLabel = new Label();
                etisalatPendingComplaintsGridLabel.Style = (Style)FindResource("labelStyleBlack");
                etisalatPendingComplaintsGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                etisalatPendingComplaintsGridLabel.Content = etisalatPendingComplaints;

                etisalatComplaintsGrid.Children.Add(etisalatPendingComplaintsGridLabel);
                Grid.SetColumn(etisalatPendingComplaintsGridLabel, 1);
                Grid.SetRow(etisalatPendingComplaintsGridLabel, 2);

                Label etisalatClosedComplaintsGridLabel = new Label();
                etisalatClosedComplaintsGridLabel.Style = (Style)FindResource("labelStyleBlack");
                etisalatClosedComplaintsGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                etisalatClosedComplaintsGridLabel.Content = etisalatClosedComplaints;

                etisalatComplaintsGrid.Children.Add(etisalatClosedComplaintsGridLabel);
                Grid.SetColumn(etisalatClosedComplaintsGridLabel, 1);
                Grid.SetRow(etisalatClosedComplaintsGridLabel, 3);


                etislataComplaintsPieChart.Series = new SeriesCollection()
            {
                new PieSeries
                {
                    Title = "Open",
                    Values = new ChartValues<Double> { etisalatOpenComplaints },
                    Fill = Brushes.Red
                },
                new PieSeries
                {
                    Title = "Pending",
                    Values = new ChartValues<Double> { etisalatPendingComplaints },
                    Fill = Brushes.Yellow
                },
                new PieSeries
                {
                    Title = "Closed",
                    Values = new ChartValues<Double> { etisalatClosedComplaints },
                    Fill = Brushes.Green
                },

            };

                /////////ORANGE///////////////////////////////////////////////

                Label orangeOpenComplaintsGridLabel = new Label();
                orangeOpenComplaintsGridLabel.Style = (Style)FindResource("labelStyleBlack");
                orangeOpenComplaintsGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                orangeOpenComplaintsGridLabel.Content = orangetOpenComplaints;

                orangeComplaintsGrid.Children.Add(orangeOpenComplaintsGridLabel);
                Grid.SetColumn(orangeOpenComplaintsGridLabel, 1);
                Grid.SetRow(orangeOpenComplaintsGridLabel, 1);

                Label orangePendingComplaintsGridLabel = new Label();
                orangePendingComplaintsGridLabel.Style = (Style)FindResource("labelStyleBlack");
                orangePendingComplaintsGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                orangePendingComplaintsGridLabel.Content = orangePendingComplaints;

                orangeComplaintsGrid.Children.Add(orangePendingComplaintsGridLabel);
                Grid.SetColumn(orangePendingComplaintsGridLabel, 1);
                Grid.SetRow(orangePendingComplaintsGridLabel, 2);

                Label orangeClosedComplaintsGridLabel = new Label();
                orangeClosedComplaintsGridLabel.Style = (Style)FindResource("labelStyleBlack");
                orangeClosedComplaintsGridLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                orangeClosedComplaintsGridLabel.Content = orangeClosedComplaints;

                orangeComplaintsGrid.Children.Add(orangeClosedComplaintsGridLabel);
                Grid.SetColumn(orangeClosedComplaintsGridLabel, 1);
                Grid.SetRow(orangeClosedComplaintsGridLabel, 3);


                orangeComplaintsPieChart.Series = new SeriesCollection()
            {
                new PieSeries
                {
                    Title = "Open",
                    Values = new ChartValues<Double> { orangetOpenComplaints },
                    Fill = Brushes.Red
                },
                new PieSeries
                {
                    Title = "Pending",
                    Values = new ChartValues<Double> { orangePendingComplaints },
                    Fill = Brushes.Yellow
                },
                new PieSeries
                {
                    Title = "Closed",
                    Values = new ChartValues<Double> { orangeClosedComplaints },
                    Fill = Brushes.Green
                },

            };
            }

            else if (categoryCombo.SelectedItem == "Missions")
            {

                if (missions.Count == 0)
                {
                    if (!commonQueries.GetMissions(ref missions))
                        return;
                }

            }

            else if (categoryCombo.SelectedItem == "Vehicles")
            {


                if (vehichles.Count == 0)
                {
                    if (!commonQueries.GetCompanyVehicles(ref vehichles))
                        return;
                }
            }

            else if (categoryCombo.SelectedItem == "Lab Equipment")
            {

                if (labEquipment.Count == 0)
                {
                    if (!commonQueries.GetHandhelds(ref labEquipment))
                        return;
                }
            }

            else if (categoryCombo.SelectedItem == "Repeaters")
            {
                if (repeaters.Count == 0)
                {
                    if (!commonQueries.GetRepeaters(ref repeaters))
                        return;
                }
            }
        }



        //////////COMMON LOGIC///////////////////////////////////////////////////////
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

        private void OnClosedMenuContectMenu(object sender, RoutedEventArgs e)
        {
            mainMenuGrid.Focusable = false;
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

        private void MonitoringMenuSelection(object sender, RoutedEventArgs e)
        {
            MissionsPage missionsPage = new MissionsPage(ref loggedInUser);
            NavigationService.Navigate(missionsPage);
        }
        private void RepeatersMenueSelection(object sender, RoutedEventArgs e)
        {
            RepeatersPage repeatersPage = new RepeatersPage(ref loggedInUser);
            NavigationService.Navigate(repeatersPage);
        }
        private void InspectionMenuSelection(object sender, RoutedEventArgs e)
        {

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

        
    }
}
