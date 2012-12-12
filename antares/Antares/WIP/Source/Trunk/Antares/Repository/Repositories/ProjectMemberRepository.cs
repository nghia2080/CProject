using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using AntaresShell.IO;
using Repository.MODELs;
using Repository.ServiceConnection.Controllers;
using System.Globalization;
using Repository.Sync;

namespace Repository.Repositories
{
    public class ProjectMemberRepository
    {
        LocalStorageManager _contrainLocal = new LocalStorageManager("Cache\\Contrain.esec");

        private ObservableCollection<ProjectMemberContrainModel> _cache = new ObservableCollection<ProjectMemberContrainModel>();

        private readonly SemaphoreSlim _sl = new SemaphoreSlim(1);

        public async Task<ObservableCollection<ProjectMemberContrainModel>> GetAllProjects(int userid)
        {
            await _sl.WaitAsync();

            try
            {
                if (_cache != null && _cache.Count != 0)
                {
                    var query = from contrain in _cache
                                where contrain.UserID == userid
                                select contrain;

                    if (query.Any())
                    {
                        return new ObservableCollection<ProjectMemberContrainModel>(query);
                    }
                }

                var data = await ProjectMemberController.Instance.GetAsync("uid$$" + userid);

                if (data != null)
                {
                    foreach (var projectMemberContrainModel in data)
                    {
                        _cache.Add(projectMemberContrainModel);
                    }
                }
                else
                {
                    if (_cache.Count == 0)
                    {
                        _cache = await _contrainLocal.RestoreAsync<ObservableCollection<ProjectMemberContrainModel>>();
                    }
                }

                return _cache;
            }
            finally
            {
                _sl.Release();
            }

        }

        public async Task<ObservableCollection<ProjectMemberContrainModel>> GetAllProjectsMember(int projectid)
        {
            await _sl.WaitAsync();

            try
            {
                if (_cache != null && _cache.Count != 0)
                {
                    var members = from m in _cache
                                  where m.ProjectID == projectid
                                  select m;

                    if (members.Count() > 1)
                    {
                        return new ObservableCollection<ProjectMemberContrainModel>(members);
                    }
                }

                var data = (ObservableCollection<ProjectMemberContrainModel>)await ProjectMemberController.Instance.GetAsync("pid$$" + projectid);
                if (data != null)
                {
                    foreach (var projectMemberContrainModel in data)
                    {
                        if (projectMemberContrainModel.UserID != GlobalData.MyUserID)
                        {
                            _cache.Add(projectMemberContrainModel);
                        }
                    }
                }
                else
                {
                    if (_cache.Count == 0)
                    {
                        _cache = await _contrainLocal.RestoreAsync<ObservableCollection<ProjectMemberContrainModel>>();
                    }
                }

                return data;
            }
            finally
            {
                _sl.Release();
            }
        }

        public bool IsManager(int projectID)
        {
            if (projectID == -1)
            {
                return true;
            }

            if (_cache != null)
            {
                var myContrain =
                    _cache.FirstOrDefault(p => (p.ProjectID == projectID && p.UserID == GlobalData.MyUserID));
                if (myContrain != null)
                {
                    // Co bat ki ki tu nao thi la PM
                    return !string.IsNullOrEmpty(myContrain.Role);
                }
            }
            return false;

        }

        public async Task<ProjectMemberContrainModel> AddContrain(ProjectMemberContrainModel member)
        {
            var existMember = _cache.FirstOrDefault(p => (p.UserID == member.UserID && p.ProjectID == member.ProjectID));
            if (existMember != null)
            {
                return null;
            }

            var response = await ProjectMemberController.Instance.PostAsync(member);
            if (response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var djs = new DataContractJsonSerializer(typeof(ProjectMemberContrainModel));
                    var projectMemberContrainModel = (ProjectMemberContrainModel)djs.ReadObject(stream);

                    if (_cache != null)
                    {
                        _cache.Add(projectMemberContrainModel);
                    }

                    return projectMemberContrainModel;
                }
            }

            return null;
        }

        public async Task<HttpResponseMessage> EditContrain(ProjectMemberContrainModel member)
        {
            var response = await ProjectMemberController.Instance.PushAsync(member.ID, member);
            if (response.IsSuccessStatusCode)
            {
                var target = _cache.FirstOrDefault(p => p.ID == member.ID);
                var index = _cache.IndexOf(target);

                _cache.RemoveAt(index);
                _cache.Insert(index, member);
            }
            return response;
        }

        public async Task<HttpResponseMessage> DeleteMember(int id)
        {
            var response = await ProjectMemberController.Instance.DeleteAsync(id);
            if (response.IsSuccessStatusCode)
            {
                if (_cache != null)
                {
                    var target = _cache.FirstOrDefault(p => p.ID == id);
                    _cache.Remove(target);
                }

                NotificationUtils.DeleteNotification(id);
            }

            return response;
        }

        #region Singleton
        private ProjectMemberRepository()
        {
            _cache.CollectionChanged += _cache_CollectionChanged;
        }

        async void _cache_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await _contrainLocal.SaveAsync(_cache);
        }

        public static ProjectMemberRepository Instance
        {
            get { return Nested._instance; }
        }

        private class Nested
        {
            /// <summary>
            /// Instance of Repository for Singleton pattern.
            /// </summary>
            internal static readonly ProjectMemberRepository _instance = new ProjectMemberRepository();

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
