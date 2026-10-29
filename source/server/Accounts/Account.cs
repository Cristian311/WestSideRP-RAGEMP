using GTANetworkAPI;
using ServerSide.Characters;
using ServerSide.Server.DB.CharacterDB;
using ServerSide.Server.Interfaces;
using System.Collections.Generic;
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
        public bool AdminDuty { get; set; } = false;
        public List<Character> Characters { get; set; }
        public Character currentCharacter { get; set; }
        public bool Muted { get; set; }
        public bool DarkMode { get; set; } = true;
        public Account(int id, string name, Player player)
        {
            Id = id;
            Name = name;
            this.player = player;
            Characters = CharacterDbManager.GetCharacters(this);
        }
    }
}
