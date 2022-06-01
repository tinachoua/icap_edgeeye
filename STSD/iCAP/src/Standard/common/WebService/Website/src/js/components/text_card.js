import { API } from "../library/api_handler";
import { getSuccess } from "../library/card_click_event";

export function textChart() {
    const lable_color = ["title-success", "title-warning"];
    const led_color = ["led-success", "led-none"];

    function getDOM(panel, clickFlag, outChart) {
        var root = document.createElement("div");
        var label, span, text, hr, widgetId, dashboardId;
        var data = panel.data;
        var labelArray = [];
        var valueArray = [];

        root.setAttribute("class", "col-md-12 pad0");

        if (clickFlag === true) {
            root._title = panel.name;
            root.setAttribute('style', 'cursor: pointer;');
            widgetId = panel.id;
            dashboardId = panel.dashboardId;
        }

        for (let i = 0; i < data.value.length; i++) {
            label = document.createElement("div");
            label.setAttribute("class", "text_header " + lable_color[i]);

            span = document.createElement("span");
            span.setAttribute("class", led_color[i]);

            label.appendChild(span);

            let labelTitle = document.createElement('span');

            labelTitle.textContent = data.label[i];

            label.appendChild(labelTitle);
            root.appendChild(label);

            text = document.createElement("div");
            text.setAttribute("class", "text_bady");
            text.textContent = data.value[i];

            root.appendChild(text);

            if (i != (data.value.length - 1)) {
                //  the middle dash in the widget
                hr = document.createElement("div");
                hr.setAttribute("class", "texthr");
                root.appendChild(hr);
            }

            if (clickFlag === true) {
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

        if (clickFlag === true) {
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

        outChart && outChart.push({
            title: panel.name,
            type: 'text',
            widgetId: widgetId,
            label: {
                title: labelArray,
                value: valueArray,
                color: undefined
            }
        });
        return root;
    }

    var publicAPI = {
        getDOM: getDOM,
    };

    return publicAPI;
}