using RAGE;
using System;
using System.Collections.Generic;

namespace ClientSide.Player
{
    internal class FallDetection : Events.Script
    {
        private DateTime fallStartTime;
        private bool isFalling;

        FallDetection()
        {
            Events.Tick += Tick;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (RAGE.Elements.Player.LocalPlayer.IsFalling() && RAGE.Elements.Player.LocalPlayer.IsRagdoll())
            {
                if (!isFalling)
                {
                    fallStartTime = DateTime.UtcNow; // Record the fall start time
                    isFalling = true;
                }
                else
                {
                    TimeSpan fallDuration = DateTime.UtcNow - fallStartTime;

                    if (fallDuration.TotalMilliseconds > 3000)
                    {
                        Events.CallRemote("CLIENT:SERVER:FallDetected");
                        isFalling = false; // Reset the flag after reporting
                    }
                }
            }
            else
            {
                isFalling = false; // Reset the flag if not falling
            }
        }
    }
}
