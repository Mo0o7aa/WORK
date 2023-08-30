using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntra_missions
{
    public class Site
    {
        private SQLServer sql;

        private int companySerial;
        private String companyName;

        private int siteSerial;

        private int cityId;
        private String city;

        private String region;

        private String latitude;
        private String longitude;

        private String siteNumber;

        private int addedById;
        private String addedBy;


        public Site()
        {
            sql = new SQLServer();
        }

        public bool InitializeSite(int mCompanySerial , int mSiteSerial)
        {
            siteSerial = mSiteSerial;
            companySerial = mCompanySerial;

            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select company_sites.company_Serial,
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

            String sqlQueryPart2 = " and company_sites.serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += mCompanySerial;
            sqlQuery += sqlQueryPart2;
            sqlQuery += mSiteSerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 3;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 7;

            if (!sql.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sql.rows.Count != 0)
            {
                companySerial = sql.rows[0].sql_int[0];
                cityId = sql.rows[0].sql_int[1];
                addedById = sql.rows[0].sql_int[2];

                companyName = sql.rows[0].sql_string[0];
                city = sql.rows[0].sql_string[1];
                siteNumber = sql.rows[0].sql_string[2];
                latitude = sql.rows[0].sql_string[3];
                longitude = sql.rows[0].sql_string[4];
                region = sql.rows[0].sql_string[5];
                addedBy = sql.rows[0].sql_string[6];

                return true;
            }

            return true;
        }

        public bool IssueNewSite()
        {
            if (!GetNewSiteSerial())
                return false;

            if (!InsertIntoCompanySites())
                return false;

            return true;
        }

        public bool EditSite()
        {
            if (!UpdateCompanySite())
                return false;

            return true;
        }

        public bool GetNewSiteSerial()
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select max(serial)
                                     from NTRA.dbo.company_sites
                                     where company_serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sql.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sql.rows.Count != 0)
            {
                siteSerial = sql.rows[0].sql_int[0] + 1;
            }
            else
                siteSerial = 1;

            if (siteSerial == 0)
                siteSerial = 1;

            return true;
        }

        public bool InsertIntoCompanySites()
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"insert into NTRA.dbo.company_sites values(";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial + ", ";
            sqlQuery += siteSerial + ",'";
            sqlQuery += latitude + "','";
            sqlQuery += longitude + "',";
            sqlQuery += cityId + ",N'";
            sqlQuery += region + "',";
            sqlQuery += addedById + ",getdate(),'";
            sqlQuery += siteNumber + "')";

            if (!sql.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool UpdateCompanySite()
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"update NTRA.dbo.company_sites set company_serial = ";
            String sqlQueryPart2 = " where company_serial = ";
            String sqlQueryPart3 = " and serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial + ", serial = ";
            sqlQuery += siteSerial + ", lat = '";
            sqlQuery += latitude + "',long = '";
            sqlQuery += longitude + "', city = ";
            sqlQuery += cityId + ", region = '";
            sqlQuery += region + "', added_by = ";
            sqlQuery += addedById + ",date_added = getdate(), site_number = '";
            sqlQuery += siteNumber + "'";
            sqlQuery += sqlQueryPart2;
            sqlQuery += companySerial;
            sqlQuery += sqlQueryPart3;
            sqlQuery += siteSerial;

            if (!sql.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public int GetCompanySerial()
        {
            return companySerial;
        }

        public String GetCompanyName()
        {
            return companyName;
        }

        public int GetSiteSerial()
        {
            return siteSerial;
        }

        public int GetCityId()
        {
            return cityId;
        }

        public String GetCity()
        {
            return city;
        }

        public String GetRegion()
        {
            return region;
        }

        public String GetLat()
        {
            return latitude;
        }
        public String GetLong()
        {
            return longitude;
        }
        public String GetSiteNumber()
        {
            return siteNumber;
        }

        public int GetAddedById()
        {
            return addedById;
        }

        public String GetAddedBy()
        {
            return addedBy;
        }


        public void SetCompanySerial(int mSerial)
        {
            companySerial = mSerial;
        }

        public void SetCompanyName(String mName)
        {
            companyName = mName;
        }

        public void SetSiteSerial(int mSerial)
        {
            siteSerial = mSerial;
        }

        public void SetCityId(int mCityId)
        {
            cityId = mCityId;
        }

        public void SetCity(String mCity)
        {
            city = mCity;
        }

        public void SetRegion(String mRegion)
        {
            region = mRegion;
        }

        public void SetLat(String mLat)
        {
            latitude = mLat;
        }
        public void SetLong(String mLong)
        {
            longitude = mLong;
        }
        public void SetSiteNumber(String mSiteNumber)
        {
            siteNumber = mSiteNumber;
        }

        public void SetAddedById(int mId)
        {
            addedById = mId;
        }

        public void SetAddedBy(String mAddedBy)
        {
            addedBy = mAddedBy;
        }

    }
}
