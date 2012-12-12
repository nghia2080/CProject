using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;
using AntaresShell.IO;
using AntaresShell.Logger;
using Repository.MODELs;
using Repository.ServiceConnection.Controllers;
using Repository.Sync;

namespace Repository.Repositories
{
    public class UserInformationRepository
    {
        private readonly LocalStorageManager _usersLocal = new LocalStorageManager("Cache\\UserInfo.esec");

        private ObservableCollection<UserModel> _cacheUsers = new ObservableCollection<UserModel>();

        private readonly SemaphoreSlim _sl = new SemaphoreSlim(1);

        public async Task<UserModel> GetUser(string username)
        {
            await _sl.WaitAsync();
            try
            {
                if (_cacheUsers.Count != 0)
                {
                    var cacheuser = _cacheUsers.FirstOrDefault(p => p.Username == username);
                    if (cacheuser != null)
                    {
                        return cacheuser;
                    }
                }

                var user = await UserInformationController.Instance.GetAsync(username);
                if (user != null)
                {
                    _cacheUsers.Add(user);
                }
                else
                {
                    if (_cacheUsers.Count == 0)
                    {
                        _cacheUsers = await _usersLocal.RestoreAsync<ObservableCollection<UserModel>>();
                        if (_cacheUsers != null)
                        {
                            var cacheuser = _cacheUsers.FirstOrDefault(p => p.Username == username);
                            if (cacheuser != null)
                            {
                                return cacheuser;
                            }
                        }
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }
            finally
            {
                _sl.Release();
            }
        }

        public async Task<UserModel> GetUser(int userID)
        {
            await _sl.WaitAsync();
            try
            {
                if (_cacheUsers.Count != 0)
                {
                    var cacheuser = _cacheUsers.FirstOrDefault(p => p.UserID == userID);
                    if (cacheuser != null)
                    {
                        return cacheuser;
                    }
                }

                var user = await UserInformationController.Instance.GetAsync(userID);
                if (user != null)
                {
                    _cacheUsers.Add(user);
                }
                else
                {
                    if (_cacheUsers.Count == 0)
                    {
                        _cacheUsers = await _usersLocal.RestoreAsync<ObservableCollection<UserModel>>();
                        if (_cacheUsers != null)
                        {
                            var cacheuser = _cacheUsers.FirstOrDefault(p => p.UserID == userID);
                            if (cacheuser != null)
                            {
                                return cacheuser;
                            }
                        }
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }
            finally
            {
                _sl.Release();
            }
        }

        public async Task<UserModel> AddUser(UserModel data)
        {
            var response = await UserInformationController.Instance.PostAsync(data);
            if (response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var djs = new DataContractJsonSerializer(typeof(UserModel));
                    var user = (UserModel)djs.ReadObject(stream);
                    _cacheUsers.Add(user);
                    return user;
                }
            }

            return null;
        }

        public async Task<HttpResponseMessage> UpdateUserData(UserModel data)
        {
            var response = await UserInformationController.Instance.PushAsync(data.UserID, data);
            if (response.IsSuccessStatusCode)
            {
                var index = _cacheUsers.IndexOf(data);
                if (index >= 0)
                {
                    _cacheUsers[index] = data;
                }
            }

            return response;
        }

        public void ClearCache()
        {
            _cacheUsers.Clear();
        }

        #region Singleton
        private UserInformationRepository()
        {
            _cacheUsers.CollectionChanged += _cacheUsers_CollectionChanged;
        }

        async void _cacheUsers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await _usersLocal.SaveAsync(_cacheUsers);
        }

        public static UserInformationRepository Instance
        {
            get { return Nested._instance; }
        }

        private class Nested
        {
            /// <summary>
            /// Instance of Repository for Singleton pattern.
            /// </summary>
            internal static readonly UserInformationRepository _instance = new UserInformationRepository();

            /// <summary>
            /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit.
            /// </summary>
            static Nested()
            {
            }
        }
        #endregion
    }
}
