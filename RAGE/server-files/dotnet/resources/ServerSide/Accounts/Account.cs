using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using ServerSide.Characters;
using ServerSide.Server.Interfaces;
namespace ServerSide.Accounts
{
    public class Account : IAccount
    {
        public int Id { get; }
        public int GameId { get; set; }
        public string Name { get; }
        public string Password { get; }
        public string Email { get; } = "";
        public Player player { get; }
        public int AdminLevel { get; set; }
        public List<ICharacter> Characters { get; set; }
        public Character currentCharacter { get; set; }
        public bool Muted { get; set; }
        public Account(int id, string name, Player player)
        {
            Id = id;
            Name = name;
            this.player = player;

        }
    }
}
