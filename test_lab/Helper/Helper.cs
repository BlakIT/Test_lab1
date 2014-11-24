using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Helper
{
    public static class Helper
    {

        private static OrganizationsEntities db = new OrganizationsEntities();

        public static void ChangeDBPath(string con)
        {
            db.Database.Connection.ConnectionString = con;
        }

        #region Login

        //public static bool CheckLogin(string name, string pass)
        //{
        //    return db.Employees.Where(x => x.Login == name && x.Password == pass);
        //}

        public static Employees Login(string name, string pass)
        {
            return db.Employees.SingleOrDefault(x => x.Login == name && x.Password == pass);
        }

        #endregion

        #region Organizations

        public static List<Clients> GetAllOrganizations()
        {
            return db.Clients.ToList();
        }

        public static List<CRM.Models.ClientsInfo> GetAllClientsInfo()
        {
            List<CRM.Models.ClientsInfo> list = new List<Models.ClientsInfo>();

            foreach (Clients client in db.Clients)
            {
                Contact_Name cn = db.Contact_Name.FirstOrDefault(x => x.ID_Clients == client.ID_Client);
                
                    list.Add(new CRM.Models.ClientsInfo() {
                        ID = client.ID_Client,
                        ID_Employee = client.ID_Employee.Value,
                        Title = client.Title,
                        Address = client.Address,
                        FIO = cn == null ? "-" : cn.FIO,
                        Post = cn == null ? "-" : cn.Post,
                        Telephone = cn == null ? "-" : cn.Telephone,
                        Dinner_Time = cn == null ? TimeSpan.Zero : cn.Dinner_Time
                    });

            }

            return list;
            
            // var items = from client in db.Clients
            //             join contact in db.Contact_Name on client.ID_Client equals contact.ID_Clients
            //               into gj from subCont in gj.DefaultIfEmpty()  
            //               //join EmClCo in db.Employee_Clients_Content on client.ID_Client equals EmClCo.ID_Addressee_Client
            //               //join content in db.Content on EmClCo.ID_Content equals content.ID_Content
            //               select new CRM.Models.ClientsInfo
            //               {
            //                   ID = client.ID_Client,
            //                   ID_Employee = client.ID_Employee.Value,
            //                   Title = client.Title,
            //                   Address = client.Address,
            //                   FIO = subCont == null ? "-" : subCont.FIO,
            //                   Post = subCont == null ? "-" : subCont.Post,
            //                   Telephone = subCont == null ? "-" : subCont.Telephone,
            //                   Dinner_Time = subCont == null ? TimeSpan.Zero : subCont.Dinner_Time//,
            //                   //LastCall = content.Create_Date == null ? DateTime.MinValue : (DateTime)content.Create_Date
            //               };

            //List<CRM.Models.ClientsInfo> list = items.ToList();
        }

        #endregion

        #region Employees

        public static Employees GetEmployeeByID(int id)
        {
            return db.Employees.SingleOrDefault(x => x.ID_Employee == id);
        }

        public static Employees GetEmployeeByLogin(string login)
        {
            return db.Employees.SingleOrDefault(x => x.Login == login);
        }

        public static List<Employees> GetAllEmployees()
        {
            return db.Employees.ToList();
        }

        public static List<CRM.Models.DialogsInfo> GetDialogsInfoByID(int id)
        {
            var items = from Cont in db.Content
                        join ClCt in db.Employee_Clients_Content on Cont.ID_Content equals ClCt.ID_Content
                        where ClCt.ID_Addressee_Client == id
                        select new CRM.Models.DialogsInfo
                        {
                            content = Cont,
                            employee_client_content = ClCt
                        };

            return items.ToList();
        }

        public static int GetEmployeeIDByLogin(string login)
        {
            return db.Employees.SingleOrDefault(x => x.Login == login).ID_Employee; 
        }

        public static void AddNewEmployee(Employees newEmployee)
        {
            db.Employees.Add(newEmployee);
            db.SaveChanges();

            if (!UsersHelper.CheckLogin(newEmployee.Login))
                UsersHelper.AddNewUser(new Users() { 
                    Login = newEmployee.Login,
                    Pass = newEmployee.Password,
                    DB_Path = db.Database.Connection.ConnectionString
                });
        }

        public static void AddFirstEmployee(Employees newEmployee)
        {
            db.Employees.Add(newEmployee);
            db.SaveChanges();
        }

        public static void DeleteEmployee(int id)
        {
            Employees empl = db.Employees.Single(x => x.ID_Employee == id);

            db.Employees.Remove(empl);
            db.SaveChanges();

            UsersHelper.DeleteUser(UsersHelper.GetUserByLogin(empl.Login));
        }

        #endregion

        #region Mail

        public static List<CRM.Models.EmployeesMail> GetInputMessage(CRM.Employees employee)
        {
            var mails = from EtC in db.Employee_To_Content
                        join content in db.Content on EtC.ID_Content equals content.ID_Content
                        join author in db.Employees on EtC.ID_Author_Employee equals author.ID_Employee
                        join contType in db.Content_Type on content.ID_Content_Type equals contType.ID_Content_Type
                        where EtC.ID_Addressee_Employee == employee.ID_Employee && contType.Type == 4
                        select new CRM.Models.EmployeesMail
                        {
                            AuthorName = author.FIO,
                            Create_Date = content.Create_Date,
                            ID_Content  = content.ID_Content,
                            ID_Content_Type = contType.Type,
                            Important = content.Important,
                            IsRead = content.IsRead,
                            Text = content.Text
                        };

            return mails.ToList();
        }

        public static List<CRM.Models.EmployeesMail> GetOutputMessage(CRM.Employees employee)
        {
            var mails = from EtC in db.Employee_To_Content
                        join content in db.Content on EtC.ID_Content equals content.ID_Content
                        join addressee in db.Employees on EtC.ID_Addressee_Employee equals addressee.ID_Employee
                        join contType in db.Content_Type on content.ID_Content_Type equals contType.ID_Content_Type
                        where EtC.ID_Author_Employee == employee.ID_Employee && contType.Type == 4
                        select new CRM.Models.EmployeesMail
                        {
                            AuthorName = addressee.FIO,
                            Create_Date = content.Create_Date,
                            ID_Content  = content.ID_Content,
                            ID_Content_Type = contType.Type,
                            Important = content.Important,
                            IsRead = content.IsRead,
                            Text = content.Text
                        };

            return mails.ToList();
        }

        public static List<CRM.Models.EmployeesMail> GetMissionMessage(CRM.Employees employee)
        {
            var mails = from EtC in db.Employee_To_Content
                        join content in db.Content on EtC.ID_Content equals content.ID_Content
                        join author in db.Employees on EtC.ID_Author_Employee equals author.ID_Employee
                        join contType in db.Content_Type on content.ID_Content_Type equals contType.ID_Content_Type
                        where EtC.ID_Addressee_Employee == employee.ID_Employee && contType.Type == 3
                        select new CRM.Models.EmployeesMail
                        {
                            AuthorName = author.FIO,
                            Create_Date = content.Create_Date,
                            ID_Content = content.ID_Content,
                            ID_Content_Type = contType.Type,
                            Important = content.Important,
                            IsRead = content.IsRead,
                            Text = content.Text
                        };

            return mails.ToList();
        }

        public static List<string> GetAllEmployeesName()
        {
            var names = from empl in db.Employees
                        select empl.Login;

            return names.ToList();
        }

        /// <summary>
        /// type : 1 - диалог с клиентом, 4 - письмо, 5 - напоминание
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="adresse"></param>
        /// <param name="idAuthor"></param>
        public static void SendMail(string message, string type, string adresse, int idAuthor)
        {
            Content cont = new Content() { 
                Create_Date = DateTime.Now,
                ID_Content_Type = Int32.Parse(type),
                Important = 0,
                IsRead = 0,
                Text = message
            };

            db.Content.Add(cont);
            db.SaveChanges();

            int contentId = cont.ID_Content;
            int adresseId = GetEmployeeIDByLogin(adresse);

            Employee_To_Content ETC = new Employee_To_Content() { 
                ID_Addressee_Employee = adresseId,
                ID_Author_Employee = idAuthor,
                ID_Content = contentId
            };

            db.Employee_To_Content.Add(ETC);
            db.SaveChanges();

        }

        public static void SaveDialog(string message,  int adresseId, int idAuthor)
        {
            Content cont = new Content()
            {
                Create_Date = DateTime.Now,
                ID_Content_Type = 1,
                Important = 0,
                IsRead = 0,
                Text = message
            };

            db.Content.Add(cont);
            db.SaveChanges();

            int contentId = cont.ID_Content;

            Employee_Clients_Content ETC = new Employee_Clients_Content()
            {
                ID_Addressee_Client = adresseId,
                ID_Author_Employee = idAuthor,
                ID_Content = contentId
            };

            db.Employee_Clients_Content.Add(ETC);
            db.SaveChanges();

        }

        /// <summary>
        /// РАзобраться  с БД! с контент-тайпом и контентом...  переделать!
        /// </summary>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <param name="id"></param>
        public static void SaveReminder(string message, DateTime date, int id)
        {
            Content cont = new Content()
            {
                Create_Date = date,
                ID_Content_Type = 5,
                Important = 0,
                IsRead = 0,
                Text = message
            };

            db.Content.Add(cont);
            db.SaveChanges();

            int contentId = cont.ID_Content;

            Employee_To_Content ETC = new Employee_To_Content()
            {
                ID_Addressee_Employee = id,
                ID_Author_Employee = id,
                ID_Content = contentId
            };

            db.Employee_To_Content.Add(ETC);
            db.SaveChanges();

        }

        public static Content GetReminders(int id, DateTime date)
        {
            DateTime moreDate = date.AddSeconds(-10);

            List<Employee_To_Content> emplToCont = db.Employee_To_Content.Where(x => (x.ID_Addressee_Employee == id) && (x.ID_Author_Employee == id) ).ToList();

            var items = from etc in emplToCont
                        join cont in db.Content on etc.ID_Content equals cont.ID_Content
                        where cont.ID_Content_Type == 5 && cont.Create_Date <= date && cont.Create_Date >= moreDate
                        select cont;

            List<Content> list = items.ToList();
            return list.Count > 0 ? list[list.Count - 1] : null;
        }

        #endregion

        #region Clients

        public static Clients GetClientByID(int id)
        {
            return db.Clients.SingleOrDefault(x => x.ID_Client == id);
        }

        public static int GetClientsIDbyName(string adresse)
        {
            return db.Clients.SingleOrDefault(x => x.Title == adresse).ID_Client;
        }

        public static void AddNewClient(Clients newClient)
        {
            db.Clients.Add(newClient);
            db.SaveChanges();
        }

        public static void DeleteClient(int id)
        {
            db.Clients.Remove(db.Clients.SingleOrDefault(x => x.ID_Client == id));

            foreach (Contact_Name cn in db.Contact_Name.Where(x => x.ID_Clients == id))
                db.Contact_Name.Remove(cn);

            db.SaveChanges();
        }

        #endregion

        #region Contact_Name

        public static List<Contact_Name> GetAllContactNameByID(int id)
        {
            return db.Contact_Name.Where(x => x.ID_Clients == id).ToList();
        }

        public static Contact_Name GetContactNameByID(int id)
        {
            return db.Contact_Name.SingleOrDefault(x => x.ID_Contact_Name == id);
        }

        public static void AddNewContactName(Contact_Name cn)
        {
            db.Contact_Name.Add(cn);
            db.SaveChanges();
        }

        public static void DeleteContactName(Contact_Name cn)
        {
            db.Contact_Name.Remove(cn);
            db.SaveChanges();
        }

        #endregion

        #region Manager

        public static List<CRM.Models.ClientsInfo> GetAllClientsInfoByManagerID(int id)
        {
            var items = from client in db.Clients
                        join contact in db.Contact_Name on client.ID_Client equals contact.ID_Clients
                        into gj
                        from subCont in gj.DefaultIfEmpty()
                        where client.ID_Employee == id
                        select new CRM.Models.ClientsInfo
                        {
                            ID = client.ID_Client,
                            ID_Employee = client.ID_Employee.Value,
                            Title = client.Title,
                            Address = client.Address,
                            FIO = subCont == null ? "-" : subCont.FIO,
                            Post = subCont == null ? "-" : subCont.Post,
                            Telephone = subCont == null ? "-" : subCont.Telephone,
                            Dinner_Time = subCont == null ? TimeSpan.Zero : subCont.Dinner_Time
                        };

            return items.ToList();

        }

        #endregion

        public static void SqveCH()
        {
            db.SaveChanges();
        }

    }
}