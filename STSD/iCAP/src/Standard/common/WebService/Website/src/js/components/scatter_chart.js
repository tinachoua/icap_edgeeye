import {
    colors
} from "../library/color"

(function(global, $){
    var Element = function(panel){
        return new Element.init(panel)
    }

    var panel;

    var pointset = [
        'crossRot',
        'circle',
        'rect',
        'triangle'
    ];

    Element.init = function(panel){
        var self = this;
        self.panel = panel;
    }

    Element.prototype = {
        getDOM: function()
        {
            var self = this;
            
            var chart_canvas = document.createElement("canvas");
            chart_canvas.setAttribute("id", "panel-chart-" + self.panel.id);
            
            return chart_canvas;
        },
        renderChart: function()
        {
            var self = this;
            var data = self.panel.data
            var dataset = [];
            var chart_content = document.getElementById("panel-chart-" + self.panel.id);
            const colorArray = colors(4);
            for(let i = 0; i < 4; i++) {
                dataset[i] = {
                    label: data.Items[i],
                    data: data.Value[i],
                    pointStyle: pointset[i],
                    backgroundColor: colorArray[i],
                    borderColor: colorArray[i],
                    borderWidth: 2,
                    radius: 4
                }
            }
            new Chart(chart_content, {
                type: 'scatter',
                data: {
                    datasets: dataset
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    legend: {
                        display: true,
                        position: 'bottom',
                        labels:{
                            usePointStyle: true,
                            fontSize: 12,
                            fontColor: "#999",
                            padding: 5
                        }
                    },
                    scales: {
                        xAxes: [{
                            type: 'linear',
                            position: 'bottom',
                            scaleLabel: {
                                display: true,
                                labelString: self.panel.label[0],
                                fontSize: 12,
                                padding: 0,
                                fontColor: "#999"
                            }
                        }],
                        yAxes: [{
                            type: 'linear',
                            position: 'bottom',
                            scaleLabel: {
                                display: true,
                                labelString: self.panel.label[1],
                                fontSize: 12,
                                padding: 0,
                                fontColor: "#999"
                            },
                        }]
                    }
                }
            });
        }
    }

    Element.init.prototype = Element.prototype;

    global.$ScatterChart = Element;

})(window, jQuery)