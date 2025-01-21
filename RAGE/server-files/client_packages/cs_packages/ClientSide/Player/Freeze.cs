using RAGE;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Player 
{
    internal class Freeze : Events.Script
    {
        private Freeze()
        {
            Events.Add("SERVER:CLIENT:FreezePlayer", FreezePlayer);
            Events.Add("SERVER:CLIENT:UnfreezePlayer", UnfreezePlayer);
        }

        private void UnfreezePlayer(object[] args)
        {
            RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
        }

        private void FreezePlayer(object[] args)
        {
            RAGE.Elements.Player.LocalPlayer.FreezePosition(true);
        }
    }
}
