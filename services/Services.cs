using System.Net.Sockets;

namespace Services
{
    enum NetState
    {
        None,
        Connected,
        Disconnected,
    }

    public interface IServices<RemoteType>
    {
        IConnection NewConnection(TcpClient client, RemoteType remote);
        void OnConnected(IConnection connection);
        void OnDisconnected(IConnection connection);
    }

    public interface IConnection
    {
        void OnConnected();
        void OnDisconnected();
    }

    public interface IClientServices<RemoteType>
    {
        void OnConnected(TcpClient client, RemoteType remote);
        void OnDisconnected();
    }
}
