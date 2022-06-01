using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Moq;
using NUnit.Framework;
using ShareLibrary.Interface;
using AuthenticationAPI.Controllers;
using ShareLibrary.DataTemplate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationAPI.Tests.Controllers
{     
    class EmployeeControllerTests
    {
        /// <summary>
        /// 1.Token Error.
        /// </summary>

        [Test]
        public void Employee_Create_TokenErrorTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload = new EmployeeProfileTemplate();
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();           

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Create(token, payload);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }    

        ///<summary>
        ///1. payload==null
        /// </summary>
         
        [Test]
        public void Employee_Create_PayloadNullTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload = null;
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Create(token, payload);

            //Assert          
            Assert.AreEqual(406, actual.StatusCode);
        }

        ///<summary>
        ///1. payload!=null
        ///2. login name exists
        /// </summary>

        [Test]
        public void Employee_Create_LoginNameExistsTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload = new EmployeeProfileTemplate()
            {
                LoginName ="admin",
                PWD="admin",
                VerifyPWD="admin"
            };
            
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckExists(payload.LoginName)).Returns(true);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Create(token, payload);

            //Assert          
            Assert.AreEqual(406, actual.StatusCode);
        }


        ///<summary>
        ///1. Password verify fail.      
        /// </summary>

        [Test]
        public void Employee_Create_PWDVerifyFailTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload = new EmployeeProfileTemplate()
            {
                LoginName = "admin",
                PWD = "YWRtaW4NCg==",
                VerifyPWD = "am9leQ0K"
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckExists(payload.LoginName)).Returns(false);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Create(token, payload);

            //Assert          
            Assert.AreEqual(406, actual.StatusCode);
        }

        ///<summary>
        ///1. Data error.      
        /// </summary>

        [Test]
        public void Employee_Create_DataErrorTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload = new EmployeeProfileTemplate()
            {
                LoginName = "admin",
                PWD = "123",
                VerifyPWD = "123"
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckExists(payload.LoginName)).Returns(false);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Create(token, payload);

            //Assert          
            Assert.AreEqual(406, actual.StatusCode);
        }

        ///<summary>
        ///1. Create Fail.      
        /// </summary>

        [Test]
        public void Employee_Create_FailTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload = new EmployeeProfileTemplate()
            {
                LoginName = "admin",
                PWD = "test",
                VerifyPWD = "test"
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckExists(payload.LoginName)).Returns(false);
            mockAdminDB_employee.Setup(e => e.Create(payload)).Returns(false);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.Create(token, payload);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }

        ///<summary>
        ///1. Create Success.      
        /// </summary>

        [Test]
        public void Employee_Create_SuccessTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload = new EmployeeProfileTemplate()
            {
                LoginName = "admin",
                PWD = "test",
                VerifyPWD = "test"
            };

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckExists(payload.LoginName)).Returns(false);
            mockAdminDB_employee.Setup(e => e.Create(payload)).Returns(true);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Create(token, payload);

            //Assert          
            Assert.AreEqual(201, actual.StatusCode);
        }

        /// <summary>
        /// 1.Token Error.
        /// </summary>

        [Test]
        public void Employee_Get_TokenErrorTest()
        {
            var token = "test";
            string loginName = "admin";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Get(loginName, token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        /// <summary>
        /// 1.LoginName is null.
        /// </summary>

        [Test]
        public void Employee_Get_LoginNameNullTest()
        {
            var token = "test";
            string loginName = null;
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Get(loginName, token);

            //Assert          
            Assert.AreEqual(402, actual.StatusCode);
        }

        /// <summary>
        /// 1. employee not found
        /// </summary>

        [Test]
        public void Employee_Get_EmployeeNotFoundTest()
        {
            var token = "test";
            string loginName = "admin";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.Get(loginName)).Returns((EmployeeProfileTemplate)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Get(loginName, token);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        /// <summary>
        /// 1. Get employee data success.
        /// </summary>

        [Test]
        public void Employee_Get_SuccessTest()
        {
            var token = "test";
            string loginName = "admin";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.Get(loginName)).Returns(new EmployeeProfileTemplate());

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Get(loginName, token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        /// <summary>
        /// 1.Token Error.
        /// </summary>

        [Test]
        public void Employee_GetFromToken_TokenErrorTest()
        {
            var token = "test";           
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetFromToken(token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        /// <summary>
        /// 1. employee not found
        /// </summary>

        [Test]
        public void Employee_GetFromToken_EmployeeNotFoundTest()
        {
            var token = "test";
           
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.Get("test")).Returns((EmployeeProfileTemplate)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetFromToken(token);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        /// <summary>
        /// 1. Get employee data success.
        /// </summary>

        [Test]
        public void Employee_GetFromToken_SuccessTest()
        {
            var token = "test";      

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.Get("test")).Returns(new EmployeeProfileTemplate());

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetFromToken(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        /// <summary>
        /// 1.Token Error.
        /// </summary>

        [Test]
        public void Employee_Update_TokenErrorTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload=new EmployeeProfileTemplate();
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, payload);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        /// <summary>
        /// 1.Payload is null.
        /// </summary>

        [Test]
        public void Employee_Update_PayloadNullTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload = null;
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, payload);

            //Assert          
            Assert.AreEqual(404, actual.StatusCode);
        }

        /// <summary>
        /// 1.Employee Password verify error.
        /// </summary>

        [Test]
        public void Employee_Update_PWDVerifyErrorTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload = new EmployeeProfileTemplate()
            {
                PWD = "test",
                VerifyPWD= "am9leQ0K"
            };
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, payload);

            //Assert          
            Assert.AreEqual(406, actual.StatusCode);
        }

        /// <summary>
        /// 1.Employee Password format error.
        /// </summary>

        [Test]
        public void Employee_Update_PWDFailTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload = new EmployeeProfileTemplate()
            {
                PWD = "test",
                VerifyPWD = "123"
            };
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, payload);

            //Assert          
            Assert.AreEqual(406, actual.StatusCode);
        }

        /// <summary>
        /// 1.Employee data not found.
        /// </summary>

        [Test]
        public void Employee_Update_EmployeeNotFoundTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload = new EmployeeProfileTemplate()
            {
                LoginName = "test",
                PWD = "test",
                VerifyPWD = "test"
            };
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.Update(payload)).Returns((bool?)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, payload);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        /// <summary>
        /// 1.Employee data upfate fail.
        /// </summary>

        [Test]
        public void Employee_Update_FailTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload = new EmployeeProfileTemplate()
            {
                LoginName = "test",
                PWD = "test",
                VerifyPWD = "test"
            };
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.Update(payload)).Returns(false);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, payload);

            //Assert          
            Assert.AreEqual(406, actual.StatusCode);
        }

        /// <summary>
        /// 1.Employee data upfate success.
        /// </summary>

        [Test]
        public void Employee_Update_SuccessTest()
        {
            var token = "test";
            EmployeeProfileTemplate payload = new EmployeeProfileTemplate()
            {
                LoginName = "test",
                PWD = "test",
                VerifyPWD = "test"
            };
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.Update(payload)).Returns(true);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, payload);

            //Assert          
            Assert.AreEqual(202, actual.StatusCode);
        }

        /// <summary>
        /// 1.Token Error.
        /// </summary>

        [Test]
        public void Employee_Delete_TokenErrorTest()
        {
            var token = "test";
            string loginName="test";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(loginName, token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        /// <summary>
        /// 1.Request does not contain payload.
        /// </summary>

        [Test]
        public void Employee_Delete_LoginNameNullTest()
        {
            var token = "test";
            string loginName = null;
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.Delete(loginName, token);

            //Assert          
            Assert.AreEqual(404, actual.StatusCode);
        }


        /// <summary>
        /// 1. LoginName is Admin.
        /// 2. Delete fail.
        /// </summary>

        [Test]
        public void Employee_Delete_LoginNameAdminTest()
        {
            var token = "test";
            string loginName = "admin";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.Delete(loginName, token);

            //Assert          
            Assert.AreEqual(406, actual.StatusCode);
        }

        /// <summary>
        /// 1. employee not found.  
        /// </summary>

        [Test]
        public void Employee_Delete_EmployeeNotFoundTest()
        {
            var token = "test";
            string loginName = "test";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.Delete(loginName)).Returns((bool?)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(loginName, token);

            //Assert          
            Assert.AreEqual(304, actual.StatusCode);
        }

        /// <summary>
        /// 1. employee delete fail.  
        /// </summary>

        [Test]
        public void Employee_Delete_FailTest()
        {
            var token = "test";
            string loginName = "test";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.Delete(loginName)).Returns(false);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.Delete(loginName, token);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }

        /// <summary>
        /// 1. employee delete success.  
        /// </summary>

        [Test]
        public void Employee_Delete_SuccessTest()
        {
            var token = "test";
            string loginName = "test";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.Delete(loginName)).Returns(true);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(loginName, token);

            //Assert          
            Assert.AreEqual(202, actual.StatusCode);
        }

        /// <summary>
        /// 1.Token Error.
        /// </summary>

        [Test]
        public void Employee_GetList_TokenErrorTest()
        {
            var token = "test";
            
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetList(token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        /// <summary>
        /// 1.Get list success.
        /// </summary>

        [Test]
        public void Employee_GetList_SuccessTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.GetList());

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetList(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Employee_GetImg_TokenErrorTest()
        {
            string token = "test";
            string empId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetImg(token, empId);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Employee_GetImg_SuccessTest()
        {
            var token = "test";
            string empId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(d => d.GetImgBase64(empId,"employees")).Returns("test");

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetImg(token, empId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Employee_GetImg_ExceptionTest()
        {
            var token = "test";
            string empId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(d => d.GetImgBase64(empId, "employees")).Throws(new Exception());

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetImg(token, empId);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Employee_UploadImg_TokenErrorTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "1";
            string empId = "1";           
            
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            
            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, empId, overwrite, files);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Employee_UploadImg_UnacceptableFileExtensionsTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "1";
            string empId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(d => d.AllowedFileExtensions(files)).Returns(false);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            ////Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, empId, overwrite, files);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Employee_UploadImg_FileExistsTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "0";
            string empId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();            

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(d => d.AllowedFileExtensions(files)).Returns(true);
            mockAdminDB_employee.Setup(d => d.UploadImg(files, !(Int32.Parse(overwrite)).Equals(0), empId, "employees")).Returns(false);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, empId, overwrite, files);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Employee_UploadImg_SuccessTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "1";
            string empId = "1";

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
            var mockAdminDB_employee = new Mock<IEmployee>();           

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(d => d.AllowedFileExtensions(files)).Returns(true);
            mockAdminDB_employee.Setup(d => d.UploadImg(files, !(Int32.Parse(overwrite)).Equals(0), empId, "employees")).Returns(true);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, empId, overwrite, files);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Employee_UploadImg_ExceptionTest()
        {
            var token = "test";
            List<IFormFile> files = new List<IFormFile>();
            string overwrite = "1";
            string empId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();          

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(d => d.AllowedFileExtensions(files)).Returns(true);
            mockAdminDB_employee.Setup(d => d.UploadImg(files, !(Int32.Parse(overwrite)).Equals(0), empId, "employees")).Throws(new Exception());

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.UploadImg(token, empId, overwrite, files);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Employee_SetCommonMap_TokenErrorTest()
        {
            var token = "test";
            int mapId = 5;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetCommonMap(token, mapId);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Employee_SetCommonMap_SuccessTest()
        {
            var token = "test";
            int mapId = 5;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("admin");
            mockAdminDB_employee.Setup(e => e.SetCommonMap(token, mapId));

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.SetCommonMap(token, mapId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Employee_SetCommonMap_ExceptionTest()
        {
            var token = "admin";
            int mapId = 5;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            Exception exc = new Exception("Data Error");

            mockRC.Setup(t => t.GetCache(0, token)).Returns("admin");
            mockAdminDB_employee.Setup(e => e.SetCommonMap(token, mapId)).Throws(exc);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetCommonMap(token, mapId);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Employee_SetCommonMap_DBExceptionTest()
        {
            var token = "admin";
            int mapId = 5;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            Exception exc = new Exception("Data Error");

            mockRC.Setup(t => t.GetCache(0, token)).Returns("admin");
            mockAdminDB_employee.Setup(e => e.SetCommonMap(token, mapId)).Throws(new DbUpdateException("", exc));

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SetCommonMap(token, mapId);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Employee_GetEmployeeName_TokenErrorTest()
        {
            var token = "admin";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmployeeName(token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Employee_GetEmployeeName_SuccessTest()
        {
            var token = "admin";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("admin");
            mockAdminDB_employee.Setup(e => e.GetName()).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate() { Id=1, Name="test"} });

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmployeeName(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Employee_GetEmployeeName_ExceptionTest()
        {
            var token = "admin";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("admin");
            mockAdminDB_employee.Setup(e => e.GetName()).Throws(new Exception());

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmployeeName(token);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }

        [Test]
        public void Employee_GetEmployeeName_NotFoundTest()
        {
            var token = "admin";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("admin");
            mockAdminDB_employee.Setup(e => e.GetName()).Returns(new SelectOptionTemplate[] { });

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetEmployeeName(token);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Employee_FindEmployeeEmail_NotFoundTest()
        {
            var token = "admin";
            string searchString = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("admin");
            mockAdminDB_employee.Setup(e => e.FindAllEmail(searchString)).Returns(new EmailSearchTemplate[0]);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.FindEmployeeEmail(token, searchString);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Employee_FindEmployeeEmail_SuccessfullyTest()
        {
            var token = "admin";
            string searchString = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("admin");
            mockAdminDB_employee.Setup(e => e.FindAllEmail(searchString)).Returns(new EmailSearchTemplate[] 
            { new EmailSearchTemplate()
                {
                    label="test"
                }
            });

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.FindEmployeeEmail(token, searchString);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

         
        [Test]
        public void Employee_FindEmployeeEmail_TokenErrorTest()
        {
            var token = "admin";
            string searchString = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);
  
            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.FindEmployeeEmail(token, searchString);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Employee_FindEmployeeEmail_ExceptionTest()
        {
            var token = "admin";
            string searchString = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.FindAllEmail(searchString)).Throws(new Exception());

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.FindEmployeeEmail(token, searchString);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }

        [Test]
        public void Employee_GetEmailByPermissionId_NotFoundTest()
        {
            var token = "admin";
            int permissionId = 1;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("admin");
            mockAdminDB_employee.Setup(e => e.GetEmailForTooltip(permissionId)).Returns(new string[0]);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetEmailByPermissionId(token, permissionId);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Employee_GetEmailByPermissionId_SuccessfullyTest()
        {
            var token = "admin";
            int permissionId = 1;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("admin");
            mockAdminDB_employee.Setup(e => e.GetEmailForTooltip(permissionId)).Returns(new string[] { "roy<roy_chen@innodisk.com>"});

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmailByPermissionId(token, permissionId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Employee_GetEmailByPermissionId_TokenErrorTest()
        {
            var token = "admin";
            int permissionId = 1;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmailByPermissionId(token, permissionId);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Employee_GetEmailByPermissionId_ExceptionTest()
        {
            var token = "admin";
            int permissionId = 1;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.GetEmailForTooltip(permissionId)).Throws(new Exception());

            var _target = new EmployeeController(mockRC.Object, mockAdminDB_employee.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetEmailByPermissionId(token, permissionId);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }



    }
}
