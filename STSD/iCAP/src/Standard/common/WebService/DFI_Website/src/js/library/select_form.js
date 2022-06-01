import { fire } from "./event_handler";

export function addOption($select, options) {
    options.forEach((option) => {
        $select.append($('<option></option>').attr('value', option.Id).text(option.Name))
    });
};

export function addGroup($select, option) {
    $select.append($('<optgroup></optgroup>').attr('value', option.Id).attr('label', option.Name));
}

export function addGroupWithOpion($select, group, options) {
    var optgroup = document.createElement('optgroup');
    optgroup.label = group.Name;
    optgroup.id = group.Id;

    options.forEach((item) => {
        var option = document.createElement('option');
        option.value = item.Id;
        option.textContent = item.Name;

        item.Unit && option.setAttribute('data-unit', item.Unit);
        optgroup.appendChild(option);
    });
    $select.append(optgroup);
}


export function addSelectTitle(id, title) {
    var selectForm = document.getElementById(id);
    var titleSelectForm = document.createElement("option");
    titleSelectForm.textContent = title;
    selectForm.appendChild(titleSelectForm);
}

export function addSelectGroup(parentNodeId, id, label) {
    var selectForm = document.getElementById(parentNodeId);
    var group = document.createElement("optgroup");
    group.label = label;
    group.id = id;
    selectForm.appendChild(group);
}

export function addOptionInSelect(selector, options) {
    options.forEach((element) => {
        var option = document.createElement("option");
        option.value = element.Id;
        option.textContent = element.Name;
        selector.appendChild(option);
    });
}

export function addOptions($select, options) {
    options.forEach((option) => {
        $select.append($('<option></option>').attr('value', option.value).text(option.text))
    });
}

export function addSelectOption(id, value, textContent) {
    var parentNode = document.getElementById(id);
    var option = document.createElement("option");
    option.value = value;
    option.textContent = textContent;
    parentNode.appendChild(option);
}

export function addOptionBySelectId(id, options) {
    var select = document.getElementById(id);
    options.forEach((element) => {
        var option = document.createElement("option");
        option.value = element.Id;
        option.textContent = element.Name;
        select.appendChild(option);
    });
}

export function addSelectOptionWithAttribute(id, value, textContent, attName, attValue) {
    var parentNode = document.getElementById(id);
    var option = document.createElement("option");
    option.value = value;
    option.textContent = textContent;
    option.setAttribute(attName, attValue);
    parentNode.appendChild(option);
}
// export function makeSelect(parentId, optionData)
// {
//     $("#"+parentId).empty();
//     var widgetSelectForm = document.getElementById(parentId);
//     optionData.forEach && optionData.forEach(element => {
//         var widgetOption = document.createElement("option");
//         widgetOption.value=element.WidgetId;
//         widgetOption.textContent=element.WidgetName;
//         widgetSelectForm.appendChild(widgetOption);
//     });
//     $('#'+parentId).selectpicker();
// }

export function makeSelect(parentId, groupData, optionData) {
    //$("#"+parentId).empty();
    if (groupData) {
        groupData.forEach(element => {
            addSelectGroup(parentId, element.groupId, element.groupLabel);
            optionData.forEach(option => {
                addSelectOption(element.groupId, option.optionValue, option.optionName);
            });
        });
    }
    else {
        var selectForm = document.getElementById(parentId);
        optionData.forEach && optionData.forEach(element => {
            var option = document.createElement("option");
            option.value = element.Id;
            option.textContent = element.Name;
            selectForm.appendChild(option);
        });
    }
    $('#' + parentId).selectpicker('refresh');
}

export function makeSelectWidthAlias(parentId, optionData) {
    var selectForm = document.getElementById(parentId);
    optionData.forEach && optionData.forEach(element => {
        var option = document.createElement("option");
        option.value = element.Id;

        if (element.Alias) {
            option.textContent = element.Alias;
        }
        else {
            option.textContent = element.Name;
        }
        selectForm.appendChild(option);
    });
    $('#' + parentId).selectpicker('refresh');
}


export function initSelectpicker(id) {
    $('#' + id).attr('title', '--Please Select One--');
    $('#' + id).selectpicker('refresh');
}

export function disableSelectGroup(parentId, groupid) {
    $('#' + parentId + ' ' + 'optgroup#' + groupid).prop('disabled', true);
    $('#' + parentId).selectpicker('refresh');
}

export function enableSelectGroup(parentId, groupId) {
    $('#' + parentId + ' ' + 'optgroup#' + groupId).prop('disabled', false);
    $('#' + parentId).selectpicker('refresh');
}

export function disableSelectOption(parentId, value) {
    $(`#${parentId} option[value = ${value}]`).attr('disabled', true);
}

