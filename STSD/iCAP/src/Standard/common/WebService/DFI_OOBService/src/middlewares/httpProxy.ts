import { Request, Response, NextFunction } from "express";
import getValidatedData from '../helpers/getValidatedData';
import axios from 'axios';
import { INNOAGE_SERVICE_HOST } from '../constants/env';
import debugLog from '../helpers/loggers';
import { ExtendableError } from "../library/errorHandler";
import httpStatus from "http-status";

interface httpRequest {
  [key: string]: Function
}

const OOB_API_ROUTER = {
    RECOVERY: 'devices/recovery',
    REBOOT: 'devices/reboot',
    SSDINFO: 'devices/SSDInfo',
    POWER_SWITCH: 'DFI/devices/power-switch',
    SPHERE_STATUS: 'devices/status/sphere-status'
}

const director = {
    '/InnoAGE/recovery': {
        method: 'post',
        path: OOB_API_ROUTER.RECOVERY,
        url: `http://${INNOAGE_SERVICE_HOST}/devices/recovery`
    },
    '/InnoAGE/reboot': {
        method: 'post',
        path: OOB_API_ROUTER.REBOOT,
        url: `http://${INNOAGE_SERVICE_HOST}/devices/reboot`
    },
    '/InnoAGE/SSDInfo/:sn': {
        method: 'get',
        path: OOB_API_ROUTER.SSDINFO,
        url: `http://${INNOAGE_SERVICE_HOST}/devices/SSDInfo`
    },
    '/InnoAGE/power-switch': {
        method: 'post',
        path: OOB_API_ROUTER.POWER_SWITCH,
        url: `http://${INNOAGE_SERVICE_HOST}/DFI/devices/power-switch`
    }
} as any;

const httpRequest: httpRequest = {
    'get': async ({ url, data }: { url: string, data: any }) => {
        return axios.get(`${url}/${data.serialNumber}`)
    },
    'post': ({ url, data }: { url: string, data: any }) => {
        return axios.post(url, data)// {ser...: sn}
    },
}

function getNewRoute(req: Request) {
    return director[req.route.path];
}

export default async function (req: Request, res: Response, next: NextFunction) {
    const data: any = getValidatedData(res) || req.query;
    const newRoute = getNewRoute(req);
    try {
        if (!data || !newRoute) {
            return next(new Error('proxy route or data not found'));
        }

        const { method, url } = newRoute;
        const { sn } = data;
        if (typeof httpRequest[method] === 'function') {
            debugLog('proxy', method, url, sn)
            const response = await httpRequest[method]({ url, data: { serialNumber: sn } });
            res.locals.proxyResponse = response.data;
            return next() // handlerProxyRecvData => response data;
        }
    } catch (error) {
        console.log('Handle API proxy error', error);
        return next(new ExtendableError('Handle API proxy error', httpStatus['INTERNAL_SERVER_ERROR'], true))
    }
}