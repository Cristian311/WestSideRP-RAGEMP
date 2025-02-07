"use strict";
let examineCefBrowser = null;
mp.events.add("SERVER:CLIENT:OpenExamineScreen", (age, description, outfit, tattoos, name) => {
    if (examineCefBrowser) {
        examineCefBrowser.destroy();
        examineCefBrowser = null;
    }
    examineCefBrowser = mp.browsers.new("http://package/html/attributes/examine.html");
    examineCefBrowser.active = true;
    examineCefBrowser.execute(`updateCharacterAge(${age});`);
    examineCefBrowser.execute(`updateCharacterDescription('${name}', '${description}');`);
    examineCefBrowser.execute(`updateOutfitDescription('${outfit}');`);
    examineCefBrowser.execute(`toggleTattooCheck(${tattoos});`);
});
mp.events.add("SERVER:CLIENT:CloseExamineScreen", () => {
    if (examineCefBrowser) {
        examineCefBrowser.destroy();
        examineCefBrowser = null;
    }
});
//# sourceMappingURL=examine.js.map