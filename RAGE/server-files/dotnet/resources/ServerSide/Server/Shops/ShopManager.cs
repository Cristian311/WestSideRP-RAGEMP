using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Server.Commands.CommandAttributes;
using ServerSide.Server.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using Color = GTANetworkAPI.Color;

namespace ServerSide.Server.Shops
{
    internal class ShopManager : Script
    {
        public static List<Shop> shops = new List<Shop>();
        public static List<ColShape> shopColshapes = new List<ColShape>();
        private static IdManager<ShopData> shopIdManager = new IdManager<ShopData>();
        private static string filePath = "json/shops/shops.json";

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
                    ServerLogger.Log($"Loaded shop: {shopData.Name} at position: {position}");

                    ColShape colShape = NAPI.ColShape.CreateCylinderColShape(position, 1f, 1f, NAPI.GlobalDimension);
                    TextLabel textLabel = NAPI.TextLabel.CreateTextLabel("Press Y to open the shop", position, 5f, 1f, 0, new Color(0, 250, 0), true, NAPI.GlobalDimension);
                    Shop shop = new Shop(shopData.Name, colShape, textLabel, position)
                    {
                        items = shopData.items ?? new List<ShopItem>()
                    };
                    shops.Add(shop);
                    shopColshapes.Add(colShape);
                }
            }
        }

        public static void SaveShopsToFile()
        {
            // Convert shops to ShopData for serialization
            List<ShopData> shopDataList = shops.ConvertAll(s => new ShopData(
                s.Name,
                s.Position,
                s.items ?? new List<ShopItem>()
            ));

            // Serialize and save to file
            string updatedJson = NAPI.Util.ToJson(shopDataList);
            File.WriteAllText(filePath, updatedJson);

            ServerLogger.Log($"Saved Shops with Items: {updatedJson}");

        }

        [RequiresAdmin]
        [Command("shops")]
        private void ShopsCommand(Player player)
        {
            int cont = 0;
            foreach (var shop in shops)
            {
                cont++;
                string json = NAPI.Util.ToJson(shop.items);
                player.SendChatMessage($"{cont}. {shop.Name} - {json}");
            }
        }

        [Command("copyshop")]
        private void CopyShopCommand(Player player, string name)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            ServerLogger.LogWithAccount(player, " copied the shop " + name + $" to {player.Position.ToString()}");
        }
        [RequiresAdmin]
        [Command("createshop")]
        private void CreateShopCommand(Player player, string name)
        {
            Console.WriteLine($"Creating shop: {name} at position: {player.Position}");

            ColShape colShape = NAPI.ColShape.CreateCylinderColShape(player.Position, 1f, 1f, NAPI.GlobalDimension);
            TextLabel textLabel = NAPI.TextLabel.CreateTextLabel("Press Y to open the shop", player.Position, 5f, 1f, 0, new Color(0, 250, 0), true, NAPI.GlobalDimension);
            Shop shop = new Shop(name, colShape, textLabel, player.Position);

            shops.Add(shop);
            shopColshapes.Add(colShape);

            ShopData shopData = new ShopData(name, player.Position, null);

            List<ShopData> existingShops = new List<ShopData>();
            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                existingShops = NAPI.Util.FromJson<List<ShopData>>(existingJson);
            }

            existingShops.Add(shopData);

            string updatedJson = NAPI.Util.ToJson(existingShops);
            File.WriteAllText(filePath, updatedJson);

            int shopId = shopIdManager.AssignNewId(shopData);
            shopData.Id = shopId;
            ServerLogger.LogWithAccount(player, $" a creat shopul {name} ID: {shopData.Id}");
        }
    }
}