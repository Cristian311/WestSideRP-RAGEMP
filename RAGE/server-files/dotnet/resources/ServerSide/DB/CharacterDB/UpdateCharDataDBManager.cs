using GTANetworkAPI;
using MySql.Data.MySqlClient;
using ServerSide.Characters;
using ServerSide.Characters.Clothes;
using ServerSide.Interfaces;


namespace ServerSide.DB.CharacterDB
{
    internal class UpdateCharDataDBManager
    {
        public static bool UpdateCharacter(ICharacter character)
        {
            Database db = new Database();
            if (!CharacterDbManager.CharacterExists(character.Name)) // Check existence by unique Id
                return false;

            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE `characters` SET AccountId = @accountId, Name = @Name, Age = @Age, Money = @Money, isCreated = @isCreated WHERE Id = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Update values, using the unique Id to identify the character
                        command.Parameters.AddWithValue("@accountId", character.Account.Id);
                        command.Parameters.AddWithValue("@Name", character.Name);
                        command.Parameters.AddWithValue("@Age", character.Age);
                        command.Parameters.AddWithValue("@Money", character.Money);
                        command.Parameters.AddWithValue("@Id", character.Id);
                        command.Parameters.AddWithValue("@isCreated", character.isCreated);

                        string debugQuery = $"UPDATE `characters` SET AccountId = {character.Account.Id}, Name = '{character.Name}', Age = {character.Age}, Money = {character.Money} WHERE Id = {character.Id}";
                        NAPI.Util.ConsoleOutput($"Executing Query: {debugQuery}");

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
        public static bool UpdateCharacterFaceData(ICharacter character)
        {
            Database db = new Database();
            if (!CharacterDbManager.CharacterExists(character.Name)) // Check existence by unique Id
                return false;

            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE `characters` SET FaceJSON = @json WHERE Id = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Update values, using the unique Id to identify the character
                        var FaceJSON = NAPI.Util.ToJson(character.Face);
                        command.Parameters.AddWithValue("@json", FaceJSON);
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
        public static bool UpdateCharacterHair(ICharacter character)
        {
            Database db = new Database();
            if (!CharacterDbManager.CharacterExists(character.Name)) // Check existence by unique Id
                return false;

            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE `characters` SET HairJSON = @json WHERE Id = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Update values, using the unique Id to identify the character
                        var HairJSON = NAPI.Util.ToJson(character.Hair);
                        command.Parameters.AddWithValue("@json", HairJSON);
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

        public static bool UpdateCharacterClothes(ICharacter character, ClothesData clothes)
        {
            Database db = new Database();
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO `clothes` (characterID, localID, clothesname, clothesdata) VALUES (@characterID, @localID, @clothesname, @clothesdata)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@characterID", character.Id);
                        command.Parameters.AddWithValue("@localID", clothes.Id);
                        command.Parameters.AddWithValue("@clothesname", character.Name);
                        var clothesJSON = NAPI.Util.ToJson(clothes);
                        command.Parameters.AddWithValue("@clothesdata", clothesJSON);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            NAPI.Util.ConsoleOutput("Character created successfully.");
                            return true;
                        }
                        else
                        {
                            NAPI.Util.ConsoleOutput("Failed to create character.");
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
