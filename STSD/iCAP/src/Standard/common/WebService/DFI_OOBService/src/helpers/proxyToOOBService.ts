import { INNOAGE_SERVICE_HOST } from '../constants/env';
import axios from 'axios';
import { getObjectValue } from './utils';
import { OOB_API_RESPONSE_DATA_PATH } from '../constants/oob';
import { camelizeKeys } from 'humps';

const httpRequest = {
    'get': async ({ path, serialNumber }: { path: string, serialNumber: string }) => {
        return axios.get(`http://${INNOAGE_SERVICE_HOST}/${path}/${serialNumber}`)
    },
    'post': ({ path, serialNumber }: { path: string, serialNumber: string }) => {
        return axios.post(`http://${INNOAGE_SERVICE_HOST}/${path}`, { serialNumber })
    },
}

export default async ({
    path,
    serialNumber,
    method
}: { path: string, serialNumber: string, method: 'get' | 'post' }) => {
    const response = await httpRequest[method]({ serialNumber, path });
    return camelizeKeys(getObjectValue(response.data, OOB_API_RESPONSE_DATA_PATH, null));
}


