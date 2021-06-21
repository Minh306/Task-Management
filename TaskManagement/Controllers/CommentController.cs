using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Commons;
using TaskManagement.Models;
using TaskManagement.Models.CommentTaskCommand;

namespace TaskManagement.Controllers
{
    public class CommentController : Controller
    {
        private SEP23Team7Entities db = new SEP23Team7Entities();
        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SaveComment(CommentSaveRequestCommand requestModel)
        {
            try
            {
                var commentTask = new CommentTask()
                {
                    TaskId = requestModel.TaskId,
                    UserId = requestModel.UserId,
                    Message = requestModel.Comment,
                    CreatedDate = DateTime.Now
                };
                db.CommentTasks.Add(commentTask);
                db.SaveChanges();
                return Json(commentTask.Id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        //[Route("{taskId}/GetComment")]
        public ActionResult GetCommentByTaskId(int taskId)
        {
            var result = db.SP_GetCommentByTaskId1(taskId).Select(s => new CommentTaskViewModel()
            {
                Id = s.Id,
                TaskId = s.TaskId,
                UserId = s.UserId,
                FullName = s.FullName,
                Comment = s.Message,
                IsActive = s.IsActive,
                CreatedDate = s.CreatedDate.Value.ToString("dd-MM-yyyy hh:mm:ss tt")
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetLastCommentId(int taskId)
        {
            var lastId = db.SP_GetLastCommentId_By_TaskId(taskId);
            return Json(new { CommentLastId = lastId }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RemoveComment(int commentId)
        {
            try
            {
                var result = db.SP_RemoveComment_By_Id(commentId);
                return Json(new { Status = StatusResponse.Success }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { Status = StatusResponse.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}