namespace SIM.Mvvm.CodeGeneration.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AutoMapTest
    {
        [TestMethod]
        public void Constructor()
        {
            var vm = new AutoMapViewModel();
            Assert.IsNotNull(vm);
            Assert.IsNotNull(vm.Data);
        }

        [TestMethod]
        public void PropertyAccessTest()
        {
            Assert.IsNotNull(typeof(AutoMapViewModel).GetProperty(nameof(Model.Age)), "Property Age was not found.");
            Assert.IsNotNull(typeof(AutoMapViewModel).GetProperty(nameof(Model.Name)), "Property Name was not found.");
            Assert.IsNotNull(typeof(AutoMapViewModel).GetProperty(nameof(Model.IsAdmin)), "Property IsAdmin was not found.");
        }

        [TestMethod]
        public void ReadTest()
        {
            var vm = new AutoMapViewModel();

            Assert.AreEqual(vm.Data.Name, vm.GetValue("Name"));
            Assert.AreEqual(vm.Data.Age, vm.GetValue("Age"));
            Assert.AreEqual(vm.Data.IsAdmin, vm.GetValue("IsAdmin"));
        }

        [TestMethod]
        public void WriteTest()
        {
            var propertyChangedEventCount = 0;
            var vm = new AutoMapViewModel();

            vm.PropertyChanged += (s, a) => propertyChangedEventCount++;

            vm.SetValue("Name", "TestOfficer");
            vm.SetValue("Age", 30);
            vm.SetValue("IsAdmin", false);

            Assert.AreEqual(3, propertyChangedEventCount);
            Assert.AreEqual("TestOfficer", vm.Data.Name);
            Assert.AreEqual(30, vm.Data.Age);
            Assert.AreEqual(false, vm.Data.IsAdmin);
        }
    }
}
