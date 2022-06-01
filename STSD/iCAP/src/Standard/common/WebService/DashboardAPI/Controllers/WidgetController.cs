using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DashboardAPI.Models.Widget;
using ShareLibrary;
using ShareLibrary.AdminDB;
using Newtonsoft.Json;
using ShareLibrary.DataTemplate;
using ShareLibrary.Interface;
using Microsoft.EntityFrameworkCore;
using System.Threading;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DashboardAPI.Controllers
{
    public class WidgetController : Controller
    {
        private IWidgetRepository _adw;
        private IRedisCacheDispatcher _rcd;
        private IEmployee _ade;
        private IDataDBDispatcher _ddb;
        private IBranch _adb;
        private IThreshold _adth;
        private IData _add;
        private ICustomizedMap _adcp;
        private static Mutex mut = new Mutex();

        public WidgetController(IWidgetRepository adw, IRedisCacheDispatcher rcd, IEmployee  ade, IDataDBDispatcher ddb, IBranch adb, IThreshold adth, IData add, ICustomizedMap adcp)
        {
            _adw = adw;
            _rcd = rcd;
            _ade = ade;
            _ddb = ddb;
            _adb = adb;
            _adth = adth;
            _add = add;
            _adcp = adcp;
        }

        /// <summary>
        /// 20. Create widget.
        /// </summary>
        /// <param name="token">The administrator identity token</param>
        /// <param name="widgetName">The widget name</param>
        /// <returns></returns>
        /// <response code="201">Create widget success</response>       
        /// <response code="400">Bad Request</response>
        /// <response code="403">The identity token not found</response>
        [HttpPost("WidgetAPI/Create")]
        [ProducesResponseType(201)]        
        [ProducesResponseType(400)]
        [ProducesResponseType(403)] 
        public IActionResult Create([FromHeader]string token, [FromBody]string widgetName)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                if (!_ade.CheckAdmin(user))
                {

                    var retPayload = new
                    {
                        ErrorCode = 1,
                        Response = "Sorry, you do not have access to create widget."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "WidgetAPI/Create",
                        ResponseCode = 403,
                        Remark = string.Format("{0} do not have enough authorization.", user)
                    });

                    return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                }

                try
                {
                    if (widgetName.Length > 16)
                    {
                        var retPayload1 = new
                        {
                            Response = "The widget name can not exceed 16 words."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "WidgetAPI/Create",
                            ResponseCode = 400,
                            Remark = "The widget name can not exceed 16 words."
                        });

                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }

                    _adw.Create(widgetName);

                    var retPayload = new
                    {
                        Response = "New widget created!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "WidgetAPI/Create",
                        ResponseCode = 201,
                        Remark = "New widget created!"
                    });

                    return StatusCode(201, JsonConvert.SerializeObject(retPayload));
                }
                catch (DbUpdateException exc)
                {

                    var retPayload = new
                    {
                        Response = "Failed to create the widget!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "WidgetAPI/Create",
                        ResponseCode = 400,
                        Remark = exc.InnerException.Message
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload));

                }
                catch (Exception exc)
                {
                    var retPayload = new
                    {
                        Response = "Failed to create the widget! Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "WidgetAPI/Create",
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
                    ErrorCode = 0,
                    Response = "You have been idle too long, please log-in again."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "POST",
                    URL = "WidgetAPI/Create",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 21. Get widget information
        /// </summary>
        /// <param name="widgetId">The widget Id</param>
        /// <param name="token">The administrator identity token</param>
        /// <returns></returns>
        /// <response code="200">Get widget data success</response>
        /// <response code="204">The widget data not found</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("WidgetAPI/Edit/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult Edit([FromHeader] string token, int id)
        {
            //AdminDBDispatcher._widget adw = new AdminDBDispatcher._widget();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "WidgetAPI/Edit",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    EditWidgetTemplate editInfo = new EditWidgetTemplate()
                    {
                        WidgetTemplate = _adw.Get(id)
                    };

                    if (editInfo.WidgetTemplate == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "WidgetAPI/Edit",
                            ResponseCode = 204,
                            Remark = "Widget could NOT be found!"
                        });
                        return StatusCode(204);
                    }
                    else
                    {
                        editInfo.DataSelect = _add.GetDataSource();
                        editInfo.BranchOption = _adw.GetBranchSelect();
                        editInfo.ChartOption = _adw.GetChartSelect();
                        editInfo.ThresholdOption = _adth.GetThresholdList();
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "WidgetAPI/Edit",
                        ResponseCode = 200,
                        Remark = "Get Widget Success!"
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(editInfo));
                }
                catch (ArgumentNullException exc)
                {
                    var retPayload = new
                    {
                        Response = "Please Choose one card!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "WidgetAPI/Edit",
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
                        URL = "WidgetAPI/Edit",
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
                    Response = ReturnCode.TokenError
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "WidgetAPI/Edit",
                    ResponseCode = 403,
                    Remark = "[WidgetAPI/Edit]Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 22. Update widget
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="widgetData">The widget data</param>
        /// <returns></returns>
        /// <response code="200">Update widget success</response>
        /// <response code="400">widget data error</response>
        /// <response code="403">The identity token not found</response>
        [HttpPut("WidgetAPI/Update")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult Update([FromHeader]string token, [FromBody]WidgetTemplate widgetData)
        {
            //AdminDBDispatcher._widget adw = new AdminDBDispatcher._widget();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();

            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "PUT",
                URL = "WidgetAPI/Update",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                if (!_ade.CheckAdmin(user))
                {

                    var retPayload = new
                    {
                        ErrorCode = 1,
                        Response = "Sorry, you do not have access to delete widget."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "WidgetAPI/Create",
                        ResponseCode = 403,
                        Remark = string.Format("{0} do not have enough authorization.", user)
                    });

                    return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                }

                try
                {
                    if (widgetData.Name.Length > 16)
                    {
                        var retPayload = new
                        {
                            Response = "The widget name can not exceed 16 words."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "WidgetAPI/Update",
                            ResponseCode = 400,
                            Remark = "The widget name can not exceed 16 words."
                        });

                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }

                    if (_adw.Update(widgetData))
                    {
                        var retPayload = new
                        {
                            Response = "Update the card successfully!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "WidgetAPI/Update",
                            ResponseCode = 200,
                            Remark = string.Format("Update widget {0} data successfully.", widgetData.WidgetId)
                        });

                        return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Widget could not be found."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "WidgetAPI/Update",
                            ResponseCode = 400,
                            Remark = "Widget could not be found."
                        });

                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }
                }
                catch(DbUpdateException exc)
                {                    
                    if(exc.InnerException.Message.Contains("FK_Data_To_Widget"))
                    {
                        var retPayload1 = new
                        {
                            Response = "The DATA could NOT be found! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "WidgetAPI/Update",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                    else if(exc.InnerException.Message.Contains("FK_Chart_To_Widget"))
                    {
                        var retPayload1 = new
                        {
                            Response = "The CHART could NOT be found! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "WidgetAPI/Update",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                    else
                    {
                        var retPayload1 = new
                        {
                            Response = "Update the card failed! Please check all fields."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "WidgetAPI/Update",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                }
                catch (Exception exc)
                {
                    if (widgetData == null)
                    {
                        var retPayload = new
                        {
                            Response = "Data Format Error! Please check all fields. Or refresh the web page, fill in the form again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "WidgetAPI/Update",
                            ResponseCode = 400,
                            Remark = exc.Message
                        });

                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }
                    else if (widgetData.BranchIdList.Length==0)
                    {
                        var retPayload1 = new
                        {
                            Response = "The Branch field can NOT be blank!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "WidgetAPI/Update",
                            ResponseCode = 400,
                            Remark = "The Branch field can NOT be blank!"
                        });

                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                    else if(widgetData.Width==null)
                    {
                        var retPayload1 = new
                        {
                            Response = "Please Choose One Width"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "WidgetAPI/Update",
                            ResponseCode = 400,
                            Remark = "Please Choose One Width"
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                    else if(widgetData.Name==null)
                    {
                        var retPayload1 = new
                        {
                            Response = "The card name field can NOT be blank!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "WidgetAPI/Update",
                            ResponseCode = 400,
                            Remark = "The card name field can NOT be blank!"
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                    else
                    {
                        var retPayload1 = new
                        {
                            Response = exc.Message
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "WidgetAPI/Update",
                            ResponseCode = 400,
                            Remark = exc.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
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
                    Method = "PUT",
                    URL = "WidgetAPI/Update",
                    ResponseCode = 403,
                    Remark = "[WidgetAPI/PUT]Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 23. Delete widget 
        /// </summary>
        /// <param name="widgetId">The widget Id</param>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="202">Delete Accepted.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="403">The identity token not found or do not have enough authorization.</response>       
        [HttpDelete("WidgetAPI/Delete")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]        
        public IActionResult Delete([FromHeader]string token, [FromHeader]string widgetId)
        {
            //AdminDBDispatcher._widget adw = new AdminDBDispatcher._widget();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();

            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "DELETE",
                URL = "WidgetAPI/Delete",
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

                            Response = "Sorry, you do not have access to delete widget."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "WidgetAPI/Delete",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do not have enough authorization.", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }

                    _adw.Delete(Int32.Parse(widgetId));

                    var retPayload1 = new
                    {
                        Response = "Delete the card successfully!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = "WidgetAPI/Delete",
                        ResponseCode = 202,
                        Remark = string.Format("Delete widget {0} data success.", widgetId)
                    });
                    return StatusCode(202, JsonConvert.SerializeObject(retPayload1));
                }
                catch (ArgumentNullException exc)
                {
                    var retPayload1 = new
                    {
                        Response = "Please choose one card!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = "DashboardAPI/Delete",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                }
                catch (Exception exc)
                {
                    if(exc.Message.Contains("Input string was not in a correct format."))
                    {
                        var retPayload = new
                        {
                            Response = "Data Error! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "DELETE",
                            URL = "DashboardAPI/Delete",
                            ResponseCode = 400,
                            Remark = "Data  Error! Please refresh the web page and try again."
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }

                    var retPayload1 = new
                    {
                        Response = exc.Message
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = "WidgetAPI/Delete",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
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
                    URL = "WidgetAPI/Delete",
                    ResponseCode = 403,
                    Remark = "[WidgetAPI/Delete]Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 35. Get Data Group List 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get data group list successfully.</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("WidgetAPI/GetDataGroupList")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetDataGroupList([FromHeader]string token)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = "WidgetAPI/GetDataGroupList",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    var retPayload = _adw.GetDataGroupList();

                    if (retPayload[0] == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "WidgetAPI/GetDataGroupList",
                            ResponseCode = 204,
                            Remark = "Data group could NOT be found!"
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "WidgetAPI/GetDataGroupList",
                        ResponseCode = 200,
                        Remark = "Get Data Group List Success."
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
                        URL = "WidgetAPI/GetDataGroupList",
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
                    URL = "WidgetAPI/GetDataGroupList",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 36. Get Numberical Data List 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="groupId">The group ID</param>
        /// <returns></returns>
        /// <response code="200">Get numberical data list successfully.</response>
        /// <response code="204">NOT Found.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("WidgetAPI/GetNumbericalDataList")]
        //[Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult GetNumbericalDataList([FromHeader]string token)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = "WidgetAPI/GetNumbericalDataList",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var retPayload = _adw.GetNumbericalDataList();

                    if (retPayload == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "WidgetAPI/GetNumbericalDataList",
                            ResponseCode = 204,
                            Remark = "Numberical data could NOT be found!"
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "WidgetAPI/GetNumbericalDataList",
                        ResponseCode = 200,
                        Remark = "Get Numberical Data List Success."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
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
                        URL = "WidgetAPI/GetNumbericalDataList",
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
                    URL = "WidgetAPI/GetNumbericalDataList",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 37. Get Chart List 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="dataId">The data ID</param>
        /// <returns></returns>
        /// <response code="200">Get chart list successfully.</response>
        /// <response code="204">The chart could not be found.</response>
        /// <response code="400">Bad Request</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("WidgetAPI/GetChartList")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]       
        public IActionResult GetChartList([FromHeader]string token, [FromHeader]string dataId)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = "WidgetAPI/GetChartList",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var retPayload = _adw.GetChartSelect(Int32.Parse(dataId));

                    if (retPayload[0] == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "WidgetAPI/GetChartList",
                            ResponseCode = 204,
                            Remark = "The chart could NOT be found!"
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "WidgetAPI/GetChartList",
                        ResponseCode = 200,
                        Remark = "Get Chart List Success."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                catch(ArgumentException exc)
                {
                    var retPayload = new
                    {
                        Response = "Please choose one data!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "WidgetAPI/GetChartList",
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
                        URL = "WidgetAPI/GetChartList",
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
                    URL = "WidgetAPI/GetChartList",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 38. Get Chart Size 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="chartId">The chart ID</param>
        /// <returns></returns>
        /// <response code="200">Get chart size successfully.</response>
        /// <response code="204">The Cart NOT fund.</response>
        /// <response code="400">Bad Request</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("WidgetAPI/GetChartSize")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]     
        public IActionResult GetChartSize([FromHeader]string token, [FromHeader]string chartId)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = "WidgetAPI/GetChartSize",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var retPayload = _adw.GetChartSize(Int32.Parse(chartId));

                    if(retPayload==null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "WidgetAPI/GetChartSize",
                            ResponseCode = 204,
                            Remark = "The chart could Not be found."
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "WidgetAPI/GetChartSize",
                        ResponseCode = 200,
                        Remark = "Get Chart Size Success."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                catch(ArgumentException exc)
                {
                    var retPayload = new
                    {
                        Response = "Please choose one chart!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "WidgetAPI/GetChartSize",
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
                        URL = "WidgetAPI/GetChartSize",
                        ResponseCode = 500,
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
                    URL = "WidgetAPI/GetChartSize",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 39. Get Widget List 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get widget list successfully.</response>
        /// <response code="204">The widget could NOT be found.</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("WidgetAPI/GetWidgetList")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetWidgetList([FromHeader]string token)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = "WidgetAPI/GetWidgetList",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var retPayload = _adw.GetWidgetList();

                    if(retPayload[0]==null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "WidgetAPI/GetChartSize",
                            ResponseCode = 204,
                            Remark = "The widget could Not be found."
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "WidgetAPI/GetWidgetList",
                        ResponseCode = 200,
                        Remark = "Get Widget List Success."
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
                        URL = "WidgetAPI/GetWidgetList",
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
                    URL = "WidgetAPI/GetWidgetList",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 40. Get device's information on the widget modal.
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="devName">The device name</param>
        /// <returns></returns>
        /// <response code="200">Get widget list successfully.</response>
        /// <response code="204">The widget could NOT be found.</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("WidgetAPI/Modal/Details")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetDeviceInfo([FromHeader]string token, string devName)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = "WidgetAPI/Modal/Details",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var retPayload = new
                    {
                        emp = _ade.GetName(),
                        branchList = _adb.GetBranchList(devName)
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "WidgetAPI/Modal/Details",
                        ResponseCode = 200,
                        Remark = "Get device's details on the widget modal successfully."
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
                        URL = "WidgetAPI/Modal/Details",
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
                    URL = "WidgetAPI/Modal/Details",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 31. Get Image
        /// </summary>
        /// <param name="token">The identity token</param>      
        /// <param name="filePath">The file path</param> 
        /// <returns></returns>
        /// <response code="200">Get dashboard name list success.</response>
        /// <response code="204">The dashboard could not be found!.</response>  
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response> 
        [HttpGet("WidgetAPI/CustomizedMap/Image")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        public IActionResult GetMap([FromHeader]string token, string filePath)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    var retPayload = new
                    {
                        File = _adcp.GetMap(filePath)
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = $"WidgetAPI/CustomizedMap/{filePath}",
                        ResponseCode = 200,
                        Remark = "Get the image successfully."
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
                        URL = $"WidgetAPI/CustomizedMap/{filePath}",
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
                    URL = $"WidgetAPI/CustomizedMap/{filePath}",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }
    }
}
