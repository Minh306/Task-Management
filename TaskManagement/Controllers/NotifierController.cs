using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    public class NotifierController : Controller
    {
        private SEP23Team7Entities db = new SEP23Team7Entities();
        // GET: Notifier
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetInfoToSendNotify(int taskId, string userAssign)
        {
            try
            {

                var _currentUserId = User.Identity.GetUserId();
                var task = db.TaskManages.Where(t => t.Id == taskId).FirstOrDefault();
                var userTask = db.UserTasks.Where(s => s.TaskId == taskId).FirstOrDefault();
                var oldUserAssign = userTask.UserId;
                var changeByInfo = db.Accounts.Where(a => a.UserId == _currentUserId).FirstOrDefault();
               
                var assignFromInfo = db.Accounts.Where(a => a.UserId == oldUserAssign).FirstOrDefault();
                var assignToInfo = db.Accounts.Where(a => a.UserId == userAssign).FirstOrDefault();
              
                if (oldUserAssign != userAssign) // change userAssign && diff currentUser
                {
                    Dictionary<string, object> objectResponse = new Dictionary<string, object>();
                    objectResponse.Add("ChangeById", changeByInfo.UserId);
                    objectResponse.Add("ChangeByFullName", changeByInfo.FullName);
                    objectResponse.Add("AssignFromId", assignFromInfo.UserId);
                    objectResponse.Add("AssignFromFullName", assignFromInfo.FullName);
                    objectResponse.Add("AssignToId", assignToInfo.UserId);
                    objectResponse.Add("AssignToFullName", assignToInfo.FullName);
                    objectResponse.Add("TaskId", task.Id);
                    objectResponse.Add("TaskName", task.Name);
                    return Json(objectResponse, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("", JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetAllNotifyByUserId()
        {
            var _currentUserId = User.Identity.GetUserId();
            //var result = db.Notifies.Where(n => n.AssignFrom == _currentUserId || n.AssignTo == _currentUserId).OrderByDescending(s =>s.Id).ToList();
            var result = db.GetListNotify_By_User1(_currentUserId).OrderByDescending(s => s.OrderId).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetHighestNotifyId()
        {
            var _currentUserId = User.Identity.GetUserId();
            var query = db.UserNotifies.Where(x => x.UserId == _currentUserId);
            var highestId = query.Any() ? query.Max(x => x.OrderId) : 0;
            return Json(highestId, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult TotalNotifierUnRead()
        {
            var _currentUserId = User.Identity.GetUserId();
            var noti = db.UserNotifies.Where(un => un.UserId == _currentUserId && un.IsRead == false).ToList();
            var result = 0;
            if (noti.Any()) result = noti.Count();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}