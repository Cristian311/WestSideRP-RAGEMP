"use strict";
let togLocalNametag = false;
let _screenX = 1600, _screenY = 900;
// Get active screen resolution
const res = mp.game.graphics.getScreenActiveResolution(0, 0);
_screenX = res.x;
_screenY = res.y;
// Toggle nametag visibility
mp.events.add("SERVER:CLIENT:TogNametag", () => {
    togLocalNametag = !togLocalNametag;
});
// Render nametags on each frame
mp.events.add("render", () => {
    mp.players.streamed.forEach((player) => {
        if (player === mp.players.local && !togLocalNametag)
            return;
        const boneCoords = player.getBoneCoords(31086, 0, 0, 0); // Head bone
        boneCoords.z += 0.6;
        const screenPos = mp.game.graphics.world3dToScreen2d(boneCoords);
        if (screenPos) {
            const name = player.name;
            const newlineCount = (name.match(/\n/g) || []).length;
            const adjustedY = _screenY * screenPos.y + 100 - (newlineCount > 0 ? (newlineCount + 1) * 30 : 30);
            mp.game.graphics.drawText(name, [_screenX * screenPos.x + 150, adjustedY], {
                font: 4, // Equivalent to ChaletComprimeCologne
                color: [255, 255, 255, 255],
                scale: [0.4, 0.4],
                outline: true,
                centre: true
            });
        }
    });
});
//# sourceMappingURL=nametags.js.map