using GTANetworkAPI;
using GTANetworkMethods;
using ServerSide.Accounts;
using ServerSide.Characters.Customization.Clothes;
using ServerSide.Characters.Customization.Customize;
using ServerSide.Characters.Customization.Hair;
using ServerSide.Characters.Customization.Tattoos;
using ServerSide.Server;
using ServerSide.Server.Commands;
using ServerSide.Server.DB.CharacterDB;
using ServerSide.Server.DB.VehicleDB;
using ServerSide.Server.Interfaces;
using ServerSide.Server.Vehicles;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using Player = GTANetworkAPI.Player;
using Resource = ServerSide.Server.Resource;


namespace ServerSide.Characters
{
    internal class CharacterManager : Script
    {
        //DEFAULT JSONS
        //public const string FaceDefaultJSON = "{\"shape\":{\"first\":\"0\",\"second\":\"0\",\"mix\":\"0\"},\"skin\":{\"first\":\"0\",\"second\":\"0\",\"mix\":\"0\"},\"nose\":{\"width\":\"0\",\"height\":\"0\",\"length\":\"0\",\"bridge\":\"0\",\"tip\":\"0\",\"bridgeShift\":\"0\"},\"brow\":{\"height\":\"0\",\"width\":\"0\"},\"cheeks\":{\"height\":\"0\",\"width\":\"0\",\"cheeksWidth\":\"0\"},\"jaw\":{\"width\":\"0\"},\"chin\":{\"lenght\":\"0\",\"position\":\"0\",\"width\":\"0\",\"shape\":\"0\"},\"neck\":{\"width\":\"0\"},\"lips\":\"0\",\"skinColor\":\"0\",\"eye\":{\"size\":\"0\",\"color\":\"0\"}}\r\n";
        public const string FaceDefaultJSON = "{\"shape\":{\"first\":25,\"second\":19,\"mix\":0.85},\"skin\":{\"first\":16,\"second\":14,\"mix\":0.76},\"nose\":{\"width\":0.46,\"height\":0.14,\"length\":0.34,\"bridge\":0.21,\"tip\":-0.14,\"bridgeShift\":0.35},\"brow\":{\"height\":0.38,\"width\":-0.34},\"cheeks\":{\"height\":0.58,\"width\":0.48,\"cheekswidth\":0.0},\"jaw\":{\"height\":0.24,\"width\":0.4},\"skinColor\":\"0\",\"eye\":{\"size\":-0.09,\"color\":12},\"lips\":-0.21,\"chin\":{\"length\":0.99,\"position\":0.64,\"width\":0.66,\"shape\":-0.3},\"neck\":{\"width\":0.67},\"facialDetails\":{\"blemishes\":{\"value\":21,\"opacity\":0.4},\"ageing\":{\"value\":7,\"opacity\":0.99},\"complexion\":{\"value\":5,\"opacity\":0.81},\"sunDamage\":{\"value\":8,\"opacity\":0.71},\"molesFreckles\":{\"value\":14,\"opacity\":0.81}}}";
        
        public const string HairDefaultJSON = "{\"facialHair\":{\"value\":14,\"color\":0},\"hair\":{\"value\":12,\"color\":0,\"secondColor\":0},\"eyebrows\":{\"value\":24,\"color\":4},\"bodyHair\":{\"value\":5,\"color\":0},\"hairTattoo\":12}";

        //PAIRS FOR PLAYER AND THEIR DATA
        private static Dictionary<Player, CharacterData> playerData = new Dictionary<Player, CharacterData>(); //this should be <Character, CharacterData>, needs fixed
        private static Dictionary<Character, HairData> characterHairData = new Dictionary<Character, HairData>();

        //player->character and character->account links
        public static Dictionary<Player, Character> PlayerCharacterPair = new Dictionary<Player, Character>();
        public static Dictionary<Character, Account> CharacterAccountPair = new Dictionary<Character, Account>();
        public static List<Character> loggedCharacters = new List<Character>();
        //public static Dictionary<Character, IAccount> CharacterAccountPair = new Dictionary<Character, IAccount>();

