import { colors } from "../library/color";
import { API } from "../library/api_handler";
import { getSuccess } from "../library/card_click_event";

export function donutChart() {
    function getDOM(panel, clickFlag, outChart) {
        if (Boolean(panel.data) === true) {
            let label = document.createElement("div");
            let widgetId, dashboardId;
            let data = panel.data;
            let dataLength = data.value.length;
            let chartColor = colors(dataLength);
            let domContent = {
                widgetId: panel.id,
                label: {
                    title: [],
                    value: [],
                    color: [],
                }
            };

            label.setAttribute("class", "col-md-4 col-sm-4 col-xs-4 date pad0");

            if (clickFlag === true) {
                label.setAttribute('style', 'cursor: pointer;');
                label._title = panel.name;
                widgetId = panel.id;
                dashboardId = panel.dashboardId;
            }


            for (let i = 0; i < dataLength; i++) {
                let labelContent, labelTitle, labelValue, labelColor;

                labelContent = document.createElement("div");
                labelContent.setAttribute("class", "padt10 text-left");

                labelTitle = document.createElement("small");
                labelTitle.textContent = data.label[i];

                labelColor = document.createElement("div");
                labelColor.setAttribute("class", "colorpie");
                labelColor.setAttribute("style", "background: " + chartColor[i] + ";");

                labelValue = document.createElement("div");
                labelValue.setAttribute("class", "num text-center");
                labelValue.textContent = data.value[i];

                labelContent.appendChild(labelColor);
                labelContent.appendChild(labelTitle);
                labelContent.appendChild(labelValue);
                label.appendChild(labelContent);

                if (clickFlag === true) {
                    labelContent._label = data.label[i];
                    labelContent._index = i;
                    labelContent._id = widgetId;
                    labelContent._dashboardId = dashboardId;
                }

                if (Boolean(outChart) === true) {
                    domContent.label.title.push(labelTitle);
                    domContent.label.color.push(labelColor);
                    domContent.label.value.push(labelValue);
                }
            }

            let root = document.createElement("div");
            let chart = document.createElement("div");
            let chartContent = document.createElement("canvas");

            chart.setAttribute("class", "col-md-8 col-sm-8 col-xs-8 padt10per");
            chartContent.setAttribute("id", "panel-chart-" + panel.id);
            chartContent.setAttribute("style", "width: 100%; height: auto;")

            chart.appendChild(chartContent);
            root.appendChild(chart);
            root.appendChild(label);

            if (true === clickFlag) {
                label.onclick = function (evt) {
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

            outChart && outChart.push(domContent);
            return root;
        }
    }

    function renderChart(panel, clickFlag, outChart) {
        var data = panel.data;
        var chartContent = document.getElementById("panel-chart-" + panel.id);
        var chartColor = colors(data.value.length);
        var widgetId, dashboardId;
        var donut;

        if (Boolean(panel.data) === true) {
            if (clickFlag === true) {
                widgetId = panel.id;
                dashboardId = panel.dashboardId;
            }

            donut = new Chart(chartContent, {
                type: 'pie',
                data: {
                    // labels: data.label,
                    labels: data.label,
                    datasets: [{
                        data: data.value,
                        backgroundColor: chartColor,
                        label: 'Dataset 1',
                    }],
                    widgetId: widgetId,
                    dashboardId: dashboardId
                },
                options: {
                    responsive: true,
                    cutoutPercentage: 50,
                    animation: {
                        animateRotate: true,
                        animateScale: true
                    },
                    legend: false,
                    //onClick: Chart_OnClick
                }
            });

            if (true === clickFlag) {
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
        } else {
            // No Data
            donut = new Chart(chartContent, {
                type: 'pie',
                data: {
                    labels: [],
                    datasets: [{
                        data: [],
                        label: 'Dataset 1'
                    }]
                },
                options: {
                    responsive: true,
                    cutoutPercentage: 50,
                    animation: {
                        animateRotate: true,
                        animateScale: true
                    },
                    legend: false,
                }
            });
        }

        if (!!outChart) {
            let target = outChart[outChart.length - 1];

            target.type = 'doughnut';
            target.body = donut;
            target.name = panel.name;
        }
    }

    var publicAPI = {
        getDOM: getDOM,
        renderChart: renderChart
    };

    return publicAPI;
}