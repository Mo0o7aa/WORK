using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntra_missions
{
    public class EMFPlan
    {
        SQLServer sqlDatabase;

        public List<BASIC_STRUCTS.EMF_STRUCT> planPoints;

        public String planName;
        public int serial;
        public DateTime deadline_date;
        public int planStatusId;
        public String planStatus;
        public String assignedEngineer;
        public int assignedEngineerId;


        public EMFPlan()
        {
            sqlDatabase = new SQLServer(); 
            planPoints = new List<BASIC_STRUCTS.EMF_STRUCT>();
        }

        public bool InitializePlan(int mPlanSerial)
        {
            serial = mPlanSerial;

            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select  emf_plans.serial as plan_serial,
		                                     emf_plan_points.point_serial as point_serial,
		                                     emf_point_bands.serial as band_serial,
		                                     emf_plans.status as plan_status_id,
		                                     emf_plans.assigned_engineer as assigned_engineer_id,
		                                     emf_points.status as point_status_id,

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
		                                     emf_point_bands.band,
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
                        
                        where emf_plans.serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += mPlanSerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 6;
            SQL_COLUMN_COUNT_STRUCT.sql_datetime = 1;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 14;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                deadline_date = sqlDatabase.rows[0].sql_datetime[0];

                serial = sqlDatabase.rows[0].sql_int[0];

                planStatusId = sqlDatabase.rows[0].sql_int[3];
                assignedEngineerId = sqlDatabase.rows[0].sql_int[4];

                planName = sqlDatabase.rows[0].sql_string[0];
                planStatus = sqlDatabase.rows[0].sql_string[1];
                assignedEngineer = sqlDatabase.rows[0].sql_string[2];


                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.EMF_STRUCT tempPoint = new BASIC_STRUCTS.EMF_STRUCT();
                    tempPoint.bands = new List<BASIC_STRUCTS.EMF_BAND_STRUCT>();
                    BASIC_STRUCTS.EMF_BAND_STRUCT tempBand = new BASIC_STRUCTS.EMF_BAND_STRUCT();

                    tempPoint.emf_serial = sqlDatabase.rows[i].sql_int[1];
                    tempBand.band_serial = sqlDatabase.rows[i].sql_int[2];
                    tempPoint.emf_status_id = sqlDatabase.rows[i].sql_int[5];

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
                        tempBand.band = sqlDatabase.rows[i].sql_string[11];
                        tempBand.average_power_density = Double.Parse(sqlDatabase.rows[i].sql_string[12]);
                        tempBand.max_power_density = Double.Parse(sqlDatabase.rows[i].sql_string[13]);

                        tempPoint.bands.Add(tempBand);
                    }

                    //if (tempPoint.emf_serial != 0)
                    //    planPoints.Add(tempPoint);

                    if (i != 0 && planPoints.Exists(x1 => x1.emf_serial == tempPoint.emf_serial))
                    {
                        if (tempBand.band_serial != 0 && planPoints.Last().bands.Exists(x1 => x1.band_serial == tempBand.band_serial))
                            planPoints.Last().bands.Add(tempBand);
                    }
                    else
                    {
                        planPoints.Add(tempPoint);
                    }
                }
            }
                return true;
        }
        public bool InsertIntoEMFPlans()
        {
            String sqlQuery = "insert into NTRA.dbo.emf_plans values(";
            sqlQuery += serial + ",'";
            sqlQuery += planName + "',";
            sqlQuery += planStatusId + ",";
            sqlQuery += assignedEngineerId + ",'";
            sqlQuery += deadline_date + "', getdate());";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool UpdateEMFPlan()
        {
            String sqlQuery = "update NTRA.dbo.emf_plans set plan_name = '";
            sqlQuery += planName + "' , status = ";
            sqlQuery += planStatusId + " , assigned_engineer = ";
            sqlQuery += assignedEngineerId + ",  deadline_date = '";
            sqlQuery += deadline_date + "' where serial = ";
            sqlQuery += serial;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool InsertIntoEMFPlansPoints()
        {
            for (int i = 0; i < planPoints.Count; i++)
            {
                String sqlQuery = "insert into NTRA.dbo.emf_plan_points values(";
                sqlQuery += serial + ",";
                sqlQuery += planPoints[i].emf_serial + ",getdate());";

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;
            }

            return true;
        }

        public bool DeleteEMFPlansPoints()
        {
            String sqlQuery = "delete from NTRA.dbo.emf_plan_points where plan_serial = ";
            sqlQuery += serial;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool GetNewSerial()
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select max(serial) from NTRA.dbo.emf_plans";

            sqlQuery = sqlQueryPart1;

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

        public bool IssueNewPlan()
        {
            if (!GetNewSerial())
                return false;
            if (!InsertIntoEMFPlans())
                return false;
            if (!InsertIntoEMFPlansPoints())
                return false;

            return true;
        }
        public void SetPlanName(String mName)
        {
            planName = mName;
        }

        public void SetPlanSerial(int mSerial)
        {
            serial = mSerial;
        }
        public void SetPlanDeadlineDate(DateTime mDate)
        {
            deadline_date = mDate;
        }
        public void SetPlanStatus(String mStatus)
        {
            planStatus = mStatus;
        }
        public void SetPlanStatusId(int mId)
        {
            planStatusId = mId;
        }
        public void SetAssignedEngineerId(int mId)
        {
            assignedEngineerId = mId;
        }
        public void SetAssignedEngineer(String mEngineer)
        {
            assignedEngineer = mEngineer;
        }
        public void SetPlanPoints(ref List<BASIC_STRUCTS.EMF_STRUCT> mPoints)
        {
            planPoints.Clear();

            for(int i = 0; i < mPoints.Count; i++)
            {
                planPoints.Add(mPoints[i]);
            }
        }

        public String GetPlanName()
        {
            return planName;
        }

        public int GetPlanSerial()
        {
            return serial;
        }
        public DateTime GetPlanDeadlineDate()
        {
            return deadline_date;
        }
        public String GetPlanStatus()
        {
            return planStatus;
        }
        public int GetPlanStatusId()
        {
            return planStatusId;
        }
        public int GetAssignedEngineerId()
        {
            return assignedEngineerId;
        }
        public string GetAssignedEngineer()
        {
            return assignedEngineer;
        }
        public List<BASIC_STRUCTS.EMF_STRUCT> GetPlanPoints()
        {
            return planPoints;

        }
    }
}
