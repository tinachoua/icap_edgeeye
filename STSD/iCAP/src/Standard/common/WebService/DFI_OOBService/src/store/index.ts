import { debugLog } from "../library/config";

// { B0011905300270070: { status: '1' } }
const data: any = {};

interface Options {
    writable: boolean
}

export function set(key: string, value: any, options?: Options) {
    if (Object.prototype.hasOwnProperty.call(data, key) && !options?.writable) {
        throw Error('key exist')
    }
    data[key] = value;
    debugLog('Update online status in the program variable.', data);
    return true;
}

export function remove(key: string) {
    delete data[key]
    debugLog('Delete online status in the program variable.', data);
    return true;
}

export function get(key: string): any {
    debugLog('Get online status in the program variable.', data);
    return data[key];
}
