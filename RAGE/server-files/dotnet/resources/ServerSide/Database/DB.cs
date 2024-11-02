
using MySql.Data.MySqlClient;
using GTANetworkAPI; // Ensure you have this namespace for NAPI

public class DatabaseHelper
{
    private string _connectionString;
    private const string server = "localhost";
    private const string database = "rage-db";
    private const string userId = "root";
    private string password = string.Empty;
    public DatabaseHelper()
    {
        _connectionString = $"Server={server};Database={database};User ID={userId};Password={password};";
        NAPI.Util.ConsoleOutput("Database: Connection successful!");
    }

    public void RegisterAccount(string username, string password)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
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
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            string query = "SELECT * FROM `accounts` WHERE Name = @username AND Password = @password";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            try
            {
                conn.Open();
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
                conn.Close();
            }
        }
    }
    public bool checkAccountRegister(string username)
    {
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            if (username == string.Empty)
                return false;
            string query = "SELECT * FROM `accounts` WHERE Name = @username";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            try
            {
                conn.Open();
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
                conn.Close();
            }
        }
    }

    public void ExecuteQuery(string query)
    {
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            try
            {
                conn.Open();
                NAPI.Util.ConsoleOutput("Connection successful!");

                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Assuming the first column is an int and the second is a string
                        NAPI.Util.ConsoleOutput($"ID: {reader.GetInt32(0)}, Name: {reader.GetString(1)}");
                    }
                }
            }
            catch (MySqlException ex)
            {
                NAPI.Util.ConsoleOutput($"An error occurred: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
