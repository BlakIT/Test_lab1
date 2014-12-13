using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRM.Models;

namespace CRM.Controllers
{
    public class AdminController : Controller
    {
        private List<CRM.Employees> allEmployeesList = Helper.Helper.GetAllEmployees();

        #region Index

        public ActionResult Index()
        {
            TempData["OrganizationList"] = GetCorrectAllClientsCollection();

            return View();               
        }

        /// <summary>
        /// Get correct clients collection dependent admin or manager
        /// </summary>
        private List<ClientsInfo> GetCorrectAllClientsCollection()
        {
            if (((CRM.Employees)Session["CurrentUser"]).Role == 1)
            {
                return Helper.Helper.GetAllClientsInfo();
            }
            else if (((CRM.Employees)Session["CurrentUser"]).Role == 2)
            {
                return Helper.Helper.GetAllClientsInfoByManagerID(((CRM.Employees)Session["CurrentUser"]).ID_Employee);
            }

            return new List<ClientsInfo>();
        }

        #endregion

        #region Partials

        public ActionResult LoadClientsInfoPartial(int id)
        {
            return PartialView("~/Views/PartialViews/_OrganizationInfoPartial.cshtml", new ClientsInfoForEdit(id));
        }

        public ActionResult LoadEmployeesInfoPartial(int id)
        {
            return PartialView("~/Views/PartialViews/_EmployeesInfoPartial.cshtml", Helper.Helper.GetEmployeeByID(id));
        }

        public ActionResult LoadDialogsInfoPartial(int id)
        {
            return PartialView("~/Views/PartialViews/_DialogsPopUpPartial.cshtml", Helper.Helper.GetDialogsInfoByID(id));
        }

        /// <summary>
        /// Load some partial view
        /// </summary>
        /// <param name="index">index of user choice</param>
        /// <returns></returns>
        public ActionResult LoadPartial(int index)
        {
            switch (index)
            {
                //list all Organizations case
                case 1:
                    return PartialView("~/Views/PartialViews/_OrganizationPartial.cshtml", GetCorrectAllClientsCollection());                   

                //list all Employees case
                case 2:     
                    return PartialView("~/Views/PartialViews/_EmployeesPartial.cshtml", allEmployeesList);
                
                //list all input mails case
                case 3:     
                    List<CRM.Models.EmployeesMail> inputList = Helper.Helper.GetInputMessage( (CRM.Employees)Session["CurrentUser"]);
                    return PartialView("~/Views/PartialViews/_MailPartial.cshtml", inputList);

                //list all output mails case
                case 4:
                    List<CRM.Models.EmployeesMail> outputList = Helper.Helper.GetOutputMessage((CRM.Employees)Session["CurrentUser"]);
                    return PartialView("~/Views/PartialViews/_MailPartial.cshtml", outputList);

                //create new mail
                case 5:     
                    ViewBag.Adressee = Helper.Helper.GetAllEmployeesName();
                    return PartialView("~/Views/PartialViews/_NewMailPartial.cshtml");

                //view Calendar
                case 6:
                    //ViewBag.Adressee = Helper.Helper.GetAllEmployeesName();
                    return PartialView("~/Views/PartialViews/_CalendarPartial.cshtml");
                    
                default:
                    return PartialView("_OrganizationPartial", Helper.Helper.GetAllClientsInfo());
            }
        }

        #endregion

        #region Mails

        /// <summary>
        /// return Mail info
        /// </summary>
        /// <param name="index">Mail index in mailCollection </param>
        /// <param name="type">type of mail   1 - input, 2 - output</param>
        /// <returns></returns>
        public ActionResult LoadMail(int index, int type)
        {
            List<CRM.Models.EmployeesMail> mailList = new List<Models.EmployeesMail>();

            if (type == 3)
                mailList = Helper.Helper.GetInputMessage((CRM.Employees)Session["CurrentUser"]);
            else if (type == 4 || type == 0)
                mailList = Helper.Helper.GetOutputMessage((CRM.Employees)Session["CurrentUser"]);


            return PartialView("~/Views/PartialViews/_OneMailPartial.cshtml", mailList[index]);
        }

        public ActionResult SendNewMail(FormCollection form)
        {
            string message = form.GetValue("Message").AttemptedValue;
            string adresse = form.GetValue("Adressee").AttemptedValue;
            string type = "";

            if(form.GetValue("Type") == null)
                type = "4";
            else
                type = form.GetValue("Type").AttemptedValue;

            Helper.Helper.SendMail(message, type, adresse, ((CRM.Employees)Session["CurrentUser"]).ID_Employee );

            TempData["InputMails"] = Helper.Helper.GetOutputMessage((CRM.Employees)Session["CurrentUser"]);

            return View("Index");
        }

        #endregion

        #region Dialogs

        [HttpPost]
        public void SaveDialog( string text, int index)
        {
            Helper.Helper.SaveDialog(text, GetCorrectAllClientsCollection()[index].ID, ((CRM.Employees)Session["CurrentUser"]).ID_Employee);
        }

        #endregion

        #region Reminders

        [HttpPost]
        public void SaveReminder(string text, string date, string time)
        {
            DateTime saveDate = DateTime.Parse(date + " " + time);

            Helper.Helper.SaveReminder(text, saveDate, ((CRM.Employees)Session["CurrentUser"]).ID_Employee);
        }

