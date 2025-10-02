namespace Common
{
    public interface IActorNull
    {
    }

    public interface ILoginClient
    {
        void LoginResult(bool ok);
    }

    public interface ILoginCore
    {
        void Login(string username, string password);
    }

    public interface IChatClient
    {
        void ReceivedMessage(string user, string msg);
    }

    public interface IChatCore
    {
        void SetName(string name);
    }

    public interface IChatShadow
    {
        void ShadowMethod(string msg);
    }
}
