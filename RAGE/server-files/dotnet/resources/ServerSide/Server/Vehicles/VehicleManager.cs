using GTANetworkAPI;
using ServerSide.Characters;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace ServerSide.Server.Vehicles
{
    internal class VehicleManager : Script
    {
        private static IdManager<Vehicle> vehicleIdManager = new IdManager<Vehicle>();
        public List<Vehicle> vehicles = new List<Vehicle>();

        [Command("createpersonalvehicle")]
        private void CreatePersonalVehicleCommand(Player player, string vehName)
        {
            var character = CharacterManager.PlayerCharacterPair[player];

            VehicleHash vehModel = NAPI.Util.VehicleNameToModel(vehName);
            GTANetworkAPI.Vehicle vehicle = NAPI.Vehicle.CreateVehicle(vehModel, player.Position, 0, 0, 0);

            //init server veh
            Vehicle serverVeh = new Vehicle(character.Name, "personal", vehicle);
            serverVeh.VehicleId = vehicleIdManager.AssignNewId(serverVeh);

            vehicles.Add(serverVeh);
            character.PersonalVehicles.Add(serverVeh);
            character.SendChatMessage("Ai primit un nou vehicul de tipul:" + vehicle.DisplayName);
        }
        [Command("vehicles")]
        private void VehiclesCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];

            int cont = 0;
            player.SendChatMessage("Masinile tale: ");
            foreach (var vehicle in character.PersonalVehicles)
            {
                player.SendChatMessage($"{cont}. {vehicle.VehicleId} {vehicle.Name}");
                cont++;
            }
        }
    }
}
