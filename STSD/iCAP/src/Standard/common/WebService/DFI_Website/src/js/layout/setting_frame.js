import PROFILE_SETTING from "../../html/pages/setting_profile.html";
import DEVICE_SETTING from "../../html/pages/setting_device.html";
import THRESHOLD_SETTING from "../../html/pages/setting_threshold.html";
import BRANCH_SETTING from "../../html/pages/setting_branch.html";
import USERCONTROL_SETTING from "../../html/pages/setting_usercontrol.html";
import USERCREATE_SETTING from "../../html/pages/setting_usercreate.html";
import MAIL_SETTING from "../../html/pages/setting_mail.html";
import WIDGET_SETTING from "../../html/pages/setting_widget.html";
import DASHBOARD_SETTING from "../../html/pages/setting_dashboard.html";
import GROUP_SETTING from "../../html/pages/setting_group.html";
import CUSTOMIZED_MAP_SETTING from "../../html/pages/setting_customized_map.html";
import DEVICE_DATA_SETTING from '../../html/pages/setting_device_data.html';
import API_KEY_SETTING from "../../html/pages/setting_api_key.html";

function SwitchToDashboardSetting() {
    $.cookie("current_page", "setting-dashboard");
    var body = document.getElementById("setting-body");
    body.innerHTML = DASHBOARD_SETTING;
    require("../pages/setting_dashboard");
    $(document).trigger("reload-setting-dashboard");
    //$('.nav-tabs li:eq(1) a').tab('show');
}

function SwitchToGroupSetting() {
    $.cookie("current_page", "setting-group");
    var body = document.getElementById("setting-body");
    body.innerHTML = GROUP_SETTING;
    require("../pages/setting_group");
    $(document).trigger("reload-setting-group");
}

function SwitchToProfileSetting() {
    $.cookie("current_page", "setting-profile");
    var body = document.getElementById("setting-body");
    body.innerHTML = PROFILE_SETTING;
    require("../pages/setting_profile");
    $(document).trigger("reload-setting-profile");
    //$('.nav-tabs li:eq(1) a').tab('show');
}

function SwitchToDeviceSetting() {
    $.cookie("current_page", "setting-device");
    var body = document.getElementById("setting-body");
    body.innerHTML = DEVICE_SETTING;
    require("../pages/setting_device");
    $(document).trigger("reload-setting-device");
    // $('.nav-tabs li:eq(2) a').tab('show');
}

function SwitchToThresholdSetting() {
    $.cookie("current_page", "setting-threshold");
    var body = document.getElementById("setting-body");
    body.innerHTML = THRESHOLD_SETTING;
    require("../pages/setting_threshold");
    $(document).trigger("reload-setting-threshold");
    //$('.nav-tabs li:eq(3) a').tab('show');
}

function SwitchToBranchSetting() {
    var body = document.getElementById("setting-body");
    body.innerHTML = BRANCH_SETTING;
}

function SwitchToUserControlSetting() {
    $.cookie("current_page", "setting-user");
    var body = document.getElementById("setting-body");
    body.innerHTML = USERCONTROL_SETTING;
    require("../pages/setting_usercontrol");
    $(document).trigger("reload-setting-usercontrol");
    //$('.nav-tabs li:eq(4) a').tab('show');
}

function SwitchToScreenShotSetting() {
    $.cookie("current_page", "setting-screenshot");
    var body = document.getElementById("setting-body");
    body.innerHTML = SCREENSHOT_SETTING;
    require("../pages/setting_screenshot");
    $(document).trigger("reload-setting-screenshot");
    //$('.nav-tabs li:eq(4) a').tab('show');
}

function SwitchToUserCreateSetting() {
    $.cookie("current_page", "setting-createuser");
    var body = document.getElementById("setting-body");
    body.innerHTML = USERCREATE_SETTING;
    require("../pages/setting_usercreate");
    $(document).trigger("reload-setting-usercreate");
    //$('.nav-tabs li:eq(5) a').tab('show');
}

function SwitchToWidgetSetting() {
    $.cookie("current_page", "setting-widget");
    var body = document.getElementById("setting-body");
    body.innerHTML = WIDGET_SETTING;
    require("../pages/setting_widget");
    $(document).trigger("reload-setting-widget");

}

function SwitchToMailSetting() {
    $.cookie("current_page", "setting-mail");
    var body = document.getElementById("setting-body");
    body.innerHTML = MAIL_SETTING;
    require("../pages/setting_mail");
    $(document).trigger("reload-setting-mail");
    //$('.nav-tabs li:eq(6) a').tab('show');
}

function SwitchToCustomizedMapSetting() {
    $.cookie("current_page", "setting-customized-map");
    var body = document.getElementById("setting-body");
    body.innerHTML = CUSTOMIZED_MAP_SETTING;
    require("../pages/setting_customized_map");
    $(document).trigger("reload-setting-customized-map");
}

