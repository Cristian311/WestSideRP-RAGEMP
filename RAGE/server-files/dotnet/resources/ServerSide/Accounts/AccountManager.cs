using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Accounts
{
    public class AccountManager
    {
        public static List<Account> loggedAccounts = new List<Account>();
        public static Dictionary<Account, Player> AccountPlayerPairs = new Dictionary<Account, Player>();


    }
}
