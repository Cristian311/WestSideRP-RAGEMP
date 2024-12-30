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
        int VehicleId { get; set; }
        string VehiclePlate { get; set; }
        double Distance { get; set; }
        double GasDistance { get; set; }
        Vector3 Position { get; set; }
        float Gas {  get; set; }
        float GasTank { get; set; }
        bool isPolice { get; set; }
        bool isAmbulance { get; set; }
        bool isTaxi { get; set; }

        DateTime InsuranceEnd { get; set; }
        GTANetworkAPI.Vehicle GTAVehicle { get; }
    }

}