function SwitchToDeviceDataSetting() {
    $.cookie("current_page", "setting-device-data");
    var body = document.getElementById("setting-body");
    body.innerHTML = DEVICE_DATA_SETTING;
    require("../pages/setting_device_data");
    $(document).trigger("reload-setting-device-data");
}

function SwitchToSettingApiKeySetting() {
    $.cookie("current_page", "setting-api-key");
    var body = document.getElementById("setting-body");
    body.innerHTML = API_KEY_SETTING;
    require("../pages/setting_api_key");
    $(document).trigger("reload-setting-api-key");
}

// function SwitchToImagesSetting()
// {
//     $.cookie("current_page", "images");
//     var body = document.getElementById("setting-body");
//     body.innerHTML = IMAGES_SETTING;
//     require("../pages/setting_images");
//     $(document).trigger("reload-setting-images");
//     $('.nav-tabs li:eq(7) a').tab('show');
// }

function RebindBootstrapTabEvent() {
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href"); // activated tab
        switch (target) {
            // case '#dashboard':
            //     SwitchToDashboardSetting();
            //     break;
            case '#dashboard':
                SwitchToDashboardSetting();
                break;
            case '#group':
                SwitchToGroupSetting();
                break;
            case '#profile':
                SwitchToProfileSetting();
                break;
            case '#device':
                SwitchToDeviceSetting();
                break;
            case '#threshold':
                SwitchToThresholdSetting();
                break;
            case '#branch':
                SwitchToBranchSetting();
                break;
            case '#usercontrol':
                SwitchToUserControlSetting();
                break;
            case '#usercreate':
                SwitchToUserCreateSetting();
                break;
            case '#mail':
                SwitchToMailSetting();
                break;
            case '#widget':
                SwitchToWidgetSetting();
                break;
            case '#customized-map':
                SwitchToCustomizedMapSetting();
                break;
            case '#device-data':
                SwitchToDeviceDataSetting();
                break;
            case '#api-key':
                SwitchToSettingApiKeySetting();
                break;
        }
    });
}
$(document).on("reload-setting-frame-dashboard", function () {
    $(document).trigger("clean_all_tab");
    RebindBootstrapTabEvent();
    $('.nav-tabs a[href="#dashboard"]').tab('show');
});

$(document).on("reload-setting-frame-group", function () {
    $(document).trigger("clean_all_tab");
    RebindBootstrapTabEvent();
    $('.nav-tabs a[href="#group"]').tab('show');
});

$(document).on("reload-setting-frame-profile", function () {
    $(document).trigger("clean_all_tab");
    RebindBootstrapTabEvent();
    $('.nav-tabs a[href="#profile"]').tab('show');
});

$(document).on("reload-setting-frame-device", function () {
    $(document).trigger("clean_all_tab");
    RebindBootstrapTabEvent();
    $('.nav-tabs a[href="#device"]').tab('show');
});

$(document).on("reload-setting-frame-threshold", function () {
    $(document).trigger("clean_all_tab");
    RebindBootstrapTabEvent();
    $('.nav-tabs a[href="#threshold"]').tab('show');
});

$(document).on("reload-setting-frame-user", function () {
    $(document).trigger("clean_all_tab");
    RebindBootstrapTabEvent();
    $('.nav-tabs a[href="#usercontrol"]').tab('show');
});

$(document).on("reload-setting-frame-createuser", function () {
    $(document).trigger("clean_all_tab");
    RebindBootstrapTabEvent();
    $('.nav-tabs a[href="#usercreate"]').tab('show');
});

$(document).on("reload-setting-frame-mail", function () {
    $(document).trigger("clean_all_tab");
    RebindBootstrapTabEvent();
    $('.nav-tabs a[href="#mail"]').tab('show');
});

$(document).on("reload-setting-frame-widget", function () {
    $(document).trigger("clean_all_tab");
    RebindBootstrapTabEvent();
    $('.nav-tabs a[href="#widget"]').tab('show');
});

$(document).on("reload-setting-frame-customized-map", function () {
    $(document).trigger("clean_all_tab");
    RebindBootstrapTabEvent();
    $('.nav-tabs a[href="#customized-map"]').tab('show');
});

$(document).on("reload-setting-frame-device-data", function () {
    $(document).trigger("clean_all_tab");
    RebindBootstrapTabEvent();
    $('.nav-tabs a[href="#device-data"]').tab('show');
});
$(document).on("reload-setting-frame-api-key", function () {
    $(document).trigger("clean_all_tab");
    RebindBootstrapTabEvent();
    $('.nav-tabs a[href="#api-key"]').tab('show');
});