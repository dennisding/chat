using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Swift;
using System.Text;
using System.Threading.Channels;

using Services;

namespace client
{
    class ClientServices : Services.IClientServices, Services.IClientMethod
    {
        IServerMethod? remote = null;

        public ClientServices()
        {
        }

        public void OnConnected(TcpClient client)
        {
            remote = RemoteBuilder.Build<IServerMethod>(client);
            Console.WriteLine("OnConnected");
            // send the msg to client
            string msg = "msg from client!!";
            remote!.Echo(msg);
        }

        public void OnDisconnected() 
        {
            Console.WriteLine("OnDisconnected");
        }

        public void EchoBack(string msg)
        {
            Console.WriteLine($"EchoBack: {msg}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World client!");

//            TestPack();

            ClientServices services = new ClientServices();

            var client = new Services.Client<IClientMethod>(services);

            client.Connect("127.0.0.1", 999);

            while (true)
            {
                client.Poll();

                Thread.Sleep(10);
            }
        }

        static byte[] BuildMethod()
        {
            MemoryStream stream = new MemoryStream();

            // rpcId, msg
            Packer.PackInt(stream, 10);
            Packer.PackString(stream, "msg from test");

            return stream.GetBuffer();
        }

        static void TestPack()
        {
            MemoryStream stream = new MemoryStream();

            Packer.PackInt(stream, 100);
            Packer.PackString(stream, "hello from client!!!!");

            MemoryStream newStream = new MemoryStream(stream.GetBuffer());
            BinaryReader reader = new BinaryReader(newStream);

            int iv = Packer.UnpackInt(reader);
            string sv = Packer.UnpackString(reader);


            Console.WriteLine($"TestPack: iv:{iv}, sv:{sv}");
        }
    }
}
