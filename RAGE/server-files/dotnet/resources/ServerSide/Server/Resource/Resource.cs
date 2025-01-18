using GTANetworkAPI;
using ServerSide.Server.DB;
using ServerSide.Util;
namespace ServerSide.Server
{
    public class Resource : Script
    {
        private static readonly string ServerName = "West-Side Roleplay";
        private static readonly string Version = "Version: 0.5a";

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            Database db = new Database();
            NAPI.Util.ConsoleOutput("resursa inceppe2");
            string query = "SELECT * FROM `accounts`";

            db.ExecuteQuery(query);
            NAPI.Server.SetGlobalServerChat(false);

        }

        [ServerEvent(Event.PlayerConnected)]
        private void OnPlayerConnected(Player player)
        {
            string text = $"{ServerName} {Version}";
            player.TriggerEvent("SERVER:CLIENT:ShowLogo", text);
            player.SendChatMessage(Colors.INFO + text);
        }
    }
}
