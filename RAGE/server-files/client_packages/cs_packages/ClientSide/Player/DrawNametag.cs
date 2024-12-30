using RAGE.Elements;
using RAGE.Game;
using RAGE.NUI;
using RAGE;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ClientSide.Player
{
    internal class DrawNametag : Events.Script
    {
        float screenX, screenY;
        private DrawNametag()
        {
            Events.Tick += Tick;
            int x = 1600, y = 900;
            RAGE.Game.Graphics.GetActiveScreenResolution(ref x, ref y);
            screenX = float.Parse(x.ToString());
            screenY = float.Parse(y.ToString());
        }
        
        public void Tick(List<Events.TickNametagData> nametags)
        {
            foreach (var player in Entities.Players.Streamed)
            {
                /* Variables */
                float _screenX = 0;
                float _screenY = 0;

                /* Player position */
                Vector3 position = player.GetBoneCoords(31086, 0, 0, 0);

                /* world3dToScreen2d */
                if (Graphics.GetScreenCoordFromWorldCoord(position.X, position.Y, position.Z + 0.60f, ref _screenX, ref _screenY))
                {
                    /* Debug */
                    //Chat.Output($"(debug) Player: {player.Name}; Position: {position.X}, {position.Y}, {position.Z}; Screen: {screenX}, {screenY}; world3dToScreen2d: {_screenX}, {_screenY}");
                    //Chat.Output($"x: {screenX} y: {screenY}");
                    /* Draw nickname */
                    UIResText.Draw(player.Name, (int)(screenX * _screenX + 100), (int)(screenY * _screenY + 100), Font.ChaletComprimeCologne, 0.4f, Color.White, RAGE.NUI.UIResText.Alignment.Centered, true, true, 0);
                }
            }
        }
    }
}
