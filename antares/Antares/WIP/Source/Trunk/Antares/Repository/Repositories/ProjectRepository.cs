using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;
using AntaresShell.IO;
using AntaresShell.Logger;
using Repository.MODELs;
using Repository.ServiceConnection.Controllers;
using Repository.Sync;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ProjectRepository
    {
        LocalStorageManager _projectLocal = new LocalStorageManager("Cache\\Project.esec");

        private ObservableCollection<ProjectInformationModel> _projects = new ObservableCollection<ProjectInformationModel>();

        private readonly SemaphoreSlim _slGetAllProject = new SemaphoreSlim(1);

        public async Task<ObservableCollection<ProjectInformationModel>> GetAllProjects()
        {
            try
            {
                var proiectlist = await ProjectMemberRepository.Instance.GetAllProjects(GlobalData.MyUserID);

                foreach (var projectMemberContrainModel in proiectlist)
                {
                    if (projectMemberContrainModel.ProjectID == -1) { continue; }

                    await GetProject(projectMemberContrainModel.ProjectID);
                }
                // _projects = await ProjectInformationController.Instance.GetProjectAsync(GlobalData.MyUserID);

                if (_projects.Count == 0)
                {
                    var dumb = await _projectLocal.RestoreAsync<ObservableCollection<ProjectInformationModel>>();
                    if (dumb != null)
                    {
                        _projects = dumb;
                    }
                }

                return _projects;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }

        }

        public async Task<ProjectInformationModel> GetProject(int id)
        {
            await _slGetAllProject.WaitAsync();
            try
            {
                if (_projects != null && _projects.Count != 0)
                {
                    if (_projects.FirstOrDefault(p => p.ID == id) != null)
                    {
                        return _projects.First(p => p.ID == id);
                    }
                }

                var project = await ProjectInformationController.Instance.GetAsync(id);

                if (project != null)
                {
                    _projects.Add(project);
                }

                return project;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }
            finally
            {
                _slGetAllProject.Release();
            }
        }

        public async Task<ProjectInformationModel> AddNewProject(ProjectInformationModel data)
        {
            var response = await ProjectInformationController.Instance.PostAsync(data);
            if (response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var djs = new DataContractJsonSerializer(typeof(ProjectInformationModel));
                    var project = (ProjectInformationModel)djs.ReadObject(stream);
                    _projects.Add(project);
                    return project;
                }
            }

            return null;
        }

        public async Task<HttpResponseMessage> UpdateProject(ProjectInformationModel data)
        {
            var response = await ProjectInformationController.Instance.PushAsync(data.ID, data);
            if (response.IsSuccessStatusCode)
            {
                var target = _projects.FirstOrDefault(p => p.ID == data.ID);
                var index = _projects.IndexOf(target);

                _projects.RemoveAt(index);
                _projects.Insert(index, data);
            }

            return response;
        }

        public async Task<HttpResponseMessage> DeleteProject(int id)
        {
            var response = await ProjectInformationController.Instance.DeleteAsync(id);
            if (response.IsSuccessStatusCode)
            {
                var target = _projects.FirstOrDefault(p => p.ID == id);
                _projects.Remove(target);
            }

            return response;
        }

        private void ClearCache()
        {
            _projects = new ObservableCollection<ProjectInformationModel>();
        }

        #region Singleton
        private ProjectRepository()
        {
            _projects.CollectionChanged += _projects_CollectionChanged;
            Messenger.Instance.Register<Refresh>(p =>
                                                     {
                                                         var type = (Refresh)p;
                                                         if (type == Refresh.All || type == Refresh.Project)
                                                         {
                                                             ClearCache();
                                                         }
                                                     });
        }

        async void _projects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await _projectLocal.SaveAsync(_projects);
        }

        public static ProjectRepository Instance
        {
            get { return Nested._instance; }
        }

        private class Nested
        {
            /// <summary>
            /// Instance of Repository for Singleton pattern.
            /// </summary>
            internal static readonly ProjectRepository _instance = new ProjectRepository();

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
