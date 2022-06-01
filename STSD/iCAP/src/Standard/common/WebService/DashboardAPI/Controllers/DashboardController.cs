using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using DashboardAPI.Models.Dashboard;
using ShareLibrary;
using ShareLibrary.AdminDB;
using ShareLibrary.DataTemplate;
using Newtonsoft.Json.Linq;
using ShareLibrary.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Threading;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DashboardAPI.Controllers// move to core service
{
    public class DashboardController : Controller
    {
        private IRedisCacheDispatcher _rcd;
        private IMultipleDashboard _adm;
        private IDataDBDispatcher _ddb;
        private IEmployee _ade;
        private IWidgetRepository _adw;
        private IEmail _ademail;
        private IDevice _add;
        private IThreshold _adth;
        private IData _adata;
        //private static List<int> newWidgetList;
        //private ISocket _socket;
        //private IFormCollection _fc;

        public DashboardController(IRedisCacheDispatcher rcd, IMultipleDashboard adm, IDataDBDispatcher ddb, IEmployee ade, IWidgetRepository adw, IEmail ademail, IDevice add, IThreshold adth, IData adata)
        {
            _rcd = rcd;
            _adm = adm;
            _ddb = ddb;
            _ade = ade;
            _adw = adw;
            _ademail = ademail;
            _add = add;
            _adth = adth;
            _adata = adata;
            //_socket = socket;
        }

        /// <summary>
        /// 29. Get Dashboard List 
        /// </summary>
        /// <param name="token">The identity token</param>        
        /// <returns></returns>
        /// <response code="200">Get dashboard name list success.</response>
        /// <response code="204">The dashboard could not be found!.</response>  
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response> 
        [HttpGet("DashboardAPI/GetDashboardList")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]       
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetDashboardList([FromHeader]string token)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    //var retPayload = _adm.GetDashboardList();
                    var retPayload = new
                    {
                        List = _adm.GetDashboardList()
                    };

                    if (retPayload.List[0]==null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DashboardAPI/GetDashboardList",
                            ResponseCode = 204,
                            Remark = "The dashboard could not be found!"
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/GetDashboardList",
                        ResponseCode = 200,
                        Remark = "Get Dashboard List Successfully!"
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
                        URL = "DashboardAPI/GetDashboardList",
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
                    URL = "DashboardAPI/GetDashboardList",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 31. Dashboard Widget Arrangement
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="dashboardElement">The Dashboard Widget Arrangement</param>
        /// <returns></returns>
        /// <response code="201">success</response>
        /// <response code="400">There is exception.</response>
        /// <response code="403">The identity token not found</response>   
        [HttpPost("DashboardAPI/Order")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult SetDashboardWidgetOrder([FromHeader]string token, [FromBody]WidgetOrderTemplate dashboardElement)
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
                            URL = "DashboardAPI/WidgetArrangement",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do not have enough authorization.", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }

                    _adm.SetWidgetOrder(dashboardElement);

                    var retPayload1 = new
                    {
                        Response = "Arrange dashboard card successfully!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "DashboardAPI/WidgetArrangement",
                        ResponseCode = 201,
                        Remark = "Arrange dashboard card successfully!"
                    });

                    return StatusCode(201, JsonConvert.SerializeObject(retPayload1));
                }                
                catch (DbUpdateException exc)
                {
                    if (exc.InnerException.Message.Contains("FK_CompanyDashboard_To_CompanyDashboardElement"))
                    {
                        var retPayload = new
                        {
                            Response = "The Dashboard could NOT be found! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "DashboardAPI/WidgetArrangement",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }
                    else if(exc.InnerException.Message.Contains("FK_Widget_To_CompanyDashboardElement"))
                    {
                        var retPayload = new
                        {
                            Response = "The card could NOT be found! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "DashboardAPI/WidgetArrangement",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Arrange Card Failed! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "DashboardAPI/WidgetArrangement",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }

                }
                catch (Exception exc)
                {
                    if (dashboardElement == null)
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
                            URL = "DashboardAPI/WidgetArrangement",
                            ResponseCode = 400,
                            Remark = exc.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                    else if (dashboardElement.WidgetIdList.Length == 0)
                    {
                        var retPayload1 = new
                        {
                            Response = "Please choose one card at least!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "DashboardAPI/WidgetArrangement",
                            ResponseCode = 400,
                            Remark = "Please choose one card at least!"
                        });

                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Arrange Card Failed! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "DashboardAPI/WidgetArrangement",
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
                    Response = "You have been idle too long, please log-in again."
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "POST",
                    URL = "DashboardAPI/WidgetArrangement",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 32. Create Dashboard 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="dashboardName">The Dashboard Information</param>
        /// <returns></returns>
        /// <response code="201">Create Dashboard Success.</response>
        /// <response code="400">Data Error.</response>
        /// <response code="403">
        ///1. The identity token not found
        ///2. Not Admin
        ///</response>   
        [HttpPost("DashboardAPI/Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult Create([FromHeader]string token, [FromBody]string dashboardName)
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
                            Response = "Sorry, you do not have access to create widget."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "DashboardAPI/Create",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do NOT have enough authorization!", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }

                    _adm.Create(dashboardName);

                    var retPayload1 = new
                    {
                        Response = "Create the dasboard successfully!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "DashboardAPI/Create",
                        ResponseCode = 201,
                        Remark = "Create the dasboard successfully!"
                    });

                    return StatusCode(201, JsonConvert.SerializeObject(retPayload1));
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
                        URL = "DashboardAPI/Create",
                        ResponseCode = 400,
                        Remark = exc.InnerException.Message
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    
                }
                catch (Exception exc)
                {
                    var retPayload = new
                    {
                        Response = "Create the dashboard failed! Please refresh the web page and try again."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "DashboardAPI/Create",
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
                    Method = "POST",
                    URL = "DashboardAPI/Create",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 34. Delete Dashboard 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="dashboardId">The Dashboard Id</param>
        /// <returns></returns>
        /// <response code="202">Delete Accepted.</response>
        /// <response code="400">Data Error.</response>
        /// <response code="403">
                                ///1. The identity token not found
                                ///2. Not Admin
        ///</response>   
        [HttpDelete("DashboardAPI/Delete")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult Delete([FromHeader]string token, [FromHeader]string dashboardId)
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
                            Response = string.Format("{0} do not have enough authorization!", user)
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "DELETE",
                            URL = "DashboardAPI/Delete",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do not have enough authorization!", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }

                    _adm.Delete(Int32.Parse(dashboardId));

                    var retPayload1 = new
                    {
                        Response = "Delete the dashboard successfully!"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = "DashboardAPI/Delete",
                        ResponseCode = 202,
                        Remark = "Delete the dashboard successfully!"
                    });

                    return StatusCode(202, JsonConvert.SerializeObject(retPayload1));
                }
                catch(ArgumentNullException exc)
                {
                    var retPayload1 = new
                    {
                        Response = "Please choose one dashboard!"
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
                    var retPayload = new
                    {
                        Response = exc.Message
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
                    URL = "DashboardAPI/Delete",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 31. Save Dashboard Information
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="dashboardInfo">The Dashboard Widget Arrangement</param>
        /// <returns></returns>
        /// <response code="200">success</response>
        /// <response code="400">The widget is over limitation.</response>
        /// <response code="403">1. The identity token not found.
        ///                      2. Permission denied.</response> 
        /// <response code="422">The incoming is null.</response>  
        /// <response code="500">Internal Server Error</response>   
        [HttpPut("DashboardAPI/Save")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult Save([FromHeader]string token, [FromBody]DashboardTemplate dashboardInfo)
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
                            Response = "Sorry, you do not have access to edit the dashbaord."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "DashboardAPI/Save",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do not have enough authorization.", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    } 

                    if (_adm.SaveDashboardInfo(dashboardInfo))
                    {
                        var retPayload1 = new
                        {
                            Response = "Update the dashbaord successfully!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "DashboardAPI/Save",
                            ResponseCode = 200,
                            Remark = "Update the dashbaord successfully!"
                        });

                        return StatusCode(200, JsonConvert.SerializeObject(retPayload1));
                    }
                    else
                    {
                        var retPayload1 = new
                        {
                            Response = "The dashboard widgets is over maximum limitation."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "DashboardAPI/Save",
                            ResponseCode = 400,
                            Remark = "The dashboard widgets is over maximum limitation."
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }
                }
                catch (DbUpdateException exc)
                {

                    if (exc.InnerException.Message.Contains("FK_Widget_To_CompanyDashboardElement"))
                    {
                        var retPayload = new
                        {
                            Response = "The widget could NOT be found! Please refresh the web page and try again."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "DashboardAPI/Save",
                            ResponseCode = 500,
                            Remark = exc.InnerException.Message
                        });
                        return StatusCode(500, JsonConvert.SerializeObject(retPayload));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Fail to edit dashbaord!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "DashboardAPI/Save",
                            ResponseCode = 500,
                            Remark = exc.InnerException.Message
                        });
                        return StatusCode(500, JsonConvert.SerializeObject(retPayload));
                    }

                }
                catch (Exception exc)
                {
                    if (dashboardInfo == null)
                    {
                        var retPayload1 = new
                        {
                            Response = "The incoming is null."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "DashboardAPI/Save",
                            ResponseCode = 422,
                            Remark = exc.Message
                        });
                        return StatusCode(422, JsonConvert.SerializeObject(retPayload1));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Fail to edit dashbaord!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "DashboardAPI/Save",
                            ResponseCode = 500,
                            Remark = exc.Message
                        });
                        return StatusCode(500, JsonConvert.SerializeObject(retPayload));
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
                    URL = "DashboardAPI/Save",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 21. Get widget information
        /// </summary>
        /// <param name="dashboardId">The widget Id</param>
        /// <param name="token">The administrator identity token</param>
        /// <returns></returns>
        /// <response code="200">Get widget data success</response>
        /// <response code="204">The widget data not found</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("DashboardAPI/Edit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult Edit([FromHeader] string token, [FromHeader] string dashboardId)
        {
            //AdminDBDispatcher._widget adw = new AdminDBDispatcher._widget();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "DashboardAPI/Edit",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    EditDashboardTemplate editInfo = new EditDashboardTemplate();
                    editInfo.Id = Int32.Parse(dashboardId);
                    editInfo.Name = _adm.GetNameByDashboardId(editInfo.Id);

                    if (editInfo.Name == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DashboardAPI/Edit",
                            ResponseCode = 204,
                            Remark = "Dashboard could NOT be found!"
                        });
                        return StatusCode(204);
                    }
                    else
                    {
                        editInfo.WidgetInfo = _adw.GetWidgetInfo();
                        editInfo.WidgetIdList = _adm.GetWidgetId(editInfo.Id);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/Edit",
                        ResponseCode = 200,
                        Remark = "Get Dashboard Successfully!"
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(editInfo));
                }
                catch (ArgumentNullException exc)
                {
                    var retPayload = new
                    {
                        Response = "Please choose one dashboard."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/Edit",
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
                        URL = "DashboardAPI/Edit",
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
                    URL = "DashboardAPI/Edit",
                    ResponseCode = 403,
                    Remark = "[DashboardAPI/Edit]Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        List<KeyValuePair<string, object[]>> GetDetailData(string[] DeviceList, string DataLocation, int count)
        {
            //Device List only one item
            JObject root;
            BsonDocument bdstorage;
            bool storageFlag = false;
            try
            {
                root = JObject.Parse(DataLocation);
            }
            catch (Exception)
            {
                return null;
            }

            List<string> queryPath = new List<string>();

            IList<string> keys = root.Properties().Select(p => p.Name).ToList();

            queryPath.Add(keys[0]);

            string keyTitle = keys[0];

            while (root != null)
            {
                JTokenType t = root[keyTitle].Type;
                if (t == JTokenType.Integer)
                {
                    break;
                }
                if (t == JTokenType.Array)
                {
                    //queryPath.Add("a");
                    JArray ja = root[keyTitle].Value<JArray>();
                    root = ja.Children<JObject>().First();
                    keys = root.Properties().Select(p => p.Name).ToList();
                    queryPath.Add(keys[0]);
                    keyTitle = keys[0];
                }
                else
                {
                    root = root[keyTitle].Value<JObject>();
                    keys = root.Properties().Select(p => p.Name).ToList();
                    if (string.Equals(keys[0], "Enable"))
                    {
                        queryPath.Add(keys[1]);
                        keyTitle = keys[1];
                    }
                    else
                    {
                        queryPath.Add(keys[0]);
                        keyTitle = keys[0];
                    }
                }
            }

            List<KeyValuePair<string, object[]>> data = new List<KeyValuePair<string, object[]>>();

            string collectionPostfix = queryPath[0];
            List<int> time = new List<int>();
            int time_index = 0;
            queryPath.RemoveAt(0);

            switch (collectionPostfix.ToLower())
            {
                case "static":
                case "dynamic":
                    foreach (string s in DeviceList)
                    {
                        DataDBDispatcher datadb = new DataDBDispatcher();
                        string CollectionName = string.Format("{0}-{1}", s, collectionPostfix.ToLower());
                        BsonDocument[] bdoc = datadb.GetRawData(CollectionName, null, count).ToArray();

                        if (bdoc.Length == 0)
                        {
                            return new List<KeyValuePair<string, object[]>>();
                        }

                        List<object[]> obj = new List<object[]>();
                        List<object[]> storage_sn = new List<object[]>();
                        Parallel.For(0, bdoc.Length, i =>
                        {
                            List<BsonDocument> bd = new List<BsonDocument>();
                            List<object> retlist = new List<object>();
                            bd.Add(bdoc[i]);
                            foreach (string query in queryPath)
                            {
                                if (string.Equals(query, "0"))
                                {
                                    BsonDocument bdd = bd[0];
                                }
                                else
                                {
                                    List<BsonDocument> tempBD = new List<BsonDocument>();
                                    foreach (BsonDocument bdd in bd)
                                    {
                                        if (bdd[query].IsBsonArray)
                                        {
                                            BsonArray ba = bdd[query].AsBsonArray;
                                            foreach (BsonValue bddd in ba)
                                            {
                                                tempBD.Add(bddd.AsBsonDocument);
                                            }
                                        }
                                        else
                                        {
                                            if (bdd[query].IsNumeric)
                                            {
                                                if (bdd[query].IsInt32)
                                                {
                                                    retlist.Add(bdd[query].AsInt32);
                                                }
                                                else if (bdd[query].IsInt64)
                                                {
                                                    retlist.Add(bdd[query].AsInt64);
                                                }
                                                else if (bdd[query].IsDouble)
                                                {
                                                    retlist.Add(bdd[query].AsDouble);
                                                }
                                                continue;
                                            }
                                            else
                                            {
                                                tempBD.Add(bdd[query].AsBsonDocument);
                                            }
                                        }
                                        //retlist.Add(CommonFunctions.GetTime24(Convert.ToInt32(CommonFunctions.GetData(bdd["time"]))));
                                    }
                                    if (tempBD.Count != 0)
                                    {
                                        bd.Clear();
                                        bd = tempBD;
                                    }
                                }
                            }
                            obj.Add(retlist.ToArray());       
                            //foreach (BsonDocument element in bdoc)
                            //{
                            //    timeArray[time_index++] = CommonFunctions.GetTime24(Convert.ToInt32(CommonFunctions.GetData(element["time"])));
                            //}

                        });
                        time.Add(Convert.ToInt32(CommonFunctions.GetData(bdoc[0]["time"])));
                        //time.Add(bdoc[0]["time"].ToString());
                        object[] temp = obj.ToArray();
                        object[] stemp;
                        List<object[]> objSN = new List<object[]>();
                        List<object> so = new List<object>();
                        int result_length = 1;
                        if ("Storage" == queryPath[0])
                        {
                            result_length++;
                            storageFlag = true;
                            bdstorage = _ddb.GetLastRawData(string.Format("{0}-static", DeviceList[0]));
                            try
                            {
                                foreach (BsonDocument bd in bdstorage["Storage"].AsBsonArray)
                                {
                                    //so = new List<object>();
                                    so.Add(bd["SN"].AsString);//"Serial Number"
                                    //storage_sn.Add(so.ToArray());
                                }
                                //objSN.Add(storage_sn.ToArray());
                                objSN.Add(so.ToArray());
                            }
                            catch
                            {
                                return new List<KeyValuePair<string, object[]>>();
                            }
                        }
                        stemp = objSN.ToArray();

                        if (temp.Count() != 0)
                        {
                            object[,] result = new object[((object[])temp[0]).Length, obj.Count + result_length];
                            object[] ttemp;
                            object[] sstemp;
                            Parallel.For(0, temp.Length, j =>
                            {
                                if (storageFlag == false)
                                {
                                    ttemp = (object[])temp[0];
                                    Parallel.For(0, ttemp.Length, k =>
                                    {
                                        result[k, j] = ttemp[k];
                                        result[k, j + 1] = time[0];
                                    });
                                }
                                else
                                {
                                    ttemp = (object[])temp[0];
                                    sstemp = (object[])stemp[0];
                                    Parallel.For(0, ttemp.Length, k =>
                                    {
                                        result[k, j] = ttemp[k];
                                        result[k, j+1] = sstemp[k];
                                        result[k, j + 2] = time[0];
                                    });
                                }
                            });

                            int l = 0;
                            object[] insert;
                            if (storageFlag == false)
                            {
                                insert = new object[count+1];
                                foreach (object o in result)
                                {
                                    insert[l] = o;
                                    //if (l == count - 1)
                                    if (l + 1 == result.Length)
                                    {
                                        object[] insert1 = new object[count+1];         
                                        Array.Copy(insert, insert1, count + 1);
                                        //insert1[1] = time[0];
                                        data.Add(new KeyValuePair<string, object[]>(s, insert1));
                                        l = 0;
                                    }
                                    else
                                    {
                                        l++;
                                    }
                                }
                            }
                            else
                            {
                                count++;
                                insert = new object[count + 1];
                                foreach (object o in result)
                                {
                                    insert[l] = o;
                                    if (l == count)
                                    {
                                        object[] insert1 = new object[count + 1];
                                        Array.Copy(insert, insert1, count + 1);
                                        data.Add(new KeyValuePair<string, object[]>(s, insert1));
                                        l = 0;
                                    }
                                    else
                                    {
                                        l++;
                                    }
                                }
                            }
                        }

                    }
                    break;
                case "redis":
                    //data = new KeyValuePair<string, object[]>[DeviceList.Length];
                    if (keys[0] == "DB2")
                    {
                        BsonDocument bdoc2 = null;
                        List<string> storageSN1 = new List<string>();

                        foreach (string s in DeviceList)
                        {
                            bdoc2 = _ddb.GetLastRawData(string.Format("{0}-static", s));
                            try
                            {
                                foreach (BsonDocument bd in bdoc2["Storage"].AsBsonArray)
                                {
                                    storageSN1.Add(bd["SN"].AsString);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        foreach (string s in storageSN1)
                        {
                            RedisCacheDispatcher rcd = new RedisCacheDispatcher();
                            object[] obj = new object[2];
                            obj[0] = new object();
                            obj[0] = (Convert.ToInt16(rcd.GetCache(2, s)) == 1);
                            obj[1] = new object();
                            obj[1] = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                            data.Add(new KeyValuePair<string, object[]>(s, obj));
                        };

                        break;
                    }

                    foreach (string s in DeviceList)
                    {
                        RedisCacheDispatcher rcd = new RedisCacheDispatcher();
                        object[] obj = new object[2];
                        obj[0] = new object();
                        obj[0] = (Convert.ToInt16(rcd.GetStatus(s)) == 1);
                        obj[1] = new object();
                        obj[1] = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        data.Add(new KeyValuePair<string, object[]>(s, obj));
                    };
                    break;
                case "storageanalyzer":
                    DataDBDispatcher datadb1 = new DataDBDispatcher();
                    //List<BsonDocument> bstor = datadb1.GetRawData("StorageAnalyzer", null, 0);
                    BsonDocument bstor = null;
                    List<string> storageSN = new List<string>();
                    BsonDocument bdoc1 = null;
                    foreach (string s in DeviceList)
                    {
                        bdoc1 = _ddb.GetLastRawData(string.Format("{0}-static", s));
                        try
                        {
                            foreach (BsonDocument bd in bdoc1["Storage"].AsBsonArray)
                            {
                                storageSN.Add(bd["SN"].AsString);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    foreach (string element in storageSN)
                    {
                        //var query = $"\"SN\":\"{element}\"";
                        var queryObj = new
                        {
                            SN = element
                        };
                        bstor = datadb1.GetRawData("StorageAnalyzer", JsonConvert.SerializeObject(queryObj), 0).First();

                        try
                        {
                            var life_span_time = bstor["Lifespan"].AsBsonArray.Last().AsBsonDocument["time"];
                            data.Add(
                                new KeyValuePair<string, object[]>(
                                    bstor["SN"].AsString,
                                    new object[] {
                                        Convert.ToInt32(CommonFunctions.GetData(bstor["Lifespan"].AsBsonArray.Last().AsBsonDocument["data"])),
                                        bstor["SN"].AsString,
                                        Convert.ToInt32(CommonFunctions.GetData(life_span_time))}
                                    ));
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    break;
            }

            return data;
        }

        /// <summary>
        /// 23. Send the email message.
        /// </summary>        
        /// <param name="token">The administrator identity token</param>
        /// <param name="msgInfo">The information about email test</param>
        /// <returns></returns>
        /// <response code="200">Send test message successfully.</response>
        /// <response code="400">Bad request.</response>  
        /// <response code="403">The identity token not found</response>

        [HttpPost("DashboardAPI/Widget/SendEmail")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]

        public IActionResult SendEmailMessage([FromHeader]string token, [FromBody]EmailMessageTemplate msgInfo)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "POST",
                URL = "DashboardAPI/Widget/SendEmail",
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
                            Response = "Sorry, you do not have access to send the message."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "DashboardAPI/Widget/SendEmail",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do not have enough authorization.", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }

                    if (null == msgInfo.Id || 0 == msgInfo.Id.Length)
                    {
                        var retPayload = new
                        {
                            Response = "Please specify at least one recipient."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "DashboardAPI/Widget/SendEmail",
                            ResponseCode = 400,
                            Remark = "Please specify at least one recipient."
                        });

                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }

                    _ademail.Send(msgInfo);

                    var retPayload3 = new
                    {
                        Response = "Send message successfully."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "DashboardAPI/Widget/SendEmail",
                        ResponseCode = 200,
                        Remark = "Send message successfully."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload3));
                    
                }
                catch (Exception exc)
                {
                    var retPayload4 = new
                    {
                        Response = exc.Message
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "DashboardAPI/Widget/SendEmail",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload4));
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
                    Method = "POST",
                    URL = "DashboardAPI/Widget/SendEmail",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 23. Get Dashbaord Panel Setting (Core Service)
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="companyId">The company id</param>
        /// <returns></returns>
        /// <response code="200">Get dashboard panel item success.</response>
        /// <response code="204">Dashboard Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="403">The identity token not found</response> 
        [HttpGet("DashboardAPI/Panel/Setting")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult GetPanelSetting([FromHeader]string token)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    var retPayload = _adm.GetPanelInfo();

                    if (retPayload == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DashboardAPI/Panel/Setting",
                            ResponseCode = 204,
                            Remark = "Dashboard Not Found"
                        });

                        return StatusCode(204, JsonConvert.SerializeObject(retPayload));
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/Panel/Setting",
                        ResponseCode = 200,
                        Remark = "Get dashboard panel setting successfully."
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
                        URL = "DashboardAPI/Panel/Setting",
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
                    Response = ReturnCode.TokenError
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "DashboardAPI/Panel/Setting",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 23. Get New Panel Setting (Core Service)
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get dashboard panel item success.</response>
        /// <response code="204">Dashboard Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="403">The identity token not found</response> 
        [HttpGet("DashboardAPI/NewPanel/Setting")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult GetNewPanelSetting([FromHeader]string token)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    var retPayload = _adm.GetNewPanelInfo();

                    if (retPayload == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DashboardAPI/NewPanel/Setting",
                            ResponseCode = 204,
                            Remark = "There is not any new widget."
                        });
                        return StatusCode(204);

                    }

                    if (retPayload.WidgetSetting.Length == 0)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DashboardAPI/NewPanel/Setting",
                            ResponseCode = 204,
                            Remark = "The widget setting could not be found."
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/NewPanel/Setting",
                        ResponseCode = 200,
                        Remark = "Get new widget settings successfully."
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
                        URL = "DashboardAPI/NewPanel/Setting",
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
                    Response = ReturnCode.TokenError
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "DashboardAPI/NewPanel/Setting",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 24. Get company dashboard's widget data.
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="dashboardId">The dashboard Id</param>
        /// <returns></returns>
        /// <response code="200">Get company dashboard success</response>
        /// <response code="204">Dashboard is be deleted./</response>
        /// <response code="400">Bad Request</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("DashboardAPI/Widget/Data")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(410)]
        [ProducesResponseType(500)]
        public IActionResult GetWidgetData([FromHeader]string token, int dashboardId)
        { 
            string user = _rcd.GetCache(0, token);

            if (user != null)
            {          
                if (false == _adm.DashboardExist(dashboardId))
                {
                    var retPayload1 = new
                    {
                        Response = "Dashbaord is be deleted."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/Widget/data",
                        ResponseCode = 410,
                        Remark = "Dashbaord is be deleted."
                    });
                    return StatusCode(410, JsonConvert.SerializeObject(retPayload1));
                }

                int[] widgetOrder = null;

                try
                {
                    widgetOrder = _adm.GetWidgetId(dashboardId);
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/Widget/data",
                        ResponseCode = 500,
                        Remark = "[AdminDB] " + exc.Message
                    });
                    return StatusCode(500, JsonConvert.SerializeObject(exc.Message));
                }

                if (widgetOrder == null)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/Widget/data",
                        ResponseCode = 204,
                        Remark = "There is not any widget."
                    });
                    return StatusCode(204);
                }

                List<string> retJsonStr = new List<string>();

                foreach (int widgetId in widgetOrder)
                {
                    try
                    {
                        BsonDocument dd_document = _ddb.GetLastRawData("Widget" + widgetId);

                        if (dd_document != null)
                        {
                            dd_document.AsBsonDocument.Remove("detailWidget");
                            dd_document.AsBsonDocument.Remove("_id");
                            dd_document.AsBsonDocument.Add("id", widgetId);
                            retJsonStr.Add(dd_document.ToJson());
                        }
                        else
                        {
                            retJsonStr.Add(JsonConvert.SerializeObject(_adw.GetWidgetNameAndWidth(widgetId)));
                        }
                    }
                    catch (Exception exe)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DashboardAPI/Widget/data",
                            ResponseCode = 404,
                            Remark = $"[Widget{widgetId}] " + exe.Message
                        });
                        retJsonStr.Add(JsonConvert.SerializeObject(_adw.GetWidgetNameAndWidth(widgetId)));
                    }
                }

                var retPayload = new
                {
                    retJsonStr,
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "DashboardAPI/Widget/data",
                    ResponseCode = 200,
                    Remark = "Get dashboard widget successfully."
                });
                return StatusCode(200, JsonConvert.SerializeObject(retPayload));
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
                    URL = "DashboardAPI/Widget/data",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 25. Get widget details on dashboard page.
        /// </summary>
        /// <param name="dashboardId">The dashboard Id</param>
        /// <param name="widgetId">The widget Id</param>
        /// <param name="index">The widget data's index</param>
        /// <param name="token">The administrator identity token</param>
        /// <returns></returns>
        /// <response code="200">Get widget's detail data successfully.</response>
        /// <response code="204">The widget was been removed.</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">The data DB Error</response>
        [HttpGet("DashboardAPI/Widget/Detail")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetWidgetDetails([FromHeader] string token, int dashboardId, int widgetId, int index)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                if (_adm.WidgetInDashboard(dashboardId, widgetId) == false)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/Widget/Detail",
                        ResponseCode = 204,
                        Remark = "The widget was been removed."
                    });
                    return StatusCode(204);
                }

                BsonDocument dd_document = null;
                BsonDocument widgetDetail = null;
                try
                {
                    dd_document = _ddb.GetLastRawData("Widget" + widgetId);
                    widgetDetail = dd_document["detailWidget"].AsBsonDocument;
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/Widget/Detail",
                        ResponseCode = 500,
                        Remark = "[DataDB] " + exc.Message
                    });
                    return StatusCode(500, JsonConvert.SerializeObject(exc.Message));
                }

                string item = null;
                string record = null;

                try
                {
                    item = widgetDetail["item"].AsBsonArray.ToJson();
                }
                catch(Exception exc)
                {
                    item = null;
                }

                try
                {
                    record = widgetDetail["record"].AsBsonArray[index].ToJson();
                }
                catch(Exception exc)
                {
                    record = null;
                }

                var retPayload = new
                {
                    item, record 
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "DashboardAPI/Widget/Detail",
                    ResponseCode = 200,
                    Remark = "Get Dashboard widget's detail information successfully."
                });
                return StatusCode(200, JsonConvert.SerializeObject(retPayload));
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
                    URL = "DashboardAPI/Widget/Detail",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 30. Get Dashboard Home Page 
        /// </summary>
        /// <param name="token">The identity token</param>        
        /// <returns></returns>
        /// <response code="200">Get dashboard name list success.</response>
        /// <response code="204">The dashboard could not be found!.</response>  
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response> 
        [HttpGet("DashboardAPI/home")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetHomePage([FromHeader]string token)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                SelectOptionTemplate[] dashboardList;

                try
                {
                    dashboardList = _adm.GetDashboardList();
                }
                catch (Exception exc)
                {
                    var retPayload1 = new
                    {
                        Response = exc.Message
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/home",
                        ResponseCode = 500,
                        Remark = "[AdminDB]Get DashboardList Error." + exc.Message
                    });
                    return StatusCode(500, JsonConvert.SerializeObject(retPayload1));
                }

                if (dashboardList[0] == null)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/home",
                        ResponseCode = 204,
                        Remark = "The dashboard could not be found!"
                    });
                    return StatusCode(204);
                }

                int dashboardId = dashboardList[0].Id;
                int[] widgetOrder = null;
                try
                {
                    widgetOrder = _adm.GetWidgetId(dashboardId);
                }
                catch(Exception exc)
                {
                    var retPayload1 = new
                    {
                        Response = exc.Message
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/home",
                        ResponseCode = 500,
                        Remark = "[AdminDB]Get WidgetId Error." + exc.Message
                    });
                    return StatusCode(500, JsonConvert.SerializeObject(retPayload1));
                }

                if (widgetOrder == null)
                {
                    var retPayload1 = new
                    {
                        dashboardList,
                        retJsonStr = (string)null,
                        isLatest = true
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/home",
                        ResponseCode = 200,
                        Remark = "Get Dashboard widget data successfully."
                    });
                    return StatusCode(200, JsonConvert.SerializeObject(retPayload1));
                }

                List<string> retJsonStr = new List<string>();

                foreach (int widgetId in widgetOrder)
                {
                    try
                    {
                        BsonDocument dd_document = _ddb.GetLastRawData("Widget" + widgetId);

                        if (dd_document != null)
                        {
                            dd_document.AsBsonDocument.Remove("detailWidget");
                            dd_document.AsBsonDocument.Remove("_id");
                            dd_document.AsBsonDocument.Add("id", widgetId);
                            retJsonStr.Add(dd_document.ToJson());
                        }
                        else
                        {
                            retJsonStr.Add(JsonConvert.SerializeObject(_adw.GetWidgetNameAndWidth(widgetId)));
                        }
                    }
                    catch (Exception exe)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DashboardAPI/home",
                            ResponseCode = 404,
                            Remark = $"[Widget{widgetId}] " + exe.Message
                        });
                        retJsonStr.Add(JsonConvert.SerializeObject(_adw.GetWidgetNameAndWidth(widgetId)));
                    }
                }

                var retPayload = new
                {
                    dashboardList,
                    retJsonStr,
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "DashboardAPI/home",
                    ResponseCode = 200,
                    Remark = "Get dashboard home page successfully."
                });
                return StatusCode(200, JsonConvert.SerializeObject(retPayload));

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
                    URL = "DashboardAPI/home",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 30. Get Widget information
        /// </summary>
        /// <param name="token">The identity token</param>      
        /// <param name="widgetId">The widget id</param> 
        /// <returns></returns>
        /// <response code="200">Get dashboard name list success.</response>
        /// <response code="204">The dashboard could not be found!.</response>  
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response> 
        [HttpGet("DashboardAPI/WidgetInfo")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]

        public IActionResult GetWidgetInfo([FromHeader]string token, int widgetId)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    BsonDocument dd_document = _ddb.GetLastRawData("Widget" + widgetId);

                    if (dd_document != null)
                    {
                        dd_document.AsBsonDocument.Remove("detailWidget");
                        dd_document.AsBsonDocument.Remove("_id");
                        dd_document.AsBsonDocument.Add("id", widgetId);
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DashboardAPI/WidgetInfo",
                            ResponseCode = 200,
                            Remark = "Get widget information successfully."
                        });
                        return StatusCode(200, dd_document.ToJson());
                    }
                    else
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DashboardAPI/WidgetInfo",
                            ResponseCode = 204,
                            Remark = $"Widget{widgetId} could not be found."
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
                        Method = "GET",
                        URL = "DashboardAPI/WidgetInfo",
                        ResponseCode = 204,
                        Remark = exc.Message
                    });
                    return StatusCode(204);
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
                    URL = "DashboardAPI/WidgetInfo",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 31. Get device specific data.
        /// </summary>
        /// <param name="id">The widget Id</param>
        /// <param name="token">The administrator identity token</param>
        /// <param name="devName">The device name</param>
        /// <returns></returns>
        /// <response code="200">Get widget data success</response>
        /// <response code="400">The identity token not found</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="404">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("DashboardAPI/Widget/GetDeviceData")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetDeviceData([FromHeader] string token, int id, string devName)
        {
            //AdminDBDispatcher._widget adw = new AdminDBDispatcher._widget();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "DashboardAPI/Widget/GetDeviceData",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                PanelItemTemplate panelSetting = _adw.GetPanelSetting(id);
                DeviceProfileTemplate device = _adw.GetDevice(devName);
                List<DeviceTemplate> payload = new List<DeviceTemplate>();
                DeviceTemplate devTemplate = null;
                List<KeyValuePair<string, object[]>> data = null;
                List<KeyValuePair<string, object[]>> denominatorData = null;
                SettingItemTemplate sit = JsonConvert.DeserializeObject<SettingItemTemplate>(panelSetting.SettingStr);
                double value;
                ShareLibrary.AdminDB.Threshold threshold;
                string location = null;
                double denominator = 100;
                int denoIndex = 0;
                try
                {
                    if (panelSetting.GroupId != 5)
                    {
                        switch (sit.Func) //Threshold Percentage boolean  Numerical
                        {
                            case 1:
                                if (panelSetting.ThresholdId != null)
                                {
                                    threshold = _adth.GetThresholdSetting(Convert.ToInt32(panelSetting.ThresholdId));
                                    location = _adata.GetDataLocation(threshold.DataId);
                                    data = GetDetailData(new string[] { devName }, location, panelSetting.DataCount);
                                    if (data.Count != 0)
                                    {
                                        value = Convert.ToDouble(data[0].Value[0]);

                                        if (threshold.DenominatorId != null)
                                        {
                                            
                                            denominatorData = GetDetailData(new string[] { devName }, _adata.GetDataLocation((int)threshold.DenominatorId), 1);

                                            if (0 == denominatorData.Count)
                                            {
                                                break;
                                            }
                                            denominator = Convert.ToDouble(denominatorData[0].Value[0]);
                                            value = (value / denominator) * 100;

                                            
                                        }
                                        devTemplate = new DeviceTemplate()
                                        {
                                            Value = Math.Round(value).ToString(),
                                            Time = Convert.ToInt32(data[0].Value[1]),
                                            OwnerName = device.OwnerName,
                                            BranchId = device.BranchId,
                                            BranchName = device.BranchName,
                                        };
                                        payload.Add(devTemplate);
                                    }
                                }
                                break;
                            case 4:
                                data = GetDetailData(new string[] { devName }, panelSetting.DataLocation, panelSetting.DataCount);
                                if (data.Count != 0)
                                {
                                    value = Convert.ToDouble(data[0].Value[0]);
                                    devTemplate = new DeviceTemplate()
                                    {
                                        //Value = String.Format("{0:0.###}", value).ToString(),
                                        Value = Math.Round(value).ToString(),
                                        Time = Convert.ToInt32(data[0].Value[1]),
                                        OwnerName = device.OwnerName,
                                        BranchId = device.BranchId,
                                        BranchName = device.BranchName,
                                    };
                                    payload.Add(devTemplate);
                                }
                                break;
                            case 2: //Percentage
                                double[] cond = sit.Divider.Percentage;
                                if (0 != sit.Divider.DenominatorId)
                                {
                                    AdminDBDispatcher._Data adata = new AdminDBDispatcher._Data();
                                    Data d = adata.Get(sit.Divider.DenominatorId);
                                    data = GetDetailData(new string[] { devName }, panelSetting.DataLocation, panelSetting.DataCount);

                                    if (data.Count != 0)
                                    {
                                        denominatorData = GetDetailData(new string[] { devName }, d.Location, 1);

                                        if (0 == denominatorData.Count)
                                        {
                                            break;
                                        }
                                        denominator = Convert.ToDouble(denominatorData[0].Value[0]);
                                        value = (Convert.ToDouble(data[0].Value[0]) / denominator) * 100;
                                        devTemplate = new DeviceTemplate()
                                        {
                                            Value = Math.Round(value).ToString(),
                                            Time = Convert.ToInt32(data[0].Value[1]),
                                            OwnerName = device.OwnerName,
                                            BranchId = device.BranchId,
                                            BranchName = device.BranchName,
                                        };
                                        payload.Add(devTemplate);
                                    }
                                }
                                else
                                {
                                    data = GetDetailData(new string[] { devName }, panelSetting.DataLocation, panelSetting.DataCount);
                                    if (data.Count != 0)
                                    {
                                        value = (Convert.ToDouble(data[0].Value[0]) / denominator) * 100;
                                        devTemplate = new DeviceTemplate()
                                        {
                                            Value = Math.Round(value).ToString(),
                                            Time = Convert.ToInt32(data[0].Value[1]),
                                            OwnerName = device.OwnerName,
                                            BranchId = device.BranchId,
                                            BranchName = device.BranchName,
                                        };
                                        payload.Add(devTemplate);
                                    }
                                }
                                break;
                            case 3: //Boolean
                                data = GetDetailData(new string[] { devName }, panelSetting.DataLocation, panelSetting.DataCount);

                                if (panelSetting.DataLocation.Contains("DB1"))
                                {
                                    if (data.Count != 0)
                                    {
                                        devTemplate = new DeviceTemplate()
                                        {
                                            //Value = "Online",
                                            //StorageSN = ky.Value[1].ToString(),
                                            Value = ((bool)data[0].Value[0] == true) ? "Online" : "Offline",
                                            Time = Convert.ToInt32(data[0].Value[1]),
                                            OwnerName = device.OwnerName,
                                            BranchId = device.BranchId,
                                            BranchName = device.BranchName,
                                        };
                                        payload.Add(devTemplate);
                                    }
                                }
                                else
                                {
                                    foreach (var item in data)
                                    {
                                        devTemplate = new DeviceTemplate()
                                        {

                                            StorageSN = item.Key,
                                            Value = ((bool)item.Value[0] == true) ? "Online" : "Offline",
                                            Time = Convert.ToInt32(item.Value[1]),
                                            OwnerName = device.OwnerName,
                                            BranchId = device.BranchId,
                                            BranchName = device.BranchName,
                                        };
                                        payload.Add(devTemplate);
                                    }
                                }
                                break;
                        }
                    }
                    else
                    { //Storage
                        switch (sit.Func) //Threshold Percentage Numerical
                        {
                            case 1:
                                if (panelSetting.ThresholdId != null)
                                {
                                    threshold = _adth.GetThresholdSetting(Convert.ToInt32(panelSetting.ThresholdId));
                                    location = _adata.GetDataLocation(threshold.DataId);

                                    data = GetDetailData(new string[] { devName }, location, panelSetting.DataCount);

                                    if (data.Count != 0)
                                    {
                                        if (threshold.DenominatorId != null)
                                        {
                                            denominatorData = GetDetailData(new string[] { devName }, _adata.GetDataLocation((int)threshold.DenominatorId), 1);

                                            if (0 == denominatorData.Count)
                                            {
                                                break;
                                            }

                                            foreach (KeyValuePair<string, object[]> ky in data)
                                            {
                                                denominator = Convert.ToDouble(denominatorData[denoIndex++].Value[0]);
                                                value = (Convert.ToDouble(ky.Value[0]) / denominator) * 100;
                                                devTemplate = new DeviceTemplate()
                                                {
                                                    Value = Math.Round(value).ToString(),
                                                    StorageSN = ky.Value[1].ToString(),
                                                    Time = Convert.ToInt32(ky.Value[2]),
                                                    OwnerName = device.OwnerName,
                                                    BranchId = device.BranchId,
                                                    BranchName = device.BranchName,
                                                };
                                                payload.Add(devTemplate);
                                            }
                                        }
                                        else
                                        {
                                            foreach (KeyValuePair<string, object[]> ky in data)
                                            {
                                                value = (Convert.ToDouble(ky.Value[0]) / denominator) * 100;
                                                devTemplate = new DeviceTemplate()
                                                {
                                                    Value = Math.Round(value).ToString(),
                                                    StorageSN = ky.Value[1].ToString(),
                                                    Time = Convert.ToInt32(ky.Value[2]),
                                                    OwnerName = device.OwnerName,
                                                    BranchId = device.BranchId,
                                                    BranchName = device.BranchName,
                                                };
                                                payload.Add(devTemplate);
                                            }
                                        }
                                    }
                                }
                                break;
                            case 4:
                                data = GetDetailData(new string[] { devName }, panelSetting.DataLocation, panelSetting.DataCount);
                                if (data.Count != 0)
                                {
                                    foreach (KeyValuePair<string, object[]> ky in data)
                                    {
                                        value = Convert.ToDouble(ky.Value[0]);
                                        devTemplate = new DeviceTemplate()
                                        {
                                            //Value = String.Format("{0:0.###}", ky.Value[0]).ToString(),
                                            Value = Math.Round(value).ToString(),
                                            StorageSN = ky.Value[1].ToString(),
                                            Time = Convert.ToInt32(ky.Value[2]),
                                            OwnerName = device.OwnerName,
                                            BranchId = device.BranchId,
                                            BranchName = device.BranchName,
                                        };
                                        payload.Add(devTemplate);
                                    }
                                }
                                break;
                            case 2: //Percentage
                                double[] cond = sit.Divider.Percentage;
                                denominator = 100;
                                if (0 != sit.Divider.DenominatorId)
                                {
                                    AdminDBDispatcher._Data adata = new AdminDBDispatcher._Data();
                                    Data d = adata.Get(sit.Divider.DenominatorId);

                                    data = GetDetailData(new string[] { devName }, panelSetting.DataLocation, panelSetting.DataCount);
                                    if (data.Count != 0)
                                    {
                                        denominatorData = GetDetailData(new string[] { devName }, d.Location, 1);
                                        if (0 == denominatorData.Count)
                                        {
                                            break;
                                        }
                                        foreach (KeyValuePair<string, object[]> ky in data)
                                        {

                                            denominator = Convert.ToDouble(denominatorData[denoIndex++].Value[0]);
                                            value = (Convert.ToDouble(ky.Value[0]) / denominator) * 100;
                                            devTemplate = new DeviceTemplate()
                                            {
                                                Value = Math.Round(value).ToString(),
                                                StorageSN = ky.Value[1].ToString(),
                                                Time = Convert.ToInt32(ky.Value[2]),
                                                OwnerName = device.OwnerName,
                                                BranchId = device.BranchId,
                                                BranchName = device.BranchName,
                                            };
                                            payload.Add(devTemplate);
                                        }
                                    }
                                }
                                else
                                {
                                    data = GetDetailData(new string[] { devName }, panelSetting.DataLocation, panelSetting.DataCount);
                                    if (data.Count != 0)
                                    {
                                        foreach (KeyValuePair<string, object[]> ky in data)
                                        {
                                            value = (Convert.ToDouble(ky.Value[0]) / denominator) * 100;
                                            devTemplate = new DeviceTemplate()
                                            {
                                                Value = Math.Round(value).ToString(),
                                                StorageSN = ky.Value[1].ToString(),
                                                Time = Convert.ToInt32(ky.Value[2]),
                                                OwnerName = device.OwnerName,
                                                BranchId = device.BranchId,
                                                BranchName = device.BranchName,
                                            };
                                            payload.Add(devTemplate);
                                        }
                                    }
                                }
                                break;
                        }
                    }

                    if (payload.Count == 0)
                    {
                        var retPayload1 = new
                        {
                            Response = "Not Found"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DashboardAPI/Widget/GetDeviceData",
                            ResponseCode = 404,
                            Remark = "[DashboardAPI/Widget/GetDeviceData]Data Not Found"
                        });

                        return StatusCode(404, JsonConvert.SerializeObject(retPayload1));
                    }

                    var record = payload.ToArray();

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DashboardAPI/Widget/GetDeviceData",
                        ResponseCode = 200,
                        Remark = "Get device data successfully!"
                    });

                    var retPayload = new
                    {
                        record,
                        emp = _ade.GetName(),
                    };

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                catch (Exception exc)
                {
                    if (id < 1 || devName == null)
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
                            URL = "DashboardAPI/Widget/GetDeviceData",
                            ResponseCode = 400,
                            Remark = exc.Message
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }
                    else
                    {
                        var retPayload = new
                        {
                            Response = "Internal Server Error"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DashboardAPI/Widget/GetDeviceData",
                            ResponseCode = 500,
                            Remark = exc.Message
                        });
                        return StatusCode(500, JsonConvert.SerializeObject(retPayload));
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
                    Method = "GET",
                    URL = "DashboardAPI/Widget/GetDeviceData",
                    ResponseCode = 403,
                    Remark = "[DashboardAPI/Widget/GetDeviceData]Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }
    }
}
