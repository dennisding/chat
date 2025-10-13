
using Common;

namespace Protocol
{
    public interface IBasicClient
    {
        void Echo(string msg);
        void EchoBack(string msg);
        void CreateActor(string name, ActorId aid);
        void BindClientTo(ActorId aid);
    }

    public interface IBasicServer
    {
        void Echo(string msg);
        void EchoBack(string msg);
    }
}
