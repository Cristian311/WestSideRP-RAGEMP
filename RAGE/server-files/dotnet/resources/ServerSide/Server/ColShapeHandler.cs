using GTANetworkAPI;
using ServerSide.Characters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Server
{
    internal class ColShapeHandler : Script
    {
        [ServerEvent(Event.PlayerEnterColshape)]
        private void OnEnterColshape(ColShape shape, Player player)
        {

            var character = CharacterManager.GetCharacter(player);
            if (character == null) return;
            character.currentColshape = shape;
        }

        [ServerEvent(Event.PlayerExitColshape)]
        private void OnExitColshape(ColShape shape, Player player)
        {
            var character = CharacterManager.GetCharacter(player);
            if (character == null) return;
            character.currentColshape = null;
        }
    }
}
