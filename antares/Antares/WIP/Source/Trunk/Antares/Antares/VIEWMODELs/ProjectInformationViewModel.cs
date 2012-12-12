using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Json;
using System.Windows.Input;
using System.Linq;
using AntaresShell.Common;
using AntaresShell.Localization;
using AntaresShell.NavigatorProvider;
using Repository.MODELs;
using AntaresShell.BaseClasses;
using Repository.Repositories;
using Repository.Sync;
using Windows.UI.Xaml;
using Repository.ServiceConnection.Controllers;
using AntaresShell.Common.MessageTemplates;
using Antares.VIEWs;

namespace Antares.VIEWMODELs
{
    public class ProjectInformationViewModel : ViewModelBase
    {
        private ObservableCollection<string> _status;
        public ObservableCollection<string> Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        private ICommand _addMemberCommand;
        public ICommand AddMemberCommand
        {
            get { return _addMemberCommand ?? (_addMemberCommand = new RelayCommand(ExecuteAddMemberCommand)); }
        }

        private ICommand _addCommand;
        public ICommand SaveCommand
        {
            get { return _addCommand ?? (_addCommand = new RelayCommand(ExecuteSaveCommand)); }
        }

        private ICommand _deleCommand;
        public ICommand DeleteCommand
        {
            get { return _deleCommand ?? (_deleCommand = new RelayCommand(ExecuteDeleteCommand)); }
        }

        private async void ExecuteDeleteCommand(object obj)
        {
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Visible;

            // Condition: No task and no other member except PM

            var memberList = await ProjectMemberRepository.Instance.GetAllProjectsMember(Information.ID);
            if (memberList.Count > 1)
            {
                Navigator.Instance.DisplayStatus(ConnectionStatus.Error);
                Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
                return;
            }
            // Validate number tasks.
            var taskAll = await TaskRepository.Instance.GetAllTasksForProject(Information.ID);
            if (taskAll.Count > 0)
            {
                Navigator.Instance.DisplayStatus(ConnectionStatus.Error);
                Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
                return;
            }

            if (memberList.Count != 0)
            {
                var res = await ProjectMemberRepository.Instance.DeleteMember(memberList[0].ID);
                if (!res.IsSuccessStatusCode)
                {
                    Navigator.Instance.DisplayStatus(ConnectionStatus.Error);
                    Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
                    return;
                }
            }

            var res1 = await ProjectRepository.Instance.DeleteProject(Information.ID);
            if (!res1.IsSuccessStatusCode)
            {
                Navigator.Instance.DisplayStatus(ConnectionStatus.Error);
                Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
                return;
            }

            Navigator.Instance.DisplayStatus(ConnectionStatus.Done);
            GlobalData.SelectedProjects = -1;
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
            Navigator.Instance.NavigateTo(typeof(ProjectManagerPage));
        }

        private async void ExecuteAddMemberCommand(object obj)
        {
            Messenger.Instance.Notify(MemberProgressRing.Show);
            var username = obj as string;
            var user = await UserInformationRepository.Instance.GetUser(username);
            if (user == null)
            {
                //Highlight textbox
                Messenger.Instance.Notify(HighlightTextBox.Fail);
            }
            else
            {
                var response = await ProjectMemberRepository.Instance.AddContrain(new ProjectMemberContrainModel
                {
                    ProjectID = Information.ID,
                    Role = "",
                    UserID = user.UserID,
                    IsActive = true,
                    IsConfirmed = false,
                });

                if (response != null)
                {
                    response.Username = user.Username;
                    Information.Members.Add(response);
                    //Highlight textbox
                    Messenger.Instance.Notify(HighlightTextBox.Success);
                }

            }
            Messenger.Instance.Notify(MemberProgressRing.Hide);
        }

