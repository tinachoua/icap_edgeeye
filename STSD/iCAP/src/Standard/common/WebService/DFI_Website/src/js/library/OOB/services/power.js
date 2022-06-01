// import Button from '../../button';
import Service from './service';
import { API } from '../../API';
import { 
  POWER_SERVICE,
  INNOAGE_REMOTE_SUCCESS,
  SERVICE_ON,
  FAKE_PROGRESS_RATE,
  PROGRESS_ZERO_RATE,
  POWER_TEXT,
  INNOAGE_OFFLINE,
  INNOAGE_ISOFFLINE_TEXT,
  INNOAGE_REMOTE_FAIL_TEXT
} from '../constants/service';
import { POWER_SERVICE_ICON } from '../constants/style';
import getPowerStatus from '../helpers/getPowerStatus';
import {
   DEVICE_POWER_OFF,
   DEVICE_POWER_ON 
} from '../constants/text';

function getConfirmText(sn){
  if (getPowerStatus() === DEVICE_POWER_OFF) {
    return {
      text: `${sn} will be power on.`,
      confirmButtonText: 'Yes, power on.'
    }
  } else if (getPowerStatus() === DEVICE_POWER_ON){
    return {
      text: `${sn} will be power off.`,
      confirmButtonText: 'Yes, power off.'
    }
  }
}

function Power(Mediator){
  this.name = POWER_TEXT;
  this.btnObj = this.generateBtn({Mediator});
  this.mediator = Mediator;
}

Power.prototype = new Service();

Power.prototype.getIcon = function(){
  return POWER_SERVICE_ICON
}

Power.prototype.start = async function({sn, devName}){
  const self = this;
  const { confirm, endService, btnObj, success, fail, mediator} = self;
  const {element:{ 
      controlBar,
      circle1,
      btn
    }, 
    processBarRotate, hideIcon,
  } = btnObj;

  const result = await confirm(getConfirmText(sn));

  if (result.value) {
    mediator[devName][POWER_SERVICE] = SERVICE_ON;
    hideIcon.call(btnObj);
    circle1.setAttributeNS(null, "stroke", '#66666675');
    processBarRotate.call(btnObj, FAKE_PROGRESS_RATE, { controlBar, btn, devName});
    
    API.post({
      url:`/InnoAGE/power-switch`,
      global: false,
      success: function({data:{ status }}){
        if (status === INNOAGE_REMOTE_SUCCESS ) {
          success.call(self, {controlBar, btn, text: POWER_SERVICE, sn, devName});
        } else if (status === INNOAGE_OFFLINE){
          processBarRotate.call(btnObj, PROGRESS_ZERO_RATE, { controlBar, btn, devName});
          fail.call(self , {
            text: INNOAGE_ISOFFLINE_TEXT({
              sn,
              SERVICE_TEXT: POWER_SERVICE
            }), 
            sn,
            callBack: ()=>{  
              endService.call(self, devName);
              circle1.setAttributeNS(null, "stroke", null);
          }});
        } else {
          processBarRotate.call(btnObj, PROGRESS_ZERO_RATE, { controlBar, btn, devName});
          fail.call(self , {
            text: INNOAGE_REMOTE_FAIL_TEXT({
              sn,
              SERVICE_TEXT: POWER_SERVICE
            }), 
            sn,
            callBack: ()=>{  
              endService.call(self, devName);
              circle1.setAttributeNS(null, "stroke", null);
          }});
        } 
      },
      fail: ()=>{
        processBarRotate.call(btnObj, PROGRESS_ZERO_RATE, { controlBar, btn, devName});

        fail.call(self , {
          text: INNOAGE_REMOTE_FAIL_TEXT({
            sn,
            SERVICE_TEXT: POWER_SERVICE
          }), 
          sn,
          callBack: ()=>{  
            endService.call(self, devName);
            circle1.setAttributeNS(null, "stroke", null);
        }});
      },
      payload: {
        sn
      }
    });

    return true
  }

  return false;
}

export default Power;