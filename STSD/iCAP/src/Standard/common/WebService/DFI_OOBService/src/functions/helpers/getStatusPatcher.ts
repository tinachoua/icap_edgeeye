import { getStatus } from '../../helpers/InnoAGEStatus';

export default async function getStatusPatcher(sn: string) {
    const status = await getStatus(sn);
    return {
        sn,
        status
    }
}
