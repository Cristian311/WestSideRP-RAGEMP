using Clientside.CustomizeChar;
using ClientSide.Character.Clothes;
using ClientSide.Character.CustomizeChar;
using ClientSide.Character.Hair;
using ClientSide.Hair;
using RAGE;
using RAGE.Ui;
using RAGE.Util;
using System.Collections.Generic;
namespace ClientSide.Character
{
    public class CharHandler : Events.Script
    {
        private HtmlWindow cefBrowser;
        private List<CharacterData> faces;
        private List<HairData> hairs;
        private List<ClothesData> clothes;

        public CharHandler()
        {
            Events.Add("SERVER:CLIENT:ShowCharInterface", ShowCharInterface);
            Events.Add("CEF:CLIENT:ShowCharInterface", ShowCharInterface);

            Events.Add("SERVER:CLIENT:CloseCharInterface", CloseCharInterface);
            Events.Add("CEF:CLIENT:CloseCharInterface", CloseCharInterface);

            Events.Add("SERVER:CLIENT:DisplayAccountInfo", DisplayAccountInfo);
            Events.Add("CEF:CLIENT:CharacterSelected", CharacterSelected);


            //character ui selection
            Events.Add("SERVER:CLIENT:CharactersInfo", CharactersInfo);
            Events.Add("CEF:CLIENT:CharacterPressed", CharacterPressed);
        }

        private void CharactersInfo(object[] args)
        {
            string facesJson = args[0].ToString();
            string hairsJson = args[1].ToString();
            string clothesJson = args[2].ToString();

            faces = Json.Deserialize<List<CharacterData>>(facesJson);
            hairs = Json.Deserialize<List<HairData>>(hairsJson);
            clothes = Json.Deserialize<List<ClothesData>>(clothesJson);

            Events.CallRemote("CLIENT:SERVER:PrintConsole", "CLIENT");
            Events.CallRemote("CLIENT:SERVER:PrintConsole", facesJson);
            Events.CallRemote("CLIENT:SERVER:PrintConsole", hairsJson);
            Events.CallRemote("CLIENT:SERVER:PrintConsole", clothesJson);
        }

        private void CharacterPressed(object[] args)
        {
            byte index = byte.Parse(args[0].ToString());

            CustomizeCharHandler.UpdatePlayerFaceFeatures(faces[index]);

            object[] HairData = new object[] { Json.Serialize(hairs[index]) };
            HairHandler.UpdateHair(HairData);

            object[] clothesData = new object[] { Json.Serialize(clothes[index]) }; // Wrap the object in an array
            ClothesHandler.UpdateClothes(clothesData);
        }

        private void ShowCharInterface(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package/html/characters/characters.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;

            Chat.Show(false);
        }
        private void CloseCharInterface(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;

        }
        private void DisplayAccountInfo(object[] args)
        {

            string username = args[0].ToString();
            string namesString = args[1].ToString();
            string moneyString = args[2].ToString();

            cefBrowser.ExecuteJs($"displayAccountInfo(`{username}`, {namesString}, {moneyString});");

        }
        private void CharacterSelected(object[] args)
        {
            string CharacterName = args[0].ToString();

            Events.CallRemote("CLIENT:SERVER:CharSelected", CharacterName);
            Events.CallRemote("CLIENT:SERVER:CustomizeScreenOnCreation");

            Chat.Show(true);
        }
    }
}
