using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Server.Commands.CommandAttributes
{
    public class RequiresFactionInvitation : CommandConditionAttribute
    {
        public RequiresFactionInvitation()
        {

        }

        public override bool Check(Player player, string cmdName, string cmdText)
        {
            var character = CharacterManager.PlayerCharacterPair[player];
            if(FactionManager.FactionInvitations.ContainsKey(character) == false)
            {
                character.SendErrorMessage("Nu esti invitat in nici o factiune.");
                return false;
            }
            return true;
        }
    }
}
