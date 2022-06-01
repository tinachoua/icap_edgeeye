import * as WebSocket from "ws";
import * as url from "url";
import { PATH } from '../config/websocketServer'
import { Server, IncomingMessage } from 'http';
import debugLog from "../config/debugLog";
import iCAPAuthentication from '../utils/authentication';
import { Subject } from 'rxjs';
import { observableOutOfBandWebsocketMessage } from './outOfBandWebsocketClient';

let webSocketServer: WebSocket.Server;
export const observableWebSocketServerMessage = new Subject<WebSocket.Data>();

export function start(server: Server) {
  webSocketServer = new WebSocket.Server({ server, path: PATH });

  debugLog.success('start')

  observableOutOfBandWebsocketMessage.subscribe({ next: (data) => websocketServerBroadcastAll(data) })

  webSocketServer.on('connection', async (webSocket: WebSocket, req: IncomingMessage) => {
    const { query: { token } } = url.parse(<string>req.url, true);

    if (!token) {
      webSocket.close();
    }

    const isValidToken = iCAPAuthentication.tokenChecker(token as string);
    if (!isValidToken) {
      webSocket.close();
    }

    webSocket.send(JSON.stringify({ TotalClient: webSocketServer.clients.size }))

    webSocket.onclose = (_event) => {

      debugLog.warn('webSocket client disconnect: ');

    }
    webSocket.onmessage = (event) => {

      debugLog.info('webSocket server onmessage: ',
        {
          data: event.data
        })

      try {

        const msg = JSON.parse(event.data.toString());

        switch (msg.dataType) {
          case 'HeartBeat':
            event.target.send('"I am here."');
            break;
          default:
            observableWebSocketServerMessage.next(event.data)
            break;
        }
      } catch (error) {
        debugLog.error('webSocket server onmessage error: ', error);
      }
    }
  });
}

export function websocketServerBroadcastAll(message: any) {
  if (!message || webSocketServer.clients.size === 0) return;

  if (typeof message === 'object') {
    message = JSON.stringify(message);
  }

  webSocketServer.clients.forEach((client: any) => {
    if (client.readyState === WebSocket.OPEN) {
      try {
        client.send(message);
      } catch (error) {
        debugLog.error('webSocket Server BroadcastAll error', error);
      }
    }
  });
}


/**
 * test block
 */
// let a = 0;
// setInterval(() => {
//   websocketServerBroadcastAll({
//     "dataType": "status", "payload": { "B0011905301234567": a }
//   })

//   if (a === 0) {
//     a = 1
//   } else {
//     a = 0
//   }
// }, 3000)

// let b = 0;
// setInterval(() => {
//   websocketServerBroadcastAll({
//     dataType: "deviceOSHeartbeat",
//     payload: {
//       serialNumber: "B0011905301234567",
//       osHeartbeat: b,
//     }
//   })

//   if (b === 0) {
//     b = 1
//   } else {
//     b = 0
//   }
// }, 3000)