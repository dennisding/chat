
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

    //string BeautifyCode()
    //{
    //    string source = builder.ToString();

    //    var syntaxTree = CSharpSyntaxTree.ParseText(source);
    //    var root = syntaxTree.GetRoot();

    //    // 使用格式化器美化代码
    //    var workspace = new AdhocWorkspace();
    //    var optionSet = workspace.Options;

    //    root.NormalizeWhitespace();

    //    var formattedRoot = Formatter.Format(root, workspace);

    //    return formattedRoot.ToFullString();
    //}

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
        //public class ILoginClient : Protocol.ILoginClient
        //    {
        //        ISender sender;
        //        public ILoginClient(ISender sender)
        //        {
        //            this.sender = sender;
        //        }
        Indent memberIndent = indent.Next();
        Indent contentIdent = memberIndent.Next();

        string name = info.name;
        string className = info.senderName;

        AppendLine($"public class {className} : {name}", indent);
        AppendLine("{", indent);

        AppendLine("ISender sender;", memberIndent);
        AppendLine($"public {className}(ISender sender)", memberIndent);
        AppendLine("{", memberIndent);
        AppendLine($"this.sender = sender;", memberIndent.Next());
        AppendLine("}", memberIndent);
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

    //public void EchoBack(string msg)
    //{
    //    MemoryStream stream = new MemoryStream();
    //    int rpcId = 3;
    //    var npcIdData = BitConverter.GetBytes(rpcId);
    //    stream.Write(npcIdData);
    //    {
    //        var rawData = MemoryMarshal.AsBytes(msg.AsSpan());
    //        byte[] lenData = BitConverter.GetBytes((int)rawData.Length);
    //        stream.Write(lenData);
    //        stream.Write(rawData);
    //    }
    //    sender.Send(stream);
    //}

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
        //    public void EchoBack(string msg)
        //{
        //    MemoryStream stream = new MemoryStream();
        //    int rpcId = 3;
        //    var npcIdData = BitConverter.GetBytes(rpcId);
        //    stream.Write(npcIdData);
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
        //    sender.Send(stream);
        //}
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

        //var action = packers[typeName];
        //action!(builder, name);
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
        //byte[] rawData = BitConverter.GetBytes(aid.value);
        //stream.Write(rawData);
        builder.AppendLine(prefix + $"byte[] rawData = BitConverter.GetBytes({name}.value);");
        builder.AppendLine(prefix + "stream.Write(rawData);");
    }
}