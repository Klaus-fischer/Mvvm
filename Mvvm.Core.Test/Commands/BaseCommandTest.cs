namespace Mvvm.Test.Commands
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mvvm.Core;
    using System;

    [TestClass]
    public class BaseCommandTest
    {
        [TestMethod]
        public void InvokeCanExecuteChanged()
        {
            bool canExecuteChangedInvoked = false;
            var cmd = new BaseCommandType();

            ((ICommandInvokeCanExecuteChangedEvent)cmd).InvokeCanExecuteChanged(cmd, new EventArgs());
            Assert.IsFalse(canExecuteChangedInvoked);

            cmd.CanExecuteChanged += (s, a) => canExecuteChangedInvoked = true;
            ((ICommandInvokeCanExecuteChangedEvent)cmd).InvokeCanExecuteChanged(cmd, new EventArgs());
            Assert.IsTrue(canExecuteChangedInvoked);
        }


        private class BaseCommandType : Command
        {
            public bool CanExecuteValue = true;

            public Action OnExecuteCallback;

            protected override bool CanExecute() => CanExecuteValue;

            protected override void OnExecute()
            {
                this.OnExecuteCallback?.Invoke();
            }
        }
    }
}
