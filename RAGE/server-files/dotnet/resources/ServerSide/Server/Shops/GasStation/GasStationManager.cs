using GTANetworkAPI;
using ServerSide.Characters;
using ServerSide.Server.Blips;
using ServerSide.Server.Commands.CommandAttributes;
using ServerSide.Server.Vehicles;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace ServerSide.Server.GasStations
{
    internal class GasStationManager : Script
    {
        public static List<Vector3> gasStations = new List<Vector3>();
        private static string filePath = "json/shops/gasstations.json";

        // List to hold ColShape objects
        private static List<ColShape> gasStationColShapes = new List<ColShape>();

        public GasStationManager()
        {
            // Load gas stations from JSON file
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                gasStations = NAPI.Util.FromJson<List<Vector3>>(jsonString);

                foreach (Vector3 position in gasStations)
                {
                    // Create information marker
                    CreateGasStationMarker(position);

                    // Debugging output
                    Console.WriteLine($"Loaded gas station at position: {position}");
                }
            }
        }

        public static void SaveGasStationsToFile()
        {
            // Serialize and save gas station positions to file
            string updatedJson = NAPI.Util.ToJson(gasStations);
            File.WriteAllText(filePath, updatedJson);

            // Debugging output
            Console.WriteLine($"Saved Gas Stations: {updatedJson}");
        }

        [RequiresAdmin]
        [Command("addgasstation")]
        private void AddGasStationCommand(Player player)
        {
            Vector3 position = player.Position;

            // Add to runtime list
            gasStations.Add(position);

            // Save to file
            SaveGasStationsToFile();

            // Create in-game marker and ColShape
            CreateGasStationMarker(position);

            BlipHandler.CreateBlipCommand(player, "Benzinarie", 361, 4);
            // Notify the player
            player.SendChatMessage($"Gas station added at your current position: {position}");
        }
        [Command("fuel")]
        private void FuelCommand(Player player, string percent)
        {
            if (float.TryParse(percent, out float percentage))
            {
                // Check if the percentage is within the valid range (0 to 100)
                if (percentage < 0 || percentage > 100)
                {
                    player.SendChatMessage($"{Colors.ERROR}Eroare: Introdu procent corect (0-100).");
                    return;
                }

                var character = CharacterManager.PlayerCharacterPair[player];

                // Check if the player is in a vehicle
                if (character.CurrentDrivingVehicle == null)
                {
                    player.SendChatMessage($"{Colors.ERROR}Eroare: Nu esti intr-o masina.");
                    return;
                }
                if (character.CurrentDrivingVehicle.EngineStatus == true)
                {
                    player.SendChatMessage($"{Colors.ERROR}Opreste motorul.");
                    return;
                }
                // Get the vehicle from the VehicleManager
                var ServerVehicle = VehicleManager.GetServerVehicle(character.CurrentDrivingVehicle);

                // Ensure that the vehicle has a gas tank capacity
                if (ServerVehicle.GasTank <= 0)
                {
                    player.SendChatMessage($"{Colors.ERROR}Eroare: Vehiculul nu are un rezervor valid.");
                    return;
                }

                // Calculate the target amount of gas based on the percentage
                float targetGas = (ServerVehicle.GasTank * percentage) / 100;

                // Calculate how much gas needs to be added
                float gasNeeded = targetGas - ServerVehicle.Gas;

                if (gasNeeded < 0)
                {
                    player.SendChatMessage($"{Colors.ERROR}Eroare: Rezervorul este deja plin sau peste {percentage}%.");
                    return;
                }

                // Calculate the cost for refueling
                float cost = gasNeeded * 100; // $100 per liter

                // Refuel the vehicle by adding gas
                ServerVehicle.Gas += gasNeeded;

                // Make sure we don't exceed the tank's capacity
                if (ServerVehicle.Gas > ServerVehicle.GasTank)
                {
                    ServerVehicle.Gas = ServerVehicle.GasTank;
                }

                // Deduct the cost from the player's money (assuming the player has money available)
                if (character.Money < cost)
                {
                    player.SendChatMessage($"{Colors.ERROR}Eroare: Nu ai suficienti bani pentru a alimenta.");
                    return;
                }

                character.Money -= (int)Math.Round(cost);
                // Deduct money from the player's account

                // Notify the player about the refueling action and the cost
                player.SendChatMessage($"{Colors.BLUE}Ai incarcat {Colors.CYAN}{(int)Math.Floor(gasNeeded)}{Colors.BLUE} litri de combustibil. Cost total: {Colors.GREEN}${(int)Math.Floor(cost)}.");
                player.SendChatMessage($"{Colors.BLUE}Ramas: {Colors.GREEN}{character.Money}$.");

                // Optionally, save the player's data and vehicle status if necessary
                //CharacterManager.SavePlayerData(character);
            }
            else
            {
                player.SendChatMessage($"{Colors.ERROR}Eroare: Introdu un numar ca procent.");
            }
        }

        private void CreateGasStationMarker(Vector3 position)
        {
            // Create a ColShape for the gas station
            ColShape colShape = NAPI.ColShape.CreateCylinderColShape(position, 5f, 1f, NAPI.GlobalDimension);

            // Add the ColShape to the list for future reference
            gasStationColShapes.Add(colShape);

            // Create a text label for the gas station
            NAPI.TextLabel.CreateTextLabel("Foloseste /fuel [procent] ca sa bagi benzina", position, 15f, 1f, 0, new Color(0, 255, 255), true, NAPI.GlobalDimension);
        }

        // Optional: Add a method to get all the colshapes if needed
        public static List<ColShape> GetAllGasStationColShapes()
        {
            return gasStationColShapes;
        }
    }
}
