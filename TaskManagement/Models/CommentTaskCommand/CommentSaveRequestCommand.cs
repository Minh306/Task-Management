using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.Models.CommentTaskCommand
{
    public class CommentSaveRequestCommand
    {
        public int TaskId { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}