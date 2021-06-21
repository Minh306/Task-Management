using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Areas.Admin.Models;
using TaskManagement.Commons;
using TaskManagement.Models;

namespace TaskManagement.Areas.Admin.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin,Leader,Member")]
    public class TaskAssignToMeController : Controller
    {
        private SEP23Team7Entities db = new SEP23Team7Entities();
        private readonly IMapper _mapper;
        public TaskAssignToMeController()
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
        // GET: Admin/ListAssign
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();
            var taskId = db.UserTasks.Where(t => t.UserId == currentUserId).Select(u => u.TaskId).ToList();
            var taskManage = db.TaskManages.Where(tm => taskId.Contains(tm.Id)).ToList();
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

        // GET: Admin/ListAssign/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/ListAssign/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ListAssign/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/ListAssign/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/ListAssign/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/ListAssign/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/ListAssign/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
