using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntra_missions
{
    

    public class Complaints
    {
        private SQLServer sql;

        private int serial;
        
        private int companySerial;

        private String complaintId;
        
        private String companyName;
        
        private String ticketNumber;
        
        private DateTime complaintDate;

        private int cityId;
        private String city;

        private String shared;

        private DateTime dateAdded;

        private Employee addedBy;

        private String region;

        private int complaintStatusId; 
        private String complaintStatus; 

        public List<BASIC_STRUCTS.SITE_STRUCT> sites;

        public Complaints()
        {
            sql = new SQLServer();
            addedBy = new Employee();

            sites = new List<BASIC_STRUCTS.SITE_STRUCT>();
            
        }

        public bool InitializeAddedBy(int mId)
        {
            if (!addedBy.InitializeEmployeeInfo(mId))
                return false;

            return true;
        }

        public bool InitializeComplaint(int mCompanySerial, int mComplaintSerial)
        {
            companySerial = mCompanySerial;
            serial = mComplaintSerial;

            String sqlQuery = string.Empty;
            
            String sqlQueryPart1 = @"with get_shared_with as (select serial , name from NTRA.dbo.companies)

                                     select complaints.serial,
                                            complaint_sites.site_serial,
	                                        complaint_site_sector_bands.sector_serial,
	                                        complaint_site_sector_bands.band_id,
                                            complaints.company_serial,
	                                        company_sites.city,
	                                        complaints.added_by,
											complaints.complaint_status as complaint_status_id,
											complaint_sites.site_status as site_status_id,
											complaint_site_sectors.shared_with,
                                            complaint_site_sectors.azimuth,
                                            complaint_site_sectors.status as sector_status_id,

                                            complaint_site_sectors.rtwp,
                                            
	                                        complaints.complaint_date,
	                                        
	                                        complaints.ticket_number,
	                                        company_sites.region,
	                                        company_sites.lat,
	                                        company_sites.long,
	                                        company_sites.site_number,
	                                        complaint_site_sectors.rru_position,
	                                        get_shared_with.name as shared_with_name,
	                                        complaint_site_sectors.sector_number,
	                                        complaint_site_sector_bands.band,
	                                        cities.city,
	                                        companies.name,
											employee_info.name,
                                            complaints.complaint_id,
											complaint_status.status as complaint_Status,
											site_status.status as site_status,
											sector_status.status as sector_status
            
            
                                     from NTRA.dbo.complaints
                                     left join NTRA.dbo.complaint_sites
									 on complaints.company_serial = complaint_sites.company_serial
                                     and complaints.serial = complaint_sites.complaint_serial
                                     left join NTRA.dbo.complaint_site_sectors
									 on complaint_sites.company_serial = complaint_site_sectors.company_serial
                                     and complaint_sites.complaint_serial = complaint_site_sectors.complaint_serial
                                     and complaint_sites.site_serial = complaint_site_sectors.site_serial
                                     left join NTRA.dbo.complaint_site_sector_bands
									 on complaint_site_sectors.company_serial = complaint_site_sector_bands.company_serial
                                     and complaint_site_sectors.complaint_serial = complaint_site_sector_bands.complaint_serial
                                     and complaint_site_sectors.site_serial = complaint_site_sector_bands.site_serial
                                     and complaint_site_sectors.sector_serial = complaint_site_sector_bands.sector_serial
									 left join NTRA.dbo.company_sites
									 on complaint_sites.company_serial = company_sites.company_serial
									 and complaint_sites.site_serial = company_sites.serial
                                     left join NTRA.dbo.cities
                                     on company_sites.city = cities.id
                                     left join NTRA.dbo.companies
                                     on complaints.company_serial = companies.serial
									 left join NTRA.dbo.employee_info
									 on complaints.added_by = employee_info.employee_id
                                     left join NTRA.dbo.complaint_status
									 on complaints.complaint_status = complaint_status.id
									 left join NTRA.dbo.site_status
									 on complaint_sites.site_status = site_status.id
									 left join get_shared_with
									 on complaint_site_sectors.shared_with = get_shared_with.serial
									 left join NTRA.dbo.sector_status
									 on complaint_site_sectors.status = sector_status.id
                                     where complaints.company_serial = ";

            String sqlQUeryPart2 = " and complaints.serial = ";
            
            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial;
            sqlQuery += sqlQUeryPart2;
            sqlQuery += serial;
            
            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 12;
            SQL_COLUMN_COUNT_STRUCT.sql_money = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_datetime = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 16;
            
            if (!sql.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;
            
            sites.Clear();
            
            if (sql.rows.Count != 0)
            {
                serial = sql.rows[0].sql_int[0];
                companySerial = sql.rows[0].sql_int[4];
            
                if (!addedBy.InitializeEmployeeInfo(sql.rows[0].sql_int[6]))
                    return false;
            
                complaintStatusId = sql.rows[0].sql_int[7];
                complaintStatus = sql.rows[0].sql_string[13];
            
                complaintDate = sql.rows[0].sql_datetime[0];
            
                ticketNumber = sql.rows[0].sql_string[0];
            
                companyName = sql.rows[0].sql_string[10];

                complaintId = sql.rows[0].sql_string[12];
            
                for (int i = 0; i < sql.rows.Count; i++)
                {
                    BASIC_STRUCTS.SITE_STRUCT tempSite = new BASIC_STRUCTS.SITE_STRUCT();
                    tempSite.sectors = new List<BASIC_STRUCTS.SECTOR_STRUCT>();
                    BASIC_STRUCTS.SECTOR_STRUCT tempSector = new BASIC_STRUCTS.SECTOR_STRUCT();
                    tempSector.sector_bands = new List<string>();
                
                    tempSite.complaint_serial = sql.rows[i].sql_int[0];
                    tempSite.site_serial = sql.rows[i].sql_int[1];
                    tempSector.site_serial = sql.rows[i].sql_int[1];
                    tempSector.sector_serial = sql.rows[i].sql_int[2];
                    tempSite.city_id = sql.rows[i].sql_int[5];
                    tempSite.site_status_id = sql.rows[i].sql_int[8];
                    tempSector.shared_with_id = sql.rows[i].sql_int[9];
                    tempSector.azimuth = sql.rows[i].sql_int[10];
                    tempSector.sector_status_id = sql.rows[i].sql_int[11];

                    tempSector.rtwp = sql.rows[i].sql_money[0];


                    tempSite.region = sql.rows[i].sql_string[1];
                    tempSite.latitude = sql.rows[i].sql_string[2];
                    tempSite.longitude = sql.rows[i].sql_string[3];
                    tempSite.site_number = sql.rows[i].sql_string[4];
                    tempSector.rru = sql.rows[i].sql_string[5];
                    tempSector.shared_with = sql.rows[i].sql_string[6];
                
                    tempSector.sector_number = sql.rows[i].sql_string[7];
                    tempSector.sector_status = sql.rows[i].sql_string[15];

                    if(sql.rows[i].sql_string[8] != "")
                        tempSector.sector_bands.Add(sql.rows[i].sql_string[8]);


                    tempSite.city = sql.rows[i].sql_string[9];

                    tempSite.site_status = sql.rows[i].sql_string[14];
                    //if(tempSector.sector_number != "")
                        tempSite.sectors.Add(tempSector);
                
                    if (i != 0 && sites.Last().site_serial == tempSite.site_serial)
                    {
                        if (sites.Last().sectors.Last().sector_serial == tempSector.sector_serial)
                        {
                            if (sql.rows[i].sql_string[8] != "")
                                sites.Last().sectors.Last().sector_bands.Add(sql.rows[i].sql_string[8]);
                        }
                        else
                        {
                            //if (tempSector.sector_number != "")
                            sites.Last().sectors.Add(tempSector);
                        }
                
                    }
                    else
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

        public bool IssueNewComplaint(int mFieldOfWorkId)
        {
            if (!GetNewComplaintSerial())
                return false;

            GetNewComplaintId();

            if (ticketNumber == "0")
                ticketNumber = serial.ToString();

            if (!InsertIntoComplaints())
                return false;

            if (!InsertIntoComplaintSites())
                return false;

            if (mFieldOfWorkId == BASIC_STRUCTS.MOBILE_OPERATOR)
            {
                if (!InsertIntoSiteSectors())
                    return false;

                if (!InsertIntoSectorBands())
                    return false;
            }

            return true;

        }

        public bool EditComplaint(int mFieldOfWorkId)
        {
            //if (mFieldOfWorkId == BASIC_STRUCTS.MOBILE_OPERATOR)
            //{
            //    if (!DeleteSectorBands())
            //        return false;
            //
            //    if (!DeleteSiteSectors())
            //        return false;
            //}
            //
            //if (!DeleteComplaintSites())
            //    return false;


            complaintId = companyName + "-";

            complaintId += complaintDate.Year.ToString()[2];
            complaintId += complaintDate.Year.ToString()[3];
            complaintId += "-";

            for (int i = 3; i > serial.ToString().Length; i--)
            {
                complaintId += "0";
            }

            complaintId += serial;


            if (!UpdateComplaint())
                return false;

            //if (!InsertIntoComplaintSites())
            //    return false;
            //
            //
            //if (mFieldOfWorkId == BASIC_STRUCTS.MOBILE_OPERATOR)
            //{
            //    if (!InsertIntoSiteSectors())
            //        return false;
            //
            //    if (!InsertIntoSectorBands())
            //        return false;
            //}

            return true;

        }

        public bool GetNewComplaintSerial()
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select max(serial) from NTRA.dbo.complaints where company_serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sql.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sql.rows.Count != 0)
            {
                serial = sql.rows[0].sql_int[0] + 1;
            }

            if (serial == 0)
                serial = 1;

            return true;

        }

        public bool GetMaxSerialForCompanyInYear(ref int mSerial)
        {
            DateTime now = DateTime.Now; 

            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select max(serial) from NTRA.dbo.complaints where company_serial = ";
            String sqlQueryPart2 = @" and complaint_date like '%";
            String sqlQueryPart3 = @"%'";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += complaintDate.Year;
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sql.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sql.rows.Count != 0)
            {
                mSerial = sql.rows[0].sql_int[0] + 1;
            }

            if (mSerial == 0)
                mSerial = 1;

            return true;
        }

        public void GetNewComplaintId()
        {
            complaintId = companyName + "-";

            complaintId += complaintDate.Year.ToString()[2];
            complaintId += complaintDate.Year.ToString()[3];
            complaintId += "-";

            int mSerial = 0;

            if (!GetMaxSerialForCompanyInYear(ref mSerial))
                return;

            for (int i = 3; i > mSerial.ToString().Length; i--)
            {
                complaintId += "0";
            }

            complaintId += mSerial;
        }

        public bool InsertIntoComplaints()
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"insert into NTRA.dbo.complaints values(";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial + ",";
            sqlQuery += serial + ",'";
            sqlQuery += ticketNumber + "','";
            sqlQuery += complaintId + "',";
            sqlQuery += complaintStatusId + ",";
            sqlQuery += addedBy.GetEmployeeId() + ",'";
            sqlQuery += complaintDate + "',getdate());";

            if (!sql.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool InsertIntoComplaintSites()
        {
            for (int i = 0; i < sites.Count; i++)
            {

                String sqlQuery = string.Empty;

                String sqlQueryPart1 = @"insert into NTRA.dbo.complaint_sites values(";

                sqlQuery = sqlQueryPart1;
                sqlQuery += companySerial + ",";
                sqlQuery += serial + ",";
                sqlQuery += sites[i].site_serial + ",";
                sqlQuery += sites[i].site_status_id + ");";

                if (!sql.InsertRows(sqlQuery))
                    return false;
            }

            return true;
        }

        public bool InsertIntoSiteSectors()
        {
            for (int i = 0; i < sites.Count; i++)
            {
                for (int j = 0; j < sites[i].sectors.Count; j++)
                {
                    String sqlQuery = string.Empty;

                    String sqlQueryPart1 = @"insert into NTRA.dbo.complaint_site_sectors values(";

                    sqlQuery = sqlQueryPart1;
                    sqlQuery += companySerial + ",";
                    sqlQuery += serial + ",";
                    sqlQuery += sites[i].site_serial + ",";
                    sqlQuery += (j + 1) + ",'";
                    sqlQuery += sites[i].sectors[j].sector_number + "','";
                    sqlQuery += sites[i].sectors[j].rru + "',";
                    if (sites[i].sectors[j].shared_with_id != 0)
                        sqlQuery += sites[i].sectors[j].shared_with_id + ",";
                    else
                        sqlQuery += "null,";
                    sqlQuery += sites[i].sectors[j].rtwp + ",";
                    sqlQuery += sites[i].sectors[j].azimuth + ",";
                    sqlQuery += sites[i].sectors[j].sector_status_id + ")";
                    if (!sql.InsertRows(sqlQuery))
                        return false;
                }
            }

            return true;
        }

        public bool InsertIntoSectorBands()
        {
            int bandNumber = 1;

            for (int i = 0; i < sites.Count; i++)
            {
                for (int j = 0; j < sites[i].sectors.Count; j++)
                {
                    for (int k = 0; k < sites[i].sectors[j].sector_bands.Count; k++)
                    {
                        String sqlQuery = string.Empty;

                        String sqlQueryPart1 = @"insert into NTRA.dbo.complaint_site_sector_bands values(";

                        sqlQuery = sqlQueryPart1;
                        sqlQuery += companySerial + ",";
                        sqlQuery += serial + ",";
                        sqlQuery += sites[i].site_serial + ",";
                        sqlQuery += (j + 1) + ",";
                        sqlQuery += (k + 1) + ",'";
                        sqlQuery += sites[i].sectors[j].sector_bands[k] + "');";

                        if (!sql.InsertRows(sqlQuery))
                            return false;

                    }
                }
            }

            return true;
        }

        public bool UpdateComplaint()
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"update NTRA.dbo.complaints set ticket_number = '";
            String sqlQueryPart2 = @" where company_serial =  ";
            String sqlQueryPart3 = @" and serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += ticketNumber + "', complaint_status = ";
            sqlQuery += complaintStatusId + ", added_by = ";
            sqlQuery += addedBy.GetEmployeeId() + ", complaint_date = '";
            sqlQuery += complaintDate + "', complaint_id =  '";
            sqlQuery += complaintId + "' ";
            sqlQuery += sqlQueryPart2;
            sqlQuery += companySerial;
            sqlQuery += sqlQueryPart3;
            sqlQuery += serial;

            if (!sql.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool DeleteComplaintSites()
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"delete from NTRA.dbo.complaint_sites where company_serial = ";
            String sqlQueryPart2 = @" and complaint_serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += serial;

            if (!sql.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool DeleteSiteSectors()
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"delete from NTRA.dbo.complaint_site_sectors where company_serial = ";
            String sqlQueryPart2 = @" and complaint_serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += serial;

            if (!sql.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool DeleteSectorBands()
        {

            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"delete from NTRA.dbo.complaint_site_sector_bands where company_serial = ";
            String sqlQueryPart2 = @" and complaint_serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += serial;

            if (!sql.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool ConfirmSiteStatus(int mSiteSerial, int mMissionStatus)
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"update NTRA.dbo.complaint_sites set site_status = ";
            String sqlQueryPart2 = @" where company_serial = ";
            String sqlQueryPart3 = @" and complaint_Serial =   ";
            String sqlQueryPart4 = @" and site_serial = ";

            sqlQuery = sqlQueryPart1;
            if (mMissionStatus == BASIC_STRUCTS.MISSION_PENDING_NTRA_ACTION_STATUS || mMissionStatus == BASIC_STRUCTS.MISSION_PENDING_OPERATOR_ACTION_STATUS)
                sqlQuery += BASIC_STRUCTS.INTERFERED_SITE_STATUS;
            else
                sqlQuery += BASIC_STRUCTS.CLEARED_SITE_STATUS;
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetCompanySerial();
            sqlQuery += sqlQueryPart3;
            sqlQuery += GetSerial();
            sqlQuery += sqlQueryPart4;
            sqlQuery += mSiteSerial;

            if (!sql.InsertRows(sqlQuery))
                return false;

            return true;
        }
        public bool UpdateSectorStatus(int mSiteSerial, int mSectorSerial, int mSectorStatus)
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"update NTRA.dbo.complaint_site_sectors set status = ";
            String sqlQueryPart2 = @" where company_serial = ";
            String sqlQueryPart3 = @" and complaint_Serial =   ";
            String sqlQueryPart4 = @" and site_serial = ";
            String sqlQueryPart5 = @" and sector_serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += mSectorStatus;
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetCompanySerial();
            sqlQuery += sqlQueryPart3;
            sqlQuery += GetSerial();
            sqlQuery += sqlQueryPart4;
            sqlQuery += mSiteSerial;
            sqlQuery += sqlQueryPart5;
            sqlQuery += mSectorSerial;

            if (!sql.InsertRows(sqlQuery))
                return false;

            return true;
        }

        // Updates a sector's status by its (site, sector_number). sector_number
        // is the reliably-loaded, user-visible identifier — unlike sector_serial
        // which is read from the bands table and is 0 for band-less sectors.
        public bool UpdateSectorStatusByNumber(int mSiteSerial, String mSectorNumber, int mSectorStatus)
        {
            String safeNumber = (mSectorNumber ?? "").Replace("'", "''");

            String sqlQuery = "update NTRA.dbo.complaint_site_sectors set status = ";
            sqlQuery += mSectorStatus;
            sqlQuery += " where company_serial = ";
            sqlQuery += GetCompanySerial();
            sqlQuery += " and complaint_serial = ";
            sqlQuery += GetSerial();
            sqlQuery += " and site_serial = ";
            sqlQuery += mSiteSerial;
            sqlQuery += " and sector_number = N'";
            sqlQuery += safeNumber;
            sqlQuery += "'";

            if (!sql.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool UpdateSiteStatus(int mSiteSerial)
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select status
                                     from NTRA.dbo.complaint_site_sectors
                                     where company_serial =";
            String sqlQueryPart2 = @" and complaint_serial = ";
            String sqlQueryPart3 = @" and site_serial = ";
            String sqlQueryPart4 = @" and status = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += GetCompanySerial();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetSerial();
            sqlQuery += sqlQueryPart3;
            sqlQuery += mSiteSerial;
            sqlQuery += sqlQueryPart4;
            sqlQuery += BASIC_STRUCTS.INTERFERED_SECTOR_STATUS;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sql.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sql.rows.Count == 0)
            {
                sqlQuery = String.Empty;

                String sqlQueryPart7 = @"update NTRA.dbo.complaint_sites set site_status = ";
                String sqlQueryPart8 = @" where company_serial = ";
                String sqlQueryPart9 = @" and complaint_serial = ";
                String sqlQueryPart10 = @" and site_serial = ";

                sqlQuery = sqlQueryPart7;
                sqlQuery += BASIC_STRUCTS.CLEARED_SITE_STATUS;
                sqlQuery += sqlQueryPart8;
                sqlQuery += GetCompanySerial();
                sqlQuery += sqlQueryPart9;
                sqlQuery += GetSerial();
                sqlQuery += sqlQueryPart10;
                sqlQuery += mSiteSerial;

                if (!sql.InsertRows(sqlQuery))
                    return false;
            }
            else
            {
                sqlQuery = String.Empty;

                String sqlQueryPart11 = @"update NTRA.dbo.complaint_sites set site_status = ";
                String sqlQueryPart12 = @" where company_serial = ";
                String sqlQueryPart13 = @" and complaint_serial = ";
                String sqlQueryPart14 = @" and site_serial = ";

                sqlQuery = sqlQueryPart11;
                sqlQuery += BASIC_STRUCTS.INTERFERED_SITE_STATUS;
                sqlQuery += sqlQueryPart12;
                sqlQuery += GetCompanySerial();
                sqlQuery += sqlQueryPart13;
                sqlQuery += GetSerial();
                sqlQuery += sqlQueryPart14;
                sqlQuery += mSiteSerial;

                if (!sql.InsertRows(sqlQuery))
                    return false;

            }

            return true;
        }

        public bool CheckSiteStatusConfirmed(int mSiteSerial)
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select complaint_serial from NTRA.dbo.complaint_sites where company_serial = ";
            String sqlQueryPart2 = @" and complaint_Serial =  ";
            String sqlQueryPart3 = @" and site_serial = ";
            String sqlQueryPart4 = @" and site_status = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += GetCompanySerial();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetSerial();
            sqlQuery += sqlQueryPart3;
            sqlQuery += mSiteSerial;
            sqlQuery += sqlQueryPart4;
            sqlQuery += BASIC_STRUCTS.CLEARED_SITE_STATUS;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sql.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sql.rows.Count == 0)
                return false;

            return true;
        }

        public bool UpdateComplaintStatus()
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select site_status
                                     from NTRA.dbo.complaint_sites
                                     where company_serial =";
            String sqlQueryPart2 = @" and complaint_serial = ";
            String sqlQueryPart3 = @" and site_status = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += GetCompanySerial();
            sqlQuery += sqlQueryPart2;
            sqlQuery += GetSerial();
            sqlQuery += sqlQueryPart3;
            sqlQuery += BASIC_STRUCTS.INTERFERED_SITE_STATUS;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sql.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sql.rows.Count == 0)
            {
                sqlQuery = String.Empty;

                String sqlQueryPart7 = @"update NTRA.dbo.complaints set complaint_status = ";
                String sqlQueryPart8 = @"where company_serial = ";
                String sqlQueryPart9 = @"and serial = ";

                sqlQuery = sqlQueryPart7;
                sqlQuery += BASIC_STRUCTS.CLOSED_COMPLAINT_STATUS;
                sqlQuery += sqlQueryPart8;
                sqlQuery += GetCompanySerial();
                sqlQuery += sqlQueryPart9;
                sqlQuery += GetSerial();

                if (!sql.InsertRows(sqlQuery))
                    return false;
            }
            else
            {
                sqlQuery = String.Empty;

                String sqlQueryPart10 = @"update NTRA.dbo.complaints set complaint_status = ";
                String sqlQueryPart11 = @"where company_serial = ";
                String sqlQueryPart12 = @"and serial = ";

                sqlQuery = sqlQueryPart10;
                sqlQuery += BASIC_STRUCTS.PENDING_COMPLAINT_STATUS;
                sqlQuery += sqlQueryPart11;
                sqlQuery += GetCompanySerial();
                sqlQuery += sqlQueryPart12;
                sqlQuery += GetSerial();

                if (!sql.InsertRows(sqlQuery))
                    return false;

            }

            return true;
        }

        // Auto-updates the status of EVERY mission on this complaint based on
        // the current sector states: a mission is CLOSED when all sectors of
        // all the sites it worked on are cleared; a mission that was closed but
        // has a sector interfered again reverts to pending. Mirrors the
        // complaint-status logic and is run after feedback is saved.
        public bool UpdateMissionStatuses()
        {
            // revert wrongly-closed missions that regained an interfered sector
            String revertQuery =
                "update m set status = " + BASIC_STRUCTS.MISSION_PENDING_OPERATOR_ACTION_STATUS + " " +
                "from NTRA.dbo.interference_missions m " +
                "where m.company_serial = " + companySerial + " and m.complaint_serial = " + serial + " " +
                "and m.status = " + BASIC_STRUCTS.MISSION_CLOSED_STATUS + " " +
                "and exists (select 1 from NTRA.dbo.mission_sites ms " +
                "  join NTRA.dbo.complaint_site_sectors css " +
                "    on css.company_serial = ms.company_serial and css.complaint_serial = ms.complaint_serial and css.site_serial = ms.site_serial " +
                "  where ms.company_serial = m.company_serial and ms.complaint_serial = m.complaint_serial and ms.mission_serial = m.serial " +
                "    and css.status = " + BASIC_STRUCTS.INTERFERED_SECTOR_STATUS + ");";

            if (!sql.InsertRows(revertQuery))
                return false;

            // close missions whose worked-on sites all have every sector cleared
            String closeQuery =
                "update m set status = " + BASIC_STRUCTS.MISSION_CLOSED_STATUS + " " +
                "from NTRA.dbo.interference_missions m " +
                "where m.company_serial = " + companySerial + " and m.complaint_serial = " + serial + " " +
                "and exists (select 1 from NTRA.dbo.mission_sites ms " +
                "  join NTRA.dbo.complaint_site_sectors css " +
                "    on css.company_serial = ms.company_serial and css.complaint_serial = ms.complaint_serial and css.site_serial = ms.site_serial " +
                "  where ms.company_serial = m.company_serial and ms.complaint_serial = m.complaint_serial and ms.mission_serial = m.serial) " +
                "and not exists (select 1 from NTRA.dbo.mission_sites ms2 " +
                "  join NTRA.dbo.complaint_site_sectors css2 " +
                "    on css2.company_serial = ms2.company_serial and css2.complaint_serial = ms2.complaint_serial and css2.site_serial = ms2.site_serial " +
                "  where ms2.company_serial = m.company_serial and ms2.complaint_serial = m.complaint_serial and ms2.mission_serial = m.serial " +
                "    and css2.status = " + BASIC_STRUCTS.INTERFERED_SECTOR_STATUS + ");";

            if (!sql.InsertRows(closeQuery))
                return false;

            return true;
        }

        public int GetSerial()
        {
            return serial;
        }

        public int GetCompanySerial()
        {
            return companySerial;
        }

        public String GetCompanyName()
        {
            return companyName;
        }

        public String GetTicketNumber()
        {
            return ticketNumber;
        }

        public int GetCityId()
        {
            return cityId;
        }

        public String GetCity()
        {
            return city;
        }

        public DateTime GetComplaintDate()
        {
            return complaintDate;
        }

        public String GetSharedSite()
        {
            return shared;
        }

        public DateTime GetDateAdded()
        {
            return dateAdded;
        }

        public List<BASIC_STRUCTS.SITE_STRUCT> GetComplaintsSites()
        {
            return sites;
        }

        public void GetComplaintSites(ref List<BASIC_STRUCTS.SITE_STRUCT> mList)
        {
            mList.Clear();

            for(int i = 0; i < sites.Count; i++)
            {
                mList.Add(sites[i]);
            }
        }

        public String GetComplaintId()
        {
            return complaintId;
        }

        public int GetComplaintStatusId()
        {
            return complaintStatusId;
        }

        public String GetComplaintStatus()
        {
            return complaintStatus;
        }

        public String GetRegion()
        {
            return region;
        }


        public void SetSerial(int mSerial)
        {
            serial = mSerial;
        }

        public void SetCompanySerial(int mCompanySerial)
        {
            companySerial = mCompanySerial;
        }

        public void SetCompanyName(String mCompanyName)
        {
            companyName = mCompanyName;
        }

        public void SetTicketNumber(String mTicketNumber)
        {
            ticketNumber = mTicketNumber;
        }

        public void SetCityId(int mId)
        {
            cityId = mId;
        }

        public void SetCity(String mCity)
        {
            city = mCity;
        }

        public void SetComplaintDate(DateTime mDate)
        {
            complaintDate = mDate;
        }

        public void SetSharedSite(String mShared)
        {
            shared = mShared;
        }

        public void SetDateAdded(DateTime mDateAdded)
        {
            dateAdded = mDateAdded;
        }

        public void SetComplaintStatusId(int mId)
        {
            complaintStatusId = mId;
        }

        public void SetComplaintStatus(String mStatus)
        {
            complaintStatus = mStatus;
        }

        public void SetRegion(String mRegion)
        {
            region = mRegion;
        }

        public void SetComplaintSites(ref List<BASIC_STRUCTS.SITE_STRUCT> mList)
        {
            sites.Clear();

            for(int i = 0; i < mList.Count; i++)
            {
                sites.Add(mList[i]);
            }
        }
    }

}
