using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            pageHeader.Attach(loggedInUser);

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
            string category = categoryCombo.SelectedItem as string;

            if (category == "Complaints")
            {
                if (complaints.Count == 0)
                {
                    if (!commonQueries.GetComplaints(ref complaints))
                        return;
                }

                UpdateComplaintCharts();
            }
            else if (category == "Missions")
            {
                if (missions.Count == 0)
                {
                    if (!commonQueries.GetMissions(ref missions))
                        return;
                }
            }
            else if (category == "Vehicles")
            {
                if (vehichles.Count == 0)
                {
                    if (!commonQueries.GetCompanyVehicles(ref vehichles))
                        return;
                }
            }
            else if (category == "Lab Equipment")
            {
                if (labEquipment.Count == 0)
                {
                    if (!commonQueries.GetHandhelds(ref labEquipment))
                        return;
                }
            }
            else if (category == "Repeaters")
            {
                if (repeaters.Count == 0)
                {
                    if (!commonQueries.GetRepeaters(ref repeaters))
                        return;
                }
            }
        }

        private void UpdateComplaintCharts()
        {
            /////////Number of Sites/////////////////////////////////////////////

            int vodafoneComplaintSites = 0;
            int etisalatMasrComplaintSites = 0;
            int orangeComplaintSites = 0;
            int weComplaintSites = 0;

            /////////Number of Complaints////////////////////////////////////////

            int vodafoneComplaints = 0;
            int etisalatMasrComplaints = 0;
            int orangeComplaints = 0;
            int weComplaints = 0;

            /////////Complaints Status///////////////////////////////////////////

            int vodafoneOpenComplaints = 0, vodafonePendingComplaints = 0, vodafoneClosedComplaints = 0;
            int etisalatOpenComplaints = 0, etisalatPendingComplaints = 0, etisalatClosedComplaints = 0;
            int orangeOpenComplaints = 0, orangePendingComplaints = 0, orangeClosedComplaints = 0;

            for (int i = 0; i < complaints.Count; i++)
            {
                string company = complaints[i].company_name;
                string status = complaints[i].complaint_status;
                int sitesCount = complaints[i].sites != null ? complaints[i].sites.Count : 0;

                if (company == "Vodafone")
                {
                    vodafoneComplaintSites += sitesCount;
                    vodafoneComplaints += 1;

                    if (status == "Open")
                        vodafoneOpenComplaints++;
                    else if (status == "Pending")
                        vodafonePendingComplaints++;
                    else
                        vodafoneClosedComplaints++;
                }
                else if (company == "Etisalat Masr")
                {
                    etisalatMasrComplaintSites += sitesCount;
                    etisalatMasrComplaints += 1;

                    if (status == "Open")
                        etisalatOpenComplaints++;
                    else if (status == "Pending")
                        etisalatPendingComplaints++;
                    else
                        etisalatClosedComplaints++;
                }
                else if (company == "Orange")
                {
                    orangeComplaintSites += sitesCount;
                    orangeComplaints += 1;

                    if (status == "Open")
                        orangeOpenComplaints++;
                    else if (status == "Pending")
                        orangePendingComplaints++;
                    else
                        orangeClosedComplaints++;
                }
                else if (company == "We")
                {
                    weComplaints += 1;
                }
            }

            vodafoneSitesText.Text = vodafoneComplaintSites.ToString();
            etisalatSitesText.Text = etisalatMasrComplaintSites.ToString();
            orangeSitesText.Text = orangeComplaintSites.ToString();
            weSitesText.Text = weComplaintSites.ToString();

            numberOfSitesPieChart.Series = BuildCompanyPie(vodafoneComplaintSites, etisalatMasrComplaintSites, orangeComplaintSites, weComplaintSites);

            vodafoneComplaintsText.Text = vodafoneComplaints.ToString();
            etisalatComplaintsText.Text = etisalatMasrComplaints.ToString();
            orangeComplaintsText.Text = orangeComplaints.ToString();
            weComplaintsText.Text = weComplaints.ToString();

            numberOfComplaintsPieChart.Series = BuildCompanyPie(vodafoneComplaints, etisalatMasrComplaints, orangeComplaints, weComplaints);

            vodafoneOpenText.Text = vodafoneOpenComplaints.ToString();
            vodafonePendingText.Text = vodafonePendingComplaints.ToString();
            vodafoneClosedText.Text = vodafoneClosedComplaints.ToString();

            vodafoneComplaintsPieChart.Series = BuildStatusPie(vodafoneOpenComplaints, vodafonePendingComplaints, vodafoneClosedComplaints);

            etisalatOpenText.Text = etisalatOpenComplaints.ToString();
            etisalatPendingText.Text = etisalatPendingComplaints.ToString();
            etisalatClosedText.Text = etisalatClosedComplaints.ToString();

            etislataComplaintsPieChart.Series = BuildStatusPie(etisalatOpenComplaints, etisalatPendingComplaints, etisalatClosedComplaints);

            orangeOpenText.Text = orangeOpenComplaints.ToString();
            orangePendingText.Text = orangePendingComplaints.ToString();
            orangeClosedText.Text = orangeClosedComplaints.ToString();

            orangeComplaintsPieChart.Series = BuildStatusPie(orangeOpenComplaints, orangePendingComplaints, orangeClosedComplaints);
        }

        private static SeriesCollection BuildCompanyPie(double vodafone, double etisalat, double orange, double we)
        {
            return new SeriesCollection
            {
                new PieSeries { Title = "Vodafone", Values = new ChartValues<Double> { vodafone }, Fill = Brushes.Red },
                new PieSeries { Title = "Etisalat", Values = new ChartValues<Double> { etisalat }, Fill = Brushes.Green },
                new PieSeries { Title = "Orange", Values = new ChartValues<Double> { orange }, Fill = Brushes.DarkOrange },
                new PieSeries { Title = "WE", Values = new ChartValues<Double> { we }, Fill = Brushes.Navy },
            };
        }

        private static SeriesCollection BuildStatusPie(double open, double pending, double closed)
        {
            return new SeriesCollection
            {
                new PieSeries { Title = "Open", Values = new ChartValues<Double> { open }, Fill = Brushes.Red },
                new PieSeries { Title = "Pending", Values = new ChartValues<Double> { pending }, Fill = Brushes.Gold },
                new PieSeries { Title = "Closed", Values = new ChartValues<Double> { closed }, Fill = Brushes.Green },
            };
        }
    }
}
