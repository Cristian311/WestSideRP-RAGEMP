using RAGE;

namespace ClientSide.Player
{
    internal class AdminEvents : Events.Script
    {
        public AdminEvents()
        {
            Events.Add("SERVER:CLIENT:FreezePlayer", FreezePlayer);
        }

        private void FreezePlayer(object[] args)
        {
        }
    }
}
