
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ProtocolGenerator;

class PartialBuilder
{
    ClassInfo classInfo;
    StringBuilder builder = new StringBuilder();
    PartialBuilder(ClassInfo info)
    {
        this.classInfo = info;
    }

    void AppendLine(string text, Indent indent)
    {
        builder.Append(indent.value);
        builder.AppendLine(text);
    }

    public static void Build(SourceProductionContext context, ClassInfo info)
    {
        PartialBuilder builder = new PartialBuilder(info);

        var source = builder.GenerateSource();

        string fileName = $"{info.containningNamespace}.{info.partialName}.g.cs";
        context.AddSource(fileName, source);
    }

    SourceText GenerateSource()
    {
        Indent indent = new Indent();
        Indent indent1 = indent.Next();

        builder.AppendLine("using Common;");
        builder.AppendLine($"namespace {classInfo.containningNamespace};");
        builder.AppendLine();

        AppendLine($"public partial class {classInfo.name}", indent);
        AppendLine("{", indent);

        foreach (var info in classInfo.properties.Values)
        {
            AddPropertyInfo(info, indent1);
        }

        AppendLine("}", indent);

        return SourceText.From(builder.ToString(), Encoding.Unicode);
    }

    void AddPropertyInfo(PropertyInfo info, Indent indent)
    {
        //public string name
        //{
        //    get { return this._name; }
        //    set
        //    {
        //        this._name = value;
        //        OnPropertyChanged(ChatData_ClassInfo._name_Info);
        //    }
        //}
        Indent indent1 = indent.Next();
        Indent indent2 = indent1.Next();

        AppendLine($"public {info.typeName} {info.name}", indent);
        AppendLine("{", indent);

        AppendLine($"get {{ return this.{info.originName}; }}", indent1);
        AppendLine("set", indent1);
        AppendLine("{", indent1);
        AppendLine($"this.{info.originName} = value;", indent2);
        AppendLine($"OnPropertyChanged({classInfo.classInfoName}.{info.infoName});", indent2);
        AppendLine("}", indent1);

        AppendLine("}", indent);
    }
}