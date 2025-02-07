"use strict";
let tattooCefBrowser = null;
let tattooCollection = {};
let tattooHash = {};
mp.events.add("SERVER:CLIENT:ShowTattooScreen", (json, usedTattoosJson, tattooCollectionJson, tattooHashJson) => {
    tattooCefBrowser = mp.browsers.new("package://html/tattoos/tattoos.html");
    mp.gui.cursor.show(true, true);
    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    tattooCefBrowser.execute(`updateTattooData("${escapedJson}");`);
    const escapedUsedTattoosJson = usedTattoosJson.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    tattooCefBrowser.execute(`updateHighlights("${escapedUsedTattoosJson}");`);
    tattooCollection = JSON.parse(tattooCollectionJson);
    tattooHash = JSON.parse(tattooHashJson);
});
mp.events.add("CEF:CLIENT:StopTattooScreen", (json) => {
    tattooCefBrowser === null || tattooCefBrowser === void 0 ? void 0 : tattooCefBrowser.destroy();
    tattooCefBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("CLIENT:SERVER:UpdateTattoo", json);
});
mp.events.add("CEF:CLIENT:ApplyTattoo", (tattooName) => {
    const collectionHash = tattooCollection[tattooName];
    const hashHash = tattooHash[tattooName];
    const collection = mp.game.joaat(collectionHash);
    const hash = mp.game.joaat(hashHash);
    mp.players.local.setDecoration(collection, hash);
    mp.events.callRemote("CLIENT:SERVER:ApplyTattoo", tattooName);
});
mp.events.add("CEF:CLIENT:RemoveTattoo", (tattooName) => {
    mp.events.callRemote("CLIENT:SERVER:RemoveTattoo", tattooName);
});
mp.events.add("CEF:CLIENT:ApplyTattooClient", (json) => {
    const tattooData = JSON.parse(json);
    applyTattoo(tattooData);
});
mp.events.add("CEF:CLIENT:RemoveTattooClient", (json) => {
    const tattooData = JSON.parse(json);
    removeTattoo(tattooData);
});
function applyTattoo(tattooData) {
    for (let tattooName of tattooData.UsedTattoos) {
        const collection = mp.game.joaat(tattooCollection[tattooName]);
        const hash = mp.game.joaat(tattooHash[tattooName]);
        mp.players.local.setDecoration(collection, hash);
    }
}
function removeTattoo(tattooData) {
    mp.players.local.clearDecorations();
    for (let tattooName of tattooData.UsedTattoos) {
        const collection = mp.game.joaat(tattooCollection[tattooName]);
        const hash = mp.game.joaat(tattooHash[tattooName]);
        mp.players.local.setDecoration(collection, hash);
    }
}
//# sourceMappingURL=tattooHandler.js.map