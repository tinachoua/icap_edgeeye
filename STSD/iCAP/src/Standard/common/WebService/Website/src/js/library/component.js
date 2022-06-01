import { myChart } from '../library/make_chart';
import { API } from '../library/api_handler';
export let g_AutoRefreshId;
import { makeChart, widget } from "../library/make_chart";
import { fire } from './event_handler';
import { GOOGLEMAP_CHART, VECTORMAP_CHART, OPENSTREETMAP_CHART, GAODEMAP_CHART } from '../library/data_mapping';
import { createIndexBtn, createBody } from '../library/common';
import { DeviceManager as deviceManager } from '../DeviceManager';
import {
    BG_COLOR_03,
    CHART_BASE_COLOR_D,
    FONT_COLOR_A
} from "../constants/globalVariable";

import updateStatus from './OOB/helpers/updateStatus'

let _lineChart = undefined;
let _chartObj = undefined;
export let render = function () {

    function RenderOSInfo(data) {
        let tab = document.getElementById("subpanel-OS-data");
        data.forEach(element => {
            let tr_root = document.createElement("tr");
            let td_key = document.createElement("td");
            let b_key = document.createElement("b");
            b_key.textContent = element.key;
            td_key.appendChild(b_key);
            tr_root.appendChild(td_key);
            let td_value = document.createElement("td");
            td_value.textContent = element.value;
            tr_root.appendChild(td_value);
            tab.appendChild(tr_root);
        });
    }

    function RenderCPUInfo(data) {
        let tab = document.getElementById("subpanel-CPU-data");
        let coreCount = data.CPU[2].value;
        let chartMaker = myChart();
        let coreFreq = document.getElementById('tab-core-freq');
        let coreLoad = document.getElementById('tab-core-loading');
        let coreTemp = document.getElementById('tab-core-temp');
        let coreVoltage = document.getElementById('tab-core-voltage');
        let coreData = document.getElementsByName('core-info');
        let tabPane = $('#cpu-info-dynamic .tab-pane');
        let firstTabPane = $('#cpu-info-dynamic .tab-pane').first();

        tabPane.removeClass('active');
        firstTabPane.addClass('active');
        $(coreData).remove();

        data.CPU.forEach(element => {
            let tr_root = document.createElement("tr");
            let td_key = document.createElement("td");
            let b_key = document.createElement("b");
            b_key.textContent = element.key;
            td_key.appendChild(b_key);
            tr_root.appendChild(td_key);
            let td_value = document.createElement("td");
            td_value.textContent = element.value;
            tr_root.appendChild(td_value);
            tab.appendChild(tr_root);
        });

        chartMaker.line(1, 'CPU Loading', 'col-md-12, pad0', ["Loading (%)"], [data.UsageTime, data.CPUUsage], 'cpu-loading');

        $('#frequency-value').text(data.CPUFreq);
        $('#fan-value').text(data.CPUFan);

        if (coreCount >= 0) {
            let coreInfo = {
                frequency: '',
                loading: '',
                temp: '',
                voltage: ''
            };

            for (let i = 0; i < coreCount; i++) {
                if (!data.CPUCore.CoreFreq[i]) {
                    break;
                }
                let paddingValue = '5px';
                if (i === 0) {
                    paddingValue = 0;
                }

                coreInfo.frequency += `<div style="padding-top: ${paddingValue};" name="core-info">` +
                    `<div style="float:left;line-height: 40px;font-size: 12px;">` +
                    `Core #${i}` +
                    '</div>' +
                    '<div class="text-right">' +
                    '<span id="fan-value" class="text-right" style="font-weight: 700;height: 40px;line-height: 38px;font-size: 40px;">' +
                    `${data.CPUCore.CoreFreq[i]}` +
                    '</span>' +
                    `<span class="fan-unit">MHz</span>` +
                    '</div>' +
                    '</div>';

                coreInfo.loading += `<div style="padding-top: ${paddingValue};" name="core-info">` +
                    `<div style="float:left;line-height: 40px;font-size: 12px;">` +
                    `Core #${i}` +
                    '</div>' +
                    '<div class="text-right">' +
                    '<span id="fan-value" class="text-right" style="font-weight: 700;height: 40px;line-height: 38px;font-size: 40px;">' +
                    `${parseFloat(data.CPUCore.CoreUsage[i].toFixed(1))}` +
                    '</span>' +
                    `<span class="fan-unit">%</span>` +
                    '</div>' +
                    '</div>';

                coreInfo.temp += `<div style="padding-top: ${paddingValue};" name="core-info">` +
                    `<div style="float:left;line-height: 40px;font-size: 12px;">` +
                    `Core #${i}` +
                    '</div>' +
                    '<div class="text-right">' +
                    '<span id="fan-value" class="text-right" style="font-weight: 700;height: 40px;line-height: 38px;font-size: 40px;">' +
                    `${data.CPUCore.CoreTemp[i]}` +
                    '</span>' +
                    `<span class="fan-unit">℃</span>` +
                    '</div>' +
                    '</div>';

                coreInfo.voltage += `<div style="padding-top: ${paddingValue};" name="core-info">` +
                    `<div style="float:left;line-height: 40px;font-size: 12px;">` +
                    `Core #${i}` +
                    '</div>' +
                    '<div class="text-right">' +
                    '<span id="fan-value" class="text-right" style="font-weight: 700;height: 40px;line-height: 38px;font-size: 40px;">' +
                    `${data.CPUCore.CoreVoltage[i]}` +
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

    function RenderMBInfo(data) {
        let tab = document.getElementById("subpanel-MB-data");
        data.MB.forEach(element => {
            let tr_root = document.createElement("tr");
            let td_key = document.createElement("td");
            let b_key = document.createElement("b");
            b_key.textContent = element.key;
            td_key.appendChild(b_key);
            tr_root.appendChild(td_key);
            let td_value = document.createElement("td");
            td_value.textContent = element.value;
            tr_root.appendChild(td_value);
            tab.appendChild(tr_root);
        });
    }

    function RenderMEMInfo(data) {
        let memInfoTable = document.getElementById('subpanel-mem-info-table');
        let memData = JSON.parse(data.MEM);
        const getTempDistribution = $.ajax({
            type: 'GET',
            url: `DeviceInfoAPI/${data.DevName}/Memory/Temperature-Distribution`,
            async: true,
            crossDomain: true,
            headers: {
                token: $.cookie('token'),
            }
        });
        getTempDistribution.done((respnse) => {
            let labels = JSON.parse(respnse).Labels;
            let TempAnalysis = JSON.parse(respnse).Value;
            let tempChart = document.getElementById('tempChart');
            tempChart.height = '100%';
            function makeTempBar(labels, TempAnalysis) {
                let data = {
                    labels: labels,
                    datasets: [{
                        label: 'probability',
                        data: TempAnalysis,
                        backgroundColor: CHART_BASE_COLOR_D,
                        borderWidth: 1
                    }],
                }
                let options = {
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
            _chartObj = makeTempBar(labels, TempAnalysis);
        })
        getTempDistribution.fail(() => {
            console.log("error");
        });
        if (Object.keys(memData).length !== 0) {
            let indexBtn = document.getElementById("btn-indx-mem");
            let featuresTable = document.getElementById('feature-table');
            let i = 0;
            let btnFragment = document.createDocumentFragment();
            let switchMemIndex = function (slot, event) {
                if (event.target.value == 1) {
                    return;
                }
                for (let btn of indexBtn.getElementsByTagName('button')) {
                    btn.value = 0;
                }
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

                if (!!slot.Feature) {
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
                i++;
            }
            indexBtn.appendChild(btnFragment);
            $(document).ready(() => {
                indexBtn.firstElementChild && fire(indexBtn.firstElementChild, 'click');
            });
        }
        function clearMemoryChart(lineChart, tempChart) {
            if (lineChart) {
                lineChart.destroy();
                lineChart = undefined;
            }

            if (tempChart) {
                tempChart.destroy();
                tempChart = undefined;
            }
        }
        clearMemoryChart(_lineChart, _chartObj);
        if (data.UsageTime.length !== 0 && data.MEMUsage.length !== 0) {
            let chartMaker = myChart();

            $(document).ready(() => {
                _lineChart = chartMaker.line('widget-mem', 'Memory Loading', 'col-md-12, pad0', ["Loading (%)"], [data.UsageTime, data.MEMUsage], 'mem-loading');
            });
        }
    }

    function SwitchToStorage() {
        let element = JSON.parse(this.getAttribute("user-data"));
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

    function RenderStorageInfo(data) {
        let index_btn = document.getElementById("btn-indx-storage");
        data.forEach(function (element, idx, array) {
            let btn = document.createElement("button");
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

    function RenderNETInfo(data) {
        function SwitchToNetwork() {
            let element = JSON.parse(this.getAttribute("user-data"));

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

        let index_btn = document.getElementById("btn-indx-net");

        data.forEach(function (element, idx, array) {
            let btn = document.createElement("button");
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

    function RenderDeviceMAP(parsed_data) {
        let data = parsed_data.markers[0];
        let lat = data.position.lat;
        let lng = data.position.lng;
        let mapDataFormat = {
            centerLat: lat,
            centerLng: lng,
            value: parsed_data.markers
        };
        let loc_long = document.getElementById("loc-long");
        let loc_lat = document.getElementById("loc-lat");
        let loc_status = document.getElementById("loc-status");
        let map = document.getElementById('subpanel-location');
        let button = map.parentElement.getElementsByTagName('button');
        let settingObj = {
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
        let setChartType = function (settingObj, mapId) {
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

        // while (map.hasChildNodes()) {
        //     map.removeChild(map.firstChild);
        // }

        while (button.length != 0) {
            button[0].parentNode.removeChild(button[0]);
        }

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
                let removeClick = function (button) {
                    let buttonArray = Array.from(button);
                    buttonArray.some((item) => {
                        if (item.classList.contains('button-click')) {
                            item.classList.remove('button-click');
                            item.style.color = BG_COLOR_03;
                            item.style.background = FONT_COLOR_A;
                            return true;
                        }
                    });
                };
                let mapId = event.target.value;
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
                let APICaller = API();
                APICaller.PUT('EmployeeAPI/SetCommonMap', Number(mapId));
            });
            map.insertAdjacentElement('beforebegin', buttonElement);
        });

        let widgetHandler = widget();
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

    function RenderEXTInfo(data) {
        let target_dom = document.getElementById("ext-info");
        let ext_gauge_max = [
            8000, 260, 220, 400, 500, 100
        ];
        let ext_gauge_min = [
            0, 0, -40, 0, 0, 0
        ];
        let ext_gauge_obj = [];
        let APICaller = API();
        let refreshEXTData = function () {
            let updateEXTInfo = function (data) {
                data.EXT && data.forEach(function (element, idx, array) {
                    ext_gauge_obj[idx].data.datasets[0].data[0] = element.Value;
                    ext_gauge_obj[idx].data.datasets[0].data[1] = ext_gauge_max[idx] - element.Value;
                    ext_gauge_obj[idx].options.elements.center.text = element.Value + " " + element.Unit;
                    ext_gauge_obj[idx].update();
                });
            };
            let promise = APICaller.GET(`DeviceInfoAPI/GetDetail?DeviceName=${data.DevName}`, null, null, false);
            promise.done((response) => {
                let parsed_data = JSON.parse(response);
                updateEXTInfo(parsed_data.EXT);
            });
        };

        $("#ext-info").empty();
        //avoid data.EX is undefined
        data.EXT.forEach(function (element, idx, array) {
            let panel_item = document.createElement("div");
            let chart = $Card({
                id: idx + 7,
                name: element.Name,
                type: "toggle-gauge",
                width: "col-md-4 col-xs-12 col-sm-6",
                label: [element.Unit],
                data: [
                    element.Value,
                    ext_gauge_min[idx],
                    ext_gauge_max[idx]
                ]
            });
            chart.put(panel_item);
            target_dom.appendChild(panel_item);
            ext_gauge_obj[idx] = chart.render();
        });

        g_AutoRefreshId = setInterval(refreshEXTData, 1000);

        return g_AutoRefreshId;
    }


    function RenderGPUInfo(data) {
        let chartMaker = myChart();
        let APICaller = API();
        if (data.GPU && data.GPU.length > 0) {
            let GPUInfData = JSON.parse(data.GPU);
            let target_dom = document.getElementById("subpanel-GPU-data");
            let GPUInformationTamplate = `
                         <tr><td><b>Name</b></td><td >${GPUInfData.Name}</td> </tr>
                         <tr><td><b>GPU</b></td><td >${GPUInfData.Arch}</td></tr>    
                         <tr><td><b>CUDA Drver Version</b></td><td >${GPUInfData.DriverVer}</td> </tr>
                         <tr><td><b>Compute Capability</b></td><td >${GPUInfData.ComputeCap}</td></tr>
                         <tr><td><b>CUDA Cores</b></td> <td >${GPUInfData.CoreNum}</td></tr>
                         <tr><td><b>Memory Type</b></td><td >${GPUInfData.MemType}</td></tr>
                         <tr><td><b>Memory Bus Width</b></td><td >${GPUInfData.MemBusWidth}</td></tr>
                         <tr><td><b>Memory Size</b></td><td >${GPUInfData.MemSize}</td></tr>
                         <tr><td><b>Memory Bandwitdh</b></td><td >${GPUInfData.MemBandWidth}</td></tr>
                         <tr><td><b>GPU Clock</b></td><td >${GPUInfData.Clock}</td></tr>
                         <tr><td><b>Memory Clock</b></td><td >${GPUInfData.MemClock}</td></tr>
                         `
            target_dom.innerHTML = GPUInformationTamplate;
        }
        if (data.GPUDynamic.length > 0) {
            let gpuDynamicData = JSON.parse(data.GPUDynamic);
            $('#gpuFrequencyValue').html(gpuDynamicData["CoreClock"]);
            $('#gpuTemperature').html(gpuDynamicData.Temp);
            $('#gpuFanTemperature').html(gpuDynamicData.FanTemp);
            $('#gpuMemoryUsed').html(gpuDynamicData.MemUsed);
            $('#gpuFrequencyValue').html(gpuDynamicData["CoreClock"]);
        }
        if (data.GPUUsage) {
            chartMaker.line('widget-gpu', 'GPU Loading', 'col-md-12, pad0', ["Loading (%)"], [data.UsageTime, data.GPUUsage], 'gpu-loading')
        }
    }

    function RenderiAnalyzer(devName) {
        $.ajax({
            type: 'GET',
            url: 'DeviceInfoAPI/iAnalyzer?DeviceName=' + devName,
            async: true,
            crossDomain: true,
            headers: {
                'token': $.cookie('token')
            },
            success: function (response) {
                var data = JSON.parse(response);

                if (JSON.stringify(data.Analyzer[0]) !== '{}') {
                    let unsupportFeature = document.getElementById('unsupported-feature');
                    unsupportFeature.setAttribute('style', 'display:none');

                    let RenderData = function RenderData(element) {
                        const { Model, Cap, SN } = element.Storage || {
                            Model: 'N/A', Cap: 'N/A', SN: 'N/A'
                        };
                        let model_label = document.getElementById("analyzer-stor-model");
                        model_label.textContent = "Model: ";
                        model_label.appendChild(document.createElement("br"));
                        model_label.appendChild(document.createTextNode(Model));

                        let capacity_label = document.getElementById("analyzer-stor-cap");
                        capacity_label.textContent = "Capacity: ";
                        capacity_label.appendChild(document.createElement("br"));
                        capacity_label.appendChild(document.createTextNode(Cap + " GB"));

                        let sn_label = document.getElementById("analyzer-stor-sn");
                        sn_label.textContent = "Serial Number: ";
                        sn_label.appendChild(document.createElement("br"));
                        sn_label.appendChild(document.createTextNode(SN));

                        let chart_TR = document.getElementById("analyzer-stor-tr");

                        $(document).ready(() => {
                            const RW = element.RW;
                            const src = RW.SRC || 0;
                            const rrc = RW.RRC || 0;
                            const swc = RW.SWC || 0;
                            const rwc = RW.RWC || 0;
                            const sr = RW.SR || { label: [''], value: [0] };
                            const sw = RW.SW || { label: [''], value: [0] };
                            const rr = RW.RR || { label: [''], value: [0] };
                            const rw = RW.RW || { label: [''], value: [0] };

                            const {
                                Health, AvgEC: { Count }, PECycle
                            } = element.Lifespan || {
                                Health: 0, AvgEC: {
                                    Count: 0
                                }, PECycle: 0
                            }

                            $Card({
                                id: 1 + '-iAanlyzer',
                                name: "Total Read",
                                type: "pie",
                                width: "col-md-4 col-sm-6 col-xs-12",
                                label: [''],
                                data: {
                                    label: ["Sequential", "Random"],
                                    value: [src, rrc]
                                }
                            }).put(chart_TR).render();

                            let chart_TW = document.getElementById("analyzer-stor-tw");
                            $Card({
                                id: 2 + '-iAanlyzer',
                                name: "Total Write",
                                type: "pie",
                                width: "col-md-4 col-sm-6 col-xs-12",
                                label: [''],
                                data: {
                                    label: ["Sequential", "Random"],
                                    value: [swc, rwc]
                                }
                            }).put(chart_TW).render();

                            let chart_SR = document.getElementById("analyzer-stor-sr");

                            $Card({
                                id: 3 + '-iAanlyzer',
                                name: "Sequential Read",
                                type: "pie",
                                width: "col-md-4 col-sm-6 col-xs-12",
                                label: [''],
                                data: sr
                            }).put(chart_SR).render();

                            let chart_SW = document.getElementById("analyzer-stor-sw");
                            $Card({
                                id: 4 + '-iAanlyzer',
                                name: "Sequential Write",
                                type: "pie",
                                width: "col-md-4 col-sm-6 col-xs-12",
                                label: [''],
                                data: sw
                            }).put(chart_SW).render();

                            let chart_RR = document.getElementById("analyzer-stor-rr");
                            $Card({
                                id: 5 + '-iAanlyzer',
                                name: "Random Read",
                                type: "pie",
                                width: "col-md-4 col-sm-6 col-xs-12",
                                label: [''],
                                data: rr
                            }).put(chart_RR).render();

                            let chart_RW = document.getElementById("analyzer-stor-rw");
                            $Card({
                                id: 6 + '-iAanlyzer',
                                name: "Random Write",
                                type: "pie",
                                width: "col-md-4 col-sm-6 col-xs-12",
                                label: [''],
                                data: rw
                            }).put(chart_RW).render();

                            let chart_health = document.getElementById("analyzer-stor-health");

                            $Card({
                                id: 7 + '-iAanlyzer',
                                name: "Storage Health",
                                type: "gauge",
                                width: "col-md-6 col-sm-6 col-xs-12 gauge",
                                label: ["Health", "Used", Health + '%'],
                                data: [
                                    Health,
                                    Math.round((100 - Health) * 100) / 100
                                ]
                            }).put(chart_health).render();

                            let chart_lifespan = document.getElementById("analyzer-stor-pe");
                            $Card({
                                id: 8 + '-iAanlyzer',
                                name: "P/E Cycle",
                                type: "gauge",
                                width: "col-md-6 col-sm-6 col-xs-12 gauge",
                                label: ["Average Erase Count", "PE Cycle", Count + '/' + PECycle],
                                data: [
                                    Count,
                                    PECycle - Count
                                ]
                            }).put(chart_lifespan).render();

                            let lifespan_date = [];
                            let lifespan_data = [];
                            const length = element.Lifespan ? element.Lifespan.Lifespan.length : 0
                            for (let i = 0; i < length; i++) {
                                lifespan_date[i] = element.Lifespan.Lifespan[i].Date;
                                lifespan_data[i] = element.Lifespan.Lifespan[i].EstimatedDays;
                            }

                            let chart_lifespan_log = document.getElementById("analyzer-stor-life");
                            $Card({
                                id: 9 + '-iAanlyzer',
                                name: "Lifespan Logs",
                                type: "line",
                                width: "col-md-12 col-sm-12 col-xs-12",
                                label: ['Estimation Lifespan (Days)'],
                                data: [
                                    lifespan_date,
                                    lifespan_data
                                ],
                                xAxes: {
                                    type: 'date'
                                }
                            }).put(chart_lifespan_log).render();
                        });
                    };
                    let SwitchToStorage = function SwitchToStorage() {
                        let element = JSON.parse(this.getAttribute("user-data"));
                        RenderData(element);
                    };
                    let index_btn = document.getElementById("analyzer-storage-btn");

                    data.Analyzer.forEach(function (element, idx, array) {
                        const lifespan = data.Lifespan.find(ele => ele.SN === element.SN);
                        const storage = data.Storage.find(ele => ele.SN === element.SN);
                        let btn_data = {
                            "Lifespan": lifespan,
                            "RW": element,
                            "Storage": storage
                        };
                        let btn = document.createElement("button");
                        btn.setAttribute("class", "btn btn-sm btn-dark");
                        btn.setAttribute("style", "margin-right: 3px;");
                        btn.setAttribute("user-data", JSON.stringify(btn_data));

                        btn.textContent = idx;
                        btn.value = idx;
                        btn.onclick = SwitchToStorage;
                        index_btn.appendChild(btn);

                        if (idx == 0) {
                            RenderData(btn_data);
                        }
                    });
                } else {
                    let unsupportFeature = document.getElementById('unsupported-feature');
                    let analyzer = document.getElementById('child-page').getElementsByClassName('device_analyzer')[0];

                    analyzer.setAttribute('style', 'display:none');
                    unsupportFeature.setAttribute('style', '');
                }
            },
            error: function (response) {
                $('body').removeClass('loading');
            }
        });
    }

    function RenderOOB(devName, oobDirector) {
        const ooblist = deviceManager.getOOBlist(devName);
        const tableBody = document.querySelector('.subpanel-OOB tbody');
        const oobService = document.querySelector('#OOBTab-service');
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

    return {
        OSInfo: RenderOSInfo,
        CPUInfo: RenderCPUInfo,
        MBInfo: RenderMBInfo,
        MEMInfo: RenderMEMInfo,
        StorageInfo: RenderStorageInfo,
        NetInfo: RenderNETInfo,
        DeviceMap: RenderDeviceMAP,
        EXTInfo: RenderEXTInfo,
        GPUInfo: RenderGPUInfo,
        iAnalyzer: RenderiAnalyzer,
        OOBInfo: RenderOOB
    };
};