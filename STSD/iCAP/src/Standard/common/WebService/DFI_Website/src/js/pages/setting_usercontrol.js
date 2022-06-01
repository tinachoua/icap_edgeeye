import { isValidEmail, isEmpty } from "../library/data_verification";

$(document).on("reload-setting-usercontrol", function () {
    if ($.cookie("current_page") !== "setting-user") {
        return;
    }
    $("#user-list").empty();
    $("#update-success").hide();
    $("#delete-success").hide();
    $("#fail").hide();
    var user_select = document.getElementById("user-list");
    user_select.onchange = function () {
        $(document).trigger("reload-usercontrol-setting", this.value);
    }
    $.ajax({
        type: 'GET',
        url: 'EmployeeAPI/List',
        async: true,
        crossDomain: true,
        headers: {
            'token': $.cookie('token')
        },
        success: function (response) {
            var parsed_data = JSON.parse(response);
            parsed_data.EmployeeList.forEach(element => {
                var user_option = document.createElement("option");
                user_option.value = element;
                user_option.textContent = element;
                user_select.appendChild(user_option);
            });
            $(document).trigger("reload-usercontrol-setting", parsed_data.EmployeeList[0]);
        },
        error: function (response) {

        }
    });
});

$(document).on('click', '#userctl-update', userctl_update);
$(document).on('click', '#userctl-delete', userctl_delete);

$(document).on("reload-usercontrol-setting", function (event, userName) {
    // function showLineModal(callBackURL)
    // {
    //     function initLineModal()
    //     {
    //         $('#line-modal').on('hidden.bs.modal',function() {
    //             //hideAlertByClassName(['alert-rename']);
    //             var unlistenId = ['send-message', 'line-modal'];
    //             unlisten(unlistenId);    
    //             document.getElementById("message").value = "";     
    //         });
    //     }

    //     function testLine( )
    //     {
    //         $('#success-line').hide();

    //         function sendMessage(response)
    //         {
    //             function toDoWhenSendMessageSuccess(response)
    //             {
    //                 $('#success-line').html('Send test message successfully.');
    //                 setTimeout(()=>{
    //                     $('#success-line').show();
    //                     $('#blank-alert-line').show();
    //                 }, 100);
    //             }

    //             if (response) {
    //                 var parsed_data = JSON.parse(response);
    //                 //console.log(parsed_data);
    //                 if (parsed_data.Response) {
    //                     $.cookie('lineToken', parsed_data.Response);
    //                 }
    //             }
    //             var lineInfo = {
    //                 LineToken: $.cookie('lineToken'),
    //                 Message: $('#message').val()
    //             };

    //             var promise = $.ajax({
    //                 type: 'POST',
    //                 url: 'EventAPI/line/SendMessage',
    //                 async: true,
    //                 crossDomain: true,
    //                 headers: {
    //                     'token': $.cookie('token'),
    //                     "Content-Type": "application/json"
    //                 },
    //                 processData: false,
    //                 data: JSON.stringify(lineInfo)});

    //             promise.done(toDoWhenSendMessageSuccess);
    //         }
    //         if (!($.cookie('line_code') || !($.cookie('call_back_url')))) {
    //             $("#fail-line").html("You need to verify your line account first.");
    //             $("#fail-line").show();
    //         } else {
    //             if ($.cookie('lineToken')){
    //                 sendMessage('');
    //             } else {
    //                 var lineInfo = {
    //                     Code : $.cookie('line_code'),
    //                     CallBackURL : $.cookie('call_back_url')
    //                 }
    //                 var promise =  $.ajax({
    //                     type: 'POST',
    //                     url: 'EventAPI/line/FetchToken',
    //                     async: true,
    //                     crossDomain: true,
    //                     headers: {
    //                         'token': $.cookie('token'),
    //                         "Content-Type": "application/json"
    //                     },
    //                     data: JSON.stringify(lineInfo),
    //                     'processData': false, 
    //                 });

    //                 promise.done(sendMessage);
    //             }
    //         } 
    //     }

    //     initLineModal();
    //     $('#send-message').click(()=>{testLine();});       
    //     $("#line-modal").modal("show");
    // }
    $("#update-success").hide();
    $("#delete-success").hide();
    $("#fail").hide();

    $.ajax({
        type: 'GET',
        url: 'EmployeeAPI/Get?loginName=' + userName,
        async: true,
        crossDomain: true,
        headers: {
            'token': $.cookie('token')
        },
        success: function (response) {
            var parsed_data = JSON.parse(response);
            $("#email").val(parsed_data.Email);
            $("#employee-number").val(parsed_data.EmployeeNumber);
            $("#first-name").val(parsed_data.FirstName);
            $("#last-name").val(parsed_data.LastName);
            $("#photo-url").val(parsed_data.PhotoURL);
            if ($("#admin-flag")[0]) {
                $("#admin-flag")[0].checked = parsed_data.AdminFlag;
            }
            // if($("#admin-flag")[0])
            //     $("#email-flag")[0].checked=parsed_data.EmailFlag;

            // var callBackURL;

            // $('#line').click(function(){
            //     callBackURL = window.location.href;
            //     $.cookie('call_back_url', callBackURL);
            //     var URL = 'https://notify-bot.line.me/oauth/authorize?';
            //     URL += 'response_type=code';
            //     URL += '&client_id=Zs50qIfCopQXDVHuqTi9zg';
            //     URL += '&redirect_uri=' + callBackURL;
            //     URL += '&scope=notify';
            //     URL += '&state=NO_STATE';
            //     window.location.href = URL;
            //     console.log('click', window.location);          
            // });

            // var tokenStart = window.location.href.indexOf("=");
            // var tokenEnd = window.location.href.lastIndexOf("&");

            // if (tokenStart != -1 && tokenEnd != -1) {
            //     var lineCode = window.location.href.substring(tokenStart+1, tokenEnd);
            //     $.cookie('line_code', lineCode);
            //     window.history.pushState({},0,$.cookie('call_back_url'));
            // }

            // $('#line-test').click(function(){
            //     showLineModal(callBackURL);  
            // });
        },
        error: function (response) {

        }
    });
});

