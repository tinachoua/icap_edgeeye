import { render } from "../library/component";
import device_analyzer_page from "../../html/pages/iAnalyzer.html";
import { SwitchPage } from "./switch_page";
import { g_device_data } from "../library/get_device_info";
import { API } from "../library/api_handler";
import { timeConverter } from '../library/common';
import { DeviceManager as deviceManager} from '../DeviceManager';

export var g_get_device_data_payload = {
    WidgetId: -1,
    DevName: null,
    Employee: null
};
export var updateTwoTableData = function (devName, alias, specificDevice) {
    var APICaller = API();
    var promise = APICaller.GET('DashboardAPI/Widget/GetDeviceData?id=' + g_get_device_data_payload.WidgetId + '&' + 'devName=' + ((Boolean(devName) === true) ? devName : $('#device-select').val()), null, null, false);
    promise.done((response) => {
        var parsed_data = JSON.parse(response);
        var updateDeviceTableInfo = function (parsed_data) {
            const deviceTableInfo = document.getElementById('device-table-info');
            var owner = deviceTableInfo.rows[1].cells[0].innerHTML;
            var group = deviceTableInfo.rows[1].cells[1].innerHTML;
            var newGroupCell = parsed_data.record[0].BranchName.join(', ');
            var newOwner = parsed_data.record[0].OwnerName;

            if (group !== newGroupCell) {
                deviceTableInfo.rows[1].cells[1].innerHTML = newGroupCell;
            }

            if (newOwner !== owner) {
                deviceTableInfo.rows[1].cells[0].innerHTML = newOwner;
            }
        };
        var addRowData = function (parsed_data) {
            $('#device-table .newData').remove();
            if (Boolean(specificDevice[0].storageSN) === true) {
                let specificStorageSN = $.map(specificDevice, (element) => {
                    return element.storageSN;
                });
                let allStorageSN = $.map(parsed_data.record, (element) => {
                    return element.storageSN
                })
                parsed_data.record.forEach((element) => {
                    let td = `<td>${alias}<span style = 'color:brown;'>(NEW)</span></td>` +
                        `<td>${element.StorageSN}</td>` +
                        `<td>${element.Value}</td>` +
                        `<td>${timeConverter(element.Time)}</td><td></td>`;
                    if (specificStorageSN.indexOf(`${element.StorageSN}`) !== -1) {
                        $('<tr class ="newData font_bold">' + td + '</tr>').prependTo("#device-table > tbody");
                    } else {
                        $('<tr class ="newData">' + td + '</tr>').prependTo("#device-table > tbody");
                    }
                });
            } else {
                parsed_data.record.forEach((element) => {
                    let td =
                        `<td>${alias}<span style = 'color:brown;'>(NEW)</span></td>` +
                        `<td>${element.Value}</td>` +
                        `<td>${timeConverter(element.Time)}</td><td></td>`;

                    $('<tr class ="newData font_bold">' + td + '</tr>').prependTo("#device-table > tbody");
                });
            }
        };
        g_get_device_data_payload.Employee = parsed_data.emp;
        updateDeviceTableInfo(parsed_data);
        addRowData(parsed_data);
    });
};

