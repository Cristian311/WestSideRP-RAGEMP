mp.events.add("playerReady", (player : PlayerMp) => {
    mp.gui.chat.show(false);
    
    const chatbox = mp.browsers.new("http://package/html/advanced-chat/index.html");
    chatbox.markAsChat();
    mp.gui.chat.colors = true;

    mp.game.ui.displayCash(false);
    mp.game.ui.displayAreaName(false);
   });