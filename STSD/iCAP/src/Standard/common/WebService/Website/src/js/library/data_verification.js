export function isValidEmail(email) {
    if (email.search(/^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z]+$/) == -1)
        return false;
    return true;
}

export function isNaturalNumber(number) {
    var r = /^\+?[1-9][0-9]*$/;
    return r.test(number);
}

export function isEmpty(data) {
    if (data === "" || data === undefined || data === null)
        return true;
    return false;
}

export function fieldsAreEmpty(className) {
    var emptyFlag = true;
    $('.' + className).each((idx, field) => {
        if (false == isEmpty($(field).val())) {
            emptyFlag = false;
            return false;
        }
    });
    return emptyFlag;
}

// function isValidInterval(className, alertId)
// {
//     var validFlag = false;
//     $('.'+className).each((idx,field)=>{
//         if(false == isEmpty($(field).val()))
//         {
//             if(is)

//         }
//     });
//     return emptyFlag;
// }



function numberFieldIsEmpty(number) {
    if (number === 0 || number === undefined || number === null)
        return true;
    return false;
}


export function isValidPassword(password, verifyPassword) {
    return Boolean(password === verifyPassword);
}

export function isValidLongitude(longitude) {
    if (Number(longitude) >= -180.0 && Number(longitude) <= 180.0)
        return true;
    else
        return false;
}

export function isValidLatitude(latitude) {
    if (Number(latitude) >= -90.0 && Number(latitude) <= 90.0)
        return true;
    else
        return false;
}

export function isGoogleSMTP(address) {
    if (!~address.indexOf("gmail"))
        return false;
    return true;
}

export function isYahooSMTP(address) {
    if (!~address.indexOf("yahoo"))
        return false;
    return true;
}

export function isHotmailSMTP(address) {
    if (!~address.indexOf('smtp.live.com'))
        return false;
    return true;
}

export function isEmptyArray(array) {
    if (array.length === 0)
        return true;
    else
        return false;
}

export function isValidLength(str, limitLength) {
    if (str.length > limitLength)
        return false;
    else
        return true;
}

export function nameExists(optionName, newName) {
    // const index = $.map($('#' + selectId + ' option'), function(item, index){
    //     return item.text;
    // }).indexOf(newName);
    const index = optionName.indexOf(newName);

    if (index !== -1) {
        return true;
    } else {
        return false;
    }
}

