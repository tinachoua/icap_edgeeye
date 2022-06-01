import WebSocket from 'ws';
import * as config from '../config/env'
import debugLog from "../config/debugLog";
import { observableWebSocketServerMessage } from './websocketServer'
import { Subject } from 'rxjs';

let ws: WebSocket;
let connectInterval: NodeJS.Timeout;

export const observableOutOfBandWebsocketMessage = new Subject<WebSocket.Data>();

export const connectOutOfBandWebsocket = () => {

    ws = new WebSocket(config.OOB_WEBSOCKET_HOST);

    const webSocketServerObserver = observableWebSocketServerMessage.subscribe({ next: (data) => broadcastToOutOfBandWebsocket(data) })

    clearTimeout(connectInterval);
    let heartbeatSetInterval: NodeJS.Timeout;
    // websocket onopen event listener
    ws.onopen = () => {
        debugLog.success("OutOfBand Websocket connected.");
        heartbeatSetInterval = setInterval(heartbeat, 30000)
    };

    // websocket onclose event listener
    ws.onclose = _e => {
        debugLog.warn("OutOfBand Websocket disconnected.");
        clearInterval(heartbeatSetInterval)
        webSocketServerObserver.unsubscribe()
        connectInterval = setTimeout(check, config.OOB_WEBSOCKET_RETRY_FREQ); //call check function after timeout
    };

    ws.onmessage = (event) => {

        debugLog.info("OutOfBand Websocket onmessage: ", event.data);
        observableOutOfBandWebsocketMessage.next(event.data)

    }
    // websocket onerror event listener
    ws.onerror = err => {
        console.error(
            "Socket encountered error: ",
            err.message,
            "Closing socket"
        );
        ws.close();
    };
};
const heartbeat = () => {

    debugLog.info("heartbeat");
    if (!ws || ws.readyState == WebSocket.CLOSED) {
        return
    }

    ws.send(JSON.stringify({
        dataType: 'heartbeat'
    }))
}

const check = () => {
    if (!ws || ws.readyState == WebSocket.CLOSED) connectOutOfBandWebsocket(); //check if websocket instance is closed, if so call `connect` function.
};


export const broadcastToOutOfBandWebsocket = (wantSendMessage: any) => {
    debugLog.info('broadcastToOutOfBandWebsocket: ', wantSendMessage)
    if (ws?.readyState === ws?.OPEN) {
        if (typeof wantSendMessage === 'object') {
            ws?.send(JSON.stringify(wantSendMessage));
        } else {
            ws?.send(wantSendMessage);
        }
    }
}

