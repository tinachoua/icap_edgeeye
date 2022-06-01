import { unlisten } from './event_handler';
import { isEmpty } from "../library/data_verification";
import { SwitchPage } from "../library/switch_page";
import { addOptionBySelectId } from "../library/select_form";
import { API } from "../library/api_handler";
import { AddHead } from "../library/data_table";
import { g_get_device_data_payload } from "../library/init_first_tabs";
import { updateTwoTableData } from "../library/init_first_tabs";
import { restartUpdateWidget, g_setIntervalId } from "../pages/dashboard";
import { timeConverter } from './common';
import { OOBDirector } from '../library/OOB';
import { DeviceManager as deviceManager} from '../DeviceManager';

export function dataTable(id) {
    function init(settingObj) {
        return $('#' + id).DataTable(settingObj);
    }

    var publicAPI = {
        init: init
    };

    return publicAPI;
}

export function insertSpecialTable(id) {
    return '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:0px;" width="100%">' +
        '<tr>' +
        `<td id = group-${id}>Group:` + '&nbsp' + '&nbsp' + '&nbsp' + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>' + `To : <select id = "emp-${id}" class = "selectpicker" multiple title = "--Please Select One--" data-dropup-auto="false"></select>` +
        '<br>' +
        `<span style = "position:absolute;margin-top:10px;">Subject : </span><textarea id = "sub-${id}"  rows="1" spellcheck="false" style = "position:absolute;left:100px;margin-top:8px;width:80%;"></textarea>` +
        '<br>' +
        `<span style = "position:absolute;margin-top:15px;">Message : </span><textarea id = "msg-${id}"  rows="5" spellcheck="false" style = "position:absolute;left:100px;margin-top:20px;width:80%"></textarea>` +
        '<br>' +
        '<button ' + 'id =send-' + id + ' name = "send-message" style = "margin-top:116px" class="btn btn-mini btn-dark" >Send</button>' + '&nbsp&nbsp&nbsp&nbsp' + `<span id="send-result-${id}"></span>` + '</td>' +
        '</tr>';
}

