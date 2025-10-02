namespace Protocol
{
    public interface ILoginClient
    {
        void LoginResult(bool isOk);
    }

    public interface ILoginCore
    {
        void Login(string name, string password);
    }

    public interface IChatClient
    {
        void ShowMessage(string msg);
    }

    public interface IChatCore
    {
        void ShowMessage(string msg);
    }

    public interface IChatShadow
    {
        void ShowMessage(string msg);
    }
}
