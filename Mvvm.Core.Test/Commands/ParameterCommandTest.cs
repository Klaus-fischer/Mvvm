namespace Mvvm.Test.Commands
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mvvm.Core;
    using System;
    using System.Windows.Input;

    [TestClass]
    public class ParameterCommandTest
    {
        [TestMethod]
        public void CanExecuteTest()
        {
            bool canExecuteCallbackCalled = false;
            bool onExecuteCallbackCalled = false;

            ICommand cmd = new ParameterCommandType
            {
                CanExecuteCallback = i => { return canExecuteCallbackCalled = true; },
                OnExecuteCallback = i => onExecuteCallbackCalled = true,
            };

            cmd.CanExecute(null);

            Assert.IsTrue(canExecuteCallbackCalled);
            Assert.IsFalse(onExecuteCallbackCalled);

            canExecuteCallbackCalled = false;
            cmd.Execute(null);

            Assert.IsTrue(canExecuteCallbackCalled);
            Assert.IsTrue(onExecuteCallbackCalled);

            canExecuteCallbackCalled = false; 
            onExecuteCallbackCalled = false;
            cmd.CanExecute((object)13);

            Assert.IsTrue(canExecuteCallbackCalled);
            Assert.IsFalse(onExecuteCallbackCalled);

            canExecuteCallbackCalled = false;
            cmd.Execute((object)13);

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
                => CanExecuteCallback?.Invoke(parameter) ?? true;

            protected override void OnExecute(int parameter) 
                => this.OnExecuteCallback?.Invoke(parameter);
        }
    }
}
