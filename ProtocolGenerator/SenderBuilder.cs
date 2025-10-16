
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using System.Text;

namespace ProtocolGenerator;

public class SenderBuilder
{
    InterfaceInfo info;
    StringBuilder builder;
    static Dictionary<string, Action<StringBuilder, string, string> > packers 
        = new Dictionary<string, Action<StringBuilder, string, string>>();

    public static void Build(SourceProductionContext context, InterfaceInfo info)
    {
        SenderBuilder builder = new SenderBuilder(info);

        SourceText source = builder.GenerateSource();
        string fileName = $"{info.senderName}.g.cs";
        context.AddSource(fileName, source);
    }

    public SenderBuilder(InterfaceInfo info)
    {
        this.info = info;
        builder = new StringBuilder();
    }

    SourceText GenerateSource()
    {
        AddUsing();
        AddNamespace();

        Indent ident = new Indent();
        AddClassBegin(ident);

        foreach (var method in info.methods)
        {
            AddMethod(method, ident.Next());
        }

        AddClassEnd(ident);

        return SourceText.From(builder.ToString(), Encoding.Unicode);
    }

    void AddUsing()
    {
        builder.AppendLine("using Common;");
        builder.AppendLine("using System.Runtime.InteropServices;");

        builder.AppendLine();
    }

    void AddNamespace()
    {
        builder.AppendLine($"namespace {info.containingNamespace}.Sender;");
        builder.AppendLine();
    }

    void AddClassBegin(Indent indent)
    {
        Indent indent1 = indent.Next();

        string name = info.name;
        string className = info.senderName;

        AppendLine($"public class {className} : {name}", indent);
        AppendLine("{", indent);

        AppendLine("ISender sender;", indent1);
        AppendLine($"public {className}(ISender sender)", indent1);
        AppendLine("{", indent1);
        AppendLine($"this.sender = sender;", indent1.Next());
        AppendLine("}", indent1);
    }

    void AppendLine(string text, Indent ident)
    {
        builder.Append(ident.value);
        builder.AppendLine(text);
    }

    void AddClassEnd(Indent ident)
    {
        builder.Append(ident.value);
        builder.AppendLine("}");
    }

    void AddMethod(MethodInfo method, Indent ident)
    {
        AddMethodBegin(method, ident);

        foreach (var parameter in method.parameters)
        {
            AddParameter(parameter, ident.Next());
        }

        AddMethodEnd(method, ident);
    }

    void AddMethodBegin(MethodInfo method, Indent ident)
    {
        string name = method.name;
        int rpcId = method.rpcId;
        string argumentString = BuildParameterString(method);

        Indent nextIdent = ident.Next();

        AppendLine($"public void {name}({argumentString})", ident);
        AppendLine("{", ident);
        AppendLine("MemoryStream stream = new MemoryStream();", nextIdent);
        AppendLine($"int rpcId = {rpcId};", nextIdent);
        AppendLine("var npcIdData = BitConverter.GetBytes(rpcId);", nextIdent);
        AppendLine("stream.Write(npcIdData);", nextIdent);
    }

    string BuildParameterString(MethodInfo method)
    {
        StringBuilder builder = new StringBuilder();
        bool needComma = false;
        foreach (var parameter in method.parameters)
        {
            if (needComma)
            {
                builder.Append(", ");
            }

            builder.Append($"{parameter.typeName} {parameter.name}");
            needComma = true;
        }

        return builder.ToString();
    }

    void AddMethodEnd(MethodInfo method, Indent ident)
    {
        AppendLine("sender.Send(stream);", ident.Next());
        AppendLine("}", ident);
    }

    void AddParameter(ParameterInfo parameter, Indent ident)
    {
        builder.AppendLine();

        AppendLine("{", ident);

        string name = parameter.name;
        // add the packer
        AddPacker(parameter.typeName, ident.Next(), parameter.name);

        AppendLine("}", ident);
    }

    void AddPacker(string typeName, Indent ident, string name)
    {
        if (packers.Count == 0)
        {
            PreparePackers();
        }

        if (packers.TryGetValue(typeName, out Action<StringBuilder, string, string>? action))
        {
            action!(builder, ident.value, name);
        }
        else
        {
            builder.AppendLine($"\"Invalid Type: {typeName}\"");
        }
    }

    void PreparePackers()
    {
        packers.Add("string", PackString);
        packers.Add("bool", PackNumber);
        packers.Add("Boolean", PackNumber);
        packers.Add("Common.ActorId", PackActorId);
        packers.Add("System.IO.MemoryStream", PackMemoryStream);
    }

    static void PackMemoryStream(StringBuilder builder, string prefix, string name)
    {
        builder.AppendLine(prefix + $"stream.Write({name}.ToArray());");
    }

    static void PackString(StringBuilder builder, string prefix, string name)
    {
        builder.AppendLine(prefix + $"var strData = MemoryMarshal.AsBytes({name}.AsSpan());");
        builder.AppendLine(prefix + "byte[] data = BitConverter.GetBytes((int)strData.Length);");
        builder.AppendLine(prefix + "stream.Write(data);");
        builder.AppendLine(prefix + "stream.Write(strData);");
    }

    static void PackNumber(StringBuilder builder, string prefix, string name)
    {
        builder.AppendLine(prefix + $"byte[] data = BitConverter.GetBytes({name});");
        builder.AppendLine(prefix + "stream.Write(data);");
    }

    static void PackActorId(StringBuilder builder, string prefix, string name)
    {
        builder.AppendLine(prefix + $"byte[] rawData = BitConverter.GetBytes({name}.value);");
        builder.AppendLine(prefix + "stream.Write(rawData);");
    }
}