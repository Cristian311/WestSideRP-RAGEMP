using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerSide.Server.Shops
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
                    var account = AccountManager.PlayerAccountPairs[player];
                    bool check = account.AdminLevel > 0 ? true : false;
                    player.TriggerEvent("SERVER:CLIENT:OpenShop", json, check);
                }
            }
        }
        [RemoteEvent("CLIENT:SERVER:BuyShopItems")]
        private void BuyShopItems(Player player, string json)
        {
            var character = CharacterManager.GetCharacter(player);

            var cart = NAPI.Util.FromJson<Cart>(json);
            //player.SendChatMessage(cart.PaymentMethod);
            if (cart.PaymentMethod.Equals("cash"))
            {
                if (character.RemoveMoney((int)cart.TotalCost))
                    character.SendInfoMessage($"Ai cumparat de la magazin pentru suma de {Colors.GREEN}{(int)cart.TotalCost}${Colors.INFO}. Bani cash ramasi: {Colors.GREEN}{character.Money}$.");
                else
                {
                    character.SendErrorMessage("N-ai destui bani cash pentru asta.");
                    return;
                } 
            }
            else if (cart.PaymentMethod.Equals("card"))
            {
                if (character.RemoveBankMoney((int)cart.TotalCost))
                    character.SendInfoMessage($"Ai cumparat de la magazin pentru suma de {Colors.GREEN}{(int)cart.TotalCost}${Colors.INFO}. Bani card ramasi: {Colors.GREEN}{character.BankMoney}$.");
                else
                {
                    character.SendErrorMessage("N-ai destui bani in banca pentru asta.");
                    return;
                }
            }
            foreach(var item in cart.Items)
            {
                var charItem = new Items.Item(character, item.Name, item.Type, item.Quantity, true);
                charItem.localID = character.ItemIdManager.AssignNewId(charItem);
                character.Items.Add(charItem);
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