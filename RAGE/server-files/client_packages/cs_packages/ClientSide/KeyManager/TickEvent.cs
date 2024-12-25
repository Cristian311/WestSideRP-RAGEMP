using System.Collections.Generic;
using CHRP.Assets;
using ClientSide.Character.Inventory;
using RAGE;
using RAGE.Ui;

namespace ClientSide
{
    public class TickEvent : RAGE.Events.Script
    {
        public TickEvent()
        {
           RAGE.Events.Tick = Tick; 
        }

        private void Tick(List<RAGE.Events.TickNametagData> nametags)
        {
            KeyManager.KeyBind(KeyManager.KeyLctrl, () =>
            {
                Chat.Output("Key Bind Work");      
            });   
            
            KeyManager.KeyBind(KeyManager.KeyMouse, delegate // ~
            {
                Cursor.Visible = !Cursor.Visible;
            });

            KeyManager.KeyBind(KeyManager.KeyY, () =>
            {
                Events.CallRemote("CLIENT:SERVER:ActionKeyPressed");
            });

            KeyManager.KeyBind(KeyManager.KeyF4, () =>
            {
                InventoryHandler.CloseInventory(null);
            });
        }
    }
}