function userctl_update() {
    $("#update-success").hide();
    $("#delete-success").hide();
    $("#fail").hide();

    if (true === isEmpty($("#email").val())) {
        $("#fail").html("Please fill in the email.");
        $("#fail").show();
    }
    else if (false === isValidEmail($("#email").val())) {
        $("#fail").html("Please fill in a valid email address.");
        $("#fail").show();
    }
    else {
        $.ajax({
            type: 'PUT',
            url: 'EmployeeAPI/Update',
            async: true,
            crossDomain: true,
            headers: {
                'token': $.cookie('token'),
                "Content-Type": "application/json"
            },
            "processData": false,
            data:
                JSON.stringify(
                    {
                        'LoginName': $("#user-list").val(),
                        'Email': $("#email").val(),
                        'EmployeeNumber': $("#employee-number").val(),
                        'FirstName': $("#first-name").val(),
                        'LastName': $("#last-name").val(),
                        'PhotoURL': $("#photo-url").val(),
                        'AdminFlag': $("#admin-flag")[0].checked,
                        // 'EmailFlag': $("#email-flag")[0].checked
                    }
                ),
            success: function (response) {
                setTimeout(() => { $("#update-success").show(); }, 100);
            },
            error: function (response) {
                $("#fail").html("Failed to update user.");
                setTimeout(() => { $("#fail").show(); }, 100);
            }
        });
    }
}//);

function userctl_delete() {
    var $failAlert = $("#fail");
    var $deleteAlert = $("#delete-success");
    $("#update-success").hide();
    $failAlert.hide();
    $deleteAlert.hide();

    if ($("#user-list").val() == "admin") {
        setTimeout(() => {
            $failAlert.html("Can not delete the user \"admin\".");
            $failAlert.show();
        }, 100);
    }
    else if (confirm("The user " + $("#user-list").val() + " will be deletd, are you sure?")) {
        $.ajax({
            type: 'DELETE',
            url: 'EmployeeAPI/Delete',
            async: true,
            crossDomain: true,
            headers: {
                'token': $.cookie('token'),
                'loginName': $("#user-list").val()
            },
            success: function (response) {
                $deleteAlert.show();
                setTimeout(() => { $(document).trigger("reload-setting-usercontrol"); }, 5000);
            },
            error: function (response) {
                setTimeout(() => {
                    $failAlert.html("Failed to delete user.");
                    $failAlert.show();
                }, 100);
            }
        });
    }
}//);