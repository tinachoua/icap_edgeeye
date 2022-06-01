import os_comp from "../../html/components/device_info_os.html";
import cpu_comp from "../../html/components/device_info_cpu.html";
import mb_comp from "../../html/components/device_info_mb.html";
import mem_comp from "../../html/components/device_info_mem.html";
import storage_comp from "../../html/components/device_info_storage.html";
import net_comp from "../../html/components/device_info_net.html";
import location_comp from "../../html/components/device_info_location.html";
import external_comp from "../../html/components/device_info_external.html";
import gpu_comp from "../../html/components/device_info_gpu.html";
import oob_comp from "../../html/components/device_info_oob.html";
import device_info_EAPI from '../../html/components/device_info_eapi.html';
import { render } from "../library/component";
import { g_device_data } from "../library/get_device_info";
import { API } from "../library/api_handler";

export var autoRefresh = {
  id: undefined
};

export function switchSecondTabs() {
  const _render = render();
  const childPage = document.getElementById('child-page');

  function toOverview(oobDirector) {
    var widget = document.getElementsByName('widget-overview');
    $(window).resize(function () {
      var width = $(window).width();
      if (width < 762) {
        $('#clock-card').hide();
      } else {
        let clock = document.getElementById('clock-card');

        if (clock.style.display === 'none') {
          clock.style.display = '';
        }
      }
    });
    $(widget).show();
    if (!oobDirector.root.contains(oobDirector.element)) {
      oobDirector.root.appendChild(oobDirector.element);
      oobDirector.setupService({
        activeServices: ['reboot', 'recovery']
      });
    }
  }

  function toOS(data) {
    $(window).off('resize');
    var comp = document.getElementsByName('os');
    if (comp.length === 0) {
      childPage.insertAdjacentHTML('beforeend', os_comp);
    } else {
      $('#subpanel-OS-data').empty();
    }
    _render.OSInfo(data.detail.OS);
    $(comp).show();
  }

  function toCPU(data) {
    $(window).off('resize');
    var comp = document.getElementsByName('cpu');

    if (comp.length === 0) {
      childPage.insertAdjacentHTML('beforeend', cpu_comp);
    } else {
      $('#subpanel-CPU-data').empty();
    }
    _render.CPUInfo(data.detail);
    $(comp).show();
  }

  function toMB(data) {
    $(window).off('resize');
    var comp = document.getElementsByName('mb');

    if (comp.length === 0) {
      childPage.insertAdjacentHTML('beforeend', mb_comp);
    } else {
      $('#subpanel-MB-data').empty();
    }
    _render.MBInfo(data.detail);
    $(comp).show();
  }

  function toMEM(data) {
    $(window).off('resize');
    var comp = document.getElementsByName('mem');

    if (comp.length === 0) {
      childPage.insertAdjacentHTML('beforeend', mem_comp);
    } else {
      $('#btn-indx-mem button').remove();
    }
    _render.MEMInfo(data.detail);
    $(comp).show();
  }

  function toStorage(data) {
    $(window).off('resize');
    var comp = document.getElementsByName('storage');

    if (comp.length === 0) {
      childPage.insertAdjacentHTML('beforeend', storage_comp);
    } else {
      $('#btn-indx-storage button').remove();
    }
    _render.StorageInfo(data.detail.Stor);
    $(comp).show();
  }

  function toNET(data) {
    $(window).off('resize');
    var comp = document.getElementsByName('net');

    if (comp.length === 0) {
      childPage.insertAdjacentHTML('beforeend', net_comp);
    } else {
      $('#btn-indx-net button').remove();
    }
    _render.NetInfo(data.detail.NET);
    $(comp).show();
  }

  function toLocation(deviceName) {
    $(window).off('resize');
    var comp = document.getElementsByName('location');

    if (comp.length === 0) {
      childPage.insertAdjacentHTML('beforeend', location_comp);
    } else {
      $('#btn-indx-net button').remove();
      //// memory leak
      let widgetBody = comp[0].getElementsByClassName('panel-body')[0];
      while (widgetBody.hasChildNodes()) {
        widgetBody.removeChild(widgetBody.firstChild);
      }
    }
    var APICaller = API();
    var promise = APICaller.GET(`DeviceInfoAPI/GetLocation?DeviceName=${deviceName}`);

    promise.done((response) => {
      _render.DeviceMap(JSON.parse(response));
    });

    $(comp).show();
  }

  function toExternal(data) {
    $(window).off('resize');
    var comp = document.getElementsByName('external');

    if (comp.length === 0) {
      childPage.insertAdjacentHTML('beforeend', external_comp);
    }
    autoRefresh.id = _render.EXTInfo(data.detail);
    $(comp).show();
  }

  function toGPU(data) {
    $(window).off('resize');
    var comp = document.getElementsByName('gpu');

    if (comp.length === 0) {
      childPage.insertAdjacentHTML('beforeend', gpu_comp);
    }

    _render.GPUInfo(data.detail);
    $(comp).show();
  }

  function toOOB(devName, oobDirector) {
    $(window).off('resize');
    var comp = document.getElementsByName('oob');

    if (comp.length === 0) {
      childPage.insertAdjacentHTML('beforeend', oob_comp);
    }

    _render.OOBInfo(devName, oobDirector);
    $(comp).show();
  }

  function toEAPIInfo(devName) {
    $(window).off('resize');
    var comp = document.getElementsByName('eapi');
    if (comp.length === 0) {
      childPage.insertAdjacentHTML('beforeend', device_info_EAPI);
    }

    var APICaller = API();
    var promise = APICaller.GET(`DeviceInfoAPI/${devName}/EAPI`);

    promise.done((response) => {
      _render.EAPIInfo(response);
    });

    $(comp).show();
  }

  return {
    toOverview,
    toOS,
    toCPU,
    toMB,
    toMEM,
    toStorage,
    toNET,
    toLocation,
    toExternal,
    toGPU,
    toOOB,
    toEAPIInfo
  };
}

