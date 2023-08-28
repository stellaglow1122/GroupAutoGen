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
using System.Configuration;

namespace MTGroupAutoGen.Models
{
    class ServiceNowManager
    {

        // For Prod Env
        private const string _consumerKeysm2ServiceNow = "ConsumerKey";
        private const string _consumerSecretsm2ServiceNow = "ConsumerSecret";

        // For Prod Env
        private const string _consumerKeyETSOPS = "ConsumerKey";
        private const string _consumerSecretETSOPS = "ConsumerSecret";

        private AccessToken _accessToken;
        //private CmdLineArgs _cmdLineArgs;
        private StringBuilder _errorMessage;
        private string _apiGateway = "APIHost";
        private string _apiContext = "APIContext";

        public string GetEventualMember(string groupName)
        {
            try
            {
                GetAccessToken(_consumerKeysm2ServiceNow, _consumerSecretsm2ServiceNow);
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
                HttpResponseMessage response = httpClient.GetAsync(new Uri(_apiGateway + _apiContext + String.Format("???={0}", groupName))).Result;
                if (response.IsSuccessStatusCode)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    return res;
                }
                else
                {
                    return "error";
                    //error
                }
            }
            catch (HttpRequestException ex)
            {
                return "error";
                //error
            }
        }

        public string CreateTask(string username, string groupName, string groupOwner, string query)
        {
            try
            {
                GetAccessToken(_consumerKeysm2ServiceNow, _consumerSecretsm2ServiceNow);
                HttpClient httpClient = new HttpClient();
                // Add User-Agent header.
                //httpClient.DefaultRequestHeaders.UserAgent.Clear();
                //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(System.AppDomain.CurrentDomain.FriendlyName);
                // Add Accept header.
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                httpClient.DefaultRequestHeaders.Add("x-sn-credential", "credential");
                // Add Authorization header.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.access_token);
                // Call API and get response.

                // For Prod Env
                
                var data = new StringContent("StringContent");

                HttpResponseMessage response = httpClient.PostAsync(new Uri(String.Format(_apiGateway + "URL")), data).Result;
                if (response.IsSuccessStatusCode)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    return res;
                }
                else
                {
                    return "error";
                    //error
                }
            }
            catch (HttpRequestException ex)
            {
                return "error";
                //error
            }
        }
        public string AutoGenQueryApproval(string username, string groupName, string groupOwner, string query, string userCount, string executionTime)
        {
            try
            {
                GetAccessToken(_consumerKeysm2ServiceNow, _consumerSecretsm2ServiceNow);
                HttpClient httpClient = new HttpClient();
                // Add User-Agent header.
                //httpClient.DefaultRequestHeaders.UserAgent.Clear();
                //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(System.AppDomain.CurrentDomain.FriendlyName);
                // Add Accept header.
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                httpClient.DefaultRequestHeaders.Add("x-sn-credential", "credential");
                // Add Authorization header.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.access_token);
                // Call API and get response.
                string approvers = ConfigurationManager.AppSettings["Approvers"];
                var data = new StringContent("StringContent");
                // For Prod Env

                HttpResponseMessage response = httpClient.PostAsync(new Uri(String.Format(_apiGateway + "URL")), data).Result;
                if (response.IsSuccessStatusCode)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    return res;
                }
                else
                {
                    return "error";
                    //error
                }
            }
            catch (HttpRequestException ex)
            {
                return "error";
                //error
            }
        }

        public bool IsOverTenRequests(string username)
        {
            try
            {
                GetAccessToken(_consumerKeyETSOPS, _consumerSecretETSOPS);
                HttpClient httpClient = new HttpClient();
                // Add User-Agent header.
                //httpClient.DefaultRequestHeaders.UserAgent.Clear();
                //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(System.AppDomain.CurrentDomain.FriendlyName);
                // Add Accept header.
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                httpClient.DefaultRequestHeaders.Add("x-sn-credential", "credential");
                // Add Authorization header.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.access_token);
                // Call API and get response.

                int requestCount = 0;

                HttpResponseMessage response = httpClient.GetAsync(new Uri(String.Format(_apiGateway + "URL"))).Result;
                if (response.IsSuccessStatusCode)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    JObject searchResults = JObject.Parse(res);
                    JArray jarr = (JArray)searchResults["results"];
                    foreach (JObject jobj in jarr)
                    {
                        JValue nameValue = (JValue)jobj["task_requested_for_userid"];
                        string name = (string)nameValue.Value;
                        JValue descriptionValue = (JValue)jobj["task_short_description"];
                        string description = (string)descriptionValue.Value;
                        if (name.ToUpper() == username.ToUpper() && description.Contains("Autogen Group Request Created") )
                        {
                            ++requestCount;
                        }
                    }
                    
                    if (requestCount > 9)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                    //error
                }
            }
            catch (HttpRequestException ex)
            {
                return false;
                //error
            }
        }


        /// <summary>
        /// This method retrieves an access token given a consumer key and secret.
        /// </summary>
        /// <remarks>
        /// This method demonstrates how to dynamically retrieve an access token, a.k.a., bearer token.
        /// </remarks>
        /// <param name="httpClient">A HttpClient object.</param>
        /// <returns></returns>
        private void GetAccessToken(string key, string secret)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                // Add User-Agent header.
                //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(System.AppDomain.CurrentDomain.FriendlyName);
                // Add Authorization header.
                var planeTextBytes = System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", key, secret));
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
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    _accessToken = (AccessToken)JObject.Parse(jsonString).ToObject(typeof(AccessToken));
                }
            }
            catch (HttpRequestException ex)
            {
                //error
            }
        }


        /// <summary>
        /// This method captures all the error message by traversing the exception.
        /// </summary>
        /// <param name="ex">An exception object.</param>
        /// <returns>An error string.</returns>
    }


}