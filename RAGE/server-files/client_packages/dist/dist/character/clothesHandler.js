"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.updatePlayerClothes = updatePlayerClothes;
let clothesCefBrowser = null;
mp.events.add("SERVER:CLIENT:OpenClothesScreen", (json, currentClothes) => {
    clothesCefBrowser = mp.browsers.new("package://html/clothes/clothes.html");
    mp.gui.cursor.show(true, true);
    mp.gui.chat.show(false);
    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    clothesCefBrowser.execute(`UpdateClothes("${escapedJson}");`);
    clothesCefBrowser.execute(`selectCharacter(${currentClothes});`);
    mp.events.callRemote("CLIENT:SERVER:GetClothes", currentClothes);
});
mp.events.add("SERVER:CLIENT:CloseClothesScreen", () => {
    clothesCefBrowser === null || clothesCefBrowser === void 0 ? void 0 : clothesCefBrowser.destroy();
    clothesCefBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.gui.chat.show(true);
});
mp.events.add("CEF:CLIENT:CloseClothesScreen", () => {
    mp.events.call("SERVER:CLIENT:CloseClothesScreen");
});
mp.events.add("CEF:CLIENT:UpdateClothes", (json) => {
    const clothesData = JSON.parse(json);
    const player = mp.players.local;
    updatePlayerClothes(clothesData);
});
function updatePlayerClothes(clothesData) {
    player.setComponentVariation(1, clothesData.Mask.Drawable, clothesData.Mask.Texture, 0);
    player.setComponentVariation(3, clothesData.Torso.Drawable, clothesData.Torso.Texture, 0);
    player.setComponentVariation(4, clothesData.Legs.Drawable, clothesData.Legs.Texture, 0);
    player.setComponentVariation(5, clothesData.Bag.Drawable, clothesData.Bag.Texture, 0);
    player.setComponentVariation(6, clothesData.Shoes.Drawable, clothesData.Shoes.Texture, 0);
    player.setComponentVariation(7, clothesData.Accessories.Drawable, clothesData.Accessories.Texture, 0);
    player.setComponentVariation(8, clothesData.Undershirt.Drawable, clothesData.Undershirt.Texture, 0);
    player.setComponentVariation(9, clothesData.BodyArmor.Drawable, clothesData.BodyArmor.Texture, 0);
    player.setComponentVariation(10, clothesData.Decals.Drawable, clothesData.Decals.Texture, 0);
    player.setComponentVariation(11, clothesData.Shirts.Drawable, clothesData.Shirts.Texture, 0);
    const clearOrSetProp = (slot, drawable, texture) => {
        if (drawable === -1) {
            player.clearProp(slot);
        }
        else {
            player.setPropIndex(slot, drawable, texture, true);
        }
    };
    clearOrSetProp(0, clothesData.Helmets.Drawable, clothesData.Helmets.Texture);
    clearOrSetProp(1, clothesData.Glasses.Drawable, clothesData.Glasses.Texture);
    clearOrSetProp(2, clothesData.EarAccessories.Drawable, clothesData.EarAccessories.Texture);
    clearOrSetProp(6, clothesData.Watches.Drawable, clothesData.Watches.Texture);
    clearOrSetProp(7, clothesData.Bracelets.Drawable, clothesData.Bracelets.Texture);
}
mp.events.add("CEF:CLIENT:SelectCostume", (index) => {
    mp.events.callRemote("CLIENT:SERVER:GetClothes", index);
});
mp.events.add("CEF:CLIENT:BuyCostume", (clothesJSON) => {
    mp.events.callRemote("CLIENT:SERVER:BuyClothes", clothesJSON);
    mp.events.call("SERVER:CLIENT:CloseClothesScreen");
});
mp.events.add("SERVER:CLIENT:ShowCostume", (json) => {
    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    clothesCefBrowser === null || clothesCefBrowser === void 0 ? void 0 : clothesCefBrowser.execute(`UpdateClothes("${escapedJson}");`);
});
mp.events.add("SERVER:CLIENT:UpdateDataMaxValues", (json) => {
    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    clothesCefBrowser === null || clothesCefBrowser === void 0 ? void 0 : clothesCefBrowser.execute(`updateMaxValuesFromJSON("${escapedJson}");`);
});
mp.events.add("SERVER:CLIENT:UpdateOutfitNames", (json) => {
    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    clothesCefBrowser === null || clothesCefBrowser === void 0 ? void 0 : clothesCefBrowser.execute(`updateOutfitNames("${escapedJson}");`);
});
//# sourceMappingURL=clothesHandler.js.map