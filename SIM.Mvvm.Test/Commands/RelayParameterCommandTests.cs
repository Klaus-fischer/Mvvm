namespace SIM.Mvvm.Test.Commands
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Windows.Input;

    [TestClass]
    public class RelayParameterCommandTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var command = new Mock<ICommand>();
            var obj = new object();

            var cmd = new RelayParameterCommand(command.Object, () => obj);
            Assert.IsNotNull(cmd);
        }

        [TestMethod]
        public void Constructor2Test()
        {
            var command = new Mock<ICommand>();
            var obj = new object();

            var cmd = new RelayParameterCommand(command.Object, obj);
            Assert.IsNotNull(cmd);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Fail()
        {
            var command = new Mock<ICommand>();
            var obj = new object();

            var cmd = new RelayParameterCommand(null, () => obj);
            Assert.IsNotNull(cmd);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Fail2()
        {
            var command = new Mock<ICommand>();
            var obj = new object();

            var cmd = new RelayParameterCommand(command.Object, (Func<object>)null);
            Assert.IsNotNull(cmd);
        }

        [TestMethod]
        public void ForewardCanExecuteChanged()
        {
            var command = new Mock<ICommand>();
            var obj = new object();

            var cmd = new RelayParameterCommand(command.Object, () => obj);

            bool canExecuteChangedRaised = false;
            cmd.CanExecuteChanged += (s, a) => canExecuteChangedRaised = true;
            command.Raise(o => o.CanExecuteChanged -= null, new EventArgs());

            Assert.IsTrue(canExecuteChangedRaised);
        }

        [TestMethod]
        public void ForewardCanExecute()
        {
            var obj = new object();
            var command = new Mock<ICommand>();
            command.Setup(o => o.CanExecute(obj)).Returns(true);

            ICommand cmd = new RelayParameterCommand(command.Object, () => obj);

            var result = cmd.CanExecute(null);

            Assert.IsTrue(result);
            command.Verify(o => o.CanExecute(obj), Times.AtLeastOnce);
        }

        [TestMethod]
        public void ForewardCanExecuteFalse()
        {
            var obj = new object();
            var command = new Mock<ICommand>();
            command.Setup(o => o.CanExecute(obj)).Returns(false);

            ICommand cmd = new RelayParameterCommand(command.Object, () => obj);

            var result = cmd.CanExecute(null);

            Assert.IsFalse(result);
            command.Verify(o => o.CanExecute(obj), Times.AtLeastOnce);
        }

        [TestMethod]
        public void ForewardExecute()
        {
            var obj = new object();
            var command = new Mock<ICommand>();
            command.Setup(o => o.CanExecute(obj)).Returns(true);

            ICommand cmd = new RelayParameterCommand(command.Object, () => obj);

            cmd.Execute(null);

            command.Verify(o => o.CanExecute(obj), Times.AtLeastOnce);
            command.Verify(o => o.Execute(obj), Times.AtLeastOnce);

        }
    }
}
