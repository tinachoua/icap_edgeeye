using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using NUnit.Framework;
using ShareLibrary.Interface;
using ShareLibrary.DataTemplate;
using Moq;
using DeviceAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using ShareLibrary.AdminDB;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using DeviceAPI.Models.Remote;
using Microsoft.AspNetCore.Http;

namespace DeviceAPI.Tests.Controllers
{
    class DeviceControllerTests
    {
        [Test]
        public void Device_Get_TokenErrorTest()
        {
            var token = "test";
            var devName = "device00001";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Get(token, devName);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Device_Get_NotFoundTest()
        {
            var token = "test";
            var devName = "device00001";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.GetDeviceProfile(devName)).Returns((DeviceProfileTemplate)null);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Get(token, devName);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Device_Get_Success()
        {
            var token = "test";
            var devName = "device00001";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.GetDeviceProfile(devName)).Returns(new DeviceProfileTemplate());

            var bdoc = new BsonDocument()
            {
                {"MEM",new BsonDocument()
                       {
                           { "Cap",100}
                       }
                },
                {"Sys",new BsonDocument()
                       {
                           { "Longitude",100},
                           { "Latitude",100}
                       }
                }
            };


            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", devName))).Returns(bdoc);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Get(token, devName);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Device_Update_TokenErrorTest()
        {
            var token = "test";
            DeviceProfileTemplate devProfile = new DeviceProfileTemplate();
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, devProfile);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Device_Update_NotFoundTest()
        {
            var token = "test";
            DeviceProfileTemplate devProfile = new DeviceProfileTemplate()
            {
                Id = 1,
                DevName="device00001",
                Alias=null,
                Longitude=100,
                Latitude=100,
                PhotoURL=null,
                OwnerName=null
            };
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.UpdateDeviceProfile(devProfile)).Returns((bool?)null);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, devProfile);

            //Assert          
            Assert.AreEqual(304, actual.StatusCode);
        }

