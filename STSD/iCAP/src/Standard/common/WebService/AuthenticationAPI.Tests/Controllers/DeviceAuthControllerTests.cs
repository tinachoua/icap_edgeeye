using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using ShareLibrary.Interface;
using ShareLibrary.AdminDB;
using AuthenticationAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using ShareLibrary;

namespace AuthenticationAPI.Tests.Controllers
{
    class DeviceAuthControllerTests
    {
        ///<summary>
        ///1. device certificate != null
        ///2. over 7 days
        ///3. device == null
        ///4. 404 not found
        /// </summary>

        [Test]

        public void DeviceAuth_GetID_NotFoundTest()
        {
            var Thumbprint = "test";
            
            //Arrange
            var mockAdminDB_device = new Mock<IDevice>();
            var mockAdminDB_licenselist = new Mock<ILicenselist>();
            var mockAdminDB_devicecertificate = new Mock<IDevicecertificate>();
            var mockRC = new Mock<IRedisCacheDispatcher>();

            Devicecertificate devCert = new Devicecertificate()
            {
                Id = 1,
                CreatedDate = Convert.ToDateTime("2017-10-25 06:42:02"),
                DeletedFlag = false,
                DeviceId = 1,
                ExpiredDate = Convert.ToDateTime("2017-10-25 14:41:43"),
                LastModifiedDate = Convert.ToDateTime("2017-10-25 6:42:02"),
                Password = "123",
                Thumbprint= "RGV2aWNlMDAwMDE="
            };

            mockAdminDB_devicecertificate.Setup(d => d.Get(Thumbprint)).Returns(devCert);
            mockAdminDB_device.Setup(d => d.Get(devCert.Id)).Returns((Device)null);

            var _target = new DeviceAuthController(mockRC.Object, mockAdminDB_device.Object,mockAdminDB_devicecertificate.Object ,mockAdminDB_licenselist.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetID(Thumbprint);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        ///<summary>
        ///1. device certification ==null
        ///2. over limitation.
        ///3. status code is 403
        ///</summary>

        [Test]
        public void DeviceAuth_GetId_OverLimitationTest()
        {
            var Thumbprint = "test";

            //Arrange
            var mockAdminDB_device = new Mock<IDevice>();
            var mockAdminDB_licenselist = new Mock<ILicenselist>();
            var mockAdminDB_devicecertificate = new Mock<IDevicecertificate>();
            var mockRC = new Mock<IRedisCacheDispatcher>();

            //Devicecertificate devCert = new Devicecertificate()
            //{
            //    Id = 1,
            //    CreatedDate = Convert.ToDateTime("2017-10-25 06:42:02"),
            //    DeletedFlag = false,
            //    DeviceId = 1,
            //    ExpiredDate = Convert.ToDateTime("2017-10-25 14:41:43"),
            //    LastModifiedDate = Convert.ToDateTime("2017-10-25 6:42:02"),
            //    Password = "123",
            //    Thumbprint = "RGV2aWNlMDAwMDE="
            //};

            mockAdminDB_devicecertificate.Setup(d => d.Get(Thumbprint)).Returns((Devicecertificate)null);
            mockAdminDB_licenselist.Setup(l => l.OverLimitation()).Returns(true);

            var _target = new DeviceAuthController(mockRC.Object, mockAdminDB_device.Object, mockAdminDB_devicecertificate.Object, mockAdminDB_licenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetID(Thumbprint);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        ///<summary>
        ///1. device certification!=null
        ///2. not over 7 days
        ///3. device != null
        ///4. Get Id Success.
        ///</summary>

        [Test]
        public void Device_AuthGetID_SuccessOneTest()
        {
            var Thumbprint = "test";

            //Arrange
            var mockAdminDB_device = new Mock<IDevice>();
            var mockAdminDB_licenselist = new Mock<ILicenselist>();
            var mockAdminDB_devicecertificate = new Mock<IDevicecertificate>();
            var mockRC = new Mock<IRedisCacheDispatcher>();

            Devicecertificate devCert = new Devicecertificate()
            {
                Id = 26,
                CreatedDate = Convert.ToDateTime("2018-03-12 10:25:12"),
                DeletedFlag = false,
                DeviceId = 1,
                ExpiredDate = Convert.ToDateTime("2018-03-19 18:25:14"),
                LastModifiedDate = Convert.ToDateTime("2018-03-12 18:28:14"),
                Password = "123",
                Thumbprint = "RGV2aWNlMDAwMDE="
            };

            mockAdminDB_devicecertificate.Setup(d => d.Get(Thumbprint)).Returns(devCert);

            Device dev = new Device() { Id=26,Name="device00026"};

            mockAdminDB_device.Setup(d => d.Get(devCert.Id)).Returns(dev);
            mockAdminDB_devicecertificate.Setup(d => d.Update(devCert));

            string DeviceName = dev.Name;
            string GenPassword = devCert.Password;

            mockRC.Setup(r => r.AddDevice(DeviceName));
            mockRC.Setup(r => r.SendPWD(DeviceName, GenPassword));

            var _target = new DeviceAuthController(mockRC.Object, mockAdminDB_device.Object, mockAdminDB_devicecertificate.Object, mockAdminDB_licenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetID(Thumbprint);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        ///<summary>
        ///1. device certification=null
        ///2. not over limitation
        ///3. create one
        ///4. GetId Success
        /// </summary>

        [Test]
        public void Device_AuthGetID_SuccessTwoTest()
        {
            var Thumbprint = "test";

            //Arrange
            var mockAdminDB_device = new Mock<IDevice>();
            var mockAdminDB_licenselist = new Mock<ILicenselist>();
            var mockAdminDB_devicecertificate = new Mock<IDevicecertificate>();
            var mockRC = new Mock<IRedisCacheDispatcher>();

            mockAdminDB_devicecertificate.Setup(d => d.Get(Thumbprint)).Returns((Devicecertificate)null);
            mockAdminDB_licenselist.Setup(l => l.OverLimitation()).Returns(false);
            mockAdminDB_device.Setup(d => d.Count()).Returns(28);

            String DeviceName = string.Format("Device{0:00000}", 28 + 1);
            
            mockAdminDB_device.Setup(d => d.Get(DeviceName)).Returns((Device)null);

            Device dev = new Device() {  Name = DeviceName, DeviceClassId=1, OwnerId=1};

            mockAdminDB_device.Setup(d => d.Create(dev));

            string GenPassword = CommonFunctions.CreateRandomPassword(16);

            Devicecertificate devCert = new Devicecertificate()
            {
                DeviceId = dev.Id,
                Thumbprint = Thumbprint,
                Password = GenPassword,
                ExpiredDate = DateTime.UtcNow.AddDays(7)
            };

            mockAdminDB_devicecertificate.Setup(d => d.Create(devCert));
            mockRC.Setup(r => r.AddDevice(DeviceName));
            mockRC.Setup(r => r.SendPWD(DeviceName, GenPassword));

            var _target = new DeviceAuthController(mockRC.Object, mockAdminDB_device.Object, mockAdminDB_devicecertificate.Object, mockAdminDB_licenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetID(Thumbprint);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void DeviceAuth_GetDeviceList_TokenError()
        {
            var token = "";

            //Arrange
            var mockAdminDB_device = new Mock<IDevice>();
            var mockAdminDB_licenselist = new Mock<ILicenselist>();
            var mockAdminDB_devicecertificate = new Mock<IDevicecertificate>();
            var mockRC = new Mock<IRedisCacheDispatcher>();

            var _target = new DeviceAuthController(mockRC.Object, mockAdminDB_device.Object, mockAdminDB_devicecertificate.Object, mockAdminDB_licenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetDeviceList(token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void DeviceAuth_GetDeviceList_GetSuccess()
        {
            var token = "innodisk1234";

            //Arrange
            var mockAdminDB_device = new Mock<IDevice>();
            var mockAdminDB_licenselist = new Mock<ILicenselist>();
            var mockAdminDB_devicecertificate = new Mock<IDevicecertificate>();
            var mockRC = new Mock<IRedisCacheDispatcher>();

            mockAdminDB_licenselist.Setup(l => l.GetDeviceListToSetOffline()).Returns(new string[] { });

            var _target = new DeviceAuthController(mockRC.Object, mockAdminDB_device.Object, mockAdminDB_devicecertificate.Object, mockAdminDB_licenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetDeviceList(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void DeviceAuth_GetDeviceList_Exception()
        {
            var token = "innodisk1234";

            //Arrange
            var mockAdminDB_device = new Mock<IDevice>();
            var mockAdminDB_licenselist = new Mock<ILicenselist>();
            var mockAdminDB_devicecertificate = new Mock<IDevicecertificate>();
            var mockRC = new Mock<IRedisCacheDispatcher>();

            mockAdminDB_licenselist.Setup(l => l.GetDeviceListToSetOffline()).Throws(new Exception());

            var _target = new DeviceAuthController(mockRC.Object, mockAdminDB_device.Object, mockAdminDB_devicecertificate.Object, mockAdminDB_licenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetDeviceList(token);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }

    }
}
