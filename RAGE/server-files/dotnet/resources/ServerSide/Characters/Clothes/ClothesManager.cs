using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Characters.Clothes
{
    internal class ClothesManager : Script
    {
        [RemoteEvent("CLIENT:SERVER:ClothesUpdate")]
        private void ClothesUpdate(Player player, string json)
        {
            var clothesData = NAPI.Util.FromJson<ClothesData>(json);
            NAPI.Util.ConsoleOutput(json);
        }
    }
}
