
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ProtocolGenerator;

class PackerBuilder
{
    StringBuilder builder = new StringBuilder();
    ClassInfo classInfo;

    PackerBuilder(ClassInfo info)
    {
        this.classInfo = info;
    }

    public static void Build(SourceProductionContext context, ClassInfo info)
    {
        PackerBuilder builder = new PackerBuilder(info);

        var source = builder.GenerateSource();

        string fileName = $"{info.containningNamespace}.{info.packerName}.g.cs";
        context.AddSource(fileName, source);
    }

    void AppendLine(string text, Indent indent)
    {
        builder.Append(indent.value);
        builder.AppendLine(text);
    }

    SourceText GenerateSource()
    {
        Indent indent = new Indent();
        Indent indent1 = indent.Next();

        builder.AppendLine("using Common;");
        builder.AppendLine($"namespace {classInfo.containningNamespace};");
        builder.AppendLine();

        AppendLine($"public class {classInfo.packerName} : {classInfo.name}", indent);
        AppendLine("{", indent);
        AppendLine("ISender sender;", indent1);
        AppendLine("PropertyFlag flag;", indent1);

        AddConstructor(indent1);
        
        foreach (var info in classInfo.methods.Values)
        {
            AddPacker(info, indent1);
        }

        AppendLine("}", indent);

        return SourceText.From(builder.ToString(), Encoding.Unicode);

        //public class IPostOffice_Sender: IPostOffice
        //{
        //    ISender sender;
        //    PropertyFlag flag;
        //    public IPostOffice_Sender(ISender sender, PropertyFlag flag = PropertyFlag.All)
        //    {
        //        this.sender = sender;
        //        this.flag = flag;
        //    }

        //    public void Echo(Mailbox mailbox, string msg)
        //    {
        //        MemoryStreamDataStream _datas = new MemoryStreamDataStream(flag);

        //        IPostOffice_ClassInfo.PackMethodIndex(_datas, IPostOffice_ClassInfo._Echo_info);
        //        IPostOffice_ClassInfo._Pack_Echo(_datas, mailbox, msg);

        //        this.sender.Send(_datas.stream);
        //    }

        //    public void EchoBack(string msg)
        //    {
        //        MemoryStreamDataStream _datas = new MemoryStreamDataStream(flag);

        //        IPostOffice_ClassInfo.PackMethodIndex(_datas, IPostOffice_ClassInfo._EchoBack_info);
        //        IPostOffice_ClassInfo._Pack_EchoBack(_datas, msg);

        //        this.sender.Send(_datas.stream);
        //    }
        //}
        void AddPacker(MethodInfo info, Indent indent)
        {
            Indent indent1 = indent.Next();
            string classInfoName = classInfo.classInfoName;
            string parameter = info.parameterNameList;
            if (parameter != "")
            {
                parameter = ", " + parameter;
            }

            builder.AppendLine();
            AppendLine($"public void {info.name}({info.parameterTypeNameList})", indent);
            AppendLine("{", indent);

            // todo 这里需要对MemoryStreamDataStream 做重构
            builder.AppendLine();
            
            AppendLine("MemoryStreamDataStream _datas = new MemoryStreamDataStream(flag);", indent1);
            AppendLine($"{classInfoName}.PackMethodIndex(_datas, {classInfoName}.{info.infoName});", indent1);
            AppendLine($"{classInfoName}.{info.packerName}(_datas{parameter});", indent1);
            
            builder.AppendLine();
            AppendLine("this.sender.Send(_datas.stream);", indent1);

            AppendLine("}", indent);
        }

        void AddConstructor(Indent indent)
        {
            //    public IPostOffice_Sender(ISender sender, PropertyFlag flag = PropertyFlag.All)
            //    {
            //        this.sender = sender;
            //        this.flag = flag;
            //    }
            Indent indent1 = indent.Next();
            builder.AppendLine();
            AppendLine($"public {classInfo.packerName}(ISender sender, PropertyFlag flag = PropertyFlag.All)", indent);
            AppendLine("{", indent);

            AppendLine("this.sender = sender;", indent1);
            AppendLine("this.flag = flag;", indent1);

            AppendLine("}", indent);
        }
    }
}
