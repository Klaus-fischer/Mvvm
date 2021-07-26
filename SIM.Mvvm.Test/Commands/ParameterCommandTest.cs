namespace SIM.Mvvm.Test.Commands
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SIM.Mvvm;
    using System;
    using System.Windows.Input;

    [TestClass]
    public class ParameterCommandTest
    {
        [TestMethod]
        public void CanExecuteTest()
        {
            var canExecuteCallbackCalled = false;
            var onExecuteCallbackCalled = false;

            ICommand cmd = new ParameterCommandType<int>
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
            cmd.CanExecute(13);

            Assert.IsTrue(canExecuteCallbackCalled);
            Assert.IsFalse(onExecuteCallbackCalled);

            canExecuteCallbackCalled = false;
            cmd.Execute(13);

            Assert.IsTrue(canExecuteCallbackCalled);
            Assert.IsTrue(onExecuteCallbackCalled);

            canExecuteCallbackCalled = false;
            onExecuteCallbackCalled = false;

            cmd = new ParameterCommandType<object>
            {
                CanExecuteCallback = i => { return canExecuteCallbackCalled = true; },
                OnExecuteCallback = i => onExecuteCallbackCalled = true,
                AllowDefault = false,
            };

            cmd.CanExecute(null);

            Assert.IsFalse(canExecuteCallbackCalled);
            Assert.IsFalse(onExecuteCallbackCalled);

            cmd.Execute(null);

            Assert.IsFalse(canExecuteCallbackCalled);
            Assert.IsFalse(onExecuteCallbackCalled);
        }

        [TestMethod]
        public void CanExecuteInterfaceTest()
        {
            var canExecuteCallbackCalled = false;
            var onExecuteCallbackCalled = false;

            IParameterCommand<int> cmd = new ParameterCommandType<int>()
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

            canExecuteCallbackCalled = false;
            onExecuteCallbackCalled = false;
        }

        [TestMethod]
        public void CanExecuteInterfaceTest2()
        {
            var canExecuteCallbackCalled = false;
            var onExecuteCallbackCalled = false;

            IParameterCommand<string> cmd = new ParameterCommandType<string>()
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

            cmd = new ParameterCommandType<string>
            {
                CanExecuteCallback = i => { return canExecuteCallbackCalled = true; },
                OnExecuteCallback = i => onExecuteCallbackCalled = true,
                AllowDefault = false,
            };

            cmd.CanExecute(null);

            Assert.IsFalse(canExecuteCallbackCalled);
            Assert.IsFalse(onExecuteCallbackCalled);

            cmd.Execute(null);

            Assert.IsFalse(canExecuteCallbackCalled);
            Assert.IsFalse(onExecuteCallbackCalled);

            ((ParameterCommand<string>)cmd).Execute(null);

            Assert.IsFalse(canExecuteCallbackCalled);
            Assert.IsFalse(onExecuteCallbackCalled);
        }

        [TestMethod]
        public void ICommandInvokeCanExecuteChangedEvent_Test()
        {
            bool eventHandlerInvoked = false;

            ICommandInvokeCanExecuteChangedEvent cmd = new ParameterCommandType<string>();

            EventHandler handler = (s, a) => eventHandlerInvoked = true;

            cmd.InvokeCanExecuteChanged();
            Assert.IsFalse(eventHandlerInvoked);

            cmd.CanExecuteChanged += handler;

            cmd.InvokeCanExecuteChanged();
            Assert.IsTrue(eventHandlerInvoked);
        }

        [TestMethod]
        public void DefaultCanExecute()
        {
            ICommand cmd = new DefaultCanExecuteCommand();
            Assert.IsTrue(cmd.CanExecute(null));
        }

        private class ParameterCommandType<T> : ParameterCommand<T>
        {
            public Func<T, bool> CanExecuteCallback;

            public Action<T> OnExecuteCallback;

            protected override bool CanExecute(T parameter)
                => CanExecuteCallback?.Invoke(parameter) ?? true;

            protected override void OnExecute(T parameter)
                => this.OnExecuteCallback?.Invoke(parameter);

            public new bool AllowDefault { get => base.AllowDefault; set => base.AllowDefault = value; }
        }

        private class DefaultCanExecuteCommand : ParameterCommand<int>
        {
            public Action<int> OnExecuteCallback;

            protected override void OnExecute(int parameter)
                => this.OnExecuteCallback?.Invoke(parameter);

            public new bool AllowDefault { get => base.AllowDefault; set => base.AllowDefault = value; }
        }
    }
}
