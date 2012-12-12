using System.Threading.Tasks;
using Microsoft.Live;

namespace Repository.LiveConnection
{
    public class LiveConnection
    {
        private LiveConnectClient _liveConnectClient;

        public static LiveConnection Instance
        {
            get
            {
                return Nested._instance;
            }
        }

        private class Nested
        {
            internal static readonly LiveConnection _instance = new LiveConnection();
        }

        /// <summary>
        /// Create connection with scope Basic, SkyDrive.
        /// </summary>
        /// <returns>Return true if connect successful. Otherwise, return false.</returns>
        public async Task<bool> CreateConnectionAsync()
        {
            try
            {
                var auth = new LiveAuthClient();
                var loginResult = await auth.LoginAsync(new[] { "wl.basic", "wl.contacts_skydrive", "wl.skydrive_update", "wl.emails", "wl.contacts_photos" });

                if (loginResult.Status == LiveConnectSessionStatus.Connected)
                {
                    _liveConnectClient = new LiveConnectClient(loginResult.Session);

                    return true;
                }

                return false;
            }
            catch(LiveAuthException)
            {
                return true;
            }
        }

        private object currentID;

        public async Task<LiveUserInformationModel> GetUserInformation()
        {
            try
            {
                if (_liveConnectClient == null) return null;

                dynamic userInfo = (await _liveConnectClient.GetAsync("me")).Result;

                if (userInfo == null)
                {
                    return null;
                }

                var userInfoModel = new LiveUserInformationModel
                                        {
                                            ID = userInfo.id,
                                            FullName = userInfo.name,
                                            FirstName = userInfo.first_name,
                                            LastName = userInfo.last_name,
                                            Link = userInfo.link,
                                            Email = userInfo.emails.preferred,
                                            Gender = userInfo.gender,
                                            Locale = userInfo.locale,
                                            UpdatedTime = userInfo.updated_time
                                        };

                currentID = userInfoModel.ID;
                return userInfoModel;
            }
            catch
            {
                return null;
            }
        }

        //OnlineIdAuthenticator _authentiecator;
        //private string _accessToken;

        //private LiveUserInformationModel _cacheInfo;

        //public static LiveConnection Instance
        //{
        //    get
        //    {
        //        return Nested._instance;
        //    }
        //}

        //private class Nested
        //{
        //    internal static readonly LiveConnection _instance = new LiveConnection();
        //}

        ///// <summary>
        ///// Create connection with scope Basic, SkyDrive.
        ///// </summary>
        ///// <returns>Return true if connect successful. Otherwise, return false.</returns>
        //public async Task<bool> CreateConnectionAsync()
        //{
        //    if (_accessToken != null)
        //    {
        //        return true;
        //    }

        //    try
        //    {
        //        var targetArray = new List<OnlineIdServiceTicketRequest>
        //                              {
        //                                  new OnlineIdServiceTicketRequest("wl.basic wl.signin wl.emails wl.contacts_photos",
        //                                                                   "DELEGATION")
        //                              };

        //        _authenticator = new OnlineIdAuthenticator();

        //        var promptType = (ApplicationData.Current.LocalSettings.Values["UserFirstName"] + string.Empty != string.Empty) ? CredentialPromptType.RetypeCredentials : CredentialPromptType.RetypeCredentials;


        //        var result = await _authenticator.AuthenticateUserAsync(targetArray, promptType);

        //        if (result.Tickets[0].Value != string.Empty)
        //        {
        //            _accessToken = result.Tickets[0].Value;
        //            return true;
        //        }
        //        else
        //        {
        //            // errors are to be handled here.
        //            return false;
        //        }
        //        //var loginResult = await _auth.LoginAsync(new[] { "wl.basic, wl.logout" });

        //        //if (loginResult.Status == LiveConnectSessionStatus.Connected)
        //        //{
        //        //    _liveConnectClient = new LiveConnectClient(loginResult.Session);
        //        //    eSECID = ApplicationData.Current.RoamingSettings.Values[ESEC_ID] + string.Empty;

        //        //    return true;
        //        //}

        //        //return false;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //public async Task<LiveUserInformationModel> GetUserInformation()
        //{
        //    if (_cacheInfo != null)
        //    {
        //        return _cacheInfo;
        //    }

        //    try
        //    {
        //        var uri = new Uri("https://apis.live.net/v5.0/me?access_token=" + _accessToken);
        //        var client = new HttpClient();
        //        var result = await client.GetAsync(uri);
        //        JsonObject userInfo = null;
        //        string jsonUserInfo = await result.Content.ReadAsStringAsync();
        //        if (jsonUserInfo != null)
        //        {
        //            userInfo = JsonObject.Parse(jsonUserInfo);
        //        }

        //        if (userInfo == null)
        //        {
        //            return null;
        //        }

        //        var emailList = (userInfo["emails"].ValueType != JsonValueType.Null) ? userInfo["emails"].GetObject() : null;
        //        var email = emailList != null && (emailList.ValueType != JsonValueType.Null) ? emailList["preferred"].GetString() : null;

        //        var userInfoModel = new LiveUserInformationModel
        //        {
        //            ID = (userInfo["id"].ValueType != JsonValueType.Null) ? userInfo["id"].GetString() : string.Empty,
        //            Email = email,
        //            FullName = (userInfo["name"].ValueType != JsonValueType.Null) ? userInfo["name"].GetString() : string.Empty,
        //            FirstName = (userInfo["first_name"].ValueType != JsonValueType.Null) ? userInfo["first_name"].GetString() : string.Empty,
        //            LastName = (userInfo["last_name"].ValueType != JsonValueType.Null) ? userInfo["last_name"].GetString() : string.Empty,
        //            Link = (userInfo["link"].ValueType != JsonValueType.Null) ? userInfo["link"].GetString() : string.Empty,
        //            Gender = (userInfo["gender"].ValueType != JsonValueType.Null) ? userInfo["gender"].GetString() : string.Empty,
        //            Locale = (userInfo["locale"].ValueType != JsonValueType.Null) ? userInfo["locale"].GetString() : string.Empty,
        //            UpdatedTime = (userInfo["updated_time"].ValueType != JsonValueType.Null) ? userInfo["updated_time"].GetString() : string.Empty
        //        };

        //        ApplicationData.Current.LocalSettings.Values["UserFirstName"] = userInfoModel.FirstName;

        //        return userInfoModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.Instance.LogException(ex.ToString());
        //        return null;
        //    }

        //}

        public string GetUserAvatarUrl()
        {
            return string.Format("https://apis.live.net/v5.0/{0}/picture", currentID);
        }
    }
}
