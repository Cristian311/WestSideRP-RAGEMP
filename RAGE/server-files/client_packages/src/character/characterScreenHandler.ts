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
    try {
        // Check if the arrays exist and the index is valid
        if (!faces || !hairs || !clothes || 
            index < 0 || 
            index >= faces.length || 
            index >= hairs.length || 
            index >= clothes.length) {
            mp.console.logError(`Invalid character index: ${index}`);
            return;
        }
        
        const character = faces[index];
        const hair = hairs[index];
        const outfit = clothes[index];

        updatePlayerFaceFeatures(character);
        updatePlayerHair(hair);
        updatePlayerClothes(outfit);
    } catch (error) {
        mp.console.logError("Error in updateCharacterCustomization: " + error);
    }
}

function updatePlayerFaceFeatures(data: CharacterData) {
    try {
        const player = mp.players.local;
        
        // Check if data and its properties exist before using them
        if (data && data.Eye && typeof data.Eye.Color !== 'undefined') {
            player.setEyeColor(data.Eye.Color);
        } else {
            mp.console.logError("Error: Eye color data is missing or undefined");
            // Set a default eye color to prevent crashes
            player.setEyeColor(0);
        }
        
        // Check Facial features
        if (data && data.Facial) {
            if (data.Facial.Blemishes) {
                player.setHeadOverlay(0, data.Facial.Blemishes.Value, data.Facial.Blemishes.Opacity, 0, 0);
            }
            if (data.Facial.Ageing) {
                player.setHeadOverlay(3, data.Facial.Ageing.Value, data.Facial.Ageing.Opacity, 0, 0);
            }
            if (data.Facial.Complexion) {
                player.setHeadOverlay(6, data.Facial.Complexion.Value, data.Facial.Complexion.Opacity, 0, 0);
            }
            if (data.Facial.SunDamage) {
                player.setHeadOverlay(7, data.Facial.SunDamage.Value, data.Facial.SunDamage.Opacity, 0, 0);
            }
            if (data.Facial.MolesFreckles) {
                player.setHeadOverlay(9, data.Facial.MolesFreckles.Value, data.Facial.MolesFreckles.Opacity, 0, 0);
            }
        }
        
        // Check Shape and Skin data
        if (data && data.Shape && data.Skin) {
            player.setHeadBlendData(
                data.Shape.First, data.Shape.Second, 0,
                data.Skin.First, data.Skin.Second, 0,
                data.Shape.Mix, data.Skin.Mix, 0, true
            );
        }

        // Get face features with safety checks
        const features = getFaceFeatures(data);
        for (let i = 0; i < 20; i++) {
            player.setFaceFeature(i, features[i]);
        }
    } catch (error) {
        mp.console.logError("Error in updatePlayerFaceFeatures: " + error);
    }
}

function getFaceFeatures(charData: CharacterData): number[] {
    try {
        // Default values in case properties are undefined
        const defaultValue = 0;
        
        // Safely access properties with fallbacks to default values
        const noseWidth = charData?.Nose?.Width ?? defaultValue;
        const noseHeight = charData?.Nose?.Height ?? defaultValue;
        const noseLength = charData?.Nose?.Length ?? defaultValue;
        const noseBridge = charData?.Nose?.Bridge ?? defaultValue;
        const noseTip = charData?.Nose?.Tip ?? defaultValue;
        const noseBridgeShift = charData?.Nose?.BridgeShift ?? defaultValue;
        
        const browHeight = charData?.Brow?.Height ?? defaultValue;
        const browWidth = charData?.Brow?.Width ?? defaultValue;
        
        const cheeksHeight = charData?.Cheeks?.Height ?? defaultValue;
        const cheeksWidth = charData?.Cheeks?.Width ?? defaultValue;
        const cheeksCheeksWidth = charData?.Cheeks?.CheeksWidth ?? defaultValue;
        
        const eyeSize = charData?.Eye?.Size ?? defaultValue;
        const lips = charData?.Lips ?? defaultValue;
        
        const jawWidth = charData?.Jaw?.Width ?? defaultValue;
        const jawHeight = charData?.Jaw?.Height ?? defaultValue;
        
        const chinLenght = charData?.Chin?.Lenght ?? defaultValue;
        const chinPosition = charData?.Chin?.Position ?? defaultValue;
        const chinWidth = charData?.Chin?.Width ?? defaultValue;
        const chinShape = charData?.Chin?.Shape ?? defaultValue;
        
        const neckWidth = charData?.Neck?.Width ?? defaultValue;
        
        return [
            noseWidth, noseHeight, noseLength,
            noseBridge, noseTip, noseBridgeShift,
            browHeight, browWidth,
            cheeksHeight, cheeksWidth, cheeksCheeksWidth,
            eyeSize, lips,
            jawWidth, jawHeight,
            chinLenght, chinPosition, chinWidth, chinShape,
            neckWidth
        ];
    } catch (error) {
        mp.console.logError("Error in getFaceFeatures: " + error);
        // Return an array of 20 zeros as a fallback
        return Array(20).fill(0);
    }
}

