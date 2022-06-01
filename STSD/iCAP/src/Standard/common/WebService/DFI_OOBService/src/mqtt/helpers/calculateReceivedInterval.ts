import debugLog from '../../helpers/loggers';
import {
    LED_STATUS_TOPIC,
    INNOAGE_STATUS_TOPIC,
    INNOAGE_RECOVERY_TOPIC
} from '../../constants/mqtt';
import perfHooks from 'perf_hooks';

interface LastReceviced {
  [key: string]: any
}

let performance: any;

try {
    performance = perfHooks.performance
} catch (e) { /** In build time, perf_hooks is initially imported fail. Ignore this error */ }

const lastReceviced: LastReceviced = {
    [LED_STATUS_TOPIC]: {},
    [INNOAGE_STATUS_TOPIC]: {},
    [INNOAGE_RECOVERY_TOPIC]: {}
}

export default function ({
    topic, sn
}: { topic: string, sn: string }) {
    if (lastReceviced[topic][sn]) {
        const end = performance.now()
        const delta = (end - lastReceviced[topic][sn]).toFixed(2);
        debugLog(`Topic:${topic} Received Interval: ${delta}ms`)
        lastReceviced[topic][sn] = end;
    } else {
        lastReceviced[topic][sn] = performance.now()
    }
}
