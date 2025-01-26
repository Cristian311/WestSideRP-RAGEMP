using GTANetworkAPI;
using MySql.Data.MySqlClient;
using ServerSide.Characters;
using ServerSide.Characters.Customization.Clothes;
using System;


namespace ServerSide.Server.DB.CharacterDB
{
    internal class UpdateCharDataDBManager : Script
    {
        [ServerEvent(Event.PlayerDisconnected)]
        private void SaveCharacterInfo(Player player, DisconnectionType disconnectionType, string reason)
        {
            if (CharacterManager.PlayerCharacterPair.ContainsKey(player) == false)
                return;
            var character = CharacterManager.PlayerCharacterPair[player];
            UpdateCharacter(character);
        }
        public static bool UpdateCharacter(Character character)
        {
            Database db = new Database();
            //if (!CharacterDbManager.CharacterExists(character.Name)) // Check existence by unique Id
            //    return false;

            using (MySqlConnection connection = db.GetDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = @"
                        UPDATE `characters` 
                        SET 
                            AccountId = @accountId, 
                            Name = @Name, 
                            faction = @faction, 
                            factionRank = @factionRank, 
                            Age = @Age, 
                            Money = @Money, 
                            isCreated = @isCreated, 
                            currentOutfit = @currentOutfit 
                            currentOutfit = @currentOutfit 
                            currentOutfit = @currentOutfit 
                        WHERE 
                            Id = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Update values, using the unique Id to identify the character
                        command.Parameters.AddWithValue("@accountId", character.Account.Id);
                        command.Parameters.AddWithValue("@Name", character.Name);
                        command.Parameters.AddWithValue("@Age", character.Age);
                        command.Parameters.AddWithValue("@Money", character.Money);
                        command.Parameters.AddWithValue("@Id", character.Id);
                        command.Parameters.AddWithValue("@isCreated", character.isCreated);
                        command.Parameters.AddWithValue("@currentOutfit", character.CurrentOutfit);
                        command.Parameters.AddWithValue("@faction", character.faction);
                        command.Parameters.AddWithValue("@factionRank", character.factionRank);
                        command.Parameters.AddWithValue("@CharacterDescription", character.CharacterDescription);
                        command.Parameters.AddWithValue("@TattooDescription", character.TattooDescription);

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

            using (MySqlConnection connection = db.GetDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = @"
                        UPDATE `characters`
                        SET
                            FaceJSON = @json
                        WHERE
                            Id = @Id";

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
        public static bool UpdateCharacterTattoos(Character character)
        {
            Database db = new Database();
            if (!CharacterDbManager.CharacterExists(character.Name)) // Check existence by unique Id
                return false;

            using (MySqlConnection connection = db.GetDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = @"
                        UPDATE `characters` 
                        SET 
                            TattooJSON = @json, 
                            HighlightsJSON = @json2 
                        WHERE 
                            Id = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Update values, using the unique Id to identify the character
                        var TattoosJson = NAPI.Util.ToJson(character.Tattoos);
                        var HighlightJson = NAPI.Util.ToJson(character.TattooNames);
                        command.Parameters.AddWithValue("@json", TattoosJson);
                        command.Parameters.AddWithValue("@json2", HighlightJson);
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
        public static bool UpdateCharacterPhone(Character character, int currentphoneId)
        {
            Database db = new Database();
            if (!CharacterDbManager.CharacterExists(character.Name)) // Check existence by unique Id
                return false;

            using (MySqlConnection connection = db.GetDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = @"
                        UPDATE `characters` 
                        SET 
                            currentphone = @currentphone 
                        WHERE 
                            Id = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Update values, using the unique Id to identify the character
                        command.Parameters.AddWithValue("@currentphone", currentphoneId);
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

            using (MySqlConnection connection = db.GetDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = @"
                        UPDATE `characters` 
                        SET 
                            HairJSON = @json 
                        WHERE 
                            Id = @Id";

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
            using (MySqlConnection connection = db.GetDatabaseConnection())
            {
                try
                {
                    connection.Open();

                    // First, check if the localID for the given characterID exists.
                    string checkQuery = @"
                        SELECT COUNT(*) 
                        FROM `clothes` 
                        WHERE 
                            `characterID` = @characterID 
                            AND `localID` = @localID";

                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@characterID", character.Id);
                        checkCommand.Parameters.AddWithValue("@localID", clothes.Id);

                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        // If the record exists, update it.
                        if (count > 0)
                        {
                            string updateQuery = @"
                                UPDATE `clothes` 
                                SET 
                                    `clothesname` = @clothesname, 
                                    `clothesdata` = @clothesdata 
                                WHERE 
                                    `characterID` = @characterID 
                                    AND `localID` = @localID";
                            using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@characterID", character.Id);
                                updateCommand.Parameters.AddWithValue("@localID", clothes.Id);
                                updateCommand.Parameters.AddWithValue("@clothesname", character.Name);
                                var clothesJSON = NAPI.Util.ToJson(clothes);
                                updateCommand.Parameters.AddWithValue("@clothesdata", clothesJSON);

                                int result = updateCommand.ExecuteNonQuery();

                                if (result > 0)
                                {
                                    NAPI.Util.ConsoleOutput("Character clothes updated successfully.");
                                    return true;
                                }
                                else
                                {
                                    NAPI.Util.ConsoleOutput("Failed to update character clothes.");
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            // If the record does not exist, insert it.
                            string insertQuery = "INSERT INTO `clothes` (characterID, localID, clothesname, clothesdata) VALUES (@characterID, @localID, @clothesname, @clothesdata)";

                            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@characterID", character.Id);
                                insertCommand.Parameters.AddWithValue("@localID", clothes.Id);
                                insertCommand.Parameters.AddWithValue("@clothesname", character.Name);
                                var clothesJSON = NAPI.Util.ToJson(clothes);
                                insertCommand.Parameters.AddWithValue("@clothesdata", clothesJSON);

                                int result = insertCommand.ExecuteNonQuery();

                                if (result > 0)
                                {
                                    NAPI.Util.ConsoleOutput("Character clothes inserted successfully.");
                                    return true;
                                }
                                else
                                {
                                    NAPI.Util.ConsoleOutput("Failed to insert character clothes.");
                                    return false;
                                }
                            }
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
