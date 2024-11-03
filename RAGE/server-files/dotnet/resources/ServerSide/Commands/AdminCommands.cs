using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Commands
{
    internal class AdminCommands : Script
    {
        [Command("createveh")]
        public void CreateVehicleCommand(Player player, string car)
        {
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
