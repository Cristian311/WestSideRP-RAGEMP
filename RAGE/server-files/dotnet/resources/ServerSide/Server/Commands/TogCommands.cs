using GTANetworkAPI;
using ServerSide.Util;

namespace ServerSide.Server.Commands
{
    internal class TogCommands : Script
    {
        [Command("tognametag")]
        private void TogNametagCommand(Player player)
        {
            if (!player.HasData("tognametag"))
            {
                player.SetData("tognametag", true);
            }

            bool tog = player.GetData<bool>("tognametag");

            tog = !tog;
            player.SetData("tognametag", tog);

            if (tog)
            {
                player.SendChatMessage($"Ai {Colors.GREEN}activat {Colors.WHITE}optiunea sa-ti vezi nametagul.");
            }
            else
            {
                player.SendChatMessage($"Ai {Colors.RED}dezactivat {Colors.WHITE}optiunea sa-ti vezi nametagul.");
            }

            player.TriggerEvent("SERVER:CLIENT:TogNametag", tog);
        }

        [Command("togambience")]
        private void cmd_togambience(Player player)
        {
            if (!player.HasData("togambience"))
            {
                player.SetData("togambience", true);
            }

            bool tog = player.GetData<bool>("togambience");

            tog = !tog;
            player.SetData("togambience", tog);

            if (tog)
            {
                player.SendChatMessage($"Ai {Colors.GREEN}activat {Colors.WHITE}optiunea sa auzi sunetele ambientale.");
            }
            else
            {
                player.SendChatMessage($"Ai {Colors.RED}dezactivat {Colors.WHITE}optiunea sa auzi sunetele ambientale.");
            }
            player.TriggerEvent("SERVER:CLIENT:TogAmbience", tog);
        }
    }
}
