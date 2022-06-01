export function unlisten(arrayId) {
    arrayId.forEach(id => {
        $('#' + id).off();
    });
}

export function unlistenForm(targetArray) {
    targetArray.each((idx, element) => {
        $(element).off();
    });
}

export function fire(elem, type) {
    var event = new Event(type);

    elem.dispatchEvent(event);
}

export function changeColor(color, evt) {
    evt.target.style.color = color;
}

// function fire( elem, type ) {
//     // create a new synthetic event
//     var evt = elem.createEvent("Events");

//     // initialize the event
//     evt.initEvent( type, true, true, window, 1);
//     // dispatch, or fire the event
//     elem.dispatchEvent( evt );
// }

// export function unlistenForm()
// {
//     $('.event').each((idx, element)=>{
//         $(element).off();
//     });
// }