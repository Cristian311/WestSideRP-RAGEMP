using GTANetworkAPI;
using ServerSide.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerSide.Shops
{
    internal class ShopHandler : Script
    {
        [RemoteEvent("CLIENT:SERVER:ActionKeyPressed")]
        private void ActionKeyPressed(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            // Normal Shop case
            if (ShopManager.shopColshapes.Contains(character.currentColshape))
            {
                Shop shop = ShopManager.shops.FirstOrDefault(s => s.colShape == character.currentColshape);
                if (shop != null)
                {
                    string json = NAPI.Util.ToJson(shop.items ?? new List<ShopItem>());
                    player.TriggerEvent("SERVER:CLIENT:OpenShop", json);
                }
            }
        }

        [RemoteEvent("CLIENT:SERVER:SaveItemData")]
        private void SaveItemData(Player player, string json)
        {
            var character = CharacterManager.PlayerCharacterPair[player];

            try
            {
                // Parse the JSON into a list of shop items
                var data = NAPI.Util.FromJson<List<ShopItem>>(json);

                // Find the shop in the current colshape
                if (ShopManager.shopColshapes.Contains(character.currentColshape))
                {
                    Shop shop = ShopManager.shops.FirstOrDefault(s => s.colShape == character.currentColshape);

                    if (shop != null)
                    {
                        // Update shop items
                        shop.items = data;

                        // Get the ShopManager instance and save to file
                        ShopManager.SaveShopsToFile();

                        // Log for debugging
                        Console.WriteLine($"Saved items for shop: {shop.Name}");
                        Console.WriteLine($"Items: {json}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during saving
                Console.WriteLine($"Error saving shop items: {ex.Message}");
                Console.WriteLine($"JSON received: {json}");
            }
        }
    }
}