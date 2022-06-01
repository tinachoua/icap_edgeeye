// import Button from '../../button';
import Service from './service';
import { API } from '../../API';
import { 
  REBOOT_SERVICE,
  INNOAGE_REMOTE_SUCCESS,
  SERVICE_ON,
  FAKE_PROGRESS_RATE,
  PROGRESS_ZERO_RATE,
  REBOOT_TEXT,
  INNOAGE_OFFLINE,
  INNOAGE_ISOFFLINE_TEXT,
  INNOAGE_REMOTE_FAIL_TEXT
} from '../constants/service';
import { REBOOT_SERVICE_ICON } from '../constants/style';

function Reboot(Mediator){
  this.name = REBOOT_TEXT;
  this.btnObj = this.generateBtn({Mediator});
  this.mediator = Mediator;
}

Reboot.prototype = new Service();

Reboot.prototype.getIcon = function(){
  return REBOOT_SERVICE_ICON;
}

Reboot.prototype.start = async function({sn, devName}){
  const self = this;
  const { confirm, endService, btnObj, success, fail, mediator} = self;
  const {element: { controlBar, btn, circle1}, 
    processBarRotate, hideIcon,
  } = btnObj;

  const result = await confirm({
    text: `${sn} will be reboot.`,
    confirmButtonText: `Yes, reboot it.`,
  });

  if (result.value) {
    mediator[devName][REBOOT_SERVICE] = SERVICE_ON;
    hideIcon.call(btnObj);
    circle1.setAttributeNS(null, "stroke", '#66666675');
    processBarRotate.call(btnObj, FAKE_PROGRESS_RATE, { 
      controlBar, 
      btn,
      devName
    });
    API.post({
      url:`/InnoAGE/reboot`,
      global: false,
      success: function({data:{ status }}){
        if (status === INNOAGE_REMOTE_SUCCESS ) {
          success.call(self, {
            controlBar, btn,
            text: REBOOT_SERVICE, 
            sn, 
            devName,
          });
        } else if (status === INNOAGE_OFFLINE){
          processBarRotate.call(btnObj, PROGRESS_ZERO_RATE, { controlBar, btn, devName});
          fail.call(self , {
            text: INNOAGE_ISOFFLINE_TEXT({
              sn,
              SERVICE_TEXT: REBOOT_SERVICE
            }), 
            sn,
            callBack: ()=>{  
              endService.call(self, devName);
              circle1.setAttributeNS(null, "stroke", null);
          }});
        } 
        else {
          processBarRotate.call(btnObj, PROGRESS_ZERO_RATE, { controlBar, btn, devName});
          fail.call(self , {
            text: INNOAGE_REMOTE_FAIL_TEXT({
              sn,
              SERVICE_TEXT: REBOOT_SERVICE
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
            SERVICE_TEXT: REBOOT_SERVICE
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

export default Reboot;
