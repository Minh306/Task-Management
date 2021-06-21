using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Areas.Owner.Controllers
{
    [Authorize]
    [Authorize(Roles = "Owner")]
    public class ManageUsersController : Controller
    {
        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            if(type == "success")
            {
                TempData["AlertType"] = "alert-success";
            }else if(type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
            }else if(type == "error")
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
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageUsersController()
        {
        }

        public ManageUsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Owner/ManageUsers
        public ActionResult Index()
        {           
            return View(db.Accounts.Where(s => s.AspNetUser.AspNetRoles.FirstOrDefault().Name != Commons.Role.Owner).ToList());
        }

        // GET: Owner/ManageUsers/Details/5
        public ActionResult Details(int id)
        {
            
            if (id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account model = db.Accounts.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            model.DepartmentDisplay = db.Departments.Where(d => d.UserMainAssign == model.UserId)?.FirstOrDefault()?.Name;
            if (model.DepartmentDisplay == null)
            {
                model.DepartmentDisplay = db.UsersOfDepartments.Where(ud => ud.UserId == model.UserId)?.FirstOrDefault()?.Department?.Name;
            }
            return View(model);
        }

        // GET: Owner/ManageUsers/Create
        public ActionResult Create(string department)
        {
            
            //Lấy giá trị role từ Role đã chọn ở View trong Dropdownlist
            //ViewBag.ListRoles = new SelectList(db.AspNetRoles, "Name", "Name");
            //Lấy giá trị role từ Deparment đã chọn ở View trong Dropdownlist
            ViewBag.ListDepartments = new SelectList(db.Departments, "Name", "Name");
            //Tìm đúng role và deparment đã chọn
            //ViewBag.Roles = db.AspNetRoles.Find(role);
            ViewBag.Departments = db.Departments.Find(department);
            return View();
        }

        // POST: Owner/ManageUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Account model, string department)
        {
            
            checkUserName(model);
            checkEmail(model);
            if (ModelState.IsValid)
            {        
                var user = new ApplicationUser {UserName = model.Email, Email = model.Email };               
                var result = UserManager.Create(user, "vlu12345");
                if (result.Succeeded)
                {
                    //Add User vào Role
                    var getRole = UserManager.AddToRole(user.Id, "Member");
                    db.SaveChanges();
                    var id = db.AspNetUsers.FirstOrDefault(x => x.Id == user.Id);
                    Account acc = new Account
                    {
                        UserId = user.Id,
                        FullName = model.FullName,
                        Email = user.Email,
                        Password = model.Password = "vlu12345",
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        Gender = true,
                    };                    
                    try
                    {
                        db.Accounts.Add(acc);                       
                        db.SaveChanges();
                        SetAlert("Thêm tài khoản thành công", "success");
                        return RedirectToAction("Index", "ManageUsers");
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống");
                }              
            }
            return View(model);
        }

        // GET: Owner/ManageUsers/Edit/5
        public ActionResult Edit(int id, string role, string department)
        {           
            if (id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account model = db.Accounts.Find(id);
            var selectedRole = db.AspNetUsers.Find(model.UserId)?.AspNetRoles?.FirstOrDefault()?.Name;
            //Lấy giá trị role từ Role đã chọn ở View trong Dropdownlist
            ViewBag.ListRoles = new SelectList(db.AspNetRoles.Where(lr => lr.Name != Commons.Role.Owner).ToList(), "Name", "Name", selectedRole);
            ViewBag.ListDepartments = new SelectList(db.Departments, "Name", "Name");
            //Tìm đúng role, department đã chọn
            ViewBag.Roles = db.AspNetRoles.Find(role);
            ViewBag.Departments = db.Departments.Find(department);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Owner/ManageUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Account model, string role, string department)
        {
            if (ModelState.IsValid)
            {
                Account acc = db.Accounts.FirstOrDefault(a => a.Id == model.Id);
                var oldRole = acc.AspNetUser.AspNetRoles.SingleOrDefault().Name;
                if (oldRole != role)
                {
                    UserManager.RemoveFromRole(acc.UserId, oldRole);
                    UserManager.AddToRole(acc.UserId, role);   
                }
                db.SaveChanges();
                SetAlert("chỉnh sửa thông tin tài khoản thành công", "success");
                return RedirectToAction("Index", "ManageUsers");
            }
            return View(model);
        }

        // GET: Owner/ManageUsers/Delete/5
        public ActionResult Delete(int id)
        {
            Session["success"] = null;
            if (id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account model = db.Accounts.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Owner/ManageUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account model = db.Accounts.FirstOrDefault(x=>x.Id == id); 
            //var getUserId = db.AspNetUsers.FirstOrDefault(x => x.Id == model.UserId);
        
            var userId = db.AspNetUsers.FirstOrDefault(u => u.Id == model.UserId);
            try
            {
                db.Accounts.Remove(model);
                db.AspNetUsers.Remove(userId);
                db.SaveChanges();
                SetAlert("Xóa tài khoản thành công !!!", "success");
                return RedirectToAction("Index", "ManageUsers");
              
            }
            catch(Exception ex)
            {
              
            }
            return View(model);
            
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public void checkUserName(Account model)
        {
            if (model.FullName == null)
            {
                ModelState.AddModelError("FullName", "Không được để trống tên người dùng");
            }
        }
        public void checkEmail (Account model)
        {
            if (model.Email == null)
            {
                ModelState.AddModelError("Email", "Không được để trống Email");
            }         
        }
    }
}
