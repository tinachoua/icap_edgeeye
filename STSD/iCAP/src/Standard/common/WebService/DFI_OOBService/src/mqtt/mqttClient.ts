import { MQTT_HOST, MQTT_PORT, MQTT_PASSWORD, MQTT_USERNAME } from '../constants/env';
import MQTT from "async-mqtt";
import { TOPIC_REGEX, TOPIC_MAP_SERVICE } from '../constants/mqtt';
import sendToClientThroughWebsocket from '../helpers/sendToClientThroughWebsocket';
import calculateReceivedInterval from './helpers/calculateReceivedInterval';
import { debugLog } from '../library/config';
import { saveInnoAGEStatus } from '../helpers/InnoAGEStatus';
import { initialMQTTTopic } from '../constants/innoAGEMQTTToppics';
interface TopicInfo {
    topic: string;
    sn: string;
}

function topicParser(completeTopic: string): TopicInfo {
    return Object.keys(TOPIC_REGEX).reduce((accu, curr, _idx, arr) => {
        const result = completeTopic.match(TOPIC_REGEX[curr as keyof typeof TOPIC_REGEX]);
        if (result) {
            const [topic, sn] = result[0].split('/');
            accu = {
                topic,
                sn
            }
            arr.splice(1);   // eject early by mutating iterated copy
        }
        return accu;
    }, { topic: '', sn: '' })
}

export default function () {
    const mqttClient = MQTT.connect(
        `${MQTT_HOST}:${MQTT_PORT}`,
        {
            username: MQTT_USERNAME,
            password: MQTT_PASSWORD
        }
    );

    mqttClient.on('message', (completeTopic: string, payload: Buffer) => {
        const { topic, sn } = topicParser(completeTopic);
        const data = JSON.parse((payload.toString()));
        calculateReceivedInterval({
            topic,
            sn
        });
        const message = {
            data: {
                sn,
                ...data
            },
            type: TOPIC_MAP_SERVICE[topic as keyof typeof TOPIC_MAP_SERVICE]
        }
        const statusRecvTopic = initialMQTTTopic.find((element) => element === 'innoageSphere/statusRecv/+') || '';

        debugLog('mqttClient on message', data);

        // ex: [innoageSphere, statusRecv, +]
        if (topic === statusRecvTopic.split('/')[1]) {
            saveInnoAGEStatus(sn, +data.Status);
        }

        sendToClientThroughWebsocket(message);
    })

    return mqttClient;
}