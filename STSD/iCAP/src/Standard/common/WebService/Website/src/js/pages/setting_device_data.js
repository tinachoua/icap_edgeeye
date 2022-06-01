import { CreateSingleFormAlert, RadiosMediator, Radio, ValidataFunc} from '../library/Form';
import { API } from '../library/API';

$(document).on("reload-setting-device-data", function () { 

  const radiosMediator = new RadiosMediator();

  let daysRadio = new Radio('save-days');
  daysRadio = {
    ...daysRadio,
    fieldId: 'days-field',
    validataFunc: function(dom){
      const {fieldId}  = this;
      return ValidataFunc.bind(this, [{
        dom: dom[fieldId],
        rules: [{
          strategy: 'isNonEmpty',
          errorMsg: 'Please fill in the expiry date.'
        },{
          strategy: 'minValue:30',
          errorMsg: 'The expiry date must be at least 30.'
        }]
      }])
    },
    getPayloadFunc: function(form){
      const {fieldId} = this;
      return {
        ExpiryDate: form[fieldId].value
      }
    },
    update: function(dom, {ExpiryDate}){
      if(ExpiryDate < 30) return false;
      
      const {radioId, fieldId} = this;

      dom[radioId].checked = true;
      dom[fieldId].value = ExpiryDate;
      dom[fieldId].disabled = false;
      return true;
    },
    handleChange: function(dom, e){
      const {fieldId} = this;
      dom[fieldId].disabled = false;
    },
    bindChange: function(dom){
      const {radioId} = this;
      
      dom[radioId].onchange = function(){
        radiosMediator.changed(dom, daysRadio);
      }
    },
    unchecked: function(dom){
      const {fieldId} = this;
      dom[fieldId].disabled = true;
    }
  }

  let keepRadio = new Radio('always-keep');
  keepRadio = {
    ...keepRadio,
    update: function(dom,{ ExpiryDate }){
      if(ExpiryDate > 0) return;
      
      const { radioId } = this;
      dom[radioId].checked = true;

      return true;
    },
    bindChange:function(dom){
      const {radioId} = this;
      
      dom[radioId].onchange = function(){
        radiosMediator.changed(dom, keepRadio);
      }
    },
    getPayloadFunc: ()=>({ExpiryDate: -1})
  }

  radiosMediator.addRadio(daysRadio)
  radiosMediator.addRadio(keepRadio)

  function Form(){
    this.dom = document.getElementById('dbData-form');
    this.radiosMediator = radiosMediator;
    this.altertObj = null;
  }

  Form.prototype.init = function(){
    let {radiosMediator, dom, altertObj} = this;

    radiosMediator.bindChange(dom);

    API.get({
      url: 'SettingAPI/RawData/Expiry-Date',
      success: function(response){
        radiosMediator.update(dom, response);
      },
      fail: function(defaultHandlerErrorFunc){
        if(typeof defaultHandlerErrorFunc === 'function') {
          defaultHandlerErrorFunc();
          return;
        }

        altertObj = altertObj || CreateSingleFormAlert('.setting-footer');

        altertObj.showFailedMsg("Get data failed");
      }
    });

    dom.onsubmit = function(e){
      e.preventDefault();
      altertObj &&　altertObj.hide();

      const errorMsg = radiosMediator.validataFunc(dom);

      if (errorMsg) {
        altertObj = altertObj || CreateSingleFormAlert('.setting-footer');
  
        altertObj.showFailedMsg(errorMsg);
        return
      }
  
      const payload = radiosMediator.getPayloadFunc(dom); 
      
      API.patch({
        url: 'SettingAPI/RawData/Expiry-Date',
        success: function(response){
          altertObj = altertObj || CreateSingleFormAlert('.setting-footer');
          altertObj.showSuccessfulMsg('Setup successfully');
        },
        fail: function(defaultHandlerErrorFunc){
          altertObj = altertObj || CreateSingleFormAlert('.setting-footer');

          if(typeof defaultHandlerErrorFunc === 'function') {
            defaultHandlerErrorFunc({dom: altertObj, target:'database data setting'});
            return;
          }

          altertObj.showFailedMsg("Database data setting setup failed.");
        },
        payload
      });

    }
  }
  
  const form = new Form();
  form.init();
});
