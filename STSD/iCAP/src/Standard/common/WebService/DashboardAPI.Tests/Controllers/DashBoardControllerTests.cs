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
using ShareLibrary;
using Newtonsoft.Json;

namespace DashboardAPI.Tests.Controllers
{
    class DashBoardControllerTests
    {
        [Test]
        public void Dashboard_GetDashboradList_SuccessTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(m => m.GetDashboardList()).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate { Id = 1, Name = "Dashboard1" } });

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.GetDashboardList(token);

            //Assert
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetDashboradList_SuccessNotFoundTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(m => m.GetDashboardList()).Returns(new SelectOptionTemplate[] { null });

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            StatusCodeResult actual = (StatusCodeResult)_target.GetDashboardList(token);

            //Assert
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetDashboardList_TokenErrorTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.GetDashboardList(token);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetDashboardList_ExceptionTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(m => m.GetDashboardList()).Throws(new Exception());

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.GetDashboardList(token);

            //Assert
            Assert.AreEqual(500, actual.StatusCode);
        }

        [Test]
        public void Dashboard_DashboradWidgetArrangement_NotAdminTest()
        {
            var token = "test";
            WidgetOrderTemplate DashboardElement = new WidgetOrderTemplate() { DashboardId = 1, WidgetIdList = new int[] { 1, 2, 3 } };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);
            mockAdminDB_multipleDashboard.Setup(m => m.SetWidgetOrder(DashboardElement));

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SetDashboardWidgetOrder(token, DashboardElement);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_DashboradWidgetArrangement_SuccessTest()
        {
            var token = "test";
            WidgetOrderTemplate DashboardElement = new WidgetOrderTemplate() { DashboardId = 1, WidgetIdList = new int[] { 1, 2, 3 } };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(m => m.SetWidgetOrder(DashboardElement));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SetDashboardWidgetOrder(token, DashboardElement);

            //Assert
            Assert.AreEqual(201, actual.StatusCode);
        }

        [Test]
        public void Dashboard_SetDashboardWidgetOrder_TokenErrorTest()
        {
            var token = "test";
            WidgetOrderTemplate DashboardElement = new WidgetOrderTemplate() { DashboardId = 1, WidgetIdList = new int[] { 1, 2, 3 } };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SetDashboardWidgetOrder(token, DashboardElement);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_SetDashboardWidgetOrder_ExceptionTest()
        {
            var token = "test";
            WidgetOrderTemplate DashboardElement = new WidgetOrderTemplate() { DashboardId = 1, WidgetIdList = new int[] { 1, 2, 3 } };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(m => m.SetWidgetOrder(DashboardElement)).Throws(new Exception());
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SetDashboardWidgetOrder(token, DashboardElement);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_SetDashboardWidgetOrder_InputWidgetNullTest()
        {
            var token = "test";
            WidgetOrderTemplate dashboardElement = new WidgetOrderTemplate() { DashboardId = 1, WidgetIdList = new int[] { } };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_multipleDashboard.Setup(d => d.SetWidgetOrder(dashboardElement)).Throws(new Exception());

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SetDashboardWidgetOrder(token, dashboardElement);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }



        [Test]
        public void Dashboard_SetDashboardWidgetOrder_DashboardIdErrorTest()
        {
            var token = "test";
            WidgetOrderTemplate DashboardElement = new WidgetOrderTemplate() { DashboardId = 1, WidgetIdList = new int[] { 1, 2, 3 } };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            Exception exc = new Exception("FK_CompanyDashboard_To_CompanyDashboardElement");

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_multipleDashboard.Setup(m => m.SetWidgetOrder(DashboardElement)).Throws(new DbUpdateException("", exc));


            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SetDashboardWidgetOrder(token, DashboardElement);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_SetDashboardWidgetOrder_WidgetIdErrorTest()
        {
            var token = "test";
            WidgetOrderTemplate DashboardElement = new WidgetOrderTemplate() { DashboardId = 1, WidgetIdList = new int[] { 1, 2, 3 } };

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            Exception exc = new Exception("FK_Widget_To_CompanyDashboardElement");

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_multipleDashboard.Setup(m => m.SetWidgetOrder(DashboardElement)).Throws(new DbUpdateException("", exc));


            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SetDashboardWidgetOrder(token, DashboardElement);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_SetDashboardWidgetOrder_DBErrorTest()
        {
            var token = "test";
            WidgetOrderTemplate DashboardElement = new WidgetOrderTemplate() { DashboardId = 1, WidgetIdList = new int[] { 1, 2, 3 } };

            //Arrange  
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_multipleDashboard.Setup(m => m.SetWidgetOrder(DashboardElement)).Throws(new DbUpdateException("", new Exception()));


            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SetDashboardWidgetOrder(token, DashboardElement);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_SetDashboardWidgetOrder_InputNullTest()
        {
            var token = "test";
            WidgetOrderTemplate dashboardElement = null;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_multipleDashboard.Setup(d => d.SetWidgetOrder(dashboardElement)).Throws(new Exception());


            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.SetDashboardWidgetOrder(token, dashboardElement);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Create_TokenErrorTest()
        {
            var token = "test";
            string dashboradName = "dashboard1";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Create(token, dashboradName);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Create_NotAdminTest()
        {
            var token = "test";
            string dashboradName = "dashboard1";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Create(token, dashboradName);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Create_SuccessTest()
        {
            var token = "test";
            string dashboradName = "dashboard1";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_multipleDashboard.Setup(m => m.Create(dashboradName));

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Create(token, dashboradName);

            //Assert
            Assert.AreEqual(201, actual.StatusCode);
        }

        //[Test]
        //public void Dashboard_Create_DashboardNameNullTest()
        //{
        //    var token = "test";
        //    string dashboradName = null;

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockEmp = new Mock<IEmployee>();
        //    Exception exc = new Exception("DashboardName can not be null");

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
        //    mockAdminDB_multipleDashboard.Setup(m => m.Create(dashboradName)).Throws(new DbUpdateException("",exc));


        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Create(token, dashboradName);

        //    //Assert
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_Create_CompanyIdErrorTest()
        //{
        //    var token = "test";
        //    DashboardTemplate createDashboard = new DashboardTemplate() { DashboardName = "test" };

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockEmp = new Mock<IEmployee>();
        //    Exception exc = new Exception("FK_Company_To_CompanyDashboardList");

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
        //    mockAdminDB_multipleDashboard.Setup(m => m.Create(createDashboard)).Throws(new DbUpdateException("FK_Company_To_CompanyDashboardList", exc));


        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Create(token, createDashboard);

        //    //Assert
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        [Test]
        public void Dashboard_Create_InputNullTest()
        {
            var token = "test";
            string dashboradName = null;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_multipleDashboard.Setup(m => m.Create(dashboradName)).Throws(new DbUpdateException("", new Exception("The dashboard name can not be blank!")));

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Create(token, dashboradName);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Create_DBErrorTest()
        {
            var token = "test";
            string dashboardName = "Dashboard 1";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_multipleDashboard.Setup(m => m.Create(dashboardName)).Throws(new DbUpdateException("", new Exception()));

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Create(token, dashboardName);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }


        [Test]
        public void Dashboard_Create_CreateFailTest()
        {
            var token = "test";
            string dashboardName = "Dashboard 1";

            //Arrange    
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_multipleDashboard.Setup(m => m.Create(dashboardName)).Throws(new Exception());

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Create(token, dashboardName);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        //[Test]
        //public void Dashboard_Update_TokenErrorTest()
        //{
        //    var token = "test";
        //    DashboardTemplate updateDashboard = new DashboardTemplate() {DashboardId = 1 , DashboardName="test" };

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Update(token, updateDashboard);

        //    //Assert
        //    Assert.AreEqual(403, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_Update_NotAdminTest()
        //{
        //    var token = "test";
        //    DashboardTemplate updateDashboard = new DashboardTemplate() { DashboardName = "test" };

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);            

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Update(token, updateDashboard);

        //    //Assert
        //    Assert.AreEqual(403, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_Update_SuccessTest()
        //{
        //    var token = "test";
        //    DashboardTemplate updateDashboard = new DashboardTemplate() { DashboardName = "test" };

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
        //    mockAdminDB_multipleDashboard.Setup(m =>m.Update(updateDashboard));

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Update(token, updateDashboard);

        //    //Assert
        //    Assert.AreEqual(200, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_Update_DashboardNameNullTest()
        //{
        //    var token = "test";
        //    DashboardTemplate createDashboard = new DashboardTemplate() { DashboardName = null };

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockEmp = new Mock<IEmployee>();
        //    Exception exc = new Exception("DashboardName can not be null");

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
        //    mockAdminDB_multipleDashboard.Setup(m => m.Update(createDashboard)).Throws(new DbUpdateException("", exc));


        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Update(token, createDashboard);

        //    //Assert
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_Update_InputNullTest()
        //{
        //    var token = "test";
        //    DashboardTemplate createDashboard = null;

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockEmp = new Mock<IEmployee>();            

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
        //    mockAdminDB_multipleDashboard.Setup(m => m.Update(createDashboard)).Throws(new Exception());


        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Update(token, createDashboard);

        //    //Assert
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_Update_CompanyIdErrorTest()
        //{
        //    var token = "test";
        //    DashboardTemplate createDashboard = new DashboardTemplate() {  DashboardName = "test" };

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockEmp = new Mock<IEmployee>();
        //    Exception exc = new Exception("FK_Company_To_CompanyDashboardList");

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
        //    mockAdminDB_multipleDashboard.Setup(m => m.Update(createDashboard)).Throws(new DbUpdateException("FK_Company_To_CompanyDashboardList", exc));


        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Update(token, createDashboard);

        //    //Assert
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_Update_UpdateFailTest()
        //{
        //    var token = "test";
        //    DashboardTemplate createDashboard = new DashboardTemplate() {  DashboardName = "test" };

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockEmp = new Mock<IEmployee>();          

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
        //    mockAdminDB_multipleDashboard.Setup(m => m.Update(createDashboard)).Throws(new Exception());

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act

        //    ObjectResult actual = (ObjectResult)_target.Update(token, createDashboard);

        //    //Assert
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        [Test]
        public void Dashboard_Delete_TokenErrorTest()
        {
            var token = "test";
            string dashboardId = "1";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, dashboardId);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Delete_NotAdminTest()
        {
            var token = "test";
            string dashboardId = "1";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, dashboardId);

            //Assert
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Delete_SuccessTest()
        {
            var token = "test";
            string dashboardId = "1";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_multipleDashboard.Setup(m => m.Delete(Int32.Parse(dashboardId)));

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, dashboardId);

            //Assert
            Assert.AreEqual(202, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Delete_DashboardIdErrorTest()
        {
            var token = "test";
            string dashboardId = "1";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_multipleDashboard.Setup(m => m.Delete(Int32.Parse(dashboardId))).Throws(new Exception("The dashboard does not exist."));


            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, dashboardId);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Delete_InputNullTest()
        {
            var token = "test";
            var dashboardId = (string)null;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            //mockAdminDB_multipleDashboard.Setup(m => m.Delete(dashboardId)).Throws(new Exception("Delete Failed"));

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, dashboardId);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Delete_FailTest()
        {
            var token = "test";
            string dashboardId = "1";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_multipleDashboard.Setup(m => m.Delete(Int32.Parse(dashboardId))).Throws(new Exception("Delete Failed"));

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act

            ObjectResult actual = (ObjectResult)_target.Delete(token, dashboardId);

            //Assert
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Save_TokenErrorTest()
        {
            DashboardTemplate dashboardIdfo = new DashboardTemplate();
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);//set not found the user from the Redis Cache                   
            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Save(token, dashboardIdfo);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        //[Test]
        //public void Dashboard_Save_PayloadNullTest()
        //{
        //    DashboardTemplate dashboardIdfo = null;
        //    var token = "test";

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
        //    Mock<IEmployee> mockEmp = new Mock<IEmployee>();
        //    Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
        //    Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
        //    Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
        //    Mock<ISocket> mockSocketService = new Mock<ISocket>();

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockAdminDB_multipleDashboard.Setup(d => d.SaveDashboardInfo(dashboardIdfo)).Throws(new Exception());
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Save(token, dashboardIdfo);

        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_Save_WidgetIdErrorTest()
        //{
        //    DashboardTemplate dashboardInfo = new DashboardTemplate();
        //    var token = "test";

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
        //    Mock<IEmployee> mockEmp = new Mock<IEmployee>();
        //    Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
        //    Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
        //    Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
        //    Mock<ISocket> mockSocketService = new Mock<ISocket>();

        //    Exception exc = new Exception("FK_Widget_To_CompanyDashboardElement");

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockAdminDB_multipleDashboard.Setup(d => d.SaveDashboardInfo(dashboardInfo)).Throws(new DbUpdateException("", exc));
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Save(token, dashboardInfo);

        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_Save_NameNullTest()
        //{
        //    DashboardTemplate dashboardInfo = new DashboardTemplate() { Id = 1, Name = null };
        //    var token = "test";

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
        //    Mock<IEmployee> mockEmp = new Mock<IEmployee>();
        //    Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
        //    Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
        //    Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
        //    Mock<ISocket> mockSocketService = new Mock<ISocket>();

        //    Exception exc = new Exception("Name can not be null");

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockAdminDB_multipleDashboard.Setup(d => d.SaveDashboardInfo(dashboardInfo)).Throws(exc);
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Save(token, dashboardInfo);

        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_Save_SaveFailTest()
        //{
        //    DashboardTemplate dashboardInfo = new DashboardTemplate() { Id = 1, Name = null };
        //    var token = "test";

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
        //    Mock<IEmployee> mockEmp = new Mock<IEmployee>();
        //    Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
        //    Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
        //    Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
        //    Mock<ISocket> mockSocketService = new Mock<ISocket>();

        //    Exception exc = new Exception("");

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockAdminDB_multipleDashboard.Setup(d => d.SaveDashboardInfo(dashboardInfo)).Throws(new DbUpdateException("", exc));
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Save(token, dashboardInfo);

        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_Save_UnexceptedDataErrorTest()
        //{
        //    DashboardTemplate dashboardInfo = new DashboardTemplate() { Id = 1, Name = null };
        //    var token = "test";

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
        //    Mock<IEmployee> mockEmp = new Mock<IEmployee>();
        //    Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
        //    Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
        //    Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
        //    Mock<ISocket> mockSocketService = new Mock<ISocket>();

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockAdminDB_multipleDashboard.Setup(d => d.SaveDashboardInfo(dashboardInfo)).Throws(new Exception());
        //    mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Save(token, dashboardInfo);

        //    // Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        [Test]
        public void Dashboard_Save_SuccessTest()
        {
            DashboardTemplate dashboardInfo = new DashboardTemplate() { Id = 1, Name = "default1", WidgetIdList = new int[] { } };
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.SaveDashboardInfo(dashboardInfo)).Returns(true);
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Save(token, dashboardInfo);

            // Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Save_WidgetNotFoundTest()
        {
            DashboardTemplate dashboardInfo = new DashboardTemplate() { Id = 1, Name = "default1", WidgetIdList = new int[] { } };
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.SaveDashboardInfo(dashboardInfo)).Throws(new DbUpdateException("", new Exception("FK_Widget_To_CompanyDashboardElement")));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Save(token, dashboardInfo);

            // Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Save_DbUpdateExceptionTest()
        {
            DashboardTemplate dashboardInfo = new DashboardTemplate() { Id = 1, Name = "default1", WidgetIdList = new int[] { } };
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.SaveDashboardInfo(dashboardInfo)).Throws(new DbUpdateException("", new Exception("")));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Save(token, dashboardInfo);

            // Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Save_InputNullTest()
        {
            DashboardTemplate dashboardInfo = null;
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.SaveDashboardInfo(dashboardInfo)).Throws(new Exception());
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Save(token, dashboardInfo);

            // Assert          
            Assert.AreEqual(422, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Save_ExceptionTest()
        {
            DashboardTemplate dashboardInfo = new DashboardTemplate() { Id = 1, Name = "default1", WidgetIdList = new int[] { } };
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.SaveDashboardInfo(dashboardInfo)).Throws(new Exception());
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Save(token, dashboardInfo);

            // Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Save_Success2Test()
        {
            DashboardTemplate dashboardInfo = new DashboardTemplate() { Id = 1, Name = "default1", WidgetIdList = new int[] { 1 } };
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.SaveDashboardInfo(dashboardInfo)).Returns(true);
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Save(token, dashboardInfo);

            // Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Save_DashboardNotFoundTest()
        {
            DashboardTemplate dashboardInfo = new DashboardTemplate() { Id = 5, Name = "default1" };
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.SaveDashboardInfo(dashboardInfo)).Returns(false);
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Save(token, dashboardInfo);

            // Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Save_HaveNotGetAccessTest()
        {
            DashboardTemplate dashboardInfo = new DashboardTemplate() { Id = 1, Name = null };
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.SaveDashboardInfo(dashboardInfo));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Save(token, dashboardInfo);

            // Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Edit_TokenErrorTest()
        {
            string dashboardId = "1";
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);//set not found the user from the Redis Cache                   
            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Edit(token, dashboardId);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Edit_DashboardNotFoundTest()
        {
            string dashboardId = "5";
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");//set not found the user from the Redis Cache                   
            mockAdminDB_multipleDashboard.Setup(d => d.GetNameByDashboardId(Int32.Parse(dashboardId))).Returns((string)null);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.Edit(token, dashboardId);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Edit_SuccessTest()
        {
            string dashboardId = "5";
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");//set not found the user from the Redis Cache                   
            mockAdminDB_multipleDashboard.Setup(d => d.GetNameByDashboardId(Int32.Parse(dashboardId))).Returns("default1");

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Edit(token, dashboardId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Edit_InputNullTest()
        {
            string dashboardId = null;
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");//set not found the user from the Redis Cache                   
                                                                    // mockAdminDB_multipleDashboard.Setup(d => d.GetNameByDashboardId(Int32.Parse(dashboardId))).Returns("default1");

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Edit(token, dashboardId);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_Edit_ExceptionTest()
        {
            string dashboardId = "1";
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");//set not found the user from the Redis Cache                   
            mockAdminDB_multipleDashboard.Setup(d => d.GetNameByDashboardId(Int32.Parse(dashboardId))).Throws(new Exception());

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Edit(token, dashboardId);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_SendEmailMessage_TokenErrorTest()
        {
            EmailMessageTemplate msgInfo = null;
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);//set not found the user from the Redis Cache                   
            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SendEmailMessage(token, msgInfo);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_SendEmailMessage_NoAccessTest()
        {
            EmailMessageTemplate msgInfo = null;
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");//set not found the user from the Redis Cache  
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);
            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SendEmailMessage(token, msgInfo);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_SendEmailMessage_NoReceiverTest()
        {
            EmailMessageTemplate msgInfo = new EmailMessageTemplate() { Id = new int[0] };
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");//set not found the user from the Redis Cache  
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SendEmailMessage(token, msgInfo);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_SendEmailMessage_SuccessTest()
        {
            EmailMessageTemplate msgInfo = new EmailMessageTemplate() { Id = new int[1], Message = "" };
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");//set not found the user from the Redis Cache  
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(s => s.Send(msgInfo));

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SendEmailMessage(token, msgInfo);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Dashboard_SendEmailMessage_FailTest()
        {
            EmailMessageTemplate msgInfo = new EmailMessageTemplate() { Id = new int[1], Message = "" };
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");//set not found the user from the Redis Cache  
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockAdminDB_email.Setup(s => s.Send(msgInfo)).Throws(new Exception());

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.SendEmailMessage(token, msgInfo);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetPanelSetting_SuccessTest()
        {
            var token = "test";
            int companyId = 1;
            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(m => m.GetPanelInfo(companyId)).Returns(new PanelTemplate());

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetPanelSetting(token, companyId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetPanelSetting_TokenErrorTest()
        {
            var token = "test";
            int companyId = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetPanelSetting(token, companyId);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetPanelSetting_DashboardNotFoundTest()
        {
            var token = "test";
            int companyId = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(s => s.GetPanelInfo(companyId)).Returns((PanelTemplate)null);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetPanelSetting(token, companyId);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetPanelSetting_ExceptionTest()
        {
            var token = "test";
            int companyId = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(s => s.GetPanelInfo(companyId)).Throws(new Exception());

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetPanelSetting(token, companyId);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetWidgetData_TokenErrorTest()
        {
            var token = "test";
            int dashboardId = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetWidgetData(token, dashboardId);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetWidgetData_AdminDBError()
        {
            var token = "test";
            int dashboardId = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.DashboardExist(dashboardId)).Returns(true);
            mockAdminDB_multipleDashboard.Setup(d => d.GetWidgetId(dashboardId)).Throws(new Exception());


            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetWidgetData(token, dashboardId);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetWidgetData_DashbaordDeletedTest()
        {
            var token = "test";
            int dashboardId = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.DashboardExist(dashboardId)).Returns(false);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetWidgetData(token, dashboardId);

            //Assert          
            Assert.AreEqual(410, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetWidgetData_WidgetNotFoundTest()
        {
            var token = "test";
            int dashboardId = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.DashboardExist(dashboardId)).Returns(true);
            mockAdminDB_multipleDashboard.Setup(d => d.GetWidgetId(dashboardId)).Returns((int[])null);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetWidgetData(token, dashboardId);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetWidgetData_Success()
        {
            var token = "test";
            int dashboardId = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            BsonDocument document = new BsonDocument()
            {
                {"_id",123 },
                { "name", "Device status"},
                { "type", "text"},
                { "width", (int)DataDefine.Width.SIZE_1X1},
                { "data", new BsonDocument()},
                { "detailWidget", new BsonDocument()}
            };

            mockRC.Setup(r => r.GetCache(0, token)).Returns("admin");
            mockAdminDB_multipleDashboard.Setup(o => o.DashboardExist(dashboardId)).Returns(true);
            mockAdminDB_multipleDashboard.Setup(o => o.GetWidgetId(dashboardId)).Returns(new int[] { 11 });
            mockDB.Setup(d => d.GetLastRawData("Widget" + 11)).Returns(document);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetWidgetData(token, dashboardId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetWidgetData_DataDBNotFoundSuccess()
        {
            var token = "test";
            int dashboardId = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            BsonDocument document = new BsonDocument()
            {
                {"_id",123 },
                { "name", "Device status"},
                { "type", "text"},
                { "width", (int)DataDefine.Width.SIZE_1X1},
                { "data", new BsonDocument()},
                { "detailWidget", new BsonDocument()}
            };

            mockRC.Setup(r => r.GetCache(0, token)).Returns("admin");
            mockAdminDB_multipleDashboard.Setup(o => o.DashboardExist(dashboardId)).Returns(true);
            mockAdminDB_multipleDashboard.Setup(o => o.GetWidgetId(dashboardId)).Returns(new int[] { 11 });
            mockDB.Setup(d => d.GetLastRawData("Widget" + 11)).Throws(new Exception());

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetWidgetData(token, dashboardId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }


        [Test]
        public void Dashboard_GetWidgetData_Computing2()
        {
            var token = "test";
            int dashboardId = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("admin");
            mockAdminDB_multipleDashboard.Setup(o => o.DashboardExist(dashboardId)).Returns(true);
            mockAdminDB_multipleDashboard.Setup(o => o.GetWidgetId(dashboardId)).Returns(new int[] { 1 });
            mockDB.Setup(d => d.GetLastRawData("DashboardData")).Returns((BsonDocument)null);
            mockAdminDB_multipleDashboard.Setup(o => o.DashboardExist(dashboardId)).Returns(true);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetWidgetData(token, dashboardId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetWidgetDetails_TokenErrorTest()
        {
            var token = "test";
            int dashboardId = 1;
            int widgetId = 11;
            int index = 1;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetWidgetDetails(token, dashboardId, widgetId, index);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetWidgetDetails_DataDBException()
        {
            var token = "test";
            int dashboardId = 1;
            int widgetId = 11;
            int index = 0;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            BsonDocument document = new BsonDocument()
            {
                { "id", 12},
                { "name", "Device status"},
                { "type", "text"},
                { "width", (int)DataDefine.Width.SIZE_1X1},
                { "data", new BsonDocument()},
                { "detailWidget", new BsonDocument()
                    {
                        { "data", new BsonArray()
                            {
                                new BsonDocument()
                                {
                                    { "deviceList", new BsonArray(){ }},
                                    { "record", new BsonArray(){ }}
                                },
                                new BsonDocument()
                                {
                                    { "deviceList", new BsonArray(){ }},
                                    { "record", new BsonArray(){ }}
                                }
                            }
                        },
                        { "item", new BsonArray()
                            {
                                {"Device Name"},
                                {"Device status"},
                                {"Device Owner"},
                                {"Device Time(UTC)"}
                            }
                        }
                    }
                }
            };

            mockRC.Setup(s => s.GetCache(0, token)).Returns("admin");
            mockAdminDB_multipleDashboard.Setup(d => d.WidgetInDashboard(dashboardId, widgetId)).Returns(true);
            mockDB.Setup(d => d.GetLastRawData("Widget"+11)).Throws(new Exception());

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetWidgetDetails(token, dashboardId, widgetId, index);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }


        [Test]
        public void Dashboard_GetWidgetDetails_NotFound()
        {
            var token = "test";
            int dashboardId = 2;
            int widgetId = 11;
            int index = 0;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            BsonDocument document = new BsonDocument()
            {
                { "id", 12},
                { "name", "Device status"},
                { "type", "text"},
                { "width", (int)DataDefine.Width.SIZE_1X1},
                { "data", new BsonDocument()},
                { "detailWidget", new BsonDocument()
                    {
                        { "data", new BsonArray()
                            {
                                new BsonDocument()
                                {
                                    { "deviceList", new BsonArray(){ }},
                                    { "record", new BsonArray(){ }}
                                },
                                new BsonDocument()
                                {
                                    { "deviceList", new BsonArray(){ }},
                                    { "record", new BsonArray(){ }}
                                }
                            }
                        },
                        { "item", new BsonArray()
                            {
                                {"Device Name"},
                                {"Device status"},
                                {"Device Owner"},
                                {"Device Time(UTC)"}
                            }
                        }
                    }
                }
            };

            mockRC.Setup(s => s.GetCache(0, token)).Returns("admin");
            mockAdminDB_multipleDashboard.Setup(d => d.WidgetInDashboard(dashboardId, widgetId)).Returns(false);
            mockDB.Setup(d => d.GetLastRawData("DashboardData")).Returns(document);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetWidgetDetails(token, dashboardId, widgetId, index);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetWidgetDetails_Success()
        {
            var token = "test";
            int dashboardId = 1;
            int widgetId = 1;
            int index = 0;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            BsonDocument document = new BsonDocument()
            {
                { "name", "Device status"},
                { "type", "text"},
                { "width", (int)DataDefine.Width.SIZE_1X1},
                { "data", new BsonDocument()},
                { "detailWidget", new BsonDocument()
                    {
                        { "record", new BsonArray()
                            {
                                new BsonArray()
                                {
                                    new BsonDocument()
                                    {
                                        {"alias", "TW002" },
                                        {"devName", "Device00002" },
                                        {"name", "TW002" },
                                        {"ownerName", "TW002" },
                                        {"storageSN", "TW002" },
                                        {"time", "TW002" },
                                        {"value", "TW002" },
                                    }
                                },
                                new BsonArray(){ }
                            }
                        },
                        { "item", new BsonArray()
                            {
                                {"Device Name"},
                                {"Device status"},
                                {"Device Owner"},
                                {"Device Time(UTC)"}
                            }
                        }
                    }
                }
            };

            mockRC.Setup(s => s.GetCache(0, token)).Returns("admin");
            mockAdminDB_multipleDashboard.Setup(d => d.WidgetInDashboard(dashboardId, widgetId)).Returns(true);
            mockDB.Setup(d => d.GetLastRawData("Widget" + widgetId)).Returns(document);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetWidgetDetails(token, dashboardId, widgetId, index);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetWidgetDetails_RecoedItemNull()
        {
            var token = "test";
            int dashboardId = 1;
            int widgetId = 1;
            int index = 0;

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            BsonDocument document = new BsonDocument()
            {
                { "name", "Device status"},
                { "type", "text"},
                { "width", (int)DataDefine.Width.SIZE_1X1},
                { "data", new BsonDocument()},
                { "detailWidget", new BsonDocument()
                    {
                    }
                }
            };

            mockRC.Setup(s => s.GetCache(0, token)).Returns("admin");
            mockAdminDB_multipleDashboard.Setup(d => d.WidgetInDashboard(dashboardId, widgetId)).Returns(true);
            mockDB.Setup(d => d.GetLastRawData("Widget" + widgetId)).Returns(document);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetWidgetDetails(token, dashboardId, widgetId, index);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }


        //[Test]
        //public void Dashboard_GetWidgetDetails_WidgetNotFound()
        //{
        //    var token = "test";
        //    int dashboardId = 1;
        //    int widgetId = 2;
        //    int index = 3;

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
        //    Mock<IEmployee> mockEmp = new Mock<IEmployee>();
        //    Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
        //    Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
        //    Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
        //    Mock<ISocket> mockSocketService = new Mock<ISocket>();

        //    BsonDocument document = new BsonDocument()
        //    {
        //        { "1", new BsonDocument()
        //            {
        //                { "name", "Device status"},
        //                { "type", "text"},
        //                { "width", (int)DataDefine.Width.SIZE_1X1},
        //                { "data", new BsonDocument()},
        //                { "detailWidget", new BsonDocument()
        //                    {
        //                        { "record", new BsonArray()
        //                            {
        //                                new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"alias", "TW002" },
        //                                        {"devName", "Device00002" },
        //                                        {"name", "TW002" },
        //                                        {"ownerName", "TW002" },
        //                                        {"storageSN", "TW002" },
        //                                        {"time", "TW002" },
        //                                        {"value", "TW002" },
        //                                    }
        //                                },
        //                                new BsonArray(){ }
        //                            }
        //                        },
        //                        { "item", new BsonArray()
        //                            {
        //                                {"Device Name"},
        //                                {"Device status"},
        //                                {"Device Owner"},
        //                                {"Device Time(UTC)"}
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("admin");
        //    mockAdminDB_multipleDashboard.Setup(d => d.WidgetInDashboard(dashboardId, widgetId)).Returns(true);
        //    mockDB.Setup(d => d.GetLastRawData("DashboardData")).Returns(document);

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.GetWidgetDetails(token, dashboardId, widgetId, index);

        //    //Assert          
        //    Assert.AreEqual(200, actual.StatusCode);
        //}



        //[Test]
        //public void Dashboard_GetWidgetDetails_RecordNotFound()
        //{
        //    var token = "test";
        //    int dashboardId = 1;
        //    int widgetId = 1;
        //    int index = 3;

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
        //    Mock<IEmployee> mockEmp = new Mock<IEmployee>();
        //    Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
        //    Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
        //    Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
        //    Mock<ISocket> mockSocketService = new Mock<ISocket>();

        //    BsonDocument document = new BsonDocument()
        //    {
        //        { "1", new BsonDocument()
        //            {
        //                { "name", "Device status"},
        //                { "type", "text"},
        //                { "width", (int)DataDefine.Width.SIZE_1X1},
        //                { "data", new BsonDocument()},
        //                { "detailWidget", new BsonDocument()
        //                    {
        //                        { "record", new BsonArray()
        //                            {
        //                                new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"alias", "TW002" },
        //                                        {"devName", "Device00002" },
        //                                        {"name", "TW002" },
        //                                        {"ownerName", "TW002" },
        //                                        {"storageSN", "TW002" },
        //                                        {"time", "TW002" },
        //                                        {"value", "TW002" },
        //                                    }
        //                                },
        //                                new BsonArray(){ }
        //                            }
        //                        },
        //                        { "item", new BsonArray()
        //                            {
        //                                {"Device Name"},
        //                                {"Device status"},
        //                                {"Device Owner"},
        //                                {"Device Time(UTC)"}
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("admin");
        //    mockAdminDB_multipleDashboard.Setup(d => d.WidgetInDashboard(dashboardId, widgetId)).Returns(true);
        //    mockDB.Setup(d => d.GetLastRawData("DashboardData")).Returns(document);

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.GetWidgetDetails(token, dashboardId, widgetId, index);

        //    //Assert          
        //    Assert.AreEqual(200, actual.StatusCode);
        //}


        [Test]
        public void Dashboard_GetHomePage_TokenErrorTest()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetHomePage(token);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetHomePage_DashboardNotFound()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.GetDashboardList()).Returns(new SelectOptionTemplate[] { null });

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetHomePage(token);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetHomePage_GetDashboardListException()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.GetDashboardList()).Throws(new Exception());


            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetHomePage(token);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetHomePage_GetWidgetListException()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
            mockAdminDB_multipleDashboard.Setup(d => d.GetDashboardList()).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Name = "Dashboard1" } });
            mockAdminDB_multipleDashboard.Setup(d => d.GetWidgetId(1)).Throws(new Exception());

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetHomePage(token);

            //Assert          
            Assert.AreEqual(500, actual.StatusCode);
        }

        //[Test]
        //public void Dashboard_GetHomePage_NotFound()
        //{
        //    var token = "test";

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
        //    Mock<IEmployee> mockEmp = new Mock<IEmployee>();
        //    Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
        //    Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
        //    Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
        //    Mock<ISocket> mockSocketService = new Mock<ISocket>();

        //    mockRC.Setup(r => r.GetCache(0, token)).Returns("test");
        //    mockAdminDB_multipleDashboard.Setup(d => d.GetDashboardList()).Returns(new SelectOptionTemplate[]
        //    {
        //                    new SelectOptionTemplate()
        //                    {
        //                        Id = 1,
        //                        Name = "Dashboard1"
        //                    }
        //    });
        //    mockDB.Setup(d => d.GetLastRawData("DashboardData")).Returns((BsonDocument)null);

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.GetHomePage(token);

        //    //Assert          
        //    Assert.AreEqual(200, actual.StatusCode);
        //}


        [Test]
        public void Dashboard_GetHomePage_Success()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            BsonDocument document = new BsonDocument()
            {
                { "_id", 123},
                { "name", "Device status"},
                { "type", "text"},
                { "width", (int)DataDefine.Width.SIZE_1X1},
                { "data", new BsonDocument()},
                { "detailWidget", new BsonDocument()}
            };

            mockRC.Setup(r => r.GetCache(0, token)).Returns("admin");
            mockAdminDB_multipleDashboard.Setup(d => d.GetDashboardList()).Returns(new SelectOptionTemplate[]
            {
                new SelectOptionTemplate()
                {
                    Id = 1,
                    Name = "Dashboard1"
                }
            });
            mockAdminDB_multipleDashboard.Setup(o => o.GetWidgetId(1)).Returns(new int[] { 11});
            mockDB.Setup(d => d.GetLastRawData("Widget" + 11)).Returns(document);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetHomePage(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetHomePage_Empty()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            mockRC.Setup(r => r.GetCache(0, token)).Returns("admin");
            mockAdminDB_multipleDashboard.Setup(d => d.GetDashboardList()).Returns(new SelectOptionTemplate[]
            {
                new SelectOptionTemplate()
                {
                    Id = 1,
                    Name = "Dashboard1"
                }
            });
            mockAdminDB_multipleDashboard.Setup(o => o.GetWidgetId(1)).Returns((int[])null);

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetHomePage(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetHomePage_DataDBWidgetNotFound()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            BsonDocument document = new BsonDocument()
            {
                { "_id", 11},
                { "name", "Device status"},
                { "type", "text"},
                { "width", (int)DataDefine.Width.SIZE_1X1},
                { "data", new BsonDocument()},
                { "detailWidget", new BsonDocument()}
            };

            mockRC.Setup(r => r.GetCache(0, token)).Returns("admin");
            mockAdminDB_multipleDashboard.Setup(d => d.GetDashboardList()).Returns(new SelectOptionTemplate[]
            {
                new SelectOptionTemplate()
                {
                    Id = 1,
                    Name = "Dashboard1"
                }
            });
            mockAdminDB_multipleDashboard.Setup(o => o.GetWidgetId(1)).Returns(new int[] { 11 });
            mockDB.Setup(d => d.GetLastRawData("Widget" + 11)).Returns((BsonDocument) null);
            mockAdminDB_widget.Setup(s => s.GetWidgetNameAndWidth(11)).Returns(
                new {
                    id = 11,
                    name = "test",
                    width = "col-md-12"}
            );
                
            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetHomePage(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Dashboard_GetHomePage_DataDBWidgetException()
        {
            var token = "test";

            //Arrange
            Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
            Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
            Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
            Mock<IEmployee> mockEmp = new Mock<IEmployee>();
            Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
            Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
            Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
            Mock<ISocket> mockSocketService = new Mock<ISocket>();

            BsonDocument document = new BsonDocument()
            {
                { "_id", 11},
                { "name", "Device status"},
                { "type", "text"},
                { "width", (int)DataDefine.Width.SIZE_1X1},
                { "data", new BsonDocument()},
                { "detailWidget", new BsonDocument()}
            };

            mockRC.Setup(r => r.GetCache(0, token)).Returns("admin");
            mockAdminDB_multipleDashboard.Setup(d => d.GetDashboardList()).Returns(new SelectOptionTemplate[]
            {
                new SelectOptionTemplate()
                {
                    Id = 1,
                    Name = "Dashboard1"
                }
            });
            mockAdminDB_multipleDashboard.Setup(o => o.GetWidgetId(1)).Returns(new int[] { 11 });
            mockDB.Setup(d => d.GetLastRawData("Widget" + 11)).Throws(new Exception());
            mockAdminDB_widget.Setup(s => s.GetWidgetNameAndWidth(11)).Returns(
                new
                {
                    id = 11,
                    name = "test",
                    width = "col-md-12"
                }
            );

            var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.GetHomePage(token);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        //[Test]
        //public void Dashboard_NotifyCoreService_TokenErrorTest()
        //{
        //    var token = "test";

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
        //    Mock<IEmployee> mockEmp = new Mock<IEmployee>();
        //    Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
        //    Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
        //    Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
        //    Mock<ISocket> mockSocketService = new Mock<ISocket>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns((string)null);

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.NotifyCoreService(token);

        //    //Assert          
        //    Assert.AreEqual(403, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_NotifyCoreService_ConnectionFailed()
        //{
        //    var token = "test";

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
        //    Mock<IEmployee> mockEmp = new Mock<IEmployee>();
        //    Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
        //    Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
        //    Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
        //    Mock<ISocket> mockSocketService = new Mock<ISocket>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");

        //    mockSocketService.Setup(s => s.Connect("172.30.0.16", 8787)).Returns(false);
        //    mockSocketService.Setup(s => s.Close());

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act
        //    StatusCodeResult actual = (StatusCodeResult)_target.NotifyCoreService(token);

        //    //Assert          
        //    Assert.AreEqual(500, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_NotifyCoreService_SendFailed()
        //{
        //    var token = "test";

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
        //    Mock<IEmployee> mockEmp = new Mock<IEmployee>();
        //    Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
        //    Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
        //    Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
        //    Mock<ISocket> mockSocketService = new Mock<ISocket>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockSocketService.Setup(s => s.Connect("172.30.0.16", 8787)).Returns(true);
        //    mockSocketService.Setup(s => s.SendMessage("Widget Changed")).Returns(false);
        //    mockSocketService.Setup(s => s.Close());

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act
        //    StatusCodeResult actual = (StatusCodeResult)_target.NotifyCoreService(token);

        //    //Assert          
        //    Assert.AreEqual(500, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_NotifyCoreService_ReceiveFailed()
        //{
        //    var token = "test";

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
        //    Mock<IEmployee> mockEmp = new Mock<IEmployee>();
        //    Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
        //    Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
        //    Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
        //    Mock<ISocket> mockSocketService = new Mock<ISocket>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockSocketService.Setup(s => s.Connect("172.30.0.16", 8787)).Returns(true);
        //    mockSocketService.Setup(s => s.SendMessage("Widget Changed")).Returns(true);
        //    mockSocketService.Setup(s => s.ReceiveMessage()).Returns((string)null);
        //    mockSocketService.Setup(s => s.Close());

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act
        //    StatusCodeResult actual = (StatusCodeResult)_target.NotifyCoreService(token);

        //    //Assert          
        //    Assert.AreEqual(500, actual.StatusCode);
        //}

        //[Test]
        //public void Dashboard_NotifyCoreService_Success()
        //{
        //    var token = "test";

        //    //Arrange
        //    Mock<IRedisCacheDispatcher> mockRC = new Mock<IRedisCacheDispatcher>();
        //    Mock<IMultipleDashboard> mockAdminDB_multipleDashboard = new Mock<IMultipleDashboard>();
        //    Mock<IDataDBDispatcher> mockDB = new Mock<IDataDBDispatcher>();
        //    Mock<IEmployee> mockEmp = new Mock<IEmployee>();
        //    Mock<IWidgetRepository> mockAdminDB_widget = new Mock<IWidgetRepository>();
        //    Mock<IEmail> mockAdminDB_email = new Mock<IEmail>();
        //    Mock<IDevice> mockAdminDB_device = new Mock<IDevice>();
        //    Mock<ISocket> mockSocketService = new Mock<ISocket>();

        //    mockRC.Setup(s => s.GetCache(0, token)).Returns("test");
        //    mockSocketService.Setup(s => s.Connect("172.30.0.16", 8787)).Returns(true);
        //    mockSocketService.Setup(s => s.SendMessage("Widget Changed")).Returns(true);
        //    mockSocketService.Setup(s => s.ReceiveMessage()).Returns("OK");
        //    mockSocketService.Setup(s => s.Close());

        //    var _target = new DashboardController(mockRC.Object, mockAdminDB_multipleDashboard.Object, mockDB.Object, mockEmp.Object, mockAdminDB_widget.Object, mockAdminDB_email.Object, mockAdminDB_device.Object);

        //    //Act
        //    StatusCodeResult actual = (StatusCodeResult)_target.NotifyCoreService(token);

        //    //Assert          
        //    Assert.AreEqual(500, actual.StatusCode);
        //}
    }
}
