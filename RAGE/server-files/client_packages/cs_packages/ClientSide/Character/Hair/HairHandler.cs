using ClientSide.Hair;
using RAGE;
using RAGE.Ui;
using System.Collections.Generic;


namespace ClientSide.Character.Hair
{
    internal class HairHandler : Events.Script
    {
        private HtmlWindow cefBrowser;
        private HairDecalsList hairDecalsList;
        private HairHandler()
        {
            Events.Add("SERVER:CLIENT:ShowHairScreen", ShowHairScreen);
            Events.Add("CEF:CLIENT:StopHairScreen", StopHairScreen);
            Events.Add("CEF:CLIENT:UpdateHair", UpdateHair);
            hairDecalsList = new HairDecalsList
            {
                Decals = new List<List<HairDecal>>
            {
                // Male
                new List<HairDecal>
                {
                    new HairDecal { ID = 0, Collection = "mpbeach_overlays", Overlay = "FM_Hair_Fuzz" },
                    new HairDecal { ID = 1, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_001" },
                    new HairDecal { ID = 2, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_002" },
                    new HairDecal { ID = 3, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_003" },
                    new HairDecal { ID = 4, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_004" },
                    new HairDecal { ID = 5, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_005" },
                    new HairDecal { ID = 6, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_006" },
                    new HairDecal { ID = 7, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_007" },
                    new HairDecal { ID = 8, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_008" },
                    new HairDecal { ID = 9, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_009" },
                    new HairDecal { ID = 10, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_013" },
                    new HairDecal { ID = 11, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_002" },
                    new HairDecal { ID = 12, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_011" },
                    new HairDecal { ID = 13, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_012" },
                    new HairDecal { ID = 14, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_014" },
                    new HairDecal { ID = 15, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_015" },
                    new HairDecal { ID = 16, Collection = "multiplayer_overlays", Overlay = "NGBea_M_Hair_000" },
                    new HairDecal { ID = 17, Collection = "multiplayer_overlays", Overlay = "NGBea_M_Hair_001" },
                    new HairDecal { ID = 18, Collection = "multiplayer_overlays", Overlay = "NGBus_M_Hair_000" },
                    new HairDecal { ID = 19, Collection = "multiplayer_overlays", Overlay = "NGBus_M_Hair_001" },
                    new HairDecal { ID = 20, Collection = "multiplayer_overlays", Overlay = "NGHip_M_Hair_000" },
                    new HairDecal { ID = 21, Collection = "multiplayer_overlays", Overlay = "NGHip_M_Hair_001" },
                    new HairDecal { ID = 22, Collection = "multiplayer_overlays", Overlay = "NGInd_M_Hair_000" },
                    new HairDecal { ID = 24, Collection = "mplowrider_overlays", Overlay = "LR_M_Hair_000" },
                    new HairDecal { ID = 25, Collection = "mplowrider_overlays", Overlay = "LR_M_Hair_001" },
                    new HairDecal { ID = 26, Collection = "mplowrider_overlays", Overlay = "LR_M_Hair_002" },
                    new HairDecal { ID = 27, Collection = "mplowrider_overlays", Overlay = "LR_M_Hair_003" },
                    new HairDecal { ID = 28, Collection = "mplowrider2_overlays", Overlay = "LR_M_Hair_004" },
                    new HairDecal { ID = 29, Collection = "mplowrider2_overlays", Overlay = "LR_M_Hair_005" },
                    new HairDecal { ID = 30, Collection = "mplowrider2_overlays", Overlay = "LR_M_Hair_006" },
                    new HairDecal { ID = 31, Collection = "mpbiker_overlays", Overlay = "MP_Biker_Hair_000_M" },
                    new HairDecal { ID = 32, Collection = "mpbiker_overlays", Overlay = "MP_Biker_Hair_001_M" },
                    new HairDecal { ID = 33, Collection = "mpbiker_overlays", Overlay = "MP_Biker_Hair_002_M" },
                    new HairDecal { ID = 34, Collection = "mpbiker_overlays", Overlay = "MP_Biker_Hair_003_M" },
                    new HairDecal { ID = 35, Collection = "mpbiker_overlays", Overlay = "MP_Biker_Hair_004_M" },
                    new HairDecal { ID = 36, Collection = "mpbiker_overlays", Overlay = "MP_Biker_Hair_005_M" },
                    new HairDecal { ID = 72, Collection = "mpgunrunning_overlays", Overlay = "MP_Gunrunning_Hair_M_000_M" },
                    new HairDecal { ID = 73, Collection = "mpgunrunning_overlays", Overlay = "MP_Gunrunning_Hair_M_001_M" }
                },
                // Female
                new List<HairDecal>
                {
                    new HairDecal { ID = 0, Collection = "mpbeach_overlays", Overlay = "FM_Hair_Fuzz" },
                    new HairDecal { ID = 1, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_001" },
                    new HairDecal { ID = 2, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_002" },
                    new HairDecal { ID = 3, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_003" },
                    new HairDecal { ID = 4, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_004" },
                    new HairDecal { ID = 5, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_005" },
                    new HairDecal { ID = 6, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_006" },
                    new HairDecal { ID = 7, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_007" },
                    new HairDecal { ID = 8, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_008" },
                    new HairDecal { ID = 9, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_009" },
                    new HairDecal { ID = 10, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_010" },
                    new HairDecal { ID = 11, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_011" },
                    new HairDecal { ID = 12, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_012" },
                    new HairDecal { ID = 13, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_013" },
                    new HairDecal { ID = 14, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_014" },
                    new HairDecal { ID = 15, Collection = "multiplayer_overlays", Overlay = "NG_M_Hair_015" },
                    new HairDecal { ID = 16, Collection = "multiplayer_overlays", Overlay = "NGBea_F_Hair_000" },
                    new HairDecal { ID = 17, Collection = "multiplayer_overlays", Overlay = "NGBea_F_Hair_001" },
                    new HairDecal { ID = 18, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_007" },
                    new HairDecal { ID = 19, Collection = "multiplayer_overlays", Overlay = "NGBus_F_Hair_000" },
                    new HairDecal { ID = 20, Collection = "multiplayer_overlays", Overlay = "NGBus_F_Hair_001" },
                    new HairDecal { ID = 21, Collection = "multiplayer_overlays", Overlay = "NGBea_F_Hair_001" },
                    new HairDecal { ID = 22, Collection = "multiplayer_overlays", Overlay = "NGHip_F_Hair_000" },
                    new HairDecal { ID = 23, Collection = "multiplayer_overlays", Overlay = "NGInd_F_Hair_000" },
                    new HairDecal { ID = 25, Collection = "mplowrider_overlays", Overlay = "LR_F_Hair_000" },
                    new HairDecal { ID = 26, Collection = "mplowrider_overlays", Overlay = "LR_F_Hair_001" },
                    new HairDecal { ID = 27, Collection = "mplowrider_overlays", Overlay = "LR_F_Hair_002" },
                    new HairDecal { ID = 28, Collection = "mplowrider2_overlays", Overlay = "LR_F_Hair_003" },
                    new HairDecal { ID = 29, Collection = "mplowrider2_overlays", Overlay = "LR_F_Hair_003" },
                    new HairDecal { ID = 30, Collection = "mplowrider2_overlays", Overlay = "LR_F_Hair_004" },
                    new HairDecal { ID = 31, Collection = "mplowrider2_overlays", Overlay = "LR_F_Hair_006" },
                    new HairDecal { ID = 32, Collection = "mpbiker_overlays", Overlay = "MP_Biker_Hair_000_F" },
                    new HairDecal { ID = 33, Collection = "mpbiker_overlays", Overlay = "MP_Biker_Hair_001_F" },
                    new HairDecal { ID = 34, Collection = "mpbiker_overlays", Overlay = "MP_Biker_Hair_002_F" },
                    new HairDecal { ID = 35, Collection = "mpbiker_overlays", Overlay = "MP_Biker_Hair_003_F" },
                    new HairDecal { ID = 36, Collection = "multiplayer_overlays", Overlay = "NG_F_Hair_003" },
                    new HairDecal { ID = 37, Collection = "mpbiker_overlays", Overlay = "MP_Biker_Hair_006_F" },
                    new HairDecal { ID = 38, Collection = "mpbiker_overlays", Overlay = "MP_Biker_Hair_004_F" },
                    new HairDecal { ID = 76, Collection = "mpgunrunning_overlays", Overlay = "MP_Gunrunning_Hair_F_000_F" },
                    new HairDecal { ID = 77, Collection = "mpgunrunning_overlays", Overlay = "MP_Gunrunning_Hair_F_001_F" }
                }
            }
            };
        }

        private void UpdateHair(object[] args)
        {
            string json = args[0].ToString();
            HairData hair = RAGE.Util.Json.Deserialize<HairData>(json);
            int facialValue = int.Parse(hair.FacialHair.Value);
            int facialOpacity = int.Parse(hair.FacialHair.Opacity);
            int eyebrowsValue = int.Parse(hair.Eyebrows.Value);
            int eyebrowsOpacity = int.Parse(hair.Eyebrows.Opacity);
            int bodyhairValue = int.Parse(hair.BodyHair.Value);
            int bodyhairOpacity = int.Parse(hair.BodyHair.Opacity);
            int hairValue = int.Parse(hair.Hair);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(1, facialValue, facialOpacity);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(2, eyebrowsValue, eyebrowsOpacity);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(10, bodyhairValue, bodyhairOpacity);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(2, hairValue, 0, 0);
        }
        private void ShowHairScreen(object[] args)
        {
            // Initialize the HTML window and show the cursor
            cefBrowser = new HtmlWindow("http://package2/html/Characters/customize_hair.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;

            string json = args[0].ToString();
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");
            cefBrowser.ExecuteJs($"updateHairDataFromJson(\"{escapedJson}\");");
        }

        private void StopHairScreen(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
            //Chat.Output("closed");

            string json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:UpdateHair", json);
        }
    }
}
