"use strict";
let charCefBrowser = null;
let faces = [];
let hairs = [];
let clothes = [];
mp.events.add("SERVER:CLIENT:ShowCharInterface", () => {
    charCefBrowser = mp.browsers.new("package://html/characters/characters.html");
    mp.gui.cursor.show(true, true);
    //mp.events.call("CEF:CLIENT:ShowCharInterface");
    mp.gui.chat.show(false);
});
mp.events.add("CEF:CLIENT:ShowCharInterface", () => {
    // The character customization interface will be shown
    charCefBrowser = mp.browsers.new("package://html/characters/characters.html");
    mp.gui.cursor.show(true, true);
    //mp.events.call("CEF:CLIENT:ShowCharInterface");
    mp.gui.chat.show(false);
    //mp.events.call("CEF:CLIENT:ShowCharInterface");
});
mp.events.add("SERVER:CLIENT:CloseCharInterface", () => {
    charCefBrowser === null || charCefBrowser === void 0 ? void 0 : charCefBrowser.destroy();
    charCefBrowser = null;
    mp.gui.cursor.show(false, false);
});
mp.events.add("CEF:CLIENT:CloseCharInterface", () => {
    charCefBrowser === null || charCefBrowser === void 0 ? void 0 : charCefBrowser.destroy();
    charCefBrowser = null;
    mp.gui.cursor.show(false, false);
});
mp.events.add("SERVER:CLIENT:DisplayAccountInfo", (username, namesString, moneyString) => {
    if (charCefBrowser) {
        charCefBrowser.execute(`displayAccountInfo('${username}', ${namesString}, ${moneyString});`);
    }
});
mp.events.add("SERVER:CLIENT:CharactersInfo", (facesJson, hairsJson, clothesJson) => {
    faces = JSON.parse(facesJson);
    hairs = JSON.parse(hairsJson);
    clothes = JSON.parse(clothesJson);
});
mp.events.add("CEF:CLIENT:CharacterPressed", (index) => {
    // Teleport the player to a customization location
    mp.players.local.position = new mp.Vector3(210.71982, 285.08926, 141.92323);
    // Apply the selected character's data
    updateCharacterCustomization(index);
});
mp.events.add("CEF:CLIENT:CharacterSelected", (CharacterName) => {
    mp.events.callRemote("CLIENT:SERVER:CharSelected", CharacterName);
    mp.events.callRemote("CLIENT:SERVER:CustomizeScreenOnCreation");
    mp.gui.chat.show(true);
});
function updateCharacterCustomization(index) {
    try {
        // Check if the arrays exist and the index is valid
        if (!faces || !hairs || !clothes ||
            index < 0 ||
            index >= faces.length ||
            index >= hairs.length ||
            index >= clothes.length) {
            mp.console.logError(`Invalid character index: ${index}`);
            return;
        }
        const character = faces[index];
        const hair = hairs[index];
        const outfit = clothes[index];
        updatePlayerFaceFeatures(character);
        updatePlayerHair(hair);
        updatePlayerClothes(outfit);
    }
    catch (error) {
        mp.console.logError("Error in updateCharacterCustomization: " + error);
    }
}
function updatePlayerFaceFeatures(data) {
    try {
        const player = mp.players.local;
        // Check if data and its properties exist before using them
        if (data && data.Eye && typeof data.Eye.Color !== 'undefined') {
            player.setEyeColor(data.Eye.Color);
        }
        else {
            mp.console.logError("Error: Eye color data is missing or undefined");
            // Set a default eye color to prevent crashes
            player.setEyeColor(0);
        }
        // Check Facial features
        if (data && data.Facial) {
            if (data.Facial.Blemishes) {
                player.setHeadOverlay(0, data.Facial.Blemishes.Value, data.Facial.Blemishes.Opacity, 0, 0);
            }
            if (data.Facial.Ageing) {
                player.setHeadOverlay(3, data.Facial.Ageing.Value, data.Facial.Ageing.Opacity, 0, 0);
            }
            if (data.Facial.Complexion) {
                player.setHeadOverlay(6, data.Facial.Complexion.Value, data.Facial.Complexion.Opacity, 0, 0);
            }
            if (data.Facial.SunDamage) {
                player.setHeadOverlay(7, data.Facial.SunDamage.Value, data.Facial.SunDamage.Opacity, 0, 0);
            }
            if (data.Facial.MolesFreckles) {
                player.setHeadOverlay(9, data.Facial.MolesFreckles.Value, data.Facial.MolesFreckles.Opacity, 0, 0);
            }
        }
        // Check Shape and Skin data
        if (data && data.Shape && data.Skin) {
            player.setHeadBlendData(data.Shape.First, data.Shape.Second, 0, data.Skin.First, data.Skin.Second, 0, data.Shape.Mix, data.Skin.Mix, 0, true);
        }
        // Get face features with safety checks
        const features = getFaceFeatures(data);
        for (let i = 0; i < 20; i++) {
            player.setFaceFeature(i, features[i]);
        }
    }
    catch (error) {
        mp.console.logError("Error in updatePlayerFaceFeatures: " + error);
    }
}
function getFaceFeatures(charData) {
    var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x, _y, _z, _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14;
    try {
        // Default values in case properties are undefined
        const defaultValue = 0;
        // Safely access properties with fallbacks to default values
        const noseWidth = (_b = (_a = charData === null || charData === void 0 ? void 0 : charData.Nose) === null || _a === void 0 ? void 0 : _a.Width) !== null && _b !== void 0 ? _b : defaultValue;
        const noseHeight = (_d = (_c = charData === null || charData === void 0 ? void 0 : charData.Nose) === null || _c === void 0 ? void 0 : _c.Height) !== null && _d !== void 0 ? _d : defaultValue;
        const noseLength = (_f = (_e = charData === null || charData === void 0 ? void 0 : charData.Nose) === null || _e === void 0 ? void 0 : _e.Length) !== null && _f !== void 0 ? _f : defaultValue;
        const noseBridge = (_h = (_g = charData === null || charData === void 0 ? void 0 : charData.Nose) === null || _g === void 0 ? void 0 : _g.Bridge) !== null && _h !== void 0 ? _h : defaultValue;
        const noseTip = (_k = (_j = charData === null || charData === void 0 ? void 0 : charData.Nose) === null || _j === void 0 ? void 0 : _j.Tip) !== null && _k !== void 0 ? _k : defaultValue;
        const noseBridgeShift = (_m = (_l = charData === null || charData === void 0 ? void 0 : charData.Nose) === null || _l === void 0 ? void 0 : _l.BridgeShift) !== null && _m !== void 0 ? _m : defaultValue;
        const browHeight = (_p = (_o = charData === null || charData === void 0 ? void 0 : charData.Brow) === null || _o === void 0 ? void 0 : _o.Height) !== null && _p !== void 0 ? _p : defaultValue;
        const browWidth = (_r = (_q = charData === null || charData === void 0 ? void 0 : charData.Brow) === null || _q === void 0 ? void 0 : _q.Width) !== null && _r !== void 0 ? _r : defaultValue;
        const cheeksHeight = (_t = (_s = charData === null || charData === void 0 ? void 0 : charData.Cheeks) === null || _s === void 0 ? void 0 : _s.Height) !== null && _t !== void 0 ? _t : defaultValue;
        const cheeksWidth = (_v = (_u = charData === null || charData === void 0 ? void 0 : charData.Cheeks) === null || _u === void 0 ? void 0 : _u.Width) !== null && _v !== void 0 ? _v : defaultValue;
        const cheeksCheeksWidth = (_x = (_w = charData === null || charData === void 0 ? void 0 : charData.Cheeks) === null || _w === void 0 ? void 0 : _w.CheeksWidth) !== null && _x !== void 0 ? _x : defaultValue;
        const eyeSize = (_z = (_y = charData === null || charData === void 0 ? void 0 : charData.Eye) === null || _y === void 0 ? void 0 : _y.Size) !== null && _z !== void 0 ? _z : defaultValue;
        const lips = (_0 = charData === null || charData === void 0 ? void 0 : charData.Lips) !== null && _0 !== void 0 ? _0 : defaultValue;
        const jawWidth = (_2 = (_1 = charData === null || charData === void 0 ? void 0 : charData.Jaw) === null || _1 === void 0 ? void 0 : _1.Width) !== null && _2 !== void 0 ? _2 : defaultValue;
        const jawHeight = (_4 = (_3 = charData === null || charData === void 0 ? void 0 : charData.Jaw) === null || _3 === void 0 ? void 0 : _3.Height) !== null && _4 !== void 0 ? _4 : defaultValue;
        const chinLenght = (_6 = (_5 = charData === null || charData === void 0 ? void 0 : charData.Chin) === null || _5 === void 0 ? void 0 : _5.Lenght) !== null && _6 !== void 0 ? _6 : defaultValue;
        const chinPosition = (_8 = (_7 = charData === null || charData === void 0 ? void 0 : charData.Chin) === null || _7 === void 0 ? void 0 : _7.Position) !== null && _8 !== void 0 ? _8 : defaultValue;
        const chinWidth = (_10 = (_9 = charData === null || charData === void 0 ? void 0 : charData.Chin) === null || _9 === void 0 ? void 0 : _9.Width) !== null && _10 !== void 0 ? _10 : defaultValue;
        const chinShape = (_12 = (_11 = charData === null || charData === void 0 ? void 0 : charData.Chin) === null || _11 === void 0 ? void 0 : _11.Shape) !== null && _12 !== void 0 ? _12 : defaultValue;
        const neckWidth = (_14 = (_13 = charData === null || charData === void 0 ? void 0 : charData.Neck) === null || _13 === void 0 ? void 0 : _13.Width) !== null && _14 !== void 0 ? _14 : defaultValue;
        return [
            noseWidth, noseHeight, noseLength,
            noseBridge, noseTip, noseBridgeShift,
            browHeight, browWidth,
            cheeksHeight, cheeksWidth, cheeksCheeksWidth,
            eyeSize, lips,
            jawWidth, jawHeight,
            chinLenght, chinPosition, chinWidth, chinShape,
            neckWidth
        ];
    }
    catch (error) {
        mp.console.logError("Error in getFaceFeatures: " + error);
        // Return an array of 20 zeros as a fallback
        return Array(20).fill(0);
    }
}
function updatePlayerHair(data) {
    try {
        const player = mp.players.local;
        // Check if data and its properties exist before using them
        if (data && data.FacialHair) {
            player.setHeadOverlay(1, data.FacialHair.Value, 1.0, data.FacialHair.Color, data.FacialHair.Color);
        }
        if (data && data.Eyebrows) {
            player.setHeadOverlay(2, data.Eyebrows.Value, 1.0, data.Eyebrows.Color, data.Eyebrows.Color);
        }
        if (data && data.BodyHair) {
            player.setHeadOverlay(10, data.BodyHair.Value, 1.0, data.BodyHair.Color, data.BodyHair.Color);
        }
        if (data && data.FacialHair) {
            player.setHeadOverlayColor(1, 1, data.FacialHair.Color, 0);
        }
        if (data && data.Eyebrows) {
            player.setHeadOverlayColor(2, 1, data.Eyebrows.Color, 0);
        }
        if (data && data.BodyHair) {
            player.setHeadOverlayColor(10, 1, data.BodyHair.Color, 0);
        }
        if (data && data.Hair) {
            player.setComponentVariation(2, data.Hair.Value, 0, 0);
            player.setHairColor(data.Hair.Color, data.Hair.SecondColor);
        }
    }
    catch (error) {
        mp.console.logError("Error in updatePlayerHair: " + error);
    }
}
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
        const clearOrSetProp = (slot, drawable, texture) => {
            if (drawable === -1) {
                player.clearProp(slot);
            }
            else {
                player.setPropIndex(slot, drawable, texture, true);
            }
        };
        // Safely set props with null checks
        if (clothesData.Helmets)
            clearOrSetProp(0, clothesData.Helmets.Drawable, clothesData.Helmets.Texture);
        if (clothesData.Glasses)
            clearOrSetProp(1, clothesData.Glasses.Drawable, clothesData.Glasses.Texture);
        if (clothesData.EarAccessories)
            clearOrSetProp(2, clothesData.EarAccessories.Drawable, clothesData.EarAccessories.Texture);
        if (clothesData.Watches)
            clearOrSetProp(6, clothesData.Watches.Drawable, clothesData.Watches.Texture);
        if (clothesData.Bracelets)
            clearOrSetProp(7, clothesData.Bracelets.Drawable, clothesData.Bracelets.Texture);
    }
    catch (error) {
        mp.console.logError("Error in updatePlayerClothes: " + error);
    }
}
//# sourceMappingURL=characterScreenHandler.js.map