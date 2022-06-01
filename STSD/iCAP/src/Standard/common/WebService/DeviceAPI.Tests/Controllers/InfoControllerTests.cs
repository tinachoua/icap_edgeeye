using System;
using System.Collections.Generic;
using System.Text;
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
using Moq;
using NUnit.Framework;
using DeviceAPI.Controllers;
using ShareLibrary.DataTemplate;

namespace DeviceAPI.Tests.Controllers
{
    class InfoControllerTests
    {


        [Test]
        public void Info_GetOverview_TokenErrorTest()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange    
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetOverview(token, DeviceName);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        //enable=1
        public void Info_GetOverview_SuccessOneTest()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var bdoc = new BsonDocument()
            {
                {"MEM",new BsonDocument (){ { "Cap",100} } },
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                    {
                                        { "SN","BCADevice00001"},
                                        { "Model","2.5\"SATA SSD 3ME3"},
                                        {"Par", new BsonDocument()
                                                {
                                                    { "TotalCap",250051725}
                                                }
                                        }
                                    },
                            }
                },
                {"Remote",new BsonDocument()
                            {
                                {"0",new BsonDocument()
                                    {
                                        {"Name","iCoverX"}
                                    }
                                }
                            }
                },
                {"Sys",new BsonDocument()
                            {
                                {"OS","Microsoft Windows 10 Enterprise" }
                            }
                },
                {"CPU",new BsonDocument()
                            {
                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz" }
                            }
                }
            };

            var dybdoc = new BsonDocument()
            {
                {"CPU",new BsonDocument(){ { "Usage",1} } },
                { "MEM", new BsonDocument()
                    {
                        { "memUsed", 5939896},
                        { "temp", 30}
                    }
                },
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                {
                                        {"iAnalyzer", new BsonDocument()
                                                        {
                                                            { "Enable",1},
                                                            {"SRC",2784002},
                                                            {"RRC",39870315},
                                                            {"SWC",1527066},
                                                            {"RWC",66674392},
                                                            {"SR",new BsonDocument()
                                                                    {
                                                                        {"0",17629}
                                                                    }
                                                            },
                                                            {"SW",new BsonDocument()
                                                                    {
                                                                        {"0",10397}
                                                                    }

                                                            },
                                                            {"RR",new BsonDocument()
                                                                    {
                                                                        {"0",9466989}
                                                                    }
                                                            },
                                                            {"RW",new BsonDocument()
                                                                    {
                                                                        {"0",4381752}
                                                                    }
                                                            }
                                                        }
                                        },
                                        {"smart",new BsonDocument()
                                                    {
                                                        {"167",12}
                                                    }
                                        }
                                }
                            }
                }
            };

            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", DeviceName))).Returns(bdoc);
            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-dynamic", DeviceName))).Returns(dybdoc);

            var bdoc3 = new BsonDocument()
                            {
                                {"PECycle",3000},
                                {"Lifespan",new BsonArray()
                                            {
                                                new BsonDocument()
                                                    {
                                                        
                                                        {"time",1505260800.0},
                                                        {"health",99.6326530612245},
                                                        {"data",2441}                                                        
                                                    }
                                            }
                                }
                            };


            mockDataDB.Setup(d => d.GetAnalyzerData("BCADevice00001")).Returns(bdoc3);
            mockAdminDB_device.Setup(d => d.Get(DeviceName)).Returns(new Device());




            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetOverview(DeviceName, token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        //enable !=1
        public void Info_GetOverview_SuccessTwoTest()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var bdoc = new BsonDocument()
            {
                {"MEM",new BsonDocument (){ { "Cap",100} } },
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                    {
                                        { "SN","BCADevice00001"},
                                        { "Model","2.5\"SATA SSD 3ME3"},
                                        {"Par", new BsonDocument()
                                                {
                                                    { "TotalCap",250051725}
                                                }
                                        }
                                    },
                            }
                },
                {"Remote",new BsonDocument()
                            {
                                {"0",new BsonDocument()
                                    {
                                        {"Name","iCoverX"}
                                    }
                                }
                            }
                },
                {"Sys",new BsonDocument()
                            {
                                {"OS","Microsoft Windows 10 Enterprise" }
                            }
                },
                {"CPU",new BsonDocument()
                            {
                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz" }
                            }
                }
            };

            var dybdoc = new BsonDocument()
            {
                {"CPU",new BsonDocument(){ { "Usage",1} } },
                { "MEM", new BsonDocument()
                    {
                        { "memUsed", 5939896},
                        { "temp", 30}
                    }
                },,
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                {
                                        {"iAnalyzer", new BsonDocument()
                                                        {
                                                            { "Enable",2},
                                                            {"SRC",2784002},
                                                            {"RRC",39870315},
                                                            {"SWC",1527066},
                                                            {"RWC",66674392},
                                                            {"SR",new BsonDocument()
                                                                    {
                                                                        {"0",17629}
                                                                    }
                                                            },
                                                            {"SW",new BsonDocument()
                                                                    {
                                                                        {"0",10397}
                                                                    }

                                                            },
                                                            {"RR",new BsonDocument()
                                                                    {
                                                                        {"0",9466989}
                                                                    }
                                                            },
                                                            {"RW",new BsonDocument()
                                                                    {
                                                                        {"0",4381752}
                                                                    }
                                                            }
                                                        }
                                        },
                                        {"smart",new BsonDocument()
                                                    {
                                                        {"167",12}
                                                    }
                                        }
                                }
                            }
                }
            };

            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", DeviceName))).Returns(bdoc);
            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-dynamic", DeviceName))).Returns(dybdoc);

            var bdoc3 = new BsonDocument()
                            {
                                {"PECycle",3000},
                                {"Lifespan",new BsonArray()
                                            {
                                                new BsonDocument()
                                                    {

                                                        {"time",1505260800.0},
                                                        {"health",99.6326530612245},
                                                        {"data",2441}
                                                    }
                                            }
                                }
                            };


            mockDataDB.Setup(d => d.GetAnalyzerData("BCADevice00001")).Returns(bdoc3);
            mockAdminDB_device.Setup(d => d.Get(DeviceName)).Returns(new Device());




            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetOverview(DeviceName, token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        //enable ==0
        public void Info_GetOverview_SuccessThreeTest()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRC.Setup(t => t.GetStatus(DeviceName)).Returns("0");

            var bdoc = new BsonDocument()
            {
                {"MEM",new BsonDocument (){ { "Cap",100} } },
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                    {
                                        { "SN","BCADevice00001"},
                                        { "Model","2.5\"SATA SSD 3ME3"},
                                        {"Par", new BsonDocument()
                                                {
                                                    { "TotalCap",250051725}
                                                }
                                        }
                                    },
                            }
                },
                {"Remote",new BsonDocument()
                            {
                                {"0",new BsonDocument()
                                    {
                                        {"Name","iCoverX"}
                                    }
                                }
                            }
                },
                {"Sys",new BsonDocument()
                            {
                                {"OS","Microsoft Windows 10 Enterprise" }
                            }
                },
                {"CPU",new BsonDocument()
                            {
                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz" }
                            }
                }
            };

            var dybdoc = new BsonDocument()
            {
                {"CPU",new BsonDocument(){ { "Usage",1} } },
                { "MEM", new BsonDocument()
                    {
                        { "memUsed", 5939896},
                        { "temp", 30}
                    }
                },
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                {
                                        {"iAnalyzer", new BsonDocument()
                                                        {
                                                            { "Enable",0},
                                                            {"SRC",2784002},
                                                            {"RRC",39870315},
                                                            {"SWC",1527066},
                                                            {"RWC",66674392},
                                                            {"SR",new BsonDocument()
                                                                    {
                                                                        {"0",17629}
                                                                    }
                                                            },
                                                            {"SW",new BsonDocument()
                                                                    {
                                                                        {"0",10397}
                                                                    }

                                                            },
                                                            {"RR",new BsonDocument()
                                                                    {
                                                                        {"0",9466989}
                                                                    }
                                                            },
                                                            {"RW",new BsonDocument()
                                                                    {
                                                                        {"0",4381752}
                                                                    }
                                                            }
                                                        }
                                        },
                                        {"smart",new BsonDocument()
                                                    {
                                                        {"167",0}
                                                    }
                                        }
                                }
                            }
                }
            };

            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", DeviceName))).Returns(bdoc);
            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-dynamic", DeviceName))).Returns(dybdoc);

            var bdoc3 = new BsonDocument()
                            {
                                {"PECycle",3000},
                                {"Lifespan",new BsonArray()
                                            {
                                                new BsonDocument()
                                                    {

                                                        {"time",1505260800.0},
                                                        {"health",0},
                                                        {"data",2441}
                                                    }
                                            }
                                },
                                {"InitHealth",100}
                            };


            mockDataDB.Setup(d => d.GetAnalyzerData("BCADevice00001")).Returns(bdoc3);
            mockAdminDB_device.Setup(d => d.Get(DeviceName)).Returns(new Device());

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetOverview(DeviceName, token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }


        [Test]
        //enable ==0
        public void Info_GetOverview_DeviceNameError()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRC.Setup(t => t.GetStatus(DeviceName)).Returns("0");

            var bdoc = new BsonDocument()
            {
                {"MEM",new BsonDocument (){ { "Cap",100} } },
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                    {
                                        { "SN","BCADevice00001"},
                                        { "Model","2.5\"SATA SSD 3ME3"},
                                        {"Par", new BsonDocument()
                                                {
                                                    { "TotalCap",250051725}
                                                }
                                        }
                                    },
                            }
                },
                {"Remote",new BsonDocument()
                            {
                                {"0",new BsonDocument()
                                    {
                                        {"Name","iCoverX"}
                                    }
                                }
                            }
                },
                {"Sys",new BsonDocument()
                            {
                                {"OS","Microsoft Windows 10 Enterprise" }
                            }
                },
                {"CPU",new BsonDocument()
                            {
                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz" }
                            }
                }
            };

            var dybdoc = new BsonDocument()
            {
                {"CPU",new BsonDocument(){ { "Usage",1} } },
                 { "MEM", new BsonDocument()
                    {
                        { "memUsed", 5939896},
                        { "temp", 30}
                    }
                },
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                {
                                        {"iAnalyzer", new BsonDocument()
                                                        {
                                                            { "Enable",0},
                                                            {"SRC",2784002},
                                                            {"RRC",39870315},
                                                            {"SWC",1527066},
                                                            {"RWC",66674392},
                                                            {"SR",new BsonDocument()
                                                                    {
                                                                        {"0",17629}
                                                                    }
                                                            },
                                                            {"SW",new BsonDocument()
                                                                    {
                                                                        {"0",10397}
                                                                    }

                                                            },
                                                            {"RR",new BsonDocument()
                                                                    {
                                                                        {"0",9466989}
                                                                    }
                                                            },
                                                            {"RW",new BsonDocument()
                                                                    {
                                                                        {"0",4381752}
                                                                    }
                                                            }
                                                        }
                                        },
                                        {"smart",new BsonDocument()
                                                    {
                                                        {"167",0}
                                                    }
                                        }
                                }
                            }
                }
            };

            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", DeviceName))).Returns(bdoc);
            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-dynamic", DeviceName))).Returns(dybdoc);

            var bdoc3 = new BsonDocument()
                            {
                                {"PECycle",3000},
                                {"Lifespan",new BsonArray()
                                            {
                                                new BsonDocument()
                                                    {

                                                        {"time",1505260800.0},
                                                        {"health",0},
                                                        {"data",2441}
                                                    }
                                            }
                                },
                                {"InitHealth",100}
                            };

            mockDataDB.Setup(d => d.GetAnalyzerData("BCADevice00001")).Returns(bdoc3);
            mockAdminDB_device.Setup(d => d.Get(DeviceName)).Returns((Device)null);

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetOverview(DeviceName, token);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        //enable ==0
        public void Info_GetOverview_Exception()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRC.Setup(t => t.GetStatus(DeviceName)).Returns("0");

            var bdoc = new BsonDocument()
            {
                {"MEM",new BsonDocument (){ { "Cap",100} } },
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                    {
                                        { "SN","BCADevice00001"},
                                        { "Model","2.5\"SATA SSD 3ME3"},
                                        {"Par", new BsonDocument()
                                                {
                                                    { "TotalCap",250051725}
                                                }
                                        }
                                    },
                            }
                },
                {"Remote",new BsonDocument()
                            {
                                {"0",new BsonDocument()
                                    {
                                        {"Name","iCoverX"}
                                    }
                                }
                            }
                },
                {"Sys",new BsonDocument()
                            {
                                {"OS","Microsoft Windows 10 Enterprise" }
                            }
                },
                {"CPU",new BsonDocument()
                            {
                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz" }
                            }
                }
            };

            var dybdoc = new BsonDocument()
            {
                {"CPU",new BsonDocument(){ { "Usage",1} } },
                { "MEM", new BsonDocument()
                    {
                        { "memUsed", 5939896},
                        { "temp", 30}
                    }
                },
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                {
                                        {"iAnalyzer", new BsonDocument()
                                                        {
                                                            { "Enable",0},
                                                            {"SRC",2784002},
                                                            {"RRC",39870315},
                                                            {"SWC",1527066},
                                                            {"RWC",66674392},
                                                            {"SR",new BsonDocument()
                                                                    {
                                                                        {"0",17629}
                                                                    }
                                                            },
                                                            {"SW",new BsonDocument()
                                                                    {
                                                                        {"0",10397}
                                                                    }

                                                            },
                                                            {"RR",new BsonDocument()
                                                                    {
                                                                        {"0",9466989}
                                                                    }
                                                            },
                                                            {"RW",new BsonDocument()
                                                                    {
                                                                        {"0",4381752}
                                                                    }
                                                            }
                                                        }
                                        },
                                        {"smart",new BsonDocument()
                                                    {
                                                        {"167",0}
                                                    }
                                        }
                                }
                            }
                }
            };

            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", DeviceName))).Returns(bdoc);
            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-dynamic", DeviceName))).Returns(dybdoc);

            var bdoc3 = new BsonDocument()
                            {
                                {"PECycle",3000},
                                {"Lifespan",new BsonArray()
                                            {
                                                new BsonDocument()
                                                    {

                                                        {"time",1505260800.0},
                                                        {"health",0},
                                                        {"data",2441}
                                                    }
                                            }
                                },
                                {"InitHealth",100}
                            };

            mockDataDB.Setup(d => d.GetAnalyzerData("BCADevice00001")).Throws(new Exception());
            mockAdminDB_device.Setup(d => d.Get(DeviceName)).Returns((Device)null);

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetOverview(DeviceName, token);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }




        [Test]
        public void Info_GetDetail_TokenErrorTest()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetDetail(token, DeviceName);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Info_GetDetail_SuccessTest()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            var bdoc = new BsonDocument()
            {
                {"MEM",new BsonDocument ()
                            {
                                { "Cap",100},
                                { "Slot",new BsonDocument()
                                            {
                                                {"0",new BsonDocument()
                                                        {
                                                            {"Manu","Innodisk"},
                                                            {"Loc","CHANNELB-DIMM1"},
                                                            {"Cap",16777216},
                                                            {"Freq",2400},
                                                            {"V",1.2}
                                                        }
                                                }
                                            }
                                }
                            }
                },
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                    {
                                        {"Index",0},
                                        {"FWVer","S16425"},
                                        { "SN","BCADevice00001"},
                                        { "Model","2.5\"SATA SSD 3ME3"},
                                        {"Par", new BsonDocument()
                                                {
                                                    { "TotalCap",250051725}
                                                }
                                        }
                                    },
                            }
                },
                {"Net" ,new BsonArray()
                            {
                                new BsonDocument()
                                    {
                                        {"Name","eth0"},
                                        {"Type","Ethernet"},
                                        {"MAC","aa:bb:cc:dd:ee:dd"},
                                        {"IPv6",""},
                                        {"IPaddr","192.168.0.2"},
                                        {"Netmask","255.255.255.0" }
                                    },
                            }
                },
                {
                    "Ext",new BsonDocument(){ }
                },
                {"Remote",new BsonDocument()
                            {
                                {"0",new BsonDocument()
                                    {
                                        {"Name","iCoverX"}
                                    }
                                }
                            }
                },
                {"Sys",new BsonDocument()
                            {
                                {"OS","Microsoft Windows 10 Enterprise" },
                                {"OSVer","x86_64" },
                                {"OSArch","64-bit" },
                                {"Name","innotest" },
                                {"Longitude",121.635847},
                                {"Latitude","25.05816" }
                            }
                },
                {"CPU",new BsonDocument()
                            {
                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz" },
                                {"Manu","GenuineIntel" },
                                {"Numofcore", 1},
                                {"L2",1024},
                                {"L3",6144}
                            }
                },
                {"MB",new BsonDocument()
                            {
                                {"Manu","Gigabyte Technology Co., Ltd." },
                                {"Product","Z270X-UD3-CF" },
                                {"SN","Default string"},
                                {"BIOSManu","American Megatrends Inc." },
                                {"BIOSVer","ALASKA - 1072009"}
                            }
                },
                {"GPU",new BsonDocument()
                            {
                                {"Name", "NVIDIA Tegra X2" },
                                {"Arch", "NVIDIA Pascal™" },
                                {"DriverVer", "9.0"},
                                {"ComputerCap", "6.2" },
                                {"CoreNum", "256"},
                                {"MemType", "LDDR4"},
                                {"MemBusWidth", "128.bit"},
                                {"MemSize", "7846 MBtyes"},
                                {"MemBandWidth", "59.7 GB/s"},
                                {"Clock", "1301 MHz"},
                                {"MemClock", "1600 MHz"}
                            }
                },
            };

            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", DeviceName))).Returns(bdoc);

            var dybdoc = new BsonDocument()
            {
                {"CPU",new BsonDocument(){
                    { "0", new BsonDocument()
                        {
                            {"Freq", 1309},
                            {"Usage", 21.0},
                            {"Temp", 35},
                            {"V", 8}
                        }
                    },
                    { "Usage", 1},
                    {"Freq", 1250},
                    {"FanRPM", 2000}

                } },
                { "MEM", new BsonDocument()
                    {
                        { "memUsed", 5939896},
                        { "temp", 30}
                    }
                },
                {"time",1506998400.0},
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                {
                                        {"PECycle",3000},
                                        {"iAnalyzer", new BsonDocument()
                                                        {
                                                            { "Enable",1},
                                                            {"SRC",2784002},
                                                            {"RRC",39870315},
                                                            {"SWC",1527066},
                                                            {"RWC",66674392},
                                                            {"SR",new BsonDocument()
                                                                    {
                                                                        {"0",17629}
                                                                    }
                                                            },
                                                            {"SW",new BsonDocument()
                                                                    {
                                                                        {"0",10397}
                                                                    }

                                                            },
                                                            {"RR",new BsonDocument()
                                                                    {
                                                                        {"0",9466989}
                                                                    }
                                                            },
                                                            {"RW",new BsonDocument()
                                                                    {
                                                                        {"0",4381752}
                                                                    }
                                                            }
                                                        }
                                        },
                                        {"smart",new BsonDocument()
                                                    {
                                                        {"167",12},
                                                        {"194",57}
                                                    }
                                        }
                                }
                            }
                },
                {"GPU",new BsonDocument()
                            {
                                {"CoreClock", "0" },
                                {"Temp", "0C" },
                                {"MemUsed", "1274/7847MB"},
                                {"Load", "0" },
                                {"FanTemp", "0C.9C"},
                            }
                },
            };

            List<BsonDocument> bdoc2 = new List<BsonDocument>();
            
            bdoc2.Add(dybdoc);
            object obj = new object();
            mockDataDB.Setup(d => d.GetRawData(string.Format("{0}-dynamic", DeviceName), JsonConvert.SerializeObject(obj), 10)).Returns(bdoc2);



            //Act
            ObjectResult actual = (ObjectResult)_target.GetDetail(DeviceName, token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Info_GetLocation_TokenErrorTest()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetLocation(DeviceName, token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }


        [Test]
        //no eventlog
        public void Info_GetLocation_SuccessOneTest()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            MapItemTemplate mapret = new MapItemTemplate()
            {
                name = "device00001"
            };

            mockAdminDB_device.Setup(d => d.GetLocation(DeviceName)).Returns(mapret);

            var bsonStatic = new BsonDocument()
            {
                {"MEM",new BsonDocument ()
                            {
                                { "Cap",100},
                                { "Slot",new BsonDocument()
                                            {
                                                {"0",new BsonDocument()
                                                        {
                                                            {"Manu","Innodisk"},
                                                            {"Loc","CHANNELB-DIMM1"},
                                                            {"Cap",16777216},
                                                            {"Freq",2400},
                                                            {"V",1.2}
                                                        }
                                                }
                                            }
                                }
                            }
                },
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                    {
                                        {"Index",0},
                                        {"FWVer","S16425"},
                                        { "SN","BCADevice00001"},
                                        { "Model","2.5\"SATA SSD 3ME3"},
                                        {"Par", new BsonDocument()
                                                {
                                                    { "TotalCap",250051725}
                                                }
                                        }
                                    },
                            }
                },
                {"Remote",new BsonDocument()
                            {
                                {"0",new BsonDocument()
                                    {
                                        {"Name","iCoverX"}
                                    }
                                }
                            }
                },
                {"Sys",new BsonDocument()
                            {
                                {"OS","Microsoft Windows 10 Enterprise" },
                                {"OSVer","x86_64" },
                                {"OSArch","64-bit" },
                                {"Name","innotest" },
                                {"Longitude",121.635847},
                                {"Latitude","25.05816" }
                            }
                },
                {"CPU",new BsonDocument()
                            {
                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz" },
                                {"Manu","GenuineIntel" },
                                {"Numofcore",4},
                                {"L2",1024},
                                {"L3",6144}
                            }
                },
                {"MB",new BsonDocument()
                            {
                                {"Manu","Gigabyte Technology Co., Ltd." },
                                {"Product","Z270X-UD3-CF" },
                                {"SN","Default string"},
                                {"BIOSManu","American Megatrends Inc." },
                                {"BIOSVer","ALASKA - 1072009"}
                            }
                },
            };

            List<BsonDocument> bson = new List<BsonDocument>();        

            string query = "{\"Dev\":\"" + string.Format("{0}", mapret.name) + "\",\"Checked\":false}";

            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", mapret.name))).Returns(bsonStatic);
            mockDataDB.Setup(d => d.GetRawData("EventLog", query, 1)).Returns(bson);

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetLocation(DeviceName, token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }



        [Test]
        //happen event
        public void Info_GetLocation_SuccessTwoTest()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            MapItemTemplate mapret = new MapItemTemplate()
            {
                name = "device00001"
            };

            mockAdminDB_device.Setup(d => d.GetLocation(DeviceName)).Returns(mapret);

            var bsonStatic = new BsonDocument()
            {
                {"MEM",new BsonDocument ()
                            {
                                { "Cap",100},
                                { "Slot",new BsonDocument()
                                            {
                                                {"0",new BsonDocument()
                                                        {
                                                            {"Manu","Innodisk"},
                                                            {"Loc","CHANNELB-DIMM1"},
                                                            {"Cap",16777216},
                                                            {"Freq",2400},
                                                            {"V",1.2}
                                                        }
                                                }
                                            }
                                }
                            }
                },
                {"Storage" ,new BsonArray()
                            {
                                new BsonDocument()
                                    {
                                        {"Index",0},
                                        {"FWVer","S16425"},
                                        { "SN","BCADevice00001"},
                                        { "Model","2.5\"SATA SSD 3ME3"},
                                        {"Par", new BsonDocument()
                                                {
                                                    { "TotalCap",250051725}
                                                }
                                        }
                                    },
                            }
                },
                {"Remote",new BsonDocument()
                            {
                                {"0",new BsonDocument()
                                    {
                                        {"Name","iCoverX"}
                                    }
                                }
                            }
                },
                {"Sys",new BsonDocument()
                            {
                                {"OS","Microsoft Windows 10 Enterprise" },
                                {"OSVer","x86_64" },
                                {"OSArch","64-bit" },
                                {"Name","innotest" },
                                {"Longitude",121.635847},
                                {"Latitude","25.05816" }
                            }
                },
                {"CPU",new BsonDocument()
                            {
                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz" },
                                {"Manu","GenuineIntel" },
                                {"Numofcore",4},
                                {"L2",1024},
                                {"L3",6144}
                            }
                },
                {"MB",new BsonDocument()
                            {
                                {"Manu","Gigabyte Technology Co., Ltd." },
                                {"Product","Z270X-UD3-CF" },
                                {"SN","Default string"},
                                {"BIOSManu","American Megatrends Inc." },
                                {"BIOSVer","ALASKA - 1072009"}
                            }
                },
            };

            var bsonEventlog = new BsonDocument()
            {
                { "Time",1506932164.0 },
                {"Message","Storage 0 temperature over thershold, value : 77 celsius."}
            };
            List<BsonDocument> bson = new List<BsonDocument>();

            bson.Add(bsonEventlog);

            string query = "{\"Dev\":\"" + string.Format("{0}", mapret.name) + "\",\"Checked\":false}";

            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", mapret.name))).Returns(bsonStatic);
            mockDataDB.Setup(d => d.GetRawData("EventLog", query, 1)).Returns(bson);

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetLocation(DeviceName, token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        //no data in {0}-static
        public void Info_GetLocation_SuccessThreeTest()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            MapItemTemplate mapret = new MapItemTemplate()
            {
                name = "device00001"
            };

            mockAdminDB_device.Setup(d => d.GetLocation(DeviceName)).Returns(mapret);

            var bsonStatic = new BsonDocument();        
                        
            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", mapret.name))).Returns((BsonDocument)null);

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetLocation(DeviceName, token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        //no data in {0}-static
        public void Info_GetLocation_DeviceNameError()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            MapItemTemplate mapret = null;

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetLocation(DeviceName, token);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Info_GetDeviceGroup_TokenErrorTest()
        {
            var token = "test";
            var DeviceName = "device00001";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetDeviceGroup(token, DeviceName);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Info_GetDeviceGroup_SuccessTest()
        {
            var token = "admin";
            var DeviceName = "device00001";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("admin");
            mockAdminDB_branch.Setup(b => b.GetBranchName(DeviceName)).Returns("all devices, taipei");

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetDeviceGroup(token, DeviceName);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Info_GetDeviceGroup_ExceptionTest()
        {
            var token = "admin";
            var DeviceName = "device00001";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockWidget = new Mock<IWidgetRepository>();
            Mock<IBranch> mockAdminDB_branch = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("admin");
            mockAdminDB_branch.Setup(b => b.GetBranchName(DeviceName)).Throws(new Exception());

            var _target = new InfoController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockWidget.Object, mockEmp.Object, mockAdminDB_branch.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetDeviceGroup(token, DeviceName);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }
    }
}
