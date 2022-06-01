import Swal from 'sweetalert2'
import Button from '../../button';

function Service() { }

Service.prototype.disabled = function (devName) {
  const { btnObj } = this;

  if (this[devName]) {
    btnObj.disabled.call({
      element: this[devName]
    });
  } else {
    btnObj.disabled();
  }
}

Service.prototype.enabled = function (devName) {
  const { btnObj } = this;

  btnObj.enabled(this[devName]);
}

Service.prototype.endService = function (devName) {
  const { mediator, btnObj } = this;

  if (btnObj[devName].timer) clearInterval(btnObj[devName].timer)

  mediator.endService(devName, this);
}

Service.prototype.confirm = function ({
  title,
  text,
  icon,
  showCancelButton,
  confirmButtonColor,
  cancelButtonColor,
  confirmButtonText,
  cancelButtonText
}) {
  return Swal.fire({
    title: title || 'Are you sure?',
    text: text || "You won't be able to revert this!",
    icon: icon || 'warning',
    showCancelButton: showCancelButton || true,
    confirmButtonColor: confirmButtonColor || '#3085D6',
    cancelButtonColor: cancelButtonColor || '#5E676C',
    confirmButtonText: confirmButtonText || `Yes`,
    cancelButtonText: cancelButtonText || 'No, cancel!',
  })
}




Service.prototype.generateBtn = function ({ Mediator }) {
  return new Button({
    onClick: () => {
      Mediator.clicked(this);
    }
  });
}

Service.prototype.setUp = function (devName) {
  const self = this;
  const { btnObj, makeIcon, mediator } = self;
  const root = self.btnObj.element.btn.parentNode;

  btnObj.element.btn.classList.add('move-away');

  if (self[devName]) {
    btnObj.element = self[devName];
    btnObj.element.btn.classList.remove('move-away');

    if (self[devName].btn.getAttribute('data-pct')) {
      btnObj.hideIcon();
    }
  } else {
    self[devName] = btnObj.make({ icon: makeIcon() });
    btnObj[devName] = {};
    mediator[devName] = {
      recovery: 0,
      reboot: 0,
      power: 0,
      backup: 0,
    };
  }
  root.insertAdjacentElement('afterbegin', self[devName].btn);
}

Service.prototype.init = function ({ devName }) {
  const self = this;
  const p = document.createElement('p');
  const fragment = document.createDocumentFragment();

  self[devName] = self.btnObj.make({
    icon: self.makeIcon(),
    webkitTransitionEnd: self.end
  });
  self.btnObj[devName] = {};
  p.innerHTML = `<b>${self.name}</b>`;

  fragment.appendChild(self[devName].btn);
  fragment.appendChild(p);
  return fragment
}


Service.prototype.success = function ({ controlBar, btn, text, sn, devName }) {
  const self = this;
  const { btnObj, endService } = self;
  const { processBarRotate, showMsg, element: { circle1 } } = btnObj;

  const webkitTransitionEnd = function () {
    circle1.setAttributeNS(null, "stroke", null);
    showMsg.call(btnObj, sn ? `InnoAGE ${sn} ${text} completed.` : text).then(() => {
      controlBar.style.opacity = 0;

      function toDefault() {
        controlBar.style.opacity = 1;
      }
      processBarRotate.call(btnObj, 0, { controlBar, btn, webkitTransitionEnd: toDefault, devName }, false);
      endService.call(self, devName);
    });
  }

  if (document.body.contains(btn)) {
    processBarRotate.call(btnObj, 100, { controlBar, btn, webkitTransitionEnd, devName });
  } else {
    showMsg.call(btnObj, `InnoAGE ${sn} ${text} completed.`)
  }
}

Service.prototype.fail = function ({ text, callBack, sn }) {
  const self = this;
  const { btnObj } = self;
  const { showError } = btnObj;

  showError.call(btnObj, sn ? `InnoAGE ${sn} ${text} failed.` : text)
    .then(() => {
      if (typeof callBack === 'function') callBack();
    });
}

export default Service;

