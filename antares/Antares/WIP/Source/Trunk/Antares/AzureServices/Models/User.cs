//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace AzureServices.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.ProjectMembers = new HashSet<ProjectMember>();
            this.Tasks = new HashSet<Task>();
        }
    
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<ProjectMember> ProjectMembers { get; set; }
        [IgnoreDataMember]
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
