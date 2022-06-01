import "../../css/pages/album.css"
import "bootstrap/dist/css/bootstrap.min.css";
import "../../css/plugins/ekko-5.3.0-lightbox.css"
import "../library/albumLightbox"
import "../plugins/jquery.twbsPagination.min.js"
// import "../plugins/bootstrap-select.min.js"
// import "bootstrap/dist/js/bootstrap.min.js";
// import "font-awesome-webpack";
//import "../../css/plugins/FontAwesome-all-min.css"

export function showAlbum(data) {//div id = child-page-album

    let doc = document;
    console.log('showAlbum');
    console.log(data);
    $('.tab-secondary').hide();
    $('.tab-primary').show();
    $('#tab-primary-continer').html(`<a style="cursor: pointer;"><i class="fa fa-database" aria-hidden="true"></i> <span> Album</span></a><span class="divider hidden-xs"></span>`);

    deviceListInit();

    $(doc).off('click', '[data-toggle="lightbox"]');
    $(doc).on('click', '[data-toggle="lightbox"]', function (event) {
        event.preventDefault();
        $(this).ekkoLightbox();
    });
    doc = null;
}

export function deleteAlbumImg(imgId) {
    console.log("showImgId: " + imgId);
};

export function getDeviceImg(deviceId, albumPage, imgNum = 6) {
    console.log("getDeviceImg: " + deviceId + ' , ' + albumPage);
    const ONEPAGE_IMG_MAX = 6;

    let doc = document;
    let $albumBody = $('#albumBody');
    let $albumPagination = $('#albumPagination');
    let $albumTotalPagNum = $('#albumTotalPagNum');
    let data =
    {
        "DeviceId": 778899,
        "DeviceAlias": deviceId,
        "Total": 25,
        "ImageList": [
            {
                "Id": "1",
                "Image": "http://img.solomo.xinmedia.com/Albums/9/596/1766/660_9B03A4B2-D923-41EA-9868-C108714A1DD7.jpg",
                "Time": 1556501188
            },
            {
                "Id": "2",
                "Image": "https://www.articlelike.com/timthumb/index.php?src=https://www.articlelike.com/manage/0/product/30802/1516761523_922.jpg",
                "Time": 1556501288
            },
            {
                "Id": "3",
                "Image": "http://gcpnews.com/wp-content/uploads/2016/09/52341a54475a8.jpg",
                "Time": 1556501388
            },
            {
                "Id": "4",
                "Image": "http://gcpnews.com/wp-content/uploads/2016/09/52341a54475a8.jpg",
                "Time": 1556501688
            },
            {
                "Id": "5",
                "Image": "http://gcpnews.com/wp-content/uploads/2016/09/52341a54475a8.jpg",
                "Time": 1556501588
            },
            {
                "Id": "6",
                "Image": "http://gcpnews.com/wp-content/uploads/2016/09/52341a54475a8.jpg",
                "Time": 1556501488
            }

        ]
    }

    $('.caption a').off('click');

    if (data.Total == 0) {
        $albumBody.html(`<p style="margin-left: 20px;">Not found Img ...</p>`);
        $albumPagination.hide();
        $albumTotalPagNum.html("1 Pages");
    } else {
        $albumBody.html(``);
        data.ImageList.forEach((e, index) => {
            //console.log('index :' + (+index + 1));
            //console.log('thumbnail: ', doc.querySelectorAll('.thumbnail').length);
            if (doc.querySelectorAll('.thumbnail').length < ONEPAGE_IMG_MAX) {
                $albumBody.append(`
            <div class="col-sm-5 col-md-4 text-center" style="padding-top:10px;" unixTime="${e.Time}">
            <div class="thumbnail Block_shadow">
            <a href=${e.Image} data-toggle="lightbox"
                data-gallery="example-gallery" data-title="Device Alias: ${data.DeviceAlias}" data-footer="${unixTimeConvert(e.Time)}" data-id="${e.Id}">
                <img src=${e.Image} class="img-fluid">
            </a>
            <div class="caption" style="height: 56px;">
                <p style="margin: 10px 0px 0px 10px;display: inline-block; float:left;font-size: 16px;">${unixTimeConvert(e.Time)}</p>
                <a style="float:right;padding:6px 12px;" href="#" class="btn btn-default" role="button" data-toggle="modal"
                    data-target="#myModal" id="${e.Id}">
                    <i class="fa fa-trash-o fa-2x"></i>
                </a>
            </div>
            </div>
        </div>`);
            }
        })
    }

    let desc = function (a, b) {
        return $(a).attr("unixTime") > $(b).attr("unixTime") ? -1 : 1;
    }
    let sortByInput = function (sortBy) {
        let sortEle = $('#albumBody>div').sort(sortBy);
        $('#albumBody').empty().append(sortEle);
    }
    sortByInput(desc);


    $('.caption a').on('click', function (e) {
        deleteAlbumImg(this.id)
    });
    initPagination(data.Total, albumPage);
}

