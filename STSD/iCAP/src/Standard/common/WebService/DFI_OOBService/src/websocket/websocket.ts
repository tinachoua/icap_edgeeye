import WebSocket from 'ws';
import url from 'url';
import { Server, IncomingMessage } from 'http';
import { debugLog } from "../library/config";
import Authentication from '../library/Authentication';

const WS_PATH = '/ws/iCAP';
let webSocketServer: any;

async function isValidToken(token: string) {
    const isAuthentication = await Authentication.tokenChecker(token);
    if (!isAuthentication) {
        debugLog('token error');
        return false;
    }
    return true;
}

export function start(server: Server) {
    webSocketServer = new WebSocket.Server({ server, path: WS_PATH });
    webSocketServer.on('connection', async (webSocket: WebSocket, req: IncomingMessage) => {
        const { query: { token } } = url.parse(<string>req.url, true);

        if (!token) {
            webSocket.close();
        }

        if (!isValidToken(token as string)) {
            webSocket.close();
        }
    });
}

export function broadcastAll(message: any) {
    if (!message || webSocketServer.clients.length === 0) return;

    if (typeof message === 'object') {
        message = JSON.stringify(message);
    }

    webSocketServer.clients.forEach((client: any) => {
        if (client.readyState === WebSocket.OPEN) {
            try {
                client.send(message);
            } catch (error) {
                debugLog('broadcastAll error', error);
            }
        }
    });
}
