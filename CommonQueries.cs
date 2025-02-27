using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace ntra_missions
{
    
    public class CommonQueries
    {
        SQLServer sqlDatabase;
        String sqlQuery;

        public CommonQueries()
        {
            sqlDatabase = new SQLServer();
            sqlQuery = String.Empty;
        }

        public bool CheckEmployeeUserName(String mUserName, ref int id)
        {
            String sqlQueryPart1 = "select employee_id from NTRA.dbo.employee_emails where email = ";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;
            sqlQuery += "'" + mUserName + "'";

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT sQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            sQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, sQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
                id = sqlDatabase.rows[0].sql_int[0];
            else
                id = 0;

            return true;
        }

        public bool CheckSiteMissions(int mCompanySerial, int mComplaintSerial,  int mSiteSerial)
        {
            String sqlQueryPart1 = "select complaint_sites.site_serial from NTRA.dbo.complaint_sites where company_serial = ";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;
            sqlQuery += mCompanySerial + " and complaint_serial = ";
            sqlQuery += mComplaintSerial + " and complaint_serial = ";
            sqlQuery += mComplaintSerial + " and site_serial = ";
            sqlQuery += mSiteSerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT sQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            sQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, sQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
                return true;
            else
                return false;

        }

        public bool CheckEmployeePassword(int mId, ref String mPassword)
        {
            String sqlQueryPart1 = "select password from NTRA.dbo.employee_password where employee_id = ";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;
            sqlQuery += mId;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
                mPassword = sqlDatabase.rows[0].sql_string[0];
            else
                mPassword = "";

            return true;
        }

        public bool GetEngineers(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select employee_id, name from NTRA.dbo.employee_info";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }

            }

            return true;
        }

        public bool GetEngineers(ref List<BASIC_STRUCTS.EMPLOYEE_MIN_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = @"select employee_id, employee_info.department, employee_info.title, 
                                            name, departments.department, titles.title  
                                     from NTRA.dbo.employee_info
                                     left join NTRA.dbo.titles
                                     on employee_info.title = titles.id
                                     left join NTRA.dbo.departments
                                     on employee_info.department = departments.id";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 3;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 3;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.EMPLOYEE_MIN_STRUCT temp = new BASIC_STRUCTS.EMPLOYEE_MIN_STRUCT();
                    
                    temp.employee_id = sqlDatabase.rows[i].sql_int[0];
                    temp.department_id = sqlDatabase.rows[i].sql_int[1];
                    temp.title_id = sqlDatabase.rows[i].sql_int[2];

                    temp.username = sqlDatabase.rows[i].sql_string[0];
                    temp.department = sqlDatabase.rows[i].sql_string[1];
                    temp.title = sqlDatabase.rows[i].sql_string[2];

                    mList.Add(temp);
                }

            }

            return true;
        }

        public bool GetServiceProviders(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select serial, name from NTRA.dbo.companies";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for(int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }

            }

            return true;
        }

        public bool GetReasonsOfInterfernce(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, reason from NTRA.dbo.reasons_of_interference";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetEMFBands(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, band from NTRA.dbo.emf_bands";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetActionsTaken(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();
            
            String sqlQueryPart1 = "select id, action from NTRA.dbo.actions_taken";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetBands(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, band from NTRA.dbo.bands";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetBandUnits(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, unit from NTRA.dbo.band_units";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetBandUnits(ref List<BASIC_STRUCTS.BAND_UNIT_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, factor, unit from NTRA.dbo.band_units";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_bigint = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.BAND_UNIT_STRUCT temp = new BASIC_STRUCTS.BAND_UNIT_STRUCT();
                    temp.serial = sqlDatabase.rows[i].sql_int[0];
                    temp.factor = sqlDatabase.rows[i].sql_bigint[0];
                    temp.unit = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetTitles(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, title from NTRA.dbo.titles";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetDepartments(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, department from NTRA.dbo.departments";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetPositions(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select position_id, position from NTRA.dbo.positions";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetCompanyFieldOfWork(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, field_of_work from NTRA.dbo.company_field_of_work";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetHandhelds(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, device_name from NTRA.dbo.handheld";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetCompanyVehicles(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, vehicle_name from NTRA.dbo.vehicles";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetCities(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, city from NTRA.dbo.cities";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetCompanies(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select serial, name from NTRA.dbo.companies";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetMissionTypes(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select type_id, type from NTRA.dbo.mission_types";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetComplaintStatus(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, status from NTRA.dbo.complaint_status";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetSiteStatus(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, status from NTRA.dbo.site_status";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetMissionStatus(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, status from NTRA.dbo.mission_status";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetRepeatersStatus(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, status from NTRA.dbo.repeaters_status";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetEMFPlanStatus(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, status from NTRA.dbo.emf_plan_status";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetAntennas(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, name from NTRA.dbo.antennas";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetCables(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, name from NTRA.dbo.cables";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetEMFPointStatus(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select id, status from NTRA.dbo.emf_point_status";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetCompanyComplaints(int mCompanySerial , ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = "select serial, ticket_number from NTRA.dbo.complaints";
            String sqlQueryPart2 = " where company_serial = ";
            String sqlQueryPart3 = " Order by serial DESC";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;
            sqlQuery += sqlQueryPart2;
            sqlQuery += mCompanySerial;
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetCompanyComplaintSites(int mCompanySerial, int mComplaintSerial,  ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = @"select site_serial, site_number from NTRA.dbo.complaint_sites
                                                         left join NTRA.dbo.company_sites
                                                         on complaint_sites.company_serial = company_sites.company_serial
                                                         and complaint_sites.site_serial = company_sites.serial";
            String sqlQueryPart2 = " where company_sites.company_serial = ";
            String sqlQueryPart3 = " and complaint_sites.complaint_serial = ";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;
            sqlQuery += sqlQueryPart2;
            sqlQuery += mCompanySerial;
            sqlQuery += sqlQueryPart3;
            sqlQuery += mComplaintSerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = sqlDatabase.rows[i].sql_int[0];
                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool GetComplaintNumberOfSites(int mCompanySerial, int mComplaintSerial, ref int mCount)
        {
            String sqlQueryPart1 = @"select count(complaint_sites.site_serial)
                                            from NTRA.dbo.complaints
                                            left join NTRA.dbo.complaint_sites
                                            on complaints.company_serial = complaint_sites.company_serial
                                            and complaints.serial = complaint_sites.complaint_serial
                                            where complaints.company_serial = ";
            String sqlQueryPart2 = "and complaints.serial = ";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;
            sqlQuery += mCompanySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += mComplaintSerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                mCount = sqlDatabase.rows[0].sql_int[0];
            }

            return true;
        }

        public bool GetMissions(ref List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> mList)
        {
            mList.Clear();

            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"with get_added_by as (select employee_id, name from NTRA.dbo.employee_info)

                              select  interference_missions.company_serial,
                                      interference_missions.complaint_serial,
                                      interference_missions.serial,
		                              interference_missions.vehichle,
		                              interference_missions.status,
		                              interference_missions.added_by as added_by_id,
		                              mission_engineers.engineer as engineer_id,
		                              mission_sites.site_serial as site_serial,
		                              mission_sites.reason_of_interference,
                                      
		                              interference_missions.mission_date,
		                              interference_missions.end_date,
                                      
		                              interference_missions.mission_id,
		                              complaints.ticket_number,
		                              interference_missions.comment,
		                              vehicles.vehicle_name,
		                              mission_status.status,
		                              get_added_by.name as added_by,
		                              employee_info.name as engineer,
		                              mission_equipment.equipment,
		                              reasons_of_interference.reason,
		                              company_sites.site_number,
		                              mission_sites.comment as site_comment

             from NTRA.dbo.interference_missions
             
             left join NTRA.dbo.complaints
             on interference_missions.company_serial = complaints.company_serial
             and interference_missions.complaint_serial = complaints.serial
             
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

             
             order by interference_missions.date_added DESC";

            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 9;
            SQL_COLUMN_COUNT_STRUCT.sql_datetime = 2;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 11;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {

                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT tempMission = new BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT();
                    tempMission.equipment = new List<string>();
                    tempMission.sites = new List<BASIC_STRUCTS.MISSION_SITE_STRUCT>();
                    tempMission.engineers = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
                    
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT tempEngineers = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT tempEquipment = new BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT();
                    BASIC_STRUCTS.MISSION_SITE_STRUCT tempSite = new BASIC_STRUCTS.MISSION_SITE_STRUCT();

                    tempMission.mission_Date = sqlDatabase.rows[i].sql_datetime[0];
                    tempMission.end_date = sqlDatabase.rows[i].sql_datetime[1];

                    tempMission.company_serial = sqlDatabase.rows[i].sql_int[0];
                    tempMission.complaint_serial = sqlDatabase.rows[i].sql_int[1];
                    tempMission.mission_serial = sqlDatabase.rows[i].sql_int[2];
                    tempMission.vehicle_id = sqlDatabase.rows[i].sql_int[3];
                    tempMission.status_id = sqlDatabase.rows[i].sql_int[4];
                    tempMission.added_by_id = sqlDatabase.rows[i].sql_int[5];


                    tempEngineers.key = sqlDatabase.rows[i].sql_int[6];
                    tempEngineers.value = sqlDatabase.rows[i].sql_string[6];
                    tempMission.engineers.Add(tempEngineers);

                    tempSite.site_serial = sqlDatabase.rows[i].sql_int[7];
                    tempSite.reason_of_interference_id = sqlDatabase.rows[i].sql_int[8];

                    tempSite.reason_of_interference = sqlDatabase.rows[i].sql_string[8];
                    tempSite.site_name = sqlDatabase.rows[i].sql_string[9];
                    tempSite.comment = sqlDatabase.rows[i].sql_string[10];

                    tempMission.sites.Add(tempSite);

                    tempMission.mission_id = sqlDatabase.rows[i].sql_string[0];
                    tempMission.ticket_number = sqlDatabase.rows[i].sql_string[1];
                    tempMission.mission_comment = sqlDatabase.rows[i].sql_string[2];
                    tempMission.vehichle = sqlDatabase.rows[i].sql_string[3];
                    tempMission.status = sqlDatabase.rows[i].sql_string[4];
                    tempMission.added_by = sqlDatabase.rows[i].sql_string[5];
                    tempMission.equipment.Add(sqlDatabase.rows[i].sql_string[7]);

                    tempEquipment.equipment = sqlDatabase.rows[i].sql_string[7];



                    if (i != 0 && mList.Last().company_serial == tempMission.company_serial && mList.Last().complaint_serial == tempMission.complaint_serial && mList.Last().mission_serial == tempMission.mission_serial)
                    {
                        if (!mList.Last().engineers.Exists(x1 => x1.key == tempEngineers.key))
                        {
                            mList.Last().engineers.Add(tempEngineers);
                        }

                        if (!mList.Last().equipment.Exists(x1 => x1 == tempEquipment.equipment) && tempEquipment.equipment != "")
                        {
                            mList.Last().equipment.Add(tempEquipment.equipment);
                        }

                        if (!mList.Last().sites.Exists(x1 => x1.site_serial == tempSite.site_serial))
                        {
                            mList.Last().sites.Add(tempSite);
                        }

                    }
                    else
                    {
                        mList.Add(tempMission);
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool GetMissions(ref List<BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT> mList, int mCompanySerial, int mSiteSerial)
        {
            mList.Clear();

            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"with get_added_by as (select employee_id, name from NTRA.dbo.employee_info)

                              select  interference_missions.company_serial,
                                      interference_missions.complaint_serial,
                                      interference_missions.serial,
		                              interference_missions.vehichle,
		                              interference_missions.status,
		                              interference_missions.added_by as added_by_id,
		                              mission_engineers.engineer as engineer_id,
		                              mission_sites.site_serial as site_serial,
		                              mission_sites.reason_of_interference,
                                      
		                              interference_missions.mission_date,
                                      
		                              interference_missions.mission_id,
		                              complaints.ticket_number,
		                              interference_missions.comment,
		                              vehicles.vehicle_name,
		                              mission_status.status,
		                              get_added_by.name as added_by,
		                              employee_info.name as engineer,
		                              mission_equipment.equipment,
		                              reasons_of_interference.reason,
		                              company_sites.site_number,
		                              mission_sites.comment as site_comment

             from NTRA.dbo.interference_missions
             
             left join NTRA.dbo.complaints
             on interference_missions.company_serial = complaints.company_serial
             and interference_missions.complaint_serial = complaints.serial
             
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

            String sqlQueryPart2 = " and mission_sites.site_serial = ";

             String sqlQueryPart3 =  " order by interference_missions.date_added DESC";

            sqlQuery = sqlQueryPart1;
            sqlQuery += mCompanySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += mSiteSerial;
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 9;
            SQL_COLUMN_COUNT_STRUCT.sql_datetime = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 11;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {

                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT tempMission = new BASIC_STRUCTS.INTERFERENCE_MISSION_STRUCT();
                    tempMission.equipment = new List<string>();
                    tempMission.sites = new List<BASIC_STRUCTS.MISSION_SITE_STRUCT>();
                    tempMission.engineers = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT tempEngineers = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT tempEquipment = new BASIC_STRUCTS.MISSION_EQUIPMENT_STRUCT();
                    BASIC_STRUCTS.MISSION_SITE_STRUCT tempSite = new BASIC_STRUCTS.MISSION_SITE_STRUCT();

                    tempMission.mission_Date = sqlDatabase.rows[i].sql_datetime[0];

                    tempMission.company_serial = sqlDatabase.rows[i].sql_int[0];
                    tempMission.complaint_serial = sqlDatabase.rows[i].sql_int[1];
                    tempMission.mission_serial = sqlDatabase.rows[i].sql_int[2];
                    tempMission.vehicle_id = sqlDatabase.rows[i].sql_int[3];
                    tempMission.status_id = sqlDatabase.rows[i].sql_int[4];
                    tempMission.added_by_id = sqlDatabase.rows[i].sql_int[5];

                    tempEngineers.key = sqlDatabase.rows[i].sql_int[6];
                    tempEngineers.value = sqlDatabase.rows[i].sql_string[6];
                    tempMission.engineers.Add(tempEngineers);

                    tempSite.site_serial = sqlDatabase.rows[i].sql_int[7];
                    tempSite.reason_of_interference_id = sqlDatabase.rows[i].sql_int[8];

                    tempSite.reason_of_interference = sqlDatabase.rows[i].sql_string[8];
                    tempSite.site_name = sqlDatabase.rows[i].sql_string[9];
                    tempSite.comment = sqlDatabase.rows[i].sql_string[10];

                    tempMission.sites.Add(tempSite);

                    tempMission.mission_id = sqlDatabase.rows[i].sql_string[0];
                    tempMission.ticket_number = sqlDatabase.rows[i].sql_string[1];
                    tempMission.mission_comment = sqlDatabase.rows[i].sql_string[2];
                    tempMission.vehichle = sqlDatabase.rows[i].sql_string[3];
                    tempMission.status = sqlDatabase.rows[i].sql_string[4];
                    tempMission.added_by = sqlDatabase.rows[i].sql_string[5];
                    tempMission.equipment.Add(sqlDatabase.rows[i].sql_string[7]);

                    tempEquipment.equipment = sqlDatabase.rows[i].sql_string[7];

                    if (i != 0 && mList.Last().company_serial == tempMission.company_serial && mList.Last().complaint_serial == tempMission.complaint_serial && mList.Last().mission_serial == tempMission.mission_serial)
                    {
                        if (!mList.Last().engineers.Exists(x1 => x1.key == tempEngineers.key))
                        {
                            mList.Last().engineers.Add(tempEngineers);
                        }

                        if (!mList.Last().equipment.Exists(x1 => x1 == tempEquipment.equipment) && tempEquipment.equipment != "")
                        {
                            mList.Last().equipment.Add(tempEquipment.equipment);
                        }

                        if (!mList.Last().sites.Exists(x1 => x1.site_serial == tempSite.site_serial))
                        {
                            mList.Last().sites.Add(tempSite);
                        }
                    }
                    else
                    {
                        mList.Add(tempMission);
                    }
                }
            }

            return true;
        }

        public bool GetCompanies(ref List<BASIC_STRUCTS.COMPANY_STRUCT> mList)
        {
            mList.Clear();
            
            String sqlQueryPart1 = @"with get_centre_frequency_unit as (select id, factor, unit from NTRA.dbo.band_units),

                                            get_bw_unit as (select id, factor, unit from NTRA.dbo.band_units),
                                            
                                            get_start_frequency_unit as (select id, factor, unit from NTRA.dbo.band_units),
                                            
                                            get_stop_frequency_unit as (select id, factor, unit from NTRA.dbo.band_units)

                                     select companies.serial,
                                            companies.work_field as work_field_id,
                                            companies.added_by as added_by_id,
                                            contacts.id,
											company_centre_frequency.serial as cf_serial,
                                            company_start_stop_frequency.serial as start_stop_serial,
											company_centre_frequency.centre_frequency_unit,
											company_centre_frequency.bandwidth_unit,
											company_start_stop_frequency.start_frequency_unit,
											company_start_stop_frequency.stop_frequency_unit,
											get_centre_frequency_unit.factor as cf_factor,
											get_bw_unit.factor as bw_factor,
											get_start_frequency_unit.factor as start_freq_factor,
											get_stop_frequency_unit.factor as stop_freq_factor,
											company_start_stop_frequency.start_frequency,
											company_start_stop_frequency.stop_frequency,
                                            company_centre_frequency.centre_frequency,
                                            company_centre_frequency.bandwidth,
                                            companies.name as company_name,
                                            company_field_of_work.field_of_work as work_field,
									        contact_name, phone,
											contacts.email,
											employee_info.name as added_by,
											get_centre_frequency_unit.unit as cf_unit,
											get_bw_unit.unit as bw_unit,
											get_start_frequency_unit.unit as start_freq_unit,
											get_stop_frequency_unit.unit as stop_freq_unit
                                     from NTRA.dbo.companies
                                     left join NTRA.dbo.company_field_of_work
                                     on companies.work_field = company_field_of_work.id
                                     left join NTRA.dbo.contacts
                                     on companies.serial = contacts.company_serial
                                     left join NTRA.dbo.employee_info
                                     on companies.added_by = employee_info.employee_id
									 left join NTRA.dbo.company_centre_frequency
									 on companies.serial = company_centre_frequency.company_serial
									 left join NTRA.dbo.company_start_stop_frequency
									 on companies.serial = company_start_stop_frequency.company_serial
									 left join get_centre_frequency_unit
									 on company_centre_frequency.centre_frequency_unit = get_centre_frequency_unit.id
									 left join get_bw_unit
									 on company_centre_frequency.bandwidth_unit = get_bw_unit.id
									 left join get_start_frequency_unit
									 on company_start_stop_frequency.start_frequency_unit = get_start_frequency_unit.id
									 left join get_stop_frequency_unit
									 on company_start_stop_frequency.stop_frequency_unit = get_stop_frequency_unit.id
                                     Order by companies.serial DESC";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 10;
            SQL_COLUMN_COUNT_STRUCT.sql_bigint = 4;
            SQL_COLUMN_COUNT_STRUCT.sql_decimal = 4;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 10;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    bool cf_BW = false;
                    bool start_stop = false;

                    BASIC_STRUCTS.COMPANY_STRUCT tempCompany = new BASIC_STRUCTS.COMPANY_STRUCT();
                    tempCompany.InitializeContactStruct();
                    tempCompany.start_and_stop_frequencies = new List<BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT>();
                    tempCompany.cf_and_bw_frequencies = new List<BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT>();

                    BASIC_STRUCTS.CONTACT_STRUCT tempContact = new BASIC_STRUCTS.CONTACT_STRUCT();

                    BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT centre_frequencies = new BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT();

                    BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT start_frequencies = new BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT();

                    tempCompany.company_serial = sqlDatabase.rows[i].sql_int[0];
                    tempCompany.company_field_of_work_id = sqlDatabase.rows[i].sql_int[1];
                    tempCompany.added_by_id = sqlDatabase.rows[i].sql_int[2];
                    
                    tempContact.contact_id = sqlDatabase.rows[i].sql_int[3];

                    centre_frequencies.serial = sqlDatabase.rows[i].sql_int[4];
                    start_frequencies.serial = sqlDatabase.rows[i].sql_int[5];

                    if (centre_frequencies.serial != 0)
                        cf_BW = true;
                    if(start_frequencies.serial != 0)
                        start_stop = true;

                    if (cf_BW == true)
                    {
                        centre_frequencies.centre_frequency_unit_id = sqlDatabase.rows[i].sql_int[6];
                        centre_frequencies.bandwidth_unit_id = sqlDatabase.rows[i].sql_int[7];

                        centre_frequencies.cf_factor = sqlDatabase.rows[i].sql_bigint[0];
                        centre_frequencies.bw_factor = sqlDatabase.rows[i].sql_bigint[1];
                    }

                    if (start_stop == true)
                    {
                        start_frequencies.start_frequency_unit_id = sqlDatabase.rows[i].sql_int[8];
                        start_frequencies.stop_frequency_unit_id = sqlDatabase.rows[i].sql_int[9];

                        start_frequencies.start_factor = sqlDatabase.rows[i].sql_bigint[2];
                        start_frequencies.stop_factor = sqlDatabase.rows[i].sql_bigint[3];
                    }


                    start_frequencies.start_frequency = sqlDatabase.rows[i].sql_decimal[0];
                    start_frequencies.stop_frequency = sqlDatabase.rows[i].sql_decimal[1];
                    centre_frequencies.centre_frequency = sqlDatabase.rows[i].sql_decimal[2];
                    centre_frequencies.bandwidth = sqlDatabase.rows[i].sql_decimal[3];

                    tempCompany.company_name = sqlDatabase.rows[i].sql_string[0];
                    tempCompany.company_field_of_work = sqlDatabase.rows[i].sql_string[1];
                    
                    tempContact.contact_name = sqlDatabase.rows[i].sql_string[2];
                    tempContact.contact_phone = sqlDatabase.rows[i].sql_string[3];
                    tempContact.contact_email = sqlDatabase.rows[i].sql_string[4];
                    
                    tempCompany.added_by = sqlDatabase.rows[i].sql_string[5];

                    if (cf_BW == true)
                    {
                        centre_frequencies.centre_frequency_unit = sqlDatabase.rows[i].sql_string[6];
                        centre_frequencies.bandwidth_unit = sqlDatabase.rows[i].sql_string[7];

                        centre_frequencies.max = (centre_frequencies.centre_frequency * centre_frequencies.cf_factor) + (centre_frequencies.bandwidth * centre_frequencies.bw_factor);
                        centre_frequencies.min = (centre_frequencies.centre_frequency * centre_frequencies.cf_factor) - (centre_frequencies.bandwidth * centre_frequencies.bw_factor);
                    }

                    if (start_stop == true)
                    {
                        start_frequencies.start_frequency_unit = sqlDatabase.rows[i].sql_string[8];
                        start_frequencies.stop_frequency_unit = sqlDatabase.rows[i].sql_string[9];

                        start_frequencies.max = start_frequencies.stop_frequency * start_frequencies.stop_factor;
                        start_frequencies.min = start_frequencies.start_frequency * start_frequencies.start_factor;
                    }


                    if (i != 0 && mList.Exists(x1 => x1.company_serial == tempCompany.company_serial))
                    {
                        if(tempContact.contact_id != 0 && !mList.Last().contacts.Exists(x1 => x1.contact_id == tempContact.contact_id))
                            mList.Last().contacts.Add(tempContact);

                        if (cf_BW == true && !mList.Last().cf_and_bw_frequencies.Exists(x1 => x1.serial == centre_frequencies.serial))
                            mList.Last().cf_and_bw_frequencies.Add(centre_frequencies);

                        if (start_stop == true && !mList.Last().start_and_stop_frequencies.Exists(x1 => x1.serial == start_frequencies.serial))
                            mList.Last().start_and_stop_frequencies.Add(start_frequencies);
                    }
                    else
                    {
                        if(tempContact.contact_id != 0)
                            tempCompany.contacts.Add(tempContact);

                        if (cf_BW == true)
                            tempCompany.cf_and_bw_frequencies.Add(centre_frequencies);

                        if (start_stop == true)
                            tempCompany.start_and_stop_frequencies.Add(start_frequencies);

                        mList.Add(tempCompany);
                    }
                }
            }

            return true;
        }

        public bool GetEMFPoints(ref List<BASIC_STRUCTS.EMF_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = @"select emf_points.serial as point_serial,
	                                        emf_point_bands.serial,
	                                        emf_points.status,
	                                        emf_point_bands.band,
	                                        emf_points.reading_date,
	                                        emf_points.name,
	                                        emf_points.area,
	                                        emf_points.district,
	                                        emf_points.latitude,
	                                        emf_points.longitude,
	                                        emf_points.actual_long,
	                                        emf_points.actual_lat,
	                                        emf_point_bands.average_power_density,
	                                        emf_point_bands.max_power_density,
                        	                emf_point_status.status,
											emf_bands.band
                        from NTRA.dbo.emf_points
                        left join NTRA.dbo.emf_point_bands
                        on emf_points.serial = emf_point_bands.point_serial
                        left join NTRA.dbo.emf_point_status
                        on emf_points.status = emf_point_status.id
						left join NTRA.dbo.emf_bands
						on emf_point_bands.band = emf_bands.id
                        Order by emf_points.serial DESC";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 4;
            SQL_COLUMN_COUNT_STRUCT.sql_datetime = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 11;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {

                    BASIC_STRUCTS.EMF_STRUCT tempPoint = new BASIC_STRUCTS.EMF_STRUCT();
                    tempPoint.bands = new List<BASIC_STRUCTS.EMF_BAND_STRUCT>();

                    BASIC_STRUCTS.EMF_BAND_STRUCT tempBand = new BASIC_STRUCTS.EMF_BAND_STRUCT();

                    tempPoint.emf_serial = sqlDatabase.rows[i].sql_int[0];
                    tempBand.band_serial = sqlDatabase.rows[i].sql_int[1];
                    tempPoint.emf_status_id = sqlDatabase.rows[i].sql_int[2];

                    tempPoint.date = sqlDatabase.rows[i].sql_datetime[0];

                    tempPoint.name = sqlDatabase.rows[i].sql_string[0];
                    tempPoint.area = sqlDatabase.rows[i].sql_string[1];
                    tempPoint.district = sqlDatabase.rows[i].sql_string[2];
                    tempPoint.latitude = sqlDatabase.rows[i].sql_string[3];
                    tempPoint.longitude = sqlDatabase.rows[i].sql_string[4];
                    tempPoint.actual_longitude = sqlDatabase.rows[i].sql_string[5];
                    tempPoint.actual_latitude = sqlDatabase.rows[i].sql_string[6];

                    if (tempBand.band_serial != 0)
                    {
                        tempBand.average_power_density = Double.Parse(sqlDatabase.rows[i].sql_string[7]);
                        tempBand.max_power_density = Double.Parse(sqlDatabase.rows[i].sql_string[8]);
                        tempBand.band_name = sqlDatabase.rows[i].sql_string[10];
                        tempBand.band = sqlDatabase.rows[i].sql_int[3];
                    }
                    tempPoint.emf_status = sqlDatabase.rows[i].sql_string[9];
                    
                    if (i != 0 && mList.Exists(x1 => x1.emf_serial == tempPoint.emf_serial))
                    {
                        if (tempBand.band_serial != 0 && !mList.Last().bands.Exists(x1 => x1.band_serial == tempBand.band_serial))
                            mList.Last().bands.Add(tempBand);

                    }
                    else
                    {
                        if (tempBand.band_serial != 0)
                            tempPoint.bands.Add(tempBand);

                        mList.Add(tempPoint);
                    }
                }
            }

            return true;
        }

        public bool GetEMFPlans(ref List<BASIC_STRUCTS.EMF_PLAN_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = @"select  emf_plans.serial as plan_serial,
		                                     emf_plan_points.point_serial as point_serial,
		                                     emf_point_bands.serial as band_serial,
		                                     emf_plans.status as plan_status_id,
		                                     emf_plans.assigned_engineer as assigned_engineer_id,
		                                     emf_points.status as point_status_id,
		                                     emf_point_bands.band,

                                             emf_plans.deadline_date,

		                                     emf_plans.plan_name,
		                                     emf_plan_status.status as plan_status,
		                                     employee_info.name as assigned_engineer,
		                                     emf_points.name as point_name,
		                                     emf_points.area,
		                                     emf_points.district,
		                                     emf_points.latitude,
		                                     emf_points.longitude,
		                                     emf_points.actual_lat,
		                                     emf_points.actual_long,
		                                     emf_point_status.status as point_status,
											 emf_bands.band,
		                                     emf_point_bands.average_power_density,
		                                     emf_point_bands.max_power_density
                        from NTRA.dbo.emf_plans
                        left join NTRA.dbo.emf_plan_points
                        on emf_plans.serial = emf_plan_points.plan_serial
                        left join NTRA.dbo.emf_plan_status
                        on emf_plans.status = emf_plan_status.id
                        left join NTRA.dbo.employee_info
                        on emf_plans.assigned_engineer = employee_info.employee_id
                        left join NTRA.dbo.emf_points
                        on emf_plan_points.point_serial = emf_points.serial
                        left join NTRA.dbo.emf_point_status
                        on emf_points.status = emf_point_status.id
                        left join NTRA.dbo.emf_point_bands
                        on emf_points.serial = emf_point_bands.point_serial
						left join NTRA.dbo.emf_bands
						on emf_point_bands.band = emf_bands.id
                        Order by emf_plans.serial DESC";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 7;
            SQL_COLUMN_COUNT_STRUCT.sql_datetime = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 14;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.EMF_PLAN_STRUCT tempPlan = new BASIC_STRUCTS.EMF_PLAN_STRUCT();
                    tempPlan.points = new List<BASIC_STRUCTS.EMF_STRUCT>();
                    BASIC_STRUCTS.EMF_STRUCT tempPoint = new BASIC_STRUCTS.EMF_STRUCT();
                    tempPoint.bands = new List<BASIC_STRUCTS.EMF_BAND_STRUCT>();

                    BASIC_STRUCTS.EMF_BAND_STRUCT tempBand = new BASIC_STRUCTS.EMF_BAND_STRUCT();

                    tempPlan.deadline_date = sqlDatabase.rows[i].sql_datetime[0];

                    tempPlan.plan_serial = sqlDatabase.rows[i].sql_int[0];
                    tempPoint.emf_serial = sqlDatabase.rows[i].sql_int[1];
                    tempBand.band_serial = sqlDatabase.rows[i].sql_int[2];
                    tempPlan.status_id = sqlDatabase.rows[i].sql_int[3];
                    tempPlan.assigned_engineer_id = sqlDatabase.rows[i].sql_int[4];
                    tempPoint.emf_status_id = sqlDatabase.rows[i].sql_int[5];
                    tempBand.band = sqlDatabase.rows[i].sql_int[6];


                    tempPlan.plan_name = sqlDatabase.rows[i].sql_string[0];
                    tempPlan.status = sqlDatabase.rows[i].sql_string[1];
                    tempPlan.assigned_engineer = sqlDatabase.rows[i].sql_string[2];
                    tempPoint.name = sqlDatabase.rows[i].sql_string[3];
                    tempPoint.area = sqlDatabase.rows[i].sql_string[4];
                    tempPoint.district = sqlDatabase.rows[i].sql_string[5];
                    tempPoint.latitude = sqlDatabase.rows[i].sql_string[6];
                    tempPoint.longitude = sqlDatabase.rows[i].sql_string[7];
                    tempPoint.actual_latitude = sqlDatabase.rows[i].sql_string[8];
                    tempPoint.actual_longitude = sqlDatabase.rows[i].sql_string[9];
                    tempPoint.emf_status = sqlDatabase.rows[i].sql_string[10];
                    if (tempBand.band_serial != 0)
                    {
                        tempBand.band_name = sqlDatabase.rows[i].sql_string[11];
                        tempBand.average_power_density = Double.Parse(sqlDatabase.rows[i].sql_string[12]);
                        tempBand.max_power_density = Double.Parse(sqlDatabase.rows[i].sql_string[13]);
                        tempBand.band = sqlDatabase.rows[i].sql_int[6];

                        tempPoint.bands.Add(tempBand);
                    }

                    if(tempPoint.emf_serial != 0)
                        tempPlan.points.Add(tempPoint);

                    if (i != 0 && mList.Exists(x1 => x1.plan_serial == tempPlan.plan_serial))
                    {
                        if (tempPoint.emf_serial != 0 && !mList.Last().points.Exists(x1 => x1.emf_serial == tempPoint.emf_serial))
                            mList.Last().points.Add(tempPoint);

                        if (tempBand.band_serial != 0 && !mList.Last().points.Last().bands.Exists(x1 => x1.band_serial == tempBand.band_serial))
                            mList.Last().points.Last().bands.Add(tempBand);
                    }
                    else
                    {
                        mList.Add(tempPlan);
                    }
                }
            }

            return true;
        }

        public bool GetAllSites(ref List<BASIC_STRUCTS.MIN_SITE_STRUCT> mList)
        {
            mList.Clear();
        
            String sqlQueryPart1 = @"select company_sites.company_Serial,
                                            company_sites.serial,
		                                    company_sites.city,
		                                    company_sites.added_by,
		                                    companies.name,
		                                    cities.city,
		                                    company_sites.site_number,
		                                    company_sites.lat,
		                                    company_sites.long,
		                                    company_sites.region,
		                                    employee_info.name
                                     from NTRA.dbo.company_sites
                                     left join NTRA.dbo.companies
                                     on company_sites.company_serial = companies.serial
                                     left join NTRA.dbo.cities
                                     on company_sites.city = cities.id
                                     left join NTRA.dbo.employee_info
                                     on company_sites.added_by = employee_info.employee_id
                                     order by company_sites.date_added DESC";
        
        
            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;
        
            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 4;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 7;
        
            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;
        
            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.MIN_SITE_STRUCT tempSite = new BASIC_STRUCTS.MIN_SITE_STRUCT();

                    tempSite.company_serial = sqlDatabase.rows[i].sql_int[0];
                    tempSite.site_serial = sqlDatabase.rows[i].sql_int[1];
                    tempSite.city_id = sqlDatabase.rows[i].sql_int[2];
                    tempSite.added_by_id = sqlDatabase.rows[i].sql_int[3];

                    tempSite.company_name = sqlDatabase.rows[i].sql_string[0];
                    tempSite.city = sqlDatabase.rows[i].sql_string[1];
                    tempSite.site_number = sqlDatabase.rows[i].sql_string[2];
                    tempSite.latitude = sqlDatabase.rows[i].sql_string[3];
                    tempSite.longitude = sqlDatabase.rows[i].sql_string[4];
                    tempSite.region = sqlDatabase.rows[i].sql_string[5];
                    tempSite.added_by = sqlDatabase.rows[i].sql_string[6];

                    mList.Add(tempSite);
                }
            }
        
            return true;
        }

        public bool GetCompanySites(ref List<BASIC_STRUCTS.MIN_SITE_STRUCT> mList, int mCompanySerial)
        {
            mList.Clear();
        
            String sqlQueryPart1 = @"select company_sites.company_Serial,
                                          company_sites.serial,
		                                  company_sites.city,
		                                  company_sites.added_by,
		                                  companies.name,
		                                  cities.city,
		                                  company_sites.site_number,
		                                  company_sites.lat,
		                                  company_sites.long,
		                                  company_sites.region,
		                                  employee_info.name
                                   from NTRA.dbo.company_sites
                                   left join NTRA.dbo.companies
                                   on company_sites.company_serial = companies.serial
                                   left join NTRA.dbo.cities
                                   on company_sites.city = cities.id
                                   left join NTRA.dbo.employee_info
                                   on company_sites.added_by = employee_info.employee_id
                                   where company_sites.company_serial = ";
        
            String sqlQueryPart2 = " order by company_sites.company_serial, company_sites.serial DESC";
        
            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;
            sqlQuery += mCompanySerial;
            sqlQuery += sqlQueryPart2;
        
            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 4;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 7;
        
            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;
        
            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.MIN_SITE_STRUCT tempSite = new BASIC_STRUCTS.MIN_SITE_STRUCT();

                    tempSite.company_serial = sqlDatabase.rows[i].sql_int[0];
                    tempSite.site_serial = sqlDatabase.rows[i].sql_int[1];
                    tempSite.city_id = sqlDatabase.rows[i].sql_int[2];
                    tempSite.added_by_id = sqlDatabase.rows[i].sql_int[3];

                    tempSite.company_name = sqlDatabase.rows[i].sql_string[0];
                    tempSite.city = sqlDatabase.rows[i].sql_string[1];
                    tempSite.site_number = sqlDatabase.rows[i].sql_string[2];
                    tempSite.latitude = sqlDatabase.rows[i].sql_string[3];
                    tempSite.longitude = sqlDatabase.rows[i].sql_string[4];
                    tempSite.region = sqlDatabase.rows[i].sql_string[5];
                    tempSite.added_by = sqlDatabase.rows[i].sql_string[6];


                    mList.Add(tempSite);
                }
            }
        
            return true;
        }

        public bool GetComplaints(ref List<BASIC_STRUCTS.COMPLAINT_STRUCT> mList)
        {
            mList.Clear();
            
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
                                     order by complaints.date_added DESC";
            
            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;
            
            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 12;
            SQL_COLUMN_COUNT_STRUCT.sql_money = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_datetime = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 16;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;
            
            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.COMPLAINT_STRUCT tempComplaint = new BASIC_STRUCTS.COMPLAINT_STRUCT();
                    tempComplaint.sites = new List<BASIC_STRUCTS.SITE_STRUCT>();
                    BASIC_STRUCTS.SITE_STRUCT tempSite = new BASIC_STRUCTS.SITE_STRUCT();
                    tempSite.sectors = new List<BASIC_STRUCTS.SECTOR_STRUCT>();
                    BASIC_STRUCTS.SECTOR_STRUCT tempSector = new BASIC_STRUCTS.SECTOR_STRUCT();
                    tempSector.sector_bands = new List<string>();
                    
                    tempComplaint.complaint_serial = sqlDatabase.rows[i].sql_int[0];
                    tempSite.complaint_serial = tempComplaint.complaint_serial;
                    tempSector.complaint_serial = tempComplaint.complaint_serial;
                    
                    tempSite.site_serial = sqlDatabase.rows[i].sql_int[1];
                    tempSector.site_serial = tempSite.site_serial;
                    
                    tempSector.sector_serial = sqlDatabase.rows[i].sql_int[2];
                    
                    tempComplaint.company_serial = sqlDatabase.rows[i].sql_int[4];
                    
                    tempSite.city_id = sqlDatabase.rows[i].sql_int[5];
                    
                    tempComplaint.added_by_id = sqlDatabase.rows[i].sql_int[6];
                    
                    tempComplaint.complaint_status_id = sqlDatabase.rows[i].sql_int[7];
                    
                    tempSite.site_status_id = sqlDatabase.rows[i].sql_int[8];

                    tempSector.shared_with_id = sqlDatabase.rows[i].sql_int[9];
                    

                    tempComplaint.complaint_date = sqlDatabase.rows[i].sql_datetime[0];
                    
                    
                    tempComplaint.ticket_number = sqlDatabase.rows[i].sql_string[0];
                    
                    tempSite.region = sqlDatabase.rows[i].sql_string[1];
                    
                    tempSite.latitude = sqlDatabase.rows[i].sql_string[2];
                    
                    tempSite.longitude = sqlDatabase.rows[i].sql_string[3];
                    
                    tempSite.site_number = sqlDatabase.rows[i].sql_string[4];
                    
                    tempSector.rru = sqlDatabase.rows[i].sql_string[5];
                    
                    tempSector.shared_with = sqlDatabase.rows[i].sql_string[6];
                    
                    tempSector.sector_number = sqlDatabase.rows[i].sql_string[7];
                    
                    if(sqlDatabase.rows[i].sql_string[8] != "")
                        tempSector.sector_bands.Add(sqlDatabase.rows[i].sql_string[8]);
                    
                    tempSite.city = sqlDatabase.rows[i].sql_string[9];
                    
                    tempComplaint.company_name = sqlDatabase.rows[i].sql_string[10];
                    
                    tempComplaint.added_by = sqlDatabase.rows[i].sql_string[11];
                    
                    tempComplaint.complaint_id = sqlDatabase.rows[i].sql_string[12];
                    
                    tempComplaint.complaint_status = sqlDatabase.rows[i].sql_string[13];
                    
                    tempSite.site_status = sqlDatabase.rows[i].sql_string[14];

                    tempSector.rtwp = sqlDatabase.rows[i].sql_money[0];

                    tempSector.azimuth = sqlDatabase.rows[i].sql_int[10];
                    tempSector.sector_status_id = sqlDatabase.rows[i].sql_int[11];

                    tempSector.sector_status = sqlDatabase.rows[i].sql_string[15];

                    tempSite.sectors.Add(tempSector);
                    tempComplaint.sites.Add(tempSite);
                    
                    if (i != 0 && mList.Last().company_serial == tempComplaint.company_serial && mList.Last().complaint_serial == tempComplaint.complaint_serial && mList.Last().sites.Last().site_serial == tempSite.site_serial && mList.Last().sites.Last().sectors.Last().sector_serial == tempSector.sector_serial)
                    {
                        if(sqlDatabase.rows[i].sql_string[8] != "")
                            mList.Last().sites.Last().sectors.Last().sector_bands.Add(sqlDatabase.rows[i].sql_string[8]);
                    }
                    else if (i != 0 && mList.Last().company_serial == tempComplaint.company_serial && mList.Last().complaint_serial == tempComplaint.complaint_serial && mList.Last().sites.Last().site_serial == tempSite.site_serial)
                    {
                        mList.Last().sites.Last().sectors.Add(tempSector);
                    }
                    else if (i != 0 && mList.Last().company_serial == tempComplaint.company_serial && mList.Last().complaint_serial == tempComplaint.complaint_serial)
                    {
                        mList.Last().sites.Add(tempSite);
                    }
                    else
                        mList.Add(tempComplaint);
                }
            }
            
            return true;
        }

        public bool GetComplaints(ref List<BASIC_STRUCTS.COMPLAINT_STRUCT> mList, int mCompanySerial, int mSerial)
        {
            mList.Clear();

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
											site_status.status as site_status
            
            
                                     from NTRA.dbo.complaints
                                     left join NTRA.dbo.complaint_sites
                                     on complaints.serial = complaint_sites.complaint_serial
                                     left join NTRA.dbo.complaint_site_sectors
                                     on complaint_sites.complaint_serial = complaint_site_sectors.complaint_serial
                                     and complaint_sites.site_serial = complaint_site_sectors.site_serial
                                     left join NTRA.dbo.complaint_site_sector_bands
                                     on complaint_site_sectors.complaint_serial = complaint_site_sector_bands.complaint_serial
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
                                     where complaints.company_serial = ";

            String sqlQueryPart2 = " and complaint_sites.site_serial = ";
            String sqlQueryPart3 = " order by complaints.date_added DESC";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;
            sqlQuery += mCompanySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += mSerial;
            sqlQuery += sqlQueryPart3;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 10;
            SQL_COLUMN_COUNT_STRUCT.sql_datetime = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 15;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.COMPLAINT_STRUCT tempComplaint = new BASIC_STRUCTS.COMPLAINT_STRUCT();
                    tempComplaint.sites = new List<BASIC_STRUCTS.SITE_STRUCT>();
                    BASIC_STRUCTS.SITE_STRUCT tempSite = new BASIC_STRUCTS.SITE_STRUCT();
                    tempSite.sectors = new List<BASIC_STRUCTS.SECTOR_STRUCT>();
                    BASIC_STRUCTS.SECTOR_STRUCT tempSector = new BASIC_STRUCTS.SECTOR_STRUCT();
                    tempSector.sector_bands = new List<string>();

                    tempComplaint.complaint_serial = sqlDatabase.rows[i].sql_int[0];
                    tempSite.complaint_serial = tempComplaint.complaint_serial;
                    tempSector.complaint_serial = tempComplaint.complaint_serial;

                    tempSite.site_serial = sqlDatabase.rows[i].sql_int[1];
                    tempSector.site_serial = tempSite.site_serial;

                    tempSector.sector_serial = sqlDatabase.rows[i].sql_int[2];

                    tempComplaint.company_serial = sqlDatabase.rows[i].sql_int[4];

                    tempSite.city_id = sqlDatabase.rows[i].sql_int[5];

                    tempComplaint.added_by_id = sqlDatabase.rows[i].sql_int[6];

                    tempComplaint.complaint_status_id = sqlDatabase.rows[i].sql_int[7];

                    tempSite.site_status_id = sqlDatabase.rows[i].sql_int[8];

                    tempSector.shared_with_id = sqlDatabase.rows[i].sql_int[9];


                    tempComplaint.complaint_date = sqlDatabase.rows[i].sql_datetime[0];


                    tempComplaint.ticket_number = sqlDatabase.rows[i].sql_string[0];

                    tempSite.region = sqlDatabase.rows[i].sql_string[1];

                    tempSite.latitude = sqlDatabase.rows[i].sql_string[2];

                    tempSite.longitude = sqlDatabase.rows[i].sql_string[3];

                    tempSite.site_number = sqlDatabase.rows[i].sql_string[4];

                    tempSector.rru = sqlDatabase.rows[i].sql_string[5];

                    tempSector.shared_with = sqlDatabase.rows[i].sql_string[6];

                    tempSector.sector_number = sqlDatabase.rows[i].sql_string[7];

                    if (sqlDatabase.rows[i].sql_string[8] != "")
                        tempSector.sector_bands.Add(sqlDatabase.rows[i].sql_string[8]);

                    tempSite.city = sqlDatabase.rows[i].sql_string[9];

                    tempComplaint.company_name = sqlDatabase.rows[i].sql_string[10];

                    tempComplaint.added_by = sqlDatabase.rows[i].sql_string[11];

                    tempComplaint.complaint_id = sqlDatabase.rows[i].sql_string[12];

                    tempComplaint.complaint_status = sqlDatabase.rows[i].sql_string[13];

                    tempSite.site_status = sqlDatabase.rows[i].sql_string[14];

                    tempSite.sectors.Add(tempSector);
                    tempComplaint.sites.Add(tempSite);

                    if (i != 0 && mList.Last().complaint_serial == tempComplaint.complaint_serial && mList.Last().sites.Last().site_serial == tempSite.site_serial && mList.Last().sites.Last().sectors.Last().sector_serial == tempSector.sector_serial)
                    {
                        if (sqlDatabase.rows[i].sql_string[8] != "")
                            mList.Last().sites.Last().sectors.Last().sector_bands.Add(sqlDatabase.rows[i].sql_string[8]);
                    }
                    else if (i != 0 && mList.Last().complaint_serial == tempComplaint.complaint_serial && mList.Last().sites.Last().site_serial == tempSite.site_serial)
                    {
                        mList.Last().sites.Last().sectors.Add(tempSector);
                    }
                    else if (i != 0 && mList.Last().complaint_serial == tempComplaint.complaint_serial)
                    {
                        mList.Last().sites.Add(tempSite);
                    }
                    else
                        mList.Add(tempComplaint);
                }
            }

            return true;
        }

        public bool CheckEMFPointNameExists(String mName)
        {
            String sqlQueryPart1 = "select serial from NTRA.dbo.emf_points where name = '" + mName + "';";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                return false;
            }

            return true;
        }

        public bool CheckSiteNameExists(int mCompanySerial, String mName)
        {
            String sqlQueryPart1 = "select serial from NTRA.dbo.company_sites where company_serial = " + mCompanySerial + " and site_number = '" + mName + "';";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                return false;
            }

            return true;
        }

        public bool GetNewRepeaterSerial(ref int mSerial)
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select max(serial) from NTRA.dbo.repeaters ";

            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                mSerial = sqlDatabase.rows[0].sql_int[0] + 1;
            }

            if (mSerial == 0)
                mSerial = 1;

            return true;
        }

        public bool DeleteRepeaters(int mSerial)
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"delete from NTRA.dbo.repeaters where serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += mSerial;


            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool InsertIntoRepeaters(ref List<BASIC_STRUCTS.REPEATER_STRUCT> mRepeaters)
        {
            int serial = 1;

            GetNewRepeaterSerial(ref serial);


            for (int i = 0; i < mRepeaters.Count; i++)
            {
                String sqlQueryPart1 = "Insert into NTRA.dbo.repeaters values (";
                sqlQueryPart1 += serial + " ,'";
                sqlQueryPart1 += mRepeaters[i].latitude + "','";
                sqlQueryPart1 += mRepeaters[i].longitude + "',N'";
                sqlQueryPart1 += mRepeaters[i].address + "',";
                sqlQueryPart1 += mRepeaters[i].status_id + ",N'";
                sqlQueryPart1 += mRepeaters[i].comment + "',";
                sqlQueryPart1 += mRepeaters[i].added_by_id + ",'";
                sqlQueryPart1 += mRepeaters[i].date + "',getdate(),";
                sqlQueryPart1 += mRepeaters[i].city_id + ",N'";
                sqlQueryPart1 += mRepeaters[i].area + "');";


                if (!sqlDatabase.InsertRows(sqlQueryPart1))
                    return false;

                serial++;
            }
            return true;
        }

        public bool InsertIntoRepeaters(ref BASIC_STRUCTS.REPEATER_STRUCT mRepeaters)
        {
            int serial = 1;

            GetNewRepeaterSerial(ref serial);


            String sqlQueryPart1 = "Insert into NTRA.dbo.repeaters values (";
            sqlQueryPart1 += serial + " ,'";
            sqlQueryPart1 += mRepeaters.latitude + "','";
            sqlQueryPart1 += mRepeaters.longitude + "',N'";
            sqlQueryPart1 += mRepeaters.address + "',";
            sqlQueryPart1 += mRepeaters.status_id + ",N'";
            sqlQueryPart1 += mRepeaters.comment + "',";
            sqlQueryPart1 += mRepeaters.added_by_id + ",'";
            sqlQueryPart1 += mRepeaters.date + "',getdate(),";
            sqlQueryPart1 += mRepeaters.city_id + ",N'";
            sqlQueryPart1 += mRepeaters.area + "');";


            if (!sqlDatabase.InsertRows(sqlQueryPart1))
                return false;

            return true;
        }

        public bool UpdateRepeater(ref BASIC_STRUCTS.REPEATER_STRUCT mRepeater)
        {
            String sqlQuery = "update NTRA.dbo.repeaters set address = N'";
            sqlQuery += mRepeater.address + "', status = ";
            sqlQuery += mRepeater.status_id + ", comment = N'";
            sqlQuery += mRepeater.comment + "', date = '";
            sqlQuery += mRepeater.date + "' where lat ='";
            sqlQuery += mRepeater.latitude + "' and long = '";
            sqlQuery += mRepeater.longitude + "';";

                if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool CheckRepeater(ref BASIC_STRUCTS.REPEATER_STRUCT mRepeater)
        {
            int serial = 0;

            String sqlQuery = @"select serial from NTRA.dbo.repeaters where lat = '";
            sqlQuery += mRepeater.latitude + "' and long = '";
            sqlQuery += mRepeater.longitude + "';";

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                serial = sqlDatabase.rows[0].sql_int[0];
            }

            if(serial == 0)
                return false;
            else
                return true;
        }

        public bool GetRepeaters(ref List<BASIC_STRUCTS.REPEATER_STRUCT> mList)
        {
            mList.Clear();

            String sqlQueryPart1 = @"select serial,
                                            repeaters.status,
                                            repeaters.added_by,
											repeaters.city,
                                            date,
                                            repeaters.date_added,
                                            lat,
                                            long,
                                            address,
                                            comment,
                                            repeaters_status.status,
                                            employee_info.name,
											cities.city,
											repeaters.area
                                     from NTRA.dbo.repeaters
                                     left join NTRA.dbo.repeaters_status
                                     on repeaters.status = repeaters_status.id
                                     left join NTRA.dbo.employee_info
                                     on repeaters.added_by = employee_info.employee_id
									 left join NTRA.dbo.cities
									 on repeaters.city = cities.id
                                     order by repeaters.serial DESC";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 4;
            SQL_COLUMN_COUNT_STRUCT.sql_datetime = 2;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 8;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.REPEATER_STRUCT temp = new BASIC_STRUCTS.REPEATER_STRUCT();
                    temp.repeater_serial = sqlDatabase.rows[i].sql_int[0];
                    temp.status_id = sqlDatabase.rows[i].sql_int[1];
                    temp.added_by_id = sqlDatabase.rows[i].sql_int[2];
                    temp.city_id = sqlDatabase.rows[i].sql_int[3];

                    temp.date = sqlDatabase.rows[i].sql_datetime[0];
                    temp.date_added = sqlDatabase.rows[i].sql_datetime[1];

                    temp.latitude = sqlDatabase.rows[i].sql_string[0];
                    temp.longitude = sqlDatabase.rows[i].sql_string[1];
                    temp.address = sqlDatabase.rows[i].sql_string[2];
                    temp.comment = sqlDatabase.rows[i].sql_string[3];
                    temp.status = sqlDatabase.rows[i].sql_string[4];
                    temp.added_by = sqlDatabase.rows[i].sql_string[5];
                    temp.city = sqlDatabase.rows[i].sql_string[6];
                    temp.area = sqlDatabase.rows[i].sql_string[7];

                    mList.Add(temp);
                }
            }

            return true;
        }


        public bool GetCityAreas(ref List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> mList, int cityId)
        {
            mList.Clear();

            String sqlQueryPart1 = @"select distinct area from NTRA.dbo.repeaters where city = ";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;
            sqlQuery += cityId;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_string = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT temp = new BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT();
                    temp.key = 0;

                    temp.value = sqlDatabase.rows[i].sql_string[0];

                    mList.Add(temp);
                }
            }

            return true;
        }

        public bool InsertEmployeesLog(String mName, String mAction)
        {
            String sqlQueryPart1 = "Insert into NTRA.dbo.employee_login_log values (getdate(),'";
            sqlQueryPart1 += mName + "','" ;
            sqlQueryPart1 += mAction + "');" ;

            if (!sqlDatabase.InsertRows(sqlQueryPart1))
                return false;

            return true;
        }

        public bool GetSiteSerial(String mSiteNumber, ref int mSiteSerial)
        {

            String sqlQueryPart1 = @"select serial from NTRA.dbo.company_sites where site_number = '";
            String sqlQueryPart2 = "'";

            String sqlQuery = string.Empty;
            sqlQuery = sqlQueryPart1;
            sqlQuery += mSiteNumber;
            sqlQuery += sqlQueryPart2;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                mSiteSerial = sqlDatabase.rows[0].sql_int[0];
            }
            else
            {
                MessageBox.Show("Site '" + mSiteNumber + "' not found in database, Please check spelling and try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

    }
}
