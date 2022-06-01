using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShareLibrary.DataTemplate;
using ShareLibrary.Interface;
using DashboardAPI.Controllers;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Bson;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DashboardAPI.Tests.Controllers
{
    class SettingControllerTests
    {


        [Test]
        public void Setting_GetThreshold_TokenErrorTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetThreshold(token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_GetThreshold_SuccessTest()
        {
            var token = "abc";
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var bdoc = new BsonDocument
            {
                { "Storage",new BsonArray()
                           {
                                new BsonDocument
                                {
                                    {"Name","Temp" },
                                    {"Value",1.0 },
                                    {"Enable",1 },
                                    {"Func",1 },
                                }
                           }
                },
                { "Ext",new BsonArray()
                       {
                            new BsonDocument
                            {
                                {"Name","Temp" },
                                {"Value",1.0 },
                                {"Enable",1 },
                                {"Func",1 },
                            }
                       }
                }


            };

            mockDataDB.Setup(d => d.GetLastRawData("ThresholdSetting")).Returns(bdoc);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetThreshold(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void SettingSetThresholdTokenErrorTest()
        {
            var token = "test";
            ThresholdSettingTemplate[] data = { };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetThreshold(token, data);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_SetThreshold_PayloadNullTest()
        {
            var token = "test";
            ThresholdSettingTemplate[] data = null;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetThreshold(token, data);

            //Assert          
            Assert.AreEqual(404, actual.StatusCode);
        }

        [Test]
        public void Setting_SetThreshold_PayloadErrorTest()
        {
            string token = "test";
            ThresholdSettingTemplate[] data = new ThresholdSettingTemplate[1];
            for (int i = 0; i < 1; i++)
            {
                data[i] = new ThresholdSettingTemplate();
            }
            data[0].Name = null;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetThreshold(token, data);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_SetThreshold_SuccessStorageTest()
        {
            string token = "test";
            ThresholdSettingTemplate[] data = new ThresholdSettingTemplate[1];
            for (int i = 0; i < 1; i++)
            {
                data[i] = new ThresholdSettingTemplate();
            }
            data[0].Name = "test";
            data[0].Class = "Storage";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetThreshold(token, data);

            //Assert          
            Assert.AreEqual(202, actual.StatusCode);
        }

        [Test]
        public void Setting_SetThreshold_SuccessExtTest()
        {
            string token = "test";
            ThresholdSettingTemplate[] data = new ThresholdSettingTemplate[1];
            for (int i = 0; i < 1; i++)
            {
                data[i] = new ThresholdSettingTemplate();
            }
            data[0].Name = "test";
            data[0].Class = "external";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetThreshold(token, data);

            //Assert          
            Assert.AreEqual(202, actual.StatusCode);
        }

        [Test]
        public void Setting_SetThreshold_ExceptionTest()
        {
            string token = "test";
            ThresholdSettingTemplate[] data = new ThresholdSettingTemplate[1];
            for (int i = 0; i < 1; i++)
            {
                data[i] = new ThresholdSettingTemplate();
            }
            data[0].Name = "test";
            data[0].Class = "external";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRC.Setup(t => t.PublishSetThreshold(data[0])).Throws(new Exception(""));

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetThreshold(token, data);

            //Assert          
            Assert.AreEqual(304, actual.StatusCode);
        }

        [Test]
        public void Setting_GetThresholdList_TokenErrorTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetThresholdList(token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_GetThresholdList_SuccessTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_th.Setup(t => t.GetThresholdList()).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate { Id = 1, Name = "test" } });

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetThresholdList(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Setting_GetThresholdList_NotFoundTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_th.Setup(t => t.GetThresholdList()).Returns((SelectOptionTemplate[])null);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetThresholdList(token);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Setting_GetThresholdList_ExceptionTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_th.Setup(b => b.GetThresholdList()).Throws(new Exception());

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetThresholdList(token);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }

        [Test]
        public void Setting_CreateThreshold_TokenErrorTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = new ThresholdTemplate()
            {
                Name = "test",
                DataId = 1,
                Value ="50",
                Func = 1
            };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.CreateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_CreateThreshold_NotAdminTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = new ThresholdTemplate()
            {
                Name = "test",
                DataId = 1,
                Value = "50",
                Func = 1
            };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.CreateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_CreateThreshold_ExistsTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = new ThresholdTemplate()
            {
                Name = "test",
                DataId = 1,
                Value = "50",
                Func = 1
            };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(t => t.NameExists(thInfo.Name)).Returns(true);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.CreateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(409, actual.StatusCode);
        }

        [Test]
        public void Setting_CreateThreshold_SuccessTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = new ThresholdTemplate()
            {
                Name = "test",
                DataId = 1,
                Value = "50",
                Func = 1
            };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(t => t.NameExists(thInfo.Name)).Returns(false);
            mockAdminDB_th.Setup(t => t.Create(thInfo));

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.CreateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(201, actual.StatusCode);
        }

        [Test]
        public void Setting_CreateThreshold_DBErrorTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = new ThresholdTemplate()
            {
                Name = "test",
                DataId = 1,
                Value = "50",
                Func = 1
            };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(t => t.NameExists(thInfo.Name)).Returns(false);
            mockAdminDB_th.Setup(t => t.Create(thInfo)).Throws(new DbUpdateException("", new Exception()));

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.CreateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_CreateThreshold_ExceptionTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = null;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            //mockAdminDB_th.Setup(t => t.NameExists(thInfo.Name)).Returns(false);
            //mockAdminDB_th.Setup(t => t.Create(thInfo)).Throws(new Exception());

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.CreateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_UpdateThreshold_TokenErrorTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = new ThresholdTemplate()
            {
                Id = 1,
                Name = "test",
                DataId = 1,
                Value = "50",
                Func = 1
            };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.UpdateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_UpdateThreshold_NotAdminTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = new ThresholdTemplate()
            {
                Id = 1,
                Name = "test",
                DataId = 1,
                Value = "50",
                Func = 1
            };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.UpdateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_UpdateThreshold_NameExistsTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = new ThresholdTemplate()
            {
                Id = 1,
                Name = "test",
                DataId = 1,
                Value = "50",
                Func = 1
            };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(b => b.NameExists(thInfo.Name, thInfo.Id)).Returns(true);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);
             
            //Act

            ObjectResult actual = (ObjectResult)_target.UpdateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(409, actual.StatusCode);
        }

        [Test]
        public void Setting_UpdateThreshold_NotFoundTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = new ThresholdTemplate()
            {
                Id = 100,
                Name = "test",
                DataId = 1,
                Value = "50",
                Func = 1
            };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(b => b.NameExists(thInfo.Name, thInfo.Id)).Returns(false);
            mockAdminDB_th.Setup(t => t.Update(thInfo)).Throws(new Exception());

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.UpdateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_UpdateThreshold_NameNullTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = new ThresholdTemplate()
            {
                Id = 100,
                Name = null,
                DataId = 1,
                Value = "50",
                Func = 1
            };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(b => b.NameExists(thInfo.Name, thInfo.Id)).Returns(false);
            mockAdminDB_th.Setup(t => t.Update(thInfo)).Throws(new Exception());

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.UpdateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_UpdateThreshold_InputNullTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = null;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            //mockAdminDB_th.Setup(b => b.NameExists(thInfo.Name, thInfo.Id));
            //mockAdminDB_th.Setup(b => b.Update(thInfo)).Throws(new Exception());


            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.UpdateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }


        [Test]
        public void Setting_UpdateThreshold_ExceptionTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = new ThresholdTemplate
            {
                Name = "test",
                Id = 1
            };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(b => b.NameExists(thInfo.Name, thInfo.Id)).Returns(false);
            mockAdminDB_th.Setup(b => b.Update(thInfo)).Throws(new Exception());


            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.UpdateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_UpdateThreshold_UpdateFailTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = new ThresholdTemplate()
            {
                Id = 100,
                Name = "test",
                DataId = 1,
                Value = "50",
                Func = 1
            };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(t => t.NameExists(thInfo.Name, thInfo.Id)).Returns(false);
            mockAdminDB_th.Setup(t => t.Update(thInfo)).Throws(new DbUpdateException("", new Exception()));


            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.UpdateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_UpdateThreshold_UpdateSuccessTest()
        {
            var token = "test";
            ThresholdTemplate thInfo = new ThresholdTemplate()
            {
                Id = 100,
                Name = "test",
                DataId = 1,
                Value = "50",
                Func = 1
            };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(t => t.NameExists(thInfo.Name)).Returns(false);
            mockAdminDB_th.Setup(b => b.Update(thInfo));


            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.UpdateThreshold(token, thInfo);

            //Assert
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Setting_Delete_TokenErrorTest()
        {
            var token = "test";
            string thresholdId = "1";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, thresholdId);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_Delete_NotAdminTest()
        {
            var token = "test";
            string thresholdId = "1";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, thresholdId);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_Delete_SuccessTest()
        {
            var token = "test";
            string thresholdId = "1";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(t => t.Delete(Int32.Parse(thresholdId)));

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, thresholdId);

            //Assert
            Assert.AreEqual(202, actual.StatusCode);
        }

        [Test]
        public void Setting_Delete_NotFoundTest()
        {
            var token = "test";
            string thresholdId = "100";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(b => b.Delete(Int32.Parse(thresholdId))).Throws(new Exception("The threshold rule can NOT be found"));

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, thresholdId);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_Delete_InputNullTest()
        {
            var token = "test";
            string thresholdId = null;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, thresholdId);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_Delete_FailTest()
        {
            var token = "test";
            string thresholdId = "1";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(t => t.Delete(Int32.Parse(thresholdId))).Throws(new Exception("Failed to delete threshold rule. Please refresh the web page and try again."));

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, thresholdId);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        //[Test]
        //public void Setting_GetThresholdDetail_TokenErrorTest()
        //{
        //    var token = "test";
        //    int thresholdId = 1;

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
        //    Mock<IData> mockAdminDB_data = new Mock<IData>();
        //    Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
        //    Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
        //    Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

        //    var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.GetThresholdDetail(token, thresholdId);

        //    //Assert
        //    Assert.AreEqual(403, actual.StatusCode);
        //}

        //[Test]
        //public void Setting_GetThresholdDetail_NotFoundTest()
        //{
        //    var token = "test";
        //    //string thresholdId = "10";
        //    int id = 10;

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
        //    Mock<IData> mockAdminDB_data = new Mock<IData>();
        //    Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
        //    Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
        //    Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockAdminDB_th.Setup(t => t.GetDetail(id)).Returns((ThresholdTemplate)null);

        //    var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

        //    //Act

        //    StatusCodeResult actual = (StatusCodeResult)_target.GetThresholdDetail(token, id);

        //    //Assert
        //    Assert.AreEqual(204, actual.StatusCode);
        //}

        //[Test]
        //public void Setting_GetThresholdDetail_SuccessTest()
        //{
        //    var token = "test";
        //    int id = 1;

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
        //    Mock<IData> mockAdminDB_data = new Mock<IData>();
        //    Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
        //    Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
        //    Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockAdminDB_th.Setup(t => t.GetDetail(id)).Returns(new ThresholdTemplate() { Id = 1, Name = "test", DataId = 1, DeletedFlag = false, Enable = true, Func = 2, Value = "30" });
        //    mockAdminDB_data.Setup(d => d.GetThresholdDataSource()).Returns(new DataSelect[]{ new DataSelect() { DataOption = new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Name = "test" } }, GroupOption = new SelectOptionTemplate() { Id = 1, Name = "CPU" } } });
        //    var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.GetThresholdDetail(token, id);

        //    //Assert
        //    Assert.AreEqual(200, actual.StatusCode);
        //}

        //[Test]
        //public void Setting_GetThresholdDetail_ExceptionTest()
        //{
        //    var token = "test";
        //    //string thresholdId = "1";
        //    int id = 1;

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
        //    Mock<IData> mockAdminDB_data = new Mock<IData>();
        //    Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
        //    Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
        //    Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockAdminDB_th.Setup(t => t.GetDetail(id)).Throws(new Exception());

        //    var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.GetThresholdDetail(token, id);

        //    //Assert
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        [Test]
        public void Setting_GetThresholdGroupSetting_TokenErrorTest()
        {
            var token = "test";
            int id = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.GetThresholdGroupSetting(token, id);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_GetThresholdGroupSetting_SuccessTest()
        {
            var token = "test";
            int id = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            //mockAdminDB_th.Setup(t => t.GetDetail(id)).Returns(new ThresholdTemplate() { Id = 1, Name = "test"});
            mockAdminDB_th.Setup(t => t.GetSelectedGroup(id)).Returns(new SelectOptionTemplate[]{ new SelectOptionTemplate() {Id = 1, Name = "All Devices"} });
            mockAdminDB_group.Setup(g => g.GetBranchList()).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Name = "All Devices"} });

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.GetThresholdGroupSetting(token, id);

            //Assert
            Assert.AreEqual(200, actual.StatusCode);
        }

        //[Test]
        //public void Setting_GetThresholdGroupSetting_NotFoundTest()
        //{
        //    var token = "test";
        //    int id = 1;

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
        //    Mock<IData> mockAdminDB_data = new Mock<IData>();
        //    Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
        //    Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
        //    Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //   // mockAdminDB_th.Setup(t => t.GetDetail(id)).Returns((ThresholdTemplate)null);
            
        //    var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

        //    //Act

        //    StatusCodeResult actual = (StatusCodeResult)_target.GetThresholdGroupSetting(token, id);

        //    //Assert
        //    Assert.AreEqual(204, actual.StatusCode);
        //}

        [Test]
        public void Setting_GetThresholdGroupSetting_NotFoundExceptionTest()
        {
            var token = "test";
            int id = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            //mockAdminDB_th.Setup(t => t.GetDetail(id)).Returns(new ThresholdTemplate() { Id = 1, Name = "test" });
            mockAdminDB_th.Setup(t => t.GetSelectedGroup(id)).Throws(new Exception());
            mockAdminDB_th.Setup(t => t.ThresholdExists(id)).Returns(false);
            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            StatusCodeResult actual = (StatusCodeResult) _target.GetThresholdGroupSetting(token, id);

            //Assert
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Setting_GetThresholdGroupSetting_ExceptionTest()
        {
            var token = "test";
            int id = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            //mockAdminDB_th.Setup(t => t.GetDetail(id)).Returns(new ThresholdTemplate() { Id = 1, Name = "test" });
            mockAdminDB_th.Setup(t => t.GetSelectedGroup(id)).Throws(new Exception());
            mockAdminDB_th.Setup(t => t.ThresholdExists(id)).Returns(true);
            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.GetThresholdGroupSetting(token, id);

            //Assert
            Assert.AreEqual(500, actual.StatusCode);
        }

        [Test]
        public void Setting_SaveThresholdSetting_TokenErrorTest()
        {
            var token = "test";
            ThresholdTemplate.GroupSetting thresholdInformation = new ThresholdTemplate.GroupSetting() { Id = 1, Enable = true};

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SaveThresholdSetting(token, thresholdInformation);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_SaveThresholdSetting_NotAdminTest()
        {
            var token = "test";
            ThresholdTemplate.GroupSetting thresholdInformation = new ThresholdTemplate.GroupSetting() { Id = 1, Enable = true };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(emp => emp.CheckAdmin("test")).Returns(false);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SaveThresholdSetting(token, thresholdInformation);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_SaveThresholdSetting_SuccessTest()
        {
            var token = "test";
            ThresholdTemplate.GroupSetting thresholdInformation = new ThresholdTemplate.GroupSetting() { Id = 1, Enable = true, Selected = new SelectOptionTemplate[] { new SelectOptionTemplate {Id = 1, Name = "test"} } };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(emp => emp.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(th => th.Save(thresholdInformation)).Returns(true);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SaveThresholdSetting(token, thresholdInformation);

            //Assert
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Setting_SaveThresholdSetting_GroupNotFoundTest()
        {
            var token = "test";
            ThresholdTemplate.GroupSetting thresholdInformation = new ThresholdTemplate.GroupSetting() { Id = 1, Enable = true, Selected = new SelectOptionTemplate[] { new SelectOptionTemplate { Id = 1, Name = "test" } } };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            Exception exc = new Exception("FK_Branch_To_ThresholdBranchList");

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(emp => emp.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(th => th.Save(thresholdInformation)).Throws(new DbUpdateException("", exc));

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SaveThresholdSetting(token, thresholdInformation);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);

        }

        [Test]
        public void Setting_SaveThresholdSetting_ThresholdNotFoundTest()
        {
            var token = "test";
            ThresholdTemplate.GroupSetting thresholdInformation = new ThresholdTemplate.GroupSetting() { Id = 1, Enable = true, Selected = new SelectOptionTemplate[] { new SelectOptionTemplate { Id = 1, Name = "test" } } };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(emp => emp.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(th => th.Save(thresholdInformation)).Returns(false);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SaveThresholdSetting(token, thresholdInformation);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_SaveThresholdSetting_SaveFailTest()
        {
            var token = "test";
            ThresholdTemplate.GroupSetting thresholdInformation = new ThresholdTemplate.GroupSetting() { Id = 1, Enable = true, Selected = new SelectOptionTemplate[] { new SelectOptionTemplate { Id = 1, Name = "test" } } };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(b => b.Save(thresholdInformation)).Throws(new DbUpdateException("", new Exception()));

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SaveThresholdSetting(token, thresholdInformation);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_SaveThresholdSetting_ExceptionTest()
        {
            var token = "test";
            ThresholdTemplate.GroupSetting thresholdInformation = new ThresholdTemplate.GroupSetting() { Id = 1, Enable = true, Selected = new SelectOptionTemplate[] { new SelectOptionTemplate { Id = 1, Name = "test" } } };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(b => b.Save(thresholdInformation)).Throws(new Exception());

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SaveThresholdSetting(token, thresholdInformation);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_SaveThresholdSetting_InputNullTest()
        {
            var token = "test";
            ThresholdTemplate.GroupSetting thresholdInformation = null;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_emp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_th.Setup(t => t.Save(thresholdInformation)).Throws(new Exception());

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SaveThresholdSetting(token, thresholdInformation);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_ThresholdSetting_TokenErrorTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.ThresholdSetting(token);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_ThresholdSetting_NotFoundTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_th.Setup(s => s.GetSetting()).Returns(new object[] { });

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            StatusCodeResult actual = (StatusCodeResult)_target.ThresholdSetting(token);

            //Assert
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Setting_ThresholdSetting_SuccessTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_th.Setup(s => s.GetSetting()).Returns(new object[] { new {
            } });

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.ThresholdSetting(token);

            //Assert
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Setting_ThresholdSetting_ExceptionTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IDataDBDispatcher> mockDataDB = new Mock<IDataDBDispatcher>();
            Mock<IData> mockAdminDB_data = new Mock<IData>();
            Mock<IThreshold> mockAdminDB_th = new Mock<IThreshold>();
            Mock<IEmployee> mockAdminDB_emp = new Mock<IEmployee>();
            Mock<IBranch> mockAdminDB_group = new Mock<IBranch>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_th.Setup(s => s.GetSetting()).Throws(new Exception());

            var _target = new SettingController(mockRC.Object, mockDataDB.Object, mockAdminDB_th.Object, mockAdminDB_data.Object, mockAdminDB_emp.Object, mockAdminDB_group.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.ThresholdSetting(token);

            //Assert
            Assert.AreEqual(500, actual.StatusCode);
        }

    }
}
