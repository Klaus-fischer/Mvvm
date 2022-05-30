namespace SIM.Mvvm.CodeGeneration
{

    public class GeneratePropertyAttributeDescription
    {
        public bool IsValid => string.IsNullOrWhiteSpace(this.ExcludeMessage);

        public string ExcludeMessage { get; set; } = string.Empty;

        public string PropertyTypeNamespace { get; set; } = string.Empty;

        public string PropertyType { get; set; } = string.Empty;

        public string FieldName { get; set; } = string.Empty;

        public string PropertyName { get; set; } = string.Empty;

        public string SetterVisibility { get; set; } = string.Empty;
    }
}
