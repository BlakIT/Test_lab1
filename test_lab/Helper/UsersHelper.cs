using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace CRM.Helper
{
    public static class UsersHelper
    {
        private static UsersEntities db = new UsersEntities();

        #region Login

        public static List<Users> GetUsersByLogin(string login)
        {
            var item = from user in db.Users
                       where user.Login.Equals(login)
                       select user;

            return item.ToList();
        }

        public static Users GetUserByLoginAndPass(string login, string pass)
        {
            List<Users> users = GetUsersByLogin(login);

            if (users.Count <= 0)
                return null;

            var userr = from user in users
                       where user.Pass.Equals(pass)
                       select user;

            if(userr.ToList().Count <= 0)
                return null;

            return userr.ToList()[0];
        }

        public static Users Registration(Users user)
        {
            if (GetUsersByLogin(user.Login).Count > 0)
                return null;

            RegisterNewUser(user);

            return GetUserByLogin(user.Login);
        }

        public static void SaveCH()
        {
            db.SaveChanges();
        }

        #endregion

        #region User

        public static Users GetUserByLogin(string login)
        {
             return db.Users.SingleOrDefault(x => x.Login == login);
        }

        public static void DeleteUser(Users user)
        {
            db.Users.Remove(user);
            db.SaveChanges();
        }

        public static void AddNewUser(Users user)
        {
            db.Users.Add(user);
            db.SaveChanges();
        }


        #endregion

        #region Register

        

        public static void RegisterNewUser(Users user) 
        {
            string DBpath = @"C:\Temp" + @"\\" + user.Login + ".mdf";

            if (!File.Exists(DBpath))
            {
                int r = db.Database.ExecuteSqlCommand("USE master; CREATE DATABASE DB" + user.Login + " ON (NAME = DB" + user.Login + ", FILENAME = '" + DBpath + "')");
                r = db.Database.ExecuteSqlCommand("USE [DB" + user.Login + "];" + GetTableScript());

                string connectFromWC = ConfigurationManager.ConnectionStrings["ServerName"].ToString();

                DBpath = @"data source=" + connectFromWC +";initial catalog=DB" + user.Login + @";integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";

                user.DB_Path = DBpath;

                AddNewUser(user);                

                Helper.ChangeDBPath(DBpath);

                Helper.AddFirstEmployee(new Employees()
                {
                    Date_Admission = DateTime.Now,
                    FIO = "Enter name",
                    Login = user.Login,
                    Password = user.Pass,
                    Role = 1,
                    Registration_Date = DateTime.Now
                });
            }            
        }
        
        #endregion

        #region Addski Script

        public static string GetTableScript()
        {
            return @"CREATE TABLE [dbo].[Clients](
	                                    [ID_Client] [int] IDENTITY(1,1) NOT NULL,
	                                    [Title] [nvarchar](50) NULL,
	                                    [Registration_Date] [date] NULL,
	                                    [ID_Employee] [int] NULL,
	                                    [Address] [text] NULL,
	                                    [Post_Address] [text] NULL,
	                                    [UNN_UNP] [nvarchar](100) NULL,
	                                    [Kind_Activity] [nvarchar](50) NULL,
	                                    [Rating] [int] NULL,
                                     CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Client] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                                            
                                    CREATE TABLE [dbo].[Contact_Name](
	                                    [ID_Contact_Name] [int] IDENTITY(1,1) NOT NULL,
	                                    [FIO] [varchar](150) NULL,
	                                    [Post] [varchar](50) NULL,
	                                    [Telephone] [varchar](50) NULL,
	                                    [Dinner_Time] [time](7) NULL,
	                                    [ID_Clients] [int] NOT NULL,
	                                    [Type] [int] NOT NULL,
                                     CONSTRAINT [PK_Contact_Name] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Contact_Name] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY]

                                    
                                    CREATE TABLE [dbo].[Content](
	                                    [ID_Content] [int] IDENTITY(1,1) NOT NULL,
	                                    [Text] [text] NULL,
	                                    [ID_Content_Type] [int] NOT NULL,
	                                    [Create_Date] [datetime] NULL,
	                                    [IsRead] [int] NULL,
	                                    [Important] [int] NULL,
                                     CONSTRAINT [PK_Content] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Content] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                                    
                                    CREATE TABLE [dbo].[Content_Type](
	                                    [ID_Content_Type] [int] IDENTITY(1,1) NOT NULL,
	                                    [Type] [int] NOT NULL,
                                     CONSTRAINT [PK_Content_Type] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Content_Type] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY]

                                    
                                    CREATE TABLE [dbo].[Employee_Clients_Content](
	                                    [ID_Content] [int] NOT NULL,
	                                    [ID_Author_Employee] [int] NOT NULL,
	                                    [ID_Addressee_Client] [int] NOT NULL,
                                     CONSTRAINT [PK_Employee_Clients_Content] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Content] ASC,
	                                    [ID_Author_Employee] ASC,
	                                    [ID_Addressee_Client] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY]

                                    
                                    CREATE TABLE [dbo].[Employee_To_Content](
	                                    [ID_Content] [int] NOT NULL,
	                                    [ID_Author_Employee] [int] NOT NULL,
	                                    [ID_Addressee_Employee] [int] NOT NULL,
                                     CONSTRAINT [PK_Employee_To_Content] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Content] ASC,
	                                    [ID_Author_Employee] ASC,
	                                    [ID_Addressee_Employee] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY]

                                    
                                    CREATE TABLE [dbo].[Employees](
	                                    [ID_Employee] [int] IDENTITY(1,1) NOT NULL,
	                                    [Login] [nvarchar](50) NOT NULL,
	                                    [Password] [nvarchar](50) NOT NULL,
	                                    [Role] [int] NOT NULL,
	                                    [FIO] [text] NULL,
	                                    [Post] [nvarchar](50) NULL,
	                                    [Registration_Date] [date] NULL,
	                                    [Date_Entry] [datetime] NULL,
	                                    [Date_Birth] [datetime] NULL,
	                                    [Date_Admission] [datetime] NULL,
	                                    [Address] [text] NULL,
                                     CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Employee] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                ";

        }


        private static string GetScript(string dbName, string filePath)
        {
            return @"USE [master]
                                    GO
                                    CREATE DATABASE [" + dbName + @"]
                                     CONTAINMENT = NONE
                                     ON  PRIMARY 
                                    ( NAME = N'" + dbName + @"', FILENAME = N'" + filePath + @"' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
                                     LOG ON 
                                    ( NAME = N'" + dbName + @"_log', FILENAME = N'" + filePath + @"_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
                                    GO
                                    ALTER DATABASE [" + dbName + @"] SET COMPATIBILITY_LEVEL = 110
                                    GO
                                    IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
                                    begin
                                    EXEC [" + dbName + @"].[dbo].[sp_fulltext_database] @action = 'enable'
                                    end                                    
                                    GO
                                    USE [" + dbName + @"]
                                    GO
                                    /****** Object:  Table [dbo].[Clients]    Script Date: 12.10.2014 3:42:23 ******/
                                    SET ANSI_NULLS ON
                                    GO
                                    SET QUOTED_IDENTIFIER ON
                                    GO
                                    CREATE TABLE [dbo].[Clients](
	                                    [ID_Client] [int] IDENTITY(1,1) NOT NULL,
	                                    [Title] [nvarchar](50) NULL,
	                                    [Registration_Date] [date] NULL,
	                                    [ID_Employee] [int] NULL,
	                                    [Address] [text] NULL,
	                                    [Post_Address] [text] NULL,
	                                    [UNN_UNP] [nvarchar](100) NULL,
	                                    [Kind_Activity] [nvarchar](50) NULL,
	                                    [Rating] [int] NULL,
                                     CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Client] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                                    GO
                                    /****** Object:  Table [dbo].[Contact_Name]    Script Date: 12.10.2014 3:42:24 ******/
                                    SET ANSI_NULLS ON
                                    GO
                                    SET QUOTED_IDENTIFIER ON
                                    GO
                                    SET ANSI_PADDING ON
                                    GO
                                    CREATE TABLE [dbo].[Contact_Name](
	                                    [ID_Contact_Name] [int] IDENTITY(1,1) NOT NULL,
	                                    [FIO] [varchar](150) NULL,
	                                    [Post] [varchar](50) NULL,
	                                    [Telephone] [varchar](50) NULL,
	                                    [Dinner_Time] [time](7) NULL,
	                                    [ID_Clients] [int] NOT NULL,
	                                    [Type] [int] NOT NULL,
                                     CONSTRAINT [PK_Contact_Name] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Contact_Name] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY]

                                    GO
                                    SET ANSI_PADDING OFF
                                    GO
                                    /****** Object:  Table [dbo].[Content]    Script Date: 12.10.2014 3:42:24 ******/
                                    SET ANSI_NULLS ON
                                    GO
                                    SET QUOTED_IDENTIFIER ON
                                    GO
                                    CREATE TABLE [dbo].[Content](
	                                    [ID_Content] [int] IDENTITY(1,1) NOT NULL,
	                                    [Text] [text] NULL,
	                                    [ID_Content_Type] [int] NOT NULL,
	                                    [Create_Date] [datetime] NULL,
	                                    [IsRead] [int] NULL,
	                                    [Important] [int] NULL,
                                     CONSTRAINT [PK_Content] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Content] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                                    GO
                                    /****** Object:  Table [dbo].[Content_Type]    Script Date: 12.10.2014 3:42:24 ******/
                                    SET ANSI_NULLS ON
                                    GO
                                    SET QUOTED_IDENTIFIER ON
                                    GO
                                    CREATE TABLE [dbo].[Content_Type](
	                                    [ID_Content_Type] [int] IDENTITY(1,1) NOT NULL,
	                                    [Type] [int] NOT NULL,
                                     CONSTRAINT [PK_Content_Type] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Content_Type] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY]

                                    GO
                                    /****** Object:  Table [dbo].[Employee_Clients_Content]    Script Date: 12.10.2014 3:42:24 ******/
                                    SET ANSI_NULLS ON
                                    GO
                                    SET QUOTED_IDENTIFIER ON
                                    GO
                                    CREATE TABLE [dbo].[Employee_Clients_Content](
	                                    [ID_Content] [int] NOT NULL,
	                                    [ID_Author_Employee] [int] NOT NULL,
	                                    [ID_Addressee_Client] [int] NOT NULL,
                                     CONSTRAINT [PK_Employee_Clients_Content] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Content] ASC,
	                                    [ID_Author_Employee] ASC,
	                                    [ID_Addressee_Client] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY]

                                    GO
                                    /****** Object:  Table [dbo].[Employee_To_Content]    Script Date: 12.10.2014 3:42:24 ******/
                                    SET ANSI_NULLS ON
                                    GO
                                    SET QUOTED_IDENTIFIER ON
                                    GO
                                    CREATE TABLE [dbo].[Employee_To_Content](
	                                    [ID_Content] [int] NOT NULL,
	                                    [ID_Author_Employee] [int] NOT NULL,
	                                    [ID_Addressee_Employee] [int] NOT NULL,
                                     CONSTRAINT [PK_Employee_To_Content] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Content] ASC,
	                                    [ID_Author_Employee] ASC,
	                                    [ID_Addressee_Employee] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY]

                                    GO
                                    /****** Object:  Table [dbo].[Employees]    Script Date: 12.10.2014 3:42:24 ******/
                                    SET ANSI_NULLS ON
                                    GO
                                    SET QUOTED_IDENTIFIER ON
                                    GO
                                    CREATE TABLE [dbo].[Employees](
	                                    [ID_Employee] [int] IDENTITY(1,1) NOT NULL,
	                                    [Login] [nvarchar](50) NOT NULL,
	                                    [Password] [nvarchar](50) NOT NULL,
	                                    [Role] [int] NOT NULL,
	                                    [FIO] [text] NULL,
	                                    [Post] [nvarchar](50) NULL,
	                                    [Registration_Date] [date] NULL,
	                                    [Date_Entry] [datetime] NULL,
	                                    [Date_Birth] [datetime] NULL,
	                                    [Date_Admission] [datetime] NULL,
	                                    [Address] [text] NULL,
                                     CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
                                    (
	                                    [ID_Employee] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                                    GO
                                    ALTER TABLE [dbo].[Contact_Name] ADD  DEFAULT ((0)) FOR [Type]
                                    GO
                                    ALTER TABLE [dbo].[Content] ADD  CONSTRAINT [DF_Content_IsRead]  DEFAULT ((0)) FOR [IsRead]
                                    GO
                                    USE [master]
                                    GO
                                    ALTER DATABASE [" + dbName + @"] SET  READ_WRITE 
                                    GO
                                    ";
        }

        #endregion

    }
}