namespace SIM.Mvvm.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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

            string value = "TestValue";
            var monitor = new PropertyMonitor<string>(vm.Object, "TestProperty", () => value, comparer.Object);

            value = "NewValue";

            bool onPropertyChangedRaised = false;
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

            bool onPropertyChangedRaised = false;
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

            bool onPropertyChangedRaised = false;
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

            bool onPropertyChangedRaised = false;
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

            bool onPropertyChangedRaised = false;
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

            bool onPropertyChangedRaised = false;
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

            var command = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            command.Setup(o => o.InvokeCanExecuteChanged(command.Object, It.IsAny<EventArgs>()));

            var monitor = new PropertyMonitor<string>(vm.Object, "TestProperty", () => "TestValue", comparer.Object);

            monitor.RegisterCommand(command.Object);
            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));
            command.Verify(o => o.InvokeCanExecuteChanged(command.Object, It.IsAny<EventArgs>()), Times.Once);

            monitor.UnregisterCommand(command.Object);

            // raise should not increment invocations after de-registration
            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));
            command.Verify(o => o.InvokeCanExecuteChanged(command.Object, It.IsAny<EventArgs>()), Times.Once);
        }

        [TestMethod]
        public void ToString_Test()
        {
            var vm = new Mock<INotifyPropertyChanged>();
            vm.Setup(o => o.ToString()).Returns("ViewModel");

            var monitor = new PropertyMonitor<string>(vm.Object, "TestProperty", () => "TestValue");

            Assert.AreEqual($"PropertyMonitor -> TestProperty/(ViewModel)", monitor.ToString());
        }

        [TestMethod]
        public void OnPropertyChangedViewModel_Test()
        {
            var vm = new Mock<IViewModel>();
            vm.Setup(o => o.OnPropertyChanged("TargetProperty"));

            var comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(o => o.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var monitor = new PropertyMonitor<string>(vm.Object, "TestProperty", () => "TestValue", comparer.Object);

            monitor.RegisterViewModelProperty(vm.Object, "TargetProperty");

            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));
            vm.Verify(o => o.OnPropertyChanged("TargetProperty"), Times.Once);

            monitor.UnregisterViewModelProperty(vm.Object, "TargetProperty");

            // raise should not increment invocations after de-registration
            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));
            vm.Verify(o => o.OnPropertyChanged("TargetProperty"), Times.Once);
        }

        [TestMethod]
        public void UpdateCommand_Test()
        {
            var command = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            command.Setup(o => o.InvokeCanExecuteChanged(command.Object, It.IsAny<EventArgs>()));

            var pm = new Mock<IPropertyMonitor>();
            pm.Setup(o => o.RegisterCommand(command.Object));
            pm.Setup(o => o.UnregisterCommand(command.Object));

            var pms = new IPropertyMonitor[] { pm.Object };

            var vm = new Mock<IViewModel>();
            vm.SetupGet(o => o[ViewModel.AllPropertyMontitorsToUnregister]).Returns(pms);

            var comparer = new Mock<IEqualityComparer<ICommandInvokeCanExecuteChangedEvent>>();
            comparer.Setup(o => o.Equals(It.IsAny<ICommandInvokeCanExecuteChangedEvent>(), It.IsAny<ICommandInvokeCanExecuteChangedEvent>())).Returns(false);

            var monitor = new PropertyMonitor<ICommandInvokeCanExecuteChangedEvent>(vm.Object, "TestProperty", () => command.Object, comparer.Object);

            vm.Raise(o => o.PropertyChanged += null, new PropertyChangedEventArgs("TestProperty"));

            pm.Verify(o => o.RegisterCommand(command.Object), Times.Once);
            pm.Verify(o => o.UnregisterCommand(command.Object), Times.Once);
        }
    }
}
