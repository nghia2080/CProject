using AntaresShell.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MODELs
{
    public class ProjectMemberContrainModel : BindableBase
    {
        public ProjectMemberContrainModel()
        {
            
        }

        public ProjectMemberContrainModel(ProjectMemberContrainModel data)
        {
            ID = data.ID;
            ProjectID = data.ProjectID;
            ProjectName = data.ProjectName;
            UserID = data.UserID;
            Username = data.Username;
            Role = data.Role;
            IsConfirmed = data.IsConfirmed;
            IsActive = data.IsActive;
        }

        private int _id;
        public int ID { get { return _id; } set { SetProperty(ref _id, value); } }

        private int _projectID;
        public int ProjectID { get { return _projectID; } set { SetProperty(ref _projectID, value); } }

        private string _projectName;
        public string ProjectName { get { return _projectName; } set { SetProperty(ref _projectName, value); } }

        private int _userID;
        public int UserID { get { return _userID; } set { SetProperty(ref _userID, value); } }

        private string _username;
        public string Username { get { return _username; } set { SetProperty(ref _username, value); } }

        private string _role;
        public string Role
        {
            get { return _role; }
            set
            {
                SetProperty(ref _role, value);
            }
        }

        private bool _isConfirmed;
        public bool IsConfirmed { get { return _isConfirmed; } set { SetProperty(ref _isConfirmed, value); } }

        private bool _isActive;
        public bool IsActive { get { return _isActive; } set { SetProperty(ref _isActive, value); } }
    }
}
