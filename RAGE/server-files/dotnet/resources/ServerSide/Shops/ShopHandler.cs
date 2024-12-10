using GTANetworkAPI;
using ServerSide.Characters;
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
            if (ShopManager.shopColshapes.Contains(character.currentColshape))
            {
                Shop shop = ShopManager.shops.FirstOrDefault(s => s.colShape == character.currentColshape);
                player.TriggerEvent("SERVER:CLIENT:OpenShop");
            }
        }
    }
}
