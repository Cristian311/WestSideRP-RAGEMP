let street = mp.browsers.new("package://street/index.html");
let player = mp.players.local;
const updateInterval = 500; // milliseconds, lower value = more accurate, at the cost of performance

let getStreet;
let streetName;
let crossingName;
let zoneName;
let direction;

setInterval(() => {
    getStreet = mp.game.pathfind.getStreetNameAtCoord(player.position.x, player.position.y, player.position.z, 0, 0);
    // Returns obj {"streetName": hash, crossingRoad: hash}

    streetName = mp.game.ui.getStreetNameFromHashKey(getStreet.streetName);
    crossingName = mp.game.ui.getStreetNameFromHashKey(getStreet.crossingRoad);
    // Returns string, if exist

    zoneName = mp.game.gxt.get(mp.game.zone.getNameOfZone(player.position.x, player.position.y, player.position.z));

    let heading = player.getHeading();
    if (player.heading != 0) {
        if (heading < 22.5 || heading > 337.5) direction = "N";
        else if (heading > 22.5 && heading < 67.5) direction = "NE";
        else if (heading > 67.5 && heading < 112.5) direction = "E";
        else if (heading > 112.5 && heading < 157.5) direction = "SE";
        else if (heading > 157.5 && heading < 202.5) direction = "S";
        else if (heading > 202.5 && heading < 247.5) direction = "SW";
        else if (heading > 247.5 && heading < 292.5) direction = "W";
        else if (heading > 292.5 && heading < 337.5) direction = "NW";
    }
}, updateInterval);
let hudtoggle = true;
mp.events.add('CLIENT:TogglePLD', (toggle) => {
    if(toggle)
        {
            street = mp.browsers.new("package://street/index.html");
            hudtoggle = true;
        }
        else
    {
        street.destroy();
        hudtoggle = false;
    }
});
mp.events.add('render', () => {
    let crossing;
    if (crossingName != "") { crossing = `/ ${crossingName}`; } else { crossing = ""; }
    if(hudtoggle)
        street.execute(`update('${streetName}', '${crossing}', '${zoneName}', '${direction}');`);
});