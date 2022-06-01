import { Request } from "express";

function getValueFromQuery(req: Request, key: string) {
    if (!req.query) return null;

    return req.query[key];
}

function getValueFromBody(req: Request, key: string) {
    if (!req.body) return null;

    return req.body[key];
}

export function getData(req: Request, key: string) {
    let result;

    result = getValueFromQuery(req, key);

    if (!result) {
        result = getValueFromBody(req, key);
    }

    return result;
}


export function getSerialNumbers(req: Request): string[] {
    const result = getData(req, 'sn');

    if (!result) return [];

    if (!Array.isArray(result)) {
        return result.split(',');
    }

    return result;
}

export function getSerialNumber(req: Request) {
    const result = getData(req, 'sn');

    if (Array.isArray(result)) {
        return result.join('');
    }

    return result;
}
