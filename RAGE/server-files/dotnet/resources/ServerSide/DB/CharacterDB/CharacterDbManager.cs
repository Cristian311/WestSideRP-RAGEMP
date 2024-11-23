using GTANetworkAPI;
using MySql.Data.MySqlClient;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.Characters.Clothes;
using ServerSide.Hair;
using ServerSide.Interfaces;
using ServerSide.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.DB.CharacterDB
{
    internal class CharacterDbManager
    {
        public static bool CreateCharacter(ICharacter character)
        {
            Database db = new Database();
            if (CharacterExists(character.Name))
                return false;
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO `characters` (AccountId, Name, Age, Money, gender, faceJSON, HairJSON) VALUES (@accountId, @Name, @Age, @Money, @gender, @FaceJSON, @HairJSON)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@accountId", character.Account.Id);
                        command.Parameters.AddWithValue("@Name", character.Name);
                        command.Parameters.AddWithValue("@Age", character.Age);
                        command.Parameters.AddWithValue("@Money", character.Money);
                        command.Parameters.AddWithValue("@gender", character.Gender);
                        command.Parameters.AddWithValue("@FaceJSON", NAPI.Util.ToJson(character.Face));
                        command.Parameters.AddWithValue("@HairJSON", NAPI.Util.ToJson(character.Hair));

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            return true;
                            NAPI.Util.ConsoleOutput("Character created successfully.");
                        }
                        else
                        {
                            return false;
                            NAPI.Util.ConsoleOutput("Failed to create character.");
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
        public static bool CharacterExists(string name)
        {
            if (name == string.Empty)
                return false;
            Database db = new Database();
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                string query = "SELECT * FROM `characters` WHERE Name = @Name";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Name", name);
                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                        else
                        {
                            // No character found
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
        public static Character getCharacter(IAccount account, string name)
        {
            if (name == string.Empty)
                return null;
            Database db = new Database();
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                string query = "SELECT * FROM `characters` WHERE Name = @name";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", name);
                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            Character character = new Character(account.player, account, reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4));
                            character.Id = reader.GetInt32(0);
                            character.isCreated = reader.GetBoolean(6);
                            character.Gender = reader.GetBoolean(9);
                            if (character.isCreated == true)
                            {
                                var FaceJSON = reader.GetString(7);
                                character.Face = NAPI.Util.FromJson<CharacterData>(FaceJSON);
                                var HairJSON = reader.GetString(8);
                                character.Hair = NAPI.Util.FromJson<HairData>(HairJSON);
                                var ItemsJSON = reader.GetString(10);
                                character.Items = NAPI.Util.FromJson<List<Item>>(ItemsJSON);
                                foreach(var item in character.Items)
                                {
                                    character.ItemIdManager.SetIdBusy(item.localID);
                                }    
                            }
                            return character;
                        }
                        else
                        {
                            // No character found
                            account.player.SendChatMessage("No characters found on this account, create one!");
                            return null;
                        }
                    }
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
        public static Character getCharacterById(IAccount account, int id)
        {
            Database db = new Database();
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                string query = "SELECT * FROM `characters` WHERE id = @id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", id);
                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            Character character = new Character(account.player, account, reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4));
                            character.Id = reader.GetInt32(0);
                            character.isCreated = reader.GetBoolean(6);
                            character.Gender = reader.GetBoolean(9);
                            if (character.isCreated == true)
                            {
                                var FaceJSON = reader.GetString(7);
                                character.Face = NAPI.Util.FromJson<CharacterData>(FaceJSON);
                                var HairJSON = reader.GetString(8);
                                character.Hair = NAPI.Util.FromJson<HairData>(HairJSON);
                                var ItemsJSON = reader.GetString(10);
                                if(ItemsJSON != string.Empty)
                                    character.Items = NAPI.Util.FromJson<List<Item>>(ItemsJSON);
                            }
                            return character;
                        }
                        else
                        {
                            // No character found
                            account.player.SendChatMessage("No characters found on this account, create one!");
                            return null;
                        }
                    }
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
        public static List<ICharacter> getCharacters(IAccount account)
        {
            if (account == null)
                return null;
            Database db = new Database();
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                List<ICharacter> characters = new List<ICharacter>();
                string query = "SELECT * FROM `characters` WHERE AccountId = @accountId";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@accountId", account.Id);
                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Character character = new Character(account.player, account, reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4));
                                character.isCreated = reader.GetBoolean(6);
                                characters.Add(character);
                            }
                        }
                        else
                        {
                            // No character found
                            account.player.SendChatMessage("No characters found on this account, create one!");
                            return null;
                        }
                    }
                    return characters;
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
        public static ClothesData getClothes(ICharacter character, byte index)
        {

            Database db = new Database();
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                ClothesData clothesData = new ClothesData();
                string query = "SELECT * FROM `clothes` WHERE characterID = @characterId AND localID = @localID";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@characterId", character.Id);
                cmd.Parameters.AddWithValue("@localID", index);
                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var clothesJSON = reader.GetString(4);
                            clothesData = NAPI.Util.FromJson<ClothesData>(clothesJSON);
                        }
                        else
                        {
                            // No character found
                            NAPI.Util.ConsoleOutput("No clothes found on this account, create one!");
                            return null;
                        }
                    }
                    return clothesData;
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
