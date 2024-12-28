using System;
using System.Drawing;
using GTANetworkAPI;
using Org.BouncyCastle.Bcpg;
using ServerSide.Characters;
using ServerSide.Server.DB;
using ServerSide.Util;
namespace ServerSide.Server
{
    public class Resource : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            Database db = new Database();
            NAPI.Util.ConsoleOutput("resursa inceppe2");
            string query = "SELECT * FROM `accounts`";

            db.ExecuteQuery(query);
            NAPI.Server.SetGlobalServerChat(false);

        }
        
    }
}
