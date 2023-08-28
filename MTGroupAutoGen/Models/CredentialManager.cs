using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Configuration;
using System.Text;

namespace MTGroupAutoGen.Models
{
    public static class CredentialManager
    {
        #region Fields

        private static string _baseURL;
        private static string _runAsUser;
        private static string _apiKey;
        public const string AUTH_SIGN_IN = "Auth/SignAppin";

        public const string AUTH_SIGN_OUT = "Auth/Signout";

        public const string AUTHORIZATION_HEADER = "Authorization";
        public const string AUTHORIZATION_KEY = "PS-Auth key={0}; runas={1}";

        public const string APPLICATION_JSON = "application/json";
        public const string SID = "SID";
        public const string MANAGED_SYSTEM_URL = "ManagedAccounts?systemName={0}&accountName={1}";
        public const string GET_PWD_URL = "Credentials/{0}?type=password";

        public const string SYSTEM_ID = "SystemId";

        public const string ACCOUNT_ID = "AccountId";

        public const string DURATION_MINUTE = "DurationMinutes";

        public const string DURATION_MINUTE_VALUE = "1";
        public const string REASON = "Reason";

        public const string CHECK_IN_REASON = "Password Received";

        public const string CHECK_IN_API = "Requests/{0}/Checkin";
        public const string REQUESTID_URL = "Requests";

        //  private static IConfigurationRoot configuration;

        #endregion

        #region Constructor

        static CredentialManager()
        {
            // var builder = new WebConfigurationManager()
            // var builder = new ConfigurationBuilder()
            //            .AddJsonFile(Constants.APP_SETTINGS_NAME, optional: false, reloadOnChange: true);
            //  configuration = builder.Build();

            // _baseURL = configuration.GetSection(Constants.BASE_URL).Value.ToString();
            //_runAsUser = configuration.GetSection(Constants.RUN_AS_USER).Value.ToString();
            //_apiKey = configuration.GetSection(Constants.API_KEY).Value.ToString();
            _baseURL = "https://biconsole.Company.com/BeyondTrust/api/public/v3/";
            _runAsUser = "DOMAINNAME\\mtgautogen";
            _apiKey = "7b5a87a833803c168ae6361316e02f519fc010cf0ac8fca8259a2fb7e6dba23a5a56dd20b066559af08c0b882cce9d91a8f822a9cf5dca592525fe54015f1c9f";
        }

        #endregion

