
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;

namespace ProtocolGenerator;

// 协议只能放在第一层namespace;
// 数据属性必须以下划线打头, 会自动生成非下划线版本.
//
// 1. 协议分为数据协议和接口协议.
// 2. 数据协议和接口协议都会生成 {ClassName}_ClassInfo对象. 该对象对Type做静态分析以产生更高效地代码
// 3. 数据协议生成 partical class {ClassName} 对象.
// 4. 接口协议生成 {ClassName}_Packer 和 {ClassName}_Dispatcher对象.
// 5. 数据协议和接口协议都生成 {CalssName}_Creator 对象

[Generator(LanguageNames.CSharp)]
public class ProtocolGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // init the enviroment
        PackerInfo.InitPacker();

        var provider = context.SyntaxProvider.CreateSyntaxProvider(IsInterfaceDeclaration, TransformInterface);

        var allClassProvider = provider.Collect();

        context.RegisterSourceOutput(allClassProvider, GenerateAll);
    }

    bool IsInterfaceDeclaration(SyntaxNode node, CancellationToken token)
    {
        if (node.IsKind(SyntaxKind.InterfaceDeclaration))
        {
            return true;
        }

        if (node.IsKind(SyntaxKind.ClassDeclaration))
        {
            return true;
        }

        return false;
    }

    GeneratorSyntaxContext TransformInterface(GeneratorSyntaxContext context, CancellationToken _)
    {
        return context;
    }

    void GenerateAll(SourceProductionContext sourceContext, 
        ImmutableArray<GeneratorSyntaxContext> syntaxContexts)
    {
        List<ClassInfo> infos = new List<ClassInfo>();

        foreach (var syntax in syntaxContexts)
        {
            ClassInfo? info = null;

            if (syntax.Node is InterfaceDeclarationSyntax inter)
            {
                var symbol = syntax.SemanticModel.GetDeclaredSymbol(inter);
                if (!HasAttribute(symbol!, "Protocol"))
                {
                    continue;
                }
                info = ClassInfo.Build(symbol!, true);
                infos.Add(info);

                PackerBuilder.Build(sourceContext, info);
                DispatcherBuilder.Build(sourceContext, info);
            }
            else if (syntax.Node is ClassDeclarationSyntax _class)
            {
                var symbol = syntax.SemanticModel.GetDeclaredSymbol(_class);

                if (!HasAttribute(symbol!, "PropertyAttribute"))
                {
                    continue;
                }

                info = ClassInfo.Build(symbol!);
                infos.Add(info);

                PartialBuilder.Build(sourceContext, info);
            }

            if (info != null)
            {
                ClassInfoBuilder.Build(sourceContext, info);
            }
        }

        CreatorBuilder.Build(sourceContext, infos);
//        CreatorBuilder.Build(sourceContext, infos);

        //List<InterfaceInfo> infos = new List<InterfaceInfo>();

        //foreach (var syntax in syntaxContexts)
        //{
        //    InterfaceDeclarationSyntax inter = (InterfaceDeclarationSyntax)syntax.Node;
        //    var symbol = syntax.SemanticModel.GetDeclaredSymbol(inter)!;

        //    bool isProtocol = false;
        //    foreach (var attrs in symbol.GetAttributes())
        //    {
        //        string name = attrs.AttributeClass!.Name;
        //        if (name == "Protocol")
        //        {
        //            isProtocol = true;
        //            break;
        //        }
        //    }
        //    if (!isProtocol)
        //    {
        //        continue;
        //    }

        //    InterfaceInfo info = InterfaceInfo.Build(symbol);

        //    SenderBuilder.Build(sourceContext, info);
        //    DispatcherBuilder.Build(sourceContext, info);

        //    infos.Add(info);
        //}

        //GenerateCreator(sourceContext, infos);
    }

    bool HasAttribute(INamedTypeSymbol symbol, string attribute)
    {
        foreach (var attrs in symbol.GetAttributes())
        {
            string name = attrs.AttributeClass!.Name;
            if (attrs.AttributeClass!.Name == attribute)
            {
                return true;
            }
        }
        return false;
    }

    //void GenerateCreator(SourceProductionContext context, List<InterfaceInfo> infos)
    //{
    //    // key = ContainingNamespace, value = List<...
    //    Dictionary<string, List<InterfaceInfo>> infoDict = new Dictionary<string, List<InterfaceInfo>>();

    //    foreach (var info in infos)
    //    {
    //        if (!infoDict.ContainsKey(info.containingNamespace))
    //        {
    //            infoDict[info.containingNamespace] = new List<InterfaceInfo>();
    //        }

    //        var infoList = infoDict[info.containingNamespace];
    //        infoList.Add(info);
    //    }

    //    foreach (var item in infoDict)
    //    {
    //        CreatorBuilder.BuildSenderCreator(item.Key, context, item.Value);
    //        CreatorBuilder.BuildDispathcerCreator(item.Key, context, item.Value);
    //    }
    //}
}