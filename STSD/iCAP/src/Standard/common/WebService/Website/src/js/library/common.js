export function findIndexInArray(data, property, value) {
    for (var i = 0, l = data.length; i < l; i++) {
        if (data[i][property] === value) {
            return i;
        }
    }
    return -1;
}

//ruleArray : [{prop: , direction: 1}]
export function sortMarker(dataArray, ruleArray) {
    dataArray.sort(function (a, b) {
        let i = 0, result = 0;
        while (i < ruleArray.length && result === 0) {
            result = ruleArray[i].direction * (a.position[ruleArray[i].prop].toString() < b.position[ruleArray[i].prop].toString() ? -1 : (a.position[ruleArray[i].prop].toString() > b.position[ruleArray[i].prop].toString() ? 1 : 0));
            i++;
        }
        return result;
    });
}

export function replaceCharacter(str) {
    return str.replace(/</g, "&lt;").replace(/>/g, "&gt;");
}

export function removeElement(element) {
    while (element.hasChildNodes()) {
        element.removeChild(element.firstChild);
    }
}

export function timeConverter(unixTimestamp) {
    let time = new Date(unixTimestamp * 1000);
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

export function alert(element) {
    function show(element) {
      setTimeout(() => {
        element.classList.remove('hide');
      }, 100);
    }
  
    function init(element) {
      if (element.classList.contains('alert-danger')) {
        element.classList.remove('alert-danger');
      } else if (element.classList.contains('alert-success')) {
        element.classList.remove('alert-success');
      }
  
      if (!element.classList.contains('hide')) {
        element.classList.add('hide');
      }
    }
    return {
      show,
      init,
    };
  }

export function getSingle(fn){
  let result;

  return function(){
    return result || (result = fn.apply(this, arguments));
  }
}

export function delay(fn, time){
  setTimeout(fn, time);
}

function setAttributes(el, attrs) {
  for(var key in attrs) {
    el.setAttribute(key, attrs[key]);
  }
}

export function createIndexBtn({ooblist, onclick}){
  const index_btn = document.getElementById("btn-indx-innoage");
  index_btn.innerHTML = '';
  
  const fragment = document.createDocumentFragment();
  
  for (let i=0; i <ooblist.length; i++){
      const btn = document.createElement("button"); 
      btn.setAttribute("class", "btn btn-sm btn-dark");
      btn.textContent = i;
      btn.onclick = function(){
        onclick(i);
      }
      fragment.appendChild(btn);
  }
  onclick(0);
  return index_btn.appendChild(fragment);
} 

export function createBody(data, sn){

  const fragment = document.createDocumentFragment();
  
  let tr = document.createElement('tr');

  tr.innerHTML = `
    <td>
      <b>Serial Number</b>
    </td>
    <td>${sn}</td>
  `
  fragment.appendChild(tr);

  for (let name in data){
    tr = document.createElement('tr');

    tr.innerHTML = `
      <td>
        <b>${name}</b>
      </td>
      <td>${data[name]}</td>
    `
    fragment.appendChild(tr);
  }

  return fragment;
}