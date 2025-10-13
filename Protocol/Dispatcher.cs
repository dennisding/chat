
// 需要source generator生成的代码
// 这里是测试用

using System.Net.Sockets;

namespace Protocol.Dispatcher;

public class ILoginClient: Protocol.ILoginClient
{
    public void LoginResult(bool isOk)
    {
    }

    public void Echo(string msg)
    {
        MemoryStream stream = new MemoryStream();
        // pack stream
    }

    public void EchoBack(string msg)
    {
    }
}