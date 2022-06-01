import { Request, Response, NextFunction } from "express";

export default async function (req: Request, res: Response, next: NextFunction) {
    res.locals.validatedData = req.params;
    return next();
}