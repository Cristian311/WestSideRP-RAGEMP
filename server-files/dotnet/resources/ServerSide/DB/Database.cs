using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ServerSide.Server.DB
{
    public class Database
    {
        private string _connectionString;

        public Database()
        {
            // Load configuration from file
            LoadConfiguration();

            NAPI.Util.ConsoleOutput("Database: Connection initialized!");
        }

        private void LoadConfiguration()
        {
            try
            {
                // Define the config file path
                string configPath = "dbconfig.json"; //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dbconfig.json");

                if (!File.Exists(configPath))
                {
                    throw new FileNotFoundException($"Database configuration file not found at path: {configPath}");
                }

                // Read the JSON content
                string jsonContent = File.ReadAllText(configPath);

                // Deserialize JSON into a strongly-typed object
                var config = JsonConvert.DeserializeObject<DbConfig>(jsonContent);

                if (string.IsNullOrEmpty(config.Server) ||
                    string.IsNullOrEmpty(config.Database) ||
                    string.IsNullOrEmpty(config.UserId))
                {
                    throw new Exception("Configuration file is missing required fields (server, database, userId).");
                }

                // Build the connection string
                _connectionString = $"Server={config.Server};Database={config.Database};User ID={config.UserId};Password={config.Password};";
                NAPI.Util.ConsoleOutput("Database: Configuration loaded successfully!");
            }
            catch (Exception ex)
            {
                NAPI.Util.ConsoleOutput($"Error loading database configuration: {ex.Message}");
                throw; // Rethrow to ensure the application doesn't proceed with an invalid setup
            }
        }

        public MySqlConnection GetDatabaseConnection()
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
                    NAPI.Util.ConsoleOutput("Database connection opened successfully!");

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Log the first column as int and second column as string
                            NAPI.Util.ConsoleOutput($"ID: {reader.GetInt32(0)}, Name: {reader.GetString(1)}");
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    NAPI.Util.ConsoleOutput($"Database error: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                    NAPI.Util.ConsoleOutput("Database connection closed.");
                }
            }
        }
    }

    // Strongly-typed configuration class
    public class DbConfig
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
