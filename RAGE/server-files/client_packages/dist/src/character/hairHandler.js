let hairCefBrowser = null;
mp.events.add("SERVER:CLIENT:ShowHairScreen", (json) => {
    hairCefBrowser = mp.browsers.new("package://html/characters/customize_hair.html");
    mp.gui.cursor.show(true, true);
    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    hairCefBrowser.execute(`updateHairDataFromJson("${escapedJson}");`);
});
mp.events.add("CEF:CLIENT:ShowHairScreen", () => {
    hairCefBrowser = mp.browsers.new("package://html/characters/customize_hair.html");
    mp.gui.cursor.show(true, true);
});
mp.events.add("CEF:CLIENT:CloseHairScreen", (json) => {
    hairCefBrowser === null || hairCefBrowser === void 0 ? void 0 : hairCefBrowser.destroy();
    hairCefBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("CLIENT:SERVER:ClosedHairScreen", json);
});
mp.events.add("CEF:CLIENT:UpdateHair", (json) => {
    mp.events.callRemote("CLIENT:SERVER:UpdateHair", json);
});
mp.events.add("CEF:CLIENT:UpdateHairClient", (json) => {
    const hairData = JSON.parse(json);
    updatePlayerHair(hairData);
});
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
export { updatePlayerHair };
//# sourceMappingURL=hairHandler.js.map