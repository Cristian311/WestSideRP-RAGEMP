let moneyHudBrowser: BrowserMp | null = null;

let cashMoney: number = 0;
let bankMoney: number = 0;
let valueChange: number = 0;
let ScreenX: number = 0;
let ScreenY: number = 0;

let notificationStartTime: number = 0;
let isNotificationActive: boolean = false;

/**
 * Toggles the money HUD UI on or off.
 * When toggled on, the HUD is created and updated with the current values.
 */
function toggleMoneyUI(toggle: boolean) {
    if (moneyHudBrowser) {
        moneyHudBrowser.destroy();
        moneyHudBrowser = null;
    }
    if (toggle) {
        moneyHudBrowser = mp.browsers.new("package://html/hud/moneyhud.html");
        // Update the browser with the current values.
        setMoney(cashMoney, bankMoney);
    }
}

/**
 * Updates the cash and bank values on the browser.
 * Also stores the new values in our local variables.
 */
function setMoney(cash: number, bank: number) {
    if (moneyHudBrowser) {
        moneyHudBrowser.execute(`updateBankMoney(${bank});`);
        moneyHudBrowser.execute(`updateNormalMoney(${cash});`);
    }
    bankMoney = bank;
    cashMoney = cash;
}

// Listen for server events to update the money values.

mp.events.add("SERVER:CLIENT:SetMoney", (cash: number, bank: number) => {
    setMoney(cash, bank);
});

mp.events.add("SERVER:CLIENT:BankMoneyUpdate", (newValue: number) => {
    if (moneyHudBrowser) {
        moneyHudBrowser.execute(`updateBankMoney(${newValue});`);
    }
    // Randomize screen position (base position + a random offset)
    screenX = 1000 + Math.floor(Math.random() * 200);
    screenY = 700 + Math.floor(Math.random() * 200);

    // Calculate the change from the previous bank value
    valueChange = newValue - bankMoney;
    bankMoney = newValue;

    startNotificationTimer();
});

mp.events.add("SERVER:CLIENT:CashMoneyUpdate", (newValue: number) => {
    if (moneyHudBrowser) {
        moneyHudBrowser.execute(`updateNormalMoney(${newValue});`);
    }
    // Randomize screen position (base position + a random offset)
    screenX = 1000 + Math.floor(Math.random() * 200);
    screenY = 600 + Math.floor(Math.random() * 200);

    // Calculate the change from the previous cash value
    valueChange = newValue - cashMoney;
    cashMoney = newValue;

    startNotificationTimer();
});

/**
 * Records the current timestamp and flags that a notification should be displayed.
 */
function startNotificationTimer() {
    notificationStartTime = Date.now();
    isNotificationActive = true;
}

/**
 * On every render tick, if a money change notification is active,
 * draw the notification text at a position that moves upward over time.
 */
mp.events.add("render", () => {
    if (isNotificationActive) {
        const elapsedTime = Date.now() - notificationStartTime;
        // Move the text upward as time passes.
        const posY = screenY - Math.floor(elapsedTime / 50);
        // Choose red for negative changes, green for positive
        const textColor: [number, number, number, number] = valueChange < 0
            ? [255, 0, 0, 255]
            : [0, 255, 0, 255];
        const text = valueChange.toString();

        // Draw the text on screen.
        // (Adjust the options as needed for your server’s drawing utilities.)
        mp.game.graphics.drawText(text, [screenX, posY], {
            font: 4,          // Example font (you can change this)
            color: textColor, // RGBA array for the text color
            scale: [0.8, 0.8],
            outline: true
        });

        // Stop displaying the notification after 3.5 seconds.
        if (elapsedTime > 3500) {
            isNotificationActive = false;
        }
    }
});
