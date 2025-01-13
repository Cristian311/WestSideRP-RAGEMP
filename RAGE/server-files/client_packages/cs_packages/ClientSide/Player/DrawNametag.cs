using RAGE.Elements;
using RAGE.Game;
using RAGE.NUI;
using RAGE;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Linq;

namespace ClientSide.Player
{
    internal class DrawNametag : Events.Script
    {
        private float screenX, screenY;
        private bool TogLocalNametag = false;
        private DrawNametag()
        {
            Events.Add("SERVER:CLIENT:TogNametag", TogNametag);

            Events.Tick += Tick;
            int x = 1600, y = 900;
            RAGE.Game.Graphics.GetActiveScreenResolution(ref x, ref y);
            screenX = float.Parse(x.ToString());
            screenY = float.Parse(y.ToString());
        }

        private void TogNametag(object[] args)
        {
            if (TogLocalNametag)
                TogLocalNametag = false;
            else
                TogLocalNametag = true;
        }

        public void Tick(List<Events.TickNametagData> nametags)
        {
            foreach (var player in Entities.Players.Streamed)
            {
                if (player == RAGE.Elements.Player.LocalPlayer && TogLocalNametag == false)
                    break;
                /* Variables */
                float _screenX = 0;
                float _screenY = 0;

                /* Player position */
                Vector3 position = player.GetBoneCoords(31086, 0, 0, 0);

                /* world3dToScreen2d */
                if (Graphics.GetScreenCoordFromWorldCoord(position.X, position.Y, position.Z + 0.60f, ref _screenX, ref _screenY))
                {
                    int newlineCount = player.Name.Count(c => c == '\n');
                    newlineCount = newlineCount > 0 ? newlineCount + 1 : 1;
                    UIResText.Draw(player.Name, (int)(screenX * _screenX + 100 ), (int)(screenY * _screenY + 100 - (newlineCount * 30)), Font.ChaletComprimeCologne, 0.4f, Color.White, RAGE.NUI.UIResText.Alignment.Centered, true, true, 0);
                }
            }
        }
    }
}
