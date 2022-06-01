import imgPrew from '../../html/components/img_preview.html'
import { ScreenShot } from '../layout/header';
import { getDeviceImg } from "../library/album";

export function connectWebSocket(id) {
    const wsUri = 'ws://172.16.36.71:52000/ws'
    let websocket;
    let isconnected = false;
    let screenshotbtn = document.querySelector('#a-screenshot');
    let screenShotIcon = document.getElementById('Icon-screenshot');
    let isError = false;

    screenshotbtn.onclick = '';
    screenshotbtn.title = `Send cmd...`;


    websocket = new WebSocket(wsUri);
    websocket.binaryType = "blob";
    websocket.onopen = function (evt) { onOpen(evt) };
    websocket.onclose = function (evt) { onClose(evt) };
    websocket.onmessage = function (evt) { onMessage(evt) };
    websocket.onerror = function (evt) { onError(evt) };



    function onOpen(evt) {
        isconnected = true;
        console.log("CONNECTED");
        let childpage = document.querySelector('#child-page-screenshotPreView');
        $("#imgPreView").remove();
        let pre = document.createElement("div");
        pre.style.wordWrap = "modal-dialog";
        pre.setAttribute("id", "imgPreView");
        pre.innerHTML = imgPrew;
        childpage.appendChild(pre);

        screenshotbtn.title = `Wait picture...`;
        screenShotIcon.className = "faicon fa fa-spinner fa-spin";
        screenshotbtn.style.backgroundColor = "#219744";

        // let data = {
        //     cmd: "screenshot",
        //     devName: id
        // }

        let data = {
            Token: $.cookie('token'),
            Cmd: "ScreenShot",
            DevName: id
        }
        doSend(JSON.stringify(data));
    }

    function onClose(evt) {
        console.log("DISCONNECTED");
        isconnected = false;

        if (!isError) {
            showScreenShot();
        } else {
            erroeEvent().then(showScreenShot);
        }
    }

    function erroeEvent() {
        return new Promise((resolve, reject) => {
            screenShotIcon.className = "fa fa-exclamation-triangle";
            screenshotbtn.style.backgroundColor = "#c3182c";
            screenshotbtn.title = `Try again later ...`;
            setTimeout((function () {
                isError = false;
                resolve();
            }), 5000);
        });
    }

    function showScreenShot() {
        screenshotbtn.title = `Screen Shot`;
        screenshotbtn.onclick = ScreenShot;
        screenShotIcon.className = "fa fa-camera";
        screenshotbtn.style.backgroundColor = "#3b5998";
    }

    function onMessage(evt) {

        let data = JSON.parse(evt.data);
        console.log('data: ', data);

        // if (evt.data instanceof Blob) {
        //     let urlCreator = window.URL || window.webkitURL;
        //     let imageUrl = urlCreator.createObjectURL(evt.data);
        //     document.querySelector("#img").src = imageUrl;
        // }

        if (document.getElementById("imgPreViewModal")) {
            screenshotbtn.title = `Picture Received.`;
            document.querySelector("#imgPreViewImg").src = data.img;
            document.querySelector("#imgTime").innerHTML = timeConverter(data.time);
            document.querySelector("#imgPreViewLabel").innerHTML = `Device ${data.target} Screen Shot Preview`;
            document.querySelector("#imgPreViewSave").onclick = saveImg;
            document.querySelector("#imgPreViewClose").onclick = noSaveImg;
            document.querySelector("#img-preview-modal").onclick = noSaveImg;
            $('#imgPreViewModal').modal('show');
            screenShotIcon.className = "fa fa-camera";
            screenshotbtn.style.backgroundColor = "#3b5998";
        } else {
            showScreenShot();
            websocket.close();
        }
    }
    function onError(evt) {
        console.log('請稍後在試');
        isError = true;
    }

    function doSend(message) {
        console.log("SENT: " + message);
        websocket.send(message);
    }

    function saveImg() {
        if (isconnected) {
            doSend('save');
            websocket.close();
            if ($.cookie("current_page") == 'album' && $.cookie('albumReadingPage') == '1') {
                getDeviceImg($.cookie('current_deviceId'), $.cookie('albumReadingPage'));
            }
        }
    }
    function noSaveImg() {
        if (isconnected) {
            doSend('nosave');
            websocket.close();
        }
    }
}

function timeConverter(UNIX_timestamp) {
    // var a = new Date(UNIX_timestamp * 1000);
    // var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    // var year = a.getFullYear();
    // var month = months[a.getMonth()];
    // var date = a.getDate();
    // var hour = a.getHours();
    // var min = a.getMinutes();
    // var sec = a.getSeconds();
    // var time = date + ' ' + month + ' ' + year + ' ' + hour + ':' + min + ':' + sec;

    let time = new Date(UNIX_timestamp * 1000);
    let y = time.getFullYear(),
        month = time.getMonth() + 1,
        m = month < 10 ? "0" + month : month,
        d = (time.getDate() < 10) ? "0" + time.getDate() : time.getDate(),
        h = (time.getHours() < 10) ? "0" + time.getHours() : time.getHours(),
        mi = (time.getMinutes() < 10) ? "0" + time.getMinutes() : time.getMinutes(),
        s = (time.getSeconds() < 10) ? "0" + time.getSeconds() : time.getSeconds();
    let sendDate = y + "-" + m + "-" + d + " " + h + ":" + mi + ":" + s;

    return sendDate;
    //return time;
}

