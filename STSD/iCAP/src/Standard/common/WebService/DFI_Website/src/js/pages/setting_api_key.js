import { API, } from "../library/api_handler";
import { showMyAlert, hideAlertById } from '../library/form_alert_controller';
import { TPYE_GOGGLEMAP } from '../constants/api_key';

async function fetchKeyUpdateTime(){
  let apiHandler = API();
  let promise = apiHandler.GET('SettingAPI/Key/UpdateTime');

  return await promise.then(({data}) => {
    const googleMapKeyUpdateTime = data.find(item => item.type === TPYE_GOGGLEMAP);
    if (googleMapKeyUpdateTime) {
      return googleMapKeyUpdateTime.updateTime;
    }
  }) 

}

function updateTime(timeStr){
  const $time = $('#time');
  const date = new Date(timeStr);
  
  $time.text(date.toLocaleString());
}

(function IIFE(){
  async function reloadSettingApiKey(){
    const $updateBtn = $('#update-key');
    const timeStr = await fetchKeyUpdateTime();
    if (timeStr) {
      updateTime(timeStr);
    }

    $updateBtn.on('click', ()=>{
      hideAlertById(['update-success', 'update-fail']);
      const key = document.getElementById('key').value;
      if(!key){
        showMyAlert("update-fail", "Please fill in the key.", "alert-blank-edit");
        return;
      }
      let $body = $("body");
      $body.addClass('loading');
      let apiHandler = API();
      let promise = apiHandler.POST('SettingAPI/Key', {
        type: TPYE_GOGGLEMAP,
        key: key
      });
      promise.done(({data})=>{
        updateTime(data);
        showMyAlert("update-success", "Update successfully.", "alert-blank-edit");
        $body.removeClass('loading');
      });
    });
  }
  $(document).on("reload-setting-api-key", reloadSettingApiKey);
})();