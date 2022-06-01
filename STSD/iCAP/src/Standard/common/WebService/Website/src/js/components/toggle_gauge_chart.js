// Reference for centering label for pie/dount chart: 
// https://stackoverflow.com/questions/20966817/how-to-add-text-inside-the-doughnut-chart-using-chart-js
import {
  HIGHLIGHT_COLOR,
  CHART_BASE_COLOR_A
} from "../constants/globalVariable"


(function(global, $){
  var Element = function(panel){
      return new Element.init(panel)
  }

  var panel;
  var chart_obj;

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
          var chart_content = document.getElementById("panel-chart-" + self.panel.id);

          Chart.pluginService.register({
            beforeDraw: function (chart) {
              if (chart.config.options.elements.center) {
                //Get ctx from string
                var ctx = chart.chart.ctx;
                
                //Get options from the center object in options
                var centerConfig = chart.config.options.elements.center;
                var fontStyle = centerConfig.fontStyle || 'Arial';
                var txt = centerConfig.text;
                var color = centerConfig.color || '#000';
                var sidePadding = centerConfig.sidePadding || 40;
                var sidePaddingCalculated = (sidePadding / 100) * (chart.innerRadius * 2)
                //Start with a base font of 30px
                ctx.font = "30px " + fontStyle;
                
                //Get the width of the string and also the width of the element minus 10 to give it 5px side padding
                var stringWidth = ctx.measureText(txt).width;
                var elementWidth = (chart.innerRadius * 2) - sidePaddingCalculated;
        
                // Find out how much the font can grow in width.
                var widthRatio = elementWidth / stringWidth;
                var newFontSize = Math.floor(30 * widthRatio);
                var elementHeight = (chart.innerRadius * 2);
        
                // Pick a new font size so it will not be larger than the height of label.
                var fontSizeToUse = 30;
        
                //Set font settings to draw it correctly.
                ctx.textAlign = 'center';
                ctx.textBaseline = 'middle';
                var centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
                var centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2) * 1.3;
                ctx.font = fontSizeToUse+"px " + fontStyle;
                ctx.fillStyle = color;
                
                //Draw text in center
                ctx.fillText(txt, centerX, centerY);
              }
              if (chart.config.options.elements.roundlabel){
                var roundlabelConfig = chart.config.options.elements.roundlabel;
                var fontStyle = centerConfig.fontStyle || 'Arial';
                var max = roundlabelConfig.max;
                var fifty = roundlabelConfig.min + (roundlabelConfig.max - roundlabelConfig.min) * 0.5;
                var min = roundlabelConfig.min;
                var centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);

                ctx.font = "15px " + fontStyle;


                ctx.fillText(min, chart.chartArea.left * 0.8, chart.chartArea.bottom * 0.95);
                ctx.fillText(max, chart.chartArea.right * 1.08, chart.chartArea.bottom * 0.95);
                ctx.fillText(fifty, centerX, chart.chartArea.top * 0.8);

              }
            }
          });

          chart_obj = new Chart(chart_content, {
            type: 'pie',
            data: {
                labels: self.panel.label,
                datasets: [{
                    data: [self.panel.data[0], self.panel.data[2] - self.panel.data[0]],
                    backgroundColor: [HIGHLIGHT_COLOR, CHART_BASE_COLOR_A],
                    label: 'Dataset 1'
                    }]
                },
                options: {
                    responsive: true,
                    cutoutPercentage: 80,
                    rotation: 1 * Math.PI,
                    circumference: Math.PI,
                    legend: { display: false },
                    tooltips:{
                      enabled: false
                    },
                    layout:{
                      padding:{
                        top: 50,
                        left: 50,
                        right: 50,
                        bottom: 0
                      }
                    },
                    elements:{
                      center:{
                        text: self.panel.data[0] + " " + self.panel.label[0],
                        color: '#444444',
                      },
                      roundlabel:{
                        max: self.panel.data[2],
                        min: self.panel.data[1]
                      }
                    }
                }
            }
          );

          return chart_obj;
      }
  }

  Element.init.prototype = Element.prototype;

  global.$ToggleGaugeChart = Element;

})(window, jQuery)