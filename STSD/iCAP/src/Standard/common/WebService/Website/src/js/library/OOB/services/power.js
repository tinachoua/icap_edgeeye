import Service from './service';
import { API } from '../../API';
import { MOCK_OOB } from '../../../constants/globalVariable'
import icon_action_power from '../../../../assets/images/icons/icon_action_power.png'
import { powerSwitchModal } from '../helpers/powerSwitchModalHandler'
function Power(Mediator) {
    this.name = 'Power'
    this.btnObj = this.generateBtn({ Mediator });
    this.mediator = Mediator;
}

Power.prototype = new Service();

Power.prototype.makeIcon = function () {

    const icon = document.createElement('img');
    icon.src = icon_action_power;
    icon.style.position = 'absolute';
    icon.style.left = '8px';
    icon.style.top = '8px'

    return icon;
}

if (MOCK_OOB) {
    Power.prototype.start = async function ({ sn, devName }) {
        const self = this;
        const { confirm, btnObj, success, mediator } = self;
        const { element: { controlBar, btn },
            processBarRotate, hideIcon,
        } = btnObj;

        const result = await confirm({
            text: `${sn} will be Power.`,
            confirmButtonText: `Yes, Power it.`,
        });

        if (result.value) {
            mediator[devName].loading = 1;
            hideIcon.call(btnObj);
            processBarRotate.call(btnObj, 90, { controlBar, btn, devName });
            setTimeout(() => {
                success.call(self, { controlBar, btn, text: 'Power', sn, devName });
            }, 8000);
            return true
        }
        return false;
    }
} else {
    Power.prototype.start = async function ({ sn, devName }) {
        const self = this;
        const { endService, btnObj, success, fail, mediator } = self;
        const { element: { controlBar, btn },
            processBarRotate, hideIcon,
        } = btnObj;


        const result = await powerSwitchModal({ deviceName: devName })
        //console.log(result)

        if (result) {
            mediator[devName].loading = 1;
            hideIcon.call(btnObj);
            processBarRotate.call(btnObj, 90, { controlBar, btn, devName });

            API.post({
                payload: result,
                url: `/innoAGE/power-switch/${sn}`,
                global: false,
                success: function ({ HTTPCode }) {
                    if (+HTTPCode === 200) {
                        success.call(self, { controlBar, btn, text: 'The power operation was completed!', devName });
                    } else {
                        processBarRotate.call(btnObj, 0, { controlBar, btn, devName });
                        fail.call(self, {
                            text: 'The power operation was failed!',
                            sn,
                            callBack: () => {
                                endService.call(self, devName);
                            }
                        });
                    }
                },
                fail: () => {
                    processBarRotate.call(btnObj, 0, { controlBar, btn, devName });

                    fail.call(self, {
                        text: 'The power operation was failed!',
                        sn,
                        callBack: () => {
                            endService.call(self, devName);
                        }
                    });
                }
            });

            return true
        }

        return false;
    }

}



export default Power;
