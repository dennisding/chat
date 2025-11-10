

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace ProtocolGenerator;

public class PropertyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(IsClassDeclaration, TransformInterface);

        var allClassProvider = provider.Collect();

        context.RegisterSourceOutput(allClassProvider, GenerateAll);
    }

    bool IsClassDeclaration(SyntaxNode node, CancellationToken token)
    {
        return node.IsKind(SyntaxKind.ClassDeclaration);
    }

    GeneratorSyntaxContext TransformInterface(GeneratorSyntaxContext context, CancellationToken _)
    {
        return context;
    }

    void GenerateAll(SourceProductionContext sourceContext,
        ImmutableArray<GeneratorSyntaxContext> syntaxContexts)
    {
//        List<InterfaceInfo> infos = new List<InterfaceInfo>();

        foreach (var syntax in syntaxContexts)
        {
            InterfaceDeclarationSyntax inter = (InterfaceDeclarationSyntax)syntax.Node;
            var symbol = syntax.SemanticModel.GetDeclaredSymbol(inter)!;

            bool isPproperty = false;
            foreach (var attrs in symbol.GetAttributes())
            {
                string name = attrs.AttributeClass!.Name;
                if (name == "Property")
                {
                    isPproperty = true;
                    break;
                }
            }
            if (!isPproperty)
            {
                continue;
            }

            ClassInfo info = ClassInfo.Build(symbol);

            //InterfaceInfo info = InterfaceInfo.Build(symbol);
            //infos.Add(info);
            //info.containingNamespace = symbol.ContainingNamespace.Name;

            //SenderBuilder.Build(sourceContext, info);
            //DispatcherBuilder.Build(sourceContext, info);
        }

//        GenerateCreator(sourceContext, infos);
    }
}
