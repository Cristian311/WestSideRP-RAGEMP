"use strict";
let inventoryCefBrowser = null;
mp.events.add("SERVER:CLIENT:OpenInventory", (json) => {
    inventoryCefBrowser = mp.browsers.new("package://html/inventory/inventory.html");
    mp.gui.cursor.show(true, true);
    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    inventoryCefBrowser.execute(`updateInventoryDataFromJson("${escapedJson}");`);
});
mp.events.add("CEF:CLIENT:OpenInventory", () => {
    inventoryCefBrowser = mp.browsers.new("package://html/inventory/inventory.html");
    mp.gui.cursor.show(true, true);
});
mp.events.add("CEF:CLIENT:CloseInventory", (json) => {
    inventoryCefBrowser === null || inventoryCefBrowser === void 0 ? void 0 : inventoryCefBrowser.destroy();
    inventoryCefBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("CLIENT:SERVER:ClosedInventory", json);
});
mp.events.add("CEF:CLIENT:UpdateInventory", (json) => {
    mp.events.callRemote("CLIENT:SERVER:UpdateInventory", json);
});
mp.events.add("CEF:CLIENT:UpdateInventoryClient", (json) => {
    const inventoryItems = JSON.parse(json);
    updatePlayerInventory(inventoryItems);
});
function updatePlayerInventory(items) {
    const player = mp.players.local;
    // Example of setting inventory items
    items.forEach(item => {
        // You would have some logic to update the player's inventory UI or logic here
        console.log(`Item ID: ${item.ItemId}, Name: ${item.Name}, Amount: ${item.Amount}`);
        // Call a function to update the player's inventory UI
    });
}
mp.events.add("CEF:CLIENT:UseItem", (json) => {
    const item = JSON.parse(json);
    mp.events.callRemote("CLIENT:SERVER:UseItem", item.ItemId, item.Amount);
});
mp.events.add("CEF:CLIENT:EquipItem", (json) => {
    const item = JSON.parse(json);
    mp.events.callRemote("CLIENT:SERVER:EquipItem", item.ItemId, item.Amount);
});
mp.events.add("CEF:CLIENT:WearItem", (json) => {
    const item = JSON.parse(json);
    mp.events.callRemote("CLIENT:SERVER:WearItem", item.ItemId, item.Amount);
});
mp.events.add("CEF:CLIENT:ShowItem", (json) => {
    const item = JSON.parse(json);
    mp.events.callRemote("CLIENT:SERVER:ShowItem", item.ItemId);
});
mp.events.add("CEF:CLIENT:GiveItem", (json) => {
    const item = JSON.parse(json);
    mp.events.callRemote("CLIENT:SERVER:GiveItem", item.ItemId, item.Amount);
});
mp.events.add("CEF:CLIENT:DropItem", (json) => {
    const item = JSON.parse(json);
    mp.events.callRemote("CLIENT:SERVER:DropItem", item.ItemId);
});
mp.events.add("CEF:CLIENT:UpdateInventoryOrder", (json) => {
    const orderJson = json;
    mp.events.callRemote("CLIENT:SERVER:UpdateInventoryOrder", orderJson);
});
//# sourceMappingURL=inventoryHandler.js.map