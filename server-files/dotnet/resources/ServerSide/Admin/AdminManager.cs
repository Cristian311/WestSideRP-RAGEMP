using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Util;
using ServerSide.Util.Admin;
using Player = GTANetworkAPI.Player;

namespace ServerSide.Admin
{
    internal class AdminManager : Script
    {
        private AdminManager()
        {

        }
        public static bool CheckIfAdminAccess(Player player, int minimumAdminLevel)
        {
            var account = AccountManager.GetAccount(player);
            if (account.AdminLevel < minimumAdminLevel)
            {
                player.SendChatMessage(AdminMessages.NoAccessAdmMessage);
                return false;
            }
            return true;
        }
        public static void SendChatMessageToAdmins(string message)
        {
            foreach (var account in AccountManager.loggedAccounts)
            {
                var player = account.player;
                if (account.AdminLevel > 0)
                    player.SendChatMessage($"{Colors.ADMIN}AdmInfo: {message}");
            }
        }
    }
}
