using RAGE;

namespace ClientSide.Player
{
    internal class Blip : Events.Script
    {
        public Blip()
        {
            Events.Add("SERVER:CLIENT:CreateBlipForCar", CreateBlipForCar);
        }

        private void CreateBlipForCar(object[] args)
        {
            var position = RAGE.Util.Json.Deserialize<Vector3>(args[0].ToString());
            RAGE.Elements.Blip blip = new RAGE.Elements.Blip(1, position, "Vehicle", 1f, 5, 255, 0, false, 0, 0, 0);
        }
    }
}
