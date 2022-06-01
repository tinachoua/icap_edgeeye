import { AxiosError } from "axios";
import { Request, Response, NextFunction } from "express";
import debugLog from '../config/debugLog'

export class ExtendableError extends Error {

    public message: string;
    public status: string;
    public isPublic: boolean;
    public code: number;
    public where: string;

    constructor(message: string, isPublic: boolean, code: number, status: string, where: string) {
        super();
        this.message = message;
        this.name = this.constructor.name;
        this.status = status;
        this.isPublic = isPublic;
        this.code = code;
        this.where = where;
        Error.captureStackTrace(this);
    }
}

export const handleError = (err: ExtendableError, _req: Request, res: Response, next: NextFunction) => {
    const { status, code, message, isPublic } = err;
    debugLog.error('Catch Error: ', err)
    res.status(code || 500).json({
        status,
        message: isPublic ? message : '',
    });
    next();
};

export const handleAxiosError = ({ res, error, target }: { res: Response, error: AxiosError, target: string }) => {

    if (!error.response) {
        const { code } = error
        console.log(error)
        return res.status(500).json({ msg: code })
    } else {
        const { status, data } = error.response
        debugLog.error(`At ${target} error: `, data, status)
        return res.status(400).json(data)
    }
}