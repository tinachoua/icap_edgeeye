import { colors } from "../library/color";
import { API } from "../library/api_handler";
import { getSuccess } from "../library/card_click_event";

export function pieChart() {
    function getDOM(panel, clickFlag, outChart) {
        if (Boolean(panel.data) === true) {
            let label = document.createElement("div");
            let widgetId, dashboardId;
            let data = panel.data;
            let valueLength = data.value.length;
            let pieColor = colors(valueLength);
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

            for (let i = 0; i < valueLength; i++) {
                let labelContent, labelTitle, labelColor, labelValue;

                labelContent = document.createElement("div");
                labelContent.setAttribute("class", "padt10 text-left");

                labelTitle = document.createElement("small");

                labelColor = document.createElement("div");
                labelColor.setAttribute("class", "colorpie");
                labelColor.setAttribute("style", "background: " + pieColor[i] + ";");

                // labelTitle.textContent = panel.data[i][0];
                labelTitle.textContent = data.label[i];

                labelValue = document.createElement("div");
                labelValue.setAttribute("class", "num text-center");
                // labelValue.textContent = panel.data[i][1];
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

                if (outChart) {
                    domContent.label.title.push(labelTitle);
                    domContent.label.color.push(labelColor);
                    domContent.label.value.push(labelValue);
                }
            }

            var root = document.createElement("div");
            var chart = document.createElement("div");
            var chartContent = document.createElement("canvas");

            chart.setAttribute("class", "col-md-8 col-sm-8 col-xs-8 padt10per");
            chartContent.setAttribute("id", "panel-chart-" + panel.id);
            chartContent.setAttribute("style", "width: 100%; height: auto;");

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
                    // let promise = APICaller.GET('DashboardAPI/Widget/Details?id=' + evt.target._id + '&' + 'index=' + evt.target._index);
                    let promise = APICaller.GET('DashboardAPI/Widget/Detail?dashboardId=' + target._dashboardId + '&widgetId=' + target._id + '&index=' + target._index);
                    promise.done((response, textStatus, xhr) => {
                        if (xhr.status === 204) {
                            alert('The widget has been removed.');
                        }
                        getSuccess(response, evt.target._id, evt.target._index);
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
        var pieColor = colors(data.value.length);
        var widgetId, dashboardId;
        var pie;

        if (Boolean(panel.data) === true) {
            if (clickFlag === true) {
                widgetId = panel.id;
                dashboardId = panel.dashboardId;
            }

            pie = new Chart(chartContent, {
                type: 'pie',
                data: {
                    labels: data.label,
                    datasets: [{
                        data: data.value,
                        backgroundColor: pieColor,
                        label: 'Dataset 1'
                    }],
                    widgetId: widgetId,
                    dashboardId: dashboardId
                },
                options: {
                    responsive: true,
                    legend: false,
                }
            });

            if (true === clickFlag) {
                chartContent.setAttribute('style', 'cursor: pointer;');
                chartContent.onclick = function (evt) {
                    let activePoints = pie.getElementsAtEvent(evt);

                    if (activePoints[0]) {
                        const body = $('body');
                        let title = $('#title');
                        let chartData = activePoints[0]['_chart'].config.data;
                        let idx = activePoints[0]['_index'];
                        let id = chartData.widgetId;
                        let APICaller = API();
                        let dashboardId = chartData.dashboardId;

                        body.addClass('loading');
                        title.html(panel.name + ` (${chartData.labels[idx]})`);
                        // let promise = APICaller.GET('DashboardAPI/Widget/Details?id=' + id + '&' + 'index=' + idx);
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
            pie = new Chart(chartContent, {
                type: 'pie',
                data: {
                    labels: [],
                    datasets: [{
                        data: [],
                        backgroundColor: pieColor,
                        label: 'Dataset 1'
                    }]
                },
                options: {
                    responsive: true,
                    legend: false,
                }
            });
        }

        if (!!outChart) {
            let target = outChart[outChart.length - 1];

            target.name = panel.name;
            target.type = 'pie';
            target.body = pie;
        }
    }

    var publicAPI = {
        getDOM: getDOM,
        renderChart: renderChart
    };

    return publicAPI;
}