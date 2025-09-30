using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Swift;
using System.Security.Cryptography;
using System.Transactions;

using Services;

namespace server
{
    class ChatServices : IServices
    {
        public ChatServices() 
        {
        }

        public IConnection NewConnection(TcpClient client)
        {
            return new ChatConnection();
        }

        public void OnConnected(IConnection connection)
        {
            Console.WriteLine("OnConnected");
        }

        public void OnDisconnected(IConnection connection)
        {
            Console.WriteLine("OnDisconnected");
        }
    }

    class ChatConnection : IConnection, IServerMethod
    {
        IClientMethod? remote;
        public ChatConnection() 
        {
        }

        public void OnConnected(object remote)
        {
            this.remote = (IClientMethod)remote;
            Console.WriteLine("OnConnectionConnected");
        }

        public void OnDisconnected()
        {
            Console.WriteLine("OnConnectionDisconnected");
        }

        public void Echo(string msg)
        {
            Console.WriteLine($"msg from client!{msg}");

            remote!.EchoBack(msg);
        }

        //public void DispatchRpc(byte[] data)
        //{
        //    Console.WriteLine("DispatchRpc");

        //    // send the data back
        //    //NetworkStream stream = client.GetStream();

        //    //byte[] buffer = BitConverter.GetBytes(data.Length);
        //    //stream.Write(buffer);
        //    //stream.Write(data);
        //    //stream.Flush();
        //}
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World server!");

            ChatServices services = new ChatServices();
            var server = new Server<IClientMethod, IServerMethod>(services);

            server.ServeForeverAt(999);

            //Server server = new Server();

            //server.ServeForeverAt(999);
        }
    }
}
