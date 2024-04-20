using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ntra_missions
{
    public class BASIC_STRUCTS
    {
        public const int START_YEAR = 2022;

        public const int SITE_VIEW_CONDITION = 0;
        public const int SITE_ADD_CONDITION = 1;
        public const int SITE_EDIT_CONDITION = 2;

        public const int COMPLAINT_VIEW_CONDITION = 0;
        public const int COMPLAINT_ADD_CONDITION = 1;
        public const int COMPLAINT_EDIT_CONDITION = 2;

        public const int MISSION_VIEW_CONDITION = 0;
        public const int MISSION_ADD_CONDITION = 1;
        public const int MISSION_EDIT_CONDITION = 2;

        public const int COMPANY_VIEW_CONDITION = 0;
        public const int COMPANY_ADD_CONDITION = 1;
        public const int COMPANY_EDIT_CONDITION = 2;

        public const int EMF_VIEW_CONDITION = 0;
        public const int EMF_ADD_CONDITION = 1;
        public const int EMF_EDIT_CONDITION = 2;


        public const int EMF_PLAN_VIEW_CONDITION = 0;
        public const int EMF_PLAN_ADD_CONDITION = 1;
        public const int EMF_PLAN_EDIT_CONDITION = 2;

        public const int REPEATER_VIEW_CONDITION = 0;
        public const int REPEATER_ADD_CONDITION = 1;
        public const int REPEATER_EDIT_CONDITION = 2;

        public const int BAND_700 = 1;
        public const int BAND_900 = 2;
        public const int BAND_1800 = 3;
        public const int BAND_2100 = 4;
        public const int BAND_2500 = 5;

        public const int MANAGER_POSITION = 1;
        public const int TEAM_LEAD_POSITION = 2;
        public const int SENIOR_POSITION = 3;
        public const int JUNIOR_POSITION = 4;

        public const int MISSION_PENDING_NTRA_ACTION_STATUS = 1;
        public const int MISSION_PENDING_OPERATOR_ACTION_STATUS = 2;
        public const int MISSION_CLOSED_STATUS = 3;

        public const int OPEN_SITE_STATUS = 1;
        public const int PENDING_SITE_STATUS = 2;
        public const int CLOSED_SITE_STATUS = 3;

        public const int OPEN_COMPLAINT_STATUS = 1;
        public const int PENDING_COMPLAINT_STATUS = 2;
        public const int CLOSED_COMPLAINT_STATUS = 3;

        public const int OPEN_EMF_POINT_STATUS = 1;
        public const int CLOSED_EMF_POINT_STATUS = 2;

        public const int OPEN_EMF_PLAN_STATUS = 1;
        public const int PENDING_EMF_PLAN_STATUS = 2;
        public const int CLOSED_EMF_PLAN_STATUS = 3;
        public const int OUTDATED_EMF_PLAN_STATUS = 4;


        public const int PENDING_REPEATER_STATUS = 1;
        public const int REMOVED_REPEATER_STATUS = 2;



        public const int MOBILE_OPERATOR = 1;

        public const String FOLDER_SHARE_PATH = @"\\GIZA-ASAMEH\Giza Software\Attachments\";
        public struct SQL_COLUMN_COUNT_STRUCT
        {
            public int sql_int;
            public int sql_tinyint;
            public int sql_smallint;
            public int sql_bigint;
            public int sql_money;
            public int sql_decimal;
            public int sql_datetime;
            public int sql_time;
            public int sql_string;
            public int sql_bit;

        };

        public struct SQL_ROW_STRUCT
        {
            public List<Int32> sql_int;
            public List<Byte> sql_tinyint;
            public List<Int16> sql_smallint;
            public List<Int64> sql_bigint;
            public List<Decimal> sql_money;
            public List<Decimal> sql_decimal;
            public List<DateTime> sql_datetime;
            public List<TimeSpan> sql_time;
            public List<String> sql_string;
            public List<Boolean> sql_bit;
        };

        public struct KEY_VALUE_PAIR_STRUCT
        {
            public int key;
            public String value;
        };

        public struct FREQUENCY_RANGE_STRUCT
        {
            public int min;
            public int max;
        };


        public struct INTERFERENCE_MISSION_STRUCT
        {
            public int company_serial;
            public int complaint_serial;
            public int mission_serial;
            public int vehicle_id;
            public int status_id;
            public int added_by_id;

            public List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> engineers;
            public List<String> equipment;
            public List<MISSION_SITE_STRUCT> sites;

            public String ticket_number;
            public String mission_comment;
            public String mission_id;
            public String vehichle;
            public String status;
            public String added_by;

            public List<REPEATER_STRUCT> repeaters;

            public DateTime mission_Date;
            public DateTime end_date;
        };

        public struct EMPLOYEE_MIN_STRUCT
        {
            public int employee_id;
            public String username;
            public int department_id;
            public String department;
            public int title_id;
            public String title;
        };

        public struct CONTACT_STRUCT
        {
            public int contact_id;

            public String contact_name;
            public String contact_phone;
            public String contact_email;

            public String contact_added_by;
            public int contact_added_by_id;
        };

        public struct COMPANY_STRUCT
        {
            public int company_serial;
            public String company_name;

            public int company_field_of_work_id;
            public String company_field_of_work;

            public int added_by_id;
            public String added_by;

            public List<COMPANY_START_AND_STOP_FREQUENCY_STRUCT> start_and_stop_frequencies;
            public List<COMPANY_CENTRE_FREQUENCY_BW_STRUCT> cf_and_bw_frequencies;

            public List<CONTACT_STRUCT> contacts;

            public void InitializeContactStruct()
            {
                contacts = new List<CONTACT_STRUCT>();
            }
        };

        public struct COMPLAINT_STRUCT
        {
            public int complaint_serial;
            public String complaint_id;

            public int company_serial;
            public String company_name;

            public String ticket_number;

            public int added_by_id;
            public String added_by;

            public int complaint_status_id;
            public String complaint_status;

            public DateTime complaint_date;

            public List<SITE_STRUCT> sites;

        };

        public struct SITE_STRUCT
        {
            public int complaint_serial;
            public int site_serial;

            public int city_id;
            public String city;

            public String region;
            public String latitude;
            public String longitude;
            public String site_number;

            public int shared_with_id;
            public String shared_with;

            public int site_status_id;
            public String site_status;

            public List<SECTOR_STRUCT> sectors;

        };

        public struct MIN_SITE_STRUCT
        {
            public int company_serial;
            public String company_name;
            public int site_serial;

            public int city_id;
            public String city;

            public String region;

            public String latitude;
            public String longitude;
            public String site_number;

            public int added_by_id;
            public String added_by;

        };

        public struct SECTOR_STRUCT
        {
            public int complaint_serial;
            public int site_serial;
            public int sector_serial;

            public String sector_number;
            public String rru;

            public int shared_with_id;
            public String shared_with;

            public List<String> sector_bands;
        };

        public struct MISSION_SITE_STRUCT
        {
            public String site_name;
            public int site_serial;

            public int reason_of_interference_id;
            public String reason_of_interference;

            public String comment;
        };

        public struct MISSION_EQUIPMENT_STRUCT
        {
            public int serial;
            public String type;
            public String equipment;
        };

        public struct COMPANY_START_AND_STOP_FREQUENCY_STRUCT
        {
            public int serial;
            public Decimal start_frequency;
            public int start_frequency_unit_id;
            public String start_frequency_unit;
            public Decimal stop_frequency;
            public int stop_frequency_unit_id;
            public String stop_frequency_unit;
            public long start_factor;
            public long stop_factor;
            public Decimal max;
            public Decimal min;
        };

        public struct COMPANY_CENTRE_FREQUENCY_BW_STRUCT
        {
            public int serial;
            public Decimal centre_frequency;
            public int centre_frequency_unit_id;
            public String centre_frequency_unit;
            public Decimal bandwidth;
            public int bandwidth_unit_id;
            public String bandwidth_unit;
            public long cf_factor;
            public long bw_factor;
            public Decimal max;
            public Decimal min;
        };

        public struct BAND_UNIT_STRUCT
        {
            public int serial;
            public long factor;        
            public String unit;        
        };

        public struct EMF_STRUCT
        {
            public int emf_serial;

            public int emf_status_id;
            public String emf_status;

            public String name;
            public String area;
            public String district;
            public String latitude;
            public String longitude;
            public String actual_latitude;
            public String actual_longitude;

            public DateTime date;

            public List<EMF_BAND_STRUCT> bands;
        };

        public struct EMF_PLAN_STRUCT
        {
            public String plan_name;
            public int plan_serial;

            public List<EMF_STRUCT> points;

            public int assigned_engineer_id;
            public String assigned_engineer;

            public int status_id;
            public String status;

            public DateTime deadline_date;
        };

        public struct EMF_BAND_STRUCT
        {
            public int emf_point_serial;
            public int band_serial;
            public int band;
            public String band_name;
            public double average_power_density;
            public double max_power_density;

        };

        public struct REPEATER_STRUCT
        {
            public int repeater_serial;
            public int added_by_id;
            public int status_id;
            public int city_id;
            public String latitude;
            public String longitude;
            public String address;
            public String status;
            public String comment;
            public String added_by;
            public String city;
            public String area;
            public DateTime date;
            public DateTime date_added;
        };


    }
}
