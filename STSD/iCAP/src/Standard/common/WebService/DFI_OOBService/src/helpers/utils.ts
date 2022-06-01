interface InputObj {
  [key: string]: any;
}

export function getObjectValue(object: InputObj, keys: string | Array<string>, fallback: any) {
    if (!object) {
        return fallback;
    }

    if (!keys) {
        return object;
    }

    if (typeof keys === 'string') {
        keys = keys.split('.')
    }

    for (let i = 0, l = keys.length; object && i < l; i++) {
        object = object[keys[i]];
    }

    if (object === null || typeof object === 'undefined') {
        return fallback
    } else {
        return object;
    }
}