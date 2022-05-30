namespace SIM.Mvvm.CodeGeneration.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Reflection;

    [TestClass]
    public class AutoGeneratePropertiesTest
    {
        [TestMethod]
        public void Constructor()
        {
            var vm = new AutoGenerateViewModel();
            Assert.IsNotNull(vm);
            Assert.IsNotNull(vm._TestString);
        }

        [TestMethod]
        public void PropertyAccessTest()
        {
            Assert.IsNotNull(typeof(AutoGenerateViewModel).GetProperty("TestString"), "Property TestString was not found.");
            Assert.IsNotNull(typeof(AutoGenerateViewModel).GetProperty("TestInteger"), "Property TestInteger was not found.");
            Assert.IsNull(typeof(AutoGenerateViewModel).GetProperty("TestBool"), "Property TestBool was found, should be private.");

            Assert.IsNotNull(
                typeof(AutoGenerateViewModel).GetProperty("TestBool", BindingFlags.Instance | BindingFlags.NonPublic),
                "Private Property TestBool was not found.");
        }

        [TestMethod]
        public void ReadTest()
        {
            var vm = new AutoGenerateViewModel();

            Assert.AreEqual(vm._TestString, vm.GetValue("TestString"));
            Assert.AreEqual(vm.testInteger, vm.GetValue("TestInteger"));
        }

        [TestMethod]
        public void WriteTest()
        {
            var propertyChangedEventCount = 0;
            var vm = new AutoGenerateViewModel();

            vm.PropertyChanged += (s, a) => propertyChangedEventCount++;

            vm.SetValue("TestString", "TestOfficer");
            vm.SetValue("TestBool", true);
            vm.SetValue("TestInteger", 21);

            Assert.AreEqual(3, propertyChangedEventCount);
            Assert.AreEqual("TestOfficer", vm._TestString);
            Assert.AreEqual(true, vm._testBool);
            Assert.AreEqual(21, vm.testInteger);
        }
    }
}
