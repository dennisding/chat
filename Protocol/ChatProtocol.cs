
namespace Protocol;

[Common.Protocol]
public interface ILoginClient
{
    void LoginResult(bool isOk);
    void Echo(string msg);
    void EchoBack(string msg);
}

[Common.Protocol]
public interface ILoginServer
{
    void Login(string name, string password);
    void Echo(string msg);
    void EchoBack(string msg);
}

public interface IChatClient
{
    void ShowMessage(string msg);
}

public interface IChatServer
{
    void ShowMessage(string msg);

    void NewRoom(string name);

    void ClientMessage(string msg);
    void ChatMessage(string msg);
}

public interface IChatShadow
{
    void ShowMessage(string msg);
}
