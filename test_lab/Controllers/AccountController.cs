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
            return RedirectToAction("Index", "Home"); 
        }          

        
        public ActionResult Login(FormCollection form)
        {
            string  login = form["login"];
            string pass = form["pass"];

            if ( Helper.UsersHelper.CheckLoginAndPass(login, pass) )
            {
                Users user = Helper.UsersHelper.Login(login, pass);
                
                Helper.Helper.ChangeDBPath(user.DB_Path);

                Session["CurrentUser"] = Helper.Helper.GetEmployeeByLogin(user.Login);
               
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Registration(Users model)
        {
            if(model.Pass != model.ConfPass)
                return RedirectToAction("Index", "Home");

            if (Helper.UsersHelper.CheckLoginAndPass(model.Login, model.Pass))
                return RedirectToAction("Index", "Home");

            Helper.UsersHelper.RegisterNewUser(model);

            Session["CurrentUser"] = Helper.Helper.GetEmployeeByLogin(model.Login); //Helper.UsersHelper.Login(model.Login, model.Pass);

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
