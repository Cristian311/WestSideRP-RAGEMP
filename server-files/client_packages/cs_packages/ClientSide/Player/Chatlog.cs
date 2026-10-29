using RAGE;

namespace ClientSide.Player
{
    internal class Chatlog : Events.Script
    {
        private Chatlog()
        {
            Events.Add("CEF:CLIENT:Chatlog", Chatlogger);
        }

        private void Chatlogger(object[] args)
        {
            string message = args[0].ToString();
            //Chat.Output(message);
            Events.CallRemote("CLIENT:SERVER:ChatLogger", message);
        }
    }
}
