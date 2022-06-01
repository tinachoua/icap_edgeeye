import mqttClient from './mqttClient';
import debugLog from '../helpers/loggers';

const client = mqttClient();

export async function subscribeAsync(topics: Array<string>) {
    try {
        return await client.subscribe(topics);
    } catch (error) {
        debugLog('error', error);
    }
}
export async function unsubscribeAsync(topics: Array<string>) {
    try {
        return await client.unsubscribe(topics);
    } catch (error) {
        debugLog('error', error);
    }
}

