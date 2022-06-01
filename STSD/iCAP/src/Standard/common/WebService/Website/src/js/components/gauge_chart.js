import {
    HIGHLIGHT_COLOR,
    CHART_BASE_COLOR_A
} from "../constants/globalVariable"

(function(global, $){
    var Element = function(panel){
        return new Element.init(panel)
    }

    Element.init = function(panel){
        var self = this;
        self.panel = panel;
    }

    Element.prototype = {
        getDOM: function()
        {
            var self = this;
            var chart_content = document.createElement("canvas");
            chart_content.setAttribute("id", "panel-chart-" + self.panel.id);
            
            return chart_content;
        },
        renderChart: function()
        {
            var self = this;
            var chart_content = document.getElementById("panel-chart-" + self.panel.id);       

            new Chart(chart_content, {
                type: 'pie',
                data: {
                    labels: self.panel.label,
                    datasets: [{
                        data: self.panel.data,
                        backgroundColor: [HIGHLIGHT_COLOR, CHART_BASE_COLOR_A],
                        label: 'Dataset 1'
                        }]
                    },
                    options: {
                        responsive: true,
                        cutoutPercentage: 70,
                        rotation: -1.0 * Math.PI,
                        circumference: Math.PI,
                        title: {
                            "display": true,
                            "text": self.panel.label[2],  
                            "position": "bottom",
                            "fontSize" :'20'          
                        },
                        legend: { display: false },
                    }
                }
              );             
        }
    }

    Element.init.prototype = Element.prototype;

    global.$GaugeChart = Element;

})(window, jQuery)