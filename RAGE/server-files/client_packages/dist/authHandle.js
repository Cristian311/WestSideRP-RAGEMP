"use strict";
let authCefBrowser = null;
mp.events.add("SERVER:CLIENT:ShowAuthInterface", () => {
    try {
        authCefBrowser = mp.browsers.new("package://html/loginregister/login_register.html");
        authCefBrowser.active = true;
        mp.gui.cursor.visible = true;
    }
    catch (error) {
        mp.console.logError("Error showing auth interface: " + error);
    }
});
mp.events.add("SERVER:CLIENT:CloseAuthInterface", () => {
    try {
        if (authCefBrowser) {
            authCefBrowser.destroy();
            authCefBrowser = null;
        }
        mp.gui.cursor.visible = false;
    }
    catch (error) {
        mp.console.logError("Error closing auth interface: " + error);
    }
});
mp.events.add("CEF:CLIENT:ClientLogin", (username, password) => {
    try {
        mp.events.callRemote("CLIENT:SERVER:ClientLogin", username, password);
    }
    catch (error) {
        mp.console.logError("Error during login: " + error);
    }
});
mp.events.add("CEF:CLIENT:ClientRegister", (username, password) => {
    try {
        mp.events.callRemote("CLIENT:SERVER:ClientRegister", username, password);
    }
    catch (error) {
        mp.console.logError("Error during registration: " + error);
    }
});
mp.events.add("SERVER:CLIENT:DisplayLoginFail", () => {
    try {
        if (authCefBrowser) {
            authCefBrowser.execute(`displayLoginFail();`);
        }
    }
    catch (error) {
        mp.console.logError("Error displaying login fail: " + error);
    }
});
mp.events.add("SERVER:CLIENT:DisplayRegisterMessage", (message) => {
    try {
        if (authCefBrowser) {
            authCefBrowser.execute(`displayRegisterMessage(\`${message}\`);`);
        }
    }
    catch (error) {
        mp.console.logError("Error displaying register message: " + error);
    }
});
//# sourceMappingURL=authHandle.js.map