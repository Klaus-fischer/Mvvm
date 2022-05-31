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
                    syntaxReceiver.Execute(context);
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
            var receiver = new SyntaxReceiver();
            context.RegisterForSyntaxNotifications(() => receiver);
            context.RegisterForPostInitialization(receiver.PostInititialize);
        }
    }
}
