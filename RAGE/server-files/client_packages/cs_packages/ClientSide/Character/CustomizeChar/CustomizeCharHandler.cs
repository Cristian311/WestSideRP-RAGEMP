using Clientside.CustomizeChar;
using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Character.CustomizeChar
{
    internal class CustomizeCharHandler : Events.Script
    {
        private HtmlWindow cefBrowser;
        bool Gender;
        public CustomizeCharHandler()
        {
            Events.Add("CEF:CLIENT:ShowCustomizeCharScreen", ShowCustomizeCharScreen);
            Events.Add("SERVER:CLIENT:ShowCustomizeCharScreen", ShowCustomizeCharScreenServer);
            Events.Add("CEF:CLIENT:CloseCustomizeCharScreen", CloseCustomizeCharScreen);
            Events.Add("CEF:CLIENT:UpdateCharacter", UpdateCharacter);
            Events.Add("CEF:CLIENT:UpdateCharacterClient", UpdateCharacterClient);
        }

        private void UpdateCharacterClient(object[] args)
        {
            string json = args[0].ToString();
            CharacterData charData = RAGE.Util.Json.Deserialize<CharacterData>(json);
            UpdatePlayerFaceFeatures(charData, Gender);
        }

        private void ShowCustomizeCharScreenServer(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package2/html/Characters/customize_char.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;

            string GenderJson = args[0].ToString();
            Gender = bool.Parse(GenderJson);

            string json = args[1].ToString();
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");

            cefBrowser.ExecuteJs($"updateCharacterDataFromJson(\"{escapedJson}\");");
        }
        private void ShowCustomizeCharScreen(object[] args)
        {
            
            cefBrowser = new HtmlWindow("http://package2/html/Characters/customize_char.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;
        }
        private void CloseCustomizeCharScreen(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;

            string DetailsJSON = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:ClosedCustomizeCharScreen", DetailsJSON); //send the json to server and and update the database
        }
        private void UpdateCharacter(object[] args)
        {
            string json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:CharCustomiseUpdate", json);
        }

        private void UpdatePlayerFaceFeatures(CharacterData data, bool gender)
        {
            //eyes
            var eyeColor = data.Eye.Color;
            RAGE.Elements.Player.LocalPlayer.SetEyeColor(eyeColor);

            //head details
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(0, data.Facial.Blemishes, 1f);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(3, data.Facial.Ageing, 1f);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(6, data.Facial.Complexion, 1f);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(7, data.Facial.SunDamage, 1f);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(9, data.Facial.MolesFreckles, 1f);

            //headblend
            var shapeFirst = data.Shape.First;
            var shapeSecond = data.Shape.Second;

            var skinFirst = data.Skin.First;
            var skinSecond = data.Skin.Second;

            var shapeMix = data.Shape.Mix;
            var skinMix = data.Skin.Mix;
            RAGE.Elements.Player.LocalPlayer.SetHeadBlendData(shapeFirst, shapeSecond, 0, skinFirst, skinSecond, 0, shapeMix, skinMix, 0, true);

            //facefeatures
            var features = GetFaceFeatures(data);
            for (int i = 0; i < 20; i++)
            {
                RAGE.Elements.Player.LocalPlayer.SetFaceFeature(i, features[i]);
                
            }
            
        }

        private static float[] GetFaceFeatures(CharacterData charData)
        {
            float[] face = new float[20];
            face[0] = charData.Nose.Width;
            face[1] = charData.Nose.Height;
            face[2] = charData.Nose.Length;
            face[3] = charData.Nose.Bridge;
            face[4] = charData.Nose.Tip ;
            face[5] = charData.Nose.BridgeShift;
            face[6] = charData.Brow.Height;
            face[7] = charData.Brow.Width;
            face[8] = charData.Cheeks.Height;
            face[9] = charData.Cheeks.Width;
            face[10] = charData.Cheeks.CheeksWidth;
            face[11] = charData.Eye.Size;
            face[12] = charData.Lips;
            face[13] = charData.Jaw.Width;
            face[14] = charData.Jaw.Width;
            face[15] = charData.Chin.Lenght;
            face[16] = charData.Chin.Position;
            face[17] = charData.Chin.Width;
            face[18] = charData.Chin.Shape;
            face[19] = charData.Neck.Width;
            return face;
        }
    }
}
