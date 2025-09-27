
namespace services
{
    public interface IDispatcher
    {
        void Dispatch(BinaryReader reader);
    }

    class Server
    {
        public Server(IDispatcher dispatcher) 
        {
        }
    }
}