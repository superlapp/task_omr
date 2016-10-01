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
        public void SearchBusStops()
        {
            // Arrange
            TicketsController controller = new TicketsController();

            // Act
            ViewResult result = controller.SearchBusStops() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}