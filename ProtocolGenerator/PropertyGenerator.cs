

//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using System.Collections.Immutable;

//namespace ProtocolGenerator;

//[Generator(LanguageNames.CSharp)]
//public class PropertyGenerator : IIncrementalGenerator
//{
//    public void Initialize(IncrementalGeneratorInitializationContext context)
//    {
//        var provider = context.SyntaxProvider.CreateSyntaxProvider(IsClassDeclaration, TransformClass);

//        var allClassProvider = provider.Collect();

//        context.RegisterSourceOutput(allClassProvider, GenerateAll);
//    }

//    bool IsClassDeclaration(SyntaxNode node, CancellationToken token)
//    {
//        return node.IsKind(SyntaxKind.ClassDeclaration);
//    }

//    GeneratorSyntaxContext TransformClass(GeneratorSyntaxContext context, CancellationToken _)
//    {
//        return context;
//    }

//    void GenerateAll(SourceProductionContext sourceContext,
//        ImmutableArray<GeneratorSyntaxContext> syntaxContexts)
//    {
//        foreach (var syntax in syntaxContexts)
//        {
//            ClassDeclarationSyntax classDecl = (ClassDeclarationSyntax)syntax.Node;
//            var symbol = syntax.SemanticModel.GetDeclaredSymbol(classDecl)!;

//            bool isProperty = false;
//            foreach (var attrs in symbol.GetAttributes())
//            {
//                string name = attrs.AttributeClass!.Name;
//                if (name == "PropertyAttribute")
//                {
//                    isProperty = true;
//                    break;
//                }
//            }
//            if (!isProperty)
//            {
//                continue;
//            }

//            ClassInfo info = ClassInfo.Build(symbol);

//            PropertyBuilder.Build(sourceContext, info);
//        }
//    }
//}
