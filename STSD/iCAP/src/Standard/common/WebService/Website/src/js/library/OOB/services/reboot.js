import Service from './service';
import { API } from '../../API';
import { MOCK_OOB } from '../../../constants/globalVariable'
import icon_action_reboot from '../../../../assets/images/icons/icon_action_reboot.png'

function Reboot(Mediator) {
  this.name = 'Reboot'
  this.btnObj = this.generateBtn({ Mediator });
  this.mediator = Mediator;
}

Reboot.prototype = new Service();

Reboot.prototype.makeIcon = function () {

  const icon = document.createElement('img');
  icon.src = icon_action_reboot;
  icon.style.position = 'absolute';
  icon.style.left = '9px';
  icon.style.top = '6px'

  return icon;
}

if (MOCK_OOB) {
  Reboot.prototype.start = async function ({ sn, devName }) {
    const self = this;
    const { confirm, btnObj, success, mediator } = self;
    const { element: { controlBar, btn, circle1 },
      processBarRotate, hideIcon,
    } = btnObj;

    const result = await confirm({
      text: `${sn} will be reboot.`,
      confirmButtonText: `Yes, reboot it.`,
    });

    if (result.value) {
      mediator[devName].loading = 1;
      hideIcon.call(btnObj);
      circle1.setAttributeNS(null, "stroke", '#D5D5D5');
      processBarRotate.call(btnObj, 90, { controlBar, btn, devName });
      setTimeout(() => {
        success.call(self, { controlBar, btn, text: 'reboot', sn, devName });
      }, 8000);
      return true
    }
    return false;
  }
} else {
  Reboot.prototype.start = async function ({ sn, devName }) {
    const self = this;
    const { confirm, endService, btnObj, success, fail, mediator } = self;
    const { element: { controlBar, btn, circle1 },
      processBarRotate, hideIcon,
    } = btnObj;

    const result = await confirm({
      text: `${sn} will be reboot.`,
      confirmButtonText: `Yes, reboot it.`,
    });

    if (result.value) {
      mediator[devName].loading = 1;
      hideIcon.call(btnObj);
      circle1.setAttributeNS(null, "stroke", '#D5D5D5');
      processBarRotate.call(btnObj, 90, { controlBar, btn, devName });

      API.get({
        url: `/innoAGE/reboot/${sn}`,
        global: false,
        success: function ({ Payload: { Data: { Status } } }) {
          if (Status === '1') {

            success.call(self, { controlBar, btn, text: 'reboot', sn, devName });
          } else {
            processBarRotate.call(btnObj, 0, { controlBar, btn, devName });
            fail.call(self, {
              text: 'reboot',
              sn,
              callBack: () => {
                endService.call(self, devName);
                circle1.setAttributeNS(null, "stroke", null);
              }
            });
          }
        },
        fail: () => {
          processBarRotate.call(btnObj, 0, { controlBar, btn, devName });

          fail.call(self, {
            text: 'reboot',
            sn,
            callBack: () => {
              endService.call(self, devName);
              circle1.setAttributeNS(null, "stroke", null);
            }
          });
        }
      });

      return true
    }

    return false;
  }

}



export default Reboot;
