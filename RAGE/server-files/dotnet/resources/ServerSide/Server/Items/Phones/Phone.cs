using ServerSide.Characters;
using ServerSide.Server.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide
{
    public class Phone : Item
    {
        public List<PhoneMessage> messages = new List<PhoneMessage>();
        public new int PhoneNumber { get; set; }
        public Phone(Character character, string Name, string Type, int phoneNumber, int Amount = 1, bool Countable = false, string UsedFor = "")
            : base(character, Name, Type, Amount, Countable)
        {
            PhoneNumber = phoneNumber;
            base.PhoneNumber = phoneNumber;
        }
    }
}
