using GTANetworkAPI;
using GTANetworkMethods;
using ServerSide.Server.DB.CharacterDB;
using System;
using System.Collections.Generic;
using Player = GTANetworkAPI.Player;

namespace ServerSide.Characters.Customization.Customize
{
    internal class CharacterCustomizeManager : Script
    {
        [RemoteEvent("CLIENT:SERVER:CheckIfCharExistsForCustomizeScreen")] // this checks if the character is created or not, if it's not created it will show the cusyomize character interface
        private void CharVerifications(Player player)
        {
            Character character = CharacterManager.PlayerCharacterPair[player];
            if (character.isCreated == false)
            {
                string faceJson = NAPI.Util.ToJson(character.Face);
                player.TriggerEvent("SERVER:CLIENT:ShowCustomizeCharScreen", character.Gender, faceJson);
            }
            character.GetCharacterInfo();//debug
        }

        [RemoteEvent("CLIENT:SERVER:ClosedCustomizeCharScreen")]
        private void ClosedCustomizeCharScreen(Player player, string faceFeaturesJSON)
        {
            Character character = CharacterManager.PlayerCharacterPair[player];
            if (character.isCreated == false) //show the hair screen if it's not created for first time hairstyle modeling
            {
                character.isCreated = true;
                var HairJSON = NAPI.Util.ToJson(character.Hair);
                player.TriggerEvent("SERVER:CLIENT:ShowHairScreen", HairJSON); // sends hair as json
                character.GetCharacterInfo(); // debug
            }
            character.Face = NAPI.Util.FromJson<CharacterData>(faceFeaturesJSON);

            //update db
            UpdateCharDataDBManager.UpdateCharacter(character);
            UpdateCharDataDBManager.UpdateCharacterFaceData(character);
        }

        [RemoteEvent("CLIENT:SERVER:CharCustomiseUpdate")]
        private void onCharCustomiseUpdate(Player player, string json)
        {
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<CharacterData>(json); //all character data vars are string
            try
            {
                UpdatePlayerFaceFeatures(player, data);
                NAPI.Util.ConsoleOutput(json);
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput("onCharCustomiseUpdate ERROR: " + e.Message + "\n" + e.StackTrace);
            }
        }

        public static void UpdatePlayerFaceFeatures(Player player, CharacterData data)
        {
            bool gender = CharacterManager.PlayerCharacterPair[player].Gender;
            //head blend
            var headBlend = GetHeadBlend(data);
            var features = GetFaceFeatures(data);
            player.SetCustomization(gender, headBlend, data.Eye.Color, 0, 0, features, new Dictionary<int, HeadOverlay>(), new Decoration[10]);

            //face features
            SetFaceFeatures(player, features);

            //face details
            SetFaceDetails(player, data);
        }

        private static void SetFaceDetails(Player player, CharacterData data)
        {
            if (data.Facial == null)
                return;
            var blemishes = CreateHeadOverlay(data.Facial.Blemishes, 0, 0, 1);
            player.SetHeadOverlay(0, blemishes);

            var ageing = CreateHeadOverlay(data.Facial.Ageing, 0, 0, 1);
            player.SetHeadOverlay(3, ageing);

            var complexion = CreateHeadOverlay(data.Facial.Complexion, 0, 0, 1);
            player.SetHeadOverlay(6, complexion);

            var sunDamage = CreateHeadOverlay(data.Facial.SunDamage, 0, 0, 1);
            player.SetHeadOverlay(7, sunDamage);

            var molesFreckles = CreateHeadOverlay(data.Facial.MolesFreckles, 0, 0, 1);
            player.SetHeadOverlay(9, molesFreckles);


        }
        
        private static void SetFaceFeatures(Player player, float[] features)
        {
            for (int i = 0; i < 20; i++)
            {
                player.SetFaceFeature(i, features[i]);
            }
        }
        private static float[] GetFaceFeatures(CharacterData charData)
        {
            float[] face = new float[20];
            face[0] = charData.Nose.Width;
            face[1] = charData.Nose.Height;
            face[2] = charData.Nose.Length;
            face[3] = charData.Nose.Bridge;
            face[4] = charData.Nose.Tip;
            face[5] = charData.Nose.BridgeShift;
            face[6] = charData.Brow.Height;
            face[7] = charData.Brow.Width;
            face[8] = charData.Cheeks.Height;
            face[9] = charData.Cheeks.Width;
            face[10] = charData.Cheeks.CheeksWidth;
            face[11] = charData.Eye.Size;
            face[12] = charData.Lips;
            face[13] = charData.Jaw.Width;
            face[14] = charData.Jaw.Height;
            face[15] = charData.Chin.Length;
            face[16] = charData.Chin.Position;
            face[17] = charData.Chin.Width;
            face[18] = charData.Chin.Shape;
            face[19] = charData.Neck.Width;
            return face;
        }

        private static HeadBlend GetHeadBlend(CharacterData charData)
        {
            HeadBlend headBlend = new HeadBlend();
            headBlend.ShapeFirst = charData.Shape.First;
            headBlend.ShapeSecond = charData.Shape.Second;
            headBlend.ShapeThird = 0; //byte.Parse(charData.Shape.Third);
            headBlend.ShapeMix = charData.Shape.Mix;

            headBlend.SkinFirst = charData.Skin.First;
            headBlend.SkinSecond = charData.Skin.Second;
            headBlend.SkinThird = 0; //byte.Parse(charData.Skin.Third);
            headBlend.SkinMix = charData.Skin.Mix;

            //headBlend.ThirdMix = int.Parse(charData.) not implemented yet
            return headBlend;
        }
        private static HeadOverlay CreateHeadOverlay(byte index, byte color, byte secondaryColor, float opacity)
        {
            return new HeadOverlay
            {
                Index = index,
                Color = color,
                SecondaryColor = secondaryColor,
                Opacity = opacity
            };
        }
    }
}
