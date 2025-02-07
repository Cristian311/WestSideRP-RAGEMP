"use strict";
let attributesCefBrowser = null;
mp.events.add("SERVER:CLIENT:OpenAttributesScreen", (darkmode) => {
    attributesCefBrowser = mp.browsers.new("http://package/html/attributes/attributes.html");
    attributesCefBrowser.active = true;
    mp.gui.cursor.visible = true;
    if (darkmode) {
        attributesCefBrowser.execute("toggleDarkMode();");
    }
});
mp.events.add("CEF:CLIENT:AttributesUpdate", (data) => {
    mp.events.callRemote("CLIENT:SERVER:AttributesUpdate", data);
    attributesCefBrowser === null || attributesCefBrowser === void 0 ? void 0 : attributesCefBrowser.destroy();
    attributesCefBrowser = null;
});
//# sourceMappingURL=attributes.js.map