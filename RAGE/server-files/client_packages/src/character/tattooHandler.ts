interface TattooData {
    TattooCollection: { [key: string]: string };
    TattooHash: { [key: string]: string };
    UsedTattoos: string[];
}

let tattooCefBrowser: BrowserMp | null = null;
let tattooCollection: { [key: string]: string } = {};
let tattooHash: { [key: string]: string } = {};

mp.events.add("SERVER:CLIENT:ShowTattooScreen", (json: string, usedTattoosJson: string, tattooCollectionJson: string, tattooHashJson: string) => {
    tattooCefBrowser = mp.browsers.new("package://html/tattoos/tattoos.html");
    mp.gui.cursor.show(true, true);

    const escapedJson = json.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    tattooCefBrowser.execute(`updateTattooData("${escapedJson}");`);

    const escapedUsedTattoosJson = usedTattoosJson.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
    tattooCefBrowser.execute(`updateHighlights("${escapedUsedTattoosJson}");`);

    tattooCollection = JSON.parse(tattooCollectionJson);
    tattooHash = JSON.parse(tattooHashJson);
});

mp.events.add("CEF:CLIENT:StopTattooScreen", (json: string) => {
    tattooCefBrowser?.destroy();
    tattooCefBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("CLIENT:SERVER:UpdateTattoo", json);
});

mp.events.add("CEF:CLIENT:ApplyTattoo", (tattooName: string) => {
    const collectionHash = tattooCollection[tattooName];
    const hashHash = tattooHash[tattooName];
    const collection = mp.game.joaat(collectionHash);
    const hash = mp.game.joaat(hashHash);
    mp.players.local.setDecoration(collection, hash);
    mp.events.callRemote("CLIENT:SERVER:ApplyTattoo", tattooName);
});

mp.events.add("CEF:CLIENT:RemoveTattoo", (tattooName: string) => {
    mp.events.callRemote("CLIENT:SERVER:RemoveTattoo", tattooName);
});

mp.events.add("CEF:CLIENT:ApplyTattooClient", (json: string) => {
    const tattooData: TattooData = JSON.parse(json);
    applyTattoo(tattooData);
});

mp.events.add("CEF:CLIENT:RemoveTattooClient", (json: string) => {
    const tattooData: TattooData = JSON.parse(json);
    removeTattoo(tattooData);
});

function applyTattoo(tattooData: TattooData) {
    for (let tattooName of tattooData.UsedTattoos) {
        const collection = mp.game.joaat(tattooCollection[tattooName]);
        const hash = mp.game.joaat(tattooHash[tattooName]);
        mp.players.local.setDecoration(collection, hash);
    }
}

function removeTattoo(tattooData: TattooData) {
    mp.players.local.clearDecorations();

    for (let tattooName of tattooData.UsedTattoos) {
        const collection = mp.game.joaat(tattooCollection[tattooName]);
        const hash = mp.game.joaat(tattooHash[tattooName]);
        mp.players.local.setDecoration(collection, hash);
    }
}
