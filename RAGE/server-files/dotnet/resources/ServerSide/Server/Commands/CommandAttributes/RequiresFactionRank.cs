using GTANetworkAPI;
using ServerSide.Accounts;
using ServerSide.Characters;
using ServerSide.Util;
using ServerSide.Util.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Server.Commands.CommandAttributes
{
    public class RequiresFactionRank : CommandConditionAttribute
    {
        public int Rank { get; set; }
        public RequiresFactionRank(int Rank = 3)
        {
            this.Rank = Rank;
        }

        public override bool Check(Player player, string cmdName, string cmdText)
        {
            var account = AccountManager.PlayerAccountPairs[player];
            if (account.AdminLevel > 0)
                return true;
            var character = CharacterManager.PlayerCharacterPair[player];
            if (character.factionRank < Rank)
            {
                player.SendChatMessage($"{Colors.COLOR_CHAT_ERROR}Nu ai grad pentru aceasta comanda.");
                return false;
            }
            return true;
        }
    }
}
