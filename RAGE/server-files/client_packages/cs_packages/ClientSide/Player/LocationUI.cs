using System.Collections.Generic;
using RAGE;
using RAGE.Game;
using RAGE.Ui;

namespace ClientSide
{
    class PLD : Events.Script
    {
        HtmlWindow CEF = null;
        string Street = null;
        string Crossing = null;
        string Zone = null;
        string Direction = null;

        public PLD()
        {
            CEF = new HtmlWindow("https://package/PLD/index.html");
            Events.Tick += Update;
        }

        public void Update(List<Events.TickNametagData> tick)
        {
            var position = RAGE.Elements.Player.LocalPlayer.Position;
            var tempStreet = 0;
            var tempCrossing = 0;
            var tempZone = "";
            float tempDirection = 0;
            Street = "";
            Crossing = "";
            Zone = "";
            Direction = "-";

            Pathfind.GetStreetNameAtCoord(position.X, position.Y, position.Z, ref tempStreet, ref tempCrossing);
            tempZone = RAGE.Game.Zone.GetNameOfZone(position.X, position.Y, position.Z);
            tempDirection = RAGE.Elements.Player.LocalPlayer.GetHeading();

            if (tempCrossing != 0)
            {
                Crossing = "/ " + Ui.GetStreetNameFromHashKey((uint)tempCrossing);
            }
            if (tempStreet != 0)
            {
                Street = Ui.GetStreetNameFromHashKey((uint)tempStreet);
            }
            if (tempZone != "")
            {
                Zone = Ui.GetLabelText(tempZone);
            }
            if (tempDirection != 0)
            {
                if ((tempDirection >= 0 && tempDirection < 22.5) || (tempDirection >= 337.5 && tempDirection <= 360))
                {
                    Direction = "N";
                }
                else if (tempDirection >= 22.5 && tempDirection < 67.5)
                {
                    Direction = "NE";
                }
                else if (tempDirection >= 67.5 && tempDirection < 112.5)
                {
                    Direction = "E";
                }
                else if (tempDirection >= 112.5 && tempDirection < 157.5)
                {
                    Direction = "SE";
                }
                else if (tempDirection >= 157.5 && tempDirection < 202.5)
                {
                    Direction = "S";
                }
                else if (tempDirection >= 202.5 && tempDirection < 247.5)
                {
                    Direction = "SW";
                }
                else if (tempDirection >= 247.5 && tempDirection < 292.5)
                {
                    Direction = "W";
                }
                else if (tempDirection >= 292.5 && tempDirection < 337.5)
                {
                    Direction = "NW";
                }

            }

            CEF.ExecuteJs($"Update(\"{Street}\", \"{Crossing}\", \"{Zone}\", \"{Direction}\")");
        }
    }
}
