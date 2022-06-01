import "../../css/pages/login.css";
import getLogo from "../helpers/logo";
import {
    LOGINFORM_LEFT_LOGO,
    LOGINFORM_RIGHT_LOGO
} from "../constants/globalVariable"

function OnLogin() {
    var username = document.getElementById("loginname");
    var password = document.getElementById("password");

    $.ajax({
        type: 'GET',
        url: 'AuthenticationAPI/Login',
        async: true,
        crossDomain: true,
        headers: {
            'username': username.value,
            'password': password.value
        },
        success: function (response) {
            var parsed_data = JSON.parse(response);
            $.removeCookie('current_page');
            $(document).trigger("login", [parsed_data.Token]);
        },
        error: function (response) {
        }
    });
}

$(document).on("reload-login", function () {
    $('#login-form').on('submit', function (e) {
        e.preventDefault();
        OnLogin();
    });

    document.getElementById("contact-buttons-bar").style.display = 'none';
    var logoimg = document.getElementById("login-icaplogo");
    var logoinno = document.getElementById("login-innologo");
    logoimg.src = getLogo(LOGINFORM_RIGHT_LOGO);
    logoinno.src = getLogo(LOGINFORM_LEFT_LOGO);
});
