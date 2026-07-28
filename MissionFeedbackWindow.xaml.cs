using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for MissionFeedbackWindow.xaml
    /// </summary>
    public partial class MissionFeedbackWindow : Window
    {
        Employee loggedInUser;
        Missions mission;

        // one checkbox per sector; the save reads their state directly so there
        // are no parallel lists to keep in sync
        private readonly List<CheckBox> sectorCheckBoxes;

        // what each checkbox needs to update its sector in the DB
        private class SectorRef
        {
            public int SiteSerial;
            public string SectorNumber;
        }

        public MissionFeedbackWindow(ref Employee mLoggedInUser, ref Missions mMission)
        {
            loggedInUser = mLoggedInUser;
            mission = mMission;

            sectorCheckBoxes = new List<CheckBox>();

            InitializeComponent();

            InitializeSitesGrid();
        }

        public void InitializeSitesGrid()
        {
            sitesStackPanel.Children.Clear();
            sectorCheckBoxes.Clear();

            for (int i = 0; i < mission.GetSites().Count; i++)
            {
                int siteSerial = mission.GetSites()[i].site_serial;

                // sectors come from the complaint (they carry sector_number and
                // the current status used to pre-tick the boxes)
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
                        currentSectorCheckBox.Tag = new SectorRef
                        {
                            SiteSerial = siteSerial,
                            SectorNumber = sector.sector_number
                        };

                        if (sector.sector_status_id == BASIC_STRUCTS.CLEARED_SECTOR_STATUS)
                            currentSectorCheckBox.IsChecked = true;

                        sectorCheckBoxes.Add(currentSectorCheckBox);
                        sectorsInnerPanel.Children.Add(currentSectorCheckBox);
                    }
                }

                currentSiteGrid.Children.Add(sectorsInnerPanel);
                Grid.SetColumn(sectorsInnerPanel, 2);

                border.Child = currentSiteGrid;

                sitesStackPanel.Children.Add(border);
            }
        }

        private void OnButtonClickSaveChanges(object sender, RoutedEventArgs e)
        {
            // write every sector's status straight from its checkbox state
            for (int i = 0; i < sectorCheckBoxes.Count; i++)
            {
                SectorRef reference = (SectorRef)sectorCheckBoxes[i].Tag;
                int status = sectorCheckBoxes[i].IsChecked == true
                    ? BASIC_STRUCTS.CLEARED_SECTOR_STATUS
                    : BASIC_STRUCTS.INTERFERED_SECTOR_STATUS;

                if (!mission.complaint.UpdateSectorStatusByNumber(reference.SiteSerial, reference.SectorNumber, status))
                    return;
            }

            // roll the change up to site, complaint and mission statuses
            for (int i = 0; i < mission.GetSites().Count; i++)
            {
                if (!mission.complaint.UpdateSiteStatus(mission.GetSites()[i].site_serial))
                    return;
            }

            if (!mission.complaint.UpdateComplaintStatus())
                return;

            if (!mission.complaint.UpdateMissionStatuses())
                return;

            MessageBox.Show("Feedback saved.", "Done", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }
    }
}
