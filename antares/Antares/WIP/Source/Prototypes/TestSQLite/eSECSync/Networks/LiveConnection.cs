using Windows.Networking.BackgroundTransfer;
using eSECSync.Helpers;
using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using eSECSync.Clients;

namespace eSECSync.Networks
{
    public class LiveConnection
    {
        private const string ESEC_ID = "ESEC_SKYDRIVE_FOLDER_ID";

        private LiveConnectClient _liveConnectClient;
        private System.Threading.CancellationTokenSource _ctsUpload;
        private System.Threading.CancellationTokenSource _ctsDownload;
        private string eSECID = null;
        private Progress<LiveOperationProgress> progressHandler;

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
                var loginResult = await auth.LoginAsync(new[] { "wl.basic", "wl.contacts_skydrive", "wl.skydrive_update" });

                if (loginResult.Status == LiveConnectSessionStatus.Connected)
                {
                    _liveConnectClient = new LiveConnectClient(loginResult.Session);
                   
                    eSECID = ApplicationData.Current.RoamingSettings.Values[ESEC_ID] + string.Empty;

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }


        public async Task<UserInformationModel> GetUserInformation()
        {
            if (_liveConnectClient == null) return null;

            dynamic userInfo = (await _liveConnectClient.GetAsync("me")).Result;

            var userInfoModel = new UserInformationModel
            {
                ID = userInfo.id,
                FullName = userInfo.name,
                FirstName = userInfo.first_name,
                LastName = userInfo.last_name,
                Link = userInfo.link,
                Gender = userInfo.gender,
                Locale = userInfo.locale,
                UpdatedTime = userInfo.updated_time
            };

            await EnsureBaseFolder.Instance.InitializeFolder(userInfoModel.ID);

            return userInfoModel;
        }

        /// <summary>
        /// Upload content to SkyDrive.
        /// </summary>
        /// <returns>True if success.</returns>
        public async Task<bool> UploadDataAsync(StorageFile file)
        {
            if (file == null || _liveConnectClient == null) return false;

            try
            {
                _ctsUpload = new System.Threading.CancellationTokenSource();
                progressHandler = new Progress<LiveOperationProgress>(
                   progress =>
                   {
                       ProgressBarHandler.Instance.ProgressBar.Value = progress.ProgressPercentage;
                   });

                string id = null;

                // Get default id from roamming setting if have, other wise, request SkyDrive to get one.
                if (!string.IsNullOrEmpty(eSECID))
                {
                    id = eSECID;
                }
                else
                {
                    dynamic documentFiles = (await _liveConnectClient.GetAsync("me/skydrive/files")).Result;
                    if (documentFiles == null) return false;

                    // SEARCH eSEC folder.
                    foreach (var item in documentFiles.data)
                    {
                        if (item.name != "eSEC") continue;
                        id = item.id.ToString();
                    }

                    if (string.IsNullOrEmpty(id))
                    {
                        // CANT FIND SPECIFIC FOLDER, SHOULD CREATE IT.
                        var folderData = new Dictionary<string, object> { { "name", "eSEC" }, { "description", "Contains synchronized data for eSEC." } };
                        var operationResult = await _liveConnectClient.PostAsync("me/skydrive", folderData);
                        dynamic skyDriveFolder = operationResult.Result;
                        id = skyDriveFolder.id.ToString();
                    }

                    if (string.IsNullOrEmpty(id))
                    {
                        // Unable to reach to esec folder.
                        return false;
                    }

                    // SAVE KEY for better performance.
                    ApplicationData.Current.RoamingSettings.Values[ESEC_ID] = id;
                }

                await _liveConnectClient.BackgroundUploadAsync(id, file.Name, file, OverwriteOption.Overwrite, _ctsUpload.Token, progressHandler);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DownloadDataAsync(StorageFile destinationFile, string fileName)
        {
            if (_liveConnectClient == null) return false;
            try
            {
                _ctsDownload = new System.Threading.CancellationTokenSource();
                string id = null;

                // Get default id from roamming setting if have, other wise, request SkyDrive to get one.
                if (!string.IsNullOrEmpty(eSECID))
                {
                    id = eSECID;
                }
                else
                {
                    dynamic documentFiles = (await _liveConnectClient.GetAsync("me/skydrive/files")).Result;
                    if (documentFiles == null) return false;

                    // SEARCH eSEC folder.
                    foreach (var item in documentFiles.data)
                    {
                        if (item.name != "eSEC") continue;
                        id = item.id.ToString();
                    }

                    if (string.IsNullOrEmpty(id))
                    {
                        // Unable to reach to esec folder.
                        return false;
                    }

                    // SAVE KEY for better performance.
                    ApplicationData.Current.RoamingSettings.Values[ESEC_ID] = id;

                }

                dynamic eSECFiles = (await _liveConnectClient.GetAsync(id + "/files")).Result;
                if (eSECFiles == null) return false;

                // SEARCH file.
                foreach (var item in eSECFiles.data)
                {
                    if (item.name != fileName) continue;
                    var backgroundDownloader = new BackgroundDownloader();
                    var operation = backgroundDownloader.CreateDownload(new Uri(item.source.ToString()), destinationFile);
                    var progressCallback = new Progress<DownloadOperation>(DownloadProgress);
                    await operation.StartAsync().AsTask(_ctsDownload.Token, progressCallback);

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

         // Note that this event is invoked on a background thread, so we cannot access the UI directly.
        private void DownloadProgress(DownloadOperation download)
        {
            if (download.Progress.TotalBytesToReceive > 0)
            {
                ProgressBarHandler.Instance.ProgressBar.Value = download.Progress.BytesReceived * 100 / download.Progress.TotalBytesToReceive;
            }
        }
    }
}
