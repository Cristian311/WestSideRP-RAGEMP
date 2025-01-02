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
            Chat.Output(json);
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
            Chat.Output(escapedJson + "hatz");
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
            var eyeColor = byte.Parse(data.Eye.Color);
            var features = GetFaceFeatures(data);
            RAGE.Elements.Player.LocalPlayer.SetEyeColor(eyeColor);
            int shapeFirst = int.Parse(data.Shape.First);
            int shapeSecond = int.Parse(data.Shape.Second);
            int skinFirst = int.Parse(data.Skin.First);
            int skinSecond = int.Parse(data.Skin.Second);
            int shapeMix = int.Parse(data.Shape.Mix);
            int skinMix = int.Parse(data.Shape.Mix);
            RAGE.Elements.Player.LocalPlayer.SetHeadBlendData(shapeFirst, shapeSecond, 0, skinFirst, skinSecond, 0, shapeMix, skinMix, 0, true);

            for (int i = 0; i < 20; i++)
            {
                var feature = features[i] * 0.01;
                RAGE.Elements.Player.LocalPlayer.SetFaceFeature(i, (float)feature);
            }
            
        }

        private static float[] GetFaceFeatures(CharacterData charData)
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
    }
}
