import Authentication from '../library/Authentication';
import { Request, Response, NextFunction } from "express";
import { debugLog } from "../library/config";
import { ExtendableError } from "../library/errorHandler";
import httpStatus from "http-status";

export const isLegitimateToken = async (req: Request, _res: Response, next: NextFunction) => {

    const { token } = req.headers

    if (!token) {
        next(new ExtendableError('沒有Token喔', httpStatus.FORBIDDEN, true));
        return;
    }

    debugLog(token)

    const isLegitimateToken = await Authentication.tokenChecker(token.toString());

    if (isLegitimateToken) {
        next();
    } else {
        next(new ExtendableError('Token怪怪的喔', httpStatus.FORBIDDEN, true));
    }

}

export const isAdmin = async (req: Request, _res: Response, next: NextFunction) => {

    const { token } = req.headers

    if (!token) {
        next(new ExtendableError('沒有Token喔', httpStatus.FORBIDDEN, true));
        return;
    }

    debugLog(token)

    const isAdmin = await Authentication.isAdmin(token.toString());

    if (isAdmin) {
        next();
    } else {
        next(new ExtendableError('權限怪怪的喔', httpStatus.FORBIDDEN, true));
    }

}