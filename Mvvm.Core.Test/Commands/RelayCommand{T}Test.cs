using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mvvm.Core;
using System;
using System.Windows.Input;

namespace Mvvm.Test.Commands
{
    [TestClass]
    public class RelayCommand_T_Test
    {
        [TestMethod]
        public void Constructor()
        {
            var rc = new RelayCommand<int>(i => { });
            Assert.IsNotNull(rc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorFail()
        {
            _ = new RelayCommand<int>(null);
        }

        [TestMethod]
        public void ActionInvokation()
        {
            var invoked = false;

            var rc = new RelayCommand<int>(i => invoked = true);

            Assert.IsFalse(invoked);

            rc.Execute(0);

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void ActionPredicateInvocation()
        {
            var invoked = false;
            var canExecuteInvoked = false;

            ICommand rc = new RelayCommand<int>(i => invoked = true,
                i => { canExecuteInvoked = true; return true; });

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            Assert.IsTrue(rc.CanExecute(0));

            Assert.IsFalse(invoked);
            Assert.IsTrue(canExecuteInvoked);

            canExecuteInvoked = false;

            rc.Execute(0);

            Assert.IsTrue(invoked);
            Assert.IsTrue(canExecuteInvoked);
        }

        [TestMethod]
        public void ActionPredicateInvocationOnExecute()
        {
            var invoked = false;
            var canExecuteInvoked = false;

            var rc = new RelayCommand<int>(i => invoked = true,
                i => { canExecuteInvoked = true; return true; });

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            rc.Execute(0);

            Assert.IsTrue(invoked);
            Assert.IsTrue(canExecuteInvoked);
        }

        [TestMethod]
        public void ActionPreventExecution()
        {
            var invoked = false;
            var canExecuteInvoked = false;

            var rc = new RelayCommand<int>(i => invoked = true,
                i => { canExecuteInvoked = true; return false; });

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            rc.Execute(0);

            Assert.IsFalse(invoked);
            Assert.IsTrue(canExecuteInvoked);
        }

        [TestMethod]
        public void ParameterTransferTest()
        {
            var param = 12356;

            var rc = new RelayCommand<int>(
                i => Assert.AreEqual(param, i),
                i => i == param);

            rc.Execute(param);
        }
    }
}
