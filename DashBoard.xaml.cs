using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Separator = LiveCharts.Wpf.Separator;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class DashBoard : Page
    {
        private Employee loggedInUser;
        private readonly CommonQueries commonQueries;

        private List<BASIC_STRUCTS.COMPLAINT_STRUCT> complaints;
        private List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> missions;
        private List<BASIC_STRUCTS.REPEATER_STRUCT> repeaters;
        private List<BASIC_STRUCTS.MIN_SITE_STRUCT> sites;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> companies; // key = serial, value = name
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> engineers;

        private bool dataLoaded;

        private readonly FontFamily appFont = new FontFamily("Segoe UI");
        private Brush brand;
        private Brush muted;
        private readonly Brush openBrush = new SolidColorBrush(Color.FromRgb(0xDC, 0x26, 0x26));
        private readonly Brush pendingBrush = new SolidColorBrush(Color.FromRgb(0xD9, 0x77, 0x06));
        private readonly Brush closedBrush = new SolidColorBrush(Color.FromRgb(0x16, 0xA3, 0x4A));

        public DashBoard(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            loggedInUser = mLoggedInUser;
            commonQueries = new CommonQueries();

            complaints = new List<BASIC_STRUCTS.COMPLAINT_STRUCT>();
            missions = new List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT>();
            repeaters = new List<BASIC_STRUCTS.REPEATER_STRUCT>();
            sites = new List<BASIC_STRUCTS.MIN_SITE_STRUCT>();
            companies = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            engineers = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            brand = (Brush)FindResource("BrandBrush");
            muted = (Brush)FindResource("MutedTextBrush");

            pageHeader.Attach(loggedInUser);

            categoryCombo.Items.Add("Complaints");
            categoryCombo.Items.Add("Missions");
            categoryCombo.Items.Add("Engineers");
            categoryCombo.Items.Add("Companies");
            categoryCombo.Items.Add("Sites");
            categoryCombo.Items.Add("Repeaters");

            startDatePicker.SelectedDate = new DateTime(DateTime.Now.Year, 1, 1);
            endDatePicker.SelectedDate = DateTime.Today;

            LoadAllData();

            categoryCombo.SelectedIndex = 0; // triggers OnFilterChanged -> Render
        }

        private void LoadAllData()
        {
            commonQueries.GetComplaints(ref complaints);
            commonQueries.GetMissions(ref missions);
            commonQueries.GetRepeaters(ref repeaters);
            commonQueries.GetAllSites(ref sites);
            commonQueries.GetCompanies(ref companies);
            commonQueries.GetEngineers(ref engineers);
            dataLoaded = true;
        }

        private void OnFilterChanged(object sender, RoutedEventArgs e)
        {
            Render();
        }

        private DateTime RangeStart()
        {
            return startDatePicker.SelectedDate.HasValue
                ? startDatePicker.SelectedDate.Value.Date
                : DateTime.MinValue;
        }

        private DateTime RangeEnd()
        {
            return endDatePicker.SelectedDate.HasValue
                ? endDatePicker.SelectedDate.Value.Date.AddDays(1).AddTicks(-1)
                : DateTime.MaxValue;
        }

        private void Render()
        {
            if (!dataLoaded || kpiPanel == null)
                return;

            kpiPanel.Children.Clear();
            chartsPanel.Children.Clear();

            string category = categoryCombo.SelectedItem as string;

            switch (category)
            {
                case "Complaints": RenderComplaints(); break;
                case "Missions": RenderMissions(); break;
                case "Engineers": RenderEngineers(); break;
                case "Companies": RenderCompanies(); break;
                case "Sites": RenderSites(); break;
                case "Repeaters": RenderRepeaters(); break;
            }
        }

        // ================= category renderers =================

        private void RenderComplaints()
        {
            filterHintText.Text = "Date range filters complaints by their date.";

            DateTime start = RangeStart(), end = RangeEnd();
            List<BASIC_STRUCTS.COMPLAINT_STRUCT> filtered = complaints
                .Where(c => c.complaint_date >= start && c.complaint_date <= end).ToList();

            int open = filtered.Count(c => c.complaint_status_id == BASIC_STRUCTS.OPEN_COMPLAINT_STATUS);
            int pending = filtered.Count(c => c.complaint_status_id == BASIC_STRUCTS.PENDING_COMPLAINT_STATUS);
            int closed = filtered.Count(c => c.complaint_status_id == BASIC_STRUCTS.CLOSED_COMPLAINT_STATUS);
            int totalSites = filtered.Sum(c => c.sites != null ? c.sites.Count : 0);

            kpiPanel.Children.Add(MakeKpi("Total Complaints", filtered.Count, brand));
            kpiPanel.Children.Add(MakeKpi("Open", open, openBrush));
            kpiPanel.Children.Add(MakeKpi("Pending", pending, pendingBrush));
            kpiPanel.Children.Add(MakeKpi("Closed", closed, closedBrush));
            kpiPanel.Children.Add(MakeKpi("Affected Sites", totalSites, brand));

            // by company
            List<string> cl; List<double> cv;
            BuildCounts(filtered.Select(c => c.company_name), out cl, out cv, 12);
            chartsPanel.Children.Add(MakeChartCard("Complaints by Company",
                MakePie(cl, cv, cl.Select(CompanyBrush).ToList())));

            // by status
            chartsPanel.Children.Add(MakeChartCard("Complaints by Status",
                MakePie(new List<string> { "Open", "Pending", "Closed" },
                        new List<double> { open, pending, closed },
                        new List<Brush> { openBrush, pendingBrush, closedBrush })));

            // per month
            List<string> ml; List<double> mv;
            BuildMonthly(filtered.Select(c => c.complaint_date), out ml, out mv);
            chartsPanel.Children.Add(MakeChartCard("Complaints per Month", MakeColumn(ml, mv, brand), 620));
        }

        private void RenderMissions()
        {
            filterHintText.Text = "Date range filters missions by their start date.";

            DateTime start = RangeStart(), end = RangeEnd();
            List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> filtered = missions
                .Where(m => m.mission_Date >= start && m.mission_Date <= end).ToList();

            int closed = filtered.Count(m => m.status_id == BASIC_STRUCTS.MISSION_CLOSED_STATUS);
            int pending = filtered.Count - closed;

            kpiPanel.Children.Add(MakeKpi("Total Missions", filtered.Count, brand));
            kpiPanel.Children.Add(MakeKpi("Closed", closed, closedBrush));
            kpiPanel.Children.Add(MakeKpi("Pending", pending, pendingBrush));

            // per month
            List<string> ml; List<double> mv;
            BuildMonthly(filtered.Select(m => m.mission_Date), out ml, out mv);
            chartsPanel.Children.Add(MakeChartCard("Missions per Month", MakeColumn(ml, mv, brand), 620));

            // by engineer
            List<string> el; List<double> ev;
            BuildEngineerCounts(filtered, out el, out ev);
            chartsPanel.Children.Add(MakeChartCard("Missions by Engineer", MakeRow(el, ev, brand)));

            // by service provider (company)
            List<string> pl; List<double> pv;
            BuildCounts(filtered.Select(m => CompanyName(m.company_serial)), out pl, out pv, 12);
            chartsPanel.Children.Add(MakeChartCard("Missions by Service Provider",
                MakePie(pl, pv, pl.Select(CompanyBrush).ToList())));
        }

        private void RenderEngineers()
        {
            filterHintText.Text = "Date range filters the missions counted per engineer.";

            DateTime start = RangeStart(), end = RangeEnd();
            List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> filtered = missions
                .Where(m => m.mission_Date >= start && m.mission_Date <= end).ToList();

            kpiPanel.Children.Add(MakeKpi("Engineers", engineers.Count, brand));
            kpiPanel.Children.Add(MakeKpi("Missions in Range", filtered.Count, brand));

            List<string> el; List<double> ev;
            BuildEngineerCounts(filtered, out el, out ev);
            chartsPanel.Children.Add(MakeChartCard("Missions by Engineer", MakeRow(el, ev, brand), 620, 400));
        }

        private void RenderCompanies()
        {
            filterHintText.Text = "Companies and their sites are totals; complaints use the date range.";

            DateTime start = RangeStart(), end = RangeEnd();
            List<BASIC_STRUCTS.COMPLAINT_STRUCT> filtered = complaints
                .Where(c => c.complaint_date >= start && c.complaint_date <= end).ToList();

            kpiPanel.Children.Add(MakeKpi("Companies", companies.Count, brand));
            kpiPanel.Children.Add(MakeKpi("Total Sites", sites.Count, brand));
            kpiPanel.Children.Add(MakeKpi("Complaints", filtered.Count, brand));

            List<string> sl; List<double> sv;
            BuildCounts(sites.Select(s => s.company_name), out sl, out sv, 12);
            chartsPanel.Children.Add(MakeChartCard("Sites by Company",
                MakePie(sl, sv, sl.Select(CompanyBrush).ToList())));

            List<string> cl; List<double> cv;
            BuildCounts(filtered.Select(c => c.company_name), out cl, out cv, 12);
            chartsPanel.Children.Add(MakeChartCard("Complaints by Company", MakeRow(cl, cv, brand)));
        }

        private void RenderSites()
        {
            filterHintText.Text = "Site totals are not time-based; the date range does not apply here.";

            kpiPanel.Children.Add(MakeKpi("Total Sites", sites.Count, brand));
            kpiPanel.Children.Add(MakeKpi("Cities", sites.Select(s => s.city).Where(c => !string.IsNullOrEmpty(c)).Distinct().Count(), brand));

            List<string> cl; List<double> cv;
            BuildCounts(sites.Select(s => s.city), out cl, out cv, 15);
            chartsPanel.Children.Add(MakeChartCard("Sites by City", MakeRow(cl, cv, brand), 620, 400));

            List<string> col; List<double> cov;
            BuildCounts(sites.Select(s => s.company_name), out col, out cov, 12);
            chartsPanel.Children.Add(MakeChartCard("Sites by Company",
                MakePie(col, cov, col.Select(CompanyBrush).ToList())));
        }

        private void RenderRepeaters()
        {
            filterHintText.Text = "Date range filters repeaters by the date they were added.";

            DateTime start = RangeStart(), end = RangeEnd();
            List<BASIC_STRUCTS.REPEATER_STRUCT> filtered = repeaters
                .Where(r => !HasDate(r.date_added) || (r.date_added >= start && r.date_added <= end)).ToList();

            int pending = filtered.Count(r => r.status_id == BASIC_STRUCTS.PENDING_REPEATER_STATUS);
            int removed = filtered.Count(r => r.status_id == BASIC_STRUCTS.REMOVED_REPEATER_STATUS);

            kpiPanel.Children.Add(MakeKpi("Total Repeaters", filtered.Count, brand));
            kpiPanel.Children.Add(MakeKpi("Pending", pending, openBrush));
            kpiPanel.Children.Add(MakeKpi("Removed", removed, closedBrush));

            chartsPanel.Children.Add(MakeChartCard("Repeaters by Status",
                MakePie(new List<string> { "Pending", "Removed" },
                        new List<double> { pending, removed },
                        new List<Brush> { openBrush, closedBrush })));

            List<string> cl; List<double> cv;
            BuildCounts(filtered.Select(r => r.city), out cl, out cv, 15);
            chartsPanel.Children.Add(MakeChartCard("Repeaters by City", MakeRow(cl, cv, brand), 620, 400));
        }

        // ================= data helpers =================

        private static bool HasDate(DateTime d)
        {
            return d.Year > 1900;
        }

        private string CompanyName(int serial)
        {
            for (int i = 0; i < companies.Count; i++)
            {
                if (companies[i].key == serial)
                    return companies[i].value;
            }
            return "Unknown";
        }

        private void BuildCounts(IEnumerable<string> keys, out List<string> labels, out List<double> values, int top)
        {
            var groups = keys
                .Select(k => string.IsNullOrEmpty(k) ? "Unknown" : k)
                .GroupBy(k => k)
                .Select(g => new { Key = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(top)
                .ToList();

            labels = groups.Select(x => x.Key).ToList();
            values = groups.Select(x => (double)x.Count).ToList();
        }

        private void BuildMonthly(IEnumerable<DateTime> dates, out List<string> labels, out List<double> values)
        {
            var groups = dates
                .Where(d => HasDate(d))
                .GroupBy(d => new DateTime(d.Year, d.Month, 1))
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .OrderBy(x => x.Month)
                .ToList();

            labels = groups.Select(x => x.Month.ToString("MMM yy", CultureInfo.InvariantCulture)).ToList();
            values = groups.Select(x => (double)x.Count).ToList();
        }

        private void BuildEngineerCounts(List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> missionList,
            out List<string> labels, out List<double> values)
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();

            foreach (BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT mission in missionList)
            {
                if (mission.engineers == null)
                    continue;

                foreach (BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT engineer in mission.engineers)
                {
                    string name = string.IsNullOrEmpty(engineer.value) ? "Unknown" : engineer.value;
                    if (counts.ContainsKey(name))
                        counts[name]++;
                    else
                        counts[name] = 1;
                }
            }

            var ordered = counts.OrderByDescending(x => x.Value).Take(15).ToList();
            labels = ordered.Select(x => x.Key).ToList();
            values = ordered.Select(x => (double)x.Value).ToList();
        }

        private Brush CompanyBrush(string name)
        {
            if (string.IsNullOrEmpty(name))
                return brand;

            string n = name.ToLowerInvariant();
            if (n.Contains("vodafone")) return openBrush;
            if (n.Contains("etisalat")) return closedBrush;
            if (n.Contains("orange")) return new SolidColorBrush(Color.FromRgb(0xF9, 0x73, 0x16));
            if (n == "we" || n.StartsWith("we ") || n.Contains("telecom egypt")) return brand;
            return brand;
        }

        // ================= UI builders =================

        private Border MakeKpi(string title, int value, Brush accent)
        {
            Border card = new Border
            {
                Style = (Style)FindResource("CardBorder"),
                Width = 180,
                Margin = new Thickness(0, 0, 14, 14),
                Padding = new Thickness(18, 14, 18, 14)
            };

            StackPanel panel = new StackPanel();
            panel.Children.Add(new TextBlock
            {
                Text = value.ToString(),
                FontFamily = appFont,
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                Foreground = accent
            });
            panel.Children.Add(new TextBlock
            {
                Text = title,
                FontFamily = appFont,
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Foreground = muted,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 2, 0, 0)
            });

            card.Child = panel;
            return card;
        }

        private Border MakeChartCard(string title, FrameworkElement chart, double width = 460, double height = 300)
        {
            Border card = new Border
            {
                Style = (Style)FindResource("CardBorder"),
                Width = width,
                Margin = new Thickness(0, 0, 14, 14)
            };

            StackPanel panel = new StackPanel();
            panel.Children.Add(new TextBlock
            {
                Text = title,
                FontFamily = appFont,
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = brand,
                Margin = new Thickness(0, 0, 0, 10)
            });

            chart.Height = height;
            panel.Children.Add(chart);

            card.Child = panel;
            return card;
        }

        private PieChart MakePie(List<string> labels, List<double> values, List<Brush> fills)
        {
            SeriesCollection series = new SeriesCollection();

            for (int i = 0; i < labels.Count; i++)
            {
                series.Add(new PieSeries
                {
                    Title = labels[i],
                    Values = new ChartValues<double> { values[i] },
                    Fill = i < fills.Count ? fills[i] : brand,
                    DataLabels = true,
                    LabelPoint = point => ((int)point.Y).ToString()
                });
            }

            return new PieChart
            {
                Series = series,
                LegendLocation = LegendLocation.Bottom,
                Hoverable = true
            };
        }

        private CartesianChart MakeColumn(List<string> labels, List<double> values, Brush fill)
        {
            return new CartesianChart
            {
                Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Values = new ChartValues<double>(values),
                        Fill = fill,
                        DataLabels = true,
                        LabelPoint = point => ((int)point.Y).ToString()
                    }
                },
                AxisX = new AxesCollection { new Axis { Labels = labels, Separator = new Separator { Step = 1 } } },
                AxisY = new AxesCollection { new Axis { LabelFormatter = v => ((int)v).ToString(), MinValue = 0 } },
                LegendLocation = LegendLocation.None
            };
        }

        private CartesianChart MakeRow(List<string> labels, List<double> values, Brush fill)
        {
            return new CartesianChart
            {
                Series = new SeriesCollection
                {
                    new RowSeries
                    {
                        Values = new ChartValues<double>(values),
                        Fill = fill,
                        DataLabels = true,
                        LabelPoint = point => ((int)point.X).ToString()
                    }
                },
                AxisY = new AxesCollection { new Axis { Labels = labels, Separator = new Separator { Step = 1 } } },
                AxisX = new AxesCollection { new Axis { LabelFormatter = v => ((int)v).ToString(), MinValue = 0 } },
                LegendLocation = LegendLocation.None
            };
        }
    }
}
