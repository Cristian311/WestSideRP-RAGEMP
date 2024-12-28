using RAGE;
using RAGE.Elements;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Player
{
    internal class Speedometer : Events.Script
    {
        private HtmlWindow cefBroswer = new HtmlWindow("https://package2/speedometer/CEF/speedometer.html");
        private static RAGE.Elements.Player player = RAGE.Elements.Player.LocalPlayer;
        private static bool showed = false;
        public Speedometer() 
        {
            Events.Tick += SpeedUpdate;
            cefBroswer.Active = true;
        }

        private void SpeedUpdate(List<Events.TickNametagData> nametags)
        {
            if(player.IsSittingInAnyVehicle())
            {
                if(showed == false)
                {
                    cefBroswer.ExecuteJs("showSpeedo();");
                    showed = true;
                }
                var speed = player.Vehicle.GetSpeed() * 3.6;
                var rpm = player.Vehicle.Rpm * 1000;
                var gas = player.Vehicle.GetPetrolTankHealth();
                gas = gas < 0 ? 0 : gas / 10;

                cefBroswer.ExecuteJs($"update({speed}, {rpm}, {gas});");
            }
            else
            {
                if(showed)
                {
                    cefBroswer.ExecuteJs("hideSpeedo();");
                    showed = false;

                }
            }
        }
    }
}
