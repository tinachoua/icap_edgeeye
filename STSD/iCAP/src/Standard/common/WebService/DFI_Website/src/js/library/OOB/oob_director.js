import { Recovery, Reboot, Power} from './services';
import SelectModal from './select_modal';
import { API } from '../API';
import { WEBSOCKET_HOST } from '../../constants/env';
import { DeviceManager as deviceManager} from '../../DeviceManager';
import { REMOTE_BUTTON } from '../OOB/constants/style';
import { 
  SERVICE_ON, 
  SERVICE_OFF, 
  POWER_SERVICE, 
  REBOOT_SERVICE, 
  RECOVERY_SERVICE 
} from '../OOB/constants/service';
import socketDataHandler from './helpers/socketDataHandler';
import { DEBUG } from '../../constants/env'; 

function OOBDirector(){
  this.element  = null,
  this.services = {
    [POWER_SERVICE]: new Power(this),
    [REBOOT_SERVICE]: new Reboot(this),
    [RECOVERY_SERVICE]: new Recovery(this)
  },
  this.selectModal = null,
  this.root = null,
  this.ooblist = [],
  this.serviceAssigned = null;
  this.activeServices = null;
}

OOBDirector.prototype.init = function(devName, activeServices){
  const self = this;
  const {element, services} = self;

  self[devName] = {};
  this.activeServices = activeServices;
  this.ooblist.forEach(({SN})=>{
    self[devName][SN] = {};
  });
  if (!element) {
    const root = self.root = document.querySelector('#OOB-service');
    const ul = document.createElement('ul');

    this.devName = devName;
    ul.className = REMOTE_BUTTON;
    this.setupService({
      activeServices,
      ul,
      devName
    })
    this.ul = ul;
    self.element = root.appendChild(ul);
    this.createWebsocket();
  } else {

    for(let i=0; i < activeServices.length; i++){
      const service = activeServices[i];
      services[service].setUp(devName);
    }

    element.style.display = '';
  }
}

OOBDirector.prototype.asyncCheckStatus = async function({ ooblist }){
  const list = ooblist.map((item)=>item.SN).join(',');
  
  try{
    const result = await API.get({
      url:`/InnoAGE/status?sn=${list}`,
      global: false
    });

    for (let sn in result){
      if (result[sn]) return true;
    }

    return false;

  } catch(e){
    return false;
  }
}

OOBDirector.prototype.createWebsocket = function(){
  const self = this;
  const { services , activeServices} = self;
  const { recovery, reboot, power } = services;

  const cache = {}
  const websocket = new WebSocket(`ws://${WEBSOCKET_HOST}/ws/iCAP?token=${$.cookie('token')}`);

  websocket.onopen = function (evt) {  
    console.log('open');
  };

  websocket.onclose = function (evt) {  
    console.log('close')
  };

  websocket.onmessage = function ({data}) {
    const { data: parsedData, type } = JSON.parse(data);

    const devName = cache[parsedData.sn] || (cache[parsedData.sn] = deviceManager.getDevNameByInnoAGE(parsedData.sn))

    if (DEBUG){
      console.log(type, parsedData);
    }
    
    if (!self[devName])  return;
    socketDataHandler[type]({
      ...parsedData, 
      recovery,
      devName,
      mediator: self
    });
  };
  websocket.onerror = function (evt) {
    console.log('Websocket error', evt)
  };
}

function isServiceRunning(services){
  let isRunning = false;

  Object.keys(services).some((service)=>{
    if(services[service] === SERVICE_ON) {
      isRunning = true;
      return true;
    }
  });

  return isRunning;
}

OOBDirector.prototype.setUp = async function({
  ooblist, 
  devName, 
  activeServices
}){
  this.activeServices = activeServices;
  const self = this;
  const { services} = self;

  self.ooblist = ooblist;
  self.current = devName;

  if(ooblist.length === 0) {
    (self.element) && (self.element.style.display = 'none')
    return;
  }
  self.init(devName, activeServices);
}

