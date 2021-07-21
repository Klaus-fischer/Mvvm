namespace SIM.Mvvm.CodeGeneration.Tests
{
    public partial class AutoMapExceptViewModel : ViewModel
    {
        [AutoMapProperties(nameof(Model.Age))]
        public Model Data { get; } = new Model();
    }
}
