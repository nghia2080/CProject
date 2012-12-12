using System;
using System.Text;
using AntaresShell.BaseClasses;

namespace Repository.MODELs
{
    public class TaskModel : SearchableBaseModel
    {
        public TaskModel()
        {

        }

        public TaskModel(TaskModel newTask)
        {
            ID = newTask.ID;
            Name = newTask.Name;
            Description = newTask.Description;
            Priority = newTask.Priority;
            Status = newTask.Status;
            Category = newTask.Category;
            RepeatType = newTask.RepeatType;
            Period = newTask.Period;
            StartTime = newTask.StartTime;
            StartDate = newTask.StartDate;
            EndTime = newTask.EndTime;
            EndDate = newTask.EndDate;
            UserID = newTask.UserID;
            ProjectID = newTask.ProjectID;
            IsConfirmed = newTask.IsConfirmed;
            IsBreakable = newTask.IsBreakable;
        }

        private int _id = -1;
        public int ID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private int _priority = 1;
        public int Priority
        {
            get { return _priority; }
            set { SetProperty(ref _priority, value); }
        }

        private int _status;
        public int Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        private int _category;
        public int Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        private int _repeatType;
        public int RepeatType
        {
            get { return _repeatType; }
            set { SetProperty(ref _repeatType, value); }
        }

        private int _period = 1;
        public int Period
        {
            get { return _period; }
            set { SetProperty(ref _period, value); }
        }

        private int? _startTime;
        public int? StartTime
        {
            get { return _startTime; }
            set { SetProperty(ref _startTime, value); }
        }

        private string _startDate;
        public string StartDate
        {
            get { return _startDate; }
            set
            {
                SetProperty(ref _startDate, value);
            }
        }

        private int _endTime;
        public int EndTime
        {
            get { return _endTime; }
            set { SetProperty(ref _endTime, value); }
        }

        private string _endDate;
        public string EndDate
        {
            get { return _endDate; }
            set { SetProperty(ref _endDate, value); }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }


        private int _userID;
        public int UserID
        {
            get { return _userID; }
            set { SetProperty(ref _userID, value); }
        }

        private int _projectID = -1;
        public int ProjectID
        {
            get { return _projectID; }
            set { SetProperty(ref _projectID, value); }
        }

        private bool? _sConfirmed;
        public bool? IsConfirmed
        {
            get { return _sConfirmed; }
            set { SetProperty(ref _sConfirmed, value); }
        }

        private bool _isBreakable = true;
        public bool IsBreakable
        {
            get { return _isBreakable; }
            set { SetProperty(ref _isBreakable, value); }
        }

        public override string ToSearchableString()
        {
            var build = new StringBuilder();
            build.Append(Name ?? string.Empty).Append("|");
            build.Append(Description ?? string.Empty).Append("|");
            build.Append(RepositoryUtils.GetCategoryNameFromId(Category)).Append("|");
            build.Append(RepositoryUtils.GetDateTimeFromStrings(StartDate, StartTime)).Append("|");
            build.Append(RepositoryUtils.GetDateTimeFromStrings(EndDate, EndTime));
            return build.ToString().Replace(":00 PM", "").Replace(":00 AM", "");
        }
    }
}
