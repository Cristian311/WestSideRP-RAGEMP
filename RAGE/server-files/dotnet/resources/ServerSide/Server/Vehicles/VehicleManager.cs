using GTANetworkAPI;
using Org.BouncyCastle.Bcpg;
using ServerSide.Characters;
using ServerSide.Server.Vehicles.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerSide.Server.Vehicles
{
    internal class VehicleManager : Script
    {
        private static IdManager<Vehicle> vehicleIdManager = new IdManager<Vehicle>();
        public static List<IVehicle> Vehicles { get; private set; } = new List<IVehicle>();
        public static List<VehicleInfo> VehicleInfos { get; private set; } = new List<VehicleInfo>();

        // Static initialization Task
        private static Task initializationTask;

        static VehicleManager()
        {
            initializationTask = InitializeAsync();
        }

        // Asynchronous initialization method
        private static async Task InitializeAsync()
        {
            var loader = await VehicleLoader.CreateAsync("output/vehicleData.json");
            VehicleInfos = loader.Vehicles;
        }

        // Ensures static initialization has completed
        public static async Task EnsureInitializedAsync()
        {
            await initializationTask;
            
        }
        [ServerEvent(Event.PlayerEnterVehicle)]
        private void PlayerEnterVehicleEvent(Player player, GTANetworkAPI.Vehicle vehicle, sbyte seatID)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            character.CurrentDrivingVehicle = vehicle;
            var ServerVehicle = Vehicles.FirstOrDefault(v => v.GTAVehicle == character.CurrentDrivingVehicle);
            if(ServerVehicle != null)
            {
                player.TriggerEvent("SERVER:CLIENT:UpdateFuelAndDistance", ServerVehicle.Distance, ServerVehicle.Gas);
            }
        }
        [RemoteEvent("CLIENT:SERVER:UpdateCarPosition")]
        private void UpdateCarPositionEvent(Player player, double distance)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            var vehicle = Vehicles.FirstOrDefault(v => v.GTAVehicle == character.CurrentDrivingVehicle);
            vehicle.Distance += distance;
            vehicle.GasDistance += distance;
            vehicle.Position = player.Position;
            if (vehicle.GasDistance > 300)
            {
                vehicle.Gas -= 0.01;
                vehicle.GasDistance = 0;
                player.SendChatMessage(vehicle.Gas.ToString());
                player.TriggerEvent("SERVER:CLIENT:UpdateFuelAndDistance", vehicle.Distance, vehicle.Gas);
            }
        }
        [Command("createpersonalvehicle", Alias = "cpv")]
        private void CreatePersonalVehicleCommand(Player player, string vehName)
        {
            var character = CharacterManager.PlayerCharacterPair[player];

            VehicleHash vehModel = NAPI.Util.VehicleNameToModel(vehName);
            GTANetworkAPI.Vehicle vehicle = NAPI.Vehicle.CreateVehicle(vehModel, player.Position, 0, 0, 0);

            // Initialize server vehicle
            Vehicle serverVeh = new Vehicle(character.Name, "personal", vehicle);
            serverVeh.VehicleId = vehicleIdManager.AssignNewId(serverVeh);

            Vehicles.Add(serverVeh);
            character.PersonalVehicles.Add(serverVeh);
            character.AccessVehicles.Add(serverVeh);
            character.SendChatMessage("Ai primit un nou vehicul de tipul: " + vehicle.DisplayName);
        }

        [Command("servervehicles")]
        private void ServerVehiclesCommand(Player player, double speed = 0)
        {
            foreach(var veh in VehicleInfos)
            {
                if (veh.MaxSpeed > speed)
                    player.SendChatMessage($"Name: {veh.VehicleName}, Speed: {veh.MaxSpeed}, Breaking: {veh.MaxBraking}");
            }
        }
        [Command("vehicles")]
        private void VehiclesCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];

            int cont = 0;
            player.SendChatMessage("Masinile tale: ");
            foreach (var vehicle in character.PersonalVehicles)
            {
                player.SendChatMessage($"{cont}. {vehicle.VehicleId} {vehicle.GTAVehicle.DisplayName}");
                cont++;
            }
        }
    }
}
