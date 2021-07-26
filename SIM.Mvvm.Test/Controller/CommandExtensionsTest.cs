namespace Mvvm.Test.Controller
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Mvvm.Test.ViewModel;
    using SIM.Mvvm;
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    [TestClass]
    public class CommandExtensionsTest
    {
        [TestMethod]
        public void RegisterPropertyDependencyTest()
        {
            var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            _ = cmd.Setup(o => o.InvokeCanExecuteChanged(It.IsAny<object>(), It.IsAny<EventArgs>()));

            var pmMock = new Mock<IPropertyMonitor>();
            pmMock.SetupGet(o => o.PropertyName).Returns("Test");
            pmMock.Setup(o => o.RegisterCommand(cmd.Object));

            IPropertyMonitor[] pm = { pmMock.Object };
            string[] key = { "Test" };

            var vm = new Mock<IViewModel>();
            vm.SetupGet(i => i[key]).Returns(pm);

            var result = CommandExtensions.RegisterPropertyDependency(cmd.Object, vm.Object, key);

            pmMock.Verify(o => o.RegisterCommand(cmd.Object), Times.AtLeastOnce());

            Assert.AreEqual(cmd.Object, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterPropertyDependencyFail()
        {
            ICommandInvokeCanExecuteChangedEvent cmd = null;
            var vm = new ViewModelMock();

            cmd.RegisterPropertyDependency(vm, "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterPropertyDependencyFail2()
        {
            var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            IViewModel vm = null;

            cmd.Object.RegisterPropertyDependency(vm, "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterPropertyDependencyFail3()
        {
            var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            var vm = new Mock<IViewModel>();
            cmd.Object.RegisterPropertyDependency(vm.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterPropertyDependencyFail4()
        {
            var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            var vm = new Mock<IViewModel>();

            cmd.Object.RegisterPropertyDependency(vm.Object, Array.Empty<string>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterPropertyDependencyFail5()
        {
            var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            var vm = new Mock<IViewModel>();

            cmd.Object.RegisterPropertyDependency(vm.Object, null);
        }

        [TestMethod]
        public void UnregisterPropertyDependencyTest()
        {
            var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            _ = cmd.Setup(o => o.InvokeCanExecuteChanged(It.IsAny<object>(), It.IsAny<EventArgs>()));

            var pmMock = new Mock<IPropertyMonitor>();
            pmMock.SetupGet(o => o.PropertyName).Returns("Test");
            pmMock.Setup(o => o.UnregisterCommand(cmd.Object));

            IPropertyMonitor[] pm = { pmMock.Object };
            string[] key = { };

            var vm = new Mock<IViewModel>();
            vm.SetupGet(i => i[key]).Returns(pm);

            CommandExtensions.UnregisterPropertyDependency(cmd.Object, vm.Object);

            pmMock.Verify(o => o.UnregisterCommand(cmd.Object), Times.AtLeastOnce());
        }

        [TestMethod]
        public void TryInvokeCanExecuteChanged_Test()
        {
            var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            cmd.Setup(o => o.InvokeCanExecuteChanged(cmd.Object, It.IsAny<EventArgs>()));

            CommandExtensions.TryInvokeCanExecuteChanged(cmd.Object);

            cmd.Verify(o => o.InvokeCanExecuteChanged(cmd.Object, It.IsAny<EventArgs>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryInvokeCanExecuteChanged_Fail()
        {
            var cmd = new Mock<ICommand>();

            CommandExtensions.TryInvokeCanExecuteChanged(cmd.Object);
        }

        [TestMethod]
        public void InvokeCanExecuteChanged_Test()
        {
            var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            cmd.Setup(o => o.InvokeCanExecuteChanged(cmd.Object, It.IsAny<EventArgs>()));

            CommandExtensions.InvokeCanExecuteChanged(cmd.Object);

            cmd.Verify(o => o.InvokeCanExecuteChanged(cmd.Object, It.IsAny<EventArgs>()), Times.Once);
        }
    }
}
