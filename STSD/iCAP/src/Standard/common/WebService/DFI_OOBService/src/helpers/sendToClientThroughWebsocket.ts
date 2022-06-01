import { broadcastAll } from '../websocket/websocket';

export default (msg: any) => broadcastAll(msg);
