using System.Collections.ObjectModel;
using AntaresShell.BaseClasses;
using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;
using Repository.MODELs;
using AntaresShell.Localization;
using System.Linq;
using Repository.Repositories;
using System.Windows.Input;
using System;

namespace Antares.VIEWMODELs
{
    public class ProjectSubtaskViewModel : ViewModelBase
    {
        private ICommand _filterCommand;       public ICommand FilterCommand { get { return _filterCommand ?? (_filterCommand = new RelayCommand(ExecuteFilter)); } }



        //1: by phase
        //2: by member
        private int _currentFilter;

        private void ExecuteFilter(object obj)
        {
            var type = Convert.ToInt32(obj);

            switch (type)
            {
                case 1:
                    _currentFilter = 1;
                    BindData(_projectID, _currentFilter);
                    break;
                case 2:
                    _currentFilter = 2;
                    BindData(_projectID, _currentFilter);
                    break;
            }
        }

        private ObservableCollection<GroupCollection> _allGroups;
       public ObservableCollection<GroupCollection> AllGroups { get { return _allGroups; } set { SetProperty(ref _allGroups, value); } }



        private readonly int _projectID;

        public ProjectSubtaskViewModel(int projectID)
        {
            Messenger.Instance.Register<Refresh>(RefreshAll);
            _projectID = projectID;
            _currentFilter = 1;
            BindData(projectID, _currentFilter);          TaskRepository.Instance.Tasks.CollectionChanged += Tasks_CollectionChanged;
        }

        private void RefreshAll(object obj)
        {
            BindData(_projectID, _currentFilter);
        }

      
 void Tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
       
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                var taskModel = (TaskModel)e.NewItems[0];
                if(taskModel.ProjectID == _projectID)
                {
                    BindData(_projectID, _currentFilter);
                }
            }

            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                var taskModel = (TaskModel)e.OldItems[0];
                if (taskModel.ProjectID == _projectID)
                {
                    BindData(_projectID, _currentFilter);
                }
            }
        }

        private async void BindData(int projectID, int filter)
        {
            if (filter == 1)
            {
                var cate = await CategoryRepository.Instance.GetSubCategories();
         var dumb = new ObservableCollection<GroupCollection>();

      

                foreach (var categoryModel in cate)
                {
                    dumb.Add(new GroupCollection
                                 {
                                     Id = categoryModel.ID,
                                     GroupName = categoryModel.Name,
                                 });
                }

                        var projectTask = await TaskRepository.Instance.GetAllTasksForProject(projectID);
       
                if (projectTask != null)
                {
                    foreach (var grp in dumb)
                    {
                        var taskList = from taskModel in projectTask
                                       where taskModel.Category == grp.Id
                                       select taskModel;

                        var tasks = new ObservableCollection<TaskModel>(taskList);

                        foreach (var taskModel in tasks)
                        {
                            taskModel.Username =                         (await UserInformationRepository.Instance.GetUser(taskModel.UserID)).Username;
       
                        }

                        grp.GroupTasks = tasks;
                    }
                }

                AllGroups = dumb;

            }
            else
            {         var members = await ProjectMemberRepository.Instance.GetAllProjectsMember(_projectID);

      
         var dumb = new ObservableCollection<GroupCollection>();

      

                foreach (var member in members)
                {
                    dumb.Add(new GroupCollection
                    {
                        Id = member.UserID,
                        GroupName = (await UserInformationRepository.Instance.GetUser(member.UserID)).Username
                    });
                }

                
         var projectTask = await TaskRepository.Instance.GetAllTasksForProject(projectID);
       
                if (projectTask != null)
                {
                    foreach (var grp in dumb)
                    {
                        var taskList = from taskModel in projectTask
                                       where taskModel.UserID == grp.Id
                                       select taskModel;

                        var tasks = new ObservableCollection<TaskModel>(taskList);

                        foreach (var taskModel in tasks)
                        {
                            taskModel.Username = "";
                        }

                        grp.GroupTasks = tasks;
                    }
                }

                AllGroups = dumb;
            }
        }
    }
}
