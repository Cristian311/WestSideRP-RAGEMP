"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.updatePlayerFaceFeatures = updatePlayerFaceFeatures;
let customizeCefBrowser = null;
mp.events.add("SERVER:CLIENT:ShowCustomizeCharScreen", (genderJson, json) => {
    customizeCefBrowser = mp.browsers.new("package://html/characters/customize_char.html");
    mp.gui.cursor.show(true, true);
    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    customizeCefBrowser.execute(`updateCharacterDataFromJson("${escapedJson}");`);
});
mp.events.add("CEF:CLIENT:ShowCustomizeCharScreen", () => {
    customizeCefBrowser = mp.browsers.new("package://html/characters/customize_char.html");
    mp.gui.cursor.show(true, true);
});
mp.events.add("CEF:CLIENT:CloseCustomizeCharScreen", (detailsJSON) => {
    customizeCefBrowser === null || customizeCefBrowser === void 0 ? void 0 : customizeCefBrowser.destroy();
    customizeCefBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("CLIENT:SERVER:ClosedCustomizeCharScreen", detailsJSON);
});
mp.events.add("CEF:CLIENT:UpdateCharacter", (json) => {
    mp.events.callRemote("CLIENT:SERVER:CharCustomiseUpdate", json);
});
mp.events.add("CEF:CLIENT:UpdateCharacterClient", (json) => {
    const charData = JSON.parse(json);
    updatePlayerFaceFeatures(charData);
});
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
//# sourceMappingURL=customizeHandler.js.map