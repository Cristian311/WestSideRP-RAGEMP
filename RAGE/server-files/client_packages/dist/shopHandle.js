"use strict";
// Shop browser instance
let shopBrowser = null;
/**
 * Opens the shop browser UI.
 * @param json - A JSON string containing shop item data.
 * @param adminBool - A boolean indicating admin status (currently not used).
 */
mp.events.add("SERVER:CLIENT:OpenShop", (json, adminBool) => {
    try {
        shopBrowser = mp.browsers.new("package://html/shops/store.html");
        mp.gui.cursor.show(true, true);
        // Escape the JSON so it can be safely injected into the JS call.
        // JSON.stringify wraps the string in quotes and escapes inner quotes as needed.
        const escapedJson = JSON.stringify(json);
        // Execute JS to update shop items and toggle admin features.
        // Note: Currently the admin flag is hardcoded to false.
        shopBrowser.execute(`setTimeout(() => UpdateItems(${escapedJson}), 100);`);
        shopBrowser.execute(`setTimeout(() => toggleAdmin(false), 100);`);
    }
    catch (error) {
        mp.console.logError("Error opening shop: " + error);
    }
});
/**
 * Called when the user confirms a purchase.
 * Sends the purchase data to the server and closes the shop UI.
 * @param json - A JSON string containing the items to buy.
 */
mp.events.add("CEF:CLIENT:BuyShopItems", (json) => {
    try {
        mp.events.callRemote("CLIENT:SERVER:BuyShopItems", json);
        closeShop();
    }
    catch (error) {
        mp.console.logError("Error buying shop items: " + error);
    }
});
/**
 * Called when item data should be saved.
 * Sends the item data to the server.
 * @param json - A JSON string containing the updated item data.
 */
mp.events.add("CEF:CLIENT:SaveItemData", (json) => {
    try {
        mp.events.callRemote("CLIENT:SERVER:SaveItemData", json);
    }
    catch (error) {
        mp.console.logError("Error saving item data: " + error);
    }
});
/**
 * Closes the shop UI.
 */
mp.events.add("CEF:CLIENT:CloseShop", () => {
    try {
        closeShop();
    }
    catch (error) {
        mp.console.logError("Error closing shop: " + error);
    }
});
/**
 * Helper function to destroy the shop browser and hide the cursor.
 */
function closeShop() {
    if (shopBrowser) {
        shopBrowser.destroy();
        shopBrowser = null;
    }
    mp.gui.cursor.show(false, false);
}
//# sourceMappingURL=shopHandle.js.map