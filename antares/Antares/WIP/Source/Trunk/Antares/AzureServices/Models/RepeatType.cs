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
    
    public partial class RepeatType
    {
        public RepeatType()
        {
            this.Tasks = new HashSet<Task>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
