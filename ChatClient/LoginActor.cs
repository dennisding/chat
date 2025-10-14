

using Common;
using Protocol;
using Client;
using System.Runtime.InteropServices.Swift;
using Services;
using System.Net.Sockets;

namespace ChatClient
{
    class ActorSender : ISender
    {
        ActorId aid;
        ActorServices services;

        public ActorSender(ActorId aid, ActorServices services)
        {
            this.aid = aid;
            this.services = services;
        }

        public void Send(MemoryStream stream)
        {
            services.remote!.ActorMessage(aid, stream);
        }

        public void Close()
        {
            services.client!.Close();
        }
    }

    class LoginActor : Actor, ILoginClient // ActorClient<ILoginCore>
    {
        ILoginCore? core;
        public LoginActor()
        {
        }

        public override void Init()
        {
        }

        public override void Finit()
        {
        }

        public override void EnterWorld()
        {
        }

        public override void LeaveWorld()
        {
        }

        public override void BindClient(ActorServices? services)
        {
            base.BindClient(services);

            if (services == null)
            {
                core = null;
                return;
            }

            ISender sender = new ActorSender(aid, services);
            core = new Protocol.Sender.ILoginCore_Sender(sender);
        }

        public override void DispatchMessage(BinaryReader reader)
        {
            //            Protocol.Dispatcher.ILoginClient.Dispatch(this, reader);
            Protocol.Dispatcher.ILoginClient_Dispatcher.Dispatch(this, reader);
        }

        // impl ILoginClient
        public void LoginResult(bool isOk)
        {
            Console.WriteLine($"LoginActor.LoginResult: {isOk}");
        }

        public void Echo(string msg)
        {
            Console.WriteLine($"LoginActor.Echo: {msg}");
            core!.EchoBack(msg);
        }

        public void EchoBack(string msg)
        {
            Console.WriteLine($"LoginActor.EchoBack: {msg}");
        }
    }
}