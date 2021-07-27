namespace SIM.Mvvm.Test.Controller
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SIM.Mvvm.Expressions;

    [TestClass]
    public class PropertyMonitorCollectionExtensionsTests
    {
        [TestMethod]
        public void RegisterCallback_Test()
        {
            Action a = new Action(() => { });
            var monitor = new Mock<IPropertyMonitor>();
            var monitors = new IPropertyMonitor[] { monitor.Object };

            monitor.SetupAdd(o => o.OnPropertyChangedCallback += a).Verifiable();

            var result = ExpressionCollectionExtensions.Call(monitors, a);
            monitor.VerifyAdd(o => o.OnPropertyChangedCallback += a, Times.Once);

            Assert.AreEqual(monitors, result);
        }

        [TestMethod]
        public void RegisterCallback_Adv_Test()
        {
            EventHandler<AdvancedPropertyChangedEventArgs> a = (s, e) => { };
            var monitor = new Mock<IPropertyMonitor>();
            monitor.SetupAdd(o => o.OnPropertyChanged += a).Verifiable();

            var monitors = new IPropertyMonitor[] { monitor.Object };

            var result = ExpressionCollectionExtensions.Call(monitors, a);
            monitor.VerifyAdd(o => o.OnPropertyChanged += a, Times.Once);

            Assert.AreEqual(monitors, result);
        }

        [TestMethod]
        public void RegisterCommands_Test()
        {
            ICommandInvokeCanExecuteChangedEvent[] commands = new ICommandInvokeCanExecuteChangedEvent[]
            {
                new EventCommand(),
            };

            var monitor = new Mock<IPropertyMonitor>();
            var monitors = new IPropertyMonitor[] { monitor.Object };

            monitor.Setup(o => o.RegisterCommand(commands[0]));

            var result = ExpressionCollectionExtensions.Notify(monitors, commands);
            monitor.Verify(o => o.RegisterCommand(commands[0]), Times.Once);

            Assert.AreEqual(monitors, result);
        }

        [TestMethod]
        public void RegisterViewModelProperties_Test()
        {
            var viewModel = new Mock<IViewModelMockTest>();
            viewModel.SetupGet(o => o.MyProperty).Returns("Test");

            string[] propertyNames = new string[] { nameof(IViewModelMockTest.MyProperty) };

            var monitor = new Mock<IPropertyMonitor>();
            var monitors = new IPropertyMonitor[] { monitor.Object };

            monitor.Setup(o => o.RegisterViewModelProperty(viewModel.Object, propertyNames[0]));

            var result = ExpressionCollectionExtensions.Notify(monitors, () => viewModel.Object.MyProperty);

            monitor.Verify(o => o.RegisterViewModelProperty(viewModel.Object, propertyNames[0]), Times.Once);

            Assert.AreEqual(monitors, result);
        }

        public interface IViewModelMockTest : IViewModel
        {
            public string MyProperty { get; set; }
        }
    }
}
