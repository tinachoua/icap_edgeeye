import { SwitchPage } from '../library/switch_page';
import { fire } from '../library/event_handler';
import { timeConverter } from "../library/common";

export function GetEventCount() {
    var store_token = $.cookie('token');
    var event_badge = document.getElementById('eventcount');
    var new_event = document.getElementById('new-event-data');
    $.ajax({
        type: 'GET',
        url: 'EventAPI/GetNew',
        async: true,
        crossDomain: true,
        headers: {
            'token': store_token
        },
        success: function (response) {
            var parsed_data = JSON.parse(response);
            if (parsed_data.length > 0) {
                if (parsed_data.length > 9) {
                    event_badge.textContent = '9+';
                }
                else {
                    event_badge.textContent = parsed_data.length;
                }
                parsed_data.forEach(element => {
                    var root_li = document.createElement("li");
                    var child_a = document.createElement("a");
                    child_a.setAttribute("style", "cursor: pointer;");
                    $(child_a).bind("click", SwitchPage().ToEvent);
                    var left_span = document.createElement("span");
                    left_span.setAttribute("class", "span-left t02");
                    left_span.textContent = element.info;
                    var right_span = document.createElement("span");
                    right_span.setAttribute("class", "span-right");
                    var child_i = document.createElement("i");
                    child_i.setAttribute("class", "fa fa-clock-o");
                    var text_time = document.createTextNode(" " + timeConverter(element.time));
                    right_span.appendChild(child_i);
                    right_span.appendChild(text_time);
                    child_a.appendChild(left_span);
                    child_a.appendChild(right_span);
                    root_li.appendChild(child_a);
                    new_event.appendChild(root_li);
                });
            }
            else {
                var root_p = document.createElement("p");
                event_badge.textContent = 0;
                root_p.setAttribute("class", "text-center");
                root_p.textContent = "No new event.";
                new_event.appendChild(root_p);
            }
        },
        error: function (response) {
            if (403 === response.status) {
                fire(document, 'logout');
            }
        }
    });
}