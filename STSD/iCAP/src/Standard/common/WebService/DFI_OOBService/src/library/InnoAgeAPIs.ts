import { innoAGEService } from "./config";
import axios from 'axios';

class InnoAgeAPIs {

    async getSSDInfo(serialNumber: string) {
        return axios.get(`http://${innoAGEService.host}/devices/SSDInfo/${serialNumber}`)
    }

    recovery(serialNumber: string) {
        return axios.post(`http://${innoAGEService.host}/devices/recovery`, { serialNumber })
    }

    reboot(serialNumber: string) {
        return axios.post(`http://${innoAGEService.host}/devices/reboot`, { serialNumber })
    }


    async getLEDStatus(serialNumber: string) {
        return axios.get(`http://${innoAGEService.host}/DFI/devices/led-status/${serialNumber}`)
    }

    powerSwitch(serialNumber: string) {
        return axios.post(`http://${innoAGEService.host}/DFI/devices/power-switch`, { serialNumber })
    }
}

export default new InnoAgeAPIs()