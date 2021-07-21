namespace Mvvm.Test.Controller
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Mvvm.Test.ViewModel;
    using SIM.Mvvm;
    using System;
    using System.ComponentModel;

    [TestClass]
    public class ViewModelPropertyListenerTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var target = new Mock<IViewModel>();
            var viewModel = new Mock<IViewModel>();

            var listener = new ViewModelPropertyListener(target.Object, viewModel.Object, "property");
            Assert.IsNotNull(listener);
        }

        [TestMethod]
        public void InvocationTest()
        {
            var target = new ViewModelMock();
            var viewModel = new ViewModelMock();
            viewModel.RegisterCounter();

            var listener = new ViewModelPropertyListener(target, viewModel, "Property", "Test");
            target.OnPropertyChanged("Test", null, null);

            Assert.AreEqual(1, viewModel.AdvancedPropertyChangedRaisedCount);
            Assert.AreEqual(1, viewModel.PropertyChangedRaisedCount);
        }

        [TestMethod]
        public void NoInvocationTest()
        {
            var target = new Mock<IViewModel>();
            var viewModel = new Mock<IViewModel>();
            viewModel.Setup(o => o.InvokeOnPropertyChanged("Property"));

            var listener = new ViewModelPropertyListener(target.Object, viewModel.Object, "Property", "Test");
            target.Raise(o => o.PropertyChanged -= null, new PropertyChangedEventArgs("OtherTest"));

            viewModel.Verify(o => o.InvokeOnPropertyChanged("Property"), Times.Never);
        }

        [TestMethod]
        public void ExtensionTest()
        {
            var target = new ViewModelMock();
            var viewModel = new ViewModelMock();
            viewModel.RegisterCounter();

            target.RegisterDependencies(viewModel, "Property", "Test");

            target.OnPropertyChanged("Test", null, null);


            Assert.AreEqual(1, viewModel.AdvancedPropertyChangedRaisedCount);
            Assert.AreEqual(1, viewModel.PropertyChangedRaisedCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExtensionFail()
        {
            IViewModel target = null;
            var viewModel = new Mock<IViewModel>();

            target.RegisterDependencies(viewModel.Object, "Property", "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExtensionFail2()
        {
            var target = new Mock<IViewModel>();
            target.Object.RegisterDependencies(null, "Property", "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExtensionFail3()
        {
            var target = new Mock<IViewModel>();
            var viewModel = new Mock<IViewModel>();
            target.Object.RegisterDependencies(viewModel.Object, null, "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtensionFail4()
        {
            var target = new Mock<IViewModel>();
            var viewModel = new Mock<IViewModel>();
            target.Object.RegisterDependencies(viewModel.Object, "Property", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtensionFail5()
        {
            var target = new Mock<IViewModel>();
            var viewModel = new Mock<IViewModel>();
            target.Object.RegisterDependencies(viewModel.Object, "Property");
        }
    }
}
