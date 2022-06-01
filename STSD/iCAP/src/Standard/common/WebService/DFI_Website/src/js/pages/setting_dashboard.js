import { makeSelect } from "../library/select_form";
import { hideAlertById, hideAlertByClassName, showMyAlert } from '../library/form_alert_controller';
import { unlisten, fire } from '../library/event_handler';
import { isEmpty, isNaturalNumber, isValidLength } from "../library/data_verification";
import { modalBack } from "../library/modal";
import { API, } from "../library/api_handler";

(function IIFE() {
    function reloadSettingDashboard() {
        var $body = $("body");
        $body.addClass("loading");

        (function initRefreshRate() {
            var time = $.cookie('dashboardUpload');

            if (time) {
                $("#time").val(time);
            }
            else {
                $("#time").val(30);
            }
        })();

        (function listenDashboardSetting() {
            var alertId = ['danger-dashboard-setting', 'success-dashboard-setting'];
            var $editDashboard = $('#edit-dashboard');
            var $createDashboard = $('#create-dashboard');
            var $deleteDashboard = $('#delete-dashboard');
            var $updateDashboard = $('#update-dashboard');

            $createDashboard.click(function () {
                hideAlertById(alertId);
                (function showCreateDashboardModal() {
                    var $submitButton = $('#submit-create-dashboard');
                    var $createDashbaordModal = $("#create-dashboard-modal");
                    var initCreateDashboardModal = function () {
                        var alertId = ['fail-create-dashboard', 'success-create-dashboard'];
                        hideAlertById(alertId);
                        $createDashbaordModal.on('hidden.bs.modal', function () {
                            var unlistenId = ['submit-create-dashboard', 'create-dashboard-modal'];
                            var dashboardName = document.getElementById("dashboard-name");
                            alertId = ['fail-create-dashboard', 'blank-create-dashboard'];
                            unlisten(unlistenId);
                            hideAlertById(alertId);
                            dashboardName.value = "";
                            modalBackController.removeFade();
                        });
                    };
                    var createDashboard = function () {
                        var name = document.getElementById("dashboard-name").value;
                        var alertId = ['fail-create-dashboard', 'blank-create-dashboard', 'success-create-dashboard'];
                        hideAlertById(alertId);
                        var failAlert = document.getElementById('fail-create-dashboard');
                        var blankAlert = document.getElementById('blank-create-dashboard');
                        if (true == isEmpty(name)) {
                            failAlert.textContent = 'Please fill in the name.'
                            setTimeout(() => {
                                failAlert.style.display = '';
                                blankAlert.style.display = '';
                            }, 100);
                        } else if (false == isValidLength(name, 20)) {
                            failAlert.textContent = 'Sorry, your dashboard name must be within 20 characters long.'
                            setTimeout(() => {
                                failAlert.style.display = '';
                                blankAlert.style.display = '';
                            }, 100);
                        } else {
                            let $body = $("body");
                            $body.addClass('loading');
                            let apiHandler = API();
                            let promise = apiHandler.POST('DashboardAPI/Create', $("#dashboard-name").val());

                            promise.done((response) => {
                                promise = apiHandler.GET('DashboardAPI/GetDashboardList');
                                promise.done((response) => {
                                    var parsedData = JSON.parse(response);
                                    $dashboardSelect.empty();
                                    $dashboardSelect.selectpicker('refresh');
                                    addOption($dashboardSelect, parsedData.List);

                                    $(document).ready(() => {
                                        var lastValue = $('#dashboardlist option:last-child').val();

                                        $createDashbaordModal.modal('hide')
                                        $dashboardSelect.val(lastValue);
                                        $dashboardSelect.selectpicker('refresh');
                                        $editDashboard.trigger('click');
                                    });

                                    $body.removeClass('loading')
                                });
                            });

                            promise.fail(() => {
                                if (403 === response.status) {
                                    fire(document, 'logout');
                                }
                                $body.removeClass('loading')
                            });
                        }
                    };
                    var modalBackController = modalBack();

                    modalBackController.fade();
                    hideAlertById(['danger-dashboard-setting']);
                    initCreateDashboardModal();
                    $submitButton.click(createDashboard);
                    $createDashbaordModal.modal({
                        backdrop: false
                    });
                    $createDashbaordModal.modal("show");
                })();
            });
            $editDashboard.click(function () {
                hideAlertById(alertId);

                (function showEditDashboardModal(id) {
                    hideAlertById(['danger-dashboard-setting']);
                    if (false == id) {
                        $('#danger-dashboard-setting').html('Please select one dashboard.');
                        setTimeout(() => { $('#danger-dashboard-setting').show(); }, 100);
                    } else {
                        var $body = $("body");
                        $body.addClass("loading");

                        var promise = $.ajax({
                            type: 'GET',
                            url: 'DashboardAPI/Edit',
                            async: true,
                            crossDomain: true,
                            headers: {
                                'token': $.cookie('token'),
                                'dashboardId': id
                            },
                            processData: false,
                        });

                        function initEditForm(response) {
                            var parsedData = JSON.parse(response);
                            var $editModal = $("#edit-dashboard-modal");
                            var $dashboardName = $('#dashboard-name-edit');
                            var $body = $("body");
                            var modalBackController = modalBack();

                            modalBackController.fade();
                            $dashboardName.val(parsedData.Name);

                            (function makeWidgetSelect(option) {
                                makeSelect('widgetlist', null, option);
                                $('.actions-btn').removeClass('btn');
                                $('.actions-btn').addClass('btn-select');
                            })(parsedData.WidgetInfo);

                            (function initWidgetSelect(option) {
                                option && option.forEach((element) => {
                                    $(`#widgetlist option[value=${element}]`).prop('selected', true);
                                });
                                $('#widgetlist').selectpicker('refresh');
                                $('#edit-dashboard-modal .dropdown-menu').css('width', 'inherit');
                            })(parsedData.WidgetIdList);

                            var $applyBtn = $('#apply-edit');
                            var $widgetSelect = $('#widgetlist');
                            $applyBtn.attr('disabled', true);

                            var applyHandler = function (callBack, dashboardId, nameOld, widgetIdOld) {
                                hideAlertByClassName(['edit-alert']);
                                var name = $('#dashboard-name-edit').val();

                                if (true == isEmpty(name)) {
                                    showMyAlert("fail-edit", "Please fill in the name.", "alert-blank-edit");
                                } else if (false == isValidLength(name, 20)) {
                                    showMyAlert("fail-edit", "Sorry, your group name must be within 20 characters long.", "alert-blank-edit");
                                } else {
                                    var selected = $("#widgetlist").find("option:selected");
                                    var arrSelected = [];

                                    selected.each((idx, val) => {
                                        arrSelected.push(Number(val.value));
                                    });

                                    var widgetOrder = [];

                                    if (widgetIdOld != null) {
                                        widgetOrder = widgetIdOld.filter(function (value) {
                                            return arrSelected.includes(value);
                                        });
                                    }

                                    arrSelected.forEach((element) => {
                                        if (false == widgetOrder.includes(element)) {
                                            widgetOrder.push(element);
                                        }
                                    });

                                    var payloadObject = {
                                        Id: dashboardId,
                                        Name: name,
                                        WidgetIdList: widgetOrder
                                    }

                                    $.ajax({
                                        async: true,
                                        method: "PUT",
                                        headers: {
                                            "token": $.cookie('token'),
                                            "content-type": "application/json ; charset=utf-8",
                                        },
                                        url: "/DashboardAPI/Save",
                                        crossDomain: true,
                                        data: JSON.stringify(payloadObject),
                                        processData: false,
                                        success: function (data) {
                                            if (nameOld != name) {
                                                $('#dashboardlist option:selected').text(name);
                                                $dashboardSelect.selectpicker('refresh');
                                            }

                                            if (typeof callBack === 'function') {
                                                callBack();
                                            } else {
                                                showMyAlert("success-edit", "Dashboard setup successfully.", "alert-blank-edit");
                                            }
                                        },
                                        error: function (response) {
                                            if (403 == response.status) {
                                                fire(document, 'logout');
                                                //showMyAlert("fail-edit", "You have been idle too long, please log-in again.", "alert-blank-edit");
                                            } else {
                                                showMyAlert("fail-edit", "Failed to edit dashboard.", "alert-blank-edit");

                                            }
                                            $body.removeClass('loading');
                                        }
                                    });
                                }
                            };

                            function listenApplyButton(dashboardId, nameOld, widgetIdOld) {
                                $applyBtn.click(applyHandler.bind(null, null, dashboardId, nameOld, widgetIdOld));

                                // $applyBtn.click(function (event, callBack) {
                                //     hideAlertByClassName(['edit-alert']);
                                //     var name = $('#dashboard-name-edit').val();

                                //     if (true == isEmpty(name)) {
                                //         showMyAlert("fail-edit", "Please fill in the name.", "alert-blank-edit");
                                //     } else if (false == isValidLength(name, 20)) {
                                //         showMyAlert("fail-edit", "Sorry, your group name must be within 20 characters long.", "alert-blank-edit");
                                //     } else {
                                //         var selected = $("#widgetlist").find("option:selected");
                                //         var arrSelected = [];

                                //         selected.each((idx, val) => {
                                //             arrSelected.push(Number(val.value));
                                //         });

                                //         var widgetOrder = [];

                                //         if (widgetIdOld != null) {
                                //             widgetOrder = widgetIdOld.filter(function (value) {
                                //                 return arrSelected.includes(value);
                                //             });
                                //         }

                                //         arrSelected.forEach((element) => {
                                //             if (false == widgetOrder.includes(element)) {
                                //                 widgetOrder.push(element);
                                //             }
                                //         });

                                //         var payloadObject = {
                                //             Id: dashboardId,
                                //             Name: name,
                                //             WidgetIdList: widgetOrder
                                //         }

                                //         $.ajax({
                                //             async: true,
                                //             method: "PUT",
                                //             headers: {
                                //                 "token": $.cookie('token'),
                                //                 "content-type": "application/json ; charset=utf-8",
                                //             },
                                //             url: "/DashboardAPI/Save",
                                //             crossDomain: true,
                                //             data: JSON.stringify(payloadObject),
                                //             processData: false,
                                //             success: function (data) {
                                //                 if (nameOld != name) {
                                //                     $('#dashboardlist option:selected').text(name);
                                //                     $dashboardSelect.selectpicker('refresh');
                                //                 }

                                //                 if (typeof callBack === 'function') {
                                //                     callBack();
                                //                 } else {
                                //                     showMyAlert("success-edit", "Dashboard setup successfully.", "alert-blank-edit");
                                //                 }
                                //             },
                                //             error: function (response) {
                                //                 if (403 == response.status) {
                                //                     fire(document, 'logout');
                                //                     //showMyAlert("fail-edit", "You have been idle too long, please log-in again.", "alert-blank-edit");
                                //                 } else {
                                //                     showMyAlert("fail-edit", "Failed to edit dashboard.", "alert-blank-edit");

                                //                 }
                                //                 $body.removeClass('loading');
                                //             }
                                //         });
                                //     }
                                // });
                            }
                            $dashboardName.on('keydown', (event) => {
                                var allowedCode = [8, 32, 46];
                                //8: BackSpace; 32: space;  46: Delete;
                                var charCode = (event.charCode) ? event.charCode : ((event.keyCode) ? event.keyCode :
                                    ((event.which) ? event.which : 0));
                                if (charCode > 31 && (charCode < 64 || charCode > 90) &&
                                    (charCode < 97 || charCode > 122) &&
                                    (charCode < 48 || charCode > 57) &&
                                    (allowedCode.indexOf(charCode) == -1)) {
                                    event.preventDefault();
                                    return false;
                                } else {
                                    $applyBtn.attr('disabled', false);
                                    $widgetSelect.off('change', activeBtn);
                                    $dashboardName.off('keydown')
                                    listenApplyButton(parsedData.Id, parsedData.Name, parsedData.WidgetIdList);
                                }
                            });
                            let activeBtn = function () {
                                if ($applyBtn.attr('disabled')) {
                                    $applyBtn.attr('disabled', false);
                                    $dashboardName.off('keydown');
                                    listenApplyButton(parsedData.Id, parsedData.Name, parsedData.WidgetIdList);
                                    $widgetSelect.off('change', activeBtn);
                                }
                            }
                            $widgetSelect.on('change', activeBtn);

                            $('#ok-edit').on('click', () => {
                                applyHandler(function closeModal() {
                                    $editModal.modal('hide');
                                    $body.removeClass('loading');
                                }, parsedData.Id, parsedData.Name, parsedData.WidgetIdList);
                            });

                            (function toDoWhenClose() {
                                $editModal.on('hidden.bs.modal', function () {
                                    hideAlertByClassName(['edit-alert']);
                                    $('.edit-event').each((idx, element) => {
                                        $(element).off();
                                    });
                                    $('#widgetlist').empty();
                                    $('#widgetlist').selectpicker('destroy');
                                    $('#dashboard-name-edit').val('');
                                    $widgetSelect.off('change', activeBtn);
                                    $dashboardName.off('keydown');
                                    $applyBtn.off();
                                    modalBackController.removeFade();
                                });
                            })();

                            $editModal.modal({
                                backdrop: false
                            });
                            $editModal.modal("show");
                            $body.removeClass("loading");
                        }

                        promise.done(initEditForm);

                        function EditAPIError(response) {
                            if (403 == response.status) {
                                $('#danger-dashboard-setting').html('You have been idle too long, please log-in again.');
                                setTimeout(() => {
                                    $('#danger-dashboard-setting').show();
                                    $("body").removeClass("loading");
                                }, 100);
                            } else {
                                $('#danger-dashboard-setting').html('Can not find the dashboard information, please refresh the page.');
                                setTimeout(() => {
                                    $('#danger-dashboard-setting').show();
                                    $("body").removeClass("loading");
                                }, 100);
                            }
                        }
                        promise.fail(EditAPIError);
                    }
                })($dashboardSelect.val());
            });
            $deleteDashboard.click(function () {
                hideAlertById(alertId);
                (function deleteDashboard(dashboardId) {
                    if (confirm("The dashboard " + `"${$("#dashboardlist :selected").text()}"` + " will be deleted, are you sure?")) {
                        let $body = $("body");
                        $body.addClass('loading');
                        let apiHandler = API();
                        let promise = apiHandler.DELETE('DashboardAPI/Delete', 'dashboardId', dashboardId);

                        promise.done(() => {
                            promise = apiHandler.GET('DashboardAPI/GetDashboardList');
                            var successAlert = document.getElementById('success-dashboard-setting');
                            successAlert.textContent = 'Dashboard successfully deleted.'
                            successAlert.style.display = '';

                            promise.done((response) => {
                                var parsedData = JSON.parse(response);
                                $dashboardSelect.empty();
                                $dashboardSelect.selectpicker('refresh');
                                addOption($dashboardSelect, parsedData.List);
                                $dashboardSelect.selectpicker('refresh');
                                $body.removeClass('loading')
                            });

                        });

                        promise.fail((response) => {
                            if (403 === response.status) {
                                fire(document, 'logout');
                            } else {
                                let failAlert = document.getElementById('danger-dashboard-setting');

                                failAlert.textContent = 'Failed to delete the dashboard!'
                                failAlert.style.display = '';
                            }
                            $body.removeClass('loading')
                        });
                    }
                })($("#dashboardlist").val());
            });
            $updateDashboard.click(function () {
                hideAlertById(alertId);
                (function updateRefreshRate() {
                    var errorFlag = false;
                    var failAlert = document.getElementById('danger-dashboard-setting');
                    if (isEmpty($("#time").val())) {
                        errorFlag = true;

                        failAlert.textContent = 'Please fill in the refresh rate.';
                        setTimeout(() => {
                            failAlert.style.display = '';
                        }, 100);
                    }
                    else if (!isNaturalNumber($("#time").val())) {
                        errorFlag = true;
                        failAlert.textContent = 'Please enter an integer greater than \"0\" for the refresh rate.';

                        setTimeout(() => {
                            failAlert.style.display = '';
                        }, 100);
                    }

                    if (!errorFlag) {
                        let successAlert = document.getElementById('success-dashboard-setting');

                        $.cookie('dashboardUpload', $("#time").val());
                        successAlert.textContent = 'Update refresh rate successfully.';

                        setTimeout(() => {
                            successAlert.style.display = '';
                        }, 100);
                    }
                })();
            });
        })();

        function addOption($select, options) {
            options.forEach((option) => {
                $select.append($('<option></option>').attr('value', option.Id).text(option.Name))
            });
        };

        var $dashboardSelect = $('#dashboardlist');
        var apiHandler = API();
        var promise = apiHandler.GET('DashboardAPI/GetDashboardList');


        promise.done((response) => {
            var parsedData = JSON.parse(response);
            addOption($dashboardSelect, parsedData.List);
            $dashboardSelect.selectpicker();
            $body.removeClass('loading')
        });

        promise.fail((response) => {
            if (403 === response.status) {
                fire(document, 'logout');
            }
            $body.removeClass('loading')
        });

    };
    $(document).on("reload-setting-dashboard", reloadSettingDashboard);
})();

