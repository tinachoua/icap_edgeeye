import { fromEvent, race, Subject } from 'rxjs'
import { take, map, mapTo } from 'rxjs/operators';
import icon_section_collapse from '../../../../assets/images/icons/icon_section_collapse.png'
import icon_section_expand from '../../../../assets/images/icons/icon_section_expand.png'
import icon_action_recovery from '../../../../assets/images/icons/icon_action_recovery.png'
import { API } from '../../API';
import Swal from 'sweetalert2';
import icon_fail from '../../../../assets/images/icons/icon_fail.png';

const doc = document;
const errorSubject = new Subject();
let SN;
let observableCancel;
let observableOk;
let backupAdvancedInfoOpenStatus = 0;
let observerBackupClearButton;

const backupModalLayout = `
<div class="modal" tabindex="-1" id="backupModal">
    <div class="modal-dialog">
        <div class="modal-content" style="width:550px;min-height: 562px;">
            <div id="backupLoadingDiv" style="position: absolute;background-color: white;z-index: 99;margin: 10px;"></div>
            <div id="backupModalMain" style="margin:24px 40px 40px 40px;">
                <div id="backupModalBody">
                    <div style="width: 460px;">
                        <div style="height: 32px; display: flex;flex-direction: row;align-items: center;">
                            <p style="padding-right: 10px;font: normal normal normal 24px/28px Arial;letter-spacing: 0px;color: #16272E;opacity: 1;">Backup</p>
                            <p id="isBackupExisting" style="border-radius: 4px;text-align: center;color: #16272E;font-size: 13px;line-height: 20px;height:20px;"></P>
                            <div id="backupModalRefreshBtn" class="icap-button-hover" style="margin-left: auto;border: 1px solid #CCCCCC;border-radius: 4px;">
                                <img style="width: 30px;height: 30px;" src="${icon_action_recovery}"/>
                            </div> 
                        </div>
                         <p id="continuingBackupInfo" style="height: 15px;font: normal normal normal 13px/15px Arial;letter-spacing: 0px;color: #16272E;opacity: 0.6;">
                            Continuing the backup operation will overwrite the existing backup.
                        </p>
                    </div>

                    <div style="width: 460px;border: 1px solid #E0E1E6;">
                        <p style="font: normal normal normal 18px/26px Arial;padding: 20px 0px 0px 20px;">
                            Please select backup partition
                        </p>
                        <hr>
                        <div id="backupPartitionInfo" style="margin: 20px;"></div>
                    </div>

                    <div style="width: 460px;border: 1px solid #E0E1E6;margin-top: 15px;">
                        <div style="display: flex;align-items: center;padding: 0px 20px;height: 35px;cursor: pointer;" id="backupAdvancedInfoDiv">
                            <span style="font: normal normal normal 18px/26px Arial;">Advanced information</span>
                            <img id="backupAdvancedInfoImg" style="margin-left: auto;" src="${icon_section_collapse}"/>
                        </div>
                        <hr style="margin: 0px;">
                        <div id="backupAdvancedInfoContent"></div>
                    </div>
                </div>
                <div style="text-align: end;padding-top: 30px;">
                    <button type="button" id="backupModalCancel" class="btn" style="font: normal normal normal 15px/17px Arial;background-color: #5E676C;color:#FFFFFF;">
                        Cancel
                    </button>
                    <button type="button" id="backupModalOk" class="btn" style="font: normal normal normal 15px/17px Arial;background-color: #2e6da4; color: #FFFFFF;">
                        Backup
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>
`;

const switchIsBackupExisting = (status) => {

    const isBackupExisting = doc.querySelector('#isBackupExisting')
    if (!isBackupExisting) return;

    switch (+status) {
        case 0:
            isBackupExisting.style.backgroundColor = '#80DB22';
            isBackupExisting.innerHTML = 'No backup existing';
            isBackupExisting.style.width = '130px';
            break;
        default:
            isBackupExisting.style.backgroundColor = '#EBD023';
            isBackupExisting.innerHTML = 'Backup existing';
            isBackupExisting.style.width = '112px';
    }
}


