namespace Mvvm.Test.ViewModel
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Mvvm.Core;
    using System;

    [TestClass]
    public class BaseViewModelTest
    {

        [TestMethod]
        public void OnPropertyChangedTest()
        {
            bool propertChangedWasRised = false;
            bool advPropertChangedWasRised = false;

            var vm = new ViewModelMock();
            vm.AdvancedPropertyChanged += (s, a) =>
            {
                Assert.AreEqual("Test", a.PropertyName);
                Assert.AreEqual("before", a.Before);
                Assert.AreEqual("after", a.After);
                propertChangedWasRised = true;
            };

            vm.PropertyChanged += (s, a) =>
            {
                Assert.AreEqual("Test", a.PropertyName);
                advPropertChangedWasRised = true;
            };

            vm.OnPropertyChanged("Test", "before", "after");

            Assert.IsTrue(propertChangedWasRised);
            Assert.IsTrue(advPropertChangedWasRised);
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

            Assert.AreEqual(1, vm.AdvancedPropertyChangedRaisedCount);
            Assert.AreEqual(1, vm.PropertyChangedRaisedCount);
            Assert.AreEqual(0, value);
        }

        [TestMethod]
        public void InvokePropertyTestChangedTest()
        {
            var vm = new ViewModelMock().RegisterCounter();

            ((IViewModel)vm).InvokeOnPropertyChanged("Test");
            vm.PropertyChanged += (s, a) => Assert.AreEqual("Test", a.PropertyName);

            Assert.AreEqual(1, vm.AdvancedPropertyChangedRaisedCount);
            Assert.AreEqual(1, vm.PropertyChangedRaisedCount);
        }

        [TestMethod]
        public void SetPropertyValueTest_EqualReference()
        {
            var vm = new ViewModelMock().RegisterCounter();

            var obj = new object();

            vm.SetPropertyValue(ref obj, obj, "PropertyName");

            Assert.AreEqual(0, vm.AdvancedPropertyChangedRaisedCount);
            Assert.AreEqual(0, vm.PropertyChangedRaisedCount);
        }

        [TestMethod]
        public void SetPropertyValueTest_NUll()
        {
            var vm = new ViewModelMock().RegisterCounter();

            object oldValue = null;
            vm.SetPropertyValue(ref oldValue, null, "PropertyName");

            Assert.AreEqual(0, vm.AdvancedPropertyChangedRaisedCount);
            Assert.AreEqual(0, vm.PropertyChangedRaisedCount);
        }

        [TestMethod]
        public void SetPropertyValueTest_Equal()
        {
            var vm = new ViewModelMock().RegisterCounter();

            string oldValue = "String";
            vm.SetPropertyValue(ref oldValue, "String", "PropertyName");

            Assert.AreEqual(0, vm.AdvancedPropertyChangedRaisedCount);
            Assert.AreEqual(0, vm.PropertyChangedRaisedCount);
        }

        public class ViewModelMock : BaseViewModel
        {
            public ViewModelMock RegisterCounter()
            {
                base.AdvancedPropertyChanged += (s, a) => AdvancedPropertyChangedRaisedCount++;
                base.PropertyChanged += (s, a) => PropertyChangedRaisedCount++;

                return this;
            }

            public int AdvancedPropertyChangedRaisedCount { get; private set; }
            public int PropertyChangedRaisedCount { get; private set; }

            public new event EventHandler<AdvancedPropertyChangedEventArgs> AdvancedPropertyChanged
            {
                add
                {
                    base.AdvancedPropertyChanged += value;
                }
                remove
                {
                    base.AdvancedPropertyChanged -= value;
                }
            }

            public new void OnPropertyChanged(string propertyName, object before, object after)
            {
                base.OnPropertyChanged(propertyName, before, after);
            }

            public new void SetPropertyValue<T>(ref T oldValue, T newValue, string propertyName)
            {
                base.SetPropertyValue<T>(ref oldValue, newValue, propertyName);
            }
        }
    }
}
