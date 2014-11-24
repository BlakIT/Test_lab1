using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Controllers
{
    public class ManagerController : Controller
    {
        //
        // GET: /Manager/

        public ActionResult Index()
        {
            TempData["OrganizationList"] = Helper.Helper.GetAllClientsInfoByManagerID(((CRM.Employees)Session["CurrentUser"]).ID_Employee);
            return View("~/Views/Admin/Index.cshtml");
        }

        public ActionResult LoadPartial(int index)
        {

            switch (index)
            {
                case 1:     //list all Organizations case
                    return PartialView("~/Views/PartialViews/_OrganizationPartial.cshtml", Helper.Helper.GetAllClientsInfoByManagerID(((CRM.Employees)Session["CurrentUser"]).ID_Employee ));

                case 2:     //list all Missions case
                    return PartialView("~/Views/PartialViews/_MailPartial.cshtml", Helper.Helper.GetMissionMessage( (CRM.Employees)Session["CurrentUser"]) );


                //list all input mails case
                case 3:
                    List<CRM.Models.EmployeesMail> inputList = Helper.Helper.GetInputMessage( (CRM.Employees)Session["CurrentUser"] );
                    return PartialView("~/Views/PartialViews/_MailPartial.cshtml", inputList);

                //list all output mails case
                case 4:
                    List<CRM.Models.EmployeesMail> outputList = Helper.Helper.GetOutputMessage( (CRM.Employees)Session["CurrentUser"]);
                    return PartialView("~/Views/PartialViews/_MailPartial.cshtml", outputList);

                //create new mail
                case 5:
                    ViewBag.Adressee = Helper.Helper.GetAllEmployeesName();
                    return PartialView("~/Views/PartialViews/_NewMailPartial.cshtml");
                    



                //case 3:     //list all input mails case
                //    return PartialView("~/Views/PartialViews/_MailPartial.cshtml", Helper.Helper.GetInputMessage((CRM.Employees)Session["CurrentUser"]));

                //case 4:     //list all output mails case
                //    List<CRM.Models.EmployeesMail> outputList = Helper.Helper.GetOutputMessage((CRM.Employees)Session["CurrentUser"]);
                //    return PartialView("~/Views/PartialViews/_MailPartial.cshtml", outputList);

                //case 5:     //list all  mails case
                //    List<CRM.Models.EmployeesMail> mails = Helper.Helper.GetOutputMessage((CRM.Employees)Session["CurrentUser"]);
                //    return PartialView("_MailPartial", mails);

                default:
                    return PartialView("_OrganizationPartial", Helper.Helper.GetAllClientsInfo());
            }
        }

    }
}
