using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ntra_missions
{
    public class Employee
    {
        SQLServer sql;

        private int id;
        private String userName;
        private String name;
        private int positionId;
        private String position;
        private int titleId;
        private String title;
        private int departmentId;
        private String department;
        private String email;
        private List<String> employeePhones;
             

        public Employee()
        {
            sql = new SQLServer();
            employeePhones = new List<string>();
        }

        public bool InitializeEmployeeInfo(int mId)
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select employee_info.employee_id, employee_info.position, employee_info.title, employee_info.department, name, employee_info.email, positions.position,
                                     titles.title, departments.department, number, employee_emails.email
                                     from NTRA.dbo.employee_info
                                     left join NTRA.dbo.positions
                                     on employee_info.position = positions.position_id
						             left join NTRA.dbo.titles
						             on employee_info.title = titles.id
						             left join NTRA.dbo.departments
						             on employee_info.department = departments.id
									 left join NTRA.dbo.employee_phones
									 on employee_info.employee_id = employee_phones.employee_id
									 left join NTRA.dbo.employee_emails
									 on employee_info.employee_id = employee_emails.employee_id
                                     where employee_info.employee_id = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += mId;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 4;
            SQL_COLUMN_COUNT_STRUCT.sql_string = 7;

            if (!sql.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if(sql.rows.Count != 0)
            {
                id = sql.rows[0].sql_int[0];
                positionId = sql.rows[0].sql_int[1];
                titleId = sql.rows[0].sql_int[2];
                departmentId = sql.rows[0].sql_int[3];

                name = sql.rows[0].sql_string[0];
                email = sql.rows[0].sql_string[1];
                position = sql.rows[0].sql_string[2];
                title = sql.rows[0].sql_string[3];
                department = sql.rows[0].sql_string[4];
                userName = sql.rows[0].sql_string[6];

                employeePhones.Clear();

                for(int i = 0; i < sql.rows.Count; i++)
                {
                    employeePhones.Add(sql.rows[i].sql_string[5]);
                }

            }
            else
            {
                return false;
            }


            return true;
        }

        public bool GetNewEmployeeId()
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"select max(employee_id)
                                     from NTRA.dbo.employee_emails";

            sqlQuery = sqlQueryPart1;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

            if (!sql.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                return false;

            if (sql.rows.Count != 0)
            {
                id = sql.rows[0].sql_int[0] + 1;
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool InsertIntoEmployeeInfo()
        {
            String sqlQuery = string.Empty;
            String sqlQueryPart1 = @"insert into NTRA.dbo.employee_info values (";

            sqlQuery = sqlQueryPart1;
            sqlQuery += id + ",'";
            sqlQuery += name + "',";
            sqlQuery += 1 + ", getdate(),";
            sqlQuery += 1 + ",";
            sqlQuery += departmentId + ",'";
            sqlQuery += email + "');";

            if (!sql.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool InsertIntoEmployeeEmails()
        {
            String sqlQuery = string.Empty;
            String sqlQueryPart1 = @"insert into NTRA.dbo.employee_emails values (";

            sqlQuery = sqlQueryPart1;
            sqlQuery += id + ",'" + userName + "', getdate());";

            if (!sql.InsertRows(sqlQuery))
                return false;


            return true;
        }

        public bool InsertIntoEmployeePasswords(String mPassword)
        {
            String sqlQuery = string.Empty;
            String sqlQueryPart1 = @"insert into NTRA.dbo.employee_password values (";

            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(mPassword));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }

            sqlQuery = sqlQueryPart1;
            sqlQuery += id + ",'" + hash + "', getdate());";

            if (!sql.InsertRows(sqlQuery))
                return false;


            return true;
        }

        public bool UpdateEmployeeInfo()
        {
            String sqlQuery = string.Empty;
            String sqlQueryPart1 = @"update NTRA.dbo.employee_info set employee_id =";

            sqlQuery = sqlQueryPart1;
            sqlQuery += id + ", name = '";
            sqlQuery += name + "', position = ";
            sqlQuery += positionId + ", title = ";
            sqlQuery += titleId + ", department = ";
            sqlQuery += departmentId + ", email = '";
            sqlQuery += email + "' where employee_id = ";
            sqlQuery += id;

            if (!sql.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public bool InsertIntoEmployeeTelephones()
        {
            for (int i = 0; i < employeePhones.Count; i++)
            {
                String sqlQuery = string.Empty;
                String sqlQueryPart1 = @"insert into NTRA.dbo.employee_phones values (";

                sqlQuery = sqlQueryPart1;
                sqlQuery += id + ",'" + employeePhones[i] + "', getdate());";

                if (!sql.InsertRows(sqlQuery))
                    return false;
            }

            return true;
        }

        public bool DeleteEmployeeTelephones()
        {
            String sqlQuery = string.Empty;

            String sqlQueryPart1 = @"delete from NTRA.dbo.employee_phones where employee_id = ";

            sqlQuery = sqlQueryPart1;
            sqlQuery += id;

            if (!sql.InsertRows(sqlQuery))
                return false;

            return true;
        }

        public int GetEmployeeId()
        {
            return id;
        }

        public int GetEmployeePositionId()
        {
            return positionId;
        }

        public String GetEmployeeUserName()
        {
            return userName;
        }

        public String GetEmployeeName()
        {
            return name;
        }

        public String GetEmployeePosition()
        {
            return position;
        }

        public int GetEmployeeTitleId()
        {
            return titleId;
        }

        public String GetEmployeeTitle()
        {
            return title;
        }

        public int GetEmloyeeDepartmentId()
        {
            return departmentId;
        }

        public String GetEmployeeDepartment()
        {
            return department;
        }

        public void GetEmployeePhones(ref List<String> temp)
        {
            temp.Clear();

            for(int i = 0; i < employeePhones.Count; i++)
            {
                temp.Add(employeePhones[i]);
            }
        }

        public List<String> GetEmployeePhones()
        {
            return employeePhones;
        }

        public String GetEmployeeEmail()
        {
            return email;
        }

        public void SetEmployeeId(int mId)
        {
            id = mId;
        }

        public void SetEmployeePositionId(int mPositionId)
        {
            positionId = mPositionId;
        }

        public void SetEmployeeUserName(String mUserName)
        {
            userName = mUserName;
        }

        public void SetEmployeeName(String mName)
        {
            name = mName;
        }

        public void SetEmployeePosition(String mPosition)
        {
            position = mPosition;
        }

        public void SetEmployeeTitleId(int mTitleId)
        {
            titleId = mTitleId;
        }

        public void SetEmployeeTitle(String mTitle)
        {
            title = mTitle;
        }

        public void SetEmloyeeDepartmentId(int mDepartmentId)
        {
            departmentId = mDepartmentId;
        }

        public void SetEmployeeDepartment(String mDepartment)
        {
            department = mDepartment;
        }

        public void SetEmployeePhones(ref List<String> temp)
        {
            employeePhones.Clear();

            for (int i = 0; i < temp.Count; i++)
            {
                employeePhones.Add(temp[i]);
            }
        }

        public void SetEmployeeEmail(String mEmail)
        {
            email = mEmail;
        }

    }
}
