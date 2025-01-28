using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters.Customization.Clothes;
using ServerSide.Characters.Customization.Customize;
using ServerSide.Characters.Customization.Hair;
using ServerSide.Characters.Customization.Tattoos;
using ServerSide.Server.Commands;
using ServerSide.Server.DB.CharacterDB;
using ServerSide.Server.DB.VehicleDB;
using ServerSide.Server.Interfaces;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Player = GTANetworkAPI.Player;
using Resource = ServerSide.Server.Resource;


namespace ServerSide.Characters
{
    internal class CharacterManager : Script
    {
        public const string FaceDefaultJSON = "{\"shape\":{\"first\":25,\"second\":19,\"mix\":0.85},\"skin\":{\"first\":16,\"second\":14,\"mix\":0.76},\"nose\":{\"width\":0.46,\"height\":0.14,\"length\":0.34,\"bridge\":0.21,\"tip\":-0.14,\"bridgeShift\":0.35},\"brow\":{\"height\":0.38,\"width\":-0.34},\"cheeks\":{\"height\":0.58,\"width\":0.48,\"cheekswidth\":0.0},\"jaw\":{\"height\":0.24,\"width\":0.4},\"skinColor\":\"0\",\"eye\":{\"size\":-0.09,\"color\":12},\"lips\":-0.21,\"chin\":{\"length\":0.99,\"position\":0.64,\"width\":0.66,\"shape\":-0.3},\"neck\":{\"width\":0.67},\"facialDetails\":{\"blemishes\":{\"value\":21,\"opacity\":0.4},\"ageing\":{\"value\":7,\"opacity\":0.99},\"complexion\":{\"value\":5,\"opacity\":0.81},\"sunDamage\":{\"value\":8,\"opacity\":0.71},\"molesFreckles\":{\"value\":14,\"opacity\":0.81}}}";

        public const string HairDefaultJSON = "{\"facialHair\":{\"value\":14,\"color\":0},\"hair\":{\"value\":12,\"color\":0,\"secondColor\":0},\"eyebrows\":{\"value\":24,\"color\":4},\"bodyHair\":{\"value\":5,\"color\":0},\"hairTattoo\":12}";

        public static Dictionary<Player, Character> PlayerCharacterPair = new Dictionary<Player, Character>();
        public static List<Character> loggedCharacters = new List<Character>();


        public static Character GetCharacter(Player player)
        {
            if (PlayerCharacterPair.ContainsKey(player))
                return PlayerCharacterPair[player];
            return null;
        }
        public static List<Character> getCharByName(string name)
        {
            List<Character> chars = new List<Character>();
            foreach (var character in loggedCharacters)
            {
                var charName = character.Name.ToLower();

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
                if (charName.Contains(name.ToLower()))
                {
                    if (playersAroundPlayer.Contains(character.player))
                        chars.Add(character);
                }
            }
            return chars;
        }

        [RemoteEvent("CLIENT:SERVER:CreateCharacter")]
        private void CreateCharacter(Player player, string firstname, string lastname, string selectedgender)
        {
            //prepare vars
            string Name = firstname + ' ' + lastname;
            var account = AccountManager.GetAccount(player);
            Character character = new Character(player, account, Name, 20, 5000);

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
            var account = AccountManager.GetAccount(player);
            string username = account.Name;

            GetNameMoneyJson(account, out string namesJSON, out string moneyJSON, account.Characters);

            //display character interface with new character data
            player.TriggerEvent("SERVER:CLIENT:ShowCharInterface");
            player.TriggerEvent("SERVER:CLIENT:DisplayAccountInfo", username, namesJSON, moneyJSON);
        }

