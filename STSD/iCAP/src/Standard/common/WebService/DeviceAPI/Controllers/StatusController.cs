using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DeviceAPI.Models.Status;
using ShareLibrary;
using ShareLibrary.AdminDB;
using ShareLibrary.Interface;
using MongoDB.Bson;

namespace DeviceAPI.Controllers
{
    /// <summary>
    /// The device status APIs
    /// </summary>
    public class StatusController : Controller
    {
        private IRedisCacheDispatcher _rcd;
        private IDevice _add;
        private IDataDBDispatcher _ddb;

        public StatusController(IRedisCacheDispatcher rcd, IDevice add, IDataDBDispatcher ddb)
        {
            _rcd = rcd;
            _add = add;
            _ddb = ddb;
        }

        /// <summary>
        /// 1. Get device list
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get deivce list success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("StatusAPI/GetList")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public IActionResult GetList([FromHeader]string token)
        {
            //icapContext ic = new icapContext();
            //AdminDBDispatcher._device adb = new AdminDBDispatcher._device();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                //List<string> devList = ic.Device.Select(x => x.Name).ToList();
                List<string> devList = _add.GetList();
                var retPayload = new
                {
                    DeviceList = devList.ToArray()
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "StatusAPI/GetList",
                    ResponseCode = 200,
                    Remark = "Get device list success."
                });

                return StatusCode(200, JsonConvert.SerializeObject(retPayload));
            }
            else
            {
                var retPayload = new
                {
                    Response = "authentication error"
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "StatusAPI/GetList",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 2. Get device status
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="device">The device identity</param>
        /// <returns></returns>
        /// <response code="200">Get deivce status success</response>
        /// <response code="204">Device not found</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("StatusAPI/Get")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]      
        public IActionResult Get([FromHeader]string token, [FromHeader]string device)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                int status = 0;
                string devStatus = _rcd.GetStatus(device);
                if(devStatus == null)
                {
                    //Device dev = ic.Device.Where(x => x.Name == device).SingleOrDefault();
                    Device dev = _add.Get(device);
                    if(dev == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "StatusAPI/Get",
                            ResponseCode = 204,
                            Remark = $"Device {device} not found."
                        });

                        return StatusCode(204);
                    }
                    else
                    {
                        _rcd.SetStatus(device, 0);
                    }
                }
                else
                {
                    int.TryParse(devStatus, out status);
                }

