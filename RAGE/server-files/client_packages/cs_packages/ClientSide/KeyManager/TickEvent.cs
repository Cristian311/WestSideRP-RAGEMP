using System.Collections.Generic;
using CHRP.Assets;
using ClientSide.Character.Inventory;
using ClientSide.Shop;
using RAGE;
using RAGE.Ui;

namespace ClientSide
{
    public class TickEvent : RAGE.Events.Script
    {
        private static bool ChatEnabled = false;
        public TickEvent()
        {
           RAGE.Events.Tick = Tick;

        }

        private void Tick(List<RAGE.Events.TickNametagData> nametags)
        {
            KeyManager.KeyBind(KeyManager.KeyMouse, delegate // ~
            {
                Cursor.Visible = !Cursor.Visible;
            });
            KeyManager.KeyBind(KeyManager.KeyF4, () =>
            {
                InventoryHandler.CloseInventory(null);
                ShopHandler.CloseShop(null);
            });
            if (RAGE.Ui.Cursor.Visible == false)
            {
                KeyManager.KeyBind(KeyManager.KeyLctrl, () =>
                {
                    //Chat.Output("Key Bind Work");      
                });

                

                KeyManager.KeyBind(KeyManager.KeyY, () =>
                {
                    //if(RAGE.Elements.Player.LocalPlayer.Vehicle.GetPedInSeat(-1,0) == RAGE.Elements.Player.LocalPlayer.Handle)
                    //    Events.CallRemote("CLIENT:SERVER:TurnEngine"); // needs fixed very poor optimization
                    Events.CallRemote("CLIENT:SERVER:ActionKeyPressed");
                    Events.CallRemote("CLIENT:SERVER:TryEnterDoor");
                });

                

                KeyManager.KeyBind(KeyManager.KeyL, () =>
                {
                    Events.CallRemote("CLIENT:SERVER:LockKeyPressed");
                });
            }
        }
    }
}