interface CreateCharacterData {
    firstName: string;
    lastName: string;
    gender: string;
}

let createCharCefBrowser: BrowserMp | null = null;

mp.events.add("CEF:CLIENT:ShowCreateCharScreen", () => {
    createCharCefBrowser = mp.browsers.new("package://html/characters/create_character.html");
    mp.gui.cursor.show(true, true);
});

mp.events.add("CEF:CLIENT:CloseCreateCharInterface", () => {
    createCharCefBrowser?.destroy();
    createCharCefBrowser = null;
    mp.gui.cursor.show(false, false);
});

mp.events.add("CEF:CLIENT:CreateNewCharacter", (firstName: string, lastName: string, selectedGender: string) => {
    const characterData: CreateCharacterData = {
        firstName: firstName,
        lastName: lastName,
        gender: selectedGender
    };

    mp.events.callRemote("CLIENT:SERVER:CreateCharacter", characterData.firstName, characterData.lastName, characterData.gender);
});
