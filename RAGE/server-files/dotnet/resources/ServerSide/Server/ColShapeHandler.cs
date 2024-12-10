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
            var character = CharacterManager.PlayerCharacterPair[player];
            character.currentColshape = shape;
        }

        [ServerEvent(Event.PlayerExitColshape)]
        private void OnExitColshape(ColShape shape, Player player)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            character.currentColshape = null;
        }
    }
}
