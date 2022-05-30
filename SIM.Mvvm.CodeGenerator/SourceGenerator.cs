namespace SIM.Mvvm.CodeGeneration
{
    using Microsoft.CodeAnalysis;
    using System;
    using System.Diagnostics;

    [Generator]
    public partial class SourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is SyntaxReceiver syntaxReceiver)
            {
                try
                {
                    var propertyGenerator = new GeneratePropertiesGenerator(syntaxReceiver);
                    propertyGenerator.Execute(context);

                    var autoMapGenerator = new AutoMapPropertiesGenerator(syntaxReceiver);
                    autoMapGenerator.Execute(context);
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                    else
                    {
                        Debugger.Launch();
                    }
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }
    }
}