const switchContinuingBackupInfo = (status) => {

    const continuingBackupInfo = doc.querySelector('#continuingBackupInfo')
    if (!continuingBackupInfo) return;

    switch (+status) {
        case 0:
            continuingBackupInfo.innerHTML = ' ';
            break;
        default:
            continuingBackupInfo.innerHTML = 'Continuing the backup operation will overwrite the existing backup.';
    }
}


const loadingTemplate = (msg) => `
<div style="width: 535px;height: 540px;display: flex;flex-direction: column;align-items: center;justify-content: center;">
    <span class="fa fa-spinner fa-3x circle-spin" aria-hidden="true"></span>
    <span style="padding-top: 25px;font: normal normal normal 28px/32px Arial;letter-spacing: 0px;color: #16272E;opacity: 1;">${msg}<span/>
</div>
`;

const loadingShow = (msg) => {
    const backupLoadingDiv = doc.querySelector('#backupLoadingDiv');
    backupLoadingDiv.innerHTML = loadingTemplate(msg)
    backupLoadingDiv.style.display = '';
}

const loadingClose = () => {
    doc.querySelector('#backupLoadingDiv').style.display = 'none'
}

const switchBackupModalOk = (status) => {
    const backupModalOk = doc.querySelector('#backupModalOk');
    if (!backupModalOk) return;

    if (+status === 1) {
        backupModalOk.disabled = false;
    } else {
        backupModalOk.disabled = true;
    }
}

const switchAdvancedInfo = (status) => {

    const backupAdvancedInfoImg = doc.querySelector('#backupAdvancedInfoImg');
    const backupAdvancedInfoContent = doc.querySelector('#backupAdvancedInfoContent');
    if (!backupAdvancedInfoImg || !backupAdvancedInfoContent) return;

    if (+status === 1) {
        backupAdvancedInfoImg.src = icon_section_expand;
        backupAdvancedInfoContent.style.display = '';
        backupAdvancedInfoOpenStatus = 1;
    } else {
        backupAdvancedInfoImg.src = icon_section_collapse;
        backupAdvancedInfoContent.style.display = 'none';
        backupAdvancedInfoOpenStatus = 0;
    }
}

const generatorBackupPartitionInfoTemplate = ({ partitionList, canBackupSize, reservedSector }) => {

    const template = [];

    template.forEach
    partitionList.forEach((partition, i) => {
        template.push(`
        <div style="display: flex;align-items: baseline;">
            <input ${partition.End_LBA < canBackupSize ? '' : 'disabled'} type="radio" id="backupPartitionInfo${partition.partId}" name="backupPartition" value="${partition.partId}">
            <label for="backupPartitionInfo${partition.partId}" style="padding-left: 10px;font: normal normal bold 16px/26px Arial;">Partition 1${i == 0 ? '' : '~' + (i + 1)}</label>
            <p style="margin-left: auto;font: normal normal normal 16px/26px Arial;">0.00GB~${partition.End}GB</p>
        </div>
        `)
    })

    template.push(`
    <div style="display: flex;align-items: baseline;padding-top: 40px;">
        <p style="font: normal normal normal 16px/26px Arial;">Sector</p>
        <input id="backupSector" style="background: #EEEEEE 0% 0% no-repeat padding-box;border: 1px solid #CCCCCC;border-radius: 4px;margin-left: 10px;width: 160px;" type="text" readonly value="${reservedSector}">
    </div>
    `)
    return template.join('');
}

const handleBackClear = async () => {
    try {
        loadingShow('Backup Clearing ...');
        switchAdvancedInfo(0)
        unsubscribeBackupClearButton()
        await API.post({
            url: `/innoAGE/backup-clear/${SN}`,
            global: false,
            success: function () {
                fetchBackUpStatus('Clear success. reloading ...')
            },
            fail: () => {
                fetchBackUpStatus('Clear fail. reloading ...')
            }
        });
    } catch (error) { }
}

const unsubscribeBackupClearButton = () => {
    if (observerBackupClearButton) {
        observerBackupClearButton.unsubscribe()
    }
}
const subscribeBackupClearButton = () => {
    unsubscribeBackupClearButton()
    observerBackupClearButton = fromEvent(doc.querySelector('#backupClearButton'), 'click').subscribe(async () => {

        const result = await Swal.fire({
            imageUrl: icon_fail,
            width: 360,
            title: 'Are you sure?',
            text: `${SN} backup will be clear.`,
            showCancelButton: true,
            confirmButtonColor: '#3085D6',
            cancelButtonColor: '#5E676C',
            confirmButtonText: 'OK',
            reverseButtons: true,
        })

        if (result.value) {
            handleBackClear()
        }
    })
}

