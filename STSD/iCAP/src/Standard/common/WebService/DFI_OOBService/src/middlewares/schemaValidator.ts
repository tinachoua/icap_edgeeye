import InnoAGESchemas from '../constants/schema/';
import { Request, Response, NextFunction } from "express";

export default function schemaValidator(useJoiError: boolean = false) {
    // useJoiError determines if we should respond with the base Joi error
    // boolean: defaults to false
    const _useJoiError = useJoiError;

    // enabled HTTP methods for request data validation
    const _supportedMethods = ['post'];

    // return the validation middleware
    return (req: Request, res: Response, next: NextFunction) => {
        const route = req.route.path;
        const method = req.method.toLowerCase();

        if (_supportedMethods.includes(method) && Object.prototype.hasOwnProperty.call(InnoAGESchemas, route)) {

            // get schema for the current route
            const _schema = InnoAGESchemas[route as keyof typeof InnoAGESchemas];

            if (_schema) {
                // Validate req.body using the schema and validation options
                const { error } = _schema.validate(req.body);

                if (error) {
                    // Custom Error
                    const CustomError = {
                        status: 'failed',
                        error: 'Invalid request data. Please review request and try again.'
                    };

                    res.status(422).json(_useJoiError ? error : CustomError);
                } else {
                    res.locals.validatedData = req.body;
                    next();
                }
            }
        } else {
            next();
        }
    };
}
