using GTANetworkAPI;

namespace ServerSide.Util
{
    internal class ServerEvents : Script
    {
        [RemoteEvent("CLIENT:SERVER:PrintMessage")]
        private void PrintMessage(Player player, string message)
        {
            NAPI.Util.ConsoleOutput(message);
        }
    }
}
