using GTANetworkAPI;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace ServerSide.Server.Commands
{
    internal class TogCommands : Script
    {
        [Command("tognametag")]
        private void TogNametagCommand(Player player)
        {
            player.TriggerEvent("SERVER:CLIENT:TogNametag");
            bool tog = player.GetData<bool>("TogNametag");
            if (tog == null)
                tog = true;
            if(tog == false)
            {
                player.SetData<bool>("TogNametag", true);
                player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}At activat optiunea sa-ti vezi nametagul.");
            }    
            else
            {
                player.SetData<bool>("TogNametag", false);
                player.SendChatMessage($"{Colors.COLOR_CHAT_INFO}At dezactivat optiunea sa-ti vezi nametagul.");
            }
        }
    }
}
