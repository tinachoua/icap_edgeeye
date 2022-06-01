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
    public class InnerAuthController : Controller
    {
        private IEmployee _ade;
        public InnerAuthController(IRedisCacheDispatcher rcd, IEmployee ade, ILicenselist adl)
        {
            _ade = ade;
        }

        /// <summary>
        /// 1. Coreservice authentication
        /// </summary>
        /// <param name="Username">The name of the user</param>
        /// <param name="Password">Login password</param>
        /// <returns>Identity token for another APIs authentication</returns>
        /// <response code="200">Authentication success</response>
        /// <response code="403">User is not exists or wrong password</response>
        [HttpGet("InnerAuthAPI/Login")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult Login([FromHeader]string Username, [FromHeader]string Password)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = Username,
                Method = "GET",
                URL = "InnerAuthAPI/Login",
                ResponseCode = 0,
                Remark = ""
            });

            if (Username == null || Password == null)
            {
                var retPayload1 = new
                {
                    Response = ReturnCode.HeaderParmNull
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = Username,
                    Method = "GET",
                    URL = "InnerAuthAPI/Login",
                    ResponseCode = 400,
                    Remark = string.Format("Username or password is empty.")
                });
                return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
            }

            if (Username != "2EKxXe)a")
            {
                var retPayload1 = new
                {
                    Response = ReturnCode.AccountError
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = Username,
                    Method = "GET",
                    URL = "InnerAuthAPI/Login",
                    ResponseCode = 403,
                    Remark = "Account error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
            }

            if (Password != "^fs1o}z}*b#-qfKj")
            {
                var retPayload1 = new
                {
                    Response = ReturnCode.PWDError
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = Username,
                    Method = "GET",
                    URL = "InnerAuthAPI/Login",
                    ResponseCode = 403,
                    Remark = "PWD Error"
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload1));
            }

            try
            {
                var retPayload = new
                {
                    Token = _ade.GetToken(Username)
                };
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = Username,
                    Method = "GET",
                    URL = "InnerAuthAPI/Login",
                    ResponseCode = 200,
                    Remark = "Core service get token successfully."
                });

                return StatusCode(200, JsonConvert.SerializeObject(retPayload));
            }
            catch (Exception exc)
            {
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = Username,
                    Method = "GET",
                    URL = "InnerAuthAPI/Login",
                    ResponseCode = 500,
                    Remark = exc.Message
                });
                return StatusCode(500);
            }
        }
    }
}