const generatorBackupAdvancedInfoTemplate = (FWStatus) => {
    return `
    <div style="display: flex;align-items: center;padding: 0px 20px;height: 38px;">
        <span style="font: normal normal bold 14px/16px Arial; width:200px;">IsSetBackup</span>
        <span style="padding-left: 20px;font: normal normal normal 14px/16px Arial;">${FWStatus['IsSetBackup']}</span>
    </div>
    <hr style="margin: 0px;">
    <div style="display: flex;align-items: center;padding: 0px 20px;height: 38px;">
        <span style="font: normal normal bold 14px/16px Arial; width:200px;">Backup status</span>
        <span style="padding-left: 20px;font: normal normal normal 14px/16px Arial;">${+FWStatus['BackupStatus'] === 0 ? 'No' : 'Yes'}</span>
        <span id="backupClearButton" style="cursor: pointer;margin-left: auto;color: #3085D6;font: normal normal normal 14px/16px Arial;">
            ${+FWStatus['BackupStatus'] !== 0 ? 'Clear existing backup' : ''}
        </span>
    </div>
    <hr style="margin: 0px;">
    <div style="display: flex;align-items: center;padding: 0px 20px;height: 38px;">
        <span style="font: normal normal bold 14px/16px Arial; width:200px;">Recovery status</span>
        <span style="padding-left: 20px;font: normal normal normal 14px/16px Arial;">${FWStatus['RecoveryStatus']}</span>
    </div>
    <hr style="margin: 0px;">
    <div style="display: flex;align-items: center;padding: 0px 20px;height: 38px;">
        <span style="font: normal normal bold 14px/16px Arial; width:200px;">Copy LBA start</span>
        <span style="padding-left: 20px;font: normal normal normal 14px/16px Arial;">${FWStatus['CopyLBAStart']} GB</span>
    </div>
    <hr style="margin: 0px;">
    <div style="display: flex;align-items: center;padding: 0px 20px;height: 38px;">
        <span style="font: normal normal bold 14px/16px Arial; width:200px;">Copy LBA end</span>
        <span style="padding-left: 20px;font: normal normal normal 14px/16px Arial;">${FWStatus['CopyLBAEnd']} GB</span>
    </div>
    <hr style="margin: 0px;">
    <div style="display: flex;align-items: center;padding: 0px 20px;height: 38px;">
        <span style="font: normal normal bold 14px/16px Arial; width:200px;">Target LBA</span>
        <span style="padding-left: 20px;font: normal normal normal 14px/16px Arial;">${FWStatus['TargetLBA']} GB</span>
    </div>
    <hr style="margin: 0px;">
    <div style="display: flex;align-items: center;padding: 0px 20px;height: 38px;">
        <span style="font: normal normal bold 14px/16px Arial; width:200px;">Write protect start</span>
        <span style="padding-left: 20px;font: normal normal normal 14px/16px Arial;">${FWStatus['WriteProtectStart']} GB</span>
    </div>
    <hr style="margin: 0px;">
    <div style="display: flex;align-items: center;padding: 0px 20px;height: 38px;">
        <span style="font: normal normal bold 14px/16px Arial; width:200px;">Write protect end</span>
        <span style="padding-left: 20px;font: normal normal normal 14px/16px Arial;">${FWStatus['WriteProtectEnd']} GB</span>
    </div>
    <hr style="margin: 0px;">
    <div style="display: flex;align-items: center;padding: 0px 20px;height: 38px;">
        <span style="font: normal normal bold 14px/16px Arial; width:200px;">Reserved sector</span>
        <span style="padding-left: 20px;font: normal normal normal 14px/16px Arial;">${FWStatus['ReservedSector']} KB</span>
    </div>
    <hr style="margin: 0px;">
    <div style="display: flex;align-items: center;padding: 0px 20px;height: 38px;">
        <span style="font: normal normal bold 14px/16px Arial; width:200px;">Hidden</span>
        <span style="padding-left: 20px;font: normal normal normal 14px/16px Arial;">${FWStatus['Hidden']} GB</span>
    </div>
    <hr style="margin: 0px;">
    <div style="display: flex;align-items: center;padding: 0px 20px;height: 38px;">
        <span style="font: normal normal bold 14px/16px Arial; width:200px;">Capacity</span>
        <span style="padding-left: 20px;font: normal normal normal 14px/16px Arial;">${FWStatus['Capacity']} GB</span>
    </div>
    `;
}

