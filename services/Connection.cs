
using System.Net.Sockets;

namespace Services
{
    class ConnectionInfo
    {
        public int connectId;
        public TcpClient clinet;
        public NetworkStream stream;
        public IConnection? connection;

        public ConnectionInfo(int connectId, 
            TcpClient client, 
            IConnection? connection = null)
        {
            this.connectId = connectId;
            this.clinet = client;
            this.stream = client.GetStream();
            this.connection = connection;
        }
    }
}