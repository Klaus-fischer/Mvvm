namespace SIM.Mvvm.Test.Controller
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass]
    public class PropertyMonitorExtensionsTests
    {
        [TestMethod]
        public void RegisterCallback_Test()
        {
            Action a = new Action(() => { });
            var monitor = new Mock<IPropertyMonitor>();
            monitor.SetupAdd(o => o.OnPropertyChangedCallback += a).Verifiable();

            var result = PropertyMonitorExtensions.RegisterCallback(monitor.Object, a);
            monitor.VerifyAdd(o => o.OnPropertyChangedCallback += a, Times.Once);

            Assert.AreEqual(monitor.Object, result);
        }

        [TestMethod]
        public void UnregisterCallback_Test()
        {
            Action a = new Action(() => { });
            var monitor = new Mock<IPropertyMonitor>();
            monitor.SetupRemove(o => o.OnPropertyChangedCallback -= a).Verifiable();

            var result = PropertyMonitorExtensions.UnregisterCallback(monitor.Object, a);
            monitor.VerifyRemove(o => o.OnPropertyChangedCallback -= a, Times.Once);
        }

        [TestMethod]
        public void RegisterCallback_Adv_Test()
        {
            EventHandler<AdvancedPropertyChangedEventArgs> a = (s, e) => { };
            var monitor = new Mock<IPropertyMonitor>();
            monitor.SetupAdd(o => o.OnPropertyChanged += a).Verifiable();

            var result = PropertyMonitorExtensions.RegisterCallback(monitor.Object, a);
            monitor.VerifyAdd(o => o.OnPropertyChanged += a, Times.Once);

            Assert.AreEqual(monitor.Object, result);
        }

        [TestMethod]
        public void UnregisterCallback_Adv_Test()
        {
            EventHandler<AdvancedPropertyChangedEventArgs> a = (s, e) => { };
            var monitor = new Mock<IPropertyMonitor>();
            monitor.SetupRemove(o => o.OnPropertyChanged -= a).Verifiable();

            var result = PropertyMonitorExtensions.UnregisterCallback(monitor.Object, a);
            monitor.VerifyRemove(o => o.OnPropertyChanged -= a, Times.Once);

            Assert.AreEqual(monitor.Object, result);
        }

        [TestMethod]
        public void RegisterCommands_Test()
        {
            ICommandInvokeCanExecuteChangedEvent[] commands = new ICommandInvokeCanExecuteChangedEvent[]
            {
                new EventCommand(),
            };

            var monitor = new Mock<IPropertyMonitor>();
            monitor.Setup(o => o.RegisterCommand(commands[0]));

            var result = PropertyMonitorExtensions.RegisterCommands(monitor.Object, commands);
            monitor.Verify(o => o.RegisterCommand(commands[0]), Times.Once);

            Assert.AreEqual(monitor.Object, result);
        }

        [TestMethod]
        public void UnregisterCommands_Test()
        {
            ICommandInvokeCanExecuteChangedEvent[] commands = new ICommandInvokeCanExecuteChangedEvent[]
            {
            new EventCommand(),
            };

            var monitor = new Mock<IPropertyMonitor>();
            monitor.Setup(o => o.UnregisterCommand(commands[0]));

            var result = PropertyMonitorExtensions.UnregisterCommands(monitor.Object, commands);
            monitor.Verify(o => o.UnregisterCommand(commands[0]), Times.Once);

            Assert.AreEqual(monitor.Object, result);
        }

        [TestMethod]
        public void RegisterViewModelProperties_Test()
        {
            var viewModel = new Mock<IViewModel>();

            string[] propertyNames = new string[] { "Test" };

            var monitor = new Mock<IPropertyMonitor>();
            monitor.Setup(o => o.RegisterViewModelProperty(viewModel.Object, propertyNames[0]));

            var result = PropertyMonitorExtensions.RegisterViewModelProperties(monitor.Object, viewModel.Object, propertyNames);

            monitor.Verify(o => o.RegisterViewModelProperty(viewModel.Object, propertyNames[0]), Times.Once);

            Assert.AreEqual(monitor.Object, result);
        }

        [TestMethod]
        public void UnregisterViewModelProperties_Test()
        {
            var viewModel = new Mock<IViewModel>();

            string[] propertyNames = new string[] { "Test" };

            var monitor = new Mock<IPropertyMonitor>();
            monitor.Setup(o => o.UnregisterViewModelProperty(viewModel.Object, propertyNames[0]));

            var result = PropertyMonitorExtensions.UnregisterViewModelProperties(monitor.Object, viewModel.Object, propertyNames);

            monitor.Verify(o => o.UnregisterViewModelProperty(viewModel.Object, propertyNames[0]), Times.Once);

            Assert.AreEqual(monitor.Object, result);
        }
    }
}
