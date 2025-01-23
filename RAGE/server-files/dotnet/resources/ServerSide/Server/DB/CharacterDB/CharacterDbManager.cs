using GTANetworkAPI;
using MySql.Data.MySqlClient;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.Characters.Customization.Clothes;
using ServerSide.Characters.Customization.Customize;
using ServerSide.Characters.Customization.Hair;
using ServerSide.Server.DB;
using ServerSide.Server.Interfaces;
using ServerSide.Server.Items;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace ServerSide.Server.DB.CharacterDB
{
    internal class CharacterDbManager
    {
        public static bool CreateCharacter(ICharacter character)
        {
            Database db = new Database();
            if (CharacterExists(character.Name))
                return false;
            using (MySqlConnection connection = db.GetDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO `characters` (AccountId, Name, Age, Money, gender, faceJSON, HairJSON, TattooJSON, HighlightsJSON) VALUES (@accountId, @Name, @Age, @Money, @gender, @FaceJSON, @HairJSON, @TattooJSON, @HighlightsJSON)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@accountId", character.Account.Id);
                        command.Parameters.AddWithValue("@Name", character.Name);
                        command.Parameters.AddWithValue("@Age", character.Age);
                        command.Parameters.AddWithValue("@Money", character.Money);
                        command.Parameters.AddWithValue("@gender", character.Gender);
                        command.Parameters.AddWithValue("@FaceJSON", CharacterManager.FaceDefaultJSON);
                        command.Parameters.AddWithValue("@HairJSON", CharacterManager.HairDefaultJSON);
                        command.Parameters.AddWithValue("@TattooJSON", "{}"); // null tattoos when created
                        command.Parameters.AddWithValue("@HighlightsJSON", "{}"); // null highlihhts when created

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
        public static bool CharacterExists(string name)
        {
            if (name == string.Empty)
                return false;
            Database db = new Database();
            using (MySqlConnection connection = db.GetDatabaseConnection())
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
        public static Character GetCharacter(IAccount account, string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            Database db = new Database();
            using (MySqlConnection connection = db.GetDatabaseConnection())
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

                            Character character = new Character(
                                account.player,
                                account,
                                reader.GetString("NAME"), // Assuming Name column is not nullable
                                reader.GetInt32("Age"),
                                reader.GetInt32("Money")
                            );

                            character.Id = reader.GetInt32("id");
                            character.isCreated = reader.GetBoolean("isCreated");
                            character.Gender = reader.GetBoolean("gender");

                            //if (character.isCreated)
                            {
                                // FACE HANDLER
                                character.Face = !reader.IsDBNull("faceJSON")
                                    ? NAPI.Util.FromJson<CharacterData>(reader.GetString("faceJSON"))
                                    : null;

                                // HAIR HANDLER
                                character.Hair = !reader.IsDBNull("HairJSON")
                                    ? NAPI.Util.FromJson<HairData>(reader.GetString("HairJSON"))
                                    : null;

                                // ITEMS HANDLER
                                if (!reader.IsDBNull("ItemsJSON"))
                                {
                                    var ItemsJSON = reader.GetString("ItemsJSON");
                                    character.Items = NAPI.Util.FromJson<List<Item>>(ItemsJSON) ?? new List<Item>();
                                }
                                else
                                {
                                    character.Items = new List<Item>();
                                }

                                foreach (var item in character.Items)
                                {
                                    item.LocalId = character.ItemIdManager.AssignNewId(item);
                                    if (item.Type == "phone" && !string.IsNullOrEmpty(item.PhoneNumber.ToString()))
                                    {
                                        var phone = new Phone(character, item.Name, "phone", item.PhoneNumber);
                                        character.SendChatMessage($"phone:{item.Name} number:{item.PhoneNumber}");
                                        PhoneManager.phoneList.Add(phone);
                                    }
                                }

                                // CURRENT OUTFIT HANDLER
                                character.CurrentOutfit = !reader.IsDBNull("currentOutfit") ? reader.GetInt32("currentOutfit") : 0;

                                // TATTOOS HANDLER
                                var tattoo = reader.GetString("TattooJSON");
                                if (!tattoo.Equals("{}"))
                                    character.Tattoos = !reader.IsDBNull("TattooJSON")
                                        ? NAPI.Util.FromJson<List<Decoration>>(tattoo)
                                        : new List<Decoration>();

                                var tattooNames = reader.GetString("HighlightsJSON");
                                if (!tattoo.Equals("{}"))
                                    character.TattooNames = !reader.IsDBNull("HighlightsJSON")
                                        ? NAPI.Util.FromJson<List<string>>(tattooNames)
                                        : new List<string>();

                                // MAIN PHONE DB HANDLER
                                if (!reader.IsDBNull("currentphone"))
                                {
                                    int mainPhoneIndex = reader.GetInt32("currentphone");

                                    if (mainPhoneIndex >= 0 && mainPhoneIndex < character.Items.Count)
                                    {
                                        var phoneItem = character.Items[mainPhoneIndex];
                                        character.MainPhone = new Phone(character, phoneItem.Name, "phone", phoneItem.PhoneNumber);
                                    }
                                    else
                                    {
                                        character.MainPhone = null;
                                    }
                                }
                                else
                                {
                                    character.MainPhone = null;
                                }

                                //faction and faction rank
                                character.faction = !reader.IsDBNull("faction")
                                    ? reader.GetInt32("faction")
                                    : -1;
                                character.factionRank = !reader.IsDBNull("factionRank")
                                    ? reader.GetInt32("factionRank")
                                    : 0;
                            }

                            return character;
                        }
                        else
                        {
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

        public static Character GetCharacterById(IAccount account, int id)
        {
            Database db = new Database();
            using (MySqlConnection connection = db.GetDatabaseConnection())
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
                                if (ItemsJSON != string.Empty)
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
        public static List<ICharacter> GetCharacters(IAccount account)
        {
            if (account == null)
                return null;
            Database db = new Database();
            using (MySqlConnection connection = db.GetDatabaseConnection())
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
                                character.Id = reader.GetInt32("id");

                                //tatooo HANDLER
                                var tattoo = reader.GetString("TattooJSON");
                                if (!tattoo.Equals("{}"))
                                    character.Tattoos = !reader.IsDBNull("TattooJSON")
                                        ? NAPI.Util.FromJson<List<Decoration>>(tattoo)
                                        : new List<Decoration>();
                                // FACE HANDLER
                                character.Face = !reader.IsDBNull("faceJSON")
                                    ? NAPI.Util.FromJson<CharacterData>(reader.GetString("faceJSON"))
                                    : null;

                                // HAIR HANDLER
                                character.Hair = !reader.IsDBNull("HairJSON")
                                    ? NAPI.Util.FromJson<HairData>(reader.GetString("HairJSON"))
                                    : null;

                                //CLOTHES
                                character.CurrentOutfit = !reader.IsDBNull("currentOutfit") ? reader.GetInt32("currentOutfit") : 0;
                                character.Clothes[character.CurrentOutfit] = getClothes(character, (byte)character.CurrentOutfit);
                                NAPI.Util.ConsoleOutput(NAPI.Util.ToJson(character.Clothes[character.CurrentOutfit]));

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
            using (MySqlConnection connection = db.GetDatabaseConnection())
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
                            var clothesJSON = reader.GetString("clothesdata");
                            clothesData = NAPI.Util.FromJson<ClothesData>(clothesJSON);
                        }
                        else
                        {
                            // No character found
                            NAPI.Util.ConsoleOutput($"No clothes found on {character.Name} #{character.Id} index: {index}, create one!");
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
        public static ClothesData[] GetAllClothes(ICharacter character)
        {
            Database db = new Database();
            using (MySqlConnection connection = db.GetDatabaseConnection())
            {
                ClothesData[] clothesArray = new ClothesData[20]; // Fixed array size of 20
                int index = 0; // Variable to track how many clothes are found
                string query = "SELECT * FROM `clothes` WHERE characterID = @characterId";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@characterId", character.Id);

                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read() && index < clothesArray.Length)
                            {
                                var clothesJSON = reader.GetString(4); // Adjust this index as needed
                                var clothesData = NAPI.Util.FromJson<ClothesData>(clothesJSON);
                                clothesArray[index] = clothesData;
                                index++;
                            }
                        }
                        else
                        {
                            // No clothes found
                            NAPI.Util.ConsoleOutput("No clothes found for this character!");
                        }
                    }
                    // If not all slots are filled, the unused entries will be null.
                    return clothesArray;
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
