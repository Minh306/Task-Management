using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.Areas.Admin.Models
{
    public class UserTaskViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public Nullable<int> TaskId { get; set; }
    }
}