export function enableSelectOption(parentId, value) {
    $(`#${parentId} option[value = ${value}]`).attr('disabled', false);
}


export function getSelectedOptgroupId(parentId) {
    return $('#' + parentId + ' :selected').parent().attr('id');
}

export function revertToDefault(id) {
    $('#' + id).val('default');
    $('#' + id).selectpicker('refresh');
}

export function selectSearch(notation, select, options, input) {

    // const select = document.querySelectorAll('select');
    // const options = Array.from(select[1].options);
    // const input = document.querySelector('input');
    var pattern = new RegExp("[`~!@#$^&*()=|{}':;',\\[\\].<>/?~！@#￥……&*（）——|{}【】‘；：”“'。，、？+]");

    function isRepeat(arr) {

        var hash = {};

        for (var i in arr) {

            if (hash[arr[i]])

                return true;

            hash[arr[i]] = true;

        }

        return false;

    }


    function findMatches(search, options) {
        var rs = "";
        for (var i = 0; i < search.length; i++) {
            rs = rs + search.substr(i, 1).replace(pattern, '\\' + `${search.substr(i, 1)}`);
        }
        return options.filter(option => {
            const regex = new RegExp(rs, 'gi');
            return option.text.match(regex);
        });
    }
    function filterOptions() {
        options.forEach(option => {
            option.selected = false;
            //option.remove();
        });
        $(select).empty();

        const matchArray = findMatches(this.value, options);
        matchArray.sort(function (a, b) {
            var nameA = a.textContent.toUpperCase();
            var nameB = b.textContent.toUpperCase();
            if (nameA < nameB) {
                return -1;
            }
            if (nameA > nameB) {
                return 1;
            }

            return 0;
        });
        select.append(...matchArray);
        $(notation).text(`Find ${matchArray.length} item(s)`);
    }
    input.addEventListener('change', filterOptions);
    input.addEventListener('keyup', filterOptions);
}

export function makeDeviceSelect(id, options) {
    const select = document.getElementById(id);
    options.forEach((element) => {
        const option = document.createElement('option');
        option.value = element.Id;
        if (element.Alias) {
            option.textContent = element.Alias;
        } else {
            option.textContent = element.Name;
        }
        select.appendChild(option);
    });
}

export function sortOptions(id) {
    $("#" + id).html($("#" + id + " option").sort(function (a, b) {
        var nameA = a.text.toUpperCase();
        var nameB = b.text.toUpperCase();
        return nameA == nameB ? 0 : nameA < nameB ? -1 : 1;
    }));
}

