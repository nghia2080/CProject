using AntaresShell.BaseClasses;
using System;

namespace Repository.MODELs
{
    public class UserModel : BindableBase
    {
        public UserModel()
        {
            
        }

        public UserModel(UserModel target)
        {
            UserID = target.UserID;
            Username = target.Username;
            Phone = target.Phone;
            Email = target.Email;
            DOB = target.DOB;
        }

        private int _userID;
        public int UserID { get { return _userID; } set { SetProperty(ref _userID, value); } }

        private string _userName;
        public string Username { get { return _userName; } set { SetProperty(ref _userName, value); } }

        private string _phone;
        public string Phone { get { return _phone; } set { SetProperty(ref _phone, value); } }

        private string _email;
        public string Email { get { return _email; } set { SetProperty(ref _email, value); } }

        private string _dob;
        public string DOB { get { return _dob; } set { SetProperty(ref _dob, value); } }
    }
}
