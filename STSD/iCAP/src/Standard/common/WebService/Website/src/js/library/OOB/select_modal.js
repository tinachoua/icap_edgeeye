import { DeviceManager } from '../../DeviceManager';

function createElement(){
  const ret = [];
  const elelist = Array.prototype.shift.call(arguments);
  for (let i = 0, ele; ele = elelist[i++];) {
    ret.push(document.createElement(ele));
  }
  return ret;
}

function modalMaker({fragTitle, fragBody, fragFooter}){
  function modalHeader(fragTitle){
    const div = document.createElement('div');
    
    div.className = 'modal-header';
    div.innerHTML = `         
      <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
    `;
    div.appendChild(fragTitle);
    return div;
  }
  function modalBody(fragBody){
    const [div1, div2] = createElement(['div', 'div']);

    
    
    
    
    const div = document.createElement('div');
    
    div.className = 'modal-body';
    //div.appendChild(fragBody);

    div.innerHTML = `
      <div class='container-fluid'>
        <div class="row">
          <div class="col-md-6">
              <label >Storage *</label>
              <select><option>222222222222222222</option></select>
          </div>
        </div>  
        <br>
        <div class="row">
          <div class="col-md-6">
              <div>Model: <span></span></div>
          </div>
        </div>
        <div class="row">
          <div class="col-md-6">
              <div>Total Capacity: <span></span></div>
          </div>
        </div>
        <br>
        <div class="row">
          <div class="col-md-6">
              <div>Partition: </div><select><option>C</option></select>
          </div>
        </div>  
        <div class="row">
          <div class="col-md-6">
              <div>Capacity: <span></span> GB</div>
          </div>
      </div>
      </div>
    `
    
    return div;
  }
  function modalFooter(fragFooter){
    const div = document.createElement('div');
    
    div.className = 'modal-footer';
    div.innerHTML = `
      <button type="button" class="btn btn-mini btn-default" data-dismiss="modal">Close</button>
    `
    div.appendChild(fragFooter);
    return div;
  }

  const div = document.createElement('div');
  const content = document.createElement('div');

  div.style.maxWidth = '440px';
  div.className = 'modal-dialog modal-dialog-centered ';
  div.setAttribute('role', "document");

  content.className = 'modal-content';
  content.appendChild(modalHeader(fragTitle));
  content.appendChild(modalBody(fragBody));
  content.appendChild(modalFooter(fragFooter));
  
  div.appendChild(content);

  return div;
}


async function asyncGetStoragelist(devName){
  return await DeviceManager.getDevice(devName).getStoragelist()
}

function storageTemplate(){

}

function rowFactory(){
  const [row, div] = createElement(['div', 'div']);

  row.className = 'row'
  div.className = 'col-md-12';

  row.appendChild(div);
  
  const model = {
    storage: function(elelist){
      for(let i=0, ele; ele=elelist[i++];) {
        div.appendChild(ele);
      }

      row.appendChild(div);
      return row;
    },
    model: function(span){
      const model = document.createElement('div');
      
      model.textContent = 'Model: '
      model.appendChild(span);
      div.appendChild(model);
      
      return row;
    },  
    capacity: function(text, span){
      const cap = document.createElement('div');

      cap.textContent = text;
      cap.appendChild(span);

      div.appendChild(cap);
      return row;
    },
    partition: function(select){
      const part = document.createElement('div');

      part.style.marginTop = '8px';
      part.textContent = 'Partition: ';
      part.appendChild(select);
      div.appendChild(part);

      return row;
    }
  }

  return model[Array.prototype.shift.call(arguments)](...arguments)
}

