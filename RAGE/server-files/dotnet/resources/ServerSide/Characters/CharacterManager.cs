using GTANetworkAPI;
using ServerSide.Interfaces;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ServerSide.Accounts;
using ServerSide.Hair;
using GTANetworkMethods;
using Player = GTANetworkAPI.Player;
using ServerSide.DB.CharacterDB;
using System.Threading;
using Org.BouncyCastle.Asn1.X509;
using ServerSide.Characters.Clothes;
using ServerSide.Characters.Tattoos;


namespace ServerSide.Characters
{
    internal class CharacterManager : Script
    {
        //DEFAULT JSONS
        public const string FaceDefaultJSON = "{\"shape\":{\"first\":\"0\",\"second\":\"0\",\"mix\":\"0\"},\"skin\":{\"first\":\"0\",\"second\":\"0\",\"mix\":\"0\"},\"nose\":{\"width\":\"0\",\"height\":\"0\",\"length\":\"0\",\"bridge\":\"0\",\"tip\":\"0\",\"bridgeShift\":\"0\"},\"brow\":{\"height\":\"0\",\"width\":\"0\"},\"cheeks\":{\"height\":\"0\",\"width\":\"0\",\"cheeksWidth\":\"0\"},\"jaw\":{\"width\":\"0\"},\"chin\":{\"lenght\":\"0\",\"position\":\"0\",\"width\":\"0\",\"shape\":\"0\"},\"neck\":{\"width\":\"0\"},\"lips\":\"0\",\"skinColor\":\"0\",\"eye\":{\"size\":\"0\",\"color\":\"0\"}}\r\n";
        public const string HairDefaultJSON = "{\"hair\":\"0\",\"facialHair\":{\"value\":\"0\",\"color\":\"0\",\"secondaryColor\":\"0\",\"opacity\":1},\"eyebrows\":{\"value\":\"0\",\"color\":\"0\",\"secondaryColor\":\"0\",\"opacity\":\"1\"},\"bodyHair\":{\"value\":\"0\",\"color\":\"0\",\"secondaryColor\":\"0\",\"opacity\":\"1\"},\"hairTattoo\":{\"value\":\"0\",\"color\":\"0\",\"secondaryColor\":\"0\",\"opacity\":\"1\"}}\r\n";
        
        //PAIRS FOR PLAYER AND THEIR DATA
        private static Dictionary<Player, CharacterData> playerData = new Dictionary<Player, CharacterData>(); //this should be <Character, CharacterData>, needs fixed
        private static Dictionary<Character, HairData> characterHairData = new Dictionary<Character, HairData>();

        //player->character and character->account links
        public static Dictionary<Player, Character> PlayerCharacterPair = new Dictionary<Player, Character>();
        public static Dictionary<Character, Account> CharacterAccountPair = new Dictionary<Character, Account>();
        public static List<Character> loggedCharacters = new List<Character>();
        //public static Dictionary<Character, IAccount> CharacterAccountPair = new Dictionary<Character, IAccount>();

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
            player.TriggerEvent("SERVER:CLIENT:showCharInterface");
            player.TriggerEvent("SERVER:CLIENT:displayAccountInfo", username, namesJSON, moneyJSON); 
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

            //!!!!!!! UPDATE PLAYER CHARACTER DETAILS AFTER SELECTED

            if (character.isCreated == true)
            {
                CharacterCustomizeManager.UpdatePlayerFaceFeatures(player, character.Face, character.Gender);
                //update hair
                if (character.Hair != null)
                {
                    HairManager.UpdateHair(player, character.Hair);
                }
            }
            ClothesManager.GetClothes(player, (byte)character.currentOutfit);
            TattooManager.ApplyTattoos(player, character.tattoos);
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
