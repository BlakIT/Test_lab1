using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using CRM.Models;

namespace CRM.Controllers
{
    public class AccountController : Controller
    {

        public ActionResult Index()
        {
            if (Session["CurrentUser"] != null)
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home"); 
        }          

        
        public ActionResult Login(FormCollection form)
        {
            string login = form["login"];
            string pass = form["pass"];

            Users user = Helper.UsersHelper.GetUserByLoginAndPass(login, pass);

            if ( user != null)
            {                
                Helper.Helper.ChangeDBPath(user.DB_Path);

                Employees em = Helper.Helper.GetEmployeeByLogin(user.Login);

                Session["CurrentUser"] = em;
               
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Registration(Users model)
        {
            Users user = Helper.UsersHelper.Registration(model);

            if (user == null)
                return RedirectToAction("Index", "Home");

            Session["CurrentUser"] = Helper.Helper.GetEmployeeByLogin(user.Login);

            return RedirectToAction("Index", "Admin");
        }

        
        public ActionResult LogOut()
        {
            Session["CurrentUser"] = null;
            return RedirectToAction("Index");
        }
        

        #region Вспомогательные методы

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }        

        #endregion
    }
}
