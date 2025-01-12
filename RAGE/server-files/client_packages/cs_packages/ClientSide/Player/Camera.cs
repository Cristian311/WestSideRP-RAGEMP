using RAGE;
using System;

namespace ClientSide.Player
{
    internal class Camera : Events.Script
    {
        private Camera()
        {
            Events.Add("SERVER:CLIENT:ToFaceCamera", ToFaceCamera);
        }

        private void ToFaceCamera(object[] args)
        {
            
        }
    }
}
