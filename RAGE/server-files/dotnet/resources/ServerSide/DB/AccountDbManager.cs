using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.DB
{
    internal class AccountDbManager
    {
        public void RegisterAccount(string username, string password)
        {
            Database db = new Database();
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                connection.Open();
                string query = "INSERT INTO `accounts` (Name, Password) VALUES (@username, @password)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        NAPI.Util.ConsoleOutput("Account created successfully.");
                    }
                    else
                    {
                        NAPI.Util.ConsoleOutput("Failed to create account.");
                    }
                }
            }
        }

        public bool checkAccountLogin(string username, string password)
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
        public bool checkAccountRegister(string username)
        {
            Database db = new Database();
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                if (username == string.Empty)
                    return false;
                string query = "SELECT * FROM `accounts` WHERE Name = @username";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@username", username);
                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
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
