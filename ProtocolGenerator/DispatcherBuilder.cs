
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace ProtocolGenerator;

class DispatcherBuilder
{
//    InterfaceInfo info;
    StringBuilder builder = new StringBuilder();
    ClassInfo classInfo;

    //static Dictionary<string, Action<StringBuilder, string, ParameterInfo>> unpackers = 
    //    new Dictionary<string, Action<StringBuilder, string, ParameterInfo>>();

    //DispatcherBuilder(InterfaceInfo info)
    //{
    //    this.info = info;
    //}

    DispatcherBuilder(ClassInfo classInfo)
    {
        this.classInfo = classInfo;
    }

    void AppendLine(string text, Indent indent)
    {
        builder.Append(indent.value);
        builder.AppendLine(text);
    }

    //public static void Build(SourceProductionContext context, InterfaceInfo info)
    //{
    //    DispatcherBuilder builder = new DispatcherBuilder(info);

    //    SourceText source = builder.GenerateSource();
    //    string fileName = $"{info.dispatcherName}.g.cs";
    //    context.AddSource(fileName, source);
    //}

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
        //public class IPostOffice_Dispatcher : IDispatcher
        //{
        //    public void Dispatch(IDataStreamReader reader, object ins)
        //    {
        //        int index = IPostOffice_ClassInfo.UnpackMethodIndex(reader);
        //        var methodInfo = IPostOffice_ClassInfo.classInfo.GetMethodInfo(index);

        //        methodInfo.caller(reader, ins);
        //    }
        //}
        Indent indent = new Indent();

        //        builder.AppendLine("using Common;");
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
        // public void Dispatch(IDataStreamReader reader, object ins)
        //    {
        //        int index = IPostOffice_ClassInfo.UnpackMethodIndex(reader);
        //        var methodInfo = IPostOffice_ClassInfo.classInfo.GetMethodInfo(index);

