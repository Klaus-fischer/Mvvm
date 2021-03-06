namespace SIM.Mvvm.CodeGeneration
{
    internal class AutoMapPropertyDescription
    {
        public bool IsValid => string.IsNullOrWhiteSpace(this.ExcludeMessage);

        public string ExcludeMessage { get; set; } = string.Empty;

        public string PropertyTypeNamespace { get; set; } = string.Empty;

        public string PropertyType { get; set; } = string.Empty;

        public string PropertyName { get; set; } = string.Empty;

        public string ModelType { get; set; } = string.Empty;

        public string ModelName { get; set; } = string.Empty;
    }
}
