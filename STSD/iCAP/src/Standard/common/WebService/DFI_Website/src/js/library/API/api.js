import { RETURN_CODE } from './return_code';
import { fire } from '../../library/event_handler';

const API = {
  handleError:{
    403:{
      [RETURN_CODE.TOKEN_ERROR]:function(){
        fire(document, 'logout');
      },
      [RETURN_CODE.PERMISSION_DENIED]: function({dom, target}){
        dom.showFailedMsg(`Sorry, you do not have access to update ${target}.`);
      }
    }
  }
}

API.get = async function({url, success, fail, global = true, timeout = 60000}){
   return $.ajax({
    type: 'GET',
    url: url,
    async: true,
    crossDomain: true,
    headers: {
        'token': $.cookie('token')
    },
    global: global,
    timeout: timeout,
    success: function (response) {
      if (typeof success === 'function') success(response);
    },
    error: function (response) {
      if (typeof fail === 'function') fail(response);

      if (API.handleError[response.status] && typeof API.handleError[response.status][response.responseJSON.Response] === 'function') {
        API.handleError[response.status][response.responseJSON.Response]()
      }
    }
  });
};

API.patch = function({url, payload, success, fail}){
  return $.ajax({
    type: 'patch',
    url: url,
    async: true,
    crossDomain: true,
    headers: {
        'token': $.cookie('token'),
        "Content-Type": "application/json"
    },
    processData: false,
    data: JSON.stringify(payload),
    global: true,
    success: function (response) {
      if (typeof success === 'function') success(response);
    },
    error: function (response) {
      if(typeof fail === 'function' && response.responseJSON) 
        fail(API.handleError[response.status][response.responseJSON.Response]);
      else
        fail();
    }
  });
};

API.post = function({url, payload, success, fail, global, complete}){
  return $.ajax({
    type: 'post',
    url: url,
    async: true,
    crossDomain: true,
    headers: {
        'token': $.cookie('token'),
        "Content-Type": "application/json"
    },
    // timeout: 1000,
    processData: false,
    data: JSON.stringify(payload),
    global: (global === undefined)? true: global,
    success: function (response) {
      if (typeof success === 'function') success(response);
    },
    error: function (response) {
      if(typeof fail === 'function') {
        fail();
      } else {
        API.handleError[response.status][response.responseJSON.Response]
      }
    },
    complete
  });
};


export default API;