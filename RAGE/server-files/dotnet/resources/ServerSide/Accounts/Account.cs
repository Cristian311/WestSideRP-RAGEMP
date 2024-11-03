using ServerSide.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace ServerSide.Accounts
{
    public class Account : IAccount
    {
        public int Id { get; }
        public string Name { get; }
        public string Password { get; }
        public string Email { get; } = "";
        public Player player { get; }
        public int AdminLevel { get; set; }

        List<ICharacter> Characters;

        public Account(int id, string name, Player player)
        {
            Id = id;
            Name = name;
            this.player = player;

        }
    }
}
