import axios from 'axios';
import { OOB_HOST } from '../config/env';

export const getDeviceProgressAPI = (mcu_number: string) => {
    return axios.get(`http://${OOB_HOST}/devices/device-progress/${mcu_number}`);
}

export const getSSDInfoAPI = (mcu_number: string) => {
    return axios.get(`http://${OOB_HOST}/devices/SSDInfo/${mcu_number}`);
}

export const recoveryAPI = (mcu_number: string) => {
    return axios.post(`http://${OOB_HOST}/devices/recovery`, { serialNumber: mcu_number });
}

export const rebootAPI = (mcu_number: string) => {
    return axios.post(`http://${OOB_HOST}/devices/reboot`, { serialNumber: mcu_number });
}

export const getStatusAPI = (mcu_numbers: string[]) => {
    return axios.post(`http://${OOB_HOST}/devices/status`, mcu_numbers);
}

export const getSphereStatusAPI = (mcu_number: string) => {
    return axios.get(`http://${OOB_HOST}/devices/status/sphere-status/${mcu_number}`);
}
export const getOSHeartbeatAPI = (mcu_number: string) => {
    return axios.get(`http://${OOB_HOST}/devices/status/os-heartbeat/${mcu_number}`);
}
export const getBackupStatusAPI = (mcu_number: string) => {
    return axios.get(`http://${OOB_HOST}/devices/status/backup-status/${mcu_number}`);
}
export const PowerSwitchAPI = ({ mcu_number, mode, time, timeUnit }: { mcu_number: string, mode: number, time: number, timeUnit: string }) => {
    return axios.post(`http://${OOB_HOST}/devices/power-switch`, { "serialNumber": mcu_number, "mode": mode, "time": time, "timeUnit": timeUnit });
}
export const BackupClearAPI = (mcu_number: string) => {
    return axios.post(`http://${OOB_HOST}/devices/backup/clear`, { "serialNumber": mcu_number });
}
export const backupAPI = ({ mcu_number, partId, sector }: { mcu_number: string, partId: number, sector: number }) => {
    return axios.post(`http://${OOB_HOST}/devices/backup`, { "serialNumber": mcu_number, "partId": partId, "sector": sector });
}


