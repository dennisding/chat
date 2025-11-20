

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace ProtocolGenerator;

class ClassInfoBuilder
{
    StringBuilder builder;
    ClassInfo classInfo;

    ClassInfoBuilder(ClassInfo classInfo)
    {
        builder = new StringBuilder();
        this.classInfo = classInfo;
    }

    void AppendLine(string text, Indent indent)
    {
        builder.Append(indent.value);
        builder.AppendLine(text);
    }

    public static void Build(SourceProductionContext context, ClassInfo info)
    {
        ClassInfoBuilder builder = new ClassInfoBuilder(info);

        var source = builder.GenerateSource();

        string name = $"{info.containningNamespace}.{info.classInfoName}.g.cs";
        context.AddSource(name, source);
    }

    public SourceText GenerateSource()
    {
        Indent indent = new Indent(0);
        Indent indent1 = indent.Next();

        builder.AppendLine("using Common;");
        builder.AppendLine($"namespace {classInfo.containningNamespace};");
        builder.AppendLine();

        AddClassBegin(indent);

        AddCreateClassInfo(indent1);
        AddIndexMethod(indent1);
        AddMethodInfos(indent1);
        AddPropertyInfos(indent1);

        AddClassEnd();

        return SourceText.From(builder.ToString(), Encoding.Unicode);
    }

    void AddCreateClassInfo(Indent indent)
    {
        Indent indent1 = indent.Next();
        AppendLine("static ClassInfo CreateClassInfo()", indent);
        AppendLine("{", indent);
        AppendLine($"ClassInfo info = new ClassInfo(\"{classInfo.name}\");", indent1);
        builder.AppendLine();
        
        AddMethodInfoForClassInfo(indent1);
        AddPropertyInfoForClassInfo(indent1);

        builder.AppendLine();
        AppendLine("info.Build();", indent1);
        AppendLine("return info;", indent1);
        AppendLine("}", indent);
    }

    void AddIndexMethod(Indent indent)
    {
        // PackMethodIndex
        builder.AppendLine();
        AppendLine("public static void PackMethodIndex(IDataStreamWriter datas, MethodInfomation info)", indent);
        AppendLine("{", indent);
        AppendLine("Common.Packer.Pack(datas, info.indexProperty, info.index);", indent.Next());
        AppendLine("}", indent);

        // UnpackMethodIndex
        builder.AppendLine();
        AppendLine("public static int UnpackMethodIndex(IDataStreamReader reader)", indent);
        AppendLine("{", indent);
        AppendLine("return reader.ReadInt(ClassInfo.methodIndexInfo);", indent.Next());
        AppendLine("}", indent);

        // PackPropertyIndex
        builder.AppendLine();
        AppendLine("public static void PackPropertyIndex(IDataStreamWriter writer, PropertyInfomation info)", indent);
        AppendLine("{", indent);
        AppendLine("Common.Packer.Pack(writer, ClassInfo.methodIndexInfo, info.index);", indent.Next());
        AppendLine("}", indent);

        // UnpackPropertyIndex
        builder.AppendLine();
        AppendLine("public static int UnpackPropertyIndex(IDataStreamReader reader)", indent);
        AppendLine("{", indent);
        AppendLine("return reader.ReadInt(ClassInfo.methodIndexInfo);", indent.Next());
        AppendLine("}", indent);
    }

    void AddMethodInfos(Indent indent)
    {
        foreach (var info in classInfo.methods.Values)
        {
            var name = info.name;
            builder.AppendLine();
            AppendLine($"public static MethodInfomation {info.infoName} = classInfo.GetMethodInfo(\"{name}\");", indent);
            AddMethodPacker(info, indent);
            AddMethodUnpacker(info, indent);
        }
    }

