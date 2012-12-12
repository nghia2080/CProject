using System.Collections.ObjectModel;
using AntaresShell.BaseClasses;

namespace Repository.MODELs
{
    public class GroupCollection : BindableBase
    {
        private int _id;
        public int Id { get { return _id; } set { SetProperty(ref _id, value); } }

        private string _groupName;
        public string GroupName { get { return _groupName; } set { SetProperty(ref _groupName, value); } }

        private ObservableCollection<TaskModel> _groupTasks;
        public ObservableCollection<TaskModel> GroupTasks { get { return _groupTasks; } set { SetProperty(ref _groupTasks, value); } }
    }
}