        public static Character GetCharacter(Player player)
        {
            if(PlayerCharacterPair.ContainsKey(player))
                return PlayerCharacterPair[player];
            return null;
        }
        public static List<Character> getCharByName(string name)
        {
            List<Character> chars = new List<Character>();
            foreach(var character in loggedCharacters)
            {
                var charName = character.Name.ToLower();
                NAPI.Util.ConsoleOutput(character.Name);
                if (charName.Contains(name.ToLower()))
                {
                    chars.Add(character);
                }
            }
            return chars;
        }
        public static List<Character> GetCharByNameAroundPlayer(double radius, Player player, string name)
        {
            List<Character> chars = new List<Character>();
            var playersAroundPlayer = NAPI.Player.GetPlayersInRadiusOfPlayer(radius, player);
            foreach (var character in loggedCharacters)
            {
                var charName = character.Name.ToLower();
                NAPI.Util.ConsoleOutput(character.Name);
                if (charName.Contains(name.ToLower()))
                {
                    if(playersAroundPlayer.Contains(character.player))
                        chars.Add(character);
                }
            }
            return chars;
        }
        [RemoteEvent("CLIENT:SERVER:CreateCharacter")]
        private void CreateCharacter(Player player, string firstname, string lastname, string selectedgender)
        {
            //prepare vars
            string tempname = firstname + ' ' + lastname;
            Character character = new Character(player, AccountManager.PlayerAccountPairs[player], tempname, 20, 5000);
            NAPI.Util.ConsoleOutput(selectedgender);
            character.Gender = GetGender(selectedgender);
            character.GetCharacterInfo();//debug

            //insert char in database
            CharacterDbManager.CreateCharacter(character);
            UpdateCharDataDBManager.UpdateCharacter(character);
            UpdateCharDataDBManager.UpdateCharacterFaceData(character);
            UpdateCharDataDBManager.UpdateCharacterHair(character);
        }
        [RemoteEvent("CLIENT:SERVER:ClosedCreateCharScreen")] // meaning character interface needs to appear
        private void OnCloseCreateCharScreen(Player player)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            string username = account.Name;

            GetNameMoneyJson(account, out string namesJSON, out string moneyJSON);
            