                var retPayload = new
                {
                    Response = status
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "StatusAPI/Get",
                    ResponseCode = 200,
                    Remark = $"Get device {device} information success."
                });

                return StatusCode(200, JsonConvert.SerializeObject(retPayload));
            }
            else
            {
                var retPayload = new
                {
                    Response = "authentication error"
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "StatusAPI/Get",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 3. Update device status
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="device">The device identity</param>
        /// <returns></returns>
        /// <response code="202">Update deivce status success</response>
        /// <response code="304">Device not found, and not modified.</response>
        /// <response code="403">The identity token not found</response>
        [HttpPut("StatusAPI/Update")]
        [Produces("application/json")]
        [ProducesResponseType(202)]
        [ProducesResponseType(304)]
        [ProducesResponseType(403)]        
        public IActionResult Update([FromHeader]string token, [FromBody]DeviceStatus device)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                string devStatus = _rcd.GetStatus(device.DeviceName);
                if (devStatus == null)
                {
                    //Device dev = ic.Device.Where(x => x.Name == device.DeviceName).SingleOrDefault();
                    Device dev = _add.Get(device.DeviceName);
                    if (dev == null)
                    {
                        var retPayload1 = new
                        {
                            Response = "device not found"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "StatusAPI/Update",
                            ResponseCode = 304,
                            Remark = $"Device {device.DeviceName} not found.."
                        });

                        return StatusCode(304, JsonConvert.SerializeObject(retPayload1));
                    }
                }

                if (device.Status)
                {
                    _rcd.SetStatus(device.DeviceName, 1);
                }
                else
                {
                    _rcd.SetStatus(device.DeviceName, 0);
                }

                var retPayload = new
                {
                    Response = (device.Status?1:0)
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "PUT",
                    URL = "StatusAPI/Update",
                    ResponseCode = 202,
                    Remark = $"Update device {device.DeviceName} information success."
                });

                return StatusCode(202, JsonConvert.SerializeObject(retPayload));
            }
            else
            {
                var retPayload = new
                {
                    Response = "authentication error"
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "PUT",
                    URL = "StatusAPI/Update",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 4. Clean all device status
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get deivce list success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("StatusAPI/CleanAllStatus")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public IActionResult CleanAllStatus([FromHeader]string token)
        {
            //icapContext ic = new icapContext();
            //AdminDBDispatcher._device adb = new AdminDBDispatcher._device();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                _rcd.CleanDeviceStatus();

                var retPayload = new
                {
                    Response = "Clean all device status success."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "PUT",
                    URL = "StatusAPI/CleanAllStatus",
                    ResponseCode = 200,
                    Remark = "Clean all device status success."
                });

                return StatusCode(200, JsonConvert.SerializeObject(retPayload));
            }
            else
            {
                var retPayload = new
                {
                    Response = "authentication error"
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "PUT",
                    URL = "StatusAPI/CleanAllStatus",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 5. Get device state
        /// </summary>
        /// <param name="token">The device thumbprint which generated from device</param>
        /// <param name="query">The device query string</param>
        /// <returns>A device ID and password use for connection into iCAP gateway</returns>
        /// <response code="200">Get device name list successfully.</response>
        /// <response code="403">token error</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("StatusAPI/Devices")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetDeviceList([FromHeader]string token, string query)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    var devices = query.Split(',');

                    List<int> retList = new List<int>();
                    foreach (string b in devices)
                    {
                        int state = (int)DataDefine.DeviceState.Normal;
                        var queryObj = new
                        {
                            Dev = b
                        };
                        List<BsonDocument> dbRet = _ddb.GetRawData("EventLog", JsonConvert.SerializeObject(queryObj), 0);
                        if (dbRet.Count != 0)
                        {
                            foreach (BsonDocument bdoc in dbRet)
                            {
                                if (!bdoc.GetValue("Checked").AsBoolean && !bdoc.GetValue("Message").AsString.ToUpper().Contains("OFFLINE"))
                                {
                                    state = (int)DataDefine.DeviceState.Warning;
                                }
                            }
                        }

                        if ((int)DataDefine.DeviceState.Warning != state)
                        {
                            int onlineFlag = (int)DataDefine.DeviceState.Offline;
                            string devOnline = _rcd.GetStatus(b);
                            if (devOnline != null)
                            {
                                int.TryParse(devOnline, out onlineFlag); //0:offline 1:online
                            }

                            state = (onlineFlag == (int)DataDefine.DeviceState.Online) ? (int)DataDefine.DeviceState.Normal : (int)DataDefine.DeviceState.Offline; // 2: normal , 3:event, 0: offline

                        }
                        retList.Add(state);
                    }

                    var retPayload = new
                    {
                        Response = retList.ToArray()
                    };
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "StatusAPI/Devices",
                        ResponseCode = 200,
                        Remark = "Get Device state successfully."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                catch (Exception exc)
                {
                    var retPayload = new
                    {
                        Response = "Unexcepted Error!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "StatusAPI/Devices",
                        ResponseCode = 500,
                        Remark = exc.Message
                    });
                    return StatusCode(500, JsonConvert.SerializeObject(retPayload));
                }
            }
            else
            {
                var retPayload = new
                {
                    Response = ReturnCode.TokenError
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "StatusAPI/Devices",
                    ResponseCode = 500,
                    Remark = "Token Error"
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }
    }
}
