
using Common;

namespace PostOffice;

public class PostOffice : IPostOffice
{
    Mailbox mailbox;

    public PostOffice(Mailbox mailbox)
    {
        this.mailbox = mailbox;
    }

    public void MailTo(Mailbox mailbox, MemoryStream stream)
    {
    }

    public void OnReceiveMail(Mailbox mailbox, MemoryStream stream)
    {
    }

    public void Echo(Mailbox mailbox, string msg)
    {
    }

    public void EchoBack(string msg)
    {
        Console.WriteLine($"PostOffice.EchoBack: {msg}");

        // 构建mail
        Mailbox mailbox = new Mailbox();
        PostOfficeMgr.sender.Echo(mailbox, "1234566");
        MemoryStream mail = new MemoryStream();

        // 发送mail
        PostOfficeMgr.SendMail(mailbox, mail);
    }
}