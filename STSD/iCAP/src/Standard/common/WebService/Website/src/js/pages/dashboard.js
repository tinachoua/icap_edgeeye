import { widget as widgetObj } from "../library/make_chart";
import { API } from "../library/api_handler";
import { findIndexInArray } from "../library/common";
import { fire } from "../library/event_handler";
export var g_setIntervalId = undefined;
export var g_outChart = [];
var reGetWidgetArray = [];
var _refreshTimeId = [];

export function restartUpdateWidget(dbLayout) {
    var dashboardId = document.getElementById('tab-primary-continer').getElementsByClassName('dashboard-tab-click')[0].value;

    updateWidget(document.getElementById('dashboard-item'), dashboardId);
    reGetWidgetArray.forEach((widgetId) => {
        reGetWidgetData(widgetId, dashboardId)
    });
    g_setIntervalId = setInterval(updateWidget.bind(null, dbLayout, dashboardId), getReloadTime() * 1000);
};

function getReloadTime() {
    return ($.cookie('dashboardUpload') === undefined) ? (function IIFE() {
        $.cookie('dashboardUpload', 30);
        return 30;
    })() : $.cookie('dashboardUpload');
}

function updateTime(timeElement, timestamp, currentTimestamp) {
    const timeValue = currentTimestamp - timestamp;
    if (timeValue <= 60) {
        timeElement.innerHTML = 'Now';
    } else {
        timeElement.innerHTML = `${Math.floor(timeValue / 60)}m ago`;
    }
}


function reGetWidgetData(widgetId, dashboardId) {
    if ($.cookie('current_page') !== 'dashboard' || $.cookie('dashboardId') !== dashboardId.toString()) {
        reGetWidgetArray = [];
        return;
    } else if (document.getElementsByTagName('body')[0].classList.contains("modal-open")) {
        return;
    }

    var APIActor = API();
    var promise = APIActor.GET(`DashboardAPI/WidgetInfo?widgetId=${widgetId}`, null, null, false);
    var reGetTimeInterval_msec = 1000;
    var widgetActor = widgetObj();
    var idx = findIndexInArray(g_outChart, 'widgetId', widgetId);

    promise.done((response, textStatus, xhr) => {
        if (response === undefined || xhr.status !== 200 || !response.data) {
            setTimeout(reGetWidgetData.bind(null, widgetId, dashboardId), reGetTimeInterval_msec);
        } else {
            const widget = document.getElementById(`chart-${widgetId}`);
            const timeElement = widget.getElementsByClassName('widgetTime')[0];
            const widgetBody = widget.getElementsByClassName('panel-body')[0];

            while (widgetBody.hasChildNodes()) {
                widgetBody.removeChild(widgetBody.firstChild);
            }

            widgetActor.makeWidgetBody(widgetBody, response, {
                clickFlag: true,
                dashboardId: dashboardId
            }, g_outChart[idx]);

            var index = reGetWidgetArray.indexOf(widgetId);
            if (index > -1) {
                reGetWidgetArray.splice(index, 1);
            }

            const timestamp = Math.floor(Date.now() / 1000);
            const timeValue = timestamp - response.time;

            if (timeValue <= 60) {
                timeElement.innerHTML = 'Now';
            } else {
                timeElement.innerHTML = `${Math.floor(timeValue / 60)}m ago`;
            }
        }
    });

    promise.fail(() => {
        setTimeout(reGetWidgetData.bind(null, widgetId, dashboardId), reGetTimeInterval_msec);
    });
}

