using GTANetworkAPI;
using GTANetworkMethods;
using ServerSide.Characters;
using ServerSide.Server.Commands;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Player = GTANetworkAPI.Player;

namespace ServerSide.Server.Vehicles
{
    internal class VehicleEvents : Script
    {
        [RemoteEvent("CLIENT:SERVER:LockKeyPressed")]
        private void LockKeyPressed(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            var closestVehicle = character.AccessVehicles
                .OrderBy(vehicle => Vector3.Distance(character.player.Position, vehicle.Position))
                .FirstOrDefault();

            if (closestVehicle != null)
            {
                if (Vector3.Distance(character.player.Position, closestVehicle.GTAVehicle.Position) > 10)
                {
                    player.SendChatMessage($"{Colors.RED}Nu ai nici o masina prin apropiere.");
                    return;
                }
                if (closestVehicle.GTAVehicle.Locked == false)
                {
                    player.SendChatMessage($"{Colors.RED}Ti-ai incuiat masina!");
                    closestVehicle.GTAVehicle.Locked = true;
                    ChatMessage.cmd_ame(player, "si-a incuiat masina.");
                }
                else
                {
                    player.SendChatMessage($"{Colors.FOREST_GREEN}Ti-ai descuiat masina!");
                    closestVehicle.GTAVehicle.Locked = false;
                    ChatMessage.cmd_ame(player, "si-a descuiat masina.");
                }    
            }
            else
            {
                player.SendChatMessage($"{Colors.RED}Nu ai nici o cheie pentru vreo masina.");
            }
            
        }
    }
}
