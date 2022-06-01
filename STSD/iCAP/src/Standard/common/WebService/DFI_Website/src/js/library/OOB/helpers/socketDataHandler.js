import {
  SERVICE_ON
} from '../constants/service';
import { 
  WS_INNOAGE_STATUS_TYPE,
  WS_INNOAGE_RECOVERY_TYPE,
  WS_LED_STATUS_TYPE
 } from '../constants/websocket';
import updateStatus  from './updateStatus';
import { PROCESS_END } from '../constants/service';

const STATUS_UNKNOWN = -1;

function getFakeStatus (){
  let status = 1;
  return function(){
    status = status ^ 1;
    return status;
  }
}

function isOOBPage(){
  const oob = document.getElementsByName('oob')[0];
  return (oob !== undefined && oob.style.display !== 'none')
}

const fakeStatus = getFakeStatus();

function hasTimer({
  mediator, devName, sn
}){
  return mediator[devName][sn].updateHDStatusTimer !== undefined 
}

function twinkleHDStatus({
  mediator,
  devName,
  sn
}){
  mediator[devName][sn].updateHDStatusTimer = setInterval(()=>{
    const hdStatus = document.getElementById('hd-status');
    isOOBPage();
    if(hdStatus && isOOBPage()){
      updateStatus({
        element:hdStatus,
        status: fakeStatus()
      })
    } else {
      clearInterval(mediator[devName][sn].updateHDStatusTimer)
      mediator[devName][sn].updateHDStatusTimer = undefined;
    }
  }, 800);
}

const socketDataHandler = {
  [WS_INNOAGE_STATUS_TYPE]: function({Status, mediator}){
    const powerStatus = document.getElementById('innoage-status');
    updateStatus({
      element: powerStatus,
      status: Status
    })
 
    if (Number(Status) === 0) {
      const powerStatus = document.getElementById('power-status');
      const hdStatus = document.getElementById('hd-status');
      updateStatus({
        element: hdStatus,
        status: STATUS_UNKNOWN
      })
      updateStatus({
        element: powerStatus,
        status: STATUS_UNKNOWN
      })
      mediator.disabledServices();
    } else {
      mediator.enabledService();
    }
  },
  [WS_INNOAGE_RECOVERY_TYPE]: function({sn, Percentage, recovery, devName, device, mediator}){
    if (!hasTimer({mediator,sn,devName}) && isOOBPage()) {
      twinkleHDStatus({
        mediator,
        devName,
        sn
      })
    } else if (Percentage === PROCESS_END && hasTimer({mediator,sn,devName})){
      clearInterval(mediator[devName][sn].updateHDStatusTimer);
      mediator[devName][sn].updateHDStatusTimer = undefined;
    }

    recovery.updateBar({
      devName, 
      SN: sn,
      Percentage: Percentage
    })

    mediator[devName].recovery = SERVICE_ON;
    mediator.disabledServices();
  },
  [WS_LED_STATUS_TYPE]: function({HDstatus, Powerstatus, sn}){
    const powerStatus = document.getElementById('power-status');
    const hdStatus = document.getElementById('hd-status');
    updateStatus({
      element: hdStatus,
      status: HDstatus
    })
    updateStatus({
      element: powerStatus,
      status: Powerstatus
    })
  }
}

export default socketDataHandler;