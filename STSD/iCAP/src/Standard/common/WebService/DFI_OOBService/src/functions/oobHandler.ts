import { Request, Response, NextFunction } from "express";
import httpStatus from "http-status";
import { ExtendableError } from "../library/errorHandler";
import { debugLog } from "../library/config";
import InnoAgeAPIs from '../library/InnoAgeAPIs'
import getStatusPatcher from './helpers/getStatusPatcher';
import proxyToOOBService from '../helpers/proxyToOOBService';
import { OOB_API_ROUTER } from '../constants/oob';
import { getSerialNumbers, getSerialNumber } from '../helpers/requestParser';

export async function recovery(req: Request, res: Response, next: NextFunction) {
    const sn = getSerialNumber(req);
    try {
        const result = await proxyToOOBService({
            path: OOB_API_ROUTER.RECOVERY,
            method: 'post',
            serialNumber: sn
        });
        res.status(200).send({ data: result })
    } catch (error) {
        debugLog('At recovery error: ', error)
        next(new ExtendableError(error, httpStatus.BAD_REQUEST, true));
    }
}

export async function powerSwitch(req: Request, res: Response, next: NextFunction) {
    const sn = getSerialNumber(req);
    try {
        const result = await proxyToOOBService({
            path: OOB_API_ROUTER.POWER_SWITCH,
            method: 'post',
            serialNumber: sn
        });
        res.status(200).send({ data: result })
    } catch (error) {
        debugLog('At powerSwitch error: ', error)
        next(new ExtendableError(error, httpStatus.BAD_REQUEST, true));
    }
}

export async function reboot(req: Request, res: Response, next: NextFunction) {
    const sn = getSerialNumber(req);
    try {
        const result = await proxyToOOBService({
            path: OOB_API_ROUTER.REBOOT,
            method: 'post',
            serialNumber: sn
        });
        res.status(200).send({ data: result })
    } catch (error) {
        debugLog('At reboot error: ', error)
        next(new ExtendableError(error, httpStatus.BAD_REQUEST, true));
    }
}

export async function getLEDStatus(req: Request, res: Response, next: NextFunction) {
    const { serialNumber } = req.params
    try {
        const result = await InnoAgeAPIs.getLEDStatus(serialNumber);
        debugLog('At getLEDStatus succsee: ', result.data)
        res.status(200).send(result.data)
        next();
    } catch (error) {
        debugLog('At getLEDStatus error: ', error)
        next(new ExtendableError(error, httpStatus.BAD_REQUEST, true));
    }
}

// export async function getSSDInfo(req: Request, res: Response, next: NextFunction) {
//   const sn = getSerialNumber(req);
//   try {
//     const result = await proxyToOOBService({
//       path: OOB_API_ROUTER.SSDInfo,
//       method: 'get',
//       serialNumber: sn
//     });
//     res.status(200).send({data: result})
//   } catch (error) {
//     debugLog('At reboot error: ', error)
//     next(new ExtendableError(error, httpStatus.BAD_REQUEST, true));
//   }
// }


export async function getStatus(req: Request, res: Response, next: NextFunction) {
    const serialNumbers = getSerialNumbers(req);
    try {
        const promises = [];

        for (let i = 0; i < serialNumbers.length; i++) {
            promises.push(getStatusPatcher(serialNumbers[i]))
        }

        Promise.all(promises).then((result) => {
            res.status(200).send({ data: result })
        });

    } catch (error) {
        debugLog(error)
        next(new ExtendableError(error, httpStatus.BAD_REQUEST, true));
    }
}