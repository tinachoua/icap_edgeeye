import debug from "debug";
import { DEBUG_MODE } from './env'

const error = debug("Error");
const info = debug("Info");
const warn = debug("Warn");
const success = debug("Success");

info.color = '0'
error.color = '1'
success.color = '2'
warn.color = '3'

if (DEBUG_MODE) {
    error.enabled = true;
    info.enabled = true;
    warn.enabled = true;
    success.enabled = true;
}

error('Debug Mode On !')
info('Debug Mode On !')
warn('Debug Mode On !')
success('Debug Mode On !')

export default {
    error,
    info,
    warn,
    success
};