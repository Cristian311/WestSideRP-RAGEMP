using GTANetworkAPI;
using ServerSide.Accounts;
using System;
using System.Collections.Generic;
using System.IO;

namespace ServerSide.Server.Logger
{
    internal class ServerLogger : Script
    {
        private static List<string> log = new List<string>();
        private static string filepath = "log.txt";
        public static void Log(string message)
        {
            message = DateTime.Now.ToString() + " - " + message + "\n";
            log.Add(message);
            File.AppendAllTextAsync(filepath, message);
        }
        public static void LogWithAccount(Player player, string message)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            message = DateTime.Now.ToString() + " (" + account.Name + ") " + message + "\n";
            log.Add(message);
            File.AppendAllTextAsync(filepath, message);
        }
    }
}
