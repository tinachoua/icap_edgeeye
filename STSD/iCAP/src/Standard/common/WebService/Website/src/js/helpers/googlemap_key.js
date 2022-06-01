
let cache;

export default async function(){
  if (cache) return cache;

  const response = await $.ajax({
    type: 'GET',
    url: 'SettingAPI/Key/GoggleMap',
    async: true,
    crossDomain: true,
    headers: {
        'token': $.cookie('token')
    },
    global: false
  }).then(response => response);

  return response.data? response.data.key: null;

}