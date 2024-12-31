using GTANetworkAPI;
using MySql.Data.MySqlClient;
using ServerSide.Server.Blips;
using System;
using System.Collections.Generic;

namespace ServerSide.Server.DB
{
    internal class BlipDbManager
    {
        public static bool CreateBlip(BlipData blip)
        {
            Database db = new Database();
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO `blips` (Name, PositionX, PositionY, PositionZ, Scale, Color, Sprite) VALUES (@Name, @PositionX, @PositionY, @PositionZ, @Scale, @Color, @Sprite)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", blip.Name);
                        command.Parameters.AddWithValue("@PositionX", blip.Position.X);
                        command.Parameters.AddWithValue("@PositionY", blip.Position.Y);
                        command.Parameters.AddWithValue("@PositionZ", blip.Position.Z);
                        command.Parameters.AddWithValue("@Scale", blip.Scale);
                        command.Parameters.AddWithValue("@Color", blip.Color);
                        command.Parameters.AddWithValue("@Sprite", blip.Sprite);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            NAPI.Util.ConsoleOutput("Blip created successfully.");
                            return true;
                        }
                        else
                        {
                            NAPI.Util.ConsoleOutput("Failed to create blip.");
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

        public static List<BlipData> GetBlips()
        {
            Database db = new Database();
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                List<BlipData> blips = new List<BlipData>();
                string query = "SELECT * FROM `blips`";

                MySqlCommand cmd = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                BlipData blip = new BlipData(
                                    reader.GetString("Name"),
                                    new Vector3(
                                        reader.GetFloat("PositionX"),
                                        reader.GetFloat("PositionY"),
                                        reader.GetFloat("PositionZ")
                                    ),
                                    reader.GetFloat("Scale"),
                                    byte.Parse(reader.GetInt32("Color").ToString()),
                                    uint.Parse(reader.GetInt32("Sprite").ToString())
                                )
                                {
                                    Id = reader.GetInt32("Id")
                                };
                                blips.Add(blip);
                            }
                        }
                        else
                        {
                            NAPI.Util.ConsoleOutput("No blips found in the database.");
                        }
                    }
                    return blips;
                }
                catch (MySqlException ex)
                {
                    NAPI.Util.ConsoleOutput($"An error occurred: {ex.Message}");
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
