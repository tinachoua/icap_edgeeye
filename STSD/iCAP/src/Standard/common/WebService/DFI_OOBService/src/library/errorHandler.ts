export class ExtendableError extends Error {

    public message: any;
    public status: number;
    public isPublic: boolean;
    public code?: number;

    constructor(message: any, status: number, isPublic: boolean, code?: number) {
        super(message);
        this.message = message;
        this.name = this.constructor.name;
        this.status = status;
        this.isPublic = isPublic;
        this.code = code;
        Error.captureStackTrace(this);
    }
}