import { isValidEmail, isValidPassword, isEmpty } from "../library/data_verification";

(function IIFE() {
    var adminFlag;
    $(document).on("reload-setting-profile", function () {
        /**
         * To Do: Check token and redirect to login page when get wrong token from cookie.
         */
        var store_token = $.cookie('token');
        $("#update-success").hide();
        $("#update-fail").hide();
        $("#password-null").hide();

        $.ajax({
            type: 'GET',
            url: 'EmployeeAPI/GetFromToken',
            async: true,
            crossDomain: true,
            headers: {
                'token': store_token
            },
            success: function (response) {
                if (response) {
                    var parsed_data = JSON.parse(response);
                    $("#login-name").val(parsed_data.LoginName);
                    $("#email").val(parsed_data.Email);
                    $("#employee-number").val(parsed_data.EmployeeNumber);
                    $("#first-name").val(parsed_data.FirstName);
                    $("#last-name").val(parsed_data.LastName);
                    $("#photo-url").val(parsed_data.PhotoURL);
                    adminFlag = parsed_data.AdminFlag;
                }
            },
            error: function (response) {
            }
        });

    });

    $(document).on('click', '#profile-setting', profile_setting);

    function profile_setting() {

        var errorFlag = false;
        $("#update-success").hide();
        $("#update-fail").hide();
        $("#update-email").hide();
        $("#update-password").hide();

        if (true == isEmpty($("#email").val())) {
            errorFlag = true;
            $("#update-email").html("Please fill in the email.");
            setTimeout(() => { $("#update-email").show() }, 100);
        }
        else if (false == isValidEmail($("#email").val())) {
            errorFlag = true;
            $("#update-email").html("Please fill in a valid email address.");
            setTimeout(() => { $("#update-email").show() }, 100);
        }

        if (isEmpty($("#pwd").val()) || isEmpty($("#pwd-check").val())) {
            errorFlag = true;
            $("#update-password").html("Please fill in the password.");
            setTimeout(() => { $("#update-password").show() }, 100);
        }
        else if (false == isValidPassword($("#pwd").val(), $("#pwd-check").val())) {
            errorFlag = true;
            $("#update-password").html("Password does not match, please enter agein.");
            setTimeout(() => { $("#update-password").show(); }, 100);
        }

        if (false === errorFlag) {
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
                            'LoginName': $("#login-name").val(),
                            'Email': $("#email").val(),
                            'EmployeeNumber': $("#employee-number").val(),
                            'FirstName': $("#first-name").val(),
                            'LastName': $("#last-name").val(),
                            'PWD': btoa($("#pwd").val()),
                            'VerifyPWD': btoa($("#pwd-check").val()),
                            'PhotoURL': $("#photo-url").val(),
                            'AdminFlag': adminFlag
                        }
                    ),
                success: function (response) {
                    setTimeout(() => { $("#update-success").show(); }, 100);
                },
                error: function (response) {
                    setTimeout(() => { $("#update-fail").show(); }, 100);
                }
            });
        }


        // if(pwd && pwd_check && (pwd == pwd_check))
        // {
        //     document.getElementById("pwd-frame").setAttribute("class", "col-md-6");
        //     document.getElementById("pwd-chk-frame").setAttribute("class", "col-md-6");

        //     $.ajax({
        //         type: 'PUT',
        //         url: 'EmployeeAPI/Update',
        //         async: true,
        //         crossDomain: true,
        //         headers: {
        //             'token': $.cookie('token'),
        //             "Content-Type": "application/json"
        //         },
        //         "processData": false,
        //         data:
        //             JSON.stringify(
        //                 {
        //                     'LoginName': $("#login-name").val(),
        //                     'Email': $("#email").val(),
        //                     'EmployeeNumber': $("#employee-number").val(),
        //                     'FirstName': $("#first-name").val(),
        //                     'LastName': $("#last-name").val(),
        //                     'PWD': btoa(pwd),
        //                     'VerifyPWD': btoa(pwd_check),
        //                     'PhotoURL': $("#photo-url").val(),
        //                     'AdminFlag': adminFlag
        //                 }
        //         ),
        //         success: function(response)
        //         {
        //             var parsed_data = JSON.parse(response);
        //             setTimeout(()=>{$("#update-success").show();},100);
        //         },
        //         error: function(response)
        //         {
        //             setTimeout(()=>{$("#update-fail").show();},100);
        //         }
        //     });        

        // }
        // else if(false==pwd || false ==pwd_check)
        // {
        //     setTimeout(()=>{$("#password-null").show();},100);
        // }
        // else
        // {
        //     document.getElementById("pwd-frame").setAttribute("class", "col-md-6 has-error");
        //     document.getElementById("pwd-chk-frame").setAttribute("class", "col-md-6 has-error");     
        //     setTimeout(()=>{$("#update-fail").show();},100);
        // }
    }//);
})();