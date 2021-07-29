namespace SIM.Mvvm.Test.Controller
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.ComponentModel;
    using SIM.Mvvm.Expressions;

    [TestClass]
    public class PropertyMonitorExtensionsTests
    {
        [TestMethod]
        public void RegisterCallback_Test()
        {
            Action a = new Action(() => { });
            var monitor = new Mock<IPropertyMonitor>();
            monitor.SetupAdd(o => o.OnPropertyChangedCallback += a).Verifiable();

            var result = ExpressionExtensions.Call(monitor.Object, a);
            monitor.VerifyAdd(o => o.OnPropertyChangedCallback += a, Times.Once);

            Assert.AreEqual(monitor.Object, result);
        }


        [TestMethod]
        public void RegisterCallback_Adv_Test()
        {
            EventHandler<AdvancedPropertyChangedEventArgs> a = (s, e) => { };
            var monitor = new Mock<IPropertyMonitor>();
            monitor.SetupAdd(o => o.OnPropertyChanged += a).Verifiable();

            var result = ExpressionExtensions.Call(monitor.Object, a);
            monitor.VerifyAdd(o => o.OnPropertyChanged += a, Times.Once);

            Assert.AreEqual(monitor.Object, result);
        }

        [TestMethod]
        public void RegisterViewModelProperties_Test()
        {
            var viewModel = new Mock<IViewModelMockTest>();

            string[] propertyNames = new string[] { nameof(IViewModelMockTest.MyProperty) };

            var monitor = new Mock<IPropertyMonitor>();
            monitor.Setup(o => o.RegisterViewModelProperty(viewModel.Object, propertyNames[0]));

            var result = ExpressionExtensions.Notify(monitor.Object, () => viewModel.Object.MyProperty);

            monitor.Verify(o => o.RegisterViewModelProperty(viewModel.Object, propertyNames[0]), Times.Once);

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
                this.Listen(()=>this.Test)
                    .Notify(() => this.TestString);
            }
        }

        public class MasterExpressionVm : ViewModel
        {
            public readonly ExpressionVm expressionVm;

            public MasterExpressionVm()
            {
                this.expressionVm = new ExpressionVm();
                this.expressionVm.Listen(()=> this.expressionVm.Test).Notify(() => this.TestString);
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


                this.Listen(() => this.nestedVm.Test).Notify(() => this.NestedVmText);
                this.Listen(() => this.nestedVmLight.Text).Notify(() => this.NestedVmLightText);
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
