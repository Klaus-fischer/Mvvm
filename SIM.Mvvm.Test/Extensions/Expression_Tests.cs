namespace SIM.Mvvm.Test.Extensions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    [TestClass]
    public class Expression_Tests : INotifyPropertyChanged, IListenerHost
    {
        public string Property { get; set; } = "initial";
        public string SecondProperty { get; set; } = "initial";

        public TestVm ViewModel { get; set; } = new TestVm();

        public ICommand TestCommand { get; } = new EventCommand();

        public event PropertyChangedEventHandler? PropertyChanged;

        [TestMethod]
        public void ListenExpression()
        {
            var monitor1 = IListenerHostExtensions.Listen(this, v => v.Property);
            Assert.IsNotNull(monitor1);

            var monitor2 = IListenerHostExtensions.Monitor(this, this.ViewModel, v => v.Property);
            Assert.IsNotNull(monitor2);

            Assert.AreNotEqual(monitor1, monitor2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ListenExpression_Fail_Expression()
        {
            var monitor1 = IListenerHostExtensions.Listen(this, v => 42);
        }

        [TestMethod]
        public void CallBack()
        {
            var callbackRaised = false;
            Action a = () => callbackRaised = true;

            this.Property = "Alter Wert";

            var monitor = IListenerHostExtensions.Listen(this, v => v.Property)
                .Call(a);

            this.Property = "Neuer Wert";

            Assert.IsFalse(callbackRaised);
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Property)));

            Assert.IsTrue(callbackRaised);
        }


        [TestMethod]
        public void NotifyViewModel()
        {
            var callbackRaised = false;
            PropertyChangedEventHandler eventHandler = (s, a) =>
            {
                if (a.PropertyName == nameof(this.ViewModel.Property))
                {
                    callbackRaised = true;
                }
            };

            this.ViewModel.PropertyChanged += eventHandler;
            try
            {
                this.Property = "Alter Wert";

                var monitor = IListenerHostExtensions.Listen(this, v => v.Property)
                    .Notify(() => this.ViewModel.Property);

                this.Property = "Neuer Wert";

                Assert.IsFalse(callbackRaised);
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Property)));

                Assert.IsTrue(callbackRaised);
            }
            finally
            {
                this.ViewModel.PropertyChanged -= eventHandler;
            }
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NotifyViewModel_Fail_Type()
        {
            var target = new List<string>();

            var monitor = IListenerHostExtensions
                .Listen(this, v => v.Property)
                .Notify(() => target.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NotifyViewModel_Fail_Expression()
        {
            var monitor = IListenerHostExtensions
                   .Listen(this,v => v.Property)
                   .Notify(() => 42);
        }

        [TestMethod]
        public void NotifyCommand()
        {
            var callbackRaised = false;
            EventHandler eventHandler = (s, a) =>
            {
                callbackRaised = true;
            };

            this.TestCommand.CanExecuteChanged += eventHandler;

            try
            {
                this.Property = "Alter Wert";

                var monitor = IListenerHostExtensions.Listen(this, v => v.Property)
                    .Notify(() => this.TestCommand);

                this.Property = "Neuer Wert";
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Property)));

                Assert.IsTrue(callbackRaised);
            }
            finally
            {
                this.TestCommand.CanExecuteChanged -= eventHandler;
            }
        }

        [TestMethod]
        public void Listener()
        {
            var callbackRaised = false;
            EventHandler<OnPropertyChangedEventArgs<string>> a = (s, e) =>
            {
                Assert.AreSame(this.ViewModel, s);
                Assert.AreEqual("Alter Wert", e.Before);
                Assert.AreEqual("Neuer Wert", e.After);
                Assert.AreEqual("Property", e.PropertyName);
                callbackRaised = true;
            };

            this.ViewModel.Property = "Alter Wert";

            var monitor = this.ViewModel.Monitor(p => p.Property);
            monitor.Call(a);

            this.ViewModel.Property = "Neuer Wert";
            this.ViewModel.OnPropertyChanged(nameof(this.Property));

            Assert.IsTrue(callbackRaised);
        }

        public void AddListener(IPropertyListener listener)
        {
        }

        public class TestVm : IViewModel, IListenerHost
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            public string Property { get; set; } = "initial";

            public void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public void AddListener(IPropertyListener listener)
            {
                this.AddListenerRaised?.Invoke(this, listener);
            }

            public event EventHandler<IPropertyListener>? AddListenerRaised;
        }
    }
}
