import AMapLoader from '@amap/amap-jsapi-loader';
import forEach from 'lodash/forEach';
import { colors } from "../library/color";
import { API } from "../library/api_handler";
import { getSuccess } from "../library/card_click_event";
import MARKERGREEN from "../../assets/images/marker-green.png";
import MARKERRED from "../../assets/images/marker-red.png";
import MARKERGREEN_SELECTED from '../../assets/images/marker-green-selected.png';
import MARKERRED_SELECTED from '../../assets/images/marker-red-selected.png';
import Map from 'ol/Map.js';
import View from 'ol/View.js';
import { defaults as defaultControls, FullScreen } from 'ol/control.js';
import OSM from 'ol/source/OSM.js';
import Feature from 'ol/Feature.js';
import Point from 'ol/geom/Point.js';
import { Icon, Style } from 'ol/style.js';
import VectorSource from 'ol/source/Vector.js';
import Overlay from 'ol/Overlay.js';
import { Tile as TileLayer, Vector as VectorLayer } from 'ol/layer.js';
import { findIndexInArray, sortMarker, timeConverter } from "../library/common";
import { changeColor, fire } from "./event_handler";
import { toOption } from "./select_form";
import fullscreen from '../../assets/images/fullscreen.svg';
import unfullscreen from '../../assets/images/unfullscreen.svg';
import unfullscreen_hover from '../../assets/images/unfullscreen_hover.svg';
import fullscreen_hover from '../../assets/images/fullscreen_hover.svg';
import { WIDGET_WIDTH } from '../library/data_define';
import '../../css/setting_customized_map.css';
import '../../css/customized_map.css';
import getGoogleMapKey from '../helpers/googlemap_key';
import {
    BG_COLOR_03
} from "../constants/globalVariable";

function makeRandomData(counts, minNumber, maxNumber) {
    var randomData;
    var data = [];
    for (var i = 0; i < counts; i++) {
        randomData = Math.floor(Math.random() * (maxNumber - minNumber + 1)) + minNumber;
        data[i] = randomData;
    }
    return data;
}

export var myChart = function () {

    function gaugeChart(id, name, width, labelArray, dataArray, rootId) {
        var gauge = $Card({
            id: id,
            name: name,
            type: "gauge",
            width: width,
            label: labelArray,
            data: dataArray
        });
        gauge.put("#" + rootId);
        gauge.render();
    }

    function barChart_threshold(id, name, width, labelArray, dataObject, rootId, chartColor) {
        var bar = $Card({
            id: id,
            name: name,
            type: "group-bar",
            width: width,
            label: labelArray,
            data: dataObject,
            color: chartColor
        });
        bar.put("#" + rootId);
        bar.render();
    }

    function lineChart(id, name, width, label, data, rootId) {
        var card_cpu_loading_line = $Card({
            id: id,
            name: name,
            type: "line",
            width: width,
            label: label,
            data: data
        });
        card_cpu_loading_line.put("#" + rootId);
        return card_cpu_loading_line.render();
    }

    var publicAPI = {
        Gauge: gaugeChart,
        Bar_Threshold: barChart_threshold,
        line: lineChart
    };

    return publicAPI;
}

export function makeMockBarChart(rootId, chartId, chartNameId, chartName, label, widthString) {
    var chart = document.getElementById(rootId);
    var name = chartName;
    var data = makeRandomData(label.length, 50, 100);
    var color = colors(label.length);

    var dataNormalize = {
        label: label,
        value: data,
        name: ''
    };

    var barPreview = $Card({
        id: chartId,
        name: name,
        type: "bar",
        width: widthString,
        label: [''],
        data: dataNormalize,
        color: color
    });
    barPreview.put(chart, chartNameId);
    barPreview.render();
}

export function makeChart(settingObj, outChart) {
    var target = settingObj.target;
    var name = settingObj.title;
    var chartId = settingObj.widgetId;
    var type = settingObj.type;
    var width = settingObj.width;
    var data = settingObj.data;
    var dashboardId = settingObj.dashboardId;
    var clickFlag = settingObj.clickFlag;
    var dragFlag = settingObj.dragFlag;
    var chartNameId = settingObj.chartNameId;
    var widget = $Card({
        id: chartId,
        name: name,
        type: type,
        width: width,
        label: [''],
        data: data,
        dashboardId: dashboardId
    });
    widget.put(target, chartNameId, dragFlag, clickFlag, outChart);
    widget.render(clickFlag, outChart);
}

