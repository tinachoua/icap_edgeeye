import { Request, Response, NextFunction } from "express";
import { getStatus } from '../helpers/InnoAGEStatus';
import getValidatedData from '../helpers/getValidatedData';

const API_RESPONSE_DATA_STATUS = {
    FAIL: '0',
    SUCCESS: '1',
    ISOFFLINE: '2'
}

export default function (_req: Request, res: Response, next: NextFunction) {
    const { sn } = getValidatedData(res);
    getStatus(sn).then((status) => {
        const IS_ONLINE_REGEX = /^[1]$/;

        if (`${status}`.match(IS_ONLINE_REGEX)) {
            next();
        } else {
            res.status(200).send({
                data: {
                    status: API_RESPONSE_DATA_STATUS.ISOFFLINE
                }
            })
        }
    });
}