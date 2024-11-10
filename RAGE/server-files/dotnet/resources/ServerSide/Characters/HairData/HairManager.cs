using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Characters.HairData
{
    internal class HairManager : Script
    {
        private static Dictionary<Player, HairData> playerHair = new Dictionary<Player, HairData>();
        [RemoteEvent("CLIENT:SERVER:HairCustomizeUpdate")]
        private void onHairCustomizeUpdate(Player player, string json)
        {
            var hairData = Newtonsoft.Json.JsonConvert.DeserializeObject<HairData>(json); //all character data vars are string
            NAPI.Util.ConsoleOutput(json);
            if(playerHair.ContainsKey(player))
                playerHair[player] = hairData;
            else
                playerHair.Add(player, hairData);
            player.SetClothes(3, 15, 0);
            player.SetClothes(11, 15, 0);
            player.SetClothes(8, 15, 0);
            player.SetClothes(2, byte.Parse(hairData.Hair), 0);
            try
            {
                var facialhair = CreateHeadOverlay(byte.Parse(hairData.FacialHair.Value), byte.Parse(hairData.FacialHair.Color), byte.Parse(hairData.FacialHair.SecondaryColor), float.Parse(hairData.FacialHair.Opacity));
                var eyebrows = CreateHeadOverlay(byte.Parse(hairData.Eyebrows.Value), byte.Parse(hairData.Eyebrows.Color), byte.Parse(hairData.Eyebrows.SecondaryColor), float.Parse(hairData.Eyebrows.Opacity));
                var bodyhair = CreateHeadOverlay(byte.Parse(hairData.BodyHair.Value), byte.Parse(hairData.BodyHair.Color), byte.Parse(hairData.BodyHair.SecondaryColor), float.Parse(hairData.BodyHair.Opacity));
                player.SetHeadOverlay(1, facialhair);
                player.SetHeadOverlay(2, eyebrows);
                player.SetHeadOverlay(10, bodyhair);
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput("HairCustomizeUpdate ERROR: " + e.Message + "\n" + e.StackTrace);
            }
        }
        private static HeadOverlay CreateHeadOverlay(Byte index, Byte color, Byte secondaryColor, float opacity)
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
