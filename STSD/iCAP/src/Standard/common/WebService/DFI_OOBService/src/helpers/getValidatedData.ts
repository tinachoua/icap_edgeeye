import { Response } from "express";

export default (res: Response) => res.locals.validatedData;