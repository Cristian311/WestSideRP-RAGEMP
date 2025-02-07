"use strict";
mp.events.add("CEF:CLIENT:Chatlog", (message) => {
    // Forward the message to the server for logging
    mp.events.callRemote("CLIENT:SERVER:ChatLogger", message);
});
//# sourceMappingURL=chatlog.js.map