export function widget() {

    function updateContent(itemArray, target, evt) {
        var itemIndex = findIndexInArray(itemArray, 'name', evt.target.value);

        target.status.textContent = itemArray[itemIndex].status;
        target.time.textContent = (itemArray[itemIndex].time > 0) ? timeConverter(itemArray[itemIndex].time) : 'N/A';
        target.msg.textContent = itemArray[itemIndex].detail;
    }

    var updateLabel = function (targetLabel, newLabel, newValue, count, colors) {
        for (let i = 0; i < count; i++) {
            targetLabel.title[i].textContent = newLabel[i];
            targetLabel.value[i].textContent = newValue[i];
            targetLabel.title[i].parentNode._label = newLabel[i];
            targetLabel.value[i].parentNode._label = newLabel[i];
        }

        // if the new settings data length is unequal to the count. The label color need to reset.
        if (!!colors) {
            for (let i = 0; i < count; i++) {
                targetLabel.color[i].style.background = colors[i];
            }
        }
    };

    var addLabel = function (target, labelRoot, newData, labelIndex, id) {
        var labelContent, labelTitle, labelValue, labelColor;
        var doc = document;
        labelTitle = doc.createElement("small");
        labelTitle.textContent = newData.label;

        labelColor = doc.createElement("div");
        labelColor.setAttribute("class", "colorpie");
        labelColor.setAttribute("style", "background: " + newData.color + ";");

        labelValue = doc.createElement("div");
        labelValue.setAttribute("class", "num text-center");
        labelValue.textContent = newData.value;

        labelContent = doc.createElement("div");
        labelContent.setAttribute("class", "padt10 text-left");
        labelContent.appendChild(labelColor);
        labelContent.appendChild(labelTitle);
        labelContent.appendChild(labelValue);
        labelContent._label = newData.label;
        labelContent._index = labelIndex;
        labelContent._id = id.widgetId;
        labelContent._dashboardId = id.dashboardId;

        labelRoot.appendChild(labelContent);

        target.title.push(labelTitle);
        target.value.push(labelValue);
        target.color.push(labelColor);
    };

    var updateChart = function (targetChart, newData) {
        targetChart.data.labels = newData.label;
        targetChart.data.datasets[0].data = newData.value;

        if (!!newData.color) {
            targetChart.data.datasets[0].backgroundColor = newData.color;
        }
        targetChart.update();
    };

    function setSize(element, size){
        switch (size) {
            case WIDGET_WIDTH.SIZE_1X1:
                element.classList.add('col-md-4');
                element.classList.add('col-sm-6');
                element.classList.add('short');
                break;
            case WIDGET_WIDTH.SIZE_1X2: 
                element.classList.add('col-md-8');
                element.classList.add('col-sm-6');
                element.classList.add('short');
                break;
            case WIDGET_WIDTH.SIZE_1X3:
                element.classList.add('col-md-12');
                element.classList.add('col-sm-12');
                element.classList.add('short');
                break;
            case WIDGET_WIDTH.SIZE_2X2:
                element.classList.add('col-md-8');
                element.classList.add('col-sm-6');
                element.classList.add('mid');
                break;
            case WIDGET_WIDTH.SIZE_3X3:
                element.classList.add('col-md-12');
                element.classList.add('col-sm-12');
                element.classList.add('long');
                break;
            default:
                element.classList.add('col-md-4');
                element.classList.add('col-sm-6');
                element.classList.add('short');
                break;
          }
    }

    var createEmptyWidget = function (settingObj) {
        var doc = document;
        var card_box = doc.createElement("div");
        card_box.classList.add('cardbox');

        var panel_head = doc.createElement("div");
        panel_head.classList.add('panel-heading');

        var panel_head_row = doc.createElement("div");
        panel_head_row.classList.add('row');

        var panel_head_text = doc.createElement("div");
        panel_head_text.classList.add('col-md-8');
        panel_head_text.classList.add('col-sm-8');
        panel_head_text.classList.add('col-xs-8');
        panel_head_text.classList.add('col-lg-8');

        var span_card_name = doc.createElement('span')
        span_card_name.textContent = settingObj.name;

        panel_head_text.appendChild(span_card_name);

        var widgetTime = doc.createElement('div');
        widgetTime.classList.add('col-md-4');
        widgetTime.classList.add('col-sm-4');
        widgetTime.classList.add('col-xs-4');
        widgetTime.classList.add('col-lg-4');
        widgetTime.classList.add('widgetTime');

        panel_head_row.appendChild(panel_head_text);
        panel_head_row.appendChild(widgetTime);

        var rootDiv = doc.createElement('div');
        rootDiv.classList.add('col-md-12');
        rootDiv.appendChild(card_box);

        var rootWidget = doc.createElement('li');
        rootWidget.appendChild(rootDiv);
        rootWidget.classList.add('pad0');

        rootWidget.classList.add('widget');
        rootWidget.classList.add('col-xs-12');
        setSize(rootWidget, settingObj.width);
        rootWidget.id = `chart-${settingObj.id}`;

        panel_head.appendChild(panel_head_row);
        card_box.appendChild(panel_head);

        var panel_content = doc.createElement("div");
        panel_content.classList.add('panel-body');
        panel_content.classList.add('text-center');
        panel_content.classList.add('textcard');
        panel_content.classList.add('pad0');
        panel_content._title = name;
        panel_content.style.position = 'relative';

        card_box.appendChild(panel_content);

        return rootWidget;
    };

    var makeWidget = function (settingObj, outChart) {

        var target = settingObj.target;
        var name = settingObj.title;
        var chartId = settingObj.widgetId;
        var type = settingObj.type;
        var width = settingObj.width;
        var data = settingObj.data;
        var dashboardId = settingObj.dashboardId;
        var clickFlag = settingObj.clickFlag;
        var dragFlag = settingObj.dragFlag;
        var chartNameId = settingObj.chartNameId;
        var widget = $Card({
            widgetId: chartId,
            name: name,
            type: type,
            width: width,
            label: [''],
            data: data,
            dashboardId: dashboardId
        });
        widget.put(target, chartNameId, dragFlag, clickFlag, outChart);
        widget.render(clickFlag, outChart);
    };

    var makeWidgetBody = function (widgetBody, panel, options, outChartObj) {
        if (!panel.data) {
            return;
        }
        // var time = document.getElementById(`chart-${panel.id}`).getElementsByClassName('widgetTime')[0];

        // time.innerHTML = 'Now';
        var doc = document;
        switch (panel.type) {
            case 'bar':
                let makeBarChart = function (widgetBody, panel, options, outChartObj) {
                    var data = panel.data;
                    var dataLength = data.value.length;
                    var chartColor = colors(dataLength);
                    var widgetId = panel.id;
                    var dashboardId = options.dashboardId;
                    var chartContent = doc.createElement("canvas");
                    chartContent.id = `panel-chart-${panel.id}`;
                    widgetBody.appendChild(chartContent);
                    var bar = new Chart(chartContent, {
                        type: 'horizontalBar',
                        data: {
                            labels: data.label,
                            datasets: [
                                {
                                    label: data.name,
                                    backgroundColor: chartColor,
                                    data: data.value
                                }
                            ],
                            widgetId: widgetId,
                            dashboardId: dashboardId
                        },
                        options: {
                            legend: { display: false },
                            maintainAspectRatio: false,
                            title: {
                                display: false
                            },
                            scales: {
                                xAxes: [{
                                    scaleLabel: {
                                        display: false,
                                        labelString: data.name
                                    },
                                    ticks: {
                                        min: 0,
                                        fontColor: BG_COLOR_03,
                                    }
                                }],
                                yAxes: [{
                                    barThickness: 20, //bar height
                                    fontColor: BG_COLOR_03,
                                }]
                            }
                        },
                        plugins: [
                            {
                                afterDatasetsDraw: function (chart) {
                                    var ctx = chart.ctx;
                                    chart.data.datasets.forEach(function (dataset, i) {
                                        var meta = chart.getDatasetMeta(i);
                                        if (!meta.hidden) {
                                            meta.data.forEach(function (element, index) {
                                                // Draw the text in black, with the specified font
                                                ctx.fillStyle = 'rgb(255, 255, 255)';
                                                var fontSize = 14;
                                                var fontStyle = 'normal';
                                                var fontFamily = 'Helvetica Neue';
                                                ctx.font = Chart.helpers.fontString(fontSize, fontStyle, fontFamily);
                                                // Just naively convert to string for now
                                                var dataString = dataset.data[index].toString();
                                                // Make sure alignment settings are correct
                                                ctx.textAlign = 'center';
                                                ctx.textBaseline = 'middle';
                                                var padding = -10;
                                                var position = element.tooltipPosition();
                                                //ctx.fillText(dataString, position.x - (fontSize / 3) - 15 + 4, position.y - (fontSize / 2) - 2 - padding);
                                                ctx.fillText(dataString, position.x - dataString.length * 4.8, position.y - (fontSize / 2) - 2 - padding);
                                            });
                                        }
                                    });
                                }
                            }
                        ]
                    });
                    outChartObj.body = bar;
                    outChartObj.name = panel.name;
                    outChartObj.type = panel.type;

                    if (options.clickFlag) {
                        widgetId = panel.id;
                        dashboardId = options.dashboardId;
                        chartContent.setAttribute('style', 'cursor: pointer;');
                        chartContent.onclick = function (evt) {
                            var activePoints = bar.getElementsAtEvent(evt);
                            if (activePoints[0]) {
                                const $body = $('body');
                                let $title = $('#title');
                                let chartData = activePoints[0]['_chart'].config.data;
                                let idx = activePoints[0]['_index'];
                                let id = chartData.widgetId;
                                let dashboardId = chartData.dashboardId;
                                let APICaller = API();
                                $body.addClass('loading');
                                $title.html(panel.name + ` (${chartData.labels[idx]})`);
                                let promise = APICaller.GET('DashboardAPI/Widget/Detail?dashboardId=' + dashboardId + '&widgetId=' + id + '&index=' + idx);

                                promise.done((response, textStatus, xhr) => {
                                    if (xhr.status === 204) {
                                        alert('The widget has been removed.');
                                    }
                                    getSuccess(response, id, idx);
                                });
                            }
                        };
                    }
                };
                makeBarChart(widgetBody, panel, options, outChartObj);
                break;
            case 'doughnut':
            case 'pie':
                let makeDonutPieChart = function (widgetBody, panel, options, outChartObj) {
                    let labelRoot = doc.createElement("div");
                    labelRoot.classList.add('col-md-4');
                    labelRoot.classList.add('col-sm-4');
                    labelRoot.classList.add('col-xs-4');
                    labelRoot.classList.add('data');
                    labelRoot.classList.add('pad0');

                    outChartObj.label = {
                        value: [],
                        title: [],
                        color: []
                    };

                    var data = panel.data;
                    var dataLength = data.value.length;
                    var chartColor = colors(dataLength);
                    var widgetId = panel.id;
                    var dashboardId = options.dashboardId;
                    for (let i = 0; i < dataLength; i++) {
                        addLabel(outChartObj.label, labelRoot, {
                            value: data.value[i],
                            label: data.label[i],
                            color: chartColor[i]
                        }, i, {
                                widgetId: widgetId,
                                dashboardId: dashboardId
                            });
                    }

                    var chartContent = doc.createElement("canvas");
                    chartContent.id = `panel-chart-${panel.id}`;
                    chartContent.setAttribute("style", "width: 100%; height: auto;");

                    var chart = doc.createElement("div");
                    chart.setAttribute("class", "col-md-8 col-sm-8 col-xs-8 padt10per");
                    chart.appendChild(chartContent);

                    var root = doc.createElement("div");
                    root.appendChild(chart);
                    root.appendChild(labelRoot);

                    widgetBody.appendChild(root);
                    let chartOptionsObj = (panel.type === 'doughnut') ? {
                        responsive: true,
                        cutoutPercentage: 50,
                        animation: {
                            animateRotate: true,
                            animateScale: true
                        },
                        legend: false,
                    } : {
                            responsive: true,
                            legend: false,
                        };

                    let donut = new Chart(chartContent, {
                        type: 'pie',
                        data: {
                            labels: data.label,
                            datasets: [{
                                data: data.value,
                                backgroundColor: chartColor,
                                label: 'Dataset 1',
                            }],
                            widgetId: widgetId,
                            dashboardId: dashboardId
                        },
                        options: chartOptionsObj
                    });

                    if (options.clickFlag) {
                        labelRoot.style.cursor = 'pointer';
                        labelRoot._title = panel.name;
                        labelRoot.onclick = function (evt) {
                            let title = $('#title');
                            let APICaller = API();
                            const body = $('body');
                            let target = (evt.target._dashboardId === undefined) ? evt.target.parentNode : evt.target;

                            body.addClass('loading');
                            title.html(this._title + ` (${target._label})`);

                            let promise = APICaller.GET('DashboardAPI/Widget/Detail?dashboardId=' + target._dashboardId + '&widgetId=' + target._id + '&index=' + target._index);
                            promise.done((response, textStatus, xhr) => {
                                if (xhr.status === 204) {
                                    alert('The widget has been removed.');
                                }
                                getSuccess(response, target._id, target._index);
                            });
                        };
                        chartContent.setAttribute('style', 'cursor: pointer;');
                        chartContent.onclick = function (evt) {
                            var activePoints = donut.getElementsAtEvent(evt);

                            if (activePoints[0]) {
                                const body = $('body');
                                let title = $('#title');
                                let chartData = activePoints[0]['_chart'].config.data;
                                let idx = activePoints[0]['_index'];
                                let id = chartData.widgetId;
                                let dashboardId = chartData.dashboardId;
                                let APICaller = API();

                                body.addClass('loading');
                                title.html(panel.name + ` (${chartData.labels[idx]})`);
                                let promise = APICaller.GET('DashboardAPI/Widget/Detail?dashboardId=' + dashboardId + '&widgetId=' + id + '&index=' + idx);
                                promise.done((response, textStatus, xhr) => {
                                    if (xhr.status === 204) {
                                        alert('The widget has been removed.');
                                    }
                                    getSuccess(response, id, idx);
                                });
                            }
                        };
                    }
                    outChartObj.body = donut;
                    outChartObj.name = panel.name;
                    outChartObj.type = panel.type;
                };
                makeDonutPieChart(widgetBody, panel, options, outChartObj);
                break;
            case 'google map':
                let makeGoogleMap = async function (widgetBody, panel, options, outChartObj) {
                    var GoogleMapsLoader = require('google-maps');
                    GoogleMapsLoader.KEY = await getGoogleMapKey();
                    var googleMap = doc.createElement("div");
                    var data = panel.data;
                    googleMap.setAttribute("class", "map_google");
                    widgetBody.appendChild(googleMap);
                    GoogleMapsLoader.load((google) => {
                        var map = new google.maps.Map(googleMap, {
                            center: new google.maps.LatLng(data.centerLat, data.centerLng),
                            zoom: 15,
                            scrollwheel: false,
                            gestureHandling: 'greedy',
                            mapTypeId: google.maps.MapTypeId.ROADMAP
                        });

                        outChartObj.body = {
                            map: map,
                            marker: undefined
                        }
                        outChartObj.type = panel.type;
                        addGoogleMapMarker(data, outChartObj.body);
                    });

                };
                setTimeout(() => {
                    makeGoogleMap(widgetBody, panel, options, outChartObj);
                }, 700);
                break;
            case 'text':
                let makeTextChart = function (widgetBody, panel, options, outChartObj) {
                    const lable_color = ["title-success", "title-warning"];
                    const led_color = ["led-success", "led-none"];
                    const dataLength = 2;
                    const clickFlag = options.clickFlag;
                    var label, span, text, hr, widgetId, dashboardId;
                    var data = panel.data;
                    var labelArray = [];
                    var valueArray = [];
                    var root = doc.createElement("div");

                    root.setAttribute("class", "col-md-12 pad0");
                    if (clickFlag) {
                        root._title = panel.name;
                        root.setAttribute('style', 'cursor: pointer;');
                        widgetId = panel.id;
                        dashboardId = options.dashboardId;
                        root.onclick = function (evt) {
                            var title = $('#title');
                            var APICaller = API();
                            const body = $('body');
                            var target = (evt.target._dashboardId === undefined) ? evt.target.parentNode : evt.target;

                            body.addClass('loading');
                            title.html(this._title + ` (${target._label})`);

                            let promise = APICaller.GET('DashboardAPI/Widget/Detail?dashboardId=' + target._dashboardId + '&widgetId=' + target._id + '&index=' + target._index);

                            promise.done((response, textStatus, xhr) => {
                                if (xhr.status === 204) {
                                    alert('The widget has been removed.');
                                }
                                getSuccess(response, target._id, target._index);
                            });
                        };
                    }

                    for (let i = 0; i < dataLength; i++) {
                        label = doc.createElement("div");
                        label.setAttribute("class", "text_header " + lable_color[i]);

                        span = doc.createElement("span");
                        span.setAttribute("class", led_color[i]);

                        label.appendChild(span);

                        let labelTitle = doc.createElement('span');

                        labelTitle.textContent = data.label[i];

                        label.appendChild(labelTitle);
                        root.appendChild(label);

                        text = doc.createElement("div");
                        text.setAttribute("class", "text_bady");
                        text.textContent = data.value[i];

                        root.appendChild(text);

                        if (i != (data.value.length - 1)) {
                            //  the middle dash in the widget
                            hr = doc.createElement("div");
                            hr.setAttribute("class", "texthr");
                            root.appendChild(hr);
                        }

                        if (clickFlag) {
                            label._label = data.label[i];
                            label._index = i;
                            label._id = widgetId;
                            label._dashboardId = dashboardId;

                            text._index = i;
                            text._id = widgetId;
                            text._label = data.label[i];
                            text._dashboardId = dashboardId;
                        }

                        labelArray.push(labelTitle);
                        valueArray.push(text);
                    }

                    outChartObj.name = panel.name;
                    outChartObj.type = panel.type;
                    outChartObj.label = {
                        title: labelArray,
                        value: valueArray,
                        color: undefined
                    }
                    widgetBody.appendChild(root);
                };
                makeTextChart(widgetBody, panel, options, outChartObj);
                break;
            case 'vector map':
                let makeVectorMap = function (widgetBody, panel, options, outChartObj) {
                    var map_vector = doc.createElement("div");

                    map_vector.setAttribute("id", panel.id);
                    map_vector.setAttribute("class", "map_vector");
                    var $mapVector = $(map_vector);
                    widgetBody.appendChild(map_vector);

                    var data = panel.data;
                    var markers = (!!data.markers) ? data.markers : undefined;
                    var vetorMapSettingObj = {
                        scaleColors: ['#C8EEFF', '#0071A4'],
                        normalizeFunction: 'polynomial',
                        hoverOpacity: 0.7,
                        hoverColor: false,
                        backgroundColor: '#383f47',
                        regionStyle: {
                            initial: {
                                fill: '#b8e186'
                            },
                            selected: {
                                fill: '#8DA0CB'
                            },
                        },
                        markers: markers,
                        markerStyle: {
                            initial: {
                                fill: '#01fbd8',
                                stroke: '#505050',
                                "fill-opacity": 1,
                                "stroke-width": 1,
                                "stroke-opacity": 1,
                                r: 6
                            },
                            hover: {
                                stroke: 'black',
                                "stroke-width": 2,
                                cursor: 'pointer'
                            }
                        },
                    };

                    switch (data.mapIndex) {
                        case 1: //africa
                            require('../plugins/jquery-jvectormap-africa-mill');
                            vetorMapSettingObj['map'] = 'africa_mill';
                            break;
                        case 2: //asia
                            require('../plugins/jquery-jvectormap-asia-mill');
                            vetorMapSettingObj['map'] = 'asia_mill';
                            break;
                        case 3: //europe
                            require('../plugins/jquery-jvectormap-europe-mill');
                            vetorMapSettingObj['map'] = 'europe_mill';
                            break;
                        case 4: //north america
                            require('../plugins/jquery-jvectormap-north_america-mill');
                            vetorMapSettingObj['map'] = 'north_america_mill';
                            break;
                        case 5: //oceania
                            require('../plugins/jquery-jvectormap-oceania-mill');
                            vetorMapSettingObj['map'] = 'oceania_mill';
                            break;
                        case 6:  //south america
                            require('../plugins/jquery-jvectormap-south_america-mill');
                            vetorMapSettingObj['map'] = 'south_america_mill';
                            break;
                        case 7: //world
                            require('../plugins/jquery-jvectormap-world-mill');
                            vetorMapSettingObj['map'] = 'world_mill';
                            break;
                        default:  //world
                            require('../plugins/jquery-jvectormap-world-mill');
                            vetorMapSettingObj['map'] = 'world_mill';
                            break;
                    }

                    vetorMapSettingObj['onMarkerTipShow'] = function (event, label, index) {
                        var _msg = this._msg;
                        label.html(
                            '<b>Map : </b>' + _msg.alias + '<br/>' +
                            '<b>Device Count : </b>' + _msg.deviceCount[index] + '</br>' +
                            '<b>Event : </b>' + _msg.eventCount[index] + ' device(s)'
                        );
                    };

                    $mapVector.vectorMap(vetorMapSettingObj);
                    data.alias = getVectotMapName(data.mapIndex).alias;
                    $mapVector.vectorMap('get', 'mapObject').container[0]._msg = data;
                    outChartObj.body = $mapVector.vectorMap('get', 'mapObject');
                    outChartObj.type = panel.type;
                };
                setTimeout(() => {
                    makeVectorMap(widgetBody, panel, options, outChartObj);
                }, 700);
                break;
            case 'open street map':
                let makeOpenStreetMap = function (widgetBody, panel, options, outChartObj) {
                    var widgetId = panel.id;
                    var closer = doc.createElement('a');

                    closer.setAttribute('href', '#');
                    closer.setAttribute('id', `popup-closer-${widgetId}`);
                    closer.setAttribute('class', 'ol-popup-closer');

                    var content = doc.createElement('div');
                    content.setAttribute('id', `popup-content-${widgetId}`);

                    var element = doc.createElement('div');
                    element.setAttribute('id', `popup-${widgetId}`);
                    element.setAttribute('class', 'ol-popup');
                    element.appendChild(closer);
                    element.appendChild(content);

                    var osm = doc.createElement("div");
                    osm.setAttribute('id', `map-${widgetId}`);
                    osm.setAttribute('style', 'height: 100%;');
                    osm.setAttribute('class', 'map');

                    var fragment = doc.createDocumentFragment();

                    fragment.appendChild(osm);
                    fragment.appendChild(element);

                    widgetBody.appendChild(fragment);

                    var rasterLayer = new TileLayer({
                        source: new OSM()
                    });
                    var data = panel.data;

                    var vector = new VectorSource({
                        features: makeOpenStreetMapFeatures(data.value),
                    });

                    var vectorLayer = new VectorLayer({
                        source: vector
                    });

                    var popup = new Overlay({
                        element: element,
                        positioning: 'bottom-center',
                        stopEvent: true,
                        offset: [0, -40],
                        autoPan: true,
                        autoPanAnimation: {
                            duration: 250
                        }
                    });

                    closer.onclick = function () {
                        popup.setPosition(undefined);
                        closer.blur();
                        return false;
                    };

                    $(doc).ready(() => {
                        var map = new Map({
                            controls: defaultControls().extend([
                                new FullScreen()
                            ]),
                            layers: [rasterLayer, vectorLayer],
                            target: doc.getElementById(`map-${panel.id}`),
                            view: new View({
                                projection: 'EPSG:4326',
                                center: [data.centerLng, data.centerLat],
                                zoom: 16
                            })
                        });
                        var toolTipObj = undefined;

                        map.on('click', function (evt) {
                            var feature = map.forEachFeatureAtPixel(evt.pixel,
                                function (feature) {
                                    return feature;
                                }
                            );

                            if (feature) {
                                while (content.hasChildNodes()) {
                                    content.removeChild(content.firstChild);
                                }

                                var coordinate = feature.getGeometry().getCoordinates();
                                var itemArray = feature.get('itemArray');
                                let toolTipActor = toolTip();
                                toolTipObj = toolTipActor.make(itemArray);

                                content.appendChild(toolTipObj.fragment);
                                popup.setPosition(coordinate);
                            }
                        });

                        map.on('pointermove', function (e) {
                            if (e.dragging) {
                                $(element).popover('destroy');
                                return;
                            }
                            var pixel = map.getEventPixel(e.originalEvent);
                            var hit = map.hasFeatureAtPixel(pixel);

                            map.getTarget().style.cursor = hit ? 'pointer' : '';
                        });

                        map.addOverlay(popup);
                        outChartObj.type = panel.type;
                        outChartObj.name = panel.name;
                        outChartObj.body = {
                            map: map,
                        }
                    });
                };
                setTimeout(() => {
                    makeOpenStreetMap(widgetBody, panel, options, outChartObj);
                }, 700);
                break;
            case 'gaode map':
                setTimeout(() => {
                    var span = doc.createElement('span');
                    span.className = 'gaode-map-fullscreen-control';
                    var modeImage = doc.createElement('img');
                    modeImage.setAttribute('src', fullscreen);
                    modeImage.style.width = '18px';
                    span.appendChild(modeImage);

                    span.addEventListener('mouseover', () => {
                        modeImage.setAttribute('src', fullscreen_hover);
                    });

                    span.addEventListener('mouseout', () => {
                        modeImage.setAttribute('src', fullscreen);
                    });
                    widgetBody.appendChild(span);

                    var div = doc.createElement('div');
                    div.style.width = 'inherit';
                    div.style.height = 'inherit';
                    widgetBody.appendChild(div);
                    $(doc).ready(() => {
                        AMapLoader.load({
                            "key": "2f4973c80c6dba11febda0d955620c24",
                            "version": "1.4.15",
                            "plugins": []
                        }).then((AMap)=>{
                            var map = new AMap.Map(div, {
                                zoom: 15,
                                center: [panel.data.centerLng, panel.data.centerLat],
                                viewMode: '3D',
                                resizeEnable: true
                            });
                            AMap.plugin(['AMap.ToolBar', 'AMap.AdvancedInfoWindow'], function () {
                                var toolbar = new AMap.ToolBar({ liteStyle: true });
    
                                map.addControl(toolbar);
                            })
                            if (!!outChartObj) {
                                outChartObj.body = {
                                    map: map,
                                    marker: []
                                }
                                outChartObj.type = panel.type;
                                outChartObj.name = panel.name;
    
                                if (!panel.data.value || panel.data.value.length === 0) {
                                    outChartObj.body.marker.length = 0;
                                    return;
                                } else {
                                    addGaodeMapMarker(panel.data.value, outChartObj.body.map);
                                }
    
                            } else {
                                addGaodeMapMarker(panel.data.value, map);
                            }
    
                            var toFull = function (map) {
                                var fullscreen = doc.getElementById('amap-fullscreen');
                                var scrollPosition = doc.documentElement.scrollTop
                                doc.documentElement.scrollTop = 0;
                                doc.documentElement.style.overflow = 'hidden';
    
                                fullscreen.style.display = '';
                                let span = doc.createElement('span');
                                span.className = 'gaode-map-fullscreen-control';
                                let modeImage = doc.createElement('img');
                                modeImage.setAttribute('src', unfullscreen);
                                modeImage.style.width = '18px';
                                span.appendChild(modeImage);
    
                                let toHoverImg = function (modeImage) {
                                    modeImage.setAttribute('src', unfullscreen_hover);
                                };
                                span.addEventListener('mouseover', toHoverImg.bind(null, modeImage));
                                let toNormalImg = function (modeImage) {
                                    modeImage.setAttribute('src', unfullscreen);
                                };
                                span.addEventListener('mouseout', toNormalImg.bind(null, modeImage));
    
                                let unfullModeHandler = function (unfullIcon, scrollPosition) {
                                    doc.documentElement.scrollTop = scrollPosition;
                                    doc.documentElement.style.overflow = '';
                                    widgetBody.appendChild(map);
                                    fullscreen.style.display = 'none';
                                    fullscreen.mode = 0;
                                    unfullIcon.removeEventListener('click', unfullModeHandler);
                                    unfullIcon.removeEventListener('mouseover', toHoverImg);
                                    unfullIcon.removeEventListener('mouseout', toNormalImg);
                                    unfullIcon.parentNode.removeChild(unfullIcon);
                                };
                                span.addEventListener('click', unfullModeHandler.bind(null, span, scrollPosition));
    
                                fullscreen.appendChild(span);
                                fullscreen.appendChild(map);
                            }
                            span.addEventListener('click', toFull.bind(null, div));
                        }).catch(e => {
                            console.log(e);
                        })
                    });
                }, 700);
                break;
            case 'customized map':
                let makeCustomizedMap = function (widgetBody, panel, options, outChartObj) {
                    const promise = $.ajax({
                      type: 'GET',
                      url: `/WidgetAPI/CustomizedMap/Image?filePath=${panel.filepath}`,
                      async: true,
                      crossDomain: true,
                      headers: {
                        token: $.cookie('token'),
                      },
                      global: false
                    });
                    promise.done((response) => {
                      const doc = document;
                      const div = doc.createElement('div');
                      const state = {
                        markers: [],
                        isOpen: false,
                        leftBar: undefined,
                        rightBar: undefined,
                        menu: 0,
                        selected: -1,
                        filePath: panel.filepath,
                      };
                      class Marker {
                        constructor(marker, props) {
                          this.marker = marker;
                          this.props = props;
                          this.handleClick = () => {
                            if (state.selected === this.props.index) return;
                            if (state.selected !== -1) {
                              const lastOne = state.markers[state.selected];
                              if (lastOne.props.isNormal) {
                                lastOne.marker.innerHTML = `<img src=${MARKERGREEN}>`;
                              } else {
                                lastOne.marker.innerHTML = `<img src=${MARKERRED}>`;
                              }
                            }
                            state.selected = this.props.index;
                            let menu;
                            if (state.menu === 0) {
                              menu = state.leftBar;
                            } else if (state.menu === 1) {
                              menu = state.rightBar;
                            }
                            if (this.props.isNormal) {
                              this.marker.innerHTML = `<img src=${MARKERGREEN_SELECTED}>`;
                            } else {
                              this.marker.innerHTML = `<img src=${MARKERRED_SELECTED}>`;
                            }
                            const name = menu.querySelector('label');
                            const icon = menu.querySelector('.menu-body > .top > div');
                            const {devices} = this.props;
                            name.textContent = this.props.name;
                            if (this.props.isNormal) {
                              icon.className = 'normal';
                            } else {
                              icon.className = 'warning';
                            }
                            function initSelect() {
                              const $select = $(`#device-list-${panel.id}`);
                              $select.empty();
                              if (devices.length > 0) {
                                const content = menu.querySelector('.content');
                                content.classList.remove('hide');
                                forEach(devices, (device) => {
                                  $select.append($('<option></option>').attr('value', device.name).text((device.alias) ? device.alias : device.name));
                                });
                                $select.selectpicker('refresh');
                                $select.trigger('change');
                              } else {
                                const content = menu.querySelector('.content');
                                content.classList.add('hide');
                                $select.selectpicker('refresh');
                              }
                            }
                            initSelect();
                            if (!state.isOpen) {
                              const showSideBar = function () {
                                const body = doc.querySelector(`#chart-${panel.id} .panel-body`);
                                if (state.isOpen) return;
                                state.isOpen = true;
                                if (marker.x < body.clientWidth / 2) {
                                  if (state.menu === 0) {
                                    while (state.leftBar.hasChildNodes()) {
                                      state.rightBar.appendChild(state.leftBar.firstChild);
                                    }
                                    state.menu = 1;
                                  }
                                  state.rightBar.classList.add('active');
                                } else {
                                  if (state.menu === 1) {
                                    while (state.rightBar.hasChildNodes()) {
                                      state.leftBar.appendChild(state.rightBar.firstChild);
                                    }
                                    state.menu = 0;
                                  }
                                  state.leftBar.classList.add('active');
                                }
                              };
                              showSideBar(this.props);
                              state.isOpen = true;
                            }
                          };
                          marker.index = this.props.index;
                          marker.addEventListener('click', this.handleClick);
                        }
                  
                        remove() {
                          this.marker.removeEventListener('click', this.handleClick);
                          this.marker.parentElement.removeChild(this.marker);
                        }
                  
                        removeListener() {
                          this.marker.removeEventListener('click', this.handleClick);
                        }
                      }
                  
                      div.style.position = 'absolute';
                      div.style.top = '0px';
                      div.style.left = '0px';
                      div.x = 0; // padding
                      div.y = 0;
                      const img = doc.createElement('img');
                  
                      img.src = response.File;
                      div.appendChild(img);
                      function makerMarkers(markers) {
                        const fragment = doc.createDocumentFragment();
                        forEach(markers, (marker, idx) => {
                          const divMarker = doc.createElement('div');
                          divMarker.className = 'marker';
                          divMarker.style.position = 'absolute';
                          divMarker.style.top = `${marker.y}px`;// /beacuse of padding 5px;
                          divMarker.style.left = `${marker.x}px`;
                          divMarker.x = marker.x;
                          divMarker.y = marker.y;
                          if (marker.isNormal) {
                            divMarker.innerHTML = `<img src=${MARKERGREEN}>`;
                          } else {
                            divMarker.innerHTML = `<img src=${MARKERRED}>`;
                          }
                          state.markers.push(new Marker(divMarker, {
                            name: marker.name,
                            devices: marker.devices,
                            index: idx,
                            isNormal: marker.isNormal,
                          }));
                          fragment.appendChild(divMarker);
                        });
                        return fragment;
                      }
                      div.appendChild(makerMarkers(panel.data.markers));
                  
                      let drag = false;
                      const start = {
                        x: 0,
                        y: 0,
                      };
                      const handleMousedown = function (evt) {
                        evt.preventDefault();
                        start.x = evt.clientX;
                        start.y = evt.clientY;
                        drag = true;
                      };
                      div.addEventListener('mousedown', handleMousedown);
                      const handleMousemove = function (evt) {
                        if (!drag) {
                          return;
                        }
                        div.x += evt.clientX - start.x;
                        div.y += evt.clientY - start.y;
                        div.style.top = `${div.y}px`;
                        div.style.left = `${div.x}px`;
                        start.x = evt.clientX;
                        start.y = evt.clientY;
                      };
                      div.addEventListener('mousemove', handleMousemove);
                      const handleMouseup = function () {
                        drag = false;
                      };
                      div.addEventListener('mouseup', handleMouseup);
                  
                      function makeSideBar() {
                        const fragment = doc.createDocumentFragment();
                        const divRightBar = doc.createElement('div');
                        divRightBar.className = 'side-menu right';
                        fragment.appendChild(divRightBar);
                        const divLeftBar = doc.createElement('div');
                        divLeftBar.className = 'side-menu left';
                        divLeftBar.innerHTML = `<div class="menu-holder">
                                                    <button id='menu-controller-${panel.id}' class="menu" type="menu">
                                                      <i class="fa fa-bars"></i>
                                                    </button>
                                                    <p>Marker</p>
                                                  </div>
                                                  <div class="menu-body">
                                                    <div class='top'>
                                                      <div>
                                                        <label><label>
                                                      </div>
                                                    </div>
                                                    <div>
                                                      <div class='name-select'>Devices: </div>
                                                      <select id='device-list-${panel.id}' class='form-control selectpicker' data-size="6"  data-live-search="true" data-dropup-auto="false"></select>
                                                    </div>
                                                    <div class='content hide'>
                                                      <div><b></b></div>
                                                      <div class='item'>Status</div>
                                                      <div><b></b></div>
                                                      <div class='item'>Event Time</div>
                                                      <div><b></b></div>
                                                      <div class='item'>Message</div>
                                                    </div>
                                                  </div>`;
                        state.leftBar = divLeftBar;
                        state.rightBar = divRightBar;
                        fragment.appendChild(divLeftBar);
                        return fragment;
                      }
                      widgetBody.appendChild(makeSideBar());
                      $(doc).ready(() => {
                        const $select = $(`#device-list-${panel.id}`);
                        const controller = doc.querySelector(`#menu-controller-${panel.id}`);
                        if (!controller) return;
                        function handleClick() {
                          const lastOne = state.markers[state.selected];
                          if (lastOne.props.isNormal) {
                            lastOne.marker.innerHTML = `<img src=${MARKERGREEN}>`;
                          } else {
                            lastOne.marker.innerHTML = `<img src=${MARKERRED}>`;
                          }
                          state.isOpen = false;
                          state.selected = -1;
                          const menu = this.closest('.side-menu');
                          menu.classList.remove('active');
                        }

                        controller.addEventListener('click', handleClick);
                        $select.selectpicker();
                        function handleChange() {
                          const marker = state.markers[state.selected];
                          const device = marker.props.devices.find((item) => item.name === this.value);
                          let menu;
                          if (state.menu === 0) {
                            menu = state.leftBar;
                          } else if (state.menu === 1) {
                            menu = state.rightBar;
                          }
                          const fields = menu.querySelectorAll('b');
                          fields[0].textContent = device.status;
                          fields[1].textContent = (device.time === -1) ? 'N/A' : timeConverter(device.time);
                          fields[2].textContent = device.detail;
                        }
                        $select.on('change', handleChange);
                        widgetBody.removeListener = () => {
                          div.removeEventListener('mousedown', handleMousedown);
                          div.removeEventListener('mousemove', handleMousemove);
                          div.removeEventListener('mouseup', handleMouseup);
                          forEach(state.markers, (marker) => {
                            marker.removeListener();
                          });
                          controller.removeEventListener('click', handleClick);
                          $select.off('change', handleChange);
                        };
                        widgetBody.update = function (newPanel) {
                          function removeAllMarkers() {
                            if (state.markers.length === 0) return;
                            forEach(state.markers, (marker) => {
                              marker.remove();
                            });
                            state.markers.length = 0;
                            state.selected = -1;
                            if (state.isOpen) {
                              if (state.menu === 0) {
                                state.leftBar.classList.remove('active');
                              } else if (state.menu === 1) {
                                state.rightBar.classList.remove('active');
                              }
                              state.isOpen = false;
                            }
                          }
                          removeAllMarkers();
                          if (newPanel.filepath !== state.filePath) {
                            state.filePath = newPanel.filepath;
                            const getMap = $.ajax({
                              type: 'GET',
                              url: `/WidgetAPI/CustomizedMap/Image?filePath=${state.filePath}`,
                              async: true,
                              crossDomain: true,
                              headers: {
                                token: $.cookie('token'),
                              },
                              global: false,
                            });
                            // eslint-disable-next-line no-shadow
                            getMap.done((response) => {
                              const map = this.querySelector('img');
                              map.src = response.File;
                              div.appendChild(makerMarkers(newPanel.data.markers));
                            });
                            // eslint-disable-next-line no-shadow
                            getMap.fail((response) => {
                              if (response.status === 403
                                  && response.responseJSON.Response === RETURN_CODE.TOKEN_ERROR) {
                                fire(document, 'logout');
                              }
                            });
                          } else {
                            div.appendChild(makerMarkers(newPanel.data.markers));
                          }
                        };
                      });
                      widgetBody.appendChild(div);
                    });
                    promise.fail((response) => {
                      if (response.status === 403
                          && response.responseJSON.Response === RETURN_CODE.TOKEN_ERROR) {
                        fire(document, 'logout');
                      }
                    });
                    outChartObj.type = panel.type;
                    outChartObj.name = panel.name;
                    outChartObj.body = widgetBody;
                    // outChartObj.body = {
                    //     map: map,
                    // }
                };
                setTimeout(() => {
                    makeCustomizedMap(widgetBody, panel, options, outChartObj);
                }, 700);
                break;
        }
    };

    function addGaodeMapMarker(markerInfo, map) {
        sortMarker(markerInfo, [{
            prop: 'lat',
            direction: 1
        }, {
            prop: 'lng',
            direction: 1
        }]);

        var jumpCount = 0;
        markerInfo.forEach((device, idx) => {
            if (jumpCount !== 0) {
                jumpCount -= 1;
            } else {
                let setItem = [];
                let eventFlag = (device.status === 'NORMAL') ? false : true;
                setItem.push(device);

                if (idx !== markerInfo.length - 1) {
                    let next = 1;
                    while ((device.position.lat === markerInfo[idx + next].position.lat) && (device.position.lng === markerInfo[idx + next].position.lng)) {
                        setItem.push(markerInfo[idx + next]);
                        if (!eventFlag) {
                            eventFlag = (markerInfo[i + next].status === 'NORMAL') ? false : true;
                        }
                        jumpCount += 1;
                        next += 1;
                        if (idx + next === markerInfo.length) {
                            break;
                        }
                    };
                }
                AMapLoader.load({
                    "key": "2f4973c80c6dba11febda0d955620c24",
                    "version": "1.4.15",
                    "plugins": []
                }).then((AMap)=>{
                    let marker = new AMap.Marker({
                        position: new AMap.LngLat(device.position.lng, device.position.lat),
                        offset: new AMap.Pixel(-10.5, -27),
                        map: map,
                        icon: (eventFlag) ? MARKERRED : MARKERGREEN,
                    });
    
                    marker.content = setItem;
                    var infoWindow = new AMap.InfoWindow({
                        offset: new AMap.Pixel(0, -30),
                        isCustom: true,
                        anchor: 'bottom-center'
                    });
                    function createInfoWindow(fragment){
                        var doc = document;
                        var info = doc.createElement("div");
                        info.className = "custom-info input-card content-window-card";
                        var top = doc.createElement("div");
                        var titleD = doc.createElement("div");
                        titleD.style.backgroundColor = 'white';
                        top.className = "info-top";
                        var closeX = doc.createElement('span');
                        closeX.classList.add('ol-popup-closer');
                        closeX.onclick = function closeInfoWindow() {
                            map.clearInfoWindow();
                        };;
    
                        top.appendChild(titleD);
                        top.appendChild(closeX);
                        info.appendChild(top);
    
                        var middle = doc.createElement("div");
    
                        middle.className = "info-middle";
                        middle.style.backgroundColor = 'white';
                        middle.appendChild(fragment);
    
                        var sharp = doc.createElement("div");
                        sharp.className = 'diamond-sharp';
                        middle.appendChild(sharp);
    
                        info.appendChild(middle);
                        return info;
                    };
                    var markerClick = function (map, evt) {
                        infoWindow.setContent(createInfoWindow(toolTip().make(evt.target.content).fragment));
                        infoWindow.open(map, evt.target.getPosition());
                    }
                    
                    marker.on('click', markerClick.bind(null, map));
                }).catch(e => {
                    console.log(e);
                })
            }
        });
    }

    function toolTip() {
        function make(itemArray) {
            var doc = document;
            var deviceSelect = doc.createElement('select');
            deviceSelect.style.fontWeight = 'bold';
            deviceSelect.style.marginLeft = '10px';
            deviceSelect.style.marginRight = '10px';

            var toBlack = changeColor.bind(null, 'black');
            var toGray = changeColor.bind(null, '#708090');
            var toNext = toOption.bind(null, deviceSelect, 'next');
            var toForward = toOption.bind(null, deviceSelect, 'forward');

            var forwardIcon = doc.createElement('i');
            forwardIcon.setAttribute('aria-hidden', true);
            forwardIcon.classList.add('fa');
            forwardIcon.classList.add('fa-chevron-circle-left');
            forwardIcon.style.cursor = 'pointer';
            forwardIcon.style.color = '#708090';
            forwardIcon.addEventListener('mouseover', toBlack, false);
            forwardIcon.addEventListener('mouseout', toGray, false);
            forwardIcon.addEventListener('click', toForward, false);

            var nextIcon = doc.createElement('i');
            nextIcon.setAttribute('aria-hidden', true);
            nextIcon.classList.add('fa');
            nextIcon.classList.add('fa-chevron-circle-right');
            nextIcon.style.cursor = 'pointer';
            nextIcon.style.color = '#708090';
            nextIcon.addEventListener('mouseover', toBlack, false);
            nextIcon.addEventListener('mouseout', toGray, false);
            nextIcon.addEventListener('click', toNext, false);
            itemArray.forEach((item) => {
                let option = doc.createElement('option');
                option.textContent = (!!item.alias ? item.alias : item.name);
                option.value = item.name;
                deviceSelect.appendChild(option);
            });

            var fragment = doc.createDocumentFragment();

            fragment.appendChild(forwardIcon);
            fragment.appendChild(deviceSelect);
            fragment.appendChild(nextIcon);

            var div = doc.createElement('div');
            div.innerHTML = `<div><h5><span></span><br/><small>Status</small></h5></div>`;
            var status = div.getElementsByTagName('span')[0];
            fragment.appendChild(div);

            div = doc.createElement('div');
            div.innerHTML = `<div><h5><span></span><br/><small>Event time</small></h5></div>`;
            var time = div.getElementsByTagName('span')[0];
            fragment.appendChild(div);

            div = doc.createElement('div');
            div.innerHTML = `<div><h5><span></span><br/><small>Message</small></h5></div>`;
            var msg = div.getElementsByTagName('span')[0];
            fragment.appendChild(div);

            var firstItem = itemArray[0];

            status.textContent = firstItem.status;
            time.textContent = (firstItem.status != 'NORMAL') ? timeConverter(firstItem.time) : 'N/A';
            msg.textContent = firstItem.detail;

            var updateTooltipMsg = updateContent.bind(null, itemArray, {
                status: status,
                time: time,
                msg: msg
            });

            deviceSelect.addEventListener('change', updateTooltipMsg, false);

            return {
                fragment: fragment,
                select: deviceSelect,
                forward: forwardIcon,
                next: nextIcon,
                handler: {
                    updateTooltipMsg: updateTooltipMsg,
                    toBlack: toBlack,
                    toGray: toGray,
                    toNext: toNext,
                    toForward: toForward
                }
            };
        }

        function removeEvent(target) {
            target.select.removeEventListener('change', target.handler.updateTooltipMsg, false);
            target.forward.removeEventListener('mouseover', target.handler.toBlack, false);
            target.forward.removeEventListener('mouseout', target.handler.toGray, false);
            target.forward.removeEventListener('click', target.handler.toForward, false);
            target.next.removeEventListener('mouseover', target.handler.toBlack, false);
            target.next.removeEventListener('mouseout', target.handler.toGray, false);
            target.next.removeEventListener('click', target.handler.toNext, false);
            target = undefined;
        }
        return {
            make: make,
            removeEvent: removeEvent
        }
    }

    var addGoogleMapMarker = function (markerInfo, outInfoObj) {
        var markerArray = [];
        var OpenedInfoWindow = undefined;
        var toolTipObj = undefined

        if (!markerInfo.value) {
            outInfoObj.marker = markerArray
            return;
        }

        sortMarker(markerInfo.value, [{
            prop: 'lat',
            direction: 1
        }, {
            prop: 'lng',
            direction: 1
        }]);

        var jumpCount = 0;

        for (let i = 0; i < markerInfo.value.length; i++) {
            if (jumpCount !== 0) {
                jumpCount -= 1;
                continue;
            }

            let item = markerInfo.value[i];
            let setItem = [];
            let eventFlag = (item.status === 'NORMAL') ? false : true;
            setItem.push(item);

            if (i !== markerInfo.value.length - 1) {
                let next = 1;
                while ((item.position.lat === markerInfo.value[i + next].position.lat) && (item.position.lng === markerInfo.value[i + next].position.lng)) {
                    setItem.push(markerInfo.value[i + next]);
                    if (!eventFlag) {
                        eventFlag = (markerInfo.value[i + next].status === 'NORMAL') ? false : true;
                    }
                    jumpCount += 1;
                    next += 1;
                    if (i + next === markerInfo.value.length) {
                        break;
                    }
                };
            }

            markerArray[i] = new google.maps.Marker({
                position: item.position,
                map: outInfoObj.map,
                icon: (eventFlag) ? MARKERRED : MARKERGREEN,
                itemArray: setItem
            });

            markerArray[i].addListener('click', function (data) {
                var toolTipActor = toolTip();
                if (OpenedInfoWindow) {
                    OpenedInfoWindow.close();
                    OpenedInfoWindow = undefined;
                    toolTipActor.removeEvent(toolTipObj);
                }

                var itemArray = this.itemArray;

                toolTipObj = toolTipActor.make(itemArray);

                OpenedInfoWindow = new google.maps.InfoWindow({
                    content: toolTipObj.fragment,
                    position: this.position
                });
                OpenedInfoWindow.open(outInfoObj.map, markerArray[i]);
            });

            markerArray[i].addListener('mouseover', function (data) {
                if (data.xa !== undefined) {
                    data.xa.target.title = '';
                    data.xa.target.parentElement.removeAttribute('title');
                } else if (data.wa !== undefined) {
                    data.wa.target.title = '';
                    data.wa.target.parentElement.removeAttribute('title');
                } else if (data.ya !== undefined) {
                    data.ya.target.title = '';
                    data.ya.target.parentElement.removeAttribute('title');
                } else if (data.va !== undefined) {
                    data.va.target.title = '';
                    data.va.target.parentElement.removeAttribute('title');
                } else if (data.za !== undefined) {
                    data.za.target.title = '';
                    data.za.target.parentElement.removeAttribute('title');
                }
            });
        }
        outInfoObj.marker = markerArray;
    }

    var moveGoogleMapCenter = function (center, map) {
        var center = new google.maps.LatLng(center.lat, center.lng);

        map.panTo(center);
    };

    var makeOpenStreetMapFeatures = function (markerInfo) {
        var features = [];

        if (!markerInfo) {
            return features;
        }

        sortMarker(markerInfo, [{
            prop: 'lat',
            direction: 1
        }, {
            prop: 'lng',
            direction: 1
        }]);

        var jumpCount = 0;

        for (let i = 0; i < markerInfo.length; i++) {
            if (jumpCount !== 0) {
                jumpCount -= 1;
                continue;
            }

            let item = markerInfo[i];
            let setItem = [];
            let eventFlag = (item.status === 'NORMAL') ? false : true;
            setItem.push(item);

            if (i !== markerInfo.length - 1) {
                let next = 1;
                while ((item.position.lat === markerInfo[i + next].position.lat) && (item.position.lng === markerInfo[i + next].position.lng)) {
                    setItem.push(markerInfo[i + next]);
                    if (!eventFlag) {
                        eventFlag = (markerInfo[i + next].status === 'NORMAL') ? false : true;
                    }
                    jumpCount += 1;
                    next += 1;
                    if (i + next === markerInfo.length) {
                        break;
                    }
                };
            }

            let iconStyle;
            let feature = new Feature({
                geometry: new Point([item.position.lng, item.position.lat]),
                itemArray: setItem,
                autoPan: true,
                autoPanAnimation: {
                    duration: 250
                }
            });

            if (item.status === 'WARNING') {
                iconStyle = new Style({
                    image: new Icon(/** @type {module:ol/style/Icon~Options} */({
                        anchor: [0.5, 46],
                        anchorXUnits: 'fraction',
                        anchorYUnits: 'pixels',
                        src: MARKERRED
                    }))
                });
            } else {
                iconStyle = new Style({
                    image: new Icon(/** @type {module:ol/style/Icon~Options} */({
                        anchor: [0.5, 46],
                        anchorXUnits: 'fraction',
                        anchorYUnits: 'pixels',
                        src: MARKERGREEN
                    }))
                });
            }
            feature.setStyle(iconStyle);
            features.push(feature);
        }
        return features;
    }

    var makeVectorMapMarker = function (markerInfo) {
        return [
            {
                latLng: [markerInfo.position.lat, markerInfo.position.lng],
                style: { fill: markerInfo.color, r: 6 }
            }
        ];
    };

    var getVectotMapName = function (mapIndex) {
        switch (mapIndex) {
            case 1:
                return {
                    name: 'africa_mill',
                    alias: 'Africa',
                };
            case 2:
                return {
                    name: 'asia_mill',
                    alias: 'Asia',
                };
            case 3:
                return {
                    name: 'europe_mill',
                    alias: 'Europe',
                };
            case 4:
                return {
                    name: 'north_america_mill',
                    alias: 'North America',
                };
            case 5:
                return {
                    name: 'oceania_mill',
                    alias: 'Oceania',
                };
            case 6:
                return {
                    name: 'south_america_mill',
                    alias: 'Africa',
                };
            case 7:
                return {
                    name: 'world_mill',
                    alias: 'World',
                };
        }
    }

    var updateVectorMapTooltipMsg = function (container, data) {
        container._msg = data;
    };

    var destroy = function (removeTarget) {
        switch (removeTarget.type) {
            case 'doughnut':
            case 'pie':
            case 'bar':
                removeTarget.body.destroy();
                break;
            case 'googe map':
                break;
            case 'open street map':
                let map = removeTarget.body.map;
                let oldLayer = map.getLayers().getArray()[0];

                if (oldLayer) {
                    oldLayer.setSource(undefined);
                    map.removeLayer(oldLayer);
                }
                oldLayer = map.getLayers().getArray()[0];
                if (oldLayer) {
                    oldLayer.getSource().clear();
                    oldLayer.setSource(undefined);
                    map.removeLayer(oldLayer);
                }
                break;
            case 'vector map':
                removeTarget.body.remove();
                break;
            case 'gaode map':
                removeTarget.body.map.destroy();
                break;
            case 'customized map':
                removeTarget.body.removeListener && removeTarget.body.removeListener();
                while (removeTarget.body.hasChildNodes()) {
                    removeTarget.body.removeChild(removeTarget.body.firstChild);
                }
                break;
        }
    };

    var removeWidget = function (currentChart, removeCount) {
        let length = currentChart.length - removeCount;
        while (currentChart.length !== length) {
            let removeTarget = currentChart[currentChart.length - 1];
            destroy(removeTarget);
            currentChart.splice(-1, 1);
        }

        let layout = document.getElementById('dashboard-item');
        if (!layout) {
            return;
        }

        for (var i = 0; i < removeCount; i++) {
            layout.removeChild(layout.lastChild);
        }
    };

    var updateTime = function (widgetId, currentTimestamp) {

    };

    var addloadingHtml = function () {
        return `<div class='loading-wraper'>
                    <div class="loadGroup">
                        <div class="loadstyle"></div>
                        <div class="loadstyle"></div>
                        <div class="loadstyle"></div>
                        <div class="loadstyle"></div>
                        <div class="loadstyle"></div>
                        <p class="loadText">Loading</p>
                    </div>
                </div>`
    }

    return {
        updateLabel: updateLabel,
        addLabel: addLabel,
        makeWidget: makeWidget,
        updateChart: updateChart,
        makeWidgetBody: makeWidgetBody,
        createEmptyWidget: createEmptyWidget,
        googleMap: {
            addMarker: addGoogleMapMarker,
            moveCenter: moveGoogleMapCenter
        },
        opsnStreetMap: {
            makeFeatures: makeOpenStreetMapFeatures
        },
        vectorMap: {
            makeMarker: makeVectorMapMarker,
            getMapName: getVectotMapName,
            updateTooltipMsg: updateVectorMapTooltipMsg
        },
        gaodeMap: {
            addMarker: addGaodeMapMarker
        },
        destroy: destroy,
        removeWidget: removeWidget,
        updateTime: updateTime,
        addloadingHtml: addloadingHtml
    };
}

