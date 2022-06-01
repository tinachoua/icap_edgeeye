import { isValidEmail, isNaturalNumber, isEmpty, isGoogleSMTP, isYahooSMTP, isHotmailSMTP } from "../library/data_verification";
import { modalBack } from "../library/modal";
import { API } from "../library/api_handler";
import { fire } from "../library/event_handler";
import { from } from "rxjs";

$(document).on("reload-setting-mail", function () {
    /**
       * To Do: Check token and redirect to login page when get wrong token from cookie.
       */
    var $smtpAddress = $("#smtp-addr");
    var port = document.getElementById('port-number');
    var emailFrom = document.getElementById('email-from');
    var passpord = document.getElementById('email-pwd');
    var interval = document.getElementById('interval');
    var noneRadio = document.getElementById('none-radio');
    var sslRadio = document.getElementById('ssl-radio');
    var tlsRadio = document.getElementById('tls-radio');
    var enableCheckBox = document.getElementById('email-enable');
    var submitButton = document.getElementById('mail-setting');
    var successAlert = document.getElementById('submit-email-success');
    var failAlert = document.getElementById('submit-email-fail');
    var sendTestButton = document.getElementById('sendEmailTest');
    successAlert.style.display = 'none';
    failAlert.style.display = 'none';

    var apiHandler = API();

    var promise = apiHandler.GET('EventAPI/GetEmailList?CompanyId=1');
    promise.done((response, textStatus, xhr) => {
        if (xhr.status === 204) {
            return;
        }
        var parsedData = JSON.parse(response)[0];
        $smtpAddress.val(parsedData.SMTPAddress);
        port.value = parsedData.PortNumber;
        emailFrom.value = parsedData.EmailFrom;
        interval.value = parsedData.ResendInterval;
        if (parsedData.EnableSSL) {
            sslRadio.checked = true;
        } else if (parsedData.EnableTLS) {
            tlsRadio.checked = true;
        } else {
            noneRadio.checked = true;
        }
        if (parsedData.Enable) {
            enableCheckBox.checked = true;
        }
    });
    promise.fail((response) => {
        if (403 === response.status) {
            fire(document, 'logout');
        }
    });

    const smtpAddress = ['smtp.gmail.com', 'smtp.mail.yahoo.com', 'smtp.live.com'];
    $smtpAddress.autocomplete({
        source: function (request, response) {
            var keyWord = request.term.trim();
            if (!keyWord) {
                return;
            }
            var source = [];
            smtpAddress.forEach((addr) => {
                if (addr.includes(keyWord)) {
                    source.push(addr);
                }
            });
            response(source);
        },
        select: function (event, ui) {
            port.value = "587";
            tlsRadio.checked = true;
        },
        change: function (event, ui) {
        },
        messages: {
            noResults: '',
            results: function () { }
        },
        focus: function (event, ui) {
            $(".ui-helper-hidden-accessible").empty(); /// pc
        },
        // appendTo: '#smtp-addr'
    });

    let submitHandler = function (evt) {
        successAlert.style.display = 'none';
        failAlert.style.display = 'none';
        var payload = {
            SMTPAddress: $smtpAddress.val(),
            PortNumber: port.value,
            EnableSSL: !!sslRadio.checked,
            EnableTLS: !!tlsRadio.checked,
            EmailFrom: emailFrom.value,
            Password: btoa(passpord.value),
            Enable: !!enableCheckBox.checked,
            ResendInterval: interval.value
        }

        var errorFlag = false;
        if (isEmpty(payload.SMTPAddress)) {
            failAlert.textContent = 'Please enter the SMTP address.';
            errorFlag = true;
        } else if (isEmpty(payload.PortNumber)) {
            failAlert.textContent = 'Please enter the port number.';
            errorFlag = true;
        } else if (isEmpty(payload.EmailFrom)) {
            failAlert.textContent = 'Please enter the email address.';
            errorFlag = true;
        } else if (!isValidEmail(payload.EmailFrom)) {
            failAlert.textContent = 'Please fill in a valid email address.';
            errorFlag = true;
        } else if (isEmpty(payload.Password)) {
            failAlert.textContent = 'Please enter the password.';
            errorFlag = true;
        } else if (isEmpty(payload.ResendInterval)) {
            failAlert.textContent = 'Please enter the event resend interval.';
            errorFlag = true;
        } else if (!isNaturalNumber(payload.ResendInterval)) {
            failAlert.textContent = 'Please fill in the right event resend interval.';
            errorFlag = true;
        }

        if (errorFlag) {
            setTimeout(() => {
                failAlert.style.display = '';
            }, 100);
            return;
        } else {
            let apiHandler = API();
            let promise = apiHandler.PUT(`EventAPI/SetEmail`, payload);

            promise.done(() => {
                successAlert.textContent = 'Email server setup successfully!';
                setTimeout(() => {
                    successAlert.style.display = '';
                }, 100);
            });

            promise.fail((response) => {
                if (403 === response.status) {
                    let errorCode = JSON.parse(response.responseText).ErrorCode;
                    if (errorCode === 0) {
                        fire(document, 'logout');
                    } else if (errorCode === 1) {
                        failAlert.textContent = 'Sorry, you do not have access to setup mail server.';
                    }
                } else {
                    failAlert.textContent = 'Failed to setup email server.';
                }
                setTimeout(() => {
                    failAlert.style.display = '';
                }, 100);
            });
        }
    }
    submitButton.addEventListener('click', submitHandler);


    var openEmailTestModal = function () {
        var failAlert = document.getElementById('send-test-message-fail');
        var successAlert = document.getElementById('send-test-message-success');
        var $modal = $("#emailTestModal");
        var sendButton = document.getElementById('confirm');
        var address = document.getElementById("test-receiving-email-Address");
        var modalBackController = modalBack();

        modalBackController.fade();
        failAlert.style.display = 'none';
        successAlert.style.display = 'none';
        $modal.modal({
            backdrop: false
        });
        $modal.modal("show");
        address.value = "";

        var sendHandler = function () {
            failAlert.style.display = 'none';
            successAlert.style.display = 'none';

            var SMTPSetting = {
                SMTPAddress: $smtpAddress.val(),
                PortNumber: port.value,
                EnableSSL: !!sslRadio.checked,
                EnableTLS: !!tlsRadio.checked,
                EmailFrom: emailFrom.value,
                Password: btoa(passpord.value),
                EmailTo: address.value,
            }

            var errorFlag = false;

            if (isEmpty(SMTPSetting.EmailTo)) {
                errorFlag = true;
                failAlert.textContent = 'Please enter the receiving email.';
            }
            else if (!isValidEmail(SMTPSetting.EmailTo)) {
                errorFlag = true;
                failAlert.textContent = 'Please fill in a valid email address.';
            } else if (isEmpty(SMTPSetting.SMTPAddress)) {
                failAlert.textContent = 'Please enter the SMTP address.';
                errorFlag = true;
            } else if (isEmpty(SMTPSetting.PortNumber)) {
                failAlert.textContent = 'Please enter the port number.';
                errorFlag = true;
            } else if (!isValidEmail(SMTPSetting.EmailFrom)) {
                failAlert.textContent = 'Please fill in a valid email address.';
                errorFlag = true;
            } else if (isEmpty(SMTPSetting.EmailFrom)) {
                failAlert.textContent = 'Please enter the email address.';
                errorFlag = true;
            } else if (isEmpty(SMTPSetting.Password)) {
                failAlert.textContent = 'Please enter the password.';
                errorFlag = true;
            }

            if (errorFlag) {
                setTimeout(() => {
                    failAlert.style.display = '';
                }, 100);
                return;
            } else {
                $('body').addClass('loading');
                let apiHandler = API();
                let promise = apiHandler.POST(`EventAPI/EmailTest`, SMTPSetting);

                promise.done(() => {
                    successAlert.textContent = 'Please check your inbox for the test email, close this dialog and then submit mail server settings.';
                    setTimeout(() => {
                        successAlert.style.display = '';
                    }, 100);
                    $('body').removeClass('loading');
                });

                promise.fail((response) => {
                    if (403 === response.status) {
                        fire(document, 'logout');
                    } else if (504 == response.status) {
                        failAlert.textContent = 'Request timed out. Please check your mail server settings and try again.';
                    } else {
                        failAlert.textContent = 'Send email failed! Please check the mail server settings and the receiving email address.'
                    }
                    setTimeout(() => {
                        failAlert.style.display = '';
                    }, 100);
                    $('body').removeClass('loading');
                });
            }
        };
        sendButton.addEventListener('click', sendHandler);
        $modal.on('hidden.bs.modal', function () {
            sendButton.removeEventListener('click', sendHandler);
            modalBackController.removeFade();
            $modal.off();
        });
    }
    sendTestButton.addEventListener('click', openEmailTestModal);
});