        [HttpPost]
        public JsonResult GetReminder()
        {
            if (Session["CurrentUser"] != null)
                return Json( Helper.Helper.GetReminders(((CRM.Employees)Session["CurrentUser"]).ID_Employee, DateTime.Now) );

            return Json(null);
        }

        #endregion

        #region Clients

        [HttpPost]
        public JsonResult CheckClientName(string name)
        {
            return Json(Helper.Helper.CheckClientNameExist(name));
        }

        [HttpPost]
        public JsonResult CheckClientUNN(string unn)
        {
            return Json(Helper.Helper.CheckClientUNNExist(unn));
        }

        [HttpPost]
        public ActionResult SaveNewClient(Clients newClient)
        {
            if (!Helper.Helper.CheckClientNameExist(newClient.Title))
            {
                newClient.ID_Employee = ((CRM.Employees)Session["CurrentUser"]).ID_Employee;
                Helper.Helper.AddNewClient(newClient);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteClient(int id)
        {
            Helper.Helper.DeleteClient(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// АААДДски метод...  надо рефакторить! Проверка формы
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditClient(FormCollection form)
        {
            int id = Int32.Parse(form["client.ID_Client"].ToString());

            ClientsInfoForEdit clientEdit;

            if (form.AllKeys.Contains("Save"))
            {
                clientEdit = new ClientsInfoForEdit(id);

                clientEdit.client.ID_Employee = Int32.Parse(form["client.ID_Employee"]);
                clientEdit.client.Title = form["client.Title"];
                clientEdit.client.Address = form["client.Address"];
                clientEdit.client.Kind_Activity = form["client.Kind_Activity"];
                clientEdit.client.Post_Address = form["client.Post_Address"];
                clientEdit.client.Registration_Date = DateTime.Parse(form["client.Registration_Date"]);
                clientEdit.client.UNN_UNP = form["client.UNN_UNP"];

                Helper.Helper.SqveCH();

                int countCnID = form.AllKeys.Where(x => x.Contains("cnid")).ToList().Count;

                Contact_Name cn;

                for (int i = 1; i <= countCnID; i++)
                {
                    cn = Helper.Helper.GetContactNameByID(Int32.Parse(form["cnid" + i.ToString()]));

                    if ((form["name" + i.ToString()] == "" && form["tel" + i.ToString()] == "" && form["dinner" + i.ToString()] == "" && form["post" + i.ToString()] == "")
                        || (form["name" + i.ToString()] == null && form["tel" + i.ToString()] == null && form["dinner" + i.ToString()] == null && form["post" + i.ToString()] == null)
                        )
                    {
                        Helper.Helper.DeleteContactName(cn);
                        continue;
                    }

                    cn.FIO = form["name" + i.ToString()];
                    cn.Telephone = form["tel" + i.ToString()];
                    cn.Dinner_Time = TimeSpan.Parse(form["dinner" + i.ToString()]);
                    cn.Post = form["post" + i.ToString()];

                    Helper.Helper.SqveCH();
                }

                int countNewCN = form.AllKeys.Where(x => x.Contains("addName")).ToList().Count;

                for (int i = 1; i <= countNewCN; i++)
                {

                    if (form["addDinner" + i.ToString()] == "" && form["addName" + i.ToString()] == "" && form["addTel" + i.ToString()] == "" && form["addPost" + i.ToString()] == "")
                        continue;

                    string str = form["addDinner" + i.ToString()];
                    TimeSpan ts = new TimeSpan();
                    TimeSpan.TryParse(str, out ts);

                    Helper.Helper.AddNewContactName(new Contact_Name()
                    {
                        FIO = form["addName" + i.ToString()],
                        Telephone = form["addTel" + i.ToString()],
                        Dinner_Time = ts,
                        Post = form["addPost" + i.ToString()],
                        ID_Clients = clientEdit.client.ID_Client
                    });

                    Helper.Helper.SqveCH();
                }

            }
            else if (form.AllKeys.Contains("Delete"))
            {
                Helper.Helper.DeleteClient(id);
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Employee

        [HttpPost]
        public JsonResult CheckEmployeeLogin(string login)
        {
            return Json(Helper.UsersHelper.GetUsersByLogin(login).Count > 0);
        }

        [HttpPost]
        public ActionResult SaveNewEmployee(Employees newEmpl)
        {
            Helper.Helper.AddNewEmployee(newEmpl);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditEmployee(Employees employee, FormCollection form)
        {
            Employees empl = Helper.Helper.GetEmployeeByID(employee.ID_Employee);

            if (form.AllKeys.Contains("Save"))
            {
                if (empl.Login != employee.Login && Helper.UsersHelper.GetUsersByLogin(employee.Login).Count > 0)
                    return RedirectToAction("Index");

                Users user = Helper.UsersHelper.GetUserByLogin(empl.Login);

                empl.Address = employee.Address;
                empl.Date_Admission = employee.Date_Admission;
                empl.Date_Birth = employee.Date_Birth;
                empl.Date_Entry = employee.Date_Entry;
                empl.FIO = employee.FIO;
                empl.Login = employee.Login;
                empl.Password = employee.Password;
                empl.Post = employee.Post;

                user.Login = employee.Login;
                user.Pass = employee.Password;
            }
            else if (form.AllKeys.Contains("Delete"))
            {
                Helper.Helper.DeleteEmployee(employee.ID_Employee);
            }

            Helper.Helper.SqveCH();
            Helper.UsersHelper.SaveCH();

            return RedirectToAction("Index");
        }

        #endregion  
        
    }
}
