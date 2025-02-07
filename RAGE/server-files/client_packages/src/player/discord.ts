mp.events.add("SERVER:CLIENT:UpdateDiscord", (details: string) => {
    mp.discord.update("Playing on West-Side Roleplay", details);
});
