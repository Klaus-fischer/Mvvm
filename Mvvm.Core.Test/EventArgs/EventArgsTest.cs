namespace Mvvm.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mvvm.Core;

    [TestClass]
    public class EventArgsTest
    {
        [TestMethod]
        public void AdvancedPropertyChangedEventArgsTest()
        {
            var args = new AdvancedPropertyChangedEventArgs("name", 13, "after");

            Assert.IsNotNull(args);
            Assert.AreEqual("name", args.PropertyName);
            Assert.AreEqual(13, args.Before);
            Assert.AreEqual("after", args.After);
        }

        [TestMethod]
        public void CanExecuteEventArgsTest()
        {
            var args = new CanExecuteEventArgs();
            Assert.IsNotNull(args);
            Assert.IsTrue(args.CanExecute);

            args = new CanExecuteEventArgs()
            {
                CanExecute = false,
            };

            Assert.IsNotNull(args);
            Assert.IsFalse(args.CanExecute);
        }

        [TestMethod]
        public void CanExecuteEventArgs_T_Test()
        {
            var args = new CanExecuteEventArgs<int>(13);
            Assert.IsNotNull(args);
            Assert.IsTrue(args.CanExecute);
            Assert.AreEqual(13, args.Parameter);

            args = new CanExecuteEventArgs<int>(42)
            {
                CanExecute = false,
            };

            Assert.IsNotNull(args);
            Assert.IsFalse(args.CanExecute);
            Assert.AreEqual(42, args.Parameter);
        }
    }
}
