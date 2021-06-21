using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.Areas.Admin.Models
{
    public class TaskRelationShipViewModel
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> TaskId { get; set; }
    }
}