export function listenBtn(label, optionsRepo, optionsTarget, payload, count) {
    function removeOption(options, rmIndex) {
        var count = 0;
        rmIndex.forEach(element => {
            element = element - count;
            options.splice(element, 1);
            count++;
        });
    }

    function sortNumber(a, b) {
        return a - b;
    }

    function findMatches(search, option) {
        var rs = "";
        const pattern = new RegExp("[`~!@#$^&*()=|{}':;',\\[\\].<>/?~！@#￥……&*（）——|{}【】‘；：”“'。，、？+]");
        for (var i = 0; i < search.length; i++) {
            rs = rs + search.substr(i, 1).replace(pattern, '\\' + `${search.substr(i, 1)}`);
        }
        const regex = new RegExp(rs, 'gi');
        return option.match(regex);
    }

    var copyOption = $.extend(true, [], optionsTarget);

    $('#btnRight').click(function (e) {
        var selectedOpts = $('#box-repo option:selected');

        if (selectedOpts.length == 0) {
            alert("Nothing to move.");
            e.preventDefault();
        } else {
            var optionsIdRepo = $.map(optionsRepo, function (item, index) {
                return item.value;
            });
            const searchTarget = $('#search-box-target').val();
            var index = -1;
            var rmIndex = [];
            var matchTag = false;
            selectedOpts.each((idx, option) => {
                index = optionsIdRepo.indexOf(option.value);
                rmIndex.push(index);
                var newItem = $.map(copyOption, function (item, index) {
                    return item.value;
                }).indexOf(option.value);
                if (newItem != -1) {
                    $(option).css('font-weight', '');
                } else {
                    $(option).css('font-weight', 'bold');
                }
                optionsTarget.push(option);
                matchTag = findMatches(searchTarget, $(option).text());
                if (matchTag) {
                    $('#box-target').append($(option).clone());
                }
            });
            rmIndex.sort(sortNumber);
            removeOption(optionsRepo, rmIndex);
            $('#label-box-repo').text(label.repo + ` (${optionsRepo.length})`);
            $('#label-box-target').text(payload.Name + ` (${optionsTarget.length})`);
            $(selectedOpts).remove();
            sortOptions('box-target');
            sortOptions('box-repo');
            count.repo = $('#box-repo option').length;
            count.target = $('#box-target option').length;
            $('#find-count-repo').text(`Find ${count.repo} item(s)`);
            $('#find-count-target').text(`Find ${count.target} item(s)`);
            payload.Selected = $.map(optionsTarget, function (item, index) {
                return { Id: Number(item.value) };
            });
            e.preventDefault();
        }
    });

    $('#btnAllRight').click(function (e) {
        var selectedOpts = $('#box-repo option');
        if (selectedOpts.length == 0) {
            alert("Nothing to move.");
            e.preventDefault();
        } else {
            var optionsIdRepo = $.map(optionsRepo, function (item, index) {
                return item.value;
            });
            var searchTarget = $('#search-box-target').val();
            var index = -1;
            var rmIndex = [];
            var matchTag = false;
            selectedOpts.each((idx, option) => {
                index = optionsIdRepo.indexOf(option.value);
                rmIndex.push(index);
                var newItem = $.map(copyOption, function (item, index) {
                    return item.value;
                }).indexOf(option.value);
                if (newItem != -1) {
                    $(option).css('font-weight', '');
                } else {
                    $(option).css('font-weight', 'bold');
                }
                optionsTarget.push(option);
                matchTag = findMatches(searchTarget, $(option).text());
                if (matchTag) {
                    $('#box-target').append($(option).clone());
                }
            });
            rmIndex.sort(sortNumber);
            removeOption(optionsRepo, rmIndex);
            $('#label-box-repo').text(label.repo + ` (${optionsRepo.length})`);
            $('#label-box-target').text(payload.Name + ` (${optionsTarget.length})`);
            //$('#box-target').append($(selectedOpts).clone());
            $(selectedOpts).remove();
            sortOptions('box-target');
            sortOptions('box-repo');
            count.repo = $('#box-repo option').length;
            count.target = $('#box-target option').length;
            $('#find-count-repo').text(`Find ${count.repo} item(s)`);
            $('#find-count-target').text(`Find ${count.target} item(s)`);
            payload.Selected = $.map(optionsTarget, function (item, index) {
                return { Id: Number(item.value) };
            });
            e.preventDefault();
        }
    });

    $('#btnLeft').click(function (e) {
        var selectedOpts = $('#box-target option:selected');
        if (selectedOpts.length == 0) {
            alert("Nothing to move.");
            e.preventDefault();
        } else {
            var optionsIdTarget = $.map(optionsTarget, function (item, index) {
                return item.value;
            });
            var searchRepo = $('#search-box-repo').val();
            var index = -1;
            var rmIndex = [];
            var matchTag = false;
            selectedOpts.each((idx, option) => {
                index = optionsIdTarget.indexOf(option.value);
                rmIndex.push(index);
                $(option).css('font-weight', '');
                optionsRepo.push(option);
                matchTag = findMatches(searchRepo, $(option).text());
                if (matchTag) {
                    $('#box-repo').append($(option).clone());
                }
            });
            rmIndex.sort(sortNumber);
            removeOption(optionsTarget, rmIndex);
            $('#label-box-repo').text(label.repo + ` (${optionsRepo.length})`);
            $('#label-box-target').text(payload.Name + ` (${optionsTarget.length})`);
            $(selectedOpts).remove();
            sortOptions('box-target');
            sortOptions('box-repo');
            count.repo = $('#box-repo option').length;
            count.target = $('#box-target option').length;
            $('#find-count-repo').text(`Find ${count.repo} item(s)`);
            $('#find-count-target').text(`Find ${count.target} item(s)`);
            payload.Selected = $.map(optionsTarget, function (item, index) {
                return { Id: Number(item.value) };
            });
            e.preventDefault();
        }
    });

    $('#btnAllLeft').click(function (e) {
        var selectedOpts = $('#box-target option');
        if (selectedOpts.length == 0) {
            alert("Nothing to move.");
            e.preventDefault();
        } else {
            var optionsIdTarget = $.map(optionsTarget, function (item, index) {
                return item.value;
            });
            var searchRepo = $('#search-box-repo').val();
            var index = -1;
            var rmIndex = [];
            var matchTag = false;
            selectedOpts.each((idx, option) => {
                index = optionsIdTarget.indexOf(option.value);
                rmIndex.push(index);
                $(option).css('font-weight', '');
                optionsRepo.push(option);
                matchTag = findMatches(searchRepo, $(option).text());
                if (matchTag) {
                    $('#box-repo').append($(option).clone());
                }
            });
            rmIndex.sort(sortNumber);
            removeOption(optionsTarget, rmIndex);
            $('#label-box-repo').text(label.repo + ` (${optionsRepo.length})`);
            $('#label-box-target').text(payload.Name + ` (${optionsTarget.length})`);
            //$('#box-repo').append($(selectedOpts).clone());
            $(selectedOpts).remove();
            sortOptions('box-target');
            sortOptions('box-repo');
            count.repo = $('#box-repo option').length;
            count.target = $('#box-target option').length;
            $('#find-count-repo').text(`Find ${count.repo} item(s)`);
            $('#find-count-target').text(`Find ${count.target} item(s)`);
            payload.Selected = $.map(optionsTarget, function (item, index) {
                return { Id: Number(item.value) };
            });
            e.preventDefault();
        }
    });
}

