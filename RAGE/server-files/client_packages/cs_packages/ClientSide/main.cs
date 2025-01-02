using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var chatbox = new HtmlWindow("http://package2/advanced-chat/index.html");
            chatbox.MarkAsChat();
            RAGE.Chat.Colors = true;
        }
    }
}
