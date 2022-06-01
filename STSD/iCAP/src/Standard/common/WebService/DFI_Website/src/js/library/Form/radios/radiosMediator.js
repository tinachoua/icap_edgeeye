function RadiosMediator(){
  this.radios = [];
  this.current = null;
}

RadiosMediator.prototype.addRadio = function(radio){
  this.radios.push(radio);
}
RadiosMediator.prototype.update = function(dom, data){
  const self = this;

  for(let i=0, radio; radio= self.radios[i++];) {  
    if(radio.update(dom, data)) {
      this.current = radio;
      break
    }
  }
}
RadiosMediator.prototype.validataFunc = function(dom){
  const self = this;

  const errorMsg = self.current.validataFunc && self.current.validataFunc(dom)();

  return errorMsg;
}
RadiosMediator.prototype.getPayloadFunc = function(dom){
  const self = this;

  return self.current.getPayloadFunc(dom);
} 
RadiosMediator.prototype.bindChange = function(dom){
  const self = this;

  for(let i=0, radio; radio= self.radios[i++];) {  
    radio.bindChange(dom);
  }
}
RadiosMediator.prototype.changed = function(dom, changedRadio){
  const self = this;
  self.current = changedRadio;

  for(let i=0, radio; radio= self.radios[i++];) {  
    if (radio !== changedRadio && typeof radio.unchecked === 'function') radio.unchecked(dom); 
  }

  if (typeof changedRadio.handleChange === 'function') {
    changedRadio.handleChange(dom);
  }
}

export default RadiosMediator;