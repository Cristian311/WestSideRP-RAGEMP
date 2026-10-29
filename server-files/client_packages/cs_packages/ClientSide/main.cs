using RAGE;
using RAGE.Game;
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
            Ui.DisplayCash(false);
            Ui.DisplayAreaName(false);


        }
    }
}
