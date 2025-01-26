using GTANetworkAPI;
using ServerSide.Util;

namespace ServerSide.Server.Commands
{
    internal class TogCommands : Script
    {
        [Command("tognametag")]
        private void TogNametagCommand(Player player)
        {
            player.TriggerEvent("SERVER:CLIENT:TogNametag");
            bool tog = player.GetData<bool>("tognametag");
            if (tog == null)
                tog = true;
            if (tog == false)
            {
                player.SetData<bool>("tognametag", true);
                player.SendChatMessage($"{Colors.INFO}At activat optiunea sa-ti vezi nametagul.");
            }
            else
            {
                player.SetData<bool>("tognametag", false);
                player.SendChatMessage($"{Colors.INFO}At dezactivat optiunea sa-ti vezi nametagul.");
            }
        }
    }
}
