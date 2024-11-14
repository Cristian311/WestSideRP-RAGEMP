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


namespace ServerSide.Characters
{
    internal class CharacterManager : Script
    {
        private static int cont = 0;
        
        //PAIRS FOR PLAYER AND THEIR DATA
        private static Dictionary<Player, CharacterData> playerData = new Dictionary<Player, CharacterData>(); //this should be <Character, CharacterData>, needs fixed
        private static Dictionary<Character, HairData> characterHairData = new Dictionary<Character, HairData>();

        //player->character and character->account links
        public static Dictionary<Player, Character> PlayerCharacterPair = new Dictionary<Player, Character>();
        public static Dictionary<Character, Account> CharacterAccountPair = new Dictionary<Character, Account>();
        //public static Dictionary<Character, IAccount> CharacterAccountPair = new Dictionary<Character, IAccount>();

        [RemoteEvent("CLIENT:SERVER:CreateCharacter")]
        private void CreateCharacter(Player player, string firstname, string lastname, string selectedgender)
        {
            //prepare vars
            string tempname = firstname + ' ' + lastname;
            Character character = new Character(AccountManager.PlayerAccountPairs[player], tempname, 20, 5000);
            NAPI.Util.ConsoleOutput(selectedgender);
            character.gender = getGender(selectedgender);
            character.getCharacterInfo();//debug
            //insert char in database
            CharacterDbManager.CreateCharacter(character);
            UpdateCharDataDBManager.UpdateCharacter(character);
            UpdateCharDataDBManager.UpdateCharacterFaceData(character);
            UpdateCharDataDBManager.UpdateCharacterHair(character);
        }
        [RemoteEvent("CLIENT:SERVER:ClosedCreateCharScreen")] // meaning character interface needs to appear
        private void onCloseCreateCharScreen(Player player)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            string username = account.Name;

            List<ICharacter> characters = CharacterDbManager.getCharacters(account);

            CharacterManager cm = new CharacterManager();
            cm.getNamesAndMoneyForCEF(characters, cm.getRequiredCount(characters), out string[] names, out int[] money);

            string namesJSON = NAPI.Util.ToJson(names);
            string moneyJSON = NAPI.Util.ToJson(money);
            
            //display character interface with new character data
            player.TriggerEvent("SERVER:CLIENT:showCharInterface");
            player.TriggerEvent("SERVER:CLIENT:displayAccountInfo", username, namesJSON, moneyJSON); 
        }
        [RemoteEvent("CLIENT:SERVER:CharSelected")]
        private void onCharSelected(Player player, string charName)
        {
            if (charName == "null")
                return;
            //Character character = AccountManager.PlayerAccountPairs[player].currentCharacter;
            Account account = AccountManager.PlayerAccountPairs[player];

            Character character = CharacterDbManager.getCharacter(account, charName);
            player.SendChatMessage(charName);
            PlayerCharacterPair[player] = character;

            //!!!!!!! UPDATE PLAYER CHARACTER DETAILS AFTER SELECTED

            if (character.isCreated == true)
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<CharacterData>(character.CharacterFaceDetailsJSON);
                CharacterCustomizeManager.UpdatePlayerFaceFeatures(player, data, character.gender);
                //update hair
                if (character.HairJSON != null)
                {
                    var hairData = Newtonsoft.Json.JsonConvert.DeserializeObject<HairData>(character.HairJSON);
                    HairManager.updateHair(player, hairData);
                }
            }
        }
        
        private bool getGender(string gender)
        {
            NAPI.Util.ConsoleOutput(gender);
            if (gender.Equals("male"))
                return true;
            else
                return false;
        }
        public void getNamesAndMoneyForCEF(List<ICharacter> characters, int requiredCount, out string[] names, out int[] money)
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
        public int getRequiredCount(List<ICharacter> characters)
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
