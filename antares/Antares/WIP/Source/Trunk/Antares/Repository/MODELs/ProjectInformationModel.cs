using System.Collections.Generic;
using System.Collections.ObjectModel;
using AntaresShell.BaseClasses;
using System;

namespace Repository.MODELs
{
    public class ProjectInformationModel : BindableBase
    {
        public ProjectInformationModel()
        {
          
        }

        public ProjectInformationModel(ProjectInformationModel target)
        {
            if (target != null)
            {
                ID = target.ID;
                Name = target.Name;
                Description = target.Description;
                StartDate = target.StartDate;
                EndDate = target.EndDate;
                Status = target.Status;
                Members = target.Members;
            }
        }

        private int _id = -1;
        public int ID { get { return _id; } set { SetProperty(ref _id, value); } }

        private string _projectName;
        public string Name { get { return _projectName; } set { SetProperty(ref _projectName, value); } }

        private string _desc;
        public string Description { get { return _desc; } set { SetProperty(ref _desc, value); } }

        private string _startDate;
        public string StartDate { get { return _startDate; } set { SetProperty(ref _startDate, value); } }

        private string _endDate;
        public string EndDate { get { return _endDate; } set { SetProperty(ref _endDate, value); } }

        private int _status;
        public int Status { get { return _status; } set { SetProperty(ref _status, value); } }

        private int _nubmerOfTask;
        public int NumberOfTask { get { return _nubmerOfTask; } set { SetProperty(ref _nubmerOfTask, value); } }

        private ObservableCollection<ProjectMemberContrainModel> _members;
        public ObservableCollection<ProjectMemberContrainModel> Members { get { return _members; } set { SetProperty(ref _members, value); } }
    }
}
