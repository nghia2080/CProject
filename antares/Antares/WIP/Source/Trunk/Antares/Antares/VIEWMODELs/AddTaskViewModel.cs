using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;
using AntaresShell.Localization;
using AntaresShell.Logger;
using AntaresShell.NavigatorProvider;
using Repository;
using Repository.MODELs;
using Repository.Repositories;
using Repository.Sync;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using System.Linq;
using AntaresShell.BaseClasses;

namespace Antares.VIEWMODELs
{
    public class AddTaskViewModel : ViewModelBase
    {
        private ICommand _addCommand;
        public ICommand SaveCommand
        {
            get { return _addCommand ?? (_addCommand = new RelayCommand(ExecuteSaveCommand)); }
        }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = new RelayCommand(ExecuteDeleteCommand)); }
        }

        private MessageDialog errorMes;
        private bool validated;

        private async void ExecuteDeleteCommand(object obj)
        {
            // Create the message dialog and set its content and title
            var messageDialog = new MessageDialog(LanguageProvider.Resource["Msg_DeleteTask"]);

            // Add commands and set their callbacks
            messageDialog.Commands.Add(new UICommand(LanguageProvider.Resource["Msg_Yes"], command => DeleteTask()));

            messageDialog.Commands.Add(new UICommand(LanguageProvider.Resource["Msg_No"], command => { }));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private async void DeleteTask()
        {
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Visible;
            var res = await TaskRepository.Instance.DeleteTask(Information.ID);
            Navigator.Instance.DisplayStatus(res.IsSuccessStatusCode
                                            ? ConnectionStatus.Done
                                            : ConnectionStatus.Error);
            if (res.IsSuccessStatusCode)
            {
                Navigator.Instance.HideTimelinePopup();
            }
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void ExecuteSaveCommand(object obj)
        {
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Visible;
            var messContent = string.Empty;

            // Validate name.
            if (string.IsNullOrEmpty(Information.Name))
            {
                GlobalData.TemporaryTask = Information;
                Navigator.Instance.DisplayStatus(ConnectionStatus.Error);
                Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
                validated = false;
                return;
            }

            var sd = RepositoryUtils.GetDateTimeFromStrings(Information.StartDate, Information.StartTime);
            var ed = RepositoryUtils.GetDateTimeFromStrings(Information.EndDate, Information.EndTime);

            // Need > 30 mins ?
            if (ed - sd < new TimeSpan(0, 0, 30))
            {
                GlobalData.TemporaryTask = Information;
                Navigator.Instance.DisplayStatus(ConnectionStatus.Error);
                Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
                validated = false;
                return;
            }

            // Validate 

            // Add.
            if (Information.ID == -1)
            {
                try
                {
                    if(Information.ProjectID != -1)
                    {
                        var members = await ProjectMemberRepository.Instance.GetAllProjectsMember(Information.ProjectID);

                        var pm = members.First(p => !string.IsNullOrEmpty(p.Role));

                        if (pm.UserID == GlobalData.MyUserID)
                        {
                            Information.IsConfirmed = Information.UserID == GlobalData.MyUserID;
                        }
                        else
                        {
                            // Have no right to add task.
                            GlobalData.TemporaryTask = Information;
                            Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
                            Navigator.Instance.DisplayStatus(ConnectionStatus.Error);
                            return;
                        }
                    }
                    

                    var response = await TaskRepository.Instance.AddNewTask(Information);
                    Navigator.Instance.DisplayStatus(response != null
                                      ? ConnectionStatus.Done
                                      : ConnectionStatus.Error);

                    GlobalData.TemporaryTask = response == null ? Information : null;
                }
                catch (Exception ex)
                {
                    LogManager.Instance.LogInfo("Add task ex: " + ex.Message);
                }
            }
            //Update
            else
            {
                if (Information.ProjectID != -1)
                {
                    // Authenticate
                    Information.IsConfirmed = Information.UserID == GlobalData.MyUserID;
                }

                var response = await TaskRepository.Instance.UpdateTask(Information);
                Navigator.Instance.DisplayStatus(response.IsSuccessStatusCode
                                                ? ConnectionStatus.Done
                                                : ConnectionStatus.Error);

                GlobalData.TemporaryTask = !response.IsSuccessStatusCode ? Information : null;
            }

            Navigator.Instance.HideTimelinePopup();
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
        }

        private TaskModel _information;
        public TaskModel Information
        {
            get { return _information; }
            set { SetProperty(ref _information, value); }
        }

        private ObservableCollection<CategoryModel> _categories;
        public ObservableCollection<CategoryModel> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        private ObservableCollection<CategoryModel> _subCategories;
        public ObservableCollection<CategoryModel> SubCategories
        {
            get { return _subCategories; }
            set { SetProperty(ref _subCategories, value); }
        }
        private ObservableCollection<CategoryModel> _cate;
        private ObservableCollection<CategoryModel> _subCate;

        private ObservableCollection<PriorityModel> _priorities;
        public ObservableCollection<PriorityModel> Priorities
        {
            get { return _priorities; }
            set { SetProperty(ref _priorities, value); }
        }

        private ObservableCollection<RepeatTypeModel> _repeatTypes;
        public ObservableCollection<RepeatTypeModel> RepeatTypes
        {
            get { return _repeatTypes; }
            set { SetProperty(ref _repeatTypes, value); }
        }

        private ObservableCollection<string> _status;
        public ObservableCollection<string> Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        private ObservableCollection<ProjectInformationModel> _projects;
        public ObservableCollection<ProjectInformationModel> Projects
        {
            get { return _projects; }
            set { SetProperty(ref _projects, value); }
        }

        private bool _readOnly;
        public bool ReadOnly
        {
            get { return _readOnly; }
            set { SetProperty(ref _readOnly, value); }
        }

        private ProjectInformationModel _selectedProject;
        public ProjectInformationModel SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                SetProperty(ref _selectedProject, value);

                Information.ProjectID = _selectedProject.ID;

                if (Information.ProjectID == -1)
                {
                    Information.UserID = GlobalData.MyUserID;
                    VisibleMember = false;
                    Categories = _cate;
                }
                else
                {
                    ReadOnly = true;

                    BindingMember();
                    VisibleMember = true;
                    Categories = _subCate;

                    UnlockReadonlyForPM(Information.ProjectID);
                }

            }
        }

        private async void UnlockReadonlyForPM(int pid)
        {
            var members = await ProjectMemberRepository.Instance.GetAllProjectsMember(pid);

            var pm = members.First(p => !string.IsNullOrEmpty(p.Role));

            if(pm.UserID == GlobalData.MyUserID)
            {
                ReadOnly = false;
            }
        }

        private async void BindingMember()
        {
            var mbers = await ProjectMemberRepository.Instance.GetAllProjectsMember(Information.ProjectID);
            var temp = new ObservableCollection<UserModel>();
            foreach (var projectMemberContrainModel in mbers)
            {
                if (projectMemberContrainModel.IsActive && projectMemberContrainModel.IsConfirmed)
                {
                    temp.Add(await UserInformationRepository.Instance.GetUser(projectMemberContrainModel.UserID));
                }
            }

            ProjectMembers = temp;
        }

        private bool _visibleMember;
        public bool VisibleMember { get { return _visibleMember; } set { SetProperty(ref _visibleMember, value); } }

        private ObservableCollection<UserModel> _projectMembers;
        public ObservableCollection<UserModel> ProjectMembers
        {
            get { return _projectMembers; }
            set
            {
                SetProperty(ref _projectMembers, value);
            }
        }

        private UserModel _selectedMember;
        public UserModel SelectedMember
        {
            get { return _selectedMember; }
            set
            {
                SetProperty(ref _selectedMember, value);
                if (value != null)
                {
                    Information.UserID = _selectedMember.UserID;
                }
            }
        }

        private CategoryModel _selectedCategory;
        public CategoryModel SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                SetProperty(ref _selectedCategory, value);
                if (value != null)
                    Information.Category = _selectedCategory.ID;
            }
        }

        public AddTaskViewModel()
            : this(new TaskModel { ID = -1 })
        {

        }

        public AddTaskViewModel(TaskModel pim)
        {
            Messenger.Instance.Register<RebindCbo>(Rebind);

            BindingStaticData(pim);
        }

        private void Rebind(object obj)
        {
            var type = (RebindCbo)obj;
            if (type == RebindCbo.RebindProject && Projects != null)
            {
                SelectedProject = Projects.FirstOrDefault(p => p.ID == Information.ProjectID);
            }
            else if (type == RebindCbo.RebindCategory && Projects != null)
            {
                if (Categories != null)
                {
                    SelectedCategory = Categories.FirstOrDefault(p => p.ID == Information.Category);
                }
            }
            else if (type == RebindCbo.RebindMember && Projects != null)
            {
                if (ProjectMembers != null)
                {
                    SelectedMember = ProjectMembers.FirstOrDefault(p => p.UserID == Information.UserID);
                }
            }
        }

        private async void BindingStaticData(TaskModel pim)
        {
            Status = new ObservableCollection<string> { LanguageProvider.Resource["Tsk_Status_NotDone"], LanguageProvider.Resource["Tsk_Status_Done"] };

            var temp = await ProjectRepository.Instance.GetAllProjects();
            if (temp != null)
            {
                var t = new ObservableCollection<ProjectInformationModel>(temp);
                    
                t.Insert(0,new ProjectInformationModel
                            {
                                ID = -1,
                                Name = LanguageProvider.Resource["Tsk_Project_None"]
                            });

                Projects = t;
                //SelectedProjectIndex = 0;//Projects.IndexOf(Projects.FirstOrDefault(p => p.ID == (Information.ProjectID ?? -1)));
            }

            _subCate = await CategoryRepository.Instance.GetSubCategories();
            _cate = await CategoryRepository.Instance.GetMainCategories();
            //Categories = new ObservableCollection<CategoryModel>(_subCate.Union(_cate));

            Priorities = await PriorityRepository.Instance.GetAllPriorities();
            RepeatTypes = await RepeatTypeRepository.Instance.GetAllRepeatTypes();

            //pim.ProjectID = -1;
            if (GlobalData.TemporaryTask != null)
            {
                Information = GlobalData.TemporaryTask;
            }
            else
            {
                if (pim.ID == -1)
                {
                    pim.UserID = GlobalData.MyUserID;
                }

                Information = new TaskModel(pim);
            }
        }
    }
}
