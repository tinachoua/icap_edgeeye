using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using DashboardAPI.Models.Setting;
using Newtonsoft.Json.Linq;
using ShareLibrary;
using ShareLibrary.DataTemplate;
using ShareLibrary.Interface;
using System.Threading;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DashboardAPI.Controllers
{
    public class SettingController : Controller
    {
        private IRedisCacheDispatcher _rcd;
        private IDataDBDispatcher _ddb;
        private IThreshold _adth;
        private IData _addata;
        private IEmployee _ade;
        private IBranch _adb;
        private static Mutex mut = new Mutex();
        private ICustomizedMap _adcm;
        private IPermission _adper;
        public SettingController(IRedisCacheDispatcher rcd, IDataDBDispatcher ddb, IThreshold adth, IData addata, IEmployee ade, IBranch adb, ICustomizedMap adcm, IPermission adper)
        {
            _rcd = rcd;
            _ddb = ddb;
            _adth = adth;
            _addata = addata;
            _ade = ade;
            _adb = adb;
            _adcm = adcm;
            _adper = adper;
        }

        /// <summary>
        /// 19. Get threshold list
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get branch list success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("SettingAPI/GetThresholdList")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        public IActionResult GetThresholdList([FromHeader]string token)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    var thresholdlist = _adth.GetThresholdList();

                    if (thresholdlist == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "SettingAPI/GetThresholdList",
                            ResponseCode = 204,
                            Remark = "The threshold rule(s) could NOT be found!"
                        });

                        return StatusCode(204);
                    }

                    var retPayload = new
                    {
                        List = thresholdlist
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "SettingAPI/GetThresholdList",
                        ResponseCode = 200,
                        Remark = "Get threshold rule(s) successfully."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                catch (Exception exc)
                {
                    var retPayload = new
                    {
                        Response = "Failed to get the threshold rule(s). Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "SettingAPI/GetThresholdList",
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
                    Response = "You have been idle too long, please log-in again."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "SettingAPI/GetThresholdList",
                    ResponseCode = 403,
                    Remark = "[SettingAPI/GetThresholdList]Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 20. Create Threshold Rule 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="thInfo">The threshold rule information</param>
        /// <returns></returns>
        /// <response code="201">Create Threshold Rule Successfully.</response>
        /// <response code="400">Data Error.</response>
        /// <response code="409">Name already exists.
        ///1. The identity token not found
        ///2. Not Admin
        ///</response>   
        [HttpPost("SettingAPI/Threshold/Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        public IActionResult CreateThreshold([FromHeader]string token, [FromBody]ThresholdTemplate thInfo)
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
                            Response = "Sorry, you do not have access to create threshold rule."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "SettingAPI/Threshold/Create",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do NOT have enough authorization!", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                    }

                    mut.WaitOne();

                    if (_adth.NameExists(thInfo.Setting.Name))
                    {
                        var retPayload1 = new
                        {
                            Response = "The threshold rule's name already exists."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "SettingAPI/Threshold/Create",
                            ResponseCode = 409,
                            Remark = "The threshold rule's name already exists."
                        });
                        mut.ReleaseMutex();
                        return StatusCode(409, JsonConvert.SerializeObject(retPayload1));
                    }

                    _adth.Create(thInfo);

                    var retPayload = new
                    {
                        Response = "Create the threshold rule successfully!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "SettingAPI/Threshold/Create",
                        ResponseCode = 201,
                        Remark = "Create the threshold rule successfully!"
                    });
                    mut.ReleaseMutex();
                    return StatusCode(201, JsonConvert.SerializeObject(retPayload));
                }
                catch (DbUpdateException exc)
                {
                    var retPayload = new
                    {
                        Response = "Failed to create the threshold rule. Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "SettingAPI/Threshold/Create",
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
                        Response = "Failed to create the threshold rule. Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "SettingAPI/Threshold/Create",
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
                    URL = "SettingAPI/Threshold/Create",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 21. Update Threshold Rule 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="thInfo">The threshold rule information</param>
        /// <returns></returns>
        /// <response code="200">Update Threshold Rule Successfully.</response>
        /// <response code="400">Data Error.</response>
        /// <response code="403"> Token error </response>
        /// <response code="409">Name already exists.
        ///1. The identity token not found
        ///2. Not Admin
        ///</response>   
        [HttpPut("SettingAPI/Threshold/Update")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        public IActionResult UpdateThreshold([FromHeader]string token, [FromBody]ThresholdTemplate thInfo)
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
                            Response = "Sorry, you do not have access to update threshold rule."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "SettingAPI/Threshold/Update",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do NOT have enough authorization!", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                    }

                    if (thInfo == null || thInfo.Setting.Name == null)
                    {
                        var retPayload1 = new
                        {
                            Response = "The rule's setting or name could not be null."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "SettingAPI/Threshold/Update",
                            ResponseCode = 400,
                            Remark = "The rule's setting could not be null.."
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }

                    mut.WaitOne();

                    if (_adth.NameExists(thInfo.Setting.Name, thInfo.Setting.Id))
                    {
                        var retPayload1 = new
                        {
                            Response = "The threshold rule's name already exists."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "SettingAPI/Threshold/Update",
                            ResponseCode = 409,
                            Remark = "The threshold rule's name already exists."
                        });
                        mut.ReleaseMutex();
                        return StatusCode(409, JsonConvert.SerializeObject(retPayload1));
                    }

                    _adth.Update(thInfo);

                    var retPayload = new
                    {
                        Response = "Update the threshold rule successfully!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "SettingAPI/Threshold/Update",
                        ResponseCode = 200,
                        Remark = "Update the threshold rule successfully!"
                    });

                    mut.ReleaseMutex();
                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                catch (DbUpdateException exc)
                {
                    var retPayload = new
                    {
                        Response = "Failed to update the threshold rule. Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "SettingAPI/Threshold/Update",
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
                        Response = "Failed to update the threshold rule. Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "SettingAPI/Threshold/Update",
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
                    ErrorCode = 1,
                    Response = "You have been idle too long, please log-in again."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "PUT",
                    URL = "SettingAPI/Threshold/Update",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 22. Delete Threshold Rule 
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
        [HttpDelete("SettingAPI/Threshold/Delete")]
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
                            Response = "Sorry, you do not have access to delete the threshold rule."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "DELETE",
                            URL = "SettingAPI/Delete",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do not have enough authorization!", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }

                    _adth.Delete(Int32.Parse(id));

                    var retPayload1 = new
                    {
                        Response = "Delete the group successfully!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = "SettingAPI/Delete",
                        ResponseCode = 202,
                        Remark = "Delete the group successfully!"
                    });

                    return StatusCode(202, JsonConvert.SerializeObject(retPayload1));
                }
                catch (ArgumentNullException exc)
                {
                    var retPayload1 = new
                    {
                        Response = "Please choose one threshold rule!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = "SettingAPI/Delete",
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
                        URL = "SettingAPI/Delete",
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
                    URL = "SettingAPI/Delete",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 24. Get Threshold Rule's Group Setting
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="id">The branch ID</param>
        /// <returns></returns>
        /// <response code="201">Arrangement success.</response>
        /// <response code="400">There is exception.</response>
        /// <response code="403">The identity token not found</response>   
        [HttpGet("SettingAPI/Threshold/Group")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult GetThresholdGroupSetting([FromHeader]string token, int id)
        {
            //AdminDBDispatcher._widget adw = new AdminDBDispatcher._widget();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "SettingAPI/Threshold/Group",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    ThresholdTemplate.GroupSetting info = new ThresholdTemplate.GroupSetting()
                    {
                        Enable = _adth.IsEnable(id)
                    };

                    info.Selected = _adth.GetSelectedGroup(id);
                    var group = _adb.GetBranchList();

                    if (null != info.Selected)
                    {
                        foreach (SelectOptionTemplate option in info.Selected)
                        {
                            group = group.Where(g => g.Id != option.Id).ToArray();
                        }
                    }

                    info.Unselected = group;

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "SettingAPI/Threshold/Group",
                        ResponseCode = 200,
                        Remark = "Get threshold rule's group setting successfully!"
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(info));
                }
                catch (Exception exc)
                {
                    if(!_adth.ThresholdExists(id))
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "SettingAPI/Threshold/Group",
                            ResponseCode = 204,
                            Remark = "The threshold rule could NOT be found!"
                        });
                        return StatusCode(204);
                    }

                    var retPayload = new
                    {
                        Response = "Failed to get settings. Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "SettingAPI/Threshold/Group",
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
                    Response = "You have been idle too long, please log-in again."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "SettingAPI/Threshold/Group",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 30. Save group information.
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="thGroupSetting">The group setting</param>
        /// <returns></returns>
        /// <response code="201">Arrangement success.</response>
        /// <response code="400">There is exception.</response>
        /// <response code="403">The identity token not found.</response>   
        /// <response code="409">Name already exists.</response>
        [HttpPut("SettingAPI/Threshold/Save")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        public IActionResult SaveThresholdSetting([FromHeader]string token, [FromBody]ThresholdTemplate.GroupSetting thGroupSetting)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "PUT",
                URL = "SettingAPI/Threshold/Save",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    if (!_ade.CheckAdmin(user))
                    {
                        var retPayload = new
                        {
                            Response = "Sorry, you do not have access to save the threshold rule's group setting."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "SettingAPI/Threshold/Save",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do not have enough authorization.", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }

                    mut.WaitOne();

                    if (false == _adth.Save(thGroupSetting))
                    {
                        var retPayload1 = new
                        {
                            Response = "The threshold rule could NOT be found. Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "SettingAPI/Threshold/Save",
                            ResponseCode = 400,
                            Remark = "The threshold rule could NOT be found. Please refresh the web page and try again."
                        });
                        mut.ReleaseMutex();
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                    else
                    {
                        var retPayload1 = new
                        {
                            Response = "Save the threshold setting successfully!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "SettingAPI/Threshold/Save",
                            ResponseCode = 200,
                            Remark = "Save the threshold setting successfully!"
                        });

                        mut.ReleaseMutex();
                        return StatusCode(200, JsonConvert.SerializeObject(retPayload1));
                    }
                }
                catch (DbUpdateException exc)
                {

                    if (exc.InnerException.Message.Contains("FK_Branch_To_ThresholdBranchList"))
                    {
                        var retPayload = new
                        {
                            Response = "The group could NOT be found. Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "SettingAPI/Threshold/Save",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        mut.ReleaseMutex();
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Failed to save threshold setting. Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "SettingAPI/Threshold/Save",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        mut.ReleaseMutex();
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }

                }
                catch (Exception exc)
                {
                    if (thGroupSetting == null)
                    {
                        var retPayload1 = new
                        {
                            Response = "Do not receive any data. Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "SettingAPI/Threshold/Save",
                            ResponseCode = 400,
                            Remark = exc.Message
                        });
                        mut.ReleaseMutex();
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Failed to save threshold setting. Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "SettingAPI/Threshold/Save",
                            ResponseCode = 400,
                            Remark = exc.Message
                        });
                        mut.ReleaseMutex();
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
                    URL = "SettingAPI/Threshold/Save",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 31. Get Dynamic Data List
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="id">The branch ID</param>
        /// <returns></returns>
        /// <response code="200">Arrangement success.</response>
        /// <response code="400">There is exception.</response>
        /// <response code="403">The identity token not found</response>   
        [HttpGet("SettingAPI/Threshold/DataSource")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult GetThresholdDataSource([FromHeader]string token)
        {
            //AdminDBDispatcher._widget adw = new AdminDBDispatcher._widget();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "SettingAPI/Threshold/DataSource",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    DataSelect[] dataOption = _addata.GetDataSource();

                    if (dataOption == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "SettingAPI/Threshold/DataSource",
                            ResponseCode = 204,
                            Remark = "Dynamic data could NOT be found."
                        });
                        return StatusCode(204, JsonConvert.SerializeObject(dataOption));
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "SettingAPI/Threshold/DataSource",
                        ResponseCode = 200,
                        Remark = "Get dynamic data list successfully!"
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(dataOption));

                }
                catch (Exception exc)
                {
                    var retPayload = new
                    {
                        Response = "Failed to get the dynamic data list. Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "SettingAPI/Threshold/DataSource",
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
                    URL = "SettingAPI/Threshold/DataSource",
                    ResponseCode = 403,
                    Remark = "[SettingAPI/Threshold/DataSource]Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 31. Get threshold settings for core service.
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get threshold settings successfully.</response>
        /// <response code="204">There is not any threshold setting.</response>
        /// <response code="403">Token Error</response>
        /// <response code="500">AdminDB Error</response> 
        [HttpGet("SettingAPI/Threshold/Setting")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult ThresholdSetting([FromHeader]string token)
        {
            //AdminDBDispatcher._widget adw = new AdminDBDispatcher._widget();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "SettingAPI/Threshold/Setting",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var retPayload = _adth.GetSetting();

                    if (retPayload.Length == 0)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "SettingAPI/Threshold/Setting",
                            ResponseCode = 204,
                            Remark = "There is not any enable threshold setting."
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "SettingAPI/Threshold/Setting",
                        ResponseCode = 200,
                        Remark = "Get threshold setting successfully."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                catch(Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "SettingAPI/Threshold/Setting",
                        ResponseCode = 500,
                        Remark = exc.Message
                    });

                    return StatusCode(500, JsonConvert.SerializeObject("AdminDB Error"));
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
                    URL = "SettingAPI/Threshold/Setting",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }


        /// <summary>
        /// 31. Get threshold setting for Web.
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="thresholdId">The threshold id</param>
        /// <returns></returns>
        /// <response code="200">Get threshold settings successfully.</response>
        /// <response code="204">There is not any threshold setting.</response>
        /// <response code="403">Token Error</response>
        /// <response code="500">AdminDB Error</response> 
        [HttpGet("SettingAPI/Threshold/Setting/Find")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult ThresholdSetting([FromHeader]string token, int thresholdId)
        {
            //AdminDBDispatcher._widget adw = new AdminDBDispatcher._widget();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "SettingAPI/Threshold/Setting/Find",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var retPayload = _adth.GetSetting(thresholdId);

                    if(retPayload == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "SettingAPI/Threshold/Setting/Find",
                            ResponseCode = 204,
                            Remark = "The specific threshold could not be found."
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "SettingAPI/Threshold/Setting/Find",
                        ResponseCode = 200,
                        Remark = "Get threshold setting successfully."
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
                        URL = "SettingAPI/Threshold/Setting/Find",
                        ResponseCode = 500,
                        Remark = exc.Message
                    });

                    return StatusCode(500, JsonConvert.SerializeObject("AdminDB Error"));
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
                    URL = "SettingAPI/Threshold/Setting/Find",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 32. Create customized map.
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="customizedMap">The customized map</param>
        /// <returns></returns>
        /// <response code="201">Create successfully.</response>
        /// <response code="204">There is not any threshold setting.</response>
        /// <response code="400">The file extension error</response>
        /// <response code="403">Token Error</response>
        /// <response code="500">AdminDB Error</response> 
        [HttpPost("SettingAPI/CustomizedMap")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult CreateMap([FromHeader]string token, [FromForm] CustomizedMapTemplate customizedMap)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "POST",
                URL = "SettingAPI/CustomizedMap",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    if (!_adper.CheckCreatePermission(user, DataDefine.PermissionFlag.CustomizedMap))
                    {
                        var retPayload1 = new
                        {
                            Response = ReturnCode.PermissionDenied
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "SettingAPI/CustomizedMap",
                            ResponseCode = 403,
                            Remark = "Permission Denied"
                        });
                        return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                    }

                    if (!_adcm.AllowedFileExtensions(customizedMap.File))
                    {
                        var retPayload1 = new
                        {
                            Response = ReturnCode.FileExtensionUnacceptable
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "SettingAPI/CustomizedMap",
                            ResponseCode = 400,
                            Remark = "The File Extension is unacceptable."
                        });

                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }

                    _adcm.Create(customizedMap);

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "SettingAPI/CustomizedMap",
                        ResponseCode = 201,
                        Remark = "Create customized map successfully."
                    });

                    return StatusCode(201);
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "SettingAPI/CustomizedMap",
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
                    Method = "POST",
                    URL = "SettingAPI/CustomizedMap",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 33. Delete customized map by id.
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="id">The customized map id</param>
        /// <returns></returns>
        /// <response code="201">Create successfully.</response>
        /// <response code="204">There is not any threshold setting.</response>
        /// <response code="400">The file extension error</response>
        /// <response code="403">Token Error</response>
        /// <response code="500">AdminDB Error</response> 
        [HttpPut("SettingAPI/CustomizedMap/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult UpdateMap([FromHeader]string token, int id, [FromForm] CustomizedMapTemplate customizedMap)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "PUT",
                URL = $"SettingAPI/CustomizedMap/{id}",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    if (!_adper.CheckDeletePermission(user, DataDefine.PermissionFlag.CustomizedMap))
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
                            URL = $"SettingAPI/CustomizedMap/{id}",
                            ResponseCode = 403,
                            Remark = "Permission Denied"
                        });
                        return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                    }

                    _adcm.Update(id, customizedMap);

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = $"SettingAPI/CustomizedMap/{id}",
                        ResponseCode = 204,
                        Remark = "Create customized map successfully."
                    });

                    return StatusCode(204);
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = $"SettingAPI/CustomizedMap/{id}",
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
                    URL = $"SettingAPI/CustomizedMap/{id}",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 33. Delete customized map by id.
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="id">The customized map id</param>
        /// <returns></returns>
        /// <response code="201">Create successfully.</response>
        /// <response code="204">There is not any threshold setting.</response>
        /// <response code="400">The file extension error</response>
        /// <response code="403">Token Error</response>
        /// <response code="500">AdminDB Error</response> 
        [HttpDelete("SettingAPI/CustomizedMap/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult DeleteMap([FromHeader]string token, int id)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "DELETE",
                URL = $"SettingAPI/CustomizedMap/{id}",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    if (!_adper.CheckDeletePermission(user, DataDefine.PermissionFlag.CustomizedMap))
                    {
                        var retPayload1 = new
                        {
                            Response = ReturnCode.PermissionDenied
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "DELETE",
                            URL = $"SettingAPI/CustomizedMap/{id}",
                            ResponseCode = 403,
                            Remark = "Permission Denied"
                        });
                        return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                    }

                    _adcm.DeleteMap(id);

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = $"SettingAPI/CustomizedMap/{id}",
                        ResponseCode = 204,
                        Remark = "Create customized map successfully."
                    });

                    return StatusCode(204);
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = $"SettingAPI/CustomizedMap/{id}",
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
                    Method = "DELETE",
                    URL = $"SettingAPI/CustomizedMap/{id}",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 34. Get Customized map list.
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get list successfully.</response>
        /// <response code="204">There is not map.</response>
        /// <response code="403">Token Error</response>
        /// <response code="500">Internel server error.</response> 
        [HttpGet("SettingAPI/CustomizedMap")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetCustomizedMap([FromHeader]string token)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "SettingAPI/CustomizedMap",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var maplist = _adcm.GetCustomizedMapList();

                    if (maplist.Length == 0)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "SettingAPI/CustomizedMap",
                            ResponseCode = 204,
                            Remark = "The customized map list could NOT be found!"
                        });

                        return StatusCode(204);
                    }

                    var retPayload = new
                    {
                        List = maplist
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "SettingAPI/CustomizedMap",
                        ResponseCode = 200,
                        Remark = "Get customized map list successfully!"
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
                        URL = "SettingAPI/CustomizedMap",
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
                    URL = "SettingAPI/CustomizedMap",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 35. Get specific map by id
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="id">The customized map id</param>
        /// <returns></returns>
        /// <response code="200">Get map successfully.</response>
        /// <response code="204">Not found.</response>
        /// <response code="403">Token Error</response>
        /// <response code="500">Internal server error</response> 
        [HttpGet("SettingAPI/CustomizedMap/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult GetSpecificMap([FromHeader]string token, int id)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = $"SettingAPI/CustomizedMap/{id}",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var map = _adcm.GetMapInfo(id);

                    if (map == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = $"SettingAPI/CustomizedMap/{id}",
                            ResponseCode = 204,
                            Remark = "The customized map could NOT be found!"
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = $"SettingAPI/CustomizedMap/{id}",
                        ResponseCode = 200,
                        Remark = "Get map successfully!"
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(map));
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = $"SettingAPI/CustomizedMap/{id}",
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
                    URL = $"SettingAPI/CustomizedMap/{id}",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 36. Get marker info
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="guid">The customized map id</param>
        /// <returns></returns>
        /// <response code="200">Get threshold settings successfully.</response>
        /// <response code="204">There is not any threshold setting.</response>
        /// <response code="403">Token Error</response>
        /// <response code="500">AdminDB Error</response> 
        [HttpGet("SettingAPI/CustomizedMap/Marker/{guid}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult GetMarkerDetail([FromHeader]string token, string guid)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var marker = _adcm.GetMarkerDetail(guid);

                    if (marker == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
                            ResponseCode = 204,
                            Remark = "marker not found."
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
                        ResponseCode = 200,
                        Remark = "Get marker successfully!"
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(marker));
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
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
                    URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 37. Create Marker
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="marker">The marker information</param>
        /// <returns></returns>
        /// <response code="201">Create marker successfully.</response>
        /// <response code="403">1.Token Error 2. Permission Denied</response>
        /// <response code="500">Internel server error.</response> 
        [HttpPost("SettingAPI/CustomizedMap/Marker")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult CreateMarker([FromHeader]string token, [FromBody]MarkerTemplate marker)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "POST",
                URL = "SettingAPI/CustomizedMap/Marker",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    if (!_adper.CheckCreatePermission(user, DataDefine.PermissionFlag.CustomizedMap))
                    {
                        var retPayload1 = new
                        {
                            Response = ReturnCode.PermissionDenied
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "SettingAPI/CustomizedMap/Marker",
                            ResponseCode = 403,
                            Remark = "Permission Denied"
                        });
                        return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                    }

                    var guid = _adcm.CreateMarker(marker);

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "SettingAPI/CustomizedMap/Marker",
                        ResponseCode = 201,
                        Remark = "Create customized map successfully."
                    });

                    return StatusCode(201, JsonConvert.SerializeObject(guid));
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "SettingAPI/CustomizedMap/Marker",
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
                    Method = "POST",
                    URL = "SettingAPI/CustomizedMap/Marker",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }


        /// <summary>
        /// 38. Update Marker
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="marker">The marker information</param>
        /// <param name="guid">The marker's primary key</param>
        /// <returns></returns>
        /// <response code="201">Create marker successfully.</response>
        /// <response code="403">1.Token Error 2. Permission Denied</response>
        /// <response code="500">Internel server error.</response> 
        [HttpPut("SettingAPI/CustomizedMap/Marker/{guid}")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult UpdateMarker([FromHeader]string token, [FromBody]MarkerTemplate marker, string guid)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "PUT",
                URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    if (!_adper.CheckUpdatePermission(user, DataDefine.PermissionFlag.CustomizedMap))
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
                            URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
                            ResponseCode = 403,
                            Remark = "Permission Denied"
                        });
                        return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                    }

                    if (_adcm.UpdateMarker(guid, marker))
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
                            ResponseCode = 200,
                            Remark = "Update successfully."
                        });
                        return StatusCode(200);
                    }
                    else
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
                            ResponseCode = 204,
                            Remark = "marker not found."
                        });
                        return StatusCode(204);
                    }
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
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
                    URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }


        /// <summary>
        /// 39. Delete Marker
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="marker">The marker information</param>
        /// <param name="guid">The marker's primary key</param>
        /// <returns></returns>
        /// <response code="201">Create marker successfully.</response>
        /// <response code="403">1.Token Error 2. Permission Denied</response>
        /// <response code="500">Internel server error.</response> 
        [HttpDelete("SettingAPI/CustomizedMap/Marker/{guid}")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult DeleteMarker([FromHeader]string token, string guid)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    if (!_adper.CheckDeletePermission(user, DataDefine.PermissionFlag.CustomizedMap))
                    {
                        var retPayload = new
                        {
                            Response = ReturnCode.PermissionDenied
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "DELETE",
                            URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
                            ResponseCode = 403,
                            Remark = "Permission Denied."
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }

                    _adcm.DeleteMarker(guid);

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
                        ResponseCode = 204,
                        Remark = "Delete the marker successfully."
                    });

                    return StatusCode(204);
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    return StatusCode(400);
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
                    Method = "DELETE",
                    URL = $"SettingAPI/CustomizedMap/Marker/{guid}",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 40. Delete All Markers
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="id">The map's primary key</param>
        /// <returns></returns>
        /// <response code="201">Create marker successfully.</response>
        /// <response code="403">1.Token Error 2. Permission Denied</response>
        /// <response code="500">Internel server error.</response> 
        [HttpDelete("SettingAPI/CustomizedMap/{id}/Markers")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult DeleteAllMarkers([FromHeader]string token, int id)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    if (!_adper.CheckDeletePermission(user, DataDefine.PermissionFlag.CustomizedMap))
                    {
                        var retPayload = new
                        {
                            Response = ReturnCode.PermissionDenied
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "DELETE",
                            URL = $"SettingAPI/CustomizedMap/{id}/Markers",
                            ResponseCode = 403,
                            Remark = "Permission Denied."
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }

                    _adcm.DeleteAllMarkers(id);

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = $"SettingAPI/CustomizedMap/{id}/Markers",
                        ResponseCode = 204,
                        Remark = "Delete all markers successfully."
                    });

                    return StatusCode(204);
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = $"SettingAPI/CustomizedMap/{id}/Markers",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    return StatusCode(400);
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
                    Method = "DELETE",
                    URL = $"SettingAPI/CustomizedMap/{id}/Markers",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }


        /// <summary>
        /// 41. Get device raw data setting
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Create marker successfully.</response>
        /// <response code="204">Create marker successfully.</response>
        /// <response code="403">1.Token Error 2. Permission Denied</response>
        /// <response code="500">Internel server error.</response> 

        [HttpGet("SettingAPI/RawData/Expiry-Date")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult GetRawDataSetting([FromHeader]string token)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "SettingAPI/RawData/Expiry-Date",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var data = _addata.GetExpiryDate();

                    if (data == 0)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "SettingAPI/RawData/Expiry-Date",
                            ResponseCode = 204,
                            Remark = "not found."
                        });
                        return StatusCode(204);
                    }

                    var retPayload = new
                    {
                        ExpiryDate = data
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "SettingAPI/RawData/Expiry-Date",
                        ResponseCode = 200,
                        Remark = "Get expiry date successfully!"
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
                        URL = "SettingAPI/RawData/Expiry-Date",
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
                    URL = "SettingAPI/RawData/Expiry-Date",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }


        /// <summary>
        /// 2. Patch device raw data setting
        /// </summary>
        /// <param name="token">The administrator identity token</param>
        /// <param name="key">The license to authorize number of devices</param>
        /// <returns></returns>
        /// <response code="202">The license has been accepted</response>
        /// <response code="403">The identity token could not be found</response>
        /// <response code="406">The license is not acceptable</response>
        [HttpPatch("SettingAPI/RawData/Expiry-Date")]
        [Produces("application/json")]
        [ProducesResponseType(202)]
        [ProducesResponseType(403)]
        [ProducesResponseType(406)]
        public IActionResult PatchRawDataSetting([FromHeader]string token, [FromBody] RawDataTemplate rawData)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "PATCH",
                URL = "SettingAPI/RawData/Expiry-Date",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    if (!_adper.CheckUpdatePermission(user, DataDefine.PermissionFlag.RawDataSetting))
                    {
                        var retPayload1 = new
                        {
                            Response = ReturnCode.PermissionDenied
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PATCH",
                            URL = "SettingAPI/RawData/Expiry-Date",
                            ResponseCode = 403,
                            Remark = "Permission Denied"
                        });
                        return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                    }


                    if (rawData.ExpiryDate > 0 && rawData.ExpiryDate < 30)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PATCH",
                            URL = "SettingAPI/RawData/Expiry-Date",
                            ResponseCode = 400,
                            Remark = "The days must be more than 30."
                        });
                        return StatusCode(400);
                    }

                    if (_addata.UpdateExpiryDate(rawData.ExpiryDate))
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user, 
                            Method = "PATCH",
                            URL = "SettingAPI/RawData/Expiry-Date",
                            ResponseCode = 204,
                            Remark = "Update the expiry-date successfully."
                        });

                        return StatusCode(204);
                    }
                    else
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PATCH",
                            URL = "SettingAPI/RawData/Expiry-Date",
                            ResponseCode = 500,
                            Remark = "Setup failed from core service."
                        });

                        return StatusCode(500);
                    }
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PATCH",
                        URL = "SettingAPI/RawData/Expiry-Date",
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
                    Method = "PATCH",
                    URL = "SettingAPI/RawData/Expiry-Date",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }


        /// <summary>
        /// 43. Get Goggle Map API Key
        /// </summary>
        /// <returns></returns>
        /// <response code="201">Create successfully.</response>
        /// <response code="204">There is not any threshold setting.</response>
        /// <response code="400">The file extension error</response>
        /// <response code="403">Token Error</response>
        /// <response code="500">AdminDB Error</response> 
        [HttpGet("SettingAPI/Key/GoggleMap")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult GetGoggleMapAPIKey([FromHeader]string token)
        {
            try
            {
                string user = _rcd.GetCache(0, token);

                object retPayload = null;

                if (user == null)
                {
                    retPayload = new
                    {
                        Response = ReturnCode.TokenError
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PATCH",
                        URL = "SettingAPI/RawData/Expiry-Date",
                        ResponseCode = 403,
                        Remark = "Request authentication token error."
                    });
                    return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                }

                retPayload = new
                {
                    data = _ddb.GetGoogleMapAPIKey()
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "GET",
                    URL = "SettingAPI/GoggleMap",
                    ResponseCode = 200,
                    Remark = "Get key successfully!"
                });

                return StatusCode(200, JsonConvert.SerializeObject(retPayload));
            }
            catch (Exception exc)
            {
                var retPayload = new
                {
                    Error = "Get key failed."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "GET",
                    URL = "SettingAPI/GoggleMap",
                    ResponseCode = 500,
                    Remark = exc.Message
                });
                return StatusCode(500, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 44. set api key
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="data">The api key</param>
        /// <returns></returns>
        /// <response code="201">Create successfully.</response>
        /// <response code="204">There is not any threshold setting.</response>
        /// <response code="400">The file extension error</response>
        /// <response code="403">Token Error</response>
        /// <response code="500">AdminDB Error</response> 
        [HttpPost("SettingAPI/Key")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]

        public IActionResult SetAPIKey([FromHeader]string token, [FromBody] JObject data)
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
                            Response = "Sorry, you do not have access to set api key."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "SettingAPI/Key",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do NOT have enough authorization!", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                    }

                    var result = _ddb.UpdateAPIKey(data.GetValue("type").ToString(), data.GetValue("key").ToString());

                    if (result == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "SettingAPI/Key",
                            ResponseCode = 500,
                            Remark = "update fail!"
                        });

                        return StatusCode(500);
                    }

                    var retPayload = new
                    {
                        data = result
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "SettingAPI/Key",
                        ResponseCode = 200,
                        Remark = "Create the threshold rule successfully!"
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                catch (Exception exc)
                {
                    var retPayload = new
                    {
                        Response = "Set key failed."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "SettingAPI/Key",
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
                    ErrorCode = 0,
                    Response = "You have been idle too long, please log-in again."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "POST",
                    URL = "SettingAPI/Key",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 46. Get API Update time
        /// </summary>
        /// <returns></returns>
        /// <response code="201">Create successfully.</response>
        /// <response code="204">There is not any threshold setting.</response>
        /// <response code="400">The file extension error</response>
        /// <response code="403">Token Error</response>
        /// <response code="500">AdminDB Error</response> 
        [HttpGet("SettingAPI/Key/UpdateTime")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult GetAPIKeyUpdateTime([FromHeader]string token)
        {
            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var retPayload = new
                    {
                        data = _ddb.GetAPIUpdateTime()
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = "",
                        Method = "GET",
                        URL = "SettingAPI/Key/UpdateTime",
                        ResponseCode = 200,
                        Remark = "Get time successfully!"
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                catch (Exception exc)
                {
                    var retPayload = new
                    {
                        Error = "Get time failed."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = "",
                        Method = "GET",
                        URL = "SettingAPI/Key/UpdateTime",
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
                    ErrorCode = 0,
                    Response = "You have been idle too long, please log-in again."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "SettingAPI/Key/UpdateTime",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }
    }
}