function unixTimeConvert(UnixTime) {
    let time = new Date(UnixTime * 1000);
    let y = time.getFullYear(),
        month = time.getMonth() + 1,
        m = month < 10 ? "0" + month : month,
        d = (time.getDate() < 10) ? "0" + time.getDate() : time.getDate(),
        h = (time.getHours() < 10) ? "0" + time.getHours() : time.getHours(),
        mi = (time.getMinutes() < 10) ? "0" + time.getMinutes() : time.getMinutes(),
        s = (time.getSeconds() < 10) ? "0" + time.getSeconds() : time.getSeconds();
    let sendDate = y + "-" + m + "-" + d + " " + h + ":" + mi + ":" + s;

    return sendDate;
}

function initPagination(totalImgsNum, startPage = 1) {

    const ONEPAGE_IMG_MAX = 6;
    let $albumPagination = $('#albumPagination');
    let $albumTotalPagNum = $('#albumTotalPagNum');
    let onPageFirstClick = false;
    let totalPages = Math.ceil(totalImgsNum / ONEPAGE_IMG_MAX);

    if (totalPages <= 1) {

        $albumPagination.hide();
        $albumTotalPagNum.html('1 Pages');
    } else {

        $albumPagination.show();
        $albumTotalPagNum.html(`${totalPages} Pages`);

        $('.pagination').twbsPagination('destroy');
        $albumPagination.twbsPagination({
            totalPages: totalPages,
            visiblePages: 3,
            next: 'Next',
            prev: 'Prev',
            first: false,
            last: false,
            startPage: startPage,
            onPageClick: function (event, page) {
                //fetch content and render here
                //console.log('event:', event);
                $.cookie('albumReadingPage', page);
                console.log('page:', page);
                if (onPageFirstClick) {
                    getDeviceImg($.cookie('current_deviceId'), page);
                    //getDeviceImg($.cookie('current_deviceId'), page, 6, $('#albumDeviceList').find("option:selected").attr('alias'));
                } else {
                    onPageFirstClick = true;
                }
            }
        });

    }
}

function deviceListInit() {
    let $albumDeviceList = $('#albumDeviceList');
    let deviceList;

    deviceList = [{
        Name: 'Device00080',
        Alias: 'Amy'
    }, {
        Name: 'Device00081',
        Alias: 'Rose'
    }, {
        Name: 'Device00083',
        Alias: 'Tom'
    }, {
        Name: 'Device00086',
        Alias: 'Jimmy'
    }];

    $albumDeviceList.off('change');

    $albumDeviceList.empty();
    $albumDeviceList.selectpicker('destroy');
    $albumDeviceList.selectpicker({ title: '--Please Select One--' });

    deviceList.forEach(element => {
        if (element.Name == $.cookie('current_deviceId')) {
            $albumDeviceList.append(`<option selected alias=${element.Alias} value=${element.Name}>${element.Alias}</option>`);
        } else {
            $albumDeviceList.append(`<option alias=${element.Alias} value=${element.Name}>${element.Alias}</option>`);
        }
    });
    $albumDeviceList.selectpicker('refresh');

    if ($albumDeviceList.selectpicker('val').length == 0) {
        $('#albumBody').html(`<p style="margin-left: 20px;">No device have be selected ...</p>`);
        $('#albumPagination').hide();
        $('#albumTotalPagNum').html('1 Pages');
        console.log("No Device");
    } else {
        //console.log("DeviceList init: ", $albumDeviceList.selectpicker('val'));
        //getDeviceImg($albumDeviceList.selectpicker('val'), 1, 6, $albumDeviceList.find("option:selected").attr('alias'));
        getDeviceImg($albumDeviceList.selectpicker('val'), 1);
    }

    $albumDeviceList.on('change', function () {
        console.log("DeviceList change: ", $albumDeviceList.selectpicker('val'));
        //getDeviceImg($albumDeviceList.selectpicker('val'), 1, 6, $albumDeviceList.find("option:selected").attr('alias'));

        getDeviceImg($albumDeviceList.selectpicker('val'), 1);
        $.cookie('current_deviceId', $albumDeviceList.selectpicker('val'));
    })
}