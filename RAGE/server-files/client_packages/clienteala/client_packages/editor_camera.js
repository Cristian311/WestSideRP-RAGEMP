// Declare the camera and UI state globally
let handCamera = null;
let showUI = true;
let movementSpeed = 0.1;
let rotationSpeed = 1.0;

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
function drawText(text, x, y, scale, font, color, outline = true) {
    mp.game.ui.setTextEntry("STRING");
    mp.game.ui.addTextComponentSubstringPlayerName(text);
    mp.game.ui.setTextFont(font);
    mp.game.ui.setTextScale(scale, scale);
    
    // Set alignment
    mp.game.ui.setTextJustification(2); // Right justify
    mp.game.ui.setTextWrap(0.0, x);
    
    mp.game.ui.setTextColour(color.r, color.g, color.b, color.a);
    if (outline) {
        mp.game.ui.setTextOutline();
    }
    mp.game.ui.drawText(x, y);
}

function drawSettings() {
    if (!showUI || !handCamera) return;

    const camPos = handCamera.getCoord();
    const camRot = handCamera.getRot(2);

    const settings = [
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

    // Draw settings text (bottom right)
    settings.forEach((text, index) => {
        drawText(
            text,
            0.95, // x position
            0.72 + (index * 0.025), // y position
            0.3, // scale
            4, // font
            { r: 255, g: 255, b: 255, a: 255 } // color
        );
    });
}
function handleCameraMovement() {
    if (!handCamera) return;

    let camPos = handCamera.getCoord();
    let camRot = handCamera.getRot(2);

    // Handle speed adjustments
    if (mp.keys.isDown(keys.speedUp)) {
        movementSpeed = Math.min(movementSpeed + 0.01, 2.0);
        rotationSpeed = Math.min(rotationSpeed + 0.1, 10.0);
    }
    if (mp.keys.isDown(keys.speedDown)) {
        movementSpeed = Math.max(movementSpeed - 0.01, 0.01);
        rotationSpeed = Math.max(rotationSpeed - 0.1, 0.1);
    }

    // Convert camera rotation to radians
    const yaw = -camRot.z * (Math.PI / 180);
    const pitch = camRot.x * (Math.PI / 180);

    // Calculate direction vectors
    const forward = new mp.Vector3(
        Math.sin(yaw) * Math.cos(pitch),
        Math.cos(yaw) * Math.cos(pitch),
        Math.sin(pitch)
    );

    const right = new mp.Vector3(
        Math.cos(yaw),
        -Math.sin(yaw),
        0
    );

    // Reset movement vector
    let movement = new mp.Vector3(0, 0, 0);

    // Handle movement relative to camera orientation
    if (mp.keys.isDown(keys.forward)) {
        movement.x += forward.x * movementSpeed;
        movement.y += forward.y * movementSpeed;
        movement.z += forward.z * movementSpeed;
    }
    if (mp.keys.isDown(keys.backward)) {
        movement.x -= forward.x * movementSpeed;
        movement.y -= forward.y * movementSpeed;
        movement.z -= forward.z * movementSpeed;
    }
    if (mp.keys.isDown(keys.left)) {
        movement.x -= right.x * movementSpeed;
        movement.y -= right.y * movementSpeed;
    }
    if (mp.keys.isDown(keys.right)) {
        movement.x += right.x * movementSpeed;
        movement.y += right.y * movementSpeed;
    }

    // Vertical movement
    if (mp.keys.isDown(keys.up)) {
        movement.z += movementSpeed;
    }
    if (mp.keys.isDown(keys.down)) {
        movement.z -= movementSpeed;
    }

    // Check for deactivate key
    if (mp.keys.isDown(keys.deactivate)) {
        mp.events.call("SERVER:CLIENT:DeactivateEditorCamera");
        return;
    }

    // Update camera position
    handCamera.setCoord(
        camPos.x + movement.x,
        camPos.y + movement.y,
        camPos.z + movement.z
    );

    // Handle view rotation
    if (mp.keys.isDown(keys.rotateUp)) {
        camRot.x += rotationSpeed;
    }
    if (mp.keys.isDown(keys.rotateDown)) {
        camRot.x -= rotationSpeed;
    }
    if (mp.keys.isDown(keys.rotateLeft)) {
        camRot.z += rotationSpeed;
    }
    if (mp.keys.isDown(keys.rotateRight)) {
        camRot.z -= rotationSpeed;
    }

    // Full 360 rotation around Y axis with CTRL/ALT
    if (mp.keys.isDown(keys.alt)) { // Changed from ctrl to alt
        camRot.y += rotationSpeed;
    }
    if (mp.keys.isDown(keys.ctrl)) { // Changed from alt to ctrl
        camRot.y -= rotationSpeed;
    }

    // Clamp rotation values
    camRot.x = Math.min(Math.max(camRot.x, -89), 89);
    camRot.y = camRot.y % 360;
    camRot.z = camRot.z % 360;

    // Update camera rotation
    handCamera.setRot(camRot.x, camRot.y, camRot.z, 2);
}

// Mouse click handler for UI toggle
mp.events.add('click', (x, y, upOrDown, leftOrRight, relativeX, relativeY, worldPosition, hitEntity) => {
    if (handCamera && upOrDown === 'up' && leftOrRight === 'left') {
        showUI = !showUI;
    }
});

// Event: Activate camera
mp.events.add("SERVER:CLIENT:ActivateEditorCamera", () => {
    let player = mp.players.local;
    let playerPosition = player.position;
    let playerRotation = player.getRotation(0);

    // Hide HUD and chat
    mp.game.ui.displayHud(false);
    mp.game.ui.displayRadar(false);
    mp.gui.chat.show(false);

    // Freeze the player
    player.freezePosition(true);

    // Create and activate the camera
    handCamera = mp.cameras.new('default', new mp.Vector3(0, 0, 0), new mp.Vector3(0, 0, 0), 40);
    handCamera.setActive(true);
    handCamera.setCoord(playerPosition.x - 0.50, playerPosition.y, playerPosition.z + 0.80);
    
    // Set initial rotation to match player's heading
    handCamera.setRot(0.0, 0.0, playerRotation.z, 2);

    // Start the movement and UI loops
    mp.events.add("render", () => {
        handleCameraMovement();
        drawSettings();
    });
    // Render the camera
    mp.game.cam.renderScriptCams(true, false, 0, true, false);

    // Reset UI state
    showUI = true;
});

// Event: Deactivate camera
mp.events.add("SERVER:CLIENT:DeactivateEditorCamera", () => {
    if (handCamera) {
        // Show HUD and chat
        mp.game.ui.displayHud(true);
        mp.game.ui.displayRadar(true);
        mp.gui.chat.show(true);

        // Stop rendering the camera
        mp.game.cam.renderScriptCams(false, false, 0, false, false);

        // Remove handlers
        mp.events.remove("render", handleCameraMovement);
        mp.events.remove("render", drawSettings);
        mp.events.callRemote("CLIENT:SERVER:ToggleHud");

        // Destroy the camera
        handCamera.destroy();
        handCamera = null;

        // Unfreeze the player
        mp.players.local.freezePosition(false);
    }
});