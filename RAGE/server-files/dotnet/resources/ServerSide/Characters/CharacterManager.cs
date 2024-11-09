using GTANetworkAPI;
using MySql.Data.MySqlClient;
using ServerSide.DB;
using ServerSide.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
//using System.Text.Json;
using ServerSide.Accounts;
using System.Reflection;
using Org.BouncyCastle.Asn1.X509;
using System.Text.Json;

namespace ServerSide.Characters
{
    internal class CharacterManager : Script
    {
        private static int cont = 0;
        [RemoteEvent("CLIENT:SERVER:CreateCharacter")]
        private void CreateCharacter(Player player, string firstname, string lastname, string selectedgender)
        {
            CharacterDbManager dbManager = new CharacterDbManager();
            string tempname = firstname + ' ' + lastname;
            Character character = new Character(AccountManager.PlayerAccountPairs[player], tempname, 20, 0);
            dbManager.CreateCharacter(character);
        }

        [RemoteEvent("CLIENT:SERVER:CharCustomiseUpdate")]
        private void onCharCustomiseUpdate(Player player, string json)
        {
            var playerData = Newtonsoft.Json.JsonConvert.DeserializeObject<CharacterData>(json); //all character data vars are string
            try
            {
                NAPI.Util.ConsoleOutput(cont++ + json + '\n');
                playerData.PrintProperties();

                HeadBlend headBlend = getHeadBlend(playerData);
                Decoration[] decoration = new Decoration[10];
                
                Dictionary<int, HeadOverlay> headOverlays = new Dictionary<int, HeadOverlay>();
                HeadOverlay head = new HeadOverlay();
                headOverlays.Add(0, head);
                var eyeColor = byte.Parse(playerData.Eye.Color);
                //var hairColor = //byte.Parse(playerData.Hair.Color);
                player.SetCustomization(true, headBlend, eyeColor, 0, 0, faceFeatures(playerData), headOverlays, decoration);
                Decoration decoration1 = new Decoration();
                //player.SetHeadOverlay
                var features = faceFeatures(playerData);
                for (int i = 0; i < 20; i++)
                {
                    
                   
                    var feature = features[i] * 0.01;
                    player.SetFaceFeature(i, (float)feature);
                    NAPI.Util.ConsoleOutput($"getfacefeature[{i}]={player.GetFaceFeature(i)}");
                    NAPI.Util.ConsoleOutput($"feature[{i}]={feature}");
                    NAPI.Util.ConsoleOutput($"featurearray[{i}]={features[i]}");
                }
            }
            catch(Exception e) 
            {
                NAPI.Util.ConsoleOutput("onCharCustomiseUpdate ERROR: " + e.Message + "\n" + e.StackTrace);
            }
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
