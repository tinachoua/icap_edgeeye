(function (global, $) {
    var Element = function (panel) {
        return new Element.init(panel)
    }

    var panel;

    Element.init = function (panel) {
        var self = this;
        self.panel = panel;
    }

    Element.prototype = {
        getDOM: function () {
            var self = this;
            var chart_content = document.createElement("canvas");
            chart_content.setAttribute("id", "panel-chart-" + self.panel.id);

            return chart_content;
        },
        renderChart: function () {
            var self = this;
            var chart_content = document.getElementById("panel-chart-" + self.panel.id);
            var label = [];
            var max = 0;
            var min = 500;
            var toolTipTitle = [];

            for (var i = 0; i < self.panel.data[0].length; i++) {
                let time = new Date(self.panel.data[0][i] * 1000);

                label[i] = time.getHours() + ':' + time.getMinutes();
                toolTipTitle[i] = new Date(time);
            }

            self.panel.data[1].forEach && self.panel.data[1].forEach(x => {
                if (max < x) {
                    max = x;
                }
                if (min > x) {
                    min = x;
                }
            })

            return new Chart(chart_content, {
                type: 'line',
                data: {
                    labels: label,
                    datasets: [
                        {
                            borderColor: color_def[0][0],
                            pointColor: color_def[0][0],
                            data: self.panel.data[1],
                            title: toolTipTitle
                        }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    legend: {
                        display: false
                    },
                    scales: {
                        yAxes: [{
                            type: 'linear',
                            position: 'bottom',
                            scaleLabel: {
                                display: true,
                                labelString: self.panel.label[0]
                            },
                            ticks: {
                                min: Math.floor((min - 10) / 10) * 10,
                                max: Math.ceil((max + 10) / 15) * 20
                            }
                        }]
                    },
                    tooltips: {
                        callbacks: {
                            title: function (tooltipItem, data) {
                                return data.datasets[0].title[tooltipItem[0].index];
                            }
                        }
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
                                        ctx.fillStyle = 'rgb(0, 0, 0)';
                                        var fontSize = 12;
                                        var fontStyle = 'normal';
                                        var fontFamily = 'Helvetica Neue';
                                        ctx.font = Chart.helpers.fontString(fontSize, fontStyle, fontFamily);
                                        // Just naively convert to string for now
                                        var dataString = dataset.data[index].toString();
                                        // Make sure alignment settings are correct
                                        ctx.textAlign = 'center';
                                        ctx.textBaseline = 'middle';
                                        var padding = 5;
                                        var position = element.tooltipPosition();
                                        ctx.fillText(dataString, position.x, position.y - (fontSize / 2) - padding);
                                    });
                                }
                            });
                        }
                    }
                ]
            });
        }
    }

    Element.init.prototype = Element.prototype;

    global.$LineChart = Element;

})(window, jQuery)