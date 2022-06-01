using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DeviceAPI.Models.Remote;
using ShareLibrary;
using ShareLibrary.AdminDB;
using ShareLibrary.DataTemplate;
using MongoDB.Bson;
using System.Net.Http.Headers;
using System.IO;
using ShareLibrary.Interface;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DeviceAPI.Controllers
{
    public class DeviceController : Controller
    {

        private IRedisCacheDispatcher _rcd;
        private IDevice _add;
        private IDataDBDispatcher _ddb;
        private IRemoteCommandSender _remoteCmdSender;
        private IPermission _adper;

        //private IFormCollection _fc;

        public DeviceController(IRedisCacheDispatcher rcd, IDevice add, IDataDBDispatcher ddb, IRemoteCommandSender remoteCmdSender, IPermission adper)
        {
            _rcd = rcd;
            _add = add;
            _ddb = ddb;
            _remoteCmdSender = remoteCmdSender;
            _adper = adper;
        }

        /// <summary>
        /// 5. Get device information
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="devName">Device Name</param>
        /// <returns></returns>
        /// <response code="200">Get device profile success</response>
        /// <response code="204">Device not found</response>
        /// <response code="403">The identity token not found</response> 
        [HttpGet("DeviceAPI/Get")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]        
        public IActionResult Get([FromHeader]string token, string devName)
        {
            //AdminDBDispatcher._device adb = new AdminDBDispatcher._device();
           // DataDBDispatcher ddb = new DataDBDispatcher();
           // RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                DeviceProfileTemplate dev = _add.GetDeviceProfile(devName);

                if (dev == null)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DeviceAPI/Get",
                        ResponseCode = 204,
                        Remark = $"Device {devName} not found."
                    });

                    return StatusCode(204);
                }

                BsonDocument bsonStatic = _ddb.GetLastRawData(string.Format("{0}-static", devName));

                if (bsonStatic != null)
                {
                    dev.Longitude = Convert.ToDouble(CommonFunctions.GetData(bsonStatic["Sys"].AsBsonDocument["Longitude"]));
                    dev.Latitude = Convert.ToDouble(CommonFunctions.GetData(bsonStatic["Sys"].AsBsonDocument["Latitude"]));
                }

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "DeviceAPI/Get",
                    ResponseCode = 200,
                    Remark = $"[Device/Get]Get device {devName} profile success."
                });

                return StatusCode(200, JsonConvert.SerializeObject(dev));
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
                    URL = "DeviceAPI/Get",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 6. Update device information
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="devProfile">The device profile need to update</param>
        /// <returns></returns>
        /// <response code="202">Update device profile success</response>        
        /// <response code="304">
        ///                      1.Update device profile fail
        ///                      2.Device not found, and not modified.</response>
        /// <response code="403">The identity token not found</response>
        [HttpPut("DeviceAPI/Update")]
        [ProducesResponseType(202)] 
        [ProducesResponseType(304)]
        [ProducesResponseType(403)]        
        public IActionResult Update([FromHeader]string token, [FromBody]DeviceProfileTemplate devProfile)
        {
           // AdminDBDispatcher._device adb = new AdminDBDispatcher._device();
           // DataDBDispatcher ddb = new DataDBDispatcher();
           // RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                if (!_adper.CheckUpdatePermission(user, DataDefine.PermissionFlag.DeviceSetting))
                {
                    var retPayload1 = new
                    {
                        Response = ReturnCode.PermissionDenied
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "DeviceAPI/Update",
                        ResponseCode = 403,
                        Remark = "Permission Denied"
                    });
                    return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                }

                bool? ret = _add.UpdateDeviceProfile(devProfile);

                if (ret == null)
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
                        URL = "Device/Update",
                        ResponseCode = 304,
                        Remark = $"Device {devProfile.DevName} not found."
                    });

                    return StatusCode(304, JsonConvert.SerializeObject(retPayload1));
                }
                else if(ret == false)
                {
                    var retPayload = new
                    {
                        Response = "Fail"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "Device/Update",
                        ResponseCode = 304,
                        Remark = $"Update device {devProfile.DevName} profile fail."
                    });

                    return StatusCode(304, JsonConvert.SerializeObject(retPayload));
                }
                else
                {

                    var retPayload = new
                    {
                        Response = "Success"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "Device/Update",
                        ResponseCode = 202,
                        Remark = $"Update device {devProfile.DevName} profile success."
                    });

                    return StatusCode(202, JsonConvert.SerializeObject(retPayload));
                }
                
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
                    URL = "Device/Update",
                    ResponseCode = 403,
                    Remark = $"Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 7. Delete device
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="devName">Device Name</param>
        /// <returns></returns>
        /// <response code="202">Delete device success</response>
        /// <response code="304">1. Delete device fail 2. Device Not Found</response>
        /// <response code="403">The identity token not found</response>

        [HttpDelete("DeviceAPI/Delete")]
        [ProducesResponseType(202)]
        [ProducesResponseType(304)]
        [ProducesResponseType(403)]        
        public IActionResult Delete([FromHeader]string token, [FromHeader]string devName)
        {
            //AdminDBDispatcher._device adb = new AdminDBDispatcher._device();
            //DataDBDispatcher ddb = new DataDBDispatcher();
           // RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                //bool ret = _add.Delete(devName);
                try
                {
                    int status=0;
                    string devStatus = _rcd.GetStatus(devName);
                    if (devStatus != null)
                    {
                        int.TryParse(devStatus, out status);
                    }

                    if(status==1)
                    {
                        var retPayload = new
                        {
                            Response = "Can not delete the device while it is online!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "DELETE",
                            URL = "DeviceAPI/Delete",
                            ResponseCode = 403,
                            Remark = "Can not delete the device while it is online!"
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }

                    if (_add.Delete(devName))
                    {
                        var retPayload = new
                        {
                            Response = "Delete the device successfully!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "DELETE",
                            URL = "DeviceAPI/Delete",
                            ResponseCode = 202,
                            Remark = "Delete the device successfully!"
                        });

                        return StatusCode(202, JsonConvert.SerializeObject(retPayload));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "The device could NOT be found!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "DELETE",
                            URL = "DeviceAPI/Delete",
                            ResponseCode = 400,
                            Remark = "The device could NOT be found!"
                        });

                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }
                }

                catch(Exception exc)
                {
                    var retPayload = new
                    {
                        Response = exc.Message
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = "DeviceAPI/Delete",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                }
            }
            else
            {
                var retPayload = new
                {
                    Response = "Authentication Error!"
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "DELETE",
                    URL = "DeviceAPI/Delete",
                    ResponseCode = 403,
                    Remark = "[Device/Delete]Request authentication token error."
                });
        
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 8. Send remote command
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="remoteCmd">The remote command</param>
        /// <returns></returns>
        /// <response code="202">Send device remote command success.</response>
        /// <response code="403">The identity token not found</response>
        [HttpPost("DeviceAPI/Remote")]
        [ProducesResponseType(202)]
        [ProducesResponseType(403)]
        public IActionResult Remote([FromHeader]string token, [FromBody]RemoteCommand remoteCmd)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                Device dev = _add.Get(remoteCmd.devName);

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
                        Method = "POST",
                        URL = "DeviceAPI/Remote",
                        ResponseCode = 400,
                        Remark = $"Device {remoteCmd.devName} not found."
                    });

                    return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                }

                _remoteCmdSender.SendRemoteCommand(remoteCmd.devName, remoteCmd.target, remoteCmd.cmd);

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "POST",
                    URL = "DeviceAPI/Remote",
                    ResponseCode = 202,
                    Remark = "Send device remote command success."
                });

                return StatusCode(202);
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
                    Method = "POST",
                    URL = "DeviceAPI/Remote",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 9. Get image
        /// </summary>      
        /// <param name="token">token</param>
        /// <param name="devName">Device Name</param>
        /// <returns></returns>
        [HttpGet("DeviceAPI/GetImg")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult GetImg([FromHeader]string token, [FromHeader]string devName)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "DeviceAPI/GetImg",
                ResponseCode = 0,
                Remark = ""
            });

            RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    string file = _add.GetImgBase64(devName, "devices");

                    var retPayload = new
                    {
                        Response = file
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DeviceAPI/GetImg",
                        ResponseCode = 200,
                        Remark = "Get Image Success."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));

                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DeviceAPI/GetImg",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });

                    return StatusCode(400, JsonConvert.SerializeObject(exc.Message));
                }
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
                    URL = "DeviceAPI/GetImg",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 10. Upload image
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="devName">Device Name</param>
        /// <param name="overwrite">When the file exists,whether overwrite.</param>
        /// <param name="files">Upload Images</param>
        /// <returns></returns>
        [HttpPost("DeviceAPI/UploadImg")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult UploadImg([FromHeader]string token,[FromHeader]string devName,[FromHeader]string overwrite,[FromForm]List<IFormFile> files)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "POST",
                URL = "DeviceAPI/UploadImg",
                ResponseCode = 0,
                Remark = ""
            });

            RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if(user!=null)
            {
                try
                {
                    if(!_add.AllowedFileExtensions(files))
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "DeviceAPI/UploadImg",
                            ResponseCode = 400,
                            Remark = "The File Extensions is unacceptable."
                        });

                        return StatusCode(400, JsonConvert.SerializeObject("The File Extensions is unacceptable."));
                    }

                    if (_add.UploadImg(files, !(Int32.Parse(overwrite)).Equals(0), devName,"devices"))
                    {
                        long size = 0;

                        foreach (var file in files)
                            size += file.Length;

                        string message = $"{files.Count} file(s) {size} bytes uploaded successfully!";

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "DeviceAPI/UploadImg",
                            ResponseCode = 200,
                            Remark = message
                        });

                        return StatusCode(200, JsonConvert.SerializeObject(message));
                    }
                    else
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "DeviceAPI/UploadImg",
                            ResponseCode = 403,
                            Remark = "The File Exists."
                        });

                        return StatusCode(403, JsonConvert.SerializeObject("The File Exists."));
                    }
                }
                catch(Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "DeviceAPI/UploadImg",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });

                    return StatusCode(400, JsonConvert.SerializeObject("Upload Failed"));
                }
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
                    Method = "POST",
                    URL = "DeviceAPI/UploadImg",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 28. Screenshot  
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="devName">The device name</param>
        /// <returns></returns>
        /// <response code="200">Get device list successfully.</response>
        /// <response code="204">The device could NOT be found.</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("DeviceAPI/Screenshot")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetDeviceScreenshot([FromHeader]string token, string devName)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = "DeviceAPI/Screenshot",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var alias = _add.GetAlias(devName);
                    string jsonStr = _add.Screenshot(devName);

                    if (jsonStr != null)
                    {
                        JObject jsonObj = JObject.Parse(jsonStr);
                        jsonObj.Add("Alias", alias);

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "Get",
                            URL = "DeviceAPI/Screenshot",
                            ResponseCode = 200,
                            Remark = "Screenshot Successfully"
                        });

                        return StatusCode(200, JsonConvert.SerializeObject(jsonObj));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Get Image failed"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "Get",
                            URL = "DeviceAPI/Screenshot",
                            ResponseCode = 500,
                            Remark = "Get Image failed"
                        });

                        return StatusCode(500, JsonConvert.SerializeObject(retPayload));
                    }
                }
                catch (Exception exc)
                {
                    var retPayload = new
                    {
                        Response = "Internael Server Error!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DeviceAPI/Screenshot",
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
                    Response = "Authentication Error!"
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "DeviceAPI/Screenshot",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 29. Screenshot delete image
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="devName">Device Name</param>
        /// <param name="id">The image id</param>
        /// <returns></returns>
        /// <response code="204">Delete screenshot image successfully.</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internael Server Error</response>
        [HttpDelete("DeviceAPI/Screenshot/Image/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult DeleteScreenshotImage([FromHeader]string token, int id, string devName)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Delete",
                URL = "DeviceAPI/Screenshot/Image/{id}",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    NotifyCoreService handler = new NotifyCoreService();
                    handler.Connect("172.30.0.11");
                    handler.Send("INNO", "D", JsonConvert.SerializeObject(new
                    {
                        Cmd = (int)ShareLibrary.Action.DeleteScreenshotImg,
                        DevName = devName,
                        ImgId = new int[] { id}
                    }));
                    SocketDispatcher.PACKAGE pkg = new SocketDispatcher.PACKAGE();
                    string state = handler.ReceivePackage(pkg);

                    if (state == "1")
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "Delete",
                            URL = "DeviceAPI/Screenshot/Image/{id}",
                            ResponseCode = 204 ,
                            Remark = "Delete image successfully."
                        });

                        return StatusCode(204);
                    }
                    else if (state == "2")
                    {
                        var retPayload = new
                        {
                            Response = "The image no longer exists."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "Delete",
                            URL = "DeviceAPI/Screenshot/Image/{id}",
                            ResponseCode = 200,
                            Remark = "The image no longer exists."
                        });

                        return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Delete image failed."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "Delete",
                            URL = "DeviceAPI/Screenshot/Image/{id}",
                            ResponseCode = 500,
                            Remark = $"Delete image failed. State: {state}"
                        });

                        return StatusCode(500, JsonConvert.SerializeObject(Response));
                    }
                }
                catch (Exception exc)
                {
                    var retPayload = new
                    {
                        Response = "Internael Server Error!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "Delete",
                        URL = "DeviceAPI/Screenshot/Image/{id}",
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
                    Response = "Authentication Error!"
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "Delete",
                    URL = "DeviceAPI/Screenshot/Image/{id}",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 27. Get All Devices
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get device list successfully.</response>
        /// <response code="204">The device could NOT be found.</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("DeviceAPI/Devices/List")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetDeviceList([FromHeader]string token)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = "DeviceAPI/Devices/List",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var retPayload = _add.GetDeviceList();

                    if (retPayload == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DeviceAPI/Devices/List",
                            ResponseCode = 204,
                            Remark = "The device could not be found."
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DeviceAPI/Devices/List",
                        ResponseCode = 200,
                        Remark = "Get Device List Success!"
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DeviceAPI/Devices/List",
                        ResponseCode = 500,
                        Remark = exc.Message
                    });
                    return StatusCode(500);
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
                    URL = "DeviceAPI/Devices/List",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }


        /// <summary>
        /// 6. Update device alias
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="data">The device profile need to update</param>
        /// <param name="devName">The device name</param>
        /// <returns></returns>
        /// <response code="204">Update device alias success</response>        
        /// <response code="400">Device not found</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut("DeviceAPI/{devName}/Alias")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(410)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult UpdateAlias([FromHeader]string token, [FromBody]JObject data, string devName)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    if (!_adper.CheckUpdatePermission(user, DataDefine.PermissionFlag.DeviceSetting))
                    {
                        var retPayload1 = new
                        {
                            Response = ReturnCode.PermissionDenied
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = $"DeviceAPI/{devName}/Alias",
                            ResponseCode = 403,
                            Remark = "Permission Denied"
                        });
                        return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                    }

                    string alias = data.GetValue("Alias").ToString();

                    if (_add.UpdateAlias(devName, alias))
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = $"DeviceAPI/{devName}/Alias",
                            ResponseCode = 204,
                            Remark = $"Alias changed to ${alias} successfully."
                        });

                        return StatusCode(204);
                    }
                    else
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = $"DeviceAPI/{devName}/Alias",
                            ResponseCode = 410,
                            Remark = "The device could NOT be found!"
                        });
                        return StatusCode(410);
                    }
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = $"DeviceAPI/{devName}/Alias",
                        ResponseCode = 500,
                        Remark = exc.Message
                    });
                    return StatusCode(500);
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
                    Method = "PUT",
                    URL = $"DeviceAPI/{devName}/Alias",
                    ResponseCode = 403,
                    Remark = "Token Error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }
        [HttpPost("DeviceAPI/RemoteReboot")]
        [ProducesResponseType(202)]
        [ProducesResponseType(403)]
        public IActionResult RemoteReboot([FromHeader] string token, [FromBody] RemoteNativeCommand remoteCmd)
        {
            //List<string> devList = _add.GetList();

            string user = _rcd.GetCache(0, token);
            //_remoteCmdSender.SendRemoteNativeCommand(remoteCmd.devName, remoteCmd.cmd);
            if (user != null)
            {
                Device dev = _add.Get(remoteCmd.devName);

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
                        Method = "POST",
                        URL = "DeviceAPI/RemoteReboot",
                        ResponseCode = 400,
                        Remark = $"Device {remoteCmd.devName} not found."
                    });

                    return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                }

                _remoteCmdSender.SendRemoteNativeCommand(remoteCmd.devName, remoteCmd.cmd);

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "POST",
                    URL = "DeviceAPI/RemoteReboot",
                    ResponseCode = 202,
                    Remark = "Send device remote command success."
                });

                return StatusCode(202);
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
                    Method = "POST",
                    URL = "DeviceAPI/RemoteReboot",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }
    }
}
