// Declare the camera and UI state globally
let editor_camera: CameraMp | null = null;
let showUI = true;
let movementSpeed = 0.1;
let rotationSpeed = 1.0;

// Key mappings
const keys = {
    forward: 0x57,  // W
    backward: 0x53, // S
    left: 0x41,     // A
    right: 0x44,    // D
    up: 0x45,       // E
    down: 0x51,     // Q
    rotateUp: 0x26, // Arrow Up
    rotateDown: 0x28, // Arrow Down
    rotateLeft: 0x25, // Arrow Left
    rotateRight: 0x27, // Arrow Right
    deactivate: 0x2E, // Delete key
    ctrl: 0x11,     // Left Control
    alt: 0x12,      // Left Alt
    speedDown: 0xDB, // [
    speedUp: 0xDD   // ]
};

// UI Settings
function drawText(
    text: string,
    x: number,
    y: number,
    scale: number,
    font: number,
    color: { r: number; g: number; b: number; a: number },
    outline: boolean = true
): void {
    mp.game.graphics.drawText(text, [x, y], { font: font, color: [color.r, color.g, color.b, color.a], scale: [scale, scale], outline: outline });
}

function drawSettings(): void {
    if (!showUI || !editor_camera) return;

    const camPos = editor_camera.getCoord();
    const camRot = editor_camera.getRot(2);

    const settings: string[] = [
        "Controale camere:",
        "WASD - Misca camera",
        "Arrow Keys - Roteste camera",
        "E/Q - Sus/Jos",
        "[ ] - Ajusteaza viteza",
        "Delete - Iesi din camera",
        "Ctrl, Alt - Invarte camera",
        "Click stanga - Ascunde acest meniu",
        `Viteza de miscare: ${movementSpeed.toFixed(2)}`,
        `Viteza de rotatie: ${rotationSpeed.toFixed(2)}`,
        `Pos: ${camPos.x.toFixed(2)}, ${camPos.y.toFixed(2)}, ${camPos.z.toFixed(2)}`,
        `Rot: ${camRot.x.toFixed(2)}, ${camRot.y.toFixed(2)}, ${camRot.z.toFixed(2)}`
    ];

    settings.forEach((text : string, index : number) => {
        drawText(
            text,
            0.95, 
            0.72 + index * 0.025, 
            0.3, 
            4, 
            { r: 255, g: 255, b: 255, a: 255 } 
        );
    });
}

function handleCameraMovement(): void {
    if (!editor_camera) return;

    const camPos = editor_camera.getCoord();
    const camRot = editor_camera.getRot(2);

    // Speed adjustments
    if (mp.keys.isDown(keys.speedUp)) {
        movementSpeed = Math.min(movementSpeed + 0.01, 2.0);
        rotationSpeed = Math.min(rotationSpeed + 0.1, 10.0);
    }
    if (mp.keys.isDown(keys.speedDown)) {
        movementSpeed = Math.max(movementSpeed - 0.01, 0.01);
        rotationSpeed = Math.max(rotationSpeed - 0.1, 0.1);
    }

    // Convert rotation to radians
    const yaw = -camRot.z * (Math.PI / 180);
    const pitch = camRot.x * (Math.PI / 180);

    // Direction vectors
    const forward = new mp.Vector3(
        Math.sin(yaw) * Math.cos(pitch),
        Math.cos(yaw) * Math.cos(pitch),
        Math.sin(pitch)
    );

    const right = new mp.Vector3(Math.cos(yaw), -Math.sin(yaw), 0);

    let movement = new mp.Vector3(0, 0, 0);

    if (mp.keys.isDown(keys.forward)) {
        movement = movement.add(forward.multiply(movementSpeed));
    }
    if (mp.keys.isDown(keys.backward)) {
        movement = movement.subtract(forward.multiply(movementSpeed));
    }
    if (mp.keys.isDown(keys.left)) {
        movement = movement.subtract(right.multiply(movementSpeed));
    }
    if (mp.keys.isDown(keys.right)) {
        movement = movement.add(right.multiply(movementSpeed));
    }
    if (mp.keys.isDown(keys.up)) {
        movement.z += movementSpeed;
    }
    if (mp.keys.isDown(keys.down)) {
        movement.z -= movementSpeed;
    }

    if (mp.keys.isDown(keys.deactivate)) {
        mp.events.call("SERVER:CLIENT:DeactivateEditorCamera");
        return;
    }

    editor_camera.setCoord(
        camPos.x + movement.x,
        camPos.y + movement.y,
        camPos.z + movement.z
    );

    // View rotation
    if (mp.keys.isDown(keys.rotateUp)) camRot.x += rotationSpeed;
    if (mp.keys.isDown(keys.rotateDown)) camRot.x -= rotationSpeed;
    if (mp.keys.isDown(keys.rotateLeft)) camRot.z += rotationSpeed;
    if (mp.keys.isDown(keys.rotateRight)) camRot.z -= rotationSpeed;

    if (mp.keys.isDown(keys.alt)) camRot.y += rotationSpeed;
    if (mp.keys.isDown(keys.ctrl)) camRot.y -= rotationSpeed;

    // Clamp and update rotation
    camRot.x = Math.min(Math.max(camRot.x, -89), 89);
    camRot.y = camRot.y % 360;
    camRot.z = camRot.z % 360;

    editor_camera.setRot(camRot.x, camRot.y, camRot.z, 2);
}

mp.events.add("click", (x, y, upOrDown, leftOrRight) => {
    if (editor_camera && upOrDown === "up" && leftOrRight === "left") {
        showUI = !showUI;
    }
});

mp.events.add("SERVER:CLIENT:ActivateEditorCamera", () => {
    const player = mp.players.local;
    const playerPosition = player.position;
    const playerRotation = player.getRotation(0);

    mp.game.ui.displayHud(false);
    mp.game.ui.displayRadar(false);
    mp.gui.chat.show(false);

    player.freezePosition(true);

    editor_camera = mp.cameras.new("default", playerPosition, new mp.Vector3(0, 0, 0), 40);
    editor_camera.setActive(true);
    editor_camera.setCoord(playerPosition.x - 0.5, playerPosition.y, playerPosition.z + 0.8);
    editor_camera.setRot(0.0, 0.0, playerRotation.z, 2);

    mp.events.add("render", () => {
        handleCameraMovement();
        drawSettings();
    });

    mp.game.cam.renderScriptCams(true, false, 0, true, false, 0);
    showUI = true;
});

mp.events.add("SERVER:CLIENT:DeactivateEditorCamera", () => {
    if (editor_camera) {
        mp.game.ui.displayHud(true);
        mp.game.ui.displayRadar(true);
        mp.gui.chat.show(true);

        mp.game.cam.renderScriptCams(false, false, 0, false, false, 0);

        mp.events.remove("render", handleCameraMovement);
        mp.events.remove("render", drawSettings);

        editor_camera.destroy();
        editor_camera = null;

        mp.players.local.freezePosition(false);
    }
});
