using GTANetworkAPI;
using MySql.Data.MySqlClient;
using ServerSide.Server.Vehicles;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Vehicle = ServerSide.Server.Vehicles.Vehicle;

namespace ServerSide.Server.DB.VehicleDB
{
    internal class VehicleDbManager
    {
        public static void SaveVehicle(IVehicle vehicle)
        {
            Database db = new Database();
            using (var connection = db.getDatabaseConnection())
            {
                connection.Open();

                string query = @"
                    INSERT INTO Vehicles 
                        (VehicleId, VehicleLocalId, VehicleName, VehicleDisplayName, OwnerName, OwnerType, VehiclePlate, 
                         PositionX, PositionY, PositionZ, InsuranceEnd, Distance, Gas, GasTank, GasDistance, 
                         IsPolice, IsAmbulance, IsTaxi)
                    VALUES 
                        (@VehicleId, @VehicleLocalId, @VehicleName, @VehicleDisplayName, @OwnerName, @OwnerType, @VehiclePlate, 
                         @PositionX, @PositionY, @PositionZ, @InsuranceEnd, @Distance, @Gas, @GasTank, @GasDistance, 
                         @IsPolice, @IsAmbulance, @IsTaxi)";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VehicleId", vehicle.VehicleId);
                    command.Parameters.AddWithValue("@VehicleLocalId", vehicle.VehicleLocalId);
                    command.Parameters.AddWithValue("@VehicleName", vehicle.VehicleName);
                    command.Parameters.AddWithValue("@VehicleDisplayName", vehicle.VehicleDisplayName);
                    command.Parameters.AddWithValue("@OwnerName", vehicle.OwnerName);
                    command.Parameters.AddWithValue("@OwnerType", vehicle.OwnerType);
                    command.Parameters.AddWithValue("@VehiclePlate", vehicle.VehiclePlate);
                    command.Parameters.AddWithValue("@PositionX", vehicle.Position.X);
                    command.Parameters.AddWithValue("@PositionY", vehicle.Position.Y);
                    command.Parameters.AddWithValue("@PositionZ", vehicle.Position.Z);
                    command.Parameters.AddWithValue("@InsuranceEnd", vehicle.InsuranceEnd);
                    command.Parameters.AddWithValue("@Distance", vehicle.Distance);
                    command.Parameters.AddWithValue("@Gas", vehicle is IGasVehicle gasVehicle ? gasVehicle.Gas : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@GasTank", vehicle is IGasVehicle ? ((IGasVehicle)vehicle).GasTank : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@GasDistance", vehicle is IGasVehicle ? ((IGasVehicle)vehicle).GasDistance : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsPolice", vehicle is IServiceVehicle ? ((IServiceVehicle)vehicle).isPolice : false);
                    command.Parameters.AddWithValue("@IsAmbulance", vehicle is IServiceVehicle ? ((IServiceVehicle)vehicle).isAmbulance : false);
                    command.Parameters.AddWithValue("@IsTaxi", vehicle is IServiceVehicle ? ((IServiceVehicle)vehicle).isTaxi : false);

                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        NAPI.Util.ConsoleOutput($"Vehicle {NAPI.Util.ToJson(vehicle)} updated successfully.");
                        return;
                    }
                    else
                    {
                        NAPI.Util.ConsoleOutput($"Failed to update vehicle {NAPI.Util.ToJson(vehicle)}.");
                        return;
                    }
                }
            }
        }
        public static void UpdateVehicle(IVehicle vehicle)
        {
            Database db = new Database();
            using (var connection = db.getDatabaseConnection())
            {
                connection.Open();

                string query = @"
            UPDATE Vehicles SET
                VehicleLocalId = @VehicleLocalId,
                VehicleName = @VehicleName,
                VehicleDisplayName = @VehicleDisplayName,
                OwnerName = @OwnerName,
                OwnerType = @OwnerType,
                VehiclePlate = @VehiclePlate,
                PositionX = @PositionX,
                PositionY = @PositionY,
                PositionZ = @PositionZ,
                InsuranceEnd = @InsuranceEnd,
                Distance = @Distance,
                Gas = @Gas,
                GasTank = @GasTank,
                GasDistance = @GasDistance,
                IsPolice = @IsPolice,
                IsAmbulance = @IsAmbulance,
                IsTaxi = @IsTaxi
            WHERE VehicleId = @VehicleId";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VehicleId", vehicle.VehicleId);
                    command.Parameters.AddWithValue("@VehicleLocalId", vehicle.VehicleLocalId);
                    command.Parameters.AddWithValue("@VehicleName", vehicle.VehicleName);
                    command.Parameters.AddWithValue("@VehicleDisplayName", vehicle.VehicleDisplayName);
                    command.Parameters.AddWithValue("@OwnerName", vehicle.OwnerName);
                    command.Parameters.AddWithValue("@OwnerType", vehicle.OwnerType);
                    command.Parameters.AddWithValue("@VehiclePlate", vehicle.VehiclePlate);
                    command.Parameters.AddWithValue("@PositionX", vehicle.Position.X);
                    command.Parameters.AddWithValue("@PositionY", vehicle.Position.Y);
                    command.Parameters.AddWithValue("@PositionZ", vehicle.Position.Z);
                    command.Parameters.AddWithValue("@InsuranceEnd", vehicle.InsuranceEnd);
                    command.Parameters.AddWithValue("@Distance", vehicle.Distance);
                    command.Parameters.AddWithValue("@Gas", vehicle is IGasVehicle ? ((IGasVehicle)vehicle).Gas : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@GasTank", vehicle is IGasVehicle ? ((IGasVehicle)vehicle).GasTank : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@GasDistance", vehicle is IGasVehicle ? ((IGasVehicle)vehicle).GasDistance : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsPolice", vehicle is IServiceVehicle ? ((IServiceVehicle)vehicle).isPolice : false);
                    command.Parameters.AddWithValue("@IsAmbulance", vehicle is IServiceVehicle ? ((IServiceVehicle)vehicle).isAmbulance : false);
                    command.Parameters.AddWithValue("@IsTaxi", vehicle is IServiceVehicle ? ((IServiceVehicle)vehicle).isTaxi : false);

                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        NAPI.Util.ConsoleOutput($"Vehicle {vehicle.VehicleName} ID: {vehicle.VehicleId} updated successfully.");
                    }
                    else
                    {
                        NAPI.Util.ConsoleOutput($"Failed to update vehicle {vehicle.VehicleName} ID:{vehicle.VehicleId}.");
                    }
                }
            }
        }

        public static List<IVehicle> LoadVehiclesByName(string ownerName)
        {
            var vehicles = new List<IVehicle>();
            Database db = new Database();

            using (var connection = db.getDatabaseConnection())
            {
                connection.Open();

                string query = "SELECT * FROM Vehicles WHERE OwnerName = @OwnerName";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OwnerName", ownerName);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var veh = MapReaderToVehicle(reader);
                            vehicles.Add(veh);
                            VehicleManager.vehicleIdManager.AssignNewId(veh);
                        }
                    }
                }
            }
            
            return vehicles;
        }
        private static IVehicle MapReaderToVehicle(MySqlDataReader reader)
        {
            string ownerName = reader.GetString("OwnerName");
            string ownerType = reader.GetString("OwnerType");
            string vehicleName = reader.GetString("VehicleName");
            string vehicleDisplayName = reader.GetString("VehicleDisplayName");
            int vehicleId = reader.GetInt32("VehicleId");
            string vehiclePlate = reader.GetString("VehiclePlate");
            Vector3 position = new Vector3(reader.GetFloat("PositionX"), reader.GetFloat("PositionY"), reader.GetFloat("PositionZ"));
            DateTime insuranceEnd = reader.GetDateTime("InsuranceEnd");

            if (!reader.IsDBNull(reader.GetOrdinal("Gas"))) // If Gas is not NULL, it's a gas vehicle
            {
                return new Vehicle(ownerName, ownerType, null)
                {
                    VehicleId = vehicleId,
                    VehiclePlate = vehiclePlate,
                    VehicleName = vehicleName,
                    VehicleDisplayName = vehicleDisplayName,
                    Position = position,
                    InsuranceEnd = insuranceEnd,
                    Distance = reader.GetDouble("Distance"),
                    Gas = reader.GetFloat("Gas"),
                    GasTank = reader.GetFloat("GasTank"),
                    GasDistance = reader.GetDouble("GasDistance"),
                    isPolice = reader.GetBoolean("IsPolice"),
                    isAmbulance = reader.GetBoolean("IsAmbulance"),
                    isTaxi = reader.GetBoolean("IsTaxi")
                };
            }
            else // Otherwise, it's a non-gas vehicle, e.g., Mountain Bike
            {
                return new MountainBike(ownerName, ownerType, null)
                {
                    VehicleId = vehicleId,
                    VehiclePlate = vehiclePlate,
                    Position = position,
                    InsuranceEnd = insuranceEnd
                };
            }
        }

    }
}
