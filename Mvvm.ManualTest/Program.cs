namespace Mvvm.ManualTest
{
    using Mvvm.Core;
    using System;
    using System.Threading;
    using System.Windows.Input;

    class Program
    {
        static void Main(string[] args)
        {
            var viewModel = new ViewModel();
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            viewModel.Command.CanExecuteChanged += Command_CanExecuteChanged;
            viewModel.Property = 5;

            Console.ReadLine();
            viewModel.Property = 0;

            Console.ReadLine();
        }

        private static void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Command.CanExecuteChanged Raised");
        }

        private static void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine(e.PropertyName);
        }
    }

    class ViewModel : BaseViewModel
    {
        private int _property;
        public int Property { get; set; }
        //{
        //    get => _property;
        //    set => _property = this.SetPropertyValue(value, _property); 
        //}

        public ICommand Command { get; }

        public ViewModel()
        {
            this.Command = new EventCommand()
                .RegisterPropertyDependency(this, nameof(Property));
        }

    }
}