export function TwoSelectForm(payload, count) {
    const label = {
        repo: 'Group Repository',
    };
    const select = document.querySelectorAll('select');
    const input = document.querySelectorAll('input');
    var notation = document.querySelectorAll('font');

    function init(Unselected, Selected) {
        if (null != Unselected) {
            count.repo = Unselected.length;
            makeDeviceSelect('box-repo', Unselected);
        } else {
            count.repo = 0;
        }

        if (null != Selected) {
            count.target = Selected.length;
            makeDeviceSelect('box-target', Selected);
            payload.Selected = Selected;
        } else {
            count.target = 0;
            payload.Selected = [];
        }

        sortOptions('box-target');
        sortOptions('box-repo');

        const optionsRepo = Array.from(select[1].options);
        const optionsTarget = Array.from(select[2].options);

        $('#find-count-repo').text(`Find ${count.repo} item(s)`);
        $('#find-count-target').text(`Find ${count.target} item(s)`);

        $('#label-box-target').text(payload.Name + ` (${count.target})`);
        $('#label-box-repo').text(label.repo + ` (${count.repo})`);

        selectSearch(notation[0], select[1], optionsRepo, input[0]);
        selectSearch(notation[1], select[2], optionsTarget, input[5]);
        listenBtn(label, optionsRepo, optionsTarget, payload, count);
    }

    var publicAPI = {
        init: init
    }

    return publicAPI;
}

export function initDataSelect(dataSelect, parendId) {
    var $select = $('#' + parendId);

    dataSelect.forEach(element => {
        addSelectGroup(parendId, element.GroupOption.Id, element.GroupOption.Name);
        element.DataOption.forEach(option => {
            addSelectOptionWithAttribute(element.GroupOption.Id, option.Id, option.Name, 'data-unit', option.Unit);
        });
    });
    $select.selectpicker('refresh');
    //$(`#${modalId} .dropdown-menu`).css('width', 'inherit');
}

export function toOption(select, target) {
    if (select.options.length === 1) {
        return;
    }
    const currentOption = select.value;
    let idx = select.selectedIndex;
    switch (target) {
        case 'forward':
            const firstOption = select.options[0].value;
            select.options[idx].selected = false;
            if (currentOption === firstOption) {
            select.options[select.options.length - 1].selected = true;
            } else {
            select.options[--idx].selected = true;
            }
            break;
        case 'next':
            const lastOption = select.options[select.options.length - 1].value;
            select.options[idx].selected = false;
            if (currentOption === lastOption) {
            select.options[0].selected = true;
            } else {
            select.options[++idx].selected = true;
            }
            break;
    }
    fire(select, 'change');
}

// var currentOption = select.value;
// switch (target) {
//     case 'forward':
//         let firstOption = select.options[0].value;
//         if (currentOption === firstOption) {
//             alert("It's first one.");
//         } else {
//             let idx = select.selectedIndex;
//             select.options[idx].selected = false;
//             select.options[--idx].selected = true;
//             fire(select, 'change');
//         }
//         break;
//     case 'next':
//         let lastOption = select.options[select.options.length - 1].value;
//         if (currentOption === lastOption) {
//             alert("It's last one.");
//         } else {
//             let idx = select.selectedIndex;
//             select.options[idx].selected = false;
//             select.options[++idx].selected = true;
//             fire(select, 'change');
//         }
//         break;
// }


export function initDenominatorSelect(selectId, dataOption, dataGroupId, addNone) {
    var index = $.map(dataOption, function (item, index) {
        return item.GroupOption.Id;
    }).indexOf(Number(dataGroupId));
    var $denominatorSelect = $(`#${selectId}`);

    if (addNone) {
        addSelectOption(selectId, 0, 'None');
    }

    addSelectGroup(selectId, dataGroupId + '-de', dataOption[index].GroupOption.Name);
    dataOption[index].DataOption.forEach(element => {
        addSelectOption(dataGroupId + '-de', element.Id, element.Name);
    });
    $denominatorSelect.selectpicker('val', 0);
    $denominatorSelect.selectpicker('refresh');
}