export function makeBarChart(rootId, chartId, chartNameId, chartName, label, widthString, data) {
    var chart = document.getElementById(rootId);
    var name = chartName;
    var color = colors(label.length);

    var barPreview = $Card({
        id: chartId,
        name: name,
        type: "bar",
        width: widthString,
        label: [''],
        data: [
            label,
            [
                {
                    name: 'Total',
                    data: data
                }
            ]
        ],
        color: color
    });
    barPreview.put(chart, chartNameId);
    barPreview.render();
}

export function makeLabel(dataInterval, limitTwo, unit) {
    var label = [];
    var deduped = dataInterval.filter((el, i, arr) => arr.indexOf(el) === i); // remove repeate number 
    if (unit == null) {
        unit = '%';
    }
    else if (unit == 'null') {
        unit = '';
    }

    if (deduped.length == 0) {
        //Default
        if (true == limitTwo) {
            label = ['≥ A ' + unit, "< A " + unit];
        }
        else {
            label = ['≥ A ' + unit, 'B ' + unit + ' - ' + 'A ' + unit, 'C ' + unit + ' - ' + 'B ' + unit, 'D ' + unit + ' - ' + 'C ' + unit, 'E ' + unit + ' - ' + 'D ' + unit, "< E " + unit];
        }
    }
    else if (deduped.length == 1) {
        label[0] = '≥ ' + deduped[0] + unit;
        label[1] = '< ' + deduped[0] + unit;
    }
    else {
        label[0] = '≥ ' + deduped[0] + unit;
        for (var i = 1; i <= deduped.length; i++) {
            if (i != deduped.length) {
                label[i] = deduped[i] + unit + ' - ' + deduped[i - 1] + unit;
            }
            else {
                label[i] = '< ' + deduped[i - 1] + unit;
            }
        }
    }

    return label;
}

