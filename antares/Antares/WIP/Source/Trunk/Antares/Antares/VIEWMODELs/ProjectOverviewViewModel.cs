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
            Messenger.Instance.Register<Refresh>(RefreshAll);
            BindingData();
            Messenger.Instance.Register<DeleteProjectMsg>(DeleteProjects);
        }

        private void RefreshAll(object obj)
        {
            BindingData();
        }

        private async void DeleteProjects(object args)
        {
            Projects = await ProjectRepository.Instance.GetAllProjects();
        }

        private async void BindingData()
        {
            var temp = await ProjectRepository.Instance.GetAllProjects();

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
