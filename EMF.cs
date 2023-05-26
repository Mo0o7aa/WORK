using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntra_missions
{
    public class EMF
    {
        private SQLServer sqlDatabase;

        private int serial;

        private String area;
        private String district;
        private String name;
        private String latitude;
        private String longitude;

        private int statusId;
        private String status;

        private DateTime missionDate;

        private double averagePowerDensity;
        private double maxPowerDensity;

        private String actualLatitude;
        private String actualLongitude;

        private List<BASIC_STRUCTS.EMF_BAND_STRUCT> pointBands;

        public EMF()
        {
            sqlDatabase = new SQLServer();

            pointBands = new List<BASIC_STRUCTS.EMF_BAND_STRUCT>();
        }

        public bool InitializePoint(int mPointSerial)
        {
            serial = mPointSerial;

            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select emf_points.serial as point_serial,
	                                        emf_point_bands.serial as band_serial,
	                                        emf_points.status,
	                                        emf_points.name,
	                                        emf_points.area,
	                                        emf_points.district,
	                                        emf_points.latitude,
	                                        emf_points.longitude,
	                                        emf_points.actual_long,
	                                        emf_points.actual_lat,
	                                        emf_point_bands.band,
	                                        emf_point_bands.average_power_density,
	                                        emf_point_bands.max_power_density,
                        	                emf_point_status.status
                        from NTRA.dbo.emf_points
                        left join NTRA.dbo.emf_point_bands
                        on emf_points.serial = emf_point_bands.point_serial
                        left join NTRA.dbo.emf_point_status
                        on emf_points.status = emf_point_status.id
                        where emf_points.serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += mPointSerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 3;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 11;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                statusId = sqlDatabase.rows[0].sql_int[2];

                name = sqlDatabase.rows[0].sql_string[0];
                area = sqlDatabase.rows[0].sql_string[1];
                district = sqlDatabase.rows[0].sql_string[2];
                latitude = sqlDatabase.rows[0].sql_string[3];
                longitude = sqlDatabase.rows[0].sql_string[4];
                actualLongitude = sqlDatabase.rows[0].sql_string[5];
                actualLatitude = sqlDatabase.rows[0].sql_string[6];
                status = sqlDatabase.rows[0].sql_string[10];

                pointBands.Clear();

                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {
                    BASIC_STRUCTS.EMF_BAND_STRUCT tempPointBands = new BASIC_STRUCTS.EMF_BAND_STRUCT();

                    tempPointBands.emf_point_serial = serial;
                    if (sqlDatabase.rows[i].sql_string[7] != "")
                    {
                        tempPointBands.band = sqlDatabase.rows[i].sql_string[7];
                        tempPointBands.average_power_density = Double.Parse(sqlDatabase.rows[i].sql_string[8]);
                        tempPointBands.max_power_density = Double.Parse(sqlDatabase.rows[i].sql_string[9]);

                        if (tempPointBands.band != "")
                            pointBands.Add(tempPointBands);
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool InsertIntoEMFPoints()
        {
            String sqlQuery = "insert into NTRA.dbo.emf_points values(";
            sqlQuery += serial + ",'";
            sqlQuery += name + "','";
            sqlQuery += area + "','";
            sqlQuery += district + "','";
            sqlQuery += latitude + "','";
            sqlQuery += longitude + "','";
            sqlQuery += actualLongitude + "','";
            sqlQuery += actualLatitude + "',";
            sqlQuery += statusId + ", getdate());";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool InsertIntoEMFPointBands()
        {
            for (int i = 0; i < pointBands.Count; i++)
            {
                String sqlQuery = "insert into NTRA.dbo.emf_point_bands values(";
                sqlQuery += serial + ",";
                sqlQuery += (i + 1) + ",'";
                sqlQuery += pointBands[i].band + "','";
                sqlQuery += pointBands[i].average_power_density.ToString() + "','";
                sqlQuery += pointBands[i].max_power_density.ToString() + "',getdate());";

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;

                return true;
            }

            return true;
        }

        public bool GetNewSerial()
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select max(serial) from NTRA.dbo.emf_points";

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

        public bool EditEMFPoint()
        {
            String sqlQuery = "update NTRA.dbo.emf_points set name = '";
            sqlQuery += name + "' , area = '";
            sqlQuery += area + "' , district = '";
            sqlQuery += district + "' , latitude = '";
            sqlQuery += latitude + "' , longitude = '";
            sqlQuery += longitude + "' , actual_long = '";
            sqlQuery += actualLongitude + "' , actual_lat = '";
            sqlQuery += actualLatitude + "' , status = ";
            sqlQuery += statusId + " where serial = ";
            sqlQuery += serial;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool AddActualLongAndLatAndStatus()
        {
            String sqlQuery = "update NTRA.dbo.emf_points set actual_long = '";
            sqlQuery += actualLongitude.ToString() + "', and actual_lat = '";
            sqlQuery += actualLatitude + "' and status = ";
            sqlQuery += statusId + ";";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool DeletePointBands()
        {
            String sqlQuery = "Delete from NTRA.dbo.emf_point_bands where point_serial = ";
            sqlQuery += serial;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool IssueNewPoint()
        {
            if (!GetNewSerial())
                return false;

            if (!InsertIntoEMFPoints())
                return false;

            return true;
        }

        public bool AddPointReading()
        {
            if (!AddActualLongAndLatAndStatus())
                return false;

            if (!DeletePointBands())
                return false;

            if (!InsertIntoEMFPointBands())
                return false;

            return true;
        }

        public void SetSerial(int mSerial)
        {
            serial = mSerial;
        }
        public void SetArea(String mArea)
        {
            area = mArea;
        }

        public void SetDistrict(String mDistrict)
        {
            district = mDistrict;
        }

        public void SetName(String mName)
        {
            name = mName;
        }

        public void SetLong(String mLong)
        {
            longitude = mLong;
        }

        public void SetLat(String mLat)
        {
            latitude = mLat;
        }

        public void SetDate(DateTime mDate)
        {
            missionDate = mDate;
        }

        public void SetAveragePowerDensiity(double mAveragePowerDensity)
        {
            averagePowerDensity = mAveragePowerDensity;
        }

        public void SetMaxPowerDensiity(double mMaxPowerDensity)
        {
            maxPowerDensity = mMaxPowerDensity;
        }

        public void SetActualLat(String mActualLat)
        {
            actualLatitude = mActualLat;
        }

        public void SetActualLong(String mActualLong)
        {
            actualLongitude = mActualLong;
        }

        public void SetStatusId(int mId)
        {
            statusId = mId;
        }

        public void SetStatus(String mStatus)
        {
            status = mStatus;
        }

        public int GetSerial()
        {
            return serial;
        }
        public String GetArea()
        {
            return area;
        }

        public String GetDistrict()
        {
            return district;
        }

        public String GetName()
        {
            return name;
        }

        public String GetLong()
        {
            return longitude;
        }

        public String GetLat()
        {
            return latitude;
        }

        public DateTime GetDate()
        {
            return missionDate;
        }

        public Double GetAveragePowerDensiity()
        {
            return averagePowerDensity;
        }

        public Double GetMaxPowerDensiity()
        {
            return maxPowerDensity;
        }

        public String GetActualLat()
        {
            return actualLatitude;
        }

        public String GetActualLong()
        {
            return actualLongitude;
        }


        public int GetStatusId()
        {
            return statusId;
        }

        public String GetStatus()
        {
            return status;
        }
    }
}
