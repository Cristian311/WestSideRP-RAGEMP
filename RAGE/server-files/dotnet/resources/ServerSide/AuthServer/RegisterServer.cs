using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.AuthServer
{
    internal class RegisterServer : Script
    {
        [RemoteEvent("CLIENT:SERVER:clientRegister")]
        public void OnRegisterAttempt(Player player, string username, string password)
        {
            DatabaseHelper db = new DatabaseHelper();
            if (db.checkAccountRegister(username))
            {
                db.RegisterAccount(username, password);
                NAPI.Util.ConsoleOutput(username + " s-a inregistrat.");
                player.TriggerEvent("SERVER:CLIENT:closeAuthInterface");
            }
            else
            {
                player.TriggerEvent("SERVER:CLIENT:displayRegisterFail");
            }
        }
    }
}
