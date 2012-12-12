using System.Collections.ObjectModel;
using AntaresShell.BaseClasses;
using System;
using AntaresShell.Common;
using Repository.MODELs;
using AntaresShell.Common.MessageTemplates;
using Repository.Repositories;
using Repository.Sync;

namespace Antares.VIEWMODELs
{
    public class ProjectOverviewViewModel : ViewModelBase
    {
        private ObservableCollection<ProjectInformationModel> _projects;

        public ObservableCollection<ProjectInformationModel> Projects
        {
            get { return _projects; }
            set { SetProperty(ref _projects, value); }
        }

        public ProjectOverviewViewModel()
        {
            BindingData();
            Messenger.Instance.Register<DeleteProjectMsg>(DeleteProjects);
        }

        private async void DeleteProjects(object args)
        {
            //foreach (var projectInformation in GlobalData.SelectedProjects)
            //{
            //    var projectInformationModel = projectInformation as ProjectInformationModel;
            //    if(projectInformationModel!=null)
            //    {
            //        await ProjectRepository.Instance.DeleteProject(projectInformationModel.ID);
            //    }
            //}

            Projects = await ProjectRepository.Instance.GetAllProjects();
        }

        private async void BindingData()
        {
            var temp = await ProjectRepository.Instance.GetAllProjects();

          
            //if(temp==null || temp.Count == 0)
            //{
            //    temp = new ObservableCollection<ProjectInformationModel>
            //                   {
            //                       new ProjectInformationModel
            //                           {
            //                               ID = -1,
            //                               Name = "You have no project",
            //                               Description = "",
            //                               StartDate = DateTime.Now.ToString(),
            //                               EndDate = null,
            //                               Status = 1
            //                           }
            //                   };
            //}

            Projects = temp;

            foreach (var projectInformationModel in Projects)
            {
                var projectTask = await TaskRepository.Instance.GetAllTasksForProject(projectInformationModel.ID);

                if (projectTask != null)
                {
                    projectInformationModel.NumberOfTask = projectTask.Count;
                }
            }

        }
    }
}
