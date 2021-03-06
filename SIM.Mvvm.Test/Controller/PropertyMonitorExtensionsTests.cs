namespace SIM.Mvvm.Test.Controller
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.ComponentModel;

    [TestClass]
    public class PropertyMonitorExtensionsTests
    {
        [TestMethod]
        public void RegisterCallback_Test()
        {
            bool raised = false;
            Action a = new Action(() => { raised = true; });

            var monitor = new Mock<IPropertyListener>();

            var result = IPropertyListenerExtensions.Call(monitor.Object, a);

            Assert.AreSame(monitor.Object, result);

            monitor.Raise(o => o.PropertyChanged += (s, a) => { }, EventArgs.Empty);

            Assert.IsTrue(raised);
        }


        [TestMethod]
        public void RegisterCallback_Adv_Test()
        {
            EventHandler<OnPropertyChangedEventArgs<string>> h = (s, e) => { };
            var monitor = new Mock<IPropertyMonitor<string>>();
            monitor.SetupAdd(o => o.PropertyChanged += h).Verifiable();

            var result = IPropertyMonitorExtensions.Call(monitor.Object, h);
            monitor.VerifyAdd(o => o.PropertyChanged += h, Times.Once);

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

        public interface IViewModelMockTest : IViewModel
        {
            public string MyProperty { get; set; }
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
                this.Listen(v => v.Test)
                    .Notify(() => this.TestString);
            }
        }

        public class MasterExpressionVm : ViewModel
        {
            public readonly ExpressionVm expressionVm;

            public MasterExpressionVm()
            {
                this.expressionVm = new ExpressionVm();
                this.expressionVm.Monitor(expressionVm, m => m.Test).Notify(() => this.TestString);
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


                this.Monitor(nestedVm, v => v.Test).Notify(() => this.NestedVmText);
                this.Monitor(nestedVmLight, v => v.Text).Notify(() => this.NestedVmLightText);
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
