import { set, get as getFromStore } from '../store';
import { getValue as getValueFromRedis } from '../redis/connecter';

const INNOAGE_STATUS_DB = 2;

export function saveInnoAGEStatus(sn: string, status: number) {
    set(sn, { status: `${status}` }, { writable: true });
}

async function getValueFromRedisPatch(db: number, sn: string) {
    const status = await getValueFromRedis(db, sn);
    set(sn, { status }, { writable: true });
    return status;
}

async function get(sn: string) {
    const savedData = getFromStore(sn);
    if (savedData) {
        return savedData.status;
    } else {
        return getValueFromRedisPatch(INNOAGE_STATUS_DB, sn)
    }
}

export async function getStatus(sn: string | Array<string>) {
    if (Array.isArray(sn)) {
        const promises = sn.map(s => {
            return get(s);
        })
        return await Promise.all(promises).then((results) => results);

    } else {
        return get(sn)
    }
}
