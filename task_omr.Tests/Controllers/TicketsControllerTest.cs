using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using task_omr;
using task_omr.Controllers;

namespace task_omr.Tests.Controllers
{
    [TestClass]
    public class TicketsControllerTest
    {
        [TestMethod]
        public void Test_SearchBusStops()
        {
            var controller = new TicketsController();
            var result = controller.SearchBusStops() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Test_OrdersInfo()
        {
            var controller = new TicketsController();
            var result = controller.OrdersInfo() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Test_ProcessOrder()
        {
            var controller = new TicketsController();
            var result = controller.ProcessOrder(Helpers.Helper.ORDER_STATUS_PURCHASED, 1) as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}