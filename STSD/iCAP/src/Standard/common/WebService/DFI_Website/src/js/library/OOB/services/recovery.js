// import Button from '../../button';
import Service from './service';
import { API } from '../../API';
import { 
  RECOVERY_SERVICE,
  PROCESS_END,
  INNOAGE_REMOTE_SUCCESS,
  API_TIMEOUT,
  PROGRESS_COMPLETE_RATE,
  RECOVERY_TEXT,
  INNOAGE_REMOTE_FAIL,
  INNOAGE_OFFLINE,
  INNOAGE_ISOFFLINE_TEXT,
  INNOAGE_REMOTE_FAIL_TEXT
 } from '../constants/service';
import { RECOVERY_SERVICE_ICON } from '../constants/style';

function Recovery(Mediator){
  this.name = RECOVERY_TEXT;
  this.btnObj = this.generateBtn({ Mediator })

  this.mediator = Mediator;
}

Recovery.prototype = new Service();

Recovery.prototype.getIcon = function(){
  return RECOVERY_SERVICE_ICON;
}

Recovery.prototype.start = async function({sn, devName}){
  ///API
  const self = this;

  const { confirm, endService, btnObj} = self;
  const { element:{controlBar, btn}, 
    processBarRotate, hideIcon, showError
  } = btnObj;
  const result = await confirm({
    text: `${sn} will be recovered.`,
    confirmButtonText: `Yes, recover it.`,
  });

  if (result.value) {
    hideIcon.call(btnObj);

    btn.setAttribute('data-pct', '0%');

    const fail = function(TEXT){
      showError.call(btnObj, TEXT);
        
      endService.call(self, devName)
    }

    API.post({
      url:`/InnoAGE/recovery`,
      global: false,
      fail,
      timeout: API_TIMEOUT,
      payload: {
        sn
      }
    }).then(({data:{status}})=>{
      if (status === INNOAGE_OFFLINE) {
        fail(INNOAGE_ISOFFLINE_TEXT({sn, RECOVERY_TEXT}));
      } else if (status === INNOAGE_REMOTE_FAIL){
        fail(INNOAGE_REMOTE_FAIL_TEXT({sn, RECOVERY_TEXT}));
      }
    }).catch(()=>{
      fail();
    });
    
    return true
  }

  return false;
}

Recovery.prototype.updateBar = function({devName, SN, Percentage}){
  if (!this[devName]) return
  const self = this;
  const { controlBar, btn } = self[devName];

  if (Percentage < PROGRESS_COMPLETE_RATE){
    this.btnObj.hideIcon();
    this.btnObj.element.circle1.setAttributeNS(null, "stroke", '#66666675');
    this.btnObj.processBarRotate(Percentage, {controlBar, btn, devName});
  } else if (Percentage === PROCESS_END){
    this.btnObj.element.circle1.setAttributeNS(null, "stroke", null);
    this.success({controlBar, btn, text: RECOVERY_SERVICE, sn: SN, devName});
  }
}

export default Recovery;
