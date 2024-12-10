using GTANetworkAPI;
using ServerSide.Characters;
using ServerSide.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSide.Shops
{
    internal class ShopHandler : Script
    {
        [RemoteEvent("CLIENT:SERVER:ActionKeyPressed")]
        private void ActionKeyPressed(Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];

            //Normal Shop case
            if (ShopManager.shopColshapes.Contains(character.currentColshape))
            {
                Shop shop = ShopManager.shops.FirstOrDefault(s => s.colShape == character.currentColshape);
                string json = NAPI.Util.ToJson(shop.items);
                player.TriggerEvent("SERVER:CLIENT:OpenShop", json);
            }
        }
        [RemoteEvent("CLIENT:SERVER:SaveItemData")]
        private void SaveItemData(Player player, string json)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            var data = NAPI.Util.FromJson<List<ShopItem>>(json);
            Console.WriteLine(json);
            if (ShopManager.shopColshapes.Contains(character.currentColshape))
            {
                Shop shop = ShopManager.shops.FirstOrDefault(s => s.colShape == character.currentColshape);
                shop.items = data;

                Console.WriteLine(NAPI.Util.ToJson(shop.items));
            }
        }
    }
}
