interface InventoryItem {
    ItemId: string;
    Amount: number;
    Name: string;
    Description: string;
}

let inventoryCefBrowser: BrowserMp | null = null;

mp.events.add("SERVER:CLIENT:OpenInventory", (json: string) => {
    inventoryCefBrowser = mp.browsers.new("package://html/inventory/inventory.html");
    mp.gui.cursor.show(true, true);

    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    inventoryCefBrowser.execute(`updateInventoryDataFromJson("${escapedJson}");`);
});

mp.events.add("CEF:CLIENT:OpenInventory", () => {
    inventoryCefBrowser = mp.browsers.new("package://html/inventory/inventory.html");
    mp.gui.cursor.show(true, true);
});

mp.events.add("CEF:CLIENT:CloseInventory", (json: string) => {
    inventoryCefBrowser?.destroy();
    inventoryCefBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("CLIENT:SERVER:ClosedInventory", json);
});

mp.events.add("CEF:CLIENT:UpdateInventory", (json: string) => {
    mp.events.callRemote("CLIENT:SERVER:UpdateInventory", json);
});

mp.events.add("CEF:CLIENT:UpdateInventoryClient", (json: string) => {
    const inventoryItems: InventoryItem[] = JSON.parse(json);
    updatePlayerInventory(inventoryItems);
});

function updatePlayerInventory(items: InventoryItem[]) {
    const player = mp.players.local;

    // Example of setting inventory items
    items.forEach(item => {
        // You would have some logic to update the player's inventory UI or logic here
        console.log(`Item ID: ${item.ItemId}, Name: ${item.Name}, Amount: ${item.Amount}`);
        // Call a function to update the player's inventory UI
    });
}

mp.events.add("CEF:CLIENT:UseItem", (json: string) => {
    const item: InventoryItem = JSON.parse(json);
    mp.events.callRemote("CLIENT:SERVER:UseItem", item.ItemId, item.Amount);
});

mp.events.add("CEF:CLIENT:EquipItem", (json: string) => {
    const item: InventoryItem = JSON.parse(json);
    mp.events.callRemote("CLIENT:SERVER:EquipItem", item.ItemId, item.Amount);
});

mp.events.add("CEF:CLIENT:WearItem", (json: string) => {
    const item: InventoryItem = JSON.parse(json);
    mp.events.callRemote("CLIENT:SERVER:WearItem", item.ItemId, item.Amount);
});

mp.events.add("CEF:CLIENT:ShowItem", (json: string) => {
    const item: InventoryItem = JSON.parse(json);
    mp.events.callRemote("CLIENT:SERVER:ShowItem", item.ItemId);
});

mp.events.add("CEF:CLIENT:GiveItem", (json: string) => {
    const item: InventoryItem = JSON.parse(json);
    mp.events.callRemote("CLIENT:SERVER:GiveItem", item.ItemId, item.Amount);
});

mp.events.add("CEF:CLIENT:DropItem", (json: string) => {
    const item: InventoryItem = JSON.parse(json);
    mp.events.callRemote("CLIENT:SERVER:DropItem", item.ItemId);
});

mp.events.add("CEF:CLIENT:UpdateInventoryOrder", (json: string) => {
    const orderJson = json;
    mp.events.callRemote("CLIENT:SERVER:UpdateInventoryOrder", orderJson);
});
