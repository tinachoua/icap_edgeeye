import { makeSelect } from '../library/select_form';

export function showErrorMessage(response, alertId, blankId, message) {
    if (403 == response.status) {
        const parsed_data = JSON.parse(response.responseText);
        $('#' + alertId).html(parsed_data.Response);
        setTimeout(() => {
            $('#' + alertId).show();
            $('#' + blankId).show();
        }, 100);
    } else {
        $('#' + alertId).html(message);
        setTimeout(() => {
            $('#' + alertId).show();
            $('#' + blankId).show();
        }, 100);
    }
}

export function initSelectOption(path, selectId, modalId, dangerAlertId, initFormFlag) {
    function toDoWhenSuccess(response) {
        if (response) {
            var parsed_data = JSON.parse(response);

            parsed_data.List[0] && makeSelect(selectId, null, parsed_data.List);
            $('#' + selectId).selectpicker('refresh');
        }
    }

    function toDoWhenFail(response) {
        if (403 == response.status) {
            $('#' + dangerAlertId).html('You have been idle too long, please log-in again.');
            setTimeout(() => {
                $('#' + dangerAlertId).show();
                $('#blank-alert').show();
                $("body").removeClass('loading');
            }, 100);
        } else {
            $('#' + dangerAlertId).html('There is something wrong. Please refresh the web page and try again.');
            setTimeout(() => {
                $('#' + dangerAlertId).show();
                $('#blank-alert').show();
                $("body").removeClass('loading');
            }, 100);
        }
    }

    function initForm() {
        var lastValue = $('#' + selectId + ' option:last-child').val();
        var select = $('#' + selectId);
        select.val(lastValue);
        select.selectpicker('refresh');
        select.trigger('change');
        // $('#' + modalId).modal('hide');
        $("body").removeClass('loading');
    }

    var promise = $.ajax({
        type: 'GET',
        url: path,
        async: true,
        crossDomain: true,
        headers: {
            'token': $.cookie('token')
        }
    });

    if (false === initFormFlag) {
        promise.done((response) => {
            toDoWhenSuccess(response);
            $("body").removeClass('loading');
        });
        promise.fail(toDoWhenSuccess);
    } else {
        promise.done(toDoWhenSuccess).done(initForm);
        promise.fail(toDoWhenFail);
    }
}

export function GET_API(path, headerKey, headerValue) {
    if (Boolean(headerKey) === true) {
        return $.ajax({
            type: 'GET',
            url: path,
            async: true,
            crossDomain: true,
            headers: {
                'token': $.cookie('token'),
                headerKey: headerValue
            },
            processData: false,
        });
    } else {
        return $.ajax({
            type: 'GET',
            url: path,
            async: true,
            crossDomain: true,
            headers: {
                'token': $.cookie('token'),
            },
            processData: true,
        });
    }
}

export function POST_PUT_API(type, path, payload) {
    return $.ajax({
        type: type,
        url: path,
        async: true,
        crossDomain: true,
        headers: {
            'token': $.cookie('token'),
            "Content-Type": "application/json"
        },
        processData: false,
        data: JSON.stringify(payload)
    });
}

export function API() {
    function Get(path, headerKey, headerValue, loading) {
        let APIObject = {
            type: 'GET',
            url: path,
            async: true,
            crossDomain: true,
            global: loading
        };
        let headerObject = {
            'token': $.cookie('token'),
        };

        if (headerKey !== null) {
            headerObject[headerKey] = headerValue;
        }
        APIObject.headers = headerObject;

        return $.ajax(APIObject);
    }

    function Put(path, payload) {
        return $.ajax({
            type: 'PUT',
            url: path,
            async: true,
            crossDomain: true,
            headers: {
                'token': $.cookie('token'),
                "Content-Type": "application/json"
            },
            processData: false,
            data: JSON.stringify(payload)
        });
    }

    function Post(path, payload, global = true) {
        return $.ajax({
            type: 'POST',
            url: path,
            async: true,
            crossDomain: true,
            headers: {
                'token': $.cookie('token'),
                "Content-Type": "application/json"
            },
            processData: false,
            data: JSON.stringify(payload),
            global: global
        });
    }

    function Delete(path, headerKey, headerValue) {
        let APIObject = {
            type: 'DELETE',
            url: path,
            async: true,
            crossDomain: true,
        };
        let headerObject = {
            'token': $.cookie('token'),
        };

        if (headerKey !== null) {
            headerObject[headerKey] = headerValue;
        }

        APIObject.headers = headerObject;
        return $.ajax(APIObject);
    }

    var publicAPI = {
        GET: Get,
        PUT: Put,
        POST: Post,
        DELETE: Delete
    };

    return publicAPI;
}

// export function PUT_API(path, payload)
// {
//     return $.ajax({
//         type: 'PUT',
//         url: path,
//         async: true,
//         crossDomain: true,
//         headers: {
//             'token': $.cookie('token'),
//             "Content-Type": "application/json"
//         },
//         processData: false,
//         data: JSON.stringify(payload)
//     });
// }