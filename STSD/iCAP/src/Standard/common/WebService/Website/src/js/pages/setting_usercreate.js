import { isValidEmail, isValidPassword, isEmpty } from "../library/data_verification";

$(document).on("reload-setting-usercreate", function () {

    $("#submit-success").hide();
    $("#submit-fail").hide();
    $("#submit-name").hide();
    $("#submit-email").hide();
    $("#submit-pwd").hide();
});

$(document).on('click', '#user-create', user_create);
function user_create() {

    var errorFlag = false;

    $("#submit-success").hide();
    $("#submit-fail").hide();
    $("#submit-name").hide();
    $("#submit-email").hide();
    $("#submit-pwd").hide();

    if (true === isEmpty($("#login-name").val())) {
        errorFlag = true;
        $("#submit-name").html("Please fill in the login name.");
        setTimeout(() => { $("#submit-name").show(); }, 100);
    }
    else if ($("#login-name").val() == "admin") {
        errorFlag = true;
        $("#submit-name").html("Login name exists.");
        setTimeout(() => { $("#submit-name").show(); }, 100);
    }

    if (true === isEmpty($("#email").val())) {
        errorFlag = true;
        $("#submit-email").html("Please fill in the email.");
        setTimeout(() => { $("#submit-email").show(); }, 100);
    }
    else if (false === isValidEmail($("#email").val())) {
        errorFlag = true;
        $("#submit-email").html("Please fill in a valid email address.");
        setTimeout(() => { $("#submit-email").show(); }, 100);
    }

    if (true === isEmpty($("#pwd").val()) || true === isEmpty($("#pwd-check").val())) {
        errorFlag = true;
        $("#submit-pwd").html("Please fill in the password.");
        setTimeout(() => { $("#submit-pwd").show(); }, 100);
    }
    else if (false === isValidPassword($("#pwd").val(), $("#pwd-check").val())) {
        errorFlag = true;
        $("#submit-pwd").html("Password does not match, please enter agein.");
        setTimeout(() => { $("#submit-pwd").show(); }, 100);
    }

    if (false === errorFlag) {
        let empNumber = $("#employee-number").val();
        let firstName = $("#first-name").val();
        let lastName = $("#last-name").val();
        let photoURL = $("#photo-url").val();

        $.ajax({
            type: 'POST',
            url: 'EmployeeAPI/Create',
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
                        'LoginName': $("#login-name").val(),
                        'Email': $("#email").val(),
                        'EmployeeNumber': (empNumber === '') ? null : empNumber,
                        'FirstName': (firstName === '') ? null : firstName,
                        'LastName': (lastName === '') ? null : lastName,
                        'PWD': btoa($("#pwd").val()),
                        'VerifyPWD': btoa($("#pwd-check").val()),
                        'PhotoURL': (photoURL === '') ? null : photoURL,
                        'AdminFlag': $("#admin-flag")[0].checked,
                        // 'EmailFlag': $("#email-flag")[0].checked
                    }
                ),
            success: function (response) {
                setTimeout(() => { $("#submit-success").show(); }, 100);
            },
            error: function (response) {
                $("#submit-fail").html("Login name exists.");
                setTimeout(() => { $("#submit-fail").show(); }, 100);
            }
        });
    }
}