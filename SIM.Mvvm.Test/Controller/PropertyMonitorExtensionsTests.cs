namespace SIM.Mvvm.Test.Controller
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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

        [TestMethod]
        public void ExpressionTest()
        {
            bool invoked = false;
            var expVm = new ExpressionVm();

            Assert.IsNotNull(expVm);
            expVm.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == nameof(expVm.TestString))
                {
                    invoked = true;
                }
            };

            // to invoke property changed
            expVm.Test = 42;

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void MasterExpressionTest()
        {
            bool invoked = false;
            var expVm = new MasterExpressionVm();

            Assert.IsNotNull(expVm);
            expVm.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == nameof(expVm.TestString))
                {
                    invoked = true;
                }
            };

            // to invoke property changed
            expVm.expressionVm.Test = 42;

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void NestedExpressionTest()
        {
            bool nestedVmLightTextInvoked = false;
            bool nestedVmTextInvoked = false;
            var expVm = new NestedExpressionVm();

            Assert.IsNotNull(expVm);
            expVm.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == nameof(expVm.NestedVmLightText))
                {
                    nestedVmLightTextInvoked = true;
                }

                if (a.PropertyName == nameof(expVm.NestedVmText))
                {
                    nestedVmTextInvoked = true;
                }
            };

            // to invoke property changed
            expVm.nestedVm.Test = 42;
            Assert.IsTrue(nestedVmTextInvoked);

            expVm.nestedVmLight.Text = "42";
            expVm.nestedVmLight.Invoke();
            Assert.IsTrue(nestedVmLightTextInvoked);
        }

        public class ExpressionVm : ViewModel
        {
            private int test;

            public int Test
            {
                get { return test; }
                set { SetPropertyValue(ref test, value); }
            }

            public string TestString => $"{test}String";

            public ExpressionVm()
            {
                this[nameof(Test)].NotifyAlso(() => this.TestString);
            }
        }

        public class MasterExpressionVm : ViewModel
        {
            public readonly ExpressionVm expressionVm;

            public MasterExpressionVm()
            {
                this.expressionVm = new ExpressionVm();
                this.expressionVm[nameof(Test)].NotifyAlso(() => TestString);
            }

            public string TestString => $"{expressionVm.Test} String";
        }

        public class NestedExpressionVm : ViewModel
        {
            public readonly ExpressionVm nestedVm;

            public readonly NestedVmLight nestedVmLight;

            public NestedExpressionVm()
            {
                this.nestedVm = new ExpressionVm();
                this.nestedVmLight = new NestedVmLight();

                this[nameof(NestedVmText)].DependsOn(() => nestedVm.Test);
                this[nameof(NestedVmLightText)].DependsOn(() => nestedVmLight.Text);
            }

            public string NestedVmText => $"{nestedVm.Test} NestedVmText";

            public string NestedVmLightText => nestedVmLight.Text;


            public class NestedVmLight : INotifyPropertyChanged
            {
                public event PropertyChangedEventHandler? PropertyChanged;

                public string Text { get; set; }

                public void Invoke()
                {
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
                }
            }
        }


    }
}