export function makeMockDonutChart(rootId, chartId, chartNameId, chartName, label, widthString) {
    var chart = document.getElementById(rootId);
    var name = chartName;
    var data = makeRandomData(label.length, 50, 100);
    var color = colors(label.length);
    var dataNormalize = {
        label: label,
        value: data,
        name: ''
    };
    var donutPreview = $Card({
        id: chartId,
        name: name,
        type: "doughnut",
        width: widthString,
        label: "",
        data: dataNormalize,
        color: color
    });
    donutPreview.put(chart, chartNameId);
    donutPreview.render();
}

export function makeDonutChart(rootId, chartId, chartNameId, chartName, label, widthString, data) {
    var chart = document.getElementById(rootId);
    var name = chartName;
    var color = colors(label.length);
    var barPreview = $Card({
        id: chartId,
        name: name,
        type: "donut",
        width: widthString,
        label: "",
        data: data,
        color: color
    });
    barPreview.put(chart, chartNameId);
    barPreview.render();
}

export function makeMockPieChart(rootId, chartId, chartNameId, chartName, label, widthString) {
    var chart = document.getElementById(rootId);
    var name = chartName;
    var data = makeRandomData(label.length, 50, 100);
    var color = colors(label.length);
    var dataNormalize = {
        label: label,
        value: data,
        name: ''
    };
    var barPreview = $Card({
        id: chartId,
        name: name,
        type: "pie",
        width: widthString,
        label: [''],
        data: dataNormalize,
        color: color
    });
    barPreview.put(chart, chartNameId);
    return barPreview.render();
}

