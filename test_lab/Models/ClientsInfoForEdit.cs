using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class ClientsInfoForEdit
    {
        public Clients client;

        public List<Contact_Name> contacts;

        public Employees employee;

        public ClientsInfoForEdit() { }

        public ClientsInfoForEdit(int id)
        {
            this.client = Helper.Helper.GetClientByID(id);
            this.contacts = Helper.Helper.GetAllContactNameByID(id);
            this.employee = Helper.Helper.GetEmployeeByID(this.client.ID_Employee.Value);
        }


    }
}