function updateWidget(dbLayout, dashboardId) {
    var APIActor = API();
    var promise = APIActor.GET(`DashboardAPI/Widget/Data?dashboardId=${dashboardId}`, null, null, false);
    const layoutWidget = dbLayout.children;

    _refreshTimeId.forEach((id) => {
        clearInterval(id);
    });
    _refreshTimeId = [];
    promise.done((response, textStatus, xhr) => {
        if (xhr.status === 204) {
            let widgetActor = widgetObj();
            widgetActor.removeWidget(g_outChart, g_outChart.length);
        } else if (response === undefined) {
            return;
        } else {
            let oriWidgetCount = g_outChart.length;
            let newWidgetInfoArray = [];
            let newWidgetBodyArray = [];
            let fragment = document.createDocumentFragment();

            response.retJsonStr.forEach((jsonStr, idx) => {
                if (!jsonStr) return true;
                
                let newWidgetInfo = undefined;

                try {
                    newWidgetInfo = JSON.parse(jsonStr);
                } catch (e) {
                    return true;
                }

                if(!newWidgetInfo.data) return true;

                let widget = undefined;
                let targetWidget = g_outChart[idx];

                if (targetWidget === undefined) {
                    // add new widget
                    let widgetActor = widgetObj();
                    let rootWidget = widgetActor.createEmptyWidget({
                        id: newWidgetInfo.id,
                        width: newWidgetInfo.width,
                        name: newWidgetInfo.name,
                    });
                    newWidgetBodyArray.push(rootWidget.getElementsByClassName('panel-body')[0]);
                    newWidgetInfoArray.push(newWidgetInfo);
                    g_outChart.push({
                        widgetId: newWidgetInfo.id,
                        name: newWidgetInfo.name,
                        width: newWidgetInfo.width
                    });
                    fragment.appendChild(rootWidget);
                    return true;
                }

                if (targetWidget.widgetId !== newWidgetInfo.id) {
                    targetWidget.label = undefined;
                    widget = layoutWidget[idx];
                    widget.setAttribute('id', `chart-${newWidgetInfo.id}`);
                    targetWidget.widgetId = newWidgetInfo.id;

                    let title = widget.getElementsByTagName('span')[0];

                    title.textContent = newWidgetInfo.name;
                    targetWidget.name = newWidgetInfo.name;

                    if (targetWidget.width !== newWidgetInfo.width) {
                        widget.classList.remove('col-md-4');
                        widget.classList.remove('col-md-8');
                        widget.classList.remove('col-sm-6');
                        widget.classList.remove('col-md-12');
                        widget.classList.remove('col-sm-12');
                        widget.classList.add(newWidgetInfo.width);
                        if (newWidgetInfo.width === 'col-md-12') {
                            widget.classList.add('col-sm-12');
                        } else {
                            widget.classList.add('col-sm-6');
                        }
                        targetWidget.width = newWidgetInfo.width;
                    }

                    let widgetActor = widgetObj();
                    if (targetWidget.type !== undefined) {
                        widgetActor.destroy(targetWidget);
                        targetWidget.body = undefined;
                        targetWidget.type = undefined;
                    }

                    let widgetBody = widget.getElementsByClassName('panel-body')[0];
                    while (widgetBody.hasChildNodes()) {
                        widgetBody.removeChild(widgetBody.firstChild);
                    }
                    let timeElement = widget.getElementsByClassName('widgetTime')[0]


                    if (newWidgetInfo.data === undefined) {
                        widgetBody.innerHTML = widgetActor.addloadingHtml();
                        targetWidget.type = undefined;
                        timeElement.innerHTML = '';
                    } else {
                        widgetActor.makeWidgetBody(widgetBody, newWidgetInfo, {
                            clickFlag: true,
                            dashboardId: dashboardId
                        }, g_outChart[idx]);
                        updateTime(timeElement, newWidgetInfo.time, Math.floor(Date.now() / 1000));
                        _refreshTimeId.push(setInterval(() => {
                            if (!document.getElementById('dashboard-item')) {
                                clearRefreshTime(_refreshTimeId);
                                return true;
                            }
                            updateTime(timeElement, newWidgetInfo.time, Math.floor(Date.now() / 1000));
                        }, 60000));
                    }
                    return true;
                }

                if (targetWidget.name !== newWidgetInfo.name) {
                    if (widget === undefined) {
                        widget = document.getElementById('chart-' + newWidgetInfo.id);
                    }

                    let title = widget.getElementsByTagName('span')[0];

                    title.textContent = newWidgetInfo.name;
                    targetWidget.name = newWidgetInfo.name;
                }

                if (newWidgetInfo.type === undefined) {
                    // widget data not found
                    return true;
                } else {
                    //Update Time
                    if (widget === undefined) {
                        widget = document.getElementById('chart-' + newWidgetInfo.id);
                    }

                    if (widget) {
                        updateTime(widget.getElementsByClassName('widgetTime')[0], newWidgetInfo.time, Math.floor(Date.now() / 1000));
                        _refreshTimeId.push(setInterval(() => {
                            if (!document.getElementById('dashboard-item')) {
                                clearRefreshTime(_refreshTimeId);
                                return true;
                            }
                            updateTime(widget.getElementsByClassName('widgetTime')[0], newWidgetInfo.time, Math.floor(Date.now() / 1000));
                        }, 60000));
                    }
                }

                if (targetWidget.type === newWidgetInfo.type) {
                    let newData = newWidgetInfo.data;
                    let widgetActor = widgetObj();

                    if (targetWidget.type === 'google map') {
                        targetWidget.body.marker.forEach((marker) => {
                            marker.setMap(null);
                        });
                        targetWidget.body.marker = undefined; // free memory
                        widgetActor.googleMap.addMarker(newData, targetWidget.body);
                        return true;
                    } else if (targetWidget.type === 'open street map') {

                        let osmMap = targetWidget.body.map;
                        let overlayContainer = osmMap.overlayContainerStopEvent_.getElementsByClassName('ol-overlay-container')[0];
                        let tooltipIsShow = function (container) {
                            if (container.style.display === 'none') {
                                return false;
                            }
                            return true;
                        };
                        if (tooltipIsShow(overlayContainer)) {
                            fire(overlayContainer.getElementsByClassName('ol-popup-closer')[0], 'click');
                        }

                        let vector = osmMap.getLayers().array_[1].sourceChangeKey_.target;

                        vector.clear();
                        vector.addFeatures(widgetActor.opsnStreetMap.makeFeatures(newData.value));
                        return true;
                    } else if (targetWidget.type === 'vector map') {

                        let map = targetWidget.body;
                        let nameObj = widgetActor.vectorMap.getMapName(newData.mapIndex);

                        map.removeAllMarkers();

                        if (map.params.map === nameObj.name) {
                            map.addMarkers(newData.markers);
                            newData.alias = nameObj.alias;
                            widgetActor.vectorMap.updateTooltipMsg(map.container[0], newData);
                        } else {
                            ///// rebuild
                            let widgetActor = widgetObj();
                            if (widget === undefined) {
                                widget = document.getElementById(`chart-${targetWidget.widgetId}`);
                            }

                            let widgetBody = widget.getElementsByClassName('panel-body')[0];

                            widgetActor.destroy(targetWidget);
                            while (widgetBody.hasChildNodes()) {
                                widgetBody.removeChild(widgetBody.firstChild);
                            }
                            widgetActor.makeWidgetBody(widgetBody, newWidgetInfo, {
                                clickFlag: true,
                                dashboardId: dashboardId
                            }, g_outChart[idx]);
                        }
                        return true;
                    } else if (targetWidget.type === 'gaode map') {
                        targetWidget.body.map.clearMap();


                        if (!!newData.value && newData.value.length !== 0) {
                            widgetActor.gaodeMap.addMarker(newData.value, targetWidget.body.map);
                        }
                    } else if (targetWidget.type === 'customized map'){
                        targetWidget.body.update(newWidgetInfo);
                    } 
                    else {
                        //chart
                        let oriDataLength = (targetWidget.label === undefined) ? targetWidget.body.data.labels.length : targetWidget.label.title.length;
                        let newDataLength = newData.label.length;
                        let colors = (targetWidget.type === 'bar' && oriDataLength !== newDataLength) ? colors(newDataLength) : undefined;

                        if (!!targetWidget.label) {
                            let updateCount = newDataLength;
                            let targetLabel = targetWidget.label;
                            if (oriDataLength > updateCount) {
                                colors = colors(newDataLength);
                                while (targetLabel.title.length !== updateCount) {
                                    targetLabel.title[targetLabel.title.length - 1].parentNode.remove();
                                    targetLabel.title.splice(-1, 1);
                                    targetLabel.value.splice(-1, 1);
                                    targetLabel.color && targetLabel.color.splice(-1, 1);
                                }
                            } else if (oriDataLength < newDataLength) {
                                let startIndex = targetLabel.title.length;
                                let labelRoot = targetLabel.title[0].parentNode.parentNode;

                                colors = colors(newDataLength);
                                updateCount = targetLabel.title.length;
                                while (targetLabel.title.length !== newDataLength) {
                                    widgetActor.addLabel({
                                        title: targetLabel.title,
                                        value: targetLabel.value,
                                        color: targetLabel.color
                                    }, labelRoot, {
                                            label: newData.label[startIndex],
                                            value: newData.value[startIndex],
                                            color: colors[startIndex],
                                        }, startIndex, {
                                            widgetId: newWidgetInfo.widgetId,
                                            dashboardId: dashboardId
                                        }
                                    );
                                    startIndex++;
                                }
                            }
                            widgetActor.updateLabel(targetLabel, newData.label, newData.value, updateCount, colors);
                        }
                        !!targetWidget.body && widgetActor.updateChart(targetWidget.body, {
                            label: newData.label,
                            value: newData.value,
                            color: colors
                        });
                    }
                } else {
                    //build new chart
                    let widgetActor = widgetObj();
                    let widgetBody = document.getElementById(`chart-${targetWidget.widgetId}`).getElementsByClassName('panel-body')[0];
                    widgetActor.destroy(targetWidget);
                    while (widgetBody.hasChildNodes()) {
                        widgetBody.removeChild(widgetBody.firstChild);
                    }
                    widgetActor.makeWidgetBody(widgetBody, newWidgetInfo, {
                        clickFlag: true,
                        dashboardId: dashboardId
                    }, g_outChart[idx]);
                }
            });

            if (newWidgetInfoArray.length !== 0) {
                //render new chart
                let widgetActor = widgetObj();

                dbLayout.appendChild(fragment);
                newWidgetInfoArray.forEach((newWidgetInfo, idx) => {
                    if (!newWidgetInfo.data) {

                        newWidgetBodyArray[idx].innerHTML = widgetActor.addloadingHtml();

                        if (!reGetWidgetArray.includes(newWidgetInfo.id)) {
                            reGetWidgetArray.push(newWidgetInfo.id);
                            reGetWidgetData(newWidgetInfo.id, dashboardId);
                        }
                        return;
                    }
                    let widget = document.getElementById(`chart-${newWidgetInfo.id}`);
                    if (!widget) {
                        return;
                    }

                    const timeElement = widget.getElementsByClassName('widgetTime')[0];
                    widgetActor.makeWidgetBody(newWidgetBodyArray[idx], newWidgetInfo, {
                        clickFlag: true,
                        dashboardId: dashboardId
                    }, g_outChart[oriWidgetCount + idx]);
                    updateTime(timeElement, newWidgetInfo.time, Math.floor(Date.now() / 1000));

                    _refreshTimeId.push(setInterval(() => {
                        if (!document.getElementById('dashboard-item')) {
                            clearRefreshTime(_refreshTimeId);
                            return;
                        }
                        updateTime(timeElement, newWidgetInfo.time, Math.floor(Date.now() / 1000));
                    }, 60000));
                });
            } else if (g_outChart.length > response.retJsonStr.length) {
                //remove chart
                widgetObj().removeWidget(g_outChart, g_outChart.length - response.retJsonStr.length);
            }
        }
    });
    promise.fail((response, textStatus, xhr) => {
        if (response.status === 410) {
            clearInterval(g_setIntervalId);
            // alert('The dashboard is be deleted')
        } else if (response.status === 403) {
            clearInterval(g_setIntervalId);
            fire(document, 'logout');
        }
    });
};

