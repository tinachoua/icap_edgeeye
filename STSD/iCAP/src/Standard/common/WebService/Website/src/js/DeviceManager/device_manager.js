import Device from './device';

const DeviceManager = {
  devices: {},
  current: {},
  getDevice: function (devName) {
    return (this.devices[devName]) || (this.devices[devName] = new Device(devName));
  },
  getDevNameByInnoAGE: function (sn) {
    const { devices } = this;

    for (let devName in devices) {
      const ooblist = devices[devName].getOOBlist();
      for (let i = 0; i < ooblist.length; i++) {
        const { SN } = ooblist[i];
        if (SN === sn) return devName;
      }
    }
  },
  setOOBStatus: function ({ devName, sn, status }) {
    return this.devices[devName].setOOBStatus({ sn, status });
  },
  getOOBStatus: function ({ devName, sn }) {
    return this.devices[devName].getOOBStatus(sn);
  },
  getOOBInfo: function ({ devName, sn }) {
    return this.devices[devName].getOOBInfo(sn);
  },
  getSphereStatus: function ({ devName, sn }) {
    return this.devices[devName].getSphereStatus(sn);
  },
  getOSHeartbeat: async function ({ devName, sn }) {
    return this.devices[devName].getOSHeartbeat(sn);
  },
  // getOOBInfo: function(devName){
  //   return this.devices[devName].getOOBInfo();
  // },
  getOOBlist: function (devName) {
    return this.devices[devName].getOOBlist();
  }
}

export default DeviceManager;