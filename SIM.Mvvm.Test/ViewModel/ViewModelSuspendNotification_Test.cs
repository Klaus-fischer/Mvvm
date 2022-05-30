namespace Mvvm.Test.ViewModel
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SIM.Mvvm;

    [TestClass]
    public class ViewModelSuspendNotification_Test
    {
        [TestMethod]
        public void SuppressNotification()
        {
            var counter = 0;
            var vm = new TestVM();
            vm.PropertyChanged += (s, a) => counter++;

            // initial value
            vm.MyProperty = 0;
            counter = 0;

            vm.MyProperty = 42;
            Assert.AreEqual(1, counter, "PropertyChanged was not raised, ViewModel is broken.");

            vm.SuppressNotifications(nameof(vm.MyProperty), vm.MyProperty);

            vm.MyProperty = 21;
            Assert.AreEqual(1, counter, "PropertyChanged was raised after SuppressNotifications.");
        }

        [TestMethod]
        public void RestoreNotification()
        {
            var counter = 0;
            var vm = new TestVM();
            vm.PropertyChanged += (s, a) => counter++;

            vm.SuppressNotifications(nameof(vm.MyProperty), vm.MyProperty);

            // initial value
            vm.MyProperty = 0;
            counter = 0;

            vm.MyProperty = 42;
            Assert.AreEqual(0, counter, "PropertyChanged was raised after SuppressNotifications.");

            vm.RestoreNotifications(nameof(vm.MyProperty), vm.MyProperty);
            Assert.AreEqual(1, counter, "PropertyChanged was not raised after RestoreNotifications");
        }

        [TestMethod]
        public void SuppressNotificationExpression()
        {
            var counter = 0;
            var vm = new TestVM();
            vm.PropertyChanged += (s, a) => counter++;

            // initial value
            vm.MyProperty = 0;
            counter = 0;

            vm.MyProperty = 42;
            Assert.AreEqual(1, counter, "PropertyChanged was not raised, ViewModel is broken.");

            vm.SuppressNotifications(() => vm.MyProperty);

            vm.MyProperty = 21;
            Assert.AreEqual(1, counter, "PropertyChanged was raised after SuppressNotifications.");
        }

        [TestMethod]
        public void RestoreNotificationExpression()
        {
            var counter = 0;
            var vm = new TestVM();
            vm.PropertyChanged += (s, a) => counter++;

            vm.SuppressNotifications(() => vm.MyProperty);

            // initial value
            vm.MyProperty = 0;
            counter = 0;

            vm.MyProperty = 42;
            Assert.AreEqual(0, counter, "PropertyChanged was raised after SuppressNotifications.");

            vm.RestoreNotifications(() => vm.MyProperty);
            Assert.AreEqual(1, counter, "PropertyChanged was not raised after RestoreNotifications");
        }

        [TestMethod]
        public void SuppressNotificationsInside()
        {
            var counter = 0;
            var vm = new TestVM();
            vm.PropertyChanged += (s, a) => counter++;

            // initial value
            vm.MyProperty = 0;
            counter = 0;

            vm.SuppressNotificationsInside(() => vm.MyProperty, () =>
            {
                vm.MyProperty = 42;
                vm.MyProperty = 21;
                vm.MyProperty = 0;

                // last value is different than initial value, to PropertyChanged should call
                vm.MyProperty = 42;
            });

            Assert.AreEqual(42, vm.MyProperty);

            Assert.AreEqual(1, counter, "PropertyChanged was raised more or less than once.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SuppressNotificationExpression_Fail()
        {
            var vm = new TestVM();

            vm.SuppressNotifications(() => 42);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RestoreNotificationExpression_Fail()
        {
            var vm = new TestVM();

            vm.RestoreNotifications(() => 42);
        }


        class TestVM : SIM.Mvvm.ViewModel
        {
            private int myProperty;

            public int MyProperty
            {
                get => myProperty;
                set => this.SetPropertyValue(ref myProperty, value);
            }
        }
    }
}
