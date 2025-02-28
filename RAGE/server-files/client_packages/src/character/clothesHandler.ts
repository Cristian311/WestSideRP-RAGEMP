interface ClothesData {
    Mask: { Drawable: number; Texture: number };
    Torso: { Drawable: number; Texture: number };
    Legs: { Drawable: number; Texture: number };
    Bag: { Drawable: number; Texture: number };
    Shoes: { Drawable: number; Texture: number };
    Accessories: { Drawable: number; Texture: number };
    Undershirt: { Drawable: number; Texture: number };
    BodyArmor: { Drawable: number; Texture: number };
    Decals: { Drawable: number; Texture: number };
    Shirts: { Drawable: number; Texture: number };
    Helmets: { Drawable: number; Texture: number };
    Glasses: { Drawable: number; Texture: number };
    EarAccessories: { Drawable: number; Texture: number };
    Watches: { Drawable: number; Texture: number };
    Bracelets: { Drawable: number; Texture: number };
}

let clothesCefBrowser: BrowserMp | null = null;

mp.events.add("SERVER:CLIENT:OpenClothesScreen", (json: string, currentClothes: number) => {
    clothesCefBrowser = mp.browsers.new("package://html/clothes/clothes.html");
    mp.gui.cursor.show(true, true);
    mp.gui.chat.show(false);

    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    clothesCefBrowser.execute(`UpdateClothes("${escapedJson}");`);
    clothesCefBrowser.execute(`selectCharacter(${currentClothes});`);

    mp.events.callRemote("CLIENT:SERVER:GetClothes", currentClothes);
});

mp.events.add("SERVER:CLIENT:CloseClothesScreen", () => {
    clothesCefBrowser?.destroy();
    clothesCefBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.gui.chat.show(true);
});

mp.events.add("CEF:CLIENT:CloseClothesScreen", () => {
    mp.events.call("SERVER:CLIENT:CloseClothesScreen");
});

mp.events.add("CEF:CLIENT:UpdateClothes", (json: string) => {
    const clothesData: ClothesData = JSON.parse(json);
    const player = mp.players.local;

    updatePlayerClothes(clothesData);
});

function updatePlayerClothes(clothesData: ClothesData) {
    try {
        const player = mp.players.local;
        
        // Check if clothesData exists before using it
        if (!clothesData) {
            mp.console.logError("Clothes data is undefined");
            return;
        }
        
        // Safely set component variations with null checks
        if (clothesData.Mask) player.setComponentVariation(1, clothesData.Mask.Drawable, clothesData.Mask.Texture, 0);
        if (clothesData.Torso) player.setComponentVariation(3, clothesData.Torso.Drawable, clothesData.Torso.Texture, 0);
        if (clothesData.Legs) player.setComponentVariation(4, clothesData.Legs.Drawable, clothesData.Legs.Texture, 0);
        if (clothesData.Bag) player.setComponentVariation(5, clothesData.Bag.Drawable, clothesData.Bag.Texture, 0);
        if (clothesData.Shoes) player.setComponentVariation(6, clothesData.Shoes.Drawable, clothesData.Shoes.Texture, 0);
        if (clothesData.Accessories) player.setComponentVariation(7, clothesData.Accessories.Drawable, clothesData.Accessories.Texture, 0);
        if (clothesData.Undershirt) player.setComponentVariation(8, clothesData.Undershirt.Drawable, clothesData.Undershirt.Texture, 0);
        if (clothesData.BodyArmor) player.setComponentVariation(9, clothesData.BodyArmor.Drawable, clothesData.BodyArmor.Texture, 0);
        if (clothesData.Decals) player.setComponentVariation(10, clothesData.Decals.Drawable, clothesData.Decals.Texture, 0);
        if (clothesData.Shirts) player.setComponentVariation(11, clothesData.Shirts.Drawable, clothesData.Shirts.Texture, 0);

        const clearOrSetProp = (slot: number, drawable: number, texture: number) => {
            if (drawable === -1) {
                player.clearProp(slot);
            } else {
                player.setPropIndex(slot, drawable, texture, true);
            }
        };

        // Safely set props with null checks
        if (clothesData.Helmets) clearOrSetProp(0, clothesData.Helmets.Drawable, clothesData.Helmets.Texture);
        if (clothesData.Glasses) clearOrSetProp(1, clothesData.Glasses.Drawable, clothesData.Glasses.Texture);
        if (clothesData.EarAccessories) clearOrSetProp(2, clothesData.EarAccessories.Drawable, clothesData.EarAccessories.Texture);
        if (clothesData.Watches) clearOrSetProp(6, clothesData.Watches.Drawable, clothesData.Watches.Texture);
        if (clothesData.Bracelets) clearOrSetProp(7, clothesData.Bracelets.Drawable, clothesData.Bracelets.Texture);
    } catch (error) {
        mp.console.logError("Error in updatePlayerClothes: " + error);
    }
}
export { updatePlayerClothes };
mp.events.add("CEF:CLIENT:SelectCostume", (index: number) => {
    mp.events.callRemote("CLIENT:SERVER:GetClothes", index);
});

mp.events.add("CEF:CLIENT:BuyCostume", (clothesJSON: string) => {
    mp.events.callRemote("CLIENT:SERVER:BuyClothes", clothesJSON);
    mp.events.call("SERVER:CLIENT:CloseClothesScreen");
});

mp.events.add("SERVER:CLIENT:ShowCostume", (json: string) => {
    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    clothesCefBrowser?.execute(`UpdateClothes("${escapedJson}");`);
});

mp.events.add("SERVER:CLIENT:UpdateDataMaxValues", (json: string) => {
    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    clothesCefBrowser?.execute(`updateMaxValuesFromJSON("${escapedJson}");`);
});

mp.events.add("SERVER:CLIENT:UpdateOutfitNames", (json: string) => {
    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    clothesCefBrowser?.execute(`updateOutfitNames("${escapedJson}");`);
});
