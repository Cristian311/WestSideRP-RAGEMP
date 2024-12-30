using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServerSide.Server.Vehicles.Util
{
    public class VehicleInfo
    {
        public string VehicleName { get; set; }
        public double MaxSpeed { get; set; }
        public double MaxBraking { get; set; }
        public double MaxTraction { get; set; }
        public double MaxAcceleration { get; set; }
        public int MaxNumberOfPassengers { get; set; }
        public int MaxOccupants { get; set; }
        public int VehicleClass { get; set; }
    }

    public class VehicleLoader
    {
        public List<VehicleInfo> Vehicles { get; private set; }

        // Private constructor to initialize the list
        private VehicleLoader(List<VehicleInfo> vehicles)
        {
            Vehicles = vehicles ?? throw new ArgumentNullException(nameof(vehicles));
        }

        // Static async factory method to handle file loading
        public static async Task<VehicleLoader> CreateAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file was not found.", filePath);
            }

            var json = await File.ReadAllTextAsync(filePath);
            var vehicles = JsonConvert.DeserializeObject<List<VehicleInfo>>(json)
                ?.Where(v => !string.IsNullOrEmpty(v.VehicleName)).ToList();

            return new VehicleLoader(vehicles);
        }
    }
}
