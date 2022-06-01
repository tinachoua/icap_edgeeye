import { hideAlertById, showMyAlert, clearForm, initCheckBox } from '../library/form_alert_controller';
import { TwoSelectForm, addOptions, addGroupWithOpion, addOption } from "../library/select_form";;
import { isEmpty, isValidEmail } from "../library/data_verification";
import { unlistenForm, fire } from "../library/event_handler";
import { showErrorMessage, initSelectOption, API } from "../library/api_handler";
import { modalBack } from "../library/modal";
import { INNOAGE_STATUS_DATA, DEVICE_STATUS_DATA, EMAIL_ACTION, NORAML_THRESHOLD_MODE, BOOLEAN_FUNCTION, RATIO_THRESHOLD_MODE, STORAGE_DATAGROUP, STORAGE_PARTITION_CAPACITY, MESSAGE_DELAY_MILLISECOND } from '../library/data_mapping';
import userIcon from "../../assets/images/user.svg";
import userGroupIcon from "../../assets/images/userGroup.svg";
import { findIndexInArray } from "../library/common";

(function IIFE() {
    function reloadSettingThreshold() {
        function listenRuleSetting() {
            function initEditForm(response) {
                unlistenForm($('.event'));
                const parsedData = JSON.parse(response);
                _TwoSelectForm.init(parsedData.Unselected, parsedData.Selected);
                initCheckBox([parsedData.Enable], ['enable-flag']);
                $body.removeClass("loading");
            }

            function EditAPIError(response) {
                var $dangerAlert = $('#danger-rule-setting');
                var $blank = $('#blank-alert');

                if (403 == response.status) {
                    $dangerAlert.html('You have been idle too long, please log-in again.');
                    setTimeout(() => {
                        $dangerAlert.show();
                        $blank.show();
                        $body.removeClass("loading");
                    }, MESSAGE_DELAY_MILLISECOND);
                } else {
                    $dangerAlert.html('Can not find the rule settings, please refresh the page.');
                    setTimeout(() => {
                        $dangerAlert.show();
                        $blank.show();
                        $body.removeClass("loading");
                    }, MESSAGE_DELAY_MILLISECOND);
                }
            }

            function openRuleSettingModal(threshold) {
                function init(response) {
                    var $modal = $("#rule-setting-modal");
                    var title = $modal.find('#modal-title')[0];
                    var denominatorLabel = $modal.find('#denominator-label')[0];
                    var parsedData = JSON.parse(response);
                    var modalBackController = modalBack();
                    var namefield = document.getElementById('rule-name');
                    var footer = $modal.find('.modal-footer')[0];
                    var $dataSelect = $('#datalist');
                    var $denominatorSelect = $('#denominator-select');
                    var $typeSelect = $('#duration-type-select');
                    var $statusSelect = $('#status-select');
                    var secondValueField = document.getElementById('value-second-field');
                    var firstValue = document.getElementById('duration-value-1');
                    var secondValue = document.getElementById('duration-value-2');
                    var $recipientField = $('#email-recipient-field');
                    var recipientTextarea = document.getElementById('recipient-textarea');
                    var normalBtn = document.getElementById('normal');
                    var ratioBtn = document.getElementById('ratio');
                    var denominator = document.getElementById('denominator');
                    var typeValueField = document.getElementById('type-value');
                    var actions = document.getElementById('actions');
                    var emailCheckBox = document.getElementById('sendEmail');
                    //var screenshotCheckBox = document.getElementById('screenShot');
                    var failAlert = document.getElementById('fail-rule');
                    var blankAlert = document.getElementById('blank-alert-rule');
                    var successAlert = document.getElementById('success-rule');
                    var deviceStatusField = document.getElementById('device-status');
                    const containerFluid = document.getElementById('rule-setting-modal').getElementsByClassName('container-fluid')[0];
                    const BETWEEN_TYPE = '6';

                    modalBackController.fade();
                    failAlert.style.display = 'none';
                    successAlert.style.display = 'none';
                    blankAlert.style.display = 'none';
                    title.textContent = 'Add New Rule';

                    normalBtn.classList.add('selected');
                    normalBtn.value = '1';

                    function activeApply() {
                        if (footer.lastElementChild.id === 'apply-rule' && footer.lastElementChild.disabled) {
                            footer.lastElementChild.disabled = false;
                            footer.lastElementChild.addEventListener('click', btnHandler.bind(null, save));
                            namefield.removeEventListener('keydown', nameFieldHandler);
                        }
                    }

                    function normalMode() {
                        if (normalBtn.value === '1')
                            return;
                        activeApply();
                        normalBtn.value = '1';
                        normalBtn.classList.add('selected');
                        ratioBtn.value = '0';
                        ratioBtn.classList.remove('selected');
                        denominator.style.visibility = 'hidden';
                        let options = $dataSelect.find('option')
                        let idx = findIndexInArray(options, 'value', DEVICE_STATUS_DATA);
                        options[idx].disabled = false;

                        idx = findIndexInArray(options, 'value', INNOAGE_STATUS_DATA);
                        options[idx].disabled = false;


                        $dataSelect.selectpicker('refresh');
                        if ($denominatorSelect[0].hasChildNodes()) {
                            $denominatorSelect.empty();
                            $denominatorSelect.selectpicker('refresh');
                        }
                    }
                    normalBtn.addEventListener('click', normalMode);

                    denominator.style.visibility = 'hidden';
                    denominatorLabel.style.color = 'darkgray';
                    typeValueField.classList.add('hide');
                    deviceStatusField.classList.add('hide');
                    actions.classList.add('hide');
                    ratioBtn.value = '0';

                    function ratioMode() {
                        if (ratioMode.value === '1') {
                            return;
                        }
                        activeApply();
                        //$denominatorSelect.selectpicker('refresh');
                        ratioBtn.value = '1'
                        ratioBtn.classList.add('selected');
                        normalBtn.value = '0';
                        normalBtn.classList.remove('selected');
                        denominator.style.visibility = '';
                        var dataIndex = undefined;
                        if ($dataSelect.val()) {
                            if ($dataSelect.val() == DEVICE_STATUS_DATA || $dataSelect.val() == INNOAGE_STATUS_DATA) {
                                $dataSelect.selectpicker('val', '');
                                let options = $dataSelect.find('option')
                                let idx = findIndexInArray(options, 'value', DEVICE_STATUS_DATA);

                                options[idx].disabled = true;

                                idx = findIndexInArray(options, 'value', INNOAGE_STATUS_DATA);

                                options[idx].disabled = true;


                                $dataSelect.selectpicker('refresh');
                                return;
                            }

                            parsedData.some((item, idex) => {
                                let tmpIdx = findIndexInArray(item.DataOption, 'Id', Number($dataSelect.val()));
                                if (~tmpIdx) {
                                    dataIndex = idex;
                                    return true;
                                }
                            });

                            let groupId = parsedData[dataIndex].GroupOption.Id;

                            $denominatorSelect.attr('disabled', false);
                            denominatorLabel.style.color = 'black';

                            parsedData.some((item, idex) => {
                                let tmpIdx = findIndexInArray(item.DataOption, 'Id', Number($dataSelect.val()));
                                if (~tmpIdx) {
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
                                    return true;
                                }
                            });
                            $denominatorSelect.selectpicker('val', '');

                            if (STORAGE_DATAGROUP == groupId) {
                                let options = $denominatorSelect.find('option')
                                let idx = findIndexInArray(options, 'value', STORAGE_PARTITION_CAPACITY);

                                options[idx].disabled = true;
                                $denominatorSelect.selectpicker('render');
                            }
                        }

                        let options = $dataSelect.find('option')
                        let idx = findIndexInArray(options, 'value', DEVICE_STATUS_DATA);
 
                        options[idx].disabled = true;

                        idx = findIndexInArray(options, 'value', INNOAGE_STATUS_DATA);
 
                        options[idx].disabled = true;
                        $dataSelect.selectpicker('refresh');
                    }
                    ratioBtn.addEventListener('click', ratioMode);

                    //screenshotCheckBox.checked = false;
                    emailCheckBox.checked = false;
                    secondValueField.style.visibility = 'hidden';

                    parsedData.forEach((element) => {
                        addGroupWithOpion($dataSelect, element.GroupOption, element.DataOption);
                    });

                    $dataSelect.selectpicker();
                    $denominatorSelect.selectpicker();
                    firstValue.value = '';
                    secondValue.value = '';

                    var lastGroup;
                    $dataSelect.change((evt) => {
                        activeApply();
                        const selected = evt.target.options[evt.target.selectedIndex];
                        if (selected.value === DEVICE_STATUS_DATA || selected.value === INNOAGE_STATUS_DATA) {
                            let typeInput = document.getElementById('duration-value').getElementsByTagName('input');

                            $typeSelect.selectpicker('val', '');
                            $typeSelect.selectpicker('refresh');

                            if (!typeValueField.classList.contains('hide')) {
                                typeValueField.classList.add('hide');
                            }

                            if (deviceStatusField.classList.contains('hide')) {
                                deviceStatusField.classList.remove('hide');
                            }

                            secondValueField.style.visibility = 'hidden';
                            for (let item of typeInput) {
                                item.value = '';
                            }
                        } else {
                            $statusSelect.selectpicker('val', '');
                            $statusSelect.selectpicker('refresh');

                            if (!deviceStatusField.classList.contains('hide')) {
                                deviceStatusField.classList.add('hide');
                            }

                            if (typeValueField.classList.contains('hide')) {
                                typeValueField.classList.remove('hide');
                            }

                            if (ratioBtn.value === '1') {
                                if (denominatorLabel.style.color !== 'black') {
                                    denominatorLabel.style.color = 'black';
                                    $denominatorSelect.attr('disabled', false);
                                }

                                let selectedDataGroupId = selected.parentNode.id;

                                if (lastGroup !== selectedDataGroupId) {
                                    $denominatorSelect.empty();
                                    $denominatorSelect.selectpicker('refresh');
                                    parsedData.some((item, idex) => {
                                        let tmpIdx = findIndexInArray(item.DataOption, 'Id', Number($dataSelect.val()));
                                        if (~tmpIdx) {
                                            if (idex === lastGroup) {
                                                return true;
                                            }
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

                                    if (STORAGE_DATAGROUP == selectedDataGroupId) {
                                        let options = $denominatorSelect.find('option')
                                        let idx = findIndexInArray(options, 'value', STORAGE_PARTITION_CAPACITY);

                                        options[idx].disabled = true;
                                        $denominatorSelect.selectpicker('render');
                                    }
                                }
                            }

                            if (emailCheckBox.disabled) {
                                let label = emailCheckBox.nextElementSibling;
                                emailCheckBox.disabled = false;
                                label.style.color = 'black';
                                label.style.cursor = '';
                            }
                        }

                        if (actions.classList.contains('hide')) {
                            actions.classList.remove('hide');
                        }
                        lastGroup = selected.parentNode.id;
                    });

                    $statusSelect.change((evt)=>{
                        activeApply();
                    })




                    addOptions($typeSelect, [
                        { value: '2', text: '>' },
                        { value: '1', text: '<' },
                        { value: '0', text: '=' },
                        { value: '3', text: '≠' },
                        { value: '5', text: '≥' },
                        { value: '4', text: '≤' },
                        { value: '6', text: 'Between' },
                    ])
                    $typeSelect.selectpicker('refresh');

                    addOptions($statusSelect, [
                        { value: '1', text: 'Online' },
                        { value: '0', text: 'Offline' },
                    ]);
                    $statusSelect.selectpicker();

                    var allTooltip = [];
                    function makeAndSaveTooltipContent(emailArray, permissionId) {
                        let tooltipContent = '';
                        emailArray.forEach((email, idx) => {
                            tooltipContent += `<div>${email}</div>`;
                            if (idx !== emailArray.length - 1) {
                                tooltipContent += '<br/>'
                            }
                        })
                        allTooltip.push({
                            permissionId: permissionId,
                            content: tooltipContent
                        });
                        return tooltipContent;
                    }

                    (function ($) {
                        // Default tooltip position.    
                        var ttpos = $.ui.tooltip.prototype.options.position;

                        $.widget("ui.tooltip", $.ui.tooltip, {
                            options: {
                                content: function () {
                                    return $(this).prop('title');
                                }
                            }
                        });

                        // Extend the autocomplete widget, using our own application namespace.
                        $.widget("app.autocomplete", $.ui.autocomplete, {
                            _create: function () {

                                this._super();
                                // After the menu has been created, apply the tooltip
                                // widget. The "items" option selects menu items with
                                // a title attribute, the position option moves the tooltip
                                // to the right of the autocomplete dropdown.
                                this.menu.element.tooltip({
                                    items: "li[title]",
                                    position: $.extend({}, ttpos, {
                                        my: "left+12",
                                        at: "right"
                                    })
                                });
                            },

                            // Clean up the tooltip widget when the autocomplete is
                            // destroyed.
                            _destroy: function () {
                                this.menu.element.tooltip("destroy");
                                this._super();
                            },

                            // The _renderItem() method is responsible for rendering each
                            // menu item in the autocomplete menu.
                            _renderItem: function (ul, item) {

                                // We want the rendered menu item generated by the default implementation.
                                var result = this._super(ul, item);
                                // If there is logo data, add our custom CSS class, and the specific
                                // logo URL.
                                if (item.logo) {
                                    result.find("div")
                                        .addClass("ui-menu-item-icon")
                                        .css("background-image", "url(" + item.logo + ")");
                                }

                                if (item.permissionId !== null) {
                                    let idx = findIndexInArray(allTooltip, 'permissionId', item.permissionId);
                                    if (idx === -1) {
                                        let apiHandler = API();
                                        let promise = apiHandler.GET(`EmployeeAPI/PermissionGroup/TooltipContent?permissionId=${item.permissionId}`);
                                        promise.done((response, data, xhr) => {
                                            if (xhr.status === 204) {
                                                result.attr("title", "&nbsp");
                                            } else if (xhr.status === 200) {
                                                result.attr('title', makeAndSaveTooltipContent(response, item.permissionId));
                                            }
                                        });
                                    } else {
                                        result.attr("title", allTooltip[idx].content);
                                    }
                                }
                                return result;
                            }
                        });
                    })(jQuery);

                    var $recipientTextarea = $(recipientTextarea);

                    function makeRecipientFragment(recipientArray) {
                        var fragment = document.createDocumentFragment();
                        function makeBase() {
                            var div = document.createElement('div');
                            var span = document.createElement('span');
                            var img = document.createElement('img');
                            img.style.maxHeight = '18px';
                            var textDiv = document.createElement('div');
                            textDiv.style.marginLeft = '5px'
                            // textDiv.textContent = ui.item.label;
                            var closeDiv = document.createElement('div');
                            closeDiv.classList.add('email-address-close');
                            closeDiv.innerHTML = '<i class="fa fa-times" aria-hidden="true"></i>';
                            return {
                                div: div,
                                span: span,
                                img: img,
                                textDiv: textDiv,
                                closeDiv: closeDiv
                            }
                        }
                        if (recipientArray.length === 0) {
                            return fragment;
                        }

                        recipientArray.forEach((item) => {
                            let base = makeBase();
                            base.textDiv.textContent = item.Label;
                            if (item.PermissionId) {
                                base.div.permissionId = item.PermissionId;
                                base.img.src = userGroupIcon;
                                base.span.appendChild(base.img);
                                base.textDiv.addEventListener('mouseover', showTootip);
                                base.textDiv.addEventListener('mouseout', removeTooltip);
                            } else if (item.EmployeeId) {
                                base.div.employeeId = item.EmployeeId;
                                base.img.src = userIcon;
                                base.span.appendChild(base.img);
                            } else if (item.ExternalRecipientId) {
                                base.div.externalRecipientId = item.ExternalRecipientId;
                            } else if (!isValidEmail(item.Label)) {
                                base.div.style.backgroundColor = '#d93025';
                                base.div.style.color = 'rgb(255,255,255)';
                            } else {
                                let apiHandler = API();
                                let pormise = apiHandler.GET(`EmployeeAPI/Search/ByEmail?email=${item.Label}`);

                                pormise.done((response, data, xhr) => {
                                    if (xhr.status === 204) {
                                        base.div.email = item.Label;
                                    } else if (xhr.status === 200) {
                                        if (!!response.Name) {
                                            base.textDiv.html = response.Name + ` &lt;${item.Label}&gt;`;
                                        }
                                        base.div.employeeId = response.Id;
                                        base.img.src = userIcon;
                                        base.span.insertAdjacentElement('afterbegin', base.img);
                                    }
                                });
                                pormise.fail(() => {
                                    base.div.email = item.Label;
                                    //base.div.innerHTML = `<span><div style="margin-left:5px">${email}</div><div class="email-address-close"><i class="fa fa-times" aria-hidden="true"></i></div></span>`;
                                });
                            }

                            base.span.appendChild(base.textDiv);
                            base.span.appendChild(base.closeDiv);
                            base.div.appendChild(base.span);
                            base.div.classList.add('email-address');
                            fragment.appendChild(base.div);
                        });
                        return fragment;
                    }

                    function showTootip(evt) {
                        var permissionId = evt.target.closest('.email-address').permissionId;
                        let idx = findIndexInArray(allTooltip, 'permissionId', permissionId);
                        let base = document.createElement('tooltip');
                        let tip = document.createElement('div');

                        if (idx === -1) {
                            let apiHandler = API();
                            let promise = apiHandler.GET(`EmployeeAPI/PermissionGroup/TooltipContent?permissionId=${permissionId}`);
                            promise.done((response, data, xhr) => {
                                if (xhr.status === 204) {
                                    tip.innerHTML = "&nbsp";
                                } else if (xhr.status === 200) {
                                    tip.innerHTML = makeAndSaveTooltipContent(response, permissionId)
                                }
                                base.appendChild(tip);
                                if (document.getElementsByTagName('tooltip')[0]) {
                                    document.getElementsByTagName('tooltip')[0].remove();
                                }
                                base.style.top = (evt.pageY + 20) + 'px';
                                base.style.left = (evt.pageX + 20) + 'px';
                                document.body.appendChild(base);
                            });
                        } else {
                            tip.innerHTML = allTooltip[idx].content;
                            base.appendChild(tip);
                            if (document.getElementsByTagName('tooltip')[0]) {
                                document.getElementsByTagName('tooltip')[0].remove();
                            }
                            base.style.top = (evt.pageY + 20) + 'px';
                            base.style.left = (evt.pageX + 20) + 'px';
                            document.body.appendChild(base);
                        }
                    }

                    function removeTooltip(evt) {
                        if (document.getElementsByTagName('tooltip')[0]) {
                            document.getElementsByTagName('tooltip')[0].remove();
                        }
                    }

                    $typeSelect.change((evt) => {
                        activeApply();
                        if (evt.target.value === BETWEEN_TYPE) {
                            //between
                            secondValueField.style.visibility = '';
                        } else {
                            secondValueField.style.visibility = 'hidden';
                        }
                    });

                    function textareaHandler(evt) {
                        $(document).ready(() => {
                            var code = (evt.keyCode ? evt.keyCode : evt.which);
                            var re = /(;|,)$/g;
                            var input = evt.target.value;
                            if (code == 32) {
                                // SPACE
                                let email = input.substring(0, input.length - 1).trim();
                                if (isValidEmail(email)) {
                                    let div = document.createElement('div');
                                    let apiHandler = API();
                                    let pormise = apiHandler.GET(`EmployeeAPI/Search/ByEmail?email=${email}`);

                                    pormise.done((response, data, xhr) => {
                                        if (xhr.status === 204) {
                                            div.email = email;
                                            div.innerHTML = `<span><div style="margin-left:5px">${email}</div><div class="email-address-close"><i class="fa fa-times" aria-hidden="true"></i></div></span>`;
                                        } else if (xhr.status === 200) {
                                            let label = (!!response.Name) ? response.Name + ` &lt;${email}&gt;` : email;
                                            div.employeeId = response.Id;
                                            div.innerHTML = `<span><img src="${userIcon}" style="max-height: 18px"/><div style="margin-left: 5px;">${label}</div><div class="email-address-close"><i class="fa fa-times" aria-hidden="true"></i></div></span>`;
                                        }
                                        div.classList.add('email-address');
                                        recipientTextarea.insertAdjacentElement('beforebegin', div);
                                        recipientTextarea.value = '';
                                        containerFluid.scrollTop = containerFluid.scrollHeight;
                                    });
                                    pormise.fail(() => {
                                        div.email = email;
                                        div.innerHTML = `<span><div style="margin-left:5px">${email}</div><div class="email-address-close"><i class="fa fa-times" aria-hidden="true"></i></div></span>`;
                                        div.classList.add('email-address');
                                        recipientTextarea.insertAdjacentElement('beforebegin', div);
                                        recipientTextarea.value = '';
                                        containerFluid.scrollTop = containerFluid.scrollHeight;
                                    });
                                }
                            } else if (code == 13 || re.test(input)) {
                                //ENTER
                                let email = input.substring(0, input.length - 1).trim();
                                recipientTextarea.value = '';
                                if (!email) {
                                    return;
                                }
                                recipientTextarea.parentNode.insertBefore(makeRecipientFragment([{
                                    Label: email
                                }]), recipientTextarea);;

                                if (evt.preventDefault)
                                    evt.preventDefault();

                                containerFluid.scrollTop = containerFluid.scrollHeight;
                            }
                        });
                    }

                    $recipientTextarea.on('keypress', textareaHandler);

                    $recipientTextarea.autocomplete({
                        source: function (request, response) {
                            var keyWord = request.term.trim();
                            if (!keyWord) {
                                return;
                            }
                            var apiHandler = API();
                            var promise = apiHandler.GET(`EmployeeAPI/FindEmail?searchString=${keyWord}`, null, null, false);
                            promise.done((data) => {
                                data && data.forEach((item) => {
                                    if (!!item.employeeId) {
                                        item.logo = userIcon;
                                    } else if (!!item.permissionId) {
                                        item.logo = userGroupIcon;
                                    }
                                })
                                response(data);
                            });
                        },
                        select: function (event, ui) {
                            var recipientObj = {
                                Label: ui.item.label,
                                PermissionId: ui.item.permissionId,
                                EmployeeId: ui.item.employeeId,
                                ExternalRecipientId: ui.item.externalRecipientId
                            };

                            recipientTextarea.parentNode.insertBefore(makeRecipientFragment([recipientObj]), recipientTextarea);;
                            containerFluid.scrollTop = containerFluid.scrollHeight;
                            activeApply();
                            $(this).val('');
                            return false;
                        },
                        change: function (event, ui) {
                            if (event.relatedTarget.classList.contains('cancel') || event.relatedTarget.classList.contains('close')) {
                                return;
                            }

                            $(document).ready(() => {
                                event.target.parentNode.insertBefore(makeRecipientFragment([{
                                    Label: event.target.value
                                }]), event.target);;
                                activeApply();
                                event.target.value = '';
                                containerFluid.scrollTop = containerFluid.scrollHeight;
                            });
                        },
                        messages: {
                            noResults: '',
                            results: function () { }
                        },
                        focus: function (event, ui) {
                            $(".ui-helper-hidden-accessible").empty(); /// pc
                        },
                        appendTo: '#email-recipient-field'
                    });

                    $recipientField.on('click', (evt) => {
                        if (evt.target.classList.contains('fa-times')) {
                            let div = evt.target.closest('.email-address');
                            div.parentNode.removeChild(div);
                            //cancel event 
                            if (!!div.permissionId) {
                                div.removeEventListener('mouseover', showTootip);
                                div.removeEventListener('mouseleave', removeTooltip);
                            }
                            activeApply();
                        }
                    });

                    function checkData(data) {
                        if (isEmpty(data.Setting.Name)) {
                            showAlertMsg(failAlert, 'Please give your rule a name first.', blankAlert);
                            return false;
                        }

                        if (isEmpty(data.Setting.DataId)) {
                            showAlertMsg(failAlert, 'Please select a source.', blankAlert);
                            return false;
                        } else if (data.Setting.DataId === DEVICE_STATUS_DATA || data.Setting.DataId === INNOAGE_STATUS_DATA) {
                            data.Setting.Value = $statusSelect.val();
                            data.Setting.Func = BOOLEAN_FUNCTION;
                            if (isEmpty(data.Setting.Value)) {
                                showAlertMsg(failAlert, 'Please select Online or Offline.', blankAlert);
                                return false;
                            }
                        } else {
                            if (isEmpty(data.Setting.Func)) {
                                showAlertMsg(failAlert, 'Please select one type.', blankAlert);
                                return false;
                            } else {
                                let valueFirst = document.getElementById('duration-value-1').value;

                                if (isEmpty(valueFirst)) {
                                    showAlertMsg(failAlert, 'Please fill in a number', blankAlert);
                                    return false;
                                } else if (isNaN(valueFirst)) {
                                    showAlertMsg(failAlert, 'Please only input numbers in the value field.', blankAlert);
                                    return false;
                                }
                                data.Setting.Value = valueFirst;
                                if (data.Setting.Func === BETWEEN_TYPE) {
                                    let valueSecond = secondValue.value;
                                    if (isEmpty(valueSecond)) {
                                        showAlertMsg(failAlert, 'Please fill in a number.', blankAlert);
                                        return false;
                                    } else if (isNaN(valueSecond)) {
                                        showAlertMsg(failAlert, 'Please only input numbers in the second value field.', blankAlert);
                                        return false;
                                    }
                                    data.Setting.Value = (parseInt(valueFirst) < parseInt(valueSecond)) ? `${valueFirst},${valueSecond}` : `${valueSecond},${valueFirst}`;
                                }
                            }
                        }

                        if (ratioBtn.value === '1') {
                            if (isEmpty($denominatorSelect.val())) {
                                showAlertMsg(failAlert, 'Please select a denominator.', blankAlert);
                                return false;
                            } else {
                                data.Setting.DenominatorId = ($denominatorSelect.val() == 0) ? null : $denominatorSelect.val();
                                data.Setting.Mode = RATIO_THRESHOLD_MODE;
                            }
                        } else {
                            data.Setting.Mode = NORAML_THRESHOLD_MODE;
                        }

                        var emailFlag = emailCheckBox.checked ? EMAIL_ACTION : 0;
                        var screenShot = 0;
                        data.Setting.Action = emailFlag | screenShot;
                        var errorFlag = false;
                        $recipientField.find('.email-address').each((idx, element) => {
                            if (!!element.permissionId) {
                                data.PermissionIdList.push(element.permissionId);
                            } else if (!!element.employeeId) {
                                data.EmployeeIdList.push(element.employeeId)
                            } else if (!!element.externalRecipientId) {
                                data.ExternalRecipientIdList.push(element.externalRecipientId);
                            } else if (!!element.email) {

                                data.ExternalRecipientList.push(element.email);
                            } else {
                                let email = element.closest('div').textContent;
                                if (isValidEmail(email)) {
                                    data.ExternalRecipientList.push(email);
                                } else {
                                    errorFlag = true;
                                    return false;
                                }
                            }
                        });

                        if (errorFlag) {
                            showAlertMsg(failAlert, 'There is invalid email address.', blankAlert);
                            return false;
                        }
                        return true;
                        // if (typeof callBack === 'function') {
                        //     callBack(data);
                        // }
                    }

                    function showAlertMsg(element, msg, blankAlert) {
                        element.textContent = msg
                        setTimeout(() => {
                            element.style.display = '';
                            blankAlert.style.display = '';
                        }, MESSAGE_DELAY_MILLISECOND);

                    }

                    function hideAlert() {
                        if (failAlert.style.display !== 'none') {
                            failAlert.style.display = 'none';
                            failAlert.textContent = ''
                            blankAlert.style.display = 'none';
                        }

                        if (successAlert.style.display !== 'none') {
                            successAlert.style.display = 'none';
                            successAlert.textContent = '';
                            blankAlert.style.display = 'none';
                        }
                    }

                    function create(payload, closeModal) {
                        let apiHandler = API();
                        let promise = apiHandler.POST(`SettingAPI/Threshold/Create`, payload);

                        promise.done(() => {
                            //showAlertMsg(successAlert, 'Create the threshold rule successfully!', blankAlert);
                            $ruleSelect.empty();
                            initSelectOption('SettingAPI/GetThresholdList', 'rulelist', 'rule-setting-modal', 'danger-rule-setting', true);
                            if (typeof closeModal === 'function') {
                                closeModal();
                            }
                        });

                        promise.fail((response) => {
                            if (403 === response.status) {
                                let errorCode = JSON.parse(response.responseText).ErrorCode;
                                if (errorCode === 0) {
                                    fire(document, 'logout');
                                } else if (errorCode === 1) {
                                    showAlertMsg(failAlert, 'Sorry, you do not have access to create threshold rule.', blankAlert)
                                }
                            } else if (409 === response.status) {
                                showAlertMsg(failAlert, 'Threshold Rule"s Name already exists.', blankAlert)
                            }
                        });
                    }

                    function save(payload, closeModal) {
                        var target = $ruleSelect.find(":selected");
                        payload.Setting.Id = target.attr('value');
                        var apiHandler = API();
                        var promise = apiHandler.PUT(`SettingAPI/Threshold/Update`, payload);
                        promise.done(() => {
                            target.text(payload.Setting.Name)
                            $ruleSelect.selectpicker('refresh');
                            $('#label-box-target').text(payload.Setting.Name + ` (${count.target})`);
                            // $('#rule-setting-modal').modal('hide');
                            $body.removeClass('loading');
                            if (typeof closeModal === 'function') {
                                closeModal();
                            } else {
                                showAlertMsg(successAlert, 'Apply successfully.', blankAlert);
                            }
                        });
                    }

                    var btnHandler = function (CreateOrSave, closeModal) {
                        hideAlert();
                        var payload = {
                            Setting: {
                                Name: namefield.value,
                                DataId: $dataSelect.val(),
                                DenominatorId: null,
                                Value: null,
                                Action: null,
                                Enable: true,
                                Func: ($dataSelect.val() === DEVICE_STATUS_DATA || $dataSelect.val() === INNOAGE_STATUS_DATA) ? 0 : $typeSelect.val(),
                                Mode: NORAML_THRESHOLD_MODE
                            },
                            PermissionIdList: [],
                            EmployeeIdList: [],
                            ExternalRecipientIdList: [],
                            ExternalRecipientList: [],
                        }
                        $(document).ready(() => {
                            checkData(payload) && CreateOrSave(payload, closeModal);
                        });
                    }

                    function getBtnFragment(setting, callBack) {
                        let fragment = document.createDocumentFragment();
                        let btn = document.createElement('button');
                        btn.classList.add('btn');
                        btn.classList.add('btn-mini');
                        btn.classList.add('btn-dark');
                        btn.id = setting.id;
                        // setting.disabled && btn.setAttribute('disabled');
                        btn.disabled = setting.disabled;
                        let span = document.createElement('span');
                        setting.classArray.forEach((ele) => {
                            span.classList.add(ele);
                        });
                        span.style.marginRight = '5px';
                        btn.appendChild(span);
                        span = document.createElement('span');
                        span.textContent = setting.name;
                        btn.appendChild(span);
                        callBack && btn.addEventListener('click', callBack);
                        fragment.appendChild(btn);
                        return fragment;
                    }

                    var nameFieldHandler = function (event) {
                        if (!footer.lastElementChild.disabled) {
                            namefield.removeEventListener('keydown', nameFieldHandler);
                            return false;
                        }

                        var allowedCode = [8, 32, 46];
                        var cursorMove = [35, 36, 37, 39];
                        //8: BackSpace; 32: space;  46: Delete;
                        var charCode = (event.charCode) ? event.charCode : ((event.keyCode) ? event.keyCode :
                            ((event.which) ? event.which : 0));

                        if (cursorMove.indexOf(charCode) != -1) {
                            return true;
                        } else if (charCode > 31 && (charCode < 64 || charCode > 90) &&
                            (charCode < 97 || charCode > 122) &&
                            (charCode < 48 || charCode > 57) &&
                            (allowedCode.indexOf(charCode) == -1)) {
                            event.preventDefault();
                            return false;
                        } else {
                            footer.lastElementChild.disabled = false;
                            footer.lastElementChild.addEventListener('click', btnHandler.bind(null, save));
                            namefield.removeEventListener('keydown', nameFieldHandler);
                        }
                    };

                    var valueFieldHandler = function (event) {
                        var charCode = (event.which) ? event.which : event.keyCode;
                        var allowedCode = [8, 32, 46, 110];
                        //8: BackSpace; 32: space;  46: Delete; 13: Enter
                        if (charCode == 13 || charCode > 31 && (charCode < 48 || charCode > 57) && !(charCode > 95 && charCode < 106) && (allowedCode.indexOf(charCode) == -1))
                            return false;
                        activeApply();
                        return true;
                    }

                    var checkBoxHandler = function () {
                        activeApply();
                        emailCheckBox.removeEventListener('click', checkBoxHandler);
                    }

                    if (threshold === undefined) {
                        footer.appendChild(getBtnFragment({
                            id: 'submit-rule',
                            name: 'Submit',
                            classArray: ['glyphicon', 'glyphicon-send'],
                            disabled: false
                        },
                            btnHandler.bind(null, create, function () {
                                $modal.modal('hide');
                            })));
                    } else {
                        title.textContent = 'Edit Rule';
                        namefield.addEventListener('keydown', nameFieldHandler);
                        namefield.value = threshold.Setting.Name;
                        $dataSelect.val(threshold.Setting.DataId);
                        $dataSelect.selectpicker('refresh');
                        if (threshold.Setting.Mode == RATIO_THRESHOLD_MODE) {
                            fire(ratioBtn, 'click');
                            $denominatorSelect.selectpicker('val', (threshold.Setting.DenominatorId === null) ? 0 : threshold.Setting.DenominatorId);
                        }

                        if (threshold.Setting.DataId == DEVICE_STATUS_DATA || threshold.Setting.DataId == INNOAGE_STATUS_DATA) {
                            typeValueField.classList.add('hide');
                            deviceStatusField.classList.remove('hide');
                            $statusSelect.val(threshold.Setting.Value);
                            $statusSelect.selectpicker('refresh');
                        } else {
                            deviceStatusField.classList.add('hide');
                            typeValueField.classList.remove('hide');
                            if (threshold.Setting.Func != BETWEEN_TYPE) {
                                firstValue.value = threshold.Setting.Value;
                            } else {
                                secondValueField.style.visibility = '';
                                let valueArray = threshold.Setting.Value.split(',');
                                firstValue.value = valueArray[0];
                                secondValue.value = valueArray[1];
                            }
                            $typeSelect.val(threshold.Setting.Func);
                            $typeSelect.selectpicker('refresh');
                        }
                        $denominatorSelect.change(() => {
                            activeApply();
                        });


                        for (var value of typeValueField.getElementsByTagName('input')) {
                            value.addEventListener('keydown', valueFieldHandler);
                        }

                        actions.classList.remove('hide');
                        emailCheckBox.checked = threshold.Setting.Action & EMAIL_ACTION;
                        let fragment = document.createDocumentFragment();

                        threshold.PermissionRecipientList && fragment.appendChild(makeRecipientFragment(threshold.PermissionRecipientList));
                        threshold.EmployeeRecipientList && fragment.appendChild(makeRecipientFragment(threshold.EmployeeRecipientList));
                        threshold.ExternalRecipientList && fragment.appendChild(makeRecipientFragment(threshold.ExternalRecipientList));
                        recipientTextarea.parentNode.insertBefore(fragment, recipientTextarea);

                        emailCheckBox.addEventListener('click', checkBoxHandler);
                        footer.appendChild(getBtnFragment({
                            id: 'apply-rule',
                            name: 'Apply',
                            classArray: ['glyphicon', 'glyphicon-pencil'],
                            disabled: true
                        }));

                        footer.insertBefore(getBtnFragment({
                            id: 'ok-rule',
                            name: 'Ok',
                            classArray: ['glyphicon', 'glyphicon-ok'],
                            disabled: false
                        },
                            btnHandler.bind(null, save, function () {
                                $modal.modal('hide');
                            })),
                            footer.firstChild
                        );
                    }

                    $modal.on('hidden.bs.modal', function () {
                        fire(normalBtn, 'click')
                        normalBtn.removeEventListener('click', normalMode);
                        ratioBtn.removeEventListener('click', ratioMode);

                        $dataSelect.off();
                        $dataSelect.empty();
                        $dataSelect.selectpicker('destroy');
                        $denominatorSelect.off();
                        $denominatorSelect.empty();
                        $denominatorSelect.selectpicker('destroy');

                        $typeSelect.off();
                        $typeSelect.empty();
                        $typeSelect.selectpicker('destroy');

                        $statusSelect.off();
                        $statusSelect.empty();
                        $statusSelect.selectpicker('destroy');
                        // emailCheckBox.removeEventListener('click', emailCheckBoxHandler);
                        $recipientField.off('click');
                        $recipientTextarea.off('keypress', textareaHandler);
                        namefield.value = '';
                        footer.lastElementChild.removeEventListener('click', btnHandler);
                        footer.lastElementChild.remove();

                        if (!!threshold) {
                            footer.firstElementChild.removeEventListener('click', btnHandler);
                            footer.firstElementChild.remove();
                            for (var value of typeValueField.getElementsByTagName('input')) {
                                value.removeEventListener('keydown', valueFieldHandler);
                            }
                        }

                        $recipientField.find('.email-address').each((idx, element) => {
                            if (!!element.permissionId) {
                                (element.getElementsByTagName('div')[0]).removeEventListener('mouseover', showTootip);
                                (element.getElementsByTagName('div')[0]).removeEventListener('mouseleave', removeTooltip);
                            }
                            element.remove();
                        });

                        $modal.off();
                        modalBackController.removeFade();
                    });

                    $modal.modal({
                        backdrop: false
                    });
                    containerFluid.scrollTop = 0;
                    $(document).ready(() => {
                        $modal.modal("show");
                        $body.removeClass('loading');
                    });
                }

                hideAlertById(['danger-rule-setting']);
                $body.addClass('loading');
                let promise = apiHandler.GET('SettingAPI/Threshold/DataSource');
                promise.done(init);
                promise.fail((response) => {
                    showErrorMessage(response, 'danger-rule-setting', 'blank-alert', 'Failed to get the dynamic data list. Please refresh the web page and try again.');
                    $body.removeClass('loading');
                });
            }

            function SaveSuccess(response) {
                showMyAlert("success-rule-setting", "Threshold rule setup successfully.", "blank-alert");
                $ruleSelect.trigger('change');
                $body.removeClass('loading');
            }

            function SaveFail(response) {
                if (403 == response.status || 409 == response.status) {
                    var messageObj = JSON.parse(response.responseText);
                    showMyAlert("danger-rule-setting", messageObj.Response, "blank-alert");
                } else {
                    showMyAlert("danger-rule-setting", "Failed to setup threshold rule.", "blank-alert");
                }
                $body.removeClass('loading');
            }

            function DeleteSuccess(response) {
                clearForm();
                $ruleSelect.empty();
                initSelectOption('SettingAPI/GetThresholdList', 'rulelist', null, 'danger-rule-setting', false);
                setTimeout(() => {
                    var $successAlert = $("#success-rule-setting");
                    var $blank = $('#blank-alert');

                    $blank.show();
                    $successAlert.html("The threshold rule deleted successfully.");
                    $successAlert.show();
                }, MESSAGE_DELAY_MILLISECOND);
            }

            function DeleteFail(response) {
                var $dangerAlert = $("#danger-rule-setting");
                var $blank = $('#blank-alert');

                if (403 == response.status) {
                    var messageObj = JSON.parse(response.responseText);
                    //1. "Sorry, you do not have access to delete the group."
                    //2. "You have been idle too long, please log-in again."
                    $dangerAlert.html(messageObj.Response);
                    setTimeout(() => {
                        $blank.show();
                        $dangerAlert.show();
                        $body.removeClass("loading");
                    }, MESSAGE_DELAY_MILLISECOND);
                } else {
                    $dangerAlert.html("Failed to delete the threshold rule. Please refresh the web page and try again.");
                    setTimeout(() => {
                        $blank.show();
                        $dangerAlert.show();
                        $body.removeClass("loading");
                    }, MESSAGE_DELAY_MILLISECOND);
                }
                $body.removeClass('loading');
            }

            var alertId = ['danger-rule-setting', 'success-rule-setting', 'blank-alert'];
            var payload = {
                Id: null,
                Name: null,
                Enable: false,
                Selected: null,
            };
            const count = {
                repo: 0,
                target: 0
            };
            var apiHandler = API();
            var $update = $('#update');
            var $save = $('#save');
            var $delete = $('#delete');
            var _TwoSelectForm = TwoSelectForm(payload, count);
            $ruleSelect.change((element) => {
                $body.addClass("loading");
                payload.Id = Number(element.target.value);
                clearForm();
                let promise = apiHandler.GET('SettingAPI/Threshold/Group?id=' + payload.Id, false, false);
                payload.Name = $ruleSelect.find('option:selected').text();
                promise.done((response) => { initEditForm(response); });
                promise.fail(EditAPIError);
            });

            $create.click(function () {
                openRuleSettingModal();
            });

            $update.click(function () {
                hideAlertById(alertId);

                var target = $ruleSelect.val();
                if (isEmpty(target)) {
                    let $dangerAlert = $('#danger-rule-setting');
                    let $blank = $('#blank-alert');

                    $dangerAlert.html('Please select one threshold rule.');
                    setTimeout(() => {
                        $blank.show();
                        $dangerAlert.show();
                    }, MESSAGE_DELAY_MILLISECOND);
                    return;
                }

                var apiHandler = API();
                var promise = apiHandler.GET(`SettingAPI/Threshold/Setting/Find?thresholdId=${$ruleSelect.val()}`);
                promise.done((response, data, xhr) => {
                    if (xhr.status === 204) {
                        alert('The threshold setting could not be found.');
                    } else {
                        openRuleSettingModal(response)
                    }
                });
            });

            $save.click(() => {
                hideAlertById(alertId);
                if (Boolean($ruleSelect.val()) === true) {
                    let $enableFlag = $('#enable-flag');

                    $body.addClass('loading');
                    payload.Enable = $enableFlag.prop('checked');
                    if (payload.Selected !== null && payload.Selected.length === 0) {
                        payload.Selected = null;
                    }
                    let promise = apiHandler.PUT('SettingAPI/Threshold/Save', payload);
                    promise.done(SaveSuccess);
                    promise.fail(SaveFail);
                } else {
                    setTimeout(() => {
                        var $dangerAlert = $('#danger-rule-setting');
                        var $blank = $('#blank-alert');

                        $dangerAlert.html('Please select one threshold rule.');
                        $blank.show();
                        $dangerAlert.show();
                    }, MESSAGE_DELAY_MILLISECOND);
                }
            });

            $delete.click(() => {
                hideAlertById(alertId);
                if (Boolean($ruleSelect.val()) === true) {
                    if (true === confirm("The threshold rule " + `"${$("#rulelist :selected").text()}"` + " will be deleted, are you sure?")) {
                        $body.addClass('loading');
                        let promise = apiHandler.DELETE('SettingAPI/Threshold/Delete', 'id', $ruleSelect.val());
                        promise.done(DeleteSuccess);
                        promise.fail(DeleteFail);
                    }
                } else {
                    setTimeout(() => {
                        var $dangerAlert = $('#danger-rule-setting');
                        var $blank = $('#blank-alert');

                        $dangerAlert.html('Please select one threshold rule.');
                        $blank.show();
                        $dangerAlert.show();
                    }, MESSAGE_DELAY_MILLISECOND);
                }
            });
        }

        var wdth = $(window).width();
        var $body = $("body");
        var $ruleSelect = $('#rulelist');
        var $create = $('#create');

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
        $ruleSelect.selectpicker();
        initSelectOption('SettingAPI/GetThresholdList', 'rulelist', 'rule-setting-modal', 'danger-rule-setting', false);
        listenRuleSetting();
    }
    $(document).on("reload-setting-threshold", reloadSettingThreshold);
})();