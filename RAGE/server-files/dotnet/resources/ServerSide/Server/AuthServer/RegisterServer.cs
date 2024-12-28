using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ServerSide.Server.DB;
namespace ServerSide.Server.AuthServer
{
    internal class RegisterServer : Script
    {
        [RemoteEvent("CLIENT:SERVER:ClientRegister")]
        public void OnRegisterAttempt(Player player, string username, string password)
        {
            AccountDbManager db = new AccountDbManager();
            if (db.CheckAccountRegister(username))
            {
                db.RegisterAccount(username, password);
                NAPI.Util.ConsoleOutput(username + " s-a inregistrat.");
                //player.TriggerEvent("SERVER:CLIENT:closeAuthInterface");
                player.TriggerEvent("SERVER:CLIENT:DisplayRegisterMessage", "Account created! Log into your account");
            }
            else
            {
                player.TriggerEvent("SERVER:CLIENT:DisplayRegisterMessage", "Account already exists");
            }
        }
    }
}
