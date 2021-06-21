using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.Models
{
    public class CommentTaskViewModel
    {
        public int Id { get; set; }
        public int? TaskId { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public string FullName { get; set; }
        public Boolean? IsActive { get; set; }
        public string CreatedDate { get; set; }
    }
}