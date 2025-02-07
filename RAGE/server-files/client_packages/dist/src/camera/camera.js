"use strict";
let handCamera = mp.cameras.new('default', new mp.Vector3(0, 0, 0), new mp.Vector3(0, 0, 0), 40);
mp.events.add("SERVER:CLIENT:FaceCamera", () => {
    let playerPosition = mp.players.local.position;
    handCamera.setActive(true);
    handCamera.pointAtPedBone(mp.players.local.handle, 31086, 0, 0, 0, true);
    handCamera.setCoord(playerPosition.x - 0.50, playerPosition.y, playerPosition.z + 0.80);
    mp.game.cam.renderScriptCams(true, false, 0, true, false, 0);
});
mp.events.add("SERVER:CLIENT:ChooseCharacterCamera", () => {
    handCamera.setActive(true);
    handCamera.setCoord(209.13136, 283.0628 + 0.0, 141.47795 + 1.0);
    handCamera.pointAtCoord(210.71982, 285.08926, 142.52323);
    mp.game.cam.renderScriptCams(true, false, 0, true, false, 0);
});
mp.events.add("SERVER:CLIENT:CameraOff", () => {
    mp.game.cam.renderScriptCams(false, false, 0, false, false, 0);
});
//# sourceMappingURL=camera.js.map