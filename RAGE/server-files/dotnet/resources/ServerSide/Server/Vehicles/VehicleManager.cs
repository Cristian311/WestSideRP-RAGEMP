using GTANetworkAPI;
using Org.BouncyCastle.Bcpg;
using ServerSide.Characters;
using ServerSide.Server.Commands;
using ServerSide.Server.Commands.CommandAttributes;
using ServerSide.Server.DB.VehicleDB;
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
        public static IdManager<IVehicle> vehicleIdManager = new IdManager<IVehicle>();
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

        [ServerEvent(Event.PlayerEnterVehicle)]
        private void PlayerEnterVehicleEvent(Player player, GTANetworkAPI.Vehicle vehicle, sbyte seatID)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (seatID == 0)
            {
                character.CurrentDrivingVehicle = vehicle;
                var ServerVehicle = GetServerVehicle(character.CurrentDrivingVehicle);
                character.CurrentServerDrivingVehicle = ServerVehicle;
                if (ServerVehicle != null)
                {
                    player.TriggerEvent("SERVER:CLIENT:UpdateFuelAndDistance", ServerVehicle.Distance, ServerVehicle.Gas);
                }
                else
                {
                    player.SendChatMessage("DEV ERROR: SERVERVEHICLE NOT FOUND");
                }
            }
        }
        [ServerEvent(Event.PlayerExitVehicle)]
        private void PlayerExitVehicleEvent(Player player, GTANetworkAPI.Vehicle vehicle)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            character.CurrentDrivingVehicle = null;
            character.CurrentServerDrivingVehicle = null;
            var ServerVehicle = GetServerVehicle(vehicle);
            if (ServerVehicle == null) return;
            ServerVehicle.Position = player.Position;
        }
        public static Vehicle GetServerVehicle(GTANetworkAPI.Vehicle vehicle)
        {
            var ServerVehicle = Vehicles.FirstOrDefault(v => v.GTAVehicle == vehicle) as Vehicle;

            return ServerVehicle;
        }

        [RemoteEvent("CLIENT:SERVER:UpdateCarPosition")]
        private void UpdateCarPositionEvent(Player player, double distance)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            //var vehicle = GetServerVehicle(character.CurrentDrivingVehicle);
            var vehicle = character.CurrentServerDrivingVehicle;
            if (vehicle == null) return;
            vehicle.Distance += distance;
            vehicle.GasDistance += distance;

            if (vehicle.GasDistance > 300)
            {
                vehicle.Gas -= 0.005f;
                vehicle.GasDistance = 0;
                vehicle.Position = player.Position;
                //player.SendChatMessage(vehicle.Gas.ToString());
                player.TriggerEvent("SERVER:CLIENT:UpdateFuelAndDistance", vehicle.Distance, vehicle.Gas);
            }
        }
        [Command("vget", Alias = "vg")]
        private void vget_cmd(Player player, int index = -1)
        {
            if(index == -1)
            {
                player.SendChatMessage($"{Colors.RED}Alege un index corect al masinii.");
                VehiclesCommand(player);
                return;
            }
            var character = CharacterManager.GetCharacter(player);
            if(index >= character.PersonalVehicles.Count)
            {
                player.SendChatMessage($"{Colors.RED}Alege un index corect al masinii.");
                VehiclesCommand(player);
                return;
            }
            var vehicle = character.PersonalVehicles.FirstOrDefault(v => v.VehicleLocalId == index);
            if (vehicle.isSpawned)
            {
                player.SendChatMessage($"{Colors.RED}Acea masina este deja spawnata undeva pe harta.");
                return;
            }
            else
            {
                VehicleHash vehModel = NAPI.Util.VehicleNameToModel(vehicle.VehicleName);
                GTANetworkAPI.Vehicle GTACar = NAPI.Vehicle.CreateVehicle(vehModel, vehicle.Position, 0, 0, 0);
                vehicle.GTAVehicle = GTACar;
                vehicle.isSpawned = true;
                GTACar.NumberPlate = vehicle.VehiclePlate;
                Vehicles.Add(vehicle);

                player.TriggerEvent("SERVER:CLIENT:CreateBlipForCar", NAPI.Util.ToJson(vehicle.Position));
                player.SendChatMessage($"{Colors.DARK_YELLOW}Vehicului tau s-a spawnat. Ti-am pus un punct pe harta la locatia acesteia.");
            }
        }
        [Command("vpark", Alias ="vp")]
        private void vp_cmd(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.CurrentDrivingVehicle == null || player.VehicleSeat != 0)
            {
                character.SendErrorMessage("Nu esti intr-o masina");
                return;
            }

            var vehicle = GetServerVehicle(character.CurrentDrivingVehicle);
            vehicle.Position = player.Position;
            vehicle.isSpawned = false;
            VehicleDbManager.UpdateVehicle(vehicle);
            character.CurrentDrivingVehicle.Delete();
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
        [RequiresAdmin(4)]
        [Command("createpersonalvehicle", Alias = "cpv")]
        private void CreatePersonalVehicleCommand(Player player, string vehName)
        {

            var character = CharacterManager.PlayerCharacterPair[player];
            Console.WriteLine($"CreatePersonalVehicleCommand invoked by {character.Name}");

            VehicleHash vehModel = NAPI.Util.VehicleNameToModel(vehName);
            if(NAPI.Vehicle.GetVehicleDisplayName(vehModel) == string.Empty)
            {
                character.SendErrorMessage($"Nu exista acea masina!");
                return;
            }
            GTANetworkAPI.Vehicle vehicle = NAPI.Vehicle.CreateVehicle(vehModel, player.Position, 0, 0, 0);
            vehicle.Dimension = NAPI.GlobalDimension;
            // Initialize server vehicle
            Vehicle serverVeh = new Vehicle(character.Name, "Personal", vehicle);
            serverVeh.VehicleId = vehicleIdManager.AssignNewId(serverVeh);

            serverVeh.VehicleLocalId = character.VehicleIdManager.AssignNewId(serverVeh);
            
            serverVeh.Position = player.Position;
            serverVeh.VehicleName = vehicle.DisplayName;
            serverVeh.VehicleDisplayName = NAPI.Vehicle.GetVehicleDisplayName(vehModel);
            serverVeh.VehiclePlate = "Personal";
            serverVeh.isSpawned = true;

            Vehicles.Add(serverVeh);            
            character.PersonalVehicles.Add(serverVeh);
            character.AccessVehicles.Add(serverVeh);

            VehicleDbManager.SaveVehicle(serverVeh);

            //i dont know why it puts two vehicles in the character.PersonalVehicles instead of one but i ll brute force it to be normal
            //FixPersonalVehicles(character);

            character.SendChatMessage(Colors.INFO + "Ai primit un nou vehicul de tipul: " + NAPI.Vehicle.GetVehicleDisplayName(vehModel));
        }
        public static void FixPersonalVehicles(Character character)
        {
            character.PersonalVehicles.Clear();
            character.PersonalVehicles = VehicleDbManager.LoadVehiclesByName(character.Name);
            character.VehicleIdManager = new IdManager<IVehicle>();
            foreach(var veh in character.PersonalVehicles)
            {
                veh.VehicleLocalId = character.VehicleIdManager.AssignNewId(veh);
            }
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
                    ChatMessage.cmd_ame(player, "a pornit motorul.");
                }
                else
                {
                    player.Vehicle.EngineStatus = false;
                    ChatMessage.cmd_ame(player, "a oprit motorul.");
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
            player.SendChatMessage($"Ai {character.PersonalVehicles.Count.ToString()} vehicule. ");
            player.SendChatMessage("Masinile tale: ");
            foreach (var vehicle in character.PersonalVehicles)
            {
                string FuelString = vehicle is IGasVehicle ? "Benzina: " + (((IGasVehicle)vehicle).Gas / ((IGasVehicle)vehicle).GasTank * 100).ToString("F1") + "%" : "";
                player.SendChatMessage($"{vehicle.VehicleLocalId}. {vehicle.VehicleDisplayName}[{vehicle.VehicleId}] KM: {vehicle.Distance} {FuelString}");
            }
        }
    }
}
