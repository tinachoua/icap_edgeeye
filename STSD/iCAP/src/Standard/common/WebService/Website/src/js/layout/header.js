import { GetEventCount } from "../library/event_count";
import { SwitchPage } from "../library/switch_page";
import getLogo from "../helpers/logo"
import {
    HEADER_LOGO,
    LEFT_SIDEBAR_BOTTOM_LOGO,
    LEFT_SIDEBAR_TOP_LOGO
} from "../constants/globalVariable"
require('../pages/dashboard');
require('../pages/event');
require('../pages/branch');
require('./device_frame');
require('../pages/device_info');
require('../pages/device_analyzer');
require('./setting_frame');
require('../layout/jsIconMenu');

$(document).on("switch-to-device", function (event, devName) {
    $.cookie("current_page", "device");
    $.cookie("current_dev", devName);
    $.cookie("current_deviceId", devName);
    $(document).trigger("reload-device", devName, $.cookie('current_group'));
});

$(document).on("clean_all_tab", function () {
    if (Boolean($.cookie('current_page')) === true && $.cookie('current_page').match("setting")) {
        $('.tab-primary').hide();
        $('.tab-secondary').hide();
    } else {
        $('.tab-primary').show();
    }
    $("#tab-primary-continer").empty();
    $("#tab-secondary-continer").empty();
});

$(document).on("reload-header", function () {
    function Logout() {
        $(document).trigger("logout");
    }

    var icaplogo = document.getElementById("icaplogo");
    var logoimg = document.getElementById("icaplogo2");
    var logoinno = document.getElementById("innologo");

    var current_page = $.cookie("current_page");
    const SWITCH = SwitchPage();

    icaplogo.src = getLogo(HEADER_LOGO);
    logoimg.src = getLogo(LEFT_SIDEBAR_TOP_LOGO);
    logoinno.src = getLogo(LEFT_SIDEBAR_BOTTOM_LOGO);;

    $("#icaplogo").bind("click", SWITCH.ToDashboard);
    $(".dashboard-btn").bind("click", SWITCH.ToDashboard);
    $(".event-btn").bind("click", SWITCH.ToEvent);
    $(".branch-btn").bind("click", SWITCH.ToGroup);
    $("#setting-dashboard").bind("click", SWITCH.ToSettingDashboard);
    $("#setting-profile").bind("click", SWITCH.ToSettingProfile);
    $('#setting-group').bind('click', SWITCH.ToSettingGroup);
    $("#setting-device").bind("click", SWITCH.ToSettingDevice);
    $("#setting-threshold").bind("click", SWITCH.ToSettingThreshold);
    $("#setting-user").bind("click", SWITCH.ToSettingUser);
    $("#setting-create").bind("click", SWITCH.ToSettingCreateUser);
    $("#setting-mail").bind("click", SWITCH.ToSettingMail);
    $("#setting-widget").bind("click", SWITCH.ToSettingWidget);
    $("#main-logout").bind("click", Logout);
    $("#setting-Screen-Shot").bind("click", SWITCH.ToSettingScreenShot);
    $("#setting-customized-map").bind("click", SWITCH.ToSettingCustomizedMap);
    $("#setting-device-data").bind("click", SWITCH.ToSettingDeviceData);
    $("#setting-api-key").bind("click", SWITCH.ToSettingApiKey);
    GetEventCount();

    if (current_page != null) {
        switch (current_page) {
            case "dashboard":
                SWITCH.ToDashboard();
                break;
            case "event":
                SWITCH.ToEvent();
                break;
            case "branch":
                SWITCH.ToGroup();
                break;
            case "setting-dashboard":
                SWITCH.ToSettingDashboard();
                break;
            case "setting-group":
                SWITCH.ToSettingGroup();
                break;
            case "device":
                $(document).trigger("switch-to-device", $.cookie("current_dev"));
                break;
            // case "setting-dashboard":
            //     SwitchToSettingDashboard();
            //     break;
            case "setting-profile":
                SWITCH.ToSettingProfile();
                break;
            case "setting-device":
                SWITCH.ToSettingDevice();
                break;
            case "setting-threshold":
                SWITCH.ToSettingThreshold();
                break;
            case "setting-threshold":
                SWITCH.ToSettingThreshold();
                break;
            case "setting-user":
                SWITCH.ToSettingUser();
                break;
            case "setting-createuser":
                SWITCH.ToSettingCreateUser();
                break;
            case "setting-mail":
                SWITCH.ToSettingMail();
                break;
            case "setting-widget":
                SWITCH.ToSettingWidget();
                break;
            case "setting-screenshot":
                SWITCH.ToSettingScreenShot();
                break;
            case "setting-customized-map":
                SWITCH.ToSettingCustomizedMap();
                break;
            case "album":
                SWITCH.ToAlbum();
                break;
            case "setting-device-data":
                SWITCH.ToSettingDeviceData();
                break;
            case "setting-api-key":
                SWITCH.ToSettingApiKey();
                break;
        }

    }
    else {
        $.removeCookie("current_page");
        SWITCH.ToDashboard();
    }

    // document.querySelector('#headerAlbum').onclick = () => {
    //     //showAlbum($.cookie("current_deviceId") || '');
    //     SWITCH.ToAlbum();
    // };

    // let screenshot_btn = document.getElementById("a-screenshot");
    // screenshot_btn.onclick = connectScreenShotWebAPI;
});