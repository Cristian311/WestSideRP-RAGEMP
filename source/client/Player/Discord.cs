using RAGE;

namespace ClientSide.Player
{
    internal class Discord : Events.Script
    {
        public Discord()
        {
            Events.Add("SERVER:CLIENT:UpdateDiscord", UpdateDiscord);
        }

        private void UpdateDiscord(object[] args)
        {
            RAGE.Discord.Update("Playing on West-Side Roleplay", args[0].ToString());
        }
    }
}
