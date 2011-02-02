using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;

namespace PoyService
{
    public class DataAccess : IDisposable
    {
        public MySqlConnection connection;
        static System.Random randomNumbeGenerator = new System.Random();
        public DataAccess()
        {
            connection = new MySqlConnection("Server=140.254.80.125;Database=PoyService;Uid=poyman;Pwd=poyman$;Pooling=false;");
			//connection.
            connection.Open();
        }

        //public void AddUser(string userName, string password, string Email)
        //{
        //    MySqlCommand command = new MySqlCommand(string.Format(
        //        "insert into User (UserName, UsersPassword,Email) values('{0}','{1}','{2}')"
        //        , userName, password, Email),connection);

        //    command.ExecuteNonQuery();
        //}

        public void AddUser(string name, string email, string organzation, string description)
        {
            MySqlCommand command = new MySqlCommand(string.Format(
                "insert into User (Name, Email,Organization,Description ) values('{0}','{1}','{2}','{3}')"
                , name, email,organzation,description), connection);

            command.ExecuteNonQuery();
        }

        //public string getPassPhrase(string userName)
        //{
        //    MySqlCommand command = new MySqlCommand(string.Format(
        //        "select PassPhrase from User where UserName='{0}';"
        //        , userName), connection);

        //    return command.ExecuteScalar().ToString();
        //}


        public static DataTable getuserData()
        {
            DataAccess access = new DataAccess();

           //MySqlCommand command = new MySqlCommand("select * from User", access.connection);
 		   MySqlCommand command = new MySqlCommand(@"select  User.UserId,Name, Email,Organization, PassPhrase, NumberOfJobs, TotalNodeMinutes from User left join 
           (select UserId, count(*) as NumberOfJobs, Sum(NodeMinutes) as TotalNodeMinutes from Job group by UserId) as a
           on User.UserId = a.UserId", access.connection);
            
            DataTable data = new DataTable();
            data.Load(command.ExecuteReader());
            return data;
        }


        public int getToken(string passPhrase, string ipAddress)
        {
            MySqlCommand command = new MySqlCommand(string.Format(
                "select UserId from User where PassPhrase='{0}';"
                , passPhrase), connection);
            
            object o= command.ExecuteScalar();
			//Console.WriteLine(command.CommandText+"  returns "+ o.ToString());
            if(o==null || string.IsNullOrEmpty(o.ToString())) return -1;

            int dir = randomNumbeGenerator.Next(int.MaxValue);
            
            command = new MySqlCommand(string.Format(
            "insert into Job (JobToken, IPAddress,UserId) values({0},'{1}',{2});"
            ,dir,ipAddress,o.ToString()),connection);
            command.ExecuteScalar();
            return dir;
        }

        public bool ValidateToken(int Token, string ipAddress)
        {
			/*
            int dir = randomNumbeGenerator.Next(int.MaxValue);
            MySqlCommand command = new MySqlCommand(string.Format(
                "select count(*) from Job where IPAddress = '{0}' and JobToken ={1} ;"
                , ipAddress,Token), connection);

            return command.ExecuteScalar().ToString()=="1";
            */
			return true;
        }
		
		public void Inactivate(int jobtoken)
		{
			 MySqlCommand command = new MySqlCommand(string.Format(
                "UPDATE PoyService.Job SET active =0 where JobId = {0};"
                , jobtoken), connection);

            command.ExecuteNonQuery();
			return;
		}

        public void Dispose()
        {
            connection.Close();
        }



        public void updateNodeMinutes(int jobtoken, int nodeMinutes)
        {
             MySqlCommand command = new MySqlCommand(string.Format(
             "update Job set NodeMinutes = NodeMinutes +{0} where JobToken ={1};",nodeMinutes,jobtoken),connection);
            command.ExecuteNonQuery();
          
        }

        public string makePassPrase(string userId)
        {
            
            StringBuilder passPhrase = new StringBuilder();
            for(int i=0;i<50;i++)
            {
                //passPhrase.Append((char)randomNumbeGenerator.Next(48, 122));
                passPhrase.Append((char)randomNumbeGenerator.Next(65, 90));
            }
          
            
            MySqlCommand command = new MySqlCommand(string.Format(
            "update User set PassPhrase = '{0}' where UserId = {1};"
            ,passPhrase.ToString(),userId),connection);

            command.ExecuteNonQuery();
            return passPhrase.ToString();
        }
    }
}