export function makePieChart(rootId, chartId, chartNameId, chartName, label, widthString, data) {
    var chart = document.getElementById(rootId);
    var name = chartName;
    var color = colors(label.length);
    var barPreview = $Card({
        id: chartId,
        name: name,
        type: "pie",
        width: widthString,
        label: [''],
        data: data,
        color: color
    });
    barPreview.put(chart, chartNameId);
    return barPreview.render();
}

export function makeMockTextChart(rootId, chartId, chartNameId, chartName, label, widthString) {
    var chart = document.getElementById(rootId);
    var name = chartName;
    var data = makeRandomData(label.length, 50, 100);
    var color = colors(label.length);
    var dataNormalize = {
        label: label,
        value: data,
        name: ''
    };
    var barPreview = $Card({
        id: chartId,
        name: name,
        type: "text",
        width: widthString,
        label: [''],
        data: dataNormalize,
        color: color
    });
    barPreview.put(chart, chartNameId);
    barPreview.render();
}

export function makeTextChart(rootId, chartId, chartNameId, chartName, label, widthString, data) {
    var chart = document.getElementById(rootId);
    var name = chartName;
    var color = colors(label.length);
    var barPreview = $Card({
        id: chartId,
        name: name,
        type: "text",
        width: widthString,
        label: [''],
        data: data,
        color: color
    });
    barPreview.put(chart, chartNameId);
    barPreview.render();
}

export function makeMapChart(rootId, chartId, chartNameId, chartName, label, widthString, data) {
    var chart = document.getElementById(rootId);
    var color = colors(label.length);
    var barPreview = $Card({
        id: chartId,
        name: chartName,
        type: "google map",
        width: widthString,
        label: null,
        data: data,
        color: color
    });
    barPreview.put(chart, chartNameId);
    barPreview.render();
}