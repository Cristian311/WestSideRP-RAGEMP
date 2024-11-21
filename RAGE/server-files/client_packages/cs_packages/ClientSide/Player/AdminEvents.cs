using RAGE;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Player
{
    internal class AdminEvents : Events.Script
    {
        public AdminEvents() 
        {
            Events.Add("SERVER:CLIENT:FreezePlayer", FreezePlayer);
        }

        private void FreezePlayer(object[] args)
        {
        }
    }
}
