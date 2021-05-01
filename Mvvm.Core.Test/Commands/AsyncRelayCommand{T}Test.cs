using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Mvvm.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mvvm.Test.Commands
{
    [TestClass]
    public class AsyncRelayCommand_T_Test
    {
        [TestMethod]
        public void Constructor()
        {
            IAsyncExecutionContext context = new AsyncExecutionContext();
            var rc = new AsyncRelayCommand<int>(context, async (i, t) => { });
            Assert.IsNotNull(rc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorFail()
        {
            IAsyncExecutionContext context = new AsyncExecutionContext();
            _ = new AsyncRelayCommand<int>(context, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorFail2()
        {
            IAsyncExecutionContext context = new AsyncExecutionContext();
            var rc = new AsyncRelayCommand<int>(null, async (i, t) => { });
            Assert.IsNotNull(rc);
        }

        [TestMethod]
        public void ActionInvokation()
        {
            IAsyncExecutionContext context = new AsyncExecutionContext();
            var invoked = false;

            var rc = new AsyncRelayCommand<int>(context, async (i, t) => invoked = true);

            Assert.IsFalse(invoked);

            rc.Execute(0);

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void ActionPredicateInvocation()
        {
            IAsyncExecutionContext context = new AsyncExecutionContext();
            var invoked = false;
            var canExecuteInvoked = false;

            var rc = new AsyncRelayCommand<int>(context, async (i, t) => invoked = true,
                i => { canExecuteInvoked = true; return true; });

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            Assert.IsTrue(rc.CanExecute(0));

            Assert.IsFalse(invoked);
            Assert.IsTrue(canExecuteInvoked);

            canExecuteInvoked = false;

            rc.Execute(0);

            Assert.IsTrue(invoked);
            Assert.IsTrue(canExecuteInvoked);
        }

        [TestMethod]
        public void ActionPredicateInvocationOnExecute()
        {
            IAsyncExecutionContext context = new AsyncExecutionContext();
            var invoked = false;
            var canExecuteInvoked = false;

            var rc = new AsyncRelayCommand<int>(context, async (i, t) => invoked = true,
                i => { canExecuteInvoked = true; return true; });

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            rc.Execute(0);

            Assert.IsTrue(invoked);
            Assert.IsTrue(canExecuteInvoked);
        }

        [TestMethod]
        public void ActionPreventExecution()
        {
            IAsyncExecutionContext context = new AsyncExecutionContext();
            var invoked = false;
            var canExecuteInvoked = false;

            var rc = new AsyncRelayCommand<int>(
                context,
                async (i, t) => invoked = true,
                i =>
                {
                    canExecuteInvoked = true;
                    return false;
                });

            Assert.IsFalse(invoked);
            Assert.IsFalse(canExecuteInvoked);

            rc.Execute(0);

            Assert.IsFalse(invoked);
            Assert.IsTrue(canExecuteInvoked);
        }

        [TestMethod]
        public void ParameterTransferTest()
        {
            IAsyncExecutionContext context = new AsyncExecutionContext();
            var param = 12356;

            var rc = new AsyncRelayCommand<int>(
                context,
                async (i, t) => Assert.AreEqual(param, i),
                i => i == param);

            rc.Execute(param);
        }

        [TestMethod]
        public void IsBusyTest()
        {
            var mock = new Mock<IAsyncExecutionContext>();
            mock.SetupGet(o => o.IsBusy).Returns(true);
            var invoked = false;

            var rc = new AsyncRelayCommand<int>(
                mock.Object,
                async (i, t) => invoked = true);

            Assert.IsFalse(invoked);

            rc.Execute(10);

            Assert.IsFalse(invoked);
        }

        [TestMethod]
        public async Task ContextValidation()
        {
            CancellationToken token = new CancellationToken();
            var invoked = false;

            var mock = new Mock<IAsyncExecutionContext>();
            mock.SetupGet(o => o.IsBusy).Returns(false);
            mock.Setup(o => o.PrepareExecution(out token));
            mock.Setup(o => o.FinalizeExecution());

            var rc = new AsyncRelayCommand<int>(
                mock.Object,
                async (i, t) =>
                {
                    invoked = true;
                    Assert.AreEqual(token, t);
                });

            await rc.ExecuteAsync(10);

            mock.VerifyGet(o => o.IsBusy, Times.AtLeastOnce());
            mock.Verify(o => o.PrepareExecution(out token), Times.Once());
            mock.Verify(o => o.FinalizeExecution(), Times.Once());

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public async Task ContextValidation_Exception()
        {
            CancellationToken token = new CancellationToken();
            var invoked = false;

            var contextMock = new Mock<IAsyncExecutionContext>();
            contextMock.SetupGet(o => o.IsBusy).Returns(false);
            contextMock.Setup(o => o.PrepareExecution(out token));
            contextMock.Setup(o => o.FinalizeExecution());

            var exceptionHandlerMock = new Mock<IExceptionHandler>();
            exceptionHandlerMock.Setup(o => o.HandleException(It.IsAny<Exception>())).Returns(false);

            var rc = new AsyncRelayCommand<int>(
                contextMock.Object,
                async (i, t) =>
                {
                    invoked = true;
                    throw new InvalidOperationException();
                });

            rc.ExceptionHandler = exceptionHandlerMock.Object;

            await rc.ExecuteAsync(10);

            contextMock.VerifyGet(o => o.IsBusy, Times.AtLeastOnce);
            contextMock.Verify(o => o.PrepareExecution(out token), Times.Once);
            contextMock.Verify(o => o.FinalizeExecution(), Times.Once);

            exceptionHandlerMock.Verify(o => o.HandleException(It.IsAny<Exception>()), Times.Once);

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ContextValidation_UnhandledException()
        {
            CancellationToken token = new CancellationToken();
            var invoked = false;

            var contextMock = new Mock<IAsyncExecutionContext>();
            contextMock.SetupGet(o => o.IsBusy).Returns(false);
            contextMock.Setup(o => o.PrepareExecution(out token));
            contextMock.Setup(o => o.FinalizeExecution());

            var rc = new AsyncRelayCommand<int>(
                contextMock.Object,
                async (i, t) => throw new InvalidOperationException());

            await rc.ExecuteAsync(10);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ContextValidation_UnhandelableException()
        {
            CancellationToken token = new CancellationToken();
            var invoked = false;

            var contextMock = new Mock<IAsyncExecutionContext>();
            contextMock.SetupGet(o => o.IsBusy).Returns(false);
            contextMock.Setup(o => o.PrepareExecution(out token));
            contextMock.Setup(o => o.FinalizeExecution());

            var exceptionHandlerMock = new Mock<IExceptionHandler>();
            exceptionHandlerMock.Setup(o => o.HandleException(It.IsAny<Exception>())).Returns(true);

            var rc = new AsyncRelayCommand<int>(
                contextMock.Object,
                async (i, t) => throw new InvalidOperationException());

            rc.ExceptionHandler = exceptionHandlerMock.Object;

            await rc.ExecuteAsync(10);
        }
    }
}
