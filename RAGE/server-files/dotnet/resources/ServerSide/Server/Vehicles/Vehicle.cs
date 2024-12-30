using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace ServerSide.Server.Vehicles
{
    public class Vehicle : IVehicle
    {
        public string OwnerName { get; }
        public string OwnerType { get; }
        public int VehicleId { get; set; }
        public string VehiclePlate { get; set; }
        public double Distance { get; set; }
        public DateTime InsuranceEnd { get; set; } = DateTime.Now;
        public Vector3 Position { get; set; }
        public double Gas { get; set; } = 10.0f;
        public double GasDistance { get; set; }
        public GTANetworkAPI.Vehicle GTAVehicle { get; }
        public bool isPolice { get; set; }
        public bool isAmbulance { get; set; }
        public bool isTaxi { get; set; }

        public Vehicle(string ownerName, string ownerType,  GTANetworkAPI.Vehicle GTAVehicle)
        {
            OwnerName = ownerName;
            OwnerType = ownerType;
            this.GTAVehicle = GTAVehicle;
        }
    }
}
