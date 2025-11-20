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
        void OnReady() { }
        IConnection NewConnection(TcpClient client);
        void OnConnected(IConnection connection);
        void OnDisconnected(IConnection connection);
    }

    public interface IConnection
    {
        void OnConnected();
        void OnDisconnected();

        void DispatchMessage(byte[] data);
    }

    public interface IClientServices
    {
        void OnConnected(TcpClient client);
        void OnDisconnected();

        void DispatchMessage(byte[] data);
    }

    public interface IBroadcastService
    {
        void OnMessage(BinaryReader reader);
    }
}
