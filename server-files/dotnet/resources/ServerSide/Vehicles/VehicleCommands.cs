using GTANetworkAPI;
using ServerSide.Server.Commands.CommandAttributes;
using System.Collections.Generic;
namespace ServerSide.Server.Vehicles
{
    internal class VehicleCommands : Script
    {
        [RequiresAdmin]
        [Command("fetchvehicledata")]
        public async void FetchVehicleDataAsync(Player player)
        {
            List<VehicleData> vehData = new List<VehicleData>();
            GetVehicleInfo vehicle = new GetVehicleInfo();

            List<string> vehiclenames = await vehicle.ReturnVehicleNamesAsync(player);

            player.SendChatMessage("Fetch complete, getting related vehicle info..");

            foreach (string v in vehiclenames)
            {
                uint id = NAPI.Util.GetHashKey(v);

                vehData.Add(new VehicleData()
                {
                    VehicleName = NAPI.Vehicle.GetVehicleDisplayName((VehicleHash)id),
                    MaxSpeed = NAPI.Vehicle.GetVehicleMaxSpeed((VehicleHash)id),
                    MaxBraking = NAPI.Vehicle.GetVehicleMaxBraking((VehicleHash)id),
                    MaxTraction = NAPI.Vehicle.GetVehicleMaxTraction((VehicleHash)id),
                    MaxAcceleration = NAPI.Vehicle.GetVehicleMaxAcceleration((VehicleHash)id),
                    MaxNumberOfPassengers = NAPI.Vehicle.GetVehicleMaxPassengers((VehicleHash)id),
                    MaxOccupants = NAPI.Vehicle.GetVehicleMaxOccupants((VehicleHash)id),
                    VehicleClass = NAPI.Vehicle.GetVehicleClass((VehicleHash)id)
                });
            }
            vehicle.WriteToFile(@"output/vehicleData.json", NAPI.Util.ToJson(vehData));
            player.SendChatMessage("vehicleData.json exported, check output folder in your server directory");
        }
    }
}