const dataHandler = (Data) => {

    //console.log(Data)

    const FWStatus = Data['FW-Status'];
    const partitionList = Data['partitionList'];
    const partitionListLength = partitionList.length;
    let canBackupSize;

    if (FWStatus.HiddenLBA <= 0) {
        canBackupSize = FWStatus['CapacityLBA'] - FWStatus['ReservedSector'] - partitionList[partitionListLength - 1]['End_LBA'];
    } else {
        canBackupSize = FWStatus['HiddenLBA'] - FWStatus['ReservedSector'];
    }
    doc.querySelector('#backupPartitionInfo').innerHTML = generatorBackupPartitionInfoTemplate({ partitionList, canBackupSize, reservedSector: FWStatus['ReservedSector'] });
    doc.querySelector('#backupAdvancedInfoContent').innerHTML = generatorBackupAdvancedInfoTemplate(FWStatus);

    switchIsBackupExisting(FWStatus.BackupStatus)
    switchContinuingBackupInfo(FWStatus.BackupStatus)
    if (+FWStatus['BackupStatus'] !== 0) {
        subscribeBackupClearButton()
    }
}

const fetchBackUpStatus = async (msg) => {
    try {

        loadingShow(msg);
        switchBackupModalOk(0)
        switchAdvancedInfo(0)
        unsubscribeBackupClearButton()
        await API.get({
            url: `/innoAGE/backup-status/${SN}`,
            global: false,
            success: function ({ "Payload": { Data } }) {
                dataHandler(Data)
            },
            fail: () => {
                //console.log('fail')
                errorSubject.next('error')
                $("#backupModal").modal('hide')
            },
            complete: () => {
                loadingClose()
            }
        });
    } catch (error) { }
}

const setObservableChild = () => {

    fromEvent(doc.querySelector('#backupAdvancedInfoDiv'), 'click')
        .subscribe(() => backupAdvancedInfoOpenStatus === 0 ? switchAdvancedInfo(1) : switchAdvancedInfo(0))

    fromEvent(doc.querySelector('#backupModalRefreshBtn'), 'click')
        .subscribe(() => fetchBackUpStatus('Refreshing'))

    fromEvent(doc.querySelector('#backupPartitionInfo'), 'click')
        .subscribe(() => {
            const checkedElement = doc.querySelector('input[name="backupPartition"]:checked');
            if (checkedElement) {
                switchBackupModalOk(1)
            } else {
                switchBackupModalOk(0)
            }
        })

    observableCancel = fromEvent(doc.querySelector('#backupModalCancel'), 'click')
        .pipe(
            take(1),
            mapTo(false)
        )
    observableOk = fromEvent(doc.querySelector('#backupModalOk'), 'click')
        .pipe(
            take(1),
            map(() => {
                return {
                    sector: doc.querySelector('#backupSector').value,
                    partId: doc.querySelector('input[name="backupPartition"]:checked').value,
                }
            })
        )
}

const checkElement = () => {
    const backupModalDiv = doc.querySelector('#backupModalDiv');
    if (!backupModalDiv) {
        const div = doc.createElement('div')
        div.id = 'backupModalDiv';
        div.innerHTML = backupModalLayout;
        doc.body.appendChild(div)
        setObservableChild()
    }
}

export const backupModal = async ({ sn }) => {

    SN = sn;

    checkElement();

    $("#backupModal").modal({ backdrop: 'static', keyboard: false });

    fetchBackUpStatus('Loading');

    return new Promise((resolve) => {
        const sub = race(observableCancel, observableOk, errorSubject).subscribe(result => {
            $("#backupModal").modal('hide')
            sub.unsubscribe()
            unsubscribeBackupClearButton()
            resolve(result)
        })
    })
}