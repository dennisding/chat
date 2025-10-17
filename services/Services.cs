using System.Net.Sockets;

namespace Services
{
    enum NetState
    {
        None,
        Connected,
        Disconnected,
    }

    public interface IServices
    {
        IConnection NewConnection(TcpClient client);
        void OnConnected(IConnection connection);
        void OnDisconnected(IConnection connection);
    }

    public interface IConnection
    {
        void OnConnected();
        void OnDisconnected();

        void DispatchMessage(BinaryReader data);
    }

    public interface IClientServices
    {
        void OnConnected(TcpClient client);
        void OnDisconnected();

        void DispatchMessage(BinaryReader reader);
    }
}
