using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class ClientsInfo : Clients
    {
        public int ID { get; set; }

        public int ID_Employee { get; set; }

        public string Title { get; set; }

        public string Address { get; set; }

        public string FIO { get; set; }

        public string Post { get; set; }

        public string Telephone { get; set; }

        public Nullable<System.TimeSpan> Dinner_Time { get; set; }

        //public DateTime? LastCall { get; set; }

        public string getSUperNumber()
        {
            return ID.ToString() + " - " + Title + " - " + FIO;
        }

        public List<Contact_Name> GetAllContact()
        {
            return Helper.Helper.GetAllContactNameByID(this.ID);
        }

    }
}