        #region GetCredentials
        /// <summary>
        /// Get credentials for DB, LDAP 
        /// </summary>
        /// <param name="hostName">Host Name</param>
        /// <param name="assetName">Server Name</param>
        /// <param name="databaseName">Database Name</param>
        /// <param name="accountName">Account Name</param>
        /// <returns></returns>
        public static string GetCredentials(string assetName, string accountName)
        {
            try
            {
                string password = GetPassword(assetName, accountName);

                password = password.Replace("\"", "");
                return password;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Calling of beyond trust rest apis to get password.
        /// 1. Call to sign in API to get SID which is the header of other APIs
        /// 2. Get the system id and account id for the asset name and system name passed.
        /// 3. Get the request id generated on the basis of system id, account id and life duration of the request
        /// 4. Get the password on the request id.
        /// 5. Dispose off the request id once password is received.
        /// 6. Sign out from the APi
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="accountName"></param>
        /// <returns></returns>
        private static string GetPassword(string assetName, string accountName)
        {
            var messageContent = new { assetName, accountName };
            var jsonMessageContent = JsonConvert.SerializeObject(messageContent);

            string password = string.Empty;
            string sid = string.Empty;
            string systemID = string.Empty;
            string accountID = string.Empty;

            using (var httpClient = new HttpClient())
            {
                string signInRequestURL = string.Format("{0}{1}", _baseURL, AUTH_SIGN_IN);
                string signOutRequestURL = string.Format("{0}{1}", _baseURL, AUTH_SIGN_OUT);

                //Sign In API. This uses api key and run as user values from appsettings and SID is received as response.
                //SID is used as header for other APIs.

                using (var signInHTTPRequest = new HttpRequestMessage(HttpMethod.Post, signInRequestURL))
                {
                    signInHTTPRequest.Headers.Add(AUTHORIZATION_HEADER, string.Format(AUTHORIZATION_KEY, _apiKey, _runAsUser));
                    signInHTTPRequest.Content = new StringContent(jsonMessageContent, Encoding.UTF8, APPLICATION_JSON);

                    var httpResponse = httpClient.SendAsync(signInHTTPRequest).Result;
                    var signInResult = httpResponse.Content.ReadAsStringAsync().Result;
                    sid = JsonConvert.DeserializeObject<Dictionary<string, string>>(signInResult)[SID];
                }

                //Get the system id and account id required to generate the request id to get password

                string managedSystemAPI = string.Format(MANAGED_SYSTEM_URL, assetName, accountName);
                string managedSysURL = string.Format("{0}{1}", _baseURL, managedSystemAPI);

                using (var managedSystemHTTPRequest = new HttpRequestMessage(HttpMethod.Get, managedSysURL))
                {
                    managedSystemHTTPRequest.Headers.Add(SID, sid);
                    // managedSystemHTTPRequest.Content = new StringContent(jsonMessageContent, Encoding.UTF8, APPLICATION_JSON);

                    var httpResponse = httpClient.SendAsync(managedSystemHTTPRequest).Result;
                    var managedSystemResult = httpResponse.Content.ReadAsStringAsync().Result;

                    var managedSystem = JsonConvert.DeserializeObject<Dictionary<string, string>>(managedSystemResult);

                    systemID = managedSystem[SYSTEM_ID].ToString();
                    accountID = managedSystem[ACCOUNT_ID].ToString();
                }

                string requestIdAPI = string.Format(REQUESTID_URL, systemID, accountID);
                string requestIdURL = string.Format("{0}{1}", _baseURL, requestIdAPI);
                string requestID = "";

                //API call to get request id using system id , account id and life duration of request id is passes as content 


                using (var requestIdHTTPRequest = new HttpRequestMessage(HttpMethod.Post, requestIdURL))
                {
                    Dictionary<string, string> passwordRequestContent = new Dictionary<string, string>();
                    passwordRequestContent.Add(SYSTEM_ID, systemID);
                    passwordRequestContent.Add(ACCOUNT_ID, accountID);
                    passwordRequestContent.Add(DURATION_MINUTE, DURATION_MINUTE_VALUE);

                    var jsonpasswordRequestContent = JsonConvert.SerializeObject(passwordRequestContent);

                    requestIdHTTPRequest.Headers.Add(SID, sid);
                    requestIdHTTPRequest.Content = new StringContent(jsonpasswordRequestContent, Encoding.UTF8, APPLICATION_JSON);

                    var httpResponse = httpClient.SendAsync(requestIdHTTPRequest).Result;
                    var requestIdResult = httpResponse.Content.ReadAsStringAsync().Result;

                    requestID = requestIdResult.ToString();
                }

                //API call to get the password using request id.

                string passwordAPI = string.Format(GET_PWD_URL, requestID);
                string passwordURL = string.Format("{0}{1}", _baseURL, passwordAPI);

                using (var passwordHTTPRequest = new HttpRequestMessage(HttpMethod.Get, passwordURL))
                {
                    passwordHTTPRequest.Headers.Add(SID, sid);
                    //  passwordHTTPRequest.Content = new StringContent(jsonMessageContent, Encoding.UTF8, APPLICATION_JSON);

                    var httpResponse = httpClient.SendAsync(passwordHTTPRequest).Result;
                    var passwordResult = httpResponse.Content.ReadAsStringAsync().Result;

                    password = passwordResult.ToString();
                }

                //API call to dispose off the Request ID.                  

                string checkInAPI = string.Format(CHECK_IN_API, requestID);
                string checkInURL = string.Format("{0}{1}", _baseURL, checkInAPI);

                using (var checkInHTTPRequest = new HttpRequestMessage(HttpMethod.Put, checkInURL))
                {
                    Dictionary<string, string> checkInRequestContent = new Dictionary<string, string>();
                    checkInRequestContent.Add(REASON, CHECK_IN_REASON);

                    var jsonCheckinRequestContent = JsonConvert.SerializeObject(checkInRequestContent);

                    checkInHTTPRequest.Headers.Add(SID, sid);
                    checkInHTTPRequest.Content = new StringContent(jsonCheckinRequestContent, Encoding.UTF8, APPLICATION_JSON);

                    var httpResponse = httpClient.SendAsync(checkInHTTPRequest).Result;
                    var checkInResult = httpResponse.Content.ReadAsStringAsync().Result;

                }

                //SignOut api to sign out from the Beyond Trust

                using (var signOutHTTPRequest = new HttpRequestMessage(HttpMethod.Post, signOutRequestURL))
                {
                    signOutHTTPRequest.Headers.Add(SID, sid);
                    signOutHTTPRequest.Content = new StringContent(jsonMessageContent, Encoding.UTF8, APPLICATION_JSON);


                    var httpResponse = httpClient.SendAsync(signOutHTTPRequest).Result;
                    var signOutResult = httpResponse.Content.ReadAsStringAsync().Result;
                }
            }
            return password;
        }
        #endregion
    }
}