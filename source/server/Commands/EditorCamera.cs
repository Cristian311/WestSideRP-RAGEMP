using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSide.Server.Commands
{
    internal class EditorCamera : Script
    {
        [Command("editor")]
        private void cmd_editor(Player player)
        {
            player.TriggerEvent("SERVER:CLIENT:ActivateEditorCamera");
            player.TriggerEvent("SERVER:CLIENT:ToggleHud", false);
        }

        //need to ping pong event from js client to server then to c# client, we need to switch all the client on js
        [RemoteEvent("CLIENT:SERVER:ToggleHud")]
        private void EventPingPong(Player player)
        {
            player.TriggerEvent("SERVER:CLIENT:ToggleHud", true);
        }

    }
}
