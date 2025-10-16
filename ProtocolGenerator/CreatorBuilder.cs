

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace ProtocolGenerator;

class CreatorBuilder
{
    string containingNamespace;
    List<InterfaceInfo> infos;
    StringBuilder builder = new StringBuilder();

    CreatorBuilder(string _namespace, List<InterfaceInfo> infos)
    {
        containingNamespace = _namespace;
        this.infos = infos;
    }

    public static void BuildSenderCreator(string containingNameSpace, 
            SourceProductionContext context, List<InterfaceInfo> infos)
    {
        CreatorBuilder builder = new CreatorBuilder(containingNameSpace, infos);
        SourceText source = builder.GenerateSenderSource();

        string name = $"{containingNameSpace}.Sender_Creator.g.s";
        context.AddSource(name, source);
    }

    public static void BuildDispathcerCreator(string containingNameSpace, 
        SourceProductionContext context, List<InterfaceInfo> infos)
    {
        CreatorBuilder builder = new CreatorBuilder(containingNameSpace, infos);
        SourceText source = builder.GenerateDispatcherSource();

        string name = $"{containingNameSpace}.Dispatcher_Creator.g.s";
        context.AddSource(name, source);
    }

    public SourceText GenerateSenderSource()
    {
        GenerateSenderCreator();
        return SourceText.From(builder.ToString(), Encoding.Unicode);
    }

    public SourceText GenerateDispatcherSource()
    {
        GenerateDispatcherCreator();
        return SourceText.From(builder.ToString(), Encoding.Unicode);
    }

    void AppendLine(string text, Indent indent)
    {
        builder.Append(indent.value);
        builder.AppendLine(text);
    }

    void GenerateSenderCreator()
    {
        Indent indent1 = new Indent(1);
        Indent indent2 = indent1.Next();

        builder.AppendLine("");
        builder.AppendLine("using Common;");
        builder.AppendLine($"namespace {containingNamespace}.Sender;");
        builder.AppendLine("public class Sender");
        builder.AppendLine("{");
        AppendLine("public static T Create<T>(ISender sender)", indent1);
        AppendLine("{", indent1);

        AddCreators(indent2);

        AppendLine("throw new NotImplementedException();", indent2);
        AppendLine("}", indent1);
        builder.AppendLine("}");
    }

    void AddCreators(Indent indent)
    {
        bool needElse = false;
        foreach (InterfaceInfo info in infos)
        {
            builder.Append(indent.value);
            if (needElse)
            {
                builder.Append("else ");
            }

            builder.AppendLine($"if (typeof(T) == typeof({info.name}))");
            AppendLine("{", indent);
            AppendLine($"return (T)(object)new {info.senderName}(sender);", indent.Next());
            AppendLine("}", indent);

            needElse = true;
        }
    }

    void GenerateDispatcherCreator()
    {
        Indent indent1 = new Indent(1);
        Indent indent2 = indent1.Next();

        builder.AppendLine("using Common;");
        builder.AppendLine($"namespace {containingNamespace}.Dispatcher;");
        builder.AppendLine("public class Dispatcher");
        builder.AppendLine("{");
        AppendLine("public static IDispatcher<T> Create<T>()", indent1);
        AppendLine("{", indent1);

        AddDispatchers(indent2);

        AppendLine("throw new NotImplementedException();", indent2);
        AppendLine("}", indent1);
        builder.AppendLine("}");
    }

    void AddDispatchers(Indent indent)
    {
        bool needElse = false;
        foreach (InterfaceInfo info in infos)
        {
            builder.Append(indent.value);
            if (needElse)
            {
                builder.Append("else ");
            }

            builder.AppendLine($"if (typeof(T) == typeof({info.name}))");
            AppendLine("{", indent);
            AppendLine($"return (IDispatcher<T>)(object)new {info.dispatcherName}();", indent.Next());
            AppendLine("}", indent);

            needElse = true;
        }
    }
}