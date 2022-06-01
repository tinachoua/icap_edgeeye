import MainRoute from "./routeAbstract";
import OutOfBandRoutes from './outOfBandRoute'

const router: Array<MainRoute> = [
    new OutOfBandRoutes()
];

export default router;