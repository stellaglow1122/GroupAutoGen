using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTGroupAutoGen.Models;
using System.Web.Script.Serialization;
using MimeKit;
using MailKit.Net.Smtp;
using System.IO;
using MTGroupAutoGen.Utilities;
using Newtonsoft.Json.Linq;

namespace MTGroupAutoGen.Controllers
{
    public class HomeController : Controller
    {
        MTGroupManager mtgroupManager = new MTGroupManager();
        DBManager dbManager = new DBManager();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public string ShowUserDetail(CourseID courseID)
        {
            string groupOwner = courseID.groupOwner.Replace(" ", "");
            string[] ownerArray = groupOwner.Split(new char[2] { ';', ',' });
            bool containsInvalidUsername = false;
            foreach (string owner in ownerArray)
            {
                if (owner != "")
                {
                    if (mtgroupManager.FindUser(owner.ToLower()).Contains("error"))
                    {
                        containsInvalidUsername = true;
                        break;
                    }
                }
            }
            string Html = "";
            if (!containsInvalidUsername)
            {
                Log.Info("Start query data for below query:");
                Log.Info(courseID.query);
                var tuple = dbManager.SelectCompleteUser(courseID.courseIDList.Replace(";", ","), courseID.statusList, courseID.andOr, courseID.query);
                Log.Info("End running query.");

                List<UserInfo> userInfoList = tuple.Item1;
                int userInfoCount = tuple.Item2;
                long executionTime = tuple.Item3;

                Html += "[" + userInfoCount.ToString() + "," + executionTime.ToString() + "]";

                if (userInfoCount > -1)
                {
                    // Company ID only
                    Html = Html + "<h3>" + Convert.ToString(userInfoCount) + " users match the condition.</h3></br>";
                    Html += "<b>Company ID: </b>";
                    foreach (UserInfo userInfo in userInfoList)
                    {
                        Html = Html + userInfo.column_name + "; ";
                    }
                    Log.Info(Convert.ToString(userInfoCount) + " users returned for the query.");
                    Log.Info("Execution time is " + executionTime.ToString() + " for the query.");
                }
                if (userInfoCount == -1)
                {
                    Html = "Because the query is not valid or runs over 30 secs which takes too long we will not accept the query. (" + executionTime + " s)";
                    Log.Info("The query is not valid or runs over 30 secs(" + executionTime + " s)");
                }
                if (userInfoCount == -2)
                {
                    Html = "There should be exactly one username and one worker number being selected so we will not accept the query.";
                    Log.Info("More than one username/worker number detected.");
                }
                if (userInfoCount == -3)
                {
                    Html = "Username should be selected before worker number so we will not accept the query.";
                    Log.Info("Username is not selected before worker number.");
                }
                if (userInfoCount == -4)
                {
                    Html = "Count of output column should be 2 so we will not accept the query.";
                    Log.Info("Count of output column is not 2.");
                }
                if (userInfoCount == -5)
                {
                    Html = "Value of output column should be characters for username and numbers for worker number so we will not accept the query.";
                    Log.Info("Value of output column is not characters for username or numbers for worker number.");
                }
            }
            else
            {
                Html += "[ 0 , 0 ]";
                Html += "<h3>Invalid username is detected in the owner list so we will not accept. Please check and validate again.</h3></br>";
            }


            return Html;
        }
        public string GenerateGroup(CourseID courseID)
        {
            //MTGroupManager mtgroupManager = new MTGroupManager();
            string isValidGroupName = mtgroupManager.CheckValidGroupName(courseID.groupName);
            if (isValidGroupName.Contains("true"))
            {
                if (mtgroupManager.CreateGroup(courseID.groupName) != "error")
                {
                    //DBManager dbManager = new DBManager();
                    if (dbManager.UpdateAutoGenRequest(courseID.groupName, courseID.groupOwner, courseID.query) != "error")
                    {
                        if (dbManager.InsertQueryDoubleCheck(courseID.groupName))
                        {
                            return "success";
                        }
                        else
                        {
                            return "Failed to insert into database!";
                        } 
                    }
                    else
                    {
                        return "Failed to insert into database!";
                    }
                    
                }
                else
                {
                    Log.Error("Failed to create group " + courseID.groupName);
                    return "Failed to create group!";
                }
                
            }
            else if (isValidGroupName.Contains("false"))
            {
                if (mtgroupManager.CheckValidGroupOwnerName(courseID.groupName, User.Identity.Name.Replace("DOMAINNAME\\", "")).Contains("true") || mtgroupManager.CheckValidGroupContactName(courseID.groupName, User.Identity.Name.Replace("DOMAINNAME\\", "")).Contains("true"))
                {
                    if (dbManager.UpdateAutoGenRequest(courseID.groupName, courseID.groupOwner, courseID.query) != "error")
                    {
                        if (dbManager.InsertQueryDoubleCheck(courseID.groupName))
                        {
                            return "success";
                        }
                        else
                        {
                            return "Failed to insert into database!";
                        }
                        
                    }
                    else
                    {
                        return "Failed to insert into database!";
                    }
                }
                else
                {
                    ServiceNowManager serviceNowManager = new ServiceNowManager();
                    if (!serviceNowManager.IsOverTenRequests(User.Identity.Name.Replace("DOMAINNAME\\", "")))
                    {
                        return "Group already exists!";
                    }
                    else
                    {
                        return "Group already exists and more than ten requests!";
                    }

                    
                }
                
            }
            else
            {
                Log.Error("Failed to create group " + courseID.groupName);
                return "Failed to create group!";
            }
            
        }
        public string AddGroupOwner(CourseID courseID)
        {
            //MTGroupManager mtgroupManager = new MTGroupManager();
            string groupOwner = courseID.groupOwner.Replace(" ", "");
            if (!courseID.groupOwner.ToUpper().Contains("ADMINUSER"))
            {
                groupOwner = "ADMINUSER;" + groupOwner;
            }
            string[] ownerArray = groupOwner.Split(new char[2] { ';', ',' });
            int errorCount = 0;
            foreach (string owner in ownerArray)
            {
                if (owner != "")
                {
                    if (mtgroupManager.CheckValidGroupOwnerName(courseID.groupName, owner).Contains("false"))
                    {
                        if (mtgroupManager.AddGroupOwner(courseID.groupName, owner.Replace(" ", "")) == "error")
                        {
                            ++errorCount;
                        }
                    }
                        
                    
                }
            }
            if (errorCount == 0)
            {
                return "success";
            }
            else
            {
                Log.Error("Error occurred while adding the owner for group " + courseID.groupName);
                return "Error occurred while adding the owner.";
            }
            

        }
        public string CreateSNTask(CourseID courseID)
        {
            ServiceNowManager serviceNowManager = new ServiceNowManager();
            string snReturn = serviceNowManager.CreateTask(User.Identity.Name.Replace("DOMAINNAME\\", ""), courseID.groupName, courseID.groupOwner, courseID.query);
            if (snReturn != "error")
            {
                var snResult = JObject.Parse(snReturn);
                Log.Info(snResult["result"]["request_number"] + " submitted for group " + courseID.groupName);
                return snReturn;
            }
            else
            {
                Log.Error("Failed to create task for group " + courseID.groupName);
                Log.Error("Requestor for failed group " + courseID.groupName + " : " + User.Identity.Name.Replace("DOMAINNAME\\", ""));
                Log.Error("Group owner for failed group " + courseID.groupName + " : " + courseID.groupOwner);
                Log.Error("Query for failed group " + courseID.groupName + " : " + courseID.query);
                return "Failed to create the task!";
            }
        }
        public string CreateSNTaskForApproval(CourseID courseID)
        {
            ServiceNowManager serviceNowManager = new ServiceNowManager();
            string snReturn = serviceNowManager.AutoGenQueryApproval(User.Identity.Name.Replace("DOMAINNAME\\", ""), courseID.groupName, courseID.groupOwner, courseID.query, courseID.statusList, courseID.andOr);
            if (snReturn != "error")
            {
                var snResult = JObject.Parse(snReturn);
                Log.Info(snResult["result"]["request_number"] + " submitted for group " + courseID.groupName);
                return snReturn;
            }
            else
            {
                Log.Error("Failed to create task for group " + courseID.groupName);
                Log.Error("Requestor for failed group " + courseID.groupName + " : " + User.Identity.Name.Replace("DOMAINNAME\\", ""));
                Log.Error("Group owner for failed group " + courseID.groupName + " : " + courseID.groupOwner);
                Log.Error("Query for failed group " + courseID.groupName + " : " + courseID.query);
                return "Failed to create the task!";
            }
        }
    }
}