        [Test]
        public void Device_Update_FailTest()
        {
            var token = "test";
            DeviceProfileTemplate devProfile = new DeviceProfileTemplate()
            {
                Id = 1,
                DevName = "device00001",
                Alias = null,
                Longitude = 100,
                Latitude = 100,
                PhotoURL = null,
                OwnerName = null
            };
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.UpdateDeviceProfile(devProfile)).Returns(false);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, devProfile);

            //Assert          
            Assert.AreEqual(304, actual.StatusCode);
        }

        [Test]
        public void Device_Update_TrueTest()
        {
            var token = "test";
            DeviceProfileTemplate devProfile = new DeviceProfileTemplate()
            {
                Id = 1,
                DevName = "device00001",
                Alias = null,
                Longitude = 100,
                Latitude = 100,
                PhotoURL = null,
                OwnerName = null
            };
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.UpdateDeviceProfile(devProfile)).Returns(true);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, devProfile);

            //Assert          
            Assert.AreEqual(202, actual.StatusCode);
        }

        [Test]
        public void Device_Delete_TokenErrorTest()
        {
            var token = "test";
            string devname = "divice00001";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(token, devname);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Device_Delete_NotFoundTest()
        {
            var token = "test";
            string devname = "divice00001";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.Delete(devname)).Returns(false);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(token, devname);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }


        [Test]
        public void Device_Delete_DeviceOnlineTest()
        {
            var token = "test";
            string devname = "divice00001";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.Delete(devname)).Throws(new Exception());
            mockRC.Setup(t => t.GetStatus(devname)).Returns("1");

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(token, devname);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Device_Delete_FailTest()
        {
            var token = "test";
            string devname = "divice00001";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.Delete(devname)).Throws(new Exception());

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(token, devname);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Device_Delete_SuccessTest()
        {
            var token = "test";
            string devname = "divice00001";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.Delete(devname)).Returns(true);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(token, devname);

            //Assert          
            Assert.AreEqual(202, actual.StatusCode);
        }

        [Test]
        public void Device_Remote_TokenErrorTest()
        {
            var token = "test";
            RemoteCommand remoteCmd = new RemoteCommand()
            {
                devName ="device00001",
                target="test",
                cmd="test"
            };
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Remote(token, remoteCmd);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Device_Remote_NotFoundTest()
        {
            var token = "test";
            RemoteCommand remoteCmd = new RemoteCommand()
            {
                devName = "device00001",
                target = "test",
                cmd = "test"
            };
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.Get(remoteCmd.devName)).Returns((Device)null);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Remote(token, remoteCmd);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Device_Remote_SuccessTest()
        {
            var token = "test";
            RemoteCommand remoteCmd = new RemoteCommand()
            {
                devName = "device00001",
                target = "test",
                cmd = "test"
            };
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.Get(remoteCmd.devName)).Returns(new Device());
            mockRemote.Setup(r => r.SendRemoteCommand(remoteCmd.devName, remoteCmd.target, remoteCmd.cmd));

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.Remote(token, remoteCmd);

            //Assert          
            Assert.AreEqual(202, actual.StatusCode);
        }

        [Test]
        public void Device_GetImg_TokenErrorTest()
        {
            string token = "test";
            string devId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetImg(token, devId);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Device_GetImg_SuccessTest()
        {
            var token = "test";
            string devName = "device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.GetImgBase64(devName, "devices")).Returns("test");

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetImg(token, devName);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Device_GetImg_ExceptionTest()
        {
            var token = "test";
            string devName = "device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.GetImgBase64(devName, "devices")).Throws(new Exception());

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetImg(token, devName);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Device_UploadImg_TokenErrorTest()
        {
            var token = "test";
            List<IFormFile> files= new List<IFormFile> ();
            string overwrite="1";
            string devId = "1";
            
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, devId, overwrite, files);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Device_UploadImg_UnacceptableFileExtensionsTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "1";
            string devId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.AllowedFileExtensions(files)).Returns(false);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            ////Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, devId, overwrite, files);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Device_UploadImg_FileExistsTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "0";
            string devName = "device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.AllowedFileExtensions(files)).Returns(true);
            mockAdminDB_device.Setup(d => d.UploadImg(files, !(Int32.Parse(overwrite)).Equals(0), devName, "devices")).Returns(false);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, devName, overwrite, files);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Device_UploadImg_SuccessTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "1";
            string devName = "device00001";
            //Mock File
            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(s => s.OpenReadStream()).Returns(ms);
            fileMock.Setup(s => s.FileName).Returns(fileName);
            fileMock.Setup(s => s.Length).Returns(ms.Length);

            var file = fileMock.Object;
            files.Add(file);

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.AllowedFileExtensions(files)).Returns(true);
            mockAdminDB_device.Setup(d => d.UploadImg(files, !(Int32.Parse(overwrite)).Equals(0), devName, "devices")).Returns(true);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, devName, overwrite, files);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Device_UploadImg_ExceptionTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "1";
            string devName = "device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.AllowedFileExtensions(files)).Returns(true);
            mockAdminDB_device.Setup(d => d.UploadImg(files, !(Int32.Parse(overwrite)).Equals(0), devName, "devices")).Throws(new Exception());

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, devName, overwrite, files);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Device_GetDeviceList_TokenErrorTest()
        {
            var token = "test";
            
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetDeviceList(token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Device_GetDeviceList_SuccessTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.GetDeviceList()).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate { Id=1, Name="device00001" } });

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetDeviceList(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Device_GetDeviceList_SuccessNotFoundTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.GetDeviceList()).Returns((SelectOptionTemplate[])null);

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetDeviceList(token);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Device_GetDeviceList_UnexceptedErrorTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockRemote = new Mock<IRemoteCommandSender>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.GetDeviceList()).Throws(new Exception());

            var _target = new DeviceController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockRemote.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetDeviceList(token);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }
    } 
}
