using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ServerSide.Characters
{
    internal class CharacterCustomise : Script
    {

        [RemoteEvent("CLIENT:SERVER:CharCustomiseUpdate")]
        private void onCharCustomiseUpdate(Player player, string json)
        {
            JsonSerializer.Deserialize<CharacterData>(json);
            NAPI.Util.ConsoleOutput(json);
        }
    }
}
