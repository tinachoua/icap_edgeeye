import axios from 'axios';
import { AUTHENTICATION_HOST } from '../constants/env';
import debugLog from '../helpers/loggers';

class Authentication {
    async tokenChecker(token: string) {
        if (!token) {
            return false;
        }

        try {
            const result = await axios.get(`http://${AUTHENTICATION_HOST}/AuthenticationAPI/TokenChecker`, { headers: { token } })
            return result.status === 200 ? result.data : false
        } catch (error) {
            debugLog('tokenChecker', error)
            return false;
        }
    }
    async isAdmin(token: string) {
        const isAuthentication = await this.tokenChecker(token);
        if (isAuthentication) {
            const { Admin } = isAuthentication;
            return Admin;
        }
        return false;
    }
}

export default new Authentication()