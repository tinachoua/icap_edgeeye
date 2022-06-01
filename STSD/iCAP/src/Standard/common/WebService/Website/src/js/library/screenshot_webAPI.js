import imgPrew from '../../html/components/img_preview.html'
import { API } from "../library/api_handler";
import { timeConverter } from "../library/common";

export function connectScreenShotWebAPI() {
    let doc = document;
    let screenshotbtn = doc.querySelector('#a-screenshot');
    let screenShotIcon = doc.getElementById('Icon-screenshot');
    screenshotbtn.onclick = function () {
        return false;
    };
    screenshotbtn.title = `Send cmd...`;
    let childpage = doc.querySelector('#child-page-screenshotPreView');
    childpage.innerHTML = imgPrew;
    screenshotbtn.title = `Wait picture...`;
    screenShotIcon.className = "faicon fa fa-spinner fa-spin";
    screenshotbtn.style.backgroundColor = "#219744";
    let apiHandler = API();
    const URL = 'DeviceAPI/Screenshot?devName='
    const deviceId = $.cookie("current_deviceId");
    let promise = apiHandler.GET(URL + deviceId);

    function onClose(isError) {
        if (!isError) {
            rebindScreenShot();
        } else {
            (function errorEvent() {
                return new Promise((resolve, reject) => {
                    screenShotIcon.className = "fa fa-exclamation-triangle";
                    screenshotbtn.style.backgroundColor = "#c3182c";
                    screenshotbtn.title = `Try again later ...`;
                    setTimeout((function () {
                        resolve();
                    }), 5000);
                });
            })().then(rebindScreenShot);
        }
        $('#img-preview-img').off();
        $('.image-zoom').off();
    }

    function rebindScreenShot() {
        screenshotbtn.title = `Screen Shot`;
        screenshotbtn.onclick = connectScreenShotWebAPI;
        screenShotIcon.className = "fa fa-camera";
        screenshotbtn.style.backgroundColor = "#3b5998";
    }

    promise.done((response, textStatus, xhr) => {
        (function onMessage(data) {
            if (doc.getElementById("img-preview-modal")) {
                screenshotbtn.title = `Picture Received.`;
                doc.querySelector("#img-preview-img").src = 'data:image/jpeg;base64,' + data.Image;
                doc.querySelector("#img-time").innerHTML = timeConverter(data.Time);
                doc.querySelector("#img-preview-label").innerHTML = `Device ${data.Alias} Screen Shot Preview`;
                doc.querySelector("#img-preview-save").onclick = saveImg;
                doc.querySelector("#img-preview-delete").onclick = () => { noSaveImg(data.Id, deviceId) };
                doc.querySelector("#img-preview-close").onclick = () => { noSaveImg(data.Id, deviceId) };
                let $imgScreenshot = $('#img-screentshot');
                let $imgModal = $('#img-preview-modal');
                $imgModal.modal('show');
                doc.documentElement.style.overflow = 'hidden';
                screenShotIcon.className = "fa fa-camera";
                screenshotbtn.style.backgroundColor = "#3b5998";

                (function () {
                    $(document).ready(() => {
                        $('#img-preview-img').click(function () {
                            $('.image-zoom img').attr('src', $(this).attr('src'));
                            $imgScreenshot.show();
                            $('img-preview-img').css('display', 'none');
                            $imgModal.modal('toggle');
                            return false;
                        });
                        // $('.image-zoom').mousemove(function (e) {
                        //     var h = $(this).find('img').height();
                        //     var vptHeight = $(document).height();
                        //     var y = -((h - vptHeight) / vptHeight) * e.pageY;

                        //     $('div img').css('top', y + "px");
                        // });
                        $imgScreenshot.click(function () {
                            $imgScreenshot.hide();
                            $('img-preview-img').css('display', 'block');
                            $imgModal.modal('toggle');
                        });
                    });
                })();
                $imgModal.on('hidden.bs.modal', function () {
                    if ($('#img-screentshot').css('display') === 'none') {
                        doc.documentElement.style.overflow = 'scroll';
                    }
                })
            } else {
                rebindScreenShot();
                onClose();
            }
        })(response);
    });
    promise.fail((response) => {
        onClose(true);
    });

    function saveImg() {
        // if (isconnected) {
        //     doSend('save');
        //     websocket.close();
        //     if ($.cookie("current_page") == 'album' && $.cookie('albumReadingPage') == '1') {
        //         getDeviceImg($.cookie('current_deviceId'), $.cookie('albumReadingPage'));
        //     }
        // }
        onClose();
    }
    function noSaveImg(id, deviceId) {
        let deletePromise = apiHandler.DELETE(`DeviceAPI/Screenshot/Image/${id}?devName=${deviceId}`);
        deletePromise.done((response, textStatus, xhr) => {
            console.log('OK!');
        })
        deletePromise.fail((response, textStatus, xhr) => {
            console.log('Error');
        })
        onClose();
    }
}