using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAGE;
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
            RAGE.Chat.Output("merge clientu3");
        }
    }
}
