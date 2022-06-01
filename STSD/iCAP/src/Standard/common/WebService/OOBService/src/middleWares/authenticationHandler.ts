import authentication from '../utils/authentication';
import { Request, Response, NextFunction } from "express";
import debugLog from "../config/debugLog";
import { ExtendableError } from "../utils/errorHandler";
import httpStatus from "http-status";

export const isLegitimateToken = async (req: Request, _res: Response, next: NextFunction) => {

    const { token } = req.headers

    if (!token) {
        next(new ExtendableError('沒有Token喔', true, httpStatus.FORBIDDEN, 'error', 'isLegitimateToken'));
        return;
    }

    debugLog.info('isLegitimateToken')
    debugLog.info('token:', token)

    const isLegitimateToken = await authentication.tokenChecker(token.toString());

    if (isLegitimateToken) {
        next();
    } else {
        next(new ExtendableError('Token怪怪的喔', true, httpStatus.FORBIDDEN, 'error', 'isLegitimateToken'));
    }
}

export const isAdmin = async (req: Request, _res: Response, next: NextFunction) => {

    const { token } = req.headers

    if (!token) {
        next(new ExtendableError('沒有Token喔', true, httpStatus.FORBIDDEN, 'error', 'isAdmin'));
        return;
    }

    debugLog.info('isAdmin')
    debugLog.info(token)

    const isAdmin = await authentication.isAdmin(token.toString());

    if (isAdmin) {
        next();
    } else {
        next(new ExtendableError('權限怪怪的喔', true, httpStatus.FORBIDDEN, 'error', 'isAdmin'));
    }

}