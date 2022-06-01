import '../../css/setting_customized_map.css';
import { modalBack } from "../library/modal";
import {alert} from '../library/common';
import inputVerification from '../library/input_verification';
import MARKER_SAVED from '../../assets/images/marker_saved.png';
import MARKER_SAVED_SELECTED from '../../assets/images/marker_saved_selected.png';
import MARKER_UNSAVED_SELECTED from '../../assets/images/marker_unsaved_selected.png';
import MARKER_UNSAVED from '../../assets/images/marker_unsaved.png';
import { fire } from '../library/event_handler';

(function IIFE() {
    function reloadSettingCustomizedMap (){
        const doc = document;
        const $body = $("body");
        $body.addClass("loading");
        const apiGetDevics = $.ajax({
            type: 'GET',
            url: '/DeviceAPI/Devices/List',
            async: true,
            crossDomain: true,
            headers: {
              token: $.cookie('token'),
            },
            processData: false,
          });
          apiGetDevics.done((response) => {
            const deviceList = (response) || [];
            $(doc).ready(() => {
              const $select = $('#maplist');
              const btns = doc.querySelectorAll('#map-setting button');
              const imgMap = doc.querySelector('#map img');
              const divMap = doc.getElementById('map');
              const inputMarkerName = doc.getElementById('name-marker');
              const state = {
                selected: -1,
                markers: [],
                menu: 0, // 0: left, 1: right
                isOpen: false,
                area: {
                  right: 0,
                  left: -15, //  0 - marker_width / 2
                  top: -28, // 0 - marker_height - 2
                  bottom: 0,
                },
              };
              class Select {
                constructor($select) {
                  this.$select = $select;
                }
        
                init(options) {
                  options.forEach(item => {this.$select.append($('<option></option>').attr('value', item.Id).text((item.Alias) ? item.Alias : item.Name))});
                  // forEach(options, (item) => {
                  //   this.$select.append($('<option></option>').attr('value', item.Id).text(item.Name));
                  // });
                  this.$select.selectpicker();
                }
        
                initWidthIndexValue(options) {
                  options.forEach((item, idx) => {this.$select.append($('<option></option>').attr('value', idx).text(item.Name))});
                  // forEach(options, (item, idx) => {
                  //   this.$select.append($('<option></option>').attr('value', idx).text(item.Name));
                  // });
                  this.$select.selectpicker();
                }
        
                addEvent(type, callBack) {
                  this.$select.on(type, callBack);
                }
        
                setSelectedArray(optionIds) {
                  this.$select.find(':selected').each((idx, option) => {
                    option.selected = false;
                  });
                  optionIds && optionIds.forEach(id => {this.$select.find(`option[value=${id}]`).prop('selected', true)});
                  // forEach(optionIds, (id) => {
                  //   this.$select.find(`option[value=${id}]`).prop('selected', true);
                  // });
                  this.$select.selectpicker('refresh');
                }
        
                setSelected(id) {
                  this.$select.find(':selected').each((idx, option) => {
                    option.selected = false;
                  });
                  this.$select.find(`option[value=${id}]`).prop('selected', true);
                  this.$select.selectpicker('refresh');
                }
        
                // addOptionHTML(optionHTML){
                //   this.$select.append(optionHTML);
                //   this.$select.selectpicker('refresh');
                // }
                addOptElement(optionElement) {
                  this.$select.append(optionElement);
                  this.$select.selectpicker('refresh');
                }
        
                removeSelected() {
                  this.$select.find(':selected').each((idx, option) => {
                    option.selected = false;
                  });
                }
        
                removeOpt(id) {
                  this.$select.find(`option[value=${id}]`).remove();
                  this.$select.selectpicker('refresh');
                }
        
                removeOptClass(id, className) {
                  const opt = this.$select.find(`option[value=${id}]`)[0];
                  if (opt.classList.contains(className)) {
                    opt.classList.remove(className);
                  }
                  this.$select.selectpicker('refresh');
                }
        
                changeOptName(id, name) {
                  const opt = this.$select.find(`option[value=${id}]`)[0];
                  opt.textContent = name;
                  opt.className = 'unsaved';
                  this.$select.selectpicker('refresh');
                }
              }
              const $deviceSelect = new Select($('#device-list'));
              $deviceSelect.init(deviceList);
              $deviceSelect.addEvent('change', () => {
                const $options = $deviceSelect.$select.find('option:selected');
                const tmpArr = [];
                const target = state.markers[state.selected];

                $options.each((idx, option) => {
                  tmpArr.push(Number(option.value));
                });
                // $options.forEach(option => {
                //   tmpArr.push(Number(option.value));
                // });
                // forEach(options, (option) => {
                //   tmpArr.push(Number(option.value));
                // });
                target.props.devicelist = tmpArr;
                target.props.isSaved = false;
                target.marker.querySelector('img').src = MARKER_UNSAVED_SELECTED;
              });
              const initMapSelect = (callBack) => {
                const apiGetMaps = $.ajax({
                  type: 'GET',
                  url: '/SettingAPI/CustomizedMap',
                  async: true,
                  crossDomain: true,
                  headers: {
                    token: $.cookie('token'),
                  },
                  processData: false,
                });
                apiGetMaps.done((response, textStatus, xhr) => {
                  $(doc).ready(() => {
                    if (xhr.status !== 204) {
                      response.List.forEach(item => {
                        $select.append($('<option></option>').attr('value', item.Id).text(item.Name));
                      });
                      // forEach(response.List, (item) => {
                      //   $select.append($('<option></option>').attr('value', item.Id).text(item.Name));
                      // });
                    }
                    $select.selectpicker('refresh');
                    if (typeof callBack === 'function') {
                      callBack();
                    }
                  });
                });
                apiGetMaps.fail((response) => {
                  if (response.status === 403
                    && response.responseJSON.Response === RETURN_CODE.TOKEN_ERROR) {
                    fire(document, 'logout');
                  }
                });
              };
              $select.selectpicker();
              initMapSelect();
              let uploadImg;
              function ekUpload(callBack) {
                const fileSelect = doc.getElementById('file-upload');
                const fileDrag = doc.getElementById('file-drag');
                const eleStart = doc.getElementById('start');
                const eleResponse = doc.getElementById('response');
                const eleNotimage = doc.getElementById('notimage');
                const eleFileImage = doc.getElementById('file-image');
                const eleForm = doc.getElementById('file-upload-form');
                function fileDragHover(e) {
                  e.stopPropagation();
                  e.preventDefault();
                  fileDrag.className = (e.type === 'dragover' ? 'hover' : 'modal-body file-upload');
                }
                function fileSelectHandler(e) {
                  // Fetch FileList object
                  const alertHandler = alert();
                  const modalAlert = doc.getElementById('alert-modal');
                  alertHandler.init(modalAlert);
                  const files = e.target.files || e.dataTransfer.files;
                  if (files.length === 0) return;
                  // Cancel event and hover styling
                  fileDragHover(e);
                  // Process all File objects
                  function parseFile(file) {
                    // Output
                    function output(msg) {
                      // Response
                      const m = doc.getElementById('messages');
                      m.innerHTML = msg;
                    }
                    output(
                      `<strong>${encodeURI(file.name)}</strong>`,
                    );
        
                    const isGood = (/\/(?=gif|jpg|png|jpeg)/gi).test(file.type);
                    if (isGood) {
                      eleStart.classList.add('hidden');
                      eleResponse.classList.remove('hidden');
                      eleNotimage.classList.add('hidden');
                      // Thumbnail Preview
                      eleFileImage.classList.remove('hidden');
                      eleFileImage.src = URL.createObjectURL(file);
                      uploadImg = file;
                      if (typeof callBack === 'function') {
                        callBack();
                      }
                    } else {
                      eleFileImage.classList.add('hidden');
                      eleNotimage.classList.remove('hidden');
                      eleStart.classList.remove('hidden');
                      eleResponse.classList.add('hidden');
                      eleForm.reset();
                      uploadImg = undefined;
                    }
                  }
                  parseFile(files[0]);
                }
                function init() {
                  fileSelect.addEventListener('change', fileSelectHandler, false);
                  fileDrag.addEventListener('dragover', fileDragHover, false);
                  fileDrag.addEventListener('dragleave', fileDragHover, false);
                  fileDrag.addEventListener('drop', fileSelectHandler, false);
                }
                // Check for the various File API support.
                if (window.File && window.FileList && window.FileReader) {
                  init();
                } else {
                  doc.getElementById('file-drag').style.display = 'none';
                }
                return {
                  unListen() {
                    eleFileImage.classList.add('hidden');
                    eleNotimage.classList.remove('hidden');
                    eleStart.classList.remove('hidden');
                    eleResponse.classList.add('hidden');
                    eleForm.reset();
                    fileSelect.removeEventListener('change', fileSelectHandler, false);
                    fileDrag.removeEventListener('dragover', fileDragHover, false);
                    fileDrag.removeEventListener('dragleave', fileDragHover, false);
                    fileDrag.removeEventListener('drop', fileSelectHandler, false);
                  },
                };
              }
              const $markerSelect = new Select($('#marker-list'));
              function handleMakerSelectChange() {
                state.selected = $markerSelect.$select.val();
                state.markers[$markerSelect.$select.val()].handleClick();
              }
              $markerSelect.addEvent('change', handleMakerSelectChange);
        
              const markerHandler = (function () {
                function removeAllMarkers() {
                  const menuLeft = doc.getElementById('marker-bar-left');
                  const menuRight = doc.getElementById('marker-bar-right');
                  if (state.markers.length === 0) return;
                  state.markers.forEach(marker => {
                    if (marker) {
                      marker.remove();
                    }
                  });
                  // forEach(state.markers, (marker) => {
                  //   if (marker) {
                  //     marker.remove();
                  //   }
                  // });
                  state.markers.length = 0;
                  state.selected = -1;
                  if (state.isOpen) {
                    if (state.menu === 0) {
                      menuLeft.classList.remove('active');
                    } else if (state.menu === 1) {
                      menuRight.classList.remove('active');
                    }
                    state.isOpen = false;
                  }
                }
                function removeAllMarkersInDB() {
                  const isSavedInDB = state.markers.some((x) => x.props.guid !== undefined);
                  if (isSavedInDB) {
                    const apiDeleteAllMarkers = $.ajax({
                      type: 'DELETE',
                      url: `/SettingAPI/CustomizedMap/${$select.val()}/Markers`,
                      async: true,
                      crossDomain: true,
                      headers: {
                        token: $.cookie('token'),
                      },
                    });
                    apiDeleteAllMarkers.done(() => {
                      removeAllMarkers();
                    });
        
                    apiDeleteAllMarkers.fail((response) => {
                      const alertHandler = alert();
                      const eleAlert = doc.getElementById('alert');
        
                      if (response.status === 403) {
                        if (response.responseJSON.Response === RETURN_CODE.TOKEN_ERROR) {
                          fire(document, 'logout');
                        } else if (response.responseJSON.Response === RETURN_CODE.PERMISSION_DENIED) {
                          eleAlert.textContent = 'Sorry, you do not have access to remove markers.';
                          eleAlert.classList.add('alert-danger');
                          alertHandler.show(eleAlert);
                        }
                      } else {
                        eleAlert.textContent = 'Remove markers failed.';
                        eleAlert.classList.add('alert-danger');
                        alertHandler.show(eleAlert);
                      }
                    });
                  } else {
                    removeAllMarkers();
                  }
                }
                return {
                  removeAllMarkers,
                  removeAllMarkersInDB,
                };
              }());
              function showMarkerSettingBar(marker) {
                const body = doc.getElementById('widget-body');
                const menuLeft = doc.getElementById('marker-bar-left');
                const menuRight = doc.getElementById('marker-bar-right');
                if (marker.x < body.clientWidth / 2) {
                  if (state.menu === 0) {
                    while (menuLeft.hasChildNodes()) {
                      menuRight.appendChild(menuLeft.firstChild);
                    }
                    state.menu = 1;
                  }
                  menuRight.classList.add('active');
                } else {
                  if (state.menu === 1) {
                    while (menuRight.hasChildNodes()) {
                      menuLeft.appendChild(menuRight.firstChild);
                    }
                    state.menu = 0;
                  }
                  menuLeft.classList.add('active');
                }
              }
              class Marker {
                constructor(marker, props) {
                  this.marker = marker;
                  this.props = props;
                  this.handleClick = () => {
                    if (!this.props.devicelist) {
                      const apiGetDevices = $.ajax({
                        type: 'GET',
                        url: `/SettingAPI/CustomizedMap/Marker/${this.props.guid}`,
                        async: true,
                        crossDomain: true,
                        headers: {
                          token: $.cookie('token'),
                        },
                        processData: false,
                        global: false,
                      });
        
                      apiGetDevices.done((response) => {
                        this.props.devicelist = response.devices;
                        $deviceSelect.setSelectedArray(this.props.devicelist);
                      });
        
                      apiGetDevices.fail((response) => {
                        if (response.status === 403
                          && response.responseJSON.Response === RETURN_CODE.TOKEN_ERROR) {
                          fire(document, 'logout');
                        }
                      });
                    }
        
                    const result = doc.getElementById('result');
                    result.className = 'hide';
                    if (state.selected !== this.props.index) {
                      inputMarkerName.value = (this.props.name) ? this.props.name : '';
                      $deviceSelect.setSelectedArray(this.props.devicelist);
                      state.selected = this.props.index;
                      state.markers.forEach(item =>{
                        if (!item) return;
                        if (item.props.isSaved) {
                          item.marker.innerHTML = `<img src=${MARKER_SAVED}>`;
                        } else {
                          item.marker.innerHTML = `<img src=${MARKER_UNSAVED}>`;
                        }
                      });
                      // forEach(state.markers, (item) => {
                      //   if (!item) return;
                      //   if (item.props.isSaved) {
                      //     item.marker.innerHTML = `<img src=${MARKER_SAVED}>`;
                      //   } else {
                      //     item.marker.innerHTML = `<img src=${MARKER_UNSAVED}>`;
                      //   }
                      // });
                      if (this.props.isSaved) {
                        this.marker.innerHTML = `<img src=${MARKER_SAVED_SELECTED}>`;
                      } else {
                        this.marker.innerHTML = `<img src=${MARKER_UNSAVED_SELECTED}>`;
                      }
                      $markerSelect.setSelected(this.props.index);
                    }
                    if (!state.isOpen) {
                      showMarkerSettingBar(this.marker);
                      state.isOpen = true;
                    }
                  };
                  marker.index = this.props.index;
                  marker.addEventListener('click', this.handleClick);
                }
        
                remove() {
                  this.marker.removeEventListener('click', this.handleClick);
                  this.marker.parentElement.removeChild(this.marker);
                }
              }
              function reloadMap(id) {
                const apiGetMap = $.ajax({
                  type: 'GET',
                  url: `/SettingAPI/CustomizedMap/${id}`,
                  async: true,
                  crossDomain: true,
                  headers: {
                    token: $.cookie('token'),
                  },
                });
                apiGetMap.done((response) => {
                  if (response) {
                    $markerSelect.initWidthIndexValue(response.Markers);
                    imgMap.src = response.Image;
                    imgMap.style.display = 'block';
                    $(doc).ready(() => {
                      state.area.right = imgMap.width - 15;
                      state.area.bottom = imgMap.height - 30;
                      response.Markers.forEach((marker, idx) => {
                        const div = doc.createElement('div');
                        div.classList.add('marker');
                        div.innerHTML = `<img src=${MARKER_SAVED}>`;
                        div.style.position = 'absolute';
                        div.style.top = `${marker.OffsetY}px`;
                        div.style.left = `${marker.OffsetX}px`;
                        div.x = marker.OffsetX;
                        div.y = marker.OffsetY;
                        state.markers.push(new Marker(div, {
                          name: marker.Name,
                          isSaved: true,
                          index: idx,
                          devicelist: undefined,
                          guid: marker.Guid,
                        }));
                        divMap.appendChild(div);
                      });
                      $body.removeClass("loading");
                    });
                  }
                });
                apiGetMap.fail((response) => {
                  if (response.status === 403
                    && response.responseJSON.Response === RETURN_CODE.TOKEN_ERROR) {
                    fire(document, 'logout');
                  }
                });
              }
              const btnComponents = [
                {
                  element: btns[0],
                  handleClick() {
                    const $modal = $('#modal');
                    const eleModalAlert = doc.getElementById('alert-modal');
                    const btnList = $modal.find('.modal-footer');
                    const inputName = doc.getElementById('map-name');
                    const title = doc.getElementById('title-modal');
        
                    const modalHandler = modalBack();
                    const alertHandler = alert();
                    title.textContent = 'Customized Map';
                    alertHandler.init(eleModalAlert);
                    const upload = ekUpload();
                    const handleSubmitClick = async function () {
                      const inputVerify = inputVerification();
                      const fileSizeLimit = 1024; // In MB
                      alertHandler.init(eleModalAlert);
        
                      if (inputVerify.isEmpty(inputName.value)) {
                        eleModalAlert.classList.add('alert-danger');
                        eleModalAlert.textContent = 'Please fill in the name.';
                        alertHandler.show(eleModalAlert);
                      } else if (!inputVerify.isValidLength(inputName.value, 15)) {
                        eleModalAlert.classList.add('alert-danger');
                        eleModalAlert.textContent = "Sorry, your map's name must be within 15 characters long.";
                        alertHandler.show(eleModalAlert);
                      } else if (!uploadImg) {
                        eleModalAlert.classList.add('alert-danger');
                        eleModalAlert.textContent = 'Sorry, you must upload one image.';
                        alertHandler.show(eleModalAlert);
                      } else if (uploadImg.size >= fileSizeLimit * 1024 * 1024) {
                        eleModalAlert.classList.add('alert-danger');
                        eleModalAlert.textContent = `Please upload a smaller file (< ${fileSizeLimit} MB).`;
                        alertHandler.show(eleModalAlert);
                      } else {
                        // const fileInput = doc.getElementById('class-roster-file');
                        const pBar = doc.getElementById('file-progress');
                        const form = new FormData();
                        form.append('File', uploadImg);
                        form.append('Name', inputName.value);
                        pBar.style.display = 'inline';
                        const xhr = new XMLHttpRequest();
                        const setProgressMaxValue = function (e) {
                          // const pBar = doc.getElementById('file-progress');
                          if (e.lengthComputable) {
                            pBar.max = e.total;
                          }
                        };
                        xhr.upload.addEventListener('loadstart', setProgressMaxValue, false);
                        const updateFileProgress = function (e) {
                          // const pBar = doc.getElementById('file-progress');
                          if (e.lengthComputable) {
                            pBar.value = e.loaded;
                          }
                        };
                        xhr.upload.addEventListener('progress', updateFileProgress, false);
                        xhr.onreadystatechange = function () {
                          if (xhr.readyState === 4) {
                            // Everything is good!
                            if (xhr.status > 400) {
                              if (xhr.status === 403) {
                                const returnCode = JSON.parse(xhr.response).Response;
                                if (returnCode === RETURN_CODE.TOKEN_ERROR) {
                                  fire(document, 'logout');
                                  history.push('/login', {state: 'normal'});
            
                                } else if (returnCode === RETURN_CODE.PERMISSION_DENIED) {
                                  eleModalAlert.textContent = 'Sorry, you do not have access to customize map.';
                                  eleModalAlert.classList.add('alert-danger');
                                  alertHandler.show(eleModalAlert);
                                }
                              } else {
                                eleModalAlert.textContent = 'Failed.';
                                eleModalAlert.classList.add('alert-danger');
                                alertHandler.show(eleModalAlert);
                              }
                            } else if (xhr.status > 200 && xhr.status < 300) {
                              $select.empty();
                              const callBack = function () {
                                $(doc).ready(() => {
                                  const lastValue = $select.find('option').last().val();
                                  $select.val(lastValue);
                                  $select.selectpicker('refresh');
                                  $select.trigger('change');// //to do
                                  $modal.modal('hide');
                                });
                              };
                              initMapSelect(callBack);
                            }
                            // progress.className = (xhr.status == 200 ? "success" : "failure");
                            // doc.location.reload(true);
                          }
                        };
                        // Start upload
                        xhr.open('POST', '/SettingAPI/CustomizedMap', true);
                        xhr.setRequestHeader('token', $.cookie('token'));
                        xhr.send(form);
                      }
                    };
                    const btnSubmit = doc.createElement('button');
                    btnSubmit.classList.add('btn');
                    btnSubmit.classList.add('btn-sm');
                    btnSubmit.classList.add('btn-dark');//btn btn-sm btn-dark
                    btnSubmit.addEventListener('click', handleSubmitClick);
                    btnSubmit.innerHTML = `<span class="glyphicon glyphicon-send"></i></span> 
                                              <span>Submit</span>`;
                    btnList.append(btnSubmit);
                    modalHandler.fade();
                    $modal.on('hidden.bs.modal', () => {
                      const pBar = doc.getElementById('file-progress');
                      pBar.value = 0;
                      inputName.value = '';
                      modalHandler.removeFade();
                      doc.getElementById('file-image').classList.add('hidden');
                      doc.getElementById('notimage').classList.remove('hidden');
                      doc.getElementById('start').classList.remove('hidden');
                      doc.getElementById('response').classList.add('hidden');
                      doc.getElementById('file-upload-form').reset();
                      uploadImg = undefined;
                      upload.unListen();
                      btnSubmit.removeEventListener('click', handleSubmitClick);
                      btnSubmit.parentNode.removeChild(btnSubmit);
                      $modal.off('hidden.bs.modal');
                    });
                    $modal.modal({
                      backdrop: false,
                    });
                    $modal.modal('show');
                  },
                },
                {
                  element: btns[1],
                  handleClick() {
                    const alertHandler = alert();
                    const eleAlert = doc.getElementById('alert');
                    const eleModalAlert = doc.getElementById('alert-modal');
                    const id = $select.val();
                    alertHandler.init(eleAlert);
                    alertHandler.init(eleModalAlert);
                    // alertHandler.init(eleModalAlert);
                    if (id) {
                      const $modal = $('#modal');
                      const btnList = $modal.find('.modal-footer')[0];
                      const inputName = doc.getElementById('map-name');
                      const title = doc.getElementById('title-modal');
                      const modalHandler = modalBack();
                      // const alertHandler = alert();
                      const btnOk = doc.createElement('button');
                      const btnApply = doc.createElement('button');
                      const inputVerify = inputVerification();
                      const fileSizeLimit = 1024; // In MB
                      title.textContent = 'Edit Map';
                      inputName.value = $select.find('option:selected').text();
                      btnOk.classList.add('btn');
                      btnOk.classList.add('btn-sm');
                      btnOk.classList.add('btn-dark');
                      btnOk.innerHTML = `<span class="glyphicon glyphicon-ok"></span> 
                                            <span>Ok</span>`;
                      btnApply.disabled = true;
                      btnApply.classList.add('btn');
                      btnApply.classList.add('btn-sm');
                      btnApply.classList.add('btn-dark');
                      btnApply.innerHTML = `<span class="glyphicon glyphicon-pencil"></span> 
                                              <span>Apply</span>`;
        
        
                      const handleApply = function (callBack) {
                        const modalAlert = doc.getElementById('alert-modal');
                        alertHandler.init(modalAlert);
                        if (inputVerify.isEmpty(inputName.value) === true) {
                          modalAlert.classList.add('alert-danger');
                          modalAlert.textContent = 'Please fill in the name.';
                          alertHandler.show(modalAlert);
                        } else if (inputVerify.isValidLength(inputName.value, 15) === false) {
                          modalAlert.classList.add('alert-danger');
                          modalAlert.textContent = "Sorry, your map's name must be within 15 characters long.";
                          alertHandler.show(modalAlert);
                        } else if (uploadImg && uploadImg.size >= fileSizeLimit * 1024 * 1024) {
                          eleModalAlert.classList.add('alert-danger');
                          eleModalAlert.textContent = `Please upload a smaller file (< ${fileSizeLimit} MB).`;
                          alertHandler.show(eleModalAlert);
                        } else {
                          const pBar = doc.getElementById('file-progress');
                          const form = new FormData();
                          form.append('File', uploadImg);
                          form.append('Name', inputName.value);
                          pBar.style.display = 'inline';
                          const xhr = new XMLHttpRequest();
                          const setProgressMaxValue = function (e) {
                            // const pBar = doc.getElementById('file-progress');
                            if (e.lengthComputable) {
                              pBar.max = e.total;
                            }
                          };
                          xhr.upload.addEventListener('loadstart', setProgressMaxValue, false);
                          const updateFileProgress = function (e) {
                            // const pBar = doc.getElementById('file-progress');
                            if (e.lengthComputable) {
                              pBar.value = e.loaded;
                            }
                          };
                          xhr.upload.addEventListener('progress', updateFileProgress, false);
                          xhr.onreadystatechange = function () {
                            if (xhr.readyState === 4) {
                              // Everything is good!
                              if (xhr.status > 400) {
                                if (xhr.status === 403) {
                                  const returnCode = JSON.parse(xhr.response).Response;
                                  if (returnCode === RETURN_CODE.TOKEN_ERROR) {
                                    fire(document, 'logout');
              
                                  } else if (returnCode === RETURN_CODE.PERMISSION_DENIED) {
                                    eleModalAlert.textContent = 'Sorry, you do not have access to customize map.';
                                    eleModalAlert.classList.add('alert-danger');
                                    alertHandler.show(eleModalAlert);
                                  }
                                } else {
                                  eleModalAlert.textContent = 'Failed.';
                                  eleModalAlert.classList.add('alert-danger');
                                  alertHandler.show(eleModalAlert);
                                }
                              } else if (xhr.status > 200 && xhr.status < 300) {
                                const optionSelected = $select.find('option:selected');
                                optionSelected.text(inputName.value);
                                $select.selectpicker('refresh');
                                if (uploadImg) {
                                  // $select.trigger('change');
                                  uploadImg = undefined;
                                  // remove all markers
                                  markerHandler.removeAllMarkersInDB();
                                  reloadMap(id);
                                }
                                if (typeof callBack === 'function') {
                                  callBack();
                                } else {
                                  eleModalAlert.textContent = 'Apply successfully';
                                  eleModalAlert.classList.add('alert-success');
                                  alertHandler.show(eleModalAlert);
                                }
                              }
                              // progress.className = (xhr.status == 200 ? "success" : "failure");
                              // doc.location.reload(true);
                            }
                          };
                          // Start upload
                          xhr.open('PUT', `/SettingAPI/CustomizedMap/${id}`, true);
                          xhr.setRequestHeader('token', $.cookie('token'));
                          xhr.send(form);
                        }
                      };
                      const handleOk = handleApply.bind(null, () => {
                        $modal.modal('hide');
                      });
                      const handleKeydown = function () {
                        const allowedCode = [8, 32, 46, 37, 39];
                        // 8: BackSpace; 32: space;  46: Delete;
                        const charCode = (event.charCode)
                          ? event.charCode : ((event.keyCode) ? event.keyCode
                            : ((event.which) ? event.which : 0));
                        if (charCode > 31 && (charCode < 64 || charCode > 90)
                              && (charCode < 97 || charCode > 122)
                              && (charCode < 48 || charCode > 57)
                              && (allowedCode.indexOf(charCode) === -1)) {
                          event.preventDefault();
                          return false;
                        }
                        if (charCode === 37 || charCode === 39) {
                          return true;
                        }
                        btnApply.disabled = false;
                        inputName.removeEventListener('keydown', handleKeydown);
                        btnApply.addEventListener('click', handleApply);
                      };
                      const upload = ekUpload(() => {
                        if (btnApply.disabled) {
                          btnApply.disabled = false;
                          inputName.removeEventListener('keydown', handleKeydown);
                          btnApply.addEventListener('click', handleApply);
                        }
                      });
                      inputName.addEventListener('keydown', handleKeydown);
                      btnOk.addEventListener('click', handleOk);
                      btnList.appendChild(btnApply);
                      btnList.insertAdjacentElement('afterbegin', btnOk);
        
                      $modal.on('hidden.bs.modal', () => {
                        const pBar = doc.getElementById('file-progress');
                        pBar.value = 0;
                        modalHandler.removeFade();
                        upload.unListen();
                        inputName.value = '';
                        inputName.removeEventListener('keydown', handleKeydown);
                        btnApply.removeEventListener('click', handleApply);
                        btnApply.parentNode.removeChild(btnApply);
                        btnOk.removeEventListener('click', handleOk);
                        btnOk.parentNode.removeChild(btnOk);
                        $modal.off('hidden.bs.modal');
                      });
                      modalHandler.fade();
                      $modal.modal({
                        backdrop: false,
                      });
                      $modal.modal('show');
                    } else {
                      eleAlert.classList.add('alert-danger');
                      eleAlert.textContent = 'Please select one map.';
                      alertHandler.show(eleAlert);
                    }
                  },
                },
                {
                  element: btns[2],
                  handleClick() {
                    // delete map
                    const $target = $select.find('option:selected');
                    const targetId = $target.val();
                    const targetName = $target.text();
                    const alertHandler = alert();
                    const eleAlert = doc.getElementById('alert');
                    alertHandler.init(eleAlert);
                    if (!targetId) {
                      eleAlert.textContent = 'Please select one map.';
                      eleAlert.classList.add('alert-danger');
                      alertHandler.show(eleAlert);
                    } else if (confirm(`The map "${targetName} will be deleted, are you sure?"`)) {
                      const apiDeleteMap = $.ajax({
                        type: 'DELETE',
                        url: `/SettingAPI/CustomizedMap/${targetId}`,
                        async: true,
                        crossDomain: true,
                        headers: {
                          token: $.cookie('token'),
                        },
                      });
                      apiDeleteMap.done(() => {
                        const options = $select.find('option');
                        const lastOption = options[options.length - 1];
                        if (targetId === lastOption.value) {
                          $target.remove();
                          if (options.length > 2) {
                            $select.val(options[1].value); // options[0] is title
                            $select.selectpicker('refresh');
                            $select.trigger('change');
                          } else {
                            $select.val(options[0].value); // options[0] is title
                            $select.selectpicker('refresh');
                            imgMap.src = '';
                            imgMap.style.display = 'none';
                          }
                        } else {
                          $select.val($target.next().val());
                          $target.remove();
                          $select.selectpicker('refresh');
                          $select.trigger('change');
                        }
                        // removeAllMarkers();     //todo
                        markerHandler.removeAllMarkers();
        
                        eleAlert.classList.add('alert-success');
                        eleAlert.textContent = `Map "${targetName}" successfully deleted.`;
                        alertHandler.show(eleAlert);
                      });
                      apiDeleteMap.fail((response) => {
                        if (response.status === 403) {
                          if (response.responseJSON.Response === RETURN_CODE.TOKEN_ERROR) {
                            fire(document, 'logout');
                            history.push('/login', {state: 'normal'});
      
                          } else if (response.responseJSON.Response === RETURN_CODE.PERMISSION_DENIED) {
                            eleAlert.textContent = 'Sorry, you do not have access to delete map.';
                            eleAlert.classList.add('alert-danger');
                            alertHandler.show(eleAlert);
                          }
                        } else {
                          eleAlert.textContent = 'Delete map failed.';
                          eleAlert.classList.add('alert-danger');
                          alertHandler.show(eleAlert);
                        }
                      });
                    }
                  },
                },
                {
                  element: btns[3],
                  handleClick() {
                    this.closest('.active').classList.remove('active');
                    state.isOpen = false;
                  },
                },
                {
                  element: btns[4],
                  handleClick() {
                    this.closest('.active').classList.remove('active');
                    state.isOpen = false;
                  },
                },
                {
                  element: btns[5],
                  handleClick() {
                    // save a marker
                    const target = state.markers[state.selected];
                    const index = state.selected;
                    const result = doc.getElementById('result');
                    result.className = 'hide';
                    if (target.props.isSaved) {
                      result.textContent = 'Aleady saved!';
                      result.className = 'success';
                      return;
                    }
                    if (target.props.guid) {
                      const inputVerify = inputVerification();
                      if (inputVerify.isEmpty(target.props.name)) {
                        result.textContent = 'Please fill in the name.';
                        result.className = 'fail';
                      } else if (!inputVerify.isValidLength(target.props.name, 16)) {
                        result.textContent = 'Sorry, your marker name must be within 16 characters long';
                        result.className = 'fail';
                      } else {
                        const apiUpdateMarker = $.ajax({
                          type: 'PUT',
                          url: `/SettingAPI/CustomizedMap/Marker/${target.props.guid}`,
                          async: true,
                          crossDomain: true,
                          headers: {
                            token: $.cookie('token'),
                            'Content-Type': 'application/json',
                          },
                          data: JSON.stringify({
                            MapId: $select.val(),
                            OffsetX: target.marker.x,
                            OffsetY: target.marker.y,
                            Devices: target.props.devicelist,
                            Name: target.props.name,
                          }),
                        });
                        apiUpdateMarker.done(() => {
                          target.props.isSaved = true;
                          if (index === state.selected) {
                            target.marker.querySelector('img').src = MARKER_SAVED_SELECTED;
                          } else {
                            target.marker.querySelector('img').src = MARKER_SAVED;
                          }
                          $markerSelect.removeOptClass(index, 'unsaved');
                          result.textContent = 'Success!';
                          result.className = 'success';
                        });
                        apiUpdateMarker.fail((response) => {
                          if (response.status === 403) {
                            if (response.responseJSON.Response === RETURN_CODE.TOKEN_ERROR) {
                              fire(document, 'logout');
        
                            } else if (response.responseJSON.Response === RETURN_CODE.PERMISSION_DENIED) {
                              result.textContent = 'Sorry, you do not have access to update marker.';
                              result.className = 'fail';
                            }
                          } else {
                            result.textContent = 'Update marker failed.';
                            result.className = 'fail';
                          }
                        });
                      }
                    } else {
                      const inputVerify = inputVerification();
                      if (inputVerify.isEmpty(target.props.name)) {
                        result.textContent = 'Please fill in the name.';
                        result.className = 'fail';
                      } else if (!inputVerify.isValidLength(target.props.name, 16)) {
                        result.textContent = 'Sorry, your marker name must be within 16 characters long';
                        result.className = 'fail';
                      } else {
                        const apiCreateMarker = $.ajax({
                          type: 'POST',
                          url: '/SettingAPI/CustomizedMap/Marker',
                          async: true,
                          crossDomain: true,
                          headers: {
                            token: $.cookie('token'),
                            'Content-Type': 'application/json',
                          },
                          data: JSON.stringify({
                            MapId: $select.val(),
                            OffsetX: target.marker.x,
                            OffsetY: target.marker.y,
                            Devices: target.props.devicelist,
                            Name: target.props.name,
                          }),
                        });
                        apiCreateMarker.done((response) => {
                          target.props.guid = response;
                          target.props.isSaved = true;
                          if (index === state.selected) {
                            target.marker.querySelector('img').src = MARKER_SAVED_SELECTED;
                          } else {
                            target.marker.querySelector('img').src = MARKER_SAVED;
                          }
                          $markerSelect.removeOptClass(index, 'unsaved');
                          result.textContent = 'Success!';
                          result.className = 'success';
                        });
                        apiCreateMarker.fail((response) => {
                          if (response.status === 403) {
                            if (response.responseJSON.Response === RETURN_CODE.TOKEN_ERROR) {
                              fire(document, 'logout');
        
                            } else if (response.responseJSON.Response === RETURN_CODE.PERMISSION_DENIED) {
                              result.textContent = 'Sorry, you do not have access to add marker.';
                              result.className = 'fail';
                            }
                          } else {
                            result.textContent = 'Add marker failed.';
                            result.className = 'fail';
                          }
                        });
                      }
                    }
                  },
                },
                {
                  element: btns[6],
                  handleClick() {
                    // Delete a marker
                    const index = state.selected;
                    const target = state.markers[index];
                    function removeAMarker(button) {
                      state.markers[index].remove();
                      button.closest('.active').classList.remove('active');
                      delete state.markers[index];
                      state.selected = -1;
                      state.isOpen = false;
                      $markerSelect.removeOpt(index);
                    }
                    const result = doc.getElementById('result');
                    result.className = 'hide';
                    if (target.props.guid) {
                      const apiDeleteMarker = $.ajax({
                        type: 'DELETE',
                        url: `/SettingAPI/CustomizedMap/Marker/${target.props.guid}`,
                        async: true,
                        crossDomain: true,
                        headers: {
                          token: $.cookie('token'),
                        },
                      });
                      apiDeleteMarker.done(() => {
                        removeAMarker(this);
                      });
                      apiDeleteMarker.fail((response) => {
                        if (response.status === 403) {
                          if (response.responseJSON.Response === RETURN_CODE.TOKEN_ERROR) {
                            fire(document, 'logout');
                            history.push('/login', {state: 'normal'});
      
                          } else if (response.responseJSON.Response === RETURN_CODE.PERMISSION_DENIED) {
                            result.textContent = 'Sorry, you do not have access to remove the marker.';
                            result.className = 'fail';
                          }
                        } else {
                          result.textContent = 'Remove marker failed.';
                          result.className = 'fail';
                        }
                      });
                    } else {
                      removeAMarker(this);
                    }
                  },
                },
                {
                  element: btns[7],
                  handleClick() {
                    const notEmpty = state.markers.some((x) => x !== undefined);
                    if (!notEmpty) return;
        
                    if (confirm('The all markers will be removed, are you sure?"')) markerHandler.removeAllMarkersInDB();
                  },
                },
              ];
              btnComponents.forEach(btn => {
                btn.element.addEventListener('click', btn.handleClick);
              });
              // forEach(btnComponents, (btn) => {
              //   btn.element.addEventListener('click', btn.handleClick);
              // });
              $select.change(() => {
                $body.addClass("loading");
                const alertHandler = alert();
                const eleAlert = doc.getElementById('alert');
                alertHandler.init(eleAlert);
                markerHandler.removeAllMarkers();
                reloadMap($select.val());
              });
        
              imgMap.addEventListener('click', (evt) => {
                if (evt.target.parentElement.classList.contains('marker')) return;
                const result = doc.getElementById('result');
                result.className = 'hide';
                $deviceSelect.setSelectedArray([]);
                inputMarkerName.value = '';
                state.markers.forEach(item => {
                  // change icon
                  if (!item) return;
                  if (item.props.isSaved) {
                    item.marker.innerHTML = `<img src=${MARKER_SAVED}>`;
                  } else {
                    item.marker.innerHTML = `<img src=${MARKER_UNSAVED}>`;
                  }
                });
                // forEach(state.markers, (item) => {
                //   // change icon
                //   if (!item) return;
                //   if (item.props.isSaved) {
                //     item.marker.innerHTML = `<img src=${MARKER_SAVED}>`;
                //   } else {
                //     item.marker.innerHTML = `<img src=${MARKER_UNSAVED}>`;
                //   }
                // });
                const marker = doc.createElement('div');
                const defautName = `marker ${state.markers.length}`;
                const index = state.markers.length;
                marker.classList.add('marker');
                marker.innerHTML = `<img src=${MARKER_UNSAVED_SELECTED}>`;
                marker.style.position = 'absolute';
                marker.style.top = `${evt.offsetY - 15}px`;
                marker.style.left = `${evt.offsetX - 15}px`;
                marker.x = evt.offsetX - 15;
                marker.y = evt.offsetY - 15;
                state.selected = index;
                state.markers.push(new Marker(marker, {
                  name: defautName,
                  isSaved: false,
                  index,
                  devicelist: [],
                  guid: undefined,
                }));
                inputMarkerName.value = defautName;
                $markerSelect.removeSelected();
                const option = doc.createElement('option');
                option.textContent = defautName;
                option.value = index;
                option.selected = true;
                option.className = 'unsaved';
                $markerSelect.addOptElement(option);
                divMap.appendChild(marker);
                if (!state.isOpen) {
                  showMarkerSettingBar(marker);
                  state.isOpen = true;
                }
              });
              function enableDragMarker() {
                let initialX;
                let initialY;
                let target;
                let active = false;
                function dragStart(evt) {
                  target = evt.target.parentElement;
                  if (target.classList.contains('marker')) {
                    initialX = evt.clientX;
                    initialY = evt.clientY;
                    active = true;
                  }
                }
                function drag(evt) {
                  if (active) {
                    evt.preventDefault();
                    const sideBar = doc.querySelector('.setup-marker-wrap > .active');
                    if (sideBar) {
                      sideBar.classList.remove('active');
                      state.isOpen = false;
                      state.selected = -1;
                    }
        
                    if (state.markers[target.index].props.isSaved) {
                      state.markers[target.index].props.isSaved = false;
                      target.querySelector('img').src = MARKER_UNSAVED_SELECTED;
                    }
        
        
                    let tmpx = target.x + evt.clientX - initialX;
                    let tmpy = target.y + evt.clientY - initialY;
                    if (tmpx < state.area.left) {
                      tmpx = state.area.left;
                    } else if (tmpx > state.area.right) {
                      tmpx = state.area.right;
                    }
                    if (tmpy < state.area.top) {
                      tmpy = state.area.top;
                    } else if (tmpy > state.area.bottom) {
                      tmpy = state.area.bottom;
                    }
        
                    target.style.left = `${tmpx}px`;
                    target.style.top = `${tmpy}px`;
                    // const captureClick = function (e) {
                    //   e.stopPropagation(); // Stop the click from being propagated.
                    //   window.removeEventListener('click', captureClick, true); // cleanup
                    // };
                    // window.addEventListener(
                    //   'click',
                    //   captureClick,
                    //   true, // <-- This registeres this listener for the capture
                    //   //     phase instead of the bubbling phase!
                    // );
                  }
                }
                function dragEnd() {
                  if (active) {
                    target.x = Number(target.style.left.match(/\d+/)[0]);
                    target.y = Number(target.style.top.match(/\d+/)[0]);
                    active = false;
                  }
                }
                divMap.addEventListener('mousedown', dragStart, false);
                divMap.addEventListener('mousemove', drag, false);
                divMap.addEventListener('mouseup', dragEnd, false);
              }
              enableDragMarker();
              inputMarkerName.addEventListener('keyup', (evt) => {
                const idx = state.selected;
                const target = state.markers[idx];
                if (evt.target.value !== target.props.name) {
                  target.props.isSaved = false;
                  target.props.name = evt.target.value;
                  target.marker.querySelector('img').src = MARKER_UNSAVED_SELECTED;
                  $markerSelect.changeOptName(idx, evt.target.value);
                }
              }, false);
            });
            $body.removeClass("loading");
          });
          apiGetDevics.fail((response) => {
            if (response.status === 403 && response.responseJSON.Response === RETURN_CODE.TOKEN_ERROR) {
              fire(document, 'logout');
            }
          });
    }
    $(document).on("reload-setting-customized-map", reloadSettingCustomizedMap);
})()