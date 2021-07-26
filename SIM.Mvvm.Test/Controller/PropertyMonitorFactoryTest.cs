namespace SIM.Mvvm.Test.Controller
{
    using System.ComponentModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class PropertyMonitorFactoryTest
    {
        [TestMethod]
        public void CreateTest()
        {
            var mock = new Mock<IViewModelMock>();
            mock.SetupProperty(o => o.Test, "Test");

            var monitor = PropertyMonitorFactory.Create(mock.Object, nameof(IViewModelMock.Test));

            Assert.IsNotNull(monitor);
        }

        [TestMethod]
        public void InvokeTest()
        {
            var onPropertyChangedRaised = false;
            var mock = new Mock<IViewModelMock>();
            mock.SetupProperty(o => o.Test, "Test");

            var monitor = PropertyMonitorFactory.Create(mock.Object, nameof(IViewModelMock.Test));

            monitor.OnPropertyChanged += (s, a) =>
            {
                onPropertyChangedRaised = true;
                Assert.AreEqual(mock.Object, s);
                Assert.AreEqual("Test", a.Before);
                Assert.AreEqual("After", a.After);
            };

            mock.Object.Test = "After";

            mock.Raise(o => o.PropertyChanged -= null, new PropertyChangedEventArgs(nameof(IViewModelMock.Test)));

            Assert.IsTrue(onPropertyChangedRaised);
        }

        public interface IViewModelMock : INotifyPropertyChanged
        {
            string Test { get; set; }
        }
    }
}
