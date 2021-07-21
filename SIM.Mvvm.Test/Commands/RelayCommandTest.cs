using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIM.Mvvm;
using System;
using System.Windows.Input;

namespace Mvvm.Test.Commands
{
    [TestClass]
    public class RelayCommandTest
    {
        [TestMethod]
        public void Constructor()
        {
            var rc = new RelayCommand(() => { });
            Assert.IsNotNull(rc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorFail()
        {
            _ = new RelayCommand(null);
        }

        [TestMethod]
        public void ActionInvokation()
        {
            var invoked = false;

            var rc = new RelayCommand(() => invoked = true);

            Assert.IsFalse(invoked);

            rc.Execute();

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void ActionPredicateInvocation()
        {
            var invoked = false;
            var canExecuteInvoked = false;

            ICommand rc = new RelayCommand(() => invoked = true,
                () => { canExecuteInvoked = true; return true; });

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            Assert.IsTrue(rc.CanExecute(null));

            Assert.IsFalse(invoked);
            Assert.IsTrue(canExecuteInvoked);

            canExecuteInvoked = false;

            rc.Execute(null);

            Assert.IsTrue(invoked);
            Assert.IsTrue(canExecuteInvoked);
        }

        [TestMethod]
        public void ActionPredicateInvocationOnExecute()
        {
            var invoked = false;
            var canExecuteInvoked = false;

            ICommand rc = new RelayCommand(() => invoked = true,
                () => { canExecuteInvoked = true; return true; });

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            rc.Execute(null);

            Assert.IsTrue(invoked);
            Assert.IsTrue(canExecuteInvoked);
        }

        [TestMethod]
        public void ActionPreventExecution()
        {
            var invoked = false;
            var canExecuteInvoked = false;

            ICommand rc = new RelayCommand(() => invoked = true,
                () => { canExecuteInvoked = true; return false; });

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            rc.Execute(null);

            Assert.IsFalse(invoked);
            Assert.IsTrue(canExecuteInvoked);
        }
    }
}