// OOBDirector.prototype.enabledService = function(){

// }


OOBDirector.prototype.setupService = function({
  activeServices,
  ul,
  devName
}){
  this.activeServices = activeServices || this.activeServices;
  const root = ul || this.ul
  const services = this.services;
  const fragment = document.createDocumentFragment();
  
  devName = devName || this.devName;
  
  while (root.firstChild) {
    root.removeChild(root.firstChild);
  }

  for(let i=0; i < activeServices.length; i++) {
    const service = activeServices[i];
    if (!this.services[service].li) {
      const li = document.createElement('li');

      li.appendChild(services[service].init({
        dom: root,
        devName
      }));

      fragment.appendChild(li);
      this[devName][service] = SERVICE_OFF;
      this.services[service].li = li;
    } else {
      fragment.appendChild(this.services[service].li);
    }
  }
  this.enabledService(); 
  root.appendChild(fragment);
}

OOBDirector.prototype.clicked = function(serviceSelected){
  const {assignService, ooblist, targetSN} = this;

  assignService.call(this, serviceSelected); //what OOB action?
  if (targetSN) {
    this.startService(targetSN);
    return;
  }

  if(ooblist.length > 1){
    this.openModal({
      titleText: serviceSelected.name
    });
  } else {
    this.startService(ooblist[0].SN);
  }
}

OOBDirector.prototype.disabledServices = function(){
  const {activeServices, services} = this;

  for(let i=0; i < activeServices.length; i++){
    services[activeServices[i]].disabled();
  }
}

OOBDirector.prototype.startService = async function (targetSN){
  const { serviceAssigned, services, current, activeServices } = this;

  if (!serviceAssigned) {
    throw new Error('must call assignService method before start');
  }

  const result = await serviceAssigned.start({sn: targetSN, devName: current});

  if (result) {
    this[current][serviceAssigned.name.toLowerCase()] = SERVICE_ON 
    
    this.disabledServices();

    return true;
  }
}

OOBDirector.prototype.assignService = function (serviceSelected) {
  this.serviceAssigned = serviceSelected;
}

OOBDirector.prototype.openModal = function({titleText}) {
  let {selectModal, ooblist} = this;

  selectModal = selectModal || (this.selectModal = new SelectModal(this))

  selectModal.open({
    ooblist, 
    titleText
  });
}

OOBDirector.prototype.closeModal = function(){
  const {selectModal} = this;

  selectModal.close();
}

OOBDirector.prototype.enabledService = async function(){
  const {activeServices , services, devName, ooblist} = this;

  if (!isServiceRunning(this[devName])){
    for(let i=0; i < activeServices.length; i++){
      services[activeServices[i]].enabled(devName);
    }
    // if (await this.asyncCheckStatus({ ooblist })) {
    //   // this.enabledService();
    //   // activeServices.forEach((service)=>{
    //   //   services[service].enabled(devName);
    //   // });
    //   for(let i=0; i < activeServices.length; i++){
    //     services[activeServices[i]].enabled(devName);
    //   }
    // }
  }
}

OOBDirector.prototype.endService = function(devName, {name}){
  for (let key in this[devName]){
    if(key === name.toLowerCase()) {
      this[devName][name.toLowerCase()] = SERVICE_OFF;
      continue;
    }
    if (this[devName][key] === SERVICE_ON) return;
  }

  this.enabledService();
}

OOBDirector.prototype.addOOBTab = function(){
  const { tab } = this;
  
  if (!tab){
    const fragment = document.createDocumentFragment();
    const container = this.tabContainer = document.querySelector('#tab-secondary-continer');
    const span = document.createElement('span');
    span.className = 'divider hidden-xs';

    var oob = $Tab('OOB').getTab();
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

OOBDirector.prototype.setTargetSN = function(sn){
  this.targetSN = sn;
}

export default OOBDirector;