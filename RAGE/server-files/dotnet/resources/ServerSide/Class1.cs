using System;
using GTANetworkAPI;
namespace ServerSide
{
    public class Class1 : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            DatabaseHelper db = new DatabaseHelper();
            NAPI.Util.ConsoleOutput("resursa inceppe");
            string query = "SELECT * FROM `accounts`";

            db.ExecuteQuery(query);
        }
    }
}
