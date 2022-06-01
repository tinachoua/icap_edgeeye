using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DeviceAPI.Models.Branch;
using MongoDB.Bson;
using ShareLibrary;
using ShareLibrary.AdminDB;
using ShareLibrary.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShareLibrary.DataTemplate;
using System.Threading;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DeviceAPI.Controllers
{
    public class BranchController : Controller
    {
        private IRedisCacheDispatcher _rcd;
        private IDevice _add;
        private IDataDBDispatcher _ddb;
        private IBranch _adb;
        private IEmployee _ade;
        private static Mutex mut = new Mutex();

        public BranchController(IRedisCacheDispatcher rcd, IDevice add, IDataDBDispatcher ddb, IBranch adb, IEmployee ade)
        {
            _rcd = rcd;
            _add = add;
            _ddb = ddb;
            _adb = adb;
            _ade = ade;
        }

        /// <summary>
        /// 11. Get branch list
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get branch list success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("BranchAPI/GetList")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        public IActionResult GetList([FromHeader]string token)
        {
            //icapContext ic = new icapContext();
            //AdminDBDispatcher._branch adb = new AdminDBDispatcher._branch();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                //ToDo : Now return all the branch data, needed by user to get group list.
                //List<Branch> branch = ic.Branch.ToList();
                try
                {
                    //List<Branch> branch = _adb.GetList();

                    //if (branch.Count == 0)
                    //{
                    //    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    //    {
                    //        Direction = true,
                    //        Name = user,
                    //        Method = "GET",
                    //        URL = "BranchAPI/GetList",
                    //        ResponseCode = 204,
                    //        Remark = "The branch could NOT be found!"
                    //    });

                    //    return StatusCode(204);
                    //}

                    //List<BranchItem> retList = new List<BranchItem>();

                    //foreach (Branch b in branch)
                    //{
                    //    retList.Add(new BranchItem { Id = b.Id, Name = b.Name });
                    //}

                    //var retPayload = new
                    //{
                    //    BranchList = retList.ToArray()
                    //};

                    var branchlist = _adb.GetBranchList();

                    if (branchlist[0] == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "BranchAPI/GetList",
                            ResponseCode = 204,
                            Remark = "The branch could NOT be found!"
                        });

                        return StatusCode(204);
                    }

                    var retPayload = new
                    {
                        List = branchlist
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "BranchAPI/GetList",
                        ResponseCode = 200,
                        Remark = "Get branch list successfully!"
                    });               

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                catch(Exception exc)
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
                        URL = "BranchAPI/GetList",
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
                    URL = "BranchAPI/GetList",
                    ResponseCode = 403,
                    Remark = "[BranchAPI/GetList]Request authentication token error."
                });
               
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 12. Get device list by branch
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="branchId">The target branch ID</param>
        /// <returns></returns>
        /// <response code="200">Get device list success</response>
        /// <response code="204">Device Not Found</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("BranchAPI/GetDeviceList")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(204)] 
        public IActionResult GetDevList([FromHeader]string token, [FromHeader]string branchId)
        {
            //icapContext ic = new icapContext();
            //AdminDBDispatcher._device adb = new AdminDBDispatcher._device();
            //DataDBDispatcher nodb = new DataDBDispatcher();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                int bid = 0;

                int.TryParse(branchId, out bid);

                //List<Device> device = ic.Device.Where(x => x.BranchId == bid).ToList();

                List<Device> device = _add.GetList(bid);

                if (device[0] == null)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "BranchAPI/GetDeviceList",
                        ResponseCode = 204,
                        Remark = "Device Not Found."
                    });

                    return StatusCode(204);
                }

                List<DevStatus> retList = new List<DevStatus>();

                //state 0 : green, state 1 :red, state 2 :grey
                foreach (Device b in device)
                {
                    int state = 0;
                    var queryObj = new
                    {
                        Dev = b.Name
                    };
                    List<BsonDocument> dbRet = _ddb.GetRawData("EventLog", JsonConvert.SerializeObject(queryObj), 0);
                    if (dbRet.Count != 0)
                    {
                        foreach(BsonDocument bdoc in dbRet)
                        {
                            if (!bdoc.GetValue("Checked").AsBoolean && !bdoc.GetValue("Message").AsString.ToUpper().Contains("OFFLINE"))
                            {
                                state = 1;
                            }
                        }
                        
                    }

                    if (1 != state)
                    {
                        int onlineFlag = 0;
                        string devOnline = _rcd.GetStatus(b.Name);
                        if (devOnline != null)
                        {
                            int.TryParse(devOnline, out onlineFlag); //0:offline 1:online
                        }

                        state = (onlineFlag == 1) ? 0 : 2; // 0: normal , 1:event, 2: offline

                    }
                    retList.Add(new DevStatus { Name = b.Name, Alias = b.Alias, State = state });
                }

                var retPayload = new
                {
                    DeviceList = retList.ToArray()
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "BranchAPI/GetDeviceList",
                    ResponseCode = 200,
                    Remark = "Get device list by branch success."
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
                    URL = "BranchAPI/GetDeviceList",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 13. Get device list by device name
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="devName">The target device name</param>
        /// <returns></returns>
        /// <response code="200">Get device list success</response>
        /// <response code="204">Device Not Found</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("BranchAPI/GetDeviceListByName")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        public IActionResult GetDevListByName([FromHeader]string token, [FromHeader]string devName)
        {
            //icapContext ic = new icapContext();
            //AdminDBDispatcher._device adb = new AdminDBDispatcher._device();
            // DataDBDispatcher nodb = new DataDBDispatcher();
            // RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                int bid = 0;

                //int.TryParse(branchId, out bid);

                //List<Device> device = ic.Device.Where(x => x.BranchId == bid).ToList();

                Device dev = _add.Get(devName);

                if (dev == null)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "BranchAPI/GetDeviceListByName",
                        ResponseCode = 204,
                        Remark = "Device Name Error."
                    });

                    var retPayload1 = new
                    {
                        Response = "Device Name Error"
                    };

                    return StatusCode(204, JsonConvert.SerializeObject(retPayload1));
                }

                List<Device> device = _add.GetList(1);

                List<DevStatus> retList = new List<DevStatus>();

                foreach (Device b in device)
                {
                    int state = 0;
                    var queryObj = new
                    {
                        Dev = b.Name
                    };
                    List<BsonDocument> dbRet = _ddb.GetRawData("EventLog", JsonConvert.SerializeObject(queryObj), 0);
                    if (dbRet.Count != 0)
                    {
                        foreach (BsonDocument bdoc in dbRet)
                        {
                            if (!bdoc.GetValue("Checked").AsBoolean && !bdoc.GetValue("Message").AsString.ToUpper().Contains("OFFLINE"))
                            {
                                state = 1;
                            }
                        }

                    }

                    if(1 != state)
                    {
                        int onlineFlag = 0;
                        string devOnline = _rcd.GetStatus(b.Name);
                        if (devOnline != null)
                        {
                            int.TryParse(devOnline, out onlineFlag);
                        }
                        state = (onlineFlag == 1) ? 0 : 2;
                    }

                    retList.Add(new DevStatus { Name = b.Name, State = state });
                }

                var retPayload = new
                {
                    DeviceList = retList.ToArray()
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "BranchAPI/GetDeviceListByName",
                    ResponseCode = 200,
                    Remark = "Get device list by device name success."
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
                    URL = "BranchAPI/GetDeviceListByName",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 17. Get branch loading
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="branchId">The target branch ID</param>
        /// <returns></returns>
        /// <response code="200">Get branch loading data success</response>
        /// <response code="204">Device Not Found</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("BranchAPI/GetBranchLoading")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)] 
        public IActionResult GetBranchLoading([FromHeader]string token, [FromHeader]string branchId)
        {
            //AdminDBDispatcher._device adb = new AdminDBDispatcher._device();
            //DataDBDispatcher nodb = new DataDBDispatcher();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                int bid = 0;

                int.TryParse(branchId, out bid);

                //List<Device> device = ic.Device.Where(x => x.BranchId == bid).ToList();

                List<Device> device = _add.GetList(bid);

                if (device[0] == null)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "BranchAPI/GetBranchLoading",
                        ResponseCode = 204,
                        Remark = "Device Not Found."
                    });

                    return StatusCode(204);
                }

                List<DevStatus> retList = new List<DevStatus>();

                object[] obj = new object[1];
                object[] obj1 = new object[5];
                object[] obj2 = new object[4];
                obj2[0] = "CPU > 80%, Memory > 80%";
                obj2[1] = "CPU < 80%, Memory > 80%";
                obj2[2] = "CPU < 80%, Memory < 80%";
                obj2[3] = "CPU > 80%, Memory < 80%";
                obj1[0] = obj2;
                List<object> th0 = new List<object>(),
                             th1 = new List<object>(),
                             th2 = new List<object>(),
                             th3 = new List<object>();
                foreach (Device b in device)
                {
                    BsonDocument bdoc = _ddb.GetLastRawData(string.Format("{0}-static", b.Name));
                    BsonDocument dybdoc = _ddb.GetLastRawData(string.Format("{0}-dynamic", b.Name));
                    double[] subobj = new double[2];
                    if (bdoc != null && dybdoc != null)
                    {
                        int memCap = Convert.ToInt32(CommonFunctions.GetData(bdoc["MEM"].AsBsonDocument["Cap"]));
                        subobj[0] = Convert.ToDouble(CommonFunctions.GetData(dybdoc["CPU"].AsBsonDocument["Usage"]));
                        subobj[1] = Math.Round((Convert.ToDouble(CommonFunctions.GetData(dybdoc["MEM"]["memUsed"])) / memCap) * 100.0, 2);
                        if (subobj[0] > 80)
                        {
                            if (subobj[1] > 80)
                            {
                                th0.Add(subobj);
                            }
                            else
                            {
                                th3.Add(subobj);
                            }
                        }
                        else
                        {
                            if (subobj[1] > 80)
                            {
                                th1.Add(subobj);
                            }
                            else
                            {
                                th2.Add(subobj);
                            }
                        }
                    }
                }

                obj1[1] = th0.ToArray();
                obj1[2] = th1.ToArray(); 
                 obj1[3] = th2.ToArray();
                obj1[4] = th3.ToArray();
                obj[0] = obj1;

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "BranchAPI/GetBranchLoading",
                    ResponseCode = 200,
                    Remark = "Get device list by branch success."
                });

                return StatusCode(200, JsonConvert.SerializeObject(obj));
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
                    URL = "BranchAPI/GetBranchLoading",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 5. Get image
        /// </summary>      
        /// <param name="token">token</param>
        /// <param name="braId">Branch Id</param>
        /// <returns></returns>
        [HttpGet("BranchAPI/GetImg")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult GetImg([FromHeader]string token, [FromHeader]string braId)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "BranchAPI/GetImg",
                ResponseCode = 0,
                Remark = ""
            });

            RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    string file = _adb.GetImgBase64(braId , "branches");

                    var retPayload = new
                    {
                        Response = file
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "BranchAPI/GetImg",
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
                        URL = "BranchAPI/GetImg",
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
                    URL = "BranchAPI/GetImg",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 6. Upload Image
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="braId">Branch Id</param>
        /// <param name="overwrite">When the file exists,whether overwrite.</param>
        /// <param name="files">Upload Images</param>
        /// <returns></returns>

        [HttpPost("BranchAPI/UploadImg")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult UploadImg([FromHeader]string token, [FromHeader]string braId, [FromHeader]string overwrite, [FromForm]List<IFormFile> files)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "POST",
                URL = "BranchAPI/UploadImg",
                ResponseCode = 0,
                Remark = ""
            });

            RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    if (!_adb.AllowedFileExtensions(files))
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "BranchAPI/UploadImg",
                            ResponseCode = 400,
                            Remark = "The File Extensions is unacceptable."
                        });

                        return StatusCode(400, JsonConvert.SerializeObject("The File Extensions is unacceptable."));
                    }

                    if (_adb.UploadImg(files, !(Int32.Parse(overwrite)).Equals(0), braId,"branches"))
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
                            URL = "BranchAPI/UploadImg",
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
                            URL = "BranchAPI/UploadImg",
                            ResponseCode = 403,
                            Remark = "The File Exists."
                        });

                        return StatusCode(403, JsonConvert.SerializeObject("The File Exists."));
                    }
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "BranchAPI/UploadImg",
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
                    Method = "POST",
                    URL = "BranchAPI/UploadImg",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 23. Create Branch 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="branchName">The branch name</param>
        /// <returns></returns>
        /// <response code="201">Create Group Success.</response>
        /// <response code="400">Data Error.</response>
        /// <response code="403">Token Errot</response>
        /// <response code="409">Name already exists.
        ///1. The identity token not found
        ///2. Not Admin
        ///</response>   
        [HttpPost("BranchAPI/Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        public IActionResult Create([FromHeader]string token, [FromBody]string branchName)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    if (!_ade.CheckAdmin(user))
                    {

                        var retPayload1 = new
                        {
                            ErrorCode = 1,
                            Response = "Sorry, you do not have access to create group."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "BranchAPI/Create",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do NOT have enough authorization!", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                    }

                    mut.WaitOne();

                    if (_adb.NameExists(branchName))
                    {
                        var retPayload1 = new
                        {
                            Response = "The group name already exists."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "BranchAPI/Create",
                            ResponseCode = 409,
                            Remark = "The group name already exists."
                        });
                        mut.ReleaseMutex();
                        return StatusCode(409, JsonConvert.SerializeObject(retPayload1));
                    }

                    _adb.Create(branchName);

                    var retPayload = new
                    {
                        Response = "Create the branch successfully!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "BranchAPI/Create",
                        ResponseCode = 201,
                        Remark = "Create the branch successfully!"
                    });
                    mut.ReleaseMutex();
                    return StatusCode(201, JsonConvert.SerializeObject(retPayload));
                }
                catch (DbUpdateException exc)
                {

                    var retPayload = new
                    {
                        Response = exc.InnerException.Message
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "BranchAPI/Create",
                        ResponseCode = 400,
                        Remark = exc.InnerException.Message
                    });
                    mut.ReleaseMutex();
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload));

                }
                catch (Exception exc)
                {
                    var retPayload = new
                    {
                        Response = "Failed to create group. Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "BranchAPI/Create",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    mut.ReleaseMutex();
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                }
            }
            else
            {
                var retPayload = new
                {
                    ErrorCode = 0,
                    Response = "You have been idle too long, please log-in again."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "POST",
                    URL = "BranchAPI/Create",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 24. Rename Group 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="branchInfo">The branch name</param>
        /// <returns></returns>
        /// <response code="200">Create Group Success.</response>
        /// <response code="400">Data Error.</response>
        /// <response code="403">Token Error.</response>
        /// <response code="409">Name already exists.
        ///1. The identity token not found
        ///2. Not Admin
        ///</response>   
        [HttpPut("BranchAPI/Rename")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        public IActionResult Rename([FromHeader]string token, [FromBody]BranchTemplate branchInfo)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    if (!_ade.CheckAdmin(user))
                    {

                        var retPayload1 = new
                        {
                            Response = string.Format("{0} do not have enough authorization!", user)
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "BranchAPI/Rename",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do NOT have enough authorization!", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                    }

                    mut.WaitOne();

                    if (_adb.NameExists(branchInfo.Name))
                    {
                        var retPayload1 = new
                        {
                            Response = "The group name already exists."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "BranchAPI/Rename",
                            ResponseCode = 409,
                            Remark = "The group name already exists."
                        });
                        mut.ReleaseMutex();
                        return StatusCode(409, JsonConvert.SerializeObject(retPayload1));
                    }

                    _adb.Update(branchInfo);

                    var retPayload = new
                    {
                        Response = "The group was renamed successfully."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "BranchAPI/Rename",
                        ResponseCode = 200,
                        Remark = "The group was renamed successfully."
                    });
                    mut.ReleaseMutex();
                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                catch (DbUpdateException exc)
                {

                    var retPayload = new
                    {
                        Response = exc.InnerException.Message
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "BranchAPI/Rename",
                        ResponseCode = 400,
                        Remark = exc.InnerException.Message
                    });
                    mut.ReleaseMutex();
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload));

                }
                catch (Exception exc)
                {
                    if(branchInfo !=null && branchInfo.Name == null)
                    {
                        var retPayload1 = new
                        {
                            Response = "Group name could not be null."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "BranchAPI/Rename",
                            ResponseCode = 400,
                            Remark = "Group name could not be null."
                        });
                        mut.ReleaseMutex();
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }

                    var retPayload = new
                    {
                        Response = "Failed to rename the group. Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "BranchAPI/Rename",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    mut.ReleaseMutex();
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                }
            }
            else
            {
                var retPayload = new
                {
                    Response = "You have been idle too long, please log-in again."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "PUT",
                    URL = "BranchAPI/Rename",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 24. Update Branch 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="updateBranch">The Branch Information</param>
        /// <returns></returns>
        /// <response code="200">Update Branch Success.</response>
        /// <response code="400">Data Error.</response>
        /// <response code="403">
        ///1. The identity token not found
        ///2. Not Admin
        ///</response>   
        //[HttpPut("BranchAPI/Update")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(403)]
        //public IActionResult Update([FromHeader]string token, [FromBody]BranchTemplate updateBranch)
        //{
        //    string user = _rcd.GetCache(0, token);
        //    if (user != null)
        //    {
        //        try
        //        {
        //            if (!_ade.CheckAdmin(user))
        //            {

        //                var retPayload = new
        //                {
        //                    Response = string.Format("{0} do not have enough authorization!", user)
        //                };

        //                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //                {
        //                    Direction = true,
        //                    Name = user,
        //                    Method = "PUT",
        //                    URL = "BranchAPI/Update",
        //                    ResponseCode = 403,
        //                    Remark = string.Format("{0} do not have enough authorization!", user)
        //                });

        //                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
        //            }

        //            _adb.Update(updateBranch);

        //            var retPayload1 = new
        //            {
        //                Response = "Update the branch name successfully!"
        //            };

        //            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //            {
        //                Direction = true,
        //                Name = user,
        //                Method = "PUT",
        //                URL = "BranchAPI/Update",
        //                ResponseCode = 200,
        //                Remark = "Update the branch successfully!"
        //            });

        //            return StatusCode(200, JsonConvert.SerializeObject(retPayload1));
        //        }
        //        catch (Exception exc)
        //        {
        //            if (updateBranch == null)
        //            {
        //                var retPayload = new
        //                {
        //                    Response = "Data Error! Please refresh the web page and try again."
        //                };

        //                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //                {
        //                    Direction = true,
        //                    Name = user,
        //                    Method = "PUT",
        //                    URL = "BranchAPI/Update",
        //                    ResponseCode = 400,
        //                    Remark = exc.Message
        //                });
        //                return StatusCode(400, JsonConvert.SerializeObject(retPayload));
        //            }
        //            else if (updateBranch.Name == null)
        //            {
        //                var retPayload = new
        //                {
        //                    Response = "The branch name field can NOT be blank!"
        //                };

        //                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //                {
        //                    Direction = true,
        //                    Name = user,
        //                    Method = "PUT",
        //                    URL = "BranchAPI/Update",
        //                    ResponseCode = 400,
        //                    Remark = "The branch name field can NOT be blank!"
        //                });
        //                return StatusCode(400, JsonConvert.SerializeObject(retPayload));
        //            }
        //            else
        //            {
        //                var retPayload1 = new
        //                {
        //                    Response = exc.Message
        //                };

        //                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //                {
        //                    Direction = true,
        //                    Name = user,
        //                    Method = "PUT",
        //                    URL = "BranchAPI/Update",
        //                    ResponseCode = 400,
        //                    Remark = exc.Message
        //                });
        //                return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var retPayload = new
        //        {
        //            Response = "Authentication Error!"
        //        };

        //        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //        {
        //            Direction = true,
        //            Name = user,
        //            Method = "PUT",
        //            URL = "BranchAPI/Update",
        //            ResponseCode = 403,
        //            Remark = "Request authentication token error."
        //        });

        //        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
        //    }
        //}

        /// <summary>
        /// 25. Delete Branch 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="id">The Group Id</param>
        /// <returns></returns>
        /// <response code="202">Delete Accepted.</response>
        /// <response code="400">Data Error.</response>
        /// <response code="403">
        ///1. The identity token not found
        ///2. Not Admin
        ///</response>   
        [HttpDelete("BranchAPI/Delete")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult Delete([FromHeader]string token, [FromHeader]string id)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    if (!_ade.CheckAdmin(user))
                    {

                        var retPayload = new
                        {
                            
                            Response = "Sorry, you do not have access to delete the group."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "DELETE",
                            URL = "BranchAPI/Delete",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do not have enough authorization!", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }

                    _adb.Delete(Int32.Parse(id));

                    var retPayload1 = new
                    {
                        Response = "Delete the group successfully!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = "BranchAPI/Delete",
                        ResponseCode = 202,
                        Remark = "Delete the group successfully!"
                    });

                    return StatusCode(202, JsonConvert.SerializeObject(retPayload1));
                }
                catch (ArgumentNullException exc)
                {
                    var retPayload1 = new
                    {
                        Response = "Please choose one branch!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = "BranchAPI/Delete",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                }
                catch (Exception exc)
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
                        URL = "BranchAPI/Delete",
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
                    Response = "You have been idle too long, please log-in again."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "DELETE",
                    URL = "BranchAPI/Delete",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 26. Get Device List by Branch Id
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="branchId">The Branch Id</param>
        /// <returns></returns>
        /// <response code="200">Get branch list success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="403">The identity token not found</response> 
        /// <response code="500">Internal Server Error.</response>
        [HttpGet("BranchAPI/GetDeviceAllocation")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetDeviceListByBranchId([FromHeader]string token, [FromHeader]string branchId)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    var retPayload = _adb.GetDeviceList(int.Parse(branchId));

                    if (retPayload[0] == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "BranchAPI/GetDeviceList",
                            ResponseCode = 204,
                            Remark = "The device could not be found!"
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "BranchAPI/GetDeviceList",
                        ResponseCode = 200,
                        Remark = "Get Branch Device List Success."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                catch (ArgumentNullException exc)
                {
                    var retPayload = new
                    {
                        Response = "Please choose one branch!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "BranchAPI/GetDeviceList",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                }
                catch (Exception exc)
                {
                    var retPayload = new
                    {
                        Response = "Data Error! Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "BranchAPI/GetDeviceList",
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
                    Method = "GET",
                    URL = "BranchAPI/GetDeviceList",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 28. Allocate Device
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="deviceAllocation">The Device Allocation</param>
        /// <returns></returns>
        /// <response code="201">Arrangement success.</response>
        /// <response code="400">There is exception.</response>
        /// <response code="403">The identity token not found</response>   
        [HttpPost("BranchAPI/AllocateDevice")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult DeviceAllocation([FromHeader]string token, [FromBody]DeviceAllocationTemplate deviceAllocation)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    if (!_ade.CheckAdmin(user))
                    {

                        var retPayload = new
                        {
                            Response = string.Format("{0} do not have enough authorization.", user)
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "BranchAPI/DeviceAllocation",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do not have enough authorization.", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }

                    //if (deviceAllocation.WidgetIdList.Length == 0)
                    //{
                    //    var retPayload = new
                    //    {
                    //        Response = "Please choose one card at least!"
                    //    };

                    //    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    //    {
                    //        Direction = true,
                    //        Name = user,
                    //        Method = "POST",
                    //        URL = "WidgetAPI/Create",
                    //        ResponseCode = 400,
                    //        Remark = "Please choose one card at least!"
                    //    });

                    //    return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    //}

                    _adb.DeviceAllocation(deviceAllocation);

                    var retPayload1 = new
                    {
                        Response = "Allocate device(s) successfully!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "BranchAPI/DeviceAllocation",
                        ResponseCode = 201,
                        Remark = "Allocate device(s) successfully!"
                    });

                    return StatusCode(201, JsonConvert.SerializeObject(retPayload1));
                }
                catch (DbUpdateException exc)
                {
                    if (exc.InnerException.Message.Contains("FK_Branch_To_BranchDeviceList"))
                    {
                        var retPayload = new
                        {
                            Response = "The branch could NOT be found! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "BranchAPI/DeviceAllocation",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }
                    else if (exc.InnerException.Message.Contains("FK_Device_To_BranchDeviceList"))
                    {
                        var retPayload = new
                        {
                            Response = "The device could NOT be found! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "BranchAPI/DeviceAllocation",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Allocate device Failed! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "BranchAPI/DeviceAllocation",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }

                }
                catch (Exception exc)
                {
                    if (deviceAllocation == null)
                    {
                        var retPayload1 = new
                        {
                            Response = "Data Format Error! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "BranchAPI/DeviceAllocation",
                            ResponseCode = 400,
                            Remark = exc.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                    else if (deviceAllocation.DeviceIdList.Length==0)
                    {
                        var retPayload1 = new
                        {
                            Response = "Please choose one device at least!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "BranchAPI/DeviceAllocation",
                            ResponseCode = 400,
                            Remark = exc.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Allocate device Failed! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "BranchAPI/DeviceAllocation",
                            ResponseCode = 400,
                            Remark = exc.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }
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
                    Method = "POST",
                    URL = "BranchAPI/DeviceAllocation",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 29. Allocate Device
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="branchId">The branch ID</param>
        /// <returns></returns>
        /// <response code="201">Arrangement success.</response>
        /// <response code="400">There is exception.</response>
        /// <response code="403">The identity token not found</response>   
        [HttpGet("BranchAPI/Edit")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult Edit([FromHeader]string token, [FromHeader]string branchId)
        {
            //AdminDBDispatcher._widget adw = new AdminDBDispatcher._widget();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "BranchAPI/Edit",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    BranchSettingInfo branchInfo = new BranchSettingInfo();
                    branchInfo.Detail = _adb.GetInfo(Int32.Parse(branchId));

                    if (branchInfo.Detail == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "BranchAPI/Edit",
                            ResponseCode = 204,
                            Remark = "Group could NOT be found!"
                        });
                        return StatusCode(204);
                    }
                    else
                    {
                        var deviceRepo = _add.GetDeviceList();

                        if (null != branchInfo.Detail.Selected)
                        {
                            foreach (SelectOptionTemplate option in branchInfo.Detail.Selected)
                            {
                                deviceRepo = deviceRepo.Where(id => id.Id != option.Id).ToArray();
                            }
                        }
                        //branchInfo.DeviceOption = _add.GetDeviceList();

                        branchInfo.Unselected = deviceRepo;

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "BranchAPI/Edit",
                            ResponseCode = 200,
                            Remark = "Get Group Successfully!"
                        });

                        return StatusCode(200, JsonConvert.SerializeObject(branchInfo));
                    }
                }
                catch (ArgumentNullException exc)
                {
                    var retPayload = new
                    {
                        Response = "Please choose one group."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "BranchAPI/Edit",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                }
                catch (Exception exc)
                {
                    var retPayload = new
                    {
                        Response = "Data Error! Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "BranchAPI/Edit",
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
                    Response = "You have been idle too long, please log-in again."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "BranchAPI/Edit",
                    ResponseCode = 403,
                    Remark = "[BranchAPI/Edit]Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 30. Save group information.
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="branchInfo">The branch ID</param>
        /// <returns></returns>
        /// <response code="201">Arrangement success.</response>
        /// <response code="400">There is exception.</response>
        /// <response code="403">The identity token not found.</response>   
        /// <response code="409">Name already exists.</response>
        [HttpPut("BranchAPI/Save")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        public IActionResult Save([FromHeader]string token, [FromBody]BranchTemplate branchInfo)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    if (!_ade.CheckAdmin(user))
                    {
                        var retPayload = new
                        {
                            Response = "Sorry, you do not have access to edit the group."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "BranchAPI/Save",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do not have enough authorization.", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }

                    //mut.WaitOne();

                    //if ((branchInfo.Name != null) && (_adb.NameExists(branchInfo.Name, branchInfo.Id)))
                    //{
                    //    var retPayload1 = new
                    //    {
                    //        Response = "The group name already exists."
                    //    };

                    //    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    //    {
                    //        Direction = true,
                    //        Name = user,
                    //        Method = "PUT",
                    //        URL = "BranchAPI/Save",
                    //        ResponseCode = 409,
                    //        Remark = "The group name already exists."
                    //    });
                    //    mut.ReleaseMutex();
                    //    return StatusCode(409, JsonConvert.SerializeObject(retPayload1));
                    //}

                    if (false == _adb.SaveInfo(branchInfo))
                    {
                        var retPayload1 = new
                        {
                            Response = "The GROUP could NOT be found! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "BranchAPI/Save",
                            ResponseCode = 400,
                            Remark = "The GROUP could NOT be found! Please refresh the web page and try again."
                        });
                        //mut.ReleaseMutex();
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                    else
                    {
                        var retPayload1 = new
                        {
                            Response = "Update the group successfully!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "BranchAPI/Save",
                            ResponseCode = 200,
                            Remark = "Update the group successfully!"
                        });

                        //mut.ReleaseMutex();
                        return StatusCode(200, JsonConvert.SerializeObject(retPayload1));
                    }
                }
                catch (DbUpdateException exc)
                {

                    if (exc.InnerException.Message.Contains("FK_Device_To_BranchDeviceList"))
                    {
                        var retPayload = new
                        {
                            Response = "The DEVICE could NOT be found! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "BranchAPI/Save",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        //mut.ReleaseMutex();
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Fail to edit group! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "BranchAPI/Save",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        //mut.ReleaseMutex();
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }

                }
                catch (Exception exc)
                {
                    if (branchInfo == null)
                    {
                        var retPayload1 = new
                        {
                            Response = "Data Error! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "BranchAPI/Save",
                            ResponseCode = 400,
                            Remark = exc.Message
                        });
                        //mut.ReleaseMutex();
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Fail to edit group! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "BranchAPI/Save",
                            ResponseCode = 400,
                            Remark = exc.Message
                        });
                        //mut.ReleaseMutex();
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }
                }
            }
            else
            {
                var retPayload = new
                {
                    Response = "You have been idle too long, please log-in again."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "PUT",
                    URL = "BranchAPI/Save",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 31. Get Group's Device Setting
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="id">The branch ID</param>
        /// <returns></returns>
        /// <response code="201">Arrangement success.</response>
        /// <response code="400">There is exception.</response>
        /// <response code="403">The identity token not found</response>   
        [HttpGet("BranchAPI/GetDevice")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult GetGroupDeviceSetting([FromHeader]string token, int id)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "BranchAPI/GetDevice",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    BranchSettingInfo branchInfo = new BranchSettingInfo();
                    branchInfo.Detail = _adb.GetInfo(id);

                    if (branchInfo.Detail == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "BranchAPI/GetDevice",
                            ResponseCode = 204,
                            Remark = "Group could NOT be found!"
                        });
                        return StatusCode(204);
                    }
                    else
                    {
                        var deviceRepo = _add.GetDeviceList();

                        if (null != branchInfo.Detail.Selected)
                        {
                            foreach (SelectOptionTemplate option in branchInfo.Detail.Selected)
                            {
                                deviceRepo = deviceRepo.Where(r => r.Id != option.Id).ToArray();
                            }
                        }
                        //branchInfo.DeviceOption = _add.GetDeviceList();

                        branchInfo.Unselected = deviceRepo;

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "BranchAPI/GetDevice",
                            ResponseCode = 200,
                            Remark = "Get Group Successfully!"
                        });

                        return StatusCode(200, JsonConvert.SerializeObject(branchInfo));
                    }
                }
                catch (Exception exc)
                {
                    var retPayload = new
                    {
                        Response = "Failed to get settings. Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "BranchAPI/GetDevice",
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
                    Response = "You have been idle too long, please log-in again."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "BranchAPI/GetDevice",
                    ResponseCode = 403,
                    Remark = "[BranchAPI/GetDevice]Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 32. Get Group
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="branchId">The identity group</param>
        /// <returns></returns>
        /// <response code="200">Get branch list success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("BranchAPI/Branch/{branchId}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        public IActionResult GetSpecificGroup([FromHeader]string token, int branchId)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    Device[] devices = _add.GetDeviceList(branchId);

                    if (devices.Length == 0)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "GroupAPI/Group",
                            ResponseCode = 200,
                            Remark = "There is not any device in this group."
                        });

                        return StatusCode(204);
                    }

                    object grouploading = null;
                    BsonDocument bdocStatic = _ddb.GetLastRawData(string.Format("{0}-static", devices[0].Name));
                    Int64 storageSize = 0;
                    List<string> storList = new List<string>();
                    var memCAP = -1;

                    string[] items = new string[4];
                    object[] value = new object[4];
                    items[0] = "CPU > 80%, Memory > 80%";
                    items[1] = "CPU < 80%, Memory > 80%";
                    items[2] = "CPU < 80%, Memory < 80%";
                    items[3] = "CPU > 80%, Memory < 80%";
                    List<object> th0 = new List<object>(),
                                    th1 = new List<object>(),
                                    th2 = new List<object>(),
                                    th3 = new List<object>();
                    foreach (Device b in devices)
                    {
                        BsonDocument bdoc = _ddb.GetLastRawData(string.Format("{0}-static", b.Name));
                        BsonDocument dybdoc = _ddb.GetLastRawData(string.Format("{0}-dynamic", b.Name));

                        if (bdoc != null && dybdoc != null)
                        {
                            int memCap = Convert.ToInt32(CommonFunctions.GetData(bdoc["MEM"].AsBsonDocument["Cap"]));
                            double cpuLoading = Convert.ToDouble(CommonFunctions.GetData(dybdoc["CPU"].AsBsonDocument["Usage"]));
                            double memLoading = Math.Round((Convert.ToDouble(CommonFunctions.GetData(dybdoc["MEM"]["memUsed"])) / memCap) * 100.0, 2);

                            if (cpuLoading > 80)
                            {
                                if (memLoading > 80)
                                {
                                    th0.Add(new
                                    {
                                        x = cpuLoading,
                                        y = memLoading
                                    });
                                }
                                else
                                {
                                    th3.Add(new
                                    {
                                        x = cpuLoading,
                                        y = memLoading
                                    });
                                }
                            }
                            else
                            {
                                if (memLoading > 80)
                                {
                                    th1.Add(new
                                    {
                                        x = cpuLoading,
                                        y = memLoading
                                    });
                                }
                                else
                                {
                                    th2.Add(new
                                    {
                                        x = cpuLoading,
                                        y = memLoading
                                    });
                                }
                            }
                        }
                    }
                    value[0] = th0.ToArray();
                    value[1] = th1.ToArray();
                    value[2] = th2.ToArray();
                    value[3] = th3.ToArray();

                    grouploading = new
                    {
                        Items = items,
                        Value = value
                    };


                    var retPayload = new
                    {
                        Devicelist = devices,
                        Grouploading = grouploading
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "GroupAPI/Group",
                        ResponseCode = 200,
                        Remark = "Get branch list successfully!"
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
                        URL = "GroupAPI/Group",
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
                    URL = "GroupAPI/Group",
                    ResponseCode = 403,
                    Remark = "Token Error"
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 32. Get Group
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get branch list success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("BranchAPI/Branch")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        public IActionResult GetGroup([FromHeader]string token)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    var grouplist = _adb.GetBranchList();

                    if (grouplist[0] == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "GroupAPI/Group",
                            ResponseCode = 204,
                            Remark = "The group could NOT be found!"
                        });

                        return StatusCode(204);
                    }

                    Device[] devices = _add.GetDeviceList(grouplist[0].Id);
                    object grouploading = null;
                    if (devices.Length > 0)
                    {
                        BsonDocument bdocStatic = _ddb.GetLastRawData(string.Format("{0}-static", devices[0].Name));
                        Int64 storageSize = 0;
                        List<string> storList = new List<string>();
                        var memCAP = -1;

                        string[] items = new string[4];
                        object[] value = new object[4];
                        items[0] = "CPU > 80%, Memory > 80%";
                        items[1] = "CPU < 80%, Memory > 80%";
                        items[2] = "CPU < 80%, Memory < 80%";
                        items[3] = "CPU > 80%, Memory < 80%";
                        List<object> th0 = new List<object>(),
                                     th1 = new List<object>(),
                                     th2 = new List<object>(),
                                     th3 = new List<object>();
                        foreach (Device b in devices)
                        {
                            BsonDocument bdoc = _ddb.GetLastRawData(string.Format("{0}-static", b.Name));
                            BsonDocument dybdoc = _ddb.GetLastRawData(string.Format("{0}-dynamic", b.Name));

                            if (bdoc != null && dybdoc != null)
                            {
                                int memCap = Convert.ToInt32(CommonFunctions.GetData(bdoc["MEM"].AsBsonDocument["Cap"]));
                                double cpuLoading = Convert.ToDouble(CommonFunctions.GetData(dybdoc["CPU"].AsBsonDocument["Usage"]));
                                double memLoading = Math.Round((Convert.ToDouble(CommonFunctions.GetData(dybdoc["MEM"]["memUsed"])) / memCap) * 100.0, 2);

                                if (cpuLoading > 80)
                                {
                                    if (memLoading > 80)
                                    {
                                        th0.Add(new
                                        {
                                            x = cpuLoading,
                                            y = memLoading
                                        });
                                    }
                                    else
                                    {
                                        th3.Add(new
                                        {
                                            x = cpuLoading,
                                            y = memLoading
                                        });
                                    }
                                }
                                else
                                {
                                    if (memLoading > 80)
                                    {
                                        th1.Add(new
                                        {
                                            x = cpuLoading,
                                            y = memLoading
                                        });
                                    }
                                    else
                                    {
                                        th2.Add(new
                                        {
                                            x = cpuLoading,
                                            y = memLoading
                                        });
                                    }
                                }
                            }
                        }
                        value[0] = th0.ToArray();
                        value[1] = th1.ToArray();
                        value[2] = th2.ToArray();
                        value[3] = th3.ToArray();

                        grouploading = new
                        {
                            Items = items,
                            Value = value
                        };
                    }


                    var retPayload = new
                    {
                        Grouplist = grouplist,
                        Devicelist = devices,
                        Grouploading = grouploading
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "GroupAPI/Group",
                        ResponseCode = 200,
                        Remark = "Get branch list successfully!"
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
                        URL = "GroupAPI/Group",
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
                    URL = "GroupAPI/Group",
                    ResponseCode = 403,
                    Remark = "Token Error"
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

    }
}
