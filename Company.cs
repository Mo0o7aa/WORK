using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntra_missions
{
    public class Company
    {
        SQLServer sqlDatabase;

        Employee addedBy;

        private String companyName;
        private int companySerial;

        private int companyFieldOfWorkId;
        private String companyFieldOfWork;

        private List<BASIC_STRUCTS.CONTACT_STRUCT> contacts;
        private List<BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT> companyCFandBW;
        private List<BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT> companyStartandStop;

        public Company()
        {
            sqlDatabase = new SQLServer();

            addedBy = new Employee();
            contacts = new List<BASIC_STRUCTS.CONTACT_STRUCT>();
            companyCFandBW = new List<BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT>();
            companyStartandStop = new List<BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT>();
        }

        public bool InitializeCompanyInfo(int mSerial)
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"with get_centre_frequency_unit as (select id, factor, unit from NTRA.dbo.band_units),

                                            get_bw_unit as (select id, factor, unit from NTRA.dbo.band_units),
                                            
                                            get_start_frequency_unit as (select id, factor, unit from NTRA.dbo.band_units),
                                            
                                            get_stop_frequency_unit as (select id, factor, unit from NTRA.dbo.band_units)
                                            
                                     select companies.serial as company_serial,
                                            companies.work_field as work_field_id,
                                            companies.added_by as added_by_id,
                                            contacts.id as contact_id,
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
                                            company_centre_frequency.centre_frequency,
                                            company_centre_frequency.bandwidth,
											company_start_stop_frequency.start_frequency,
											company_start_stop_frequency.stop_frequency,
                                            companies.name as company_name,
                                            company_field_of_work.field_of_work as work_field,
                                            contact_name,
                                            phone,
                                            contacts.email,
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
                                     where companies.serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += mSerial;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 10;
            SQL_COLUMN_COUNT_STRUCT.sql_bigint = 4;
            SQL_COLUMN_COUNT_STRUCT.sql_decimal = 4;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 9;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {

                bool cf_bw;

                companySerial = sqlDatabase.rows[0].sql_int[0];
                companyFieldOfWorkId = sqlDatabase.rows[0].sql_int[1];

                if (!addedBy.InitializeEmployeeInfo(sqlDatabase.rows[0].sql_int[2]))
                    return false;

                if (sqlDatabase.rows[0].sql_int[4] != 0)
                {
                    cf_bw = true;
                }
                else
                    cf_bw = false;

                companyName = sqlDatabase.rows[0].sql_string[0];
                companyFieldOfWork = sqlDatabase.rows[0].sql_string[1];

                contacts.Clear();

                for (int i = 0; i < sqlDatabase.rows.Count; i++)
                {

                    BASIC_STRUCTS.CONTACT_STRUCT tempContact = new BASIC_STRUCTS.CONTACT_STRUCT();

                    tempContact.contact_id = sqlDatabase.rows[i].sql_int[3];

                    tempContact.contact_name = sqlDatabase.rows[i].sql_string[2];
                    tempContact.contact_phone = sqlDatabase.rows[i].sql_string[3];
                    tempContact.contact_email = sqlDatabase.rows[i].sql_string[4];

                    if (tempContact.contact_id != 0 && !contacts.Exists(x1 => x1.contact_id == tempContact.contact_id))
                        contacts.Add(tempContact);

                    if (cf_bw == true)
                    {
                        BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT centre_frequencies = new BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT();


                        centre_frequencies.centre_frequency = sqlDatabase.rows[i].sql_decimal[0];
                        centre_frequencies.bandwidth = sqlDatabase.rows[i].sql_decimal[1];

                        centre_frequencies.serial = sqlDatabase.rows[i].sql_int[4];
                        centre_frequencies.centre_frequency_unit_id = sqlDatabase.rows[i].sql_int[6];
                        centre_frequencies.bandwidth_unit_id = sqlDatabase.rows[i].sql_int[7];
                        
                        centre_frequencies.cf_factor = sqlDatabase.rows[i].sql_bigint[0];
                        centre_frequencies.bw_factor = sqlDatabase.rows[i].sql_bigint[1];

                        centre_frequencies.centre_frequency_unit = sqlDatabase.rows[i].sql_string[5];
                        centre_frequencies.bandwidth_unit = sqlDatabase.rows[i].sql_string[6];

                        centre_frequencies.max = (centre_frequencies.centre_frequency * centre_frequencies.cf_factor) + (centre_frequencies.bandwidth * centre_frequencies.bw_factor);
                        centre_frequencies.min = (centre_frequencies.centre_frequency * centre_frequencies.cf_factor) - (centre_frequencies.bandwidth * centre_frequencies.bw_factor);

                        if(i == 0 || !companyCFandBW.Exists(x1 => x1.serial == centre_frequencies.serial))
                            companyCFandBW.Add(centre_frequencies);
                    }
                    else
                    {
                        BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT start_frequencies = new BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT();


                        start_frequencies.start_frequency = sqlDatabase.rows[i].sql_decimal[2];
                        start_frequencies.stop_frequency = sqlDatabase.rows[i].sql_decimal[3];

                        start_frequencies.serial = sqlDatabase.rows[i].sql_int[5];
                        start_frequencies.start_frequency_unit_id = sqlDatabase.rows[i].sql_int[8];
                        start_frequencies.stop_frequency_unit_id = sqlDatabase.rows[i].sql_int[9];
                        
                        start_frequencies.start_factor = sqlDatabase.rows[i].sql_bigint[2];
                        start_frequencies.stop_factor = sqlDatabase.rows[i].sql_bigint[3];
                        
                        start_frequencies.start_frequency_unit = sqlDatabase.rows[i].sql_string[7];
                        start_frequencies.stop_frequency_unit = sqlDatabase.rows[i].sql_string[8];

                        start_frequencies.max = start_frequencies.stop_frequency * int.Parse(start_frequencies.stop_factor.ToString());
                        start_frequencies.min = start_frequencies.start_frequency * int.Parse(start_frequencies.start_factor.ToString());

                        if(i == 0 || !companyStartandStop.Exists(x1 => x1.serial == start_frequencies.serial))
                            companyStartandStop.Add(start_frequencies);
                    }
                }

            }
            else
            {
                return false;
            }


            return true;
        }
        public bool InitializeAddedBy(int mEmployeeId)
        {
            if (!addedBy.InitializeEmployeeInfo(mEmployeeId))
                return false;

            return true;
        }

        public bool GetNewCompanySerial()
        {

            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select max(serial) from NTRA.dbo.companies";

            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sqlDatabase.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sqlDatabase.rows.Count != 0)
            {
                companySerial = sqlDatabase.rows[0].sql_int[0] + 1;
            }

            if (companySerial == 0)
                companySerial = 1;

            return true;
        }

        public bool IssueNewCompany()
        {
            if (!GetNewCompanySerial())
                return false;

            if (!InsertIntoCompanies())
                return false;

            if (!InsertIntoContacts())
                return false;

            if (companyStartandStop.Count > 0)
            {
                if (!InsertIntoCompanyStartFrequencies())
                    return false;
            }
            else
            {
                if (!InsertIntoCompanyCentreFrequency())
                    return false;
            }

            return true;

        }

        public bool EditCompanyContacts()
        {
            if (!DeleteContacts())
                return false;

            if (!InsertIntoContacts())
                return false;

            if(companyStartandStop.Count > 0)
            {
                if (!DeleteCompanyStartFrequencies())
                    return false;
                if (!InsertIntoCompanyStartFrequencies())
                    return false;
            }
            else
            {
                if (!DeleteCompanyCentreFrequency())
                    return false;
                if (!InsertIntoCompanyCentreFrequency())
                    return false;
            }

            return true;

        }

        public bool InsertIntoCompanies()
        {
            String sqlQuery = string.Empty;
            String sqlQueryPart1 = @"insert into NTRA.dbo.companies values (";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial + ",N'";
            sqlQuery += companyName + "',";
            sqlQuery += addedBy.GetEmployeeId() + ", getdate(),";
            sqlQuery += companyFieldOfWorkId + ");";

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool InsertIntoContacts()
        {
            int mContactId = 1;

            for (int i = 0; i < contacts.Count; i++)
            {
                String sqlQuery = string.Empty;
                String sqlQueryPart1 = @"insert into NTRA.dbo.contacts values (";

                sqlQuery = sqlQueryPart1;
                sqlQuery += companySerial + ",";
                sqlQuery += mContactId + ",N'";
                sqlQuery += contacts[i].contact_name + "','";
                sqlQuery += contacts[i].contact_phone + "','";
                sqlQuery += contacts[i].contact_email + "',";
                sqlQuery += contacts[i].contact_added_by_id + ",getdate());";

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;

                mContactId++;
            }

            return true;
        }

        public bool InsertIntoCompanyStartFrequencies()
        {
            int mSerial = 1;

            for (int i = 0; i < companyStartandStop.Count; i++)
            {
                String sqlQuery = string.Empty;
                String sqlQueryPart1 = @"insert into NTRA.dbo.company_start_stop_frequency values (";

                sqlQuery = sqlQueryPart1;
                sqlQuery += companySerial + ",";
                sqlQuery += mSerial + ",";
                sqlQuery += companyStartandStop[i].start_frequency + ",";
                sqlQuery += companyStartandStop[i].start_frequency_unit_id + ",";
                sqlQuery += companyStartandStop[i].stop_frequency + ",";
                sqlQuery += companyStartandStop[i].stop_frequency_unit_id + ",";
                sqlQuery += addedBy.GetEmployeeId() + ",getdate());";

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;

                mSerial++;
            }

            return true;
        }

        public bool InsertIntoCompanyCentreFrequency()
        {

            int mSerial = 1;

            for (int i = 0; i < companyCFandBW.Count; i++)
            {
                String sqlQuery = string.Empty;
                String sqlQueryPart1 = @"insert into NTRA.dbo.company_centre_frequency values (";

                sqlQuery = sqlQueryPart1;
                sqlQuery += companySerial + ",";
                sqlQuery += mSerial + ",";
                sqlQuery += companyCFandBW[i].centre_frequency + ",";
                sqlQuery += companyCFandBW[i].centre_frequency_unit_id + ",";
                sqlQuery += companyCFandBW[i].bandwidth + ",";
                sqlQuery += companyCFandBW[i].bandwidth_unit_id + ",";
                sqlQuery += addedBy.GetEmployeeId() + ",getdate());";

                if (!sqlDatabase.InsertRows(sqlQuery))
                    return false;

                mSerial++;
            }

            return true;
        }

        public bool DeleteFromCompanies()
        {
            String sqlQuery = string.Empty;
            String sqlQueryPart1 = @"delete from NTRA.dbo.companies where serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool DeleteContacts()
        {
            String sqlQuery = string.Empty;
            String sqlQueryPart1 = @"delete from NTRA.dbo.contacts where company_serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;


            return true;
        }

        public bool DeleteCompanyStartFrequencies()
        {
            String sqlQuery = string.Empty;
            String sqlQueryPart1 = @"delete from NTRA.dbo.company_start_stop_frequency where company_serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial;

            if (!sqlDatabase.InsertRows(sqlQuery))
                return false;


            return true;
        }

        public bool DeleteCompanyCentreFrequency()
        {
            String sqlQuery = string.Empty;
            String sqlQueryPart1 = @"delete from NTRA.dbo.company_centre_frequency where company_serial = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += companySerial;

            if (!sqlDatabase.InsertRows(sqlQuery))
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

        public int GetCompanyFieldOfWorkId()
        {
            return companyFieldOfWorkId;
        }

        public String GetCompanyFieldOfWork()
        {
            return companyFieldOfWork;
        }

        public List<BASIC_STRUCTS.CONTACT_STRUCT> GetCompanyContacts()
        {
            return contacts;
        }

        public void GetCompanyContacts(ref List<BASIC_STRUCTS.CONTACT_STRUCT> temp)
        {
            temp.Clear();

            for(int i = 0; i < contacts.Count; i++)
            {
                temp.Add(contacts[i]);
            }
        }

        public List<BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT> GetCompanyStartFrequencies()
        {
            return companyStartandStop;
        }

        public List<BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT> GetCompanyCFandBw()
        {
            return companyCFandBW;
        }

        public void SetCompanySerial(int mId)
        {
            companySerial = mId;
        }

        public void SetCompanyName(String mName)
        {
            companyName = mName;
        }

        public void SetCompanyFieldOfWorkId(int mId)
        {
            companyFieldOfWorkId = mId;
        }

        public void SetCompanyFieldOfWork(String mField)
        {
            companyFieldOfWork = mField;
        }

        public void SetCompanyContacts(List<BASIC_STRUCTS.CONTACT_STRUCT> temp)
        {
            contacts.Clear();

            for(int i = 0; i < temp.Count; i++)
            {
                contacts.Add(temp[i]);
            }
        }


        public void SetCompanyStartFrequencies(List<BASIC_STRUCTS.COMPANY_START_AND_STOP_FREQUENCY_STRUCT> mList)
        {
            companyStartandStop.Clear();

            for(int i = 0; i < mList.Count; i++)
            {
                companyStartandStop.Add(mList[i]);
            }
        }

        public void SetCompanyCfAndBW(List<BASIC_STRUCTS.COMPANY_CENTRE_FREQUENCY_BW_STRUCT> mList)
        {
            companyCFandBW.Clear();

            for (int i = 0; i < mList.Count; i++)
            {
                companyCFandBW.Add(mList[i]);
            }
        }
    }
}
