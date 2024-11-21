using GTANetworkAPI;
using ServerSide.Characters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Interfaces
{
    public interface IAccount
    {
        int Id { get; }
        int GameId { get; set; }
        string Name { get; }
        string Password { get; }
        string Email { get; }
        int AdminLevel { get; set; }
        Player player { get; }
        List<ICharacter> Characters { get; set; }
        Character currentCharacter { get; set; }
    }
}
