
import { Request, Response, NextFunction } from 'express';
import debugLog from '../config/debugLog'
import * as outOfBandAPIs from './outOfBandAPIs'
import { handleAxiosError } from '../utils/errorHandler'
import { getValue as getRedisValue } from '../redis/index'

class outOfBandAPIController {
    public async reboot(req: Request, res: Response, _next: NextFunction) {

        try {

            const { serialNumber } = req.params

            const result = await outOfBandAPIs.rebootAPI(serialNumber);

            debugLog.info('at reboot status: ', result.status)

            return res.status(result.status).json(result.data)

        } catch (error) {

            return handleAxiosError({
                error,
                res,
                target: 'reboot'
            })
        }
    }
    public async recovery(req: Request, res: Response, _next: NextFunction) {

        try {

            const { serialNumber } = req.params
            const result = await outOfBandAPIs.recoveryAPI(serialNumber);

            debugLog.info('at recovery status: ', result.status)

            return res.status(result.status).json(result.data)

        } catch (error) {
            return handleAxiosError({
                error,
                res,
                target: 'recovery'
            })
        }
    }

    public async powerSwitch(req: Request, res: Response, _next: NextFunction) {

        try {

            const { serialNumber } = req.params
            const { mode, time, timeUnit } = req.body
            const result = await outOfBandAPIs.PowerSwitchAPI({
                mcu_number: serialNumber,
                mode,
                time,
                timeUnit
            });

            debugLog.info('at powerSwitch status: ', result.status)

            return res.status(result.status).json(result.data)

        } catch (error) {

            return handleAxiosError({
                error,
                res,
                target: 'powerSwitch'
            })
        }
    }

    public async backup(req: Request, res: Response, _next: NextFunction) {

        try {

            const { serialNumber } = req.params
            const { partId, sector } = req.body
            const result = await outOfBandAPIs.backupAPI({
                mcu_number: serialNumber,
                partId,
                sector,
            });

            debugLog.info('at backup status: ', result.status)

            return res.status(result.status).json(result.data)

        } catch (error) {

            return handleAxiosError({
                error,
                res,
                target: 'backup'
            })
        }
    }

    public async getSSDInfo(req: Request, res: Response, _next: NextFunction) {

        try {

            const { serialNumber } = req.params

            const result = await outOfBandAPIs.getSSDInfoAPI(serialNumber);

            debugLog.info('at getSSDInfoAPI result: ', result.data)

            return res.status(result.status).json(result.data)

        } catch (error) {

            return handleAxiosError({
                error,
                res,
                target: 'getSSDInfo'
            })

        }
    }
    public async getBackupStatus(req: Request, res: Response, _next: NextFunction) {

        try {

            const { serialNumber } = req.params

            const result = await outOfBandAPIs.getBackupStatusAPI(serialNumber);

            debugLog.info('at getBackupStatus result: ', result.data)

            return res.status(result.status).json(result.data)

        } catch (error) {

            return handleAxiosError({
                error,
                res,
                target: 'getBackupStatus'
            })

        }
    }

    public async backupClear(req: Request, res: Response, _next: NextFunction) {

        try {

            const { serialNumber } = req.params

            const result = await outOfBandAPIs.BackupClearAPI(serialNumber);

            debugLog.info('at getBackupClear result: ', result.data)

            return res.status(result.status).json(result.data)

        } catch (error) {

            return handleAxiosError({
                error,
                res,
                target: 'getBackupClear'
            })

        }
    }

    public async getSphereStatus(req: Request, res: Response, _next: NextFunction) {

        try {

            const { serialNumber } = req.params

            const result = await outOfBandAPIs.getSphereStatusAPI(serialNumber);

            debugLog.info('at getSphereStatus result: ', result.data)

            return res.status(result.status).json(result.data)

        } catch (error) {

            return handleAxiosError({
                error,
                res,
                target: 'getSphereStatus'
            })

        }
    }

    public async getOSHeartbeat(req: Request, res: Response, _next: NextFunction) {

        try {

            const { serialNumber } = req.params

            const result = await outOfBandAPIs.getOSHeartbeatAPI(serialNumber);

            debugLog.info('at geOSHeartbeat result: ', result.data)

            return res.status(result.status).json(result.data)

        } catch (error) {

            return handleAxiosError({
                error,
                res,
                target: 'geOSHeartbeat'
            })

        }
    }

    public async getDevicesStatus(req: Request, res: Response, _next: NextFunction) {
        try {

            const { sn } = req.query
            const serialNumbers = (sn as string).split(',')

            let statusObj: { [index: string]: string } = {}

            for await (let sn of serialNumbers) {
                statusObj[sn] = '' + (await getRedisValue(2, sn))
            }

            debugLog.info('at getDevicesStatus result: ', statusObj)
            return res.status(200).json(statusObj)

        } catch (error) {

            const { code } = error
            debugLog.error('getDevicesStatus error.code: ', code)
            if (error.code) {
                return res.status(500).json({ msg: code })
            }

            const { status, data } = error.response
            debugLog.error('at getDevicesStatus error: ', data, status)
            return res.status(400).json(data)

        }
    }
}

export default outOfBandAPIController;