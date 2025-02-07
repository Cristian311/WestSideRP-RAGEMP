interface CharacterData {
    Eye: { Color: number; Size: number };
    Facial: {
        Blemishes: { Value: number; Opacity: number };
        Ageing: { Value: number; Opacity: number };
        Complexion: { Value: number; Opacity: number };
        SunDamage: { Value: number; Opacity: number };
        MolesFreckles: { Value: number; Opacity: number };
    };
    Shape: { First: number; Second: number; Mix: number };
    Skin: { First: number; Second: number; Mix: number };
    Nose: {
        Width: number; Height: number; Length: number;
        Bridge: number; Tip: number; BridgeShift: number;
    };
    Brow: { Height: number; Width: number };
    Cheeks: { Height: number; Width: number; CheeksWidth: number };
    Lips: number;
    Jaw: { Width: number; Height: number };
    Chin: { Lenght: number; Position: number; Width: number; Shape: number };
    Neck: { Width: number };
}

interface HairData {
    Hair: { Value: number; Color: number; SecondColor: number };
    FacialHair: { Value: number; Color: number };
    Eyebrows: { Value: number; Color: number };
    BodyHair: { Value: number; Color: number };
}

interface ClothesData {
    Mask: { Drawable: number; Texture: number };
    Torso: { Drawable: number; Texture: number };
    Legs: { Drawable: number; Texture: number };
    Bag: { Drawable: number; Texture: number };
    Shoes: { Drawable: number; Texture: number };
    Accessories: { Drawable: number; Texture: number };
    Undershirt: { Drawable: number; Texture: number };
    BodyArmor: { Drawable: number; Texture: number };
    Decals: { Drawable: number; Texture: number };
    Shirts: { Drawable: number; Texture: number };
    Helmets: { Drawable: number; Texture: number };
    Glasses: { Drawable: number; Texture: number };
    EarAccessories: { Drawable: number; Texture: number };
    Watches: { Drawable: number; Texture: number };
    Bracelets: { Drawable: number; Texture: number };
}

let charCefBrowser: BrowserMp | null = null;
let faces: CharacterData[] = [];
let hairs: HairData[] = [];
let clothes: ClothesData[] = [];

mp.events.add("SERVER:CLIENT:ShowCharInterface", () => {
    charCefBrowser = mp.browsers.new("package://html/characters/characters.html");
    mp.gui.cursor.show(true, true);
    //mp.events.call("CEF:CLIENT:ShowCharInterface");
    mp.gui.chat.show(false);
});

mp.events.add("CEF:CLIENT:ShowCharInterface", () => {
    // The character customization interface will be shown
    charCefBrowser = mp.browsers.new("package://html/characters/characters.html");
    mp.gui.cursor.show(true, true);
    //mp.events.call("CEF:CLIENT:ShowCharInterface");
    mp.gui.chat.show(false);
    //mp.events.call("CEF:CLIENT:ShowCharInterface");
});

mp.events.add("SERVER:CLIENT:CloseCharInterface", () => {
    charCefBrowser?.destroy();
    charCefBrowser = null;
    mp.gui.cursor.show(false, false);
});

mp.events.add("CEF:CLIENT:CloseCharInterface", () => {
    charCefBrowser?.destroy();
    charCefBrowser = null;
    mp.gui.cursor.show(false, false);
});

mp.events.add("SERVER:CLIENT:DisplayAccountInfo", (username: string, namesString: string, moneyString: string) => {
    if (charCefBrowser) {
        charCefBrowser.execute(`displayAccountInfo('${username}', ${namesString}, ${moneyString});`);
    }
});

mp.events.add("SERVER:CLIENT:CharactersInfo", (facesJson: string, hairsJson: string, clothesJson: string) => {
    faces = JSON.parse(facesJson);
    hairs = JSON.parse(hairsJson);
    clothes = JSON.parse(clothesJson);
});

mp.events.add("CEF:CLIENT:CharacterPressed", (index: number) => {
    // Teleport the player to a customization location
    mp.players.local.position = new mp.Vector3(210.71982, 285.08926, 141.92323);

    // Apply the selected character's data
    updateCharacterCustomization(index);
});

mp.events.add("CEF:CLIENT:CharacterSelected", (CharacterName: string) => {
    mp.events.callRemote("CLIENT:SERVER:CharSelected", CharacterName);
    mp.events.callRemote("CLIENT:SERVER:CustomizeScreenOnCreation");

    mp.gui.chat.show(true);
});



function updateCharacterCustomization(index: number) {
    const character = faces[index];
    const hair = hairs[index];
    const outfit = clothes[index];

    updatePlayerFaceFeatures(character);
    updatePlayerHair(hair);
    updatePlayerClothes(outfit);
}