function SelectModal(director){
  const self = this;
  const {root, startService} = director;
  const [
    wrapper, modal, content, head, title, body, container, footer,
    model, totalCapacity, capacity,
    snSelect, parSelect,
    button, label,
  ] 
    = createElement([
      'div', 'div', 'div','div', 'div', 'div', 'div', 'div',
      'span', 'span', 'span',
      'select', 'select',
      'button', 'label',
  ]);

  wrapper.className = 'modal fade';
  wrapper.tabIndex = '-1';
  wrapper.setAttribute('role', 'dialog');
    
  modal.style.maxWidth = '440px';
  modal.className = 'modal-dialog modal-dialog-centered';
  modal.setAttribute('role', "document");

  head.className = 'modal-header';
  head.innerHTML = `         
    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
  `;

  title.className = 'modal-title';
  this.title = head.appendChild(title);

  body.className = 'modal-body';
  
  container.className = 'container-fluid'
  
  label.textContent = 'InnoAGE *';

  snSelect.onchange = function(){
    const {updateContent, cache:{ooblist}} = self;

    for(let i=0, storage; storage=ooblist[i++];){
      const {SN} = storage;
      if (SN === this.value) {
        self.current = storage;
        updateContent.call(self);
        return;
      }
    }
  }

  parSelect.onchange = function(){
    self.updatePartion(this.value);
  }

  container.appendChild(rowFactory('storage', [label, document.createElement('br'), snSelect]));
  container.appendChild(document.createElement('br'));  
  container.appendChild(rowFactory('model', model));
  container.appendChild(rowFactory('capacity', 'Total Capacity: ', totalCapacity));
  container.appendChild(rowFactory('partition', parSelect));
  container.appendChild(rowFactory('capacity', 'Capacity: ', capacity));
  
  body.appendChild(container);

  footer.className = 'modal-footer'
  footer.innerHTML = `
    <button type="button" class="btn btn-mini btn-default" data-dismiss="modal">Close</button>
  `
  button.className = 'btn btn-dark';
  button.style.height = '29px';
  button.textContent = 'Start';
  button.onclick = async function(){
    const result = await startService.call(director, snSelect.value);

    if(result) {
      $(wrapper).modal('toggle');
    }
  }
  footer.appendChild(button);

  content.className = 'modal-content';
  content.appendChild(head);
  content.appendChild(body);
  content.appendChild(footer);

  modal.appendChild(content);
  wrapper.appendChild(modal);

  
  this.director = director;
  this.modal = root.appendChild(wrapper);
  this.content = {
    selectlist: [snSelect, parSelect],
    button,
    title,
    model,
    totalCapacity,
    capacity
  }
  this.cache = {};
}

SelectModal.prototype.init = function(){

};

SelectModal.prototype.updatePartion = function(mountAt) {
  const {current:{ParInfo}, content:{capacity}} = this;

  for (let i=0, part; part =ParInfo[i++];){
    const {MountAt, Capacity} = part;
    if (MountAt === mountAt) {
      capacity.textContent = Capacity + 'GB';
      return;
    }
  }
  
}

SelectModal.prototype.clear = function(){
  const {selectlist} = this.content;

  for(let i=0, select; select=selectlist[i++];){
    select.options.length = 0;
  }
}

SelectModal.prototype.update = function({titleText, ooblist}){
  const {cache, content:{title, selectlist}} = this;
  const [storSelect] = selectlist;

  if (cache.titleText !== titleText) {
    title.textContent = titleText;
    cache.titleText = titleText;
  }  

  if (cache.ooblist !== ooblist){
    this.clear();
    let fragment = document.createDocumentFragment();
    
    for (let i=0, storage; storage = ooblist[i++];){
      const opt = document.createElement('option');
      opt.textContent = storage.SN;
      fragment.appendChild(opt);
    }
    storSelect.appendChild(fragment);
    this.current = ooblist[0];    
    this.updateContent(ooblist[0]);

    cache.ooblist = ooblist
  }
}

SelectModal.prototype.updateContent = function(){
  const {
    content:{selectlist, totalCapacity, model, capacity}, 
    current:{Cap, Model, ParInfo}} 
  = this;
  const fragment = document.createDocumentFragment();

  totalCapacity.textContent = Cap + ' GB';  
  model.textContent = Model;

  for (let i=0, part; part =ParInfo[i++];){
    const opt = document.createElement('option');
    const {MountAt} = part;

    opt.textContent = MountAt;
    fragment.appendChild(opt);
  }

  ParInfo[0] && (capacity.textContent = ParInfo[0].Capacity + 'GB');
  selectlist[1].options.length = 0;
  selectlist[1].appendChild(fragment);
}

SelectModal.prototype.open = function(config){
  this.update(config);
  const { modal } = this;

  $(modal).modal('show');
}

SelectModal.prototype.close = function(){
  const { modal } = this;

  $(modal).modal('toggle');
}

export default SelectModal;