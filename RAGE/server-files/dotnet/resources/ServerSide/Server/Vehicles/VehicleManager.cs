using GTANetworkAPI;
using Org.BouncyCastle.Bcpg;
using ServerSide.Characters;
using ServerSide.Server.Commands;
using ServerSide.Server.Commands.CommandAttributes;
using ServerSide.Server.Interfaces;
using ServerSide.Server.Vehicles.Util;
using ServerSide.Util;
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
            if (seatID == 0)
            {
                character.CurrentDrivingVehicle = vehicle;
                var ServerVehicle = GetServerVehicle(character.CurrentDrivingVehicle);
                if (ServerVehicle != null)
                {
                    player.TriggerEvent("SERVER:CLIENT:UpdateFuelAndDistance", ServerVehicle.Distance, ServerVehicle.Gas);
                }
            }
        }
        [ServerEvent(Event.PlayerExitVehicle)]
        private void PlayerExitVehicleEvent(Player player, GTANetworkAPI.Vehicle vehicle)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            character.CurrentDrivingVehicle = null;
            var ServerVehicle = GetServerVehicle(vehicle);
            ServerVehicle.Position = player.Position;
        }
        public static Vehicle GetServerVehicle(GTANetworkAPI.Vehicle vehicle)
        {
            var ServerVehicle = Vehicles
                .FirstOrDefault(v => v.GTAVehicle == vehicle) as Vehicle;

            // If the result is null, ServerVehicle will also be null.
            return ServerVehicle;
        }

        [RemoteEvent("CLIENT:SERVER:UpdateCarPosition")]
        private void UpdateCarPositionEvent(Player player, double distance)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            var vehicle = GetServerVehicle(character.CurrentDrivingVehicle);
            vehicle.Distance += distance;
            vehicle.GasDistance += distance;
            //vehicle.Position = player.Position;
            if (vehicle.GasDistance > 300)
            {
                vehicle.Gas -= 0.005f;
                vehicle.GasDistance = 0;
                //player.SendChatMessage(vehicle.Gas.ToString());
                player.TriggerEvent("SERVER:CLIENT:UpdateFuelAndDistance", vehicle.Distance, vehicle.Gas);
            }
        }

        [Command("vw")]
        private void vw_cmd(Player player, string window)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.CurrentDrivingVehicle == null || player.VehicleSeat != 0)
            {
                player.SendChatMessage($"{Colors.ERROR}Nu esti intr-o masina");
                return;
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
            character.SendChatMessage(Colors.INFO + "Ai primit un nou vehicul de tipul: " + NAPI.Vehicle.GetVehicleDisplayName(vehicle.Model));
        }
        [RequiresAdmin]
        [Command("servervehicles")]
        private void ServerVehiclesCommand(Player player, double speed = 0)
        {
            foreach(var veh in VehicleInfos)
            {
                if (veh.MaxSpeed > speed)
                    player.SendChatMessage($"Name: {veh.VehicleName}, Speed: {veh.MaxSpeed}, Breaking: {veh.MaxBraking}");
            }
        }
        [RemoteEvent("CLIENT:SERVER:TurnEngine")]
        [Command("engine")]
        private void EngineCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if(character.CurrentDrivingVehicle == null || player.VehicleSeat != 0)
            {
                player.SendChatMessage($"{Colors.ERROR}Nu esti intr-o masina");
                return;
            }
            if(player.VehicleSeat == 0)
            {
                bool engineStatus = player.Vehicle.EngineStatus;
                if(engineStatus == false)
                {
                    player.Vehicle.EngineStatus = true;
                    ChatMessage.ameCommand(player, "a pornit motorul.");
                }
                else
                {
                    player.Vehicle.EngineStatus = false;
                    ChatMessage.ameCommand(player, "a oprit motorul.");
                }
            }
        }
        [Command("lights")]
        private void LightsCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.CurrentDrivingVehicle == null || player.VehicleSeat != 0)
            {
                player.SendChatMessage($"{Colors.ERROR}Nu esti intr-o masina");
                return;
            }
            if (player.VehicleSeat == 0)
            {
                bool lightsStatus = player.Vehicle.SpecialLight;
                if (lightsStatus == false)
                {
                    player.Vehicle.SpecialLight = true;
                }
                else
                {
                    player.Vehicle.SpecialLight = false;
                }
            }
        }
        [Command("vehicles")]
        private void VehiclesCommand(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];

            int cont = 1;
            player.SendChatMessage("Masinile tale: ");
            foreach (var vehicle in character.PersonalVehicles)
            {
                player.SendChatMessage($"{cont}. {vehicle.VehicleId} {vehicle.GTAVehicle.DisplayName}");
                cont++;
            }
        }
    }
}
