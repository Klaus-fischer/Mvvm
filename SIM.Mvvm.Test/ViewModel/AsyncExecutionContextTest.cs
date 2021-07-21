namespace Mvvm.Test.ViewModel
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SIM.Mvvm;
    using System;

    [TestClass]
    public class AsyncExecutionContextTest
    {
        [TestMethod]
        public void Constructor()
        {
            var aec = new AsyncExecutionContext();
            Assert.IsNotNull(aec);
            Assert.IsNotNull(aec.Cancel);
            Assert.IsNull(aec.CancellationTokenSource);
            Assert.IsFalse(aec.IsBusy);
            Assert.IsFalse(aec.Cancel.CanExecute(null));
        }

        [TestMethod]
        public void SetBusy()
        {
            var canExecuteChangedInvoked = false;
            var aec = new AsyncExecutionContext();
            aec.Cancel.CanExecuteChanged += (s, a) => canExecuteChangedInvoked = true;

            aec.PrepareExecution(out _);

            Assert.IsNotNull(aec.CancellationTokenSource);
            Assert.IsTrue(aec.IsBusy);
            Assert.IsTrue(aec.Cancel.CanExecute(null));
            Assert.IsTrue(canExecuteChangedInvoked);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetBusyFail()
        {
            var aec = new AsyncExecutionContext();

            aec.PrepareExecution(out _);
            Assert.IsTrue(aec.IsBusy);
            aec.PrepareExecution(out _);
        }

        [TestMethod]
        public void ClearBusy()
        {
            var canExecuteChangedInvoked = false;
            var aec = new AsyncExecutionContext();
            aec.PrepareExecution(out _);
            aec.Cancel.CanExecuteChanged += (s, a) => canExecuteChangedInvoked = true;

            aec.FinalizeExecution();

            Assert.IsNull(aec.CancellationTokenSource);
            Assert.IsFalse(aec.IsBusy);
            Assert.IsFalse(aec.Cancel.CanExecute(null));
            Assert.IsTrue(canExecuteChangedInvoked);
        }

        [TestMethod]
        public void TokenTest()
        {
            var aec = new AsyncExecutionContext();
            aec.PrepareExecution(out var token);

            Assert.IsFalse(token.IsCancellationRequested);

            aec.Cancel.Execute(null);

            Assert.IsTrue(token.IsCancellationRequested);
        }
    }
}
