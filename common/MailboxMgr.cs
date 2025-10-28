

using Utils;

namespace Common;

public class Mailbox
{
    public Uuid serverId;
    public Uuid actorId;

    public Mailbox(Uuid id)
    {
        this.serverId = id;
        this.actorId = id;
    }

    public bool Equals(Mailbox other)
    {
        return other.serverId == this.serverId;
    }

    public override int GetHashCode()
    {
        return serverId.value.GetHashCode();
    }
}

public class MailboxMgr
{
    Dictionary<Uuid, IMailSender> servers = new Dictionary<Uuid, IMailSender>();

    public MailboxMgr()
    {

    }

    public Mailbox GenMailbox()
    {
        return new Mailbox(Utils.Utils.GenUuid());
    }

    public void RegisterMailSender(Uuid serverId, IMailSender sender)
    {
        servers[serverId] = sender;
    }

    public void UnregisterMailSender(Uuid serverId)
    {
        servers.Remove(serverId);
    }

    public void Mail(Mailbox mailbox, MemoryStream stream)
    {
        if (servers.TryGetValue(mailbox.serverId, out IMailSender? sender))
        {
            sender.Send(mailbox, stream);
        }
        else
        {
            Console.WriteLine($"Invalid Mailbox: {mailbox}");
        }
    }
}