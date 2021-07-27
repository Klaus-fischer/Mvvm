namespace Mvvm.Test.ViewModel
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using SIM.Mvvm;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using SIM.Mvvm.Expressions;
    using System.Diagnostics.CodeAnalysis;

    [TestClass]
    public class BaseViewModelTest
    {

        [TestMethod]
        public void OnPropertyChangedTest()
        {
            bool propertChangedWasRised = false;

            var vm = new ViewModelMock();

            vm.OnPropertyChanged();

            vm.PropertyChanged += (s, a) =>
            {
                Assert.AreEqual("Test", a.PropertyName);
                propertChangedWasRised = true;
            };

            vm.OnPropertyChanged("Test", "before", "after");

            Assert.IsTrue(propertChangedWasRised);
        }

        [TestMethod]
        public void DependsOnTest()
        {
            List<string> changedProperties = new List<string>();
            var vm = new ViewModelMock();
            vm.PropertyChanged += (s, a) => changedProperties.Add(a.PropertyName);

            vm.Name = "Klaus";

            Assert.IsTrue(changedProperties.Contains(nameof(vm.Name)));
            Assert.IsTrue(changedProperties.Contains(nameof(vm.AgedName)));

            changedProperties.Clear();

            vm.Age = 35;

            Assert.IsTrue(changedProperties.Contains(nameof(vm.Age)));
            Assert.IsTrue(changedProperties.Contains(nameof(vm.AgedName)));

            Assert.AreEqual("Klaus (35)", vm.AgedName);
        }

        [TestMethod]
        public void OnPropertyChanged_NoEventsTest()
        {
            var vm = new ViewModelMock();
            vm.OnPropertyChanged("Test", "before", "after");
        }

        [TestMethod]
        public void SetPropertyValueTest()
        {
            var vm = new ViewModelMock().RegisterCounter();

            int value = 13;

            vm.SetPropertyValue(ref value, 0, "Test");

            Assert.AreEqual(1, vm.PropertyChangedRaisedCount);
            Assert.AreEqual(0, value);
        }

        [TestMethod]
        public void InvokePropertyTestChangedTest()
        {
            var vm = new ViewModelMock().RegisterCounter();

            ((IViewModel)vm).OnPropertyChanged("Test");
            vm.PropertyChanged += (s, a) => Assert.AreEqual("Test", a.PropertyName);

            Assert.AreEqual(1, vm.PropertyChangedRaisedCount);
        }

        [TestMethod]
        public void SetPropertyValueTest_EqualReference()
        {
            var vm = new ViewModelMock().RegisterCounter();

            var obj = new object();

            vm.SetPropertyValue(ref obj, obj, "PropertyName");

            Assert.AreEqual(0, vm.PropertyChangedRaisedCount);
        }

        [TestMethod]
        public void SetPropertyValueTest_Null()
        {
            var vm = new ViewModelMock().RegisterCounter();

            object oldValue = null;
            vm.SetPropertyValue(ref oldValue, null, "PropertyName");

            Assert.AreEqual(0, vm.PropertyChangedRaisedCount);
        }

        [TestMethod]
        public void SetPropertyValueTest_AssignNull()
        {
            var vm = new ViewModelMock().RegisterCounter();

            object oldValue = new object();
            vm.SetPropertyValue(ref oldValue, null, "PropertyName");

            Assert.AreEqual(1, vm.PropertyChangedRaisedCount);
        }

        [TestMethod]
        public void SetPropertyValueTest_ClearNull()
        {
            var vm = new ViewModelMock().RegisterCounter();

            object? oldValue = null;
            vm.SetPropertyValue(ref oldValue, new object(), "PropertyName");

            Assert.AreEqual(1, vm.PropertyChangedRaisedCount);
        }

        [TestMethod]
        public void SetPropertyValueTest_Equal()
        {
            var vm = new ViewModelMock().RegisterCounter();

            string oldValue = "String";
            vm.SetPropertyValue(ref oldValue, "String", "PropertyName");

            Assert.AreEqual(0, vm.PropertyChangedRaisedCount);
        }

        [TestMethod]
        public void SetModelPropertyTest()
        {
            var vm = new ViewModelMock().RegisterCounter();

            vm.model.FirstValue = 10;

            Assert.AreEqual(10, vm.model.FirstValue);
            Assert.AreEqual(0, vm.PropertyChangedRaisedCount);

            vm.FirstValue = 13;

            Assert.AreEqual(13, vm.model.FirstValue);
            Assert.AreEqual(1, vm.PropertyChangedRaisedCount);

            vm.model.FirstValue = 10;

            vm.FirstValue = 10;

            Assert.AreEqual(10, vm.model.FirstValue);
            Assert.AreEqual(1, vm.PropertyChangedRaisedCount);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetModelProperty_Fail()
        {
            var vm = new ViewModelMock();

            vm.SecondValue = 10;
        }

        [TestMethod]
        public void CommandInvokeTest()
        {
            var commandMock = new Mock<ICommand>();

            var vm = new ViewModelMock();

            vm.Command = commandMock.Object;
            vm.Command.CanExecuteChanged += (s, a) => Assert.Fail();

            vm.Name = "anything";

            vm.Command = new RelayCommand(() => { });

            bool canExecuteChangedInvoked = false;
            vm.Command.CanExecuteChanged += (s, a) => canExecuteChangedInvoked = true;

            vm.Name = "NewName";

            Assert.IsTrue(canExecuteChangedInvoked);
        }

        [TestMethod]
        public void GetPropertyMonitorTest()
        {
            var vm = new ViewModelMock();
            var monitor = vm.Listen(() => vm.FirstValue);
            Assert.AreEqual(nameof(ViewModelMock.FirstValue), monitor.PropertyName);

            // get from collection
            string[] collection = new string[] { nameof(ViewModelMock.FirstValue), nameof(ViewModelMock.Command) };

            var monitors = vm.Listen(() => vm.FirstValue, () => vm.Command).ToArray();
            Assert.AreEqual(2, monitors.Length);
            Assert.IsTrue(collection.SequenceEqual(monitors.Select(o => o.PropertyName)));

            // get from empty collection
            collection = Array.Empty<string>();
            monitors = vm.Listen().ToArray();
            Assert.AreEqual(0, monitors.Length);

            // get from remove all collection
        }

        [TestMethod]
        public void InvokeEqualNotifier()
        {
            bool invoked = false;
            var vm = new ViewModelMock();
            vm.PropertyChanged += (s, a) => invoked = true;

            vm.InvariantCaseString = "HalloWelt";

            Assert.IsTrue(invoked);

            invoked = false;

            vm.InvariantCaseString = "hALLOwELT";
            Assert.IsFalse(invoked);
        }
    }

    public class ViewModelMock : ViewModel
    {
        public ViewModelMock RegisterCounter()
        {
            base.PropertyChanged += (s, a) => this.PropertyChangedRaisedCount++;

            return this;
        }

        public int PropertyChangedRaisedCount { get; private set; }

        public new void OnPropertyChanged(string propertyName, object before, object after)
        {
            base.OnPropertyChanged(propertyName);
        }

        public new void SetPropertyValue<T>(ref T? oldValue, T? newValue, string propertyName)
        {
            base.SetPropertyValue<T>(ref oldValue, newValue, null, propertyName);
        }

        internal Model model = new Model();

        public int FirstValue
        {
            get => this.model.FirstValue;
            set => this.SetPropertyValue(() => this.model.FirstValue, value);
        }

        public int SecondValue
        {
            get => this.model.FirstValue;
            set => this.SetPropertyValue(() => new object(), value);
        }

        private string invariantCaseString;
        public string InvariantCaseString
        {
            get => this.invariantCaseString;
            set => this.SetPropertyValue(() => this.invariantCaseString, value, InvariantCaseStringComparer.Current);

        }

        private int age;

        public int Age
        {
            get => this.age;
            set => this.SetPropertyValue(ref this.age, value);
        }

        [DependsOn(nameof(Name))]
        public ICommand Command
        {
            get => command;
            set => this.SetPropertyValue(ref command, value);
        }

        private string name;
        private ICommand command;

        public string Name
        {
            get => this.name;
            set => this.SetPropertyValue(ref this.name, value);
        }

        [DependsOn(nameof(Name), nameof(Age))]
        public string AgedName => $"{this.Name} ({this.Age})";


        internal class Model
        {
            public int FirstValue { get; set; }
        }

        internal class InvariantCaseStringComparer : IEqualityComparer<string>
        {
            private static InvariantCaseStringComparer current;

            /// <summary>
            /// Gets access to a singleton <see cref="InvariantCaseStringComparer"/>.
            /// </summary>
            public static InvariantCaseStringComparer Current
            {
                get
                {
                    if (current == null)
                    {
                        current = new InvariantCaseStringComparer();
                    }

                    return current;
                }
            }

            public bool Equals(string? x, string? y)
                => string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);


            public int GetHashCode([DisallowNull] string obj)
                => HashCode.Combine(obj);

        }
    }
}
