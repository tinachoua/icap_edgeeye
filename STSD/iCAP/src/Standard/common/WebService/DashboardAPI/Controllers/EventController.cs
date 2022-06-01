using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShareLibrary;
using Newtonsoft.Json;
using MongoDB.Bson;
using DashboardAPI.Models.Event;
using ShareLibrary.DataTemplate;
using ShareLibrary.Interface;
using ShareLibrary.AdminDB;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Net.Http;
using Newtonsoft.Json.Linq;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DashboardAPI.Controllers
{
    public class EventController : Controller
    {
        private IRedisCacheDispatcher _rcd;
        private IDevice _add;
        private IDataDBDispatcher _ddb;
        private IEmail _adEmail;
        private IEmployee _adEmployee;
        private static Mutex mut = new Mutex();

        public EventController(IRedisCacheDispatcher rcd, IDevice add, IDataDBDispatcher ddb, IEmail adEmail,IEmployee adEmployee)
        {
            _rcd = rcd;
            _add = add;
            _ddb = ddb;
            _adEmail = adEmail;
            _adEmployee = adEmployee;
        }

        /// <summary>
        /// 2. Get all event logs
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="count">The event log's count</param>
        /// <returns></returns>
        /// <response code="200">Get all event data success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("EventAPI/GetAll")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public IActionResult GetAll([FromHeader]string token, int count)
        {
            //AdminDBDispatcher._device add = new AdminDBDispatcher._device();
            //DataDBDispatcher ddb = new DataDBDispatcher();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                List<BsonDocument> bson = _ddb.GetRawData("EventLog", null, count);
                Dictionary<string,string> dev_owner = _add.GetOwner_All();
                Dictionary<string, string> dev_alias = _add.GetAlias_All();
                //List<EventDataTemplate> retitem = new List<EventDataTemplate>();
                EventDataTemplate[] retitem = new EventDataTemplate[bson.Count];

#if false
                foreach (BsonDocument bd in bson)
                {
                    EventDataTemplate evd = new EventDataTemplate();
                    evd.devName = bd["Dev"].AsString;
                    evd.eventclass = bd["Class"].AsString;
                    evd.EventId = bd["_id"].AsObjectId.ToString();
                    evd.info = bd["Message"].AsString;
                    evd.IsChecked = bd["Checked"].AsBoolean;
                    evd.level = Convert.ToInt32(CommonFunctions.GetData(bd["Level"]));
                    evd.owner = _add.GetOwner(bd["Dev"].AsString);//slow
                    evd.time = CommonFunctions.epoch2string(Convert.ToInt32(CommonFunctions.GetData(bd["Time"])));
                    retitem.Add(evd);
                }
#endif

#if true
                Parallel.For(0, bson.Count, i =>
                {
                    try
                    {
                        EventDataTemplate evd = new EventDataTemplate();
                        evd.devName = bson[i]["Dev"].AsString;
                        evd.alias = dev_alias[evd.devName];
                        //evd.eventclass = bson[i]["Class"].AsString;
                        evd.EventId = bson[i]["_id"].AsObjectId.ToString();
                        evd.info = bson[i]["Message"].AsString;
                        evd.IsChecked = bson[i]["Checked"].AsBoolean;
                        //evd.level = Convert.ToInt32(CommonFunctions.GetData(bson[i]["Level"]));
                        evd.owner = dev_owner[bson[i]["Dev"].AsString]; //fast
                        evd.time = Convert.ToInt32(CommonFunctions.GetData(bson[i]["Time"]));
                        //evd.name = dev_alias[evd.devName] ?? bson[i]["Dev"].AsString;
                        evd.name = (dev_alias[evd.devName] == null || dev_alias[evd.devName].Length == 0) ? bson[i]["Dev"].AsString : dev_alias[evd.devName];
                        evd.index = i+1;
                        evd.action = (bson[i]["Checked"].AsBoolean)? "" : "<button name = \"btn-solve\" class = \"btn btn-mini btn-dark\">Solve</button>";
                        retitem[i] = evd;
                    }
                    catch (Exception ex)
                    {
                        // If there has any expection, skip this data.                        
                        //Console.WriteLine("[EventAPI/GetAll]Event data has expection, message:{0}", ex.Message);
                    }
                });
#endif

                //Console.WriteLine("[EventAPI/GetAll]Get all event data success.");
                return StatusCode(200, JsonConvert.SerializeObject(retitem));
            }
            else
            {
                var retPayload = new
                {
                    Response = "authentication error"
                };

                //Console.WriteLine("[EventAPI/GetAll]Request authentication token error.");
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 3. Get new event logs
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="count">The event log's count</param>
        /// <returns></returns>
        /// <response code="200">Get new event data success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("EventAPI/GetNew")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public IActionResult GetNew([FromHeader]string token, int count)
        {
            //AdminDBDispatcher._device add = new AdminDBDispatcher._device();
            //DataDBDispatcher ddb = new DataDBDispatcher();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                List<BsonDocument> bson = _ddb.GetRawData("EventLog", "{\"Checked\":false}", count);
                List<EventDataTemplate> retitem = new List<EventDataTemplate>();
                Dictionary<string, string> dev_alias = _add.GetAlias_All();
                Dictionary<string, string> dev_owner = _add.GetOwner_All();
                int index = 1;
                foreach (BsonDocument bd in bson)
                {
                    EventDataTemplate evd = new EventDataTemplate();
                    evd.devName = bd["Dev"].AsString;
                    evd.alias = dev_alias[evd.devName];
                    //evd.eventclass = bd["Class"].AsString;
                    evd.EventId = bd["_id"].AsObjectId.ToString();
                    evd.info = bd["Message"].AsString;
                    evd.IsChecked = bd["Checked"].AsBoolean;
                    //evd.level = Convert.ToInt32(CommonFunctions.GetData(bd["Level"]));
                    //evd.owner = _add.GetOwner(bd["Dev"].AsString);
                    evd.owner = dev_owner[evd.devName];
                    evd.time = Convert.ToInt32(CommonFunctions.GetData(bd["Time"]));
                    evd.index = index++;
                    //evd.name = dev_alias[evd.devName] ?? bd["Dev"].AsString;
                    evd.name = (dev_alias[evd.devName] == null || dev_alias[evd.devName].Length == 0) ? bd["Dev"].AsString : dev_alias[evd.devName];
                    evd.action = "<button name = \"btn-solve\" class = \"btn btn-mini btn-dark\">Solve</button>";
                    retitem.Add(evd);
                }
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "EventAPI/GetNew",
                    ResponseCode = 200,
                    Remark = "Get new event data success."
                });

                return StatusCode(200, JsonConvert.SerializeObject(retitem.ToArray()));
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
                    URL = "EventAPI/GetNew",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 4. Get already done event logs
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="count">The event log's count</param>
        /// <returns></returns>
        /// <response code="200">Get already done event data success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("EventAPI/GetDone")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public IActionResult GetDone([FromHeader]string token, int count)
        {
            //AdminDBDispatcher._device add = new AdminDBDispatcher._device();
            //DataDBDispatcher ddb = new DataDBDispatcher();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                List<BsonDocument> bson = _ddb.GetRawData("EventLog", "{\"Checked\":true}", count);
                List<EventDataTemplate> retitem = new List<EventDataTemplate>();
                Dictionary<string, string> dev_alias = _add.GetAlias_All();
                Dictionary<string, string> dev_owner = _add.GetOwner_All();
                int index = 1;
                foreach (BsonDocument bd in bson)
                {
                    EventDataTemplate evd = new EventDataTemplate();
                    evd.devName = bd["Dev"].AsString;
                    evd.alias = dev_alias[evd.devName];
                    //evd.eventclass = bd["Class"].AsString;
                    evd.EventId = bd["_id"].AsObjectId.ToString();
                    evd.info = bd["Message"].AsString;
                    evd.IsChecked = bd["Checked"].AsBoolean;
                    //evd.level = Convert.ToInt32(CommonFunctions.GetData(bd["Level"]));
                    //evd.owner = _add.GetOwner(bd["Dev"].AsString);
                    evd.owner = dev_owner[evd.devName];
                    evd.time = Convert.ToInt32(CommonFunctions.GetData(bd["Time"]));
                    evd.index = index++;
                    //evd.name = dev_alias[evd.devName] ?? bd["Dev"].AsString;
                    evd.name = (dev_alias[evd.devName] == null || dev_alias[evd.devName].Length == 0) ? bd["Dev"].AsString : dev_alias[evd.devName];
                    evd.action = "";
                    retitem.Add(evd);
                }

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "EventAPI/GetDone",
                    ResponseCode = 200,
                    Remark = "Get new event data success."
                });

                return StatusCode(200, JsonConvert.SerializeObject(retitem.ToArray()));
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
                    URL = "EventAPI/GetDone",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 5. Update Event Log
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="eventData">The event data needs to updated.</param>
        /// <returns></returns>
        /// <response code="202">Update event data success</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="406">Update event data fail</response>
        [HttpPut("EventAPI/Update")]
        [ProducesResponseType(202)]
        [ProducesResponseType(403)]
        [ProducesResponseType(406)]
        public IActionResult Update([FromHeader]string token, [FromBody]EventDataTemplate eventData)
        {
            //DataDBDispatcher ddb = new DataDBDispatcher();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                if (!_ddb.UpdateEventlog(eventData))
                {
                    var retPayload1 = new
                    {
                        Response = "Fail"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "EventAPI/Update",
                        ResponseCode = 406,
                        Remark = "Update event data fail."
                    });

                    return StatusCode(406, JsonConvert.SerializeObject(retPayload1));
                }

                var retPayload = new
                {
                    Response = "Success"
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "PUT",
                    URL = "EventAPI/Update",
                    ResponseCode = 202,
                    Remark = "Update event data success."
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
                    URL = "EventAPI/Update",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 6. Set the EmailSetting.
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="payload">The email setting data needs to updated.</param>
        /// <returns></returns>       
        /// <response cdoe="200">Success to update the email setting.</response>   
        /// <response code="400">
        ///                      1. Payload data error or happen exception.
        ///                      2. Payload is null.</response>
        /// <response code="403">
        ///                      1. The identity token not found
        ///                      2. User do not have enough authorization.</response>      
        [HttpPut("EventAPI/SetEmail")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        //[ProducesResponseType(406)]
        public IActionResult SetEmail([FromHeader]string token, [FromBody]EmailSettingTemplate payload)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "PUT",
                URL = "EventAPI/SetEmail",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {

                //check payload data                
                try
                {
                    if (!_adEmployee.CheckAdmin(user))
                    {

                        var retPayload = new
                        {
                            ErrorCode = 1,
                            Response = string.Format("{0} do not have enough authorization.", user)
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "EventAPI/SetEmail",
                            ResponseCode = 403,
                            Remark = string.Format("{0} do not have enough authorization.", user)
                        });

                        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                    }
                    else
                    {
                        mut.WaitOne();
                        if (_adEmail.CteateOrUpdate(payload))
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
                                URL = "EventAPI/SetEmail",
                                ResponseCode = 200,
                                Remark = string.Format("Set the email setting success")
                            });
                            mut.ReleaseMutex();
                            return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                        }
                        else
                        {
                            var retPayload = new
                            {
                                Response = "SMTP Address, Port Number, E-mail Address, Password, and Enable SSL can not be null."
                            };

                            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                            {
                                Direction = true,
                                Name = user,
                                Method = "PUT",
                                URL = "EventAPI/SetEmail",
                                ResponseCode = 400,
                                Remark = string.Format("SMTP Address, Port Number, E - mail Address, Password, and Enable SSL can not be null.")
                            });
                            mut.ReleaseMutex();
                            return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                        }
                    }
                }
                catch (ArgumentException exc)
                {
                    var retPayload1 = new
                    {
                        Response = "Do not receive any email infomation.Please Retry."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "EventAPI/SetEmail",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    mut.ReleaseMutex();
                    return StatusCode(400, JsonConvert.SerializeObject(exc.Message));
                }                    
                catch(DbUpdateException exc)
                {
                    if(exc.InnerException.Message.Contains("emailFrom") && exc.InnerException.Message.Contains("null"))
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "EventAPI/SetEmail",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        mut.ReleaseMutex();
                        return StatusCode(400, "The E-mail Address can not be empty");
                    }
                    else
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "EventAPI/SetEmail",
                            ResponseCode = 400,
                            Remark = exc.InnerException.Message
                        });
                        mut.ReleaseMutex();
                        return StatusCode(400, "Set E-mail Failed!");
                    }
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "EventAPI/SetEmail",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    mut.ReleaseMutex();
                    return StatusCode(400, "Data Error");
                }                
            }
            else
            {
                var retPayload = new
                {
                    ErrorCode = 0,
                    Response = "authentication error"
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "PUT",
                    URL = "EventAPI/SetEmail",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }
        //public IActionResult SetEmail([FromHeader]string token, [FromBody]EmailSettingTemplate payload)
        //{
        //    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //    {
        //        Direction = false,
        //        Name = "",
        //        Method = "PUT",
        //        URL = "EventAPI/SetEmail",
        //        ResponseCode = 0,
        //        Remark = ""
        //    });

        //    string user = _rcd.GetCache(0, token);
        //    if (user != null)
        //    {
        //        if (payload == null)
        //        {

        //            var retPayload = new
        //            {
        //                Response = "no contain data"
        //            };

        //            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //            {
        //                Direction = true,
        //                Name = user,
        //                Method = "PUT",
        //                URL = "EventAPI/SetEmail",
        //                ResponseCode = 400,
        //                Remark = "Request does not contain payload."
        //            });
        //            return StatusCode(400, JsonConvert.SerializeObject(retPayload));
        //        }

        //        //check payload data

        //        if(payload.emailFrom==null)
        //        {
        //            var retPayload = new
        //            {                       
        //                Response = "There is not emailFrom in the payload data."
        //            };

        //            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //            {
        //                Direction = true,
        //                Name = user,
        //                Method = "PUT",
        //                URL = "EventAPI/SetEmail",
        //                ResponseCode = 400,                        
        //                Remark= "There is no emailFrom in the payload data."
        //            });
        //            return StatusCode(400, JsonConvert.SerializeObject(retPayload));
        //        }

        //        if (!_adEmployee.CheckAdmin(user))
        //        {

        //            var retPayload = new
        //            {
        //                Response = string.Format("{0} do not have enough authorization.", user)
        //            };

        //            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //            {
        //                Direction = true,
        //                Name = user,
        //                Method = "PUT",
        //                URL = "EventAPI/SetEmail",
        //                ResponseCode = 403,
        //                Remark = string.Format("{0} do not have enough authorization.", user)
        //            });

        //            return StatusCode(403, JsonConvert.SerializeObject(retPayload));

        //        }
        //        else
        //        {
        //            try
        //            {
        //                if (_adEmail.CteateOrUpdate(payload))
        //                {
        //                    var retPayload = new
        //                    {
        //                        Response = "Success"
        //                    };

        //                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //                    {
        //                        Direction = true,
        //                        Name = user,
        //                        Method = "PUT",
        //                        URL = "EventAPI/SetEmail",
        //                        ResponseCode = 202,
        //                        Remark = string.Format("Set the email setting success")
        //                    });

        //                    return StatusCode(202, JsonConvert.SerializeObject(retPayload));
        //                }
        //                else
        //                {
        //                    var retPayload = new
        //                    {
        //                        Response = "Bad Request"
        //                    };

        //                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //                    {
        //                        Direction = true,
        //                        Name = user,
        //                        Method = "PUT",
        //                        URL = "EventAPI/SetEmail",
        //                        ResponseCode = 400,
        //                        Remark = string.Format("Bad Request")
        //                    });

        //                    return StatusCode(400, JsonConvert.SerializeObject(retPayload));
        //                }
        //            }
        //            catch(Exception exc)
        //            {
        //                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //                {
        //                    Direction = true,
        //                    Name = user,
        //                    Method = "PUT",
        //                    URL = "EventAPI/SetEmail",
        //                    ResponseCode = 400,
        //                    Remark = exc.Message
        //                });
        //                return StatusCode(400, JsonConvert.SerializeObject(exc.Message));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var retPayload = new
        //        {
        //            Response = "authentication error"
        //        };

        //        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
        //        {
        //            Direction = true,
        //            Name = user,
        //            Method = "PUT",
        //            URL = "EventAPI/SetEmail",
        //            ResponseCode = 403,
        //            Remark = "Request authentication token error."
        //        });

        //        return StatusCode(403, JsonConvert.SerializeObject(retPayload));
        //    }
        //}

        /// <summary>
        /// 7. Get the email setting list.
        /// </summary>
        /// <param name="CompanyId">The CompanyId in the email table</param>
        /// <param name="token">The administrator identity token</param>
        /// <returns></returns>
        /// <response code="200">Get email setting success</response>
        /// <response code="204">The email data not found</response>
        /// <response code="400">Request does not contain CompanyId</response>
        /// <response code="403">The identity token not found</response>      
        [HttpGet("EventAPI/GetEmailList")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]        
        public IActionResult GetEmailList(int? CompanyId, [FromHeader]string token)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "EventAPI/GetEmailList",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                if (CompanyId == null)
                {
                    var retPayload = new
                    {
                        Response = "Input CompanyId should be an integer."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EventAPI/GetEmailList",
                        ResponseCode = 400,
                        Remark = "Input CompanyId should be an integer."
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                }

                List<EmailSettingTemplate> estList = _adEmail.GetEmailList(CompanyId);

                if (estList == null)
                {

                    var retPayload = new
                    {
                        Response = "Email setting not found"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EventAPI/GetEmailList",
                        ResponseCode = 204,
                        Remark = "Email setting not found."
                    });
                    return StatusCode(204, JsonConvert.SerializeObject(retPayload));
                }
                else
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EventAPI/GetEmailList",
                        ResponseCode = 200,
                        Remark = "Get Email setting List Success."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(estList));
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
                    URL = "EventAPI/GetEmailList",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
               
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 8. Delete email
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="emailFrom">The sender email</param>
        /// <returns></returns>
        /// <response code="202">Delete email success</response>
        
        /// <response code="304">1. Delete email fail 2.Email Not Found</response>
        /// <response code="403">The identity token not found</response>

        [HttpDelete("EventAPI/DeleteEmail")]
        [ProducesResponseType(202)]       
        [ProducesResponseType(304)]
        [ProducesResponseType(403)]        
        public IActionResult DeleteEmail([FromHeader]string token, [FromHeader]string emailFrom)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Delete",
                URL = "EventAPI/DeleteEmail",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if(user!=null)
            {
                if (!_adEmployee.CheckAdmin(user))
                {
                    var retPayload = new
                    {
                        Response = string.Format("{0} do not have enough authorization.", user)
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "Delete",
                        URL = "EventAPI/DeleteEmail",
                        ResponseCode = 403,
                        Remark = string.Format("{0} do not have enough authorization.", user)
                    });

                    return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                }
                else
                {
                    bool? status = _adEmail.Delete(emailFrom);

                    if (status == null)
                    {
                        var retPayload = new
                        {
                            Response = "email not found"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "Delete",
                            URL = "EventAPI/DeleteEmail",
                            ResponseCode = 304,
                            Remark = string.Format("Email {0} not found.", emailFrom)
                        });
                     
                        return StatusCode(304, JsonConvert.SerializeObject(retPayload));
                    }
                    else if (status == false)
                    {
                        var retPayload = new
                        {
                            Response = "Fail"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "Delete",
                            URL = "EventAPI/DeleteEmail",
                            ResponseCode = 304,
                            Remark = string.Format("Delete email {0} setting fail.", emailFrom)
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
                            Method = "Delete",
                            URL = "EventAPI/DeleteEmail",
                            ResponseCode = 202,
                            Remark = string.Format("Delete email {0} setting success.", emailFrom)
                        });

                        
                        return StatusCode(202, JsonConvert.SerializeObject(retPayload));
                    }
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
                    Method = "Delete",
                    URL = "EventAPI/DeleteEmail",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }       
        }

        /// <summary>
        /// 9. Send the email.
        /// P.S. : If the email which is used to send the message has been authenticate by cellphone, Need to set the gmail low safety.
        /// Otherwise the smtp sever will reject log in.
        /// </summary> 
        /// <param name="token">The administrator identity token</param>
        /// <param name="payload">The ema</param>
        /// <returns></returns>
        /// <response code="200">Send email success.</response>
        /// <response code="400">
        ///                      1. Request does not contain deviceName.
        ///                      2. The input email-sending information is null.
        ///                      3. Send email fail.Please check the port number and the ssl and ensure they are correct.
        ///                      4. The device was not found in the database.
        ///                      5. The email was not found or the field enable is false.</response>
        /// <response code="403">The identity token not found</response>
        [HttpPost("EventAPI/SendEmail")]
        [ProducesResponseType(200)]       
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]            
        public IActionResult SendEmail([FromHeader] string token, [FromBody]EmailSendingInfoTemplate payload)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "POST",
                URL = "EventAPI/SendEmail",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                if(payload==null)
                {
                    var retPayload1 = new
                    {
                        Response = "Input payload is null."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "EventAPI/SendEmail",
                        ResponseCode = 400,
                        Remark = "Input payload is null."
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                }

                if (payload.deviceName == null)
                {
                    var retPayload1 = new
                    {
                        Response = "The deviceName is null in the payload."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "EventAPI/SendEmail",
                        ResponseCode = 400,
                        Remark = "The deviceName is null in the payload."
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                }

                int? ownerId = _adEmail.GetOwnerId(payload.deviceName);

                if(ownerId==null)
                {
                    var retPayload2 = new
                    {
                        Response = "Device was not found,so can't get the ownerId."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "EventAPI/SendEmail",
                        ResponseCode = 400,
                        Remark = "Device was not found,so could not get the ownerId."
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload2));
                }

                try
                {                   
                    if(!_adEmail.Send(payload, ownerId))
                    {
                        var retPayload3 = new
                        {
                            Response = "The email setting was not found or the field enable is false."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "EventAPI/SendEmail",
                            ResponseCode = 400,
                            Remark = "The email setting was not found or the field enable is false."
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload3));
                    }
                    
                    var retPayload = new
                    {
                        Response = "Send email success."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "EventAPI/SendEmail",
                        ResponseCode = 200,
                        Remark = "Send email success."
                    });
                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));                    
                }                             
                catch
                {
                    var retPayload4 = new
                    {
                        Response = "Send email fail.There is smtp protocol exception or the recipient address is not a valid address.Please check the port number , SSL,and the recipient address."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "EventAPI/SendEmail",
                        ResponseCode = 400,
                        Remark = "Send email fail.There is smtp protocol exception or the recipient address is not a valid address.Please check the port number , SSL,and the recipient address."
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload4));
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
                    URL = "EventAPI/SendEmail",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 10. Get the email setting(Core Service).
        /// </summary>
        /// <param name="CompanyId">The CompanyId in the email table</param>
        /// <param name="token">The administrator identity token</param>
        /// <returns></returns>
        /// <response code="200">Get email success</response>
        /// <response code="204">The email data not found</response>
        /// <response code="400">Get email fail.</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("EventAPI/SMTP/Setting")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]        
        public IActionResult GetEmail([FromHeader]string CompanyId, [FromHeader]string token)
        {            
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "EventAPI/GetEmail",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                if (!_adEmployee.CheckAdmin(user))
                {
                    var retPayload = new
                    {
                        Response = string.Format("{0} do not have enough authorization.", user)
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "Get",
                        URL = "EventAPI/GetEmail",
                        ResponseCode = 403,
                        Remark = string.Format("{0} do not have enough authorization.", user)
                    });

                    return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                }

                try
                {
                    var email = _adEmail.GetEmail(Int32.Parse(CompanyId));

                    if (email==null)
                    {
                        var retPayload = new
                        {
                            Response = "Email setting was not found."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "EventAPI/GetEmail",
                            ResponseCode = 204,
                            Remark = "Email setting was not found."
                        });
                        return StatusCode(204, JsonConvert.SerializeObject(retPayload));
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EventAPI/GetEmail",
                        ResponseCode = 200,
                        Remark = "Get Email Success."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(email));

                }
                catch(Exception e)//"Get Email fail.There is a exception."
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EventAPI/GetEmail",
                        ResponseCode = 400,
                        Remark = e.Message
                    });

                    return StatusCode(400, JsonConvert.SerializeObject(e.Message));
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
                    URL = "EventAPI/GetEmail",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 11. Get the employee email list.(Core Service).
        /// </summary>
        /// <param name="CompanyId">The CompanyId in the employee table</param>
        /// <param name="token">The administrator identity token</param>
        /// <returns></returns>
        /// <response code="200">Get the employee email list success</response>
        /// <response code="204">The employee email list was not found</response>
        /// <response code="400">Get the employee email list fail.</response>
        /// <response code="403">The identity token not found</response>        
        [HttpGet("EventAPI/GetEmployeeEmailList")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]       
        public IActionResult GetEmployeeEmailList([FromHeader]string CompanyId, [FromHeader]string token)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "EventAPI/GetEmployeeEmailList",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                if (!_adEmployee.CheckAdmin(user))
                {
                    var retPayload = new
                    {
                        Response = string.Format("{0} do not have enough authorization.", user)
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "Get",
                        URL = "EventAPI/GetEmployeeEmailList",
                        ResponseCode = 403,
                        Remark = string.Format("{0} do not have enough authorization.", user)
                    });

                    return StatusCode(403, JsonConvert.SerializeObject(retPayload));
                }

                try
                {
                    List<string> emailList = _adEmail.GetEmployeeEmailList(Int32.Parse(CompanyId));
                    
                    if ( 0 == emailList.Count)
                    {
                        var retPayload = new
                        {
                            Response = "The email could not be found!"
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "Get",
                            URL = "EventAPI/GetEmployeeEmailList",
                            ResponseCode = 204,
                            Remark = "The email of the employees was not found."
                        });
                        return StatusCode(204, JsonConvert.SerializeObject(retPayload));
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EventAPI/GetEmployeeEmailList",
                        ResponseCode = 200,
                        Remark = "Get Email Success."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(emailList));

                }
                catch (Exception e)//"Get Email fail.There is a exception."
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EventAPI/GetEmployeeEmailList",
                        ResponseCode = 400,
                        Remark = e.Message
                    });

                    return StatusCode(400, JsonConvert.SerializeObject("Input CompanyId was not in a correct format."));
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
                    URL = "EventAPI/GetEmployeeEmailList",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 12. Test the Mail Server settings.
        /// </summary>        
        /// <param name="token">The administrator identity token</param>
        /// <param name="emailTestInfo">The information about email test</param>
        /// <returns></returns>
        /// <response code="200">Send test message successfully.</response>
        /// <response code="400">Bad request.</response>  
        /// <response code="403">The identity token not found</response>

        [HttpPost("EventAPI/EmailTest")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        [ProducesResponseType(501)]
        public IActionResult EmailTest([FromHeader]string token, [FromBody]EmailTestTemplate emailTestInfo)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "POST",
                URL = "EventAPI/EmailTest",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {              
                try
                {
                    if(!_adEmail.IsValidEmail(emailTestInfo.EmailTo))
                    {
                        var retPayload = new
                        {
                            Response = "Please enter the right email format."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "EventAPI/EmailTest",
                            ResponseCode = 400,
                            Remark = "Please enter the right email format."
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }

                    if (false == _adEmail.IsValidMailServerData(emailTestInfo))
                    {
                        var retPayload = new
                        {
                            Response = "Mail Server Settings ERROR! Please check the mail server settings."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "EventAPI/EmailTest",
                            ResponseCode = 400,
                            Remark = "Mail Server Settings ERROR! Please check the mail server settings."
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload));
                    }
                    else
                    {
                        _adEmail.Send(user, emailTestInfo);
                        
                        var retPayload3 = new
                        {
                            Response = "Please check your inbox for the test email, close this dialog and then submit mail server settings."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "EventAPI/EmailTest",
                            ResponseCode = 200,
                            Remark = "Please check your inbox for the test email, close this dialog and then submit mail server settings."
                        });
                        return StatusCode(200, JsonConvert.SerializeObject(retPayload3));                        
                    }
                }
                catch 
                {
                    var retPayload4 = new
                    {
                        Response = "Send email failed! Please check the mail server settings and the receiving email address."
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "EventAPI/SendEmail",
                        ResponseCode = 400,
                        Remark = "Send email failed! Please check the mail server settings and the receiving email address."
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload4));
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
                    URL = "EventAPI/SendEmail",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });              
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 12. Test the Mail Server settings.
        /// </summary>        
        /// <param name="token">The administrator identity token</param>
        /// <param name="lineInfo">The information about email test</param>
        /// <returns></returns>
        /// <response code="200">Send test message successfully.</response>
        /// <response code="400">Bad request.</response>  
        /// <response code="403">The identity token not found</response>

        [HttpPost("EventAPI/line/FetchToken")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> FetchToken([FromHeader]string token, [FromBody]LineTemplate lineInfo)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "POST",
                URL = "EventAPI/line/FetchToken",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, 60);
                    client.BaseAddress = new Uri("https://notify-bot.line.me/oauth/token");

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "authorization_code"),
                        new KeyValuePair<string, string>("code", lineInfo.Code),
                        new KeyValuePair<string, string>("redirect_uri", lineInfo.CallBackURL),
                        new KeyValuePair<string, string>("client_id", "Zs50qIfCopQXDVHuqTi9zg"),
                        new KeyValuePair<string, string>("client_secret", "taquMR9MAOqvR473IlSe07PfhIUrXLQMLFJAOz1wCEo")
                    });
                    var response = await client.PostAsync("", content);
                    var data = await response.Content.ReadAsStringAsync();

                    if(JsonConvert.DeserializeObject<JObject>(data)["access_token"].ToString() == "")
                    {
                        var retPayload1 = new
                        {
                            Response = "Token could NOT be found."
                        };

                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "EventAPI/line/FetchToken",
                            ResponseCode = 400,
                            Remark = "Request authentication token error."
                        });
                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }

                    var retPayload = new
                    {
                        Response = JsonConvert.DeserializeObject<JObject>(data)["access_token"].ToString()
                    };
                    //var lineToken = JsonConvert.DeserializeObject<JObject>(data)["access_token"].ToString();
                    //return JsonConvert.DeserializeObject<JObject>(data)["access_token"].ToString();
                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
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
                    URL = "EventAPI/line/FetchToken",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 12. Test the Mail Server settings.
        /// </summary>        
        /// <param name="token">The administrator identity token</param>
        /// <param name="lineInfo">The information about email test</param>
        /// <returns></returns>
        /// <response code="200">Send test message successfully.</response>
        /// <response code="400">Bad request.</response>  
        /// <response code="403">The identity token not found</response>

        [HttpPost("EventAPI/line/SendMessage")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        [ProducesResponseType(501)]
        public async Task<IActionResult> SendLineMessage([FromHeader]string token, [FromBody]LineTemplate lineInfo)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "POST",
                URL = "EventAPI/line/SendMessage",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://notify-api.line.me/api/notify");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + lineInfo.LineToken);
                    //4QxO0koVQyMtdobHCe7oBBWbwtHCmFQatVV03pOzBmc
                    var form = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("message", lineInfo.Message)
                    });

                    await client.PostAsync("", form);
                }

                return new EmptyResult();
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
                    URL = "EventAPI/line/SendMessage",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 3. Get new event count
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get new event data success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("EventAPI/GetNew/Count")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public IActionResult GetNewCount([FromHeader]string token)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "EventAPI/GetNew/Count",
                ResponseCode = 0,
                Remark = ""
            });
            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    long count = _ddb.GetRawDataCount("EventLog", "{\"Checked\":false}");
                    var retPayload = new
                    {
                        Response = count
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EventAPI/GetNew/Count",
                        ResponseCode = 200,
                        Remark = "Get new event count successfully."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
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
                        Method = "GET",
                        URL = "EventAPI/GetNew/Count",
                        ResponseCode = 500,
                        Remark = "Internal Server Error"
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
                    URL = "EventAPI/GetNew/Count",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }
    }
}
