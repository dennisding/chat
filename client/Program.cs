using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Swift;
using System.Text;
using System.Threading.Channels;

using services;

namespace client
{

    class ClassBuilder
    {
        
        public void AddMethod()
        {
            AssemblyName name = new AssemblyName("Test");
            var builder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);

            var moduleBuilder = builder.DefineDynamicModule("Test");

            var typeBuilder = moduleBuilder.DefineType("Remote", TypeAttributes.Public);
        }
    }

    class ClientRemote : services.IServerMethod
    {
        TcpClient client;
        NetworkStream stream;

        public ClientRemote(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();
        }

        public void Echo(string msg)
        {
        }
    }

    class ClientServices : services.IConnection
    {
        TcpClient? client = null;
        //        ClientRemote? remote = null;
        services.IServerMethod? remote = null;

        public ClientServices()
        {
        }

        public void OnConnected(TcpClient client)
        {
            Console.WriteLine("OnConnected");
            this.client = client;
//            remote = new ClientRemote(client);

            // send the msg to client
            string msg = "msg from client";
            remote.Echo(msg);
//            SendString(msg);
        }

        //void SendString(string msg)
        //{
        //    ReadOnlySpan<byte> buff = MemoryMarshal.AsBytes(msg.AsSpan());
        //    byte[] lenBuffer = BitConverter.GetBytes(buff.Length);

        //    NetworkStream stream = client!.GetStream();

        //    stream.Write(lenBuffer);
        //    stream.Write(buff);
        //    stream.Flush();

        //    Console.WriteLine($"send data to server: {buff.Length}");
        //}

        public void OnDisconnected() 
        {
            Console.WriteLine("OnDisconnected");
        }

        public void DispatchRpc(byte[] data)
        {
            Console.WriteLine("dispatch rpc!!!");

            string msg = Encoding.Unicode.GetString(data);
            Console.WriteLine($"msg from server, {msg}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            IClientMethod remote = (IClientMethod)RemoteBuilder.Build(typeof(IClientMethod));

            try
            {
                remote.EchoBack("call remote method");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"exception: {ex}");
            }

            //ClientServices services = new ClientServices();
            //services.Client client = new services.Client(services);

            //client.Connect("127.0.0.1", 999);

            //while (true)
            //{
            //    client.Poll();

            //    Thread.Sleep(10);
            //}
        }
    }
}
