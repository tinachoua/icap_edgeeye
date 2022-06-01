using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using ShareLibrary.Interface;
using ShareLibrary.DataTemplate;
using AuthenticationAPI.Controllers;
using Microsoft.AspNetCore.Mvc; 


namespace AuthenticationAPI.Tests.Controllers
{
    class UseAuthControllerTests
    {
        /// <summary>
        /// 1.Employee response password is empty.
        /// </summary>       

        [Test]
        public void UseAuthLoginPWDNullTest()
        {
            string Username="admin";
            string Password=null;
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            var mockAdminDB_lincenselist = new Mock<ILicenselist>();

            EmployeeProfileTemplate emp = new EmployeeProfileTemplate() {LoginName="admin"};

            mockAdminDB_employee.Setup(e => e.Get(Username)).Returns(emp);


            var _target = new UserAuthController(mockRC.Object, mockAdminDB_employee.Object,mockAdminDB_lincenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Login(Username, Password);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }
        /// <summary>
        /// 1.Employee response password invalid password.
        /// </summary>

        [Test]
        public void UseAuthLoginInvalidPWDTest()
        {
            string Username = "admin";
            string Password = "123";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            var mockAdminDB_lincenselist = new Mock<ILicenselist>();

            EmployeeProfileTemplate emp = new EmployeeProfileTemplate() { LoginName = "admin" };

            mockAdminDB_employee.Setup(e => e.Get(Username)).Returns(emp);
            mockAdminDB_employee.Setup(e => e.CheckPWD(Username, Password)).Returns(false);

            var _target = new UserAuthController(mockRC.Object, mockAdminDB_employee.Object, mockAdminDB_lincenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Login(Username, Password);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        /// <summary>
        /// 1. employee not found
        /// </summary>

        [Test]
        public void UseAuthLoginEmployeeNotFoundTest()
        {
            string Username = "testccc";
            string Password = "testccc";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            var mockAdminDB_lincenselist = new Mock<ILicenselist>();

            EmployeeProfileTemplate emp = null;

            mockAdminDB_employee.Setup(e => e.Get(Username)).Returns(emp);       

            var _target = new UserAuthController(mockRC.Object, mockAdminDB_employee.Object, mockAdminDB_lincenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Login(Username, Password);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }


        /// <summary>
        /// 1. Employee login success first.
        /// </summary>       
        [Test]
        public void UseAuthLoginSuccessFirstTest()
        {
            string Username = "testccc";
            string Password = "testccc";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            var mockAdminDB_lincenselist = new Mock<ILicenselist>();

            EmployeeProfileTemplate emp = new EmployeeProfileTemplate() { LoginName=Username};

            mockAdminDB_employee.Setup(e => e.Get(Username)).Returns(emp);
            mockAdminDB_employee.Setup(e => e.CheckPWD(Username, Password)).Returns(true);

            var _target = new UserAuthController(mockRC.Object, mockAdminDB_employee.Object, mockAdminDB_lincenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Login(Username, Password);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        /// <summary>
        /// 1. Employee login success.
        /// </summary>

        [Test]
        public void UseAuthLoginSuccessTest()
        {
            string Username = "testccc";
            string Password = "testccc";
            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            var mockAdminDB_lincenselist = new Mock<ILicenselist>();

            EmployeeProfileTemplate emp = new EmployeeProfileTemplate() { LoginName = Username };

            mockAdminDB_employee.Setup(e => e.Get(Username)).Returns(emp);
            mockAdminDB_employee.Setup(e => e.CheckPWD(Username, Password)).Returns(true);

            var _target = new UserAuthController(mockRC.Object, mockAdminDB_employee.Object, mockAdminDB_lincenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Login(Username, Password);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        /// <summary>
        /// 1.User login successful.
        /// </summary>

        [Test]
        public void UseAuthTokenCheckerSuccessTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            var mockAdminDB_lincenselist = new Mock<ILicenselist>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new UserAuthController(mockRC.Object, mockAdminDB_employee.Object, mockAdminDB_lincenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.TokenChecker(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        /// <summary>
        /// 1.Request authentication token error.
        /// </summary>

        [Test]
        public void UseAuthTokenCheckerErrorTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            var mockAdminDB_lincenselist = new Mock<ILicenselist>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new UserAuthController(mockRC.Object, mockAdminDB_employee.Object, mockAdminDB_lincenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.TokenChecker(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        /// <summary>
        /// 1.Request authentication token error.
        /// </summary>

        [Test]
        public void UseAuthCheckAdminTokenErrorTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            var mockAdminDB_lincenselist = new Mock<ILicenselist>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new UserAuthController(mockRC.Object, mockAdminDB_employee.Object, mockAdminDB_lincenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.CheckAdmin(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        /// <summary>
        /// 1. Username is administrator.
        /// </summary>

        [Test]
        public void UseAuthCheckAdminTrueTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            var mockAdminDB_lincenselist = new Mock<ILicenselist>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new UserAuthController(mockRC.Object, mockAdminDB_employee.Object, mockAdminDB_lincenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.CheckAdmin(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        /// <summary>
        /// 1. Username is not administrator.
        /// </summary>

        [Test]
        public void UseAuthCheckAdminFalseTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockAdminDB_employee = new Mock<IEmployee>();
            var mockAdminDB_lincenselist = new Mock<ILicenselist>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_employee.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new UserAuthController(mockRC.Object, mockAdminDB_employee.Object, mockAdminDB_lincenselist.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.CheckAdmin(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }


    }
}
