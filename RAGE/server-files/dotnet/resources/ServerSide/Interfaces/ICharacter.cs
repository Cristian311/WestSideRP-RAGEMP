using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Interfaces
{
    internal interface ICharacter
    {
        IAccount Account { get; }
        int Id { get; }
        string Name { get; }
        int Age { get; }
        int Money { get; }
        

    }
}
