
using MySql.Data.MySqlClient;
using GTANetworkAPI;
namespace ServerSide.Server.DB
{
    public class Database
    {
        private string _connectionString;
        private const string server = "localhost";
        private const string database = "rage-db";
        private const string userId = "root";
        private string password = string.Empty;
        public Database()
        {
            _connectionString = $"Server={server};Database={database};User ID={userId};Password={password};";
            NAPI.Util.ConsoleOutput("Database: Connection successful!");
        }
        public MySqlConnection getDatabaseConnection()
        {
            return new MySqlConnection(_connectionString);
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
}