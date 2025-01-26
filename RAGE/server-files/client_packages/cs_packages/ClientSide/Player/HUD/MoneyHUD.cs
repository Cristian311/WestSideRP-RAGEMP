using RAGE;
using RAGE.Game;
using RAGE.NUI;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ClientSide.Player
{
    public class MoneyHUD : Events.Script
    {
        private HtmlWindow cefBrowser = new HtmlWindow("http://package/hud/moneyhud.html");
        Random rand = new Random();

        private int CashMoney = 0;
        private int BankMoney = 0;
        private int Value;
        private int screenX, screenY;

        private DateTime notificationStartTime;
        private bool isNotificationActive = false;

        public MoneyHUD()
        {
            Events.Add("SERVER:CLIENT:CashMoneyUpdate", CashMoneyUpdate);
            Events.Add("SERVER:CLIENT:BankMoneyUpdate", BankMoneyUpdate);
            Events.Add("SERVER:CLIENT:SetMoney", SetMoney);


            Events.Tick += Tick;
        }

        private void SetMoney(object[] args)
        {
            int cash = int.Parse(args[0].ToString());
            int bank = int.Parse(args[1].ToString());
            cefBrowser.ExecuteJs($"updateBankMoney({bank});");
            cefBrowser.ExecuteJs($"updateNormalMoney({cash});");
            BankMoney = bank;
            CashMoney = cash;
        }

        private void BankMoneyUpdate(object[] args)
        {
            int NewValue = int.Parse(args[0].ToString());
            cefBrowser.ExecuteJs($"updateBankMoney({NewValue});");

            screenX = 1000 + rand.Next(0, 200);
            screenY = 700 + rand.Next(0, 200);

            Value = NewValue - BankMoney;
            BankMoney = NewValue;

            StartNotificationTimer();
        }

        private void CashMoneyUpdate(object[] args)
        {
            int NewValue = int.Parse(args[0].ToString());
            cefBrowser.ExecuteJs($"updateNormalMoney({NewValue});");

            screenX = 1000 + rand.Next(0, 200);
            screenY = 600 + rand.Next(0, 200);

            Value = NewValue - CashMoney;
            CashMoney = NewValue;

            StartNotificationTimer();
        }

        private void StartNotificationTimer()
        {
            notificationStartTime = DateTime.UtcNow; // Record the current timestamp
            isNotificationActive = true;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (isNotificationActive)
            {
                TimeSpan elapsedTime = DateTime.UtcNow - notificationStartTime;

                // Draw the money change notification
                if (Value < 0)
                    UIResText.Draw(Value.ToString(), screenX, screenY - (int)(elapsedTime.TotalMilliseconds / 50), Font.ChaletComprimeCologne, 0.8f, Color.Red, RAGE.NUI.UIResText.Alignment.Centered, true, true, 0);
                else
                    UIResText.Draw(Value.ToString(), screenX, screenY - (int)(elapsedTime.TotalMilliseconds / 50), Font.ChaletComprimeCologne, 0.8f, Color.Green, RAGE.NUI.UIResText.Alignment.Centered, true, true, 0);


                // Check if the notification has been displayed for more than 2 seconds
                if (elapsedTime.TotalMilliseconds > 3500)
                {
                    isNotificationActive = false;
                }
            }
        }
    }
}