        //        methodInfo.caller(reader, ins);
        //    }
    }

    //SourceText GenerateSource()
    //{
    //    AddUsing();
    //    AddNamespace();

    //    AddClassBegin();

    //    AddDispatcher();

    //    AddClassEnd();

    //    return SourceText.From(builder.ToString(), Encoding.Unicode);
    //}

    //void AddUsing()
    //{
    //    builder.AppendLine("using System.Text;");
    //    builder.AppendLine("using Common;");
    //    builder.AppendLine();
    //}

    //void AddNamespace()
    //{
    //    builder.AppendLine($"namespace {info.containingNamespace}.Dispatcher;");
    //    builder.AppendLine();
    //}

    //void AddClassBegin()
    //{
    //    string className = info.dispatcherName;
    //    builder.AppendLine($"public class {info.dispatcherName} : IDispatcher<{info.name}>");
    //    builder.AppendLine("{");
    //}

    //void AddClassEnd()
    //{
    //    builder.AppendLine("}");
    //}

    //void AddDispatcher()
    //{
    //    Indent indent = new Indent(1);

    //    AddDispatcherBegin(indent);

    //    foreach (var method in info.methods)
    //    {
    //        AddMethodCase(method, indent.Next());
    //    }

    //    AddDefaultCase(indent.Next());

    //    AddDispatcherEnd(indent);
    //}

    //void AddMethodCase(MethodInfo method, Indent indent)
    //{
    //    Indent nextIndent = indent.Next();
    //    AddCaseBegin(method, indent);

    //    foreach (var parameter in method.parameters)
    //    {
    //        AddParameterUnpacker(parameter, nextIndent);
    //    }

    //    CallInstanceMethod(method, nextIndent);

    //    AddCaseEnd(method, indent);
    //}

    //void AddParameterUnpacker(ParameterInfo parameter, Indent indent)
    //{
    //    // {typeName} name = Common.Packer.UnpackInt(reader);


    //    string type = parameter.typeName;
    //    string name = parameter.name;

    //    string unpacker = PackerInfo.GetUnpackerName(type);
    //    string result = $"{type} {name} = {unpacker}(reader);";
    //    AppendLine(result, indent);
    //}

    //void AddUnpacker(ParameterInfo parameter, Indent indent)
    //{
    //    if (unpackers.Count == 0)
    //    {
    //        PrepareUnpackers();
    //    }
    //    if (unpackers.TryGetValue(parameter.typeName, out Action<StringBuilder, string, ParameterInfo> action))
    //    {
    //        action(builder, indent.value, parameter);
    //    }
    //    else
    //    {
    //        AppendLine($"\"unhandle type: {parameter.typeName}\"", indent);
    //    }
    //}

    //void CallInstanceMethod(MethodInfo method, Indent indent)
    //{
    //    bool needComma = false;
    //    StringBuilder argumentBuilder = new StringBuilder();
    //    foreach (var parameter in method.parameters)
    //    {
    //        if (needComma)
    //        {
    //            argumentBuilder.Append(", ");
    //        }
    //        needComma = true;
    //        argumentBuilder.Append(parameter.name);
    //    }
    //    // build name list
    //    string argumentNames = argumentBuilder.ToString();

    //    // call the instance method
    //    AppendLine($"instance.{method.name}({argumentNames});", indent);
    //}

    //void AddCaseBegin(MethodInfo method, Indent indent)
    //{
    //    AppendLine($"case {method.index}:", indent);
    //    AppendLine("{", indent);
    //}

    //void AddCaseEnd(MethodInfo method, Indent indent)
    //{
    //    AppendLine("break;", indent.Next());
    //    AppendLine("}", indent);
    //    builder.AppendLine();
    //}

    //void AddDefaultCase(Indent indent)
    //{
    //    Indent nextIndent = indent.Next();
    //    AppendLine("default:", indent);
    //    AppendLine("{", indent);
    //    AppendLine("Console.WriteLine($\"invalid rpcId: {rpcId}\");", nextIndent);
    //    AppendLine("break;", nextIndent);
    //    AppendLine("}", indent);
    //}

    //void AddDispatcherBegin(Indent indent)
    //{
    //    Indent nextIndent = indent.Next();
    //    string interfaceType = info.name;
    //    AppendLine($"public void Dispatch({info.name} instance, BinaryReader reader)", indent);
    //    AppendLine("{", indent);
    //    AppendLine("int rpcId = reader.ReadInt32();", nextIndent);
    //    AppendLine("switch (rpcId)", nextIndent);
    //    AppendLine("{", nextIndent);
    //}

    //void AddDispatcherEnd(Indent indent)
    //{
    //    AppendLine("}", indent.Next()); // end of switch
    //    AppendLine("}", indent); // end of Dispatcher
    //}

    // unpack code goes here
    //static void PrepareUnpackers()
    //{
    //    unpackers["bool"] = UnpackBool;
    //    unpackers["string"] = UnpackString;
    //    unpackers["Common.ActorId"] = UnpackActorId;
    //    unpackers["System.IO.MemoryStream"] = UnpackMemoryStream;
    //}

    //static void UnpackMemoryStream(StringBuilder builder, string prefix, ParameterInfo info)
    //{
    //    builder.AppendLine(prefix + "long size = reader.BaseStream.Length - reader.BaseStream.Position;");
    //    builder.AppendLine(prefix + "return new MemoryStream(reader.ReadBytes((int)size));");
    //}

    //static void UnpackString(StringBuilder builder, string prefix, ParameterInfo info)
    //{
    //    builder.AppendLine(prefix + "int len = reader.ReadInt32();");
    //    builder.AppendLine(prefix + "byte[] data = reader.ReadBytes(len);");
    //    builder.AppendLine(prefix + "return Encoding.Unicode.GetString(data);");
    //}

    //static void UnpackBool(StringBuilder builder, string prefix, ParameterInfo info)
    //{
    //    builder.AppendLine(prefix + "return reader.ReadBoolean();");
    //}

    //static void UnpackActorId(StringBuilder builder, string prefix,ParameterInfo info)
    //{
    //    builder.AppendLine(prefix + "long value = reader.ReadInt64();");
    //    builder.AppendLine(prefix + "return new Common.ActorId(value);");
    //}
}