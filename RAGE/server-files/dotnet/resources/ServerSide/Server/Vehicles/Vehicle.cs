using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace ServerSide.Server.Vehicles
{
    public class Vehicle : IVehicle, IGasVehicle, IServiceVehicle
    {
        public string OwnerName { get; }
        public string OwnerType { get; }
        public string VehicleName { get; set; }
        public string VehicleDisplayName { get; set; }
        public int VehicleId { get; set; }
        public int VehicleLocalId { get; set; }
        public string VehiclePlate { get; set; }
        public double Distance { get; set; }
        public DateTime InsuranceEnd { get; set; } = DateTime.Now;
        public Vector3 Position { get; set; }
        public float Gas { get; set; } = 10.0f;
        public double GasDistance { get; set; }
        public float GasTank { get; set; } = 30;
        public GTANetworkAPI.Vehicle GTAVehicle { get; set; }
        public bool isPolice { get; set; }
        public bool isAmbulance { get; set; }
        public bool isTaxi { get; set; }
        public bool isSpawned { get; set; } = false;
        public Vehicle(string ownerName, string ownerType,  GTANetworkAPI.Vehicle GTAVehicle)
        {
            OwnerName = ownerName;
            OwnerType = ownerType;
            this.GTAVehicle = GTAVehicle;
        }
    }

    public class MountainBike : IVehicle
    {
        public string OwnerName { get; private set; }
        public string OwnerType { get; private set; }
        public string VehicleName { get; set; }
        public string VehicleDisplayName { get; set; }
        public int VehicleId { get; set; }
        public int VehicleLocalId { get; set; }
        public string VehiclePlate { get; set; }
        public double Distance { get; set; }
        public double GasDistance { get; set; } = 0; // Always 0 for non-gas vehicles
        public Vector3 Position { get; set; }
        public float Gas { get; set; } = 0; // Always 0 for non-gas vehicles
        public float GasTank { get; set; } = 0; // Always 0 for non-gas vehicles
        public bool isPolice { get; set; } = false; // Default to non-service vehicle
        public bool isAmbulance { get; set; } = false; // Default to non-service vehicle
        public bool isTaxi { get; set; } = false; // Default to non-service vehicle
        public DateTime InsuranceEnd { get; set; }
        public GTANetworkAPI.Vehicle GTAVehicle { get; set; }
        public bool isSpawned { get; set; } = false;
        

        public MountainBike(string ownerName, string ownerType, GTANetworkAPI.Vehicle gtaVehicle)
        {
            OwnerName = ownerName;
            OwnerType = ownerType;
            GTAVehicle = gtaVehicle;
        }
    }
}
