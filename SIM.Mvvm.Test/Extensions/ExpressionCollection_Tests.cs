namespace SIM.Mvvm.Test.Extensions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SIM.Mvvm.Expressions;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    [TestClass]
    public class ExpressionCollection_Tests : INotifyPropertyChanged
    {
        public string Property { get; set; } = "initial";

        public TestVm ViewModel { get; set; } = new TestVm();

        public ICommand TestCommand { get; } = new EventCommand();

        public event PropertyChangedEventHandler? PropertyChanged;

        [TestMethod]
        public void ListenExpression()
        {
            var monitorCollection = ExpressionCollectionExtensions
                .Listen(this, () => this.Property, () => this.ViewModel.IntProperty, () => this.Property)
                .ToArray();

            Assert.AreEqual(3, monitorCollection.Length);
            Assert.AreEqual(nameof(this.Property), monitorCollection[0].PropertyName);
            Assert.AreEqual(nameof(this.ViewModel.IntProperty), monitorCollection[1].PropertyName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ListenExpression_Fail()
        {
            var monitorCollection = ExpressionCollectionExtensions
                .Listen(this, () => this.ViewModel.ToString())
                .ToArray();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ListenExpression_Field()
        {
            var monitorCollection = ExpressionCollectionExtensions
                .Listen(this, () => this.ViewModel.IntField)
                .ToArray();
        }


        [TestMethod]
        public void CallBack()
        {
            var callbackRaised = 0;
            Action a = () => callbackRaised++;

            this.Property = "Alter Wert";
            this.ViewModel.IntProperty = 0;

            var monitor = ExpressionCollectionExtensions
                .Listen(this, () => this.Property, () => this.ViewModel.IntProperty)
                .Call(a, a);

            this.Property = "Neuer Wert";
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Property)));

            Assert.AreEqual(2, callbackRaised);

            callbackRaised = 0;

            this.ViewModel.IntProperty = 42;
            this.ViewModel.OnPropertyChanged(nameof(this.ViewModel.IntProperty));

            Assert.AreEqual(2, callbackRaised);
        }

        [TestMethod]
        public void EventHandler()
        {
            var callbackRaised = 0;
            EventHandler<AdvancedPropertyChangedEventArgs> eventHandler = (s, e) => callbackRaised++;

            this.Property = "Alter Wert";
            this.ViewModel.IntProperty = -1;

            var monitor = ExpressionCollectionExtensions
                .Listen(this, () => this.Property, () => this.ViewModel.IntProperty)
                .Call(eventHandler, eventHandler);

            this.Property = "Neuer Wert";
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Property)));

            Assert.AreEqual(2, callbackRaised);

            callbackRaised = 0;

            this.ViewModel.IntProperty = 42;
            this.ViewModel.OnPropertyChanged(nameof(this.ViewModel.IntProperty));

            Assert.AreEqual(2, callbackRaised);
        }

        [TestMethod]
        public void NotifyViewModel()
        {
            var callbackRaised = false;
            PropertyChangedEventHandler eventHandler = (s, a) =>
            {
                if (a.PropertyName == nameof(this.ViewModel.SubProperty))
                {
                    callbackRaised = true;
                }
            };

            this.ViewModel.PropertyChanged += eventHandler;
            try
            {

                this.Property = "Alter Wert";
                this.ViewModel.IntProperty = -1;

                var monitor = ExpressionCollectionExtensions
                    .Listen(this, () => this.Property, () => this.ViewModel.IntProperty)
                    .Notify(() => this.ViewModel.SubProperty);

                this.Property = "Neuer Wert";
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Property)));

                Assert.IsTrue(callbackRaised);

                callbackRaised = false;

                this.ViewModel.IntProperty = 42;
                this.ViewModel.OnPropertyChanged(nameof(this.ViewModel.IntProperty));

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
                this.ViewModel.IntProperty = -1;

                var monitor = ExpressionCollectionExtensions
                    .Listen(this, () => this.Property, () => this.ViewModel.IntProperty)
                    .Notify(() => this.TestCommand);

                this.Property = "Neuer Wert";
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Property)));

                Assert.IsTrue(callbackRaised);

                callbackRaised = false;

                this.ViewModel.IntProperty = 42;
                this.ViewModel.OnPropertyChanged(nameof(this.ViewModel.IntProperty));

                Assert.IsTrue(callbackRaised);
            }
            finally
            {
                this.TestCommand.CanExecuteChanged -= eventHandler;
            }
        }

        public class TestVm : IViewModel
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            public string SubProperty { get; set; } = "initial";

            public int IntProperty { get; set; }

            public int IntField;

            public Collection<IPropertyMonitor> PropertyMonitors { get; }
                = new Collection<IPropertyMonitor>();

            public void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
