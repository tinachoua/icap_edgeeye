export const LED_STATUS_TOPIC = 'ledstatusRecv';
export const INNOAGE_STATUS_TOPIC = 'statusRecv';
export const INNOAGE_RECOVERY_TOPIC = 'percentage';

const LED_STATUS_SERVICE = 'LED_STATUS';
const INNOAGE_STATUS_SERVICE = 'INNOAGE_STATUS';
const RECOVERY_PERCENTAGE_SERVICE = 'RECOVERY_PERCENTAGE';


export const TOPIC_MAP_SERVICE = {
    [LED_STATUS_TOPIC]: LED_STATUS_SERVICE,
    [INNOAGE_STATUS_TOPIC]: INNOAGE_STATUS_SERVICE,
    [INNOAGE_RECOVERY_TOPIC]: RECOVERY_PERCENTAGE_SERVICE
}

const LED_STATUS_TOPIC_REGEX = new RegExp(`${LED_STATUS_TOPIC}\\/\\w+`, 'i');
const INNOAGE_STATUS_TOPIC_REGEX = new RegExp(`${INNOAGE_STATUS_TOPIC}\\/\\w+`, 'i');
const INNOAGE_RECOVERY_TOPIC_REGEX = new RegExp(`${INNOAGE_RECOVERY_TOPIC}\\/\\w+`, 'i');

export const TOPIC_REGEX = {
    LED_STATUS_TOPIC_REGEX,
    INNOAGE_STATUS_TOPIC_REGEX,
    INNOAGE_RECOVERY_TOPIC_REGEX
}