using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Util.Admin;

namespace ServerSide.Server.Commands.CommandAttributes
{
    public class RequiresAdmin : CommandConditionAttribute
    {
        public int AdminLevel { get; set; }
        public RequiresAdmin(int adminLevel = 1)
        {
            AdminLevel = adminLevel;
        }

        public override bool Check(Player player, string cmdName, string cmdText)
        {

            var account = AccountManager.PlayerAccountPairs[player];
            if (account.AdminLevel < AdminLevel)
            {
                player.SendChatMessage(AdminMessages.NoAccessAdmMessage);
                return false;
            }
            return true;
        }
    }
}
