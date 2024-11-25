using ClientSide.Hair;
using RAGE;
using RAGE.Ui;


namespace ClientSide.Character.Hair
{
    internal class HairHandler : Events.Script
    {
        private HtmlWindow cefBrowser;
        private HairHandler()
        {
            Events.Add("SERVER:CLIENT:ShowHairScreen", showHairScreen);
            Events.Add("CEF:CLIENT:StopHairScreen", stopHairScreen);
            Events.Add("CEF:CLIENT:UpdateHair", UpdateHair);

        }

        private void UpdateHair(object[] args)
        {
            string json = args[0].ToString();
            HairData hair = RAGE.Util.Json.Deserialize<HairData>(json);
            int facialValue = int.Parse(hair.FacialHair.Value);
            int facialOpacity = int.Parse(hair.FacialHair.Opacity);
            int eyebrowsValue = int.Parse(hair.Eyebrows.Value);
            int eyebrowsOpacity = int.Parse(hair.Eyebrows.Opacity);
            int bodyhairValue = int.Parse(hair.BodyHair.Value);
            int bodyhairOpacity = int.Parse(hair.BodyHair.Opacity);
            int hairValue = int.Parse(hair.Hair);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(1, facialValue, facialOpacity);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(2, eyebrowsValue, eyebrowsOpacity);
            RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(10, bodyhairValue, bodyhairOpacity);
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation(2, hairValue, 0, 0);
        }
        private void showHairScreen(object[] args)
        {
            // Initialize the HTML window and show the cursor
            cefBrowser = new HtmlWindow("http://package2/html/Characters/customize_hair.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;

            string json = args[0].ToString();
            Chat.Output(json);

            // Escape the JSON for embedding in JavaScript
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");
            cefBrowser.ExecuteJs($"updateHairDataFromJson(\"{escapedJson}\");");
        }

        private void stopHairScreen(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
            Chat.Output("closed");

            string json = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:UpdateHair", json);
        }
    }
}
