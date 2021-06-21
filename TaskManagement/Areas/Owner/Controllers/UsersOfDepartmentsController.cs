using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Areas.Owner.Controllers
{
    [Authorize]
    [Authorize(Roles = "Owner")]
    public class UsersOfDepartmentsController : Controller
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
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserID"] != null)
                base.OnActionExecuting(filterContext);
            else
                filterContext.Result = new RedirectResult("~/Account/Login");
        }
        private SEP23Team7Entities db = new SEP23Team7Entities();

        // GET: Owner/UsersOfDepartments
        public ActionResult Index()
        {
            var usersOfDepartments = db.UsersOfDepartments.Include(u => u.AspNetUser).Include(u => u.Department);
            //ViewBag.Message = TempData["Message"];
            return View(usersOfDepartments.ToList());
        }

        // GET: Owner/UsersOfDepartments/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersOfDepartment usersOfDepartment = db.UsersOfDepartments.Find(id);
            if (usersOfDepartment == null)
            {
                return HttpNotFound();
            }
            return View(usersOfDepartment);
        }

        // GET: Owner/UsersOfDepartments/Create
        public ActionResult Create()
        {
            //var checkrole = db.Accounts.Where(s => s.AspNetUser.AspNetRoles.FirstOrDefault().Name != Commons.Role.Owner).ToList();
            ViewBag.UserId = new SelectList(db.AspNetUsers.Where(asu => asu.AspNetRoles.FirstOrDefault().Name != Commons.Role.Owner && asu.AspNetRoles.FirstOrDefault().Name != Commons.Role.Admin), "Id", "UserName");
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name");
            return View();
        }

        // POST: Owner/UsersOfDepartments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,DepartmentId,DateGetIn")] UsersOfDepartment usersOfDepartment)
        {
            if (ModelState.IsValid)
            {
               
                var checkIsExist = db.UsersOfDepartments.Where(ufd => ufd.UserId == usersOfDepartment.UserId && ufd.DepartmentId == usersOfDepartment.DepartmentId).Any();
                if (checkIsExist)
                {
                    //SetAlert("Tài khoản này đã có trong bộ phận này", "warning");
                    ModelState.AddModelError("UserId", "Tài khoản này đã có trong bộ phận này");
                }
                else
                {
                    try
                    {
                        db.UsersOfDepartments.Add(usersOfDepartment);
                        db.SaveChanges();
                        SetAlert("Đã thêm tài khoản vào phòng ban", "success");                     
                    }
                    catch(Exception ex)
                    {
                        //SetAlert("Tài khoản này đã có một bộ phận khác", "error");
                        ModelState.AddModelError("UserId", "Tài khoản này đã có một bộ phận khác");
                    }
                }
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers.Where(asu => asu.AspNetRoles.FirstOrDefault().Name != Commons.Role.Owner && asu.AspNetRoles.FirstOrDefault().Name != Commons.Role.Admin), "Id", "UserName");
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name");
            return View(usersOfDepartment);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetUserByDepartment(string departmentId)
        {
            Department department = db.Departments.Find(departmentId); // Department
            var userOfDepartment = db.UsersOfDepartments.Where(ud => ud.DepartmentId == departmentId).Select(s => s.UserId);
            var listUser = db.Accounts.Where(user => user.UserId != department.UserMainAssign && !userOfDepartment.Contains(user.UserId)).Select(ss=> new { UserId = ss.UserId, FullName = ss.FullName }).ToArray();
            return Json(listUser, JsonRequestBehavior.AllowGet);
        }

        // GET: Owner/UsersOfDepartments/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersOfDepartment usersOfDepartment = db.UsersOfDepartments.Find(id);
            if (usersOfDepartment == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "UserName", usersOfDepartment.UserId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", usersOfDepartment.DepartmentId);
            return View(usersOfDepartment);
        }

        // POST: Owner/UsersOfDepartments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,DepartmentId,DateGetIn")] UsersOfDepartment usersOfDepartment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(usersOfDepartment).State = EntityState.Modified;
                    db.SaveChanges();
                    SetAlert("Cập nhật thông tin thành công", "success");
                    return RedirectToAction("Index", "UsersOfDepartments");
                }
                catch(Exception ex)
                {
                    //code here
                }
                
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "UserName", usersOfDepartment.UserId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", usersOfDepartment.DepartmentId);
            return View(usersOfDepartment);
        }

        // GET: Owner/UsersOfDepartments/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersOfDepartment usersOfDepartment = db.UsersOfDepartments.Find(id);
            if (usersOfDepartment == null)
            {
                return HttpNotFound();
            }
           
            return View(usersOfDepartment);
        }

        // POST: Owner/UsersOfDepartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {

            UsersOfDepartment usersOfDepartment = db.UsersOfDepartments.Find(id);

            try
            {
                db.UsersOfDepartments.Remove(usersOfDepartment);
                db.SaveChanges();
                SetAlert("Xóa tài khoản ra khỏi bộ phận thành công", "success");
                return RedirectToAction("Index", "UsersOfDepartments");
            }
            catch(Exception ex)
            {
                // Code here
            }
          
            return View(usersOfDepartment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
