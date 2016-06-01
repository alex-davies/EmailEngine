using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmailEngine.Razor;
using EmailEngine.Test.Models;
using System.Net.Mail;
using EmailEngine.Generator;

namespace EmailEngine.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class TestCompilation
    {
        EmailGenerator emailGenerator { get; set; }

        [TestInitialize]
        public void Setup()
        {
            emailGenerator = new EmailGenerator();
        }

        [TestMethod]
        public void TestFixedTemplate()
        {
            AssertTemplate("Simple Template", "Simple Template");
           
        }

        [TestMethod]
        public void TestSimpleTemplate()
        {
            AssertTemplate(@"<ul><li>1</li><li>2</li><li>3</li></ul>", @"<ul>@for(int i=1;i<=3;i++) {<li>@i</li>}</ul>");
        }

        [TestMethod]
        public void TestTemplateWithLinq()
        {
            AssertTemplate(@"<ul><li>1</li><li>2</li><li>3</li></ul>", @"<ul>@foreach(var s in new int[]{1,2,3}.Select(n=>n.ToString())){<li>@s</li>}</ul>");
        }

        [TestMethod]
        [ExpectedException(typeof(CompilationException))]
        public void TestIllegalTemplate()
        {
            AssertTemplate(null, @"<ul>@for(<ul>){<li></li>}");
        }

       
        [TestMethod]
        public void TestDynamicModel()
        {
            AssertTemplate("Hello Alex", "Hello @Model.Name", new { Name = "Alex" });
        }

        [TestMethod]
        public void TestExplicitDynamicModel()
        {
            AssertTemplate("Hello Alex", @"@model dynamic
Hello @Model.Name", new { Name = "Alex" });
        }

        [TestMethod]
        public void TestExplicitDynamicWithTypedModel()
        {
            AssertTemplate("Hello Alex", @"@model dynamic
Hello @Model.Name", new Person { Name = "Alex" });
        }

        [TestMethod]
        public void TestTypedModel()
        {
            AssertTemplate("Hello Alex", @"@model EmailEngine.Test.Models.Person
Hello @Model.Name", new Person { Name = "Alex" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestTypedIncorrectModel()
        {
            AssertTemplate("Hello Alex", @"@model EmailEngine.Test.Models.Person
Hello @Model.Name", new { Name = "Alex" });
        }

        public void AssertTemplate(string expectedOutput, string templateString, object model = null)
        {
            Assert.AreEqual(expectedOutput, emailGenerator.Generate(templateString, model, new string[0]).Body);
        }




    }
   
}
