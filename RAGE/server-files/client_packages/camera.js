let handCamera = mp.cameras.new('default', new mp.Vector3(0,  0,  0), new mp.Vector3(0,0,0), 40);

mp.events.add("SERVER:CLIENT:FaceCamera", () => {
    let playerPosition = mp.players.local.position
    
    handCamera.setActive(true);
    handCamera.pointAtPedBone(mp.players.local.handle, 31086, 0, 0, 0, true);
    handCamera.setCoord(playerPosition.x - 0.50, playerPosition.y, playerPosition.z + 0.80);
    mp.game.cam.renderScriptCams(true, false, 0, true, false);
});

mp.events.add("SERVER:CLIENT:FaceCameraOff", () => {
    mp.game.cam.renderScriptCams(false, false, 0, false, false);
});