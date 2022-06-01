$(document).on("reload-setting-images", function(){

    var store_token = $.cookie('token');
    $("#update-email-success").hide();
    $("#update-email-fail").hide();

    // $.ajax({
    //     type: 'GET',
    //     url: 'DeviceAPI/GetImg',
    //     async: true,
    //     crossDomain: true,
    //     headers: {
    //         "token" : store_token,
    //         "devName" : document.getElementById("devName").value   
    //     },
        
    // })
})
// -------------------------------
$(document).ready(function(){
	// Translated
	$('.dropify').dropify({
		messages: {
            'default': 'Drag and drop a file here or click',
            'replace': 'Drag and drop or click to replace',
            'remove':  'Remove',
            'error':   'Ooops, something wrong happended.'
		}
	});
	// Used events
	var drEvent = $('.dropify-event').dropify();

	drEvent.on('dropify.beforeClear', function(event, element){
		return confirm("Do you really want to delete \"" + element.filename + "\" ?");
	});

	drEvent.on('dropify.afterClear', function(event, element){
		alert('File deleted');
	});
});