namespace Mvvm.VisualTest
{
    using System.ComponentModel;

    public class Data : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public int Alter { get; set; }

        public int Herkunft { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
