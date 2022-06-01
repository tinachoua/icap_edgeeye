import debug from "debug";
export const debugLog = debug("debug");
export const servicePort = process.env.PORT || 8165;
export const authenticationService = process.env.AuthenticationAPIHost || '172.16.92.123'

export const mqttConfig = {
    host: process.env.MQTT_HOST || 'mqtt://172.16.92.114',
    port: process.env.MQTT_PORT || '1883',
    passWord: process.env.MQTT_PASSWORD || 'B673AEBC6D65E7F42CFABFC7E01C02D0',
    userName: process.env.MQTT_USERNAME || 'innoage',
}

export const innoAGEService = {
    host: process.env.INNOAGE_SERVICE_HOST || '172.16.92.114:8161'
}

export const redisConfig = {
    host: process.env.REDIS_HOST || '172.16.92.123',
    port: process.env.REDIS_PORT || '6379',
}