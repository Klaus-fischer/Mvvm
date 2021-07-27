namespace Mvvm.Test.Commands
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SIM.Mvvm;
    using System;

    [TestClass]
    public class BaseCommandTest
    {
        [TestMethod]
        public void InvokeCanExecuteChanged()
        {
            bool canExecuteChangedInvoked = false;
            var cmd = new BaseCommandType();

            ((INotifyCommand)cmd).Notify(cmd, new EventArgs());
            Assert.IsFalse(canExecuteChangedInvoked);

            cmd.CanExecuteChanged += (s, a) => canExecuteChangedInvoked = true;
            ((INotifyCommand)cmd).Notify(cmd, new EventArgs());
            Assert.IsTrue(canExecuteChangedInvoked);
        }

        [TestMethod]
        public void InvokeCanExecuteChanged2()
        {
            bool canExecuteChangedInvoked = false;
            var cmd = new BaseCommandType();

            cmd.BaseInvokeCanExecuteChanged();
            Assert.IsFalse(canExecuteChangedInvoked);

            cmd.CanExecuteChanged += (s, a) => canExecuteChangedInvoked = true;
            cmd.BaseInvokeCanExecuteChanged();
            Assert.IsTrue(canExecuteChangedInvoked);
        }


        [TestMethod]
        public void InvokeOnExecuteCallback()
        {
            bool executeInvoked = false;
            var cmd = new BaseCommandType();
            cmd.OnExecuteCallback = () => executeInvoked = true;

            cmd.Execute();
            Assert.IsTrue(executeInvoked);
        }


        private class BaseCommandType : Command
        {
            public bool CanExecuteValue = true;

            public Action OnExecuteCallback;

            protected override void OnExecute()
            {
                this.OnExecuteCallback?.Invoke();
            }

            public void BaseInvokeCanExecuteChanged()
                => base.InvokeCanExecuteChanged();
        }
    }
}
