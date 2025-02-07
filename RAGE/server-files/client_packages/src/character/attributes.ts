let attributesCefBrowser: BrowserMp | null = null;

mp.events.add("SERVER:CLIENT:OpenAttributesScreen", (darkmode: boolean) => {
    attributesCefBrowser = mp.browsers.new("http://package/html/attributes/attributes.html");
    attributesCefBrowser.active = true;
    mp.gui.cursor.visible = true;
    
    if (darkmode) {
        attributesCefBrowser.execute("toggleDarkMode();");
    }
});

mp.events.add("CEF:CLIENT:AttributesUpdate", (data: string) => {
    mp.events.callRemote("CLIENT:SERVER:AttributesUpdate", data);
    attributesCefBrowser?.destroy();
    attributesCefBrowser = null;
});
