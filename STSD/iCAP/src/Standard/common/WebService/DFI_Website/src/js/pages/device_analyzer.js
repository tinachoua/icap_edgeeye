import imgicaplogo2 from "../../assets/images/logo.svg";

function RenderData(element) {
    const {Model, Cap, SN} = element.Storage || {
        Model: 'N/A', Cap:'N/A', SN:'N/A'
    };
    var model_label = document.getElementById("stor-model");
    model_label.textContent = "Model: ";
    model_label.appendChild(document.createElement("br"));
    model_label.appendChild(document.createTextNode(Model));

    var capacity_label = document.getElementById("stor-cap");
    capacity_label.textContent = "Capacity: ";
    capacity_label.appendChild(document.createElement("br"));
    capacity_label.appendChild(document.createTextNode(Cap + " GB"));

    var sn_label = document.getElementById("stor-sn");
    sn_label.textContent = "Serial Number: ";
    sn_label.appendChild(document.createElement("br"));
    sn_label.appendChild(document.createTextNode(SN));

    var chart_TR = document.getElementById("stor-tr");

    const RW = element.RW;
    const src = RW.SRC || 0;
    const rrc = RW.RRC || 0;
    const swc = RW.SWC || 0;
    const rwc = RW.RWC || 0;
    const sr = RW.SR || {label:[''], value:[0]};
    const sw = RW.SW || {label:[''], value:[0]};
    const rr = RW.RR || {label:[''], value:[0]};
    const rw = RW.RW || {label:[''], value:[0]};

    const {
        Health, AvgEC:{Count}, PECycle
    } = element.Lifespan || {
        Health: 0, AvgEC:{
            Count: 0
        }, PECycle: 0
    }

    $Card({
        id: 1,
        name: "Total Read",
        type: "pie",
        width: "col-md-4 col-sm-6 col-xs-12",
        label: [''],
        data: {
            label: ["Sequential", "Random"],
            value: [src, rrc]
        }
    }).put(chart_TR).render();

    var chart_TW = document.getElementById("stor-tw");
    $Card({
        id: 2,
        name: "Total Write",
        type: "pie",
        width: "col-md-4 col-sm-6 col-xs-12",
        label: [''],
        data: {
            label: ["Sequential", "Random"],
            value: [swc, rwc]
        }
    }).put(chart_TW).render();

    var chart_SR = document.getElementById("stor-sr");
    $Card({
        id: 3,
        name: "Sequential Read",
        type: "pie",
        width: "col-md-4 col-sm-6 col-xs-12",
        label: [''],
        data: sr
    }).put(chart_SR).render();

    var chart_SW = document.getElementById("stor-sw");
    $Card({
        id: 4,
        name: "Sequential Write",
        type: "pie",
        width: "col-md-4 col-sm-6 col-xs-12",
        label: [''],
        data: sw
    }).put(chart_SW).render();

    var chart_RR = document.getElementById("stor-rr");
    $Card({
        id: 5,
        name: "Random Read",
        type: "pie",
        width: "col-md-4 col-sm-6 col-xs-12",
        label: [''],
        data: rr
    }).put(chart_RR).render();

    var chart_RW = document.getElementById("stor-rw");
    $Card({
        id: 6,
        name: "Random Write",
        type: "pie",
        width: "col-md-4 col-sm-6 col-xs-12",
        label: [''],
        data: rw
    }).put(chart_RW).render();

    var chart_health = document.getElementById("stor-health");
    $Card({
        id: 7,
        name: "Storage Health",
        type: "gauge",
        width: "col-md-6 col-sm-6 col-xs-12 gauge",
        label: ["Health", "Used", Health + '%'],
        data: [
            Health,
            Math.round((100 - Health) * 100) / 100
        ]
    }).put(chart_health).render();

    var chart_lifespan = document.getElementById("stor-pe");
    $Card({
        id: 8,
        name: "P/E Cycle",
        type: "gauge",
        width: "col-md-6 col-sm-6 col-xs-12 gauge",
        label: ["Average Erase Count", "PE Cycle", Count + '/' + PECycle],
        data: [
            Count,
            PECycle - Count
        ]
    }).put(chart_lifespan).render();

    var lifespan_date = [];
    var lifespan_data = [];
    const length = element.Lifespan ? element.Lifespan.Lifespan.length : 0
    for (var i = 0; i < length; i++) {
        lifespan_date[i] = element.Lifespan.Lifespan[i].Date;
        lifespan_data[i] = element.Lifespan.Lifespan[i].EstimatedDays;
    }

    var chart_lifespan_log = document.getElementById("stor-life");
    $Card({
        id: 9,
        name: "Lifespan Logs",
        type: "line",
        width: "col-md-12 col-sm-12 col-xs-12",
        label: ['Estimation Lifespan (Days)'],
        data: [
            lifespan_date,
            lifespan_data
        ]
    }).put(chart_lifespan_log).render();
}

/**
 * Switch the storage information by installed index.
 */
function SwitchToStorage() {
    var element = JSON.parse(this.getAttribute("user-data"));
    RenderData(element);
}

function RenderStorageAnalysisData(data) {
    var index_btn = document.getElementById("analyzer-storage-btn");

    data.Analyzer.forEach(function (element, idx, array) {
        const lifespan = data.Lifespan.find(ele => ele.SN === element.SN);
        const storage = data.Storage.find(ele => ele.SN === element.SN);
        var btn_data = {
            "Lifespan": lifespan,
            "RW": element,
            "Storage":storage
        };
        var btn = document.createElement("button");
        btn.setAttribute("class", "btn btn-mini btn-dark");
        btn.setAttribute("style", "margin-right: 3px;");
        btn.setAttribute("user-data", JSON.stringify(btn_data));

        btn.textContent = idx;
        btn.value = idx;
        btn.onclick = SwitchToStorage;
        index_btn.appendChild(btn);

        if (idx == 0) {
            RenderData(btn_data);
        }
    });
}

/**
 * Render Device Overview
 * @param {string} store_token The identity token
 * @param {string} devName Device name
 */
function RenderOverview(store_token, devName) {
    $.ajax({
        type: 'GET',
        url: 'DeviceInfoAPI/iAnalyzer?DeviceName=' + devName,
        async: true,
        crossDomain: true,
        headers: {
            'token': store_token
        },
        success: function (response) {
            var parsed_data = JSON.parse(response);

            if (JSON.stringify(parsed_data.Analyzer[0]) !== '{}') {
                let unsupportFlag = document.getElementById('unsupported-feature');

                unsupportFlag.setAttribute('style', 'display:none');
                RenderStorageAnalysisData(parsed_data);
            } else {
                let unsupportFeature = document.getElementById('unsupported-feature');
                let logo = unsupportFeature.getElementsByTagName('img')[0];
                let analyzer = document.getElementById('child-page').getElementsByClassName('device_analyzer')[0];

                analyzer.setAttribute('style', 'display:none');
                unsupportFeature.setAttribute('style', '');
                logo.setAttribute('src', imgicaplogo2);
            }

            $('body').removeClass('loading');
        },
        error: function (response) {
            $('body').removeClass('loading');
        }
    });
}

$(document).on("reload-device-analyzer", function (event, devName) {
    /**
     * To Do: Check token and redirect to login page when get wrong token from cookie.
     */
    var store_token = $.cookie('token');
    RenderOverview(store_token, devName);
});