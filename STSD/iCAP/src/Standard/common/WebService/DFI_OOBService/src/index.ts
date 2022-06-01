import express from "express";
import http from "http";
import helmet from "helmet";
import bodyparser from "body-parser";
import { SERVICE_PORT } from "./constants/env";
import debugLog from "./helpers/loggers";
import { errorCatch } from "./models/errorCatch";
import { isLegitimateToken, isAdmin } from './models/AuthenticationHandler';
import { start as startWebsocket } from './websocket/websocket';
import checkInnoAGEStatus from './middlewares/checkInnoAGEStatus';
import httpProxy from './middlewares/httpProxy';
import handlerProxyRecvData from './middlewares/handlerProxyRecvData';
import { getStatus } from './functions/oobHandler';
import handlerParams from './middlewares/handlerParams';
import { subscribeAsync } from "./mqtt";
import schemaValidator from './middlewares/schemaValidator';
import { initialMQTTTopic } from './constants/innoAGEMQTTToppics';

subscribeAsync(initialMQTTTopic).then((response) => {
    debugLog(response);
});

const app = express();
const server = http.createServer(app);

if (!process.env.DEBUG) {
    app.use(helmet());
}

// 使用 bodyparser.json() 將 HTTP 請求方法 POST、DELETE、PUT 和 PATCH，放在 HTTP 主體 (body) 發送的參數存放在 req.body
app.use(bodyparser.urlencoded({ extended: false }));
app.use(bodyparser.json());
app.use(isLegitimateToken);
app.use(errorCatch);

const validateRequest = schemaValidator(true);

app.get('/InnoAGE/status', getStatus);

app.get('/InnoAGE/SSDInfo/:sn',
    handlerParams,
    checkInnoAGEStatus,
    httpProxy,
    handlerProxyRecvData
);

app.post('/InnoAGE/recovery',
    isAdmin,
    validateRequest,
    checkInnoAGEStatus,
    httpProxy,
    handlerProxyRecvData
);

app.post('/InnoAGE/reboot',
    isAdmin,
    validateRequest,
    checkInnoAGEStatus,
    httpProxy,
    handlerProxyRecvData
);

app.post('/InnoAGE/power-switch',
    isAdmin,
    validateRequest,
    checkInnoAGEStatus,
    httpProxy,
    handlerProxyRecvData
);

server.listen(SERVICE_PORT, () => {
    debugLog(`Service listening on port ${SERVICE_PORT}!`);
});

startWebsocket(server)