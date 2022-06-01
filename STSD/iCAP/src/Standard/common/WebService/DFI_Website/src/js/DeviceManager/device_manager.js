import Device from './device';

const DeviceManager = {
  devices:{},
  current:{},
  getDevice: function(devName){
    return (this.devices[devName]) || (this.devices[devName] = new Device(devName)); 
  },
  getDevNameByInnoAGE: function(sn){
    const { devices } = this;

    for(let devName in devices){
      const ooblist =  devices[devName].getOOBlist();
      for (let i=0; i < ooblist.length; i++){
        const {SN} = ooblist[i];
        if (SN === sn) return devName;
      }
    }
  },
  getOOBInfo: async function({devName, sn}){
      let result;
    try
    {
       result = await this.devices[devName].getOOBInfo(sn);
    }catch(e){
      console.log(e);
    }
    return result;
  },
  getOOBlist: function(devName){
    return this.devices[devName].getOOBlist();
  }
}

export default DeviceManager;