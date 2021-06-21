using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Areas.Admin.Interfaces;
using TaskManagement.Areas.Admin.Models;
using TaskManagement.Commons;
using TaskManagement.Hubs;
using TaskManagement.Models;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;

namespace TaskManagement.Areas.Admin.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin,Leader,Member")]
    public class TaskManagesController : Controller
    {
        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";
            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (type == "error")
            {
                TempData["AlertType"] = "alert-danger";
            }
        }
        private SEP23Team7Entities db = new SEP23Team7Entities();
        private readonly IMapper _mapper;

        public TaskManagesController()
        {
          
            _mapper = AutoMapperProfile.Mapper;
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserID"] != null)
                base.OnActionExecuting(filterContext);
            else
                filterContext.Result = new RedirectResult("~/Account/Login");
        }

        // GET: Admin/TaskManages
        /// <summary>
        /// Hiển thị danh sách nhiệm vụ 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var finalTask = db.TaskRelationships.Where(task => task.ParentId == 0).Select(s => s.TaskId).ToList();
            // get task to view
            var taskManage = db.TaskManages.Where(t => finalTask.Contains(t.Id)).ToList();
            // map view model
            var res = _mapper.Map<List<TaskManageViewModel>>(taskManage);
            var fullName = db.Accounts.ToList();
            res.ForEach(ts =>
            {
                if (fullName.Select(f => f.UserId).ToList().Contains(ts.CreatedBy))
                {
                    ts.UserFullName = fullName.Where(fl => fl.UserId == ts.CreatedBy).FirstOrDefault().FullName;
                }
            });
            return View(res);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskManage taskManage = db.TaskManages.FirstOrDefault(x => x.Id == id);
            UserTask task = db.UserTasks.FirstOrDefault(x => x.TaskId == id);
            Account u = db.Accounts.FirstOrDefault(x => x.UserId == task.UserId);
            var taskManageV = _mapper.Map<TaskManageViewModel>(taskManage);
            taskManageV.UserFullName = u.FullName;
            if (taskManageV == null)
            {
                return HttpNotFound();
            }
            ViewBag.FullNameCreatedBy = db.Accounts.Where(ac => ac.UserId == taskManage.CreatedBy).FirstOrDefault()?.FullName;
            return View(taskManageV);
        }

        // GET: Admin/TaskManages/Create
        [Authorize]
        //[Authorize(Roles = "Admin")]
        public ActionResult Create(int? parentId)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Value = TaskPriority.High, Text = TaskPriority.High });
            items.Add(new SelectListItem { Value = TaskPriority.Medium, Text = TaskPriority.Medium });
            items.Add(new SelectListItem { Value = TaskPriority.Low, Text = TaskPriority.Low });
            ViewBag.Priority = new SelectList(items, "Value", "Text", TaskPriority.High);
            ViewBag.ParentId = parentId;
            return View();
        }

        // POST: Admin/TaskManages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "parentId,Name,StartDate,EndDate,Description,Rate,Priority,Targets,UnitTargets")] TaskManage taskManage, HttpPostedFileBase FilesUpload, int? parentId)
        {
            CheckNameMission(taskManage);
            CheckDate(taskManage);
            if (ModelState.IsValid)
            {
                taskManage.CreatedDate = DateTime.Now;
                taskManage.CreatedBy = User.Identity.GetUserId();
                taskManage.Status = Commons.TaskStatus.Todo;
                taskManage.Rate = 100;               
                db.TaskManages.Add(taskManage);
                var resSave = db.SaveChanges();
                if (resSave > 0)
                {
                    if (FilesUpload != null)
                    {
                        // Converting to bytes.  
                        byte[] uploadedFile = new byte[FilesUpload.InputStream.Length];
                        FilesUpload.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                        var fileUpload = new FileUpload();
                        fileUpload.FileName = FilesUpload.FileName;
                        fileUpload.FileType = FilesUpload.ContentType;
                        fileUpload.FileContent = Convert.ToBase64String(uploadedFile);
                        fileUpload.TaskId = taskManage.Id;
                        db.FileUploads.Add(fileUpload);
                    }

                    var subTask = new TaskRelationship();
                    if (parentId != null && parentId > 0) subTask.ParentId = parentId;
                    else subTask.ParentId = 0;
                    subTask.TaskId = taskManage.Id;
                    db.TaskRelationships.Add(subTask);

                    var userTask = new UserTask()
                    {
                        TaskId = taskManage.Id,
                        UserId = User.Identity.GetUserId()
                    };
                    db.UserTasks.Add(userTask);

                    db.SaveChanges();
                    SetAlert("Thêm nhiệm vụ thành công !!!", "success");
                }
                if (parentId != null && parentId > 0) return RedirectToAction("Edit", new { id = parentId });
                return RedirectToAction("Index");
            }
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = TaskPriority.High, Value = TaskPriority.High });
            items.Add(new SelectListItem { Text = TaskPriority.Medium, Value = TaskPriority.Medium });
            items.Add(new SelectListItem { Text = TaskPriority.Low, Value = TaskPriority.Low });
            ViewBag.Priority = new SelectList(items, "Value", "Text", taskManage.Priority);
            return View(taskManage);
        }

        // GET: Admin/TaskManages/Edit/5
        public ActionResult Edit(int? id, int? source)
        {
            var currentUserId = User.Identity.GetUserId();
            ViewBag.UserInfo = db.Accounts.Where(c => c.UserId == currentUserId).Select(s => new { UserId = s.UserId, TaskId = id, FullName = s.FullName }).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            #region update status notify
            if (source != null)
            {
                var userNotify = db.UserNotifies.Where(un => un.OrderId == source && un.UserId == currentUserId).FirstOrDefault();
                if(userNotify != null)
                {
                    userNotify.IsRead = true;
                    db.Entry(userNotify).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            #endregion

            var listIdSubtask = db.TaskRelationships.Where(t => t.ParentId == id).Select(s => s.TaskId).ToList();
            var getSubTask = db.TaskManages.Where(t => listIdSubtask.Contains(t.Id)).Select(s => new SubTask() { Id = s.Id, Name = s.Name, Status = s.Status, UpdatedDate = s.ModifiedDate }).ToList();
            ViewBag.SubTasks = getSubTask;
            // Parent current Task
            var getParentTaskId = db.TaskRelationships.Where(t => t.TaskId == id).Select(s => s.ParentId).FirstOrDefault();
            var getParentTask = new SubTask();
            if (getParentTaskId != null && getParentTaskId > 0)
            {
                getParentTask = db.TaskManages.Where(x => x.Id == getParentTaskId).Select(p => new SubTask() { Id = p.Id, Name = p.Name, Status = p.Status, UpdatedDate = p.ModifiedDate }).FirstOrDefault();
            }
            ViewBag.ParentCurrentTaskId = getParentTaskId;
            ViewBag.ParentTask = getParentTask;
            TaskManage taskManageEdit = db.TaskManages.Find(id);
            ViewBag.FullNameCreatedBy = db.Accounts.Where(ac => ac.UserId == taskManageEdit.CreatedBy).FirstOrDefault()?.FullName;

            var listFile = db.FileUploads.Where(f => f.TaskId == id).Select(s => new FileViewModel(){ Id = s.Id, FileName = s.FileName }).ToList();
            ViewBag.ListFile = listFile;
            if (taskManageEdit == null)
            {
                return HttpNotFound();
            }
            var listUser = db.Accounts.Where(a => a.AspNetUser.AspNetRoles.FirstOrDefault().Name != Commons.Role.Owner).ToList();
            ViewBag.UserAssign = new SelectList(listUser, "UserId", "FullName", db.UserTasks.Where(us => us.TaskId == taskManageEdit.Id).FirstOrDefault().UserId);
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = TaskPriority.High, Value = TaskPriority.High });
            items.Add(new SelectListItem { Text = TaskPriority.Medium, Value = TaskPriority.Medium });
            items.Add(new SelectListItem { Text = TaskPriority.Low, Value = TaskPriority.Low });
            ViewBag.Priority = new SelectList(items, "Value", "Text", taskManageEdit.Priority);
            List<SelectListItem> items1 = new List<SelectListItem>();
            items1.Add(new SelectListItem { Text = TaskStatus.Todo, Value = TaskStatus.Todo });
            items1.Add(new SelectListItem { Text = TaskStatus.Inproress, Value = TaskStatus.Inproress });
            items1.Add(new SelectListItem { Text = TaskStatus.Done, Value = TaskStatus.Done });
            items1.Add(new SelectListItem { Text = TaskStatus.Removed, Value = TaskStatus.Removed });
            ViewBag.Status = new SelectList(items1, "Value", "Text", taskManageEdit.Status);
            var lastCommentId = db.SP_GetLastCommentId_By_TaskId(id);
            ViewBag.LastCommentId = lastCommentId;
            return View(taskManageEdit);
        }

        // POST: Admin/TaskManages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CreatedDate,CreatedBy,StartDate,EndDate,Description,Rate,Priority,Status,Targets,UnitTargets")] TaskManage taskManage, string UserAssign, HttpPostedFileBase FilesUpload)
        {
            CheckNameMission(taskManage);
            CheckDate(taskManage);
            var currentUserId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                if (FilesUpload != null)
                {
                    // Converting to bytes.  
                    byte[] uploadedFile = new byte[FilesUpload.InputStream.Length];
                    FilesUpload.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                    var fileUpload = new FileUpload();
                    fileUpload.FileName = FilesUpload.FileName;
                    fileUpload.FileType = FilesUpload.ContentType;
                    fileUpload.FileContent = Convert.ToBase64String(uploadedFile);
                    fileUpload.TaskId = taskManage.Id;
                    db.FileUploads.Add(fileUpload);
                }

                taskManage.ModifiedBy = currentUserId;
                taskManage.ModifiedDate = DateTime.Now;
                db.Entry(taskManage).State = EntityState.Modified;
                var userTask = db.UserTasks.Where(s => s.TaskId == taskManage.Id).FirstOrDefault();
                // Create Notifier
                var oldUserAssign = userTask.UserId;
                if (userTask != null)
                {
                    userTask.UserId = UserAssign;
                    db.Entry(userTask).State = EntityState.Modified;
                }

                db.SaveChanges();


                if (oldUserAssign != UserAssign) // change userAssign && diff currentUser
                {
                    // get info to send notifier
                    var changeByInfo = db.Accounts.Where(a => a.UserId == currentUserId).FirstOrDefault();
                    var assignFromInfo = db.Accounts.Where(a => a.UserId == oldUserAssign).FirstOrDefault();
                    var assignToInfo = db.Accounts.Where(a => a.UserId == UserAssign).FirstOrDefault();
                    var notify = new Notify()
                    {
                        TaskId = taskManage.Id,
                        ChangeBy = changeByInfo.UserId,
                        ChangeByFullName = changeByInfo.FullName,
                        AssignFrom = assignFromInfo.UserId,
                        AssignFromFullName = assignFromInfo.FullName,
                        AssignTo = UserAssign,
                        AssignToFullName = assignToInfo.FullName,
                        TaskName = taskManage.Name,
                        IsRead = false,
                        CreatedDate = DateTime.Now
                    };
                    db.Notifies.Add(notify);
                    db.SaveChanges();
                    var query0 = db.UserNotifies.Where(x => x.UserId == assignFromInfo.UserId);
                    //var highestId = query.Any() ? query.Max(x => x.Id) : 0;
                    var query1 = db.UserNotifies.Where(x => x.UserId == assignToInfo.UserId);
                    //var highestId = query.Any() ? query.Max(x => x.Id) : 0;
                    var dataUserNofify = new List<UserNotify>() { 
                        new UserNotify()
                        {
                            NotifyId = notify.Id,
                            UserId = assignFromInfo.UserId,
                            AssignFrom = assignFromInfo.UserId,
                            AssignTo = assignToInfo.UserId,
                            IsRead = false,
                            OrderId = query0.Any() ? query0.Max(x => x.OrderId) + 1  : 1
                        },
                        new UserNotify()
                        {
                            NotifyId = notify.Id,
                            UserId = assignToInfo.UserId,
                            AssignFrom = assignFromInfo.UserId,
                            AssignTo = assignToInfo.UserId,
                            IsRead = false,
                            OrderId = query1.Any() ? query1.Max(x => x.OrderId) + 1  : 1
                        }
                    };
                    db.UserNotifies.AddRange(dataUserNofify);
                    db.SaveChanges();
                }

                SetAlert("Cập nhật thành công !!!", "success");
                return RedirectToAction("Edit", new { id = taskManage.Id});
            }
            var listUser = db.Accounts.Where(a => a.AspNetUser.AspNetRoles.FirstOrDefault().Name != Commons.Role.Owner).ToList();
            var listIdSubtask = db.TaskRelationships.Where(t => t.ParentId == taskManage.Id).Select(s => s.TaskId).ToList();
            var getSubTask = db.TaskManages.Where(t => listIdSubtask.Contains(t.Id)).Select(s => new SubTask() { Id = s.Id, Name = s.Name, Status = s.Status, UpdatedDate = s.ModifiedDate }).ToList();
            ViewBag.SubTasks = getSubTask;
            ViewBag.FullNameCreatedBy = db.Accounts.Where(ac => ac.UserId == taskManage.CreatedBy).FirstOrDefault()?.FullName;
            // Parent current Task
            var getParentTaskId = db.TaskRelationships.Where(t => t.TaskId == taskManage.Id).Select(s => s.ParentId).FirstOrDefault();
            var getParentTask = new SubTask();
            ViewBag.UserAssign = new SelectList(listUser, "UserId", "FullName", db.UserTasks.Where(us => us.TaskId == taskManage.Id).FirstOrDefault().UserId);
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = TaskPriority.High, Value = TaskPriority.High });
            items.Add(new SelectListItem { Text = TaskPriority.Medium, Value = TaskPriority.Medium });
            items.Add(new SelectListItem { Text = TaskPriority.Low, Value = TaskPriority.Low });
            ViewBag.Priority = new SelectList(items, "Value", "Text");
            List<SelectListItem> items1 = new List<SelectListItem>();
            items1.Add(new SelectListItem { Text = TaskStatus.Todo, Value = TaskStatus.Todo });
            items1.Add(new SelectListItem { Text = TaskStatus.Inproress, Value = TaskStatus.Inproress });
            items1.Add(new SelectListItem { Text = TaskStatus.Done, Value = TaskStatus.Done });
            items1.Add(new SelectListItem { Text = TaskStatus.Removed, Value = TaskStatus.Removed });
            ViewBag.Status = new SelectList(items1, "Value", "Text", taskManage.Status);
            return View(taskManage);
        }

        // GET: Admin/TaskManages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskManage taskManage = db.TaskManages.Find(id);
            if (taskManage == null)
            {
                return HttpNotFound();
            }
            ViewBag.FullNameCreatedBy = db.Accounts.Where(ac => ac.UserId == taskManage.CreatedBy).FirstOrDefault()?.FullName;
            return View(taskManage);
        }

        // POST: Admin/TaskManages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var res = db.SP_GetAllChild_Task_By_ParentId(id).Select(s => s.TaskId).ToList();
            if (res != null)
            {
                var userTask = db.UserTasks.Where(task => res.Contains(task.TaskId)).ToList();
                var listTaskRemove = db.TaskManages.Where(w => res.Contains(w.Id)).ToList();
                var listTaskRelationShipRemove = db.TaskRelationships.Where(w => res.Contains(w.TaskId)).ToList();
                if (userTask != null) db.UserTasks.RemoveRange(userTask);
                if (listTaskRemove != null) db.TaskRelationships.RemoveRange(listTaskRelationShipRemove);
                if (listTaskRelationShipRemove != null) db.TaskManages.RemoveRange(listTaskRemove);
            }
            db.SaveChanges();
            SetAlert("Xóa nhiệm vụ thành công !!!", "success");
            return RedirectToAction("Index");
        }
        public ActionResult DeleteFile(int fileId)
        {
            var file = db.FileUploads.Find(fileId);
            int? taskId = file.TaskId;
            if (file != null)
            {
                try
                {
                    db.FileUploads.Remove(file);
                    db.SaveChanges();
                    SetAlert("Xóa file thành công !!!", "success");
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return RedirectToAction("Edit", new { id = taskId });
        }
        public ActionResult DownloadFile(int fileId)
        {
            try
            {
                // Loading dile info.  
                var fileInfo = db.FileUploads.Where(f => f.Id == fileId).FirstOrDefault();
                if(fileInfo != null)
                    return this.GetFile(fileInfo.FileContent, fileInfo.FileType, fileInfo.FileName);
            }
            catch (Exception ex)
            {
                // Info  
                return Json("KO", JsonRequestBehavior.AllowGet);
            }

            // Info.  
            return Json("KO", JsonRequestBehavior.AllowGet);
        }
        private FileResult GetFile(string fileContent, string fileContentType, string fileName)
        {
            // Initialization.  
            FileResult file = null;

            try
            {
                // Get file.  
                byte[] byteContent = Convert.FromBase64String(fileContent);
                file = this.File(byteContent, fileContentType, fileName);
            }
            catch (Exception ex)
            {
                // Info.  
                throw ex;
            }

            // info.  
            return file;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public void CheckDate(TaskManage task)
        {
            if (task.StartDate > task.EndDate)
            {
                ModelState.AddModelError("StartDate", "Ngày tạo không được lớn hơn ngày Kết thúc");
            }
        }
        public void CheckNameMission(TaskManage task)
        {
            if (task.Name == null)
            {
                ModelState.AddModelError("Name", "Không được để trống tên task");
            }         
        }
    }
}
