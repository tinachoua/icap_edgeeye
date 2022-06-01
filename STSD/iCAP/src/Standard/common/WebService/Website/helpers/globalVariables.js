const JSON5 = require('json5');
const fs = require('fs');
const path = require('path');

const { CONFIG_PATH } = require('../constants/env');

function getObjectValue(obj, keys, fallback) {
  if (!obj) {
    return fallback;
  }

  if (!keys) {
    return obj
  }

  if (typeof keys === 'string') {
    keys = keys.split('.')
  }

  for (var i = 0, l = keys.length; obj && i < l; i++) {
    obj = obj[keys[i]]
  }

  if (obj === null || typeof obj === 'undefined') {
    return fallback;
  } else {
    return obj
  }
}

const postsDirectory = path.join(process.cwd(), 'config');

const GLOBAL_SETUP = JSON5.parse(fs.readFileSync(path.join(postsDirectory, CONFIG_PATH)));
const WIDGET_HEADER_BG_COLOR = getObjectValue(GLOBAL_SETUP, "CUSTOMIZE.WIDGET_HEADER_BG_COLOR", GLOBAL_SETUP["BG_COLOR_03"]);
const SIDE_MENU_BG_COLOR = getObjectValue(GLOBAL_SETUP, "CUSTOMIZE.SIDE_MENU_BG_COLOR", GLOBAL_SETUP["BG_COLOR_02"]);
const DASHBOARD_TOP_NAVBAR_BG_COLOR = getObjectValue(GLOBAL_SETUP, "CUSTOMIZE.DASHBOARD_TOP_NAVBAR_BG_COLOR", GLOBAL_SETUP["BG_COLOR_03"])
const DASHBOARD_TOP_NAVBAR_COLOR = getObjectValue(GLOBAL_SETUP, "CUSTOMIZE.DASHBOARD_TOP_NAVBAR_COLOR", GLOBAL_SETUP["FONT_COLOR_B"])
const DASHBOARD_TOP_NAVBAR_HEIGHTLIGHT_COLOR = getObjectValue(GLOBAL_SETUP, "CUSTOMIZE.DASHBOARD_TOP_NAVBAR_HEIGHTLIGHT_COLOR", GLOBAL_SETUP["FONT_COLOR_A"])
const TOP_NAVBAR_BG_COLOR = getObjectValue(GLOBAL_SETUP, "CUSTOMIZE.TOP_NAVBAR_BG_COLOR", GLOBAL_SETUP["BG_COLOR_03"]);
const TOP_SUB_NAVBAR_BG_COLOR = getObjectValue(GLOBAL_SETUP, "CUSTOMIZE.TOP_SUB_NAVBAR_BG_COLOR", GLOBAL_SETUP["BG_COLOR_07"]);
const DEVICE_LATEST_DATA_BG_COLOR = getObjectValue(GLOBAL_SETUP, "CUSTOMIZE.DEVICE_LATEST_DATA_BG_COLOR", GLOBAL_SETUP["BG_COLOR_8"]);
const LOGIN_LOGO_WIDTH = getObjectValue(GLOBAL_SETUP, "LOGINFORM_LOGO_SIZE.WIDTH", "");
const LOGIN_LOGO_HEIGHT = getObjectValue(GLOBAL_SETUP, "LOGINFORM_LOGO_SIZE.HEIGHT", "");
const SIDE_MENU_BOTTOM_LOGO_WIDTH =  getObjectValue(GLOBAL_SETUP, "LOGINFORM_LOGO_SIZE.WIDTH", "");
const SIDE_MENU_BOTTOM_LOGO_HEIGHT =  getObjectValue(GLOBAL_SETUP, "LOGINFORM_LOGO_SIZE.HEIGHT", "");
const UNDRAG_WIDGET_HEADER_COLOR = getObjectValue(GLOBAL_SETUP, "CUSTOMIZE.UNDRAG_WIDGET_HEADER_COLOR", GLOBAL_SETUP["FONT_COLOR_D"]);
const HEADER_LOGO_SIZE_HEIGHT = getObjectValue(GLOBAL_SETUP, "HEADER_LOGO_SIZE.HEIGHT");

module.exports = {
  ...GLOBAL_SETUP,
  WIDGET_HEADER_BG_COLOR,
  SIDE_MENU_BG_COLOR,
  DASHBOARD_TOP_NAVBAR_BG_COLOR,
  DASHBOARD_TOP_NAVBAR_COLOR,
  DASHBOARD_TOP_NAVBAR_HEIGHTLIGHT_COLOR,
  TOP_NAVBAR_BG_COLOR,
  TOP_SUB_NAVBAR_BG_COLOR,
  DEVICE_LATEST_DATA_BG_COLOR,
  LOGIN_LOGO_WIDTH,
  LOGIN_LOGO_HEIGHT,
  HEADER_LOGO_SIZE_HEIGHT,
  SIDE_MENU_BOTTOM_LOGO_WIDTH,
  SIDE_MENU_BOTTOM_LOGO_HEIGHT,
  UNDRAG_WIDGET_HEADER_COLOR
}