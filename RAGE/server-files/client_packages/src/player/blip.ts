mp.events.add("SERVER:CLIENT:CreateBlipForCar", (jsonPosition: string) => {
    // Parse the JSON string to retrieve the position coordinates
    const posObj = JSON.parse(jsonPosition);
    const position = new mp.Vector3(posObj.x, posObj.y, posObj.z);

    // Create the blip on the map
    // The options below correspond roughly to:
    // sprite: 1, name: "Vehicle", scale: 1.0, color: 5, alpha: 255, shortRange: false, dimension: 0
    let blip = mp.blips.new(1, position, {
        name: "Vehicle",
        scale: 1.0,
        color: 5,
        shortRange: false,
        alpha: 255,
        dimension: 0
    });
});
