using RAGE;
using RAGE.Ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSide.Character.Inventory
{
    internal class InventoryHandler : Events.Script
    {
        private static HtmlWindow cefBrowser;

        public InventoryHandler()
        {
            // Server to client events
            Events.Add("SERVER:CLIENT:OpenInventory", OpenInventory);
            Events.Add("SERVER:CLIENT:UpdateInventory", UpdateInventory);
            Events.Add("SERVER:CLIENT:PopulateNearbyCharacters", PopulateNearbyCharacters);

            // CEF to client events
            Events.Add("CEF:CLIENT:CloseInventory", CloseInventory);
            Events.Add("CEF:CLIENT:UseItem", UseItem);
            Events.Add("CEF:CLIENT:EquipItem", EquipItem);
            Events.Add("CEF:CLIENT:WearItem", WearItem);
            Events.Add("CEF:CLIENT:ShowItem", ShowItem);
            Events.Add("CEF:CLIENT:GiveItem", GiveItem);
            Events.Add("CEF:CLIENT:DropItem", DropItem);
            Events.Add("CEF:CLIENT:PlayerSelected", PlayerSelected);
            Events.Add("CEF:CLIENT:UpdateInventoryOrder", UpdateInventoryOrder);
            Events.Add("CEF:CLIENT:GetNearbyCharacters", GetNearbyCharacters);
            
        }

        private void PlayerSelected(object[] args)
        {
            string json = args[0].ToString();

            var action = RAGE.Util.Json.Deserialize<PlayerAction>(json);

            if (action.Action == "give")
            {
                Events.CallRemote("CLIENT:SERVER:GiveItem", action.PlayerId, action.ItemId, action.Amount);
            }    
            else if(action.Action == "show")
            {
                Events.CallRemote("CLIENT:SERVER:ShowItem", action.PlayerId, action.ItemId);
            }
        }

        private void PopulateNearbyCharacters(object[] args)
        {
            string json = args[0].ToString();
            cefBrowser.ExecuteJs($"populateNearbyPlayers({json});");
        }

        private void GetNearbyCharacters(object[] args)
        {
            Events.CallRemote("CLIENT:SERVER:GetNearbyCharacters");
        }

        private void UpdateInventory(object[] args)
        {
            string json = args[0].ToString();
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");
            cefBrowser.ExecuteJs($"updateInventory(\"{escapedJson}\");");
        }

        public static void CloseInventory(object[] args)
        {
            cefBrowser.Destroy();
            cefBrowser = null;
            Cursor.Visible = false;
        }

        private void OpenInventory(object[] args)
        {
            cefBrowser = new HtmlWindow("http://package/html/inventory/inventory.html");
            cefBrowser.Active = true;
            Cursor.Visible = true;
            string json = args[0].ToString();
            string escapedJson = json.Replace("\\", "\\\\").Replace("\"", "\\\"");
            cefBrowser.ExecuteJs($"updateInventory(\"{escapedJson}\");");
        }

        private void UseItem(object[] args)
        {
            string itemId = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:UseItem", itemId);
        }

        private void EquipItem(object[] args)
        {
            string itemId = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:EquipItem", itemId);
        }

        private void WearItem(object[] args)
        {
            string itemId = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:WearItem", itemId);
        }

        private void ShowItem(object[] args)
        {
            string itemId = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:ShowItem", itemId);
        }

        private void GiveItem(object[] args)
        {
            string itemId = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:GiveItem", itemId);
        }

        private void DropItem(object[] args)
        {
            string itemId = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:DropItem", itemId);
        }

        private void UpdateInventoryOrder(object[] args)
        {
            string orderJson = args[0].ToString();
            Events.CallRemote("CLIENT:SERVER:UpdateInventoryOrder", orderJson);
        }
    }
}