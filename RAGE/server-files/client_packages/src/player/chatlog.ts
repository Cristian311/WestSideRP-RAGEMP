mp.events.add("CEF:CLIENT:Chatlog", (message: string) => {
    // Forward the message to the server for logging
    mp.events.callRemote("CLIENT:SERVER:ChatLogger", message);
});
