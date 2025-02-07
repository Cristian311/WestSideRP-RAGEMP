mp.events.add("SERVER:CLIENT:TogAmbience", (toggle : boolean) => {
    if(toggle == true)
        mp.game.audio.startAudioScene("CHARACTER_CHANGE_IN_SKY_SCENE");
    else
        mp.game.audio.stopAudioScene("CHARACTER_CHANGE_IN_SKY_SCENE");
});