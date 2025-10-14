
using Common;
using Server;
using Protocol;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Services;

namespace ChatServer
{
    class ClientSender : ISender
    {
        ActorConnection connection;
        ActorId aid;
        public ClientSender(ActorId aid, ActorConnection con)
        {
            this.aid = aid;
            this.connection = con;
        }

        public void Send(MemoryStream data)
        {
            connection.remote.ActorMessage(aid, data);
        }

        public void Close()
        {
            connection.client.Close();
        }
    }

    class LoginCore : Actor, ILoginCore // ActorCore<ILoginClient, IActorNull>
    {
        ILoginClient? client;
        public LoginCore(): base()
        {
        }

        public override void EnterWorld(World _world)
        {
            base.EnterWorld(_world);
        }
        public override void BindClient(ActorConnection con)
        {
            base.BindClient(con);

            // send the client sender
            ClientSender sender = new ClientSender(aid, con);
            client = new Protocol.Sender.ILoginClient(sender);

            BecomePlayer();
        }

        public override void DispatchMessage(MemoryStream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            Protocol.Dispatcher.ILoginCore_Dispatcher.Dispatch(this, reader);
        }

        public override void BecomePlayer()
        {
            base.BecomePlayer();
            client!.Echo("msg from server!!!!");
        }

        // impl ILoginCore

        public void Login(string name, string password)
        {
            Console.WriteLine($"Login: {name}, {password}");
            client!.LoginResult(name == password);
            //            this.client!.LoginResult(name == password);
        }

        public void Echo(string msg)
        {
            Console.WriteLine($"Echo: {msg}");
            client!.EchoBack(msg);
        }

        public void EchoBack(string msg)
        {
            Console.WriteLine($"EchoBack: {msg}");
        }
    }
}