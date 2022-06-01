import * as WebSocket from "ws";
import { Server, IncomingMessage } from 'http';
import { debugLog } from "./config";
import * as url from "url";

import Authentication from './Authentication';


export default class WebSocketHandler {

    private webSocketServer!: WebSocket.Server;

    start(server: Server) {
        debugLog("WebSocketHandler init")
        this.webSocketServer = new WebSocket.Server({ server, path: '/ws/iCAP' });

        this.webSocketServer.on("connection", this.webSocketConnectionEvent.bind(this))
    }
    async webSocketConnectionEvent(webSocket: WebSocket, req: IncomingMessage) {


        const { query: { token } } = url.parse(<string>req.url, true);
        debugLog('token: ', token)

        if (!token) {
            webSocket.close();
        }

        const isAuthentication = await Authentication.tokenChecker(token.toString())
        if (!isAuthentication) {
            webSocket.close();
            debugLog('非法的webSocket連線')
            return;
        }

        webSocket.onmessage = (evt) => {

            debugLog('At webSocket.onmessage');
            debugLog('webSocket.onmessage: ', evt.data)
            try {

                const msg = JSON.parse(evt.data.toString());
                const { DataType } = msg

                switch (DataType) {
                case 'DeviceList':

                    break;
                case 'HeartBeat':
                    evt.target.send('"I am here."');
                    break;

                }
            } catch (error) {
                debugLog(' webSocket.onmessage error: ', error);
            }
        }

        webSocket.onclose = () => {

            debugLog('At webSocket.onclose');
        }

        debugLog("Total connected clients:", this.webSocketServer.clients.size);
    }

    broadcastPercentage(obj: { DeviceId: string, Percentage: Number }) {

        const { DeviceId, Percentage } = obj
        debugLog('broadcastPercentage: ', DeviceId, Percentage)

        this.webSocketServer.clients.forEach((client) => {

            if (client.readyState === WebSocket.OPEN) {
                try {
                    const sendData = JSON.stringify({ SN: DeviceId, Percentage })
                    client.send(sendData);
                    debugLog('data is: ', sendData)
                } catch (e) {
                    debugLog(e);
                }
            }
        })
    }

    broadcast(data: any) {
        this.webSocketServer.clients.forEach(client => {
            if (client.readyState === WebSocket.OPEN) {
                try {
                    debugLog(`Broadcasting data ${data}`);
                    client.send(data);
                } catch (e) {
                    debugLog(e);
                }
            }
        });
    }

}

// export const webSocketHandler = new WebSocketHandler();