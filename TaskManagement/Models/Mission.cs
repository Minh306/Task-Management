//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaskManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Mission
    {
        public System.Guid Id { get; set; }
        public string MissionName { get; set; }
        public string MissionInfor { get; set; }
        public string CreatorId { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public Nullable<System.DateTime> DateDeadLine { get; set; }
        public Nullable<System.DateTime> DateStart { get; set; }
        public Nullable<System.DateTime> DateSubmit { get; set; }
        public Nullable<int> Priority { get; set; }
        public Nullable<int> Proportion { get; set; }
        public Nullable<int> Status { get; set; }
        public string Acceptance { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual MissionInforLeader MissionInforLeader { get; set; }
        public virtual MissionMember MissionMember { get; set; }
    }
}
