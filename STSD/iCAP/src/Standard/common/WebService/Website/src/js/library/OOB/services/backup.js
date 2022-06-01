// import Button from '../../button';
import Service from './service';
import { API } from '../../API';
import { MOCK_OOB } from "../../../constants/globalVariable";
import icon_action_backup from '../../../../assets/images/icons/icon_action_backup.png'
import { backupModal } from '../helpers/backupModalHandler'

function Backup(Mediator) {
  this.name = 'Backup';
  this.btnObj = this.generateBtn({ Mediator })

  this.mediator = Mediator;
}

Backup.prototype = new Service();

Backup.prototype.makeIcon = function () {

  const icon = document.createElement('img');
  icon.src = icon_action_backup;
  icon.style.position = 'absolute';
  icon.style.left = '8px';
  icon.style.top = '8px'

  return icon;
}

if (MOCK_OOB) {
  Backup.prototype.start = async function ({ sn, devName }) {
    ///API
    const self = this;

    const { confirm, btnObj, mediator } = self;
    const { element: { btn },
      hideIcon,
    } = btnObj;


    const result = await confirm({
      text: `${sn} will be backup.`,
      confirmButtonText: `Yes, backup it.`,
    });

    if (result.value) {
      hideIcon.call(btnObj);

      btn.setAttribute('data-pct', '0%');

      mediator.fakeBackup({ SN: sn, devName })

      return true
    }

    return false;
  }
} else {
  Backup.prototype.start = async function ({ sn, devName }) {
    ///API
    const self = this;

    const { confirm, endService, btnObj, mediator } = self;
    const { element: { controlBar, btn },
      processBarRotate, hideIcon, showError
    } = btnObj;

    const result = await backupModal({ sn })
    //console.log(result)

    if (result === 'error') {
      showError.call(btnObj, 'Please try again later.');
    } else if (result) {
      mediator[devName].loading = 1;
      hideIcon.call(btnObj);

      btn.setAttribute('data-pct', '0%');

      const fail = function () {
        showError.call(btnObj, 'InnoAGE ' + sn + ' Backup failed.');

        endService.call(self, devName)
      }

      API.post({
        url: `/innoAGE/backup/${sn}`,
        global: false,
        fail,
        timeout: 90000,
        payload: result,
        // timeout: 1000 //test
      }).then(() => {

      }).catch(() => {
        fail();
      });

      return true
    }

    return false;
  }
}

Backup.prototype.updateBar = function ({ devName, SN, Percentage }) {
  if (!this[devName]) return
  const self = this;
  const { btnObj, success } = self;
  const { controlBar, btn, icon } = self[devName];
  const { processBarRotate, hideIcon } = btnObj;

  if (Percentage < 100) {
    hideIcon(icon);
    processBarRotate.call(btnObj, Percentage, { controlBar, btn, devName });
  } else if (Percentage === '100') {
    success.call(self, { controlBar, btn, text: 'Backup had been done. ', devName });
  }
}

export default Backup;
