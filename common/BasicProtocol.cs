

namespace Common
{
    public interface ISender
    {
        void Send(MemoryStream _stream);
        void Close();
    }

    //public interface IDispatcher<T>
    //{
    //    void Dispatch(T obj, BinaryReader reader);
    //}

    public interface IActorNull
    {

    }

    public interface IBasicClient
    {
        void Echo(string msg);
        void EchoBack(string msg);
        void CreateActor(string name, ActorId aid);
        void BindClientTo(ActorId aid);
        void ActorMessage(ActorId aid, MemoryStream msg);
    }

    public interface IBasicServer
    {
        void Echo(string msg);
        void EchoBack(string msg);
        void ActorMessage(ActorId aid, MemoryStream msg);
    }
}
