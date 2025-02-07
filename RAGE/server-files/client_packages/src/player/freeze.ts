mp.events.add("SERVER:CLIENT:FreezePlayer", () => {
    mp.players.local.freezePosition(true);
});

mp.events.add("SERVER:CLIENT:UnfreezePlayer", () => {
    mp.players.local.freezePosition(false);
});
