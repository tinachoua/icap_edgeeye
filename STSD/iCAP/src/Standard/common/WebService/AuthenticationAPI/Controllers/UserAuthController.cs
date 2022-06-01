using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Security.Claims;
using ShareLibrary;
using ShareLibrary.AdminDB;
using ShareLibrary.DataTemplate;
using ShareLibrary.Interface;

namespace AuthenticationAPI.Controllers
{
    /// <summary>
    /// The user authentication APIs
    /// </summary>
    public class UserAuthController : Controller
    {
        static List<KeyValuePair<string, string>> AdminTokenList = new List<KeyValuePair<string, string>>();
        static List<KeyValuePair<string, string>> UserTokenList = new List<KeyValuePair<string, string>>();

        private IRedisCacheDispatcher _rcd;
        private ILicenselist _adl;
        private IEmployee _ade;

        public UserAuthController(IRedisCacheDispatcher rcd, IEmployee ade,ILicenselist adl )
        {
            _rcd = rcd; 
            _ade = ade; 
            _adl = adl;                   
        }

        /// <summary>
        /// 2. Device license authentication
        /// </summary>
        /// <param name="token">The administrator identity token</param>
        /// <param name="key">The license to authorize number of devices</param>
        /// <returns></returns>
        /// <response code="202">The license has been accepted</response>
        /// <response code="403">The identity token could not be found</response>
        /// <response code="406">The license is not acceptable</response>
        [HttpPatch("AuthenticationAPI/UploadLicense")]
        [ProducesResponseType(202)]
        [ProducesResponseType(403)]
        [ProducesResponseType(406)]
        public IActionResult UploadLicense([FromHeader]string token, [FromHeader]string key)
        {
            //icapContext ic = new icapContext();
            //AdminDBDispatcher._licenselist adl = new AdminDBDispatcher._licenselist();

            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "PATCH",
                URL = "AuthenticationAPI/UploadLicense",
                ResponseCode = 0,
                Remark = ""
            });

            KeyValuePair<string, string> admin = AdminTokenList.Where(x => x.Value == token).SingleOrDefault();

            if (!admin.Equals(new KeyValuePair<string, string>()))
            {
                if (key == null)
                {
                    var retPayload1 = new
                    {
                        Response = "license error"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = "",
                        Method = "PATCH",
                        URL = "AuthenticationAPI/UploadLicense",
                        ResponseCode = 406,
                        Remark = "Request license is empty."
                    });

                    return StatusCode(406, JsonConvert.SerializeObject(retPayload1));
                }

                if (!CommonFunctions.IsBase64String(key))
                {
                    var retPayload1 = new
                    {
                        Response = "license error"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = "",
                        Method = "PATCH",
                        URL = "AuthenticationAPI/UploadLicense",
                        ResponseCode = 406,
                        Remark = "Request license error."
                    });

