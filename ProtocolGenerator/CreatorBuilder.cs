

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
        //        CreatorBuilder builder = new CreatorBuilder(containingNameSpace, infos);
        //        SourceText source = builder.GenerateSenderSource();

        //        string name = $"{containingNameSpace}.Sender_Creator.g.s";
        //        context.AddSource(name, source);
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
        //public class ProtocolCreator
        //{
        //    public static T CreatePacker<T>(ISender sender, PropertyFlag flag = PropertyFlag.All)
        //    {
        //        if (typeof(T) == typeof(ILoginClient))
        //        {
        //            return (T)(object)new ILoginClient_Packer(sender, flag);
        //        }
        //        else if (typeof(T) == typeof(ILoginServer))
        //        {
        //            return (T)(object)new ILoginServer_Packer(sender, flag);
        //        }

        //        throw new NotImplementedException();
        //    }

        //    public static IDispatcher CreateDispatcher<T>()
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
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

//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.Text;
//using System.Dynamic;
//using System.Text;

//namespace ProtocolGenerator;

//class CreatorBuilder
//{
//    string containingNamespace;
//    List<InterfaceInfo> infos;
//    StringBuilder builder = new StringBuilder();

//    CreatorBuilder(string _namespace, List<InterfaceInfo> infos)
//    {
//        containingNamespace = _namespace;
//        this.infos = infos;
//    }

//    public static void Build(SourceProductionContext context, List<ClassInfo> infos)
//    {
//    }

//    public static void BuildSenderCreator(string containingNameSpace, 
//            SourceProductionContext context, List<InterfaceInfo> infos)
//    {
//        CreatorBuilder builder = new CreatorBuilder(containingNameSpace, infos);
//        SourceText source = builder.GenerateSenderSource();

//        string name = $"{containingNameSpace}.Sender_Creator.g.s";
//        context.AddSource(name, source);
//    }

//    public static void BuildDispathcerCreator(string containingNameSpace, 
//        SourceProductionContext context, List<InterfaceInfo> infos)
//    {
//        CreatorBuilder builder = new CreatorBuilder(containingNameSpace, infos);
//        SourceText source = builder.GenerateDispatcherSource();

//        string name = $"{containingNameSpace}.Dispatcher_Creator.g.s";
//        context.AddSource(name, source);
//    }

//    public SourceText GenerateSenderSource()
//    {
//        GenerateSenderCreator();
//        return SourceText.From(builder.ToString(), Encoding.Unicode);
//    }

//    public SourceText GenerateDispatcherSource()
//    {
//        GenerateDispatcherCreator();
//        return SourceText.From(builder.ToString(), Encoding.Unicode);
//    }

//    void AppendLine(string text, Indent indent)
//    {
//        builder.Append(indent.value);
//        builder.AppendLine(text);
//    }

//    void GenerateSenderCreator()
//    {
//        Indent indent1 = new Indent(1);
//        Indent indent2 = indent1.Next();

//        builder.AppendLine("");
//        builder.AppendLine("using Common;");
//        builder.AppendLine($"namespace {containingNamespace}.Sender;");
//        builder.AppendLine("public class Sender");
//        builder.AppendLine("{");
//        AppendLine("public static T Create<T>(ISender sender)", indent1);
//        AppendLine("{", indent1);

//        AddCreators(indent2);

//        AppendLine("throw new NotImplementedException();", indent2);
//        AppendLine("}", indent1);
//        builder.AppendLine("}");
//    }

//    void AddCreators(Indent indent)
//    {
//        bool needElse = false;
//        foreach (InterfaceInfo info in infos)
//        {
//            builder.Append(indent.value);
//            if (needElse)
//            {
//                builder.Append("else ");
//            }

//            builder.AppendLine($"if (typeof(T) == typeof({info.name}))");
//            AppendLine("{", indent);
//            AppendLine($"return (T)(object)new {info.senderName}(sender);", indent.Next());
//            AppendLine("}", indent);

//            needElse = true;
//        }
//    }

//    void GenerateDispatcherCreator()
//    {
//        Indent indent1 = new Indent(1);
//        Indent indent2 = indent1.Next();

//        builder.AppendLine("using Common;");
//        builder.AppendLine($"namespace {containingNamespace}.Dispatcher;");
//        builder.AppendLine("public class Dispatcher");
//        builder.AppendLine("{");
//        AppendLine("public static IDispatcher<T> Create<T>()", indent1);
//        AppendLine("{", indent1);

//        AddDispatchers(indent2);

//        AppendLine("throw new NotImplementedException();", indent2);
//        AppendLine("}", indent1);
//        builder.AppendLine("}");
//    }

//    void AddDispatchers(Indent indent)
//    {
//        bool needElse = false;
//        foreach (InterfaceInfo info in infos)
//        {
//            builder.Append(indent.value);
//            if (needElse)
//            {
//                builder.Append("else ");
//            }

//            builder.AppendLine($"if (typeof(T) == typeof({info.name}))");
//            AppendLine("{", indent);
//            AppendLine($"return (IDispatcher<T>)(object)new {info.dispatcherName}();", indent.Next());
//            AppendLine("}", indent);

//            needElse = true;
//        }
//    }
//}