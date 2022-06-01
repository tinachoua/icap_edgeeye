import * as express from "express";
import helmet from "helmet";
import morgan from "morgan";
import router from './routes/router'
import { handleError } from './utils/errorHandler';
import cors from 'cors'

class App {
    public app: express.Application;

    constructor() {
        this.app = express.default();
        this.config();
        this.routerSetup();
    }

    private routerSetup(): void {

        for (const route of router) {
            this.app.use(route.getPrefix(), route.getRouter());
        }
        this.app.use(handleError)
    }

    private config(): void {
        this.app.use(helmet());
        this.app.use(cors({ origin: '*' }))
        this.app.use(express.json());
        if (process.env.DEBUG) {
            this.app.use(morgan("combined"));
        }
    }
}


export default new App().app;