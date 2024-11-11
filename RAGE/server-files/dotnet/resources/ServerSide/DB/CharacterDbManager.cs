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
                    string query = "INSERT INTO `characters` (AccountId, Name, Age, Money) VALUES (@accountId, @Name, @Age, @Money)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@accountId", character.Account.Id);
                        command.Parameters.AddWithValue("@Name", character.Name);
                        command.Parameters.AddWithValue("@Age", character.Age);
                        command.Parameters.AddWithValue("@Money", character.Money);

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
   