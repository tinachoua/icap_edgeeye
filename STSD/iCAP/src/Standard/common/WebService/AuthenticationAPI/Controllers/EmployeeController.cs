using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ShareLibrary;
using ShareLibrary.AdminDB;
using ShareLibrary.DataTemplate;
using ShareLibrary.Interface;
using Microsoft.AspNetCore.Http;
using System.Threading;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticationAPI.Controllers
{
    public class EmployeeController : Controller
    {
        private IRedisCacheDispatcher _rcd;
        private IEmployee _ade;
        private static Mutex mutex = new Mutex();
        //private IFormCollection _fc;

        public EmployeeController(IRedisCacheDispatcher rcd, IEmployee ade)
        {
            _rcd = rcd;
            _ade = ade;          
        }

        /// <summary>
        /// 5. Create employee
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="payload">The employee data</param>
        /// <returns></returns>
        /// <response code="201">Create employee success</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="406">Employee data error</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("EmployeeAPI/Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(403)]
        [ProducesResponseType(406)]
        [ProducesResponseType(500)]
        public IActionResult Create([FromHeader]string token, [FromBody]EmployeeProfileTemplate payload)
        {
            //AdminDBDispatcher._employee ade = new AdminDBDispatcher._employee();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();

            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "POST",
                URL = "EmployeeAPI/Create",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if(user != null)
            {
                if(payload == null)
                {
                    var retPayload1 = new
                    {
                        Response = "no contain data"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "EmployeeAPI/Create",
                        ResponseCode = 406,
                        Remark = "Request does not contain payload."
                    });
                    return StatusCode(406, JsonConvert.SerializeObject(retPayload1));
                }
                mutex.WaitOne();
                if (_ade.CheckExists(payload.LoginName))
                {
                    var retPayload1 = new
                    {
                        Response = "login name exists"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "EmployeeAPI/Create",
                        ResponseCode = 406,
                        Remark = "Login name already exists."
                    });
                    mutex.ReleaseMutex();
                    return StatusCode(406, JsonConvert.SerializeObject(retPayload1));
                }

                if(CommonFunctions.IsBase64String(payload.PWD) && 
                    CommonFunctions.IsBase64String(payload.VerifyPWD))
                {
                    if(!string.Equals(payload.PWD, payload.VerifyPWD))
                    {
                        var retPayload1 = new
                        {
                            Response = "data error"
                        };
                        
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "EmployeeAPI/Create",
                            ResponseCode = 406,
                            Remark = "Password verify fail."
                        });
                        mutex.ReleaseMutex();
                        return StatusCode(406, JsonConvert.SerializeObject(retPayload1));
                    }
                }
                else
                {
                    var retPayload1 = new
                    {
                        Response = "data error"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "EmployeeAPI/Create",
                        ResponseCode = 406,
                        Remark = "Employee data is error."
                    });
                    mutex.ReleaseMutex();
                    return StatusCode(406, JsonConvert.SerializeObject(retPayload1));
                }
                
                if(!_ade.Create(payload))
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "POST",
                        URL = "EmployeeAPI/Create",
                        ResponseCode = 500,
                        Remark = "Create employee error."
                    });
                    mutex.ReleaseMutex();
                    return StatusCode(500);
                }

                var retPayload = new
                {
                    Response = "Success"
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "POST",
                    URL = "EmployeeAPI/Create",
                    ResponseCode = 201,
                    Remark = string.Format("Add employee {0} success.", payload.Email)
                });
                mutex.ReleaseMutex();
                return StatusCode(201, JsonConvert.SerializeObject(retPayload));
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
                    Name = "",
                    Method = "POST",
                    URL = "EmployeeAPI/Create",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 6. Get employee information
        /// </summary>
        /// <param name="loginName">User login name</param>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get employee profile success</response>
        /// <response code="204">Employee not found</response>
        /// <response code="402">Request does not contain login name</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("EmployeeAPI/Get")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(402)]
        [ProducesResponseType(403)]        
        public IActionResult Get(string loginName,[FromHeader]string token)
        {
           // AdminDBDispatcher._employee ade = new AdminDBDispatcher._employee();
           // RedisCacheDispatcher rcd = new RedisCacheDispatcher();

            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = loginName,
                Method = "GET",
                URL = "EmployeeAPI/Get",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                if (loginName == null)
                {
                    var retPayload1 = new
                    {
                        Response = "no input email"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EmployeeAPI/Get",
                        ResponseCode = 402,
                        Remark = "Request does not contain login name."
                    });
                    return StatusCode(402, JsonConvert.SerializeObject(retPayload1));
                }

                EmployeeProfileTemplate p = _ade.Get(loginName);

                if(p == null)
                {
                    var retPayload1 = new
                    {
                        Response = "employee not found"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EmployeeAPI/Get",
                        ResponseCode = 204,
                        Remark = "Employee not found."
                    });
                    return StatusCode(204, JsonConvert.SerializeObject(retPayload1));
                }

                p.PWD = null;
                
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "EmployeeAPI/Get",
                    ResponseCode = 200,
                    Remark = string.Format("Get employee {0} data success.", p.Email)
                });
                return StatusCode(200, JsonConvert.SerializeObject(p));
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
                    Name = loginName,
                    Method = "GET",
                    URL = "EmployeeAPI/Get",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 7. Get employee information from token
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get employee profile success</response>
        /// <response code="204">Employee not found</response>
        /// <response code="402">Request does not contain login name</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("EmployeeAPI/GetFromToken")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(402)]
        [ProducesResponseType(403)]        
        public IActionResult GetFromToken([FromHeader]string token)
        {
            //AdminDBDispatcher._employee ade = new AdminDBDispatcher._employee();
           // RedisCacheDispatcher rcd = new RedisCacheDispatcher();

            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "EmployeeAPI/GetFromToken",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                string username = user.Replace("\"", "");
                EmployeeProfileTemplate p = _ade.Get(username);

                if (p == null)
                {
                    var retPayload1 = new
                    {
                        Response = "employee not found"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EmployeeAPI/GetFromToken",
                        ResponseCode = 204,
                        Remark = string.Format("Employee {0} not found.", username)
                    });
                    return StatusCode(204, JsonConvert.SerializeObject(retPayload1));
                }

                p.PWD = null;

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "EmployeeAPI/GetFromToken",
                    ResponseCode = 200,
                    Remark = string.Format("Get employee {0} data success.", p.Email)
                });

                return StatusCode(200, JsonConvert.SerializeObject(p));
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
                    Name = "",
                    Method = "GET",
                    URL = "EmployeeAPI/GetFromToken",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 8. Update employee
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="payload">The employee data</param>
        /// <returns></returns>
        /// <response code="200">Update employee profile success</response>        
        /// <response code="403">The identity token not found</response>
        /// <response code="404">no input data</response>
        /// <response code="406">Employee data error</response>
        [HttpPut("EmployeeAPI/Update")]
        [ProducesResponseType(202)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]               
        [ProducesResponseType(406)]
        public IActionResult Update([FromHeader]string token, [FromBody]EmployeeProfileTemplate payload)
        {
            //AdminDBDispatcher._employee ade = new AdminDBDispatcher._employee();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();

            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "PUT",
                URL = "EmployeeAPI/Update",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                if (payload == null)
                {
                    var retPayload1 = new
                    {
                        Response = "no input data"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "EmployeeAPI/Update",
                        ResponseCode = 404,
                        Remark = "Request does not contain payload."
                    });

                    return StatusCode(404, JsonConvert.SerializeObject(retPayload1));
                }

                if(payload.PWD != null)
                {
                    if(CommonFunctions.IsBase64String(payload.PWD) && CommonFunctions.IsBase64String(payload.VerifyPWD))
                    {
                        if(!Equals(payload.PWD, payload.VerifyPWD))
                        {
                            var retPayload1 = new
                            {
                                Response = "Password fail"
                            };
                            
                            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                            {
                                Direction = true,
                                Name = user,
                                Method = "PUT",
                                URL = "EmployeeAPI/Update",
                                ResponseCode = 406,
                                Remark = string.Format("Employee {0} Password verify error.", payload.LoginName)
                            });

                            return StatusCode(406, JsonConvert.SerializeObject(retPayload1));
                        }
                    }
                    else
                    {
                        var retPayload1 = new
                        {
                            Response = "Password fail"
                        };
                        
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "PUT",
                            URL = "EmployeeAPI/Update",
                            ResponseCode = 406,
                            Remark = string.Format("Employee {0} Password format error.", payload.LoginName)
                        });

                        return StatusCode(406, JsonConvert.SerializeObject(retPayload1));
                    }
                }
                

                bool? ret = _ade.Update(payload);

                if (ret == null)
                {
                    var retPayload1 = new
                    {
                        Response = "employee not found"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "EmployeeAPI/Update",
                        ResponseCode = 204,
                        Remark = string.Format("Employee {0} not found.", payload.LoginName)
                    });

                    return StatusCode(204, JsonConvert.SerializeObject(retPayload1));
                }
                else if(ret == false)
                {
                    var retPayload1 = new
                    {
                        Response = "data error"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "EmployeeAPI/Update",
                        ResponseCode = 406,
                        Remark = string.Format("Employee {0} data is error.", payload.LoginName)
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
                    URL = "EmployeeAPI/Update",
                    ResponseCode = 202,
                    Remark = string.Format("Update employee {0} data success.", payload.LoginName)
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
                    Name = "",
                    Method = "PUT",
                    URL = "EmployeeAPI/Update",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 09. Delete employee
        /// </summary>
        /// <param name="loginName">User login name</param>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="202">Delete employee success</response>
        /// <response code="304">Employee not found. Not modified.</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="404">Request does not contain payload.</response>
        /// <response code="406">Employee data error</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("EmployeeAPI/Delete")]
        [ProducesResponseType(202)]
        [ProducesResponseType(304)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(406)]
        [ProducesResponseType(500)]
        public IActionResult Delete([FromHeader]string loginName, [FromHeader]string token)
        {
            //AdminDBDispatcher._employee ade = new AdminDBDispatcher._employee();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();

            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "DELETE",
                URL = "EmployeeAPI/Delete",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                if (loginName == null)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = "EmployeeAPI/Delete",
                        ResponseCode = 404,
                        Remark = "Request does not contain payload."
                    });
                    return StatusCode(404);
                }

                if(loginName == "admin")
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = "EmployeeAPI/Delete",
                        ResponseCode = 406,
                        Remark = "Administrator cannot be delete."
                    });
                    return StatusCode(406);
                }

                bool? p = _ade.Delete(loginName);

                if (p == null)
                {
                    var retPayload1 = new
                    {
                        Response = "employee not found"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = "EmployeeAPI/Delete",
                        ResponseCode = 304,
                        Remark = string.Format("Employee {0} not found.", loginName)
                    });
                    return StatusCode(304, JsonConvert.SerializeObject(retPayload1));
                }
                else if(p == false)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "DELETE",
                        URL = "EmployeeAPI/Delete",
                        ResponseCode = 500,
                        Remark = string.Format("Error on delete, login name: {0}.", loginName)
                    });
                    return StatusCode(500);
                }

                var retPayload = new
                {
                    Response = "Deleted"
                };

                _rcd.KeyDeleteByValue(0, loginName);

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "DELETE",
                    URL = "EmployeeAPI/Delete",
                    ResponseCode = 202,
                    Remark = string.Format("Delete employee {0} data success.", loginName)
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
                    Name = "",
                    Method = "DELETE",
                    URL = "EmployeeAPI/Delete",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 10. Get employee list
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get employee list success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("EmployeeAPI/List")]
        [ProducesResponseType(202)]
        [ProducesResponseType(403)]
        public IActionResult GetList([FromHeader]string token)
        {
            //AdminDBDispatcher._employee ade = new AdminDBDispatcher._employee();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();

            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "EmployeeAPI/List",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                var retPayload1 = new
                {
                    EmployeeList = _ade.GetList()
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "EmployeeAPI/List",
                    ResponseCode = 200,
                    Remark = "Get employee list success."
                });

                return StatusCode(200, JsonConvert.SerializeObject(retPayload1));
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
                    Name = "",
                    Method = "GET",
                    URL = "EmployeeAPI/List",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 12. Get image
        /// </summary>      
        /// <param name="token">token</param>
        /// <param name="empId">Employee Id</param>
        /// <returns></returns>
        [HttpGet("EmployeeAPI/GetImg")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]

        public IActionResult GetImg([FromHeader]string token, [FromHeader]string empId)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "EmployeeAPI/GetImg",
                ResponseCode = 0,
                Remark = ""
            });

            RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    string file = _ade.GetImgBase64(empId,"employees");

                    var retPayload = new
                    {
                        Response = file
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EmployeeAPI/GetImg",
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
                        URL = "EmployeeAPI/GetImg",
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
                    URL = "EmployeeAPI/GetImg",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 13. Upload image
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="empId">Employee Id</param>
        /// <param name="overwrite">When the file exists,whether overwrite.</param>
        /// <param name="files">Upload Images</param>
        /// <returns></returns>

        [HttpPost("EmployeeAPI/UploadImg")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult UploadImg([FromHeader]string token, [FromHeader]string empId, [FromHeader]string overwrite, [FromForm]List<IFormFile> files)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "POST",
                URL = "EmployeeAPI/UploadImg",
                ResponseCode = 0,
                Remark = ""
            });

            RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {                    
                    if (!_ade.AllowedFileExtensions(files))
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "POST",
                            URL = "EmployeeAPI/UploadImg",
                            ResponseCode = 400,
                            Remark = "The File Extensions is unacceptable."
                        });

                        return StatusCode(400, JsonConvert.SerializeObject("The File Extensions is unacceptable."));
                    }

                    if (_ade.UploadImg(files, !(Int32.Parse(overwrite)).Equals(0), empId, "employees"))
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
                            URL = "EmployeeAPI/UploadImg",
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
                            URL = "EmployeeAPI/UploadImg",
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
                        URL = "EmployeeAPI/UploadImg",
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
                    URL = "EmployeeAPI/UploadImg",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 5. Create employee
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="mapId">The employee data</param>
        /// <returns></returns>
        /// <response code="201">Create employee success</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="406">Employee data error</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("EmployeeAPI/SetCommonMap")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult SetCommonMap([FromHeader]string token, [FromBody]int mapId)
        {
            //AdminDBDispatcher._employee ade = new AdminDBDispatcher._employee();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();

            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "PUT",
                URL = "EmployeeAPI/SetCommonMap",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    _ade.SetCommonMap(user, mapId);

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "PUT",
                        URL = "EmployeeAPI/SetCommonMap",
                        ResponseCode = 200,
                        Remark = string.Format("Set the common map successfully!")
                    });

                    return StatusCode(200);
                }
                catch (DbUpdateException exc)
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
                        URL = "EmployeeAPI/SetCommonMap",
                        ResponseCode = 400,
                        Remark = exc.Message
                    });
                    return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
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
                        Method = "PUT",
                        URL = "EmployeeAPI/SetCommonMap",
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
                    Name = "",
                    Method = "PUT",
                    URL = "EmployeeAPI/SetCommonMap",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 6. Get employees' name with email
        /// 
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="devName">The device name</param>
        /// <returns></returns>
        /// <response code="200">Get widget list successfully.</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("EmployeeAPI/Name")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetEmployeeName([FromHeader]string token)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = "EmployeeAPI/Name",
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
                    };

                    if (retPayload.emp.Length == 0)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "EmployeeAPI/Name",
                            ResponseCode = 204,
                            Remark = "employees' name not found"
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EmployeeAPI/Name",
                        ResponseCode = 200,
                        Remark = "Get employees' name successfully."
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
                        URL = "EmployeeAPI/Name",
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
                    URL = "EmployeeAPI/Name",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 6. Get employees' name with email
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="searchString">The device name</param>
        /// <returns></returns>
        /// <response code="200">Get widget list successfully.</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("EmployeeAPI/FindEmail")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult FindEmployeeEmail([FromHeader]string token, string searchString)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = "EmployeeAPI/FindEmail",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var retPayload = _ade.FindAllEmail(searchString);

                    if (retPayload.Length == 0)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "EmployeeAPI/FindEmail",
                            ResponseCode = 204,
                            Remark = "Didn't find anything."
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EmployeeAPI/FindEmail",
                        ResponseCode = 200,
                        Remark = "Search successfully."
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
                        URL = "EmployeeAPI/FindEmail",
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
                    URL = "EmployeeAPI/FindEmail",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }


        /// <summary>
        /// 7. Get email by permission
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="permissionId">The permission id</param>
        /// <returns></returns>
        /// <response code="200">Get widget list successfully.</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("EmployeeAPI/PermissionGroup/TooltipContent")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetEmailByPermissionId([FromHeader]string token, int permissionId)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = "EmployeeAPI/PermissionGroup/TooltipContent",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var retPayload = _ade.GetEmailForTooltip(permissionId);

                    if (retPayload.Length == 0)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "EmployeeAPI/PermissionGroup/TooltipContent",
                            ResponseCode = 204,
                            Remark = "Didn't find anything."
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EmployeeAPI/PermissionGroup/TooltipContent",
                        ResponseCode = 200,
                        Remark = "Search successfully."
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
                        URL = "EmployeeAPI/PermissionGroup/TooltipContent",
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
                    URL = "EmployeeAPI/PermissionGroup/TooltipContent",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 7. Get employee name and employeeId by email
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="email">The permission id</param>
        /// <returns></returns>
        /// <response code="200">Get widget list successfully.</response>
        /// <response code="204">Not Found</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("EmployeeAPI/Search/ByEmail")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult SearchByEmail([FromHeader]string token, string email)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = "EmployeeAPI/Search/ByEmail",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    var retPayload = _ade.GetEmployeeInfo(email);

                    if (retPayload ==null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "EmployeeAPI/Search/ByEmail",
                            ResponseCode = 204,
                            Remark = "Didn't find anything."
                        });
                        return StatusCode(204);
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "EmployeeAPI/Search/ByEmail",
                        ResponseCode = 200,
                        Remark = "Search successfully."
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
                        URL = "EmployeeAPI/Search/ByEmail",
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
                    URL = "EmployeeAPI/Search/ByEmail",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }
    }
}
