using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace OrganisationWebsite.DatabaseRepository
{
    public class DBConnection
    {
        MySqlConnection connection;
        MySqlCommand command;


        public Dictionary<string, List<string>> RetrieveFromDatabase(string[] columns, string query, List<Tuple<string, string>> parameters = null)
        {
            try {
                Dictionary<string, List<string>> extractedData = new Dictionary<string, List<string>>();
                foreach (string column in columns) {
                    extractedData[column] = new List<string>();
                }
                connection = new MySqlConnection("server=nimbus.rangitoto.school.nz; database=student93061; port=3307; Uid=93061; pwd=93061");
                command = connection.CreateCommand();
                command.CommandText = query;
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        command.Parameters.AddWithValue(parameters[i].Item1, parameters[i].Item2);
                    }
                }
                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    foreach (string column in columns)
                    {
                        extractedData[column].Add(reader[column].ToString());
                    }
                }
                connection.Close();
                return extractedData;

            }
            catch(Exception error)
            {
                Debug.WriteLine(error.Message);
                Dictionary<string, List<string>> extractedData = new Dictionary<string, List<string>>();
                return extractedData;
            }
        }

		public void InsertIntoDatabase(string statement, List<Tuple<string, string>> parameters = null) {
            try {
                connection = new MySqlConnection("server=nimbus.rangitoto.school.nz; database=student93061; port=3307; Uid=93061; pwd=93061");
                command = connection.CreateCommand();
                command.CommandText = statement;
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        command.Parameters.AddWithValue(parameters[i].Item1, parameters[i].Item2);
                    }
                }
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch(Exception error) {
                Debug.WriteLine(error.Message);
            }
		}


    }
}