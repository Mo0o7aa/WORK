using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace ntra_missions
{
    // Lightweight row view-models over BASIC_STRUCTS.
    // WPF data binding requires properties, the structs expose fields —
    // these classes are what page templates bind to.

    public static class RowBrushes
    {
        public static readonly Brush Open = new SolidColorBrush(Color.FromRgb(0xDC, 0x26, 0x26));
        public static readonly Brush Pending = new SolidColorBrush(Color.FromRgb(0xD9, 0x77, 0x06));
        public static readonly Brush Closed = new SolidColorBrush(Color.FromRgb(0x16, 0xA3, 0x4A));

        static RowBrushes()
        {
            Open.Freeze();
            Pending.Freeze();
            Closed.Freeze();
        }
    }

    public class FileRow
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public bool CanDelete { get; set; }
        public Visibility DeleteVisibility
        {
            get { return CanDelete ? Visibility.Visible : Visibility.Collapsed; }
        }
    }

    public class ContactRow
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class FreqRow
    {
        public string First { get; set; }
        public string Second { get; set; }
    }

    public class CompanyRow
    {
        public int Serial { get; set; }
        public string Name { get; set; }
        public int WorkFieldId { get; set; }
        public string WorkField { get; set; }
        public string FreqHeader1 { get; set; }
        public string FreqHeader2 { get; set; }
        public List<ContactRow> Contacts { get; set; }
        public List<FreqRow> Frequencies { get; set; }
        public List<BASIC_STRUCTS.FREQUENCY_RANGE_STRUCT> Ranges { get; set; }
        public List<KeyValuePair<decimal, decimal>> DecimalRanges { get; set; }

        public CompanyRow(BASIC_STRUCTS.COMPANY_STRUCT company)
        {
            Serial = company.company_serial;
            Name = company.company_name;
            WorkFieldId = company.company_field_of_work_id;
            WorkField = company.company_field_of_work;

            Contacts = new List<ContactRow>();
            if (company.contacts != null)
            {
                foreach (BASIC_STRUCTS.CONTACT_STRUCT contact in company.contacts)
                {
                    Contacts.Add(new ContactRow
                    {
                        Name = contact.contact_name,
                        Phone = contact.contact_phone,
                        Email = contact.contact_email
                    });
                }
            }

            bool hasStartStop = company.start_and_stop_frequencies != null && company.start_and_stop_frequencies.Count > 0;
            FreqHeader1 = hasStartStop ? "Start Frequency" : "Centre Frequency";
            FreqHeader2 = hasStartStop ? "Stop Frequency" : "Bandwidth";

            Frequencies = new List<FreqRow>();
            DecimalRanges = new List<KeyValuePair<decimal, decimal>>();

            if (company.start_and_stop_frequencies != null)
            {
                foreach (BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT f in company.start_and_stop_frequencies)
                {
                    Frequencies.Add(new FreqRow
                    {
                        First = f.start_frequency + " " + f.start_frequency_unit,
                        Second = f.stop_frequency + " " + f.stop_frequency_unit
                    });
                    DecimalRanges.Add(new KeyValuePair<decimal, decimal>(f.min, f.max));
                }
            }

            if (company.cf_and_bw_frequencies != null)
            {
                foreach (BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT f in company.cf_and_bw_frequencies)
                {
                    Frequencies.Add(new FreqRow
                    {
                        First = f.centre_frequency + " " + f.centre_frequency_unit,
                        Second = f.bandwidth + " " + f.bandwidth_unit
                    });
                    DecimalRanges.Add(new KeyValuePair<decimal, decimal>(f.min, f.max));
                }
            }
        }

        public bool MatchesBand(decimal frequency)
        {
            foreach (KeyValuePair<decimal, decimal> range in DecimalRanges)
            {
                if (frequency >= range.Key && frequency <= range.Value)
                    return true;
            }
            return false;
        }
    }

    public class ComplaintRow
    {
        public int CompanySerial { get; set; }
        public int ComplaintSerial { get; set; }
        public string ComplaintId { get; set; }
        public string CompanyName { get; set; }
        public string Ticket { get; set; }
        public string AddedBy { get; set; }
        public int AddedById { get; set; }
        public string Area { get; set; }
        public int SitesCount { get; set; }
        public DateTime Date { get; set; }
        public string DateText { get; set; }
        public int StatusId { get; set; }
        public string StatusText { get; set; }
        public Brush StatusBrush { get; set; }
        public bool CanEdit { get; set; }
        public Visibility EditVisibility
        {
            get { return CanEdit ? Visibility.Visible : Visibility.Collapsed; }
        }

        // Complaints are the shared, dynamic side: any engineer may attach and
        // remove files, unlike missions which are restricted to their own team.
        public bool CanManageAttachments
        {
            get { return true; }
        }

        public List<string> SiteNumbers { get; set; }
        public ObservableCollection<FileRow> Attachments { get; private set; }

        public ComplaintRow(BASIC_STRUCTS.COMPLAINT_STRUCT complaint, int loggedInUserId)
        {
            CompanySerial = complaint.company_serial;
            ComplaintSerial = complaint.complaint_serial;
            ComplaintId = complaint.complaint_id;
            CompanyName = complaint.company_name;
            Ticket = complaint.ticket_number;
            AddedBy = complaint.added_by;
            AddedById = complaint.added_by_id;
            Date = complaint.complaint_date;
            DateText = complaint.complaint_date.ToString();
            StatusId = complaint.complaint_status_id;
            StatusText = complaint.complaint_status;
            CanEdit = complaint.added_by_id == loggedInUserId;
            Attachments = new ObservableCollection<FileRow>();

            SiteNumbers = new List<string>();
            if (complaint.sites != null)
            {
                SitesCount = complaint.sites.Count;
                Area = complaint.sites.Count > 0 ? complaint.sites[0].region : "";
                foreach (BASIC_STRUCTS.SITE_STRUCT site in complaint.sites)
                    SiteNumbers.Add(site.site_number ?? "");
            }

            if (complaint.complaint_status_id == BASIC_STRUCTS.OPEN_COMPLAINT_STATUS)
                StatusBrush = RowBrushes.Open;
            else if (complaint.complaint_status_id == BASIC_STRUCTS.PENDING_COMPLAINT_STATUS)
                StatusBrush = RowBrushes.Pending;
            else
                StatusBrush = RowBrushes.Closed;
        }
    }

    public class MissionSiteRow
    {
        public string SiteName { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
        public int CompanySerial { get; set; }
        public int ComplaintSerial { get; set; }
        public int SiteSerial { get; set; }
    }

    public class MissionRow
    {
        public int CompanySerial { get; set; }
        public int ComplaintSerial { get; set; }
        public int MissionSerial { get; set; }
        public string MissionId { get; set; }
        public string Area { get; set; }
        public List<string> Engineers { get; set; }
        public List<int> EngineerKeys { get; set; }
        public List<MissionSiteRow> Sites { get; set; }
        public List<string> SiteNames { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartDateText { get; set; }
        public string EndDateText { get; set; }
        public int StatusId { get; set; }
        public string StatusText { get; set; }
        public Brush StatusBrush { get; set; }
        public bool CanEdit { get; set; }
        public Visibility EditVisibility
        {
            get { return CanEdit ? Visibility.Visible : Visibility.Collapsed; }
        }

        // Attachments on a mission belong to the engineers who actually went on it:
        // they attach and remove, every other engineer can review but not change.
        // Deliberately NOT added_by — a mission is logged by one engineer on behalf
        // of the whole team.
        public bool CanManageAttachments { get; set; }
        public Visibility AttachVisibility
        {
            get { return CanManageAttachments ? Visibility.Visible : Visibility.Collapsed; }
        }

        public ObservableCollection<FileRow> Attachments { get; private set; }

        public MissionRow(BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT mission, int loggedInUserId)
        {
            CompanySerial = mission.company_serial;
            ComplaintSerial = mission.complaint_serial;
            MissionSerial = mission.mission_serial;
            MissionId = mission.mission_id;
            StartDate = mission.mission_Date;
            EndDate = mission.end_date;
            StartDateText = mission.mission_Date.Day + "/" + mission.mission_Date.Month + "/" + mission.mission_Date.Year;
            EndDateText = mission.end_date.Day + "/" + mission.end_date.Month + "/" + mission.end_date.Year;
            StatusId = mission.status_id;
            StatusText = mission.status;
            CanEdit = mission.added_by_id == loggedInUserId;
            Attachments = new ObservableCollection<FileRow>();

            Engineers = new List<string>();
            EngineerKeys = new List<int>();
            if (mission.engineers != null)
            {
                foreach (BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT engineer in mission.engineers)
                {
                    Engineers.Add(engineer.value);
                    EngineerKeys.Add(engineer.key);
                }
            }

            CanManageAttachments = EngineerKeys.Contains(loggedInUserId);

            Sites = new List<MissionSiteRow>();
            SiteNames = new List<string>();
            if (mission.sites != null)
            {
                Area = mission.sites.Count > 0 ? mission.sites[0].region : "";
                foreach (BASIC_STRUCTS.MISSION_SITE_STRUCT site in mission.sites)
                {
                    Sites.Add(new MissionSiteRow
                    {
                        SiteName = site.site_name,
                        Reason = site.reason_of_interference,
                        Comment = site.comment,
                        CompanySerial = mission.company_serial,
                        ComplaintSerial = mission.complaint_serial,
                        SiteSerial = site.site_serial
                    });
                    SiteNames.Add(site.site_name ?? "");
                }
            }

            if (mission.status_id == BASIC_STRUCTS.MISSION_PENDING_OPERATOR_ACTION_STATUS ||
                mission.status_id == BASIC_STRUCTS.MISSION_PENDING_NTRA_ACTION_STATUS)
                StatusBrush = RowBrushes.Pending;
            else
                StatusBrush = RowBrushes.Closed;
        }
    }

    public class SiteRow
    {
        public int CompanySerial { get; set; }
        public int SiteSerial { get; set; }
        public string SiteNumber { get; set; }
        public string CompanyName { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int AddedById { get; set; }
        public string AddedBy { get; set; }

        public SiteRow(BASIC_STRUCTS.MIN_SITE_STRUCT site)
        {
            CompanySerial = site.company_serial;
            SiteSerial = site.site_serial;
            SiteNumber = site.site_number;
            CompanyName = site.company_name;
            CityId = site.city_id;
            City = site.city;
            Region = site.region;
            Latitude = site.latitude;
            Longitude = site.longitude;
            AddedById = site.added_by_id;
            AddedBy = site.added_by;
        }
    }

    public class RepeaterRow
    {
        public int Serial { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Coordinates { get; set; }
        public string Comment { get; set; }
        public string AddedBy { get; set; }
        public int StatusId { get; set; }
        public string StatusText { get; set; }
        public Brush StatusBrush { get; set; }

        public RepeaterRow(BASIC_STRUCTS.REPEATER_STRUCT repeater)
        {
            Serial = repeater.repeater_serial;
            CityId = repeater.city_id;
            City = repeater.city;
            Area = repeater.area;
            Address = repeater.address;
            Latitude = repeater.latitude;
            Longitude = repeater.longitude;
            Coordinates = repeater.latitude + ", " + repeater.longitude;
            Comment = repeater.comment;
            AddedBy = repeater.added_by;
            StatusId = repeater.status_id;
            StatusText = repeater.status;
            StatusBrush = repeater.status_id == BASIC_STRUCTS.PENDING_REPEATER_STATUS
                ? RowBrushes.Open
                : RowBrushes.Closed;
        }
    }

    public class NumberedRow
    {
        public string Number { get; set; }
        public string Text { get; set; }
    }
}
