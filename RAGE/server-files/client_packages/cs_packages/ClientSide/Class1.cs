using RAGE;
using RAGE.Ui;
using System;

namespace ClientSide
{
    public class Class1 : Events.Script
    {
        private HtmlWindow cefBrowser;

        public Class1()
        {
            Events.OnPlayerReady += onPlayerSpawn;
        }

        private void onPlayerSpawn()
        {
            RAGE.Chat.Output("ddd");
        }
    }
}
