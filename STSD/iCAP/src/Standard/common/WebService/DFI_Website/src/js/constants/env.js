const IS_OFFLINE = process.env.IS_OFFLINE;

export const API_HOST = (IS_OFFLINE)? '172.16.92.123': window.location.host
export const WEBSOCKET_HOST = (IS_OFFLINE)? process.env.WEBSOCKET_HOST : window.location.host;
export const DEBUG = process.env.DEBUG;