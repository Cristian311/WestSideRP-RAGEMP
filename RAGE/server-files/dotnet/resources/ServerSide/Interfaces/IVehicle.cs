using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Server.Vehicles
{
    public interface IVehicle
    {
        string OwnerName { get; }
        string OwnerType { get; }
        string VehicleName { get; set; }
        string VehicleDisplayName { get; set; }
        int VehicleId { get; set; }
        int VehicleLocalId { get; set; }
        string VehiclePlate { get; set; }
        Vector3 Position { get; set; }
        DateTime InsuranceEnd { get; set; }
        GTANetworkAPI.Vehicle GTAVehicle { get; set; }
        bool isSpawned { get; set; }
    }
    public interface IGasVehicle : IVehicle
    {
        float Gas { get; set; }
        float GasTank { get; set; }
        double GasDistance { get; set; }
    }

    public interface IServiceVehicle : IVehicle
    {
        bool isPolice { get; set; }
        bool isAmbulance { get; set; }
        bool isTaxi { get; set; }
    }


}
