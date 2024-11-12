using GTANetworkAPI;
using MySql.Data.MySqlClient;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.DB
{
    internal class CharacterDbManager
    {
        public bool UpdateCharacter(ICharacter character)
        {
            Database db = new Database();
            if (!CharacterExists(character.Name)) // Check existence by unique Id
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
        public bool UpdateCharacterFaceData(ICharacter character)
        {
            Database db = new Database();
            if (!CharacterExists(character.Name)) // Check existence by unique Id
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
                        command.Parameters.AddWithValue("@json", character.CharacterFaceDetailsJSON);
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
        public bool UpdateCharacterHair(ICharacter character)
        {
            Database db = new Database();
            if (!CharacterExists(character.Name)) // Check existence by unique Id
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
                        command.Parameters.AddWithValue("@json", character.HairJSON);
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
        public bool CreateCharacter(ICharacter character)
        {
            Database db = new Database();
            if (CharacterExists(character.Name))
                return false;
            using (MySqlConnection connection = db.getDatabaseConnection())
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO `characters` (AccountId, Name, Age, Money, gender) VALUES (@accountId, @Name, @Age, @Money, @gender)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@accountId", character.Account.Id);
                        command.Parameters.AddWithValue("@Name", character.Name);
                        command.Parameters.AddWithValue("@Age", character.Age);
                        command.Parameters.AddWithValue("@Money", character.Money);
                        command.Parameters.AddWithValue("@gender", character.gender);

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
        public bool CharacterExists(string name)
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
        public Character getCharacter(IAccount account, string name)
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

                            Character character = new Character(account, reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4));
                            character.Id = reader.GetInt32(0);
                            character.isCreated = reader.GetBoolean(6);
                            character.gender = reader.GetBoolean(9);
                            if (character.isCreated == true)
                            {
                                character.CharacterFaceDetailsJSON = reader.GetString(7);
                                character.HairJSON = reader.GetString(8);
                                
                                NAPI.Util.ConsoleOutput(character.HairJSON);
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
        public List<ICharacter> getCharacters(IAccount account)
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
                                Character character = new Character(account, reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4));
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
    }
} 
   