using services;
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Swift;
using System.Security.Cryptography;
using System.Transactions;

namespace server
{
    class ChatServices : services.IServices
    {
        public ChatServices() 
        {
        }

        public IConnection NewConnection(TcpClient client)
        {
            return new ChatConnection(client);
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

    class ChatConnection : services.IConnection
    {
        TcpClient client;

        public ChatConnection(TcpClient client) 
        {
            this.client = client;
        }

        public void OnConnected(TcpClient client)
        {
            Console.WriteLine("OnConnectionConnected");
        }

        public void OnDisconnected()
        {
            Console.WriteLine("OnConnectionDisconnected");
        }

        public void DispatchRpc(byte[] data)
        {
            Console.WriteLine("DispatchRpc");

            // send the data back
            NetworkStream stream = client.GetStream();

            byte[] buffer = BitConverter.GetBytes(data.Length);
            stream.Write(buffer);
            stream.Write(data);
            stream.Flush();
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            services.Server server = new services.Server(new ChatServices());

            server.ServeForeverAt(999);

            //Server server = new Server();

            //server.ServeForeverAt(999);
        }
    }
}
