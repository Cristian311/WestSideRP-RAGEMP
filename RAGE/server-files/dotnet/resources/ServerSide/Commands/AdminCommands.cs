using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Admin;
using ServerSide.Util.Admin;
using System;
using System.Collections.Generic;
using System.Text;
using Player = GTANetworkAPI.Player;

namespace ServerSide.Commands
{
    internal class AdminCommands : Script
    {
        AccountManager accountManager = new AccountManager();
        [Command("createveh")]
        public void CreateVehicleCommand(Player player, string car)
        {
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;
            VehicleHash busModel = NAPI.Util.VehicleNameToModel(car); //Gets the hash of bus model
            NAPI.Vehicle.CreateVehicle(busModel, player.Position, 0, 0, 0); //Creates a bus on the player's position

        }
        [Command("weapon")]
        public void WeaponCommand(Player player, string weapon, int ammo)
        {
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;
            WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(weapon);
            player.GiveWeapon(weaponHash, ammo);
        }
        [Command("weather")]
        public void cmd_weather(Player player, string weather)
        {
            if (!AdminManager.CheckIfAdminAccess(player,1))
                return;
            NAPI.World.SetWeather(weather);
            player.SendNotification($"Set weather to {weather}.");
        }
        [Command("time")]
        public void cmd_time(Player player, int hour)
        {
            if (!AdminManager.CheckIfAdminAccess(player, 1))
                return;
            NAPI.World.SetTime(hour, 0, 0);
            player.SendNotification($"Set time to {hour}.");
        }
    }
}
