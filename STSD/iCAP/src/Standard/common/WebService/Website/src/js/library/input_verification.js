export default function inputVerification() {
  function isValidEmail(email) {
    // eslint-disable-next-line no-useless-escape
    if (email.search(/^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z]+$/) === -1) { return false; }
    return true;
  }

  function isNaturalNumber(number) {
    const r = /^\+?[1-9][0-9]*$/;
    return r.test(number);
  }

  function isEmpty(data) {
    if (data === '' || data === undefined || data === null) { return true; }
    return false;
  }

  function fieldsAreEmpty(className) {
    let emptyFlag = true;
    $(`.${className}`).each((idx, field) => {
      if (isEmpty($(field).val()) === false) {
        emptyFlag = false;
        return false;
      }
    });
    return emptyFlag;
  }

  function numberFieldIsEmpty(number) {
    if (number === 0 || number === undefined || number === null) { return true; }
    return false;
  }

  function isValidPassword(password, verifyPassword) {
    return Boolean(password === verifyPassword);
  }

  function isValidLongitude(longitude) {
    if (Number(longitude) >= -180.0 && Number(longitude) <= 180.0) { return true; }
    return false;
  }

  function isValidLatitude(latitude) {
    if (Number(latitude) >= -90.0 && Number(latitude) <= 90.0) { return true; }
    return false;
  }

  function isGoogleSMTP(address) {
    if (!~address.indexOf('gmail')) { return false; }
    return true;
  }

  function isYahooSMTP(address) {
    if (!~address.indexOf('yahoo')) { return false; }
    return true;
  }

  function isHotmailSMTP(address) {
    if (!~address.indexOf('smtp.live.com')) { return false; }
    return true;
  }

  function isEmptyArray(array) {
    if (array.length === 0) { return true; }
    return false;
  }

  function isValidLength(str, limitLength) {
    if (str.length > limitLength) { return false; }
    return true;
  }

  function nameExists(optionName, newName) {
    // const index = $.map($('#' + selectId + ' option'), function(item, index){
    //     return item.text;
    // }).indexOf(newName);
    const index = optionName.indexOf(newName);

    if (index !== -1) {
      return true;
    }
    return false;
  }

  function isValidateIPaddress(ipaddress) {
    if (/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(ipaddress)) {
      return (true);
    }
    return (false);
  }

  return {
    isValidEmail,
    isNaturalNumber,
    isEmpty,
    fieldsAreEmpty,
    numberFieldIsEmpty,
    isValidPassword,
    isValidLongitude,
    isValidLatitude,
    isGoogleSMTP,
    isYahooSMTP,
    isHotmailSMTP,
    isEmptyArray,
    isValidLength,
    nameExists,
    isValidateIPaddress,
  };
}