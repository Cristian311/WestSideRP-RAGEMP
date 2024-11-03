using GTANetworkAPI;
using ServerSide.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Commands
{
    internal class AdminCommands : Script
    {
        AccountManager accountManager = new AccountManager();
        [Command("createveh")]
        public void CreateVehicleCommand(Player player, string car)
        {
            //if (AccountManager.PlayerAccountPairs[player].AdminLevel)
            VehicleHash busModel = NAPI.Util.VehicleNameToModel(car); //Gets the hash of bus model
            NAPI.Vehicle.CreateVehicle(busModel, player.Position, 0, 0, 0); //Creates a bus on the player's position
        }
        [Command("weather")]
        public void cmd_weather(Player player, string weather)
        {
            NAPI.World.SetWeather(weather);
            player.SendNotification($"Set weather to {weather}.");
        }
    }
}
