using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using ServerSide.Characters;

namespace ServerSide.Server.DB.CharacterDB
{
    internal class ItemDbManager
    {
        public static bool UpdateCharacterItems(ICharacter character)
        {
            Database db = new Database();
            if (!CharacterDbManager.CharacterExists(character.Name)) // Check existence by unique Id
                return false;

            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE `characters` SET ItemsJSON = @json WHERE Id = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {

                        // Update values, using the unique Id to identify the character
                        string ItemsJSON = JsonConvert.SerializeObject(character.Items);
                        command.Parameters.AddWithValue("@json", ItemsJSON);
                        command.Parameters.AddWithValue("@Id", character.Id);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            NAPI.Util.ConsoleOutput("Character updated successfully.");
                            return true;
                        }
                        else
                        {
                            NAPI.Util.ConsoleOutput("Failed to update character.");
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
