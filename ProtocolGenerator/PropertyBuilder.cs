
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace ProtocolGenerator;

class PropertyBuilder
{
    StringBuilder builder = new StringBuilder();

    PropertyBuilder(SourceProductionContext context, ClassInfo infos)
    {
        Indent indent = new Indent(1);

        AddUsing();

        builder.AppendLine();
        builder.AppendLine($"namespace {infos.containningNamespace};");
        builder.AppendLine();

        builder.AppendLine($"public partial class {infos.name}");
        builder.AppendLine("{");
        AppendLine("public static ClassInfo classInfo = CreateClassInfo();", indent);

        AddCreateClassInfo(infos, indent);
        AddGetClassInfo(infos, indent);
        AddProperties(infos, indent);

        builder.AppendLine("}");
    }

    void AddGetClassInfo(ClassInfo infos, Indent indent)
    {
        builder.AppendLine();
        AppendLine($"public override ClassInfo GetClassInfo()", indent);
        AppendLine("{", indent);
        AppendLine("return classInfo;", indent.Next());
        AppendLine("}", indent);
        //    public override ClassInfo GetClassInfo()
        //    {
        //        return classInfo;
        //    }
    }

    void AddUsing()
    {
        builder.AppendLine("using Common;");
    }

    void AddCreateClassInfo(ClassInfo classInfo, Indent indent)
    {
        Indent nextIndent = indent.Next();

        AppendLine("public static ClassInfo CreateClassInfo()", indent);
        AppendLine("{", indent);
        AppendLine($"ClassInfo info = new ClassInfo(\"{classInfo.name}\");", nextIndent);

        builder.AppendLine();
        foreach (var info in classInfo.EnumeratePropertyInfo())
        {
            AddPropertyInfo(info, nextIndent);
        }

        builder.AppendLine();
        AppendLine("info.Build();", nextIndent);
        AppendLine("return info;", nextIndent);
        AppendLine("}", indent);
    }

    void AddPropertyInfo(PropertyInfo info, Indent indent)
    {
        //info.AddPropertyInfo(new Common.PropertyInfo(10, PropertyFlag.Client, "hp", _Pack_hp, _Unpack_hp));
        string argument = $"{info.index}, {info.flag}, \"{info.name}\", {info.packerName}, {info.unpackerName}";
        string result = $"info.AddPropertyInfo(new Common.PropertyInfo({argument}));";
        AppendLine(result, indent);
    }

    void AddProperties(ClassInfo infos, Indent indent)
    {
        string typeName = infos.name;
        foreach(var info in infos.EnumeratePropertyInfo())
        {
            AddProperty(typeName, info, indent);
        }
    }

    void AddPacker(string typeName, PropertyInfo info, Indent indent)
    {
        //    public static void _Pack_name(object obj, MemoryStream stream)
        //    {
        //        ChatData self = (ChatData)obj;
        //        //int index = 11;
        //        //Common.Packer.PackInt(stream, index);
        //        Common.Packer.PackString(stream, self._name);
        //    }
        Indent indent1 = indent.Next();
        string packerName = PackerInfo.GetPackerName(info.typeName);

        AppendLine($"public static void _Pack_{info.name}(object obj, MemoryStream stream)", indent);
        AppendLine("{", indent);
        AppendLine($"{typeName} self = ({typeName})obj;", indent1);
        AppendLine($"{packerName}(stream, self._{info.name});", indent1);
        AppendLine("}", indent);

    }

    void AddUnpacker(string typeName, PropertyInfo info, Indent indent)
    {
        //    public static void _Unpack_name(object obj, BinaryReader reader)
        //    {
        //        ChatData self = (ChatData)obj;

        //        string name = Common.Packer.UnpackString(reader);
        //        self.name = name;
        //    }

        Indent indent1 = indent.Next();
        string unpackerName = PackerInfo.GetUnpackerName(info.typeName);

        AppendLine($"public static void _Unpack_{info.name}(object obj, BinaryReader reader)", indent);
        AppendLine("{", indent);
        AppendLine($"{typeName} self = ({typeName})obj;", indent1);
        AppendLine($"self.{info.name} = {unpackerName}(reader);", indent1);
        AppendLine("}", indent);
    }

    void AddProperty(string typeName, PropertyInfo info, Indent indent)
    {
        Indent indent1 = indent.Next();
        Indent indent2 = indent1.Next();

        builder.AppendLine();
        AddPacker(typeName, info, indent);
        AddUnpacker(typeName, info, indent);

        string infoName = $"_{info.name}_Info";

        builder.AppendLine();
        AppendLine($"public static Common.PropertyInfo {infoName} = classInfo.GetPropertyInfo(\"{info.name}\");", indent);
        AppendLine($"public {info.typeName} {info.name}", indent);
        AppendLine("{", indent);

        AppendLine($"get {{ return this._{info.name}; }}", indent1);
        AppendLine("set", indent1);
        AppendLine("{", indent1);

        AppendLine($"this._{info.name} = value;", indent2);
        AppendLine($"OnPropertyChanged({infoName});", indent2);

        AppendLine("}", indent1);
        AppendLine("}", indent);
    }

    public static void Build(SourceProductionContext context, ClassInfo infos)
    {
        PropertyBuilder builder = new PropertyBuilder(context, infos);

        SourceText source = builder.GenerateSource();
        string fileName = $"{infos.name}.g.cs";
        context.AddSource(fileName, source);
        //context.AddSource(fileName, source);
    }

    void AppendLine(string line, Indent indent)
    {
        builder.Append(indent.value);
        builder.AppendLine(line);
    }

    SourceText GenerateSource()
    {
        return SourceText.From(builder.ToString(), Encoding.Unicode);
    }
}