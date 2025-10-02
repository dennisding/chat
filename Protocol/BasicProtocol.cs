
namespace Protocol
{
    interface IBasicClient
    {
        void EchoBack(string msg);
    }

    interface IBasicServer
    {
        void Echo(string msg);
    }
}
