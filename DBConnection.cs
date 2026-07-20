using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Scheduling_Application
{
    internal class DBConnection
    {
        public static MySqlConnection conn {  get; set; }
        public static void StartConnection()
        {
            string connectionString = "server=localhost;database=client_schedule;user id=sqlUser;password=Passw0rd!;";
            conn = new MySqlConnection(connectionString);
            conn.Open();
        }

        public static void CloseConnection()
        {
            conn.Close();
        }
    }
}
