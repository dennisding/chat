using System.Net.Sockets;

namespace services
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
        void OnConnected(TcpClient client);
        void OnDisconnected();
        void DispatchRpc(byte[] data);
    }
}
