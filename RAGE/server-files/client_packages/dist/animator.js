"use strict";
// Define types for variables and event handlers
let CurrentAnimation = 0;
let Animations = [];
let AnimatorLaunched = false;
const player = mp.players.local;
// Functions to handle animations
function PlayAnim() {
    try {
        mp.events.callRemote("ANIMATOR_EVENT", "Animator_PlayAnim", Animations[CurrentAnimation]);
    }
    catch (error) {
        mp.console.logError("Error playing animation: " + error);
    }
}
// Event Handlers
mp.events.add({
    StartClientAnimator: () => {
        try {
            if (!AnimatorLaunched) {
                AnimatorLaunched = true;
            }
        }
        catch (error) {
            mp.console.logError("Error starting animator: " + error);
        }
    },
    AddAnimatorAnims: (n) => {
        try {
            Animations = Animations.concat(JSON.parse(n));
        }
        catch (error) {
            mp.console.logError("Error adding animations: " + error);
        }
    },
    StopClientAnimator: () => {
        try {
            Animations = [];
            AnimatorLaunched = false;
        }
        catch (error) {
            mp.console.logError("Error stopping animator: " + error);
        }
    },
    SkipAnimatorData: (n) => {
        try {
            if (n >= Animations.length || n < 0)
                return;
            CurrentAnimation = n;
            mp.events.callRemote("ANIMATOR_EVENT", "Animator_PlayAnim", Animations[CurrentAnimation]);
        }
        catch (error) {
            mp.console.logError("Error skipping animation: " + error);
        }
    }
});
// Render event to draw text
mp.events.add("render", function () {
    try {
        if (AnimatorLaunched && Animations.length > 0) {
            const currentAnimationData = Animations[CurrentAnimation];
            const splitData = currentAnimationData.split(" ");
            mp.game.graphics.drawText(`Animation: ~n~~g~[ ${CurrentAnimation} ] ~y~${splitData[0]} ~b~${splitData[1]}`, [0.5, 0.78], { font: 4, color: [255, 255, 255, 185], scale: [0.5, 0.5], outline: true });
        }
    }
    catch (error) {
        mp.console.logError("Error in render event: " + error);
    }
});
// Key bindings for animation navigation
mp.keys.bind(39, false, function () {
    try {
        if (AnimatorLaunched && CurrentAnimation < Animations.length - 1) {
            CurrentAnimation++;
            PlayAnim();
        }
    }
    catch (error) {
        mp.console.logError("Error in key binding (39): " + error);
    }
});
mp.keys.bind(37, false, function () {
    try {
        if (AnimatorLaunched && CurrentAnimation > 0) {
            CurrentAnimation--;
            PlayAnim();
        }
    }
    catch (error) {
        mp.console.logError("Error in key binding (37): " + error);
    }
});
mp.keys.bind(38, false, function () {
    try {
        if (AnimatorLaunched && CurrentAnimation + 100 < Animations.length - 1) {
            CurrentAnimation += 100;
            PlayAnim();
        }
    }
    catch (error) {
        mp.console.logError("Error in key binding (38): " + error);
    }
});
mp.keys.bind(40, false, function () {
    try {
        if (AnimatorLaunched && CurrentAnimation - 100 > 0) {
            CurrentAnimation -= 100;
            PlayAnim();
        }
    }
    catch (error) {
        mp.console.logError("Error in key binding (40): " + error);
    }
});
//# sourceMappingURL=animator.js.map