export var putSecondTabs = function (data, oobDirector) {
  var tabSecondContainer = document.getElementById("tab-secondary-continer");
  var secondTabs = ["Overview", "OS", "CPU", "MB", "MEM", "EAPI", "Storage", "NET", "Location"];
  var secondTabsTooltip = [
    "Device overview",
    "Device operating system information",
    "Device CPU information",
    "Device motherboard information",
    "Device memory information",
    "Device storage information",
    "Device network card information",
    "Device location information"
  ];

  if ((data.detail.EXT && data.detail.EXT.length !== 0) === true) {
    secondTabs.push('External');
    secondTabsTooltip.push('Device external sensor information');
  }

  if (data.detail.GPUDynamic && data.detail.GPUDynamic.length !== 0) {
    secondTabs.push('GPU');
    secondTabsTooltip.push('Device GPU information');
  }

  if (data.detail.isEAPI) {
    secondTabs.push('EAPI');
    secondTabsTooltip.push(null);
  }

  if (data.detail.IsInnoAGE) {
    secondTabs.push('OOB');
    secondTabsTooltip.push('Device out of band');
  }


  secondTabs.forEach((element, idx, array) => {
    var subTab = $Tab(element).getTab();

    $(subTab).on('click', (evt) => {
      var currentDevice = document.getElementById('device-select').value;
      var targetTabs = evt.target.closest('a').id;
      var currentTabs = document.getElementById('tab-secondary-continer').getElementsByClassName('hover')[0];

      if (!!currentTabs && currentTabs.id === targetTabs) {
        return;
      }

      if (autoRefresh.id !== undefined) {
        clearInterval(autoRefresh.id);
        autoRefresh.id = undefined;
      }
      $('#tab-primary-continer a').removeClass('tab-click');
      $('#tab-secondary-continer a').removeClass('hover');
      $(subTab).addClass('hover');
      $('#child-page .device_analyzer').hide();
      var widget = document.getElementsByClassName('widget');
      $(widget).hide();

      var toTabsActor = switchSecondTabs();
      switch (targetTabs) {
        case 'Overview':
          toTabsActor.toOverview(oobDirector);
          break;
        case 'OS':
          toTabsActor.toOS(g_device_data);
          break;
        case 'CPU':
          toTabsActor.toCPU(g_device_data);
          break;
        case 'MB':
          toTabsActor.toMB(g_device_data);
          break;
        case 'MEM':
          toTabsActor.toMEM(g_device_data);
          break;
        case 'Storage':
          toTabsActor.toStorage(g_device_data);
          break;
        case 'NET':
          toTabsActor.toNET(g_device_data);
          break;
        case 'Location':
          toTabsActor.toLocation(currentDevice);
          break;
        case 'External':
          toTabsActor.toExternal(g_device_data);
          break;
        case 'GPU':
          toTabsActor.toGPU(g_device_data);
          break;
        case 'OOB':
          toTabsActor.toOOB(currentDevice, oobDirector);
          break;
        case 'EAPI':
          toTabsActor.toEAPIInfo(currentDevice);
          break;
      }
    });

    subTab.setAttribute("class", "sub-tab-content");
    subTab.setAttribute("id", element);
    subTab.setAttribute("data-toggle", "tooltip");
    subTab.setAttribute("data-placement", "bottom");
    subTab.setAttribute("title", secondTabsTooltip[idx]);
    tabSecondContainer.appendChild(subTab);
    tabSecondContainer.appendChild($Tab().getDivider());
  });
  $('#Overview').addClass('hover');
  $('#main .tab-secondary').show();
};