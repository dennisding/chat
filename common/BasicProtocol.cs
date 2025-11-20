

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
