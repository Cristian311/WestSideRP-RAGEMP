"use strict";
let createCharCefBrowser = null;
mp.events.add("CEF:CLIENT:ShowCreateCharScreen", () => {
    createCharCefBrowser = mp.browsers.new("package://html/characters/create_character.html");
    mp.gui.cursor.show(true, true);
});
mp.events.add("CEF:CLIENT:CloseCreateCharInterface", () => {
    createCharCefBrowser === null || createCharCefBrowser === void 0 ? void 0 : createCharCefBrowser.destroy();
    createCharCefBrowser = null;
    mp.gui.cursor.show(false, false);
});
mp.events.add("CEF:CLIENT:CreateNewCharacter", (firstName, lastName, selectedGender) => {
    const characterData = {
        firstName: firstName,
        lastName: lastName,
        gender: selectedGender
    };
    mp.events.callRemote("CLIENT:SERVER:CreateCharacter", characterData.firstName, characterData.lastName, characterData.gender);
});
//# sourceMappingURL=createCharHandler.js.map