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
        public static Dictionary<Account, Player> AccountPlayerPairs = new Dictionary<Account, Player>();
        public static Dictionary<Player, Account> PlayerAccountPairs = new Dictionary<Player, Account>();

    }
}
