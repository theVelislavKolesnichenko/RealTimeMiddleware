var WebSocketClient = function (cnn)
{
    var self = this;
    self.url = cnn.url;
    self.wsInitSelector = cnn.wsInitSelector;
    self.wsSwndSelector = cnn.wsSwndSelector;
    self.Send = cnn.OnSend;

    self.initWS = function (url) {
        var wsImpl = window.WebSocket || window.MozWebSocket;
        ws = new wsImpl(url);

        ws.onopen = (cnn.OnOpen)
           ? cnn.OnOpen()
           : function () { console.log('WebSocket connection Open'); }

        ws.onclose = (cnn.OnClose)
            ? cnn.OnClose() :
            function (e) { console.log('WebSocket connection Close'); };

        ws.onerror = (cnn.OnError)
            ? cnn.OnError() :
            function (e) { console.log('WebSocket erron: ' + e.data); };

        ws.onmessage = 
            function (e) {
                if (cnn.OnMessage) {
                    cnn.OnMessage(e.data)
                } else {
                    console.log('WebSocket message: ' + e.data);
                }
            };

        return ws;
    }

    if (self.wsInitSelector) {
        self.wsInitSelector.onclick = function () {
            init();
            //self.ws = self.initWS(self.url);
        };
    } else {
        init();
        //self.ws = self.initWS(self.url);
    }

    var init = function () {
        self.ws = self.initWS(self.url);

        self.OnSend = function send(message) {
            if (message) {
                self.ws.send(message);
            } else {
                if (self.Send) {
                    debugger
                    var message = self.Send();
                    self.ws.send(message);
                } else {
                    //self.ws.send("1000");
                }
            }
        };

        //self.OnSend = function send(message) {
        //    self.ws.send(message);
        //};

        if (self.wsSwndSelector) {
            self.wsSwndSelector.onclick = function () {
                debugger
                self.OnSend();
                //ws.send("Hello World");
            };
        } else {
            self.OnSend();
            //ws.send("Hello World");
        }
    }

    return self;
}