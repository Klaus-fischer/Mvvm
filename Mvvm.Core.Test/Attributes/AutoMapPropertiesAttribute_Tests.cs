namespace Mvvm.Core.Test.Attributes
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mvvm.Core.CodeGeneration;

    [TestClass]
    public class AutoMapPropertiesAttribute_Tests
    {
        [TestMethod]
        public void Constructor()
        {
            var attr = new AutoMapPropertiesAttribute();
            Assert.IsNotNull(attr);
            Assert.AreEqual(0, attr.Except.Length);
        }

        [TestMethod]
        public void Constructor_Params()
        {
            var attr = new AutoMapPropertiesAttribute("Test1", "Test2");
            Assert.IsNotNull(attr);
            Assert.AreEqual(2, attr.Except.Length);
            Assert.AreEqual("Test1", attr.Except[0]);
            Assert.AreEqual("Test2", attr.Except[1]);
        }
    }
}
