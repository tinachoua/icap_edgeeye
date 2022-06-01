
import MainRoute from './routeAbstract';
import { isLegitimateToken, isAdmin } from '../middleWares/authenticationHandler';

import OutOfBandAPIController from '../controller/outOfBandAPIController'

class OutOfBandRoutes extends MainRoute {

    private outOfBandAPIController: OutOfBandAPIController = new OutOfBandAPIController()

    constructor() {
        super();
        this.setRoutes();
    }

    protected setRoutes() {

        this.router.route('/status')
            .get(isLegitimateToken, this.outOfBandAPIController.getDevicesStatus)

        this.router.route('/recovery/:serialNumber')
            .get(isAdmin, this.outOfBandAPIController.recovery)

        this.router.route('/reboot/:serialNumber')
            .get(isAdmin, this.outOfBandAPIController.reboot)

        this.router.route('/sphere-status/:serialNumber')
            .get(isAdmin, this.outOfBandAPIController.getSphereStatus)

        this.router.route('/ssd-info/:serialNumber')
            .get(isAdmin, this.outOfBandAPIController.getSSDInfo)

        this.router.route('/os-heartbeat/:serialNumber')
            .get(isAdmin, this.outOfBandAPIController.getOSHeartbeat)

        this.router.route('/backup-status/:serialNumber')
            .get(isAdmin, this.outOfBandAPIController.getBackupStatus)

        this.router.route('/power-switch/:serialNumber')
            .post(isAdmin, this.outOfBandAPIController.powerSwitch)

        this.router.route('/backup-clear/:serialNumber')
            .post(isAdmin, this.outOfBandAPIController.backupClear)

        this.router.route('/backup/:serialNumber')
            .post(isAdmin, this.outOfBandAPIController.backup)

    }
}

export default OutOfBandRoutes;