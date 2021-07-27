namespace SIM.Mvvm.Test.Controller
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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

            var monitor = new PropertyMonitor<string>(vm.Object, "TestProperty", () => "TestValue", null);

            Assert.IsNotNull(monitor);
            Assert.AreEqual("TestProperty", monitor.PropertyName);
        }

        [TestMethod]
        public void OnPropertyChanged_Test()
        {
            var vm = new Mock<INotifyPropertyChanged>();
            var comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(o => o.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var value = "TestValue";
            var monitor = new PropertyMonitor<string>(vm.Object, "TestProperty", () => value, comparer.Object);

            value = "NewValue";

            var onPropertyChangedRaised = false;
            monitor.OnPropertyChanged += (s, a) =>
            {
                onPropertyChangedRaised = true;
                Assert.AreEqual(vm.Object, s);
                Assert.AreEqual("TestProperty", a.PropertyName);
                Assert.AreEqual("TestValue", a.Before);
                Assert.AreEqual("NewValue", a.After);
            };

            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));

            Assert.IsTrue(onPropertyChangedRaised);
            comparer.Verify(o => o.Equals(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
        }

        [TestMethod]
        public void OnPropertyChangedCallback_Test()
        {
            var vm = new Mock<INotifyPropertyChanged>();
            var comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(o => o.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var monitor = new PropertyMonitor<string>(vm.Object, "TestProperty", () => "TestValue", comparer.Object);

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

            var monitor = new PropertyMonitor<string>(vm.Object, "TestProperty", () => "TestValue", comparer.Object);

            var onPropertyChangedRaised = false;
            monitor.OnPropertyChangedCallback += () =>
            {
                onPropertyChangedRaised = true;
            };

            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty2"));

            Assert.IsFalse(onPropertyChangedRaised);
        }

        [TestMethod]
        public void OnPropertyChangedCallback_IsSuppressed_Test()
        {
            var vm = new Mock<INotifyPropertyChanged>();
            var comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(o => o.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var monitor = new PropertyMonitor<string>(vm.Object, "TestProperty", () => "TestValue", comparer.Object);

            var onPropertyChangedRaised = false;
            monitor.OnPropertyChangedCallback += () =>
            {
                onPropertyChangedRaised = true;
            };

            monitor.SuspendPropertyChanged();

            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));

            Assert.IsFalse(onPropertyChangedRaised);
        }

        [TestMethod]
        public void OnPropertyChangedCallback_Equals_Test()
        {
            var vm = new Mock<INotifyPropertyChanged>();
            var comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(o => o.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var monitor = new PropertyMonitor<string>(vm.Object, "TestProperty", () => "TestValue", comparer.Object);

            var onPropertyChangedRaised = false;
            monitor.OnPropertyChangedCallback += () =>
            {
                onPropertyChangedRaised = true;
            };

            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));

            Assert.IsFalse(onPropertyChangedRaised);
        }

        [TestMethod]
        public void OnPropertyChangedCallback_SuppressRestore_Test()
        {
            var vm = new Mock<INotifyPropertyChanged>();
            var comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(o => o.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var monitor = new PropertyMonitor<string>(vm.Object, "TestProperty", () => "TestValue", comparer.Object);

            var onPropertyChangedRaised = false;
            monitor.OnPropertyChangedCallback += () =>
            {
                onPropertyChangedRaised = true;
            };

            monitor.SuspendPropertyChanged();
            monitor.RestorePropertyChanged();   // raises property changed event.

            Assert.IsTrue(onPropertyChangedRaised);
        }

        [TestMethod]
        public void OnPropertyChangedCommand_Test()
        {
            var vm = new Mock<INotifyPropertyChanged>();
            var comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(o => o.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var command = new Mock<INotifyCommand>();
            command.Setup(o => o.NotifyCanExecuteChanged());

            var monitor = new PropertyMonitor<string>(vm.Object, "TestProperty", () => "TestValue", comparer.Object);

            monitor.RegisterCommand(command.Object);
            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));
            command.Verify(o => o.NotifyCanExecuteChanged(), Times.Once);

            monitor.UnregisterCommand(command.Object);

            // raise should not increment invocations after de-registration
            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));
            command.Verify(o => o.NotifyCanExecuteChanged(), Times.Once);
        }

        [TestMethod]
        public void OnPropertyChangedViewModel_Test()
        {
            var vm = new Mock<IViewModel>();
            vm.Setup(o => o.OnPropertyChanged("TargetProperty"));
            vm.SetupGet(o => o.PropertyMonitors).Returns(new Collection<IPropertyMonitor>());


            var comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(o => o.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            IPropertyMonitor monitor = new PropertyMonitor<string>(vm.Object, "TestProperty", () => "TestValue", comparer.Object);

            monitor.RegisterViewModelProperty(vm.Object, "TargetProperty");

            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));
            vm.Verify(o => o.OnPropertyChanged("TargetProperty"), Times.Once);

            monitor.UnregisterViewModelProperty(vm.Object, "TargetProperty");

            // raise should not increment invocations after de-registration
            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));
            vm.Verify(o => o.OnPropertyChanged("TargetProperty"), Times.Once);
        }

        [TestMethod]
        public void FinalizerTest()
        {
            List<IPropertyMonitor> monitors = new();

            for (int i = 0; i < 1_000; i++)
            {
                var target = new TestVm();

                monitors.Add(new PropertyMonitor<string>(target, nameof(target.Property), () => target.Property, null));

                target = null;

            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.WaitForFullGCComplete();
            GC.Collect();

            Assert.IsTrue(monitors.Any(o => o.Target is null));
        }
    }
}
