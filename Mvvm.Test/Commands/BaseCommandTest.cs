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

            ((ICommandInvokeCanExecuteChangedEvent)cmd).RaiseCanExecuteChanged();
            Assert.IsFalse(canExecuteChangedInvoked);

            cmd.CanExecuteChanged += (s, a) => canExecuteChangedInvoked = true;
            ((ICommandInvokeCanExecuteChangedEvent)cmd).RaiseCanExecuteChanged();
            Assert.IsTrue(canExecuteChangedInvoked);
        }


        private class BaseCommandType : BaseCommand
        {
            public bool CanExecuteValue = true;

            public Action<object> OnExecute;

            public override bool CanExecute(object parameter) => CanExecuteValue;

            public override void Execute(object parameter)
            {
                this.OnExecute?.Invoke(parameter);
            }
        }
    }
}
