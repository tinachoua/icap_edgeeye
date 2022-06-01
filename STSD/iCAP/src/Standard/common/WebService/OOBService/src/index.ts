import debugLog from './config/debugLog'
import { appServer } from "./server"
import { PORT, OOB_WEBSOCKET_HOST, OOB_HOST, OOB_WEBSOCKET_RETRY_FREQ, ICAP_AUTH_SERVICE, REDIS_HOST, DEBUG_MODE } from './config/env'

appServer.listen(PORT, () => {
    console.log('Express server listening on Port ', PORT);
})
debugLog.info('Hello World')

debugLog.info('OOB_WEBSOCKET_HOST: ', OOB_WEBSOCKET_HOST)
debugLog.info('OOB_HOST: ', OOB_HOST)
debugLog.info('OOB_WEBSOCKET_RETRY_FREQ: ', OOB_WEBSOCKET_RETRY_FREQ)
debugLog.info('ICAP_AUTH_SERVICE: ', ICAP_AUTH_SERVICE)
debugLog.info('REDIS_HOST: ', REDIS_HOST)
debugLog.info('DEBUG_MODE: ', DEBUG_MODE)