using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace Symbioz.D2OView
{
    public class SqlConnection
    {
        string m_connectionString { get; set; }
        MySqlConnection m_connection { get; set; }
        public SqlConnection(string host, string dbname, string dbuser, string dbpass)
        {
            this.m_connectionString = "SERVER=" + host + ";" + "DATABASE=" + dbname + ";" + "UID=" + dbuser + ";" + "PASSWORD=" + dbpass + ";";
            this.m_connection = new MySqlConnection(m_connectionString);
        }
        public bool Execute(string query)
        {
            if (m_connection.State == ConnectionState.Open)
            {
                MySqlCommand cmd = new MySqlCommand(query, m_connection);
                try
                {
                  
                    cmd.ExecuteNonQuery();
                    
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.ReadKey();
                    return false;
                }

            }
            else
            {
                Console.WriteLine("Unable to execute query " + query + " SqlConnection cannot be oppened");
                return false;
            }
        }
        public bool Open()
        {
            try
            {
                this.m_connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
