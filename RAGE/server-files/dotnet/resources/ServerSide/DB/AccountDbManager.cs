using GTANetworkAPI;
using MySql.Data.MySqlClient;

namespace ServerSide.DB
{
    internal class AccountDbManager
    {
        public bool RegisterAccount(string username, string password)
        {
            if (checkAccountLogin(username, password, out int? _))
                return false;
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
                        return true;
                    }
                    else
                    {
                        NAPI.Util.ConsoleOutput("Failed to create account.");
                        return false;
                    }
                }
            }
        }

        public bool checkAccountLogin(string username, string password, out int? Id)
        {
            Id = null;
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
                            reader.Read();
                            Id = reader.GetInt32(0);
                            // User found
                            NAPI.Util.ConsoleOutput("User authenticated successfully. ID: " + Id);
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
        public bool checkAccountAdminLevel(string username, out int? adminLevel)
        {
            adminLevel = null;

            if (string.IsNullOrEmpty(username))
                return false;

            Database db = new Database();
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
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
                            reader.Read();
                            adminLevel = reader.GetInt32(reader.GetOrdinal("adminLevel")); // Read adminLevel
                            return true; // User exists, admin level retrieved
                        }
                        else
                        {
                            NAPI.Util.ConsoleOutput("No user found with the given username.");
                            return false; // No user found
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    NAPI.Util.ConsoleOutput($"An error occurred: {ex.Message}");
                    return false; // Database error
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
