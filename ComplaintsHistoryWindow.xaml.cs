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
using System.Windows.Shapes;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for ComplaintsHistoryWindow.xaml
    /// </summary>
    public partial class ComplaintsHistoryWindow : Window
    {
        CommonQueries commonQueries;

        Employee loggedInUser;
        List<BASIC_STRUCTS.COMPLAINT_STRUCT> complaints;
        List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> missions;

        private Expander currentExpander;
        private Expander previousExpander;


        public ComplaintsHistoryWindow(ref Employee mLoggedInUser, List<BASIC_STRUCTS.COMPLAINT_STRUCT> tempComplaints, List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> tempMissions)
        {
            loggedInUser = mLoggedInUser;
            complaints = tempComplaints;
            missions = tempMissions;

            commonQueries = new CommonQueries();

            InitializeComponent();

            InitializeComplaintsStackPanel();
            InitializeMissionsStackPanel();
        }

        private void InitializeComplaintsStackPanel()
        {
            BrushConverter brushConverter = new BrushConverter();

            complaintsStackPanel.Children.Clear();

            for(int i = 0; i < complaints.Count; i++)
            {
                Border border = new Border() { BorderThickness = new Thickness(3) };
                border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");
                border.CornerRadius = new CornerRadius(3);
                border.Background = Brushes.White;

                Grid grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80) });
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                Label complaintHeaderLabel = new Label();
                complaintHeaderLabel.Style = (Style)FindResource("stackPanelHeaderLabelStyle");
                complaintHeaderLabel.Content = complaints[i].complaint_id;
                complaintHeaderLabel.Margin = new Thickness(10);

                grid.Children.Add(complaintHeaderLabel);
                Grid.SetColumnSpan(complaintHeaderLabel, 3);
                Grid.SetRow(complaintHeaderLabel, 0);

                WrapPanel companyNameWrapPanel = new WrapPanel();

                Label companyNameLabel = new Label();
                companyNameLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                companyNameLabel.Content = "Company: ";

                Label companyNameLabelValue = new Label();
                companyNameLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                companyNameLabelValue.Content = complaints[i].company_name;

                companyNameWrapPanel.Children.Add(companyNameLabel);
                companyNameWrapPanel.Children.Add(companyNameLabelValue);

                grid.Children.Add(companyNameWrapPanel);
                Grid.SetColumn(companyNameWrapPanel, 0);
                Grid.SetRow(companyNameWrapPanel, 1);

                WrapPanel cityWrapPanel = new WrapPanel();

                Label cityLabel = new Label();
                cityLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                cityLabel.Content = "Added by: ";

                Label cityLabelValue = new Label();
                cityLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                cityLabelValue.Content = complaints[i].added_by;

                cityWrapPanel.Children.Add(cityLabel);
                cityWrapPanel.Children.Add(cityLabelValue);

                grid.Children.Add(cityWrapPanel);
                Grid.SetColumn(cityWrapPanel, 2);
                Grid.SetRow(cityWrapPanel, 1);

                WrapPanel ticketNumberWrapPanel = new WrapPanel();

                Label ticketNumberLabel = new Label();
                ticketNumberLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                ticketNumberLabel.Content = "Ticket: ";

                Label ticketNumberLabelValue = new Label();
                ticketNumberLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                ticketNumberLabelValue.Content = complaints[i].ticket_number;

                ticketNumberWrapPanel.Children.Add(ticketNumberLabel);
                ticketNumberWrapPanel.Children.Add(ticketNumberLabelValue);

                grid.Children.Add(ticketNumberWrapPanel);
                Grid.SetRow(ticketNumberWrapPanel, 1);
                Grid.SetColumn(ticketNumberWrapPanel, 1);

                //WrapPanel regionWrapPanel = new WrapPanel();
                //
                //Label regionLabel = new Label();
                //regionLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                //regionLabel.Content = "Region: ";
                //
                //Label regionLabelValue = new Label();
                //regionLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                //regionLabelValue.Content = complaints[i].region;
                //
                //regionWrapPanel.Children.Add(regionLabel);
                //regionWrapPanel.Children.Add(regionLabelValue);
                //
                //grid.Children.Add(regionWrapPanel);
                //Grid.SetColumn(regionWrapPanel, 0);
                //Grid.SetRow(regionWrapPanel, 3);

                WrapPanel siteWrapPanel = new WrapPanel();

                Label siteLabel = new Label();
                siteLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                siteLabel.Content = "No of Sites: ";

                Label siteLabelValue = new Label();
                siteLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                int sitesCount = 0;
                if (!commonQueries.GetComplaintNumberOfSites(complaints[i].company_serial, complaints[i].complaint_serial, ref sitesCount))
                    return;
                siteLabelValue.Content = sitesCount;

                siteWrapPanel.Children.Add(siteLabel);
                siteWrapPanel.Children.Add(siteLabelValue);

                grid.Children.Add(siteWrapPanel);
                Grid.SetRow(siteWrapPanel, 2);
                Grid.SetColumn(siteWrapPanel, 0);

                Border statusBorder = new Border();
                statusBorder.Style = (Style)FindResource("statusBorderStyle");
                statusBorder.HorizontalAlignment = HorizontalAlignment.Left;
                if (complaints[i].complaint_status_id == 1)
                    statusBorder.Background = Brushes.Red;
                else if (complaints[i].complaint_status_id == 2)
                    statusBorder.Background = Brushes.Yellow;
                else
                    statusBorder.Background = Brushes.Green;

                Label statusLabel = new Label();
                statusLabel.Style = (Style)FindResource("statusLabelStyle");
                statusLabel.HorizontalAlignment = HorizontalAlignment.Center;
                statusLabel.Content = complaints[i].complaint_status;

                statusBorder.Child = statusLabel;
                grid.Children.Add(statusBorder);
                Grid.SetRow(statusBorder, 2);
                Grid.SetColumn(statusBorder, 2);

                Expander expander = new Expander();
                expander.HorizontalContentAlignment = HorizontalAlignment.Left;
                expander.VerticalAlignment = VerticalAlignment.Center;
                expander.ExpandDirection = ExpandDirection.Down;
                expander.Expanded += OnExpandExpander;
                expander.Margin = new Thickness(0,6,0,0);
                expander.Tag = i;

                StackPanel expanderstackPanel = new StackPanel();
                expanderstackPanel.Width = 50;

                Button viewButton = new Button();
                viewButton.Content = "View";
                viewButton.Width = 50;
                viewButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                viewButton.Click += OnClickViewComplaint;

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.Width = 50;
                editButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                editButton.Click += OnClickEditComplaint;

                expanderstackPanel.Children.Add(viewButton);
                expanderstackPanel.Children.Add(editButton);

                expander.Content = expanderstackPanel;

                grid.Children.Add(expander);
                Grid.SetRow(expander, 0);
                Grid.SetRowSpan(expander, 2);
                Grid.SetColumn(expander, 2);

                WrapPanel dateWrapPanel = new WrapPanel();

                Label dateLabel = new Label();
                dateLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                dateLabel.Content = "Date: ";

                Label dateLabelValue = new Label();
                dateLabelValue.Style = (Style)FindResource("stackPanelLabelStyle");
                dateLabelValue.Content = complaints[i].complaint_date;

                dateWrapPanel.Children.Add(dateLabel);
                dateWrapPanel.Children.Add(dateLabelValue);

                grid.Children.Add(dateWrapPanel);
                Grid.SetRow(dateWrapPanel, 2);
                Grid.SetColumn(dateWrapPanel, 1);

                border.Child = grid;

                complaintsStackPanel.Children.Add(border);
            }

            void OnClickEditComplaint(object sender, RoutedEventArgs e)
            {
                Button currentButton = (Button)sender;
                StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
                Expander expander = (Expander)currentStackPanel.Parent;

                Complaints complaint = new Complaints();

                if (!complaint.InitializeComplaint(complaints[int.Parse(expander.Tag.ToString())].company_serial, complaints[int.Parse(expander.Tag.ToString())].complaint_serial))
                    return;

                int viewAddCondition = BASIC_STRUCTS.COMPLAINT_EDIT_CONDITION;

                ComplaintsWindow complaintsWindow = new ComplaintsWindow(ref loggedInUser, ref complaint, ref viewAddCondition);
                complaintsWindow.Show();
            }

            void OnClickViewComplaint(object sender, RoutedEventArgs e)
            {
                Button currentButton = (Button)sender;
                StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
                Expander expander = (Expander)currentStackPanel.Parent;

                Complaints complaint = new Complaints();

                if (!complaint.InitializeComplaint(complaints[int.Parse(expander.Tag.ToString())].company_serial, complaints[int.Parse(expander.Tag.ToString())].complaint_serial))
                    return;

                int viewAddCondition = BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION;

                ComplaintsWindow complaintsWindow = new ComplaintsWindow(ref loggedInUser, ref complaint, ref viewAddCondition);
                complaintsWindow.Show();
            }

            void OnExpandExpander(object sender, RoutedEventArgs e)
            {
                previousExpander = currentExpander;
                currentExpander = (Expander)sender;

                if (previousExpander != null && previousExpander != currentExpander)
                    previousExpander.IsExpanded = false;
            }


        }

        private void InitializeMissionsStackPanel()
        {
            missionsStackPanel.Orientation = Orientation.Vertical;
            missionsStackPanel.Children.Clear();

            for (int i = 0; i < missions.Count; i++)
            {

                Grid missionGrid = new Grid();
                missionGrid.Background = Brushes.White;
                missionGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                missionGrid.VerticalAlignment = VerticalAlignment.Stretch;

                missionGrid.RowDefinitions.Add(new RowDefinition());
                missionGrid.RowDefinitions.Add(new RowDefinition());
                missionGrid.RowDefinitions.Add(new RowDefinition());

                missionGrid.ColumnDefinitions.Add(new ColumnDefinition());
                missionGrid.ColumnDefinitions.Add(new ColumnDefinition());
                missionGrid.ColumnDefinitions.Add(new ColumnDefinition());

                BrushConverter brushConverter = new BrushConverter();

                Border border = new Border() { BorderThickness = new Thickness(3) };
                border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");

                //WrapPanel logoWrapPanel = new WrapPanel();
                //
                //String imageSource = @"\Photos\" + missions[i].service_provider_id.ToString() + ".png";
                //
                //Image logo = new Image();
                //logo.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));
                //
                //logo.Width = 80;
                //logo.Height = 80;

                Label missionIdLabel = new Label();
                missionIdLabel.Content = missions[i].mission_id;
                missionIdLabel.Style = (Style)FindResource("stackPanelHeaderLabelStyle");
                missionIdLabel.Width = 500;

                //logoWrapPanel.Children.Add(logo);
                //logoWrapPanel.Children.Add(missionIdLabel);

                missionGrid.Children.Add(missionIdLabel);
                Grid.SetRow(missionIdLabel, 0);
                Grid.SetColumnSpan(missionIdLabel, 3);

                StackPanel engineersStackPanel = new StackPanel();
                engineersStackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                engineersStackPanel.VerticalAlignment = VerticalAlignment.Stretch;

                for (int j = 0; j < missions[i].engineers.Count; j++)
                {
                    WrapPanel wrapPanel = new WrapPanel();
                    wrapPanel.HorizontalAlignment = HorizontalAlignment.Center;

                    Label tempEngineerLabel = new Label();
                    tempEngineerLabel.Content = "Eng." + (j + 1) + ":";
                    tempEngineerLabel.Style = (Style)FindResource("labelStyleBlack");

                    Label tempNameLabel = new Label();
                    tempNameLabel.Style = (Style)FindResource("mediumLabelStyle");
                    tempNameLabel.Content = missions[i].engineers[j].value;

                    wrapPanel.Children.Add(tempEngineerLabel);
                    wrapPanel.Children.Add(tempNameLabel);

                    engineersStackPanel.Children.Add(wrapPanel);
                }

                missionGrid.Children.Add(engineersStackPanel);
                Grid.SetRow(engineersStackPanel, 1);
                Grid.SetColumn(engineersStackPanel, 0);

                WrapPanel SitesNumberWrapPanel = new WrapPanel();
                SitesNumberWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
                SitesNumberWrapPanel.VerticalAlignment = VerticalAlignment.Center;

                Label sitesNumberLabel = new Label();
                sitesNumberLabel.Content = "No of Sites:";
                sitesNumberLabel.Style = (Style)FindResource("labelStyleBlack");

                Label sitesNumberValueLabel = new Label();
                sitesNumberValueLabel.Content = missions[i].sites.Count;
                sitesNumberValueLabel.Style = (Style)FindResource("mediumLabelStyle");

                SitesNumberWrapPanel.Children.Add(sitesNumberLabel);
                SitesNumberWrapPanel.Children.Add(sitesNumberValueLabel);

                missionGrid.Children.Add(SitesNumberWrapPanel);
                Grid.SetRow(SitesNumberWrapPanel, 2);
                Grid.SetColumn(SitesNumberWrapPanel, 0);

                WrapPanel vehichleWrapPanel = new WrapPanel();
                vehichleWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
                vehichleWrapPanel.VerticalAlignment = VerticalAlignment.Center;

                Label vehicleLabel = new Label();
                vehicleLabel.Content = "Vehicle: ";
                vehicleLabel.Style = (Style)FindResource("labelStyleBlack");

                Label vehichleValueLabel = new Label();
                vehichleValueLabel.Content = missions[i].vehichle;
                vehichleValueLabel.Style = (Style)FindResource("mediumLabelStyle");

                vehichleWrapPanel.Children.Add(vehicleLabel);
                vehichleWrapPanel.Children.Add(vehichleValueLabel);

                missionGrid.Children.Add(vehichleWrapPanel);
                Grid.SetRow(vehichleWrapPanel, 1);
                Grid.SetColumn(vehichleWrapPanel, 1);

                Border statusBorder = new Border();
                statusBorder.Style = (Style)FindResource("statusBorderStyle");
                statusBorder.Width = 200;
                if (missions[i].status_id == BASIC_STRUCTS.MISSION_PENDING_OPERATOR_ACTION_STATUS || missions[i].status_id == BASIC_STRUCTS.MISSION_PENDING_NTRA_ACTION_STATUS)
                    statusBorder.Background = Brushes.Yellow;
                else
                    statusBorder.Background = Brushes.Green;

                Label statusLabel = new Label();
                statusLabel.Content = missions[i].status;
                statusLabel.Style = (Style)FindResource("statusLabelStyle");
                statusLabel.Width = 200;

                statusBorder.Child = statusLabel;

                missionGrid.Children.Add(statusBorder);
                Grid.SetRow(statusBorder, 2);
                Grid.SetColumn(statusBorder, 1);
                Grid.SetColumnSpan(statusBorder, 2);

                WrapPanel dateWrapPanel = new WrapPanel();
                dateWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
                dateWrapPanel.VerticalAlignment = VerticalAlignment.Center;

                Label dateLabel = new Label();
                dateLabel.Style = (Style)FindResource("labelStyleBlack");
                dateLabel.Content = "Date: ";

                Label dateLabelValue = new Label();
                dateLabelValue.Content = missions[i].mission_Date.Day + "/" + missions[i].mission_Date.Month + "/" + missions[i].mission_Date.Year;
                dateLabelValue.Style = (Style)FindResource("mediumLabelStyle");

                dateWrapPanel.Children.Add(dateLabel);
                dateWrapPanel.Children.Add(dateLabelValue);

                missionGrid.Children.Add(dateWrapPanel);
                Grid.SetRow(dateWrapPanel, 1);
                Grid.SetColumn(dateWrapPanel, 2);

                Expander expander = new Expander();
                expander.Tag = missions[i].company_serial + "," + missions[i].complaint_serial + "," + missions[i].mission_serial;
                expander.HorizontalContentAlignment = HorizontalAlignment.Left;
                expander.VerticalAlignment = VerticalAlignment.Top;
                expander.ExpandDirection = ExpandDirection.Down;
                expander.Expanded += OnExpandExpander;
                expander.Margin = new Thickness(0, 24, 0, 0);

                StackPanel expanderStackPanel = new StackPanel();

                Button viewButton = new Button();
                viewButton.Content = "View Mission";
                viewButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                viewButton.Click += OnBtnClickView;

                Button viewComplaintButton = new Button();
                viewComplaintButton.Content = "View Complaint";
                viewComplaintButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                viewComplaintButton.Tag = missions[i].company_serial + "," + missions[i].complaint_serial;
                viewComplaintButton.Click += OnClickViewComplaint;


                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                editButton.Click += OnClickEdit;


                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                deleteButton.Click += OnBtnClickDelete;

                expanderStackPanel.Children.Add(viewButton);
                expanderStackPanel.Children.Add(viewComplaintButton);
                expanderStackPanel.Children.Add(editButton);
                //expanderStackPanel.Children.Add(deleteButton);

                expander.Content = expanderStackPanel;

                missionGrid.Children.Add(expander);
                Grid.SetColumn(expander, 2);

                border.Child = missionGrid;

                missionsStackPanel.Children.Add(border);
            }

        }

        private void OnClickViewComplaint(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)currentStackPanel.Parent;

            Complaints complaint = new Complaints();

            if (!complaint.InitializeComplaint(int.Parse(currentExpander.Tag.ToString().Split(',')[0]), int.Parse(currentExpander.Tag.ToString().Split(',')[1])))
                return;

            int viewAddCondition = BASIC_STRUCTS.COMPLAINT_VIEW_CONDITION;

            ComplaintsWindow complaintsWindow = new ComplaintsWindow(ref loggedInUser, ref complaint, ref viewAddCondition);
            complaintsWindow.Show();

        }

        private void OnClickShowSite(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;

            Site currentSite = new Site();

            String currentCompanySerial = currentButton.Tag.ToString().Split(',')[0];
            String currentSiteSerial = currentButton.Tag.ToString().Split(',')[2];

            if (!currentSite.InitializeSite(int.Parse(currentCompanySerial), int.Parse(currentSiteSerial)))
                return;

            int viewAddCondition = BASIC_STRUCTS.SITE_VIEW_CONDITION;

            SiteWindow siteWindow = new SiteWindow(ref loggedInUser, ref currentSite, ref viewAddCondition);

            siteWindow.Show();
        }

        ///////BUTTON CLICK HANDLERS////////////////////////
        ////////////////////////////////////////////////////

        private void OnBtnClickDelete(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)currentStackPanel.Parent;


            Missions mission = new Missions();
            //mission.InitializeMission(missions[index].id);
            //
            //if (!mission.DeleteMission())
            //    return;

            if (!commonQueries.GetMissions(ref missions))
                return;

            //InitializeMissionsStackPanel();
        }

        private void OnClickEdit(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)currentStackPanel.Parent;

            int missionCondition = BASIC_STRUCTS.MISSION_EDIT_CONDITION;

            Missions mission = new Missions();
            if (!mission.InitializeMission(int.Parse(currentExpander.Tag.ToString().Split(',')[0]), int.Parse(currentExpander.Tag.ToString().Split(',')[1]), int.Parse(currentExpander.Tag.ToString().Split(',')[0])))
                return;

            MissionWindow missionWindow = new MissionWindow(ref loggedInUser, ref mission, ref missionCondition);
            missionWindow.Closed += OnClosedMissionWindow;
            missionWindow.Show();
        }

        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel currentStackPanel = (StackPanel)currentButton.Parent;
            Expander currentExpander = (Expander)currentStackPanel.Parent;

            int missionCondition = BASIC_STRUCTS.MISSION_VIEW_CONDITION;

            Missions mission = new Missions();

            if (!mission.InitializeMission(int.Parse(currentExpander.Tag.ToString().Split(',')[0]), int.Parse(currentExpander.Tag.ToString().Split(',')[1]), int.Parse(currentExpander.Tag.ToString().Split(',')[2])))
                return;

            MissionWindow missionWindow = new MissionWindow(ref loggedInUser, ref mission, ref missionCondition);
            missionWindow.Show();

        }


        private void OnClosedMissionWindow(object sender, EventArgs e)
        {
            if (!commonQueries.GetMissions(ref missions))
                return;

            InitializeMissionsStackPanel();
        }

        private void OnExpandExpander(object sender, RoutedEventArgs e)
        {
            previousExpander = currentExpander;
            currentExpander = (Expander)sender;

            if (previousExpander != null && previousExpander != currentExpander)
                previousExpander.IsExpanded = false;
        }
    }
}
