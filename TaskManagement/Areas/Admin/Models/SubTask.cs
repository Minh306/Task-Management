using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.Areas.Admin.Models
{
    public class SubTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}