using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DeviceAPI.Tests.Controllers
{
    class BranchControllerTests
    {
        [Test]
        public void Branch_GetList_TokenErrorTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetList(token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_GetList_SuccessTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(b => b.GetBranchList()).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate { Id=1,Name="test"} });

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetList(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Branch_GetList_BranchNotFoundTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(b => b.GetBranchList()).Returns(new SelectOptionTemplate[] {null });

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetList(token);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Branch_GetList_ExceptionTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(b => b.GetBranchList()).Throws(new Exception());

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetList(token);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }

        [Test]
        public void Branch_GetDevList_TokenErrorTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetDevList(token, branchId);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_GetDevList_DeviceNotFoundTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();
            List<Device> device=new List<Device>();
            device.Add(null);

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.GetList(Int32.Parse(branchId))).Returns(device);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetDevList(token, branchId);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Branch_GetDevList_SuccessTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();


            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            Device div = new Device()
            {
                Name = "divce00001"
            };

            List<Device> device = new List<Device>();
            device.Add(new Device { Name = "divce00001" });

            mockAdminDB_device.Setup(d => d.GetList(Int32.Parse(branchId))).Returns(device);

            var queryObj = new
            {
                Dev = "divce00001"
            };

            var doc = new BsonDocument
            {
                { "Checked", false },
                {"Message", "Storage 0 lifespan over thershold, value : 88 days." }
            };

            List<BsonDocument> dbRet = new List<BsonDocument>();
            dbRet.Add(doc);

            mockDataDB.Setup(d => d.GetRawData("EventLog", JsonConvert.SerializeObject(queryObj), 0)).Returns(dbRet);
            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetDevList(token, branchId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Branch_GetDevListByName_TokenErrorTest()
        {
            var token = "test";
            string devName = "device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetDevListByName(token, devName);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_GetDevListByName_SuccessTest()
        {
            var token = "test";
            string devName = "device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            Device dev = new Device
            {
                Name="device00001",                
            };
            List<Device> device = new List<Device>();

            device.Add(dev);
            mockAdminDB_device.Setup(d => d.Get(devName)).Returns(dev);
            mockAdminDB_device.Setup(d => d.GetList(1)).Returns(device);

            var bd = new BsonDocument
            {
                 { "Dev", "device00001" },
                 { "_id",ObjectId.Parse("59f032455d9e0d131cb338ed") },
                 {"Message","Device00001 offline" },
                 { "Checked", false},
                 {"Time",1506993684.0 }
            };
            List<BsonDocument> bson = new List<BsonDocument>();

            bson.Add(bd);
            var queryObj = new
            {
                Dev = dev.Name
            };

            mockDataDB.Setup(s => s.GetRawData("EventLog", JsonConvert.SerializeObject(queryObj), 0)).Returns(bson);


            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetDevListByName(token, devName);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Branch_GetDevListByName_DeviceNameError()
        {
            var token = "test";
            string devName = "device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");          
 
            mockAdminDB_device.Setup(d => d.Get(devName)).Returns((Device)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetDevListByName(token, devName);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Branch_GetBranchLoading_TokenErrorTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetBranchLoading(token, branchId);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_GetBranchLoading_DeviceNotFoundTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();
            List<Device> device = new List<Device>();
            device.Add(null);

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_device.Setup(d => d.GetList(Int32.Parse(branchId))).Returns(device);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetBranchLoading(token, branchId);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Branch_GetBranchLoading_SuccessOneTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            Device dev = new Device
            {
                Name = "device00001",                
            };
            List<Device> device = new List<Device>();

            device.Add(dev);
            mockAdminDB_device.Setup(d => d.GetList(Int32.Parse(branchId))).Returns(device);
            mockAdminDB_device.Setup(d => d.GetList(1)).Returns(device);

            var bdoc = new BsonDocument()
            {
                {"MEM",new BsonDocument { { "Cap",1} } }

            };
            var dybdoc = new BsonDocument()
            {
                {"CPU",new BsonDocument { { "Usage",100} } },
                { "memUsed", new BsonDocument()
                {
                    { "memUsed", 5939896},
                    { "temp", 30}
                } },
            };

            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", dev.Name))).Returns(bdoc);
            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-dynamic", dev.Name))).Returns(dybdoc);
            
            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetBranchLoading(token, branchId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Branch_GetBranchLoading_SuccessTwoTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            Device dev = new Device
            {
                Name = "device00001"   
            };
            List<Device> device = new List<Device>();

            device.Add(dev);
            mockAdminDB_device.Setup(d => d.GetList(Int32.Parse(branchId))).Returns(device);
            mockAdminDB_device.Setup(d => d.GetList(1)).Returns(device);

            var bdoc = new BsonDocument()
            {
                {"MEM",new BsonDocument { { "Cap",100} } }

            };
            var dybdoc = new BsonDocument()
            {
                {"CPU",new BsonDocument { { "Usage",100} } },
                { "MEM", new BsonDocument()
                    {
                        { "memUsed", 5939896},
                        { "temp", 30}
                    }
                },
            };

            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", dev.Name))).Returns(bdoc);
            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-dynamic", dev.Name))).Returns(dybdoc);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetBranchLoading(token, branchId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Branch_GetBranchLoading_SuccessThreeTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            Device dev = new Device
            {
                Name = "device00001"               
            };
            List<Device> device = new List<Device>();

            device.Add(dev);
            mockAdminDB_device.Setup(d => d.GetList(Int32.Parse(branchId))).Returns(device);
            mockAdminDB_device.Setup(d => d.GetList(1)).Returns(device);

            var bdoc = new BsonDocument()
            {
                {"MEM",new BsonDocument { { "Cap",100} } }

            };
            var dybdoc = new BsonDocument()
            {
                {"CPU",new BsonDocument { { "Usage",1} } },
                { "MEM", new BsonDocument()
                    {
                        { "memUsed", 5939896},
                        { "temp", 30}
                    }
                },
            };

            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", dev.Name))).Returns(bdoc);
            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-dynamic", dev.Name))).Returns(dybdoc);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetBranchLoading(token, branchId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Branch_GetBranchLoading_SuccessFourTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            Device dev = new Device
            {
                Name = "device00001",                
            };
            List<Device> device = new List<Device>();

            device.Add(dev);
            mockAdminDB_device.Setup(d => d.GetList(Int32.Parse(branchId))).Returns(device);
            mockAdminDB_device.Setup(d => d.GetList(1)).Returns(device);

            var bdoc = new BsonDocument()
            {
                {"MEM",new BsonDocument { { "Cap",100} } }

            };
            var dybdoc = new BsonDocument()
            {
                {"CPU",new BsonDocument { { "Usage",1} } },
                {"memUsed",1}
            };

            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-static", dev.Name))).Returns(bdoc);
            mockDataDB.Setup(d => d.GetLastRawData(string.Format("{0}-dynamic", dev.Name))).Returns(dybdoc);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetBranchLoading(token, branchId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Branch_GetImg_TokenErrorTest()
        {
            string token = "test";
            string braId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetImg(token, braId);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_GetImg_SuccessTest()
        {
            var token = "test";
            string braId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(d => d.GetImgBase64(braId,"branches")).Returns("test");

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetImg(token, braId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Branch_GetImg_ExceptionTest()
        {
            var token = "test";
            string braId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(d => d.GetImgBase64(braId,"branches")).Throws(new Exception());

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetImg(token, braId);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_UploadImg_TokenErrorTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "1";
            string braId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, braId, overwrite, files);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_UploadImg_UnacceptableFileExtensionsTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "1";
            string braId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(d => d.AllowedFileExtensions(files)).Returns(false);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            ////Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, braId, overwrite, files);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_UploadImg_FileExistsTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "0";
            string braId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(d => d.AllowedFileExtensions(files)).Returns(true);
            mockAdminDB_branch.Setup(d => d.UploadImg(files, !(Int32.Parse(overwrite)).Equals(0), braId, "branches")).Returns(false);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, braId, overwrite, files);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_UploadImg_SuccessTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "1";
            string braId = "1";
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
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(d => d.AllowedFileExtensions(files)).Returns(true);
            mockAdminDB_branch.Setup(d => d.UploadImg(files, !(Int32.Parse(overwrite)).Equals(0), braId,"branches")).Returns(true);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, braId, overwrite, files);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Branch_UploadImg_ExceptionTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "1";
            string braId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(d => d.AllowedFileExtensions(files)).Returns(true);
            mockAdminDB_branch.Setup(d => d.UploadImg(files, !(Int32.Parse(overwrite)).Equals(0), braId, "branches")).Throws(new Exception());

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, braId, overwrite, files);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Create_TokenErrorTest()
        {
            var token = "test";
            string branchName = "branch1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Create(token, branchName);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_Create_NotAdminTest()
        {
            var token = "test";
            string branchName = "branch1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);
            
            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Create(token, branchName);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_Create_ExistsTest()
        {
            var token = "test";
            string branchName = "branch1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.NameExists(branchName)).Returns(true);      
            mockAdminDB_branch.Setup(b => b.Create(branchName));

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Create(token, branchName);

            //Assert
            Assert.AreEqual(409, actual.StatusCode);
        }

        [Test]
        public void Branch_Create_SuccessTest()
        {
            var token = "test";
            string branchName = "branch1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.NameExists(branchName)).Returns(false);
            mockAdminDB_branch.Setup(b => b.Create(branchName));

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Create(token, branchName);

            //Assert
            Assert.AreEqual(201, actual.StatusCode);
        }

        [Test]
        public void Branch_Create_InputNullTest()
        {
            var token = "test";
            string branchName = null;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.NameExists(branchName)).Returns(false);
            mockAdminDB_branch.Setup(b => b.Create(branchName)).Throws(new DbUpdateException("", new Exception("The branch name can not be blank!")));

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Create(token, branchName);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Create_DBErrorTest()
        {
            var token = "test";
            string branchName = "branch 1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.NameExists(branchName)).Returns(false);
            mockAdminDB_branch.Setup(b => b.Create(branchName)).Throws(new DbUpdateException("", new Exception()));

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Create(token, branchName);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Create_CreateFailTest()
        {
            var token = "test";
            string branchName = "branch 1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.NameExists(branchName)).Returns(false);
            mockAdminDB_branch.Setup(b => b.Create(branchName)).Throws(new Exception());

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Create(token, branchName);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Rename_TokenErrorTest()
        {
            var token = "test";
            BranchTemplate branchInfo = new BranchTemplate() { Id=1, Name="test"};

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Rename(token, branchInfo);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_Rename_NotAdminTest()
        {
            var token = "test";
            BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "test" };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Rename(token, branchInfo);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_Rename_NameExistsTest()
        {
            var token = "test";
            BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "test" };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.NameExists(branchInfo.Name)).Returns(true);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Rename(token, branchInfo);

            //Assert
            Assert.AreEqual(409, actual.StatusCode);
        }

        [Test]
        public void Branch_Rename_BranchNameNullTest()
        {
            var token = "test";
            BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = null };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();         

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.NameExists(branchInfo.Name)).Returns(false);
            mockAdminDB_branch.Setup(b => b.Update(branchInfo)).Throws(new Exception());
            

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Rename(token, branchInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Rename_InputNullTest()
        {
            var token = "test";
            BranchTemplate branchInfo = null;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            //mockAdminDB_branch.Setup(b => b.NameExists(branchInfo.Name)).Returns(false);
            //mockAdminDB_branch.Setup(b => b.Update(branchInfo)).Throws(new Exception());


            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Rename(token, branchInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]        
        public void Branch_Update_RenameFailTest()
        {
            var token = "test";
            BranchTemplate branchInfo = new BranchTemplate() { Id=1 ,Name="test"};

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.NameExists(branchInfo.Name)).Returns(false);
            mockAdminDB_branch.Setup(b => b.Update(branchInfo)).Throws(new DbUpdateException("", new Exception()));


            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Rename(token, branchInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Update_RenameSuccessTest()
        {
            var token = "test";
            BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "test" };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.NameExists(branchInfo.Name)).Returns(false);
            mockAdminDB_branch.Setup(b => b.Update(branchInfo));


            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Rename(token, branchInfo);

            //Assert
            Assert.AreEqual(200, actual.StatusCode);
        }

        //[Test]
        //public void Branch_Update_TokenErrorTest()
        //{
        //    var token = "test";
        //    BranchTemplate updateBranch = new BranchTemplate() { Id=1, Name="test"};

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_device = new Mock<IDevice>();
        //    var mockDataDB = new Mock<IDataDBDispatcher>();
        //    var mockAdminDB_branch = new Mock<IBranch>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

        //    var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Update(token, updateBranch);

        //    //Assert
        //    Assert.AreEqual(403, actual.StatusCode);
        //}

        //[Test]
        //public void Branch_Update_NotAdminTest()
        //{
        //    var token = "test";
        //    BranchTemplate updateBranch = new BranchTemplate() { Id = 1, Name = "test" };

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_device = new Mock<IDevice>();
        //    var mockDataDB = new Mock<IDataDBDispatcher>();
        //    var mockAdminDB_branch = new Mock<IBranch>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);

        //    var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Update(token, updateBranch);

        //    //Assert
        //    Assert.AreEqual(403, actual.StatusCode);
        //}

        //[Test]
        //public void Branch_Update_BranchNameNullTest()
        //{
        //    var token = "test";
        //    BranchTemplate updateBranch = new BranchTemplate() { Id = 1, Name = null };

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_device = new Mock<IDevice>();
        //    var mockDataDB = new Mock<IDataDBDispatcher>();
        //    var mockAdminDB_branch = new Mock<IBranch>();
        //    var mockEmp = new Mock<IEmployee>();         

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
        //    mockAdminDB_branch.Setup(b => b.Update(updateBranch)).Throws(new Exception());


        //    var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Update(token, updateBranch);

        //    //Assert
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Branch_Update_InputNullTest()
        //{
        //    var token = "test";
        //    BranchTemplate updateBranch = null;

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_device = new Mock<IDevice>();
        //    var mockDataDB = new Mock<IDataDBDispatcher>();
        //    var mockAdminDB_branch = new Mock<IBranch>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
        //    mockAdminDB_branch.Setup(b => b.Update(updateBranch)).Throws(new Exception());


        //    var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Update(token, updateBranch);

        //    //Assert
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]        
        //public void Branch_Update_UpdateFailTest()
        //{
        //    var token = "test";
        //    BranchTemplate updateBranch = new BranchTemplate() { Id=1 ,Name="test"};

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_device = new Mock<IDevice>();
        //    var mockDataDB = new Mock<IDataDBDispatcher>();
        //    var mockAdminDB_branch = new Mock<IBranch>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
        //    mockAdminDB_branch.Setup(b => b.Update(updateBranch)).Throws(new Exception());


        //    var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Update(token, updateBranch);

        //    //Assert
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        [Test]
        public void Branch_Delete_TokenErrorTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, branchId);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_Delete_NotAdminTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, branchId);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_Delete_SuccessTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.Delete(Int32.Parse(branchId)));

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, branchId);

            //Assert
            Assert.AreEqual(202, actual.StatusCode);
        }

        [Test]
        public void Branch_Delete_BranchIdErrorTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.Delete(Int32.Parse(branchId))).Throws(new Exception("The branch would not be found"));

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, branchId);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Delete_InputNullTest()
        {
            var token = "test";
            string branchId = null;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);            

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, branchId);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Delete_FailTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.Delete(Int32.Parse(branchId))).Throws(new Exception("Delete the branch failed! Please refresh the web page and try again."));

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, branchId);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_DeviceAllocation_TokenErrorTest()
        {
            var token = "test";
            DeviceAllocationTemplate deviceAllocation = new DeviceAllocationTemplate() { BranchId = 1, DeviceIdList = new int[] { 1, 2, 3 } };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.DeviceAllocation(token, deviceAllocation);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_DeviceAllocation_NotAdminTest()
        {
            var token = "test";
            DeviceAllocationTemplate deviceAllocation = new DeviceAllocationTemplate() { BranchId = 1, DeviceIdList = new int[] { 1, 2, 3 } };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);
           

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.DeviceAllocation(token, deviceAllocation);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_DeviceAllocation_SuccessTest()
        {
            var token = "test";
            DeviceAllocationTemplate deviceAllocation = new DeviceAllocationTemplate() { BranchId = 1, DeviceIdList = new int[] { 1, 2, 3 } };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.DeviceAllocation(deviceAllocation));

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.DeviceAllocation(token, deviceAllocation);

            //Assert
            Assert.AreEqual(201, actual.StatusCode);
        }

        [Test]
        public void Branch_DeviceAllocation_FailTest()
        {
            var token = "test";
            DeviceAllocationTemplate deviceAllocation = new DeviceAllocationTemplate() { BranchId = 1, DeviceIdList = new int[] { 1, 2, 3 } };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.DeviceAllocation(deviceAllocation)).Throws(new Exception());

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.DeviceAllocation(token, deviceAllocation);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_DeviceAllocation_InputDeviceNullTest()
        {
            var token = "test";
            DeviceAllocationTemplate deviceAllocation = new DeviceAllocationTemplate() { BranchId = 1, DeviceIdList = new int[] { } };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.DeviceAllocation(deviceAllocation)).Throws(new Exception());

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.DeviceAllocation(token, deviceAllocation);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_DeviceAllocation_BranchIdErrorTest()
        {
            var token = "test";
            DeviceAllocationTemplate deviceAllocation = new DeviceAllocationTemplate() { BranchId = 2, DeviceIdList = null };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();
            Exception exc = new Exception("FK_Branch_To_BranchDeviceList");

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.DeviceAllocation(deviceAllocation)).Throws(new DbUpdateException("", exc));

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.DeviceAllocation(token, deviceAllocation);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_DeviceAllocation_DeviceIdErrorTest()
        {
            var token = "test";
            DeviceAllocationTemplate deviceAllocation = new DeviceAllocationTemplate() { BranchId = 2, DeviceIdList = new int []{ 999} };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();
            Exception exc = new Exception("FK_Device_To_BranchDeviceList");

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.DeviceAllocation(deviceAllocation)).Throws(new DbUpdateException("", exc));

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.DeviceAllocation(token, deviceAllocation);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_DeviceAllocation_DBErrorTest()
        {
            var token = "test";
            DeviceAllocationTemplate deviceAllocation = new DeviceAllocationTemplate() { BranchId = 2, DeviceIdList = new int[] { 1 } };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();           

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.DeviceAllocation(deviceAllocation)).Throws(new DbUpdateException("", new Exception()));

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.DeviceAllocation(token, deviceAllocation);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_DeviceAllocation_InputNullTest()
        {
            var token = "test";
            DeviceAllocationTemplate deviceAllocation = null;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.DeviceAllocation(deviceAllocation)).Throws(new Exception());
            
            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.DeviceAllocation(token, deviceAllocation);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_GetDeviceListByBranchId_TokenErrorTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.GetDeviceListByBranchId(token, branchId);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_GetDeviceListByBranchId_SuccessTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(b => b.GetDeviceList(Int32.Parse(branchId))).Returns(new DeviceOption[]{ new DeviceOption() { DeviceId=1, DeviceName="device00001"} });

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.GetDeviceListByBranchId(token, branchId);

            //Assert
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Branch_GetDeviceListByBranchId_SuccessNotFoundTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(b => b.GetDeviceList(Int32.Parse(branchId))).Returns(new DeviceOption[] { null });

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            StatusCodeResult actual = (StatusCodeResult)_target.GetDeviceListByBranchId(token, branchId);

            //Assert
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Branch_GetDeviceListByBranchId_InputNullTest()
        {
            var token = "test";
            string branchId = null;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");           

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.GetDeviceListByBranchId(token, branchId);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_GetDeviceListByBranchId_UnexceptedErrorTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(b => b.GetDeviceList(Int32.Parse(branchId))).Throws(new Exception());

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.GetDeviceListByBranchId(token, branchId);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Edit_TokenErrorTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Edit(token, branchId);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_Edit_IdNullTest()
        {
            var token = "test";
            string branchId = null;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Edit(token, branchId);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Edit_NotFoundTest()
        {
            var token = "test";
            string branchId = "10";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(b => b.GetInfo(Int32.Parse(branchId))).Returns((BranchTemplate)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            StatusCodeResult actual = (StatusCodeResult)_target.Edit(token, branchId);

            //Assert
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Branch_Edit_SuccessTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(b => b.GetInfo(Int32.Parse(branchId))).Returns(new BranchTemplate() { Id = 1, Name = "test", Selected = new SelectOptionTemplate[]{ new SelectOptionTemplate() { Id = 1, Name = "Device00001"} } });
            mockAdminDB_device.Setup(d => d.GetDeviceList()).Returns(new SelectOptionTemplate[1] { new SelectOptionTemplate() { Id = 1, Name = "device00001" } });

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Edit(token, branchId);

            //Assert
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Branch_Edit_ExceptionTest()
        {
            var token = "test";
            string branchId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(b => b.GetInfo(Int32.Parse(branchId))).Throws(new Exception());

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Edit(token, branchId);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Save_TokenErrorTest()
        {
            var token = "test";
            //BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "ALL Devices", EmailFlag = true, EventFlag = true, DeviceIdList = new int[] { 1, 2, 3} };
            BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "ALL Devices", Selected = new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Alias = "test", Name = "Device000001"}} };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Save(token, branchInfo);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_Save_NotAdminTest()
        {
            var token = "test";
            //BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "ALL Devices", EmailFlag = true, EventFlag = true, DeviceIdList = new int[] { 1, 2, 3 } };
            BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "ALL Devices", Selected = new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Alias = "test", Name = "Device000001" } } };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Save(token, branchInfo);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Branch_Save_NameNullTest()
        {
            var token = "test";
            //BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = null, EmailFlag = true, EventFlag = true, DeviceIdList = new int[] { 1, 2, 3 } };
            BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "ALL Devices", Selected = new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Alias = "test", Name = "Device000001" } } };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);


            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Save(token, branchInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        //[Test]
        //public void Branch_Save_ExistsTest()
        //{
        //    var token = "test";
        //    //BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "all devices", EmailFlag = true, EventFlag = true, DeviceIdList = new int[] { 1, 2, 3 } };
        //    BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "ALL Devices", Selected = new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Alias = "test", Name = "Device000001" } } };

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_device = new Mock<IDevice>();
        //    var mockDataDB = new Mock<IDataDBDispatcher>();
        //    var mockAdminDB_branch = new Mock<IBranch>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
        //    mockAdminDB_branch.Setup(b => b.NameExists(branchInfo.Name, branchInfo.Id)).Returns(true);
       

        //    var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Save(token, branchInfo);

        //    //Assert
        //    Assert.AreEqual(409, actual.StatusCode);
        //}

        [Test]
        public void Branch_Save_SuccessTest()
        {
            var token = "test";
            BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "ALL Devices", Selected = new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Alias = "test", Name = "Device000001" } } };
            //BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "ALL Devices", EmailFlag = true, EventFlag = true, Selected = new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Alias = "test", Name = "Device000001" } } };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.NameExists(branchInfo.Name)).Returns(false);
            mockAdminDB_branch.Setup(b => b.SaveInfo(branchInfo)).Returns(true);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Save(token, branchInfo);

            //Assert
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Branch_Save_DeviceNotFoundTest()
        {
            var token = "test";
            //BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "all devices", EmailFlag = true, EventFlag = true, DeviceIdList = new int[] { 1, 2, 3 } };
            BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "ALL Devices", Selected = new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Alias = "test", Name = "Device000001" } } };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();
            Exception exc = new Exception("FK_Device_To_BranchDeviceList");

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.NameExists(branchInfo.Name));
            mockAdminDB_branch.Setup(b => b.SaveInfo(branchInfo)).Throws(new DbUpdateException("", exc));

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Save(token, branchInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Save_GroupNotFoundTest()
        {
            var token = "test";
            //BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "all devices", EmailFlag = true, EventFlag = true, DeviceIdList = new int[] { 1, 2, 3 } };
            BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "ALL Devices", Selected = new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Alias = "test", Name = "Device000001" } } };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(b => b.NameExists(branchInfo.Name)).Returns(false);
            mockAdminDB_branch.Setup(b => b.SaveInfo(branchInfo)).Returns(false);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Save(token, branchInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Save_SaveFailTest()
        {
            var token = "test";
            //BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "all devices", EmailFlag = true, EventFlag = true, DeviceIdList = new int[] { 1, 2, 3 } };
            BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "ALL Devices", Selected = new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Alias = "test", Name = "Device000001" } } };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(e => e.NameExists(branchInfo.Name)).Returns(false);
            mockAdminDB_branch.Setup(b => b.SaveInfo(branchInfo)).Throws(new DbUpdateException("", new Exception()));

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Save(token, branchInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Save_ExceptionTest()
        {
            var token = "test";
            //BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "all devices", EmailFlag = true, EventFlag = true, DeviceIdList = new int[] { 1, 2, 3 } };
            BranchTemplate branchInfo = new BranchTemplate() { Id = 1, Name = "ALL Devices", Selected = new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Alias = "test", Name = "Device000001" } } };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_branch.Setup(e => e.NameExists(branchInfo.Name)).Returns(false);
            mockAdminDB_branch.Setup(b => b.SaveInfo(branchInfo)).Throws(new Exception());

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Save(token, branchInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Branch_Save_InputNullTest()
        {
            var token = "test";           
            BranchTemplate branchInfo = null;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Save(token, branchInfo);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Setting_GetGroupDeviceSetting_TokenErrorTest()
        {
            var token = "test";
            int id = 1;

            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.GetGroupDeviceSetting(token, id);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Setting_GetGroupDeviceSetting_SuccessTest()
        {
            var token = "test";
            int id = 1;

            //Arrange

            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(t => t.GetInfo(id)).Returns(new BranchTemplate() { Id = 1, Name = "test" });
            mockAdminDB_device.Setup(g => g.GetDeviceList()).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Name = "All Devices" } });

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.GetGroupDeviceSetting(token, id);

            //Assert
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Setting_GetGroupDeviceSetting_NotFoundTest()
        {
            var token = "test";
            int id = 1;

            //Arrange

            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(t => t.GetInfo(id)).Returns((BranchTemplate)null);

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            StatusCodeResult actual = (StatusCodeResult)_target.GetGroupDeviceSetting(token, id);

            //Assert
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Setting_GetGroupDeviceSetting_ExceptionTest()
        {
            var token = "test";
            int id = 1;

            //Arrange

            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_device = new Mock<IDevice>();
            var mockDataDB = new Mock<IDataDBDispatcher>();
            var mockAdminDB_branch = new Mock<IBranch>();
            var mockEmp = new Mock<IEmployee>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_branch.Setup(t => t.GetInfo(id)).Throws(new Exception());

            var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.GetGroupDeviceSetting(token, id);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        //[Test]
        //public void Branch_GetBranchList_TokenErrorTest()
        //{
        //    var token = "test";
        //    var companyId = "1";

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_device = new Mock<IDevice>();
        //    var mockDataDB = new Mock<IDataDBDispatcher>();
        //    var mockAdminDB_branch = new Mock<IBranch>();

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

        //    var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);
        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.GetBranchList(token, companyId);
        //    //Assert          
        //    Assert.AreEqual(403, actual.StatusCode);
        //}

        //[Test]
        //public void Branch_GetBranchList_SuccessTest()
        //{
        //    var token = "test";
        //    var companyId = "1";

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_device = new Mock<IDevice>();
        //    var mockDataDB = new Mock<IDataDBDispatcher>();
        //    var mockAdminDB_branch = new Mock<IBranch>();

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockAdminDB_branch.Setup(b => b.GetBranchList(Int32.Parse(companyId))).Returns(new BranchOption[] { new BranchOption() { BranchId=1, BranchName="Default"} });

        //    var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);
        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.GetBranchList(token,companyId);
        //    //Assert          
        //    Assert.AreEqual(200, actual.StatusCode);
        //}

        //[Test]
        //public void Branch_GetBranchList_InputNullTest()
        //{
        //    var token = "test";
        //    string companyId = null;

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_device = new Mock<IDevice>();
        //    var mockDataDB = new Mock<IDataDBDispatcher>();
        //    var mockAdminDB_branch = new Mock<IBranch>();

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    //mockAdminDB_branch.Setup(b => b.GetBranchList(Int32.Parse(companyId))).Throws(new ArgumentNullException());

        //    var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);
        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.GetBranchList(token, companyId);
        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Branch_GetBranchList_ExceptionTest()
        //{
        //    var token = "test";
        //    var companyId = "1";

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_device = new Mock<IDevice>();
        //    var mockDataDB = new Mock<IDataDBDispatcher>();
        //    var mockAdminDB_branch = new Mock<IBranch>();

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockAdminDB_branch.Setup(b => b.GetBranchList(Int32.Parse(companyId))).Throws(new Exception());

        //    var _target = new BranchController(mockRC.Object, mockAdminDB_device.Object, mockDataDB.Object, mockAdminDB_branch.Object, mockEmp.Object);

        //    ObjectResult actual = (ObjectResult)_target.GetBranchList(token, companyId);

        //    Assert.AreEqual(500, actual.StatusCode);
        //}


    }
}
