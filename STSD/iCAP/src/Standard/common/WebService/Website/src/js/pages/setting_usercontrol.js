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