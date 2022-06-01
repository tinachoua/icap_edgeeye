export const LATER_BAD = 'Later Bad';
export const POWER_ON = 'Power On Hours';
export const POWER_CYCLE = 'Power Cycle Count';
export const TOTAL_BAD_BLOCK = 'Total Bad Block Count';
export const MAX_ERASE = 'Max Erase Count';
export const AVG_ERASE = 'Avg Erase Count';
export const DEVICE_LIFE = 'Device Life';
export const SPARE_BLOCK = 'Spare Block Count';
export const PROGRAM_FAIL = 'Program Fail Count';
export const ERASE_FAIL = 'Erase Fail Count';
export const ABNORMAL_POWER = 'Abnormal power cycle count';
export const TEMPERATURE = 'Temperature';
export const FLASH_ID = 'Flash ID';
export const LATER_BAD_BLOCK_READ = 'Later Bad Block Read';
export const LATER_BAD_BLOCK_WRITE = 'Later Bad Block Write';
export const LATER_BAD_BLOCK_ERASE = 'Later Bad Block Erase';
export const TOTAL_LBAS_WRITTEN = 'Total LBAs Written';
export const TOTAL_LBAS_READ = 'Total LBAs Read';
export const PE_CYCLE = 'P/E Cycle';

const ABNORMAL_POWER_ATTR = 'abnormalPowerCycleCount';
const AVG_ERASE_ATTR = 'avgEraseCount';
const DEVICE_LIFE_ATTR = 'deviceLife';
const ERASE_FAIL_ATTR = 'eraseFailCount';
const FLASH_ID_ATTR = 'flashID';
const LATER_BAD_ATTR = 'laterBad';
const LATER_BAD_BLOCK_ERASE_ATTR = 'laterBadBlockErase';
const LATER_BAD_BLOCK_READ_ATTR = 'laterBadBlockRead';
const LATER_BAD_BLOCK_WRITE_ATTR = 'laterBadBlockWrite';
const MAX_ERASE_ATTR = 'maxEraseCount';
const PE_CYCLE_ATTR = 'p/ECycle';
const POWER_CYCLE_ATTR = 'powerCycleCount';
const POWER_ON_ATTR = 'powerOnHours';
const SPARE_BLOCK_ATTR = 'spareBlockCount';
const TEMPERATURE_ATTR = 'temperature';
const TOTAL_BAD_BLOCK_ATTR = 'totalBadBlockCount';
const TOTAL_LBAS_READ_ATTR = 'totalLBAsRead';
const TOTAL_LBAS_WRITTEN_ATTR = 'totalLBAsWritten';


export const ISMART_ATTR_MAP_NAME = {
  [ABNORMAL_POWER_ATTR]: ABNORMAL_POWER,
  [AVG_ERASE_ATTR]: AVG_ERASE,
  [DEVICE_LIFE_ATTR]: DEVICE_LIFE,
  [ERASE_FAIL_ATTR]: ERASE_FAIL,
  [FLASH_ID_ATTR]: FLASH_ID,
  [LATER_BAD_ATTR]: LATER_BAD,
  [LATER_BAD_BLOCK_ERASE_ATTR]: LATER_BAD_BLOCK_ERASE,
  [LATER_BAD_BLOCK_READ_ATTR]: LATER_BAD_BLOCK_READ,
  [LATER_BAD_BLOCK_WRITE_ATTR]: LATER_BAD_BLOCK_WRITE,
  [MAX_ERASE_ATTR]: MAX_ERASE,
  [PE_CYCLE_ATTR]: PE_CYCLE,
  [POWER_CYCLE_ATTR]: POWER_CYCLE,
  [POWER_ON_ATTR]: POWER_ON,
  [SPARE_BLOCK_ATTR]: SPARE_BLOCK,
  [TEMPERATURE_ATTR]: TEMPERATURE,
  [TOTAL_BAD_BLOCK_ATTR]: TOTAL_BAD_BLOCK,
  [TOTAL_LBAS_READ_ATTR]: TOTAL_LBAS_READ,
  [TOTAL_LBAS_WRITTEN_ATTR]: TOTAL_LBAS_WRITTEN
}

export const DEVICE_POWER_OFF = 'power off';
export const DEVICE_POWER_ON = 'power on';