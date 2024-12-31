using RAGE;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Player
{
    internal class Detections : Events.Script
    {
        Detections()
        {
            Events.Tick += Tick;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if(RAGE.Elements.Player.LocalPlayer.IsFalling() && RAGE.Elements.Player.LocalPlayer.IsRagdoll())
            {
                Events.CallRemote("CLIENT:SERVER:FallDetected");
            }
        }
    }
}
