using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class DialogsInfo
    {

        public Employee_Clients_Content employee_client_content { get; set; }
        public Content content { get; set; }


        public string GetEmployeeName()
        {
            return Helper.Helper.GetEmployeeByID(this.employee_client_content.ID_Author_Employee).FIO;
        }
    }
}