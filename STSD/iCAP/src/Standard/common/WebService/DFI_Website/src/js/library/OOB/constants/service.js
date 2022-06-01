export const POWER_SERVICE = 'power';
export const REBOOT_SERVICE = 'reboot';
export const RECOVERY_SERVICE = 'recovery';

export const PROCESS_END = 'end';
 
export const INNOAGE_REMOTE_SUCCESS = '1';
export const INNOAGE_REMOTE_FAIL = '0';
export const INNOAGE_OFFLINE = '2'

export const SERVICE_ON = 1;
export const SERVICE_OFF = 0;

export const API_TIMEOUT = 9000;

export const FAKE_PROGRESS_RATE = 90;
export const PROGRESS_ZERO_RATE = 0;
export const PROGRESS_COMPLETE_RATE = 100;

export const POWER_TEXT = 'Power';
export const REBOOT_TEXT = 'Reboot';
export const RECOVERY_TEXT = 'Recovery';

export const INNOAGE_ISOFFLINE_TEXT = ({sn}) => `InnoAGE ${sn} is offline.`;
export const INNOAGE_REMOTE_FAIL_TEXT = ({sn, SERVICE_TEXT}) => `InnoAGE ${sn} ${SERVICE_TEXT} failed.`;