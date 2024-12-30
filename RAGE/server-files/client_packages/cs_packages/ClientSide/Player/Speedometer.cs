using RAGE;
using RAGE.Elements;
using RAGE.Game;
using RAGE.NUI;
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
        private Vector3 lastPosition;
        private double totalDistance = 0.0;

        public Speedometer()
        {
            Events.Tick += SpeedUpdate;
            Events.Add("SERVER:CLIENT:UpdateFuelAndDistance", UpdateFuelAndDistance);
            cefBroswer.Active = true;
        }

        private void UpdateFuelAndDistance(object[] args)
        {
            double distance = double.Parse(args[0].ToString());
            double fuel = double.Parse(args[1].ToString());
            // Ensure fuel is formatted correctly in C#
            string fuelString = fuel.ToString("F2");  // Ensure two decimal places
            //Chat.Output($"updateFuelAndDistance({distance}, {fuelString});");
            cefBroswer.ExecuteJs($"updateFuelAndDistance({distance}, {fuelString});");
            cefBroswer.ExecuteJs($"updateFuel({fuel*100});");
        }

        private void SpeedUpdate(List<Events.TickNametagData> nametags)
        {
            if (player.IsSittingInAnyVehicle())
            {
                if (showed == false)
                {
                    cefBroswer.ExecuteJs("showSpeedo();");
                    showed = true;
                }

                var speed = player.Vehicle.GetSpeed() * 3.6;
                var rpm = player.Vehicle.Rpm * 1000;
                var gas = player.Vehicle.GetPetrolTankHealth();
                gas = gas < 0 ? 0 : gas / 10;

                // Update the travel distance
                Vector3 currentPosition = player.Vehicle.Position;
                if (lastPosition != null)
                {
                    totalDistance += Vector3.Distance(currentPosition, lastPosition);
                }
                lastPosition = currentPosition;
                if (totalDistance > 15)
                {
                    Events.CallRemote("CLIENT:SERVER:UpdateCarPosition", totalDistance);
                    totalDistance = 0;
                }
                cefBroswer.ExecuteJs($"update({speed}, {rpm}, {gas});");
            }
            else
            {
                if (showed)
                {
                    
                    cefBroswer.ExecuteJs("hideSpeedo();");
                    showed = false;
                }

                // Reset lastPosition to null when not in a vehicle
                lastPosition = null;
            }
        }
    }
}