    void AddPropertyInfos(Indent indent)
    {
        foreach (var info in classInfo.properties.Values)
        {
            var name = info.name;
            var infoName = info.infoName;

            builder.AppendLine();
            AppendLine($"public static PropertyInfomation {infoName} = classInfo.GetPropertyInfo(\"{name}\");", indent);

            AddPropertyPacker(info, indent);
            AddPropertyUnpacker(info, indent);
        }
    }

    void AddPropertyPacker(PropertyInfo info, Indent indent)
    {
        Indent indent1 = indent.Next();

        AppendLine($"public static void {info.packerName}(IDataStreamWriter datas, object ins)", indent);
        AppendLine("{", indent);

        AppendLine($"{classInfo.name} self = ({classInfo.name})ins;", indent1);
        AppendLine($"Common.Packer.Pack(datas, {info.infoName}, self.{info.name});", indent1);

        AppendLine("}", indent);
    }

    void AddPropertyUnpacker(PropertyInfo info, Indent indent)
    {
        Indent indent1 = indent.Next();
        string unpacker = PackerInfo.GetUnpackerName(info.typeName);

        builder.AppendLine();
        AppendLine($"public static void {info.unpackerName}(IDataStreamReader reader, object ins)", indent);
        AppendLine("{", indent);

        AppendLine($"{classInfo.name} self = ({classInfo.name})ins;", indent1);
        AppendLine($"self.{info.name} = Common.Packer.{unpacker}(reader, {info.infoName});", indent1);

        AppendLine("}", indent);
    }

    void AddMethodPacker(MethodInfo info, Indent indent)
    {
        Indent indent1 = indent.Next();
        string parameters = info.parameterTypeNameList;
        if (parameters != "")
        {
            parameters = ", " + parameters;
        }

        AppendLine($"public static void {info.packerName}(IDataStreamWriter _datas{parameters})", indent);
        AppendLine("{", indent);

        for (int index = 0; index < info.parameters.Count; ++index)
        {
            string name = info.parameters[index].name;
            AppendLine($"Common.Packer.Pack(_datas, ClassInfo.builtinInfos[{index}], {name});", indent1);
        }

        AppendLine("}", indent);
    }

    void AddMethodUnpacker(MethodInfo info, Indent indent)
    {
        Indent indent1 = indent.Next();
        AppendLine($"public static void {info.unpackerName}(IDataStreamReader _reader, object _ins)", indent);
        AppendLine("{", indent);
        AppendLine($"{classInfo.name} _self = ({classInfo.name})_ins;", indent1);

        for (int index = 0; index < info.parameters.Count; ++index)
        {
            var pinfo = info.parameters[index];
            string decl = $"{pinfo.typeName} {pinfo.name}";
            string unpacker = PackerInfo.GetUnpackerName(pinfo.typeName);
            AppendLine($"{decl} = Common.Packer.{unpacker}(_reader, ClassInfo.builtinInfos[{index}]);", indent1);
        }

        builder.AppendLine();
        AppendLine($"_self.{info.name}({info.parameterNameList});", indent1);
        AppendLine("}", indent);
    }

    void AddMethodInfoForClassInfo(Indent indent)
    {
        foreach (var info in classInfo.methods.Values)
        {
            AppendLine($"info.AddMethodInfo(new MethodInfomation({info.index}, \"{info.name}\", _Unpack_{info.name}));", indent);
        }
    }

    void AddPropertyInfoForClassInfo(Indent indent)
    {
        foreach (var info in classInfo.properties.Values)
        {
            AppendLine($"info.AddPropertyInfo(new PropertyInfomation({info.index}, \"{info.name}\", {info.flag}));", indent);
        }
    }

    void AddClassBegin(Indent indent)
    {
        Indent indent1 = indent.Next();
        AppendLine($"public class {classInfo.name}_ClassInfo", indent);
        AppendLine("{", indent);
        AppendLine("public static ClassInfo classInfo = CreateClassInfo();", indent1);
    }

    void AddClassEnd()
    {
        builder.AppendLine("}");
        builder.AppendLine();
    }
}