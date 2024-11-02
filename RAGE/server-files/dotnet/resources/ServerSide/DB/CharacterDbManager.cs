using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.DB
{
    internal class CharacterDbManager
    {
        public bool checkCharacter(string username)
        {
            if (username == string.Empty || password == string.Empty)
                return false;
            Database db = new Database();
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                string query = "SELECT * FROM `accounts` WHERE Name = @username AND Password = @password";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            // User found
                            NAPI.Util.ConsoleOutput("User authenticated successfully.");
                            return true;
                        }
                        else
                        {
                            // No user found
                            NAPI.Util.ConsoleOutput("Authentication failed: No user found with the given credentials.");
                            return false;
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    NAPI.Util.ConsoleOutput($"An error occurred: {ex.Message}");
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
} 
   