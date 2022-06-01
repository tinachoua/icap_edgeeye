import  { getSingle } from '../../common';
import FormAlert from './form_alert';

const CreateSingleFormAlert = getSingle(function(){
  const _alert = new FormAlert();    
  const parent = Array.prototype.shift.call(arguments);

  _alert.init((typeof parent === 'string')? document.querySelector(parent): parent);
  return _alert;
});

export default CreateSingleFormAlert;