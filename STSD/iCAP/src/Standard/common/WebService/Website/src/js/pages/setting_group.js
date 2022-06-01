import { hideAlertById, hideAlertByClassName, showMyAlert, clearForm } from '../library/form_alert_controller';
import { TwoSelectForm } from "../library/select_form";
import { unlisten } from '../library/event_handler';
import { isEmpty, isValidLength, nameExists } from "../library/data_verification";
import { unlistenForm, fire } from "../library/event_handler";
import { API } from "../library/api_handler";
import { modalBack } from "../library/modal";

(function IIFE() {
    function reloadSettingGroup() {

        function addOption($select, options) {
            options.forEach((option) => {
                $select.append($('<option></option>').attr('value', option.Id).text(option.Name))
            });
        }

        var wdth = $(window).width();
        var $body = $("body");
        var $create = $('#create');
        var $groupSelect = $('#grouplist');

        if (wdth < 762) {
            $create.css('top', 11);
        }

        $(window).resize(function () {
            var wdth = $(window).width();
            if (wdth < 762) {
                $create.css('top', 11);
            } else {
                $create.css('top', 5);
            }
        });

        $body.addClass("loading");

        var apiHandler = API();
        var promise = apiHandler.GET('BranchAPI/GetList');

        promise.done((response) => {
            var parsedData = JSON.parse(response);

            addOption($groupSelect, parsedData.List);
            $groupSelect.selectpicker();
            $body.removeClass('loading')
        });

        promise.fail((response) => {
            if (403 === response.status) {
                fire(document, 'logout');
            }
            $body.removeClass('loading')
        });

        (function listenGroupSetting() {
            function initEditForm(response) {
                unlistenForm($('.event'));
                const parsed_data = JSON.parse(response);
                payload.Name = parsed_data.Detail.Name;
                _TwoSelectForm.init(parsed_data.Unselected, parsed_data.Detail.Selected);
                //initCheckBox([parsed_data.Detail.EmailFlag, parsed_data.Detail.EventFlag], ['event-flag', 'email-flag']);
                $body.removeClass("loading");
            }

            function EditAPIError(response) {
                var $dangerAlert = $('#danger-group-setting');
                var $blank = $('#blank-alert');

                if (403 == response.status) {
                    $dangerAlert.html('You have been idle too long, please log-in again.');
                    setTimeout(() => {
                        $dangerAlert.show();
                        $blank.show();
                        $body.removeClass("loading");
                    }, 100);
                } else {
                    $dangerAlert.html('Can not find the group information, please refresh the page.');
                    setTimeout(() => {
                        $dangerAlert.show();
                        $blank.show();
                        $body.removeClass("loading");
                    }, 100);
                }
            }

            function isValidName(nameId, alertDangerId, blankId, current_option_name) {
                const name = document.getElementById(nameId).value;
                var $dangerAlert = $("#" + alertDangerId);
                var $blank = $("#" + blankId);

                if (true === isEmpty(name)) {
                    $dangerAlert.html("Please fill in the name.");
                    setTimeout(() => {
                        $dangerAlert.show();
                        $blank.show();
                    }, 100);
                    return false;
                } else if (true === nameExists(current_option_name, name)) {
                    $dangerAlert.html("The group's name already exists.");
                    setTimeout(() => {
                        $dangerAlert.show();
                        $blank.show();
                    }, 100);
                    return false;
                } else if (false === isValidLength(name, 20)) {
                    $dangerAlert.html("Sorry, your group's name must be within 20 characters long.");
                    setTimeout(() => {
                        $dangerAlert.show();
                        $blank.show();
                    }, 100);
                    return false;
                } else {
                    return true;
                }
            }

            var alertId = ['danger-group-setting', 'success-group-setting', 'blank-alert'];
            var payload = {
                Id: null,
                Name: null,
                Selected: null
            };
            const count = {
                repo: 0,
                target: 0
            };
            var APICaller = API();
            var $delete = $('#delete');
            var $save = $('#save');
            var $rename = $('#rename');
            var _TwoSelectForm = TwoSelectForm(payload, count);

            $create.click(function () {
                hideAlertById(alertId);
                (function showCreateGroupModal() {
                    function createGroup() {
                        var createSuccess = function () { };
                        var createFail = function () { };

                        hideAlertByClassName(['alert-create-name']);
                        var current_option_name = $.map($('#grouplist option'), function (item, index) {
                            return item.text;
                        });

                        if (true === isValidName('group-name', 'fail-create-group', 'blank-alert-create-group', current_option_name)) {
                            $body.addClass('loading');
                            let promise = APICaller.POST('BranchAPI/Create', $("#group-name").val());
                            createSuccess = function (response) {
                                $groupSelect.empty();
                                $groupSelect.selectpicker('refresh');
                                promise = apiHandler.GET('BranchAPI/GetList');
                                promise.done((response) => {
                                    var parsedData = JSON.parse(response);
                                    $groupSelect.empty();
                                    $groupSelect.selectpicker('refresh');
                                    addOption($groupSelect, parsedData.List);
                                    $(document).ready(() => {
                                        var lastValue = $('#grouplist option:last-child').val();
                                        var $body = $('body');
                                        $groupSelect.val(lastValue);
                                        $groupSelect.selectpicker('refresh');
                                        $createGroupModal.modal('hide')

                                        var lastValue = $groupSelect.val();
                                        $groupSelect.val(lastValue);
                                        $groupSelect.selectpicker('refresh');
                                        $groupSelect.trigger('change');
                                        $body.removeClass('loading');
                                    });
                                });
                            };

                            promise.done(createSuccess);

                            createFail = function (response) {
                                let failAlert = document.getElementById('fail-create-group');
                                let blankAlert = document.getElementById('blank-alert-create-group');
                                if (409 == response.status) {
                                    failAlert.textContent = 'The group name already exists.';
                                } else if (403 == response.status) {
                                    let errorCode = JSON.parse(response.responseText).ErrorCode;
                                    if (errorCode === 0) {
                                        fire(document, 'logout');
                                    } else if (errorCode === 1) {
                                        failAlert.textContent = 'Sorry, you do not have access to create group.'
                                    }
                                } else {
                                    failAlert.textContent = 'Failed to create the group!';
                                }

                                setTimeout(() => {
                                    failAlert.style.display = '';
                                    blankAlert.style.display = '';
                                    $body.removeClass('loading');
                                }, 100);
                            };
                            promise.fail(createFail);
                        }
                    }

                    var $createGroupModal = $("#create-group-modal");
                    var $submitButton = $('#submit-create-group');
                    var modalBackController = modalBack();

                    modalBackController.fade();
                    hideAlertById(['danger-group-setting']);
                    (function initCreateGroupModal() {
                        var alertId = ['fail-create-group', 'success-create-group'];

                        hideAlertById(alertId);
                        $createGroupModal.on('hidden.bs.modal', function () {
                            var unlistenId = ['submit-create-group', 'create-group-modal'];
                            var groupName = document.getElementById("group-name");
                            alertId = ['fail-create-group', 'blank-alert-create-group'];
                            unlisten(unlistenId);
                            hideAlertById(alertId);
                            groupName.value = "";
                            modalBackController.removeFade();
                        });
                    })();
                    $submitButton.click(createGroup);
                    $createGroupModal.modal({
                        backdrop: false
                    });
                    $createGroupModal.modal("show");
                })();
            });

            $groupSelect.change((element) => {
                payload.Id = Number(element.target.value);
                $body.addClass("loading");
                clearForm();
                let promise = APICaller.GET('BranchAPI/GetDevice?id=' + payload.Id, false, false);
                promise.done((response) => { initEditForm(response); });
                promise.fail(EditAPIError);
            });

            $delete.click(function () {
                hideAlertById(alertId);

                (function deleteGroup() {

                    if ($groupSelect.val()) {
                        if (confirm("The group " + `"${$("#grouplist :selected").text()}"` + " will be deleted, are you sure?")) {
                            $body.addClass('loading');
                            let promise = APICaller.DELETE('BranchAPI/Delete', 'id', $groupSelect.val());
                            promise.done(() => {
                                clearForm();
                                $groupSelect.empty();
                                $groupSelect.selectpicker('refresh');
                                var promise = APICaller.GET('BranchAPI/GetList');
                                promise.done((response) => {
                                    var successAlert = document.getElementById('success-group-setting');
                                    var blankAlert = document.getElementById('blank-alert');

                                    var parsedData = JSON.parse(response);
                                    addOption($groupSelect, parsedData.List);
                                    $groupSelect.selectpicker('refresh');
                                    successAlert.textContent = 'The group deleted successfully.';
                                    setTimeout(() => {
                                        successAlert.style.display = '';
                                        blankAlert.style.display = ''
                                        $body.removeClass('loading');
                                    }, 100);
                                });
                            });

                            promise.fail((response) => {
                                let failAlert = document.getElementById('danger-group-setting');
                                let blankAlert = document.getElementById('blank-alert');
                                if (403 == response.status) {
                                    let errorCode = JSON.parse(response.responseText).ErrorCode;
                                    if (errorCode === 0) {
                                        fire(document, 'logout');
                                    } else if (errorCode === 1) {
                                        failAlert.textContent = 'Sorry, you do not have access to delete group.'
                                    }
                                } else {
                                    failAlert.textContent = 'Failed to delete the group!';
                                }

                                setTimeout(() => {
                                    failAlert.style.display = '';
                                    blankAlert.scroll.display = '';
                                    $body.removeClass('loading');
                                }, 100);

                                $body.removeClass('loading');
                            });
                        }
                    } else {
                        let failAlert = document.getElementById('danger-group-setting');
                        let blankAlert = document.getElementById('blank-alert');
                        failAlert.textContent = 'Please select one group.';

                        setTimeout(() => {
                            failAlert.style.display = '';
                            blankAlert.style.display = ''
                        }, 100);
                    }
                })();
            });

            $save.click(function () {
                hideAlertById(alertId);

                (function saveGroup() {
                    function toDoWhenSaveSuccess(response) {
                        showMyAlert("success-group-setting", "Group setup successfully.", "blank-alert");
                        $groupSelect.trigger('change');
                        $body.removeClass('loading');
                    }

                    function toDoWhenSaveFail(response) {
                        if (403 == response.status || 409 == response.status) {
                            var messageObj = JSON.parse(response.responseText);
                            showMyAlert("danger-group-setting", messageObj.Response, "blank-alert");
                        } else {
                            showMyAlert("danger-group-setting", "Failed to edit group.", "blank-alert");
                        }
                        $body.removeClass('loading');
                    }

                    if (payload.Id !== null) {
                        $body.addClass('loading');

                        if (payload.Selected !== null && payload.Selected.length === 0) {
                            payload.Selected = null;
                        }

                        let promise = APICaller.PUT("/BranchAPI/Save", payload);
                        promise.done(toDoWhenSaveSuccess);
                        promise.fail(toDoWhenSaveFail);
                    } else {
                        setTimeout(() => {
                            var $dangerAlert = $('#danger-group-setting');
                            var $blank = $('#blank-alert');

                            $dangerAlert.html('Please select one group.');
                            $blank.show();
                            $dangerAlert.show();
                        }, 100);
                    }
                })();
            });

            $rename.click(function () {
                hideAlertById(alertId);
                if ($groupSelect.val()) {
                    (function showRenameModal() {
                        var modalBackController = modalBack();
                        var $okBtn = $('#ok-rename');
                        var $modal = $("#rename-modal");
                        var $applyBtn = $('#apply-rename');
                        var $namefield = $('#change-group-name');
                        $applyBtn.attr('disabled', true);
                        modalBackController.fade();
                        var applyHandler = function (event, callBack) {
                            var current_option_name, renameSuccess, renameFail;

                            hideAlertByClassName(['alert-rename']);
                            current_option_name = $.map($('#grouplist option'), function (item, index) {
                                return item.text;
                            });
                            renameSuccess = function () { };
                            renameFail = function () { };

                            if (true === isValidName('change-group-name', 'fail-rename', 'blank-alert-rename', current_option_name)) {
                                renameSuccess = function (response) {
                                    if (response) {
                                        let optionSelected = $('#grouplist option:selected');
                                        let targetLabel = $('#label-box-target');

                                        payload.Id = $groupSelect.val();
                                        payload.Name = reGroupName.value;
                                        optionSelected.text(payload.Name);
                                        targetLabel.text(payload.Name + ` (${count.target})`);
                                        $groupSelect.selectpicker('refresh');
                                    }
                                    // $modal.modal('hide');

                                    if (typeof callBack === 'function') {
                                        callBack();
                                    } else {
                                        let $successAlert = $('#success-rename')
                                        $successAlert.html('Apply successfully.');
                                        setTimeout(() => {
                                            $successAlert.show();
                                            $("#blank-alert-rename").show();
                                            $body.removeClass('loading');
                                        }, 100);
                                    }
                                };
                                renameFail = function (response) {
                                    if (403 == response.status || 409 == response.status) {
                                        var messageObj = JSON.parse(response.responseText);
                                        let $failAlert = $("#fail-rename");

                                        $failAlert.html(messageObj.Response);
                                        setTimeout(() => {
                                            $failAlert.show();
                                            $("#blank-alert-rename").show();
                                            $body.removeClass('loading');
                                        }, 100);
                                    } else {
                                        let $failAlert = $("#fail-rename");

                                        $failAlert.html("Failed to rename the group. Please refresh the web page and try again.");
                                        setTimeout(() => {
                                            $failAlert.show();
                                            $("#blank-alert-rename").show();
                                            $body.removeClass('loading');
                                        }, 100);
                                    }
                                };

                                $body.addClass('loading');

                                let reGroupName = document.getElementById('change-group-name');
                                let renamePayload = {
                                    Id: $groupSelect.val(),
                                    Name: reGroupName.value
                                }
                                let promise = APICaller.PUT('BranchAPI/Rename', renamePayload);

                                promise.done(renameSuccess);
                                promise.fail(renameFail);
                            }
                        }
                        $namefield.on('keydown', (event) => {
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
                                $namefield.off('keydown')
                                $applyBtn.on('click', applyHandler);
                            }
                        });
                        $okBtn.on('click', () => {
                            applyHandler(null, function () {
                                $modal.modal('hide');
                                $body.removeClass('loading');
                            });
                        });
                        $modal.on('hidden.bs.modal', function () {
                            var groupName = document.getElementById("change-group-name");
                            hideAlertByClassName(['alert-rename']);
                            $modal.off();
                            $applyBtn.off();
                            $namefield.off('keydown');
                            $okBtn.off('click');
                            groupName.value = "";
                            modalBackController.removeFade();
                        });
                        $modal.modal({
                            backdrop: false
                        })
                        $modal.modal("show");
                    })();
                    //showRenameModal();
                } else {
                    setTimeout(() => {
                        var $dangetAlert = $('#danger-group-setting');
                        var $blank = $('#blank-alert');

                        $dangetAlert.html('Please select one group.');
                        $blank.show();
                        $dangetAlert.show();
                    }, 100);
                }
            });
        })();
    }
    $(document).on("reload-setting-group", reloadSettingGroup);
})();
