using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace ServerSide.Blips
{
    internal class BlipHandler : Script
    {
        private static Blip blip;
        private BlipHandler()
        {
            Vector3 blipPos = new Vector3(-418.45023, 1151.701, 326.87988);
            blip = NAPI.Blip.CreateBlip(11, blipPos, 1f, 2, "Hatz", 255, 0, false, 0, 0);
        }
        [Command("blip")]
        private void blipCommand(Player player)
        {
            player.SendChatMessage(NAPI.Util.ToJson(blip));
            
        }
    }
}
