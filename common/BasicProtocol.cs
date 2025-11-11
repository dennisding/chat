

namespace Common;

public interface ISender
{
    void Send(MemoryStream _stream);
    void Close();
}

public interface IMailSender
{
    void Send(Mailbox mailbox, MemoryStream stream);
}

public interface IDispatcher<T>
{
    void Dispatch(T ins, BinaryReader reader);
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
    void CreateActor(string name, ActorId aid);
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