function updatePlayerHair(data: HairData) {
    try {
        const player = mp.players.local;

        // Check if data and its properties exist before using them
        if (data && data.FacialHair) {
            player.setHeadOverlay(1, data.FacialHair.Value, 1.0, 0, 0);
            player.setHeadOverlayColor(1, 1, data.FacialHair.Color, 0);
        }
        
        if (data && data.Eyebrows) {
            player.setHeadOverlay(2, data.Eyebrows.Value, 1.0, 0, 0);
            player.setHeadOverlayColor(2, 1, data.Eyebrows.Color, 0);
        }
        
        if (data && data.BodyHair) {
            player.setHeadOverlay(10, data.BodyHair.Value, 1.0, 0, 0);
            player.setHeadOverlayColor(10, 1, data.BodyHair.Color, 0);
        }

        if (data && data.Hair) {
            player.setComponentVariation(2, data.Hair.Value, 0, 0);
            player.setHairColor(data.Hair.Color, data.Hair.SecondColor);
        }
    } catch (error) {
        mp.console.logError("Error in updatePlayerHair: " + error);
    }
}

function updatePlayerClothes(clothesData: ClothesData) {
    try {
        const player = mp.players.local;
        
        // Check if clothesData exists before using it
        if (!clothesData) {
            mp.console.logError("Clothes data is undefined");
            return;
        }
        
        // Safely set component variations with null checks
        if (clothesData.Mask) player.setComponentVariation(1, clothesData.Mask.Drawable, clothesData.Mask.Texture, 0);
        if (clothesData.Torso) player.setComponentVariation(3, clothesData.Torso.Drawable, clothesData.Torso.Texture, 0);
        if (clothesData.Legs) player.setComponentVariation(4, clothesData.Legs.Drawable, clothesData.Legs.Texture, 0);
        if (clothesData.Bag) player.setComponentVariation(5, clothesData.Bag.Drawable, clothesData.Bag.Texture, 0);
        if (clothesData.Shoes) player.setComponentVariation(6, clothesData.Shoes.Drawable, clothesData.Shoes.Texture, 0);
        if (clothesData.Accessories) player.setComponentVariation(7, clothesData.Accessories.Drawable, clothesData.Accessories.Texture, 0);
        if (clothesData.Undershirt) player.setComponentVariation(8, clothesData.Undershirt.Drawable, clothesData.Undershirt.Texture, 0);
        if (clothesData.BodyArmor) player.setComponentVariation(9, clothesData.BodyArmor.Drawable, clothesData.BodyArmor.Texture, 0);
        if (clothesData.Decals) player.setComponentVariation(10, clothesData.Decals.Drawable, clothesData.Decals.Texture, 0);
        if (clothesData.Shirts) player.setComponentVariation(11, clothesData.Shirts.Drawable, clothesData.Shirts.Texture, 0);

        // Handle props with explicit checks for -1 values
        if (clothesData.Helmets) {
            if (clothesData.Helmets.Drawable === -1) {
                player.clearProp(0);
            } else {
                player.setPropIndex(0, clothesData.Helmets.Drawable, clothesData.Helmets.Texture, true);
            }
        }
        
        if (clothesData.Glasses) {
            if (clothesData.Glasses.Drawable === -1) {
                player.clearProp(1);
            } else {
                player.setPropIndex(1, clothesData.Glasses.Drawable, clothesData.Glasses.Texture, true);
            }
        }
        
        
        if (clothesData.EarAccessories) {
            if (clothesData.EarAccessories.Drawable === -1) {
                player.clearProp(2);
            } else {
                player.setPropIndex(2, clothesData.EarAccessories.Drawable, clothesData.EarAccessories.Texture, true);
            }
        }
        
        if (clothesData.Watches) {
            if (clothesData.Watches.Drawable === -1) {
                player.clearProp(6);
            } else {
                player.setPropIndex(6, clothesData.Watches.Drawable, clothesData.Watches.Texture, true);
            }
        }
        
        if (clothesData.Bracelets) {
            if (clothesData.Bracelets.Drawable === -1) {
                player.clearProp(7);
            } else {
                player.setPropIndex(7, clothesData.Bracelets.Drawable, clothesData.Bracelets.Texture, true);
            }
        }
    } catch (error) {
        mp.console.logError("Error in updatePlayerClothes: " + error);
    }
}