        private async void ExecuteSaveCommand(object obj)
        {
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Visible;

            if (string.IsNullOrEmpty(Information.Name))
            {
                Navigator.Instance.DisplayStatus(ConnectionStatus.Error);
                Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
                return;
            }

            // Add
            if (Information.ID == -1)
            {
                var response = await ProjectRepository.Instance.AddNewProject(Information);

                if (response != null)
                {
                    Information = response;

                    var response1 =
                        await ProjectMemberRepository.Instance.AddContrain(new ProjectMemberContrainModel
                                                                             {
                                                                                 ProjectID = response.ID,
                                                                                 Role = "PM",
                                                                                 UserID = GlobalData.MyUserID,
                                                                                 IsActive = true,
                                                                                 IsConfirmed = true,
                                                                             });

                    Navigator.Instance.DisplayStatus(response1 != null
                                            ? ConnectionStatus.Done
                                            : ConnectionStatus.Error);

                    if (response1 != null)
                    {

                        response1.Username = (await UserInformationRepository.Instance.GetUser(response1.UserID)).Username;

                        Information.Members = new ObservableCollection<ProjectMemberContrainModel>
                                                  {
                                                      response1,
                                                  };

                        GlobalData.SelectedProjects = response.ID;
                        Messenger.Instance.Notify(UpdateProject.NewlyAdded);
                     
                        //hide save btn
                        Messenger.Instance.Notify(HideSaveBtn.Hide);
                    }
                }
                else
                {
                    Navigator.Instance.DisplayStatus(ConnectionStatus.Error);
                }

            }
            //Update
            else
            {
                var response = await ProjectRepository.Instance.UpdateProject(Information);
                Navigator.Instance.DisplayStatus(response.IsSuccessStatusCode
                                                ? ConnectionStatus.Done
                                                : ConnectionStatus.Error);
                if (response.IsSuccessStatusCode)
                {
                    //hide save btn
                    Messenger.Instance.Notify(HideSaveBtn.Hide);
                }
            }

            Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
        }

        private ProjectInformationModel _information;
        public ProjectInformationModel Information
        {
            get { return _information; }
            set { SetProperty(ref _information, value); }
        }

        private bool _readOnly;

        public bool ReadOnly
        {
            get { return _readOnly; }
            set { SetProperty(ref _readOnly, value); }
        }

        private int _pid;

        public ProjectInformationViewModel(int pid)
        {
            Messenger.Instance.Register<Refresh>(RefreshAll);
            Messenger.Instance.Register<RefreshMember>(o => GetMemberList(Information.ID));

            Status = new ObservableCollection<string> { LanguageProvider.Resource["Prj_Status_Inactive"], LanguageProvider.Resource["Prj_Status_Active"] };
            _pid = pid;
            BindData(pid);

            ReadOnly = !ProjectMemberRepository.Instance.IsManager(Information.ID);
        }

        private void RefreshAll(object obj)
        {
            BindData(_pid);
        }

        private async void BindData(int pid)
        {
            Information = pid == -1 ? new ProjectInformationModel { ID = -1 } : new ProjectInformationModel((await ProjectRepository.Instance.GetAllProjects()).FirstOrDefault(p => p.ID == pid));

            if (Information != null)
            {
                GetMemberList(Information.ID);
            }
        }

        private async void GetMemberList(int projectID)
        {
            Messenger.Instance.Notify(MemberProgressRing.Show);
            if (projectID == -1)
            {
                Messenger.Instance.Notify(MemberProgressRing.Hide);
                return;
            }

            var members = (ObservableCollection<ProjectMemberContrainModel>)await ProjectMemberRepository.Instance.GetAllProjectsMember(projectID);

            if (members != null)
            {
                foreach (var member in members)
                {
                    var user = await UserInformationRepository.Instance.GetUser(member.UserID);
                    if (user != null)
                    {
                        member.Username = user.Username;
                    }
                    Information.Members = members;
                }
            }
            Messenger.Instance.Notify(MemberProgressRing.Hide);
        }
    }
}
