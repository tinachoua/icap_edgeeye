import { isEmpty, isValidLongitude, isValidLatitude } from "../library/data_verification";
import { addOption, addGroupWithOpion } from "../library/select_form";
import { makeMockBarChart, makeLabel, makeMockDonutChart, makeMockPieChart, makeMockTextChart } from "../library/make_chart";
import { modalBack } from "../library/modal";
import { API } from "../library/api_handler";
import { INNOAGE_STATUS_DATA, GOOGLEMAP_CHART, VECTORMAP_CHART, OPENSTREETMAP_CHART, DEVICE_STATUS_DATA, BAR_CHART, DONUT_CHART, PIE_CHART, TEXT_CHART, MESSAGE_DELAY_MILLISECOND, RATIO_PROCESS, BOOLEAN_PROCESS, NUMERICAL_PROCESS, STORAGE_PARTITION_CAPACITY, STORAGE_DATAGROUP, DEVICE_LOCATION_DATA, GAODEMAP_CHART } from "../library/data_mapping";
import { findIndexInArray } from "../library/common";
import { fire } from "../library/event_handler";
import { WIDGET_WIDTH } from '../library/data_define';

(function IIFE() {
    function reloadSettingWidget() {
        var $body = $("body");
        $body.addClass("loading");

        (function listenWidgetSettings() {
            var $create = $('#create-widget');
            var $editBtn = $('#edit-widget');
            var $delete = $('#delete-widget');

            function hideAlert() {
                var failAlert = document.getElementById('danger-widget-setting');
                var successAlert = document.getElementById('success-widget-setting');

                failAlert.style.display = 'none';
                successAlert.style.display = 'none';
            }

            $create.click(function () {
                hideAlert();
                (function showCreateWidgetModal() {
                    var $dangerAlert = $('#danger-widget-setting');
                    var $submit = $('#submit-create-widget');
                    var $modal = $("#create-widget-modal");
                    var backModalController = modalBack();

                    backModalController.fade();
                    $dangerAlert.hide();
                    function createWidget() {
                        var widgetName = document.getElementById("widget-name").value;

                        (function hideCreateAlert() {
                            var failAlert = document.getElementById('fail-create-widget');
                            var blankAlert = document.getElementById('blank-create-widget');
                            var successAlert = document.getElementById('success-create-widget');

                            failAlert.style.display = 'none';
                            blankAlert.style.display = 'none'
                            successAlert.style.display = 'none';
                        })();

                        if (isEmpty(widgetName)) {
                            let $failAlert = $("#fail-create-widget");
                            $failAlert.html("Please fill in the name.");
                            setTimeout(() => {
                                var $blank = $("#blank-create-widget");
                                $failAlert.show();
                                $blank.show();
                            }, MESSAGE_DELAY_MILLISECOND);
                        } else {
                            let $body = $("body");
                            $body.addClass('loading');
                            let promise = $.ajax({
                                type: 'POST',
                                url: 'WidgetAPI/Create',
                                async: true,
                                crossDomain: true,
                                headers: {
                                    'token': $.cookie('token'),
                                    "Content-Type": "application/json"
                                },
                                processData: false,
                                data: JSON.stringify($("#widget-name").val())
                            });
                            let toDoWhenCreateSuccess = function (response) {
                                if (response) {
                                    let $widgetSelect = $('#widgetlist');

                                    $widgetSelect.empty();
                                    $widgetSelect.selectpicker('refresh');

                                    let promise = apiHandler.GET('WidgetAPI/GetWidgetList');

                                    promise.done((response) => {
                                        var parsedData = JSON.parse(response);
                                        addOption($widgetSelect, parsedData);

                                        $(document).ready(() => {
                                            var lastValue = $('#widgetlist option:last-child').val();
                                            var $body = $('body');
                                            $widgetSelect.val(lastValue);
                                            $widgetSelect.selectpicker('refresh');
                                            $modal.modal('hide')

                                            var lastValue = $widgetSelect.val();
                                            $widgetSelect.val(lastValue);
                                            $widgetSelect.selectpicker('refresh');
                                            $editBtn.trigger('click');
                                            $body.removeClass('loading');
                                        });
                                    });
                                }
                            }
                            promise.done(toDoWhenCreateSuccess);
                            let toDoWhenCreateFail = function (response) {
                                var failAlert = document.getElementById('fail-create-widget');
                                var blankAlert = document.getElementById('blank-create-widget');

                                if (403 == response.status) {
                                    let errorCode = JSON.parse(response.responseText).ErrorCode;
                                    if (errorCode === 0) {
                                        fire(document, 'logout');
                                    } else if (errorCode === 1) {
                                        failAlert.textContent = 'Sorry, you do not have access to create widget.'
                                    }
                                } else {
                                    failAlert.textContent = 'Failed to create the widget!';
                                }

                                setTimeout(() => {
                                    failAlert.style.display = '';
                                    blankAlert.style.display = '';
                                    $body.removeClass('loading');
                                }, MESSAGE_DELAY_MILLISECOND);
                                $body.removeClass('loading');
                            }
                            promise.fail(toDoWhenCreateFail);
                        }
                    }
                    $submit.click(createWidget);

                    (function initCreateWidgetModal(backModalController) {
                        var $relogAlert = $("#re-log-in-widgetlist");
                        var $failAlert = $("#fail-create-widget");
                        var $successAlert = $("#success-create-widget");
                        var $modal = $('#create-widget-modal');

                        $relogAlert.hide();
                        $failAlert.hide();
                        $successAlert.hide();
                        $modal.on('hidden.bs.modal', function () {
                            var $submit = $('#submit-create-widget');
                            var name = document.getElementById("widget-name");
                            var $blank = $("#blank-create-widget");

                            $submit.off();
                            name.value = "";
                            $failAlert.hide();
                            $blank.hide();
                            $modal.off();
                            backModalController.removeFade();
                        });
                    })(backModalController);

                    $modal.modal({
                        backdrop: false
                    });
                    $modal.modal("show");
                })();
            });
            $editBtn.click(function (evt) {
                hideAlert();
                (function showEditWidgetModal(widgetId) {
                    var $dangerAlert = $('#danger-widget-setting');
                    $dangerAlert.hide();
                    if (false == widgetId) {
                        $dangerAlert.html('Please select one widget.');
                        setTimeout(() => { $dangerAlert.show(); }, MESSAGE_DELAY_MILLISECOND);
                    } else {
                        let $body = $("body");
                        $body.addClass("loading");
                        let apiHandler = API();
                        let promise = apiHandler.GET(`WidgetAPI/Edit/${widgetId}`);
                        promise.done((response) => {
                            (function initEditForm(response) {
                                var $modal = $("#edit-widget-modal");
                                var $widgetName = $('#widget-name-edit');
                                var parsedData = response;
                                var backModalController = modalBack();
                                var $chartSelect = $('#chartlist');
                                var $widthSelect = $('#widthlist');
                                var centerField = document.getElementById('map-center-field');
                                var vectorField = document.getElementById('vector-map-field');
                                var $deviceLocationSelect = $('#device-location-data-source');
                                var mapField = document.getElementById('map-field');
                                var $vectorMapSelect = $('#vector-map-list');
                                var $groupSelect = $('#grouplist');
                                var groupField = document.getElementById('group-field');
                                var $sourceSelect = $('#datalist');
                                var processField = document.getElementById('process-field');
                                var processType = document.getElementById('process-type');
                                var processBtn = processType.getElementsByClassName('process-btn');
                                var processForm = document.getElementById('process-form');
                                var sourceRadio = document.getElementById('source-radio');
                                var ruleRadio = document.getElementById('rule-radio');
                                var $ruleSelect = $('#rulelist');
                                var booleanBtn = document.getElementById('boolean-btn');
                                var ratioBtn = document.getElementById('percentage-btn');
                                var numericalBtn = document.getElementById('numerical-btn');
                                var $denominatorSelect = $('#denominatorlist');
                                var ratioForm = document.getElementById('percentage-form');
                                var booleanForm = document.getElementById('boolean-form');
                                var numericalForm = document.getElementById('numerical-form');
                                var dataChooseField = document.getElementById('data-choose');
                                var percentageFirst = document.getElementById('percentage-interval-0');
                                var percentageInterval = document.getElementById('process-form').getElementsByClassName('percentage-field');
                                var numericalFirst = document.getElementById('numerical-interval-0');
                                var numericalInterval = document.getElementById('process-form').getElementsByClassName('numerical-field');
                                var failAlert = document.getElementById('fail-edit');
                                var successAlert = document.getElementById('success-edit');
                                var booleanLabel = document.getElementsByClassName('boolean-label');
                                var applyBtn = document.getElementById('apply-edit');
                                var longitude = document.getElementById('lng');
                                var latitude = document.getElementById('lat');
                                const containerFluid = document.getElementById('edit-widget-modal').getElementsByClassName('container-fluid')[0];
                                const okBtn = document.getElementById('ok-edit');
                                document.getElementById('top-label').value = 'Online';
                                document.getElementById('bottom-label').value = 'Offline';
                                applyBtn.disabled = false;

                                addOption($vectorMapSelect, [
                                    { Id: 1, Name: 'Africa' },
                                    { Id: 2, Name: 'Asia' },
                                    { Id: 3, Name: 'Europe' },
                                    { Id: 4, Name: 'North America' },
                                    { Id: 5, Name: 'Oceania' },
                                    { Id: 6, Name: 'South America' },
                                    { Id: 7, Name: 'World' },
                                ]);
                                $vectorMapSelect.selectpicker();

                                addOption($groupSelect, parsedData.BranchOption);
                                $groupSelect.selectpicker();
                                var $actionsBtn = $('#edit-widget-modal .actions-btn');
                                $actionsBtn.removeClass('btn');
                                $actionsBtn.addClass('btn-select');

                                parsedData.DataSelect.forEach((element) => {
                                    addGroupWithOpion($sourceSelect, element.GroupOption, element.DataOption);
                                });
                                $sourceSelect.attr('disabled', true);
                                $sourceSelect.selectpicker();

                                addOption($ruleSelect, parsedData.ThresholdOption);
                                $ruleSelect.attr('disabled', true);
                                $ruleSelect.selectpicker();
                                $denominatorSelect.selectpicker();

                                mapField.style.display = 'none';
                                centerField.style.display = 'none';
                                vectorField.style.display = 'none';
                                groupField.style.display = 'none';
                                processField.style.display = 'none';
                                processType.style.display = 'none';
                                dataChooseField.style.display = 'none';
                                sourceRadio.checked = false;
                                ruleRadio.checked = false;
                                successAlert.style.display = 'none';
                                failAlert.style.display = 'none';
                                numericalBtn.value = '0';
                                ratioBtn.value = '0';
                                for (let btn of processBtn) {
                                    btn.style.display = 'none';
                                }

                                backModalController.fade();
                                $modal.modal({
                                    backdrop: false
                                });

                                $modal.modal("show");
                                $widgetName.val(parsedData.WidgetTemplate.Name);

                                addOption($chartSelect, parsedData.ChartOption);
                                $chartSelect.selectpicker();
                                $widthSelect.selectpicker();
                                $sourceSelect.selectpicker();
                                $deviceLocationSelect.selectpicker();
                                var activeApply = function (callBack) {
                                    if (applyBtn.disabled) {
                                        applyBtn.disabled = false;
                                        applyBtn.addEventListener('click', btnHandler);
                                        if (typeof callBack === 'function') {
                                            callBack();
                                        }
                                    }
                                };
                                $chartSelect.on('change', (evt, callBack) => {
                                    activeApply();
                                    centerField.style.display = 'none';
                                    vectorField.style.display = 'none';
                                    mapField.style.display = 'none';
                                    groupField.style.display = 'none';
                                    processField.style.display = 'none';
                                    ratioForm.style.display = 'none';
                                    numericalForm.style.display = 'none';
                                    booleanForm.style.display = 'none';
                                    processType.style.display = 'none';

                                    $widthSelect.empty();
                                    $widthSelect.selectpicker('refresh')
                                    ratioBtn.classList.remove('selected');
                                    numericalBtn.classList.remove('selected');

                                    var idx = findIndexInArray(parsedData.ChartOption, 'Id', Number(evt.target.value))
                                    var size = parsedData.ChartOption[idx]
                                    var options = [];

                                    if (size.Small) {
                                        options.push({
                                            Id: WIDGET_WIDTH.SIZE_1X1,
                                            Name: 'Small'
                                        });
                                    }

                                    if (size.Medium) {
                                        options.push({
                                            Id: WIDGET_WIDTH.SIZE_1X2,
                                            Name: 'Medium'
                                        });
                                    }

                                    if (size.Large) {
                                        options.push({
                                            Id: WIDGET_WIDTH.SIZE_1X3,
                                            Name: 'Large'
                                        });
                                    }

                                    addOption($widthSelect, options);
                                    $widthSelect.selectpicker('refresh');

                                    $(document).ready(() => {
                                        if (typeof callBack === 'function') {
                                            callBack();
                                        }
                                    });
                                    processForm.style.display = 'none';
                                    $sourceSelect.selectpicker('val', '');
                                    $ruleSelect.selectpicker('val', '');
                                    $denominatorSelect.selectpicker('val', '');

                                });

                                $widthSelect.on('change', (evt, callBack) => {
                                    activeApply();
                                    if ($chartSelect.val() == GOOGLEMAP_CHART || $chartSelect.val() == OPENSTREETMAP_CHART || $chartSelect.val() == GAODEMAP_CHART) {
                                        mapField.style.display = '';
                                        centerField.style.display = '';
                                        vectorField.style.display = 'none';
                                        dataChooseField.style.display = 'none';
                                        groupField.style.display = '';
                                    } else if ($chartSelect.val() == VECTORMAP_CHART) {
                                        mapField.style.display = '';
                                        centerField.style.display = 'none';
                                        vectorField.style.display = '';
                                        dataChooseField.style.display = 'none';
                                        groupField.style.display = '';
                                    } else {
                                        mapField.style.display = 'none';
                                        centerField.style.display = 'none';
                                        vectorField.style.display = 'none';
                                        processField.style.display = '';
                                        dataChooseField.style.display = '';
                                    }

                                    if (typeof callBack == 'function') {
                                        callBack();
                                    }

                                });

                                function sourceRadioHandler(evt) {
                                    if ($sourceSelect.attr('disabled') === undefined) {
                                        return;
                                    }
                                    activeApply();
                                    $sourceSelect.attr('disabled', false);
                                    $sourceSelect.selectpicker('refresh');
                                    $ruleSelect.attr('disabled', true);
                                    $ruleSelect.selectpicker('refresh');

                                    if (!$sourceSelect.val()) {
                                        processType.style.display = 'none';
                                        groupField.style.display = 'none';
                                    } else {
                                        processType.style.display = '';
                                        groupField.style.display = '';
                                    }

                                }
                                sourceRadio.addEventListener('click', sourceRadioHandler);

                                function ruleRadioHandler() {
                                    if ($ruleSelect.attr('disabled') === undefined) {
                                        return;
                                    }
                                    activeApply();
                                    $ruleSelect.attr('disabled', false);
                                    $ruleSelect.selectpicker('refresh');
                                    $sourceSelect.attr('disabled', true);
                                    $sourceSelect.selectpicker('refresh')
                                    processType.style.display = 'none';

                                    if (!$ruleSelect.val()) {
                                        groupField.style.display = 'none';
                                    }
                                }
                                ruleRadio.addEventListener('click', ruleRadioHandler);

                                let lastGroup;
                                $sourceSelect.on('change', (evt) => {
                                    activeApply();
                                    processForm.style.display = '';
                                    groupField.style.display = '';
                                    for (let btn of processBtn) {
                                        btn.style.display = 'none';
                                    }
                                    processType.style.display = '';
                                    if ($sourceSelect.val() == DEVICE_STATUS_DATA || $sourceSelect.val() == INNOAGE_STATUS_DATA) {
                                        booleanBtn.style.display = '';
                                        booleanForm.style.display = '';
                                        ratioForm.style.display = 'none';
                                        numericalForm.style.display = 'none';
                                        reloadBooleanWidget(null, function () {
                                            containerFluid.scrollTop = containerFluid.scrollHeight;
                                        });
                                    } else {
                                        for (let btn of processBtn) {

                                            if (btn.id == 'boolean-btn') {
                                                continue;
                                            }
                                            btn.style.display = '';
                                        }
                                        booleanForm.style.display = 'none';

                                        if (ratioBtn.classList.contains('selected')) {
                                            ratioForm.style.display = '';
                                        } else if (numericalBtn.classList.contains('selected')) {
                                            numericalForm.style.display = '';
                                        }

                                        parsedData.DataSelect.some((item, idex) => {
                                            let tmpIdx = findIndexInArray(item.DataOption, 'Id', Number($sourceSelect.val()));
                                            if (~tmpIdx) {
                                                if (idex === lastGroup) {
                                                    return true;
                                                }
                                                $denominatorSelect.empty();
                                                $denominatorSelect.selectpicker('refresh');
                                                addOption($denominatorSelect, [{
                                                    Id: 0,
                                                    Name: 'None'
                                                }]);
                                                addGroupWithOpion($denominatorSelect, item.GroupOption, item.DataOption);
                                                if (item.GroupOption.Id == STORAGE_DATAGROUP) {
                                                    let options = $denominatorSelect.find('option');
                                                    let idx = findIndexInArray(options, 'value', STORAGE_PARTITION_CAPACITY);
                                                    options[idx].disabled = true;
                                                }
                                                $denominatorSelect.selectpicker('refresh');
                                                lastGroup = idex;
                                                return true;
                                            }
                                        });
                                    }
                                });

                                function isValidInterval(target, dataInterval) {
                                    var state = {
                                        isNaN: false,
                                        isDuplicate: false,
                                        isValid: true,
                                        isEmpty: true,
                                    }

                                    for (let element of target) {
                                        let value = element.value;

                                        if (isEmpty(value)) {
                                            continue;
                                        } else {
                                            if (isNaN(value)) {
                                                state.isNaN = true;
                                                state.isValid = false;
                                                continue;
                                            }

                                            if (~dataInterval.indexOf(Number(value))) {
                                                state.isDuplicate = true;
                                                state.isValid = false;
                                                continue;
                                            }

                                            dataInterval.push(Number(element.value));
                                            state.isEmpty = false;
                                        }
                                    }

                                    if (state.isDuplicate || state.isNaN) {
                                        dataInterval = [];
                                    } else if (dataInterval.length > 1) {
                                        dataInterval.sort(function (a, b) {
                                            return b - a;
                                        });
                                    }
                                    return state;
                                }

                                function reloadWidget(setting) {
                                    var state = isValidInterval(setting.target, setting.dataInterval);
                                    failAlert.style.display = 'none';
                                    var index = 1;
                                    failAlert.innerHTML = '';
                                    if (state.isNaN) {
                                        failAlert.innerHTML = `<div>${index++}. Please only input numbers in the interval section.</div>`;
                                    }

                                    if (state.isDuplicate) {
                                        failAlert.innerHTML += `<div>${index}. Please do not input the same numbers in the interval section.</div>`;
                                    }

                                    if (state.isValid) {
                                        let label;

                                        switch (setting.chartType) {
                                            case BAR_CHART:
                                                label = makeLabel(setting.dataInterval, false, setting.unit);
                                                makeMockBarChart(setting.rootId, setting.chartId, setting.chartNameId, setting.chartName, label, "col-md-8 col-sm-8 col-xs-8");
                                                break;
                                            case PIE_CHART:
                                                label = makeLabel(setting.dataInterval, false, setting.unit);
                                                makeMockPieChart(setting.rootId, setting.chartId, setting.chartNameId, setting.chartName, label, "col-md-8 col-sm-8 col-xs-8");
                                                break;
                                            case DONUT_CHART:
                                                label = makeLabel(setting.dataInterval, false, setting.unit);
                                                makeMockDonutChart(setting.rootId, setting.chartId, setting.chartNameId, setting.chartName, label, "col-md-8 col-sm-8 col-xs-8");
                                                break;
                                            case TEXT_CHART:
                                                label = makeLabel(setting.dataInterval, true, setting.unit);
                                                makeMockTextChart(setting.rootId, setting.chartId, setting.chartNameId, setting.chartName, label, "col-md-8 col-sm-8 col-xs-8");
                                                break;
                                        }
                                    } else {
                                        setTimeout(() => {
                                            failAlert.style.display = ''
                                        }, MESSAGE_DELAY_MILLISECOND);
                                    }
                                    containerFluid.scrollTop = containerFluid.scrollHeight;
                                }

                                function ratioHandler(evt) {
                                    if (ratioForm.style.display == '')
                                        return;

                                    activeApply();
                                    ratioBtn.classList.add('selected');
                                    ratioBtn.value = 1;
                                    numericalBtn.value = 0;
                                    numericalBtn.classList.remove('selected');

                                    ratioForm.style.display = '';
                                    numericalForm.style.display = 'none';
                                    var chartType = $chartSelect.val();

                                    if (chartType == TEXT_CHART) {
                                        for (let element of percentageInterval) {
                                            element.style.display = 'none';
                                        }
                                        percentageFirst.style.display = '';
                                    } else {
                                        for (let element of percentageInterval) {
                                            element.style.display = '';
                                        }
                                    }
                                    reloadWidget({
                                        target: percentageInterval,
                                        dataInterval: [],
                                        unit: null,
                                        chartType: chartType,
                                        rootId: 'percentage-chart',
                                        chartId: 'content-percentage-chart',
                                        chartNameId: 'name-percentage',
                                        chartName: $('#widget-name-edit').val(),
                                    });
                                }
                                ratioBtn.addEventListener('click', ratioHandler);

                                function numericalHandler(evt) {
                                    if (numericalForm.style.display == '')
                                        return;

                                    activeApply();
                                    ratioBtn.value = 0;
                                    numericalBtn.value = 1;
                                    numericalBtn.classList.add('selected');
                                    ratioBtn.classList.remove('selected');
                                    numericalForm.style.display = ''
                                    ratioForm.style.display = 'none';

                                    var chartType = $chartSelect.val();

                                    if (chartType == TEXT_CHART) {
                                        for (let element of numericalInterval) {
                                            element.style.display = 'none';
                                        }
                                        numericalFirst.style.display = '';
                                    } else {
                                        for (let element of numericalInterval) {
                                            element.style.display = '';
                                        }
                                    }

                                    reloadWidget({
                                        target: numericalInterval,
                                        dataInterval: [],
                                        unit: $sourceSelect.find(':selected').attr('data-unit'),
                                        chartType: chartType,
                                        rootId: 'numerical-chart',
                                        chartId: 'content-numerical-chart',
                                        chartNameId: 'name-numerical',
                                        chartName: $('#widget-name-edit').val(),
                                    });
                                }
                                numericalBtn.addEventListener('click', numericalHandler);

                                function reloadRatioWidget(evt) {
                                    failAlert.style.display = 'none';
                                    reloadWidget({
                                        target: percentageInterval,
                                        dataInterval: [],
                                        unit: null,
                                        chartType: $chartSelect.val(),
                                        rootId: 'percentage-chart',
                                        chartId: 'content-percentage-chart',
                                        chartNameId: 'name-percentage',
                                        chartName: $('#widget-name-edit').val(),
                                    });
                                }

                                for (let element of percentageInterval) {
                                    element.value = '';
                                    element.addEventListener('change', reloadRatioWidget);
                                }

                                function reloadNumbericalWidget(evt) {
                                    failAlert.style.display = 'none';
                                    reloadWidget({
                                        target: numericalInterval,
                                        dataInterval: [],
                                        unit: $sourceSelect.find(':selected').attr('data-unit'),
                                        chartType: $chartSelect.val(),
                                        rootId: 'numerical-chart',
                                        chartId: 'content-numerical-chart',
                                        chartNameId: 'name-numerical',
                                        chartName: $('#widget-name-edit').val(),
                                    });
                                }

                                for (let element of numericalInterval) {
                                    element.value = '';
                                    element.addEventListener('change', reloadNumbericalWidget);
                                }

                                var btnHandler = function (callBack) {
                                    failAlert.textContent = '';
                                    failAlert.style.display = 'none';
                                    successAlert.style.display = 'none'

                                    var selected = $groupSelect.find("option:selected");
                                    var arrSelected = [];

                                    selected.each((idx, val) => {
                                        arrSelected.push(Number(val.value));
                                    });

                                    var payload = {
                                        WidgetId: widgetId,
                                        Name: $widgetName.val(),
                                        chartId: $chartSelect.val(),
                                        width: $widthSelect.val(),
                                        DataCount: 1,
                                        BranchIdList: arrSelected,
                                        DataId: (sourceRadio.checked) ? $sourceSelect.val() : null,
                                        ThresholdId: (ruleRadio.checked) ? $ruleSelect.val() : null,
                                        SettingStr: null
                                    };

                                    if (isEmpty(payload.Name)) {
                                        failAlert.textContent = 'Please give your widget a name first.';
                                    } else if (payload.Name.length > 16) {
                                        failAlert.textContent = 'The widget name can not exceed 16 words.'
                                    } else if (isEmpty(payload.chartId)) {
                                        failAlert.textContent = 'Please select a chart.';
                                    } else if (isEmpty(payload.width)) {
                                        failAlert.textContent = 'Please select a width for the chart.';
                                    } else if (payload.chartId == GOOGLEMAP_CHART || payload.chartId == OPENSTREETMAP_CHART || payload.chartId == GAODEMAP_CHART) {
                                        payload.DataId = DEVICE_LOCATION_DATA;
                                        if (isEmpty(longitude.value)) {
                                            failAlert.textContent = 'Please fill in the map center longitude.';
                                        } else if (!isValidLongitude(longitude.value)) {
                                            failAlert.textContent = 'Please fill in the right map center longitude.';
                                        } else if (isEmpty(latitude.value)) {
                                            failAlert.textContent = 'Please fill in the map center latitude.';
                                        } else if (!isValidLatitude(latitude.value)) {
                                            failAlert.textContent = 'Please fill in the right map center latitude.';
                                        } else {
                                            payload.SettingStr = JSON.stringify({
                                                Func: 5,
                                                lng: longitude.value,
                                                lat: latitude.value
                                            });
                                        }

                                    } else if (payload.chartId == VECTORMAP_CHART) {
                                        payload.DataId = DEVICE_LOCATION_DATA;
                                        if (isEmpty($vectorMapSelect.val())) {
                                            failAlert.textContent = 'Please select one location.';
                                        } else {
                                            payload.SettingStr = JSON.stringify({
                                                Func: 6,
                                                MapIndex: Number($vectorMapSelect.val())
                                            });
                                        }
                                    } else {
                                        if (sourceRadio.checked) {
                                            if (isEmpty($sourceSelect.val())) {
                                                failAlert.textContent = 'Please select a source.';
                                            } else if ($sourceSelect.val() == DEVICE_STATUS_DATA || $sourceSelect.val() == INNOAGE_STATUS_DATA) {
                                                payload.SettingStr = JSON.stringify({
                                                    Label: [booleanLabel[0].value, booleanLabel[1].value],
                                                    Func: 3,
                                                });
                                            } else {
                                                if (ratioBtn.value === '1') {
                                                    if (isEmpty($denominatorSelect.val())) {
                                                        failAlert.textContent = 'Please select a denominator.';
                                                    } else {
                                                        let dataInterval = [];
                                                        let state = isValidInterval(percentageInterval, dataInterval);
                                                        if (state.isEmpty) {
                                                            failAlert.textContent = 'Please fill in a number.';
                                                        } else if (state.isNaN) {
                                                            failAlert.textContent = 'Please only input numbers in the interval section.';
                                                        } else if (state.isDuplicate) {
                                                            failAlert.textContent = 'Please do not input the same numbers in the interval section.';
                                                        } else {
                                                            payload.SettingStr = JSON.stringify({
                                                                Func: 2,
                                                                Divider: {
                                                                    Percentage: dataInterval,
                                                                    DenominatorId: Number($denominatorSelect.val()),
                                                                }
                                                            });
                                                        }
                                                    }
                                                } else if (numericalBtn.value === '1') {
                                                    let dataInterval = [];
                                                    let state = isValidInterval(numericalInterval, dataInterval);
                                                    if (state.isEmpty) {
                                                        failAlert.textContent = 'Please fill in a number.';
                                                    } else if (state.isNaN) {
                                                        failAlert.textContent = 'Please only input numbers in the interval section.';
                                                    } else if (state.isDuplicate) {
                                                        failAlert.textContent = 'Please do not input the same numbers in the interval section.';
                                                    } else {
                                                        payload.SettingStr = JSON.stringify({
                                                            Func: 4,
                                                            Divider: {
                                                                Number: dataInterval
                                                            }
                                                        });
                                                    }
                                                }
                                            }
                                        } else if (ruleRadio.checked) {
                                            if (isEmpty($ruleSelect.val())) {
                                                failAlert.textContent = 'Please select a threshold rule.';
                                            }
                                        } else {
                                            failAlert.textContent = 'Please select source or threshold rule.';
                                        }
                                    }

                                    if (!failAlert.textContent) {
                                        let apiHandler = API();
                                        let promise = apiHandler.PUT('/WidgetAPI/Update', payload);

                                        promise.done((response) => {
                                            if (payload.name !== parsedData.WidgetTemplate.Name) {
                                                let $widgetSelect = $('#widgetlist');
                                                $widgetSelect.find("option:selected").text(payload.Name);
                                                $widgetSelect.selectpicker('refresh');
                                            }
                                            if (typeof callBack === 'function') {
                                                callBack();
                                            } else {
                                                successAlert.textContent = 'Widget setup successfully.';

                                                setTimeout(() => {
                                                    successAlert.style.display = '';
                                                }, MESSAGE_DELAY_MILLISECOND);
                                            }
                                        });
                                        promise.fail((response) => {
                                            if (403 === response.status) {
                                                let errorCode = JSON.parse(response.responseText).ErrorCode;
                                                if (errorCode === 0) {
                                                    fire(document, 'logout');
                                                } else if (errorCode === 1) {
                                                    failAlert.textContent = 'Sorry, you do not have access to create threshold rule.';
                                                    setTimeout(() => {
                                                        failAlert.style.display = ''
                                                    }, MESSAGE_DELAY_MILLISECOND);
                                                }
                                            } else {
                                                failAlert.textContent = 'Failed to edit widget property.';
                                                setTimeout(() => {
                                                    failAlert.style.display = ''
                                                }, MESSAGE_DELAY_MILLISECOND);
                                            }
                                        });
                                    } else {
                                        setTimeout(() => {
                                            failAlert.style.display = '';
                                        }, MESSAGE_DELAY_MILLISECOND);
                                    }
                                }
                                var handleOkBtnClick = btnHandler.bind(null, function () {
                                    $modal.modal('hide');
                                })
                                var namekeydownHandler = function (event) {
                                    var allowedCode = [8, 32, 46];
                                    //8: BackSpace; 32: space;  46: Delete;
                                    var cursorMove = [35, 36, 37, 39];
                                    var charCode = (event.charCode) ? event.charCode : ((event.keyCode) ? event.keyCode :
                                        ((event.which) ? event.which : 0));

                                    if (cursorMove.indexOf(charCode) != -1) {
                                        return true;
                                    } else if (charCode == 13 || charCode > 31 && (charCode < 64 || charCode > 90) &&
                                        (charCode < 97 || charCode > 122) &&
                                        (charCode < 48 || charCode > 57) &&
                                        (allowedCode.indexOf(charCode) == -1)) {
                                        event.preventDefault();
                                        return false;
                                    } else {
                                        activeApply();
                                        let chartList = processForm.getElementsByClassName('chart');
                                        for (let chart of chartList) {
                                            let name = chart.getElementsByTagName('span');
                                            if (name.length > 0) {
                                                $(document).ready(() => {
                                                    name[0].textContent = event.target.value;
                                                });
                                            }
                                        }
                                    }
                                };

                                $widgetName.on('keydown', namekeydownHandler);

                                $ruleSelect.on('change', (evt) => {
                                    activeApply();
                                    groupField.style.display = '';
                                });

                                function reloadBooleanWidget(evt, callBack) {
                                    var label = [];
                                    for (let input of booleanLabel) {
                                        label.push(input.value);
                                    }

                                    switch ($chartSelect.val()) {
                                        case BAR_CHART:
                                            makeMockBarChart('boolean-chart', 'content-boolean-chart', 'name-boolean', $widgetName.val(), label, "col-md-8 col-sm-8 col-xs-8");
                                            break;
                                        case PIE_CHART:
                                            makeMockPieChart('boolean-chart', 'content-boolean-chart', 'name-boolean', $widgetName.val(), label, "col-md-8 col-sm-8 col-xs-8");
                                            break;
                                        case DONUT_CHART:
                                            makeMockDonutChart('boolean-chart', 'content-boolean-chart', 'name-boolean', $widgetName.val(), label, "col-md-8 col-sm-8 col-xs-8");
                                            break;
                                        case TEXT_CHART:
                                            makeMockTextChart('boolean-chart', 'content-boolean-chart', 'name-boolean', $widgetName.val(), label, "col-md-8 col-sm-8 col-xs-8");
                                            break;
                                    }

                                    if (typeof callBack == 'function') {
                                        callBack();
                                    }
                                }

                                for (let element of booleanLabel) {
                                    element.addEventListener('change', reloadBooleanWidget);
                                }

                                var valueFieldHandler = function (event) {
                                    var charCode = (event.which) ? event.which : event.keyCode;
                                    var allowedCode = [8, 32, 46, 110];
                                    //8: BackSpace; 32: space;  46: Delete; 13: Enter
                                    if (charCode == 13 || charCode > 31 && (charCode < 48 || charCode > 57) && !(charCode > 95 && charCode < 106) && (allowedCode.indexOf(charCode) == -1))
                                        return false;
                                    activeApply();
                                    this.removeEventListener('keydown', valueFieldHandler);
                                    return true;
                                }
                                $modal.on('hidden.bs.modal', function () {
                                    $deviceLocationSelect.selectpicker('destroy');
                                    $vectorMapSelect.empty();
                                    $vectorMapSelect.selectpicker('destroy');
                                    $groupSelect.empty();
                                    $groupSelect.selectpicker('destroy');
                                    $groupSelect.off();
                                    $sourceSelect.empty();
                                    $sourceSelect.selectpicker('destroy');
                                    $sourceSelect.off();
                                    $ruleSelect.empty();
                                    $ruleSelect.selectpicker('destroy');
                                    $ruleSelect.off();
                                    $chartSelect.empty();
                                    $chartSelect.selectpicker('destroy');
                                    $chartSelect.off();
                                    $widthSelect.empty();
                                    $widthSelect.selectpicker('destroy');
                                    $widthSelect.off();
                                    sourceRadio.removeEventListener('click', sourceRadioHandler);
                                    ruleRadio.removeEventListener('click', ruleRadioHandler);
                                    $denominatorSelect.off('change');
                                    applyBtn.removeEventListener('click', btnHandler);
                                    okBtn.removeEventListener('click', handleOkBtnClick);
                                    for (let element of percentageInterval) {
                                        element.removeEventListener('keydown', valueFieldHandler);
                                    }
                                    for (let element of numericalInterval) {
                                        element.removeEventListener('keydown', valueFieldHandler);
                                    }
                                    longitude.removeEventListener('keydown', valueFieldHandler);
                                    latitude.removeEventListener('keydown', valueFieldHandler);
                                    $modal.off();
                                    backModalController.removeFade();

                                });

                                $(document).ready(() => {
                                    if (parsedData.WidgetTemplate.ChartId) {
                                        $chartSelect.selectpicker('val', parsedData.WidgetTemplate.ChartId);
                                        let setWidthcallBack = function (width) {
                                            let setSetting = function (WidgetTemplate) {
                                                if (WidgetTemplate.ChartId == GOOGLEMAP_CHART || WidgetTemplate.chartId == OPENSTREETMAP_CHART || WidgetTemplate.ChartId == GAODEMAP_CHART) {
                                                    longitude.value = WidgetTemplate.MapCenter.lng;
                                                    latitude.value = WidgetTemplate.MapCenter.lat;
                                                } else if (WidgetTemplate.ChartId == VECTORMAP_CHART) {
                                                    $vectorMapSelect.selectpicker('val', WidgetTemplate.MapIndex);
                                                } else if (WidgetTemplate.DataId) {
                                                    sourceRadio.checked = true;
                                                    fire(sourceRadio, 'click');
                                                    $sourceSelect.selectpicker('val', WidgetTemplate.DataId);
                                                    $sourceSelect.trigger('change');

                                                    switch (WidgetTemplate.ProcessType) {
                                                        case RATIO_PROCESS:
                                                            ratioBtn.value = 1;
                                                            fire(ratioBtn, 'click');
                                                            $denominatorSelect.selectpicker('val', WidgetTemplate.Percentage.DenominatorId);
                                                            WidgetTemplate.Percentage.Divider.forEach((value, idx) => {
                                                                percentageInterval[idx].value = value;
                                                            });
                                                            break;
                                                        case NUMERICAL_PROCESS:
                                                            numericalBtn.value = 1;
                                                            fire(numericalBtn, 'click');
                                                            WidgetTemplate.Numberical.Divider.forEach((value, idx) => {
                                                                numericalInterval[idx].value = value;
                                                            });
                                                            break;
                                                        case BOOLEAN_PROCESS:
                                                            WidgetTemplate.Boolean.label.forEach((label, idx) => {
                                                                booleanLabel[idx].value = label;
                                                            });
                                                            fire(booleanLabel[1], 'change');
                                                    }

                                                } else if (WidgetTemplate.ThresholdId) {
                                                    ruleRadio.checked = true;
                                                    fire(ruleRadio, 'click')
                                                    $ruleSelect.attr('disabled', false);
                                                    $ruleSelect.selectpicker('val', WidgetTemplate.ThresholdId);
                                                    $ruleSelect.trigger('change');
                                                }

                                                $denominatorSelect.on('change', () => {
                                                    activeApply();
                                                });

                                                for (let element of percentageInterval) {
                                                    element.addEventListener('keydown', valueFieldHandler);
                                                }
                                                for (let element of numericalInterval) {
                                                    element.addEventListener('keydown', valueFieldHandler);
                                                }

                                                $groupSelect.on('change', activeApply.bind(null, function () {
                                                    $groupSelect.off('change', activeApply);
                                                }));
                                                longitude.addEventListener('keydown', valueFieldHandler);
                                                latitude.addEventListener('keydown', valueFieldHandler);
                                                $vectorMapSelect.on('change', activeApply.bind(null, function () {
                                                    $vectorMapSelect.off('change', activeApply);
                                                }));
                                            }

                                            $widthSelect.selectpicker('val', width);
                                            $widthSelect.trigger('change', setSetting.bind(null, parsedData.WidgetTemplate));
                                        }

                                        $chartSelect.trigger('change', setWidthcallBack.bind(null, parsedData.WidgetTemplate.Width));

                                        parsedData.WidgetTemplate.BranchIdList && parsedData.WidgetTemplate.BranchIdList.forEach((id) => {
                                            $groupSelect.find(`option[value=${id}]`).prop('selected', true);
                                            $groupSelect.selectpicker('refresh');
                                        });

                                        setTimeout(() => {
                                            containerFluid.scrollTop = 0;
                                            applyBtn.disabled = true;
                                        }, null);
                                    }
                                    okBtn.addEventListener('click', handleOkBtnClick);
                                    applyBtn.addEventListener('click', btnHandler);
                                });

                                $modal.css('overflow-y', 'scroll');
                            })(response);
                            $body.removeClass("loading");
                        });
                        function EditAPIError(response) {
                            var failAlert = document.getElementById('danger-widget-setting');

                            if (403 == response.status) {
                                fire(document, 'logout');
                            } else {
                                failAlert.textContent = "The widget could not be found.";

                                setTimeout(() => {
                                    failAlert.style.display = '';
                                    $body.removeClass("loading");
                                }, MESSAGE_DELAY_MILLISECOND);
                            }
                        }
                        promise.fail(EditAPIError);
                    }
                })($('#widgetlist').val());
            });
            $delete.click(function () {
                hideAlert();
                (function deleteWidget(widgetId) {
                    if (widgetId) {
                        if (confirm("The widget " + `"${$("#widgetlist :selected").text()}"` + " will be deleted, are you sure?")) {
                            var $body = $("body");
                            $body.addClass('loading');
                            var promise = $.ajax({
                                type: 'DELETE',
                                url: 'WidgetAPI/Delete',
                                async: true,
                                crossDomain: true,
                                headers: {
                                    'token': $.cookie('token'),
                                    'widgetId': widgetId
                                }
                            });

                            function toDoWhenDeleteSuccess(response) {
                                var $widgetSelect = $('#widgetlist');
                                $widgetSelect.empty();
                                $widgetSelect.selectpicker('refresh');
                                var successAlert = document.getElementById('success-widget-setting');
                                successAlert.textContent = 'Widget successfully deleted.'
                                setTimeout(() => {
                                    successAlert.style.display = ''
                                }, MESSAGE_DELAY_MILLISECOND);
                                var apiHandler = API();
                                var promise = apiHandler.GET('WidgetAPI/GetWidgetList');

                                promise.done((response) => {
                                    var parsedData = JSON.parse(response);
                                    addOption($widgetSelect, parsedData);
                                    $widgetSelect.selectpicker('refresh');
                                    $body.removeClass('loading')
                                });
                            }

                            promise.done(toDoWhenDeleteSuccess);

                            function toDoWhenDeleteFail(response) {
                                var failAlert = document.getElementById('danger-widget-setting');
                                let blankAlert = document.getElementById('blank-alert');

                                if (403 == response.status) {
                                    let errorCode = JSON.parse(response.responseText).ErrorCode;
                                    if (errorCode === 0) {
                                        fire(document, 'logout');
                                    } else if (errorCode === 1) {
                                        failAlert.textContent = 'Sorry, you do not have access to delete widget.'
                                    }
                                } else {
                                    failAlert.textContent = 'Failed to delete the widget!';
                                }

                                setTimeout(() => {
                                    failAlert.style.display = '';
                                    blankAlert.scroll.display = '';
                                    $body.removeClass('loading');
                                }, MESSAGE_DELAY_MILLISECOND);

                                $body.removeClass('loading');
                            }
                            promise.fail(toDoWhenDeleteFail);
                        }
                    } else {
                        setTimeout(() => {
                            var $dangerAlert = $('#danger-widget-setting');
                            var $blank = $('#blank-alert');

                            $dangerAlert.html('Please select one widget.');
                            $blank.show();
                            $dangerAlert.show();
                        }, MESSAGE_DELAY_MILLISECOND);
                    }
                })($('#widgetlist').val());
            });
        })();

        var apiHandler = API();
        var promise = apiHandler.GET('WidgetAPI/GetWidgetList');

        promise.done((response) => {
            var $widgetSelect = $('#widgetlist');

            var parsedData = JSON.parse(response);
            addOption($widgetSelect, parsedData);
            $widgetSelect.selectpicker();
            $body.removeClass('loading')
        });

        promise.fail((response) => {
            if (403 === response.status) {
                fire(document, 'logout');
            }
            $body.removeClass('loading')
        });
    }
    $(document).on("reload-setting-widget", reloadSettingWidget);
})();