        [RemoteEvent("CLIENT:SERVER:CharSelected")]
        private void OnCharSelected(Player player, string charName)
        {
            if (charName == "null")
                return;

            //account setup
            var account = AccountManager.GetAccount(player);
            var character = CharacterDbManager.GetCharacter(account, charName);

            loggedCharacters.Add(character);
            PlayerCharacterPair[player] = character;
            account.currentCharacter = character;


            //character update
            character.GameId = account.GameId;
            character.PersonalVehicles = VehicleDbManager.LoadVehiclesByName(character.Name);
            foreach (var veh in character.PersonalVehicles)
            {
                veh.VehicleLocalId = character.VehicleIdManager.AssignNewId(veh);
            }
            character.AccessVehicles = character.PersonalVehicles;
            character.SetMoneyHud(character.Money, character.BankMoney);
            character.Clothes = CharacterDbManager.GetAllClothes(character);
            character.SendInfoMessage($"Te-ai conectat ca {Colors.WHITE}{character.Name}{Colors.INFO} ID:{Colors.WHITE} {character.GameId}");

            //features update
            UpdatePhones(character);
            CharacterCustomizeManager.UpdatePlayerFaceFeatures(character.player, character.Face);
            UpdateHair(character);
            ClothesManager.GetClothes(player, (byte)character.CurrentOutfit);
            ApplyTattoos(character);

            //player update
            player.Nametag = charName;
            player.Position = new Vector3(253.85786, 287.4269, 105.533104);
            player.Name = character.Name;
            player.Dimension = 0;
            CustomizeCommands.fixskinCommand(player);
            player.TriggerEvent("SERVER:CLIENT:ShowLogo", $"{Resource.ServerName} {Resource.Version}");//show hud
            player.TriggerEvent("SERVER:CLIENT:UnfreezePlayer");
            player.TriggerEvent("SERVER:CLIENT:CameraOff");
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
        public static void ShowChangeCharacterScreen(IAccount account)
        {
            var player = account.player;

            var facesJson = NAPI.Util.ToJson(account.Characters.Select(c => c.Face));
            var hairsJson = NAPI.Util.ToJson(account.Characters.Select(c => c.Hair));
            var clothesJson = NAPI.Util.ToJson(account.Characters.Select(c => CharacterDbManager.GetClothes(c, (byte)c.CurrentOutfit)));


            player.TriggerEvent("SERVER:CLIENT:CharactersInfo", facesJson, hairsJson, clothesJson);

            CharacterManager.GetNameMoneyJson(account, out string namesJSON, out string moneyJSON, account.Characters);
            player.TriggerEvent("SERVER:CLIENT:ShowCharInterface");
            player.TriggerEvent("SERVER:CLIENT:DisplayAccountInfo", account.Name, namesJSON, moneyJSON);
            player.TriggerEvent("SERVER:CLIENT:ChooseCharacterCamera");
            player.TriggerEvent("SERVER:CLIENT:FreezePlayer");
            
            player.Rotation = new Vector3(0, 0, 144);
            player.Dimension = account.GameId != 0 ? (uint)account.GameId : 1001;
        }
        public static Character GetCharacterByNameOrId(Player player, string nameOrId)
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
        public static Character GetCharacterByNameOrId(string nameOrId)
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
            if (gender.Equals("male"))
                return true;
            else
                return false;
        }
        public static void GetNameMoneyJson(IAccount account, out string namesJSON, out string moneyJSON, List<Character> characters)
        {


            GetNamesAndMoneyForCEF(characters, GetRequiredCount(characters), out string[] names, out int[] money);

            namesJSON = NAPI.Util.ToJson(names);
            moneyJSON = NAPI.Util.ToJson(money);
        }

        private static void GetNamesAndMoneyForCEF(List<Character> characters, int requiredCount, out string[] names, out int[] money)
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
        private static int GetRequiredCount(List<Character> characters)
        {
            int requiredCount;
            if (characters == null)
                requiredCount = 3;
            else
                requiredCount = Math.Max(3, characters.Count);
            return requiredCount;
        }
        private void UpdatePhones(Character character)
        {
            foreach (var item in character.Items)
            {
                if (item.Type == "phone")
                {
                    Phone phone = new Phone(character, item.Name, item.Type, item.PhoneNumber);
                    PhoneManager.phoneList.Add(phone);
                }
            }
        }
        private void ApplyTattoos(Character character)
        {
            if (character.Tattoos != null)
            {
                Decoration[] decorations = new Decoration[character.Tattoos.Count];
                for (int i = 0; i < character.Tattoos.Count; i++)
                {
                    decorations[i] = character.Tattoos[i];
                }
                TattooManager.ApplyTattoos(character.player, decorations);
            }
        }
        private void UpdateHair(Character character)
        {
            if (character.isCreated == true)
            {
                //update hair
                if (character.Hair != null)
                {
                    HairManager.UpdateHair(character.player, character.Hair);
                }
            }
        }
    }
}
