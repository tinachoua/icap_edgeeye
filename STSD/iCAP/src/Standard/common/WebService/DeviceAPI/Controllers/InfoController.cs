using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using ShareLibrary;
using ShareLibrary.DataTemplate;
using ShareLibrary.AdminDB;
using ShareLibrary.Interface;
using MongoDB.Driver;
using System.Reflection;
using System.Text.RegularExpressions;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DeviceAPI.Controllers
{    
    /// <summary>
    /// 
    /// </summary>
    public class InfoController : Controller
    {
        private IRedisCacheDispatcher _rcd;
        private IDevice _add;
        private IDataDBDispatcher _ddb;
        private IWidgetRepository _adw;
        private IEmployee _ade;
        private IBranch _adb;

        public InfoController(IRedisCacheDispatcher rcd, IDevice add, IDataDBDispatcher ddb, IWidgetRepository adw, IEmployee ade, IBranch adb)
        {
            _rcd = rcd;
            _add = add;
            _ddb = ddb;
            _adw = adw;
            _ade = ade;
            _adb = adb;
        }

        /// <summary>
        /// 18. Get device overview
        /// </summary>
        /// <param name="DeviceName">The device identity</param>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get deivce overview success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("DeviceInfoAPI/GetOverview")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public IActionResult GetOverview(string DeviceName, [FromHeader]string token)
        {
            Int64 storageSize = 0;
            List<StorInfo> storInfolist = new List<StorInfo>();
            List<object> ooblist = new List<object>();

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    BsonDocument bdoc = _ddb.GetLastRawData(string.Format("{0}-static", DeviceName));
                    BsonDocument bdoc2 = _ddb.GetLastRawData(string.Format("{0}-dynamic", DeviceName));
                    Regex rgx = new Regex(@"^B");

                    foreach (BsonDocument bd in bdoc["Storage"].AsBsonArray)
                    {
                        var cap = Convert.ToInt32(CommonFunctions.GetData(bd["Par"].AsBsonDocument["TotalCap"])) / 1024 / 1024;
                        storageSize += cap;
                        storInfolist.Add(new StorInfo() {
                            SN = bd["SN"].AsString,
                        });

                        if (rgx.IsMatch(bd["FWVer"].AsString))
                        {
                            List<object> parlist = new List<object>(); 
                            foreach (BsonDocument par in bd["Par"]["ParInfo"].AsBsonArray) {
                                parlist.Add(new {
                                    MountAt = par["MountAt"].AsString,
                                    Capacity = Convert.ToInt32(CommonFunctions.GetData(par["Capacity"])) / 1024 / 1024
                                });
                            }
                            ooblist.Add(new
                            {
                                SN = bd["SN"].AsString,
                                Cap = cap,
                                Model = bd["Model"].AsString,
                                ParInfo = parlist
                            });
                        }
                    }

                    int index = 0;

                    #region Lifespan

                    List<object> lifespan = new List<object>();
                    foreach (StorInfo s in storInfolist)
                    {
                        double health = 0;
                        int PECycle = 0;
                        BsonDocument bdoc3 = _ddb.GetAnalyzerData(s.SN);
                        int count = 0;
                        if (bdoc3 != null)
                        {
                            count = bdoc3["Lifespan"].AsBsonArray.Count;
                            PECycle = Convert.ToInt32(CommonFunctions.GetData(bdoc3["PECycle"]));
                        }

                        object[] obj = new object[count];
                        for (int i = 0; i < count; i++)
                        {
                            obj[i] = new
                            {
                                Date = (UInt64)Convert.ToDouble(CommonFunctions.GetData(bdoc3["Lifespan"].AsBsonArray[i].AsBsonDocument["time"])),
                                EstimatedDays = Convert.ToInt32(CommonFunctions.GetData(bdoc3["Lifespan"].AsBsonArray[i].AsBsonDocument["data"]))
                            };
                            health = Convert.ToDouble(CommonFunctions.GetData(bdoc3["Lifespan"].AsBsonArray[i].AsBsonDocument["health"]));
                        }

                        if (health == 0 && bdoc3 != null)
                        {
                            health = Convert.ToDouble(CommonFunctions.GetData(bdoc3["InitHealth"]));
                        }

                        health = Math.Round(health, 2);

                        var life = new
                        {
                            SN = s.SN,
                            Lifespan = obj
                        };

                        lifespan.Add(life);
                        index++;
                    }

                    #endregion

                    int status = 0;
                    string devStatus = _rcd.GetStatus(DeviceName);
                    if (devStatus != null)
                    {
                        int.TryParse(devStatus, out status);
                    }

                    Device dev = _add.Get(DeviceName);

                    if (dev == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DeviceInfoAPI/GetOverview",
                            ResponseCode = 400,
                            Remark = "Device Name Error."
                        });

                        var retPayload1 = new
                        {
                            Response = "Device Name Error"
                        };

                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }

                    bool EnableiCover = false;

                    foreach (BsonElement b in bdoc["Remote"].AsBsonDocument)
                    {
                        if (string.Equals(b.Value["Name"].AsString, "iCoverX"))
                        {
                            EnableiCover = true;
                            break;
                        }
                    }

                    string gpuStr = null;
                    try
                    {
                        gpuStr = bdoc["GPU"].AsBsonDocument.ToJson();
                    }
                    catch (Exception)
                    {
                        gpuStr = null;
                    }

                    var info = new
                    {
                        DevName = DeviceName,
                        Alias = dev.Alias,
                        OS = bdoc["Sys"].AsBsonDocument["OS"].AsString,
                        CPU = bdoc["CPU"].AsBsonDocument["Name"].AsString,
                        MEMCap = Convert.ToInt32(CommonFunctions.GetData(bdoc["MEM"].AsBsonDocument["Cap"])) / 1024 / 1024,
                        StorCap = storageSize,
                        Status = (status == 1) ? "Online" : "Offline",
                        ImgSrc = dev.PhotoUrl,
                        iCover = EnableiCover,
                        Lifespan = lifespan.ToArray(),
                        BranchName = _adb.GetBranchName(DeviceName),
                        GPU = gpuStr,
                        OOBlist = ooblist
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DeviceInfoAPI/GetOverview",
                        ResponseCode = 200,
                        Remark = "Get device overview information success."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(info));
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
                        URL = "DeviceInfo/GetOverview",
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
                    URL = "DeviceInfo/GetOverview",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 18. Get device overview
        /// </summary>
        /// <param name="DeviceName">The device identity</param>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get deivce overview success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("DeviceInfoAPI/iAnalyzer")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public IActionResult GetiAnalyzer(string DeviceName, [FromHeader]string token)
        {
            Int64 storageSize = 0;
            List<StorInfo> storInfolist = new List<StorInfo>();

            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    BsonDocument bdoc = _ddb.GetLastRawData(string.Format("{0}-static", DeviceName));
                    BsonDocument bdoc2 = _ddb.GetLastRawData(string.Format("{0}-dynamic", DeviceName));
                    foreach (BsonDocument bd in bdoc["Storage"].AsBsonArray)
                    {
                        storageSize += Convert.ToInt32(CommonFunctions.GetData(bd["Par"].AsBsonDocument["TotalCap"])) / 1024 / 1024;
                        storInfolist.Add(new StorInfo()
                        {
                            SN = bd["SN"].AsString,
                            Cap = Convert.ToInt32(CommonFunctions.GetData(bd["Par"].AsBsonDocument["TotalCap"])) / 1024 / 1024,
                            Model = bd["Model"].AsString,
                        });
                    }

                    List<object> analyzer = new List<object>();
                    List<StorCount> avgCount = new List<StorCount>();

                    int index = 0;
                    foreach (BsonDocument bd in bdoc2["Storage"].AsBsonArray)
                    {
                        int enable = Convert.ToInt32(CommonFunctions.GetData(bd["iAnalyzer"].AsBsonDocument["Enable"]));
                        if (enable != 0)
                        {
                            int subIndex = 0;
                            ChartDataTemplate SR, SW, RR, RW;
                            if (enable == 1)
                            {
                                SR = new ChartDataTemplate()
                                {
                                    label = new string[6],
                                    value = new int[6],
                                };
                                SW = new ChartDataTemplate()
                                {
                                    label = new string[6],
                                    value = new int[6]
                                };
                                RR = new ChartDataTemplate()
                                {
                                    label = new string[5],
                                    value = new int[5]
                                };
                                RW = new ChartDataTemplate()
                                {
                                    label = new string[5],
                                    value = new int[5]
                                };
                            }
                            else
                            {
                                SR = new ChartDataTemplate()
                                {
                                    label = new string[7],
                                    value = new int[7],
                                };
                                SW = new ChartDataTemplate()
                                {
                                    label = new string[7],
                                    value = new int[7]
                                };
                                RR = new ChartDataTemplate()
                                {
                                    label = new string[7],
                                    value = new int[7]
                                };
                                RW = new ChartDataTemplate()
                                {
                                    label = new string[7],
                                    value = new int[7]
                                };
                            }

                            #region SR
                            subIndex = 0;
                            foreach (BsonElement bdd in bd["iAnalyzer"].AsBsonDocument["SR"].AsBsonDocument)
                            {
                                SR.label[subIndex] = CommonFunctions.GetBehivorString(enable, subIndex, true);
                                SR.value[subIndex] = bdd.Value.AsInt32;
                                subIndex++;
                            }
                            #endregion

                            #region SW
                            subIndex = 0;
                            foreach (BsonElement bdd in bd["iAnalyzer"].AsBsonDocument["SW"].AsBsonDocument)
                            {
                                SW.label[subIndex] = CommonFunctions.GetBehivorString(enable, subIndex, true);
                                SW.value[subIndex] = bdd.Value.AsInt32;
                                subIndex++;
                            }
                            #endregion

                            #region RR
                            subIndex = 0;
                            foreach (BsonElement bdd in bd["iAnalyzer"].AsBsonDocument["RR"].AsBsonDocument)
                            {
                                RR.label[subIndex] = CommonFunctions.GetBehivorString(enable, subIndex, false);
                                RR.value[subIndex] = bdd.Value.AsInt32;
                                subIndex++;
                            }
                            #endregion

                            #region RW
                            subIndex = 0;
                            foreach (BsonElement bdd in bd["iAnalyzer"].AsBsonDocument["RW"].AsBsonDocument)
                            {
                                RW.label[subIndex] = CommonFunctions.GetBehivorString(enable, subIndex, false);
                                RW.value[subIndex] = bdd.Value.AsInt32;
                                subIndex++;
                            }
                            #endregion

                            var storrw = new
                            {
                                SN = bd["SN"].AsString,
                                SRC = Convert.ToInt32(CommonFunctions.GetData(bd["iAnalyzer"].AsBsonDocument["SRC"])),
                                RRC = Convert.ToInt32(CommonFunctions.GetData(bd["iAnalyzer"].AsBsonDocument["RRC"])),
                                SWC = Convert.ToInt32(CommonFunctions.GetData(bd["iAnalyzer"].AsBsonDocument["SWC"])),
                                RWC = Convert.ToInt32(CommonFunctions.GetData(bd["iAnalyzer"].AsBsonDocument["RWC"])),
                                SR = SR,
                                SW = SW,
                                RR = RR,
                                RW = RW
                            };

                            try
                            {

                                avgCount.Add(new StorCount()
                                {
                                    SN = bd["SN"].AsString,
                                    Count = Convert.ToInt32(CommonFunctions.GetData(bd["smart"].AsBsonDocument["167"]))
                                });

                            }
                            catch (Exception)
                            {
                                avgCount.Add(new StorCount
                                {
                                    SN = bd["SN"].AsString,
                                    Count = 0
                                });
                            }

                            analyzer.Add(storrw);
                        }
                        else
                        {
                            avgCount.Add(new StorCount
                            {
                                SN = bd["SN"].AsString,
                                Count = 0
                            });
                            analyzer.Add(new object());
                        }
                        index++;
                    }
                    #region Lifespan

                    List<object> lifespan = new List<object>();

                    index = 0;
                    foreach (StorInfo s in storInfolist)
                    {
                        double health = 0;
                        int PECycle = 0;
                        BsonDocument bdoc3 = _ddb.GetAnalyzerData(s.SN);
                        int count = 0;
                        if (bdoc3 != null)
                        {
                            count = bdoc3["Lifespan"].AsBsonArray.Count;
                            PECycle = Convert.ToInt32(CommonFunctions.GetData(bdoc3["PECycle"]));
                        }

                        object[] obj = new object[count];
                        for (int i = 0; i < count; i++)
                        {
                            obj[i] = new
                            {
                                Date = (UInt64)Convert.ToDouble(CommonFunctions.GetData(bdoc3["Lifespan"].AsBsonArray[i].AsBsonDocument["time"])),
                                EstimatedDays = Convert.ToInt32(CommonFunctions.GetData(bdoc3["Lifespan"].AsBsonArray[i].AsBsonDocument["data"]))
                            };
                            health = Convert.ToDouble(CommonFunctions.GetData(bdoc3["Lifespan"].AsBsonArray[i].AsBsonDocument["health"]));
                        }

                        if (health == 0 && bdoc3 != null)
                        {
                            health = Convert.ToDouble(CommonFunctions.GetData(bdoc3["InitHealth"]));
                        }

                        health = Math.Round(health, 2);
                        var storIndex = avgCount.FindIndex(a => a.SN == s.SN);

                        var life = new
                        {
                            SN = s.SN,
                            Health = health,
                            AvgEC = avgCount[storIndex],
                            PECycle,
                            Lifespan = obj
                        };

                        lifespan.Add(life);
                        index++;
                    }

                    #endregion

                    Device dev = _add.Get(DeviceName);

                    if (dev == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DeviceInfoAPI/GetOverview",
                            ResponseCode = 400,
                            Remark = "Device Name Error."
                        });

                        var retPayload1 = new
                        {
                            Response = "Device Name Error"
                        };

                        return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                    }

                    var info = new
                    {
                        StorCap = storageSize,
                        Analyzer = analyzer.ToArray(),
                        Lifespan = lifespan.ToArray(),
                        Storage = storInfolist.ToArray(),
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DeviceInfoAPI/GetOverview",
                        ResponseCode = 200,
                        Remark = "Get device overview information success."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(info));
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
                        URL = "DeviceInfo/GetOverview",
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
                    URL = "DeviceInfo/GetOverview",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 19. Get device detail information
        /// </summary>
        /// <param name="DeviceName">The device identity</param>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get deivce overview success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("DeviceInfoAPI/GetDetail")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public IActionResult GetDetail(string DeviceName, [FromHeader]string token)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                object obj = new object();
                BsonDocument bdoc = _ddb.GetLastRawData(string.Format("{0}-static", DeviceName));
                List<BsonDocument> bdoc2 = _ddb.GetRawData(string.Format("{0}-dynamic", DeviceName), JsonConvert.SerializeObject(obj), 10);
                string alias = _add.GetAlias(DeviceName);

                List<int> CPUUsage = new List<int>();
                List<int> MEMUsage = new List<int>();
                List<object> UsageTime = new List<object>();
                List<int> GPUUsage = new List<int>();

                int CPUFreq = Convert.ToInt32(((bdoc2[0])["CPU"]).AsBsonDocument["Freq"]);
                int CPUFan = Convert.ToInt32((bdoc2[0])["CPU"].AsBsonDocument["FanRPM"]);
                int CoreCount = Convert.ToInt32(CommonFunctions.GetData(bdoc["CPU"].AsBsonDocument["Numofcore"]));


                List<int> coreTempList = new List<int>();
                List<int> coreFreqList = new List<int>();
                List<double> coreUsageList = new List<double>();
                List<int> coreVoltageList = new List<int>();

                int memCap = Convert.ToInt32(CommonFunctions.GetData(bdoc["MEM"].AsBsonDocument["Cap"]));

                try
                {
                    foreach (BsonDocument bd in bdoc2)
                    {
                        GPUUsage.Add((Int32)Math.Round(Convert.ToDouble(CommonFunctions.GetData(bd["GPU"].AsBsonDocument["Load"])), MidpointRounding.AwayFromZero));
                        CPUUsage.Add((Int32)Math.Round(Convert.ToDouble(CommonFunctions.GetData(bd["CPU"].AsBsonDocument["Usage"])), MidpointRounding.AwayFromZero));
                        MEMUsage.Add((Int32)Math.Round((Convert.ToDouble(CommonFunctions.GetData(bd["MEM"]["memUsed"])) / memCap) * 100.0, MidpointRounding.AwayFromZero));
                        UsageTime.Add(bd["time"].AsInt32);
                    }
                    GPUUsage.Reverse();
                }
                catch (Exception)
                {
                    CPUUsage = new List<int>();
                    MEMUsage = new List<int>();
                    UsageTime = new List<object>();

                    foreach (BsonDocument bd in bdoc2)
                    {
                        CPUUsage.Add((Int32)Math.Round(Convert.ToDouble(CommonFunctions.GetData(bd["CPU"].AsBsonDocument["Usage"])), MidpointRounding.AwayFromZero));
                        MEMUsage.Add((Int32)Math.Round((Convert.ToDouble(CommonFunctions.GetData(bd["MEM"]["memUsed"])) / memCap) * 100.0, MidpointRounding.AwayFromZero));
                        UsageTime.Add(bd["time"].AsInt32);
                    }
                }

                for (int i = 0; i < CoreCount; i++)
                {
                    try
                    {
                        coreFreqList.Add(Convert.ToInt32(((bdoc2[0])["CPU"][$"{i}"]).AsBsonDocument["Freq"]));
                        coreUsageList.Add(Convert.ToDouble(((bdoc2[0])["CPU"][$"{i}"]).AsBsonDocument["Usage"]));
                        coreTempList.Add(Convert.ToInt32(((bdoc2[0])["CPU"][$"{i}"]).AsBsonDocument["Temp"]));
                        coreVoltageList.Add(Convert.ToInt32(((bdoc2[0])["CPU"][$"{i}"]).AsBsonDocument["V"]));
                    }
                    catch(Exception)
                    {
                        continue;
                    }
                }

                CPUCoreTemplate CoreInfo = new CPUCoreTemplate()
                {
                    CoreTemp = coreTempList.ToArray(),
                    CoreFreq = coreFreqList.ToArray(),
                    CoreUsage = coreUsageList.ToArray(),
                    CoreVoltage = coreVoltageList.ToArray()
                };

                CPUUsage.Reverse();
                MEMUsage.Reverse();
                UsageTime.Reverse();

                List<object> OS = new List<object>();
                OS.Add(new { key = "Operating System", value = bdoc["Sys"].AsBsonDocument["OS"].AsString });
                OS.Add(new { key = "OS Version", value = bdoc["Sys"].AsBsonDocument["OSVer"].AsString });
                OS.Add(new { key = "OS Architecture", value = bdoc["Sys"].AsBsonDocument["OSArch"].AsString });
                OS.Add(new { key = "Computer Name", value = bdoc["Sys"].AsBsonDocument["Name"].AsString });
                OS.Add(new { key = "Longitude", value = string.Format("{0}", Convert.ToDouble(CommonFunctions.GetData(bdoc["Sys"].AsBsonDocument["Longitude"]))) });
                OS.Add(new { key = "Latitude", value = string.Format("{0}", Convert.ToDouble(CommonFunctions.GetData(bdoc["Sys"].AsBsonDocument["Latitude"]))) });

                List<object> CPU = new List<object>();
                CPU.Add(new { key = "Manufacturer", value = bdoc["CPU"].AsBsonDocument["Manu"].AsString });
                CPU.Add(new { key = "Name", value = bdoc["CPU"].AsBsonDocument["Name"].AsString });
                CPU.Add(new { key = "Number of cores", value = string.Format("{0}", CoreCount) });
                CPU.Add(new { key = "L2 Cache size", value = string.Format("{0}KB", Convert.ToInt32(CommonFunctions.GetData(bdoc["CPU"].AsBsonDocument["L2"]))) });
                CPU.Add(new { key = "L3 Cache size", value = string.Format("{0}KB", Convert.ToInt32(CommonFunctions.GetData(bdoc["CPU"].AsBsonDocument["L3"]))) });

                List<object> MB = new List<object>();
                MB.Add(new { key = "Manufacturer", value = bdoc["MB"].AsBsonDocument["Manu"].AsString });
                MB.Add(new { key = "Product name", value = bdoc["MB"].AsBsonDocument["Product"].AsString });
                MB.Add(new { key = "Serial Number", value = bdoc["MB"].AsBsonDocument["SN"].AsString });
                MB.Add(new { key = "BIOS Manufacturer", value = bdoc["MB"].AsBsonDocument["BIOSManu"].AsString });
                MB.Add(new { key = "BIOS Version", value = bdoc["MB"].AsBsonDocument["BIOSVer"].AsString });

                string MEM = null;
                MEM = bdoc["MEM"].AsBsonDocument["Slot"].AsBsonDocument.ToJson();

                List<object[]> Stor = new List<object[]>();

                var isInnoAGE = false;
                Regex rgx = new Regex(@"^B");

                foreach (BsonDocument bd in bdoc["Storage"].AsBsonArray)
                {
                    List<object> so = new List<object>();
                    string firmwareVersion = bd["FWVer"].AsString;

                    if (rgx.IsMatch(bd["FWVer"].AsString))
                    {
                        isInnoAGE = true;
                    }

                    int index = bd["Index"].AsInt32;
                    so.Add(new { key = "Index", value = string.Format("{0}", index) });
                    so.Add(new { key = "Model", value = bd["Model"].AsString });
                    so.Add(new { key = "Serial Number", value = bd["SN"].AsString });
                    so.Add(new { key = "Firmware Version", value = firmwareVersion });
                    so.Add(new { key = "Capacity", value = string.Format("{0} GB", Math.Ceiling(Convert.ToInt32(CommonFunctions.GetData(bd["Par"].AsBsonDocument["TotalCap"])) / 1024.0 / 1024.0)) });
                    so.Add(new { key = "PECycle", value = string.Format("{0}", Convert.ToInt32(CommonFunctions.GetData(bdoc2.First()["Storage"].AsBsonArray[index].AsBsonDocument["PECycle"]))) });
                    so.Add(new { key = "Temperature", value = string.Format("{0} °C", CommonFunctions.GetData(bdoc2.First()["Storage"].AsBsonArray[index].AsBsonDocument["smart"].AsBsonDocument["194"]))});
                    Stor.Add(so.ToArray());
                }

                List<object[]> NET = new List<object[]>();
                foreach(BsonDocument bd in bdoc["Net"].AsBsonArray)
                {
                    List<object> net_obj = new List<object>();
                    net_obj.Add(new { key = "Name", value = bd["Name"].AsString });
                    net_obj.Add(new { key = "Type", value = bd["Type"].AsString });
                    net_obj.Add(new { key = "MAC", value = bd["MAC"].AsString });
                    net_obj.Add(new { key = "IPv4 Address", value = bd["IPaddr"].AsString });
                    net_obj.Add(new { key = "IPv4 Netmask", value = bd["Netmask"].AsString });
                    net_obj.Add(new { key = "IPv6 Address", value = bd["IPv6"].AsString });
                    NET.Add(net_obj.ToArray());
                }

                List<object> EXT = new List<object>();
                foreach(BsonElement bd in bdoc["Ext"].AsBsonDocument)
                {
                    EXT.Add(new
                    {
                        Name = bd.Value["Name"].AsString,
                        Unit = bd.Value["Unit"].AsString,
                        Type = bd.Value["Type"].AsInt32,
                        Value = CommonFunctions.GetData(bdoc2.First()["Ext"].AsBsonDocument[string.Format("{0}", Convert.ToInt32(bd.Name))])
                    });
                }

                BsonDocument gpuDynamic = null;
                string gpuStr = null;
                try
                {
                    gpuDynamic = (bdoc2[0])["GPU"].AsBsonDocument;

                    if (gpuDynamic.Count() > 0)
                    {
                        gpuDynamic.Remove("Load");
                        gpuStr = gpuDynamic.ToJson();
                    }
                    else
                    {
                        gpuStr = null;
                    }
                }
                catch (Exception)
                {
                    gpuStr = null;
                }


                string gpuStatic = null;
                try
                {
                    var gpuBson = bdoc["GPU"].AsBsonDocument;
                    gpuStatic = gpuBson.Count() > 0 ? gpuBson.ToJson() : null;
                }
                catch (Exception)
                {
                    gpuStatic = null;
                }

                var info = new
                {
                    DevName = DeviceName,
                    Alias = alias,
                    CPUUsage = CPUUsage.ToArray(),
                    MEMUsage = MEMUsage.ToArray(),
                    UsageTime = UsageTime.ToArray(),
                    OS = OS.ToArray(),
                    CPU = CPU.ToArray(),
                    MB = MB.ToArray(),
                    MEM,
                    Stor = Stor.ToArray(),
                    NET = NET.ToArray(),
                    EXT = EXT.ToArray(),
                    CPUCore = CoreInfo,
                    CPUFreq,
                    CPUFan,
                    GPUUsage = (GPUUsage.Count !=0) ? GPUUsage.ToArray() : null,
                    GPUDynamic = gpuStr,
                    GPU = gpuStatic,
                    IsInnoAGE = isInnoAGE
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "DeviceInfoAPI/GetDetail",
                    ResponseCode = 200,
                    Remark = "Get device detail information success."
                });

                return StatusCode(200, JsonConvert.SerializeObject(info));
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
                    URL = "DeviceInfoAPI/GetDetail",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 20. Get device location information
        /// </summary>
        /// <param name="DeviceName">The device identity</param>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get deivce overview success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("DeviceInfoAPI/GetLocation")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public IActionResult GetLocation(string DeviceName, [FromHeader]string token)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                MapItemTemplate mapret = _add.GetLocation(DeviceName);


                if (mapret == null)
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DeviceInfoAPI/GetLocation",
                        ResponseCode = 400,
                        Remark = "Device Name Error."
                    });

                    var retPayload1 = new
                    {
                        Response = "Device Name Error"
                    };

                    return StatusCode(400, JsonConvert.SerializeObject(retPayload1));
                }

                string query = "{\"Dev\":\"" + string.Format("{0}", mapret.name) + "\",\"Checked\":false}";
                BsonDocument bsonStatic = _ddb.GetLastRawData(string.Format("{0}-static", mapret.name));
                List<BsonDocument> bson = _ddb.GetRawData("EventLog", query, 1);
                if (bsonStatic != null)
                {
                    if (bson.Count == 0)
                    {
                        mapret.color = "green";
                        mapret.status = "NORMAL";
                        mapret.detail = "There is no event of this device.";
                        //mapret.time = DateTime.Now.ToLocalTime().ToString("yyyy/MM/dd hh:mm");
                        mapret.time = Convert.ToInt32((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
                        mapret.position = new MapItemTemplate.Pos();
                        mapret.position.lat = Convert.ToDouble(CommonFunctions.GetData(bsonStatic["Sys"].AsBsonDocument["Latitude"]));
                        mapret.position.lng = Convert.ToDouble(CommonFunctions.GetData(bsonStatic["Sys"].AsBsonDocument["Longitude"]));
                    }
                    else
                    {
                        mapret.color = "red";
                        mapret.status = "WARNING";
                        mapret.detail = bson.Last()["Message"].AsString;
                        mapret.time = Convert.ToInt32(CommonFunctions.GetData(bson.Last()["Time"]));
                        mapret.position = new MapItemTemplate.Pos();
                        mapret.position.lat = Convert.ToDouble(CommonFunctions.GetData(bsonStatic["Sys"].AsBsonDocument["Latitude"]));
                        mapret.position.lng = Convert.ToDouble(CommonFunctions.GetData(bsonStatic["Sys"].AsBsonDocument["Longitude"]));
                    }
                }
                else
                {
                    mapret.position = new MapItemTemplate.Pos();
                    mapret.position.lat = 100.0;
                    mapret.position.lng = 100.0;
                }

                MapItemTemplate[] markers = new MapItemTemplate[1];

                markers[0] = mapret;

                SelectOptionTemplate[] mapList = _adw.GetMapList();
                int mapId = _ade.GetUserCommonMap(user);
                var retPayload = new
                {
                    markers,
                    mapList,
                    mapId
                };

                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = user,
                    Method = "GET",
                    URL = "DeviceInfoAPI/GetLocation",
                    ResponseCode = 200,
                    Remark = "Get device location information success."
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
                    URL = "DeviceInfoAPI/GetLocation",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 21. Get device's group.
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="devName">The device name</param>
        /// <returns></returns>
        /// <response code="200">Get widget list successfully.</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("DeviceInfoAPI/Group")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetDeviceGroup([FromHeader]string token, string devName)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = "DeviceInfoAPI/Group",
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
                        branchName= _adb.GetBranchName(devName)
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DeviceInfoAPI/Group",
                        ResponseCode = 200,
                        Remark = "Get device's group successfully."
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
                        URL = "DeviceInfoAPI/Group",
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
                    URL = "DeviceInfoAPI/Group",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }


        /// <summary>
        /// 22. Get memory temperature
        /// </summary>
        /// <param name="token">The identity token</param>
        /// <param name="devName">The device name</param>
        /// <returns></returns>
        /// <response code="200">Get widget list successfully.</response>
        /// <response code="403">The identity token not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("DeviceInfoAPI/{devName}/Memory/Temperature-Distribution")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetTemperatureDistribution([FromHeader]string token, string devName)
        {
            LogAgent.WriteToLog(new LogAgent.LogFileFormat()
            {
                Direction = false,
                Name = "",
                Method = "Get",
                URL = $"DeviceInfoAPI/{devName}/Memory/Temperature-Distribution",
                ResponseCode = 0,
                Remark = ""
            });

            string user = _rcd.GetCache(0, token);

            if (user != null)
            {
                try
                {
                    DateTime time = Convert.ToDateTime(DateTimeOffset.UtcNow.ToString("MM/dd/yyyy"));
                    var today = (long)(time - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;///result
                    var builder = Builders<BsonDocument>.Filter;
                    var filter = builder.Gt("time", today);
                    List<BsonDocument> documents = _ddb.GetRawData($"{devName}-dynamic", filter);
                    string[] labels = new string[] { "below -21℃", "-20℃~0℃", "1℃~20℃", "21℃~40℃", "41℃~60℃", "61℃~80℃", "81℃~95℃", "above 96℃" };
                    int[] value = new int[8];
                    foreach (BsonDocument doc in documents)
                    {
                        int temp = (int)doc["MEM"]["temp"].AsBsonValue;
                        if (temp <= -21)
                        {
                            value[0]++;
                        }
                        else if(temp <= 0 && temp >= -20)
                        {
                            value[1]++;
                        }
                        else if (temp <= 20 && temp >= 1)
                        {
                            value[2]++;
                        }
                        else if (temp <=40 && temp >= 21)
                        {
                            value[3]++;
                        }
                        else if (temp <= 60 && temp >= 41)
                        {
                            value[4]++;
                        }
                        else if (temp <= 80 && temp >= 61)
                        {
                            value[5]++;
                        }
                        else if (temp <= 95 && temp >= 81)
                        {
                            value[6]++;
                        }
                        else if (temp >= 96)
                        {
                            value[7]++;
                        }
                    }

                    var retPayload = new
                    {
                        Labels = labels,
                        Value = value
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = $"DeviceInfoAPI/{devName}/Memory/Temperature-Distribution",
                        ResponseCode = 200,
                        Remark = "Get device's group successfully."
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
                        URL = $"DeviceInfoAPI/{devName}/Memory/Temperature-Distribution",
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
                    URL = $"DeviceInfoAPI/{devName}/Memory/Temperature-Distribution",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 1. Get device static raw data
        /// </summary>
        /// <param name="devName">The device name</param>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get deivce overview success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("DeviceInfoAPI/{devName}/Static")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetStaticData(string devName, [FromHeader]string token)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    Device dev = _add.Get(devName);
                    if (dev == null)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = $"DeviceInfoAPI/{devName}/Static",
                            ResponseCode = 204,
                            Remark = "Not Found"
                        });

                        return StatusCode(204);
                    }

                    BsonDocument bdocStatic = _ddb.GetLastRawData(string.Format("{0}-static", devName));
                    Int64 storageSize = 0;
                    var memCAP = -1;
                    object[] Static = null;
                    if (bdocStatic != null)
                    {
                        Static = new string[5];
                        foreach (BsonDocument bd in bdocStatic["Storage"].AsBsonArray)
                        {
                            storageSize += Convert.ToInt32(CommonFunctions.GetData(bd["Par"].AsBsonDocument["TotalCap"])) / 1024 / 1024;
                        }

                        memCAP = bdocStatic["MEM"].AsBsonDocument["Cap"].AsInt32;

                        int status = 0;
                        string devStatus = _rcd.GetStatus(devName);
                        if (devStatus != null)
                        {
                            int.TryParse(devStatus, out status);
                        }
                        Static[0] = bdocStatic["Sys"].AsBsonDocument["OS"].AsString;
                        Static[1] = bdocStatic["CPU"].AsBsonDocument["Name"].AsString;
                        Static[2] = Math.Round(memCAP / 1024.0 / 1024.0, 0, MidpointRounding.AwayFromZero) + " GB";
                        Static[3] = storageSize + " GB";
                        Static[4] = (status == 1) ? "Online" : "Offline";
                    }

                    var retPayload = new
                    {
                        DeviceInfo = Static,
                    };
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = $"DeviceInfoAPI/{devName}/Static",
                        ResponseCode = 200,
                        Remark = "Get device overview information success."
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
                        URL = $"DeviceInfoAPI/{devName}/Static",
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
                    URL = $"DeviceInfoAPI/{devName}/Static",
                    ResponseCode = 403,
                    Remark = "Token Error"
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 24. Get DFI Information
        /// </summary>
        /// <param name="devName">The device name</param>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get deivce overview success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("DeviceInfoAPI/{devName}/EAPI")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetEAPI([FromHeader] string token, string devName)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    BsonDocument bdoc2 = _ddb.GetLastRawData($"{devName}-dynamic");

                    if (bdoc2["EAPI"].AsBsonDocument.Contains("HWMON"))
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = $"DeviceInfoAPI/{devName}/EAPI",
                            ResponseCode = 200,
                            Remark = "Not Found"
                        });

                        return StatusCode(200, bdoc2["EAPI"]["HWMON"].ToJson());
                    }
                    else
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = $"DeviceInfoAPI/{devName}/EAPI",
                            ResponseCode = 404,
                            Remark = "Not Found"
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
                        URL = "DeviceInfoAPI/InnoAgelist",
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
                    URL = "DeviceInfoAPI/InnoAgelist",
                    ResponseCode = 403,
                    Remark = "Token Error"
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }
        /// <summary>
        /// 23. Get device storage list
        /// </summary>
        /// <param name="devName">The device name</param>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get deivce overview success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("DeviceInfoAPI/{devName}/Storagelist")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetStoragelist(string devName, [FromHeader]string token)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    BsonDocument bdoc = _ddb.GetLastRawData(string.Format("{0}-static", devName));
                    List<string> storagelist = new List<string>();

                    foreach (BsonDocument bd in bdoc["Storage"].AsBsonArray)
                    {
                        storagelist.Add(bd["SN"].AsString);
                    }

                    if (storagelist.Count == 0)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = $"DeviceInfoAPI/{devName}/Storagelist",
                            ResponseCode = 204,
                            Remark = "Not Found"
                        });

                        return StatusCode(204);
                    }

                    var retPayload = new
                    {
                        Storagelist = storagelist
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = $"DeviceInfoAPI/{devName}/Storagelist",
                        ResponseCode = 200,
                        Remark = "Get device overview information success."
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
                        URL = $"DeviceInfoAPI/{devName}/Storagelist",
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
                    URL = $"DeviceInfoAPI/{devName}/Storagelist",
                    ResponseCode = 403,
                    Remark = "Token Error"
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 23. Get InnoAge list
        /// </summary>
        /// <param name="devName">The device name</param>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get deivce overview success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("DeviceInfoAPI/InnoAGElist")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetInnoAGEelist([FromHeader]string token)
        {
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    List<string> innoAGElist = new List<string>();
                    var devices = _add.GetList();
                    Regex rgx = new Regex(@"^B");

                    foreach (string devName in devices)
                    {
                        BsonDocument bdoc = _ddb.GetLastRawData(string.Format("{0}-static", devName));

                        foreach (BsonDocument bd in bdoc["Storage"].AsBsonArray)
                        {
                            if (rgx.IsMatch(bd["FWVer"].AsString))
                            {
                                innoAGElist.Add(bd["SN"].AsString);
                            }
                        }
                    }

                    if (innoAGElist.Count == 0)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = user,
                            Method = "GET",
                            URL = "DeviceInfoAPI/InnoAgelist",
                            ResponseCode = 204,
                            Remark = "Not Found"
                        });

                        return StatusCode(204);
                    }

                    var retPayload = new
                    {
                        InnoAGElist = innoAGElist
                    };

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = "DeviceInfoAPI/InnoAgelist",
                        ResponseCode = 200,
                        Remark = "Get innoAge list successfully."
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
                        URL = "DeviceInfoAPI/InnoAgelist",
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
                    URL = "DeviceInfoAPI/InnoAgelist",
                    ResponseCode = 403,
                    Remark = "Token Error"
                });
                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }

        /// <summary>
        /// 23. Get storage information
        /// </summary>
        /// <param name="devName">The device name</param>
        /// <param name="token">The identity token</param>
        /// <returns></returns>
        /// <response code="200">Get deivce overview success</response>
        /// <response code="403">The identity token not found</response>
        [HttpGet("DeviceInfoAPI/{devName}/Storage")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult GetStorageInfo(string devName, [FromHeader]string token)
        {
            //Int64 storageSize = 0;
            //List<StorInfo> storInfolist = new List<StorInfo>();
            string user = _rcd.GetCache(0, token);
            if (user != null)
            {
                try
                {
                    BsonDocument bdoc = _ddb.GetLastRawData(string.Format("{0}-static", devName));
                    BsonDocument bdoc2 = _ddb.GetLastRawData(string.Format("{0}-dynamic", devName));
                    List<object[]> Stor = new List<object[]>();
                    List<object[]> lifespanHistory = new List<object[]>();
                    List<string> sns = new List<string>();
                    List<object> analyzer = new List<object>();
                    List<StorCount> avgCount = new List<StorCount>();
                    List<object> ret = new List<object>();
                    var storrw = new object();
                    foreach (BsonDocument bd in bdoc["Storage"].AsBsonArray)
                    {
                        List<object> so = new List<object>();
                        string firmwareVersion = bd["FWVer"].AsString;
                        int index = bd["Index"].AsInt32;
                        string sn = bd["SN"].AsString;
                        //var test = bdoc2["Storage"].AsBsonArray.Where(s=>s.AsBsonDocument["SN"] == sn).Select(s=>s).SingleOrDefault();
                        BsonDocument dynamic = bdoc2["Storage"].AsBsonArray.Where(s => s.AsBsonDocument["SN"] == sn).Select(s => s.AsBsonDocument).SingleOrDefault();
                        so.Add(new { name = "Index", value = string.Format("{0}", index) });
                        so.Add(new { name = "Model", value = bd["Model"].AsString });
                        so.Add(new { name = "Serial Number", value = sn });
                        so.Add(new { name = "Firmware Version", value = firmwareVersion });
                        so.Add(new { name = "Capacity", value = string.Format("{0} GB", Math.Ceiling(Convert.ToInt32(CommonFunctions.GetData(bd["Par"].AsBsonDocument["TotalCap"])) / 1024.0 / 1024.0)) });
                        so.Add(new { name = "PECycle", value = string.Format("{0}", Convert.ToInt32(CommonFunctions.GetData(dynamic["PECycle"]))) });
                        so.Add(new { name = "Temperature", value = string.Format("{0} °C", CommonFunctions.GetData(dynamic["smart"].AsBsonDocument["194"])) });
                        int? AvgECCount = Convert.ToInt32(CommonFunctions.GetData(dynamic["smart"].AsBsonDocument["167"]));
                        //// lifespan
                        BsonDocument bdoc3 = _ddb.GetAnalyzerData(sn);
                        if (bdoc3 == null) continue;

                        double initHealth = Convert.ToDouble(CommonFunctions.GetData(bdoc3["InitHealth"]));
                        int PECycle = Convert.ToInt32(CommonFunctions.GetData(bdoc3["PECycle"]));
                        int count = bdoc3["Lifespan"].AsBsonArray.Count;
                        object[] history = new object[count];
                        double? lastHealth = null;
                        BsonArray lifespan = bdoc3["Lifespan"].AsBsonArray;

                        for (int i = 0; i < count; i++)
                        {
                            history[i] = new
                            {
                                Date = (UInt64)Convert.ToDouble(CommonFunctions.GetData(lifespan[i].AsBsonDocument["time"])),
                                EstimatedDays = Convert.ToInt32(CommonFunctions.GetData(lifespan[i].AsBsonDocument["data"]))
                            };

                            if (i == count - 1)
                            {
                                lastHealth = Convert.ToDouble(CommonFunctions.GetData(lifespan[i].AsBsonDocument["health"]));
                            }
                        }
                        ////
                        double health = lastHealth == null ? Convert.ToDouble(CommonFunctions.GetData(bdoc3["InitHealth"])) : (double)lastHealth;
                        ////ianalyzer
                        int enable = Convert.ToInt32(CommonFunctions.GetData(dynamic["iAnalyzer"].AsBsonDocument["Enable"]));
                        if (enable != 0)
                        {
                            int subIndex = 0;
                            ChartDataTemplate SR, SW, RR, RW;
                            if (enable == 1)
                            {
                                SR = new ChartDataTemplate()
                                {
                                    label = new string[6],
                                    value = new int[6],
                                };
                                SW = new ChartDataTemplate()
                                {
                                    label = new string[6],
                                    value = new int[6]
                                };
                                RR = new ChartDataTemplate()
                                {
                                    label = new string[5],
                                    value = new int[5]
                                };
                                RW = new ChartDataTemplate()
                                {
                                    label = new string[5],
                                    value = new int[5]
                                };
                            }
                            else
                            {
                                SR = new ChartDataTemplate()
                                {
                                    label = new string[7],
                                    value = new int[7],
                                };
                                SW = new ChartDataTemplate()
                                {
                                    label = new string[7],
                                    value = new int[7]
                                };
                                RR = new ChartDataTemplate()
                                {
                                    label = new string[7],
                                    value = new int[7]
                                };
                                RW = new ChartDataTemplate()
                                {
                                    label = new string[7],
                                    value = new int[7]
                                };
                            }

                            #region SR
                            subIndex = 0;
                            foreach (BsonElement bdd in dynamic["iAnalyzer"].AsBsonDocument["SR"].AsBsonDocument)
                            {
                                SR.label[subIndex] = CommonFunctions.GetBehivorString(enable, subIndex, true);
                                SR.value[subIndex] = bdd.Value.AsInt32;
                                subIndex++;
                            }
                            #endregion

                            #region SW
                            subIndex = 0;
                            foreach (BsonElement bdd in dynamic["iAnalyzer"].AsBsonDocument["SW"].AsBsonDocument)
                            {
                                SW.label[subIndex] = CommonFunctions.GetBehivorString(enable, subIndex, true);
                                SW.value[subIndex] = bdd.Value.AsInt32;
                                subIndex++;
                            }
                            #endregion

                            #region RR
                            subIndex = 0;
                            foreach (BsonElement bdd in dynamic["iAnalyzer"].AsBsonDocument["RR"].AsBsonDocument)
                            {
                                RR.label[subIndex] = CommonFunctions.GetBehivorString(enable, subIndex, false);
                                RR.value[subIndex] = bdd.Value.AsInt32;
                                subIndex++;
                            }
                            #endregion

                            #region RW
                            subIndex = 0;
                            foreach (BsonElement bdd in dynamic["iAnalyzer"].AsBsonDocument["RW"].AsBsonDocument)
                            {
                                RW.label[subIndex] = CommonFunctions.GetBehivorString(enable, subIndex, false);
                                RW.value[subIndex] = bdd.Value.AsInt32;
                                subIndex++;
                            }
                            #endregion

                            storrw = new
                            {
                                SN = sn,
                                SRC = Convert.ToInt32(CommonFunctions.GetData(dynamic["iAnalyzer"].AsBsonDocument["SRC"])),
                                RRC = Convert.ToInt32(CommonFunctions.GetData(dynamic["iAnalyzer"].AsBsonDocument["RRC"])),
                                SWC = Convert.ToInt32(CommonFunctions.GetData(dynamic["iAnalyzer"].AsBsonDocument["SWC"])),
                                RWC = Convert.ToInt32(CommonFunctions.GetData(dynamic["iAnalyzer"].AsBsonDocument["RWC"])),
                                SR = SR,
                                SW = SW,
                                RR = RR,
                                RW = RW
                            };
                        }

                        ret.Add(new
                        {
                            SerialNumber = sn,
                            Information = so.ToArray(),
                            LifespanHistory = history,
                            Health = Math.Round(health, 2),
                            PECycle = PECycle,
                            AvgECCount = AvgECCount,
                            Analyzer = storrw
                        });
                    }

                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "GET",
                        URL = $"DeviceInfoAPI/{devName}/Storage",
                        ResponseCode = 200,
                        Remark = "Get device Storage information success."
                    });

                    return StatusCode(200, JsonConvert.SerializeObject(ret));
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
                        URL = $"DeviceInfoAPI/{devName}/Storage",
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
                    URL = $"DeviceInfoAPI/{devName}/Storage",
                    ResponseCode = 403,
                    Remark = "Request authentication token error."
                });

                return StatusCode(403, JsonConvert.SerializeObject(retPayload));
            }
        }
    }
}
