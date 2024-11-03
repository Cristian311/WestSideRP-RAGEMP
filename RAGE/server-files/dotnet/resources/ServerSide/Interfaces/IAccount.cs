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
        string Name { get; }
        string Password { get; }
        string Email { get; }
        Player player { get; }
        List<ICharacter> Characters { get; }
        Character currentCharacter { get; }
    }
}
