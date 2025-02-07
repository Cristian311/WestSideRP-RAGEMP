"use strict";
// Define types for variables and event handlers
let CurrentAnimation = 0;
let Animations = [];
let AnimatorLaunched = false;
const player = mp.players.local;
// Functions to handle animations
function PlayAnim() {
    mp.events.callRemote("ANIMATOR_EVENT", "Animator_PlayAnim", Animations[CurrentAnimation]);
}
// Event Handlers
mp.events.add({
    StartClientAnimator: () => {
        if (!AnimatorLaunched) {
            AnimatorLaunched = true;
        }
    },
    AddAnimatorAnims: (n) => {
        Animations = Animations.concat(JSON.parse(n));
    },
    StopClientAnimator: () => {
        Animations = [];
        AnimatorLaunched = false;
    },
    SkipAnimatorData: (n) => {
        if (n >= Animations.length || n < 0)
            return;
        CurrentAnimation = n;
        mp.events.callRemote("ANIMATOR_EVENT", "Animator_PlayAnim", Animations[CurrentAnimation]);
    }
});
// Render event to draw text
mp.events.add("render", function () {
    if (AnimatorLaunched && Animations.length > 0) {
        const currentAnimationData = Animations[CurrentAnimation];
        const splitData = currentAnimationData.split(" ");
        mp.game.graphics.drawText(`Animation: ~n~~g~[ ${CurrentAnimation} ] ~y~${splitData[0]} ~b~${splitData[1]}`, [0.5, 0.78], { font: 4, color: [255, 255, 255, 185], scale: [0.5, 0.5], outline: true });
    }
});
// Key bindings for animation navigation
mp.keys.bind(39, false, function () {
    if (AnimatorLaunched && CurrentAnimation < Animations.length - 1) {
        CurrentAnimation++;
        PlayAnim();
    }
});
mp.keys.bind(37, false, function () {
    if (AnimatorLaunched && CurrentAnimation > 0) {
        CurrentAnimation--;
        PlayAnim();
    }
});
mp.keys.bind(38, false, function () {
    if (AnimatorLaunched && CurrentAnimation + 100 < Animations.length - 1) {
        CurrentAnimation += 100;
        PlayAnim();
    }
});
mp.keys.bind(40, false, function () {
    if (AnimatorLaunched && CurrentAnimation - 100 > 0) {
        CurrentAnimation -= 100;
        PlayAnim();
    }
});
//# sourceMappingURL=animator.js.map