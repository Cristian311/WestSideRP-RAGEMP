let authCefBrowser: BrowserMp | null = null;

mp.events.add("SERVER:CLIENT:ShowAuthInterface", () => {
    authCefBrowser = mp.browsers.new("http://package/html/loginregister/login_register.html");
    authCefBrowser.active = true;
    mp.gui.cursor.visible = true;
});

mp.events.add("SERVER:CLIENT:CloseAuthInterface", () => {
    authCefBrowser?.destroy();
    authCefBrowser = null;
    mp.gui.cursor.visible = false;
});

mp.events.add("CEF:CLIENT:ClientLogin", (username: string, password: string) => {
    mp.events.callRemote("CLIENT:SERVER:ClientLogin", username, password);
});

mp.events.add("CEF:CLIENT:ClientRegister", (username: string, password: string) => {
    mp.events.callRemote("CLIENT:SERVER:ClientRegister", username, password);
});

mp.events.add("SERVER:CLIENT:DisplayLoginFail", () => {
    authCefBrowser?.execute(`displayLoginFail();`);
});

mp.events.add("SERVER:CLIENT:DisplayRegisterMessage", (message: string) => {
    authCefBrowser?.execute(`displayRegisterMessage(\`${message}\`);`);
});
