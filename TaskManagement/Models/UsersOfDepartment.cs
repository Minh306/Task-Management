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
    
    public partial class UsersOfDepartment
    {
        public string UserId { get; set; }
        public string DepartmentId { get; set; }
        public Nullable<System.DateTime> DateGetIn { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Department Department { get; set; }
    }
}