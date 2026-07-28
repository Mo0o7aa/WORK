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
    /// Interaction logic for MissionFeedbackWindow.xaml
    /// </summary>
    public partial class MissionFeedbackWindow : Window
    {
        Employee loggedInUser;
        SQLServer sqlDatabase;
        CommonQueries commonQueries;

        List<BASIC_STRUCTS.SECTOR_STRUCT> clearedSectors;
        List<BASIC_STRUCTS.SECTOR_STRUCT> impactedSectors;

        Missions mission;

        public MissionFeedbackWindow(ref Employee mLoggedInUser, ref Missions mMission)
        {
            loggedInUser = mLoggedInUser;
            mission = mMission;

            sqlDatabase = new SQLServer();
            commonQueries = new CommonQueries();

            clearedSectors = new List<BASIC_STRUCTS.SECTOR_STRUCT>();
            impactedSectors = new List<BASIC_STRUCTS.SECTOR_STRUCT>();

            InitializeComponent();

            InitializeSitesGrid();
        }

        public void InitializeSitesGrid()
        {
            for (int i = 0; i < mission.GetSites().Count; i++)
            {

                BrushConverter brushConverter = new BrushConverter();

                Border border = new Border() { BorderThickness = new Thickness(5) };
                border.BorderBrush = (Brush)brushConverter.ConvertFrom("#000080");

                Grid currentSiteGrid = new Grid();
                currentSiteGrid.ColumnDefinitions.Add(new ColumnDefinition());
                currentSiteGrid.ColumnDefinitions.Add(new ColumnDefinition());
                currentSiteGrid.ColumnDefinitions.Add(new ColumnDefinition());

                //////////////SITE NUMBER//////////////////
                WrapPanel currentSiteNumberWrapPanel = new WrapPanel();
                currentSiteNumberWrapPanel.VerticalAlignment = VerticalAlignment.Center;
                currentSiteNumberWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;

                Label currentSiteNumberLabel = new Label();
                currentSiteNumberLabel.Style = (Style)FindResource("mediumLabelStyleBlack");
                currentSiteNumberLabel.Content = "Site Number: ";

                Label currentSiteNumberLabelValue = new Label();
                currentSiteNumberLabelValue.Style = (Style)FindResource("labelStyle");
                currentSiteNumberLabelValue.Content = mission.GetSites()[i].site_name;

                currentSiteNumberWrapPanel.Children.Add(currentSiteNumberLabel);
                currentSiteNumberWrapPanel.Children.Add(currentSiteNumberLabelValue);

                currentSiteGrid.Children.Add(currentSiteNumberWrapPanel);
                Grid.SetColumn(currentSiteNumberWrapPanel, 0);


                //////////////SECTORS//////////////////

                Label currentSectorLabel = new Label();
                currentSectorLabel.Style = (Style)FindResource("wideLabelStyleBlack");
                currentSectorLabel.Content = "Select enhanced sectors: ";
                currentSectorLabel.VerticalAlignment = VerticalAlignment.Center;

                currentSiteGrid.Children.Add(currentSectorLabel);
                Grid.SetColumn(currentSectorLabel, 1);


                StackPanel sectorsInnerStackPanel = new StackPanel();
                for (int j = 0; j < mission.complaint.GetComplaintsSites().Find(x1 => x1.site_serial == mission.GetSites()[i].site_serial).sectors.Count; j++)
                {
                    CheckBox currentSectorCheckBox = new CheckBox();
                    currentSectorCheckBox.Style = (Style)FindResource("checkBoxStyle");
                    currentSectorCheckBox.Margin = new Thickness(12);
                    currentSectorCheckBox.HorizontalAlignment = HorizontalAlignment.Center;
                    currentSectorCheckBox.Cursor = Cursors.Hand;
                    currentSectorCheckBox.Content = "Sector " + mission.complaint.GetComplaintsSites().Find(x1 => x1.site_serial == mission.GetSites()[i].site_serial).sectors[j].sector_number;
                    currentSectorCheckBox.Tag = mission.GetSites()[i].site_serial + "," + mission.complaint.GetComplaintsSites().Find(x1 => x1.site_serial == mission.GetSites()[i].site_serial).sectors[j].sector_number;
                    currentSectorCheckBox.Checked += OnCheckCurrentSectorCheckBox;
                    currentSectorCheckBox.Unchecked += OnUnCheckCurrentSectorCheckBox;

                    if(mission.complaint.GetComplaintsSites().Find(x1 => x1.site_serial == mission.GetSites()[i].site_serial).sectors[j].sector_status_id == BASIC_STRUCTS.CLEARED_SECTOR_STATUS)
                        currentSectorCheckBox.IsChecked = true;

                    sectorsInnerStackPanel.Children.Add(currentSectorCheckBox);
                }

                currentSiteGrid.Children.Add(sectorsInnerStackPanel);
                Grid.SetColumn(sectorsInnerStackPanel, 2);

                border.Child = currentSiteGrid;

                sitesStackPanel.Children.Add(border);

            }
        }

        private void OnCheckCurrentSectorCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;

            BASIC_STRUCTS.SECTOR_STRUCT currentSector = new BASIC_STRUCTS.SECTOR_STRUCT();

            currentSector.complaint_serial = mission.complaint.GetSerial();
            currentSector.site_serial = int.Parse(currentCheckBox.Tag.ToString().Split(',')[0]);
            currentSector.sector_serial = int.Parse(currentCheckBox.Tag.ToString().Split(',')[1]);

            clearedSectors.Add(currentSector);

            impactedSectors.Remove(currentSector);
        }

        private void OnUnCheckCurrentSectorCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;

            BASIC_STRUCTS.SECTOR_STRUCT currentSector = new BASIC_STRUCTS.SECTOR_STRUCT();

            currentSector.complaint_serial = mission.complaint.GetSerial();
            currentSector.site_serial = int.Parse(currentCheckBox.Tag.ToString().Split(',')[0]);
            currentSector.sector_serial = int.Parse(currentCheckBox.Tag.ToString().Split(',')[1]);

            impactedSectors.Add(currentSector);

            clearedSectors.Remove(currentSector);
        }

        private void OnButtonClickSaveChanges(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < clearedSectors.Count; i++)
            {
                if (!mission.complaint.UpdateSectorStatus(clearedSectors[i].site_serial, clearedSectors[i].sector_serial, BASIC_STRUCTS.CLEARED_SECTOR_STATUS))
                    return;
            }

            for (int i = 0; i < impactedSectors.Count; i++)
            {
                if (!mission.complaint.UpdateSectorStatus(impactedSectors[i].site_serial, impactedSectors[i].sector_serial, BASIC_STRUCTS.INTERFERED_SECTOR_STATUS))
                    return;
            }

            for(int i = 0; i < mission.GetSites().Count; i++)
            {
                if (!mission.complaint.UpdateSiteStatus(mission.GetSites()[i].site_serial))
                    return;
            }

            if (!mission.complaint.UpdateComplaintStatus())
                return;

            // auto-close this mission and any earlier mission on the same
            // complaint whose sites are now fully cleared
            if (!mission.complaint.UpdateMissionStatuses())
                return;

            this.Close();
        }
    }
}
