import { API } from '../library/API';
import { MOCK_OOB } from '../constants/globalVariable';

function Device(devName) {
  this.name = devName;
  this.ooblist = [];
  this.oobStatusList = {};
}
Device.prototype.setOOBStatus = function ({ sn, status }) {
  this.oobStatusList[sn] = status;
}
Device.prototype.getOOBStatus = function (sn) {
  return this.oobStatusList[sn];
}
Device.prototype.hasOOB = function () {
  return this.ooblist.length > 0;
}

Device.prototype.asyncGetStatus = async function () {
  const { ooblist } = this;
}

Device.prototype.update = function ({ OOBlist }) {
  this.ooblist = OOBlist;
}

Device.prototype.getOOBlist = function () {
  return this.ooblist;
}

Device.prototype.getName = function () {
  return this.name;
}

if (MOCK_OOB) {
  Device.prototype.getOOBInfo = async function (SN) {

    const { Payload: { Data } } = {
      "Status": "OK",
      "HTTPCode": 200,
      "Payload": {
        "Methodname": "/devices/SSDInfo/B0011905300270085",
        "Data": {
          "Later Bad": { "value": 0, "id": "05", "raw value": "1200646400000000000000" },
          "Power On Hours": { "value": 476, "id": "09", "raw value": "1200DC64DC010000000000" },
          "Power Cycle Count": { "value": 30, "id": "0C", "raw value": "12001E001E000000000000" },
          "Total Bad Block Count": { "value": 9, "id": "A3", "raw value": "1200090009000000000000" },
          "Max Erase Count": { "value": 3, "id": "A5", "raw value": "1200030003000000000000" },
          "Avg Erase Count": { "value": 1, "id": "A7", "raw value": "1200010001000000000000" },
          "Device Life": { "value": 100, "id": "A9", "raw value": "0000646464000000000000" },
          "Spare Block Count": { "value": 350, "id": "AA", "raw value": "130064645E010000000000" },
          "Program Fail Count": { "value": 0, "id": "AB", "raw value": "1200006400000000000000" },
          "Erase Fail Count": { "value": 0, "id": "AC", "raw value": "1200006400000000000000" },
          "Abnormal power cycle count": { "value": 28, "id": "C0", "raw value": "12001C001C000000000000" },
          "Temperature": { "value": 38, "id": "C2", "raw value": "0200266426001700290326" },
          "Flash ID": { "value": 0, "id": "E5", "raw value": "00006464983C98B3767200" },
          "Later Bad Block Read": { "value": 0, "id": "EB", "raw value": "0200000000000000000000" },
          "Later Bad Block Write": { "value": 0, "id": "EB", "raw value": "0200000000000000000000" },
          "Later Bad Block Erase": { "value": 0, "id": "EB", "raw value": "0200000000000000000000" },
          "Total LBAs Written": { "value": 2, "id": "F1", "raw value": "1200646402000000000000" },
          "Total LBAs Read": { "value": 300, "id": "F2", "raw value": "120064642C010000000000" },
          "lib_ver": "V5.3.20", "build_date": "2020/05/11",
          "Timestamp": 1590729681698
        }
      }
    }

    const { Timestamp, Temperature, ['Device Life']: Health, build_date, lib_ver, ...data, } = Data

    for (let key in data) {
      data[key] = data[key].value;
    }

    data.Temperature = Temperature.value + '°C';
    data.Health = Health.value + '%';

    return {
      data,
      timestamp: Timestamp
    }
  }
} else {
  Device.prototype.getOOBInfo = async function (SN) {
    let result;
    try {
      result = await API.get({
        url: `/innoAGE/ssd-info/${SN}`,
      })
    } catch (e) {
      return null;
    }

    const {
      Payload: {
        Data: {
          Timestamp, Temperature, ['Device Life']: Health, build_date, lib_ver, ...data
        }
      } } = result;

    for (let key in data) {
      data[key] = data[key].value;
    }

    data.Temperature = Temperature.value + '°C';
    data.Health = Health.value + '%';

    return {
      data,
      timestamp: Timestamp
    }
  }

  Device.prototype.getSphereStatus = async function (SN) {
    let result;
    let data = {}
    try {
      result = await API.get({
        url: `/innoAGE/sphere-status/${SN}`,
      })
    } catch (e) {
      return null;
    }

    const {
      Payload: {
        Data: {
          Timestamp, AppVersion, NetworkingType
        }
      } } = result;

    data['App Version'] = AppVersion;
    data['Connection Type'] = NetworkingType;

    return {
      data,
      Timestamp
    }
  },

    Device.prototype.getOSHeartbeat = async function (SN) {
      let result;

      try {
        result = await API.get({
          url: `/innoAGE/os-heartbeat/${SN}`,
        })
      } catch (e) {
        return null;
      }

      const {
        Payload: {
          Data: {
            osHeartbeat
          }
        } } = result;


      return osHeartbeat

    }

}

export default Device;