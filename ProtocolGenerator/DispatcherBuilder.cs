
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ProtocolGenerator;

public class DispatcherBuilder
{
    InterfaceInfo info;
    StringBuilder builder = new StringBuilder();

    static Dictionary<string, Action<StringBuilder, string, ParameterInfo>> unpackers = 
        new Dictionary<string, Action<StringBuilder, string, ParameterInfo>>();

    DispatcherBuilder(InterfaceInfo info)
    {
        this.info = info;
    }

    public static void Build(SourceProductionContext context, InterfaceInfo info)
    {
        DispatcherBuilder builder = new DispatcherBuilder(info);

        SourceText source = builder.GenerateSource();
        string fileName = $"{info.name}_Dispatcher.g.cs";
        context.AddSource(fileName, source);
    }

    SourceText GenerateSource()
    {
        AddUsing();
        AddNamespace();

        AddClassBegin();

        AddDispatcher();

        AddClassEnd();

        return SourceText.From(builder.ToString(), Encoding.Unicode);
    }

    void AddUsing()
    {
        builder.AppendLine("using System.Text;");
        builder.AppendLine();
    }

    void AddNamespace()
    {
        builder.AppendLine($"namespace {info.containingNamespace}.Dispatcher;");
        builder.AppendLine();
    }

    void AddClassBegin()
    {
        builder.AppendLine($"public class {info.name}_Dispatcher");
        builder.AppendLine("{");
    }

    void AddClassEnd()
    {
        builder.AppendLine("}");
    }

    void AddDispatcher()
    {
        Indent indent = new Indent(1);

        AddDispatcherBegin(indent);

        foreach (var method in info.methods)
        {
            AddMethodCase(method, indent.Next());
        }

        AddDefaultCase(indent.Next());

        AddDispatcherEnd(indent);
    }

    void AddMethodCase(MethodInfo method, Indent indent)
    {
        //case 2:
        //{
        //    string msg = ((Func<string>)(() => {
        //        int len = reader.ReadInt32();
        //        byte[] data = reader.ReadBytes(len);

        //        return Encoding.Unicode.GetString(data);
        //    }))();
        //    ins.Echo(msg);
        //    break;
        //}
        Indent nextIndent = indent.Next();
        AddCaseBegin(method, indent);

        foreach (var parameter in method.parameters)
        {
            AddParameterUnpacker(parameter, nextIndent);
        }

        CallInstanceMethod(method, nextIndent);

        AddCaseEnd(method, indent);
    }

    void AddParameterUnpacker(ParameterInfo parameter, Indent indent)
    {
        string type = parameter.typeName;
        string name = parameter.name;
        AppendLine($"{type} {name} = ((Func<{type}>)(() => {{", indent);

        AddUnpacker(parameter, indent.Next());

        AppendLine("}))();", indent);
        
        builder.AppendLine();
    }

    void AddUnpacker(ParameterInfo parameter, Indent indent)
    {
        if (unpackers.Count == 0)
        {
            PrepareUnpackers();
        }
        if (unpackers.TryGetValue(parameter.typeName, out Action<StringBuilder, string, ParameterInfo> action))
        {
            action(builder, indent.value, parameter);
        }
        else
        {
            AppendLine($"\"unhandle type: {parameter.typeName}\"", indent);
        }
    }

    void CallInstanceMethod(MethodInfo method, Indent indent)
    {
        bool needComma = false;
        StringBuilder argumentBuilder = new StringBuilder();
        foreach (var parameter in method.parameters)
        {
            if (needComma)
            {
                argumentBuilder.Append(", ");
            }
            needComma = true;
            argumentBuilder.Append(parameter.name);
        }
        // build name list
        string argumentNames = argumentBuilder.ToString();

        // call the instance method
        AppendLine($"instance.{method.name}({argumentNames});", indent);
    }

    void AddCaseBegin(MethodInfo method, Indent indent)
    {
        AppendLine($"case {method.rpcId}:", indent);
        AppendLine("{", indent);
    }

    void AddCaseEnd(MethodInfo method, Indent indent)
    {
        AppendLine("break;", indent.Next());
        AppendLine("}", indent);
        builder.AppendLine();
    }

    void AddDefaultCase(Indent indent)
    {
        //default:
        //        {
        //    Console.WriteLine($"invalid rpcId: {rpcId}");
        //    break;
        //}
        Indent nextIndent = indent.Next();
        AppendLine("default:", indent);
        AppendLine("{", indent);
        AppendLine("Console.WriteLine($\"invalid rpcId: {rpcId}\");", nextIndent);
        AppendLine("break;", nextIndent);
        AppendLine("}", indent);
    }

    void AddDispatcherBegin(Indent indent)
    {
        //        public static void Dispatch(Protocol.ILoginClient ins, BinaryReader reader)
        //{
        //    int rpcId = reader.ReadInt32();
        //    switch (rpcId)

        Indent nextIndent = indent.Next();
        string interfaceType = info.name;
        AppendLine($"public static void Dispatch({info.name} instance, BinaryReader reader)", indent);
        AppendLine("{", indent);
        AppendLine("int rpcId = reader.ReadInt32();", nextIndent);
        AppendLine("switch (rpcId)", nextIndent);
        AppendLine("{", nextIndent);
    }

    void AddDispatcherEnd(Indent indent)
    {
        AppendLine("}", indent.Next()); // end of switch
        AppendLine("}", indent); // end of Dispatcher
    }

    void AppendLine(string text, Indent indent)
    {
        builder.Append(indent.value);
        builder.AppendLine(text);
    }

    // unpack code goes here
    static void PrepareUnpackers()
    {
        unpackers["bool"] = UnpackBool;
        unpackers["string"] = UnpackString;
        unpackers["Common.ActorId"] = UnpackActorId;
        unpackers["System.IO.MemoryStream"] = UnpackMemoryStream;
    }

    static void UnpackMemoryStream(StringBuilder builder, string prefix, ParameterInfo info)
    {
        //long size = reader.BaseStream.Length - reader.BaseStream.Position;
        //MemoryStream stream = new MemoryStream(reader.ReadBytes((int)size));
        //return stream;
        builder.AppendLine(prefix + "long size = reader.BaseStream.Length - reader.BaseStream.Position;");
        builder.AppendLine(prefix + "return new MemoryStream(reader.ReadBytes((int)size));");
    }

    static void UnpackString(StringBuilder builder, string prefix, ParameterInfo info)
    {
        builder.AppendLine(prefix + "int len = reader.ReadInt32();");
        builder.AppendLine(prefix + "byte[] data = reader.ReadBytes(len);");
        builder.AppendLine(prefix + "return Encoding.Unicode.GetString(data);");
        //int len = reader.ReadInt32();
        //byte[] data = reader.ReadBytes(len);

        //return Encoding.Unicode.GetString(data);
    }

    static void UnpackBool(StringBuilder builder, string prefix, ParameterInfo info)
    {
        builder.AppendLine(prefix + "return reader.ReadBoolean();");
    }

    static void UnpackActorId(StringBuilder builder, string prefix,ParameterInfo info)
    {
        //long value = reader.ReadInt64();
        //return new ActorId(value);
        builder.AppendLine(prefix + "long value = reader.ReadInt64();");
        builder.AppendLine(prefix + "return new Common.ActorId(value);");
    }
}