using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Items
{
    public class Item
    {
        private static int ItemsCounter = 0;
        public int Id {  get; }
        public int localID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public bool Countable { get; }

        public Item(string Name, string Type, int Amount, bool Countable)
        {
            Id = ItemsCounter;
            ItemsCounter++;
            this.Name = Name;
            this.Type = Type;
            this.Amount = Amount;
            this.Countable = Countable;
        }
    }
}
