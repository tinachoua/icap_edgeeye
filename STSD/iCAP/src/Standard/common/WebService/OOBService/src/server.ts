import * as http from "http";
import app from "./app"
import { start as startWebsocketServer } from './websocket/websocketServer';
import { connectOutOfBandWebsocket } from './websocket/outOfBandWebsocketClient'

const appServer = http.createServer(app);
startWebsocketServer(appServer)
connectOutOfBandWebsocket()

export {
    appServer
}
