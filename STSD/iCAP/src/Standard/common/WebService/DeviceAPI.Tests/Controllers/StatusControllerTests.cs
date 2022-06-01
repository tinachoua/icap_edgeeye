using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.Interface;
using Moq;
using NUnit.Framework;
using DeviceAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using ShareLibrary.AdminDB;
using DeviceAPI.Models.Status;

namespace DeviceAPI.Tests.Controllers
{
    class StatusControllerTests
    {
        [Test]
        public void Status_GetList_TokenErrorTest()
        {
            var token = "test";
            
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();            

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new StatusController(mockRC.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetList(token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Status_GetList_SuccessTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.GetList()).Returns(new List<string>());

            var _target = new StatusController(mockRC.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetList(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Status_Get_TokenErrorTest()
        {
            var token = "test";
            var device = "device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new StatusController(mockRC.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Get(token, device);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        // device not found
        public void Status_Get_NotFoundTest()
        {
            var token = "test";
            var device = "device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRC.Setup(t => t.GetStatus(device)).Returns((string)null);
            mockAdminDB_device.Setup(d => d.Get(device)).Returns((Device)null);

            var _target = new StatusController(mockRC.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Get(token, device);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        // Device status is null in the redis cache.
        public void Status_Get_SuccessOneTest()
        {
            var token = "test";
            var device = "device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRC.Setup(t => t.GetStatus(device)).Returns((string)null);
            mockAdminDB_device.Setup(d => d.Get(device)).Returns(new Device());

            var _target = new StatusController(mockRC.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Get(token, device);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        // devStatus != null in the Redis Cache
        public void Status_Get_SuccessTwoTest()
        {
            var token = "test";
            var device = "device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRC.Setup(t => t.GetStatus(device)).Returns("0");

            var _target = new StatusController(mockRC.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Get(token, device);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Status_Update_TokenErrorTest()
        {
            var token = "test";
            var device = new DeviceStatus()
                            {
                                DeviceName ="device00001",
                                Status =true
                            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);           

            var _target = new StatusController(mockRC.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, device);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Status_Update_NotFoundTest()
        {
            var token = "test";
            var device = new DeviceStatus()
            {
                DeviceName = "device00001",
                Status = true
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRC.Setup(t => t.GetStatus(device.DeviceName)).Returns((string)null);

            var _target = new StatusController(mockRC.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, device);

            //Assert          
            Assert.AreEqual(304, actual.StatusCode);
        }

        [Test]
        public void Status_Update_SetOnlineSuccessTest()
        {
            var token = "test";
            var device = new DeviceStatus()
            {
                DeviceName = "device00001",
                Status = true
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRC.Setup(t => t.GetStatus(device.DeviceName)).Returns("0");
            mockRC.Setup(t => t.SetStatus(device.DeviceName, 1));

            var _target = new StatusController(mockRC.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, device);

            //Assert          
            Assert.AreEqual(202, actual.StatusCode);
        }

        [Test]
        public void Status_Update_SetOfflineSuccessTest()
        {
            var token = "test";
            var device = new DeviceStatus()
            {
                DeviceName = "device00001",
                Status = false
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRC.Setup(t => t.GetStatus(device.DeviceName)).Returns("1");
            mockRC.Setup(t => t.SetStatus(device.DeviceName, 0));

            var _target = new StatusController(mockRC.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, device);

            //Assert          
            Assert.AreEqual(202, actual.StatusCode);
        }

        [Test]
        public void Status_CleanAllStatus_TokenErrorTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new StatusController(mockRC.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.CleanAllStatus(token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Status_CleanAllStatus_SuccessTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new StatusController(mockRC.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.CleanAllStatus(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }



    }
}
