using RAGE;
using RAGE.Ui;
namespace ClientSide
{
    internal class Main : Events.Script
    {
        Main()
        {
            Events.OnPlayerReady += onPlayerReady;
        }
        private void onPlayerReady()
        {
            Chat.Show(false);
            var chatbox = new HtmlWindow("http://package/advanced-chat/index.html");
            chatbox.MarkAsChat();
            RAGE.Chat.Colors = true;
            RAGE.Game.Ui.HideHudComponentThisFrame(7);
            RAGE.Game.Ui.HideHudComponentThisFrame(9);
            RAGE.Game.Ui.HideHudComponentThisFrame(3);


        }
    }
}
