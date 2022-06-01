import { fromEvent, race } from 'rxjs'
import { take, map, mapTo } from 'rxjs/operators';
import icon_collapse from '../../../../assets/images/icons/icon_collapse.svg'
import icon_expand from '../../../../assets/images/icons/icon_expand.svg'

const doc = document;

const checkElement = () => {
    const powerSwitchModalDiv = doc.querySelector('#powerSwitchModalDiv');
    if (!powerSwitchModalDiv) {
        const div = doc.createElement('div')
        div.id = 'powerSwitchModalDiv'
        doc.body.appendChild(div)
    }
}

const advancedSettingCloseTemplate = `
        <p style="font: normal normal normal 15px/26px Arial;
                letter-spacing: 0px;
                color: #3085D6;
                opacity: 1;">Advanced Settings</p>
        <img style="width: 24px;
        height: 24px;" src=${icon_collapse}>
    `;

const advancedSettingOpenTemplate = `
        <p style="font: normal normal normal 15px/26px Arial;
                letter-spacing: 0px;
                color: #3085D6;
                opacity: 1;">Advanced Settings</p>
        <img style="width: 24px;
        height: 24px;" src=${icon_expand}>
    `;

export const powerSwitchModal = ({ deviceName }) => {

    checkElement();
    const powerSwitchModalDiv = doc.querySelector('#powerSwitchModalDiv');

    let advancedSettingStatus = false;

    const powerSwitchModalTemplate = `<div  class="modal" tabindex="-1" id="powerSwitchModal">
    <div class="modal-dialog">
        <div class="modal-content" style="width:550px">
            <div class="modal-header">
                <h5 class="modal-title">Power switch</h5>
            </div>
            <div class="modal-body" >
                <p style="font: normal normal normal 15px/26px Arial;letter-spacing: 0px;color: #16272EBF;opacity: 1;padding-top: 15px;padding-left: 70px;">
                    ${deviceName} will be switched on or off.
                </p>

                <div id="powerSwitchAdvancedSetting" style="display: flex;flex-direction: row;padding-left: 70px;cursor: pointer;height: 17px;width: fit-content;">
                    ${advancedSettingCloseTemplate}
                </div>

                <div style="display: flex;flex-direction: column;padding-top: 24px;padding-left: 70px;" id="powerSwitchAdvancedSettingBody">
                 
                    <label for="triggerMode">Trigger mode</label>
                    <select style="width: 360px;" class="form-control" id="triggerMode" title="0 is low level, 1 is high level">
                        <option>0</option>
                        <option>1</option>
                    </select>
            
                    <div style="padding-top: 20px;
                    display: flex;">
                        <div>
                            <label for="triggerTime">Time:</label>
                            <input style="width: 170px;" type="number" class="form-control" id="triggerTime" placeholder="Trigger time length"
                                value="500">
                            </input>
                        </div>
                        <div style="padding-left: 20px;">
                        <label for="timeUnit">Time unit:</label>
                        <select style="width: 170px;" class="form-control" id="timeUnit" title="time unit">
                            <option>ms</option>
                            <option>s</option>
                        </select>
                    </div>
                    </div>
                </div>
                <div style="text-align: end;padding-top: 44px;">
                    <button type="button" id="powerSwitchModalCancel" class="btn"
                        style="font: normal normal normal 15px/17px Arial;background-color: #5E676C;color:#FFFFFF;">Cancel</button>
                    <button type="button" id="powerSwitchModalOk" class="btn"
                        style="font: normal normal normal 15px/17px Arial;background-color: #2e6da4; color: #FFFFFF;">OK</button>
                </div>
            </div>
        </div>
    </div>
</div>`;

    powerSwitchModalDiv.innerHTML = powerSwitchModalTemplate

    $("#powerSwitchModal").modal({ backdrop: 'static', keyboard: false });

    const powerSwitchAdvancedSettingBody = doc.querySelector('#powerSwitchAdvancedSettingBody');
    powerSwitchAdvancedSettingBody.style.display = 'none';
    const SubscriptionPowerSwitchAdvancedSetting = fromEvent(doc.querySelector('#powerSwitchAdvancedSetting'), 'click')
        .pipe(
            map(event => event.currentTarget)
        )
        .subscribe((currentTarget) => {
            currentTarget.innerHTML = advancedSettingStatus ? advancedSettingCloseTemplate : advancedSettingOpenTemplate;
            doc.querySelector('#powerSwitchAdvancedSettingBody').style.display = advancedSettingStatus ? 'none' : '';
            advancedSettingStatus = !advancedSettingStatus;
        })


    return new Promise((resolve) => {

        const ObservableCancel = fromEvent(doc.querySelector('#powerSwitchModalCancel'), 'click')
            .pipe(
                take(1),
                mapTo(false)
            )
        const ObservableOk = fromEvent(doc.querySelector('#powerSwitchModalOk'), 'click')
            .pipe(
                take(1),
                map(() => {
                    return {
                        mode: doc.querySelector('#triggerMode').value,
                        time: doc.querySelector('#triggerTime').value || 0,
                        timeUnit: doc.querySelector('#timeUnit').value,
                    }
                })
            )
        race(ObservableCancel, ObservableOk).subscribe(result => {
            $("#powerSwitchModal").modal('hide')
            SubscriptionPowerSwitchAdvancedSetting.unsubscribe()
            resolve(result)
        })
    })

}