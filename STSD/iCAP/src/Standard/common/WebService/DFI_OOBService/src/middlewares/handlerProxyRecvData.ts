import { Request, Response, NextFunction } from "express";
import { camelizeKeys } from 'humps';
import { getObjectValue } from '../helpers/utils';
import { OOB_API_RESPONSE_DATA_PATH } from '../constants/oob';

export default async function (_req: Request, res: Response, next: NextFunction) {
    const formatedData = camelizeKeys(getObjectValue(res.locals.proxyResponse, OOB_API_RESPONSE_DATA_PATH, null));
    try {
        res.status(200).send({ data: formatedData })
    } catch (error) {
        next(error)
    }
}