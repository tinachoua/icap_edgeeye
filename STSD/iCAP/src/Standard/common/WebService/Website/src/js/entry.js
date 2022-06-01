import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.min.js";
import "font-awesome-webpack";
require('jquery');
import "jquery.cookie";
import "paginationjs/dist/pagination.min.js";
import "paginationjs/dist/pagination.css";
var Chart = require('chart.js');
import dt from 'datatables.net';
import 'datatables.net-dt/css/jquery.dataTables.css';
import 'bootstrap-select/dist/css/bootstrap-select.min.css';
import 'bootstrap-select';

// dropify
import "dropify";
import "dropify/dist/css/dropify.min.css";

import "../css/plugins/jquery-jvectormap-2.0.3.css";
import "./plugins/jquery-jvectormap-2.0.3.min.js";
import "./plugins/jquery-jvectormap-world-mill.js";
import "../css/style.css";

import "../css/layout/header.css";
import "../css/layout/event.css";
import "../css/layout/setting.css";
import "../css/components/buttons.css";
import "../css/components/cards.css";
import "jquery-ui-stable/jquery-ui.min.css";
import "jquery-ui-stable/jquery-ui.min";
import '../css/eden.css'
// import "jquery-ui-stable/jquery-ui.css";
import login_page from "../html/pages/login.html";
import header from "../html/layout/header.html";

import "../css/plugins/ol.css";
import "../css/plugins/google-materialicons.css";
import "../css/components/oobButtons.css"
import {
    VERSION_NUMBER
} from "./constants/env"

require("./pages/login");
require("./layout/side");
require("./layout/header");
require("../js/plugins/polyfill");

(function IIFE() {
    var $document = $(document);

    window.onload = () => {
        $document
            .ajaxStart(() => {
                // ajax request went so show the loading image
                document.querySelector('body').classList.add('loading');
            })
            .ajaxStop(() => {
                // got response so hide the loading image
                document.querySelector('body').classList.remove('loading');
            });
    }

    $document.on("logout", function () {
        $.removeCookie('token');
        document.getElementById("main").innerHTML = login_page;
        $document.trigger("reload-login");
    });

    $document.on("login", function (event, store_token) {
        $.cookie('token', store_token, { expires: 7 });
        document.getElementById("main").innerHTML = header;
        const version = document.getElementById('version');

        version.textContent = VERSION_NUMBER;
        $document.trigger("reload-header");
        $document.trigger("rebind-toggle-menu");
    });

    $document.ready(function () {
        var store_token = $.cookie('token');
        if (store_token != null) {
            $.ajax({
                type: 'GET',
                url: 'AuthenticationAPI/TokenChecker',
                async: true,
                crossDomain: true,
                headers: {
                    'token': store_token
                },
                success: function (response) {
                    if (response) {
                        var ret = JSON.parse(response);
                        $document.trigger("login", [store_token]);
                    } else {
                        $document.trigger("logout");
                    }
                },
                error: function (response) {
                    $document.trigger("logout");
                }
            });
        }
        else {
            $document.trigger("logout");
        }

    });

})();

