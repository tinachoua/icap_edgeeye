import event_page from "../../html/pages/event.html";
import album_page from "../../html/pages/album.html"
import dashboard_page from "../../html/pages/dashboard.html";
import setting_frame from "../../html/layout/setting_frame.html";
import { g_setIntervalId } from "../pages/dashboard";
import widget_device_page from "../../html/pages/widget_device_link.html";
import { AddHead } from "../library/data_table";
import { API } from "../library/api_handler";
import { initDeviceInfoPage } from "../library/get_device_info";
import { addOptionBySelectId } from "../library/select_form";
import { autoRefresh } from "../library/init_second_tabs";
import { g_get_device_data_payload } from "../library/init_first_tabs";
import send_email_modal from "../../html/pages/send_email_modal.html";
import overview from "../../html/components/device_info_overview.html";
import { isEmpty } from "../library/data_verification";
import { g_outChart } from "../pages/dashboard";
import { widget } from "../library/make_chart";
import { showAlbum } from '../library/album'
import { timeConverter } from '../library/common';
import { DeviceManager as deviceManager} from '../DeviceManager';
import {
    BG_COLOR_05
} from "../constants/globalVariable"

export function SwitchPage() {
    function remvoeBSDropdown() {
        var BSSelect = document.querySelector('body > div.bootstrap-select div.open');

        BSSelect && BSSelect.parentNode.removeChild(BSSelect);
    }

    function pageInit() {
        if (g_outChart.length !== 0) {
            widget().removeWidget(g_outChart, g_outChart.length);
        }
    }


    function SwitchToEvent() {
        var child_page = document.getElementById("child-page");

        pageInit();
        remvoeBSDropdown();

        clearInterval(g_setIntervalId);
        child_page.innerHTML = event_page;
        $.cookie("current_page", "event");
        $(document).trigger("reload-event");
        document.getElementById("contact-buttons-bar").style.display = 'none';
    }

    function SwitchToAlbum() {
        var child_page = document.getElementById("child-page");

        pageInit();
        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = album_page;
        $.cookie("current_page", "album");
        showAlbum($.cookie("current_deviceId") || '');
        document.getElementById("contact-buttons-bar").style.display = 'block';
    }

    function SwitchToGroup() {
        pageInit();
        remvoeBSDropdown();

        clearInterval(g_setIntervalId);
        $.cookie("current_page", "branch");
        $(document).trigger("reload-branch");
        document.getElementById("contact-buttons-bar").style.display = 'block';
    }

    function SwitchToDashboard() {
        var child_page = document.getElementById("child-page");
        var addComputingBar = function () {

            var computingModal = document.querySelector('#main > div.wrapper > div.computingModal');

            if (computingModal === null) {
                let wrapper = document.querySelector('#main > div.wrapper');
                let computingHtml = '<div class="computingModal">' +
                    '<div class="loading-bar"></div>' +
                    '</div>';

                wrapper.insertAdjacentHTML('afterbegin', computingHtml);
            }
        };

        pageInit();
        addComputingBar();
        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = dashboard_page;
        $.cookie("current_page", "dashboard");
        $(document).trigger("reload-dashboard");
        document.getElementById("contact-buttons-bar").style.display = 'none';
    }

    function SwitchToSettingDashboard() {
        var child_page = document.getElementById("child-page");

        pageInit();
        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = setting_frame;
        $.cookie("current_page", "setting-dashboard");
        $(document).trigger("reload-setting-frame-dashboard");
        document.getElementById("contact-buttons-bar").style.display = 'none';
    }

    function SwitchToSettingGroup() {
        var child_page = document.getElementById("child-page");

        pageInit();
        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = setting_frame;
        $.cookie("current_page", "setting-group");
        $(document).trigger("reload-setting-frame-group");
        document.getElementById("contact-buttons-bar").style.display = 'none';
    }

    function SwitchToSettingProfile() {
        var child_page = document.getElementById("child-page");

        pageInit();
        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = setting_frame;
        $.cookie("current_page", "setting-profile");
        $(document).trigger("reload-setting-frame-profile");
        document.getElementById("contact-buttons-bar").style.display = 'none';
    }

    function SwitchToSettingDevice() {
        var child_page = document.getElementById("child-page");

        pageInit();

        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = setting_frame;
        $.cookie("current_page", "setting-device");
        $(document).trigger("reload-setting-frame-device");
        document.getElementById("contact-buttons-bar").style.display = 'none';
    }

    function SwitchToSettingThreshold() {
        var child_page = document.getElementById("child-page");

        pageInit();
        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = setting_frame;
        $.cookie("current_page", "setting-threshold");
        $(document).trigger("reload-setting-frame-threshold");
        document.getElementById("contact-buttons-bar").style.display = 'none';
    }

    function SwitchToSettingUser() {
        var child_page = document.getElementById("child-page");

        pageInit();
        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = setting_frame;
        $.cookie("current_page", "setting-user");
        $(document).trigger("reload-setting-frame-user");
        document.getElementById("contact-buttons-bar").style.display = 'none';
    }

    function SwitchToSettingScreenShot() {
        var child_page = document.getElementById("child-page");

        pageInit();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = setting_frame;
        $.cookie("current_page", "setting-screenshot");
        $(document).trigger("reload-setting-frame-screenshot");
        document.getElementById("contact-buttons-bar").style.display = 'none';
    }

    function SwitchToSettingCreateUser() {
        var child_page = document.getElementById("child-page");

        pageInit();
        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = setting_frame;
        $.cookie("current_page", "setting-createuser");
        $(document).trigger("reload-setting-frame-createuser");
        document.getElementById("contact-buttons-bar").style.display = 'none';
    }

    function SwitchToSettingMail() {
        var child_page = document.getElementById("child-page");

        pageInit();
        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = setting_frame;
        $.cookie("current_page", "setting-mail");
        $(document).trigger("reload-setting-frame-mail");
        document.getElementById("contact-buttons-bar").style.display = 'none';
    }

    function SwitchToSettingWidget() {
        var child_page = document.getElementById("child-page");
        pageInit();
        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = setting_frame;
        $.cookie("current_page", "setting-widget");
        $(document).trigger("reload-setting-frame-widget");
        document.getElementById("contact-buttons-bar").style.display = 'none';
    }

    function SwitchToWidgetDevice(e, event_table, clickFlag, deviceInfo, refreshFlag, deviceData, oobDirector) {
        var data = null;
        var data = (Boolean($(e.target).attr('data-button')) === true) ?
            JSON.parse($(e.target).attr('data-button')) : { devName: $(e.target).val() };
        const child_page = $('#child-page');
        const widgetModal = $('#myModal');
        const specificDevice = $.map(deviceInfo.record, function (item, index) {
            if (item.devName === data.devName)
                return item;
        });

        var initPage = function (deviceData) {
            var width = -1;
            pageInit();
            clearInterval(autoRefresh.id);
            if (clickFlag.deviceLink === false) {
                $(document).trigger("clean_all_tab");
            } else {
                //$('#tab-secondary-continer').empty();
                $('#device-select').val(data.devName);
            }
            $.cookie("current_deviceId", $('#device-select').val());
            if (clickFlag.initTable === false) {
                let initDeviceTable = function () {
                    var $Attach = $(`<thead style = "background-color: ${BG_COLOR_05};"><tr></tr></thead>`);

                    AddHead($Attach, $.map(deviceInfo.item, (element) => {
                        if (element !== 'Owner')
                            return element;
                    }));
                    const more = document.createElement('th');
                    more.textContent = 'More Devices';
                    more.id = 'device-modal';
                    $Attach[0].querySelector('tr').appendChild(more)
                    $('#device-table' + " thead").replaceWith($Attach);
                };
                let cancelBindModalTable = function () {
                    $('#modal-data tbody').off('click', 'td.details-control');
                };
                let changeButton = function () {
                    $('.details-control').addClass('no-content').removeClass('details-control');
                    $('#modal-data').on('draw.dt', function () {
                        $('body').addClass('loading');
                        $('.details-control').addClass('no-content').removeClass('details-control');
                        $('body').removeClass('loading');
                    });
                };
                let addDeviceInfoTableData = function (deviceData) {
                    let suffer = deviceData.overview.BranchName;
                    var td = `<td>${specificDevice[0].ownerName}</td>` +
                        '<td>' + suffer + '</td>' +
                        `<td><i id = 'send-message' class="fa fa-envelope-o btn-link" aria-hidden="true" style="cursor: pointer;"><font face="Arial">Send Message</font></i></td>`;
                    $('#device-table-info > tbody:last-child').append('<tr>' + td + '</tr>');
                };
                let addClock = function () {
                    // UpdateClock
                    function UpdateClock() {
                        clearTimeout(clock);
                        var dt_now = new Date();
                        var hh = dt_now.getHours();
                        var mm = dt_now.getMinutes();
                        var ss = dt_now.getSeconds();

                        if (hh < 10) {
                            hh = "0" + hh;
                        }
                        if (mm < 10) {
                            mm = "0" + mm;
                        }
                        if (ss < 10) {
                            ss = "0" + ss;
                        }
                        $("#myclock").html(hh + ":" + mm + ":" + ss);

                        // set timer
                        clock = setTimeout(UpdateClock, interval_msec);
                    }
                    var clock = 0;
                    var interval_msec = 1000;
                    UpdateClock();
                };
                var initSendMail_Modal = function () {
                    g_get_device_data_payload.Employee = deviceInfo.emp;
                    const subject = $('#subject');
                    const message = $('#content');
                    $('#send-message').click(() => {
                        addOptionBySelectId('emp', g_get_device_data_payload.Employee);
                        $(`#emp`).selectpicker('refresh');
                        $('#send-email-modal').modal('show');
                    });

                    $('#send-email-modal').on('hidden.bs.modal', function () {
                        $('#emp').empty();
                        $('#emp').selectpicker('destroy');
                        $('.alert').hide();
                        subject.val('');
                        message.val('');
                    });

                    $('#send').click((e) => {
                        const failAlert = $('#fail-send-email');
                        const successAlert = $('#success-send-email');
                        const blank = $('#blank-send-email');
                        const payload = {
                            Message: isEmpty(message.val()) ? '' : message.val(),
                            Subject: subject.val(),
                            Id: $.map($('#emp').val(), (element) => {
                                return Number(element);
                            })
                        };
                        var SendSuccess = function (response) {
                            var messageObj = JSON.parse(response);
                            successAlert.html(messageObj.Response);
                            setTimeout(() => {
                                successAlert.show();
                                blank.show();
                                $('body').removeClass('loading');
                            }, 100);

                        };
                        var SendFail = function (response) {
                            var messageObj = JSON.parse(response.responseText);

                            failAlert.html(messageObj.Response);
                            setTimeout(() => {
                                failAlert.show();
                                blank.show();
                                $('body').removeClass('loading');
                            });
                        };
                        $('.alert').hide();
                        if (payload.Id.length === 0) {
                            failAlert.html('Please specify at least one recipient.');
                            setTimeout(() => {
                                failAlert.show();
                                blank.show();
                                $('body').removeClass('loading');
                            });
                        } else if (true === isEmpty(payload.Subject)) {
                            if (true === confirm("Send this message without a subject or text in the body?")) {
                                let promise;
                                let APICaller = API();
                                $('body').addClass('loading');
                                promise = APICaller.POST('DashboardAPI/Widget/SendEmail', payload);
                                promise.done(SendSuccess);
                                promise.fail(SendFail);
                            }
                        } else {
                            let promise;
                            let APICaller = API();
                            $('body').addClass('loading');
                            promise = APICaller.POST('DashboardAPI/Widget/SendEmail', payload);
                            promise.done(SendSuccess);
                            promise.fail(SendFail);
                        }

                    });
                };
                child_page.append(widget_device_page);
                child_page.append(send_email_modal);
                child_page.append(overview);
                initDeviceTable();
                addDeviceInfoTableData(deviceData);
                cancelBindModalTable();
                changeButton();
                addClock();
                initSendMail_Modal();
                clickFlag.initTable = true;
                widgetModal.off('hidden.bs.modal');
                $(window).resize(function () {
                    var width = $(window).width();
                    if (width < 762) {
                        $('#clock-card').hide();
                    } else {
                        let clock = document.getElementById('clock-card');
                        if (!clock) {
                            $(window).off('resize');
                            return;
                        }

                        if (clock.style.display === 'none') {
                            clock.style.display = '';
                        }
                    }
                });

            } else if (refreshFlag !== true) {
                $('#device-table tbody').find('tr').remove();
            }

            widgetModal.modal("hide");
            width = $(window).width();

            if (width < 762) {
                $('#clock-card').hide();
            } else {
                $('#clock-card').show();
            }
        };

        var addDeviceTableRowData = function () {
            specificDevice.forEach((element) => {
                let td = ((!element.alias) ? `<td>${element.devName}</td>` : `<td>${element.alias}</td>`) +
                    ((element.storageSN === '') ? '' : `<td>${element.storageSN}</td>`) +
                    `<td>${element.value}</td>` +
                    `<td>${timeConverter(element.time)}<td></td></td>`;

                $('#device-table > tbody:last-child').append('<tr>' + td + '</tr>');
            });
        };

        initPage(deviceData);

        if (refreshFlag !== true) {
            addDeviceTableRowData();
        }
        
        const device = deviceManager.getDevice(deviceData.detail.DevName);
        device.update(deviceData.overview);
        
        oobDirector.setUp({
            ooblist: device.getOOBlist(),
            devName: device.getName()
        });

        initDeviceInfoPage(deviceData, deviceInfo, clickFlag, event_table, oobDirector);

        $('#device-modal').click(() => {
            widgetModal.modal("show");
        });

        clearInterval(g_setIntervalId); //remove auto flash
        document.getElementById("contact-buttons-bar").style.display = 'block';
    }

    function SwithchToSettingCustomizedMap(){
        var child_page = document.getElementById("child-page");
        pageInit();
        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = setting_frame;
        $.cookie("current_page", "setting-customized-map");
        $(document).trigger("reload-setting-frame-customized-map");
        document.getElementById("contact-buttons-bar").style.display = 'none';
    }

    function SwithchToSettingDeviceData (){
        var child_page = document.getElementById("child-page");
        pageInit();
        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = setting_frame;
        $.cookie("current_page", "setting-device-data");
        $(document).trigger("reload-setting-frame-device-data");
        document.getElementById("contact-buttons-bar").style.display = 'none'; 
    }

    function SwitchToSettingApiKey(){
        var child_page = document.getElementById("child-page");
        pageInit();
        remvoeBSDropdown();
        clearInterval(g_setIntervalId);
        child_page.innerHTML = setting_frame;
        $.cookie("current_page", "setting-api-key");
        $(document).trigger("reload-setting-frame-api-key");
    }

    return {
        ToEvent: SwitchToEvent,
        ToAlbum: SwitchToAlbum,
        ToGroup: SwitchToGroup,
        ToDashboard: SwitchToDashboard,
        ToSettingDashboard: SwitchToSettingDashboard,
        ToSettingGroup: SwitchToSettingGroup,
        ToSettingProfile: SwitchToSettingProfile,
        ToSettingDevice: SwitchToSettingDevice,
        ToSettingThreshold: SwitchToSettingThreshold,
        ToSettingUser: SwitchToSettingUser,
        ToSettingScreenShot: SwitchToSettingScreenShot,
        ToSettingCreateUser: SwitchToSettingCreateUser,
        ToSettingMail: SwitchToSettingMail,
        ToSettingWidget: SwitchToSettingWidget,
        ToWidgetDevice: SwitchToWidgetDevice,
        ToSettingCustomizedMap: SwithchToSettingCustomizedMap,
        ToSettingDeviceData: SwithchToSettingDeviceData,
        ToSettingApiKey: SwitchToSettingApiKey
    };
}