require("../components/card");
require("../components/tab");
import branch_page from "../../html/pages/branch.html";
import { DEVICE_STATE } from '../library/data_define';
import { fire } from '../library/event_handler';
import { OOBDirector } from '../library/OOB';
import { NXReboot } from '../library/NXReboot';
import { DeviceManager } from '../DeviceManager';

$(document).on("reload-branch", function () {
    $(document).trigger("clean_all_tab");
    var store_token = $.cookie('token');
    var secondary_tab = document.getElementsByClassName("tab-secondary");
    secondary_tab[0].style.display = "none";

    $.ajax({
        type: 'GET',
        url: 'BranchAPI/GetList',
        async: true,
        crossDomain: true,
        headers: {
            'token': store_token
        },
        success: function (response) {
            var parsed_data = JSON.parse(response);
            $.cookie('current_group', parsed_data.List[0].Id);
            const navTabWrapper = document.createElement('div');
            navTabWrapper.classList.add('branch-navbar-wrapper');
            var tab_container = document.getElementById("tab-primary-continer");

            function RenderBranchPage(branchId, branchName) {
                $("body").addClass('loading');
            
                var store_token = $.cookie('token');
            
                $.cookie('current_group', branchId);
            
                const apiGetGroupInfo = $.ajax({
                    type: 'GET',
                    url: `/BranchAPI/Branch/${branchId}`,
                    async: true,
                    crossDomain: true,
                    headers: {
                      token: store_token,
                    },
                    processData: false,
                  });
                apiGetGroupInfo.done((response)=>{
                if (response != undefined) {
                    let child_page = document.getElementById("child-page");
                    //if branch page do not exist, hang the page again. And re bind the event.
        
                    child_page.innerHTML = branch_page;

                    const oobDirector = new OOBDirector();
                    const rebootDirector = new NXReboot();

                    let title_text = document.getElementById("branch-title");
                    if (title_text) {
                        title_text.textContent = branchName;
                    }
                    let switch_btn = document.getElementById("viewmore-btn");
        
                    function SwitchToDevice() {
                        $.cookie('temp_groupId', $.cookie('current_group'));
                        $(document).trigger("switch-to-device", $(this).attr("value"));        
                    }
                    $(switch_btn).bind("click", SwitchToDevice);

                    const reData = Object.assign({}, response);

                    function RenderOverview({Name}) {
                        var doc = document;
                        $.cookie("current_deviceId", Name);
                        
                        var store_token = $.cookie('token');
                        var switch_btn = doc.getElementById("viewmore-btn");
                        
                        switch_btn.setAttribute("value", Name);
                        
                        $.ajax({
                            type: 'GET',
                            url: 'DeviceInfoAPI/GetOverview?DeviceName=' + Name,
                            async: true,
                            crossDomain: true,
                            headers: {
                                'token': store_token
                            },
                            success: function (response) {
                                if (response != undefined) {
                                    var parsed_data = JSON.parse(response);

                                    const device = DeviceManager.getDevice(Name);
                                    device.update(parsed_data);

                                    oobDirector.setUp({
                                        ooblist: device.getOOBlist(),
                                        devName: device.getName()
                                    });
                                    
                                    rebootDirector.setUp({
                                        devName: device.getName()
                                    });

                                    const items = doc.querySelectorAll('td[id^=overview]');
                                    const eleDev = doc.getElementById("devName");
                                    items[0].textContent = parsed_data.OS;
                                    items[1].textContent = parsed_data.CPU;
                                    items[2].textContent = parsed_data.MEMCap + "GB";
                                    items[3].textContent = parsed_data.StorCap + "GB";
                                    items[4].textContent = parsed_data.Status;
                        
                                    if (parsed_data.Alias != null && Name != '') {
                                        eleDev.textContent = parsed_data.Alias;
                                    } else {
                                        eleDev.textContent = parsed_data.DevName;
                                    }
                        
                                }
                            },
                            error: function (response) {
                            }
                        });
                        
                        $.ajax({
                            type: 'GET',
                            url: 'DeviceAPI/GetImg',
                            async: true,
                            crossDomain: true,
                            headers: {
                                'token': store_token,
                                "devName": Name
                            },
                            success: function (response) {
                                var ret = JSON.parse(response);
                                $("#devimg").attr('src', ret.Response);
                            },
                            error: function (response) {
                            }
                        });
                    }
        
                    function RenderDeviceList(list, state) {
                        $('#device-list').empty();
                        var devlist = document.getElementById("device-list");
                        
                        function RenderOverviewHandler() {
                            RenderOverview({Name: $(this).attr("value")});
                        }
                        list.forEach((element, idx) => {
                            var root = document.createElement("a");
                            var li = document.createElement("li");
                            li.setAttribute("class", "list-group-item col-md-6");
                            var span = document.createElement("span");
                            var devState = state[idx];
                            if (DEVICE_STATE.WARNING == devState) {
                                span.setAttribute("class", "fa fa-exclamation-triangle");
                            }
                            else if (DEVICE_STATE.OFFLINE == devState) {
                                span.setAttribute("class", "led-none");
                            }
                            else {
                                span.setAttribute("class", "led-success");
                            }
                            var devname = document.createElement("b");
                            devname.setAttribute("class", "fontMW2");
                        
                            if (element.Alias != null && element.Alias != '') {
                                devname.textContent = element.Alias;
                            }
                            else {
                                devname.textContent = element.Name;
                            }
                        
                            li.appendChild(span);
                            li.appendChild(devname);
                            root.appendChild(li);
                            root.setAttribute("value", element.Name);
                            $(root).bind("click", RenderOverviewHandler);
                            devlist.appendChild(root);
                        });
                    }
        
                    $('#pagination-container').pagination({
                        dataSource: reData.Devicelist,
                        pageSize: 10,
                        showGoInput: true,
                        showGoButton: true,
                        showPageNumbers: false,
                        showNavigator: true,
                        pageRange: 0,
                        formatGoInput: '<%= input %>',
                        callback: function (data, pagination) {
                            const getDevceState = $.ajax({
                                type: 'GET',
                                url: `/StatusAPI/Devices?query=${data.map(x => x.Name).toString()}`,
                                async: true,
                                crossDomain: true,
                                headers: {
                                token: $.cookie('token'),
                                },
                                processData: false,
                            });
                            getDevceState.done((response)=>{ 
                                RenderDeviceList(data, response.Response)
                                RenderOverview(data[0]);
                                // $("body").removeClass('loading');
                            });
                        }
                    });
                    function renderLoading(data) {
                        var item = {
                            id: 1,
                            name: 'Device loading',
                            type: 'scatter',
                            width: 'col-md-12 col-sm-12 col-xs-12 pad0',
                            label: ['CPU Loading (%)', 'Memory Loading (%)'],
                            data: data
                        };
                        let p = $Card(item);
                        p.put('#devLoading');
                        p.render();
                    }
                    reData.Grouploading && renderLoading(reData.Grouploading);
                } else {
                    $("body").removeClass('loading');
                    let child_page = document.getElementById("child-page");
                    //if branch page do not exist, hang the page again. And re bind the event.
        
                    child_page.innerHTML = branch_page;
                    let title_text = document.getElementById("branch-title");
                    if (title_text) {
                        title_text.textContent = branchName;
                    }
                }
                });
                apiGetGroupInfo.fail((response)=>{
                if (response.status === 403 && response.responseJSON.Response === RETURN_CODE.TOKEN_ERROR) {
                    fire(document, 'logout');
                }
                });
            }
            
            function ChangeBranch() {
                $('#child-page').empty();
                $('#tab-primary-continer .branch-navbar-wrapper > a ').each((idx, tab) => {
                    tab.classList.remove('branch-tab-click');
                });
                this.classList.add('branch-tab-click');
                RenderBranchPage($(this).attr("value"), $(this)[0].textContent);
            }
            parsed_data.List.forEach((element, index) => {
                var tb1 = $Tab({name: element.Name}).getTab();
                tb1.setAttribute("value", element.Id);
                tb1.addEventListener("click", ChangeBranch);
                if (index === 0) {
                    tb1.classList.add('branch-tab-click');
                }
                navTabWrapper.appendChild(tb1);
                navTabWrapper.appendChild($Tab().getDivider());
            });
            tab_container.appendChild(navTabWrapper);
            if (!$.cookie('temp_groupId')) {
                RenderBranchPage(parsed_data.List[0].Id, parsed_data.List[0].Name);
            } else {
                var index = $.map(parsed_data.List, function (item, index) {
                    return item.Id;
                }).indexOf(Number($.cookie('temp_groupId')));
                RenderBranchPage(parsed_data.List[index].Id, parsed_data.List[index].Name);
                $.cookie('temp_groupId', "");
            }
        },
        error: function (response) {
        }
    });
});
