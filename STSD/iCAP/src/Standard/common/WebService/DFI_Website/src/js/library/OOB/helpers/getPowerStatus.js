import { DEVICE_POWER_OFF, DEVICE_POWER_ON } from '../constants/text';

export default function(){
  const powerStatus = document.getElementById('power-status');

  ///CHECK ATTR
  if (`${powerStatus.status}` === '0') {
    return DEVICE_POWER_OFF
  } else if (`${powerStatus.status}` === '1'){
    return DEVICE_POWER_ON
  } 

  ///CHECK CSS
  if (powerStatus.className.includes('grey')){
    return DEVICE_POWER_OFF
  } else if (powerStatus.className.includes('green')){
    return DEVICE_POWER_ON
  }
}