namespace Mvvm.Test.ViewModel
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SIM.Mvvm;
    using System.Windows.Input;
    using SIM.Mvvm.Expressions;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [TestClass]
    public class ExpressionViewModelTest
    {
        [TestMethod]
        public void Constructor_Test()
        {
            var vm = new ExpressionViewModelMock();
            Assert.IsNotNull(vm);

            var vm1 = new ExpressionViewModelMock(new EventCommand());
            Assert.IsNotNull(vm1);
        }

        [TestMethod]
        public void DirectDependencies_Test()
        {
            int counter = 0;
            var vm = new ExpressionViewModelMock();

            vm.Name = "Initial";
            vm.Age = 0;

            vm.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == nameof(vm.AgedName))
                {
                    counter++;
                }
            };

            vm.Name = "SomeName";
            vm.Age = 42;

            Assert.AreEqual(2, counter);
        }

        [TestMethod]
        public void SubModelDependency_Test()
        {
            int counter = 0;
            var vm = new ExpressionViewModelMock();

            vm.model.FirstValue = 0;
            vm.model.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == nameof(vm.model.FirstValue))
                {
                    counter++;
                }
            };

            vm.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == nameof(vm.FirstValue))
                {
                    counter++;
                }
            };

            vm.model.FirstValue = 42;

            Assert.AreEqual(2, counter);
        }

        [TestMethod]
        public void DirectCommandNotify_Test()
        {
            int counter = 0;
            var vm = new ExpressionViewModelMock();

            vm.CommandCanExecute = false;
            vm.model.FirstValue = 0;

            vm.Command.CanExecuteChanged += (s, a) => counter++;

            vm.CommandCanExecute = true;
            vm.model.FirstValue = 42;

            Assert.AreEqual(2, counter);
        }

        [TestMethod]
        public void UpdatedCommandNotify_Test()
        {
            int counter = 0;
            var vm = new ExpressionViewModelMock(new EventCommand());

            vm.CommandCanExecute = false;
            vm.model.FirstValue = 0;

            vm.Command.CanExecuteChanged += (s, a) => counter++;

            vm.CommandCanExecute = true;
            vm.model.FirstValue = 42;

            Assert.AreEqual(2, counter);
        }
    }

    public class ExpressionViewModelMock : ViewModel
    {
        internal Model model = new Model();
        private int age;
        private string name = "";
        private ICommand command;
        private bool commandCanExecute;

        public ExpressionViewModelMock()
        {
            // Registers command dependencies
            this.command = new RelayCommand(() => CommandExecutedCount++, () => this.CommandCanExecute);

            this.Listen(() => this.CommandCanExecute, () => this.model.FirstValue)
                .Notify(() => this.Command);

            // Aged Name depends on name and age.
            this.Listen(() => this.Name, () => this.Age)
                .Notify(() => this.AgedName);

            // forward property changed event.
            this.Listen(() => this.model.FirstValue)
                .Notify(() => this.FirstValue);
        }

        /// <summary>
        /// Overrides the "registered" command, so check if can execute will invoke after doing this.
        /// </summary>
        /// <param name="command"></param>
        public ExpressionViewModelMock(ICommand command)
            : this()
        {
            this.Command = command;
        }
        public int PropertyChangedRaisedCount { get; private set; }
        public int CommandExecutedCount { get; private set; }

        public int FirstValue => model.FirstValue;

        public int Age
        {
            get => this.age;
            set => this.SetPropertyValue(ref this.age, value);
        }

        public ICommand Command
        {
            get => command;
            set => this.SetPropertyValue(ref command, value);
        }

        public string Name
        {
            get => this.name;
            set => this.SetPropertyValue(ref this.name, value);
        }

        public bool CommandCanExecute
        {
            get => commandCanExecute;
            set => this.SetPropertyValue(ref commandCanExecute, value);
        }

        public string AgedName => $"{this.Name} ({this.Age})";

        public ExpressionViewModelMock RegisterCounter()
        {
            base.PropertyChanged += (s, a) => this.PropertyChangedRaisedCount++;

            return this;
        }

        internal class Model : IViewModel
        {
            private int firstValue;

            public int FirstValue
            {
                get => firstValue;
                set
                {
                    firstValue = value;
                    this.OnPropertyChanged();
                }
            }

            public Collection<IPropertyMonitor> PropertyMonitors { get; }
                = new Collection<IPropertyMonitor>();

            public event PropertyChangedEventHandler? PropertyChanged;

            public void OnPropertyChanged([CallerMemberName] string propertyName = "")
                => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
