using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace ServerSide.Server.Vehicles
{
    public class Vehicle
    {
        public string OwnerName { get; }

        public string OwnerType { get; }
        public int VehicleId { get; set; }
        public string VehiclePlate { get; set; }
        public double Distance { get; set; }
        public DateTime InsuranceEnd { get; set; }

        public GTANetworkAPI.Vehicle GTAVehicle { get; }

        public Vehicle(string ownerName, string ownerType,  GTANetworkAPI.Vehicle vehicleHash)
        {
            OwnerName = ownerName;
            OwnerType = ownerType;
            GTAVehicle = vehicleHash;
        }
    }
}
