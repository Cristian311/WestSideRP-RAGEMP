using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Characters.Tattoos
{
    internal class TattooManager : Script
    {
        [Command("tattoo")]
        private void tattooCommand(Player player)
        {
            player.TriggerEvent("SERVER:CLIENT:OpenTattooMenu");
        }
    }
}
