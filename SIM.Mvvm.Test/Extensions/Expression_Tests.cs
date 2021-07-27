namespace SIM.Mvvm.Test.Extensions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using SIM.Mvvm.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    [TestClass]
    public class Expression_Tests : INotifyPropertyChanged
    {
        public string Property { get; set; } = "initial";

        public TestVm ViewModel { get; set; } = new TestVm();

        public ICommand TestCommand { get; } = new EventCommand();

        public event PropertyChangedEventHandler? PropertyChanged;

        [TestMethod]
        public void ListenExpression()
        {
            var monitor1 = ExpressionExtensions.Listen(this, () => this.Property);
            Assert.IsNotNull(monitor1);

            var monitor2 = ExpressionExtensions.Listen(this, () => this.ViewModel.Property);
            Assert.IsNotNull(monitor2);

            Assert.AreNotEqual(monitor1, monitor2);

            monitor2 = ExpressionExtensions.Listen(this, () => this.Property);

            Assert.AreEqual(monitor1, monitor2);
        }

        [TestMethod]
        public void CallBack()
        {
            var callbackRaised = false;
            Action a = () => callbackRaised = true;

            this.Property = "Alter Wert";

            var monitor = ExpressionExtensions.Listen(this, () => this.Property)
                .Call(a);

            this.Property = "Neuer Wert";
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

                var monitor = ExpressionExtensions.Listen(this, () => this.Property)
                    .Notify(() => this.ViewModel.Property);

                this.Property = "Neuer Wert";
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Property)));

                Assert.IsTrue(callbackRaised);
            }
            finally
            {
                this.ViewModel.PropertyChanged -= eventHandler;
            }
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

                var monitor = ExpressionExtensions.Listen(this, () => this.Property)
                    .Notify(this.TestCommand);

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
        public void NotifyCommandByListen()
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

                ExpressionExtensions.Listen((ICommandInvokeCanExecuteChangedEvent)this.TestCommand, () => this.Property);

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
        public void EventHandler()
        {
            var callbackRaised = false;
            EventHandler<AdvancedPropertyChangedEventArgs> a = (s, e) => callbackRaised = true;

            this.Property = "Alter Wert";

            var monitor = ExpressionExtensions.Listen(null, () => this.Property)
                .Call(a);

            this.Property = "Neuer Wert";
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Property)));

            Assert.IsTrue(callbackRaised);
        }

        public class TestVm : IViewModel
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            public string Property { get; set; } = "initial";

            public Collection<IPropertyMonitor> PropertyMonitors { get; } 
                = new Collection<IPropertyMonitor>();

            public void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
