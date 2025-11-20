

using System.Security.AccessControl;
using System.Security.Cryptography;

namespace Common;

public interface ISender
{
    void Send(MemoryStream _stream);
    void Close();
}

public interface IActor
{
}

public interface IDispatcher
{
    void Dispatch(IDataStreamReader reader, object ins);
}

public interface IActorBuilder<PackerImpl, DataImpl>
{
    PackerImpl CreatePacker(ISender sender);
    IDispatcher CreateDispatcher();
    DataImpl CreateProperties();
}

[Protocol]
public interface IActorNull
{

}

public class NullData : Property
{
}

[Protocol]
public interface IBasicClient
{
    void Echo(string msg);
    void EchoBack(string msg);
    void CreateActor(string name, ActorId aid, MemoryStream properties);
    void DelActor(ActorId aid);
    void BindClientTo(ActorId aid);
    void ActorMessage(ActorId aid, MemoryStream msg);
    void ActorPropertyChanged(ActorId aid, int index, MemoryStream msg);
}

[Protocol]
public interface IBasicServer
{
    void Echo(string msg);
    void EchoBack(string msg);
    void ActorMessage(ActorId aid, MemoryStream msg);
}

[Protocol]
public interface IPostOffice
{
    void Echo(Mailbox mailbox, string msg);
    void EchoBack(string msg);
}

//// 以下是测试代码
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

//public class IPostOffice_Dispatcher : IDispatcher
//{
//    public void Dispatch(IDataStreamReader reader, object ins)
//    {
//        int index = IPostOffice_ClassInfo.UnpackMethodIndex(reader);
//        var methodInfo = IPostOffice_ClassInfo.classInfo.GetMethodInfo(index);

//        methodInfo.caller(reader, ins);
//    }
//}

//public class IPostOffice_ClassInfo
//{
//    public static ClassInfo classInfo = GetClassInfo();

//    static ClassInfo GetClassInfo()
//    {
//        ClassInfo info = new ClassInfo("IPostOffice");

//        info.AddMethodInfo(new MethodInfomation(10, "Echo", _Unpack_Echo));
//        info.AddMethodInfo(new MethodInfomation(11, "EchoBack", _Unpack_EchoBack));

//        return info;
//    }

//    public static void PackMethodIndex(IDataStreamWriter datas, MethodInfomation info)
//    {
//        Common.Packer.Pack(datas, info.indexProperty, info.index);
//    }

//    public static int UnpackMethodIndex(IDataStreamReader reader)
//    {
//        return reader.ReadInt(ClassInfo.methodIndexInfo);
//    }

//    public static void PackPropertyIndex(IDataStreamWriter writer, PropertyInfomation info)
//    {
//        Common.Packer.Pack(writer, ClassInfo.methodIndexInfo, info.index);
//    }

//    public static int UnpackPropertyIndex(IDataStreamReader reader)
//    {
//        return reader.ReadInt(ClassInfo.methodIndexInfo);
//    }

//    public static MethodInfomation _Echo_info = classInfo.GetMethodInfo("Echo");
//    public static void _Pack_Echo(IDataStreamWriter _datas, Mailbox mailbox, string msg)
//    {
//        Common.Packer.Pack(_datas, ClassInfo.builtinInfos[0], mailbox);
//        Common.Packer.Pack(_datas, ClassInfo.builtinInfos[1], msg);
//    }

//    public static void _Unpack_Echo(IDataStreamReader _reader, object _ins)
//    {
//        IPostOffice _self = (IPostOffice)_ins;

//        Mailbox mailbox = Common.Packer.UnpackMailbox(_reader, ClassInfo.builtinInfos[0]);
//        string msg = Common.Packer.UnpackString(_reader, ClassInfo.builtinInfos[1]);

//        _self.Echo(mailbox, msg);
//    }

//    public static MethodInfomation _EchoBack_info = classInfo.GetMethodInfo("Echoback");
//    public static void _Pack_EchoBack(IDataStreamWriter _datas, string msg)
//    {
//        Common.Packer.Pack(_datas, ClassInfo.builtinInfos[0], msg);
//    }

//    public static void _Unpack_EchoBack(IDataStreamReader _reader, object _ins)
//    {
//        IPostOffice _self = (IPostOffice)_ins;

//        string msg = Common.Packer.UnpackString(_reader, ClassInfo.builtinInfos[0]);

//        _self.EchoBack(msg);
//    }
//}