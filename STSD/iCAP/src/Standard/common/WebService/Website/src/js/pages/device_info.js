require("../components/card");
require("../components/tab");
require("../pages/branch");
import { makeColorsWithoutHeightlight, getHeightLightColor } from "../library/color";
import { API } from "../library/api_handler";
import { makeChart, widget } from "../library/make_chart";
import { fire } from '../library/event_handler';
export var g_AutoRefreshId;
import { GOOGLEMAP_CHART, VECTORMAP_CHART, OPENSTREETMAP_CHART, GAODEMAP_CHART } from '../library/data_mapping';
import '../../css/pages/device_info.css'
import { OOBDirector } from '../library/OOB';
import { NXReboot } from '../library/NXReboot';
import { DeviceManager as deviceManager } from '../DeviceManager';
import { createIndexBtn, createBody } from '../library/common';
import { BG_COLOR_03, FONT_COLOR_A, CHART_BASE_COLOR_D } from "../constants/globalVariable";

import updateStatus from '../library/OOB/helpers/updateStatus'

$(document).on("reload-device-info", function (event, devName) {
    var token, deviceName;
    var sub_tabs;
    var sub_tabs_tooltip;
    var card_cpu_loading_gauge;
    var card_mem_loading_gauge;
    var card_storage_lifespan_bar;
    var card_cpu_loading_line;
    var card_mem_loading_line;
    var card_dev_location;
    var card_mem_tempChart;
    var ext_gauge_max = [
        8000, 260, 220, 400, 500, 100
    ];

    var ext_gauge_min = [
        0, 0, -40, 0, 0, 0
    ];

    var ext_gauge_obj = [];

    const oobDirector = new OOBDirector();
    const rebootDirector = new NXReboot();
    console.log("dev info js")
    /**
     * Render Device Overview
     * @param {string} store_token The identity token
     * @param {string} devName Device name
     */
    function RenderOverview(store_token, devName) {
        console.log("----RenderOverview");
        $.ajax({
            type: 'GET',
            url: 'DeviceInfoAPI/GetOverview?DeviceName=' + devName,
            async: true,
            crossDomain: true,
            headers: {
                'token': store_token
            },
            success: function (response) {
                console.log("----GetOverview?DeviceName");
                function RenderStorageAnalysisData(data) {
                    var storName = [];
                    var storData = [];
                    var chartColor = makeColorsWithoutHeightlight(data.Lifespan.length);
                    const LIFESPAN_THRESHOLD = 150;

                    for (var i = 0; i < data.Lifespan.length; i++) {
                        storName[i] = 'Storage ' + i;
                        if (data.Lifespan[i].Lifespan.length > 0) {
                            storData[i] = data.Lifespan[i].Lifespan[data.Lifespan[i].Lifespan.length - 1].EstimatedDays;
                        }

                        if (storData[i] < LIFESPAN_THRESHOLD) {
                            chartColor[i] = getHeightLightColor();
                        }
                    }

                    card_storage_lifespan_bar = $Card({
                        id: 6,
                        name: "Storage Lifespan",
                        type: "group-bar",
                        width: "col-md-4",
                        label: [''],
                        data: {
                            label: storName,
                            value: storData,
                            name: 'Estimation Lifespan (Days)'
                        },
                        color: chartColor
                    });
                    card_storage_lifespan_bar.put("#stor-lifespan");
                    card_storage_lifespan_bar.render();
                }

                var parsed_data = JSON.parse(response);

                const device = deviceManager.getDevice(devName);
                device.update(parsed_data);

                oobDirector.setUp({
                    ooblist: device.getOOBlist(),
                    devName: device.getName(),
                });
                //console.log("kkkkkk")
                //console.log(device.Status)
                console.log("dev info page");
                rebootDirector.setUp({

                    devName: device.getName()
                });

                if (parsed_data.Alias) {
                    $('#devName').html(parsed_data.Alias);
                } else {
                    $('#devName').html(devName);
                }

                document.getElementById("overview-os").textContent = parsed_data.OS;
                document.getElementById("overview-cpu").textContent = parsed_data.CPU;
                document.getElementById("overview-mem").textContent = parsed_data.MEMCap + " GB";
                document.getElementById("overview-stor").textContent = parsed_data.StorCap + " GB";
                document.getElementById("overview-status").textContent = parsed_data.Status;

                RenderStorageAnalysisData(parsed_data);

                if (parsed_data.GPU && parsed_data.GPU.length > 0) {
                    let target_dom = document.getElementById("subpanel-GPU-data");
                    let gpuInformationData = JSON.parse(parsed_data.GPU);
                    let gpuInformationTamplate = `
                    <tr><td><b>Name</b></td><td >${gpuInformationData.Name}</td> </tr>
                    <tr><td><b>GPU</b></td><td >${gpuInformationData.Arch}</td></tr>    
                    <tr><td><b>CUDA Drver Version</b></td><td >${gpuInformationData.DriverVer}</td> </tr>
                    <tr><td><b>Compute Capability</b></td><td >${gpuInformationData.ComputeCap}</td></tr>
                    <tr><td><b>CUDA Cores</b></td> <td >${gpuInformationData.CoreNum}</td></tr>
                    <tr><td><b>Memory Type</b></td><td >${gpuInformationData.MemType}</td></tr>
                    <tr><td><b>Memory Bus Width</b></td><td >${gpuInformationData.MemBusWidth}</td></tr>
                    <tr><td><b>Memory Size</b></td><td >${gpuInformationData.MemSize}</td></tr>
                    <tr><td><b>Memory Bandwitdh</b></td><td >${gpuInformationData.MemBandWidth}</td></tr>
                    <tr><td><b>GPU Clock</b></td><td >${gpuInformationData.Clock}</td></tr>
                    <tr><td><b>Memory Clock</b></td><td >${gpuInformationData.MemClock}</td></tr>
                    `
                    target_dom.innerHTML = gpuInformationTamplate;
                }
            },
            error: function (response) {
            }
        });

        $.ajax({
            type: 'GET',
            url: 'DeviceAPI/GetImg',
            async: true,
            crossDomain: true,
            headers: {
                'token': $.cookie('token'),
                "devName": devName
            },
            success: function (response) {

                var ret = JSON.parse(response);
                $("#devimg").attr('src', ret.Response);
            },
            error: function (response) {
            }
        });
    }

    /**
     * Render Device Information View
     * @param {string} store_token The identity token
     * @param {string} devName Device name
     */
    function RenderInformationView(store_token, devName) {
        console.log("get d RenderInformationView");
        $.ajax({
            type: 'GET',
            url: 'DeviceInfoAPI/GetDetail?DeviceName=' + devName,
            async: true,
            crossDomain: true,
            headers: {
                'token': store_token
            },
            success: function (response) {
                /**
                 * Render secondary tabs to device information.
                 */
                function RenderSecondaryTab(sub_tabs) {
                    /**
                     * Switch the secondary tab hover class
                     */
                    function SwitchSubPage() {
                        /**
                         * Switch the secondary tab pages
                         * @param {string} pageTag The identity tag for the page
                         */
                        function SwitchTo(pageTag) {
                            var tabs = $('[class*="subpanel"]');
                            if (card_mem_tempChart) {
                                card_mem_tempChart.destroy();
                                card_mem_tempChart = undefined;
                            }
                            for (var i = 0; i < tabs.length; i++) {
                                $(tabs[i]).hide();
                            }
                            tabs = document.getElementsByClassName("subpanel-" + pageTag);
                            for (var i = 0; i < tabs.length; i++) {
                                $(tabs[i]).show();
                                if (pageTag === "Location") {
                                    let RenderDeviceMAP = function (parsed_data) {
                                        var data = parsed_data.markers[0];
                                        var lat = data.position.lat;
                                        var lng = data.position.lng;
                                        var mapDataFormat = {
                                            centerLat: lat,
                                            centerLng: lng,
                                            value: parsed_data.markers
                                        };
                                        var loc_long = document.getElementById("loc-long");
                                        var loc_lat = document.getElementById("loc-lat");
                                        var loc_status = document.getElementById("loc-status");
                                        var map = document.getElementById('subpanel-location');
                                        var button = map.parentElement.getElementsByTagName('button');
                                        var settingObj = {
                                            target: document.getElementById('subpanel-location'),
                                            widgetId: 3,
                                            chartNameId: null,
                                            title: 'Device Location',
                                            type: '',
                                            data: mapDataFormat,
                                            clickFlag: false,
                                            dragFlag: false,
                                            dashboardId: null,
                                            width: 'col-md-12 pad0'
                                        };
                                        var setChartType = function (settingObj, mapId) {
                                            switch (String(mapId)) {
                                                case GOOGLEMAP_CHART:
                                                    settingObj.type = 'google map';
                                                    break;
                                                case VECTORMAP_CHART:
                                                    settingObj.type = 'vector map';
                                                    break;
                                                case OPENSTREETMAP_CHART:
                                                    settingObj.type = 'open street map';
                                                    break;
                                                case GAODEMAP_CHART:
                                                    settingObj.type = 'gaode map';
                                                    break;
                                            }
                                        };

                                        while (button.length != 0) {
                                            button[0].parentNode.removeChild(button[0]);
                                        }
                                        var buttonArray = [];
                                        parsed_data.mapList.forEach((element, idx) => {
                                            let buttonElement = document.createElement('button');

                                            buttonElement.innerHTML = element.Name;
                                            buttonElement.value = element.Id;

                                            if (parsed_data.mapId === element.Id) {
                                                buttonElement.classList.add('button-click');
                                                buttonElement.style.color = FONT_COLOR_A;
                                                buttonElement.style.background = BG_COLOR_03;
                                            }

                                            if (idx !== 0) {
                                                buttonElement.style.marginLeft = '10px';
                                            }
                                            buttonElement.addEventListener('mousedown', (event) => {
                                                var removeClick = function (button) {
                                                    var buttonArray = Array.from(button);
                                                    buttonArray.some((item) => {
                                                        if (item.classList.contains('button-click')) {
                                                            item.classList.remove('button-click');
                                                            item.style.color = BG_COLOR_03;
                                                            item.style.background = FONT_COLOR_A;
                                                            return true;
                                                        }
                                                    });
                                                };
                                                var mapId = event.target.value;
                                                let widgetBody = settingObj.target.getElementsByClassName('panel-body')[0];
                                                while (widgetBody.hasChildNodes()) {
                                                    widgetBody.removeChild(widgetBody.firstChild);
                                                }

                                                removeClick(button);
                                                event.target.classList.add('button-click');
                                                event.target.style.color = FONT_COLOR_A;
                                                event.target.style.background = BG_COLOR_03;
                                                if (mapId === GAODEMAP_CHART) {
                                                    settingObj.type = 'gaode map';
                                                    let widgetHandler = widget();
                                                    widgetHandler.makeWidgetBody(widgetBody, settingObj, null, null);
                                                } else {
                                                    setChartType(settingObj, mapId);
                                                    makeChart(settingObj);
                                                }
                                                APICaller.PUT('EmployeeAPI/SetCommonMap', Number(mapId));
                                            });
                                            map.insertAdjacentElement('beforebegin', buttonElement);
                                            buttonArray.push(buttonElement);
                                        });

                                        var widgetHandler = widget();
                                        setChartType(settingObj, parsed_data.mapId);
                                        if (parsed_data.mapId == GAODEMAP_CHART) {
                                            widgetHandler.makeWidgetBody(map.getElementsByClassName('panel-body')[0], settingObj, null, null);
                                        } else {
                                            makeChart(settingObj);
                                        }

                                        loc_long.textContent = "Longitude: " + lng;
                                        loc_lat.textContent = "Latitude: " + lat;
                                        loc_status.textContent = "Status: " + data.status;
                                    };

                                    let APICaller = API();
                                    let promise = APICaller.GET('DeviceInfoAPI/GetLocation?DeviceName=' + devName);

                                    promise.done((response) => {
                                        RenderDeviceMAP(JSON.parse(response));
                                    });
                                }

                                if (pageTag === "OOB") {
                                    const ooblist = deviceManager.getOOBlist(devName);
                                    const tableBody = document.querySelector('.subpanel-OOB tbody');
                                    const oobService = document.querySelector('#OOBTab-service');

                                    // document.querySelector('#oob-led').src = icon_status_on
                                    // document.querySelector('#os-heartbeat-led').src = icon_status_on

                                    oobService.appendChild(oobDirector.element);

                                    tableBody.innerHTML = '';

                                    createIndexBtn({
                                        ooblist,
                                        onclick: async function (i) {

                                            const sn = ooblist[i].SN;
                                            oobDirector.setTargetSN(sn);
                                            tableBody.innerHTML = '';

                                            try {

                                                updateStatus({
                                                    element: document.querySelector('#oob-led'),
                                                    status: deviceManager.getOOBStatus({ devName, sn }),
                                                })

                                                const { data: sphereStatus } = await deviceManager.getSphereStatus({ devName, sn: sn })
                                                const { data: infoData } = await deviceManager.getOOBInfo({ devName, sn: sn })
                                                tableBody.appendChild(createBody({ ...infoData, ...sphereStatus }, sn));

                                                updateStatus({
                                                    element: document.querySelector('#os-heartbeat-led'),
                                                    status: await deviceManager.getOSHeartbeat({ devName, sn }),
                                                })

                                            } catch (error) {
                                                return;
                                            }
                                        }
                                    })
                                }
                            }

                            clearTimeout(g_AutoRefreshId);
                            switch (pageTag) {
                                case sub_tabs[0]:
                                    if (card_cpu_loading_gauge) {
                                        card_cpu_loading_gauge.render();
                                        card_mem_loading_gauge.render();
                                        card_storage_lifespan_bar && card_storage_lifespan_bar.render();
                                    }
                                    oobDirector.root && oobDirector.root.appendChild(oobDirector.element);
                                    break;
                                case sub_tabs[2]:
                                    card_cpu_loading_line.render();
                                    break;
                                case sub_tabs[4]:
                                    card_mem_loading_line && card_mem_loading_line.render();
                                    const getTempDistribution = $.ajax({
                                        type: 'GET',
                                        url: `DeviceInfoAPI/${devName}/Memory/Temperature-Distribution`,
                                        async: true,
                                        crossDomain: true,
                                        headers: {
                                            token: $.cookie('token'),
                                        }
                                    });
                                    getTempDistribution.done((respnse) => {
                                        var labels = JSON.parse(respnse).Labels;
                                        var TempAnalysis = JSON.parse(respnse).Value;
                                        let tempChart = document.getElementById('tempChart');
                                        tempChart.height = '100%';

                                        function makeTempBar(labels, TempAnalysis) {
                                            var data = {
                                                labels: labels,
                                                datasets: [{
                                                    label: 'probability',
                                                    data: TempAnalysis,
                                                    backgroundColor: CHART_BASE_COLOR_D,
                                                    borderWidth: 1
                                                }],
                                            }
                                            var options = {
                                                responsive: true,
                                                legend: {
                                                    display: false
                                                },
                                                animation: {
                                                    onProgress: function (animation) {
                                                        tempChart.value = animation.animationObject.currentStep / animation.animationObject.numSteps;
                                                    }
                                                },
                                                scales: {
                                                    xAxes: [{
                                                        barPercentage: 0.5,
                                                        barThickness: 35,
                                                        maxBarThickness: 35,
                                                        minBarLength: 2,
                                                        gridLines: {
                                                            offsetGridLines: true,
                                                            display: false
                                                        },
                                                        scaleLabel: {
                                                            labelString: 'TempAnalysis',
                                                            display: true,
                                                        }
                                                    }],
                                                    yAxes: [{
                                                        gridLines: {
                                                            display: false
                                                        },
                                                        scaleLabel: {
                                                            display: true,
                                                            labelString: 'times(Today)',
                                                        },
                                                    }],

                                                },
                                                layout: {
                                                    padding: {
                                                        top: 25
                                                    }
                                                }
                                            }
                                            return new Chart(tempChart, {
                                                type: 'bar',
                                                data: data,
                                                options: options
                                            });
                                        }
                                        card_mem_tempChart = makeTempBar(labels, TempAnalysis);
                                    });
                                    getTempDistribution.fail(() => {
                                    })
                                    break;
                                case 'External':
                                    var startRefresh = function () {
                                        var time = 1000;
                                        var current_page = $.cookie("current_page");
                                        var store_token = $.cookie('token');
                                        if (current_page == "device") {
                                            if ($("#External").is(':visible')) {
                                                var RefreshEXTData = function (store_token, devName) {
                                                    console.log("get d RefreshEXTData");
                                                    $.ajax({
                                                        type: 'GET',
                                                        url: 'DeviceInfoAPI/GetDetail?DeviceName=' + devName,
                                                        async: true,
                                                        crossDomain: true,
                                                        headers: {
                                                            'token': store_token
                                                        },
                                                        global: false,
                                                        success: function (response) {
                                                            function UpdateEXTInfo(parsed_data) {
                                                                parsed_data.EXT && parsed_data.EXT.forEach(function (element, idx, array) {
                                                                    ext_gauge_obj[idx].data.datasets[0].data[0] = element.Value;
                                                                    ext_gauge_obj[idx].data.datasets[0].data[1] = ext_gauge_max[idx] - element.Value;
                                                                    ext_gauge_obj[idx].options.elements.center.text = element.Value + " " + element.Unit;
                                                                    ext_gauge_obj[idx].update();
                                                                });
                                                            }

                                                            var parsed_data = JSON.parse(response);
                                                            UpdateEXTInfo(parsed_data);
                                                        },
                                                        error: function (response) {
                                                        }
                                                    });
                                                };

                                                g_AutoRefreshId = setTimeout(startRefresh, time);
                                                RefreshEXTData(token, deviceName);
                                            }
                                        }
                                    };
                                    g_AutoRefreshId = startRefresh();
                                    break;
                                case sub_tabs[8]:
                                    break;
                            }
                        }
                        var tabs = document.getElementById('tab-secondary-continer').getElementsByClassName('sub-tab-content');
                        for (var i = 0; i < tabs.length; i++) {
                            tabs[i].classList.remove('hover');
                        }
                        var current_tab = document.getElementById($(this).attr("id"));
                        current_tab.className += " hover";
                        SwitchTo($(this).attr("id"));
                    }

                    var tab_secondary_container = document.getElementById("tab-secondary-continer");

                    $("#tab-secondary-continer").empty();

                    sub_tabs.forEach(function (element, idx, array) {
                        var tab = $Tab({
                            name: element
                        }).getTab();
                        tab.addEventListener("click", SwitchSubPage);
                        tab.setAttribute("class", "sub-tab-content");
                        tab.setAttribute("id", element);
                        tab.setAttribute("data-toggle", "tooltip");
                        tab.setAttribute("data-placement", "bottom");
                        tab.setAttribute("title", sub_tabs_tooltip[idx]);
                        tab_secondary_container.appendChild(tab);
                        if (idx != array.length - 1) {
                            tab_secondary_container.appendChild($Tab().getDivider());
                        }
                    });

                    document.getElementById(sub_tabs[0]).click();
                    //document.getElementById(sub_tabs[2]).click();
                }
                /**
                 * Render Operating System Information View
                 * @param {object} parsed_data JSON object of response data from GetDetail API
                 */
                function RenderOSInfo(parsed_data) {
                    var tab = document.getElementById("subpanel-OS-data");

                    parsed_data.OS.forEach(element => {
                        var tr_root = document.createElement("tr");
                        var td_key = document.createElement("td");
                        var b_key = document.createElement("b");
                        b_key.textContent = element.key;
                        td_key.appendChild(b_key);
                        tr_root.appendChild(td_key);
                        var td_value = document.createElement("td");
                        td_value.textContent = element.value;
                        tr_root.appendChild(td_value);
                        tab.appendChild(tr_root);
                    });
                }
                /**
                 * Render CPU Information View
                 * @param {object} parsed_data JSON object of response data from GetDetail API
                 */
                function RenderCPUInfo(parsed_data) {
                    var tab = document.getElementById("subpanel-CPU-data");
                    var coreCount = parsed_data.CPU[2].value;

                    parsed_data.CPU.forEach(element => {
                        var tr_root = document.createElement("tr");
                        var td_key = document.createElement("td");
                        var b_key = document.createElement("b");
                        var td_value = document.createElement("td");

                        b_key.textContent = element.key;
                        td_key.appendChild(b_key);
                        tr_root.appendChild(td_key);
                        td_value.textContent = element.value;
                        tr_root.appendChild(td_value);
                        tab.appendChild(tr_root);
                    });
                    card_cpu_loading_line = $Card({
                        id: 1,
                        name: "CPU Loading",
                        type: "line",
                        width: "col-md-12, pad0",
                        label: ["Loading (%)"],
                        data: [
                            parsed_data.UsageTime,
                            parsed_data.CPUUsage
                        ]
                    });
                    card_cpu_loading_line.put("#cpu-loading");
                    card_cpu_loading_line.render();

                    $('#frequency-value').text(parsed_data.CPUFreq);
                    $('#fan-value').text(parsed_data.CPUFan);

                    if (coreCount >= 0) {
                        let coreFreq = document.getElementById('tab-core-freq');
                        let coreLoad = document.getElementById('tab-core-loading');
                        let coreTemp = document.getElementById('tab-core-temp');
                        let coreVoltage = document.getElementById('tab-core-voltage');
                        let coreInfo = {
                            frequency: '',
                            loading: '',
                            temp: '',
                            voltage: ''
                        };

                        for (var i = 0; i < coreCount; i++) {
                            if (!parsed_data.CPUCore.CoreFreq[i]) {
                                break;
                            }
                            let paddingValue = '5px';
                            if (i === 0) {
                                paddingValue = 0;
                            }

                            coreInfo.frequency += `<div style="padding-top: ${paddingValue};">` +
                                `<div style="float:left;line-height: 40px;font-size: 12px;">` +
                                `Core #${i}` +
                                '</div>' +
                                '<div class="text-right">' +
                                '<span id="fan-value" class="text-right" style="font-weight: 700;height: 40px;line-height: 38px;font-size: 40px;">' +
                                `${parsed_data.CPUCore.CoreFreq[i]}` +
                                '</span>' +
                                `<span class="fan-unit">MHz</span>` +
                                '</div>' +
                                '</div>';

                            coreInfo.loading += `<div style="padding-top: ${paddingValue};">` +
                                `<div style="float:left;line-height: 40px;font-size: 12px;">` +
                                `Core #${i}` +
                                '</div>' +
                                '<div class="text-right">' +
                                '<span id="fan-value" class="text-right" style="font-weight: 700;height: 40px;line-height: 38px;font-size: 40px;">' +
                                `${parseFloat(parsed_data.CPUCore.CoreUsage[i].toFixed(1))}` +
                                '</span>' +
                                `<span class="fan-unit">%</span>` +
                                '</div>' +
                                '</div>';

                            coreInfo.temp += `<div style="padding-top: ${paddingValue};">` +
                                `<div style="float:left;line-height: 40px;font-size: 12px;">` +
                                `Core #${i}` +
                                '</div>' +
                                '<div class="text-right">' +
                                '<span id="fan-value" class="text-right" style="font-weight: 700;height: 40px;line-height: 38px;font-size: 40px;">' +
                                `${parsed_data.CPUCore.CoreTemp[i]}` +
                                '</span>' +
                                `<span class="fan-unit">℃</span>` +
                                '</div>' +
                                '</div>';

                            coreInfo.voltage += `<div style="padding-top: ${paddingValue};">` +
                                `<div style="float:left;line-height: 40px;font-size: 12px;">` +
                                `Core #${i}` +
                                '</div>' +
                                '<div class="text-right">' +
                                '<span id="fan-value" class="text-right" style="font-weight: 700;height: 40px;line-height: 38px;font-size: 40px;">' +
                                `${parsed_data.CPUCore.CoreVoltage[i]}` +
                                '</span>' +
                                `<span class="fan-unit">V</span>` +
                                '</div>' +
                                '</div>';

                        }
                        $(coreFreq).append($(coreInfo.frequency));
                        $(coreLoad).append($(coreInfo.loading));
                        $(coreTemp).append($(coreInfo.temp));
                        $(coreVoltage).append($(coreInfo.voltage));
                    }
                }
                /**
                 * Render motherboard information view
                 * @param {object} parsed_data JSON object of response data from GetDetail API
                 */
                function RenderMBInfo(parsed_data) {
                    var tab = document.getElementById("subpanel-MB-data");
                    parsed_data.MB.forEach(element => {
                        var tr_root = document.createElement("tr");
                        var td_key = document.createElement("td");
                        var b_key = document.createElement("b");
                        b_key.textContent = element.key;
                        td_key.appendChild(b_key);
                        tr_root.appendChild(td_key);
                        var td_value = document.createElement("td");
                        td_value.textContent = element.value;
                        tr_root.appendChild(td_value);
                        tab.appendChild(tr_root);
                    });
                }
                /**
                 * Render memory information view
                 * @param {object} parsed_data JSON object of response data from GetDetail API
                 */
                function RenderMEMInfo(parsedData) {
                    devName = parsedData.DevName;
                    var indexBtn = document.getElementById("btn-indx-mem");
                    var memData = JSON.parse(parsedData.MEM);
                    var i = 0;
                    var btnFragment = document.createDocumentFragment();
                    card_mem_loading_line = $Card({
                        id: 2,
                        name: "Memory Loading",
                        type: "line",
                        width: "col-xs-12, pad0",
                        label: ["Loading (%)"],
                        data: [
                            parsed_data.UsageTime,
                            parsed_data.MEMUsage
                        ]
                    });
                    card_mem_loading_line.put("#mem-loading");
                    card_mem_loading_line.render();
                    var switchMemIndex = function (slot, event) {
                        if (event.target.value == 1) {
                            return;
                        }
                        for (var btn of indexBtn.getElementsByTagName('button')) {
                            btn.value = 0;
                        }
                        var memInfoTable = document.getElementById('subpanel-mem-info-table');
                        event.target.value = 1;
                        memInfoTable.rows[0].cells[1].innerHTML = (slot.PN) ? slot.PN : '';
                        memInfoTable.rows[1].cells[1].innerHTML = (slot.Manu) ? slot.Manu : '';
                        memInfoTable.rows[2].cells[1].innerHTML = (slot.Type) ? slot.Type : '';
                        memInfoTable.rows[3].cells[1].innerHTML = (slot.Rate) ? slot.Rate : '';
                        memInfoTable.rows[4].cells[1].innerHTML = (slot.Cap) ? slot.Cap : '';
                        memInfoTable.rows[5].cells[1].innerHTML = (slot.DIMMType) ? slot.DIMMType : '';
                        memInfoTable.rows[6].cells[1].innerHTML = (slot.OPTemp) ? slot.OPTemp : '';
                        memInfoTable.rows[7].cells[1].innerHTML = (slot.CAS_Ltc) ? slot.CAS_Ltc : '';
                        memInfoTable.rows[8].cells[1].innerHTML = (slot.IC_Cfg) ? slot.IC_Cfg : '';
                        memInfoTable.rows[9].cells[1].innerHTML = (slot.IC_Brand) ? slot.IC_Brand : '';
                        memInfoTable.rows[10].cells[1].innerHTML = (slot.SN) ? slot.SN : '';
                        memInfoTable.rows[11].cells[1].innerHTML = (slot.Date) ? slot.Date : '';
                        memInfoTable.rows[12].cells[1].innerHTML = (slot.Therm) ? slot.Therm : '';

                        let featuresTable = document.getElementById('feature-table');

                        function insertFeatures(Feature) {
                            featuresTable.rows[0].cells[1].innerHTML = Feature.sICGrade;
                            // 0:No 1:Yes 2:RDIMM/LRDIMM
                            switch (Feature.AntiSul) {
                                case 0:
                                    featuresTable.rows[1].cells[1].innerHTML = "No";
                                    break;
                                case 1:
                                    featuresTable.rows[1].cells[1].innerHTML = "Yes";
                                    break;
                                case 2:
                                    featuresTable.rows[1].cells[1].innerHTML = "RDIMM/LRDIMM";
                                    break;
                            }
                            featuresTable.rows[2].cells[1].innerHTML = (Feature["30GF"]) ? 'Yes' : 'N/A';
                            featuresTable.rows[3].cells[1].innerHTML = (Feature.bWP) ? 'Enabled' : 'Uneabled';
                        }
                        if (!!slot.Feature) {//雙驚嘆號表示純粹的true、false
                            insertFeatures(slot.Feature);
                        }
                    }

                    while (!!memData[`${i}`]) {
                        let slot = memData[`${i}`];
                        let btn = document.createElement("button");
                        btn.setAttribute("class", "btn btn-sm btn-dark");
                        btn.textContent = i;
                        btn.value = 0;
                        btnFragment.appendChild(btn);
                        btn.addEventListener('click', switchMemIndex.bind(this, slot));
                        //btn.addEventListener('click', makeChart.bind(this, labels, TempAnalysis));
                        i++;
                    }
                    indexBtn.appendChild(btnFragment);
                    $(document).ready(() => {
                        indexBtn.firstElementChild && fire(indexBtn.firstElementChild, 'click');
                    });
                }

                /**
                 * Render storage information view
                 * @param {object} parsed_data JSON object of response data from GetDetail API
                 */
                function RenderStorageInfo(parsed_data) {
                    /**
                     * Switch the storage information by installed index.
                     */
                    function SwitchToStorage() {
                        var element = JSON.parse(this.getAttribute("user-data"));

                        element.forEach(subElement => {
                            switch (String(subElement.key)) {
                                case "Index":
                                    document.getElementById("stor-id").textContent = subElement.value;
                                    break;
                                case "Model":
                                    document.getElementById("stor-model").textContent = subElement.value;
                                    break;
                                case "Serial Number":
                                    document.getElementById("stor-sn").textContent = subElement.value;
                                    break;
                                case "Firmware Version":
                                    document.getElementById("stor-firmver").textContent = subElement.value;
                                    break;
                                case "Capacity":
                                    document.getElementById("stor-cap").textContent = subElement.value;
                                    break;
                                case "PECycle":
                                    document.getElementById("stor-pe").textContent = subElement.value;
                                    break;
                            }
                        });
                    }

                    var tab = document.getElementById("subpanel-Stor-data");
                    var index_btn = document.getElementById("btn-indx-storage");

                    parsed_data.Stor.forEach(function (element, idx, array) {
                        var btn = document.createElement("button");
                        btn.setAttribute("class", "btn btn-sm btn-dark");
                        btn.setAttribute("user-data", JSON.stringify(element));
                        btn.textContent = idx;
                        btn.value = idx;
                        btn.onclick = SwitchToStorage;
                        index_btn.appendChild(btn);
                        if (idx == 0) {
                            element.forEach(subElement => {
                                switch (String(subElement.key)) {
                                    case "Index":
                                        document.getElementById("stor-id").textContent = subElement.value;
                                        break;
                                    case "Model":
                                        document.getElementById("stor-model").textContent = subElement.value;
                                        break;
                                    case "Serial Number":
                                        document.getElementById("stor-sn").textContent = subElement.value;
                                        break;
                                    case "Firmware Version":
                                        document.getElementById("stor-firmver").textContent = subElement.value;
                                        break;
                                    case "Capacity":
                                        document.getElementById("stor-cap").textContent = subElement.value;
                                        break;
                                    case "PECycle":
                                        document.getElementById("stor-pe").textContent = subElement.value;
                                        break;
                                }
                            });
                        }
                    });
                }
                /**
                 * Render network card information view
                 * @param {object} parsed_data JSON object of response data from GetDetail API
                 */
                function RenderNETInfo(parsed_data) {
                    /**
                     * Switch the network card information by installed index.
                     */
                    function SwitchToNetwork() {
                        var element = JSON.parse(this.getAttribute("user-data"));

                        element.forEach(subElement => {
                            switch (String(subElement.key)) {
                                case "Index":
                                    document.getElementById("net-id").textContent = subElement.value;
                                    break;
                                case "Name":
                                    document.getElementById("net-name").textContent = subElement.value;
                                    break;
                                case "Type":
                                    document.getElementById("net-type").textContent = subElement.value;
                                    break;
                                case "MAC":
                                    document.getElementById("net-mac").textContent = subElement.value;
                                    break;
                                case "IPv4 Address":
                                    document.getElementById("net-ipv4").textContent = subElement.value;
                                    break;
                                case "IPv4 Netmask":
                                    document.getElementById("net-gateway").textContent = subElement.value;
                                    break;
                                case "IPv6 Address":
                                    document.getElementById("net-ipv6").textContent = subElement.value;
                                    break;
                            }
                        });
                    }

                    var index_btn = document.getElementById("btn-indx-net");
                    parsed_data.NET.forEach(function (element, idx, array) {
                        var btn = document.createElement("button");
                        btn.setAttribute("class", "btn btn-sm btn-dark");
                        btn.setAttribute("user-data", JSON.stringify(element));
                        btn.textContent = idx;
                        btn.value = idx;
                        btn.onclick = SwitchToNetwork;
                        index_btn.appendChild(btn);
                        if (idx == 0) {
                            element.forEach(subElement => {
                                switch (String(subElement.key)) {
                                    case "Index":
                                        document.getElementById("net-id").textContent = subElement.value;
                                        break;
                                    case "Name":
                                        document.getElementById("net-name").textContent = subElement.value;
                                        break;
                                    case "Type":
                                        document.getElementById("net-type").textContent = subElement.value;
                                        break;
                                    case "MAC":
                                        document.getElementById("net-mac").textContent = subElement.value;
                                        break;
                                    case "IPv4 Address":
                                        document.getElementById("net-ipv4").textContent = subElement.value;
                                        break;
                                    case "IPv4 Netmask":
                                        document.getElementById("net-gateway").textContent = subElement.value;
                                        break;
                                    case "IPv6 Address":
                                        document.getElementById("net-ipv6").textContent = subElement.value;
                                        break;
                                }
                            });
                        }
                    });
                }

                /**
                 * Render external sensor information view
                 * @param {object} parsed_data JSON object of response data from GetDetail API
                 */
                function RenderEXTInfo(parsed_data) {
                    var target_dom = document.getElementById("ext-info");
                    $("#ext-info").empty();
                    //avoid parsed_data.EX is undefined
                    parsed_data.EXT && parsed_data.EXT.forEach(function (element, idx, array) {
                        var panel_item = document.createElement("div");
                        var chart = $Card({
                            id: idx + 7,
                            name: element.Name,
                            type: "toggle-gauge",//toggle-gauge
                            width: "col-md-4 col-xs-12 col-sm-6",
                            label: [element.Unit],
                            data: [
                                element.Value,
                                ext_gauge_min[idx],// Minimum(In this case, use fixed data, needed get from the widget table setting.)
                                ext_gauge_max[idx] // Maximum(In this case, use fixed data, needed get from the widget table setting.)
                            ]
                        });
                        chart.put(panel_item);
                        target_dom.appendChild(panel_item);
                        ext_gauge_obj[idx] = chart.render();
                    });
                }

                /**
                 * Render GPU sensor information view
                 * @param {object} parsed_data JSON object of response data from GetDetail API
                 */
                function RenderGPUInfo(parsed_data) {
                    if (parsed_data.GPUDynamic && parsed_data.GPUDynamic.length > 0) {
                        let GPUDynamicData = JSON.parse(parsed_data.GPUDynamic);
                        //GPU Loading
                        card_cpu_loading_line = $Card({
                            id: 1,
                            name: "GPU Loading",
                            type: "line",
                            width: "col-md-12, pad0",
                            label: ["Loading (%)"],
                            data: [
                                parsed_data.UsageTime,
                                parsed_data.GPUUsage
                            ]
                        });
                        card_cpu_loading_line.put("#gpu-loading");
                        card_cpu_loading_line.render();

                        //gpu parse data string
                        $('#gpuFrequencyValue').html(GPUDynamicData["CoreClock"]);
                        $('#gpuTemperature').html(GPUDynamicData.Temp);
                        $('#gpuFanTemperature').html(GPUDynamicData.FanTemp);
                        $('#gpuMemoryUsed').html(GPUDynamicData.MemUsed);
                    }
                }

                var parsed_data = JSON.parse(response);

                card_cpu_loading_gauge = $Card({
                    id: 4,
                    name: "CPU Loading",
                    type: "gauge",
                    width: "col-md-4 col-sm-6 col-xs-12 w375",
                    label: ["Used", "Free", parsed_data.CPUUsage[parsed_data.CPUUsage.length - 1] + '%'],
                    data: [
                        parsed_data.CPUUsage[parsed_data.CPUUsage.length - 1],
                        100 - parsed_data.CPUUsage[parsed_data.CPUUsage.length - 1]
                    ]
                });
                card_cpu_loading_gauge.put("#cpu-loading-gauge");
                card_cpu_loading_gauge.render();

                card_mem_loading_gauge = $Card({
                    id: 5,
                    name: "MEM Loading",
                    type: "gauge",
                    width: "col-md-4 col-sm-6 col-xs-12 w375",
                    label: ["Used", "Free", parsed_data.MEMUsage[parsed_data.MEMUsage.length - 1] + '%'],
                    data: [
                        parsed_data.MEMUsage[parsed_data.MEMUsage.length - 1],
                        Math.round((100 - parsed_data.MEMUsage[parsed_data.MEMUsage.length - 1]) * 100) / 100
                    ]
                });

                card_mem_loading_gauge.put("#mem-loading-gauge");
                card_mem_loading_gauge.render();
                console.log(parsed_data);
                if (parsed_data.EXT && parsed_data.EXT.length !== 0) {
                    sub_tabs_tooltip = [
                        "Device overview",
                        "Device operating system information",
                        "Device CPU information",
                        "Device motherboard information",
                        "Device memory information",
                        "Device storage information",
                        "Device network card information",
                        "Device external sensor information",
                        "Device location information"
                    ];
                    sub_tabs = [
                        "Overview", "OS", "CPU", "MB", "MEM", "Storage", "NET", "External", "Location"
                    ];
                    RenderEXTInfo(parsed_data);
                }
                else {
                    sub_tabs_tooltip = [
                        "Device overview",
                        "Device operating system information",
                        "Device CPU information",
                        "Device motherboard information",
                        "Device memory information",
                        "Device storage information",
                        "Device network card information",
                        "Device location information"
                    ];
                    sub_tabs = [
                        "Overview", "OS", "CPU", "MB", "MEM", "Storage", "NET", "Location"
                    ];
                }

                if (parsed_data.GPUDynamic && parsed_data.GPUDynamic.length > 0) {
                    sub_tabs.push('GPU');
                    sub_tabs_tooltip.push('Device external sensor informatio');
                    RenderGPUInfo(parsed_data);
                }

                if (parsed_data.IsInnoAGE) {
                    sub_tabs.push('OOB');
                    sub_tabs_tooltip.push('Device out of band');
                }

                RenderEXTInfo(parsed_data);
                RenderOSInfo(parsed_data);
                RenderCPUInfo(parsed_data);
                RenderMBInfo(parsed_data);
                RenderMEMInfo(parsed_data);
                RenderStorageInfo(parsed_data);
                RenderNETInfo(parsed_data);
                RenderSecondaryTab(sub_tabs);
                $('body').removeClass('loading');
            },
            error: function (response) {
                $('body').removeClass('loading');
            }
        });
    }

    /**
     * To Do: Check token and redirect to login page when get wrong token from cookie.
     */
    var store_token = $.cookie('token');

    card_cpu_loading_gauge = undefined;
    card_mem_loading_gauge = undefined;
    card_storage_lifespan_bar = undefined;
    card_cpu_loading_line = undefined;
    card_mem_loading_line = undefined;
    card_dev_location = undefined;

    RenderOverview(store_token, devName);
    RenderInformationView(store_token, devName);
    token = store_token;
    deviceName = devName;
});
