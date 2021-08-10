namespace SIM.Mvvm.CodeGeneration.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AutoMapExceptTest
    {
        [TestMethod]
        public void Constructor()
        {
            var vm = new AutoMapExceptViewModel();
            Assert.IsNotNull(vm);
            Assert.IsNotNull(vm.Data);
        }

        [TestMethod]
        public void PropertyAccessTest()
        {
            Assert.IsNull(typeof(AutoMapExceptViewModel).GetProperty(nameof(Model.Age)), "Property Age was found, but it should not.");
            Assert.IsNotNull(typeof(AutoMapExceptViewModel).GetProperty(nameof(Model.Name)), "Property Name was not found.");
            Assert.IsNotNull(typeof(AutoMapExceptViewModel).GetProperty(nameof(Model.IsAdmin)), "Property IsAdmin was not found.");

        }

        [TestMethod]
        public void ReadTest()
        {
            var vm = new AutoMapExceptViewModel();

            Assert.AreEqual(vm.Data.Name, vm.GetValue("Name"));
            Assert.AreEqual(vm.Data.IsAdmin, vm.GetValue("IsAdmin"));
        }

        [TestMethod]
        public void WriteTest()
        {
            var propertyChangedEventCount = 0;
            var vm = new AutoMapExceptViewModel();

            vm.PropertyChanged += (s, a) => propertyChangedEventCount++;

            vm.SetValue("Name", "TestOfficer");
            vm.SetValue("IsAdmin", false);

            Assert.AreEqual(2, propertyChangedEventCount);
            Assert.AreEqual("TestOfficer", vm.Data.Name);
            Assert.AreEqual(false, vm.Data.IsAdmin);
        }
    }
}
