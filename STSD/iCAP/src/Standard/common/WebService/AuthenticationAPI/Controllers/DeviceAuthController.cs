using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System;
using Newtonsoft.Json;
using System.Threading;
using ShareLibrary;
using ShareLibrary.AdminDB;
using ShareLibrary.Interface;

namespace AuthenticationAPI.Controllers
{
    /// <summary>
    /// The device authentication APIs
    /// </summary>
	public class DeviceAuthController : Controller
	{
        /// <summary>
        /// Mutex for calculate device ID
        /// </summary>
        public static Mutex mut = new Mutex();

        private IRedisCacheDispatcher _rcd;
        private IDevice _add;
        private IDevicecertificate _adcert;
        private ILicenselist _adl;
        
        //private IFormCollection _fc;

        public DeviceAuthController(IRedisCacheDispatcher rcd, IDevice add,IDevicecertificate adcert,ILicenselist adl)
        {
            _rcd = rcd;
            _add = add;
            _adl = adl;
            _adcert = adcert;
        }

        /// <summary>
        /// 1. Device authentication
        /// </summary>
        /// <param name="Thumbprint">The device thumbprint which generated from device</param>
        /// <returns>A device ID and password use for connection into iCAP gateway</returns>
        /// <response code="200">Authentication success</response>
        /// <response code="403">Device already over limitaion</response>
        [HttpGet("AuthenticationAPI/GetID")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult GetID([FromHeader]string Thumbprint)
		{
            //try
            {
                String GenPassword = null;
                Device dev;
                String DeviceName = null;

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = false,
                    Name = Thumbprint,
                    Method = "GET",
                    URL = "AuthenticationAPI/GetID",
                    ResponseCode = 0,
                    Remark = string.Format("Get thumbprint {0}", Thumbprint)
                });

                mut.WaitOne();
                Devicecertificate devCert = _adcert.Get(Thumbprint);

                if (devCert != null)
                {
                    if (devCert.ExpiredDate < DateTime.UtcNow)
                    {
                        GenPassword = CommonFunctions.CreateRandomPassword(16);

                        devCert.ExpiredDate = DateTime.UtcNow.AddDays(7);
                        devCert.Password = GenPassword;
                    }
                    else
                    {
                        GenPassword = devCert.Password;
                    }
                    devCert.LastModifiedDate = DateTime.UtcNow;
                    dev = _add.Get(devCert.DeviceId);
                    if (dev == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = Thumbprint,
                            Method = "GET",
                            URL = "AuthenticationAPI/GetID",
                            ResponseCode = 204,
                            Remark = string.Format("Get password success but device {0} not found in device table.", devCert.DeviceId)
                        });
                        mut.ReleaseMutex();
                        return StatusCode(204);
                    }
                    DeviceName = dev.Name;
                    _adcert.Update(devCert);
                }
                else
                {
                    //mut.WaitOne();
                    DeviceName = string.Format("Device{0:00000}", _add.Count() + 1);
                    dev = _add.Get(DeviceName);

                    if (dev == null)
                    {
                        _add.Create(new Device()
                        {
                            Name = DeviceName,
                            DeviceClassId = 1,
                            OwnerId = 1,
                            CreatedDate = DateTime.UtcNow,
                            LastModifiedDate = DateTime.UtcNow
                        });
                        dev = _add.Get(DeviceName);
                    }

                    if (dev != null)
                    {
                        GenPassword = CommonFunctions.CreateRandomPassword(16);

                        devCert = new Devicecertificate()
                        {
                            CreatedDate = DateTime.UtcNow,
                            LastModifiedDate = DateTime.UtcNow,
                            DeviceId = dev.Id,
                            Thumbprint = Thumbprint,
                            Password = GenPassword,
                            ExpiredDate = DateTime.UtcNow.AddDays(7)
                        };
                        _adcert.Create(devCert);
                    }
                    else
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = Thumbprint,
                            Method = "GET",
                            URL = "AuthenticationAPI/GetID",
                            ResponseCode = 500,
                            Remark = string.Format("Add device {0} into device table fail.", devCert.DeviceId)
                        });
                        mut.ReleaseMutex();
                        return StatusCode(500);
                    }

                    var retPayload1 = new
                    {
                        Cmd = "Auth",
                        DeviceId = DeviceName,
                        PWD = GenPassword
                    };

                    _rcd.AddDevice(DeviceName);
                    _rcd.SendPWD(DeviceName, GenPassword);

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = Thumbprint,
                        Method = "GET",
                        URL = "AuthenticationAPI/GetID",
                        ResponseCode = 200,
                        Remark = string.Format("Add deivce {0} successful.", DeviceName)
                    });
                    mut.ReleaseMutex();
                    return StatusCode(200, JsonConvert.SerializeObject(retPayload1));
                }

                var retPayload = new
                {
                    Cmd = "Auth",
                    DeviceId = DeviceName,
                    PWD = GenPassword
                };

                _rcd.AddDevice(DeviceName);
                _rcd.SendPWD(DeviceName, GenPassword);

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = Thumbprint,
                    Method = "GET",
                    URL = "AuthenticationAPI/GetID",
                    ResponseCode = 200,
                    Remark = string.Format("Add deivce {0} successful.", DeviceName)
                });
                mut.ReleaseMutex();
                return StatusCode(200, JsonConvert.SerializeObject(retPayload));

            }
            /*catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }*/
        }

        /// <summary>
        /// 1. Device authentication
        /// </summary>
        /// <param name="token">The device thumbprint which generated from device</param>
        /// <returns>A device ID and password use for connection into iCAP gateway</returns>
        /// <response code="200">Get device name list successfully.</response>
        /// <response code="403">token error</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("AuthenticationAPI/KeyChanges/GetDeviceList")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetDeviceList([FromHeader]string token)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "AuthenticationAPI/KeyChanges/GetDeviceList",
                ResponseCode = 0,
                Remark = ""
            });

            if (token == "innodisk1234")
            {
                try
                {
                    mut.WaitOne();
                    string[] deiveNameList = _adl.GetDeviceListToSetOffline();

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = "",
                        Method = "GET",
                        URL = "AuthenticationAPI/KeyChanges/GetDeviceList",
                        ResponseCode = 200,
                        Remark = "Get device name list successfully."
                    });
                    mut.ReleaseMutex();
                    return StatusCode(200, JsonConvert.SerializeObject(deiveNameList));
                }
                catch(Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = "",
                        Method = "GET",
                        URL = "AuthenticationAPI/KeyChanges/GetDeviceList",
                        ResponseCode = 500,
                        Remark = exc.Message
                    });
                    mut.ReleaseMutex();
                    return StatusCode(500, JsonConvert.SerializeObject(exc.Message));
                }
            }
            else
            {
                var retPayload = new
                {
                    Response = "token error"
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "GET",
                    URL = "AuthenticationAPI/KeyChanges/GetDeviceList",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }
    }
}
