import { Request, Response, NextFunction } from "express";

import debugLog from '../helpers/loggers';

export function errorCatch(err: any, _req: Request, res: Response, next: NextFunction) {
    debugLog("At errorCatch : ", err);

    res.status(err.status ? err.status : 500).json({
        message: err.isPublic ? err.message : err.status,
        code: err.code ? err.code : err.status,
    });

    next();
}