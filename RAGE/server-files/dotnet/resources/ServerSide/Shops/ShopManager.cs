using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Color = GTANetworkAPI.Color;

namespace ServerSide.Shops
{
    internal class ShopManager : Script
    {
        public static List<Shop> shops = new List<Shop>();
        public static List<ColShape> shopColshapes = new List<ColShape>();
        private string filePath = "json/shops/shops.json";

        public ShopManager()
        {
            // Load existing shops from the JSON file
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                // Ensure to read the existing shops properly and convert them back from JSON
                List<ShopData> loadedShops = NAPI.Util.FromJson<List<ShopData>>(jsonString);

                foreach (ShopData shopData in loadedShops)
                {
                    // Convert back from SerializableVector3 to Vector3
                    Vector3 position = shopData.Position.ToVector3();

                    // Debugging output: check if position is deserialized correctly
                    Console.WriteLine($"Loaded shop: {shopData.Name} at position: {position}");

                    ColShape colShape = NAPI.ColShape.CreateCylinderColShape(position, 1f, 1f, NAPI.GlobalDimension);
                    TextLabel textLabel = NAPI.TextLabel.CreateTextLabel("Press Y to open the shop", position, 5f, 1f, 0, new Color(0, 250, 0), true, NAPI.GlobalDimension);
                    Shop shop = new Shop(shopData.Name, colShape, textLabel);
                    shops.Add(shop);
                    shopColshapes.Add(colShape);
                }
            }
        }

        [Command("createshop")]
        private void CreateShopCommand(Player player, string name)
        {
            // Ensure position is correct before saving
            Console.WriteLine($"Creating shop: {name} at position: {player.Position}");

            // Create the shop in-game
            ColShape colShape = NAPI.ColShape.CreateCylinderColShape(player.Position, 1f, 1f, NAPI.GlobalDimension);
            TextLabel textLabel = NAPI.TextLabel.CreateTextLabel("Press Y to open the shop", player.Position, 5f, 1f, 0, new Color(0, 250, 0), true, NAPI.GlobalDimension);
            Shop shop = new Shop(name, colShape, textLabel);

            // Add to runtime lists
            shops.Add(shop);
            shopColshapes.Add(colShape);

            // Create ShopData to save
            ShopData shopData = new ShopData(name, player.Position, null);
            List<ShopData> shopDataList = shops.ConvertAll(s => new ShopData(s.Name, s.colShape.Position, s.items));

            // Debugging output: Check what data will be saved
            foreach (var data in shopDataList)
            {
                Console.WriteLine($"Shop to save: {data.Name} at position: {data.Position.ToVector3()}");
            }

            // Read existing shop data from the file
            List<ShopData> existingShops = new List<ShopData>();
            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                existingShops = NAPI.Util.FromJson<List<ShopData>>(existingJson);
            }

            // Add the new shop to the existing list
            existingShops.Add(shopData);

            // Save the updated list back to the file
            string updatedJson = NAPI.Util.ToJson(existingShops);
            File.WriteAllText(filePath, updatedJson);

            // Debugging output to confirm the final saved JSON
            Console.WriteLine($"Saved Shops: {updatedJson}");
        }
    }

    // Class to hold shop data for saving/loading
    internal class ShopData
    {
        public string Name { get; set; }
        public SerializableVector3 Position { get; set; }
        public List<ShopItem> items { get; set; }
        public ShopData(string name, Vector3 position, List<ShopItem> items)
        {
            Name = name;
            Position = new SerializableVector3(position);
            this.items = items;
        }
    }

    // Helper class to serialize Vector3 to JSON
    public class SerializableVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public SerializableVector3(Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }

}
