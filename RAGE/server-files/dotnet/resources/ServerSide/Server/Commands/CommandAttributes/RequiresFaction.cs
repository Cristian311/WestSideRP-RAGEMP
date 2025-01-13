using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Server.Commands.CommandAttributes
{
    public class RequiresFaction : CommandConditionAttribute
    {
        public RequiresFaction()
        {

        }

        public override bool Check(Player player, string cmdName, string cmdText)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.faction == -1)
            {
                player.SendChatMessage($"{Colors.ERROR}Nu esti intr-o factiune.");
                character.factionRank = 0;
                return false;
            }
            return true;
        }
    }
}
