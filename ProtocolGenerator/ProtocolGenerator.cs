
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using System.Text;

namespace ProtocolGenerator;

[Generator(LanguageNames.CSharp)]
public class ProtocolGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(IsInterfaceDeclaration, TransformInterface);

        context.RegisterSourceOutput(provider, GenerateCode);
    }

    bool IsInterfaceDeclaration(SyntaxNode node, CancellationToken token)
    {
        return node.IsKind(SyntaxKind.InterfaceDeclaration);
    }

    GeneratorSyntaxContext TransformInterface(GeneratorSyntaxContext context, CancellationToken _)
    {
        return context;
    }

    void GenerateCode(SourceProductionContext sourceContext, GeneratorSyntaxContext syntaxContext)
    {
        StringBuilder builder = new StringBuilder();
        InterfaceDeclarationSyntax inter = (InterfaceDeclarationSyntax)syntaxContext.Node;

        string fileName = $"Protocol_{inter.Identifier.Text}.g.cs";

        SourceText code = GenerateProtocolCode(syntaxContext);
        sourceContext.AddSource(fileName, code);
    }

    SourceText GenerateProtocolCode(GeneratorSyntaxContext context)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine();

        return SourceText.From(builder.ToString(), Encoding.Unicode);
    }
}