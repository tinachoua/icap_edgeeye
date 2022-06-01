export function showMyAlert(alertId, message, blankId)
{
    $('#' + alertId).html(message);
    setTimeout(()=>{
        $('#' + alertId).show();
        $('#' + blankId).show();
    },100);
}

export function hideAlertById(arrayId)
{
    arrayId.forEach(alertId => {
        $("#" + alertId).hide();    
    });
}

export function hideAlertByClassName(arrayName)
{
    arrayName.forEach(name =>{
        $('.' + name).hide();
    });
}

export function clearForm()
{
    hideAlertByClassName(['alert']);
    $('.box').each((idx, target)=>{
        $(target).empty();
    });
    $('.input-filter').each((idx, input)=>{
        $(input).val('');
    });
    $('.label-box').each((idx, label)=>{
        $(label).empty();
    });
    $('.flag').each((idx, flag)=>{
        $(flag).prop('checked','');
    });
}

export function initCheckBox(FlagArray, FlagIdArray)
{
    var index = 0;
    FlagArray.forEach((element, index, array)=>{
        if (true == element) {
            $('#' + FlagIdArray[index]).prop('checked', 'true');
        } else {
            $('#' + FlagIdArray[index]).prop('checked', '');
        }
    })
}