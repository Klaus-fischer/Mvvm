namespace Mvvm.Test.Controller
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Mvvm.Test.ViewModel;
    using SIM.Mvvm;
    using System;
    using System.ComponentModel;

    [TestClass]
    public class ViewModelCommandListenerTest
    {
        //[TestMethod]
        //public void ConstructorTest()
        //{
        //    var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
        //    var vm = new Mock<IViewModel>();

        //    var listener = new ViewModelCommandListener(vm.Object, cmd.Object);
        //    Assert.IsNotNull(listener);
        //}

        //[TestMethod]
        //public void InvocationTest()
        //{
        //    var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
        //    _ = cmd.Setup(o => o.InvokeCanExecuteChanged(It.IsAny<object>(), It.IsAny<EventArgs>()));

        //    var vm = new Mock<IViewModel>();

        //    var listener = new ViewModelCommandListener(vm.Object, cmd.Object, "Test");
        //    vm.Raise(o => o.PropertyChanged -= null, new PropertyChangedEventArgs("Test"));

        //    cmd.Verify(o => o.InvokeCanExecuteChanged(It.IsAny<object>(), It.IsAny<EventArgs>()), Times.Once);
        //}

        //[TestMethod]
        //public void NoInvocationTest()
        //{
        //    var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
        //    _ = cmd.Setup(o => o.InvokeCanExecuteChanged(It.IsAny<object>(), It.IsAny<EventArgs>()));

        //    var vm = new Mock<IViewModel>();

        //    var listener = new ViewModelCommandListener(vm.Object, cmd.Object, "Test");

        //    vm.Raise(o => o.PropertyChanged -= null, new PropertyChangedEventArgs("OtherTest"));

        //    cmd.Verify(o => o.InvokeCanExecuteChanged(It.IsAny<object>(), It.IsAny<EventArgs>()), Times.Never);
        //}

        [TestMethod]
        public void ExtensionTest()
        {
            var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            _ = cmd.Setup(o => o.InvokeCanExecuteChanged(It.IsAny<object>(), It.IsAny<EventArgs>()));

            PropertyMonitor[] pm = { new PropertyMonitor("Test") };
            string[] key = { "Test" };

            var vm = new Mock<IViewModel>();
            vm.SetupGet(i => i[key]).Returns(pm);

            cmd.Object.RegisterPropertyDependency(vm.Object, key);

            pm[0].InvokeOnViewModelPropertyChanged(vm.Object, new AdvancedPropertyChangedEventArgs("Test", null, null));

            cmd.Verify(o => o.InvokeCanExecuteChanged(It.IsAny<object>(), It.IsAny<EventArgs>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExtensionFail()
        {
            ICommandInvokeCanExecuteChangedEvent cmd = null;
            var vm = new ViewModelMock();

            cmd.RegisterPropertyDependency(vm, "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExtensionFail2()
        {
            var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            IViewModel vm = null;

            cmd.Object.RegisterPropertyDependency(vm, "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtensionFail3()
        {
            var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            var vm = new Mock<IViewModel>();
            cmd.Object.RegisterPropertyDependency(vm.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtensionFail4()
        {
            var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            var vm = new Mock<IViewModel>();

            cmd.Object.RegisterPropertyDependency(vm.Object, Array.Empty<string>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtensionFail5()
        {
            var cmd = new Mock<ICommandInvokeCanExecuteChangedEvent>();
            var vm = new Mock<IViewModel>();

            cmd.Object.RegisterPropertyDependency(vm.Object, null);
        }
    }
}
