namespace SIM.Mvvm.Test.Controller
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using static SIM.Mvvm.Test.Extensions.Expression_Tests;

    [TestClass]
    public class PropertyMonitorTests
    {
        [TestMethod]
        public void Constructor_Test()
        {
            var vm = new Mock<INotifyPropertyChanged>();

            var monitor = new PropertyMonitor(vm.Object, "TestProperty");

            Assert.IsNotNull(monitor);
            Assert.AreEqual("TestProperty", monitor.PropertyName);
        }

        [TestMethod]
        public void OnPropertyChanged_Test()
        {
            var vm = new Mock<INotifyPropertyChanged>();
            var monitor = new PropertyMonitor(vm.Object, "TestProperty");

            var onPropertyChangedRaised = false;
            monitor.OnPropertyChanged += (s, a) =>
            {
                onPropertyChangedRaised = true;
                Assert.AreEqual(vm.Object, s);
                Assert.AreEqual("TestProperty", a.PropertyName);
                Assert.AreEqual("TestValue", a.Before);
                Assert.AreEqual("NewValue", a.After);
            };

            vm.Raise(o => o.PropertyChanged += null, new AdvancedPropertyChangedEventArgs("TestProperty", "TestValue", "NewValue"));

            Assert.IsTrue(onPropertyChangedRaised);
        }

        [TestMethod]
        public void OnPropertyChangedCallback_Test()
        {
            var vm = new Mock<INotifyPropertyChanged>();

            var monitor = new PropertyMonitor(vm.Object, "TestProperty");

            var onPropertyChangedRaised = false;
            monitor.OnPropertyChangedCallback += () =>
            {
                onPropertyChangedRaised = true;
            };

            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));

            Assert.IsTrue(onPropertyChangedRaised);
        }

        [TestMethod]
        public void OnPropertyChangedCallback_WrongPropertyName_Test()
        {
            var vm = new Mock<INotifyPropertyChanged>();
            var comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(o => o.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var monitor = new PropertyMonitor(vm.Object, "TestProperty");

            var onPropertyChangedRaised = false;
            monitor.OnPropertyChangedCallback += () =>
            {
                onPropertyChangedRaised = true;
            };

            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty2"));

            Assert.IsFalse(onPropertyChangedRaised);
        }

        [TestMethod]
        public void OnPropertyChangedViewModel_Test()
        {
            var vm = new Mock<IViewModel>();
            vm.Setup(o => o.OnPropertyChanged("TargetProperty"));

            var comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(o => o.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            IPropertyMonitor monitor = new PropertyMonitor(vm.Object, "TestProperty");

            monitor.RegisterViewModelProperty(vm.Object, "TargetProperty");

            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));
            vm.Verify(o => o.OnPropertyChanged("TargetProperty"), Times.Once);

            monitor.UnregisterViewModelProperty(vm.Object, "TargetProperty");

            // raise should not increment invocations after de-registration
            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));
            vm.Verify(o => o.OnPropertyChanged("TargetProperty"), Times.Once);
        }
    }
}
