using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Util
{
    internal class UtilEvents : Events.Script
    {
        public UtilEvents() 
        {
            Events.Add("SERVER:CLIENT:enableCursor", enableCursorHandler);
            Events.Add("SERVER:CLIENT:disableCursor", disableCursorHandler);
        }
        
        private void disableCursorHandler(object[] args)
        {
            Cursor.Visible = true;
        }

        private void enableCursorHandler(object[] args)
        {
            Cursor.Visible = false;
        }
    }
}
