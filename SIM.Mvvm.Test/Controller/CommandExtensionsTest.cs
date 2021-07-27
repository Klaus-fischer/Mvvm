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
