
namespace Services
{
    class Message
    {
        public NetState netState = NetState.None;
        public ConnectionInfo? info = null;
        public byte[]? data = null;

        public static Message Connect(ConnectionInfo info)
        {
            Message msg = new Message();
            msg.netState = NetState.Connected;
            msg.info = info;

            return msg;
        }

        public static Message Disconnect(ConnectionInfo info)
        {
            Message msg = new Message();
            msg.netState = NetState.Disconnected;
            msg.info = info;

            return msg;
        }

        public static Message DataReceived(ConnectionInfo info, byte[] data)
        {
            Message msg = new Message();
            msg.info = info;
            msg.data = data;
            return msg;
        }
    }
}