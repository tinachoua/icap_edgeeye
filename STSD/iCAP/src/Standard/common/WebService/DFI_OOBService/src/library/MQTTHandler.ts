import { debugLog, mqttConfig } from "./config";
import * as  mqtt from 'async-mqtt'
import { AsyncMqttClient } from 'async-mqtt'

class MQTTHandler {

    private client!: AsyncMqttClient

    async start() {
        try {
            this.client = await mqtt.connectAsync(`${mqttConfig.host}:${mqttConfig.port}`, { username: mqttConfig.userName, password: mqttConfig.passWord })
            debugLog("MQTT Starting");
        } catch (error) {
            debugLog('mqtt start error: ', error);
        }
    }

    getMQTTClient() {
        return this.client;
    }

    async stop() {
        await this.client.end();
    }
}

export const mqttHandler = new MQTTHandler();