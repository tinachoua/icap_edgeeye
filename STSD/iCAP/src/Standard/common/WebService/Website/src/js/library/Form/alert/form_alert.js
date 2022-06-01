function FormAlert(){}
FormAlert.prototype.init = function(root){
  const self = this;
  const div = document.createElement('div');
  
  div.style.display = 'none';
  self.element = root && root.appendChild(div);
}
FormAlert.prototype.showSuccessfulMsg = function(text){
  const self = this;

  const element = self.element;

  element.textContent = text;
  element.style.display = '';
  element.className = 'alert alert-success'
};
FormAlert.prototype.showFailedMsg = function(text){
  const self = this;

  const element = self.element;

  element.textContent = text;
  element.style.display = '';
  element.className = 'alert alert-danger'
}
FormAlert.prototype.hide = function(){
  this.element.style.display = 'none';
}


export default FormAlert;