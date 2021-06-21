using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TaskManagement.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {

            if (User.IsInRole("Owner")) {


                return RedirectToAction("Index", "ManageUsers", new { area = "Owner" });
            }
            else if(User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "TaskManages", new { area = "Admin" }); // đã chỉnh chỗ này
            }
            else if (User.IsInRole("Leader"))
            {
                return RedirectToAction("", "", new { area = "Leader" });
            }
            return RedirectToAction("Login", "Account");
        }
           
        

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}