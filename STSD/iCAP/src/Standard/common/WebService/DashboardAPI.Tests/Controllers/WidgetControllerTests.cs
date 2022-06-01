using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DashboardAPI.Controllers;
using DashboardAPI.Models;
using ShareLibrary;
using ShareLibrary.DataTemplate;
using ShareLibrary.Interface;
using ShareLibrary.AdminDB;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using DashboardAPI.Models.Widget;
using Microsoft.EntityFrameworkCore;




namespace DashboardAPI.Tests.Controllers
{
    public class WidgetControllerTests
    {
        [Test]
        public void Widget_Create_TokenErrorTest()
        {

            string widgetName = "test";
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);         

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Create(token, widgetName);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }
        
        [Test]
        public void Widget_Create_WidgetNameNullTest()
        {
            string widgetName = "test";
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();
            Exception exc = new Exception("Please enter the widget name.");

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(repo => repo.Create(widgetName)).Throws(new DbUpdateException("", exc));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Create(token, widgetName);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_Create_HaveNotGetAccessTest()
        {
            string widgetName = "test";
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();
            Exception exc = new Exception("Please enter the widget name.");

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(repo => repo.Create(widgetName)).Throws(new DbUpdateException("", exc));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Create(token, widgetName);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Widget_Create_CreateFailTest()
        {
            string widgetName = "test";
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();
            Exception exc = new Exception("Failed to create the widget! Please refresh the web page and try again.");

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(repo => repo.Create(widgetName)).Throws(new DbUpdateException("", exc));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Create(token, widgetName);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }
        
        [Test]
        public void Widget_Create_SuccessTest()
        {
            string widgetName = "test";
            string token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");        
            mockRepo.Setup(repo => repo.Create(widgetName));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Create(token, widgetName);

            //Assert          
            Assert.AreEqual(201, actual.StatusCode);

        }

        [Test]
        public void Widget_Create_Exception()
        {
            string widgetName = "test";
            string token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(repo => repo.Create(widgetName)).Throws(new Exception());
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Create(token, widgetName);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        //[Test]
        //public void Widget_Create_TokenErrorTest()
        //{

        //    WidgetTemplate wid = new WidgetTemplate();                     

        //    var token = "test";           

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);            
        //    mockRepo.Setup(c => c.CheckDataExists(wid.DataId)).Returns(true).Verifiable();


        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Create(token, wid);

        //    //Assert          
        //    Assert.AreEqual(403, actual.StatusCode);
        //}

        //[Test]
        //public void Widget_Create_InputNullTest()
        //{
        //    WidgetTemplate wid = null;
        //    var token = "test";         

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockRepo.Setup(repo => repo.Create(wid)).Throws(new Exception());

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual =(ObjectResult)_target.Create(token, wid);

        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Widget_Create_InputBranchIdNullTest()
        //{
        //    WidgetTemplate wid = new WidgetTemplate() { DataId = 20, BranchIdList = new int[] {} };

        //    var token = "test";

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();
        //    Exception exc = new Exception("FK_Branch_To_Widget");

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockRepo.Setup(repo => repo.Create(wid)).Throws(new Exception());

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Create(token, wid);

        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Widget_Create_BranchIdErrorTest()
        //{
        //    WidgetTemplate wid = new WidgetTemplate() { DataId=20 ,BranchIdList=new int[]{ 1,2,20} };

        //    var token = "test";           

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();
        //    Exception exc = new Exception("FK_Branch_To_Widget");

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");            
        //    mockRepo.Setup(repo => repo.Create(wid)).Throws(new Exception("The BRANCH could NOT be found! Please check the field or refresh the web page."));

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual =(ObjectResult)_target.Create(token, wid);

        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Widget_Create_DataIdErrorTest()
        //{
        //    WidgetTemplate wid = new WidgetTemplate() { WidgetId = 100 , BranchIdList = new int[] { 1, 2}, DataId =555};

        //    var token = "test";

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();
        //    Exception exc = new Exception("FK_Data_To_Widget");

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockRepo.Setup(repo => repo.Create(wid)).Throws(new DbUpdateException("FK_Data_To_Widget", exc));

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Create(token, wid);

        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Widget_Create_ChartIdErrorTest()
        //{
        //    WidgetTemplate wid = new WidgetTemplate() { WidgetId = 100 , BranchIdList = new int[] { 1, 2 } };

        //    var token = "test";

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();
        //    Exception exc = new Exception("FK_Chart_To_Widget");

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockRepo.Setup(repo => repo.Create(wid)).Throws(new DbUpdateException("FK_Chart_To_Widget", exc));

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Create(token, wid);

        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Widget_Create_WidthNullTest()
        //{
        //    WidgetTemplate wid = new WidgetTemplate() { Width=null , BranchIdList = new int[] { 1, 2 } };

        //    var token = "test";

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();
        //    Exception exc = new Exception("Width can not be null");

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockRepo.Setup(repo => repo.Create(wid)).Throws(new DbUpdateException("Width can not be null", exc));

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Create(token, wid);

        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Widget_Create_NameNullTest()
        //{
        //    WidgetTemplate wid = new WidgetTemplate() { Name = null , BranchIdList = new int[] { 1, 2 } };

        //    var token = "test";

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();
        //    Exception exc = new Exception("Name can not be null");

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockRepo.Setup(repo => repo.Create(wid)).Throws(new DbUpdateException("Name can not be null", exc));

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Create(token, wid);

        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Widget_Create_CreateFailTest()
        //{
        //    WidgetTemplate wid = new WidgetTemplate() { Name = null , BranchIdList = new int[] { 1, 2 } };

        //    var token = "test";

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();
        //    Exception exc = new Exception("Create Fail");

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
        //    mockRepo.Setup(repo => repo.Create(wid)).Throws(new DbUpdateException("", exc));

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Create(token, wid);

        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Widget_Create_InputSettingStrNullTest()
        //{
        //    WidgetTemplate wid = new WidgetTemplate() { SettingStr=null , BranchIdList = new int[] { 1, 2 } };
        //    var token = "test";


        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");            
        //    //mockRepo.Setup(repo => repo.CheckDataExists(wid.DataId)).Returns(true);
        //    mockRepo.Setup(repo => repo.Create(wid)).Throws(new Exception("Data Processing Error. Please Reset."));

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Create(token, wid);

        //    // Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Widget_Create_SuccessTest()
        //{
        //    WidgetTemplate wid = new WidgetTemplate() { BranchIdList = new int[] { 1, 2 } };
        //    string token = "test";                  

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");            
        //    mockRepo.Setup(repo => repo.CheckDataExists(wid.DataId)).Returns(true);                 
        //    mockRepo.Setup(repo => repo.Create(wid));

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Create(token, wid);

        //    //Assert          
        //    Assert.AreEqual(201, actual.StatusCode);

        //}

        //[Test]
        //public void Widget_Create_UnexceptedDataErrorTest()
        //{
        //    WidgetTemplate wid = new WidgetTemplate() { BranchIdList = new int[] { 1, 2 } };
        //    string token = "test";

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");            
        //    mockRepo.Setup(repo => repo.Create(wid)).Throws(new Exception());

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Create(token, wid);

        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);

        //}

        [Test]
        public void Widget_Edit_TokenErrorTest()
        {
            var widgetId = "1";
            var token = "test";

            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);//set not found the user from the Redis Cache                   


            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Edit(token, widgetId);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Widget_Edit_SuccessTest()
        {
            var widgetId = "1";
            var token = "test";

            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");//set not found the user from the Redis Cache                  
            mockRepo.Setup(w => w.Get(Int32.Parse(widgetId))).Returns(new WidgetTemplate());
            mockRepo.Setup(w => w.GetBranchSelect()).Returns(new SelectOptionTemplate[] { });
            mockRepo.Setup(w => w.GetChartSelect()).Returns(new SelectOptionTemplate[] { });
            mockRepo.Setup(w => w.GetChartSize()).Returns(new ChartSizeTemplate[] { });
            mockData.Setup(d => d.GetDataSource()).Returns(new DataSelect[] { });

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Edit(token, widgetId);

            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Widget_Edit_NullTest()
        {
            var widgetId = "1";
            var token = "test";

            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");//set not found the user from the Redis Cache                  
            mockRepo.Setup(w => w.Get(Int32.Parse(widgetId))).Returns((WidgetTemplate)null);
            mockRepo.Setup(w => w.GetBranchSelect()).Returns(new SelectOptionTemplate[] { });
            mockRepo.Setup(w => w.GetChartSelect()).Returns(new SelectOptionTemplate[] { });
            mockRepo.Setup(w => w.GetChartSize()).Returns(new ChartSizeTemplate[] { });
            mockData.Setup(d=>d.GetDataSource()).Returns(new DataSelect[] { });

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.Edit(token, widgetId);

            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Widget_Edit_InputNullTest()
        {
            var widgetId = (string)null;
            var token = "test";

            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");//set not found the user from the Redis Cache                  
            //mockRepo.Setup(w => w.Get(Int32.Parse(widgetId)));
            //mockRepo.Setup(w => w.GetDataSelect()).Returns(new DataSelect[] { });
            //mockRepo.Setup(w => w.GetBranchSelect()).Returns(new SelectOptionTemplate[] { });
            //mockRepo.Setup(w => w.GetChartSelect()).Returns(new SelectOptionTemplate[] { });
            //mockRepo.Setup(w => w.GetChartSize()).Returns(new ChartSizeTemplate[] { });

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Edit(token, widgetId);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_Edit_ExceptionTest()
        {
            var widgetId = "1";
            var token = "test";

            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");//set not found the user from the Redis Cache                  
            mockRepo.Setup(w => w.Get(Int32.Parse(widgetId))).Throws(new Exception());
            //mockRepo.Setup(w => w.GetDataSelect()).Returns(new DataSelect[] { });
            //mockRepo.Setup(w => w.GetBranchSelect()).Returns(new SelectOptionTemplate[] { });
            //mockRepo.Setup(w => w.GetChartSelect()).Returns(new SelectOptionTemplate[] { });
            //mockRepo.Setup(w => w.GetChartSize()).Returns(new ChartSizeTemplate[] { });

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Edit(token, widgetId);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        //[Test]
        //public void Widget_Get_TokenErrorTest()
        //{
        //    var widgetId = "1";
        //    var token = "test";           

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockBranch = new Mock<IBranch>();
        //    var mockThreshold = new Mock<IThreshold>();
        //    var mockData = new Mock<IData>();

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);//set not found the user from the Redis Cache                   
            

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Get(token, widgetId);
            
        //    //Assert          
        //    Assert.AreEqual(403, actual.StatusCode);
        //}

        //[Test]
        //public void Widget_Get_NotFoundTest()
        //{
        //    var widgetId = "100";
        //    var token = "test";
            
        //    //Arrange
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockEmp = new Mock<IEmployee>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockBranch = new Mock<IBranch>();
        //    var mockThreshold = new Mock<IThreshold>();
        //    var mockData = new Mock<IData>();

        //    mockRepo.Setup(repo => repo.Get(Int32.Parse(widgetId))).Returns((WidgetTemplate)null);            
        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    StatusCodeResult  actual = (StatusCodeResult)_target.Get(token, widgetId);        
            
        //    //Assert        
        //    Assert.AreEqual(204, actual.StatusCode);
        //}
        //[Test]
        //public void Widget_Get_SuccessTest()
        //{
        //    var widgetId = "100";
        //    var token = "test";
         
        //    //Arrange
            
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockEmp = new Mock<IEmployee>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockBranch = new Mock<IBranch>();
        //    var mockThreshold = new Mock<IThreshold>();
        //    var mockData = new Mock<IData>();

        //    mockRepo.Setup(repo => repo.Get(Int32.Parse(widgetId))).Returns(new WidgetTemplate());          
        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Get(token, widgetId);
        //    //Assert         
        //    Assert.AreEqual(200, actual.StatusCode);
        //}

        //[Test]
        //public void Widget_Get_WidgetIdNullTest()
        //{
        //    string widgetId = null;
        //    var token = "test";

        //    //Arrange

        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockEmp = new Mock<IEmployee>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockBranch = new Mock<IBranch>();
        //    var mockThreshold = new Mock<IThreshold>();
        //    var mockData = new Mock<IData>();


        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Get(token, widgetId);
        //    //Assert         
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        //[Test]
        //public void Widget_Get_FailTest()
        //{
        //    var widgetId = "100";
        //    var token = "test";

        //    //Arrange

        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockEmp = new Mock<IEmployee>();
        //    var mockDB = new Mock<IDataDBDispatcher>();
        //    var mockBranch = new Mock<IBranch>();
        //    var mockThreshold = new Mock<IThreshold>();
        //    var mockData = new Mock<IData>();

        //    mockRepo.Setup(repo => repo.Get(Int32.Parse(widgetId))).Throws(new Exception());
        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.Get(token, widgetId);
        //    //Assert         
        //    Assert.AreEqual(400, actual.StatusCode);
        //}
        //[Test]
        //public void WidgetGetIdErrorTest()
        //{
        //    int? Id =null;
        //    var token = "test";          

        //    //Arrange

        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockRC = new Mock<IRedisCacheDispatcher>();

        //    mockRepo.Setup(repo => repo.Get(Id)).Returns(new WidgetTemplate());
        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object);

        //    //Act
        //    ObjectResult actual =(ObjectResult)_target.Get(Id,token);

        //    //Assert         
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        [Test]
        public void Widget_Update_TokenErrorTest()
        {
            WidgetTemplate wid = new WidgetTemplate();
            var token = "test";           

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);//set not found the user from the Redis Cache                   
            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, wid);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Widget_Update_PayloadNullTest()
        {
            WidgetTemplate wid = null;
            var token = "test";            

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(s => s.Update(wid)).Throws(new ArgumentException());
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            var mockDB = new Mock<IDataDBDispatcher>();

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, wid);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_Update_BranchIdErrorTest()
        {
            WidgetTemplate wid = new WidgetTemplate();
            var token = "test";            

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            Exception exc = new Exception("FK_Branch_To_Widget");

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(s => s.Update(wid)).Throws(new DbUpdateException("",exc));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);


            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, wid);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_Update_InputBranchIdNullTest()
        {
            WidgetTemplate wid = new WidgetTemplate() { BranchIdList=new int[] { } };
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            Exception exc = new Exception("FK_Branch_To_Widget");

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(s => s.Update(wid)).Throws(new Exception());
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, wid);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }
        [Test]
        public void Widget_Update_DataIdErrorTest()
        {
            WidgetTemplate wid = new WidgetTemplate();
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            Exception exc = new Exception("FK_Data_To_Widget");

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(s => s.Update(wid)).Throws(new DbUpdateException("", exc));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, wid);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_Update_ChartIdErrorTest()
        {
            WidgetTemplate wid = new WidgetTemplate();
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            Exception exc = new Exception("FK_Chart_To_Widget");

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(s => s.Update(wid)).Throws(new DbUpdateException("", exc));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, wid);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_Update_WidthNullTest()
        {
            WidgetTemplate wid = new WidgetTemplate() { BranchIdList=new int[]{1,2} };
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            Exception exc = new Exception("Width can not be null");

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(s => s.Update(wid)).Throws(new Exception());
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, wid);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_Update_NameNullTest()
        {
            WidgetTemplate wid = new WidgetTemplate() { BranchIdList = new int[] { 1,2}, Width="test" };
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            Exception exc = new Exception("Name can not be null");

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(s => s.Update(wid)).Throws(new Exception());
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, wid);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_Update_UpdateFailTest()
        {
            WidgetTemplate wid = new WidgetTemplate();
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            Exception exc = new Exception("");

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(s => s.Update(wid)).Throws(new DbUpdateException("", exc));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, wid);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_Update_UnexceptedDataErrorTest()
        {
            WidgetTemplate wid = new WidgetTemplate() { BranchIdList = new int[] { 1, 2 } ,Name="test",Width="test"};
            var token = "test";         

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");          
            mockRepo.Setup(repo => repo.Update(wid)).Throws(new Exception());
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, wid);

            // Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_Update_SuccessTest()
        {
            WidgetTemplate wid = new WidgetTemplate();
            var token = "test";            

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(repo => repo.CheckDataExists(wid.DataId)).Returns(true);
            mockRepo.Setup(repo => repo.Update(wid));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, wid);

            // Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Widget_Update_HaveNotGetAccessTest()
        {
            WidgetTemplate wid = new WidgetTemplate();
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(repo => repo.CheckDataExists(wid.DataId)).Returns(true);
            mockRepo.Setup(repo => repo.Update(wid));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);           

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Update(token, wid);

            // Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Widget_Delete_TokenErrorTest()
        {
            string Id = "1";
            var token = "test";           

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);//set not found the user from the Redis Cache                   
            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(token, Id);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }
        [Test]
        public void Widget_Delete_ExceptionTest()
        {
            string Id = "1";
            var token = "test";           

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");                     
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);
            mockRepo.Setup(repo => repo.Delete(Int32.Parse(Id))).Throws(new Exception());

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(token, Id);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_Delete_WidgetIdNullTest()
        {
            string widgetId = null;
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);          

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(token, widgetId);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_Delete_WidgetIdFormatErrorTest()
        {
            string widgetId = "abc";
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(token, widgetId);

            //Assert          
            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_Delete_NotAdminTest()
        {
            string Id = "1";
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(repo => repo.Delete(Int32.Parse(Id)));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(false);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(token, Id);

            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }


        [Test]
        public void Widget_Delete_SuccessTest()
        {
            string Id = "1";
            var token = "test";          

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(repo => repo.Delete(Int32.Parse(Id)));
            mockEmp.Setup(e => e.CheckAdmin("test")).Returns(true);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            //Act
            ObjectResult actual = (ObjectResult)_target.Delete(token, Id);

            //Assert          
            Assert.AreEqual(202, actual.StatusCode);
        }

        [Test]
        public void Widget_GetDataGroupList_TokenErrorTest()
        {           
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);            

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetDataGroupList(token);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Widget_GetDataGroupList_SuccessTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetDataGroupList()).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Name = "System" } });

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetDataGroupList(token);
            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Widget_GetDataGroupList_SuccessNotFoundTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetDataGroupList()).Returns(new SelectOptionTemplate[] {null});

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetDataGroupList(token);
            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Widget_GetDataGroupList_ExceptionTest()
        {
            var token = "test";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetDataGroupList()).Throws(new Exception());

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            ObjectResult actual = (ObjectResult)_target.GetDataGroupList(token);

            Assert.AreEqual(500, actual.StatusCode);
        }

        [Test]
        public void Widget_GetNumbericalDataList_TokenErrorTest()
        {
            var token = "test";
            var groupId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetNumbericalDataList(token);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Widget_GetNumbericalDataList_SuccessTest()
        {
            var token = "test";
            var groupId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r =>r.GetNumbericalDataList()).Returns(new NumbericalDataOption());

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetNumbericalDataList(token);
            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Widget_GetNumbericalDataList_SuccessNotFoundTest()
        {
            var token = "test";
            var groupId = "6";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetNumbericalDataList()).Returns((NumbericalDataOption)null);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetNumbericalDataList(token);
            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }


        //[Test]
        //public void Widget_GetNumbericalDataList_InputNullTest()
        //{
        //    var token = "test";
        //    string groupId = null;

        //    //Arrange
        //    var mockRC = new Mock<IRedisCacheDispatcher>();
        //    var mockRepo = new Mock<IWidgetRepository>();
        //    var mockEmp = new Mock<IEmployee>();

        //    mockRC.Setup(t => t.GetCache(0, token)).Returns("test");            

        //    var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
        //    //Act
        //    ObjectResult actual = (ObjectResult)_target.GetNumbericalDataList(token, groupId);
        //    //Assert          
        //    Assert.AreEqual(400, actual.StatusCode);
        //}

        [Test]
        public void Widget_GetNumbericalDataList_ExceptionTest()
        {
            var token = "test";
            var groupId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetNumbericalDataList()).Throws(new Exception());

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            ObjectResult actual = (ObjectResult)_target.GetNumbericalDataList(token);

            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_GetChartList_TokenErrorTest()
        {
            var token = "test";
            var dataId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetChartList(token, dataId);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Widget_GetChartList_SuccessTest()
        {
            var token = "test";
            var dataId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetChartSelect(Int32.Parse(dataId))).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Name = "bar" } });

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetChartList(token, dataId);
            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Widget_GetChartList_SuccessNotFoundTest()
        {
            var token = "test";
            var dataId = "999";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetChartSelect(Int32.Parse(dataId))).Returns(new SelectOptionTemplate[] { null });

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetChartList(token, dataId);
            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Widget_GetChartList_InputNullTest()
        {
            var token = "test";
            string dataId = null;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            ObjectResult actual = (ObjectResult)_target.GetChartList(token, dataId);

            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_GetChartList_UnceptedErrorTest()
        {
            var token = "test";
            var dataId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetChartSelect(Int32.Parse(dataId))).Throws(new Exception());

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            ObjectResult actual = (ObjectResult)_target.GetChartList(token, dataId);

            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_GetChartSize_TokenErrorTest()
        {
            var token = "test";
            var chartId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetChartSize(token, chartId);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Widget_GetChartSize_ChartNotFoundTest()
        {
            var token = "test";
            var chartId = "10";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetChartSize(Int32.Parse(chartId))).Returns((ChartSizeTemplate)null);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetChartSize(token, chartId);
            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Widget_GetChartSize_SuccessTest()
        {
            var token = "test";
            var chartId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetChartSize(Int32.Parse(chartId))).Returns(new ChartSizeTemplate (){Large=true, Medium=true, Small=true});

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetChartSize(token, chartId);
            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Widget_GetChartSize_InputNullTest()
        {
            var token = "test";
            string chartId = null;

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");      

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            ObjectResult actual = (ObjectResult)_target.GetChartSize(token, chartId);

            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_GetChartSize_UnexceptedErrorTest()
        {
            var token = "test";
            var chartId = "1";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetChartSize(Int32.Parse(chartId))).Throws(new Exception());

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            ObjectResult actual = (ObjectResult)_target.GetChartSize(token, chartId);

            Assert.AreEqual(400, actual.StatusCode);
        }

        [Test]
        public void Widget_GetWidgetList_TokenErrorTest()
        {
            var token = "test";           

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetWidgetList(token);
            //Assert          
            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Widget_GetWidgetList_SuccessTest()
        {
            var token = "test";           

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetWidgetList()).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Name = "CPU Loading" } });

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            ObjectResult actual = (ObjectResult)_target.GetWidgetList(token);
            //Assert          
            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Widget_GetWidgetList_WidgetNotFoundTest()
        {
            var token = "test";           

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetWidgetList()).Returns(new SelectOptionTemplate[] { null });

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);
            //Act
            StatusCodeResult actual = (StatusCodeResult)_target.GetWidgetList(token);
            //Assert          
            Assert.AreEqual(204, actual.StatusCode);
        }

        [Test]
        public void Widget_GetWidgetList_ExceptionTest()
        {
            var token = "test";            

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockRepo.Setup(r => r.GetWidgetList()).Throws(new Exception());

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            ObjectResult actual = (ObjectResult)_target.GetWidgetList(token);

            Assert.AreEqual(500, actual.StatusCode);
        }

        [Test]
        public void Widget_GetDeviceInfo_TokenError()
        {
            var token = "test";
            var devName = "Device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns((string)null);

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            ObjectResult actual = (ObjectResult)_target.GetDeviceInfo(token, devName);

            Assert.AreEqual(403, actual.StatusCode);
        }

        [Test]
        public void Widget_GetDeviceInfo_Success()
        {
            var token = "test";
            var devName = "Device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockBranch.Setup(b => b.GetBranchList(devName)).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Name = "All Devices"} });
            mockEmp.Setup(e => e.GetName()).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Name = "Admin"} });

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            ObjectResult actual = (ObjectResult)_target.GetDeviceInfo(token, devName);

            Assert.AreEqual(200, actual.StatusCode);
        }

        [Test]
        public void Widget_GetDeviceInfo_Exception()
        {
            var token = "test";
            var devName = "Device00001";

            //Arrange
            var mockRC = new Mock<IRedisCacheDispatcher>();
            var mockRepo = new Mock<IWidgetRepository>();
            var mockEmp = new Mock<IEmployee>();
            var mockDB = new Mock<IDataDBDispatcher>();
            var mockBranch = new Mock<IBranch>();
            var mockThreshold = new Mock<IThreshold>();
            var mockData = new Mock<IData>();

            mockRC.Setup(t => t.GetCache(0, token)).Returns("test");
            mockBranch.Setup(b => b.GetBranchList(devName)).Throws(new Exception());
            mockEmp.Setup(e => e.GetName()).Returns(new SelectOptionTemplate[] { new SelectOptionTemplate() { Id = 1, Name = "Admin" } });

            var _target = new WidgetController(mockRepo.Object, mockRC.Object, mockEmp.Object, mockDB.Object, mockBranch.Object, mockThreshold.Object, mockData.Object);

            ObjectResult actual = (ObjectResult)_target.GetDeviceInfo(token, devName);

            Assert.AreEqual(500, actual.StatusCode);
        }
    }
}