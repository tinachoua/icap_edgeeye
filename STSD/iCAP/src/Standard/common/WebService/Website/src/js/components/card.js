import { donutChart } from './donut_chart';
import { barChart } from './bar_chart';
import { pieChart } from './pie_chart';
import { textChart } from './text_card';
import { vectorMap } from './jvector_map';
import { openStreetMap } from './open_street_map';
require("./gauge_chart");
require("./line_chart");
require("./google_map");
require("./scatter_chart");
require("./toggle_gauge_chart");
var index = 0;

(function (global, $) {

    var Card = function (panel, center_longitude, center_latitude) {
        return new Card.init(panel, center_longitude, center_latitude)
    };

    Card.init = function (panel, center_longitude, center_latitude) {
        var self = this;
        self.panel = panel;
        self.center_longitude = center_longitude;
        self.center_latitude = center_latitude;
    };

    function GetPanel(panel, center_longitude, center_latitude, clickFlag, outChart) {
        var p;
        switch (panel.type) {
            case 'bar':
                p = barChart();
                return p.getDOM(panel.id, clickFlag);
            case 'doughnut':
                p = donutChart();
                return p.getDOM(panel, clickFlag, outChart);
            case 'gauge':
                p = $GaugeChart(panel);
                return p.getDOM();
            case 'line':
                p = $LineChart(panel);
                return p.getDOM();
            case 'google map':
                p = $MapCard(panel, center_longitude, center_latitude);
                return p.getDOM();
            case 'pie':
                p = pieChart();
                return p.getDOM(panel, clickFlag, outChart);
            case 'scatter':
                p = $ScatterChart(panel);
                return p.getDOM();
            case 'text':
                p = textChart();
                return p.getDOM(panel, clickFlag, outChart);
            case 'toggle-gauge':
                p = $ToggleGaugeChart(panel);
                return p.getDOM();
            case 'group-bar':
                p = barChart();
                return p.getDOM(panel.id, clickFlag);
            case 'vector map':
                p = vectorMap();
                return p.getDOM(panel.id);
            case 'open street map':
                p = openStreetMap();
                return p.getDOM(panel.id);
            default:
                return document.createTextNode("");
        }
    }

    function RenderPanel(panel, clickFlag, center_longitude, center_latitude, outChart) {
        var p;
        switch (panel.type) {
            case 'bar':
                p = barChart();
                return p.renderChart(panel, clickFlag, outChart);
            case 'doughnut':
                p = donutChart();
                return p.renderChart(panel, clickFlag, outChart);
            case 'gauge':
                p = $GaugeChart(panel);
                return p.renderChart();
            case 'line':
                p = $LineChart(panel);
                return p.renderChart();
            case 'google map':
                break;
            case 'pie':
                p = pieChart();
                return p.renderChart(panel, clickFlag, outChart);
            case 'scatter':
                p = $ScatterChart(panel);
                return p.renderChart();
            case 'text':
                break;
            case 'toggle-gauge':
                p = $ToggleGaugeChart(panel);
                return p.renderChart();
            case 'group-bar':
                p = barChart();
                return p.renderChart(panel);
            case 'vector map':
                p = vectorMap();
                return p.renderChart(panel);
            case 'open street map':
                p = openStreetMap();
                return p.renderChart(panel, center_longitude, center_latitude);
            default:
                return document.createTextNode("");
        }
    }

    Card.prototype = {
        put: function (selector, widgetNameId, dragFlag, clickFlag, outChart) {
            var self = this;
            var root_div = document.createElement("div");
            var card_box = document.createElement("div");
            var panel_head = document.createElement("div");
            var panel_head_row = document.createElement("div");
            var panel_head_text = document.createElement("div");
            var span_card_name = document.createElement('span');
            var panel_content = document.createElement("div");

            root_div.setAttribute("class", self.panel.width);
            card_box.setAttribute("class", "cardbox");
            panel_head.setAttribute("class", "panel-heading");
            panel_head_row.setAttribute("class", "row");
            panel_head_text.setAttribute("class", "col-md-12 col-sm-12 col-xs-12 col-lg-12");
            span_card_name.textContent = self.panel.name;
            panel_head_text.appendChild(span_card_name);
            panel_head_row.appendChild(panel_head_text);

            if (true === dragFlag) {
                let i_card_dragTag = document.createElement('i');
                let span_card_dragTag = document.createElement('div');
                //span_card_dragTag.setAttribute('class', 'col-md-2 col-sm-2 col-xs-2 col-lg-2');
                span_card_dragTag.setAttribute('style', 'text-align:right');
                i_card_dragTag.setAttribute('class', 'fa fa-arrows');
                i_card_dragTag.setAttribute('aria-hidden', 'true');
                i_card_dragTag.setAttribute('style', 'cursor: pointer;');
                span_card_dragTag.appendChild(i_card_dragTag);
                panel_head_row.appendChild(span_card_dragTag);
            }

            if (!!widgetNameId) {
                panel_head_text.setAttribute('id', widgetNameId);
            }

            panel_head.appendChild(panel_head_row);
            card_box.appendChild(panel_head);

            panel_content.setAttribute("class", "panel-body text-center textcard pad0");
            panel_content._title = self.panel.name;
            panel_content.setAttribute("style", "position: relative;");
            panel_content.appendChild(GetPanel(self.panel, self.center_longitude, self.center_latitude, clickFlag, outChart));
            card_box.appendChild(panel_content);
            root_div.appendChild(card_box);
            $(selector).html(root_div);
            return this;
        },
        render: function (clickFlag, outChart) {
            var self = this;
            var chart_obj = RenderPanel(self.panel, clickFlag, self.center_longitude, self.center_latitude, outChart);
            return chart_obj;
        }
    };

    Card.init.prototype = Card.prototype;

    global.Card = global.$Card = Card;

})(window, jQuery);