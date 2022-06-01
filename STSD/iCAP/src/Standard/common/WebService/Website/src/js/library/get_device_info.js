import { putSecondTabs, switchSecondTabs } from "../library/init_second_tabs";
import { myChart } from "../../js/library/make_chart";
import { makeColorsWithoutHeightlight, getHeightLightColor } from "../library/color";
import { putFirstsTabs } from "../library/init_first_tabs";
import { autoRefresh } from "../library/init_second_tabs";

export var g_device_data = {};

export function initDeviceInfoPage(deviceData, parsed_data, clickFlag, event_table, oobDirector) {
    g_device_data = deviceData;

    var insertData_ov = function (parsed_data) {
        document.getElementById("overview-os").textContent = parsed_data.OS;
        document.getElementById("overview-cpu").textContent = parsed_data.CPU;
        document.getElementById("overview-mem").textContent = parsed_data.MEMCap + " GB";
        document.getElementById("overview-stor").textContent = parsed_data.StorCap + " GB";
        document.getElementById("overview-status").textContent = parsed_data.Status;
        $("#devimg").attr('src', g_device_data.img.Response);


        if (parsed_data.iCover === true) {
            $("#recovery_btn").show();
        } else {
            $("#recovery_btn").hide();
        }




        if (Boolean(parsed_data.Alias) === true) {
            $('#devName').html(parsed_data.Alias);
        } else {
            $('#devName').html(devName);
        }
    };
    const chartMaker = myChart();
    var insertWidget_ov = function (detail, overview, threshold) {
        var label = ["Used", "Free", detail.CPUUsage[detail.CPUUsage.length - 1] + '%'];
        var data = [
            detail.CPUUsage[detail.CPUUsage.length - 1],
            100 - detail.CPUUsage[detail.CPUUsage.length - 1]
        ];
        var chartColor;
        var storName = [];
        var storData = [];
        const LIFESPAN_THRESHOLD = 150;
        chartMaker.Gauge(4, "CPU Loading", "col-md-4 col-sm-6 col-xs-12 w375", label, data, 'cpu-loading-gauge');

        label = ["Used", "Free", detail.MEMUsage[detail.MEMUsage.length - 1] + '%'];
        data = [
            detail.MEMUsage[detail.MEMUsage.length - 1],
            Math.round((100 - detail.MEMUsage[detail.MEMUsage.length - 1]) * 100) / 100
        ];
        chartMaker.Gauge(5, "MEM Loading", "col-md-4 col-sm-6 col-xs-12 w375", label, data, 'mem-loading-gauge');

        chartColor = makeColorsWithoutHeightlight(overview.Lifespan.length);
        overview.Lifespan.forEach((element, idx) => {
            storName[idx] = 'Storage ' + idx;

            if (element.Lifespan !== null && element.Lifespan.length > 0) {
                storData[idx] = element.Lifespan.pop().EstimatedDays;
                if (storData[idx] < LIFESPAN_THRESHOLD) {
                    chartColor[idx] = getHeightLightColor();
                }
            }
        });

        data = {
            label: storName,
            value: storData,
            name: 'Estimation Lifespan (Days)'
        };

        chartMaker.Bar_Threshold(6, "Storage Lifespan", "col-md-4", [''], data, 'stor-lifespan', chartColor);
    };

    if (clickFlag.deviceLink === false) {
        $('#child-page div')[0].remove();
        clickFlag.deviceLink = true;
        putFirstsTabs(parsed_data, clickFlag, event_table, oobDirector);
        putSecondTabs(g_device_data, oobDirector);
    } else {
        $('#tab-secondary-continer a').removeClass('hover');
        $('#Overview').addClass('hover');

        if ($('#External').length > 0) {
            $('#External').next().remove();
            $('#External').remove();
        }
        if ($('#GPU').length > 0) {
            $('#GPU').next().remove();
            $('#GPU').remove();
        }
        if ($('#OOB').length > 0) {
            $('#OOB').next().remove();
            $('#OOB').remove();
        }

        if ((g_device_data.detail.EXT && g_device_data.detail.EXT.length !== 0) === true) {
            let tabSecondContainer = document.getElementById("tab-secondary-continer");
            let subTab = $Tab({
                name: 'External'
            }).getTab();
            $(subTab).on('click', () => {
                var $primaryTab = $('#tab-primary-continer a');
                var $secondaryTab = $('#tab-secondary-continer a');
                var $childPage = $('#child-page .device_analyzer');
                var widget = document.getElementsByClassName('widget');

                if (autoRefresh.id !== undefined) {
                    clearInterval(autoRefresh.id);
                    autoRefresh.id = undefined;
                }
                $primaryTab.removeClass('tab-click');
                $secondaryTab.removeClass('hover');
                $(subTab).addClass('hover');
                $childPage.hide();
                $(widget).hide();
                var toTabsActor = switchSecondTabs();
                toTabsActor.toExternal(g_device_data);
            });
            subTab.setAttribute("class", "sub-tab-content");
            subTab.setAttribute("id", 'External');
            subTab.setAttribute("data-toggle", "tooltip");
            subTab.setAttribute("data-placement", "bottom");
            subTab.setAttribute("title", 'Device external sensor information');
            const span = document.createElement('span');
            span.className = 'divider hidden-xs';
            tabSecondContainer.appendChild(subTab);
            tabSecondContainer.appendChild(span);
        }

        if (g_device_data.detail.GPUDynamic && g_device_data.detail.GPUDynamic.length !== 0) {
            let tabSecondContainer = document.getElementById("tab-secondary-continer");
            let subTab = $Tab({
                name: 'GPU'
            }).getTab();
            $(subTab).on('click', () => {
                var $primaryTab = $('#tab-primary-continer a');
                var $secondaryTab = $('#tab-secondary-continer a');
                var $childPage = $('#child-page .device_analyzer');
                var widget = document.getElementsByClassName('widget');

                if (autoRefresh.id !== undefined) {
                    clearInterval(autoRefresh.id);
                    autoRefresh.id = undefined;
                }

                $primaryTab.removeClass('tab-click');
                $secondaryTab.removeClass('hover');
                $(subTab).addClass('hover');
                $childPage.hide();
                $(widget).hide();
                var toTabsActor = switchSecondTabs();
                
                toTabsActor.toGPU(g_device_data);
                //switchSubPage(g_device_data);
            });
            subTab.setAttribute("class", "sub-tab-content");
            subTab.setAttribute("id", 'GPU');
            subTab.setAttribute("data-toggle", "tooltip");
            subTab.setAttribute("data-placement", "bottom");
            subTab.setAttribute("title", 'Device GPU information');
            const span = document.createElement('span');
            span.className = 'divider hidden-xs';
            tabSecondContainer.appendChild(subTab);
            tabSecondContainer.appendChild(span);
        }

        if (g_device_data.detail.IsInnoAGE){
            let tabSecondContainer = document.getElementById("tab-secondary-continer");
            let subTab = $Tab({
                name: 'OOB'
            }).getTab();
            $(subTab).on('click', () => {
                var $primaryTab = $('#tab-primary-continer a');
                var $secondaryTab = $('#tab-secondary-continer a');
                var $childPage = $('#child-page .device_analyzer');
                var widget = document.getElementsByClassName('widget');

                if (autoRefresh.id !== undefined) {
                    clearInterval(autoRefresh.id);
                    autoRefresh.id = undefined;
                }

                $primaryTab.removeClass('tab-click');
                $secondaryTab.removeClass('hover');
                $(subTab).addClass('hover');
                $childPage.hide();
                $(widget).hide();
                var toTabsActor = switchSecondTabs();
                toTabsActor.toOOB(g_device_data.detail.DevName, oobDirector);
            });
            subTab.setAttribute("class", "sub-tab-content");
            subTab.setAttribute("id", 'OOB');
            subTab.setAttribute("data-toggle", "tooltip");
            subTab.setAttribute("data-placement", "bottom");
            subTab.setAttribute("title", 'Device out of band');
            const span = document.createElement('span');
            span.className = 'divider hidden-xs';
            tabSecondContainer.appendChild(subTab);
            tabSecondContainer.appendChild(span);
        }
    }
    insertData_ov(g_device_data.overview);
    insertWidget_ov(g_device_data.detail, g_device_data.overview, g_device_data.threshold);
}