"use strict";
let authCefBrowser = null;
mp.events.add("SERVER:CLIENT:ShowAuthInterface", () => {
    authCefBrowser = mp.browsers.new("http://package/html/loginregister/login_register.html");
    authCefBrowser.active = true;
    mp.gui.cursor.visible = true;
});
mp.events.add("SERVER:CLIENT:CloseAuthInterface", () => {
    authCefBrowser === null || authCefBrowser === void 0 ? void 0 : authCefBrowser.destroy();
    authCefBrowser = null;
    mp.gui.cursor.visible = false;
});
mp.events.add("CEF:CLIENT:ClientLogin", (username, password) => {
    mp.events.callRemote("CLIENT:SERVER:ClientLogin", username, password);
});
mp.events.add("CEF:CLIENT:ClientRegister", (username, password) => {
    mp.events.callRemote("CLIENT:SERVER:ClientRegister", username, password);
});
mp.events.add("SERVER:CLIENT:DisplayLoginFail", () => {
    authCefBrowser === null || authCefBrowser === void 0 ? void 0 : authCefBrowser.execute(`displayLoginFail();`);
});
mp.events.add("SERVER:CLIENT:DisplayRegisterMessage", (message) => {
    authCefBrowser === null || authCefBrowser === void 0 ? void 0 : authCefBrowser.execute(`displayRegisterMessage(\`${message}\`);`);
});
//# sourceMappingURL=authHandle.js.map