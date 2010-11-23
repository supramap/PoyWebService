using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using System.Web.SessionState;

namespace PoyService
{
    public class PoyServiceUser
    {
        public string UserId;
        public string UserName;
        public string PassPhrase;
        public string Email;
        public string Date;
        public string Admin;

        private PoyServiceUser() { }

        public PoyServiceUser(string userId,DataAccess access)
        {
            MySqlCommand command = new MySqlCommand("Select * from User where UserId ="+userId, access.connection);
            MySqlDataReader reader =  command.ExecuteReader();
            reader.Read();
            UserId = userId;
            UserName = reader["Name"].ToString();
            Email = reader["Email"].ToString();
            Date = reader["UpdateTimeStamp"].ToString();
            PassPhrase = reader["PassPhrase"].ToString();
        }

        //public static PoyServiceUser getUAuthenticatedUser(HttpSessionState Session, DataAccess access)
        //{
        //    if (Session["password"] == null || Session["userName"]==null) return null;
        //   string password =  Session["password"].ToString();
        //   string userName =  Session["userName"].ToString(); 

        //    MySqlCommand command = new MySqlCommand(string.Format("Select * from User where UserName = '{0}' and UsersPassword ='{1}'", userName,password), access.connection);
        //    MySqlDataReader reader = command.ExecuteReader();
        //    if (!reader.Read()) return null;

        //    return new PoyServiceUser
        //    {
        //        UserId = reader["UserId"].ToString(),
        //        UserName = reader["UserName"].ToString(),
        //        Email = reader["Email"].ToString(),
        //        Date = reader["UpdateTimeStamp"].ToString(),
        //        PassPhrase = reader["PassPhrase"].ToString(),
        //        Admin = reader["Admin"].ToString(),
        //    };
        //}
    }
}