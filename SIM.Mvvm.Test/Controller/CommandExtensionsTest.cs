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
            var cmd = new Mock<INotifyCommand>();
            cmd.Setup(o => o.NotifyCanExecuteChanged());

            CommandExtensions.TryInvokeCanExecuteChanged(cmd.Object);

            cmd.Verify(o => o.NotifyCanExecuteChanged(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryInvokeCanExecuteChanged_Fail()
        {
            var cmd = new Mock<ICommand>();

            CommandExtensions.TryInvokeCanExecuteChanged(cmd.Object);
        }
    }
}
