let logoBrowser: BrowserMp | null = null;
let versionText: string = "";

/**
 * Toggles the logo UI on or off.
 * When enabled, creates the browser and sends the current version text.
 */
function toggleVersionUI(toggle: boolean): void {
    if (toggle) {
        logoBrowser = mp.browsers.new("package://html/hud/logo.html");
        // Once the browser is created, update it with the current version text.
        logoBrowser.execute(`updateText("${versionText}");`);
    } else if (logoBrowser) {
        logoBrowser.destroy();
        logoBrowser = null;
    }
}

// Listen for the server event to show/update the logo version.
mp.events.add("SERVER:CLIENT:ShowLogo", (text: string) => {
    versionText = text;
    if (logoBrowser) {
        logoBrowser.execute(`updateText("${text}");`);
    }
});
