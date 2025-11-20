

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ProtocolGenerator;

// 所有的protocol只能放在第一层namespace.
class CreatorBuilder
{

    public string containningNamespace = "";

    public StringBuilder builder = new StringBuilder();
    List<ClassInfo> classInfos;

    CreatorBuilder(List<ClassInfo> classInfos)
    {
        this.containningNamespace = classInfos[0].containningNamespace;
        this.classInfos = classInfos;
    }

    void AppendLine(string text, Indent indent)
    {
        builder.Append(indent.value);
        builder.AppendLine(text);
    }

    static bool NeedBuild(List<ClassInfo> infos)
    {
        foreach (var info in infos)
        {
            if (info.isInterface)
            {
                return true;
            }
        }

        return false;
    }

    public static void Build(SourceProductionContext context, List<ClassInfo> infos)
    {
        if (!NeedBuild(infos))
        {
            return;
        }

        var builder = new CreatorBuilder(infos);

        SourceText source = builder.GenerateSource();
        string fileName = $"{builder.containningNamespace}.ProtocolCreator.g.cs";

        context.AddSource(fileName, source);
    }

    SourceText GenerateSource()
    {
        Indent indent = new Indent();
        Indent indent1 = indent.Next();

        builder.AppendLine("using Common;");
        builder.AppendLine($"namespace {containningNamespace};");

        AppendLine("public class ProtocolCreator", indent);
        AppendLine("{", indent);

        AddCreatePacker(indent1);
        AddCreateDispatcher(indent1);

        AppendLine("}", indent);

        return SourceText.From(builder.ToString(), Encoding.Unicode);
    }

    void AddCreatePacker(Indent indent)
    {
        Indent indent1 = indent.Next();

        AppendLine("public static T CreatePacker<T>(ISender sender, PropertyFlag flag = PropertyFlag.All)", indent);
        AppendLine("{", indent);

        bool needElse = false;
        foreach (var info in classInfos)
        {
            if (!info.isInterface)
            {
                continue;
            }

            string construct = $"(T)(object)new {info.packerName}(sender, flag)";
            AddCondition(info, needElse, construct, indent1);
            needElse = true;
        }

        AppendLine("throw new NotImplementedException();", indent1);
        AppendLine("}", indent);
    }

    void AddCondition(ClassInfo info, bool needElse, string construct, Indent indent)
    {
        string ifStatement = $"if (typeof(T) == typeof({info.name}))";
        if (needElse)
        {
            ifStatement = "else " + ifStatement;
        }

        AppendLine(ifStatement, indent);
        AppendLine("{", indent);
        AppendLine($"return {construct};", indent.Next());
        AppendLine("}", indent);
    }

    void AddCreateDispatcher(Indent indent)
    {
        Indent indent1 = indent.Next();

        AppendLine("public static IDispatcher CreateDispatcher<T>()", indent);
        AppendLine("{", indent);

        bool needElse = false;
        foreach (var info in classInfos)
        {
            if (!info.isInterface)
            {
                continue;
            }
            string construct = $"new {info.dispatcherName}()";
            AddCondition(info, needElse, construct, indent1);
            needElse = true;
        }

        AppendLine("throw new NotImplementedException();", indent1);
        AppendLine("}", indent);
    }
}
