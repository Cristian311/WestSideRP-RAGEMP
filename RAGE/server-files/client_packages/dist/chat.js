"use strict";
mp.events.add("playerReady", () => {
    try {
        mp.gui.chat.show(false);
        const chatbox = mp.browsers.new("package://html/advanced-chat/index.html");
        chatbox.markAsChat();
        mp.gui.chat.colors = true;
        mp.game.ui.displayCash(false);
        mp.game.ui.displayAreaName(false);
    }
    catch (error) {
        mp.console.logError("Error in playerReady event: " + error);
    }
});
//# sourceMappingURL=chat.js.map