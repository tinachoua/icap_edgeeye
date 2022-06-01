import axios from 'axios';
import debugLog from "../config/debugLog";
import { ICAP_AUTH_SERVICE } from '../config/env'
class Authentication {

    async tokenChecker(token: string) {
        if (!token) {
            return false;
        }

        try {
            debugLog.info('At tokenChecker')

            const result = await axios.get(`http://${ICAP_AUTH_SERVICE}/AuthenticationAPI/TokenChecker`, { headers: { token } })

            debugLog.success(result.status)
            debugLog.success(result.data)

            return result.status === 200 ? result.data : false

        } catch (error) {
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