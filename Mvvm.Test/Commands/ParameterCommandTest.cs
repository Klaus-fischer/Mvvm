namespace Mvvm.Test.Commands
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mvvm.Core;
    using System;

    [TestClass]
    public class ParameterCommandTest
    {
        [TestMethod]
        public void CanExecuteTest()
        {
            bool canExecuteCallbackCalled = false;
            bool onExecuteCallbackCalled = false;

            var cmd = new ParameterCommandType
            {
                CanExecuteCallback = i => { return canExecuteCallbackCalled = true; },
                OnExecuteCallback = i => onExecuteCallbackCalled = true,
            };

            cmd.CanExecute(null);

            Assert.IsFalse(canExecuteCallbackCalled);
            Assert.IsFalse(onExecuteCallbackCalled);


            cmd.Execute(null);

            Assert.IsFalse(canExecuteCallbackCalled);
            Assert.IsFalse(onExecuteCallbackCalled);

            cmd.Execute(13);

            Assert.IsTrue(canExecuteCallbackCalled);
            Assert.IsTrue(onExecuteCallbackCalled);
        }

        [TestMethod]
        public void CanExecuteInterfaceTest()
        {
            bool canExecuteCallbackCalled = false;
            bool onExecuteCallbackCalled = false;

            IParameterCommand<int> cmd = new ParameterCommandType
            {
                CanExecuteCallback = i => { return canExecuteCallbackCalled = true; },
                OnExecuteCallback = i => onExecuteCallbackCalled = true,
            };

            cmd.CanExecute(0);

            Assert.IsTrue(canExecuteCallbackCalled);
            Assert.IsFalse(onExecuteCallbackCalled);

            canExecuteCallbackCalled = false;

            cmd.Execute(13);

            Assert.IsTrue(canExecuteCallbackCalled);
            Assert.IsTrue(onExecuteCallbackCalled);
        }

        private class ParameterCommandType : ParameterCommand<int>
        {
            public Func<int, bool> CanExecuteCallback;

            public Action<object> OnExecuteCallback;

            protected override bool CanExecute(int parameter)
            {
                return CanExecuteCallback?.Invoke(parameter) ?? true;
            }

            protected override void Execute(int parameter)
            {
                this.OnExecuteCallback?.Invoke(parameter);
            }
        }
    }
}
