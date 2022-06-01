import { DEBUG } from '../constants/env';

export default function (...args: any) {
    if (DEBUG) {
        console.log(...args)
    }
}