                    return StatusCode(406, JsonConvert.SerializeObject(retPayload1));
                }

                string keyString1 = Encoding.UTF8.GetString(Convert.FromBase64String(key));

                if (!CommonFunctions.IsBase64String(keyString1))
                {
                    var retPayload1 = new
                    {
                        Response = "license error"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = "",
                        Method = "PATCH",
                        URL = "AuthenticationAPI/UploadLicense",
                        ResponseCode = 406,
                        Remark = "Request license error."
                    });

                    return StatusCode(406, JsonConvert.SerializeObject(retPayload1));
                }

                string keyString2 = Encoding.UTF8.GetString(Convert.FromBase64String(keyString1));
                string[] keySplit = keyString2.Split(',');

                if(keySplit.Count() != 4)
                {
                    var retPayload1 = new
                    {
                        Response = "license error"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = "",
                        Method = "PATCH",
                        URL = "AuthenticationAPI/UploadLicense",
                        ResponseCode = 406,
                        Remark = "Request license error."
                    });

                    return StatusCode(406, JsonConvert.SerializeObject(retPayload1));
                }

                int addDeviceCount = 0;

                if(!int.TryParse(keySplit[1], out addDeviceCount))
                {
                    var retPayload1 = new
                    {
                        Response = "license error"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = "",
                        Method = "PATCH",
                        URL = "AuthenticationAPI/UploadLicense",
                        ResponseCode = 406,
                        Remark = "Request license error."
                    });

                    return StatusCode(406, JsonConvert.SerializeObject(retPayload1));
                }

                DateTime dt;

                if(!DateTime.TryParse(keySplit[3], out dt))
                {
                    var retPayload1 = new
                    {
                        Response = "license error"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = "",
                        Method = "PATCH",
                        URL = "AuthenticationAPI/UploadLicense",
                        ResponseCode = 406,
                        Remark = "Request license error."
                    });

                    return StatusCode(406, JsonConvert.SerializeObject(retPayload1));
                }

                LicenseList lic = new LicenseList()
                {
                    DeviceCount = addDeviceCount,
                    Key = key,
                    CreatedDate = dt
                };

                //ic.LicenseList.Add(lic);
                //ic.SaveChanges();
                _adl.Create(lic);

                /*List<LicenseList> p = ic.LicenseList.Where(x => x.Id > 0).ToList();

                int TotalDeviceCount = 0;

                foreach (LicenseList l in p)
                {
                    TotalDeviceCount += l.DeviceCount;
                }*/

                int TotalDeviceCount = _adl.DeviceCount();

                var retPayload = new
                {
                    Response = "Success",
                    NumOfDev = TotalDeviceCount
                };
                
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "PATCH",
                    URL = "AuthenticationAPI/UploadLicense",
                    ResponseCode = 202,
                    Remark = string.Format("Add linense success, total device count in final: {0}.", TotalDeviceCount)
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
                    Method = "PATCH",
                    URL = "AuthenticationAPI/UploadLicense",
                    ResponseCode = 403,
                    Remark = "License exists."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 3. Employee authentication
        /// </summary>
        /// <param name="Username">The name of the user</param>
        /// <param name="Password">Login password</param>
        /// <returns>Identity token for another APIs authentication</returns>
        /// <response code="200">Authentication success</response>
        /// <response code="403">User is not exists or wrong password</response>
        [HttpGet("AuthenticationAPI/Login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public IActionResult Login([FromHeader]string Username, [FromHeader]string Password)
        {
           // AdminDBDispatcher._employee ade = new AdminDBDispatcher._employee();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();

            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = Username,
                Method = "GET",
                URL = "AuthenticationAPI/Login",
                ResponseCode = 0,
                Remark = ""
            });

            EmployeeProfileTemplate emp = _ade.Get(Username);

            if (emp != null)
            {
                if (Password == null)
                {
                    var retPayload1 = new
                    {
                        error = "Invalid password"
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = Username,
                        Method = "GET",
                        URL = "AuthenticationAPI/Login",
                        ResponseCode = 403,
                        Remark = string.Format("Employee {0} response password is empty.", Username)
                    });

                    return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                }

                if (!_ade.CheckPWD(Username, Password))
                {
                    var retPayload1 = new
                    {
                        error = "Invalid password"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = Username,
                        Method = "GET",
                        URL = "AuthenticationAPI/Login",
                        ResponseCode = 403,
                        Remark = string.Format("Employee {0} response invalid password.", Username)
                    });

                    return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
                }

                KeyValuePair<string, string> admin;

                if (UserTokenList.Count == 0)
                {
                    admin = new KeyValuePair<string, string>();
                }
                else
                {
                    try
                    {
                        admin = UserTokenList.Where(x => x.Key == Username).First();
                    }
                    catch (Exception)
                    {
                        admin = new KeyValuePair<string, string>();
                    }
                }
                if (admin.Equals(new KeyValuePair<string, string>()))
                {
                    admin = new KeyValuePair<string, string>(Username, Convert.ToBase64String(Encoding.UTF8.GetBytes(CommonFunctions.CreateRandomPassword(16))));
                    UserTokenList.Add(admin);
                }

                var retPayload = new
                {
                    Token = admin.Value
                };
                
                _rcd.SetCache(0, admin.Value, admin.Key);
                
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = Username,
                    Method = "GET",
                    URL = "AuthenticationAPI/Login",
                    ResponseCode = 200,
                    Remark = string.Format("Employee {0} login success.", admin.Key)
                });

                return StatusCode(200, JsonConvert.SerializeObject(retPayload));
            }
            else
            {
                var retPayload = new
                {
                    error = "Invalid username"
                };
                
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = Username,
                    Method = "GET",
                    URL = "AuthenticationAPI/Login",
                    ResponseCode = 403,
                    Remark = string.Format("Employee {0} not found.", Username)
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 3. Check token
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get response success</response>
        [HttpGet("AuthenticationAPI/TokenChecker")]
        [ProducesResponseType(200)]
        public IActionResult TokenChecker([FromHeader] string token)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "AuthenticationAPI/TokenChecker",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                var retPayload = new
                {
                    Admin = _ade.CheckAdmin(user)
                };
                
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "AuthenticationAPI/TokenChecker",
                    ResponseCode = 200,
                    Remark = "User login successful."
                });

                return StatusCode(200, JsonConvert.SerializeObject(retPayload));
            }
            else
            {
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "GET",
                    URL = "AuthenticationAPI/TokenChecker",
                    ResponseCode = 204,
                    Remark = "user not found"
                });

                return StatusCode(204);
            }
        }

        /// <summary>
        /// 5. Check administrator
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get response success</response>
        [HttpGet("AuthenticationAPI/CheckAdmin")]
        [ProducesResponseType(200)]
        public IActionResult CheckAdmin([FromHeader]string token)
        {
            //AdminDBDispatcher._employee ade = new AdminDBDispatcher._employee();
            //RedisCacheDispatcher rcd = new RedisCacheDispatcher();

            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "GET",
                URL = "AuthenticationAPI/CheckAdmin",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                string username = user.Replace("\"", "");
                if (_ade.CheckAdmin(username))
                {
                    var retPayload = new
                    {
                        Response = "true"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "AuthenticationAPI/CheckAdmin",
                        ResponseCode = 200,
                        Remark = string.Format("{0} is administrator.", username)
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
                }
                else
                {
                    var retPayload = new
                    {
                        Response = "false"
                    };
                    
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "AuthenticationAPI/CheckAdmin",
                        ResponseCode = 200,
                        Remark = string.Format("{0} is not administrator.", username)
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(retPayload));
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
                    Name = "",
                    Method = "GET",
                    URL = "AuthenticationAPI/CheckAdmin",
                    ResponseCode = 200,
                    Remark = "Request authentication token error."
                });

                return StatusCode(200, JsonConvert.SerializeObject(retPayload));
            }
        }
    }
}
