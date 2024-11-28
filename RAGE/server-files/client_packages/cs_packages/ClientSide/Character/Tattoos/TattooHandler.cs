using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;


namespace ClientSide.Character
{
    internal class TattooHandler : Events.Script
    {
        private HtmlWindow cefBrowser;
        private Dictionary<string, string> tattooCollection = new Dictionary<string, string>();
        private Dictionary<string, string> tattooHash = new Dictionary<string, string>();

        private TattooHandler()
        {
            Events.Add("SERVER:CLIENT:ShowTattooScreen", ShowTattooScreen);
            Events.Add("CEF:CLIENT:StopTattooScreen", StopTattooScreen);
            Events.Add("CEF:CLIENT:ApplyTattoo", ApplyTattoo);
            Events.Add("CEF:CLIENT:ApplyTattooClient", ApplyTattooClient);
            Events.Add("CEF:CLIENT:RemoveTattoo", RemoveTattoo);
            Events.Add("CEF:CLIENT:RemoveTattooClient", RemoveTattooClient);
        }

        private void RemoveTattooClient(object[] args)
        {
            RAGE.Elements.Player.LocalPlayer.ClearDecorations();

            string tattoosJson = args[0].ToString();
            var tattoos = RAGE.Util.Json.Deserialize<List<string>>(tattoosJson);
            foreach (var tattooName in tattoos)
            {
                var collection = RAGE.Util.Joaat.Hash(tattooCollection[tattooName]);
                var hash = RAGE.Util.Joaat.Hash(tattooHash[tattooName]);
                RAGE.Elements.Player.LocalPlayer.SetDecoration(collection, hash);
            }
            
        }

        private void ApplyTattooClient(object[] args)
        {
            string Name = args[0].ToString();
            var collection = RAGE.Util.Joaat.Hash(tattooCollection[Name]);
            var hash = RAGE.Util.Joaat.Hash(tattooHash[Name]);
            RAGE.Elements.Player.LocalPlayer.SetDecoration(collection, hash);
        }

        private void RemoveTattoo(object[] args)
        {
            string tattooName = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:RemoveTattoo", tattooName);
        }

        private void ApplyTattoo(object[] args)
        {
            string tattooName = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:ApplyTattoo", tattooName);
        }
        private void ShowTattooScreen(object[] args)
        {
            try
            {
                // Initialize the HTML window and show the cursor
                string json = args[0].ToString();
                string UsedTattoosJson = args[1].ToString();
                string tattooCollectionJson = args[2].ToString();
                string tattooHashJson = args[3].ToString();

                cefBrowser = new HtmlWindow("http://package2/html/tattoos/tattoos.html");

                cefBrowser.Active = true;
                Cursor.Visible = true;
                Chat.Show(false);
                //string json = args[0].ToString();
                //RAGE.Chat.Output(json);
                string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");
                cefBrowser.ExecuteJs($"updateTattooData(\"{escapedJson}\");");

                escapedJson = UsedTattoosJson.Replace("\\", "\\\\").Replace("\"", "\\\"");
                cefBrowser.ExecuteJs($"updateHighlights(\"{escapedJson}\");");

                tattooCollection = RAGE.Util.Json.Deserialize<Dictionary<string, string>>(tattooCollectionJson);
                tattooHash = RAGE.Util.Json.Deserialize<Dictionary<string, string>>(tattooHashJson);
            }
            catch (Exception ex)
            {
                RAGE.Chat.Output(ex.Message);
            }
        }

        private void StopTattooScreen(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
            Chat.Show(true);
            string json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:UpdateTattoo", json);
        }
    }
}