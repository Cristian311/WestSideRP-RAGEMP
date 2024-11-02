using ServerSide.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace ServerSide.Account
{
    public class Account : IAccount
    {

        public string Name { get; }
        public string Password { get; }
        public string Email { get; } = "";
        public Player player { get; }
        List<ICharacter> Characters;

        public Account(string name, string password, string email, Player player)
        {
            Name = name;
            Password = password;
            Email = email;
            this.player = player;

        }
    }
}
