using GTANetworkAPI;
using ServerSide.Server.DB.CharacterDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Characters.Customization.Customize
{
    internal class CharacterCustomizeManager : Script
    {
        [RemoteEvent("CLIENT:SERVER:CheckIfCharExistsForCustomizeScreen")] // this checks if the character is created or not, if it's not created it will show the cusyomize character interface
        private void CharVerifications(Player player)
        {
            Character character = CharacterManager.PlayerCharacterPair[player];
            if (character.isCreated == false)
                player.TriggerEvent("SERVER:CLIENT:ShowCustomizeCharScreen", character.Gender);

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
            CharacterData data = Newtonsoft.Json.JsonConvert.DeserializeObject<CharacterData>(json); //all character data vars are string
            Character character = CharacterManager.PlayerCharacterPair[player];
            try
            {
                UpdatePlayerFaceFeatures(player, data, character.Gender);
                NAPI.Util.ConsoleOutput(character.Gender.ToString());
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput("onCharCustomiseUpdate ERROR: " + e.Message + "\n" + e.StackTrace);
            }
        }

        public static void UpdatePlayerFaceFeatures(Player player, CharacterData data, bool gender)
        {
            //placeholder empty vars
            Decoration[] decoration = new Decoration[10];
            Dictionary<int, HeadOverlay> headOverlays = new Dictionary<int, HeadOverlay>();
            HeadOverlay head = new HeadOverlay();

            //actual vars
            HeadBlend headBlend = GetHeadBlend(data);
            var eyeColor = byte.Parse(data.Eye.Color);
            var features = FaceFeatures(data);
            NAPI.Util.ConsoleOutput(gender.ToString());
            player.SetCustomization(gender, headBlend, eyeColor, 0, 0, features, headOverlays, decoration);

            for (int i = 0; i < 20; i++)
            {
                var feature = features[i] * 0.01;
                player.SetFaceFeature(i, (float)feature);
            }
        }

        private static float[] FaceFeatures(CharacterData charData)
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
        private static HeadBlend GetHeadBlend(CharacterData charData)
        {
            HeadBlend headBlend = new HeadBlend();
            headBlend.ShapeFirst = byte.Parse(charData.Shape.First);
            headBlend.ShapeSecond = byte.Parse(charData.Shape.Second);
            headBlend.ShapeThird = 0; //byte.Parse(charData.Shape.Third);
            headBlend.ShapeMix = float.Parse(charData.Shape.Mix) * 0.01f;

            headBlend.SkinFirst = byte.Parse(charData.Skin.First);
            headBlend.SkinSecond = byte.Parse(charData.Skin.Second);
            headBlend.SkinThird = 0; //byte.Parse(charData.Skin.Third);
            headBlend.SkinMix = float.Parse(charData.Skin.Mix) * 0.01f;

            //headBlend.ThirdMix = int.Parse(charData.) not implemented yet
            return headBlend;
        }
    }
}
