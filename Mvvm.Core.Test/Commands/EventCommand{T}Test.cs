using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mvvm.Core;
using System;
using System.Windows.Input;

namespace Mvvm.Test.Commands
{
    [TestClass]
    public class EventCommand_T_Test
    {
        [TestMethod]
        public void Constructor()
        {
            var rc = new EventCommand<int>();
            Assert.IsNotNull(rc);
        }

        [TestMethod]
        public void ActionInvokation()
        {
            var invoked = false;

            var rc = new EventCommand<int>();
            rc.OnExecuted += (s, a) => invoked = true;

            Assert.IsFalse(invoked);

            rc.Execute(0);

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void ActionPredicateInvocation()
        {
            var invoked = false;
            var canExecuteInvoked = false;

            var rc = new EventCommand<int>();
            rc.OnExecuted += (s, a) => invoked = true;
            rc.OnCanExecuted += (s, a) => { canExecuteInvoked = true; a.CanExecute = true; };

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            Assert.IsTrue(((ICommand)rc).CanExecute(0));

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

            var rc = new EventCommand<int>();
            rc.OnExecuted += (s, a) => invoked = true;
            rc.OnCanExecuted += (s, a) => { canExecuteInvoked = true; a.CanExecute = true; };

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

            var rc = new EventCommand<int>();
            rc.OnExecuted += (s, a) => invoked = true;
            rc.OnCanExecuted += (s, a) => { canExecuteInvoked = true; a.CanExecute = false; };

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            rc.Execute(0);

            Assert.IsFalse(invoked);
            Assert.IsTrue(canExecuteInvoked);
        }

        [TestMethod]
        public void NullActionExecution()
        {
            var invoked = false;
            var canExecuteInvoked = false;

            var rc = new EventCommand<int>();
            rc.OnCanExecuted += (s, a) => { canExecuteInvoked = true; };

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            rc.Execute(0);
            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            EventHandler<int> onExecuteEvent = (s, a) => { invoked = true; };

            rc.OnExecuted += onExecuteEvent;
            rc.OnCanExecuted += (s, a) => rc.OnExecuted -= onExecuteEvent;

            rc.Execute(0);

            Assert.IsFalse(invoked);
            Assert.IsTrue(canExecuteInvoked);
        }

        [TestMethod]
        public void ParameterTransferTest()
        {
            var param = 12356;

            var rc = new EventCommand<int>();
            rc.OnExecuted += (s, i) => Assert.AreEqual(param, i);
            rc.OnCanExecuted += (s, a) => Assert.AreEqual(param, a.Parameter);

            rc.Execute(param);
        }
    }
}
