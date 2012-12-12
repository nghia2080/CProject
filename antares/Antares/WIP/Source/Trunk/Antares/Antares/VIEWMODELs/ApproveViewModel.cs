using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntaresShell.BaseClasses;
using AntaresShell.Common;
using Repository.MODELs;
using Repository.Repositories;
using System.Windows.Input;
using Repository.ServiceConnection.Controllers;
using Repository.Sync;
using Windows.UI.Popups;

namespace Antares.VIEWMODELs
{
    public class ApproveViewModel : ViewModelBase
    {
        private ICommand _approvedCommand;
        public ICommand ApprovedCommand
        {
            get { return _approvedCommand ?? (_approvedCommand = new RelayCommand(ExecuteApprove)); }
        }

        private void ExecuteApprove(object obj)
        {
            string content = string.Empty;

            if (SelectedTasks != null)
            {
                foreach (var selectedTask in SelectedTasks)
                {
                    var extendTaskModel = selectedTask as ExtendTaskModel;
                    content += "Task: " + extendTaskModel.Task.Name + "\r\n";
                }
            }

            if (SelectedProjects != null)
            {
                foreach (var extendProjectMember in SelectedProjects)
                {
                    var extendProjectMemberContrainModel = extendProjectMember as ExtendProjectMemberContrainModel;
                    content += "Project:" + extendProjectMemberContrainModel.ProjectName + "\r\n";
                }
            }

            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            var md = new MessageDialog(content, "Accepted ?");
            // TODO: check async
            md.ShowAsync();
        }

        private ICommand _unApprovedCommand;
        public ICommand UnapprovedCommand
        {
            get { return _unApprovedCommand ?? (_unApprovedCommand = new RelayCommand(ExecuteUnapprove)); }
        }

        private void ExecuteUnapprove(object obj)
        {
            string content = string.Empty;

            if (SelectedTasks != null)
            {
                foreach (var selectedTask in SelectedTasks)
                {
                    var extendTaskModel = selectedTask as ExtendTaskModel;
                    content += "Task: " + extendTaskModel.Task.Name + "\r\n";
                }
            }

            if (SelectedProjects != null)
            {
                foreach (var extendProjectMember in SelectedProjects)
                {
                    var extendProjectMemberContrainModel = extendProjectMember as ExtendProjectMemberContrainModel;
                    content += "Project:" + extendProjectMemberContrainModel.ProjectName + "\r\n";
                }
            }

            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            var md = new MessageDialog(content, "Rejected ?");
            // TODO: Check async
            md.ShowAsync();
        }

        public IList<object> SelectedTasks { get; set; }
        public IList<object> SelectedProjects { get; set; }

        private ObservableCollection<ExtendTaskModel> _unapprovedTasks;
        public ObservableCollection<ExtendTaskModel> UnapprovedTasks
        {
            get { return _unapprovedTasks; }
            set { SetProperty(ref _unapprovedTasks, value); }
        }

        private ObservableCollection<ExtendProjectMemberContrainModel> _unapprovedProjects;
        public ObservableCollection<ExtendProjectMemberContrainModel> UnapprovedProjects
        {
            get { return _unapprovedProjects; }
            set { SetProperty(ref _unapprovedProjects, value); }
        }

        public ApproveViewModel()
        {
            BindData();
        }

        private async void BindData()
        {
            var tasks = await TaskRepository.Instance.GetAllTasksForUser(GlobalData.MyUserID);

            if (tasks != null)
            {
                var temp = from task in tasks
                           where task.IsConfirmed == false
                           select task;

                UnapprovedTasks = await ConvertTask(new ObservableCollection<TaskModel>(temp));

                //UnapprovedTasks = await ConvertTask(tasks);
            }


            var projects = await ProjectMemberRepository.Instance.GetAllProjects(GlobalData.MyUserID);
            if (projects != null)
            {
                var temp = from project in projects
                           where project.IsConfirmed != true
                           select project;

                UnapprovedProjects = await ConvertProject(new ObservableCollection<ProjectMemberContrainModel>(temp));

               // UnapprovedProjects = await ConvertProject(projects);
            }
        }

        private async Task<ObservableCollection<ExtendProjectMemberContrainModel>> ConvertProject(IEnumerable<ProjectMemberContrainModel> old)
        {
            var projectList = await ProjectRepository.Instance.GetAllProjects();

            var result = new ObservableCollection<ExtendProjectMemberContrainModel>();
            foreach (var projectMemberContrainModel in old)
            {
                var newproject = new ExtendProjectMemberContrainModel { ProjectMemberContrain = projectMemberContrainModel };
                if (newproject.ProjectMemberContrain.ProjectID != -1)
                {
                    newproject.ProjectName = projectList.First(p => p.ID == newproject.ProjectMemberContrain.ProjectID).Name;
                }
                else
                {
                    newproject.ProjectName = "None";
                }
                result.Add(newproject);
            }

            return result;
        }

        private async Task<ObservableCollection<ExtendTaskModel>> ConvertTask(IEnumerable<TaskModel> old)
        {
            var projectList = await ProjectRepository.Instance.GetAllProjects();

            var result = new ObservableCollection<ExtendTaskModel>();
            foreach (var taskmodel in old)
            {
                var newtask = new ExtendTaskModel { Task = taskmodel };
                if (newtask.Task.ProjectID != -1)
                {
                    newtask.ProjectName = projectList.First(p => p.ID == newtask.Task.ProjectID).Name;
                }
                else
                {
                    newtask.ProjectName = "None";
                }

                result.Add(newtask);
            }

            return result;
        }
    }

    public class ExtendTaskModel : BindableBase
    {
        private TaskModel _task;
        public TaskModel Task
        {
            get { return _task; }
            set { SetProperty(ref _task, value); }
        }

        private string _projectName;
        public string ProjectName
        {
            get { return _projectName; }
            set { SetProperty(ref _projectName, value); }
        }
    }

    public class ExtendProjectMemberContrainModel : BindableBase
    {
        private ProjectMemberContrainModel _projectMemberContrain;
        public ProjectMemberContrainModel ProjectMemberContrain
        {
            get { return _projectMemberContrain; }
            set { SetProperty(ref _projectMemberContrain, value); }
        }

        private string _projectName;
        public string ProjectName
        {
            get { return _projectName; }
            set { SetProperty(ref _projectName, value); }
        }
    }
}
