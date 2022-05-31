namespace SIM.Mvvm.CodeGeneration;

using Microsoft.CodeAnalysis;

internal class SetterVisibilityContentGenerator : IContentGenerator
{
    private const string fullName = "SIM.Mvvm.CodeGeneration.SetterVisibility";
    private const string content = @"
// <copyright file=""GeneratePropertyAttribute.cs"" company=""SIM Automation"">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.CodeGeneration
{
    internal enum SetterVisibility
    {
        Private = 0,
        Protected = 1,
        Internal = 2,
        Public = 3,
    }
}
";

    public void PostInititialize(GeneratorPostInitializationContext context)
    {
        context.AddSource("SetterVisibility.g.cs", content);
    }

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
    }
}
