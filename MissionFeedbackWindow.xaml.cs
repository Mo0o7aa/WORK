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
            sitesStackPanel.Children.Clear();

            for (int i = 0; i < mission.GetSites().Count; i++)
            {
                int siteSerial = mission.GetSites()[i].site_serial;

                // the sectors for this site come from the complaint (they carry
                // the sector_serial the DB update filters on)
                BASIC_STRUCTS.SITE_STRUCT complaintSite =
                    mission.complaint.GetComplaintsSites().Find(x1 => x1.site_serial == siteSerial);

                Border border = new Border()
                {
                    Style = (Style)FindResource("RowCardBorder"),
                    Margin = new Thickness(0, 0, 0, 12)
                };

                Grid currentSiteGrid = new Grid();
                currentSiteGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(240) });
                currentSiteGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                currentSiteGrid.ColumnDefinitions.Add(new ColumnDefinition());

                //////////////SITE NUMBER//////////////////
                StackPanel siteInfoPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };

                siteInfoPanel.Children.Add(new TextBlock
                {
                    Text = "Site Number",
                    Style = (Style)FindResource("FieldLabelText")
                });
                siteInfoPanel.Children.Add(new TextBlock
                {
                    Text = mission.GetSites()[i].site_name,
                    Style = (Style)FindResource("CardTitleText"),
                    Margin = new Thickness(0, 2, 0, 0)
                });

                currentSiteGrid.Children.Add(siteInfoPanel);
                Grid.SetColumn(siteInfoPanel, 0);

                //////////////SECTORS//////////////////
                TextBlock currentSectorLabel = new TextBlock
                {
                    Text = "Enhanced sectors:",
                    Style = (Style)FindResource("FieldLabelText"),
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 16, 0)
                };

                currentSiteGrid.Children.Add(currentSectorLabel);
                Grid.SetColumn(currentSectorLabel, 1);

                WrapPanel sectorsInnerPanel = new WrapPanel { VerticalAlignment = VerticalAlignment.Center };

                if (complaintSite.sectors != null)
                {
                    for (int j = 0; j < complaintSite.sectors.Count; j++)
                    {
                        BASIC_STRUCTS.SECTOR_STRUCT sector = complaintSite.sectors[j];

                        CheckBox currentSectorCheckBox = new CheckBox();
                        currentSectorCheckBox.Style = (Style)FindResource("checkBoxStyle");
                        currentSectorCheckBox.Width = double.NaN;
                        currentSectorCheckBox.Margin = new Thickness(0, 6, 20, 6);
                        currentSectorCheckBox.Cursor = Cursors.Hand;
                        currentSectorCheckBox.Content = "Sector " + sector.sector_number;
                        // Tag carries the sector_serial (what the DB update filters on)
                        currentSectorCheckBox.Tag = siteSerial + "," + sector.sector_serial;
                        currentSectorCheckBox.Checked += OnCheckCurrentSectorCheckBox;
                        currentSectorCheckBox.Unchecked += OnUnCheckCurrentSectorCheckBox;

                        if (sector.sector_status_id == BASIC_STRUCTS.CLEARED_SECTOR_STATUS)
                            currentSectorCheckBox.IsChecked = true;

                        sectorsInnerPanel.Children.Add(currentSectorCheckBox);
                    }
                }

                currentSiteGrid.Children.Add(sectorsInnerPanel);
                Grid.SetColumn(sectorsInnerPanel, 2);

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

            MessageBox.Show("Feedback saved.", "Done", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }
    }
}
