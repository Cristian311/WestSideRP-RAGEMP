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
    var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x, _y, _z, _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15, _16, _17, _18, _19, _20, _21, _22, _23, _24, _25, _26, _27, _28, _29, _30, _31, _32, _33, _34, _35;
    try {
        const data = JSON.parse(json);
        // Transform the data to match our ClothesData interface
        const clothesData = {
            Mask: {
                Drawable: (_b = (_a = data.masks) === null || _a === void 0 ? void 0 : _a.drawable) !== null && _b !== void 0 ? _b : 0,
                Texture: (_d = (_c = data.masks) === null || _c === void 0 ? void 0 : _c.texture) !== null && _d !== void 0 ? _d : 0
            },
            Torso: {
                Drawable: (_f = (_e = data.torsos) === null || _e === void 0 ? void 0 : _e.drawable) !== null && _f !== void 0 ? _f : 0,
                Texture: (_h = (_g = data.torsos) === null || _g === void 0 ? void 0 : _g.texture) !== null && _h !== void 0 ? _h : 0
            },
            Legs: {
                Drawable: (_k = (_j = data.legs) === null || _j === void 0 ? void 0 : _j.drawable) !== null && _k !== void 0 ? _k : 0,
                Texture: (_m = (_l = data.legs) === null || _l === void 0 ? void 0 : _l.texture) !== null && _m !== void 0 ? _m : 0
            },
            Bag: {
                Drawable: (_p = (_o = data.bags) === null || _o === void 0 ? void 0 : _o.drawable) !== null && _p !== void 0 ? _p : 0,
                Texture: (_r = (_q = data.bags) === null || _q === void 0 ? void 0 : _q.texture) !== null && _r !== void 0 ? _r : 0
            },
            Shoes: {
                Drawable: (_t = (_s = data.shoes) === null || _s === void 0 ? void 0 : _s.drawable) !== null && _t !== void 0 ? _t : 0,
                Texture: (_v = (_u = data.shoes) === null || _u === void 0 ? void 0 : _u.texture) !== null && _v !== void 0 ? _v : 0
            },
            Accessories: {
                Drawable: (_x = (_w = data.accessories) === null || _w === void 0 ? void 0 : _w.drawable) !== null && _x !== void 0 ? _x : 0,
                Texture: (_z = (_y = data.accessories) === null || _y === void 0 ? void 0 : _y.texture) !== null && _z !== void 0 ? _z : 0
            },
            Undershirt: {
                Drawable: (_1 = (_0 = data.undershirts) === null || _0 === void 0 ? void 0 : _0.drawable) !== null && _1 !== void 0 ? _1 : 0,
                Texture: (_3 = (_2 = data.undershirts) === null || _2 === void 0 ? void 0 : _2.texture) !== null && _3 !== void 0 ? _3 : 0
            },
            BodyArmor: {
                Drawable: (_5 = (_4 = data.bodyArmor) === null || _4 === void 0 ? void 0 : _4.drawable) !== null && _5 !== void 0 ? _5 : 0,
                Texture: (_7 = (_6 = data.bodyArmor) === null || _6 === void 0 ? void 0 : _6.texture) !== null && _7 !== void 0 ? _7 : 0
            },
            Decals: {
                Drawable: (_9 = (_8 = data.decals) === null || _8 === void 0 ? void 0 : _8.drawable) !== null && _9 !== void 0 ? _9 : 0,
                Texture: (_11 = (_10 = data.decals) === null || _10 === void 0 ? void 0 : _10.texture) !== null && _11 !== void 0 ? _11 : 0
            },
            Shirts: {
                Drawable: (_13 = (_12 = data.shirts) === null || _12 === void 0 ? void 0 : _12.drawable) !== null && _13 !== void 0 ? _13 : 0,
                Texture: (_15 = (_14 = data.shirts) === null || _14 === void 0 ? void 0 : _14.texture) !== null && _15 !== void 0 ? _15 : 0
            },
            Helmets: {
                Drawable: (_17 = (_16 = data.helmets) === null || _16 === void 0 ? void 0 : _16.drawable) !== null && _17 !== void 0 ? _17 : -1,
                Texture: (_19 = (_18 = data.helmets) === null || _18 === void 0 ? void 0 : _18.texture) !== null && _19 !== void 0 ? _19 : 0
            },
            Glasses: {
                Drawable: (_21 = (_20 = data.glasses) === null || _20 === void 0 ? void 0 : _20.drawable) !== null && _21 !== void 0 ? _21 : -1,
                Texture: (_23 = (_22 = data.glasses) === null || _22 === void 0 ? void 0 : _22.texture) !== null && _23 !== void 0 ? _23 : 0
            },
            EarAccessories: {
                Drawable: (_25 = (_24 = data.earAccesories) === null || _24 === void 0 ? void 0 : _24.drawable) !== null && _25 !== void 0 ? _25 : -1,
                Texture: (_27 = (_26 = data.earAccesories) === null || _26 === void 0 ? void 0 : _26.texture) !== null && _27 !== void 0 ? _27 : 0
            },
            Watches: {
                Drawable: (_29 = (_28 = data.watches) === null || _28 === void 0 ? void 0 : _28.drawable) !== null && _29 !== void 0 ? _29 : -1,
                Texture: (_31 = (_30 = data.watches) === null || _30 === void 0 ? void 0 : _30.texture) !== null && _31 !== void 0 ? _31 : 0
            },
            Bracelets: {
                Drawable: (_33 = (_32 = data.bracelets) === null || _32 === void 0 ? void 0 : _32.drawable) !== null && _33 !== void 0 ? _33 : -1,
                Texture: (_35 = (_34 = data.bracelets) === null || _34 === void 0 ? void 0 : _34.texture) !== null && _35 !== void 0 ? _35 : 0
            }
        };
        updatePlayerClothes(clothesData);
    }
    catch (error) {
        mp.console.logError("Error in CEF:CLIENT:UpdateClothes: " + error);
    }
});
function updatePlayerClothes(clothesData) {
    try {
        const player = mp.players.local;
        // Check if clothesData exists before using it
        if (!clothesData) {
            mp.console.logError("Clothes data is undefined");
            return;
        }
        // Safely set component variations with null checks
        if (clothesData.Mask)
            player.setComponentVariation(1, clothesData.Mask.Drawable, clothesData.Mask.Texture, 0);
        if (clothesData.Torso)
            player.setComponentVariation(3, clothesData.Torso.Drawable, clothesData.Torso.Texture, 0);
        if (clothesData.Legs)
            player.setComponentVariation(4, clothesData.Legs.Drawable, clothesData.Legs.Texture, 0);
        if (clothesData.Bag)
            player.setComponentVariation(5, clothesData.Bag.Drawable, clothesData.Bag.Texture, 0);
        if (clothesData.Shoes)
            player.setComponentVariation(6, clothesData.Shoes.Drawable, clothesData.Shoes.Texture, 0);
        if (clothesData.Accessories)
            player.setComponentVariation(7, clothesData.Accessories.Drawable, clothesData.Accessories.Texture, 0);
        if (clothesData.Undershirt)
            player.setComponentVariation(8, clothesData.Undershirt.Drawable, clothesData.Undershirt.Texture, 0);
        if (clothesData.BodyArmor)
            player.setComponentVariation(9, clothesData.BodyArmor.Drawable, clothesData.BodyArmor.Texture, 0);
        if (clothesData.Decals)
            player.setComponentVariation(10, clothesData.Decals.Drawable, clothesData.Decals.Texture, 0);
        if (clothesData.Shirts)
            player.setComponentVariation(11, clothesData.Shirts.Drawable, clothesData.Shirts.Texture, 0);
        // Handle props with explicit checks for -1 values
        if (clothesData.Helmets) {
            if (clothesData.Helmets.Drawable === -1) {
                player.clearProp(0);
            }
            else {
                player.setPropIndex(0, clothesData.Helmets.Drawable, clothesData.Helmets.Texture, true);
            }
        }
        if (clothesData.Glasses) {
            if (clothesData.Glasses.Drawable === -1) {
                player.clearProp(1);
            }
            else {
                player.setPropIndex(1, clothesData.Glasses.Drawable, clothesData.Glasses.Texture, true);
            }
        }
        if (clothesData.EarAccessories) {
            if (clothesData.EarAccessories.Drawable === -1) {
                player.clearProp(2);
            }
            else {
                player.setPropIndex(2, clothesData.EarAccessories.Drawable, clothesData.EarAccessories.Texture, true);
            }
        }
        if (clothesData.Watches) {
            if (clothesData.Watches.Drawable === -1) {
                player.clearProp(6);
            }
            else {
                player.setPropIndex(6, clothesData.Watches.Drawable, clothesData.Watches.Texture, true);
            }
        }
        if (clothesData.Bracelets) {
            if (clothesData.Bracelets.Drawable === -1) {
                player.clearProp(7);
            }
            else {
                player.setPropIndex(7, clothesData.Bracelets.Drawable, clothesData.Bracelets.Texture, true);
            }
        }
    }
    catch (error) {
        mp.console.logError("Error in updatePlayerClothes: " + error);
    }
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