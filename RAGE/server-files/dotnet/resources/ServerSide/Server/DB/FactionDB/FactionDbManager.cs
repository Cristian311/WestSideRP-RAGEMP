using GTANetworkAPI;
using MySql.Data.MySqlClient;
using ServerSide.Characters;
using ServerSide.Factions;
using ServerSide.Server.DB.CharacterDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSide.Server.DB.FactionDB
{
    internal class FactionDbManager
    {
        public static bool SaveFaction(Faction faction)
        {
            Database db = new Database();

            using (MySqlConnection connection = db.GetDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = @"
                INSERT INTO `factions` (Id, Name, Type, Characters, RankNames)
                VALUES (@Id, @Name, @Type, @Characters, @RankNames)
                ON DUPLICATE KEY UPDATE
                    Name = @Name,
                    Type = @Type,
                    Characters = @Characters,
                    RankNames = @RankNames";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", faction.Id);
                        command.Parameters.AddWithValue("@Name", faction.Name);
                        command.Parameters.AddWithValue("@Type", faction.Type);

                        // Serialize Characters as comma-separated values
                        string characters = string.Join(",", faction.Characters);
                        command.Parameters.AddWithValue("@Characters", characters);

                        // Serialize RankNames as JSON
                        string rankNamesJson = NAPI.Util.ToJson(faction.RankNames);
                        command.Parameters.AddWithValue("@RankNames", rankNamesJson);

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"An error occurred while saving faction: {ex.Message}");
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static List<Faction> LoadFactions()
        {
            List<Faction> factions = new List<Faction>();
            Database db = new Database();

            using (MySqlConnection connection = db.GetDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Id, Name, Type, Characters, RankNames FROM `factions`";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("Id");
                            string name = reader.GetString("Name");
                            string type = reader.GetString("Type");

                            // Deserialize Characters (split the string and fetch characters)
                            string charactersData = reader.GetString("Characters");
                            var characters = charactersData.Split(',')
                                         .Select(item => item.Trim()) // Remove extra spaces
                                         .Where(item => !string.IsNullOrWhiteSpace(item)) // Remove empty entries
                                         .ToList();

                            // Deserialize RankNames
                            string rankNamesJson = reader.GetString("RankNames");
                            var rankNames = NAPI.Util.FromJson<Dictionary<int, string>>(rankNamesJson)
                                ?? new Dictionary<int, string>(); // Use empty dictionary if null

                            // Create and populate the Faction object
                            Faction faction = new Faction(id, name, type)
                            {
                                Characters = characters,
                                RankNames = rankNames
                            };

                            factions.Add(faction);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"An error occurred while loading factions: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }

            return factions;
        }


        public static bool UpdateFaction(Faction faction)
        {
            Database db = new Database();
            using (MySqlConnection connection = db.GetDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = @"
                UPDATE `factions`
                SET `Name` = @Name, `Type` = @Type, `RankNames` = @RankNames, `Characters` = @Characters
                WHERE `Id` = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", faction.Id);
                        command.Parameters.AddWithValue("@Name", faction.Name);
                        command.Parameters.AddWithValue("@Type", faction.Type);

                        string characters = string.Join(",", faction.Characters);
                        command.Parameters.AddWithValue("@Characters", characters);

                        // Serialize the RankNames dictionary to a JSON string
                        string rankNamesJson = NAPI.Util.ToJson(faction.RankNames);
                        command.Parameters.AddWithValue("@RankNames", rankNamesJson);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            NAPI.Util.ConsoleOutput($"Faction {faction.Name} updated successfully.");
                            return true;
                        }
                        else
                        {
                            NAPI.Util.ConsoleOutput("Failed to update faction.");
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
