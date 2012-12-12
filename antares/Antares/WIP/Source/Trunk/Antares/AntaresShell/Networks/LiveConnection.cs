using System.Net.Http;
using Windows.Data.Json;
using Windows.Networking.BackgroundTransfer;
using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Authentication.OnlineId;
using Windows.Storage;
using AntaresShell.IO;
using Windows.UI.Xaml.Media.Imaging;

namespace AntaresShell.Networks
{
    public class LiveConnection
    {
        private const string ESEC_ID = "ESEC_SKYDRIVE_FOLDER_ID";
        private LiveAuthClient _auth = new LiveAuthClient();
        private LiveConnectClient _liveConnectClient;
        private string eSECID = null;
        public string AccessToken { get; set; }
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
        OnlineIdAuthenticator _authenticator;
        /// <summary>
        /// Create connection with scope Basic, SkyDrive.
        /// </summary>
        /// <returns>Return true if connect successful. Otherwise, return false.</returns>
        public async Task<bool> CreateConnectionAsync()
        {
            if (AccessToken != null)
            {
                return true;
            }

            try
            {
                var targetArray = new List<OnlineIdServiceTicketRequest>
                                      {
                                          new OnlineIdServiceTicketRequest("wl.basic wl.signin wl.contacts_photos",
                                                                           "DELEGATION")
                                      };

                _authenticator = new OnlineIdAuthenticator();

                var promptType = (ApplicationData.Current.LocalSettings.Values["UserFirstName"] + string.Empty != string.Empty) ? CredentialPromptType.PromptIfNeeded : CredentialPromptType.RetypeCredentials;


                var result = await _authenticator.AuthenticateUserAsync(targetArray, promptType);

                if (result.Tickets[0].Value != string.Empty)
                {
                    AccessToken = result.Tickets[0].Value;
                    return true;
                }
                else
                {
                    // errors are to be handled here.
                    return false;
                }
                //var loginResult = await _auth.LoginAsync(new[] { "wl.basic, wl.logout" });

                //if (loginResult.Status == LiveConnectSessionStatus.Connected)
                //{
                //    _liveConnectClient = new LiveConnectClient(loginResult.Session);
                //    eSECID = ApplicationData.Current.RoamingSettings.Values[ESEC_ID] + string.Empty;

                //    return true;
                //}

                //return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LogOut()
        {
            //// Check to see if the user can sign out (Live account or Local account)
            //var onlineIdAuthenticator = new OnlineIdAuthenticator();
            //var serviceTicketRequest = new OnlineIdServiceTicketRequest("wl.basic", "DELEGATION");
            //await onlineIdAuthenticator.AuthenticateUserAsync(serviceTicketRequest);
            ApplicationData.Current.LocalSettings.Values["UserFirstName"] = string.Empty;

            //if (_authenticator.CanSignOut)
            //{
            //    await _authenticator.SignOutUserAsync();

            //    AccessToken = null;
            //}

            return true;
        }

        public async Task<LiveUserInformationModel> GetUserInformation()
        {
            try
            {
                var uri = new Uri("https://apis.live.net/v5.0/me?access_token=" + AccessToken);
                var client = new HttpClient();
                var result = await client.GetAsync(uri);
                JsonObject userInfo = null;
                string jsonUserInfo = await result.Content.ReadAsStringAsync();
                if (jsonUserInfo != null)
                {
                    userInfo = JsonObject.Parse(jsonUserInfo);
                }

                if (userInfo == null)
                {
                    return null;
                }

                var userInfoModel = new LiveUserInformationModel
                {
                    ID = (userInfo["id"].ValueType != JsonValueType.Null) ? userInfo["id"].GetString() : string.Empty,
                    FullName = (userInfo["name"].ValueType != JsonValueType.Null) ? userInfo["name"].GetString() : string.Empty,
                    FirstName = (userInfo["first_name"].ValueType != JsonValueType.Null) ? userInfo["first_name"].GetString() : string.Empty,
                    LastName = (userInfo["last_name"].ValueType != JsonValueType.Null) ? userInfo["last_name"].GetString() : string.Empty,
                    Link = (userInfo["link"].ValueType != JsonValueType.Null) ? userInfo["link"].GetString() : string.Empty,
                    Gender = (userInfo["gender"].ValueType != JsonValueType.Null) ? userInfo["gender"].GetString() : string.Empty,
                    Locale = (userInfo["locale"].ValueType != JsonValueType.Null) ? userInfo["locale"].GetString() : string.Empty,
                    UpdatedTime = (userInfo["updated_time"].ValueType != JsonValueType.Null) ? userInfo["updated_time"].GetString() : string.Empty
                };

                ApplicationData.Current.LocalSettings.Values["UserFirstName"] = userInfoModel.FirstName;

                return userInfoModel;
            }
            catch (Exception)
            {
                return null;
            }

        }

        ///// <summary>
        ///// Upload content to SkyDrive.
        ///// </summary>
        ///// <returns>True if success.</returns>
        //public async Task<bool> UploadDataAsync(StorageFile file)
        //{
        //    if (file == null || _liveConnectClient == null) return false;

        //    try
        //    {
        //        _ctsUpload = new System.Threading.CancellationTokenSource();
        //        progressHandler = new Progress<LiveOperationProgress>(
        //           progress =>
        //           {
        //              // ProgressBarHandler.Instance.ProgressBar.Value = progress.ProgressPercentage;
        //           });

        //        string id = null;

        //        // Get default id from roamming setting if have, other wise, request SkyDrive to get one.
        //        if (!string.IsNullOrEmpty(eSECID))
        //        {
        //            id = eSECID;
        //        }
        //        else
        //        {
        //            dynamic documentFiles = (await _liveConnectClient.GetAsync("me/skydrive/files")).Result;
        //            if (documentFiles == null) return false;

        //            // SEARCH eSEC folder.
        //            foreach (var item in documentFiles.data)
        //            {
        //                if (item.name != "eSEC") continue;
        //                id = item.id.ToString();
        //            }

        //            if (string.IsNullOrEmpty(id))
        //            {
        //                // CANT FIND SPECIFIC FOLDER, SHOULD CREATE IT.
        //                var folderData = new Dictionary<string, object> { { "name", "eSEC" }, { "description", "Contains synchronized data for eSEC." } };
        //                var operationResult = await _liveConnectClient.PostAsync("me/skydrive", folderData);
        //                dynamic skyDriveFolder = operationResult.Result;
        //                id = skyDriveFolder.id.ToString();
        //            }

        //            if (string.IsNullOrEmpty(id))
        //            {
        //                // Unable to reach to esec folder.
        //                return false;
        //            }

        //            // SAVE KEY for better performance.
        //            ApplicationData.Current.RoamingSettings.Values[ESEC_ID] = id;
        //        }

        //        await _liveConnectClient.BackgroundUploadAsync(id, file.Name, file, OverwriteOption.Overwrite, _ctsUpload.Token, progressHandler);

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //public async Task<bool> DownloadDataAsync(StorageFile destinationFile, string fileName)
        //{
        //    if (_liveConnectClient == null) return false;
        //    try
        //    {
        //        _ctsDownload = new System.Threading.CancellationTokenSource();
        //        string id = null;

        //        // Get default id from roamming setting if have, other wise, request SkyDrive to get one.
        //        if (!string.IsNullOrEmpty(eSECID))
        //        {
        //            id = eSECID;
        //        }
        //        else
        //        {
        //            dynamic documentFiles = (await _liveConnectClient.GetAsync("me/skydrive/files")).Result;
        //            if (documentFiles == null) return false;

        //            // SEARCH eSEC folder.
        //            foreach (var item in documentFiles.data)
        //            {
        //                if (item.name != "eSEC") continue;
        //                id = item.id.ToString();
        //            }

        //            if (string.IsNullOrEmpty(id))
        //            {
        //                // Unable to reach to esec folder.
        //                return false;
        //            }

        //            // SAVE KEY for better performance.
        //            ApplicationData.Current.RoamingSettings.Values[ESEC_ID] = id;

        //        }

        //        dynamic eSECFiles = (await _liveConnectClient.GetAsync(id + "/files")).Result;
        //        if (eSECFiles == null) return false;

        //        // SEARCH file.
        //        foreach (var item in eSECFiles.data)
        //        {
        //            if (item.name != fileName) continue;
        //            var backgroundDownloader = new BackgroundDownloader();
        //            var operation = backgroundDownloader.CreateDownload(new Uri(item.source.ToString()), destinationFile);
        //            var progressCallback = new Progress<DownloadOperation>(DownloadProgress);
        //            await operation.StartAsync().AsTask(_ctsDownload.Token, progressCallback);

        //            return true;
        //        }

        //        return false;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        // // Note that this event is invoked on a background thread, so we cannot access the UI directly.
        //private void DownloadProgress(DownloadOperation download)
        //{
        //    if (download.Progress.TotalBytesToReceive > 0)
        //    {
        //       // ProgressBarHandler.Instance.ProgressBar.Value = download.Progress.BytesReceived * 100 / download.Progress.TotalBytesToReceive;
        //    }
        //}
    }
}
