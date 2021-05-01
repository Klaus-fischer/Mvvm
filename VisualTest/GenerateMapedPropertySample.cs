namespace Mvvm.VisualTest
{
    using Mvvm.Core;
    using Mvvm.Core.CodeGeneration;

    public partial class GenerateMapedProperty : BaseViewModel
    {
        public GenerateMapedProperty()
        {
            this.AdvancedPropertyChanged += this.GenerateMapedPropertySample_AdvancedPropertyChanged;
        }

        private void GenerateMapedPropertySample_AdvancedPropertyChanged(object? sender, AdvancedPropertyChangedEventArgs e)
        {
        }

        [AutoMapProperties]
        public Data DataObject { get; set; } = new Data();
    }
}
