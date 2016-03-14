using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicProject.Controllers;
using System.Web.Mvc;
using MusicProject.Models;

namespace MusicProject.Tests.Controllers
{
    /// <summary>
    /// Summary description for CompaniesControllerTest
    /// </summary>
    [TestClass]
    public class CompaniesControllerTest    {
       
        public CompaniesControllerTest()
        {
            
        }

        [TestMethod]
        public void QuickSearch(string name)
        {
            CompaniesController controller = new CompaniesController();

            ViewResult result = controller.QuickSearch("JYP") as ViewResult;
            List<Company> model = result.Model as List<Company>;
            Assert.AreEqual(1, model.Count);
                       
        }

        [TestMethod]
        public void Index(string name, string currentFilter, string searchString)
        {
            
        }
    }
}
