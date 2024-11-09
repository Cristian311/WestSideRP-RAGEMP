using System;
using GTANetworkAPI;
using ServerSide.DB;
namespace ServerSide
{
    public class Class1 : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            Database db = new Database();
            NAPI.Util.ConsoleOutput("resursa inceppe2");
            string query = "SELECT * FROM `accounts`";

            db.ExecuteQuery(query);
        }
    }
}
