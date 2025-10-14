
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

        var interfaceSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(inter);

        InterfaceInfo info = InterfaceInfo.Build(interfaceSymbol!);
        // set the module
        info.containingNamespace = interfaceSymbol!.ContainingNamespace.Name;

        SenderBuilder.Build(sourceContext, info);
        DispatcherBuilder.Build(sourceContext, info);
    }
}