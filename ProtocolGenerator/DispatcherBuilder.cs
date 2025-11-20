
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace ProtocolGenerator;

class DispatcherBuilder
{
    StringBuilder builder = new StringBuilder();
    ClassInfo classInfo;

    DispatcherBuilder(ClassInfo classInfo)
    {
        this.classInfo = classInfo;
    }

    void AppendLine(string text, Indent indent)
    {
        builder.Append(indent.value);
        builder.AppendLine(text);
    }

    public static void Build(SourceProductionContext context, ClassInfo info)
    {
        DispatcherBuilder builder = new DispatcherBuilder(info);

        SourceText source = builder.GenerateSource();

        string fileName = $"{info.containningNamespace}.{info.dispatcherName}.g.cs";
        context.AddSource(fileName, source);
    }

    SourceText GenerateSource()
    {
        AddDispatcher();

        return SourceText.From(builder.ToString(), Encoding.Unicode);
    }

    void AddDispatcher()
    {
        Indent indent = new Indent();

        builder.AppendLine("using Common;");
        builder.AppendLine($"namespace {classInfo.containningNamespace};");
        builder.AppendLine();

        AppendLine($"public class {classInfo.dispatcherName} : Common.IDispatcher", indent);
        AppendLine("{", indent);

        AddDispatchMethod(indent.Next());

        AppendLine("}", indent);
    }

    void AddDispatchMethod(Indent indent)
    {
        Indent indent1 = indent.Next();
        AppendLine("public void Dispatch(IDataStreamReader reader, object ins)", indent);
        AppendLine("{", indent);

        AppendLine($"int index = {classInfo.classInfoName}.UnpackMethodIndex(reader);", indent1);
        AppendLine($"var methodInfo = {classInfo.classInfoName}.classInfo.GetMethodInfo(index);", indent1);
        AppendLine("methodInfo.caller(reader, ins);", indent1);

        AppendLine("}", indent);
    }
}