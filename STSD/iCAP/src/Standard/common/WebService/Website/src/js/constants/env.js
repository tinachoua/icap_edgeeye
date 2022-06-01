import { ENABLE_SSL } from "./globalVariable";

const IS_OFFLINE = process.env.IS_OFFLINE;

const WEBSOCKET_HSOT = process.env.WEBSOCKET_HOST ? process.env.WEBSOCKET_HOST : window.location.host;
//const WEBSOCKET_HSOT = process.env.WEBSOCKET_HOST ? process.env.WEBSOCKET_HOST : '172.16.92.116';
const VERSION_NUMBER = process.env.VERSION_NUMBER;
const WEBSOCKET_URL = `${ENABLE_SSL ? 'wss' : 'ws'}://${WEBSOCKET_HSOT}/ws/iCAP?token=${$.cookie('token')}`

export {
  IS_OFFLINE,
  WEBSOCKET_HSOT,
  VERSION_NUMBER,
  WEBSOCKET_URL
}