export var putFirstsTabs = function (deviceInfo, clickFlag, event_table, oobDirector) {
    const DeviceInfoWrapper = document.createElement('div');
    DeviceInfoWrapper.classList.add('device-navbar-wrapper');

    var tab_primary_container = document.getElementById("tab-primary-continer");
    var deiveIcon = document.createElement('i');
    var deviceSelect = document.createElement('select');
    var forward = $Tab({
        name: '',
        style: 'padding: 12px 10px;',
        className: 'fa fa-chevron-circle-left'
    }).getTab();
    var next = $Tab({
        name: '',
        style: 'padding: 12px 10px; margin-right:12px',
        className: 'fa fa-chevron-circle-right'
    }).getTab();
    var iAnalyzer = $Tab({
        name: 'iAnalyzer',
        className: 'fa fa-hdd-o'
    }).getTab();
    var refresh = $Tab({
        name: 'Refresh',
        className: 'fa fa-refresh'
    }).getTab();
    var select = [deiveIcon, deviceSelect, forward, next];
    var currentDevice = g_device_data.detail.DevName;
    var initSelect = function (select) {
        select.setAttribute('id', 'device-select');
        select.setAttribute('style', 'margin-top:12px;');
        deviceInfo.deviceList.forEach((element) => {
            let option = document.createElement("option");
            option.value = element.devName;
            option.textContent = (!element.alias) ? element.devName : element.alias;
            select.appendChild(option);
        });
        $('#device-select').val(g_device_data.detail.DevName);
        select.onchange = function (e, refreshFlag) {
            $(window).off('resize');

            var promise = {
                GetDetail: function () { },
                GetOverView: function () { },
                GetLocation: function () { },
                GetImg: function () { }
            };
            var _SwitchPage = SwitchPage();
            const overview = document.getElementsByName('widget-overview');
            const APICaller = API();
            const target = $(e.target).val();

            $('body').addClass('loading');

            promise.GetDetail = APICaller.GET(`DeviceInfoAPI/GetDetail?DeviceName=${target}`);
            promise.GetOverView = APICaller.GET(`DeviceInfoAPI/GetOverview?DeviceName=${target}`);
            promise.GetLocation = APICaller.GET(`DeviceInfoAPI/GetLocation?DeviceName=${target}`);
            promise.GetImg = APICaller.GET('DeviceAPI/GetImg', 'devName', target);

            $.when(promise.GetDetail, promise.GetOverView, promise.GetLocation, promise.GetImg).done((response_D, response_OV, response_L, response_Img) => {
                $('#child-page .widget').hide();
                $(overview).show();
                $('#tab-primary-continer a').removeClass('tab-click');
                $('#child-page .device_analyzer').remove();
                var deviceData = {
                    detail: JSON.parse(response_D[0]),
                    overview: JSON.parse(response_OV[0]),
                    location: JSON.parse(response_L[0]),
                    img: JSON.parse(response_Img[0]),
                };

                _SwitchPage.ToWidgetDevice(e, event_table, clickFlag, deviceInfo, refreshFlag, deviceData, oobDirector);
                if (Boolean(refreshFlag) === false) {
                    const specificDevice = $.map(deviceInfo.record, function (item, index) {
                        if (item.devName === $(e.target).val())
                            return item;
                    });
                    updateTwoTableData(null, $('#device-select option:selected').text(), specificDevice);
                }
                currentDevice = target;
                $('body').removeClass('loading');
            });

            $.when(promise.GetDetail, promise.GetOverView, promise.GetLocation, promise.GetImg).fail(() => {
                alert(`${target}'s dynamic/static raw data could not be found.`);
                $('#device-select').val(currentDevice);
                $('body').removeClass('loading');
            });
        };
    };
    const child_page = $('#child-page');
    const switchTo = {
        Forward: function () {
            var currentOption = $('#device-select').val();
            var firstOption = $('#device-select >option:first').val();

            if (currentOption === firstOption) {
                alert("It's first one.");
            } else {
                $("#device-select > option:selected")
                    .prop("selected", false)
                    .prev()
                    .prop("selected", true);
                $('#device-select').trigger('change');
            }
        },
        Next: function () {
            var currentOption = $('#device-select').val();
            var lastOption = $('#device-select >option:last').val();

            if (currentOption === lastOption) {
                alert("It's last one.");
            } else {
                $("#device-select > option:selected")
                    .prop("selected", false)
                    .next()
                    .prop("selected", true);
                $('#device-select').trigger('change');
            }
        },
        iAnalyzer: function (g_device_data) {
            var _render = render();
            var $widget = child_page.find('.widget');
            var $analyzer = child_page.find('.device_analyzer');
            var $secondTabs = $('#tab-secondary-continer a');

            $(window).off('resize');
            $widget.hide();
            $secondTabs.removeClass('hover');
            $analyzer.remove();
            child_page.append(device_analyzer_page);
            _render.iAnalyzer(g_device_data.DevName);
        },
        Refresh: function () {
            var refreshFlag = true;
            const currentDevName = $('#device-select option:selected').val();
            const Alias = $('#device-select option:selected').text();
            const specificDevice = $.map(deviceInfo.record, function (item, index) {
                if (item.devName === currentDevName)
                    return item;
            });
            $('#device-select').trigger('change', refreshFlag);
            updateTwoTableData(null, Alias, specificDevice);
        }
    };
    var put = function (tabs, container) {
        tabs.forEach((element) => {
            container.appendChild(element);
            container.appendChild($Tab().getDivider());
        });
    };


    deiveIcon.setAttribute('class', 'fa fa-laptop');
    deiveIcon.setAttribute('style', 'margin-top:15px; margin-left:16px; margin-right:2px; color:white;');

    forward.addEventListener("click", switchTo.Forward);
    next.addEventListener('click', switchTo.Next);
    iAnalyzer.addEventListener('click', (e) => {
        $(iAnalyzer).addClass('tab-click');
        switchTo.iAnalyzer(g_device_data.overview);
    });
    refresh.addEventListener('click', (e) => {
        switchTo.Refresh();
    });

    //----------------------------put tabs----------------------------------------------------
    select.forEach((element) => {
        DeviceInfoWrapper.appendChild(element);
        // tab_primary_container.appendChild(element);
    });
    DeviceInfoWrapper.appendChild($Tab().getDivider());
    put([iAnalyzer, refresh], DeviceInfoWrapper);
    tab_primary_container.appendChild(DeviceInfoWrapper)
    //-----------------------------------------------------------------------------------------
    initSelect(deviceSelect);
};