export function getSuccess(response, widgetId, labelIndex) {
    var id = 0;
    var event_table;
    var widgetModal = $('#myModal');
    var APICaller = API();
    const tableId = '#modal-data';
    const parsed_data = {
        record: JSON.parse(response.record),
        item: JSON.parse(response.item),
        deviceList: null,
        emp: response.emp
    };
    const settingObj = {
        data: parsed_data.record,
        columns: [{
            "className": 'details-control',
            "orderable": false,
            "data": null,
            "defaultContent": ''
        }, {
            "className": 'device-link-control',
            "data": 'name'
        }],
        order: [
            [1, 'asc']
        ]
    };
    var $toAttach = $("<thead><tr></tr></thead>");
    var $thead = $("<th></th>");
    var clickFlag = {
        iAnalyzer: false,
        deviceLink: false,
        initTable: false,
    };
    const _SwitchPage = SwitchPage();
    const $body = $('body');

    g_get_device_data_payload.WidgetId = widgetId;


    $toAttach.find("tr").append($thead);
    AddHead($toAttach, parsed_data.item);
    $(tableId + " thead").replaceWith($toAttach);


    if (parsed_data.item.length === 5) {
        settingObj.columns.push({
            data: 'storageSN'
        });
    }
    settingObj.columns.push({
        data: 'value'
    });
    settingObj.columns.push({
        data: 'ownerName'
    });
    settingObj.columns.push({
        data: 'time',
        render: function (d) {
            return timeConverter(d)
        }
    });


    event_table = $(tableId).DataTable(settingObj);

    $('#modal-data tbody').on('click', 'td.device-link-control button', (e) => {
        var data = JSON.parse($(e.target).attr('data-button'));
        var targetDevice = data.devName;
        var deviceList = [];
        var tmp = null;
        const specificDevice = $.map(parsed_data.record, function (item, index) {
            if (item.devName !== tmp) {
                deviceList.push({
                    alias: item.alias,
                    devName: item.devName
                });
                tmp = item.devName;
            }
            if (item.devName === data.devName)
                return item;
        });

        var promise = {
            GetDetail: function () { },
            GetOverView: function () { },
            GetLocation: function () { },
            GetImg: function () { },
            GetGroup: function () { }
        };

        parsed_data.deviceList = deviceList;
        $body.addClass('loading');

        //promise.GetThreshold = APICaller.GET('SettingAPI/GetThreshold');
        promise.GetDetail = APICaller.GET(`DeviceInfoAPI/GetDetail?DeviceName=${targetDevice}`);
        promise.GetOverView = APICaller.GET(`DeviceInfoAPI/GetOverview?DeviceName=${targetDevice}`);
        promise.GetLocation = APICaller.GET(`DeviceInfoAPI/GetLocation?DeviceName=${targetDevice}`);
        promise.GetImg = APICaller.GET('DeviceAPI/GetImg', 'devName', targetDevice);
        $.cookie("current_deviceId", targetDevice);

        $.when(promise.GetDetail, promise.GetOverView, promise.GetLocation, promise.GetImg).done((response_D, response_OV, response_L, response_Img) => {
            const oobDirector = new OOBDirector();
            var deviceData = {
                detail: JSON.parse(response_D[0]),
                overview: JSON.parse(response_OV[0]),
                location: JSON.parse(response_L[0]),
                img: JSON.parse(response_Img[0]),
            };

            _SwitchPage.ToWidgetDevice(e, event_table, clickFlag, parsed_data, false, deviceData, oobDirector);
            $(document).ready(() => {
                updateTwoTableData(targetDevice, $(e.target).text(), specificDevice);
            });
            $body.removeClass('loading');
        });

        $.when(promise.GetDetail, promise.GetOverView, promise.GetLocation, promise.GetImg).fail(() => {
            alert(`${targetDevice}'s dynamic/static raw data could not be found.`);
            $body.removeClass('loading');
        });
    });

    $('#modal-data tbody').on('click', 'td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = event_table.row(tr);
        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        } else {

            let $toAttach;
            let data = row.data();
            let promise = APICaller.GET(`WidgetAPI/Modal/Details?devName=${data.devName}`);

            $body.addClass('loading');
            promise.done((response) => {
                var parsed_data = JSON.parse(response);

                row.child(insertSpecialTable(id)).show();
                $toAttach = $(`#group-${id}`);
                parsed_data.branchList.forEach(element => {
                    let button = document.createElement('button');
                    button.setAttribute('name', `group-link-${id}`);
                    button.setAttribute('class', 'btn-link');
                    button.setAttribute('data-button', `{"branchId" : ${element.Id}}`);
                    button.textContent = element.Name;
                    $toAttach.append(button);
                });
                tr.addClass('shown');
                const message = $(`#msg-${id}`);
                const subject = $(`#sub-${id}`);
                addOptionBySelectId(`emp-${id}`, parsed_data.emp);
                $(`#emp-${id}`).selectpicker('refresh');
                $(`[name=group-link-${id}]`).click((e) => {
                    var data = JSON.parse($(e.target).attr('data-button'));
                    $.cookie('temp_groupId', data.branchId);
                    $("#myModal").modal("hide");
                    $('.modal-backdrop').remove();
                    $('.fade').remove();
                    $('.in').remove();
                    SwitchPage().ToGroup();
                });

                $('#send-' + id++).click((e) => {
                    var result = $(`#send-result-${id - 1}`);
                    var payload = {
                        //Message : isEmpty(message.val()) ? '(No Content)': message.val(),
                        Message: isEmpty(message.val()) ? '' : message.val(),
                        Subject: subject.val(),
                        Id: $.map($(`#emp-${id - 1}`).val(), (element) => {
                            return Number(element);
                        })
                    };
                    var promise;
                    var SendSuccess = function (response) {
                        var messageObj = JSON.parse(response);
                        result.css('color', 'green');

                        result.text(messageObj.Response);
                        $body.removeClass('loading');
                    };
                    var SendFail = function (response) {
                        var messageObj = JSON.parse(response.responseText);
                        result.css('color', 'red');
                        result.text(messageObj.Response);
                        $body.removeClass('loading');
                    };

                    result.text('');

                    if (payload.Id.length === 0) {
                        result.css('color', 'red');
                        result.text('Please specify at least one recipient.');
                    } else if (true === isEmpty(payload.Subject)) {
                        if (true === confirm("Send this message without a subject or text in the body?")) {
                            $body.addClass('loading');
                            promise = APICaller.POST('DashboardAPI/Widget/SendEmail', payload);
                            promise.done(SendSuccess);
                            promise.fail(SendFail);
                        }
                    } else {
                        $body.addClass('loading');
                        promise = APICaller.POST('DashboardAPI/Widget/SendEmail', payload);
                        promise.done(SendSuccess);
                        promise.fail(SendFail);
                    }
                });
                $body.removeClass('loading');
            });
        }
    });

    widgetModal.on('hidden.bs.modal', function () {
        var unlistenId = ['myModal', 'modal-data tbody', 'modal-data'];

        event_table.clear();
        event_table.destroy();
        restartUpdateWidget(document.getElementById('dashboard-item'));
        unlisten(unlistenId);
    });

    widgetModal.modal('show');
    clearInterval(g_setIntervalId);// Cancel auto update widget data
    $body.removeClass('loading');
}