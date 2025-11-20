
using Common;
using Common.Sender;
using System.Runtime.CompilerServices;

namespace PostOffice;

public class PostOfficeMgr
{
    public static IPostOffice sender;
    public static Dictionary<ActorId, PostOffice> offices = new Dictionary<ActorId, PostOffice>();

    static IDispatcher<IPostOffice> dispatcher = Common.Dispatcher.Dispatcher.Create<IPostOffice>();

    static PostOfficeMgr()
    {
        sender = Common.Sender.Sender.Create<IPostOffice>(new NullSender());
    }

    public static void NewSender(ISender _sender)
    {
        sender = Common.Sender.Sender.Create<IPostOffice>(_sender);
    }

    public static void AddOffice(ActorId aid, PostOffice office)
    {
        offices[aid] = office;
    }

    public static void OnReceiveMail(Mailbox mailbox, MemoryStream stream)
    {
        if (offices.TryGetValue(mailbox.office, out PostOffice? office))
        {
            BinaryReader reader = new BinaryReader(stream);
            dispatcher.Dispatch(office, reader);
        }
        else
        {
            Console.WriteLine($"Invalid mailbox: {mailbox}");
        }
    }

    public static void SendMail(Mailbox mailbox, MemoryStream mail)
    {
    }
}
