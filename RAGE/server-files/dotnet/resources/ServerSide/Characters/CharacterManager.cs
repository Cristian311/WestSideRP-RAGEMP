using GTANetworkAPI;
using ServerSide.DB;
using ServerSide.Interfaces;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ServerSide.Accounts;
using ServerSide.Hair;
using GTANetworkMethods;
using Player = GTANetworkAPI.Player;

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
            CharacterDbManager dbManager = new CharacterDbManager();
            dbManager.CreateCharacter(character);
            dbManager.UpdateCharacter(character);
            dbManager.UpdateCharacterFaceData(character);
            dbManager.UpdateCharacterHair(character);
        }
        [RemoteEvent("CLIENT:SERVER:ClosedCreateCharScreen")] // meaning character interface needs to appear
        private void onCloseCreateCharScreen(Player player)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            string username = account.Name;

            CharacterDbManager characterDb = new CharacterDbManager();
            List<ICharacter> characters = characterDb.getCharacters(account);

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

            CharacterDbManager characterDbManager = new CharacterDbManager();
            Character character = characterDbManager.getCharacter(account, charName);
            player.SendChatMessage(charName);
            PlayerCharacterPair[player] = character;

            //!!!!!!! UPDATE PLAYER CHARACTER DETAILS AFTER SELECTED

            if (character.isCreated == true)
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<CharacterData>(character.CharacterFaceDetailsJSON);
                UpdatePlayerFaceFeatures(player, data, character.gender);
                //update hair
                if (character.HairJSON != null)
                {
                    var hairData = Newtonsoft.Json.JsonConvert.DeserializeObject<HairData>(character.HairJSON);
                    HairManager.updateHair(player, hairData);
                }
            }
        }
        [RemoteEvent("CLIENT:SERVER:CheckIfCharExistsForCustomizeScreen")] // this checks if the character is created or not, if it's not created it will show the cusyomize character interface
        private void CharVerifications(Player player)
        {
            Character character = PlayerCharacterPair[player];
            if (character.isCreated == false)
                player.TriggerEvent("SERVER:CLIENT:showCustomizeCharScreen");

            character.getCharacterInfo();//debug
        }

        [RemoteEvent("CLIENT:SERVER:ClosedCustomizeCharScreen")]
        private void ClosedCustomizeCharScreen(Player player, string faceFeaturesJSON)
        {
            Character character = PlayerCharacterPair[player];
            if (character.isCreated == false) //show the hair screen if it's not created for first time hairstyle modeling
            {
                character.isCreated = true;
                player.TriggerEvent("SERVER:CLIENT:showHairScreen", character.HairJSON);
                character.getCharacterInfo(); // debug
            }
            character.CharacterFaceDetailsJSON = faceFeaturesJSON;

            //update db
            CharacterDbManager cm = new CharacterDbManager();
            cm.UpdateCharacter(character);
            cm.UpdateCharacterFaceData(character);
        }
        
        [RemoteEvent("CLIENT:SERVER:CharCustomiseUpdate")]
        private void onCharCustomiseUpdate(Player player, string json)
        {
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<CharacterData>(json); //all character data vars are string
            Character character = PlayerCharacterPair[player];
            try
            {
                UpdatePlayerFaceFeatures(player, data, character.gender);
                NAPI.Util.ConsoleOutput(character.gender.ToString());
            }
            catch(Exception e) 
            {
                NAPI.Util.ConsoleOutput("onCharCustomiseUpdate ERROR: " + e.Message + "\n" + e.StackTrace);
            }
        }
        private void UpdatePlayerFaceFeatures(Player player, CharacterData data, bool gender)
        {
            //placeholder empty vars
            Decoration[] decoration = new Decoration[10];
            Dictionary<int, HeadOverlay> headOverlays = new Dictionary<int, HeadOverlay>();
            HeadOverlay head = new HeadOverlay();

            //actual vars
            HeadBlend headBlend = getHeadBlend(data);
            var eyeColor = byte.Parse(data.Eye.Color);
            var features = faceFeatures(data);
            NAPI.Util.ConsoleOutput(gender.ToString());
            player.SetCustomization(gender, headBlend, eyeColor, 0, 0, features, headOverlays, decoration);
            
            for (int i = 0; i < 20; i++)
            {
                var feature = features[i] * 0.01;
                player.SetFaceFeature(i, (float)feature);
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
        private float[] faceFeatures(CharacterData charData)
        {
            float[] face = new float[20];
            face[0] = float.Parse(charData.Nose.Width);
            face[1] = float.Parse(charData.Nose.Height);
            face[2] = float.Parse(charData.Nose.Length);
            face[3] = float.Parse(charData.Nose.Bridge);
            face[4] = float.Parse(charData.Nose.Tip);
            face[5] = float.Parse(charData.Nose.BridgeShift);
            face[6] = float.Parse(charData.Brow.Height);
            face[7] = float.Parse(charData.Brow.Width);
            face[8] = float.Parse(charData.Cheeks.Height);
            face[9] = float.Parse(charData.Cheeks.Width);
            face[10] = float.Parse(charData.Cheeks.CheeksWidth);
            face[11] = float.Parse(charData.Eye.Size);
            face[12] = float.Parse(charData.Lips);
            face[13] = float.Parse(charData.Jaw.Width);
            face[14] = float.Parse(charData.Jaw.Width); 
            face[15] = float.Parse(charData.Chin.Lenght);
            face[16] = float.Parse(charData.Chin.Position);
            face[17] = float.Parse(charData.Chin.Width);
            face[18] = float.Parse(charData.Chin.Shape);
            face[19] = float.Parse(charData.Neck.Width);
            return face;
        }
        private HeadBlend getHeadBlend(CharacterData charData)
        {
            HeadBlend headBlend = new HeadBlend();
            headBlend.ShapeFirst = byte.Parse(charData.Shape.First);
            headBlend.ShapeSecond = byte.Parse(charData.Shape.Second);
            headBlend.ShapeThird = 0; //byte.Parse(charData.Shape.Third);
            headBlend.ShapeMix = float.Parse(charData.Shape.Mix)*0.01f;

            headBlend.SkinFirst = byte.Parse(charData.Skin.First);
            headBlend.SkinSecond = byte.Parse(charData.Skin.Second);
            headBlend.SkinThird = 0; //byte.Parse(charData.Skin.Third);
            headBlend.SkinMix = float.Parse(charData.Skin.Mix)*0.01f;

            //headBlend.ThirdMix = int.Parse(charData.) not implemented yet
            return headBlend;
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
