using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;
using System.Web.UI;
using MimeKit;
using MailKit.Net.Smtp;
using System.Runtime.Remoting.Messaging;
using MTGroupAutoGen.Utilities;

namespace MTGroupAutoGen.Models
{
    public class DBManager
    {
        public int stringOccurrence (string input, string substring)
        {
            int freq = 0;

            int index = input.IndexOf(substring);
            while (index >= 0)
            {
                index = input.IndexOf(substring, index + substring.Length);
                freq++;
            }
            return freq;
        }
        public Tuple<List<UserInfo>, int, long> SelectCompleteUser(string lemlist, string statuslist, string andOr, string query)
        {
            List<UserInfo> userInfoList = new List<UserInfo>();

            int selectIndex = query.ToLower().IndexOf("select");
            int fromIndex = query.ToLower().IndexOf("from");
            if (selectIndex != -1 && fromIndex != -1)
            {
                string columnSelected = query.Substring(selectIndex + 6, fromIndex - selectIndex - 6);
                int usernameIndex = columnSelected.ToLower().IndexOf("column_name");
                int workerNumberIndex = columnSelected.ToLower().IndexOf("column_name");
                int usernameOccurrence = stringOccurrence(columnSelected.ToLower(), "column_name");
                int workerNumberOccurrence = stringOccurrence(columnSelected.ToLower(), "column_name");

                if (usernameOccurrence != 1 || workerNumberOccurrence != 1)
                {
                    UserInfo userInfo = new UserInfo
                    {
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                    };
                    userInfoList.Add(userInfo);
                    return new Tuple<List<UserInfo>, int, long>(userInfoList, -2, 0);
                }
                if (usernameIndex > workerNumberIndex)
                {
                    UserInfo userInfo = new UserInfo
                    {
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                    };
                    userInfoList.Add(userInfo);
                    return new Tuple<List<UserInfo>, int, long>(userInfoList, -3, 0);
                }
            }
            


            SqlConnection sqlConnection = GetSqlConnectionMTGroup();
            SqlCommand sqlCommand;
            if (andOr == "OR")
            {
                try
                {
                    sqlCommand = new SqlCommand(query);
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandTimeout = int.MaxValue;
                    sqlConnection.Open();
                }
                catch (SqlException ex)
                {
                    sqlConnection.Close();
                    UserInfo userInfo = new UserInfo
                    {
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                    };
                    userInfoList.Add(userInfo);
                    return new Tuple<List<UserInfo>, int, long>(userInfoList, -1, -1);
                }
                
                
            }
            else
            {
                sqlCommand = new SqlCommand(query);
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandTimeout = int.MaxValue;
                sqlConnection.Open();
            }
            
            try
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                watch.Stop();
                if (watch.ElapsedMilliseconds > 40000 && andOr == "null")
                {
                    UserInfo userInfo = new UserInfo
                    {
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                    };
                    userInfoList.Add(userInfo);
                    return new Tuple<List<UserInfo>, int, long>(userInfoList, -1, (watch.ElapsedMilliseconds) / 1000);
                }
                int dataCount = 0;
                if (reader.FieldCount != 2)
                {
                    UserInfo userInfo = new UserInfo
                    {
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                        column_name = "null",
                    };
                    userInfoList.Add(userInfo);
                    return new Tuple<List<UserInfo>, int, long>(userInfoList, -4, 0);
                }
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ++dataCount;
                        if (dataCount <= 30000)
                        {
                            
                            string CompanyUsername = Convert.ToString(reader[reader.GetOrdinal("column_name")]);
                            string workerNo = Convert.ToString(reader[reader.GetOrdinal("column_name")]);
                            string[] userDetailArray = GetUserDetailFromDB(CompanyUsername);
                            if (dataCount == 1)
                            {
                                if (CompanyUsername.All(Char.IsLetter) == false || workerNo.All(Char.IsNumber) == false)
                                {
                                    UserInfo userInfoNull = new UserInfo
                                    {
                                        column_name = "null",
                                        column_name = "null",
                                        column_name = "null",
                                        column_name = "null",
                                        column_name = "null",
                                    };
                                    userInfoList.Add(userInfoNull);
                                    return new Tuple<List<UserInfo>, int, long>(userInfoList, -5, 0);
                                }
                            }
                            

                            UserInfo userInfo = new UserInfo
                            {
                                column_name = CompanyUsername,
                                column_name = workerNo,
                                column_name = userDetailArray[2],
                                column_name = userDetailArray[0],
                                column_name = userDetailArray[1],
                            };
                            userInfoList.Add(userInfo);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Datebase is empty!");
                }
                sqlConnection.Close();

                return new Tuple<List<UserInfo>, int, long>(userInfoList, dataCount, (watch.ElapsedMilliseconds) / 1000);
            }
            catch
            {
                UserInfo userInfo = new UserInfo
                {
                    column_name = "null",
                    column_name = "null",
                    column_name = "null",
                    column_name = "null",
                    column_name = "null",
                };
                userInfoList.Add(userInfo);
                return new Tuple<List<UserInfo>, int, long>(userInfoList, -1, -1);
            }
            
        }
        public string[] GetUserDetailFromDB(string username)
        {
            string[] userDetailArray = new string[3];
            SqlConnection sqlConnection = GetSqlConnectionReference();
            SqlCommand sqlCommand;
            sqlCommand = new SqlCommand("select query");
            sqlCommand.Parameters.AddWithValue("@column_name", username);
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    userDetailArray[0] = Convert.ToString(reader[reader.GetOrdinal("column_name")]);
                    userDetailArray[1] = Convert.ToString(reader[reader.GetOrdinal("column_name")]);
                    userDetailArray[2] = Convert.ToString(reader[reader.GetOrdinal("column_name")]);
                }
            }
            else
            {
                Console.WriteLine("Datebase is empty!");
            }
            sqlConnection.Close();

            return userDetailArray;
        }

        public string UpdateAutoGenRequest(string groupName, string groupOwner, string query)
        {
            string storedProcedureQuery;
            if (InsertQueryDoubleCheck(groupName))
            {
                storedProcedureQuery = "storedProcedureQuery";
            }
            else
            {
                storedProcedureQuery = "storedProcedureQuery";
            }
            SqlConnection sqlConnection = GetSqlConnectionMTAutoGen();
            SqlCommand sqlCommand;
            sqlCommand = new SqlCommand(storedProcedureQuery);
            sqlCommand.Parameters.AddWithValue("@column_name", groupName);
            sqlCommand.Parameters.AddWithValue("@column_name", "MTGroup");
            if (groupOwner.ToUpper().Contains("ADMINUSER"))
            {
                sqlCommand.Parameters.AddWithValue("@column_name", groupOwner);
            }
            else
            {
                sqlCommand.Parameters.AddWithValue("@column_name", "ADMINUSER;" + groupOwner);
            }

            sqlCommand.Parameters.AddWithValue("@column_name", "value");
            sqlCommand.Parameters.AddWithValue("@column_name", "value");
            sqlCommand.Parameters.AddWithValue("@column_name", "value");
            sqlCommand.Parameters.AddWithValue("@column_name", "value");
            sqlCommand.Parameters.AddWithValue("@column_name", "value");
            sqlCommand.Parameters.AddWithValue("@column_name", "value");
            sqlCommand.Parameters.AddWithValue("@column_name", query);
            sqlCommand.Parameters.AddWithValue("@column_name", @"value");
            sqlCommand.Parameters.AddWithValue("@column_name", "value");
            sqlCommand.Parameters.AddWithValue("@column_name", "value");
            sqlCommand.Parameters.AddWithValue("@column_name", "value");
            sqlCommand.Parameters.AddWithValue("@column_name", "");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandTimeout = int.MaxValue;

            try
            {
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
                return "success";
            }
            catch (SqlException ex)
            {
                sqlConnection.Close();
                return "error";
            }
        }

        public Boolean InsertQueryDoubleCheck(string groupName)
        {
            SqlConnection sqlConnection = GetSqlConnectionMTAutoGen();
            SqlCommand sqlCommand;
            sqlCommand = new SqlCommand("select query");

            sqlCommand.Parameters.AddWithValue("@groupName", groupName);
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                sqlConnection.Close();
                return true;
            }
            else
            {
                sqlConnection.Close();
                return false;
            }

        }

        public List<GroupInfo> EventualMemberUpdateQueueList()
        {
            List<GroupInfo> queuelist = new List<GroupInfo>();
            SqlConnection sqlConnection = GetSqlConnectionMTGroup();
            SqlCommand sqlCommand;
            sqlCommand = new SqlCommand("select query");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string[] groupFull = Convert.ToString(reader[reader.GetOrdinal("group_DN")]).Split(',');
                    GroupInfo groupInfo = new GroupInfo
                    {
                        groupName = groupFull[0].Substring(3),
                        priority = Convert.ToInt32(reader[reader.GetOrdinal("priority")]),

                    };
                    queuelist.Add(groupInfo);
                }
            }
            else
            {
                Console.WriteLine("Datebase is empty!");
            }
            sqlConnection.Close();

            return queuelist;
        }
        public Boolean GroupExistInQueue(string group, string type)
        {
            SqlConnection sqlConnection = GetSqlConnectionMTGroup();
            SqlCommand sqlCommand;
            if (type == "EventualMemberUpdate")
            {
                sqlCommand = new SqlCommand("select query");
            }
            else
            {
                sqlCommand = new SqlCommand("select query");
            }
                
            sqlCommand.Parameters.AddWithValue("@group", "%cn=" + group + ",%");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                sqlConnection.Close();
                return true;
            }
            else
            {
                sqlConnection.Close();
                return false;
            }
            
        }

        public Boolean WorkerNumberExist(int workerNumber)
        {
            SqlConnection sqlConnection = GetSqlConnectionReference();
            SqlCommand sqlCommand;
            sqlCommand = new SqlCommand("select query");

            sqlCommand.Parameters.AddWithValue("@workerNumber", workerNumber);
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                sqlConnection.Close();
                return true;
            }
            else
            {
                sqlConnection.Close();
                return false;
            }

        }

        public Boolean WorkerExistInDB(string column_name)
        {
            SqlConnection sqlConnection = GetSqlConnectionReference();
            SqlCommand sqlCommand;
            sqlCommand = new SqlCommand("select query");

            sqlCommand.Parameters.AddWithValue("@column_name", column_name);
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                sqlConnection.Close();
                return true;
            }
            else
            {
                sqlConnection.Close();
                return false;
            }

        }


        public void InsertWorkerNumber(int workerNumber, string type)
        {
            SqlConnection sqlConnection = GetSqlConnectionMTRep();
            SqlCommand sqlCommand;
            DateTime date = DateTime.Now;
            string date_str = date.ToString("yyyy/MM/dd,HH:mm:ss.fff");
            if (type == "WorkerMemberCheckService")
            {
                sqlCommand = new SqlCommand("INSERT query", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@column_name", workerNumber);
                sqlCommand.Parameters.AddWithValue("@column_name", date_str);
                sqlCommand.Parameters.AddWithValue("@column_name", date_str);
                sqlCommand.Parameters.AddWithValue("@column_name", 1);
                sqlCommand.Parameters.AddWithValue("@column_name", "");
                sqlCommand.Parameters.AddWithValue("@column_name", "");
                sqlCommand.Parameters.AddWithValue("@column_name", "");
            }
            else if (type == "SupervisorCheckService")
            {
                sqlCommand = new SqlCommand("INSERT query", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@column_name", workerNumber);
                sqlCommand.Parameters.AddWithValue("@column_name", date_str);
                sqlCommand.Parameters.AddWithValue("@column_name", date_str);
                sqlCommand.Parameters.AddWithValue("@column_name", 1);
                sqlCommand.Parameters.AddWithValue("@column_name", "");
            }
            else
            {
                sqlCommand = new SqlCommand("INSERT query", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@column_name", workerNumber);
                sqlCommand.Parameters.AddWithValue("@column_name", "UPD");
                sqlCommand.Parameters.AddWithValue("@column_name", date_str);
                sqlCommand.Parameters.AddWithValue("@column_name", 0);
                sqlCommand.Parameters.AddWithValue("@column_name", 1);
                sqlCommand.Parameters.AddWithValue("@column_name", date_str);
                sqlCommand.Parameters.AddWithValue("@column_name", 1);
                sqlCommand.Parameters.AddWithValue("@column_name", "");
                sqlCommand.Parameters.AddWithValue("@column_name", "");
            }
            try
            {
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (SqlException ex)
            {

            }
        }

        private SqlConnection GetSqlConnectionMTGroup()
        {
            return new SqlConnection("ConnectionString");
        }

        private SqlConnection GetSqlConnectionMTRep()
        {
            return new SqlConnection("ConnectionString");
        }

        private SqlConnection GetSqlConnectionMTAutoGen()
        {
            return new SqlConnection("ConnectionString");
        }

        private SqlConnection GetSqlConnectionReference()
        {
            return new SqlConnection("ConnectionString");
        }
    }

    public class IDJson
    {
        public int id { get; set; }
        public string alert_on { get; set; }
    }

    public class Input
    {
        public string inputString { get; set; }
        public string type { get; set; }
    }
    public class GroupInfo
    {
        public string groupName { get; set; }
        public int priority { get; set; }
    }
    public class WorkerInfo
    {
        public string username { get; set; }
        public int workerNumber { get; set; }
        public string changeCode { get; set; }
        public string requestedDateTime { get; set; }
    }
    public class ResultJson
    {
        public string result { get; set; }
        public List<string> exist { get; set; }
        public List<string> notExist { get; set; }
    }
    public class MemberComparison
    {
        public string memberName { get; set; }
        public char ldap { get; set; }
        public string ad { get; set; }
        public char referenceDB { get; set; }
        public char inLDAPGroup { get; set; }
        public char inADGroup { get; set; }
    }
}