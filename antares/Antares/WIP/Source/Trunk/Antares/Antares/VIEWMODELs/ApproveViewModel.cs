using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntaresShell.BaseClasses;
using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;
using AntaresShell.Localization;
using AntaresShell.NavigatorProvider;
using Repository.MODELs;
using Repository.Repositories;
using System.Windows.Input;
using Repository.ServiceConnection.Controllers;
using Repository.Sync;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Antares.VIEWMODELs
{
    public class ApproveViewModel : ViewModelBase
    {
        private ICommand _approvedCommand;
        public ICommand ApprovedCommand
        {
            get { return _approvedCommand ?? (_approvedCommand = new RelayCommand(ExecuteApprove)); }
        }

        private async void ExecuteApprove(object obj)
        {
            if ((SelectedTasks == null || SelectedTasks.Count == 0) && (SelectedProjects.Count == 0 || SelectedProjects == null))
            {
                return;
            }

            var messageDialog = new MessageDialog(LanguageProvider.Resource["Msg_ConfirmApprove"]);

            // Add commands and set their callbacks
            messageDialog.Commands.Add(new UICommand(LanguageProvider.Resource["Msg_Yes"], command => Approved()));

            messageDialog.Commands.Add(new UICommand(LanguageProvider.Resource["Msg_No"], command => { }));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private async void Approved()
        {
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Visible;
            try
            {
                int successCount = 0;
                if (SelectedTasks != null)
                {
                    foreach (var selectedTask in SelectedTasks)
                    {
                        var taskModel = selectedTask as TaskModel;
                        if (taskModel != null)
                        {
                            taskModel.IsConfirmed = true;
                            var res = await TaskRepository.Instance.UpdateTask(taskModel);
                            if (res.IsSuccessStatusCode)
                            {
                                successCount++;
                            }
                        }
                    }
                }

                if (SelectedProjects != null)
                {
                    foreach (var extendProjectMember in SelectedProjects)
                    {
                        var projectMemberContrainModel = extendProjectMember as ProjectMemberContrainModel;
                        if (projectMemberContrainModel != null)
                        {
                            projectMemberContrainModel.IsConfirmed = true;
                            var res1 = await ProjectMemberRepository.Instance.EditContrain(projectMemberContrainModel);
                            if (res1.IsSuccessStatusCode)
                            {
                                successCount++;
                            }
                        }
                    }
                }

                var numberOfTask = (SelectedTasks == null ? 0 : SelectedTasks.Count) +
                           (SelectedProjects == null ? 0 : SelectedProjects.Count);

                Navigator.Instance.DisplayStatus(numberOfTask == successCount
                                                     ? ConnectionStatus.Done
                                                     : ConnectionStatus.Error);
                BindData();
            }
            catch
            {
                Navigator.Instance.DisplayStatus(ConnectionStatus.Error);
            }
            finally
            {
                Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private ICommand _unApprovedCommand;
        public ICommand UnapprovedCommand
        {
            get { return _unApprovedCommand ?? (_unApprovedCommand = new RelayCommand(ExecuteUnapprove)); }
        }

        private async void ExecuteUnapprove(object obj)
        {
            if ((SelectedTasks == null || SelectedTasks.Count == 0) && (SelectedProjects.Count == 0 || SelectedProjects == null))
            {
                return;
            }

            var messageDialog = new MessageDialog(LanguageProvider.Resource["Msg_ConfirmReject"]);

            // Add commands and set their callbacks
            messageDialog.Commands.Add(new UICommand(LanguageProvider.Resource["Msg_Yes"], command => Rejected()));

            messageDialog.Commands.Add(new UICommand(LanguageProvider.Resource["Msg_No"], command => { }));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private async void Rejected()
        {
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Visible;
            try
            {
                int successCount = 0;
                if (SelectedTasks != null)
                {
                    foreach (var selectedTask in SelectedTasks)
                    {
                        var taskModel = selectedTask as TaskModel;
                        if (taskModel != null)
                        {
                            taskModel.IsConfirmed = true;
                            var res = await TaskRepository.Instance.DeleteTask(taskModel.ID);
                            if (res.IsSuccessStatusCode)
                            {
                                successCount++;
                            }
                        }
                    }
                }

                if (SelectedProjects != null)
                {
                    foreach (var extendProjectMember in SelectedProjects)
                    {
                        var projectMemberContrainModel = extendProjectMember as ProjectMemberContrainModel;
                        if (projectMemberContrainModel != null)
                        {
                            projectMemberContrainModel.IsConfirmed = true;
                            var res1 = await ProjectMemberRepository.Instance.DeleteMember(projectMemberContrainModel.ID);
                            if (res1.IsSuccessStatusCode)
                            {
                                successCount++;
                            }
                        }
                    }
                }

                var numberOfTask = (SelectedTasks == null ? 0 : SelectedTasks.Count) +
                           (SelectedProjects == null ? 0 : SelectedProjects.Count);

                Navigator.Instance.DisplayStatus(numberOfTask == successCount
                                                     ? ConnectionStatus.Done
                                                     : ConnectionStatus.Error);
                BindData();
            }
            catch
            {
                Navigator.Instance.DisplayStatus(ConnectionStatus.Error);
            }
            finally
            {
                Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        public IList<object> SelectedTasks { get; set; }
        public IList<object> SelectedProjects { get; set; }

        private ObservableCollection<TaskModel> _unapprovedTasks;
        public ObservableCollection<TaskModel> UnapprovedTasks
        {
            get { return _unapprovedTasks; }
            set { SetProperty(ref _unapprovedTasks, value); }
        }

        private ObservableCollection<ProjectMemberContrainModel> _unapprovedProjects;
        public ObservableCollection<ProjectMemberContrainModel> UnapprovedProjects
        {
            get { return _unapprovedProjects; }
            set { SetProperty(ref _unapprovedProjects, value); }
        }

        public ApproveViewModel()
        {
            Messenger.Instance.Register<Refresh>(RefreshAll);
            BindData();
        }

        private void RefreshAll(object obj)
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

                UnapprovedTasks = await UpdateTask(new ObservableCollection<TaskModel>(temp));

                //UnapprovedTasks = await ConvertTask(tasks);
            }


            var projects = await ProjectMemberRepository.Instance.GetAllProjects(GlobalData.MyUserID);
            if (projects != null)
            {
                var temp = from project in projects
                           where project.IsConfirmed != true
                           select project;

                UnapprovedProjects = await UpdateProject(new ObservableCollection<ProjectMemberContrainModel>(temp));

                // UnapprovedProjects = await ConvertProject(projects);
            }
        }

        private async Task<ObservableCollection<ProjectMemberContrainModel>> UpdateProject(IEnumerable<ProjectMemberContrainModel> old)
        {
            var projectList = await ProjectRepository.Instance.GetAllProjects();

            var result = new ObservableCollection<ProjectMemberContrainModel>(old);
            foreach (var projectMemberContrainModel in result)
            {
                if (projectMemberContrainModel.ProjectID != -1)
                {
                    projectMemberContrainModel.ProjectName = projectList.FirstOrDefault(p => p.ID == projectMemberContrainModel.ProjectID).Name;
                }
                else
                {
                    projectMemberContrainModel.ProjectName = "None";
                }
            }

            return result;
        }

        private async Task<ObservableCollection<TaskModel>> UpdateTask(IEnumerable<TaskModel> old)
        {
            var projectList = await ProjectRepository.Instance.GetAllProjects();

            var result = new ObservableCollection<TaskModel>(old);
            foreach (var taskmodel in result)
            {
                if (taskmodel.ProjectID != -1)
                {
                    taskmodel.ProjectName = projectList.FirstOrDefault(p => p.ID == taskmodel.ProjectID).Name;
                }
                else
                {
                    taskmodel.ProjectName = "None";
                }
            }

            return result;
        }
    }

}
