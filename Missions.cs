using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntra_missions
{
    public class Missions
    {
        private SQLServer sqlDatabase;
        
        public Complaints complaint;

        private int serial;

        private List<BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT> equipment;

        private int vehicle_id;
        private String vehicle;

        private int missionStatusId;
        private String missionStatus;

        private String missionId;

        private int addedById;
        private String addedBy;

        private DateTime mission_date;
        private DateTime mission_end_date;

        private String missionComment;

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> engineers;

        private List<BASIC_STRUCTS.MISSION_SITE_STRUCT> sites;

        
        public Missions()
        {
            sqlDatabase = new SQLServer();
            complaint = new Complaints();

            engineers = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            sites = new List<BASIC_STRUCTS.MISSION_SITE_STRUCT>();
            equipment = new List<BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT>();
        }

        public bool InitializeMission(int mCompanySerial, int mComplaintSerial, int mSerial)
        {
            serial = mSerial;
          
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"with get_added_by as (select employee_id, name from NTRA.dbo.employee_info)

                              select interference_missions.company_serial,
                                     interference_missions.complaint_serial,
		                             interference_missions.vehichle,
		                             interference_missions.status,
		                             interference_missions.added_by as added_by_id,
		                             mission_engineers.engineer as engineer_id,
		                             mission_equipment.serial as equipment_serial,
		                             mission_sites.site_serial as site_serial,
		                             mission_sites.reason_of_interference,
                                     
		                             interference_missions.mission_date,
		                             interference_missions.end_date,

		                             interference_missions.mission_id,
		                             interference_missions.comment,
		                             vehicles.vehicle_name,
		                             mission_status.status,
		                             get_added_by.name as added_by,
		                             employee_info.name as engineer,
		                             mission_equipment.type,
		                             mission_equipment.equipment,
		                             reasons_of_interference.reason,
		                             company_sites.site_number,
		                             mission_sites.comment as site_comment


                        from NTRA.dbo.interference_missions
                        
                        left join NTRA.dbo.mission_status
                        on interference_missions.status = mission_status.id
                        
                        left join NTRA.dbo.vehicles
                        on interference_missions.vehichle = vehicles.id
                        
                        left join get_added_by
                        on interference_missions.added_by = get_added_by.employee_id
                        
                        left join NTRA.dbo.mission_engineers
                        on interference_missions.company_serial = mission_engineers.company_serial
                        and interference_missions.complaint_serial = mission_engineers.complaint_serial
                        and interference_missions.serial = mission_engineers.mission_serial
                        
                        left join NTRA.dbo.employee_info 
                        on mission_engineers.engineer = employee_info.employee_id
                        
                        left join NTRA.dbo.mission_equipment
                        on interference_missions.company_serial = mission_equipment.company_serial
                        and interference_missions.complaint_serial = mission_equipment.complaint_serial
                        and interference_missions.serial = mission_equipment.mission_serial 
                        
                        left join NTRA.dbo.mission_sites
                        on interference_missions.company_serial = mission_sites.company_serial
                        and interference_missions.complaint_serial = mission_sites.complaint_serial
                        and interference_missions.serial = mission_sites.mission_serial
                        
                        left join NTRA.dbo.company_sites
                        on mission_sites.company_serial =  company_sites.company_serial
                        and mission_sites.site_serial = company_sites.serial
                        
                        left join NTRA.dbo.reasons_of_interference
                        on mission_sites.reason_of_interference = reasons_of_interference.id
                        
                        where interference_missions.company_serial = ";

            String sqlQueryPart2 = " and interference_missions.complaint_serial = ";
            String sqlQueryPart3 = " and interference_missions.serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += mCompanySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += mComplaintSerial;
            sqlQuery += sqlQueryPart3;
            sqlQuery += mSerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 9;
            SQL_COLUMN_COUNT_STRUCT.sql_datetime = 2;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 11;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                if (!complaint.InitializeComplaint(sqlDatabase.rows[0].sql_int[0], sqlDatabase.rows[0].sql_int[1]))
                    return false;

                vehicle_id = sqlDatabase.rows[0].sql_int[2];
                missionStatusId = sqlDatabase.rows[0].sql_int[3];
                addedById = sqlDatabase.rows[0].sql_int[4];
                
                mission_date = sqlDatabase.rows[0].sql_datetime[0];
                mission_end_date = sqlDatabase.rows[0].sql_datetime[1];

                missionId = sqlDatabase.rows[0].sql_string[0];
                missionComment = sqlDatabase.rows[0].sql_string[1];
                vehicle = sqlDatabase.rows[0].sql_string[2];
                missionStatus = sqlDatabase.rows[0].sql_string[3];
                addedBy = sqlDatabase.rows[0].sql_string[4];
                
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT tempEngineers = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT tempEquipment = new BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT();
                    BASIC_STRUCTS.MISSION_SITE_STRUCT tempSite = new BASIC_STRUCTS.MISSION_SITE_STRUCT();

                    tempEngineers.key = sqlDatabase.rows[i].sql_int[5];
                    tempEquipment.serial = sqlDatabase.rows[i].sql_int[6];
                    tempSite.site_serial = sqlDatabase.rows[i].sql_int[7];
                    tempSite.reason_of_interference_id = sqlDatabase.rows[i].sql_int[8];

                    tempEngineers.value = sqlDatabase.rows[i].sql_string[5];
                    tempEquipment.type = sqlDatabase.rows[i].sql_string[6];
                    tempEquipment.equipment = sqlDatabase.rows[i].sql_string[7];
                    tempSite.reason_of_interference = sqlDatabase.rows[i].sql_string[8];
                    tempSite.site_name = sqlDatabase.rows[i].sql_string[9];
                    tempSite.comment = sqlDatabase.rows[i].sql_string[10];

                    if(i == 0 || !engineers.Exists(x1 => x1.key == tempEngineers.key))
                    {
                        engineers.Add(tempEngineers);
                    }

                    if (i == 0 || !equipment.Exists(x1 => x1.serial == tempEquipment.serial))
                    {
                        equipment.Add(tempEquipment);
                    }

                    if (i == 0 || !sites.Exists(x1 => x1.site_serial == tempSite.site_serial))
                    {
                        sites.Add(tempSite);
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool InsertIntoInterferenceMissions()
        {
            String sqlQuery = "insert into NTRA.dbo.interference_missions values(";
            sqlQuery += complaint.GetCompanySerial() + ",";
            sqlQuery += complaint.GetSerial() + ",";
            sqlQuery += serial + ",";
            sqlQuery += vehicle_id + ",";
            sqlQuery += missionStatusId + ",'";
            sqlQuery += mission_date + "',N'";
            sqlQuery += missionComment + "',getdate(),";
            sqlQuery += addedById + ",'";
            sqlQuery += missionId + "','";
            sqlQuery += mission_end_date + "');";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool InsertIntoMissionEngineers()
        {
            for (int i = 0; i < engineers.Count; i++)
            {
                String sqlQuery = "insert into NTRA.dbo.mission_engineers values(";
                sqlQuery += complaint.GetCompanySerial() + ",";
                sqlQuery += complaint.GetSerial() + ",";
                sqlQuery += serial + ",";
                sqlQuery += engineers[i].key + ");";

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }

            return true;
        }

        public bool InsertIntoMissionEquipment()
        {
            for (int i = 0; i < equipment.Count; i++)
            {
                String sqlQuery = "insert into NTRA.dbo.mission_equipment values(";
                sqlQuery += complaint.GetCompanySerial() + ",";
                sqlQuery += complaint.GetSerial() + ",";
                sqlQuery += serial + ",";
                sqlQuery += (i + 1) + ",'";
                sqlQuery += equipment[i].type + "','";
                sqlQuery += equipment[i].equipment + "');";

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }

            return true;
        }

        public bool InsertIntoMissionSites()
        {
            for (int i = 0; i < sites.Count; i++)
            {
                String sqlQuery = "insert into NTRA.dbo.mission_sites values(";
                sqlQuery += complaint.GetCompanySerial() + ",";
                sqlQuery += complaint.GetSerial() + ",";
                sqlQuery += sites[i].site_serial + ",";
                sqlQuery += serial + ",";
                sqlQuery += sites[i].reason_of_interference_id + ",N'";
                sqlQuery += sites[i].comment + "',getdate());";

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }

            return true;
        }

        public bool UpdateMission()
        {
            String sqlQuery = "update NTRA.dbo.interference_missions set vehichle = ";
            sqlQuery += vehicle_id + ", status = ";
            sqlQuery += missionStatusId + ", mission_date = ";
            sqlQuery += "'" + mission_date + "', end_date = ";
            sqlQuery += "'" + mission_end_date + "', comment = ";
            sqlQuery += "N'" + missionComment + "' where company_serial = ";
            sqlQuery += complaint.GetCompanySerial() + " and complaint_serial = ";
            sqlQuery += complaint.GetSerial() + " and serial = ";
            sqlQuery += serial + ";";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool DeleteEngineers()
        {
            String sqlQuery = "Delete from NTRA.dbo.mission_engineers where company_serial = ";
            sqlQuery += complaint.GetCompanySerial() + " and complaint_serial = ";
            sqlQuery += complaint.GetSerial() + " and mission_serial = ";
            sqlQuery += serial + ";";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool DeleteEquipment()
        {
            String sqlQuery = "Delete from NTRA.dbo.mission_engineers where company_serial = ";
            sqlQuery += complaint.GetCompanySerial() + " and complaint_serial = ";
            sqlQuery += complaint.GetSerial() + " and mission_serial = ";
            sqlQuery += serial + ";";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool DeleteSites()
        {
            for (int i = 0; i < sites.Count(); i++)
            {

                String sqlQuery = "Delete from NTRA.dbo.mission_sites where company_serial = ";
                sqlQuery += complaint.GetCompanySerial() + " and complaint_serial = ";
                sqlQuery += complaint.GetSerial() + " and mission_serial = ";
                sqlQuery += serial + ";";

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }

            return true;
        }

        //public bool DeleteMission()
        //{
        //    String sqlQuery = "delete from NTRA.dbo.missions where id = ";
        //    sqlQuery += id;

        //    if (!sqlDatabase.InsertRows(sqlQuery))
        //        return false;

        //    return true;
        //}
        public bool GetNewSerial()
        {

            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select max(serial) from NTRA.dbo.interference_missions where company_serial = ";
            String sqlQueryPart2 = @" and complaint_serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += complaint.GetCompanySerial();
            sqlQuery += sqlQueryPart2;
            sqlQuery += complaint.GetSerial();

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                serial = sqlDatabase.rows[0].sql_int[0] + 1;
            }

            if (serial == 0)
                serial = 1;

            return true;
        }

        public bool GenerateNewMissionId()
        {
            missionId = string.Empty;

            missionId = complaint.GetComplaintId() + "-" + "MISSION-";

            for(int i = 3; i > serial.ToString().Length; i--)
            {
                missionId += "0";
            }

            missionId += serial;

            return true;
        }

        public void SetSerial(int mSerial)
        {
            serial = mSerial;
        }

        public void SetAddedBy(int mId, String mName)
        {
            addedById = mId;
            addedBy = mName;
        }

        public void SetEquipment(ref List<BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT> mEquipment)
        {
            equipment.Clear();

            for(int i = 0; i < mEquipment.Count; i++)
            {
                equipment.Add(mEquipment[i]);
            }
        }


        public void SetVehicleId(int mId)
        {
            vehicle_id = mId;
        }

        public void SetVehicle(String mVehicle)
        {
            vehicle = mVehicle;
        }

        public void SetStatusId(int mId)
        {
            missionStatusId = mId;
        }

        public void SetStatus(String mStatus)
        {
            missionStatus = mStatus;
        }

        public void SetDate(DateTime mDate)
        {
            mission_date = mDate;
        }

        public void SetEndDate(DateTime mDate)
        {
            mission_end_date = mDate;
        }

        public void SetComment(String mComment)
        {
            missionComment = mComment;
        }

        public void SetEngineers(List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mEngineers)
        {
            engineers.Clear();

            for(int i = 0; i < mEngineers.Count; i++)
            {
                engineers.Add(mEngineers[i]);
            }
        }

        public void SetSites(List<BASIC_STRUCTS.MISSION_SITE_STRUCT> mSites)
        {
            sites.Clear();

            for (int i = 0; i < mSites.Count; i++)
            {
                sites.Add(mSites[i]);
            }
        }

        public int GetSerial()
        {
            return serial;
        }

        
        public List<BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT> GetEquipment()
        {
            return equipment;
        }

        public String GetMissionId()
        {
            return missionId;
        }

        public void GetEquipment(ref List<BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT> mEquipment)
        {
            mEquipment.Clear();

            for(int i = 0; i < equipment.Count; i++)
            {
                mEquipment.Add(equipment[i]);
            }
        }

        public int GetVehicleId()
        {
            return vehicle_id;
        }

        public String GetVehicle()
        {
            return vehicle;
        }

        public int GetStatusId()
        {
            return missionStatusId;
        }

        public String GetStatus()
        {
            return missionStatus;
        }

        public DateTime GetDate()
        {
            return mission_date;
        }

        public DateTime GetEndDate()
        {
            return mission_end_date;
        }

        public String GetComment()
        {
            return missionComment;
        }

        public int GetAddedById()
        {
            return addedById;
        }

        public String GetAddedBy()
        {
            return addedBy;
        }

        public void GetEngineers(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mEngineers)
        {
            mEngineers.Clear();

            for (int i = 0; i < engineers.Count; i++)
            {
                mEngineers.Add(engineers[i]);
            }
        }

        public List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> GetEngineers()
        {
            return engineers;
        }

        public void GetSites(ref List<BASIC_STRUCTS.MISSION_SITE_STRUCT> mSites)
        {
            mSites.Clear();

            for (int i = 0; i < sites.Count; i++)
            {
                mSites.Add(sites[i]);
            }
        }

        public List<BASIC_STRUCTS.MISSION_SITE_STRUCT> GetSites()
        {
            return sites;
        }
    }
}
