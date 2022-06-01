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
using ShareLibrary.AdminDB;
using Microsoft.EntityFrameworkCore;

namespace DashboardAPI.Tests.Controllers
{
    class EventControllerTests
    {
        /// <summary>
        /// 2. GetAllAPI Test
        /// </summary>

        [Test]
        public void Event_GetAll_TokenErrorTest()
        {   
            var token = "test";
            int count = 1;
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetAll(token, count);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_GetAll_SuccessTest()
        {
            var token = "test";
            int count = 1;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var bd = new BsonDocument
            {
                 { "Dev", "device00001" },
                 { "_id",ObjectId.Parse("59f032455d9e0d131cb338ed") },
                 {"Message","Device00001 offline" },
                 { "Checked", true},
                 {"Level", 0},
                 {"Time",1506993684.0 }
            };
            List<BsonDocument> bson = new List<BsonDocument>();

            bson.Add(bd);

            mockDataDB.Setup(d => d.GetRawData("EventLog", null, count)).Returns(bson);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetAll(token, count);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        /// <summary>
        /// 3. GetNewAPI Test
        /// </summary>

        [Test]
        public void Event_GetNew_TokenErrorTest()
        {
            var token = "test";
            int count = 1;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetNew(token, count);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_GetNew_SuccessTest()
        {
            var token = "test";
            int count = 1;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var bd = new BsonDocument
            {
                 { "Dev", "Device00001" },
                 { "_id",ObjectId.Parse("59f032455d9e0d131cb338ed") },
                 {"Message","Device00001 offline" },
                 { "Checked", false},
                 {"Level", 0},
                 {"Time",1506993684.0 }
            };
            List<BsonDocument> bson = new List<BsonDocument>();
            Dictionary<string, string> dev_alias = new Dictionary<string, string>()
            {
                {"Device00001", "test" }
            };
            Dictionary<string, string> dev_owner = new Dictionary<string, string>()
            {
                {"Device00001", "test"}
            };

            bson.Add(bd);
            
            mockDataDB.Setup(d => d.GetRawData("EventLog", "{\"Checked\":false}", count)).Returns(bson);
            mockAdminDB_device.Setup(a => a.GetAlias_All()).Returns(dev_alias);
            mockAdminDB_device.Setup(a => a.GetOwner_All()).Returns(dev_owner);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetNew(token, count);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        /// <summary>
        /// 4. GetDoneAPI Test
        /// </summary>

        [Test]
        public void Event_GetDone_TokenErrorTest()
        {
            var token = "test";
            int count = 1;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetDone(token, count);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_GetDone_SuccessTest()
        {
            var token = "test";
            int count = 1;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");           

            var bd = new BsonDocument
            {
                 { "Dev", "Device00001" },
                 { "_id",ObjectId.Parse("59f032455d9e0d131cb338ed") },
                 {"Message","Device00001 offline" },
                 { "Checked", true},
                 //{"Level", 0},
                 {"Time",1506993684.0 }                 
            };
            List<BsonDocument> bson = new List<BsonDocument>();

            bson.Add(bd);
            Dictionary<string, string> myDic = new Dictionary<string, string>();

            myDic.Add("Device00001", "test");
            mockDataDB.Setup(d => d.GetRawData("EventLog", "{\"Checked\":true}", count)).Returns(bson);
            mockAdminDB_device.Setup(a => a.GetAlias_All()).Returns(myDic);
            mockAdminDB_device.Setup(a => a.GetOwner_All()).Returns(myDic);
            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetDone(token, count);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        /// <summary>
        /// 5. Get UpdateAPI Test
        /// </summary>


        [Test]
        public void Event_Update_TokenErrorTest()
        {
            var token = "test";
            EventDataTemplate eventData = new EventDataTemplate();

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token,eventData);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_Update_FalseTest()
        {
            string token= "test";
            EventDataTemplate eventData = new EventDataTemplate();

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();          
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockDataDB.Setup(d => d.UpdateEventlog(eventData)).Returns(false);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, eventData);
            //Assert          
            Assert.AreEqual(406, actual.StatusCode);
        }

        [Test]
        public void Event_Update_SuccessTest()
        {
            string token = "test";
            EventDataTemplate eventData = new EventDataTemplate();

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockDataDB.Setup(d => d.UpdateEventlog(eventData)).Returns(true);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, eventData);
            //Assert          
            Assert.AreEqual(202, actual.StatusCode);
        }

        /// <summary>
        /// 6. SetEmailAPI Test
        /// </summary>

        [Test]
        public void  Event_SetEmail_TokenErrorTest()
        {
            string token = "test";
            EmailSettingTemplate payload = new EmailSettingTemplate();

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns((string)null);
            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetEmail(token, payload);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_SetEmail_PayloadNullTest()
        {
            string token = "test";
            EmailSettingTemplate payload = null;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(c => c.CteateOrUpdate(null)).Throws(new ArgumentException());
            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetEmail(token, payload);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event_SetEmail_BadRequestTest()
        {
            string token = "test";
            EmailSettingTemplate payload = new EmailSettingTemplate()
            {
                SMTPAddress = "mail.innodisk.com",
                PortNumber = 25,
                EnableSSL = false,
                //emailFrom = "test@innodisk.com",
                Password = "test",
                Enable = true,
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetEmail(token, payload);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event_SetEmail_NoAuthorizationTest()
        {
            string token = "test";
            EmailSettingTemplate payload = new EmailSettingTemplate()
            {
                SMTPAddress = "mail.innodisk.com",
                PortNumber = 25,
                EnableSSL = false,
                EmailFrom = "test@innodisk.com",
                Password = "test",
                Enable = true,
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetEmail(token, payload);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_SetEmail_SuccessTest()
        {
            string token = "test";
            EmailSettingTemplate payload = new EmailSettingTemplate()
            {
                SMTPAddress = "mail.innodisk.com",
                PortNumber = 25,
                EnableSSL = false,
                EmailFrom = "test@innodisk.com",
                Password = "test",
                Enable = true,
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.CteateOrUpdate(payload)).Returns(true);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetEmail(token, payload);
            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Event_SetEmail_EmailFromNullTest()
        {
            string token = "test";
            EmailSettingTemplate payload = new EmailSettingTemplate()
            {
                SMTPAddress = "mail.innodisk.com",
                PortNumber = 25,
                EnableSSL = false,
                EmailFrom = "test@innodisk.com",
                Password = "test",
                Enable = true,
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            Exception exc = new Exception("emailFrom can not be null");

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.CteateOrUpdate(payload)).Throws(new DbUpdateException("emailFrom can not be null", exc));

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetEmail(token, payload);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event_SetEmail_SetFailTest()
        {
            string token = "test";
            EmailSettingTemplate payload = new EmailSettingTemplate()
            {
                SMTPAddress = "mail.innodisk.com",
                PortNumber = 25,
                EnableSSL = false,
                EmailFrom = "test@innodisk.com",
                Password = "test",
                Enable = true,
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            Exception exc = new Exception("Set Fail");

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.CteateOrUpdate(payload)).Throws(new DbUpdateException("Set Fail", exc));

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetEmail(token, payload);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event_SetEmail_FailTest()
        {
            string token = "test";
            EmailSettingTemplate payload = new EmailSettingTemplate()
            {
                SMTPAddress = "mail.innodisk.com",
                PortNumber = 25,
                //enableSSL = false,
                EmailFrom = "test@innodisk.com",
                Password = "test",
                Enable = true,
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.CteateOrUpdate(payload)).Returns(false);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetEmail(token, payload);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event_SetEmail_ExceptionTest()
        {
            string token = "test";
            EmailSettingTemplate payload = new EmailSettingTemplate()
            {
                SMTPAddress = "mail.innodisk.com",
                PortNumber = 25,
                EnableSSL = false,
                EmailFrom = "test@innodisk.com",
                Password = "test",
                Enable = true,
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.CteateOrUpdate(payload)).Throws(new Exception());

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetEmail(token, payload);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        /// <summary>
        /// 7. GetEmailListAPI Test
        /// </summary>

        [Test]
        public void Event_GetEmailList_TokenErrorTest()
        {
            string token = "test";
            int? CompanyId = 1;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns((string)null);
            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmailList(CompanyId, token);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_GetEmailList_CompanyIdErrorTest()
        {
            string token = "test";
            int? CompanyId = null;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");            

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmailList(CompanyId, token);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event_GetEmailList_NotFoundTest()
        {
            string token = "test";
            int? CompanyId = 2;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_email.Setup(e => e.GetEmailList(CompanyId)).Returns((List<EmailSettingTemplate>)null);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmailList(CompanyId, token);
            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Event_GetEmailList_SuccessTest()
        {
            string token = "test";
            int? CompanyId = 1;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_email.Setup(e => e.GetEmailList(CompanyId)).Returns(new List<EmailSettingTemplate>());

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmailList(CompanyId, token);
            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        /// <summary>
        /// 8. DeleteEmailAPI Test
        /// </summary>


        [Test]
        public void Event_DeleteEmail_TokenErrorTest()
        {
            string token = "test";
            string emailFrom = "test@test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns((string)null);
            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.DeleteEmail(token, emailFrom);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_DeleteEmail_NoAuthorizationTest()
        {
            string token = "test";
            string emailFrom = "test@test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.DeleteEmail(token, emailFrom);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }


        [Test]
        public void Event_DeleteEmail_NotFoundTest()
        {
            string token = "test";
            string emailFrom = "test@test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.Delete(emailFrom)).Returns((bool?)null);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.DeleteEmail(token, emailFrom);
            //Assert          
            Assert.AreEqual(304, actual.StatusCode);
        }

        [Test]
        public void Event_DeleteEmail_SuccessTest()
        {
            string token = "test";
            string emailFrom = "test@test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.Delete(emailFrom)).Returns(true);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.DeleteEmail(token, emailFrom);
            //Assert          
            Assert.AreEqual(202, actual.StatusCode);
        }

        [Test]
        public void Event_DeleteEmail_ExceptionTest()
        {
            string token = "test";
            string emailFrom = "test@test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.Delete(emailFrom)).Returns(false);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.DeleteEmail(token, emailFrom);
            //Assert          
            Assert.AreEqual(304, actual.StatusCode);
        }

        /// <summary>
        /// 9. SendEmailAPI Test
        /// </summary>

        [Test]
        public void Event_SendEmail_TokenErrorTest()
        {
            string token = "test";
            EmailSendingInfoTemplate payload=null;           

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns((string)null);
            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SendEmail(token, payload);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_SendEmail_PayloadNullTest()
        {
            string token = "test";
            EmailSendingInfoTemplate payload = null;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");           

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SendEmail(token, payload);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event_SendEmail_PayloadDeviceNameNullTest()
        {
            string token = "test";
            EmailSendingInfoTemplate payload = new EmailSendingInfoTemplate()
            {
                deviceName=null,
                Class="test",
                Info="test"
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");  

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SendEmail(token, payload);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event_SendEmail_PayloadDeviceDataNotFoundTest()
        {
            string token = "test";
            EmailSendingInfoTemplate payload = new EmailSendingInfoTemplate()
            {
                deviceName = "device80000",
                Class = "test",
                Info = "test"
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_email.Setup(e => e.GetOwnerId(payload.deviceName)).Returns((int?)null);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SendEmail(token, payload);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }


        [Test]
        public void Event_SendEmail_NoSettingOrEnableFalseTest()
        {
            string token = "test";
            EmailSendingInfoTemplate payload = new EmailSendingInfoTemplate()
            {
                deviceName="device00001",
                Class="test",
                Info="test"
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_email.Setup(e => e.GetOwnerId(payload.deviceName)).Returns(1);
            mockAdminDB_email.Setup(e => e.Send(payload, 1)).Returns(false);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SendEmail(token, payload);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event_SendEmail_ExceptionTest()
        {
            string token = "test";
            EmailSendingInfoTemplate payload = new EmailSendingInfoTemplate()
            {
                deviceName = "device00001",
                Class = "Storage",
                Info = "Storage 0 temperature over thershold, value : 77 celsius."
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_email.Setup(e => e.GetOwnerId(payload.deviceName)).Returns(1);
            mockAdminDB_email.Setup(e => e.Send(payload, 1)).Throws(new Exception());

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SendEmail(token, payload);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event_SendEmail_SuccessTest()
        {
            string token = "test";
            EmailSendingInfoTemplate payload = new EmailSendingInfoTemplate()
            {
                deviceName = "device00001",
                Class = "Storage",
                Info = "Storage 0 temperature over thershold, value : 77 celsius."
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_email.Setup(e => e.GetOwnerId(payload.deviceName)).Returns(1);
            mockAdminDB_email.Setup(e => e.Send(payload, 1)).Returns(true);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object,mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SendEmail(token, payload);
            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        ///<summary>
        /// 10. GetemailAPI Test.
        ///</summary>

        [Test]
        public void Event_GetEmail_TokenErrorTest()
        {
            string token = "test";
            string CompanyId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns((string)null);
            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmail(CompanyId, token);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_GetEmail_NoAuthorizationTest()
        {
            string token = "test";
            string ConpanyId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmail(ConpanyId, token);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_GetEmail_CompanyIdErrorTest()
        {
            string token = "test";
            string CompanyId ="abc";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmail(CompanyId, token);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }       

        [Test]
        public void Event_GetEmail_NotFoundTest()
        {
            string token = "test";
            string CompanyId = "2";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.GetEmail(Int32.Parse(CompanyId))).Returns((Email)null);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmail(CompanyId, token);
            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Event_GetEmail_SuccessTest()
        {
            string token = "test";
            string CompanyId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.GetEmail(Int32.Parse(CompanyId))).Returns(new Email());

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmail(CompanyId, token);
            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Event_GetEmail_ExceptionTest()
        {
            string token = "test";
            string CompanyId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.GetEmail(Int32.Parse(CompanyId))).Throws(new Exception());

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmail(CompanyId, token);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        ///<summary>
        /// 11. GetEmployeeEmailListAPI test.
        ///</summary>

        [Test]
        public void Event_GetEmployeeEmailList_TokenErrorTest()
        {
            string token = "test";
            string CompanyId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns((string)null);
            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmployeeEmailList(CompanyId, token);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_GetEmployeeEmailList_NoAuthorizationTest()
        {
            string token = "test";
            string ConpanyId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmployeeEmailList(ConpanyId, token);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_GetEmployeeEmailList_CompanyIdErrorTest()
        {
            string token = "test";
            string CompanyId = "abc";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmployeeEmailList(CompanyId, token);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event_GetEmployeeEmailList_NotFoundTest()
        {
            string token = "test";
            string CompanyId = "2";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.GetEmployeeEmailList(Int32.Parse(CompanyId))).Returns(new List<string>());

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmployeeEmailList(CompanyId, token);
            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Event_GetEmployeeEmailList_SuccessTest()
        {
            string token = "test";
            string CompanyId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.GetEmployeeEmailList(Int32.Parse(CompanyId))).Returns(new List<string>() { "test@gmail.com"});

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmployeeEmailList(CompanyId, token);
            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Event_GetEmployeeEmailList_ExceptionTest()
        {
            string token = "test";
            string CompanyId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(e => e.GetEmail(Int32.Parse(CompanyId))).Throws(new Exception());

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmail(CompanyId, token);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event__EmailTest_TokenErrorTest()
        {
            string token = "test";
            EmailTestTemplate emailTestInfo = new EmailTestTemplate()
            {
                SMTPAddress = "gmail.smtp.com",
                PortNumber = 587,
                EmailFrom = "guest@example.com",
                Password = "test",
                EnableSSL = true,
                EmailTo = "guest@example.com"
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns((string)null);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.EmailTest(token, emailTestInfo);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event__EmailTest_NotValidEmailAddressTest()
        {
            string token = "test";
            EmailTestTemplate emailTestInfo = new EmailTestTemplate()
            {
                SMTPAddress = "gmail.smtp.com",
                PortNumber = 587,
                EmailFrom = "guest@example.com",
                Password = "test",
                EnableSSL = true,
                EmailTo = "guest@example.com"
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_email.Setup(e => e.IsValidEmail(emailTestInfo.EmailTo)).Returns(false);            

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.EmailTest(token, emailTestInfo);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event__EmailTest_SuccessTest()
        {
            string token = "test";
            EmailTestTemplate emailTestInfo = new EmailTestTemplate()
            {
                SMTPAddress = "gmail.smtp.com",
                PortNumber = 587,
                EmailFrom = "guest@example.com",
                Password = "test",
                EnableSSL = true,
                EmailTo = "guest@example.com"
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_email.Setup(e => e.IsValidEmail(emailTestInfo.EmailTo)).Returns(true);
            mockAdminDB_email.Setup(e => e.IsValidMailServerData(emailTestInfo)).Returns(true);
            mockAdminDB_email.Setup(e => e.Send("test", emailTestInfo));

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.EmailTest(token, emailTestInfo);
            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Event__EmailTest_InValidEmailServerSettingTest()
        {
            string token = "test";
            EmailTestTemplate emailTestInfo = new EmailTestTemplate()
            {
                SMTPAddress = "gmail.smtp.com",
                PortNumber = 587,
                EmailFrom = "guest@example.com",
                Password = "test",
                EnableSSL = true,
                EmailTo = "guest@example.com"
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_email.Setup(e => e.IsValidEmail(emailTestInfo.EmailTo)).Returns(true);
            mockAdminDB_email.Setup(e => e.IsValidMailServerData(emailTestInfo)).Returns(false);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.EmailTest(token, emailTestInfo);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event__EmailTest_FailTest()
        {
            string token = "test";
            EmailTestTemplate emailTestInfo = new EmailTestTemplate()
            {
                SMTPAddress = "gmail.smtp.com",
                PortNumber = 587,
                EmailFrom = "guest@example.com",
                Password = "test",
                EnableSSL = true,
                EmailTo = "guest@example.com"
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_email.Setup(e => e.IsValidEmail(emailTestInfo.EmailTo)).Returns(true);
            mockAdminDB_email.Setup(e => e.IsValidMailServerData(emailTestInfo)).Returns(true);
            mockAdminDB_email.Setup(e => e.Send("test", emailTestInfo)).Throws(new Exception());

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.EmailTest(token, emailTestInfo);
            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Event_GetNewCount_TokenErrorTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetNewCount(token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Event_GetNewCount_SuccessTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockDataDB.Setup(d => d.GetRawDataCount("EventLog", "{\"Checked\":false}")).Returns(1);
            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetNewCount(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Event_GetNewCount_ExceptionTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_email = new Mock<IEmail>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockDataDB.Setup(d => d.GetRawDataCount("EventLog", "{\"Checked\":false}")).Throws(new Exception());
            var _target = new EventController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_email.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetNewCount(token);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }
    }
}
