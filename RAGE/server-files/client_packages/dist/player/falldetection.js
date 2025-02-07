"use strict";
let fallStartTime = null;
let isFalling = false;
// Tick event to check for falling state
mp.events.add("render", () => {
    const player = mp.players.local;
    if (player.isFalling() && player.isRagdoll()) {
        if (!isFalling) {
            fallStartTime = Date.now(); // Record the fall start time
            isFalling = true;
        }
        else if (fallStartTime) {
            const fallDuration = Date.now() - fallStartTime;
            if (fallDuration > 3000) {
                mp.events.callRemote("CLIENT:SERVER:FallDetected");
                isFalling = false; // Reset the flag after reporting
            }
        }
    }
    else {
        isFalling = false; // Reset if the player is no longer falling
        fallStartTime = null;
    }
});
//# sourceMappingURL=falldetection.js.map