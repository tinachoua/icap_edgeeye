$(document).on("reload-setting-device", function () {
    /**
     * To Do: Check token and redirect to login page when get wrong token from cookie.
     */
    if ($.cookie("current_page") !== "setting-device") {
        return;
    }
    $("#update-success").hide();
    $("#delete-success").hide();
    $("#fail").hide();

    var store_token = $.cookie('token');
    $("#devListObj").empty();
    var select_obj = document.getElementById("devListObj");
    select_obj.onchange = function () {
        $(document).trigger("reload-device-setting", this.value);
    };

    $.ajax({
        type: 'GET',
        url: 'StatusAPI/GetList',
        async: true,
        crossDomain: true,
        headers: {
            'token': store_token
        },
        success: function (response) {
            var parsed_data = JSON.parse(response);

            parsed_data.DeviceList.forEach(function (element, idx, array) {
                var option_obj = document.createElement("option");
                option_obj.value = element;
                option_obj.textContent = element;
                if (idx == 0) {
                    option_obj.selected = "selected";
                }

                select_obj.appendChild(option_obj);
            });
            $(document).trigger("reload-device-setting", parsed_data.DeviceList[0]);
        },
        error: function (response) {
        }
    });

    //$(document).on('click', "#update-device", update_dev);
    //$(document).on('click', "#delete-device", delete_dev);
});

$(document).on('click', "#update-device", update_dev);
$(document).on('click', "#delete-device", delete_dev);

$(document).on("reload-device-setting", function (event, devName) {
    $("#update-success").hide();
    $("#delete-success").hide();
    $("#fail").hide();

    var store_token = $.cookie('token');

    $.ajax({
        type: 'GET',
        url: 'DeviceAPI/Get?devName=' + devName,
        async: true,
        crossDomain: true,
        headers: {
            'token': store_token
        },
        success: function (response) {
            if (response) {
                var parsed_data = JSON.parse(response);
                if (document.getElementById("devLabel"))
                    document.getElementById("devLabel").setAttribute("user-data", parsed_data.Id);
                $("#alias").val(parsed_data.Alias);
                $("#lat").val(parsed_data.Latitude);
                $("#long").val(parsed_data.Longitude);
                $("#photourl").val(parsed_data.PhotoURL);
                $("#owner").val(parsed_data.OwnerName);
            }
        },
        error: function (response) {
            $("#fail").html("Failed to retrieve device information.");
            setTimeout(() => { $("#fail").show(); }, 100);
        }
    });

});

//$("#update-device").click(function(event){
function update_dev() {

    $("#update-success").hide();
    $("#delete-success").hide();
    $("#fail").hide();

    $.ajax({
        type: 'PUT',
        url: 'DeviceAPI/Update',
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
                    'Id': document.getElementById("devLabel").getAttribute("user-data"),
                    'DevName': $("#devListObj").val(),
                    'Alias': $("#alias").val(),
                    'Longitude': $("#long").val(),
                    'Latitude': $("#lat").val(),
                    'PhotoURL': $("#photourl").val(),
                    'OwnerName': $("#owner").val()
                }
            ),
        success: function (response) {
            //var parsed_data = JSON.parse(response);
            setTimeout(() => { $("#update-success").show(); }, 100);
        },
        error: function (response) {
            $("#fail").html("Failed to update device.");
            setTimeout(() => { $("#fail").show(); }, 100);
        }
    });
}//)

//$("#delete-device").click(function(event){
function delete_dev() {

    $("#update-success").hide();
    $("#delete-success").hide();
    $("#fail").hide();

    if (confirm("The device " + $("#devListObj").val() + " will be deleted, are you sure?")) {
        $.ajax({
            type: 'DELETE',
            url: 'DeviceAPI/Delete',
            async: true,
            crossDomain: true,
            headers: {
                'token': $.cookie('token'),
                'devName': $("#devListObj").val()
            },
            success: function (response) {
                $("#delete-success").show();
                setTimeout(() => { $(document).trigger("reload-setting-device"); }, 5000);
            },
            error: function (response) {
                var dataObject = JSON.parse(response.responseText);
                $("#fail").html(dataObject.Response);
                setTimeout(() => { $("#fail").show(); }, 100);
            }
        });
    }
}//)