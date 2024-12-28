using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace ServerSide.Server.Vehicles
{
    public class Vehicle
    {
        public string OwnerName { get; }
        public string Name { get; }
        public int Color1 { get; set; }
        public int Color2 { get; set; }

        public string OwnerType { get; }
        public int VehicleId { get; set; }
        public string VehiclePlate { get; set; }
        public GTANetworkAPI.Vehicle vehicle { get; }

        public Vehicle(string ownerName, string ownerType,  GTANetworkAPI.Vehicle vehicleHash)
        {
            OwnerName = ownerName;
            OwnerType = ownerType;
            vehicle = vehicleHash;
            Color1 = vehicle.PrimaryColor;
            Color2 = vehicle.SecondaryColor;

        }
    }
}
