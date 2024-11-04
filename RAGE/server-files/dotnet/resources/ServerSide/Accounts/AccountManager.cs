using GTANetworkAPI;
using ServerSide.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Accounts
{
    public class AccountManager
    {
        public static List<IAccount> loggedAccounts = new List<IAccount>();
        public static Dictionary<IAccount, Player> AccountPlayerPairs = new Dictionary<IAccount, Player>();
        public static Dictionary<Player, IAccount> PlayerAccountPairs = new Dictionary<Player, IAccount>();

    }
}
