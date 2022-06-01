import { API } from "../library/api_handler";
import { GetEventCount } from "../library/event_count";
import { isEmpty } from "../library/data_verification";
import { isNaturalNumber } from "../library/data_verification";
import { timeConverter } from "../library/common";

$(document).on("reload-event", function () {
    function RenderTab() {
        const EventTabWrapper = document.createElement('div');
        EventTabWrapper.classList.add('event-navbar-wrapper');

        var tab_primary_container = document.getElementById("tab-primary-continer");
        var tab_secondary_container = document.getElementById("tab-secondary-continer");
        var secondary_tab = document.getElementsByClassName("tab-secondary");
        var enent_Tab = $Tab({
            name: "Event"
        }).getTab();
        var tabsName = ['New', 'Completed', 'All'];

        $(document).trigger("clean_all_tab");
        $(secondary_tab[0]).show();
        
        EventTabWrapper.appendChild(enent_Tab);
        EventTabWrapper.appendChild($Tab().getDivider()); 

        tabsName.forEach(function (element, idx, array) {
            var sub_Tab = $Tab({
                name: element
            }).getTab();
            var SwitchSubPage = function () { };
            switch (element) {
                case 'New':
                    $(sub_Tab).addClass('hover');
                    SwitchSubPage = function (count) {
                        var promise = APICaller.GET('EventAPI/GetNew?count=' + count);
                        clickFlag.new = true;
                        clickFlag.completed = false;
                        clickFlag.all = false;
                        promise.done((response) => {
                            getSuccess(response, true);
                        });
                    };
                    break;
                case 'Completed':
                    SwitchSubPage = function (count) {
                        var promise = APICaller.GET('EventAPI/GetDone?count=' + count);
                        clickFlag.new = false;
                        clickFlag.completed = true;
                        clickFlag.all = false;
                        promise.done((response) => {
                            getSuccess(response, false);
                        });
                    };
                    break;
                case 'All':
                    SwitchSubPage = function (count) {
                        var promise = APICaller.GET('EventAPI/GetAll?count=' + count);
                        clickFlag.new = false;
                        clickFlag.completed = false;
                        clickFlag.all = true;
                        promise.done((response) => {
                            getSuccess(response, false);
                        });
                    };
                    break;
            }

            $(sub_Tab).addClass('sub-tab-content');
            sub_Tab.setAttribute("id", element);
            sub_Tab.addEventListener("click", () => {
                var count = $('#count').val();
                $('#tab-secondary-continer a').removeClass('hover');
                $(sub_Tab).addClass('hover');
                TableInit();
                SwitchSubPage(count);
            });

            if (idx === 0) {
                var badge_span = document.createElement("span");
                badge_span.setAttribute("class", "badge");
                badge_span.setAttribute("id", "new-event-badge");
                sub_Tab.appendChild(badge_span);
            }

            tab_secondary_container.appendChild(sub_Tab);
            tab_secondary_container.appendChild($Tab().getDivider()); 8
        });
        tab_primary_container.appendChild(EventTabWrapper);
    }

    function getSuccess(response, COUNT_FLAG) {
        var parsed_data = JSON.parse(response);
        const settingObj = {
            data: parsed_data,
            columns: [{
                "data": 'index'
            }, {
                "data": 'time',
                render: function (d) {
                    return timeConverter(d);
                }
            }, {
                "data": 'name'
            }, {
                "data": 'info'
            }, {
                "data": 'owner'
            }, {
                "orderable": false,
                "data": 'action'
            },],
            order: [
                [0, 'asc']
            ]
        };

        event_table = $(tableId).DataTable(settingObj);

        $(`${tableId} tbody`).on('click', 'button', function () {
            var data = event_table.row($(this).parents('tr')).data();
            let promise;
            var UpdateSuccess = function (data, btn) {
                if (COUNT_FLAG === true) {
                    data.remove().draw();
                } else {
                    btn.remove();
                }
                eventCount.textContent = eventCount.textContent - 1;
                $('#new-event-data').empty();
                GetEventCount();
                $('body').removeClass('loading');
            };
            $('body').addClass('loading');
            data.IsChecked = true;
            promise = APICaller.PUT('EventAPI/Update', data);
            promise.done(() => {
                UpdateSuccess(event_table.row($(this).parents('tr')), $(this));
            });
        });
        $('body').removeClass('loading');
    }

    var APICaller = API();
    let promise;
    var event_table;
    const tableId = '#event-table';
    const clickFlag = {
        new: true,
        completed: false,
        all: false
    };
    var TableInit = function () {
        $('body').addClass('loading');
        $(`${tableId} tbody`).off();
        event_table.clear().draw();
        event_table.destroy();
    };
    var eventCount;
    $('body').addClass('loading');
    RenderTab();
    eventCount = document.getElementById("new-event-badge");
    $('#find').on('click', () => {
        var count = $('#count').val();
        $('#alert').text('');
        if (isEmpty(count) === true || isNaturalNumber(count) === false) {
            setTimeout(() => {
                $('#alert').text('Please enter an interger which is no less than 1.');
            }, 100);
        } else if (clickFlag.new === true) {
            let promise = APICaller.GET('EventAPI/GetNew?count=' + count);
            TableInit();
            promise.done((response) => {
                getSuccess(response, true);
            });
        } else if (clickFlag.completed === true) {
            let promise = APICaller.GET('EventAPI/GetDone?count=' + count);
            TableInit();
            promise.done((response) => {
                getSuccess(response, false);
            });
        } else if (clickFlag.all === true) {
            let promise = APICaller.GET('EventAPI/GetAll?count=' + count);
            TableInit();
            promise.done((response) => {
                getSuccess(response, false);
            });
        }
    });

    $('#find-all').on('click', () => {
        $('#alert').text('');
        if (clickFlag.new === true) {
            let promise = APICaller.GET('EventAPI/GetNew?count=' + 0);
            TableInit();
            promise.done((response) => {
                getSuccess(response, true);
            });
        } else if (clickFlag.completed === true) {
            let promise = APICaller.GET('EventAPI/GetDone?count=' + 0);
            TableInit();
            promise.done((response) => {
                getSuccess(response, false);
            });
        } else if (clickFlag.all === true) {
            let promise = APICaller.GET('EventAPI/GetAll?count=' + 0);
            TableInit();
            promise.done((response) => {
                getSuccess(response, false);
            });
        }
    });

    promise = APICaller.GET('EventAPI/GetNew/Count');
    promise.done((response) => {
        var parsed_data = JSON.parse(response);
        eventCount.textContent = parsed_data.Response;
    });
    promise = APICaller.GET('EventAPI/GetNew?count=' + 100);
    promise.done((response) => {
        getSuccess(response, true);
    });
});