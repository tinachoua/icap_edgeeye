// import Button from '../../button';
import Service from './service';
import { API } from '../../API';
import { MOCK_OOB } from "../../../constants/globalVariable";
import icon_action_recovery from '../../../../assets/images/icons/icon_action_recovery.png'

function Recovery(Mediator) {
  this.name = 'Recovery';
  this.btnObj = this.generateBtn({ Mediator })

  this.mediator = Mediator;
}

Recovery.prototype = new Service();

Recovery.prototype.makeIcon = function () {

  const icon = document.createElement('img');
  icon.src = icon_action_recovery;
  icon.style.position = 'absolute';
  icon.style.left = '8px';
  icon.style.top = '8px'

  return icon;
}

if (MOCK_OOB) {
  Recovery.prototype.start = async function ({ sn, devName }) {
    ///API
    const self = this;

    const { confirm, endService, btnObj, mediator } = self;
    const { element: { controlBar, btn },
      processBarRotate, hideIcon, showError
    } = btnObj;
    const result = await confirm({
      text: `${sn} will be recovered.`,
      confirmButtonText: `Yes, recover it.`,
    });

    if (result.value) {
      hideIcon.call(btnObj);

      btn.setAttribute('data-pct', '0%');

      mediator.fakeRecovery({ SN: sn, devName })

      return true
    }

    return false;
  }
} else {
  Recovery.prototype.start = async function ({ sn, devName }) {
    ///API
    const self = this;

    const { confirm, endService, btnObj } = self;
    const { element: { controlBar, btn },
      processBarRotate, hideIcon, showError
    } = btnObj;
    const result = await confirm({
      text: `${sn} will be recovered.`,
      confirmButtonText: `Yes, recover it.`,
    });

    if (result.value) {
      hideIcon.call(btnObj);

      btn.setAttribute('data-pct', '0%');
      this.btnObj.element.circle1.setAttributeNS(null, "stroke", '#D5D5D5');
      const fail = function () {
        showError.call(btnObj, 'InnoAGE ' + sn + ' recovery failed.');
        this.btnObj.element.circle1.setAttributeNS(null, "stroke", null);
        endService.call(self, devName)
      }

      API.get({
        url: `/innoAGE/recovery/${sn}`,
        global: false,
        fail,
        timeout: 90000
        // timeout: 1000 //test
      }).then(({ Payload: { Data: { Status } } }) => {
        if (Status !== '1') {
          fail();
        }
      }).catch(() => {
        fail();
      });

      return true
    }

    return false;
  }
}

Recovery.prototype.updateBar = function ({ devName, SN, Percentage }) {
  if (!this[devName]) return
  const self = this;
  const { btnObj, success } = self;
  const { controlBar, btn, icon } = self[devName];
  const { processBarRotate, hideIcon } = btnObj;

  if (Percentage < 100) {
    hideIcon(icon);
    this.btnObj.element.circle1.setAttributeNS(null, "stroke", '#D5D5D5');
    processBarRotate.call(btnObj, Percentage, { controlBar, btn, devName });
  } else if (Percentage === '100') {
    this.btnObj.element.circle1.setAttributeNS(null, "stroke", null);
    success.call(self, { controlBar, btn, text: 'recovery', sn: SN, devName });
  }
}

export default Recovery;
