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
    const character = faces[index];
    const hair = hairs[index];
    const outfit = clothes[index];
    updatePlayerFaceFeatures(character);
    updatePlayerHair(hair);
    updatePlayerClothes(outfit);
}
function updatePlayerFaceFeatures(data) {
    const player = mp.players.local;
    player.setEyeColor(data.Eye.Color);
    player.setHeadOverlay(0, data.Facial.Blemishes.Value, data.Facial.Blemishes.Opacity, 0, 0);
    player.setHeadOverlay(3, data.Facial.Ageing.Value, data.Facial.Ageing.Opacity, 0, 0);
    player.setHeadOverlay(6, data.Facial.Complexion.Value, data.Facial.Complexion.Opacity, 0, 0);
    player.setHeadOverlay(7, data.Facial.SunDamage.Value, data.Facial.SunDamage.Opacity, 0, 0);
    player.setHeadOverlay(9, data.Facial.MolesFreckles.Value, data.Facial.MolesFreckles.Opacity, 0, 0);
    player.setHeadBlendData(data.Shape.First, data.Shape.Second, 0, data.Skin.First, data.Skin.Second, 0, data.Shape.Mix, data.Skin.Mix, 0, true);
    const features = getFaceFeatures(data);
    for (let i = 0; i < 20; i++) {
        player.setFaceFeature(i, features[i]);
    }
}
function getFaceFeatures(charData) {
    return [
        charData.Nose.Width, charData.Nose.Height, charData.Nose.Length,
        charData.Nose.Bridge, charData.Nose.Tip, charData.Nose.BridgeShift,
        charData.Brow.Height, charData.Brow.Width,
        charData.Cheeks.Height, charData.Cheeks.Width, charData.Cheeks.CheeksWidth,
        charData.Eye.Size, charData.Lips,
        charData.Jaw.Width, charData.Jaw.Height,
        charData.Chin.Lenght, charData.Chin.Position, charData.Chin.Width, charData.Chin.Shape,
        charData.Neck.Width
    ];
}
function updatePlayerHair(data) {
    const player = mp.players.local;
    player.setHeadOverlay(1, data.FacialHair.Value, 1.0, data.FacialHair.Color, data.FacialHair.Color);
    player.setHeadOverlay(2, data.Eyebrows.Value, 1.0, data.FacialHair.Color, data.FacialHair.Color);
    player.setHeadOverlay(10, data.BodyHair.Value, 1.0, data.FacialHair.Color, data.FacialHair.Color);
    player.setHeadOverlayColor(1, 1, data.FacialHair.Color, 0);
    player.setHeadOverlayColor(2, 1, data.Eyebrows.Color, 0);
    player.setHeadOverlayColor(10, 1, data.BodyHair.Color, 0);
    player.setComponentVariation(2, data.Hair.Value, 0, 0);
    player.setHairColor(data.Hair.Color, data.Hair.SecondColor);
}
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
//# sourceMappingURL=characterScreenHandler.js.map