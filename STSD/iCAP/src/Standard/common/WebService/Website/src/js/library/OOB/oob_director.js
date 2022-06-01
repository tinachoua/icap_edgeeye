import { Recovery, Reboot, Power, Backup } from './services';
import SelectModal from './select_modal';
import { API } from '../API';
import { WEBSOCKET_URL } from '../../constants/env';
import { DeviceManager as deviceManager } from '../../DeviceManager';
import { MOCK_OOB } from "../../constants/globalVariable";
import updateStatus from './helpers/updateStatus';

function OOBDirector() {
  this.element = null,
    this.services = {
      power: new Power(this),
      recovery: new Recovery(this),
      reboot: new Reboot(this),
      backup: new Backup(this),
    },
    this.selectModal = null,
    this.root = null,
    this.ooblist = [],
    this.serviceAssigned = null;
  this.createWebsocket();
}

OOBDirector.prototype.init = function (devName) {
  const { element, services } = this;

  if (!element) {
    const root = this.root = document.querySelector('#OOB-service');
    const ul = document.createElement('ul');
    const fragment = document.createDocumentFragment();

    ul.className = 'remote_btn';

    for (let service in services) {
      const li = document.createElement('li');

      li.appendChild(services[service].init({
        dom: ul,
        devName
      }));
      fragment.appendChild(li);
    }

    ul.appendChild(fragment)

    this[devName] = {
      recovery: 0,
      reboot: 0,
      power: 0,
      loading: 0,
    };


    this.element = root.appendChild(ul);
  } else {
    for (let service in services) {
      services[service].setUp(devName);
    }
    element.style.display = '';
  }
}

OOBDirector.prototype.createWebsocket = function () {
  const self = this;
  const { services } = self;
  const { recovery, backup } = services;


  const cache = {}
  const websocket = new WebSocket(WEBSOCKET_URL);

  websocket.onopen = function (evt) {
    console.log('open');
  };

  websocket.onclose = function (evt) {
    console.log('close')
  };

  websocket.onmessage = function ({ data }) {

    const { dataType, payload } = JSON.parse(data);

    if (dataType === 'deviceProgress') {

      const { serialNumber, percentage, methodName } = payload;

      const devName = cache[serialNumber] || (cache[serialNumber] = deviceManager.getDevNameByInnoAGE(serialNumber))

      if (!self[devName]) return;

      self[devName].loading = 1;

      for (let service in services) {
        services[service].disabled(devName);
      }

      if (methodName === 'recovery') {
        recovery.updateBar({ devName, SN: serialNumber, Percentage: percentage })
      }

      if (methodName === 'backup') {
        backup.updateBar({ devName, SN: serialNumber, Percentage: percentage })
      }
    } else if (dataType === 'status') {

      const sn = Object.keys(payload)[0];
      const devName = deviceManager.getDevNameByInnoAGE(sn)
      if (!self[devName]) return;

      if (+payload[sn] === 1 && +self[devName].loading === 0) {
        for (let service in services) {
          services[service].enabled(devName);
        }
      } else {
        for (let service in services) {
          //services[service].disabled(devName);
        }
      }

      if (+payload[sn] === 0) {
        updateStatus({
          element: document.querySelector('#os-heartbeat-led'),
          status: 0,
        })
      }

      updateStatus({
        element: document.querySelector('#oob-led'),
        status: payload[sn],
      })
    } else if (dataType === 'deviceOSHeartbeat') {

      const sn = payload.serialNumber;
      const devName = deviceManager.getDevNameByInnoAGE(sn)
      if (!self[devName]) return;

      updateStatus({
        element: document.querySelector('#os-heartbeat-led'),
        status: payload.osHeartbeat,
      })
    }

  };

  websocket.onerror = function (evt) { };
}

OOBDirector.prototype.setUp = async function ({ ooblist, devName, addTab = false }) {
  const self = this;
  const { services } = self;

  self.ooblist = ooblist;
  self.current = devName;

  if (ooblist.length === 0) {
    (self.element) && (self.element.style.display = 'none')
    return;
  }

  self.init(devName)

  if (!(self[devName].loading === 1)) {/// device is recovering or rebooting
    if (await self.asyncCheckStatus({ devName, ooblist })) {

      for (let service in services) {
        services[service].enabled(devName);
      }
    }
  }
}

