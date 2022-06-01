import { colors } from "../library/color";
import { API } from "../library/api_handler";
import { getSuccess } from "../library/card_click_event";
import { BG_COLOR_03 } from "../constants/globalVariable"

export function barChart() {
    function getDOM(id) {
        var chart = document.createElement("canvas");

        chart.setAttribute("id", "panel-chart-" + id);

        return chart;
    }

    function renderChart(panel, clickFlag, outChart) {
        var chartContent = document.getElementById("panel-chart-" + panel.id);
        var barChartColor = [];
        var widgetId, dashboardId;
        var data = panel.data;

        if (Boolean(panel.color) === true) {
            barChartColor = panel.color;
        } else {
            barChartColor = colors(data.label.length);
        }

        if (clickFlag === true) {
            widgetId = panel.id;
            dashboardId = panel.dashboardId;
        }

        var bar = new Chart(chartContent, {
            type: 'horizontalBar',
            data: {
                labels: data.label,
                datasets: [
                    {
                        label: data.name,
                        backgroundColor: barChartColor,
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
                        //type: 'linear',
                        //position: 'bottom',
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
                        //barPercentage: 0.6 
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

        outChart && outChart.push({
            name: panel.name,
            widgetId: widgetId,
            type: 'bar',
            body: bar
        });


        if (true === clickFlag) {
            chartContent.setAttribute('style', 'cursor: pointer;');
            chartContent.onclick = function (evt) {
                var activePoints = bar.getElementsAtEvent(evt);
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
    }

    var publicAPI = {
        getDOM: getDOM,
        renderChart: renderChart
    };

    return publicAPI;
}