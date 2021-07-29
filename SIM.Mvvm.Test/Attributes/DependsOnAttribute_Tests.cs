namespace SIM.Mvvm.Test.Attributes
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class DependsOnAttribute_Tests
    {
        [TestMethod]
        public void Constructor()
        {
            var attr = new DependsOnAttribute("Test");
            Assert.IsNotNull(attr);
            Assert.AreEqual(1, attr.PropertyNames.Length);
            Assert.AreEqual("Test", attr.PropertyNames[0]);
        }

        [TestMethod]
        public void Constructor_Params()
        {
            var attr = new DependsOnAttribute("Test", "Test1", "Test3");
            Assert.IsNotNull(attr);
            Assert.AreEqual(3, attr.PropertyNames.Length);
            Assert.AreEqual("Test", attr.PropertyNames[0]);
            Assert.AreEqual("Test1", attr.PropertyNames[1]);
            Assert.AreEqual("Test3", attr.PropertyNames[2]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_Fail()
        {
            _ = new DependsOnAttribute(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_Fail2()
        {
            _ = new DependsOnAttribute(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_Fail3()
        {
            _ = new DependsOnAttribute(" ");
        }
    }
}
