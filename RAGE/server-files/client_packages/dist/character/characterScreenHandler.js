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
    try {
        // Parse the JSON data
        const facesData = JSON.parse(facesJson);
        const hairsData = JSON.parse(hairsJson);
        const clothesData = JSON.parse(clothesJson);
        // Transform the data to match our interfaces
        faces = facesData.map((face) => {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x, _y, _z, _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15, _16, _17, _18, _19, _20, _21, _22, _23, _24, _25, _26, _27, _28, _29, _30, _31, _32, _33, _34, _35, _36, _37, _38, _39, _40, _41, _42, _43, _44, _45, _46, _47, _48, _49, _50, _51, _52, _53, _54, _55, _56, _57, _58;
            return ({
                Eye: {
                    Color: (_b = (_a = face.eye) === null || _a === void 0 ? void 0 : _a.color) !== null && _b !== void 0 ? _b : 0,
                    Size: (_d = (_c = face.eye) === null || _c === void 0 ? void 0 : _c.size) !== null && _d !== void 0 ? _d : 0
                },
                Facial: {
                    Blemishes: {
                        Value: (_g = (_f = (_e = face.facialDetails) === null || _e === void 0 ? void 0 : _e.blemishes) === null || _f === void 0 ? void 0 : _f.value) !== null && _g !== void 0 ? _g : 0,
                        Opacity: (_k = (_j = (_h = face.facialDetails) === null || _h === void 0 ? void 0 : _h.blemishes) === null || _j === void 0 ? void 0 : _j.opacity) !== null && _k !== void 0 ? _k : 0
                    },
                    Ageing: {
                        Value: (_o = (_m = (_l = face.facialDetails) === null || _l === void 0 ? void 0 : _l.ageing) === null || _m === void 0 ? void 0 : _m.value) !== null && _o !== void 0 ? _o : 0,
                        Opacity: (_r = (_q = (_p = face.facialDetails) === null || _p === void 0 ? void 0 : _p.ageing) === null || _q === void 0 ? void 0 : _q.opacity) !== null && _r !== void 0 ? _r : 0
                    },
                    Complexion: {
                        Value: (_u = (_t = (_s = face.facialDetails) === null || _s === void 0 ? void 0 : _s.complexion) === null || _t === void 0 ? void 0 : _t.value) !== null && _u !== void 0 ? _u : 0,
                        Opacity: (_x = (_w = (_v = face.facialDetails) === null || _v === void 0 ? void 0 : _v.complexion) === null || _w === void 0 ? void 0 : _w.opacity) !== null && _x !== void 0 ? _x : 0
                    },
                    SunDamage: {
                        Value: (_0 = (_z = (_y = face.facialDetails) === null || _y === void 0 ? void 0 : _y.sunDamage) === null || _z === void 0 ? void 0 : _z.value) !== null && _0 !== void 0 ? _0 : 0,
                        Opacity: (_3 = (_2 = (_1 = face.facialDetails) === null || _1 === void 0 ? void 0 : _1.sunDamage) === null || _2 === void 0 ? void 0 : _2.opacity) !== null && _3 !== void 0 ? _3 : 0
                    },
                    MolesFreckles: {
                        Value: (_6 = (_5 = (_4 = face.facialDetails) === null || _4 === void 0 ? void 0 : _4.molesFreckles) === null || _5 === void 0 ? void 0 : _5.value) !== null && _6 !== void 0 ? _6 : 0,
                        Opacity: (_9 = (_8 = (_7 = face.facialDetails) === null || _7 === void 0 ? void 0 : _7.molesFreckles) === null || _8 === void 0 ? void 0 : _8.opacity) !== null && _9 !== void 0 ? _9 : 0
                    }
                },
                Shape: {
                    First: (_11 = (_10 = face.shape) === null || _10 === void 0 ? void 0 : _10.first) !== null && _11 !== void 0 ? _11 : 0,
                    Second: (_13 = (_12 = face.shape) === null || _12 === void 0 ? void 0 : _12.second) !== null && _13 !== void 0 ? _13 : 0,
                    Mix: (_15 = (_14 = face.shape) === null || _14 === void 0 ? void 0 : _14.mix) !== null && _15 !== void 0 ? _15 : 0
                },
                Skin: {
                    First: (_17 = (_16 = face.skin) === null || _16 === void 0 ? void 0 : _16.first) !== null && _17 !== void 0 ? _17 : 0,
                    Second: (_19 = (_18 = face.skin) === null || _18 === void 0 ? void 0 : _18.second) !== null && _19 !== void 0 ? _19 : 0,
                    Mix: (_21 = (_20 = face.skin) === null || _20 === void 0 ? void 0 : _20.mix) !== null && _21 !== void 0 ? _21 : 0
                },
                Nose: {
                    Width: (_23 = (_22 = face.nose) === null || _22 === void 0 ? void 0 : _22.width) !== null && _23 !== void 0 ? _23 : 0,
                    Height: (_25 = (_24 = face.nose) === null || _24 === void 0 ? void 0 : _24.height) !== null && _25 !== void 0 ? _25 : 0,
                    Length: (_27 = (_26 = face.nose) === null || _26 === void 0 ? void 0 : _26.length) !== null && _27 !== void 0 ? _27 : 0,
                    Bridge: (_29 = (_28 = face.nose) === null || _28 === void 0 ? void 0 : _28.bridge) !== null && _29 !== void 0 ? _29 : 0,
                    Tip: (_31 = (_30 = face.nose) === null || _30 === void 0 ? void 0 : _30.tip) !== null && _31 !== void 0 ? _31 : 0,
                    BridgeShift: (_33 = (_32 = face.nose) === null || _32 === void 0 ? void 0 : _32.bridgeShift) !== null && _33 !== void 0 ? _33 : 0
                },
                Brow: {
                    Height: (_35 = (_34 = face.brow) === null || _34 === void 0 ? void 0 : _34.height) !== null && _35 !== void 0 ? _35 : 0,
                    Width: (_37 = (_36 = face.brow) === null || _36 === void 0 ? void 0 : _36.width) !== null && _37 !== void 0 ? _37 : 0
                },
                Cheeks: {
                    Height: (_39 = (_38 = face.cheeks) === null || _38 === void 0 ? void 0 : _38.height) !== null && _39 !== void 0 ? _39 : 0,
                    Width: (_41 = (_40 = face.cheeks) === null || _40 === void 0 ? void 0 : _40.width) !== null && _41 !== void 0 ? _41 : 0,
                    CheeksWidth: (_43 = (_42 = face.cheeks) === null || _42 === void 0 ? void 0 : _42.cheekswidth) !== null && _43 !== void 0 ? _43 : 0
                },
                Lips: (_44 = face.lips) !== null && _44 !== void 0 ? _44 : 0,
                Jaw: {
                    Width: (_46 = (_45 = face.jaw) === null || _45 === void 0 ? void 0 : _45.width) !== null && _46 !== void 0 ? _46 : 0,
                    Height: (_48 = (_47 = face.jaw) === null || _47 === void 0 ? void 0 : _47.height) !== null && _48 !== void 0 ? _48 : 0
                },
                Chin: {
                    Lenght: (_50 = (_49 = face.chin) === null || _49 === void 0 ? void 0 : _49.length) !== null && _50 !== void 0 ? _50 : 0,
                    Position: (_52 = (_51 = face.chin) === null || _51 === void 0 ? void 0 : _51.position) !== null && _52 !== void 0 ? _52 : 0,
                    Width: (_54 = (_53 = face.chin) === null || _53 === void 0 ? void 0 : _53.width) !== null && _54 !== void 0 ? _54 : 0,
                    Shape: (_56 = (_55 = face.chin) === null || _55 === void 0 ? void 0 : _55.shape) !== null && _56 !== void 0 ? _56 : 0
                },
                Neck: {
                    Width: (_58 = (_57 = face.neck) === null || _57 === void 0 ? void 0 : _57.width) !== null && _58 !== void 0 ? _58 : 0
                }
            });
        });
        hairs = hairsData.map((hair) => {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t;
            return ({
                Hair: {
                    Value: (_b = (_a = hair.hair) === null || _a === void 0 ? void 0 : _a.value) !== null && _b !== void 0 ? _b : 0,
                    Color: (_d = (_c = hair.hair) === null || _c === void 0 ? void 0 : _c.color) !== null && _d !== void 0 ? _d : 0,
                    SecondColor: (_f = (_e = hair.hair) === null || _e === void 0 ? void 0 : _e.secondColor) !== null && _f !== void 0 ? _f : 0
                },
                FacialHair: {
                    Value: (_h = (_g = hair.facialHair) === null || _g === void 0 ? void 0 : _g.value) !== null && _h !== void 0 ? _h : 0,
                    Color: (_k = (_j = hair.facialHair) === null || _j === void 0 ? void 0 : _j.color) !== null && _k !== void 0 ? _k : 0
                },
                Eyebrows: {
                    Value: (_m = (_l = hair.eyebrows) === null || _l === void 0 ? void 0 : _l.value) !== null && _m !== void 0 ? _m : 0,
                    Color: (_p = (_o = hair.eyebrows) === null || _o === void 0 ? void 0 : _o.color) !== null && _p !== void 0 ? _p : 0
                },
                BodyHair: {
                    Value: (_r = (_q = hair.bodyHair) === null || _q === void 0 ? void 0 : _q.value) !== null && _r !== void 0 ? _r : 0,
                    Color: (_t = (_s = hair.bodyHair) === null || _s === void 0 ? void 0 : _s.color) !== null && _t !== void 0 ? _t : 0
                }
            });
        });
        clothes = clothesData.map((cloth) => {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x, _y, _z, _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15, _16, _17, _18, _19, _20, _21, _22, _23, _24, _25, _26, _27, _28, _29, _30, _31, _32, _33, _34, _35;
            if (!cloth)
                return null;
            return {
                Mask: {
                    Drawable: (_b = (_a = cloth.masks) === null || _a === void 0 ? void 0 : _a.drawable) !== null && _b !== void 0 ? _b : 0,
                    Texture: (_d = (_c = cloth.masks) === null || _c === void 0 ? void 0 : _c.texture) !== null && _d !== void 0 ? _d : 0
                },
                Torso: {
                    Drawable: (_f = (_e = cloth.torsos) === null || _e === void 0 ? void 0 : _e.drawable) !== null && _f !== void 0 ? _f : 0,
                    Texture: (_h = (_g = cloth.torsos) === null || _g === void 0 ? void 0 : _g.texture) !== null && _h !== void 0 ? _h : 0
                },
                Legs: {
                    Drawable: (_k = (_j = cloth.legs) === null || _j === void 0 ? void 0 : _j.drawable) !== null && _k !== void 0 ? _k : 0,
                    Texture: (_m = (_l = cloth.legs) === null || _l === void 0 ? void 0 : _l.texture) !== null && _m !== void 0 ? _m : 0
                },
                Bag: {
                    Drawable: (_p = (_o = cloth.bags) === null || _o === void 0 ? void 0 : _o.drawable) !== null && _p !== void 0 ? _p : 0,
                    Texture: (_r = (_q = cloth.bags) === null || _q === void 0 ? void 0 : _q.texture) !== null && _r !== void 0 ? _r : 0
                },
                Shoes: {
                    Drawable: (_t = (_s = cloth.shoes) === null || _s === void 0 ? void 0 : _s.drawable) !== null && _t !== void 0 ? _t : 0,
                    Texture: (_v = (_u = cloth.shoes) === null || _u === void 0 ? void 0 : _u.texture) !== null && _v !== void 0 ? _v : 0
                },
                Accessories: {
                    Drawable: (_x = (_w = cloth.accessories) === null || _w === void 0 ? void 0 : _w.drawable) !== null && _x !== void 0 ? _x : 0,
                    Texture: (_z = (_y = cloth.accessories) === null || _y === void 0 ? void 0 : _y.texture) !== null && _z !== void 0 ? _z : 0
                },
                Undershirt: {
                    Drawable: (_1 = (_0 = cloth.undershirts) === null || _0 === void 0 ? void 0 : _0.drawable) !== null && _1 !== void 0 ? _1 : 0,
                    Texture: (_3 = (_2 = cloth.undershirts) === null || _2 === void 0 ? void 0 : _2.texture) !== null && _3 !== void 0 ? _3 : 0
                },
                BodyArmor: {
                    Drawable: (_5 = (_4 = cloth.bodyArmor) === null || _4 === void 0 ? void 0 : _4.drawable) !== null && _5 !== void 0 ? _5 : 0,
                    Texture: (_7 = (_6 = cloth.bodyArmor) === null || _6 === void 0 ? void 0 : _6.texture) !== null && _7 !== void 0 ? _7 : 0
                },
                Decals: {
                    Drawable: (_9 = (_8 = cloth.decals) === null || _8 === void 0 ? void 0 : _8.drawable) !== null && _9 !== void 0 ? _9 : 0,
                    Texture: (_11 = (_10 = cloth.decals) === null || _10 === void 0 ? void 0 : _10.texture) !== null && _11 !== void 0 ? _11 : 0
                },
                Shirts: {
                    Drawable: (_13 = (_12 = cloth.shirts) === null || _12 === void 0 ? void 0 : _12.drawable) !== null && _13 !== void 0 ? _13 : 0,
                    Texture: (_15 = (_14 = cloth.shirts) === null || _14 === void 0 ? void 0 : _14.texture) !== null && _15 !== void 0 ? _15 : 0
                },
                Helmets: {
                    Drawable: (_17 = (_16 = cloth.helmets) === null || _16 === void 0 ? void 0 : _16.drawable) !== null && _17 !== void 0 ? _17 : -1,
                    Texture: (_19 = (_18 = cloth.helmets) === null || _18 === void 0 ? void 0 : _18.texture) !== null && _19 !== void 0 ? _19 : 0
                },
                Glasses: {
                    Drawable: (_21 = (_20 = cloth.glasses) === null || _20 === void 0 ? void 0 : _20.drawable) !== null && _21 !== void 0 ? _21 : -1,
                    Texture: (_23 = (_22 = cloth.glasses) === null || _22 === void 0 ? void 0 : _22.texture) !== null && _23 !== void 0 ? _23 : 0
                },
                EarAccessories: {
                    Drawable: (_25 = (_24 = cloth.earAccesories) === null || _24 === void 0 ? void 0 : _24.drawable) !== null && _25 !== void 0 ? _25 : -1,
                    Texture: (_27 = (_26 = cloth.earAccesories) === null || _26 === void 0 ? void 0 : _26.texture) !== null && _27 !== void 0 ? _27 : 0
                },
                Watches: {
                    Drawable: (_29 = (_28 = cloth.watches) === null || _28 === void 0 ? void 0 : _28.drawable) !== null && _29 !== void 0 ? _29 : -1,
                    Texture: (_31 = (_30 = cloth.watches) === null || _30 === void 0 ? void 0 : _30.texture) !== null && _31 !== void 0 ? _31 : 0
                },
                Bracelets: {
                    Drawable: (_33 = (_32 = cloth.bracelets) === null || _32 === void 0 ? void 0 : _32.drawable) !== null && _33 !== void 0 ? _33 : -1,
                    Texture: (_35 = (_34 = cloth.bracelets) === null || _34 === void 0 ? void 0 : _34.texture) !== null && _35 !== void 0 ? _35 : 0
                }
            };
        }).filter(Boolean);
        // Debug output to help diagnose issues
        mp.console.logInfo(`Faces: ${faces.length}, Hairs: ${hairs.length}, Clothes: ${clothes.length}`);
    }
    catch (error) {
        mp.console.logError("Error parsing character data: " + error);
        mp.console.logError("Faces JSON: " + facesJson.substring(0, 200) + "...");
        mp.console.logError("Hairs JSON: " + hairsJson.substring(0, 200) + "...");
        mp.console.logError("Clothes JSON: " + clothesJson.substring(0, 200) + "...");
    }
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
            player.setHeadOverlay(1, data.FacialHair.Value, 1.0, 0, 0);
            player.setHeadOverlayColor(1, 1, data.FacialHair.Color, 0);
        }
        if (data && data.Eyebrows) {
            player.setHeadOverlay(2, data.Eyebrows.Value, 1.0, 0, 0);
            player.setHeadOverlayColor(2, 1, data.Eyebrows.Color, 0);
        }
        if (data && data.BodyHair) {
            player.setHeadOverlay(10, data.BodyHair.Value, 1.0, 0, 0);
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
//# sourceMappingURL=characterScreenHandler.js.map