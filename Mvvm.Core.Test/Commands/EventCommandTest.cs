using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mvvm.Core;
using System;
using System.Windows.Input;

namespace Mvvm.Test.Commands
{
    [TestClass]
    public class EventCommandTest
    {
        [TestMethod]
        public void Constructor()
        {
            var rc = new EventCommand();
            Assert.IsNotNull(rc);
        }

        [TestMethod]
        public void ActionInvokation()
        {
            var invoked = false;

            var rc = new EventCommand();
            rc.OnExecuted += (s, a) => invoked = true;

            Assert.IsFalse(invoked);

            rc.Execute(null);

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void ActionPredicateInvocation()
        {
            var invoked = false;
            var canExecuteInvoked = false;

            var rc = new EventCommand();
            rc.OnExecuted += (s, a) => invoked = true;
            rc.OnCanExecuted += (s, a) => { canExecuteInvoked = true; a.CanExecute = true; };

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

            var rc = new EventCommand();
            rc.OnExecuted += (s, a) => invoked = true;
            rc.OnCanExecuted += (s, a) => { canExecuteInvoked = true; a.CanExecute = true; };

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

            var rc = new EventCommand();
            rc.OnExecuted += (s, a) => invoked = true;
            rc.OnCanExecuted += (s, a) => { canExecuteInvoked = true; a.CanExecute = false; };

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            rc.Execute(null);

            Assert.IsFalse(invoked);
            Assert.IsTrue(canExecuteInvoked);
        }

        [TestMethod]
        public void NullActionExecution()
        {
            var invoked = false;
            var canExecuteInvoked = false;

            var rc = new EventCommand();
            rc.OnCanExecuted += (s, a) => { canExecuteInvoked = true; };

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            rc.Execute(null);

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            void onExecuteEvent(object s, EventArgs a) { invoked = true; }

            rc.OnExecuted += onExecuteEvent;
            rc.OnCanExecuted += (s, a) => rc.OnExecuted -= onExecuteEvent;

            rc.Execute(null);

            Assert.IsFalse(invoked);
            Assert.IsTrue(canExecuteInvoked);

        }
    }
}
