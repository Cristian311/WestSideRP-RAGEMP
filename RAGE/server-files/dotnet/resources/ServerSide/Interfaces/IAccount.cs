using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Interfaces
{
    internal interface IAccount
    {
        int Id { get; }
        string Name { get; }
        string Password { get; }
        string Email { get; }
        Player player { get; }

    }
}
