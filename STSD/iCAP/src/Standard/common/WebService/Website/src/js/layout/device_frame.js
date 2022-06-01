import device_info_page from "../../html/pages/device_info.html";
import device_analyzer_page from "../../html/pages/device_analyzer.html";
import { SwitchPage } from "../library/switch_page";

/**
 * Change the primary container content to device information page
 */
function SwitchToInfoPage() {
    $('body').addClass('loading');
    $('#child-page').empty();
    var secondary_tab = document.getElementsByClassName("tab-secondary");
    var child_page = document.getElementById("child-page");
    child_page.innerHTML = device_info_page;
    $(secondary_tab[0]).show();
    $(document).trigger("reload-device-info", $.cookie('current_dev'));
}

/**
 * Change the primary container content to iAnalyzer page.
 */
function SwitchToiAnalyzerPage() {
    $('body').addClass('loading');
    var secondary_tab = document.getElementsByClassName("tab-secondary");
    var child_page = document.getElementById("child-page");
    child_page.innerHTML = device_analyzer_page;
    $(secondary_tab[0]).hide();
    $(document).trigger("reload-device-analyzer", $.cookie('current_dev'));
}

/**
 * Render primary tabs
 * @param {string} store_token The identity token
 * @param {string} devName Device name
 */
function RenderPrimaryTab(store_token, devName, branchId) {
    const DeviceInfoWrapper = document.createElement('div');
    DeviceInfoWrapper.classList.add('device-navbar-wrapper');

    var secondary_tab = document.getElementsByClassName("tab-secondary");
    secondary_tab[0].style.display = "block";

    var tab_primary_container = document.getElementById("tab-primary-continer");
    var tab_secondary_container = document.getElementById("tab-secondary-continer");

    var group_Tab = $Tab({
        name: "Group"
    }).getTab();
    group_Tab.addEventListener("click", switchToGroupPage);
    DeviceInfoWrapper.appendChild(group_Tab);
    DeviceInfoWrapper.appendChild($Tab().getDivider());

    var groupList_Tab = $Tab({
        name:'',
        className: 'fa fa-exchange'
    }).getTab();
    var group_select_obj = document.createElement("select");

    group_select_obj.setAttribute('id', 'group-select');

    group_select_obj.onchange = function () {
        $('body').addClass('loading');
        $.cookie('current_group', this.value);
        $('#device-select').empty();
        var promise = $.ajax({
            type: 'GET',
            url: 'BranchAPI/GetDeviceList',
            async: true,
            crossDomain: true,
            headers: {
                'token': $.cookie('token'),
                'branchId': $.cookie('current_group')
            },
        });
        promise.done(makeDeviceSelect).done(() => {
            if ($('#device-select').val()) {
                $.cookie('current_dev', $('#device-select').val());
                $.cookie("current_deviceId", $('#device-select').val());
                SwitchToInfoPage();
            } else {
                $.cookie('current_dev', '');
                $('#child-page').empty();
                $('#tab-secondary-continer').empty();
                $('.tab-secondary').hide();
                $('body').removeClass('loading');
            }
        });
    };

    groupList_Tab.appendChild(group_select_obj);

    DeviceInfoWrapper.appendChild(groupList_Tab);
    DeviceInfoWrapper.appendChild($Tab().getDivider());


    var dev_Tab = $Tab({
        name: 'Device'
    }).getTab();
    dev_Tab.addEventListener("click", SwitchToInfoPage);
    DeviceInfoWrapper.appendChild(dev_Tab);
    DeviceInfoWrapper.appendChild($Tab().getDivider());

    var devList_Tab = $Tab({
        name: '',
        className: 'fa fa-exchange'
    }).getTab();

    var select_obj = document.createElement("select");
    select_obj.setAttribute('id', 'device-select');
    select_obj.onchange = function () {
        $('body').addClass('loading');
        $.cookie("current_page", "device");
        $.cookie("current_dev", $('#device-select').val());
        $.cookie("current_deviceId", $('#device-select').val());
        SwitchToInfoPage();
    }
    devList_Tab.appendChild(select_obj);

    DeviceInfoWrapper.appendChild(devList_Tab);
    DeviceInfoWrapper.appendChild($Tab().getDivider());

    var pri_info_Tab = $Tab({
        name: 'Information',
        className: 'fa fa-address-card'
    }).getTab();
    pri_info_Tab.addEventListener("click", SwitchToInfoPage);
    DeviceInfoWrapper.appendChild(pri_info_Tab);
    DeviceInfoWrapper.appendChild($Tab().getDivider());

    var pri_analy_Tab = $Tab({
        name: "iAnalyzer",
        className: 'fa fa-hdd-o'
    }).getTab();
    pri_analy_Tab.addEventListener("click", SwitchToiAnalyzerPage);
    DeviceInfoWrapper.appendChild(pri_analy_Tab);
    DeviceInfoWrapper.appendChild($Tab().getDivider());

    var promise = $.ajax({
        type: 'GET',
        url: 'BranchAPI/GetList',
        async: true,
        crossDomain: true,
        headers: {
            'token': $.cookie('token')
        },
    });
    tab_primary_container.appendChild(DeviceInfoWrapper)
    promise.done(toToWhenGetGroupSuccess);
}

function switchToGroupPage() {
    $('#child-page').empty();
    SwitchPage().ToGroup();
}

function toToWhenGetGroupSuccess(response) {
    var parsed_data = JSON.parse(response);
    makeGroupSelect(parsed_data.List);
    $('#group-select').val($.cookie('current_group'));

    var promise = $.ajax({
        type: 'GET',
        url: 'BranchAPI/GetDeviceList',
        async: true,
        crossDomain: true,
        headers: {
            'token': $.cookie('token'),
            'branchId': $.cookie('current_group')
        },
    });

    promise.done(makeDeviceSelect).done(function () { $('#device-select').val($.cookie('current_dev')); });
}

function makeDeviceSelect(response) {
    if (response != undefined) {
        var parsed_data = JSON.parse(response);
        parsed_data.DeviceList.sort(function (a, b) {
            return 1 * (((!!a.Alias) ? a.Alias : a.Name).toString() < ((!!b.Alias) ? b.Alias : b.Name).toString() ? -1 : (((!!a.Alias) ? a.Alias : a.Name).toString() > ((!!b.Alias) ? b.Alias : b.Name).toString() ? 1 : 0));
        });
        var device_select = document.getElementById('device-select');
        parsed_data.DeviceList.forEach(element => {
            var option_obj = document.createElement("option");
            option_obj.value = element.Name;
            if (element.Alias) {
                option_obj.textContent = element.Alias;
            } else {
                option_obj.textContent = element.Name;
            }
            device_select.appendChild(option_obj);
        });
    }
}

function makeGroupSelect(option) {
    var group_select = document.getElementById('group-select');
    option.forEach(element => {
        var option = document.createElement('option');
        option.value = element.Id;
        option.textContent = element.Name;
        group_select.appendChild(option);
    });
}

$(document).on("reload-device", function (event, devName, branchId) {
    /**
     * To Do: Check token and redirect to login page when get wrong token from cookie.
     */
    if ($.cookie('current_dev')) {
        var store_token = $.cookie('token');
        $(document).trigger("clean_all_tab");
        RenderPrimaryTab(store_token, devName, branchId);
        SwitchToInfoPage();
    } else {
        RenderPrimaryTab($.cookie('token'), devName, branchId);
        $('.tab-secondary').hide();
    }
});