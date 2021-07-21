namespace SIM.Mvvm.CodeGeneration.Tests
{
    public partial class AutoMapViewModel : ViewModel
    {
        [AutoMapProperties]
        public Model Data { get; } = new Model();
    }
}
