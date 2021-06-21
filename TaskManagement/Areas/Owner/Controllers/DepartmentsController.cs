using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Areas.Owner.Controllers
{
    [Authorize]
    [Authorize(Roles = "Owner")]
    public class DepartmentsController : Controller
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

        // GET: Owner/Departments
        public ActionResult Index()
        {
            var res = db.Departments.ToList();
            foreach (var item in res)
            {
                var fullName = db.Accounts.Where(u => u.UserId == item.UserMainAssign)?.FirstOrDefault()?.FullName;
                item.UserMainAssignDisplay = fullName;
            }
            return View(res);
        }

        // GET: Owner/Departments/Details/5
        public ActionResult Details(string id)
        {
            //var res = db.Departments.ToList();
            //foreach(var item in res)
            //{
            //    var nameAdmin = db.Accounts.Where(na => na.UserId == item.UserMainAssign)?.FirstOrDefault()?.FullName;
            //    //item.UserMainAssignDisplay == nameAdmin;
            //}

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            department.UserMainAssignDisplay = db.Accounts.Where(na => na.UserId == department.UserMainAssign)?.FirstOrDefault()?.FullName;
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Owner/Departments/Create
        public ActionResult Create()
        {
            var listUserAssign = db.Departments.Select(x => x.UserMainAssign).ToList();
            var listRoleAdmin = db.AspNetUsers.Where(u => u.AspNetRoles.FirstOrDefault().Name == Commons.Role.Admin && !listUserAssign.Contains(u.Accounts.FirstOrDefault().UserId)).Select(us => new SelectListItem() { Text = us.Accounts.FirstOrDefault().FullName, Value = us.Accounts.FirstOrDefault().UserId }).ToList();
            ViewBag.ListRoleAdmin = listRoleAdmin;
            return View();
        }

        // POST: Owner/Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Introduction,UserMainAssign")] Department department)
        {
            CheckNameDepartment(department);
            if (ModelState.IsValid)
            {
                department.Id = Guid.NewGuid().ToString();
                try
                {
                    db.Departments.Add(department);
                    db.SaveChanges();
                    SetAlert("Thêm bộ phận thành công !!!", "success");
                    return RedirectToAction("Index","Departments");
                }
                catch (Exception ex)
                {
                    // Code here
                }
            }
            var listUserAssign = db.Departments.Select(x => x.UserMainAssign).ToList();
            var listRoleAdmin = db.AspNetUsers.Where(u => u.AspNetRoles.FirstOrDefault().Name == Commons.Role.Admin && !listUserAssign.Contains(u.Accounts.FirstOrDefault().UserId)).Select(us => new SelectListItem() { Text = us.Accounts.FirstOrDefault().FullName, Value = us.Accounts.FirstOrDefault().UserId }).ToList();
            ViewBag.ListRoleAdmin = listRoleAdmin;
            return View(department);
        }

        // GET: Owner/Departments/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var listRoleAdmin = db.AspNetUsers.Where(u => u.AspNetRoles.FirstOrDefault().Name == Commons.Role.Admin).Select(us => new SelectListItem() { Text = us.Accounts.FirstOrDefault().FullName, Value = us.Accounts.FirstOrDefault().UserId }).ToList();
            //ViewBag.ListRoleAdmin = listRoleAdmin;
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            var listUserAssign = db.Departments.Select(x => x.UserMainAssign).ToList();
            var listRoleAdmin = db.AspNetUsers.Where(u => u.AspNetRoles.FirstOrDefault().Name == Commons.Role.Admin && !listUserAssign.Contains(u.Accounts.FirstOrDefault().UserId)).Select(us => new SelectListItem() { Text = us.Accounts.FirstOrDefault().FullName, Value = us.Accounts.FirstOrDefault().UserId }).ToList();
            ViewBag.Assgin = department.UserMainAssign;
            ViewBag.ListRoleAdmin = listRoleAdmin;
            return View(department);
        }

        // POST: Owner/Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Introduction,UserMainAssign")] Department department, string userCh)
        {
            CheckNameDepartment(department);
            if (ModelState.IsValid)
            {
                try
                {
                    if (String.IsNullOrEmpty(department.UserMainAssign))
                    {
                        department.UserMainAssign = userCh;
                    }
                    else
                    {
                        department.UserMainAssign = department.UserMainAssign;
                    }
                    db.Entry(department).State = EntityState.Modified;
                    db.SaveChanges();
                    SetAlert("Cập nhật thông tin phòng ban thành công !!!", "success");
                    return RedirectToAction("Index", "Departments");
                }
                catch (Exception ex)
                {
                   
                }

            }
            var listUserAssign = db.Departments.Select(x => x.UserMainAssign).ToList();
            var listRoleAdmin = db.AspNetUsers.Where(u => u.AspNetRoles.FirstOrDefault().Name == Commons.Role.Admin && !listUserAssign.Contains(u.Accounts.FirstOrDefault().UserId)).Select(us => new SelectListItem() { Text = us.Accounts.FirstOrDefault().FullName, Value = us.Accounts.FirstOrDefault().UserId }).ToList();
            ViewBag.ListRoleAdmin = listRoleAdmin;
            return View(department);
        }

        // GET: Owner/Departments/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            department.UserMainAssignDisplay = db.Accounts.Where(na => na.UserId == department.UserMainAssign)?.FirstOrDefault()?.FullName;
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Owner/Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Department department = db.Departments.Find(id);
            try
            {
              
                var usersOfDepartment = db.UsersOfDepartments.Where(ud => ud.DepartmentId == id).ToList();
                db.UsersOfDepartments.RemoveRange(usersOfDepartment);
                db.Departments.Remove(department);
                db.SaveChanges();
                SetAlert("xóa bộ phận thành công !!!", "success");
                return RedirectToAction("Index", "Departments");
            }
            catch (Exception ex)
            {
                // Code here
            }
            return View(department);
           
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public void CheckNameDepartment(Department department)
        {
            if (department.Name == null)
            {
                ModelState.AddModelError("Name", "Không được để trống tên bộ phận");
            }
        }
    }
}
