namespace SIM.Mvvm.CodeGeneration
{
    using Microsoft.CodeAnalysis;

    internal interface IContentGenerator
    {
        void OnVisitSyntaxNode(GeneratorSyntaxContext context);

        void Execute(GeneratorExecutionContext context);

        void PostInititialize(GeneratorPostInitializationContext context);
    }
}
