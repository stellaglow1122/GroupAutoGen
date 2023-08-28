using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using MTGroupAutoGen.Utilities;

namespace MTGroupAutoGen.Models
{
    class MTGroupManager
    {
        // Get Consumer Key, and Consumer Secret by creating an application
        // in the API store and subscribing the application to MTGroups Web API.

        // Apigee key & secret
        private const string _consumerKey = "Consumer Key";
        private const string _consumerSecret = "Consumer Secret";

        private AccessToken _accessToken;
        //private CmdLineArgs _cmdLineArgs;
        private StringBuilder _errorMessage;
        private string _apiGateway = "API host";
        private string _apiContext = "API Context";

        /// <summary>
        /// This method gets the MTGroup contacts.
        /// </summary>
        /// <remarks>
        /// This method demonstrates the code to initiate a GET MTGroup Web API call to get MTGroup contacts.
        /// </remarks>
        /// <param name="httpClient">A http client.</param>
        /// <returns></returns>

        public string CreateGroup(string groupName)
        {
            int tryCount = 0;
            string res = "";
            do
            {
                try
                {
                    if (IsTokenExpire())
                    {
                        GetAccessToken();
                    }
                    HttpClient httpClient = new HttpClient();
                    httpClient.Timeout = TimeSpan.FromMinutes(30);
                    // Add User-Agent header.
                    //httpClient.DefaultRequestHeaders.UserAgent.Clear();
                    //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(System.AppDomain.CurrentDomain.FriendlyName);
                    // Add Accept header.
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                    // Add Authorization header.
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.access_token);
                    // Call API and get response.
                    var data = new StringContent("String");
                    HttpResponseMessage response = httpClient.PostAsync(new Uri(String.Format("???={0}", groupName)), data).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        res = response.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        ++tryCount;
                        res = "";
                        Thread.Sleep(3000);
                        //error
                    }
                }
                catch (HttpRequestException ex)
                {
                    ++tryCount;
                    res = "";
                    Thread.Sleep(3000);
                    //error
                }
            } while (tryCount < 3);
            return "error";
        }

        public string AddGroupOwner(string groupName, string groupOwner)
        {
            int tryCount = 0;
            string res = "";
            do
            {
                try
                {
                    if (IsTokenExpire())
                    {
                        GetAccessToken();
                    }
                    HttpClient httpClient = new HttpClient();
                    // Add User-Agent header.
                    //httpClient.DefaultRequestHeaders.UserAgent.Clear();
                    //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(System.AppDomain.CurrentDomain.FriendlyName);
                    // Add Accept header.
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                    // Add Authorization header.
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.access_token);
                    // Call API and get response.
                    var data = new StringContent("String");
                    HttpResponseMessage response = httpClient.PostAsync(new Uri(String.Format("???={0}&???={1}", groupName, groupOwner)), data).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        res = response.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        ++tryCount;
                        res = "";
                        Thread.Sleep(3000);
                        //error
                    }
                }
                catch (HttpRequestException ex)
                {
                    ++tryCount;
                    res = "";
                    Thread.Sleep(3000);
                    //error
                }
            } while (tryCount < 3);
            return "error";
        }


        public string CheckValidGroupName(string groupName)
        {
            int tryCount = 0;
            string res = "";
            do
            {
                try
                {
                    if (IsTokenExpire())
                    {
                        GetAccessToken();
                    }
                    HttpClient httpClient = new HttpClient();
                    // Add User-Agent header.
                    //httpClient.DefaultRequestHeaders.UserAgent.Clear();
                    //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(System.AppDomain.CurrentDomain.FriendlyName);
                    // Add Accept header.
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                    // Add Authorization header.
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.access_token);
                    // Call API and get response.
                    HttpResponseMessage response = httpClient.GetAsync(new Uri(String.Format("???={0}", groupName))).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        res = response.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        ++tryCount;
                        res = "";
                        Thread.Sleep(3000);
                        //error
                    }
                }
                catch (HttpRequestException ex)
                {
                    ++tryCount;
                    res = "";
                    Thread.Sleep(3000);
                    //error
                }
            } while (tryCount < 3);
            return "error";
        }

        public string CheckValidGroupOwnerName(string groupName, string ownerName)
        {
            int tryCount = 0;
            string res = "";
            do
            {
                try
                {
                    if (IsTokenExpire())
                    {
                        GetAccessToken();
                    }
                    HttpClient httpClient = new HttpClient();
                    // Add User-Agent header.
                    //httpClient.DefaultRequestHeaders.UserAgent.Clear();
                    //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(System.AppDomain.CurrentDomain.FriendlyName);
                    // Add Accept header.
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                    // Add Authorization header.
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.access_token);
                    // Call API and get response.
                    HttpResponseMessage response = httpClient.GetAsync(new Uri(String.Format("???={0}&???={1}", groupName, ownerName))).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        res = response.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        ++tryCount;
                        res = "";
                        Thread.Sleep(3000);
                        //error
                    }
                }
                catch (HttpRequestException ex)
                {
                    ++tryCount;
                    res = "";
                    Thread.Sleep(3000);
                    //error
                }
            } while (tryCount < 3);
            return "error";
        }
        public string CheckValidGroupContactName(string groupName, string contactName)
        {
            int tryCount = 0;
            string res = "";
            do
            {
                try
                {
                    if (IsTokenExpire())
                    {
                        GetAccessToken();
                    }
                    HttpClient httpClient = new HttpClient();
                    // Add User-Agent header.
                    //httpClient.DefaultRequestHeaders.UserAgent.Clear();
                    //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(System.AppDomain.CurrentDomain.FriendlyName);
                    // Add Accept header.
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                    // Add Authorization header.
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.access_token);
                    // Call API and get response.
                    HttpResponseMessage response = httpClient.GetAsync(new Uri(String.Format("???={0}&???={1}", groupName, contactName))).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        res = response.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        ++tryCount;
                        res = "";
                        Thread.Sleep(3000);
                        //error
                    }
                }
                catch (HttpRequestException ex)
                {
                    ++tryCount;
                    res = "";
                    Thread.Sleep(3000);
                    //error
                }
            } while (tryCount < 3);
            return "error";
        }
        


        /// <summary>
        /// This method retrieves an access token given a consumer key and secret.
        /// </summary>
        /// <remarks>
        /// This method demonstrates how to dynamically retrieve an access token, a.k.a., bearer token.
        /// </remarks>
        /// <param name="httpClient">A HttpClient object.</param>
        /// <returns></returns>
        private void GetAccessToken()
        {
            int tryCount = 0;
            var jsonString = "error";
            do
            {
                try
                {
                    HttpClient httpClient = new HttpClient();
                    // Add User-Agent header.
                    //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(System.AppDomain.CurrentDomain.FriendlyName);
                    // Add Authorization header.
                    var planeTextBytes = System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", _consumerKey, _consumerSecret));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(planeTextBytes));
                    // Set form data.
                    var formContent = new FormUrlEncodedContent(new[]
                            {
                                new KeyValuePair<string, string>("grant_type", "client_credentials")
                            });
                    // Call API and get response.
                    HttpResponseMessage response = httpClient.PostAsync(new Uri(_apiGateway + "token"), formContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        jsonString = response.Content.ReadAsStringAsync().Result;
                        _accessToken = (AccessToken)JObject.Parse(jsonString).ToObject(typeof(AccessToken));
                        _accessToken.expireTime = DateTime.Now.AddSeconds(_accessToken.expires_in);
                    }
                }
                catch (HttpRequestException ex)
                {
                    jsonString = "error";
                    ++tryCount;
                    Thread.Sleep(3000);
                }
            } while (tryCount < 3 && string.Equals(jsonString, "error"));

        }

        private bool IsTokenExpire()
        {
            if (_accessToken == null)
            {
                return true;
            }
            else
            {
                return DateTime.Compare(_accessToken.expireTime, DateTime.Now) < 0;
            }
        }
        /// <summary>
        /// This method captures all the error message by traversing the exception.
        /// </summary>
        /// <param name="ex">An exception object.</param>
        /// <returns>An error string.</returns>
    }

    public class AccessToken
    {
        public string access_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }
        public DateTime expireTime { get; set; }
    }

    public class Recipient
    {
        public string type { get; set; }
        public string group { get; set; }
        public string username { get; set; }
        public List<string> usernameList { get; set; }
    }


}