function clearRefreshTime(intervalId) {
    intervalId.forEach((id) => {
        clearInterval(id);
    });
    intervalId.length = 0;
}

function injectStyle(setting){
    const doms = setting.map(({
        selectors,
    }) => {
        return document.querySelector(selectors);
    })
    for(let i=0; i < doms.length; i++) {
        const style = doms[i].style;
        const newStyle = setting[i].style;

        Object.keys(newStyle).forEach((key)=>{
            style[key] = newStyle[key];
        });
    };
}



(function IIFE() {
    var $document = $(document);
    var dashboardId = 0;
    //var reGetWidgetArray = [];
    var putWidget = function arrangeWidget(dashboardItem, jsonStrArray, dashboardId, unfinishedWidgetArray) {
        if (!jsonStrArray) {
            return;
        }
        var widgetActor = widgetObj();
        var fragment = document.createDocumentFragment();
        var widgetInfoArray = [];
        var widgetBodyArray = [];

        jsonStrArray.forEach((jsonStr) => {
            if (jsonStr !== null) {
                let widgetInfoObj

                try{
                    widgetInfoObj = JSON.parse(jsonStr);
                }
                catch (e) {
                    return true;
                }
                
                let rootWidget = widgetActor.createEmptyWidget({
                    id: widgetInfoObj.id,
                    width: widgetInfoObj.width,
                    name: widgetInfoObj.name,
                });
                widgetBodyArray.push(rootWidget.getElementsByClassName('panel-body')[0]);
                widgetInfoArray.push(widgetInfoObj);
                g_outChart.push({
                    widgetId: widgetInfoObj.id,
                });
                fragment.appendChild(rootWidget);
            }
        });

        dashboardItem.appendChild(fragment);
        widgetInfoArray.forEach((widgetInfoObj, idx) => {
            if (!widgetInfoObj.data) {
                unfinishedWidgetArray.push(widgetInfoObj.id);
                widgetBodyArray[idx].innerHTML = widgetActor.addloadingHtml();
                return;
            }

            const timeElement = document.getElementById(`chart-${widgetInfoObj.id}`).getElementsByClassName('widgetTime')[0];
            widgetActor.makeWidgetBody(widgetBodyArray[idx], widgetInfoObj, {
                clickFlag: true,
                dashboardId: dashboardId
            }, g_outChart[idx]);

            updateTime(timeElement, widgetInfoObj.time, Math.floor(Date.now() / 1000));
            _refreshTimeId.push(setInterval(() => {
                if (!document.getElementById('dashboard-item')) {
                    clearRefreshTime(_refreshTimeId);
                    return;
                }
                updateTime(timeElement, widgetInfoObj.time, Math.floor(Date.now() / 1000));
            }, 60000));
        });
    };

    $document.on("reload-dashboard", () => {
        var APIActor = API();
        var promise = APIActor.GET("DashboardAPI/home");
        var body = document.getElementsByTagName('body')[0];
        
        var initHome = function (response, data, xhr) {
            if (xhr.status === 204) {
                body.classList.remove('loading');
                return; 
            }
            const Wrapper = document.createElement('div');
            var dashboardList = response.dashboardList;
            var jsonStrArray = response.retJsonStr;
            var tab_container = document.getElementById("tab-primary-continer");
            var tabFragment = document.createDocumentFragment();
            var tab_secondary = document.getElementById('main').getElementsByClassName('wrapper')[0].getElementsByClassName('tab-secondary')[0];
            var dbLayout = document.getElementById('dashboard-item');
            var unfinishedWidgetArray = [];
            Wrapper.classList.add('dashboard-navbar-wrapper');

            tab_secondary.style.display = 'none';
            $document.trigger("clean_all_tab");
            dashboardList.forEach((element, index) => {
                var tb = $Tab({
                    name: element.Name,
                }).getTab();
                if (index === 0) {
                    tb.classList.add('dashboard-tab-click');
                    dashboardId = element.Id;
                    $.cookie('dashboardId', dashboardId);
                }
                tb.value = element.Id;
                tabFragment.appendChild(tb);
                tabFragment.appendChild($Tab().getDivider());

                tb.addEventListener("click", function (e) {
                    if (g_setIntervalId !== undefined) {
                        clearInterval(g_setIntervalId);
                        g_setIntervalId = undefined;
                    }
                    clearRefreshTime(_refreshTimeId);
                    $('#tab-primary-continer .dashboard-navbar-wrapper > a ').each((idx, tab) => {
                        tab.classList.remove('dashboard-tab-click');
                    });
                    this.classList.add('dashboard-tab-click');
                    var id = this.value;
                    var APIActor = API();
                    var promise;
                    var body = document.getElementsByTagName('body')[0];
                    let widgetActor = widgetObj();
                    var unfinishedWidgetArray = [];
                    body.classList.add('loading');
                    dashboardId = id;
                    $.cookie('dashboardId', dashboardId);
                    widgetActor.removeWidget(g_outChart, g_outChart.length);
                    promise = APIActor.GET(`DashboardAPI/Widget/Data?dashboardId=${dashboardId}`);
                    promise.done((response, textStatus, xhr) => {
                        if (response !== undefined) {
                            putWidget(dbLayout, response.retJsonStr, dashboardId, unfinishedWidgetArray);
                            unfinishedWidgetArray.forEach((widgetId) => {
                                if (!reGetWidgetArray.includes(widgetId)) {
                                    reGetWidgetArray.push(widgetId);
                                    reGetWidgetData(widgetId, dashboardId)
                                }
                            })
                        }
                        g_setIntervalId = setInterval(updateWidget.bind(null, dbLayout, dashboardId), reloadTime);
                        body.classList.remove('loading');
                    });
                    promise.fail((response, textStatus, xhr) => {
                        clearInterval(g_setIntervalId);
                        body.classList.remove('loading');
                        if (response.status === 410) {
                            // alert('The dashboard is be deleted');
                        } else if (response.status === 403) {
                            fire(document, 'logout');
                        }
                    });
                });
            });

            Wrapper.appendChild(tabFragment);
            tab_container.appendChild(Wrapper);
            putWidget(dbLayout, jsonStrArray, dashboardId, unfinishedWidgetArray);

            unfinishedWidgetArray.forEach((widgetId) => {
                reGetWidgetArray.push(widgetId);
                reGetWidgetData(widgetId, dashboardId)
            })
            var reloadTime = getReloadTime() * 1000; // second

            g_setIntervalId = setInterval(updateWidget.bind(null, dbLayout, dashboardId), reloadTime);

            $document.ready(() => {
                var $dashboardLayout = $("#dashboard-item");

                $dashboardLayout.sortable({
                    connectWith: ".connectedSortable",
                    cursor: "move",
                    handle: ".panel-heading",
                    cancel: ".chartjs-render-monitor",
                    update: function () {
                        var order = $dashboardLayout.sortable('toArray');
                        var widgetId = [];
                        var saveWidgetOrder = function (widgetOrder) {
                            var payloadObj = {
                                DashboardId: dashboardId,
                                WidgetIdList: widgetOrder
                            };
                            promise = APIActor.POST("/DashboardAPI/Order", payloadObj, false);
                            promise.done(() => {
                                var newg_OutChart = [];

                                widgetOrder.forEach((id, idx) => {
                                    let tmpIndex = findIndexInArray(g_outChart, 'widgetId', id);

                                    newg_OutChart[idx] = g_outChart[tmpIndex];
                                });
                                g_outChart = newg_OutChart;
                                newg_OutChart = undefined;
                            });
                        };

                        order.forEach((element) => {
                            var id = Number(element.substring(6));

                            widgetId.push(id);
                        });
                        saveWidgetOrder(widgetId);
                    }
                }).disableSelection();
            });
            body.classList.remove('loading');
        };
        clearRefreshTime(_refreshTimeId);
        body.classList.add('loading');
        promise.done(initHome);
        promise.fail((response, textStatus, xhr) => {
            clearInterval(g_setIntervalId);
            body.classList.remove('loading');
            if (response.status === 403) {
                fire(document, 'logout');
            } else {
                $document.trigger('reload-dashboard');
            }
        });
    });
})();