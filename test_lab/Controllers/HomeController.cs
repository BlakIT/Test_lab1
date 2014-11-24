using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //if (Session["CurrentUser"] != null)
            //{
            //    Employees curUser = (Employees)Session["CurrentUser"];

            //    if (curUser.Role == 1)
            //        return RedirectToAction("Index", "Admin");
            //    else if (curUser.Role == 2)
            //        return RedirectToAction("Index", "Manager");
            //}

            return View();
        }

        public ActionResult About()
        {

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult MainPage()
        {
            return View();
        }

    }
}
