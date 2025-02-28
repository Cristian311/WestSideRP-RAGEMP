using CHRP.Assets;
using ClientSide.Character.Inventory;
using ClientSide.Player;
using ClientSide.Player.HUD;
using ClientSide.Shop;
using RAGE;
using RAGE.Game;
using RAGE.Ui;
using System;
using System.Collections.Generic;

namespace ClientSide
{
    public class TickEvent : Events.Script
    {
        private static bool ChatEnabled = false;
        private static bool HUDEnabled = true;
        public TickEvent()
        {
            RAGE.Events.Tick = Tick;
            Events.Add("SERVER:CLIENT:ToggleHud", ToggleHud);
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

                KeyManager.KeyBind(KeyManager.KeyE, () =>
                {
                    Examine.CloseExamineScreen();
                });

                KeyManager.KeyBind(KeyManager.KeyF7, () =>
                {
                    HUDEnabled = !HUDEnabled;
                    object[] args = new object[1] { HUDEnabled };
                    ToggleHud(args);
                });
            }
        }

        public static void ToggleHud(object[] args)
        {
            bool toggle;
            if (args[0] != null)
            {
                toggle = bool.Parse(args[0].ToString());
                HUDEnabled = toggle;
            }
            else
                toggle = HUDEnabled;
            ToggleHuds(toggle);
            
        }
        private static void ToggleHuds(bool toggle)
        {
            Chat.Show(toggle);
            Ui.DisplayHud(toggle);
            Ui.DisplayRadar(toggle);
           
            LogoAndVersion.toggleUI(toggle);
            MoneyHUD.toggleUI(toggle);
            Events.CallLocal("CLIENT:TogglePLD", toggle);
        }
    }
}