function updatePlayerFaceFeatures(data: CharacterData) {
    const player = mp.players.local;
    
    player.setEyeColor(data.Eye.Color);
    
    player.setHeadOverlay(0, data.Facial.Blemishes.Value, data.Facial.Blemishes.Opacity, 0, 0);
    player.setHeadOverlay(3, data.Facial.Ageing.Value, data.Facial.Ageing.Opacity, 0, 0);
    player.setHeadOverlay(6, data.Facial.Complexion.Value, data.Facial.Complexion.Opacity, 0, 0);
    player.setHeadOverlay(7, data.Facial.SunDamage.Value, data.Facial.SunDamage.Opacity, 0, 0);
    player.setHeadOverlay(9, data.Facial.MolesFreckles.Value, data.Facial.MolesFreckles.Opacity, 0, 0);
    
    player.setHeadBlendData(
        data.Shape.First, data.Shape.Second, 0,
        data.Skin.First, data.Skin.Second, 0,
        data.Shape.Mix, data.Skin.Mix, 0, true
    );

    const features = getFaceFeatures(data);
    for (let i = 0; i < 20; i++) {
        player.setFaceFeature(i, features[i]);
    }
}
function getFaceFeatures(charData: CharacterData): number[] {
    return [
        charData.Nose.Width, charData.Nose.Height, charData.Nose.Length,
        charData.Nose.Bridge, charData.Nose.Tip, charData.Nose.BridgeShift,
        charData.Brow.Height, charData.Brow.Width,
        charData.Cheeks.Height, charData.Cheeks.Width, charData.Cheeks.CheeksWidth,
        charData.Eye.Size, charData.Lips,
        charData.Jaw.Width, charData.Jaw.Height,
        charData.Chin.Lenght, charData.Chin.Position, charData.Chin.Width, charData.Chin.Shape,
        charData.Neck.Width
    ];
}
function updatePlayerHair(data: HairData) {
    const player = mp.players.local;

    player.setHeadOverlay(1, data.FacialHair.Value, 1.0, data.FacialHair.Color, data.FacialHair.Color);
    player.setHeadOverlay(2, data.Eyebrows.Value, 1.0, data.FacialHair.Color, data.FacialHair.Color);
    player.setHeadOverlay(10, data.BodyHair.Value, 1.0, data.FacialHair.Color, data.FacialHair.Color);

    player.setHeadOverlayColor(1, 1, data.FacialHair.Color, 0);
    player.setHeadOverlayColor(2, 1, data.Eyebrows.Color, 0);
    player.setHeadOverlayColor(10, 1, data.BodyHair.Color, 0);

    player.setComponentVariation(2, data.Hair.Value, 0, 0);
    player.setHairColor(data.Hair.Color, data.Hair.SecondColor);
}
function updatePlayerClothes(clothesData : ClothesData){
    player.setComponentVariation(1, clothesData.Mask.Drawable, clothesData.Mask.Texture, 0);
    player.setComponentVariation(3, clothesData.Torso.Drawable, clothesData.Torso.Texture, 0);
    player.setComponentVariation(4, clothesData.Legs.Drawable, clothesData.Legs.Texture, 0);
    player.setComponentVariation(5, clothesData.Bag.Drawable, clothesData.Bag.Texture, 0);
    player.setComponentVariation(6, clothesData.Shoes.Drawable, clothesData.Shoes.Texture, 0);
    player.setComponentVariation(7, clothesData.Accessories.Drawable, clothesData.Accessories.Texture, 0);
    player.setComponentVariation(8, clothesData.Undershirt.Drawable, clothesData.Undershirt.Texture, 0);
    player.setComponentVariation(9, clothesData.BodyArmor.Drawable, clothesData.BodyArmor.Texture, 0);
    player.setComponentVariation(10, clothesData.Decals.Drawable, clothesData.Decals.Texture, 0);
    player.setComponentVariation(11, clothesData.Shirts.Drawable, clothesData.Shirts.Texture, 0);

    const clearOrSetProp = (slot: number, drawable: number, texture: number) => {
        if (drawable === -1) {
            player.clearProp(slot);
        } else {
            player.setPropIndex(slot, drawable, texture, true);
        }
    };

    clearOrSetProp(0, clothesData.Helmets.Drawable, clothesData.Helmets.Texture);
    clearOrSetProp(1, clothesData.Glasses.Drawable, clothesData.Glasses.Texture);
    clearOrSetProp(2, clothesData.EarAccessories.Drawable, clothesData.EarAccessories.Texture);
    clearOrSetProp(6, clothesData.Watches.Drawable, clothesData.Watches.Texture);
    clearOrSetProp(7, clothesData.Bracelets.Drawable, clothesData.Bracelets.Texture);
}