OOBDirector.prototype.clicked = function (serviceSelected) {
  const { assignService, ooblist, targetSN } = this;

  assignService.call(this, serviceSelected);

  if (targetSN) {
    this.startService(targetSN);
    return;
  }

  if (ooblist.length > 1) {
    this.openModal({
      titleText: serviceSelected.name
    });
  } else {
    this.startService(ooblist[0].SN);
  }
}

OOBDirector.prototype.startService = async function (targetSN) {
  const { serviceAssigned, services, current } = this;

  if (!serviceAssigned) {
    throw new Error('must call assignService method before start');
  }

  const result = await serviceAssigned.start({ sn: targetSN, devName: current });

  if (result) {
    this[current][serviceAssigned.name.toLowerCase()] = 1
    for (let service in services) {
      services[service].disabled();
    }
    return true;
  }
}

OOBDirector.prototype.assignService = function (serviceSelected) {
  this.serviceAssigned = serviceSelected;
}

OOBDirector.prototype.openModal = function ({ titleText }) {
  let { selectModal, ooblist } = this;

  selectModal = selectModal || (this.selectModal = new SelectModal(this))

  selectModal.open({
    ooblist,
    titleText
  });
}

OOBDirector.prototype.closeModal = function () {
  const { selectModal } = this;

  selectModal.close();
}

OOBDirector.prototype.endService = function (devName, { name }) {
  const { services } = this;

  this[devName].loading = 0
  // for (let key in this[devName]) {
  //   if (key === name.toLowerCase()) {
  //     this[devName][name.toLowerCase()] = 0;
  //     continue;
  //   }
  //   if (this[devName][key] === 1) return;
  // }

  for (let service in services) {
    services[service].enabled(devName);
  }
}

OOBDirector.prototype.addOOBTab = function () {
  const { tab } = this;

  if (!tab) {
    const fragment = document.createDocumentFragment();
    const container = this.tabContainer = document.querySelector('#tab-secondary-continer');
    const span = document.createElement('span');
    span.className = 'divider hidden-xs';

    var oob = $Tab({
      name: 'OOB'
    }).getTab();
    oob.setAttribute("class", "sub-tab-content");
    oob.setAttribute("data-toggle", "tooltip");
    oob.setAttribute("data-placement", "bottom");
    oob.setAttribute("title", 'Out of Band');

    fragment.appendChild(span);
    fragment.appendChild(oob);
    container.appendChild(fragment);
  } else {

  }
}

OOBDirector.prototype.setTargetSN = function (sn) {
  this.targetSN = sn;
}

if (MOCK_OOB) {
  OOBDirector.prototype.fakeRecovery = function ({ SN, devName }) {
    const self = this;
    const { services } = self;
    const { recovery } = services;

    self[devName].recovery = 1;
    let Percentage = 5;
    const timer = setInterval(() => {
      for (let service in services) {
        services[service].disabled(devName);
      }

      if (Percentage === 100) {
        recovery.updateBar({ devName, SN, Percentage: 'end' })
        clearInterval(timer);
        return
      }
      recovery.updateBar({ devName, SN, Percentage })
      Percentage += 5;
    }, 1000);
  }
  OOBDirector.prototype.asyncCheckStatus = function () {
    return true;
  }
} else {
  OOBDirector.prototype.asyncCheckStatus = async function ({ devName, ooblist }) {
    const list = ooblist.map((item) => item.SN).join(',');
    try {
      const result = await API.get({
        url: `/innoAGE/status?sn=${list}`,
      });

      let isTrue = false;

      for (let sn in result) {
        deviceManager.setOOBStatus({ devName, sn, status: result[sn] })
        if (+result[sn] > 0) isTrue = true
      }

      return isTrue;

    } catch (e) {
      return false;
    }
  }
}

export default OOBDirector;