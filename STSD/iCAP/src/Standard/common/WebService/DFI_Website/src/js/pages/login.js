import "../../style/DFI/login.css"

import logo from "../../assets/images/dfi-logo.svg";
// import imgicaplogo from "../../assets/images/iCAPlogo2.png";
// import imginnologo from "../../assets/images/logo-innodisk.png";
import header from "../../html/layout/header.html";

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
            // $.cookie('token', parsed_data.Token, { expires: 7 });
            $.removeCookie('current_page');
            $(document).trigger("login", [parsed_data.Token]);
            // document.getElementById("main").innerHTML = header;
            // require("../layout/side");
            // require("../layout/header");
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
    var logoimg = document.getElementById("logo");

    logoimg.src = logo;
});
