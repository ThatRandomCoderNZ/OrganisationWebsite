using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace OrganisationWebsite.Database_Scripts
{
    public class DatabaseRepository
    {
        MySqlConnection connection;
        MySqlCommand command;


        public void Test()
        {
            string[] columns = new string[] { "student_id", "name", "DOB" };
            Dictionary<string, List<string>> extractedData = new Dictionary<string, List<string>>() {
                { "student_id", new List<string>()},
                { "name", new List<string>()},
                {"DOB", new List<string>()}
            };
            connection = new MySqlConnection("server=nimbus.rangitoto.school.nz; database=student93061; port=3307; Uid=93061; pwd=93061");
            command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM student";
            connection.Open();
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                foreach (string column in columns)
                {
                    extractedData[column].Add(reader[column].ToString());
                }
            }
        }
}