using GTANetworkAPI;
using ServerSide.Characters;
using System.Collections.Generic;

namespace ServerSide.Server.Interfaces
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
        List<Character> Characters { get; set; }
        Character currentCharacter { get; set; }
    }
}
