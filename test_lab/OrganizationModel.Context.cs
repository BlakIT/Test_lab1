﻿//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CRM
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class OrganizationsEntities : DbContext
    {
        public OrganizationsEntities()
            : base("name=OrganizationsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Clients> Clients { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Contact_Name> Contact_Name { get; set; }
        public DbSet<Content> Content { get; set; }
        public DbSet<Content_Type> Content_Type { get; set; }
        public DbSet<Employee_To_Content> Employee_To_Content { get; set; }
        public DbSet<Employee_Clients_Content> Employee_Clients_Content { get; set; }
    }
}
