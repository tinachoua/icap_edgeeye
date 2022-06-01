import { API } from '../library/API';
import { data } from '../library/data_define';
import { ISMART_ATTR } from '../library/OOB/constants/text';

function Device(devName){
  this.name = devName;
  this.ooblist = [];
}

Device.prototype.hasOOB = function(){
  return this.ooblist.length > 0;
}

Device.prototype.asyncGetStatus = async function(){
  const { ooblist } = this;
}

Device.prototype.update = function({OOBlist}){
  this.ooblist = OOBlist;
}

Device.prototype.getOOBlist = function(){
  return this.ooblist;
}

Device.prototype.getName = function(){
  return this.name;
}

Device.prototype.getOOBInfo = async function(SN){
  try {
    const {data} = await API.get({
      url:`/InnoAGE/SSDInfo/${SN}`,
      timeout: 10000
    });

    if (data) {
      const { 
        buildDate,
        libVer,
        timestamp,
        ...ismart
      } = data
      return ismart
    } else {
      return null;
    }
  } catch(e) {
    console.log(e);
    throw e;
  }




  // const {
  //   Payload:{
  //   Data: {
  //     Timestamp, Temperature, ['Device Life']: Health, build_date, lib_ver, ...data
  //   }
  // }} = result;

  // for (let key in data){
  //   data[key] = data[key].value;
  // }

  // data.Temperature = Temperature.value + '°C';
  // data.Health = Health.value + '%';

}

export default Device;