            //display character interface with new character data
            player.TriggerEvent("SERVER:CLIENT:ShowCharInterface");
            player.TriggerEvent("SERVER:CLIENT:DisplayAccountInfo", username, namesJSON, moneyJSON); 
        }
        [RemoteEvent("CLIENT:SERVER:CharSelected")]
        private void OnCharSelected(Player player, string charName)
        {
            if (charName == "null")
                return;
            //Character character = AccountManager.PlayerAccountPairs[player].currentCharacter;
            Account account = AccountManager.PlayerAccountPairs[player];

            Character character = CharacterDbManager.getCharacter(account, charName);
            loggedCharacters.Add(character);
            PlayerCharacterPair[player] = character;
            CharacterManager.CharacterAccountPair[character] = account;
            account.currentCharacter = character;
            player.Nametag = charName;

            //add the phones to phone list
            foreach (var item in character.Items)
            {
                if (item.Type == "phone")
                {
                    Phone phone = new Phone(character, item.Name, item.Type, item.PhoneNumber);
                    PhoneManager.phoneList.Add(phone);
                }
            }
            //!!!!!!! UPDATE PLAYER CHARACTER DETAILS AFTER SELECTED


            if (character.isCreated == true)
            {
                CharacterCustomizeManager.UpdatePlayerFaceFeatures(player, character.Face);
                //update hair
                if (character.Hair != null)
                {
                    HairManager.UpdateHair(player, character.Hair);
                }
            }
            //else
            //{
            //    string faceJson = NAPI.Util.ToJson(character.Face);
            //    player.TriggerEvent("SERVER:CLIENT:ShowCustomizeCharScreen", character.Gender, faceJson);
            //}
            ClothesManager.GetClothes(player, (byte)character.currentOutfit);
            if (character.Tattoos != null)
            {
                Decoration[] decorations = new Decoration[character.Tattoos.Count];
                for (int i = 0; i < character.Tattoos.Count; i++)
                {
                    decorations[i] = character.Tattoos[i];
                }
                TattooManager.ApplyTattoos(player, decorations);
            }
            player.Position = new Vector3(253.85786, 287.4269, 105.533104);
            player.Name = character.Name;
            CustomizeCommands.fixskinCommand(player);
            character.GameId = account.GameId;

            character.PersonalVehicles = VehicleDbManager.LoadVehiclesByName(character.Name);
            foreach(var veh in character.PersonalVehicles)
            {
                veh.VehicleLocalId = character.VehicleIdManager.AssignNewId(veh);
            }
            character.AccessVehicles = character.PersonalVehicles;
            character.SetMoneyHud(character.Money, character.BankMoney);
            //VehicleManager.FixPersonalVehicles(character);

            character.Clothes = CharacterDbManager.GetAllClothes(character);
            //show hud
            player.TriggerEvent("SERVER:CLIENT:ShowLogo", $"{Resource.ServerName} {Resource.Version}");
            character.SendInfoMessage($"Te-ai conectat ca {Colors.WHITE}{character.Name}{Colors.INFO} ID:{Colors.WHITE} {character.GameId}");
        }
        public static List<Character> GetMatchingCharacters(string nameOrId)
        {
            // If the input is an integer (ID), filter by ID
            if (int.TryParse(nameOrId, out int id))
            {
                return loggedCharacters.Where(c => c.Id == id).ToList();
            }

            // Otherwise, filter by name
            return loggedCharacters.Where(c => c.Name.Contains(nameOrId, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public static Character GetCharacterByName(Player player, string nameOrId)
        {
            var matches = GetMatchingCharacters(nameOrId);

            if (matches.Count == 1)
            {
                return matches[0];
            }
            else if (matches.Count > 1)
            {
                player.SendChatMessage($"Multiple characters found matching '{nameOrId}':");
                foreach (var character in matches)
                {
                    player.SendChatMessage($"- {character.Name} (ID: {character.Id})");
                }
                return null;
            }
            else
            {
                player.SendChatMessage($"No character found with the name or ID '{nameOrId}'.");
                return null;
            }
        }
        public static Character GetCharacterByName(string nameOrId)
        {
            var matches = GetMatchingCharacters(nameOrId);

            if (matches.Count == 1)
            {
                return matches[0];
            }
            else if (matches.Count > 1)
            {
                return null;
            }
            else
            {
                return null;
            }
        }
        public static Character GetCharacterNearby(Player player, string targetName, float range = 20f)
        {
            var chars = GetCharByNameAroundPlayer(range, player, targetName);

            if (chars.Count == 1)
            {
                return chars[0]; // Return the character if exactly one is found
            }
            else if (chars.Count > 1)
            {
                foreach (var character in chars)
                {
                    player.SendChatMessage(character.Name); // Print all found characters
                }
            }

            return null; // Return null if no character or multiple characters are found
        }

        private bool GetGender(string gender)
        {
            NAPI.Util.ConsoleOutput(gender);
            if (gender.Equals("male"))
                return true;
            else
                return false;
        }
        public static void GetNameMoneyJson(Account account, out string namesJSON, out string moneyJSON)
        {
            List<ICharacter> characters = CharacterDbManager.getCharacters(account);

            GetNamesAndMoneyForCEF(characters, GetRequiredCount(characters), out string[] names, out int[] money);

            namesJSON = NAPI.Util.ToJson(names);
            moneyJSON = NAPI.Util.ToJson(money);
        }
            
        public static void GetNamesAndMoneyForCEF(List<ICharacter> characters, int requiredCount, out string[] names, out int[] money)
        {
            names = new string[requiredCount];
            money = new int[requiredCount];
            if (characters != null)
            {
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i] != null)
                    {
                        names[i] = characters[i].Name;
                        money[i] = characters[i].Money;
                    }
                    else
                    {
                        names[i] = "none";
                        money[i] = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < requiredCount; i++)
                {
                    names[i] = "none";
                    money[i] = 0;
                }    
            }
        }
        public static int GetRequiredCount(List<ICharacter> characters)
        {
            int requiredCount;
            if (characters == null)
                requiredCount = 3;
            else
                requiredCount = Math.Max(3, characters.Count);
            return requiredCount;
        }
    }
}
