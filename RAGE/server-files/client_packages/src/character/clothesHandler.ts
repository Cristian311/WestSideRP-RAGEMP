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
    try {
        const data = JSON.parse(json);
        
        // Transform the data to match our ClothesData interface
        const clothesData: ClothesData = {
            Mask: {
                Drawable: data.masks?.drawable ?? 0,
                Texture: data.masks?.texture ?? 0
            },
            Torso: {
                Drawable: data.torsos?.drawable ?? 0,
                Texture: data.torsos?.texture ?? 0
            },
            Legs: {
                Drawable: data.legs?.drawable ?? 0,
                Texture: data.legs?.texture ?? 0
            },
            Bag: {
                Drawable: data.bags?.drawable ?? 0,
                Texture: data.bags?.texture ?? 0
            },
            Shoes: {
                Drawable: data.shoes?.drawable ?? 0,
                Texture: data.shoes?.texture ?? 0
            },
            Accessories: {
                Drawable: data.accessories?.drawable ?? 0,
                Texture: data.accessories?.texture ?? 0
            },
            Undershirt: {
                Drawable: data.undershirts?.drawable ?? 0,
                Texture: data.undershirts?.texture ?? 0
            },
            BodyArmor: {
                Drawable: data.bodyArmor?.drawable ?? 0,
                Texture: data.bodyArmor?.texture ?? 0
            },
            Decals: {
                Drawable: data.decals?.drawable ?? 0,
                Texture: data.decals?.texture ?? 0
            },
            Shirts: {
                Drawable: data.shirts?.drawable ?? 0,
                Texture: data.shirts?.texture ?? 0
            },
            Helmets: {
                Drawable: data.helmets?.drawable ?? -1,
                Texture: data.helmets?.texture ?? 0
            },
            Glasses: {
                Drawable: data.glasses?.drawable ?? -1,
                Texture: data.glasses?.texture ?? 0
            },
            EarAccessories: {
                Drawable: data.earAccesories?.drawable ?? -1,
                Texture: data.earAccesories?.texture ?? 0
            },
            Watches: {
                Drawable: data.watches?.drawable ?? -1,
                Texture: data.watches?.texture ?? 0
            },
            Bracelets: {
                Drawable: data.bracelets?.drawable ?? -1,
                Texture: data.bracelets?.texture ?? 0
            }
        };
        
        updatePlayerClothes(clothesData);
    } catch (error) {
        mp.console.logError("Error in CEF:CLIENT:UpdateClothes: " + error);
    }
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

        // Handle props with explicit checks for -1 values
        if (clothesData.Helmets) {
            if (clothesData.Helmets.Drawable === -1) {
                player.clearProp(0);
            } else {
                player.setPropIndex(0, clothesData.Helmets.Drawable, clothesData.Helmets.Texture, true);
            }
        }
        
        if (clothesData.Glasses) {
            if (clothesData.Glasses.Drawable === -1) {
                player.clearProp(1);
            } else {
                player.setPropIndex(1, clothesData.Glasses.Drawable, clothesData.Glasses.Texture, true);
            }
        }
        
        if (clothesData.EarAccessories) {
            if (clothesData.EarAccessories.Drawable === -1) {
                player.clearProp(2);
            } else {
                player.setPropIndex(2, clothesData.EarAccessories.Drawable, clothesData.EarAccessories.Texture, true);
            }
        }
        
        if (clothesData.Watches) {
            if (clothesData.Watches.Drawable === -1) {
                player.clearProp(6);
            } else {
                player.setPropIndex(6, clothesData.Watches.Drawable, clothesData.Watches.Texture, true);
            }
        }
        
        if (clothesData.Bracelets) {
            if (clothesData.Bracelets.Drawable === -1) {
                player.clearProp(7);
            } else {
                player.setPropIndex(7, clothesData.Bracelets.Drawable, clothesData.Bracelets.Texture, true);
            }
        }
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
