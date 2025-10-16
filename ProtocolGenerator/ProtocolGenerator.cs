
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace ProtocolGenerator;

[Generator(LanguageNames.CSharp)]
public class ProtocolGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(IsInterfaceDeclaration, TransformInterface);

        var allClassProvider = provider.Collect();

//        context.RegisterSourceOutput(provider, GenerateCode);
        context.RegisterSourceOutput(allClassProvider, GenerateAll);
    }

    bool IsInterfaceDeclaration(SyntaxNode node, CancellationToken token)
    {
        if (!node.IsKind(SyntaxKind.InterfaceDeclaration))
        {
            return false;
        }

        return true;
    }

    GeneratorSyntaxContext TransformInterface(GeneratorSyntaxContext context, CancellationToken _)
    {
        return context;
    }

    //void GenerateCode(SourceProductionContext sourceContext, GeneratorSyntaxContext syntaxContext)
    //{
    //    InterfaceDeclarationSyntax inter = (InterfaceDeclarationSyntax)syntaxContext.Node;

    //    var interfaceSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(inter);

    //    InterfaceInfo info = InterfaceInfo.Build(interfaceSymbol!);
    //    // set the module
    //    info.containingNamespace = interfaceSymbol!.ContainingNamespace.Name;

    //    SenderBuilder.Build(sourceContext, info);
    //    DispatcherBuilder.Build(sourceContext, info);
    //}

    void GenerateAll(SourceProductionContext sourceContext, ImmutableArray<GeneratorSyntaxContext> syntaxContexts)
    {
        List<InterfaceInfo> infos = new List<InterfaceInfo>();

        foreach (var syntax in syntaxContexts)
        {
            InterfaceDeclarationSyntax inter = (InterfaceDeclarationSyntax)syntax.Node;
            var symbol = syntax.SemanticModel.GetDeclaredSymbol(inter)!;

            bool isProtocol = false;
            foreach (var attrs in symbol.GetAttributes())
            {
                string name = attrs.AttributeClass!.Name;
                if (name == "Protocol")
                {
                    isProtocol = true;
                    break;
                }
            }
            if (!isProtocol)
            {
                continue;
            }

            InterfaceInfo info = InterfaceInfo.Build(symbol);
            infos.Add(info);
            info.containingNamespace = symbol.ContainingNamespace.Name;

//            GenerateCode(sourceContext, info);

            SenderBuilder.Build(sourceContext, info);
            DispatcherBuilder.Build(sourceContext, info);
        }

        GenerateCreator(sourceContext, infos);

        //CreatorBuilder.BuildSenderCreator(sourceContext, infos);
        //CreatorBuilder.BuildDispathcerCreator(sourceContext, infos);
//        GenerateCreator(infos);
    }

    void GenerateCreator(SourceProductionContext context, List<InterfaceInfo> infos)
    {
        // key = ContainingNamespace, value = List<...
        Dictionary<string, List<InterfaceInfo>> infoDict = new Dictionary<string, List<InterfaceInfo>>();

        foreach (var info in infos)
        {
            if (!infoDict.ContainsKey(info.containingNamespace))
            {
                infoDict[info.containingNamespace] = new List<InterfaceInfo>();
            }

            var infoList = infoDict[info.containingNamespace];
            infoList.Add(info);
        }

        foreach (var item in infoDict)
        {
            CreatorBuilder.BuildSenderCreator(item.Key, context, item.Value);
            CreatorBuilder.BuildDispathcerCreator(item.Key, context, item.Value);
        }
    }

    //void GenerateCode(SourceProductionContext context, InterfaceInfo info)
    //{
    //    SenderBuilder.Build(context, info);
    //    DispatcherBuilder.Build(context, info);
    //}

    //void GenerateCreator(List<InterfaceInfo> infos)
    //{
    //    CreatorBuilder.Build(infos);
    //}
}