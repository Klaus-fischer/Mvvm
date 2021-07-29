namespace Mvvm.Test.ViewModel
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SIM.Mvvm;
    using System;
    using System.ComponentModel;

    [TestClass]
    public class CallOnPropertyChangedAttributeTests
    {
        [TestMethod]
        public void Constructor()
        {
            var attribute = new CallOnPropertyChangedAttribute("Test");
            Assert.IsNotNull(attribute);
            Assert.IsNotNull(attribute.PropertyNames);
            Assert.AreEqual("Test", attribute.PropertyNames[0]);
        }

        [TestMethod]
        public void AssignCallOnPropertyChanged()
        {
            bool invoked = false;
            var vm = new ValidTestVm();
            Assert.IsNotNull(vm);
            vm.OnTestChangedInvoked += () =>
            {
                invoked = true;
            };

            vm.Test = 42;

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void AssignCallOnPropertyChangedAdvanced()
        {
            bool invoked = false;
            var vm = new ValidAdvTestVm();
            Assert.IsNotNull(vm);

            vm.Test = 0;
            vm.OnTestChangedInvoked += (args) =>
            {
                invoked = true;
                Assert.AreEqual("Test", args.PropertyName);
                Assert.AreEqual(0, args.Before);
                Assert.AreEqual(42, args.After);
            };

            vm.Test = 42;

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void AssignCallOnPropertyChangedAdvanced_PC()
        {
            bool invoked = false;
            var vm = new ValidAdvTestPC();
            Assert.IsNotNull(vm);
            vm.Test = 0;

            vm.OnTestChangedInvoked += (args) =>
            {
                invoked = true;
                Assert.AreEqual("Test", args.PropertyName);
            };

            vm.Test = 42;

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void AssignCallOnPropertyChangedAdvanced_EA()
        {
            bool invoked = false;
            var vm = new ValidAdvTestEA();
            Assert.IsNotNull(vm);
            vm.Test = 0;

            vm.OnTestChangedInvoked += (args) =>
            {
                invoked = true;
            };

            vm.Test = 42;

            Assert.IsTrue(invoked);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InvalidAssignCallOnPropertyChanged_ReturnType()
        {
            bool invoked = false;
            var vm = new InvalidTestVm_Return();
            Assert.IsNotNull(vm);
            vm.OnTestChangedInvoked += () =>
            {
                invoked = true;
            };

            vm.Test = 42;

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InvalidAssignCallOnPropertyChanged_ArgLength()
        {
            bool invoked = false;
            var vm = new InvalidTestVm_ArgLength();
            Assert.IsNotNull(vm);
            vm.OnTestChangedInvoked += () =>
            {
                invoked = true;
            };

            vm.Test = 42;

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InvalidAssignCallOnPropertyChanged_ArgType()
        {
            bool invoked = false;
            var vm = new InvalidTestVm_ArgType();
            Assert.IsNotNull(vm);
            vm.OnTestChangedInvoked += () =>
            {
                invoked = true;
            };

            vm.Test = 42;

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InvalidAssignCallOnPropertyChanged_ArgType2()
        {
            bool invoked = false;
            var vm = new InvalidTestVm_ArgType2();
            Assert.IsNotNull(vm);
            vm.OnTestChangedInvoked += () =>
            {
                invoked = true;
            };

            vm.Test = 42;

            Assert.IsTrue(invoked);
        }

        public class TestVM<T> : ViewModel
        {
            private T test;

            public T Test
            {
                get { return test; }
                set { this.SetPropertyValue(ref test, value); }
            }
        }

        public class ValidTestVm : TestVM<int>
        {
            public event Action? OnTestChangedInvoked;

            [CallOnPropertyChanged(nameof(Test))]
            public void OnTestChanged()
            {
                this.OnTestChangedInvoked?.Invoke();
            }
        }

        public class ValidAdvTestVm : TestVM<int>
        {
            public event Action<AdvancedPropertyChangedEventArgs>? OnTestChangedInvoked;

            [CallOnPropertyChanged(nameof(Test))]
            public void OnTestChanged(object sender, AdvancedPropertyChangedEventArgs args)
            {
                this.OnTestChangedInvoked?.Invoke(args);
            }
        }

        public class ValidAdvTestPC : TestVM<int>
        {
            public event Action<PropertyChangedEventArgs>? OnTestChangedInvoked;

            [CallOnPropertyChanged(nameof(Test))]
            public void OnTestChanged(object sender, PropertyChangedEventArgs args)
            {
                this.OnTestChangedInvoked?.Invoke(args);
            }
        }

        public class ValidAdvTestEA : TestVM<int>
        {
            public event Action<EventArgs>? OnTestChangedInvoked;

            [CallOnPropertyChanged(nameof(Test))]
            public void OnTestChanged(object sender, EventArgs args)
            {
                this.OnTestChangedInvoked?.Invoke(args);
            }
        }

        public class InvalidTestVm_Return : TestVM<int>
        {
            public event Action? OnTestChangedInvoked;

            [CallOnPropertyChanged(nameof(Test))]
            public bool OnTestChanged()
            {
                this.OnTestChangedInvoked?.Invoke();
                return true;
            }
        }

        public class InvalidTestVm_ArgLength : TestVM<int>
        {
            public event Action? OnTestChangedInvoked;

            [CallOnPropertyChanged(nameof(Test))]
            public void OnTestChanged(AdvancedPropertyChangedEventArgs args)
            {
                this.OnTestChangedInvoked?.Invoke();
            }
        }

        public class InvalidTestVm_ArgType : TestVM<int>
        {
            public event Action? OnTestChangedInvoked;

            [CallOnPropertyChanged(nameof(Test))]
            public void OnTestChanged(int something, AdvancedPropertyChangedEventArgs args)
            {
                this.OnTestChangedInvoked?.Invoke();
            }
        }

        public class InvalidTestVm_ArgType2 : TestVM<int>
        {
            public event Action? OnTestChangedInvoked;

            [CallOnPropertyChanged(nameof(Test))]
            public void OnTestChanged(object sender, object eventArgs)
            {
                this.OnTestChangedInvoked?.Invoke();
            }
        }
    }
}
